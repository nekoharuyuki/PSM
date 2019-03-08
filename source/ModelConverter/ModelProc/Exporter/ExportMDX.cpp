#include "ExportMDX.h"

namespace mdx {

#define SUPPORT_PSMSTYLE 1

#if ( SUPPORT_PSMSTYLE )
static void correct_errors( int *values, const float *errors, int count, int error ) ;
static bool compare_reverse( const float *p1, const float *p2 ) ;
#endif // SUPPORT_PSMSTYLE

static void enum_refs( set<int> &refs, MdxBlock *block ) ;

//----------------------------------------------------------------
//  ExportMDX
//----------------------------------------------------------------

bool ExportMDX::Export( MdxBlock *block, const char *filename )
{
	if ( !m_stream.open( filename ) ) {
		Error( "open failed \"%s\"\n", filename ) ;
		return false ;
	}
	if ( !PreProcess( block ) ) {
		return false ;
	}
	if ( !ExportHeader( block ) ) {
		Error( "write failed \"%s\"\n", filename ) ;
		m_stream.close() ;
		return false ;
	}
	if ( !ExportBlock( block ) ) {
		Error( "write failed \"%s\"\n", filename ) ;
		m_stream.close() ;
		return false ;
	}
	if ( !PostProcess( block ) ) {
		m_stream.close() ;
		return false ;
	}
	m_stream.close() ;
	return true ;
}

bool ExportMDX::ExportHeader( MdxBlock *block )
{
	m_stream.put_int32( MDX_FORMAT_SIGNATURE ) ;
	m_stream.put_int32( MDX_FORMAT_VERSION ) ;
	m_stream.put_int32( m_format_style ) ;
	m_stream.put_int32( m_format_option ) ;
	return true ;
}

bool ExportMDX::ExportBlock( MdxBlock *block )
{
	int type_id = block->GetTypeID() ;
	if ( m_output_filters[ type_id ] ) {
		if ( m_output_anchor == true || m_output_anchors[ type_id ] ) {
			m_stream.open_chunk( type_id, block->GetName() ) ;
			if ( !ExportArgs( block ) ) return false ;
			m_stream.close_chunk() ;
		}
		return true ;
	}
	int pos = m_stream.get_pos() ;
	if ( !ExportSpecific( block ) ) return false ;
	m_output_counts[ type_id ] ++ ;
	m_output_sizes[ type_id ] += m_stream.get_pos() - pos ;
	return true ;
}

bool ExportMDX::ExportSpecific( MdxBlock *block )
{
	#if ( SUPPORT_PSMSTYLE )
	if ( m_format_style == MDX_FORMAT_STYLE_PSM ) {
		switch ( block->GetTypeID() ) {
		    case MDX_MODEL :
			return ExportModelPSM( (MdxModel *)block ) ;
		    case MDX_ARRAYS :
			return ExportArraysPSM( (MdxArrays *)block ) ;
		    case MDX_FCURVE :
			return ExportFCurvePSM( (MdxFCurve *)block ) ;
		    case MDX_FILE_NAME :
			return ExportFileNamePSM( (MdxFileName *)block ) ;
		    case MDX_DEFINE_ENUM :
		    case MDX_DEFINE_BLOCK :
		    case MDX_DEFINE_COMMAND :
			if ( !m_output_define ) return true ;
			break ;
		}
	}
	#endif // SUPPORT_PSMSTYLE

	return ExportGeneric( block ) ;
}

bool ExportMDX::ExportGeneric( MdxBlock *block )
{
	block->UpdateArgs() ;
	block->UpdateData() ;
	block->GetArgsImage( m_args_buf, m_args_size, m_format_style, m_format_option ) ;
	block->GetDataImage( m_data_buf, m_data_size, m_format_style, m_format_option ) ;

	if ( block->IsCommand() ) {
		int short_flag = ( m_args_size + 4 <= 0xffff ) ? MDX_SHORT_CHUNK : 0 ;
		m_stream.open_chunk( block->GetTypeID() | short_flag ) ;
		if ( !ExportArgs( block ) ) return false ;
		m_stream.close_chunk() ;
	} else {
		m_stream.open_chunk( block->GetTypeID(), block->GetName() ) ;
		if ( !ExportArgs( block ) ) return false ;
		if ( !ExportData( block ) ) return false ;
		if ( !ExportChildren( block ) ) return false ;
		m_stream.close_chunk() ;
	}
	return true ;
}

bool ExportMDX::ExportArgs( MdxBlock *block )
{
	m_stream.open_chunk_args() ;
	m_stream.write( (const char *)m_args_buf, m_args_size ) ;
	m_stream.close_chunk_args() ;
	return true ;
}

bool ExportMDX::ExportData( MdxBlock *block )
{
	m_stream.open_chunk_data() ;
	m_stream.write( (const char *)m_data_buf, m_data_size ) ;
	m_stream.close_chunk_data() ;
	return true ;
}

bool ExportMDX::ExportChildren( MdxBlock *block )
{
	for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
		MdxBlock *child = (MdxBlock *)block->GetChild( i ) ;
		if ( !ExportBlock( child ) ) return false ;
	}
	return true ;
}

//----------------------------------------------------------------
//  pre-process / post-process
//----------------------------------------------------------------

bool ExportMDX::PreProcess( MdxBlock *&block )
{
	if ( !CheckOptions( block ) ) return false ;

	int flags = m_output_name ? 0 : MDX_STREAM_NO_NAME ;
	m_stream.set_flags( flags ) ;

	m_args_buf = m_data_buf = 0 ;
	m_args_size = m_data_size = 0 ;
	return true ;
}

bool ExportMDX::PostProcess( MdxBlock *&block )
{
	PrintStat() ;
	CheckLimit() ;
	return true ;
}

bool ExportMDX::CheckOptions( MdxBlock *&block )
{
	int i ;

	m_stat_mode = true ;
	m_debug_mode = false ;
	m_output_name = true ;
	m_output_anchor = false ;
	m_output_define = true ;
	m_output_option = true ;
	m_fixed_voffs = false ;
	m_fixed_toffs = false ;

	m_format_style = 0 ;
	m_format_option = 0 ;
	m_format_keyframe = MDX_VT_FLOAT ;

	m_vertex_type = MDX_VT_FLOAT ;
	m_normal_type = MDX_VT_FLOAT ;
	m_vcolor_type = MDX_VT_FLOAT ;
	m_tcoord_type = MDX_VT_FLOAT ;
	m_weight_type = MDX_VT_FLOAT ;

	m_vertex_offset = 0.0f ;
	m_vertex_scale = 1.0f ;
	m_tcoord_offset = 0.0f ;
	m_tcoord_scale = 1.0f ;

	m_output_filters.clear() ;
	m_output_anchors.clear() ;
	m_output_limits.clear() ;
	m_output_counts.clear() ;
	m_output_sizes.clear() ;

	//  modes

	string var = GetVar( "stat_mode" ) ;
	if ( var != "" ) m_stat_mode = ( var != "off" ) ;
	var = GetVar( "debug_mode" ) ;
	if ( var != "" ) m_debug_mode = ( var != "off" ) ;
	var = GetVar( "output_name" ) ;
	if ( var != "" ) m_output_name = ( var != "off" ) ;
	var = GetVar( "output_define" ) ;
	if ( var != "" ) m_output_define = ( var != "off" ) ;
	var = GetVar( "output_option" ) ;
	if ( var != "" ) m_output_option = ( var != "off" ) ;

	//  filters

	static char *Names[] = { "bone", "part", "material", "texture", "motion" } ;
	static int Types[] = { MDX_BONE, MDX_PART, MDX_MATERIAL, MDX_TEXTURE, MDX_MOTION } ;
	bool filtered = false ;
	bool only = false ;
	for ( i = 0 ; i < 5 ; i ++ ) {
		string name = str_format( "output_%s", Names[ i ] ) ;
		var = GetVar( name.c_str() ) ;
		if ( var == "only" ) only = true ;
	}
	for ( i = 0 ; i < 5 ; i ++ ) {
		int type = Types[ i ] ;

		string name = str_format( "output_%s", Names[ i ] ) ;
		var = GetVar( name.c_str() ) ;
		if ( var != "" ) {
			bool mode = !only ? ( var == "off" ) : ( var != "only" ) ;
			m_output_filters[ type ] = mode ;
			filtered |= mode ;
		}

		string name2 = str_format( "limit_%s", Names[ i ] ) ;
		var = GetVar( name2.c_str() ) ;
		if ( var != "" && str_isdigit( var ) ) {
			int mode = ( var.find( "kb" ) == string::npos ) ? 1 : -1 ;
			m_output_limits[ type ] = str_atoi( var ) * mode ;
		}
	}

	var = GetVar( "output_anchor" ) ;
	if ( var == "on" ) {
		m_output_anchor = true ;
	} else if ( var == "auto" && filtered ) {
		MdxBlocks blocks ;
		for ( i = 0 ; i < 5 ; i ++ ) {
			if ( !m_output_filters[ Types[ i ] ] ) blocks.EnumTree( block, Types[ i ] ) ;
		}
		set<int> refs ;
		for ( i = 0 ; i < blocks.size() ; i ++ ) {
			enum_refs( refs, blocks[ i ] ) ;
		}
		for ( i = 0 ; i < 5 ; i ++ ) {
			if ( !m_output_filters[ Types[ i ] ] ) continue ;
			if ( refs.find( Types[ i ] ) != refs.end() ) m_output_anchors[ Types[ i ] ] = 1 ;
		}
	}

	//  styles

	m_format_style = MDX_FORMAT_STYLE_PSM ;
	/*
	var = GetVar( "format_style" ) ;
	if ( var != "" ) {
		int sel = str_search( "std psm", var ) ;
		if ( sel < 0 ) Warning( "unknown format style \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_format_style = ( sel == 0 ) ? 0 : MDX_FORMAT_STYLE_PSM ;

		#if ( SUPPORT_PSMSTYLE == 0 )
		if ( m_format_style == MDX_FORMAT_STYLE_PSM ) {
			Warning( "unsupported format style \"%s\"\n", var.c_str() ) ;
			m_format_style = 0 ;
		}
		#endif // SUPPORT_PSMSTYLE
	}
	*/

	//  formats

	var = GetVar( "format_keyframe" ) ;
	if ( var != "" ) {
		int sel = str_search( "default float half", var ) ;
		if ( sel < 0 ) Warning( "unknown keyframe type \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_format_keyframe = ( sel <= 1 ) ? MDX_VT_FLOAT : MDX_VT_HALF ;
	}

	static char VNames[] = "default on off float half byte ubyte" ;
	static int VTypes[] = {
		MDX_VT_FLOAT, MDX_VT_FLOAT, MDX_VT_NONE,
		MDX_VT_FLOAT, MDX_VT_HALF, MDX_VT_BYTE, MDX_VT_UBYTE
	} ;

	var = GetVar( "format_position" ) ;
	if ( var != "" ) {
		int sel = str_search( VNames, var ) ;
		if ( sel < 0 ) Warning( "unknown vertex position type \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_vertex_type = VTypes[ sel ] ;
	}
	var = GetVar( "format_normal" ) ;
	if ( var != "" ) {
		int sel = str_search( VNames, var ) ;
		if ( sel < 0 ) Warning( "unknown vertex normal type \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_normal_type = VTypes[ sel ] ;
	}
	var = GetVar( "format_color" ) ;
	if ( var != "" ) {
		int sel = str_search( VNames, var ) ;
		if ( sel < 0 ) Warning( "unknown vertex color type \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_vcolor_type = VTypes[ sel ] ;
	}
	var = GetVar( "format_texcoord" ) ;
	if ( var != "" ) {
		int sel = str_search( VNames, var ) ;
		if ( sel < 0 ) Warning( "unknown vertex texcoord type \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_tcoord_type = VTypes[ sel ] ;
	}
	var = GetVar( "format_weight" ) ;
	if ( var != "" ) {
		int sel = str_search( VNames, var ) ;
		if ( sel < 0 ) Warning( "unknown vertex weight type \"%s\"\n", var.c_str() ) ;
		if ( sel >= 0 ) m_weight_type = VTypes[ sel ] ;
	}

	//  vertex offsets

	var = GetVar( "offset_vertex" ) ;
	if ( var == "auto" ) {
		m_fixed_voffs = false ;
	} else if ( str_isdigit( var ) ) {
		m_fixed_voffs = true ;
		vector<string> vals ;
		str_split( vals, var, ',' ) ;
		if ( vals.size() >= 1 ) {
			m_vertex_scale = str_atof( vals[ 0 ] ) ;
		}
		if ( vals.size() >= 4 ) {
			m_vertex_offset.x = str_atof( vals[ 1 ] ) ;
			m_vertex_offset.y = str_atof( vals[ 2 ] ) ;
			m_vertex_offset.z = str_atof( vals[ 3 ] ) ;
		}
	}
	var = GetVar( "offset_tcoord" ) ;
	if ( var == "auto" ) {
		m_fixed_toffs = false ;
	} else if ( str_isdigit( var ) ) {
		m_fixed_toffs = true ;
		vector<string> vals ;
		str_split( vals, var, ',' ) ;
		if ( vals.size() >= 1 ) {
			m_tcoord_scale = str_atof( vals[ 0 ] ) ;
		}
		if ( vals.size() >= 4 ) {
			m_tcoord_offset.x = str_atof( vals[ 1 ] ) ;
			m_tcoord_offset.y = str_atof( vals[ 2 ] ) ;
			m_tcoord_offset.z = str_atof( vals[ 3 ] ) ;
		}
	}

	return true ;
}

void ExportMDX::PrintStat()
{
	static int Types[] = { MDX_BONE, MDX_PART, MDX_MATERIAL, MDX_TEXTURE, MDX_MOTION } ;
	static char *Names[] = { "bones", "parts", "materials", "textures", "motions" } ;

	if ( !m_stat_mode ) return ;
	int total = m_stream.get_pos() ;
	if ( total == 0 ) total = 1 ;
	for ( int i = 0 ; i < 5 ; i ++ ) {
		int type = Types[ i ] ;
		int count = m_output_counts[ type ] ;
		int size = m_output_sizes[ type ] ;
		int ratio = size * 100 / total ;
		Message( "%3d %-9s : %8d bytes ( %3d %% )\n", count, Names[ i ], size, ratio ) ;
	}
	Message( " filesize     : %8d bytes ( 100 %% )\n", total ) ;
}

void ExportMDX::CheckLimit()
{
	map<int,int>::iterator it ;
	for ( it = m_output_limits.begin() ; it != m_output_limits.end() ; it ++ ) {
		int type = it->first ;
		int limit = it->second ;
		const char *name = m_format->GetSymbolName( MDX_SCOPE_CHUNK_TYPE, type ) ;
		if ( limit >= 0 ) {
			int count = m_output_counts[ type ] ;
			if ( count <= limit ) continue ;
			Warning( "%s limit ( %d / %d )\n", name, count, limit ) ;
		} else {
			int size = ( m_output_sizes[ type ] + 1023 ) / 1024 ;
			if ( size <= -limit ) continue ;
			Warning( "%s limit ( %dKB / %dKB )\n", name, size, -limit ) ;
		}
	}
}

//----------------------------------------------------------------
//  "PSM" specific output
//----------------------------------------------------------------

#if ( SUPPORT_PSMSTYLE )

bool ExportMDX::ExportModelPSM( MdxModel *model )
{
	m_stream.open_chunk( MDX_MODEL, model->GetName() ) ;
	if ( !ExportArgs( model ) ) return false ;
	if ( !ExportData( model ) ) return false ;

	//  vertex offset

	if ( m_vertex_type == MDX_VT_BYTE || m_vertex_type == MDX_VT_UBYTE
	  || m_tcoord_type == MDX_VT_BYTE || m_tcoord_type == MDX_VT_UBYTE ) {
		vec4 pmin = 1000000.0f ;
		vec4 pmax = -1000000.0f ;
		vec4 tmin = 1000000.0f ;
		vec4 tmax = -1000000.0f ;

		MdxBlocks blocks ;
		blocks.EnumTree( model, MDX_ARRAYS ) ;

		for ( int i = 0 ; i < blocks.size() ; i ++ ) {
			MdxArrays *arrays = (MdxArrays *)blocks[ i ] ;
			int n_verts = arrays->GetVertexCount() ;
			int n_pos = arrays->GetVertexPositionCount() ;
			int n_tex = arrays->GetVertexTexCoordCount() ;
			for ( int k = 0 ; k < n_verts ; k ++ ) {
				const MdxVertex &v = arrays->GetVertex( k ) ;
				if ( n_pos > 0 ) {
					vec4 p = v.GetPosition() ;
					if ( pmin.x > p.x ) pmin.x = p.x ;
					if ( pmin.y > p.y ) pmin.y = p.y ;
					if ( pmin.z > p.z ) pmin.z = p.z ;
					if ( pmax.x < p.x ) pmax.x = p.x ;
					if ( pmax.y < p.y ) pmax.y = p.y ;
					if ( pmax.z < p.z ) pmax.z = p.z ;
				}
				if ( n_tex > 0 ) {
					vec4 t = v.GetTexCoord() ;
					if ( tmin.x > t.x ) tmin.x = t.x ;
					if ( tmin.y > t.y ) tmin.y = t.y ;
					if ( tmax.x < t.x ) tmax.x = t.x ;
					if ( tmax.y < t.y ) tmax.y = t.y ;
				}
			}
		}

		if ( m_vertex_type == MDX_VT_BYTE || m_vertex_type == MDX_VT_UBYTE ) {
			float ver_max = 1.0f ;
			#if 0
			vec4 scale ;
			scale.x = ( pmax.x - pmin.x ) / ver_max ;
			scale.y = ( pmax.y - pmin.y ) / ver_max ;
			scale.z = ( pmax.z - pmin.z ) / ver_max ;
			#else
			//  keep normal vector direction
			float d = pmax.x - pmin.x ;
			if ( d < pmax.y - pmin.y ) d = pmax.y - pmin.y ;
			if ( d < pmax.z - pmin.z ) d = pmax.z - pmin.z ;
			d /= ver_max ;
			vec4 scale( d, d, d ) ;
			#endif
			vec4 offset = pmin + scale ;
			if ( !m_fixed_voffs ) {
				m_vertex_offset = offset ;
				m_vertex_scale = scale ;
			} else {
				//  check only
				float s = m_vertex_scale.x ;
				if ( m_vertex_offset.x - s > pmin.x
				  || m_vertex_offset.x + s < pmax.x
				  || m_vertex_offset.y - s > pmin.y
				  || m_vertex_offset.y + s < pmax.y
				  || m_vertex_offset.z - s > pmin.z
				  || m_vertex_offset.z + s < pmax.z  ) {
					Warning( "vertex offset has insufficient range\n" ) ;
					Warning( "lower bound is %f %f %f\n",
						pmin.x, pmin.y, pmin.z ) ;
					Warning( "upper bound is %f %f %f\n",
						pmax.x, pmax.y, pmax.z ) ;
				}
			}
			/*
			//  TODO: export vertex offset
			m_stream.open_short_chunk( MDX_VERTEX_OFFSET ) ;
			m_stream.open_chunk_args() ;
			m_stream.put_int32( MDX_VF_POSITION ) ;
			m_stream.put_vec3( m_vertex_offset ) ;
			m_stream.put_vec3( m_vertex_scale ) ;
			m_stream.close_chunk_args() ;
			m_stream.close_chunk() ;
			*/
		}
		if ( m_tcoord_type == MDX_VT_BYTE || m_tcoord_type == MDX_VT_UBYTE ) {
			if ( !m_fixed_toffs ) {
				float tex_max = 1.0f ;
				m_tcoord_scale.x = ( tmax.x - tmin.x ) / tex_max ;
				m_tcoord_scale.y = ( tmax.y - tmin.y ) / tex_max ;
				m_tcoord_offset = tmin ;
			} else {
				//  check only
				float s = m_tcoord_scale.x * 2.0f ;
				if ( m_tcoord_offset.x > tmin.x
				  || m_tcoord_offset.x + s < tmax.x
				  || m_tcoord_offset.y > tmin.y
				  || m_tcoord_offset.y + s < tmax.y ) {
					Warning( "tcoord offset has insufficient range\n" ) ;
					Warning( "lower bound is %f %f\n", tmin.x, tmin.y ) ;
					Warning( "upper bound is %f %f\n", tmax.x, tmax.y ) ;
				}
			}
			/*
			//  TODO: export vertex offset
			m_stream.open_short_chunk( MDX_VERTEX_OFFSET ) ;
			m_stream.open_chunk_args() ;
			m_stream.put_int32( MDX_VF_TEXCOORD ) ;
			m_stream.put_vec2( m_tcoord_offset ) ;
			m_stream.put_vec2( m_tcoord_scale ) ;
			m_stream.close_chunk_args() ;
			m_stream.close_chunk() ;
			*/
		}
	}

	if ( !ExportChildren( model ) ) return false ;
	m_stream.close_chunk() ;
	return true ;
}

bool ExportMDX::ExportArraysPSM( MdxArrays *arrays )
{
	m_stream.open_chunk( MDX_ARRAYS, arrays->GetName() ) ;

	int format = GetArraysFormatPSM( arrays ) ;
	int stride = GetArraysStridePSM( arrays ) ;
	int n_verts = arrays->GetVertexCount() ;

	m_stream.open_chunk_args() ;
	m_stream.put_int32( format ) ;
	m_stream.put_int32( stride ) ;
	m_stream.put_int32( n_verts ) ;
	m_stream.close_chunk_args() ;
	m_stream.open_chunk_data() ;

	for ( int j = 0 ; j < n_verts ; j ++ ) {
		if ( !ExportVertexPSM( arrays, j ) ) return false ;
	}

	m_stream.align_pos( 4 ) ;
	m_stream.close_chunk_data() ;

	if ( !ExportChildren( arrays ) ) return false ;
	m_stream.close_chunk() ;
	return true ;
}

bool ExportMDX::ExportVertexPSM( MdxArrays *arrays, int index )
{
	const MdxVertex &v = arrays->GetVertex( index ) ;
	int align = 0 ;

	//  TODO: export NORMALn COLORn

	int type = arrays->GetVertexPositionCount() ? m_vertex_type : MDX_VT_NONE ;
	if ( type != MDX_VT_NONE ) {
		const vec4 &p = v.GetPosition() ;
		switch ( type ) {
		    case MDX_VT_FLOAT : {
			m_stream.align_pos( 4 ) ;
			m_stream.put_vec3( p ) ;
			align |= 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			m_stream.align_pos( 2 ) ;
			m_stream.put_int16( float16( p.x ).bits ) ;
			m_stream.put_int16( float16( p.y ).bits ) ;
			m_stream.put_int16( float16( p.z ).bits ) ;
			align |= 1 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			vec4 p2 = ( p - m_vertex_offset ) / m_vertex_scale ;
			m_stream.put_int8( fixed8n( p2.x ).bits ) ;
			m_stream.put_int8( fixed8n( p2.y ).bits ) ;
			m_stream.put_int8( fixed8n( p2.z ).bits ) ;
			break ;
		    }
		}
	}
	type = arrays->GetVertexNormalCount() ? m_normal_type : MDX_VT_NONE ;
	if ( type != MDX_VT_NONE ) {
		const vec4 &n = v.GetNormal() ;
		switch ( type ) {
		    case MDX_VT_FLOAT : {
			m_stream.align_pos( 4 ) ;
			m_stream.put_vec3( n ) ;
			align |= 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			m_stream.align_pos( 2 ) ;
			m_stream.put_int16( float16( n.x ).bits ) ;
			m_stream.put_int16( float16( n.y ).bits ) ;
			m_stream.put_int16( float16( n.z ).bits ) ;
			align |= 1 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			m_stream.put_int8( fixed8n( n.x ).bits ) ;
			m_stream.put_int8( fixed8n( n.y ).bits ) ;
			m_stream.put_int8( fixed8n( n.z ).bits ) ;
			break ;
		    }
		}
	}
	type = arrays->GetVertexColorCount() ? m_vcolor_type : MDX_VT_NONE ;
	if ( type != MDX_VT_NONE ) {
		vec4 c = v.GetColor() ;
		switch ( type ) {
		    case MDX_VT_FLOAT : {
			m_stream.align_pos( 4 ) ;
			m_stream.put_vec4( c ) ;
			align |= 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			m_stream.align_pos( 2 ) ;
			m_stream.put_int16( float16( c.x ).bits ) ;
			m_stream.put_int16( float16( c.y ).bits ) ;
			m_stream.put_int16( float16( c.z ).bits ) ;
			m_stream.put_int16( float16( c.w ).bits ) ;
			align |= 1 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			m_stream.put_int8( ufixed8n( c.x ).bits ) ;
			m_stream.put_int8( ufixed8n( c.y ).bits ) ;
			m_stream.put_int8( ufixed8n( c.z ).bits ) ;
			m_stream.put_int8( ufixed8n( c.w ).bits ) ;
			break ;
		    }
		}
	}
	type = arrays->GetVertexTexCoordCount() ? m_tcoord_type : MDX_VT_NONE ;
	if ( type != MDX_VT_NONE ) {
		const vec4 &t = v.GetTexCoord() ;
		switch ( type ) {
		    case MDX_VT_FLOAT : {
			m_stream.align_pos( 4 ) ;
			m_stream.put_vec2( t ) ;
			align |= 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			m_stream.align_pos( 2 ) ;
			m_stream.put_int16( float16( t.x ).bits ) ;
			m_stream.put_int16( float16( t.y ).bits ) ;
			align |= 1 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			vec4 t2 = ( t - m_tcoord_offset ) / m_tcoord_scale ;
			m_stream.put_int8( ufixed8n( t2.x ).bits ) ;
			m_stream.put_int8( ufixed8n( t2.y ).bits ) ;
			break ;
		    }
		}
	}
	type = arrays->GetVertexWeightCount() ? m_weight_type : MDX_VT_NONE ;
	if ( type != MDX_VT_NONE ) {
		int n_weights = arrays->GetVertexWeightCount() ;
		switch ( type ) {
		    case MDX_VT_FLOAT : {
			m_stream.align_pos( 4 ) ;
			for ( int i = 0 ; i < n_weights ; i ++ ) {
				m_stream.put_float( v.GetWeight( i ) ) ;
			}
			align |= 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			m_stream.align_pos( 2 ) ;
			for ( int i = 0 ; i < n_weights ; i ++ ) {
				m_stream.put_int16( float16( v.GetWeight( i ) ).bits ) ;
			}
			align |= 1 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			int weights[ 8 ] ;
			float errors[ 8 ] ;
			int isum = 0 ;
			float fsum = 0.0f ;
			for ( int i = 0 ; i < n_weights ; i ++ ) {
				float fw = v.GetWeight( i ) ;
				int iw = ufixed8n( fw ).bits ;
				weights[ i ] = iw ;
				errors[ i ] = (float)iw - fw * 255.0f ;
				isum += iw ;
				fsum += fw ;
			}
			int error = isum - (int)( fsum * 255.0f + 0.5f ) ;
			correct_errors( weights, errors, n_weights, error ) ;
			m_stream.put_int8( weights, n_weights ) ;
			break ;
		    }
		}
	}
	int n_indices = arrays->GetVertexBlendIndicesCount() ;
	for ( int i = 0 ; i < n_indices ; i ++ ) {
		m_stream.put_uint8( v.GetBlendIndex( i ) ) ;
	}

	m_stream.align_pos( align + 1 ) ;
	return true ;
}


int ExportMDX::GetArraysFormatPSM( MdxArrays *arrays )
{
	int format = 0 ;
	int count = arrays->GetVertexPositionCount() ;
	if ( count > 0 && m_vertex_type != MDX_VT_NONE ) {
		format |= MDX_VF_POSITIONS( count ) | MDX_VT_POSITIONS( m_vertex_type ) ;
	}
	count = arrays->GetVertexNormalCount() ;
	if ( count > 0 && m_normal_type != MDX_VT_NONE ) {
		format |= MDX_VF_NORMALS( count ) | MDX_VT_NORMALS( m_normal_type ) ;
	}
	count = arrays->GetVertexColorCount() ;
	if ( count > 0 && m_vcolor_type != MDX_VT_NONE ) {
		format |= MDX_VF_COLORS( count ) | MDX_VT_COLORS( m_vcolor_type ) ;
	}
	count = arrays->GetVertexTexCoordCount() ;
	if ( count > 0 && m_tcoord_type != MDX_VT_NONE ) {
		format |= MDX_VF_TEXCOORDS( count ) | MDX_VT_TEXCOORDS( m_tcoord_type ) ;
	}
	count = arrays->GetVertexWeightCount() ;
	if ( count > 0 && m_weight_type != MDX_VT_NONE ) {
		format |= MDX_VF_WEIGHTS( count ) | MDX_VT_WEIGHTS( m_weight_type ) ;
	}
	if ( arrays->GetVertexBlendIndicesMode() ) format |= MDX_VF_INDICES ;
	return format ;
}

int ExportMDX::GetArraysStridePSM( MdxArrays *arrays )
{
	static int Sizes[] = { 4, 2, 1, 1 } ;
	int align = 1 ;
	int stride = 0 ;
	int count = arrays->GetVertexPositionCount() ;
	if ( count > 0 && m_vertex_type != MDX_VT_NONE ) {
		int size = Sizes[ m_vertex_type ] ;
		if ( size > 1 ) stride = ( stride + size - 1 ) / size * size ;
		if ( align < size ) align = size ;
		stride += size * 3 * count ;
	}
	count = arrays->GetVertexNormalCount() ;
	if ( count > 0 && m_normal_type != MDX_VT_NONE ) {
		int size = Sizes[ m_normal_type ] ;
		if ( size > 1 ) stride = ( stride + size - 1 ) / size * size ;
		if ( align < size ) align = size ;
		stride += size * 3 * count ;
	}
	count = arrays->GetVertexColorCount() ;
	if ( count > 0 && m_vcolor_type != MDX_VT_NONE ) {
		int size = Sizes[ m_vcolor_type ] ;
		if ( size > 1 ) stride = ( stride + size - 1 ) / size * size ;
		if ( align < size ) align = size ;
		stride += size * 4 * count ;
	}
	count = arrays->GetVertexTexCoordCount() ;
	if ( count > 0 && m_tcoord_type != MDX_VT_NONE ) {
		int size = Sizes[ m_tcoord_type ] ;
		if ( size > 1 ) stride = ( stride + size - 1 ) / size * size ;
		if ( align < size ) align = size ;
		stride += size * 2 * count ;
	}
	count = arrays->GetVertexWeightCount() ;
	if ( count > 0 && m_weight_type != MDX_VT_NONE ) {
		int size = Sizes[ m_weight_type ] ;
		if ( size > 1 ) stride = ( stride + size - 1 ) / size * size ;
		if ( align < size ) align = size ;
		stride += size * count ;
	}
	stride += arrays->GetVertexBlendIndicesCount() ;

	if ( align > 1 ) stride = ( stride + align - 1 ) / align * align ;
	return stride ;
}

bool ExportMDX::ExportFCurvePSM( MdxFCurve *fcurve )
{
	int i, j ;

	if ( m_format_keyframe != MDX_VT_HALF ) return ExportGeneric( fcurve ) ;

	//  16bit float keyframes

	m_stream.open_chunk( MDX_FCURVE, fcurve->GetName() ) ;

	MdxFCurve fcurve2 = *fcurve ;
	int format = fcurve2.GetFormat() ;
	int extrap = fcurve2.GetExtrap() ;
	int n_dims = fcurve2.GetDimCount() ;
	int n_keys = fcurve2.GetKeyFrameCount() ;

	float frame = 0.0f ;
	int bits = 0 ;
	for ( i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = fcurve2.GetKeyFrame( i ) ;
		float frame2 = key.GetFrame() ;
		int bits2 = float16( frame2 ).bits ;
		if ( i != 0 && frame2 != frame && bits2 == bits ) {
			fcurve2.DeleteKeyFrame( i --, true ) ;
			n_keys -- ;
		}
		frame = frame2 ;
		bits = bits2 ;
	}

	m_stream.open_chunk_args() ;
	m_stream.put_int32( format | MDX_FCURVE_FLOAT16 ) ;
	m_stream.put_int32( extrap ) ;
	m_stream.put_int32( n_dims ) ;
	m_stream.put_int32( n_keys ) ;
	m_stream.close_chunk_args() ;

	int interp = fcurve2.GetInterp() ;
	m_stream.open_chunk_data() ;
	for ( i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = fcurve2.GetKeyFrame( i ) ;
		m_stream.put_int16( float16( key.GetFrame() ).bits ) ;
		for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetValue( j ) ).bits ) ;
		if ( interp == MDX_FCURVE_HERMITE ) {
			for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetInDY( j ) ).bits ) ;
			for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetOutDY( j ) ).bits ) ;
		}
		if ( interp == MDX_FCURVE_CUBIC ) {
			for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetInDY( j ) ).bits ) ;
			for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetOutDY( j ) ).bits ) ;
			for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetInDX( j ) ).bits ) ;
			for ( j = 0 ; j < n_dims ; j ++ ) m_stream.put_int16( float16( key.GetOutDX( j ) ).bits ) ;
		}
	}
	m_stream.align_pos( 4 ) ;
	m_stream.close_chunk_data() ;

	if ( !ExportChildren( fcurve ) ) return false ;
	m_stream.close_chunk() ;
	return true ;
}

bool ExportMDX::ExportFileNamePSM( MdxFileName *cmd )
{
	//  modify filename

	string name = cmd->GetFileName() ;
	string var = GetVar( "format_filename" ) ;
	if ( var != "" ) {
		vector<string> opts ;
		str_split( opts, var, ',' ) ;
		for ( int i = 0 ; i < (int)opts.size() ; i ++ ) {
			const string &opt = opts[ i ] ;
			if ( opt == "tail" ) name = str_tailname( name ) ;
			if ( opt == "root" ) name = str_rootname( name ) ;
			if ( opt == "lower" ) name = str_tolower( name ) ;
			if ( opt == "upper" ) name = str_toupper( name ) ;
		}
	}

	m_stream.open_short_chunk( MDX_FILE_NAME ) ;
	m_stream.open_chunk_args() ;
	m_stream.put_string( name ) ;
	m_stream.close_chunk_args() ;
	m_stream.close_chunk() ;
	return true ;
}

static void correct_errors( int *values, const float *errors, int count, int error )
{
	if ( error == 0 ) return ;
	if ( error == -1 ) {
		const float *min_p = errors + count - 1 ;
		for ( int i = 0 ; i < count ; i ++ ) {
			if ( errors[ i ] < *min_p ) min_p = errors + i ;
		}
		values[ min_p - errors ] += 1 ;
		return ;
	}

	//  sort pointers ( decreasing order )

	vector<const float *> indices ;
	indices.resize( count ) ;
	for ( int i = 0 ; i < count ; i ++ ) {
		indices[ i ] = errors + i ;
	}
	stable_sort( indices.begin(), indices.end(), compare_reverse ) ;

	//  correct errors

	if ( error > 0 ) {
		if ( error > count ) error = count ;
		for ( int i = 0 ; i < error ; i ++ ) {
			values[ indices[ i ] - errors ] -= 1 ;
		}
	} else {
		if ( error < -count ) error = -count ;
		for ( int i = error ; i < 0 ; i ++ ) {
			values[ indices[ count + i ] - errors ] += 1 ;
		}
	}
}

static bool compare_reverse( const float *p1, const float *p2 )
{
	return ( *p1 >= *p2 ) ;
}

#endif // SUPPORT_PSMSTYLE

static void enum_refs( set<int> &refs, MdxBlock *block )
{
	if ( block->HasArgsRef() ) {
		int count = block->GetArgsCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int desc = block->GetArgsDesc( i ) ;
			if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) {
				refs.insert( MDX_WORD_SCOPE( desc ) ) ;
			}
		}
	}
	if ( block->HasDataRef() ) {
		int count = block->GetDataCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int desc = block->GetDataDesc( i ) ;
			if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) {
				refs.insert( MDX_WORD_SCOPE( desc ) ) ;
			}
		}
	}
	int count = block->GetChildCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		enum_refs( refs, (MdxBlock *)block->GetChild( i ) ) ;
	}
}


} // namespace mdx
