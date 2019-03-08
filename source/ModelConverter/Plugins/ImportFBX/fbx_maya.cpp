#include "fbx_loader.h"

#pragma warning( disable:4244 )

//----------------------------------------------------------------
//  config
//----------------------------------------------------------------

#define USE_EXTRA_ATTRIBUTES		0

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static char NodeNames[] = "animCurveTL animCurveTA animCurveTU"
			" place2dTexture script transform"
			" lambert blinn phong phongE anisotropic" ;
enum {
	NODE_ANIM_CURVE_TL, NODE_ANIM_CURVE_TA, NODE_ANIM_CURVE_TU,
	NODE_PLACE_2D_TEXTURE, NODE_SCRIPT, NODE_TRANSFORM,
	NODE_LAMBERT, NODE_BLINN, NODE_PHONG, NODE_PHONG_E, NODE_ANISOTROPIC,
} ;

static char TimeNames[] = "film pal ntsc show palf ntscf 100fps 120fps millisec game sec min hour" ;
static KTime::ETimeMode TimeModes[] = {
	KTime::eCINEMA, KTime::ePAL, KTime::eFRAMES30,
	KTime::eFRAMES48, KTime::eFRAMES50, KTime::eFRAMES60,
	KTime::eFRAMES100, KTime::eFRAMES120, KTime::eFRAMES1000,
	#if ( AFTER_FBXSDK_200901 == 0 )
	KTime::eDEFAULT_MODE, KTime::eDEFAULT_MODE, KTime::eDEFAULT_MODE, KTime::eDEFAULT_MODE,
	#else // AFTER_FBXSDK_200901
	KTime::eCUSTOM, KTime::eCUSTOM, KTime::eCUSTOM, KTime::eCUSTOM,
	#endif // AFTER_FBXSDK_200901
} ;
static float FrameRates[] = {
	24, 25, 30, 48, 50, 60, 100, 120, 1000, 15, 1, 1.0f / 60.0f, 1.0f / 3600.0f,
} ;

static int ExtrapModes[] = {
	MDX_FCURVE_HOLD,	//  0 = constant ?
	MDX_FCURVE_EXTEND,	//  1 = linear
	MDX_FCURVE_HOLD,	//  2 = constant ?
	MDX_FCURVE_CYCLE,	//  3 = cycle
	MDX_FCURVE_REPEAT,	//  4 = cycle with offset
	MDX_FCURVE_SHUTTLE,	//  5 = oscillate
} ;

static float transparency_to_opacity( float t ) ;
static float eccentricity_to_shiny( float e ) ;

//----------------------------------------------------------------
//  fbx_maya
//----------------------------------------------------------------

fbx_maya::fbx_maya( MdxShell *shell )
{
	m_shell = shell ;
	m_notes_transform_mode = fbx_loader::MODE_OFF ;
	m_notes_material_mode = fbx_loader::MODE_OFF ;
	m_notes_userdata_mode = fbx_loader::MODE_OFF ;
	m_notes_transform_prefix = "" ;
	m_notes_material_prefix = "" ;
	m_notes_userdata_prefix = "" ;

	clear() ;
}

fbx_maya::~fbx_maya()
{
	clear() ;
}

//----------------------------------------------------------------
//  clear
//----------------------------------------------------------------

void fbx_maya::clear()
{
	m_place_texs.clear() ;
	m_place_texs2.clear() ;
	m_anim_curves.clear() ;
	m_connect_attrs.clear() ;
	m_transform_notes.clear() ;
	m_material_notes.clear() ;
	m_time_mode = (int)KTime::eDEFAULT_MODE ;
	m_frame_rate = 30.0f ;
	m_frame_start = 0.0f ;
	m_frame_end = -1.0f ;
}

//----------------------------------------------------------------
//  load
//----------------------------------------------------------------

bool fbx_maya::load( const string &filename )
{
	clear() ;

	txt_ifstream stream ;
	stream.open( m_filename = filename ) ;
	if ( !stream.is_open() ) return false ;
	if ( m_shell != 0 ) m_shell->Message( "load \"%s\"\n", m_filename.c_str() ) ;
	stream.set_char_type( ';', MDX_STREAM_CHAR_SPECIAL ) ;	//  blank -> special
	stream.set_char_type( '(', MDX_STREAM_CHAR_BLANK ) ;	//  normal -> blank
	stream.set_char_type( ')', MDX_STREAM_CHAR_BLANK ) ;	//  normal -> blank
	stream.set_char_type( '+', MDX_STREAM_CHAR_BLANK ) ;	//  normal -> blank

	int node_type = -1 ;
	string node_name = "" ;

	vector<string> line ;
	while ( !stream.is_eof() ) {
		stream.get_line( line, true ) ;
		if ( line.empty() ) continue ;
		while ( line.back() != ";" ) {
			if ( stream.is_eof() ) break ;
			vector<string> line2 ;
			stream.get_line( line2, true ) ;
			line.insert( line.end(), line2.begin(), line2.end() ) ;
		}
		if ( line.back() == ";" ) line.pop_back() ;
		const string &cmd = line.front() ;
		//------------------------------------------------
		//  createNode $type -s -n $name -p $name
		//------------------------------------------------
		if ( cmd == "createNode" ) {
			node_type = -1 ;
			node_name = str_trimleft( get_option( line, "-n" ), ":" ) ;
			if ( remove_options( line, "-n -p" ) < 2 ) continue ;
			node_type = str_search( NodeNames, line[ 1 ] ) ;
		//------------------------------------------------
		//  select $name -all -adn -ado -add -vis -hi -r -d -tgl -cl -ne
		//------------------------------------------------
		} else if ( cmd == "select" ) {
			node_type = -1 ;
			node_name = "" ;
			if ( remove_options( line, "" ) < 2 ) continue ;
			node_name = str_trimleft( line[ 1 ], ":" ) ;
			if ( node_name == "lambert1" ) node_type = NODE_LAMBERT ;
		//------------------------------------------------
		//  setAttr $name $val ... -k $mode -l $mode -s $size -type $type
		//------------------------------------------------
		} else if ( cmd == "setAttr" ) {
			int count = remove_options( line, "-k -l -s -type" ) - 2 ;
			if ( count < 1 ) continue ;
			if ( node_type == NODE_PLACE_2D_TEXTURE ) {
				if ( count < 2 ) continue ;
				const string &name = line[ 1 ] ;
				if ( name == ".of" ) {
					rect &pt = m_place_texs[ node_name ] ;
					pt.x = str_atof( line[ 2 ] ) ;
					pt.y = str_atof( line[ 3 ] ) ;
				} else if ( name == ".re" ) {
					rect &pt = m_place_texs[ node_name ] ;
					pt.w = str_atof( line[ 2 ] ) ;
					pt.h = str_atof( line[ 3 ] ) ;
				} else if ( name == ".tf" ) {
					rect &pt = m_place_texs2[ node_name ] ;
					pt.x = str_atof( line[ 2 ] ) ;
					pt.y = str_atof( line[ 3 ] ) ;
				} else if ( name == ".c" ) {
					rect &pt = m_place_texs2[ node_name ] ;
					pt.w = str_atof( line[ 2 ] ) ;
					pt.h = str_atof( line[ 3 ] ) ;
				}
			} else if ( node_type >= NODE_ANIM_CURVE_TL
			         && node_type <= NODE_ANIM_CURVE_TU ) {
				curve &ac = m_anim_curves[ node_name ] ;
				const char *name = line[ 1 ].c_str() ;
				if ( strncmp( name, ".ktv[", 5 ) == 0 ) {
					ac.ktvs.resize( count ) ;
					for ( int i = 0 ; i < count ; i ++ ) {
						ac.ktvs[ i ] = str_atof( line[ i + 2 ] ) ;
					}
				} else if ( strncmp( name, ".kot[", 5 ) == 0 ) {
					ac.kots.resize( count ) ;
					for ( int i = 0 ; i < count ; i ++ ) {
						ac.kots[ i ] = str_atoi( line[ i + 2 ] ) ;
					}
				} else if ( strncmp( name, ".tan", 4 ) == 0 ) {
					ac.tan = str_atoi( line[ 2 ] ) ;
				} else if ( strncmp( name, ".pre", 4 ) == 0 ) {
					ac.pre = str_atoi( line[ 2 ] ) ;
				} else if ( strncmp( name, ".pst", 4 ) == 0 ) {
					ac.pst = str_atoi( line[ 2 ] ) ;
				}
			} else if ( node_type == NODE_SCRIPT ) {
				if ( node_name != "sceneConfigurationScriptNode" ) continue ;
				if ( line[ 1 ] != ".b" ) continue ;
				vector<string> opts ;
				str_split( opts, line[ 2 ] ) ;
				m_frame_start = str_atof( get_option( opts, "-min" ) ) ;
				m_frame_end = str_atof( get_option( opts, "-max" ) ) ;
			} else if ( node_type == NODE_TRANSFORM ) {
				if ( line[ 1 ] != ".nts" ) continue ;
				notes &n = m_transform_notes[ node_name ] ;
				for ( int i = 0 ; i < count ; i ++ ) n.nts += line[ i + 2 ] ;
				n.line_no = stream.get_line_count() ;
			} else if ( node_type >= NODE_LAMBERT
			         && node_type <= NODE_ANISOTROPIC ) {
				if ( line[ 1 ] != ".nts" ) continue ;
				notes &n = m_material_notes[ node_name ] ;
				for ( int i = 0 ; i < count ; i ++ ) n.nts += line[ i + 2 ] ;
				n.line_no = stream.get_line_count() ;
			}
		//------------------------------------------------
		//  connectAttr $src $dst -f -na -l $mode
		//------------------------------------------------
		} else if ( cmd == "connectAttr" ) {
			if ( remove_options( line, "-l" ) < 3 ) continue ;
			string src = str_trimleft( line[ 1 ], ":" ) ;
			string dst = str_trimleft( line[ 2 ], ":" ) ;
			m_connect_attrs[ dst ] = src ;
		//------------------------------------------------
		//  currentUnit -q -f -ua $mode -l $mode -a $mode -t $mode
		//------------------------------------------------
		} else if ( cmd == "currentUnit" ) {
			string time = get_option( line, "-t" ) ;
			int mode = str_search( TimeNames, time ) ;
			if ( mode >= 0 ) {
				m_time_mode = (int)TimeModes[ mode ] ;
				m_frame_rate = FrameRates[ mode ] ;
			} else if ( str_isdigit( time ) ) {
				#if ( AFTER_FBXSDK_200901 )
				m_time_mode = (int)KTime::eCUSTOM ;
				#endif // AFTER_FBXSDK_200901
				m_frame_rate = str_atof( time ) ;
			}
		}
	}
	stream.close() ;
	return true ;
}

//----------------------------------------------------------------
//  save
//----------------------------------------------------------------

bool fbx_maya::save( fbx_loader &loader )
{
	m_model = loader.m_model ;
	if ( m_model == 0 ) return false ;
	m_motion = (MdxMotion *)m_model->FindChild( MDX_MOTION ) ;

	bool res = true ;
	if ( !save_material_animation( loader ) ) res = false ;
	if ( !save_texture_animation( loader ) ) res = false ;
	if ( !save_extrapolation( loader ) ) res = false ;
	if ( !save_transform_notes( loader ) ) res = false ;
	if ( !save_material_notes( loader ) ) res = false ;
	if ( !save_frame_info( loader ) ) res = false ;
	return res ;
}

//----------------------------------------------------------------
//  material animation
//----------------------------------------------------------------

bool fbx_maya::save_material_animation( fbx_loader &loader )
{
	for ( int i = 0 ; i < (int)loader.m_material_infos.size() ; i ++ ) {
		m_material = (MdxMaterial *)m_model->FindChild( MDX_MATERIAL, i ) ;
		if ( m_material == 0 ) continue ;

		//  "lambert" "blinn" ...

		KFbxMaterial *material = loader.m_material_infos[ i ].material ;
		if ( material == 0 ) continue ;
		string name = material->GetName() ;
		string::size_type colons = name.find( "::" ) ;
		if ( colons != string::npos ) name = name.substr( colons + 2 ) ;

		//  "animCurveTU"

		string exts = ".cr .cg .cb .sr .sg .sb .ir .ig .ib .acr .acg .acb .itr .ec" ;
		for ( int j = 0 ; j < 14 ; j ++ ) {
			string ext = str_index( exts, j ) ;
			string name2 = m_connect_attrs[ name + ext ] ;
			if ( name2 == "" ) continue ;
			name2 = str_rootname( name2 ) ;
			curve &ac = m_anim_curves[ name2 ] ;
			if ( ac.ktvs.empty() ) continue ;

			static int Types[] = {
				MDX_DIFFUSE, MDX_SPECULAR, MDX_EMISSION, MDX_AMBIENT, 0 
			} ;
			int type = Types[ j / 3 ] ;
			int offset = j % 3 ;
			float (*modify)( float ) = 0 ;
			if ( j == 12 ) {
				type = MDX_OPACITY ;
				offset = 0 ;
				modify = transparency_to_opacity ;
			}
			if ( j == 13 ) {
				type = MDX_SHININESS ;
				offset = 0 ;
				modify = eccentricity_to_shiny ;
			}

			//  animation

			if ( m_motion == 0 ) {
				m_model->AttachChild( m_motion = new MdxMotion ) ;
				m_motion->AttachChild( new MdxFrameLoop( 0.0f, 48.0f ) ) ;
				m_motion->AttachChild( new MdxFrameRate( 24.0f ) ) ;
			}
			m_motion->AttachChild( m_fcurve = create_fcurve( ac, modify ) ) ;
			MdxAnimate *cmd = new MdxAnimate ;
			m_motion->AttachChild( cmd ) ;
			cmd->SetScope( MDX_MATERIAL ) ;
			cmd->SetBlock( m_material->GetName() ) ;
			cmd->SetCommand( type ) ;
			cmd->SetOffset( offset ) ;
			cmd->SetFCurve( m_fcurve->GetName() ) ;
		}
	}
	return true ;
}

//----------------------------------------------------------------
//  texture animation
//----------------------------------------------------------------

bool fbx_maya::save_texture_animation( fbx_loader &loader )
{
	for ( int i = 0 ; i < (int)loader.m_texture_infos.size() ; i ++ ) {
		KFbxTexture *texture = loader.m_texture_infos[ i ] ;

		//  "file"

		string name = texture->GetName() ;
		string::size_type colons = name.find( "::" ) ;
		if ( colons != string::npos ) name = name.substr( colons + 2 ) ;

		//  "place2dTexture"

		string name2 = m_connect_attrs[ name + ".of" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".ofu" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".ofv" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".re" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".reu" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".rev" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".tf" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".tfu" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".tfv" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".c" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".cu" ] ;
		if ( name2 == "" ) name2 = m_connect_attrs[ name + ".cv" ] ;
		if ( name2 == "" ) continue ;
		name2 = str_rootname( name2 ) ;

		//  "animCurveTU"

		MdxFCurve *fcurves[ 4 ] = { 0, 0, 0, 0 } ;
		int flags = 0 ;
		for ( int j = 0 ; j < 8 ; j ++ ) {
			if ( j == 4 && flags != 0 ) break ;
			string ext = str_index( ".ofu .ofv .reu .rev .tfu .tfv .cu .cv", j ) ;
			string name3 = m_connect_attrs[ name2 + ext ] ;
			if ( name3 == "" ) continue ;
			name3 = str_rootname( name3 ) ;
			curve &ac = m_anim_curves[ name3 ] ;
			if ( ac.ktvs.empty() ) continue ;
			flags |= 1 << j ;

			static int Types[] = {
				MDX_UV_TRANSLATE, MDX_UV_SCALE
			} ;
			int type = Types[ j / 2 % 2 ] ;
			int offset = j % 2 ;

			//  animation

			if ( m_motion == 0 ) {
				m_model->AttachChild( m_motion = new MdxMotion ) ;
				m_motion->AttachChild( new MdxFrameLoop( 0.0f, 48.0f ) ) ;
				m_motion->AttachChild( new MdxFrameRate( 24.0f ) ) ;
			}
			m_motion->AttachChild( m_fcurve = create_fcurve( ac ) ) ;
			MdxAnimate *cmd = new MdxAnimate ;
			m_motion->AttachChild( cmd ) ;
			cmd->SetScope( MDX_TEXTURE ) ;
			cmd->SetBlock( loader.get_texture_name( i ).c_str() ) ;
			cmd->SetCommand( type ) ;
			cmd->SetOffset( offset ) ;
			cmd->SetFCurve( m_fcurve->GetName() ) ;
			fcurves[ j % 4 ] = m_fcurve ;
		}
		if ( flags & 0x0f ) {
			if ( fcurves[ 1 ] != 0 ) {
				m_fcurve = fcurves[ 1 ] ;
				float h = m_place_texs[ name2 ].h ;
				MdxKeyFrame tmp ;
				for ( int j = 0 ; j < m_fcurve->GetKeyFrameCount() ; j ++ ) {
					MdxKeyFrame &key = m_fcurve->GetKeyFrame( j ) ;
					if ( fcurves[ 3 ] != 0 ) {
						fcurves[ 3 ]->Eval( key.GetFrame(), tmp ) ;
						h = tmp.GetValue( 0 ) ;
					}
					key.SetValue( 0, 1.0f - h - key.GetValue( 0 ) ) ;
				}
			}
		} else if ( flags & 0xf0 ) {
			for ( int j = 2 ; j < 4 ; j ++ ) {
				if ( fcurves[ j ] == 0 ) continue ;
				m_fcurve = fcurves[ j ] ;
				for ( int k = 0 ; k < m_fcurve->GetKeyFrameCount() ; k ++ ) {
					MdxKeyFrame &key = m_fcurve->GetKeyFrame( k ) ;
					float f = key.GetValue( 0 ) ;
					key.SetValue( 0, ( f == 0.0f ) ? 0.0f : 1.0f / f ) ;
				}
			}
			float c[ 2 ] ;
			c[ 0 ] = m_place_texs2[ name2 ].w ;
			c[ 1 ] = m_place_texs2[ name2 ].h ;
			if ( c[ 0 ] != 0.0f ) c[ 0 ] = 1.0f / c[ 0 ] ;
			if ( c[ 1 ] != 0.0f ) c[ 1 ] = 1.0f / c[ 1 ] ;
			MdxKeyFrame tmp ;
			for ( int j = 0 ; j < 2 ; j ++ ) {
				if ( fcurves[ j ] == 0 ) continue ;
				m_fcurve = fcurves[ j ] ;
				for ( int k = 0 ; k < m_fcurve->GetKeyFrameCount() ; k ++ ) {
					MdxKeyFrame &key = m_fcurve->GetKeyFrame( k ) ;
					if ( fcurves[ j + 2 ] != 0 ) {
						fcurves[ j + 2 ]->Eval( key.GetFrame(), tmp ) ;
						c[ j ] = tmp.GetValue( 0 ) ;
					}
					float f = key.GetValue( 0 ) ;
					if ( j == 0 ) {
						key.SetValue( 0, - f * c[ 0 ] ) ;
					} else {
						key.SetValue( 0, 1.0f - c[ 1 ] + f * c[ 1 ] ) ;
					}
				}
			}
		}
	}
	return true ;
}

//----------------------------------------------------------------
//  extrapolation
//----------------------------------------------------------------

bool fbx_maya::save_extrapolation( fbx_loader &loader )
{
	if ( m_motion == 0 ) return true ;

	MdxBlocks cmds ;
	cmds.EnumChild( m_motion, MDX_ANIMATE ) ;
	for ( int i = 0 ; i < cmds.size() ; i ++ ) {
		MdxAnimate *cmd = (MdxAnimate *)cmds[ i ] ;

		//  "transform"

		string name = cmd->GetBlock() ;
		const char *exts ;
		switch ( cmd->GetCommand() ) {
		    case MDX_TRANSLATE : exts = ".tx .ty .tz" ; break ;
		    case MDX_ROTATE : exts = ".rx .ry .rz" ; break ;
		    case MDX_SCALE : exts = ".sx .sy .sz" ; break ;
		    default : continue ;
		}

		//  "animCurveTL/TA/TU"

		bool defined = false ;
		int extrap_l = 0, extrap_r = 0 ;
		for ( int j = 0 ; j < 3 ; j ++ ) {
			string ext = str_index( exts, j ) ;
			string name2 = m_connect_attrs[ name + ext ] ;
			if ( name2 == "" ) continue ;
			curve &ac = m_anim_curves[ str_rootname( name2 ) ] ;
			if ( ac.ktvs.empty() ) continue ;
			defined = true ;

			int extrap = get_extrap( ac ) ;
			int l = extrap & MDX_FCURVE_EXTRAP_IN_MASK ;
			int r = extrap & MDX_FCURVE_EXTRAP_OUT_MASK ;
			if ( extrap_l < l ) extrap_l = l ;
			if ( extrap_r < r ) extrap_r = r ;
		}

		//  extrapolation

		if ( defined ) {
			m_fcurve = cmd->GetFCurveRef() ;
			if ( m_fcurve != 0 ) m_fcurve->SetExtrap( extrap_l | extrap_r ) ;
		}
	}
	return true ;
}

//----------------------------------------------------------------
//  transform notes
//----------------------------------------------------------------

bool fbx_maya::save_transform_notes( fbx_loader &loader )
{
	if ( m_notes_transform_mode == fbx_loader::MODE_OFF
	  && m_notes_userdata_mode == fbx_loader::MODE_OFF ) return true ;

	map<string,notes>::iterator it ;
	for ( it = m_transform_notes.begin() ; it != m_transform_notes.end() ; it ++ ) {
		const string &name = it->first ;
		const notes &n = it->second ;
		m_bone = (MdxBone *)m_model->FindChild( MDX_BONE, name.c_str() ) ;
		if ( m_bone != 0 ) parse_notes( m_bone, n ) ;
	}
	return true ;
}

//----------------------------------------------------------------
//  material notes
//----------------------------------------------------------------

bool fbx_maya::save_material_notes( fbx_loader &loader )
{
	if ( m_notes_material_mode == fbx_loader::MODE_OFF
	  && m_notes_userdata_mode == fbx_loader::MODE_OFF ) return true ;

	map<string,notes>::iterator it ;
	for ( it = m_material_notes.begin() ; it != m_material_notes.end() ; it ++ ) {
		const string &name = it->first ;
		const notes &n = it->second ;
		m_material = (MdxMaterial *)m_model->FindChild( MDX_MATERIAL, name.c_str() ) ;
		if ( m_material != 0 ) parse_notes( m_material, n ) ;
	}
	return true ;
}

//----------------------------------------------------------------
//  frame info
//----------------------------------------------------------------

bool fbx_maya::save_frame_info( fbx_loader &loader )
{
	if ( m_motion == 0 ) return true ;

	if ( m_time_mode != (int)KTime::eDEFAULT_MODE ) {
		MdxFrameRate *cmd = (MdxFrameRate *)m_motion->FindChild( MDX_FRAME_RATE ) ;
		if ( cmd == 0 ) m_motion->AttachChild( cmd = new MdxFrameRate ) ;
		cmd->SetFPS( m_frame_rate ) ;
	}
	if ( m_frame_start <= m_frame_end ) {
		MdxFrameLoop *cmd = (MdxFrameLoop *)m_motion->FindChild( MDX_FRAME_LOOP ) ;
		if ( cmd == 0 ) m_motion->AttachChild( cmd = new MdxFrameLoop ) ;
		cmd->SetStart( m_frame_start ) ;
		cmd->SetEnd( m_frame_end ) ;
	}
	return true ;
}

//----------------------------------------------------------------
//  subroutines ( option )
//----------------------------------------------------------------

string fbx_maya::get_option( vector<string> &args, const string &name, int start )
{
	int end = args.size() - 1 ;
	for ( int i = start ; i < end ; i ++ ) {
		if ( args[ i ] == name ) return args[ i + 1 ] ;
	}
	return "" ;
}

int fbx_maya::remove_options( vector<string> &args, const string &value_options )
{
	int n_args = 0 ;
	for ( int i = 0 ; i < (int)args.size() ; i ++ ) {
		const string &word = args[ i ] ;
		if ( word[ 0 ] == '-' && !str_isdigit( word ) ) {
			if ( str_search( value_options, word ) >= 0 ) i ++ ;
			continue ;
		}
		if ( n_args < i ) args[ n_args ] = word ;
		n_args ++ ;
	}
	args.resize( n_args ) ;
	return n_args ;
}

//----------------------------------------------------------------
//  subroutines ( notes )
//----------------------------------------------------------------

#if ( USE_EXTRA_ATTRIBUTES == 0 )

bool fbx_maya::parse_notes( MdxBlock *block, const notes &n )
{
	return true ;
}

#else // USE_EXTRA_ATTRIBUTES

bool fbx_maya::parse_notes( MdxBlock *block, const notes &n )
{
	const char *cur_ptr = n.nts.c_str() ;
	const char *end_ptr = n.nts.c_str() + n.nts.length() + 1 ;
	for ( const char *cp = cur_ptr ; cp < end_ptr ; cp ++ ) {
		if ( cp[ 0 ] != '\0' ) {
			if ( cp[ 0 ] != '\\' ) continue ;
			if ( cp[ 1 ] != 'r' && cp[ 1 ] != 'n' ) continue ;
		}
		string line = str_trim( string( cur_ptr, cp - cur_ptr ) ) ;
		cur_ptr = cp + 2 ;
		if ( line == "" ) continue ;

		if ( m_notes_transform_mode != fbx_loader::MODE_OFF
		  && block->GetTypeID() == MDX_BONE ) {
			if ( parse_transform( block, line ) ) continue ;
		}
		if ( m_notes_material_mode != fbx_loader::MODE_OFF
		  && block->GetTypeID() == MDX_MATERIAL ) {
			if ( parse_material( block, line ) ) continue ;
		}
		if ( m_notes_userdata_mode != fbx_loader::MODE_OFF ) {
			if ( parse_userdata( block, line ) ) continue ;
		}
	}
	return true ;
}

bool fbx_maya::parse_transform( MdxBlock *block, const string &line )
{
	const string &prefix = m_notes_transform_prefix ;
	if ( strnicmp( line.c_str(), prefix.c_str(), prefix.length() ) != 0 ) return false ;

	vector<string> args ;
	str_split( args, sanitize( line.substr( prefix.length() ) ) ) ;
	if ( args.size() < 2 ) return false ;

	for ( int i = 0 ; i < (int)args.size() / 2 ; i ++ ) {
		const string &name = args[ i * 2 + 0 ] ;
		const string &value = args[ i * 2 + 1 ] ;

		//  state

		static const char *StateNames = "Z_SORT" ;
		static const char *StateValues = "OFF ON" ;
		static const int States[] = { MDX_BONE_STATE_Z_SORT } ;
		int state = str_search( StateNames, name ) ;
		if ( state >= 0 ) {
			int mode = str_search( StateValues, str_toupper( value ) ) ;
			if ( mode >= 0 ) {
				block->AttachChild( new MdxBoneState( States[ state ], mode ) ) ;
			} else if ( m_shell != 0 ) {
				m_shell->Message( "unknown %s : \"%s\"\n", name.c_str(), value.c_str() ) ;
			}
			continue ;
		}
		return ( i > 0 ) ;
	}
	return true ;
}

bool fbx_maya::parse_material( MdxBlock *block, const string &line )
{
	const string &prefix = m_notes_material_prefix ;
	if ( strnicmp( line.c_str(), prefix.c_str(), prefix.length() ) != 0 ) return false ;

	vector<string> args ;
	str_split( args, sanitize( line.substr( prefix.length() ) ) ) ;
	if ( args.size() < 2 ) return false ;

	for ( int i = 0 ; i < (int)args.size() / 2 ; i ++ ) {
		const string &name = args[ i * 2 + 0 ] ;
		const string &value = args[ i * 2 + 1 ] ;

		//  blend

		static const char *BlendNames = "OFF MIX ADD SUB MIN MAX ABS" ;
		static const int BlendParams[][ 3 ] = {
			{ MDX_BLEND_ADD, MDX_BLEND_ONE, MDX_BLEND_ZERO },
			{ MDX_BLEND_ADD, MDX_BLEND_SRC_ALPHA, MDX_BLEND_INV_SRC_ALPHA },
			{ MDX_BLEND_ADD, MDX_BLEND_SRC_ALPHA, MDX_BLEND_ONE },
			{ MDX_BLEND_REV, MDX_BLEND_SRC_ALPHA, MDX_BLEND_ONE },
			{ MDX_BLEND_MIN, MDX_BLEND_SRC_ALPHA, MDX_BLEND_ONE },
			{ MDX_BLEND_MAX, MDX_BLEND_SRC_ALPHA, MDX_BLEND_ONE },
			{ MDX_BLEND_DIFF, MDX_BLEND_SRC_ALPHA, MDX_BLEND_ONE }
		} ;
		if ( name == "BLEND" ) {
			int mode = str_search( BlendNames, str_toupper( value ) ) ;
			if ( mode >= 0 ) {
				MdxBlock *layer = (MdxBlock *)block->FindChild( MDX_LAYER ) ;
				if ( layer == 0 ) block->AttachChild( layer = new MdxLayer ) ;
				layer->ClearChild( MDX_BLEND_FUNC ) ;
				const int *param = BlendParams[ mode ] ;
				layer->AttachChild( new MdxBlendFunc( param[0], param[1], param[2] ) ) ;
			} else if ( m_shell != 0 ) {
				m_shell->Message( "unknown %s : \"%s\"\n", name.c_str(), value.c_str() ) ;
			}
			continue ;
		}

		//  state

		static const char *StateNames = "LIGHTING FOG TEXTURE CULL_FACE DEPTH_TEST DEPTH_MASK "
						"ALPHA_TEST ALPHA_MASK FLIP_FACE FLIP_NORMAL" ;
		static const char *StateValues = "OFF ON" ;
		static const int States[] = {
			MDX_STATE_LIGHTING, MDX_STATE_FOG, MDX_STATE_TEXTURE, MDX_STATE_CULL_FACE,
			MDX_STATE_DEPTH_TEST, MDX_STATE_DEPTH_MASK, MDX_STATE_ALPHA_TEST,
			MDX_STATE_ALPHA_MASK, MDX_STATE_FLIP_FACE, MDX_STATE_FLIP_NORMAL
		} ;
		int state = str_search( StateNames, name ) ;
		if ( state >= 0 ) {
			int mode = str_search( StateValues, str_toupper( value ) ) ;
			if ( mode >= 0 ) {
				block->AttachChild( new MdxRenderState( States[ state ], mode ) ) ;
			} else if ( m_shell != 0 ) {
				m_shell->Message( "unknown %s : \"%s\"\n", name.c_str(), value.c_str() ) ;
			}
			continue ;
		}
		return ( i > 0 ) ;
	}
	return true ;
}

bool fbx_maya::parse_userdata( MdxBlock *block, const string &line )
{
	const string &prefix = m_notes_userdata_prefix ;
	if ( strnicmp( line.c_str(), prefix.c_str(), prefix.length() ) != 0 ) return false ;

	vector<string> args ;
	str_split( args, line.substr( prefix.length() ) ) ;
	if ( args.empty() ) return false ;

	MdxBlindData *cmd = new MdxBlindData ;
	for ( int i = 0 ; i < (int)args.size() ; i ++ ) {
		string &word = args[ i ] ;
		bool quoted = false ;
		if ( word[ 0 ] == '"' ) {
			word = str_unquote( word ) ;
			cmd->SetArgsString( i, word.c_str() ) ;
		} else if ( !str_isdigit( word ) ) {
			cmd->SetArgsString( i, word.c_str() ) ;
		} else if ( word.find_first_of( '.' ) != string::npos ) {
			cmd->SetArgsFloat( i, str_atof( word ) ) ;
		} else {
			cmd->SetArgsInt( i, str_atoi( word ) ) ;
		}
	}
	block->AttachChild( cmd ) ;
	return true ;
}

string fbx_maya::sanitize( const string &str )
{
	string tmp ;
	for ( int i = 0 ; i < (int)str.length() ; i ++ ) {
		char c = str[ i ] ;
		if ( 0x80 & c ) break ;
		if ( !isalnum( c ) && c != '_' ) c = ' ' ;
		tmp += c ;
	}
	return tmp ;
}

#endif // USE_EXTRA_ATTRIBUTES

//----------------------------------------------------------------
//  subroutines ( fcurve )
//----------------------------------------------------------------

MdxFCurve *fbx_maya::create_fcurve( const curve &ac, float (*modify)( float ) )
{
	int n_keys = ac.ktvs.size() / 2 ;
	MdxFCurve *fcurve = new MdxFCurve ;
	fcurve->SetFormat( MDX_FCURVE_LINEAR ) ;
	fcurve->SetExtrap( get_extrap( ac ) ) ;
	fcurve->SetDimCount( 1 ) ;
	fcurve->SetKeyFrameCount( n_keys ) ;
	bool is_const = true ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = fcurve->GetKeyFrame( i ) ;
		float frame = ac.ktvs[ i * 2 + 0 ] ;
		float value = ac.ktvs[ i * 2 + 1 ] ;
		if ( modify != 0 ) value = modify( value ) ;
		key.SetFrame( frame ) ;
		key.SetValue( 0, value ) ;
		if ( value != ac.ktvs[ 1 ] ) is_const = false ;
	}
	if ( !is_const ) {
		if ( ac.kots.empty() ) {
			if ( ac.tan != 5 ) return fcurve ;
		} else {
			for ( int i = 0 ; i < (int)ac.kots.size() ; i ++ ) {
				if ( ac.kots[ i ] != 5 ) return fcurve ;
			}
		}
	}
	fcurve->SetInterp( MDX_FCURVE_CONSTANT ) ;
	return fcurve ;
}

int fbx_maya::get_extrap( const curve &ac )
{
	int extrap_l = ExtrapModes[ ac.pre % 6 ] & MDX_FCURVE_EXTRAP_IN_MASK ;
	int extrap_r = ExtrapModes[ ac.pst % 6 ] & MDX_FCURVE_EXTRAP_OUT_MASK ;
	return extrap_l | extrap_r ;
}

float transparency_to_opacity( float t )
{
	return 1.0f - t ;
}

float eccentricity_to_shiny( float e )
{
	return ( e < 0.01f ) ? 500.0f : 5.0f / e ;	// (>_<) ?
}
