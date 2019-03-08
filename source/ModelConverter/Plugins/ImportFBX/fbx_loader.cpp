#include "fbx_loader.h"

#pragma warning( disable:4244 )
#pragma warning( disable:4996 )

#if ( AFTER_FBXSDK_200611 == 0 && _MSC_VER >= 1500 )
static int winmajor = 5, winminor = 1 ;
extern "C" int *_imp___winmajor = &winmajor ;
extern "C" int *_imp___winminor = &winminor ;
#endif // AFTER_FBXSDK_200611

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

#define DEFAULT_PATCH_SUBDIV	(16)

#define RADIANS_PER_DEGREE	(3.1415926536f/180.0f)

#define KNOT_REL_MARGIN		(0.01f)
#define KNOT_ABS_MARGIN		(0.0001f)
enum {
	KNOT_TYPE_CLOSE		= 0x0000,
	KNOT_TYPE_OPEN		= 0x0001,
	KNOT_TYPE_UNIFORM	= 0x0000,
	KNOT_TYPE_NONUNIFORM	= 0x0002,
} ;

#define WEIGHT_MARGIN		(0.0001f)

//----------------------------------------------------------------
//  fbx_loader
//----------------------------------------------------------------

fbx_loader::fbx_loader( MdxShell *shell )
{
	m_shell = shell ;

	m_time_mode = MODE_OFF ;
	m_output_root = MODE_AUTO ;
	m_filter_fcurve = MODE_AUTO ;
	m_filter_transp = MODE_AUTO ;
	m_use_material_name = MODE_AUTO ;
	m_use_texture_name = MODE_AUTO ;
	m_default_vcolor = MODE_AUTO ;
	m_vcolor_value = 0xffffffff ;

	m_check_maya_ascii = MODE_AUTO ;
	m_maya_notes_transform_mode = MODE_OFF ;
	m_maya_notes_material_mode = MODE_OFF ;
	m_maya_notes_userdata_mode = MODE_OFF ;
	m_maya_notes_transform_prefix = "" ;
	m_maya_notes_material_prefix = "" ;
	m_maya_notes_userdata_prefix = "" ;

	m_shiny_scale = 4.0f ;
}

fbx_loader::~fbx_loader()
{
	;
}

//----------------------------------------------------------------
//  load file
//----------------------------------------------------------------

bool fbx_loader::load( MdxBlock *&block, const string &filename )
{
	block = 0 ;

	//  reset

	m_filename = filename ;

	//  check Maya ASCII

	bool maya_loaded = false ;
	fbx_maya maya( m_shell ) ;
	maya.set_notes_transform( m_maya_notes_transform_mode, m_maya_notes_transform_prefix ) ;
	maya.set_notes_material( m_maya_notes_material_mode, m_maya_notes_material_prefix ) ;
	maya.set_notes_userdata( m_maya_notes_userdata_mode, m_maya_notes_userdata_prefix ) ;
	if ( m_check_maya_ascii != MODE_OFF ) {
		maya_loaded = maya.load( str_rootname( m_filename ) + ".ma" ) ;
	}

	//  open scene

	#if ( AFTER_FBXSDK_200901 == 0 )
	KFbxSdkManager *manager = KFbxSdkManager::CreateKFbxSdkManager() ;
	#else // AFTER_FBXSDK_200901
	KFbxSdkManager *manager = KFbxSdkManager::Create() ;
	#endif // AFTER_FBXSDK_200901
	if ( manager == 0 ) {
		m_shell->Error( "unable to create the fbx sdk manager" ) ;
		return false ;
	}

	#if ( AFTER_FBXSDK_200611 == 0 )
	KFbxImporter::EFileFormat format ;
	string ext = str_toupper( str_extension( m_filename ) ) ;
	switch ( str_search( "OBJ 3DS DXF", ext ) ) {
	    case 0 : format = KFbxImporter::eALIAS_OBJ ;	break ;
	    case 1 : format = KFbxImporter::e3D_STUDIO_3DS ;	break ;
	    case 2 : format = KFbxImporter::eAUTOCAD_DXF ;	break ;
	    default :  format = KFbxImporter::eFBX_BINARY ;	break ;
	}
	#elif ( AFTER_FBXSDK_200901 == 0 )
	KFbxIOPluginRegistry *registry = KFbxIOPluginRegistryAccessor::Get() ;
	int format = registry->FindReaderIDByDescription( "FBX binary (*.fbx)" ) ;
	registry->DetectFileFormat( m_filename.c_str(), format ) ;
	#elif ( AFTER_FBXSDK_201102 == 0 )
	KFbxIOPluginRegistry *registry = manager->GetIOPluginRegistry() ;
	int format = registry->FindReaderIDByDescription( "FBX binary (*.fbx)" ) ;
	registry->DetectFileFormat( m_filename.c_str(), format ) ;
	#else // AFTER_FBXSDK_201102
	KFbxIOPluginRegistry *registry = manager->GetIOPluginRegistry() ;
	int format = registry->FindReaderIDByDescription( "FBX binary (*.fbx)" ) ;
	registry->DetectReaderFileFormat( m_filename.c_str(), format ) ;
	#endif // AFTER_FBXSDK_201102

	#if ( AFTER_FBXSDK_200901 == 0 )
	KFbxImporter *importer = manager->CreateKFbxImporter() ;
	importer->SetFileFormat( format ) ;
	if ( !importer->Initialize( m_filename.c_str() ) ) {
		m_shell->Error( "%s\n", importer->GetLastErrorString() ) ;
		manager->DestroyKFbxImporter( importer ) ;
		manager->DestroyKFbxSdkManager() ;
		return false ;
	}
	#elif ( AFTER_FBXSDK_201102 == 0 )
	KFbxImporter *importer = KFbxImporter::Create( manager, "" ) ;
	importer->SetFileFormat( format ) ;
	if ( !importer->Initialize( m_filename.c_str() ) ) {
		m_shell->Error( "%s\n", importer->GetLastErrorString() ) ;
		importer->Destroy() ;
		manager->Destroy() ;
		return false ;
	}
	#else // AFTER_FBXSDK_201102
	KFbxImporter *importer = KFbxImporter::Create( manager, "" ) ;
	if ( !importer->Initialize( m_filename.c_str(), format ) ) {
		m_shell->Error( "%s\n", importer->GetLastErrorString() ) ;
		importer->Destroy() ;
		manager->Destroy() ;
		return false ;
	}
	#endif // AFTER_FBXSDK_201102

	//  load

	#if ( AFTER_FBXSDK_200901 == 0 )
	KFbxScene *scene = manager->CreateKFbxScene() ;
	if ( !importer->Import( *scene ) ) {
		m_shell->Error( "%s\n", importer->GetLastErrorString() ) ;
		manager->DestroyKFbxImporter( importer ) ;
		manager->DestroyKFbxSdkManager() ;
		return false ;
	}
	#else // AFTER_FBXSDK_200901
	KFbxScene *scene = KFbxScene::Create( manager, "" ) ;
	if ( !importer->Import( scene ) ) {
		m_shell->Error( "%s\n", importer->GetLastErrorString() ) ;
		importer->Destroy() ;
		manager->Destroy() ;
		return false ;
	}
	#endif // AFTER_FBXSDK_200901

	m_file = new MdxFile ;
	m_file->AttachChild( m_model = new MdxModel ) ;

	KTime::ETimeMode time_mode = (KTime::ETimeMode)maya.get_time_mode() ;
	if ( time_mode == KTime::eDEFAULT_MODE ) {
		time_mode = KTime::eCINEMA ;
		if ( m_time_mode == MODE_ON ) time_mode = scene->GetGlobalTimeSettings().GetTimeMode() ;
		if ( m_time_mode == 24 ) time_mode = KTime::eCINEMA ;
		if ( m_time_mode == 25 ) time_mode = KTime::ePAL ;
		if ( m_time_mode == 30 ) time_mode = KTime::eFRAMES30 ;
		if ( m_time_mode == 48 ) time_mode = KTime::eFRAMES48 ;
		if ( m_time_mode == 50 ) time_mode = KTime::eFRAMES50 ;
		if ( m_time_mode == 60 ) time_mode = KTime::eFRAMES60 ;
		if ( m_time_mode == 100 ) time_mode = KTime::eFRAMES100 ;
		if ( m_time_mode == 120 ) time_mode = KTime::eFRAMES120 ;
		if ( m_time_mode == 1000 ) time_mode = KTime::eFRAMES1000 ;
	}
	#if ( AFTER_FBXSDK_200901 == 0 )
	KTime::SetGlobalTimeMode( time_mode ) ;
	#else // AFTER_FBXSDK_200901
	if ( time_mode == KTime::eCUSTOM ) KTime::SetGlobalTimeMode( KTime::eCINEMA ) ;	// (>_<) workaround
	KTime::SetGlobalTimeMode( time_mode, maya.get_frame_rate() ) ;
	#endif // AFTER_FBXSDK_200901

	m_motion_step.SetTime( 0, 0, 0, 1, 0 ) ;
	float fps = KTime::GetFrameRate( KTime::eDEFAULT_MODE ) ;

	KArrayTemplate<KString *> take_names ;
	scene->FillTakeNameArray( take_names ) ;
	#if ( AFTER_FBXSDK_201102 == 0 )
	for ( int i = 1 ; i < take_names.GetCount() ; i ++ ) {
	#else // AFTER_FBXSDK_201102
	for ( int i = 0 ; i < take_names.GetCount() ; i ++ ) {
	#endif // AFTER_FBXSDK_201102
		KString *take_name = take_names[ i ] ;
		m_model->AttachChild( m_motion = new MdxMotion( take_name->Buffer() ) ) ;

		KFbxTakeInfo *info = scene->GetTakeInfo( *take_name ) ;
		m_motion_infos.push_back( info ) ;
		if ( info != 0 ) {
			float s = info->mLocalTimeSpan.GetStart().GetFrame( true ) ;
			float e = info->mLocalTimeSpan.GetStop().GetFrame( true ) ;
			m_motion->AttachChild( new MdxFrameLoop( s, e ) ) ;
			m_motion->AttachChild( new MdxFrameRate( fps ) ) ;
		}
	}

	// (>_<) workaround for fbxsdk 2010.x
	#if ( AFTER_FBXSDK_200903 == 1 && AFTER_FBXSDK_201102 == 0 )
	if ( !m_motion_infos.empty() ) scene->SetCurrentTake( m_motion_infos[ 0 ]->mName.Buffer() ) ;
	#endif // AFTER_FBXSDK_200901

	load_geometries( scene->GetRootNode(), 0 ) ;
	load_materials() ;

	//  check Maya ASCII

	if ( maya_loaded ) {
		maya.save( *this ) ;
	}

	//  close scene

	#if ( AFTER_FBXSDK_200901 == 0 )
	manager->DestroyKFbxImporter( importer ) ;
	manager->DestroyKFbxSdkManager() ;
	#else // AFTER_FBXSDK_200901
	importer->Destroy() ;
	manager->Destroy() ;
	#endif // AFTER_FBXSDK_200901

	block = m_file ;
	return true ;
}

//----------------------------------------------------------------
//  load all geometries
//----------------------------------------------------------------

void fbx_loader::load_geometries( KFbxNode *node, MdxBone *parent )
{
	m_node = node ;

	KFbxNodeAttribute *attr = node->GetNodeAttribute() ;
	KFbxNodeAttribute::EAttributeType type = KFbxNodeAttribute::eUNIDENTIFIED ;
	if ( attr != 0 ) type = attr->GetAttributeType() ;

	switch ( type ) {
	    case KFbxNodeAttribute::eUNIDENTIFIED :
	    case KFbxNodeAttribute::eNULL :
	    case KFbxNodeAttribute::eMARKER :
	    case KFbxNodeAttribute::eSKELETON :
		if ( node->GetParent() == 0 && m_output_root == MODE_OFF ) break ;
		load_bone( node, parent ) ;
		parent = m_bone ;
		break ;
	    case KFbxNodeAttribute::eMESH :
	    case KFbxNodeAttribute::eNURB :
	    case KFbxNodeAttribute::ePATCH :
	    #if ( AFTER_FBXSDK_200611 )
	    case KFbxNodeAttribute::eNURBS_SURFACE :
	    #endif // AFTER_FBXSDK_200611
		load_bone( node, parent ) ;
		load_part( node ) ;
		parent = m_bone ;
		break ;
	    case KFbxNodeAttribute::eCAMERA :
	    case KFbxNodeAttribute::eCAMERA_SWITCHER :
	    case KFbxNodeAttribute::eLIGHT :
	    case KFbxNodeAttribute::eOPTICAL_REFERENCE :
	    case KFbxNodeAttribute::eOPTICAL_MARKER :
		if ( node->GetChildCount() != 0 ) {
			load_bone( node, parent ) ;
			parent = m_bone ;
		}
		break ;
	    default :
		break ;
	}

	for ( int i = 0 ; i < node->GetChildCount() ; i ++ ) {
		load_geometries( node->GetChild( i ), parent ) ;
	}
}

//----------------------------------------------------------------
//  load bone
//----------------------------------------------------------------

void fbx_loader::load_bone( KFbxNode *node, MdxBone *parent )
{
	//  hierarchy

	string name = get_node_name( node ) ;
	m_model->AttachChild( m_bone = new MdxBone( name.c_str() ) ) ;

	if ( parent != 0 ) {
		m_bone->AttachChild( new MdxParentBone( parent->GetName() ) ) ;
	}

	//  rotation order

	ERotationOrder order ;
	node->GetRotationOrder( KFbxNode::eSOURCE_SET, order ) ;

	quat (*quat_from_rot)( const vec4 &r ) = quat_from_rotzyx ;
	switch ( order ) {
	    case eEULER_XYZ :	quat_from_rot = quat_from_rotzyx ;	break ;
	    case eEULER_XZY :	quat_from_rot = quat_from_rotyzx ;	break ;
	    case eEULER_YZX :	quat_from_rot = quat_from_rotxzy ;	break ;
	    case eEULER_YXZ :	quat_from_rot = quat_from_rotzxy ;	break ;
	    case eEULER_ZXY :	quat_from_rot = quat_from_rotyxz ;	break ;
	    case eEULER_ZYX :	quat_from_rot = quat_from_rotxyz ;	break ;
	    case eSPHERIC_XYZ :	break ;		// (>_<) ?
	}

	//  transform

	KFbxNode::EPivotState ps ;
	node->GetPivotState( KFbxNode::eSOURCE_SET, ps ) ;
	bool has_pivot = ( ps == KFbxNode::ePIVOT_STATE_ACTIVE ) ;
	#if ( AFTER_FBXSDK_200901 )
	has_pivot = true ;
	#endif // AFTER_FBXSDK_200901
	if ( has_pivot ) {
		KFbxVector4 &pv = node->GetRotationPivot( KFbxNode::eSOURCE_SET ) ;
		vec3 p( pv[ 0 ], pv[ 1 ], pv[ 2 ] ) ;
		if ( p != 0.0f ) {
			m_bone->AttachChild( new MdxPivot( p ) ) ;
		}
	}

	KFbxVector4 &pr = node->GetPreRotation( KFbxNode::eSOURCE_SET ) ;
	vec4 r0 = vec4( pr[ 0 ], pr[ 1 ], pr[ 2 ] ) * RADIANS_PER_DEGREE ;
	quat q0 = quat_from_rotzyx( r0 ) ;

	#if ( AFTER_FBXSDK_201102 == 0 )
	// node->SetCurrentTakeNode( 0 ) ;	// (>_<) ?
	KFbxTakeNode *take_node = node->GetCurrentTakeNode() ;
	#endif // AFTER_FBXSDK_201102

	vec3 t, r, s ;
	#if ( AFTER_FBXSDK_200901 == 0 )
	t.x = take_node->GetTranslationX()->Evaluate( 0 ) ;
	t.y = take_node->GetTranslationY()->Evaluate( 0 ) ;
	t.z = take_node->GetTranslationZ()->Evaluate( 0 ) ;
	r.x = take_node->GetEulerRotationX()->Evaluate( 0 ) ;
	r.y = take_node->GetEulerRotationY()->Evaluate( 0 ) ;
	r.z = take_node->GetEulerRotationZ()->Evaluate( 0 ) ;
	s.x = take_node->GetScaleX()->Evaluate( 0 ) ;
	s.y = take_node->GetScaleY()->Evaluate( 0 ) ;
	s.z = take_node->GetScaleZ()->Evaluate( 0 ) ;
	#elif ( AFTER_FBXSDK_200903 == 0 )
	t.x = eval_fcurve( take_node->GetTranslationX(), 0, node->LclTranslation.Get()[ 0 ] ) ;
	t.y = eval_fcurve( take_node->GetTranslationY(), 0, node->LclTranslation.Get()[ 1 ] ) ;
	t.z = eval_fcurve( take_node->GetTranslationZ(), 0, node->LclTranslation.Get()[ 2 ] ) ;
	r.x = eval_fcurve( take_node->GetEulerRotationX(), 0, node->LclRotation.Get()[ 0 ] ) ;
	r.y = eval_fcurve( take_node->GetEulerRotationY(), 0, node->LclRotation.Get()[ 1 ] ) ;
	r.z = eval_fcurve( take_node->GetEulerRotationZ(), 0, node->LclRotation.Get()[ 2 ] ) ;
	s.x = eval_fcurve( take_node->GetScaleX(), 0, node->LclScaling.Get()[ 0 ] ) ;
	s.y = eval_fcurve( take_node->GetScaleY(), 0, node->LclScaling.Get()[ 1 ] ) ;
	s.z = eval_fcurve( take_node->GetScaleZ(), 0, node->LclScaling.Get()[ 2 ] ) ;
	#else // AFTER_FBXSDK_200903
	t.x = eval_fcurve( node->LclTranslation.GetKFCurve( "X" ), 0, node->LclTranslation.Get()[ 0 ] ) ;
	t.y = eval_fcurve( node->LclTranslation.GetKFCurve( "Y" ), 0, node->LclTranslation.Get()[ 1 ] ) ;
	t.z = eval_fcurve( node->LclTranslation.GetKFCurve( "Z" ), 0, node->LclTranslation.Get()[ 2 ] ) ;
	r.x = eval_fcurve( node->LclRotation.GetKFCurve( "X" ), 0, node->LclRotation.Get()[ 0 ] ) ;
	r.y = eval_fcurve( node->LclRotation.GetKFCurve( "Y" ), 0, node->LclRotation.Get()[ 1 ] ) ;
	r.z = eval_fcurve( node->LclRotation.GetKFCurve( "Z" ), 0, node->LclRotation.Get()[ 2 ] ) ;
	s.x = eval_fcurve( node->LclScaling.GetKFCurve( "X" ), 0, node->LclScaling.Get()[ 0 ] ) ;
	s.y = eval_fcurve( node->LclScaling.GetKFCurve( "Y" ), 0, node->LclScaling.Get()[ 1 ] ) ;
	s.z = eval_fcurve( node->LclScaling.GetKFCurve( "Z" ), 0, node->LclScaling.Get()[ 2 ] ) ;
	#endif // AFTER_FBXSDK_200903
	if ( t != 0.0f ) {
		m_bone->AttachChild( new MdxTranslate( t ) ) ;
	}
	if ( r != 0.0f || r0 != 0.0f ) {
		quat q = quat_from_rot( r * RADIANS_PER_DEGREE ) ;
		m_bone->AttachChild( new MdxRotate( q0 * q ) ) ;
	}
	if ( s != 1.0f ) {
		 m_bone->AttachChild( new MdxScale( s ) ) ;
	}

	//  visibility

	bool vis = node->GetVisibility() ;
	if ( !vis ) {
		m_bone->AttachChild( new MdxVisibility( vis ) ) ;
	}

	//  animation

	for ( int i = 0 ; i < (int)m_motion_infos.size() ; i ++ ) {
		m_motion = (MdxMotion *)m_model->FindChild( MDX_MOTION, i ) ;
		if ( m_motion == 0 ) continue ;

		KFbxTakeInfo *take_info = m_motion_infos[ i ] ;
		if ( take_info == 0 ) continue ;
		KTime start = take_info->mLocalTimeSpan.GetStart() ;
		KTime end = take_info->mLocalTimeSpan.GetStop() ;
		KTime step = m_motion_step ;

		#if ( AFTER_FBXSDK_201102 == 0 )
		if ( !node->SetCurrentTakeNode( i + 1 ) ) continue ;
		KFbxTakeNode *take_node = node->GetCurrentTakeNode() ;
		#endif // AFTER_FBXSDK_201102

		fbx_fcurve fcurve ;
		#if ( AFTER_FBXSDK_200903 == 0 )
		fcurve.load( take_node->GetTranslationX(), t.x ) ;
		fcurve.load( take_node->GetTranslationY(), t.y ) ;
		fcurve.load( take_node->GetTranslationZ(), t.z ) ;
		#else // AFTER_FBXSDK_200903
		fcurve.load( node->LclTranslation.GetKFCurve( "X" ), t.x ) ;
		fcurve.load( node->LclTranslation.GetKFCurve( "Y" ), t.y ) ;
		fcurve.load( node->LclTranslation.GetKFCurve( "Z" ), t.z ) ;
		#endif // AFTER_FBXSDK_200903
		if ( fcurve.is_active( m_filter_fcurve, start, end, step ) ) {
			m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
			fcurve.save( m_fcurve, start, end, step ) ;
			set_animation( m_bone, MDX_TRANSLATE, 0, m_fcurve ) ;
		}
		fcurve.clear() ;
		#if ( AFTER_FBXSDK_200903 == 0 )
		fcurve.load( take_node->GetEulerRotationX(), r.x ) ;
		fcurve.load( take_node->GetEulerRotationY(), r.y ) ;
		fcurve.load( take_node->GetEulerRotationZ(), r.z ) ;
		#else // AFTER_FBXSDK_200903
		fcurve.load( node->LclRotation.GetKFCurve( "X" ), r.x ) ;
		fcurve.load( node->LclRotation.GetKFCurve( "Y" ), r.y ) ;
		fcurve.load( node->LclRotation.GetKFCurve( "Z" ), r.z ) ;
		#endif // AFTER_FBXSDK_200903
		if ( fcurve.is_active( m_filter_fcurve, start, end, step ) ) {
			m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
			fcurve.save( m_fcurve, start, end, step ) ;
			adjust_rotate_fcurve( m_fcurve, q0, quat_from_rot ) ;
			set_animation( m_bone, MDX_ROTATE, 0, m_fcurve ) ;
		}
		fcurve.clear() ;
		#if ( AFTER_FBXSDK_200903 == 0 )
		fcurve.load( take_node->GetScaleX(), s.x ) ;
		fcurve.load( take_node->GetScaleY(), s.y ) ;
		fcurve.load( take_node->GetScaleZ(), s.z ) ;
		#else // AFTER_FBXSDK_200903
		fcurve.load( node->LclScaling.GetKFCurve( "X" ), s.x ) ;
		fcurve.load( node->LclScaling.GetKFCurve( "Y" ), s.y ) ;
		fcurve.load( node->LclScaling.GetKFCurve( "Z" ), s.z ) ;
		#endif // AFTER_FBXSDK_200903
		if ( fcurve.is_active( m_filter_fcurve, start, end, step ) ) {
			m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
			fcurve.save( m_fcurve, start, end, step ) ;
			set_animation( m_bone, MDX_SCALE, 0, m_fcurve ) ;
		}
		fcurve.clear() ;
		#if ( AFTER_FBXSDK_200903 == 0 )
		fcurve.load( take_node->GetVisibility(), (float)vis ) ;
		#else // AFTER_FBXSDK_200903
		fcurve.load( node->Visibility.GetKFCurve(), (float)vis ) ;
		#endif // AFTER_FBXSDK_200903
		if ( fcurve.is_active( m_filter_fcurve, start, end, step ) ) {
			m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
			fcurve.save( m_fcurve, start, end, step ) ;
			set_animation( m_bone, MDX_VISIBILITY, 0, m_fcurve ) ;
		}
	}
}

//----------------------------------------------------------------
//  load part
//----------------------------------------------------------------

void fbx_loader::load_part( KFbxNode *node )
{
	if ( node->GetNodeAttribute() == 0 ) return ;

	string name = get_node_name( node ) ;
	m_model->AttachChild( m_part = new MdxPart( name.c_str() ) ) ;
	m_bone->AttachChild( new MdxDrawPart( m_part->GetName() ) ) ;
	m_part->AttachChild( m_arrays = new MdxArrays ) ;
	m_mesh = 0 ;

	fbx_arrays arrays ;
	arrays.set_default_vcolor( m_vcolor_value ) ;

	switch ( node->GetNodeAttribute()->GetAttributeType() ) {
	    case KFbxNodeAttribute::eMESH : {
		KFbxMesh *mesh = (KFbxMesh *)node->GetNodeAttribute() ;
		arrays.load( mesh ) ;
		load_mesh( mesh, arrays ) ;
		load_shape( node, arrays ) ;
		load_link( node, arrays ) ;
		arrays.save( m_arrays ) ;
		break ;
	    }
	    case KFbxNodeAttribute::eNURB : {
		KFbxNurb *nurb = (KFbxNurb *)node->GetNodeAttribute() ;
		arrays.load( nurb ) ;
		load_nurb( nurb, arrays ) ;
		load_shape( node, arrays ) ;
		load_link( node, arrays ) ;
		arrays.save( m_arrays ) ;
		break ;
	    }
	    case KFbxNodeAttribute::ePATCH : {
		KFbxPatch *patch = (KFbxPatch *)node->GetNodeAttribute() ;
		arrays.load( patch ) ;
		load_patch( patch, arrays ) ;
		load_shape( node, arrays ) ;
		load_link( node, arrays ) ;
		arrays.save( m_arrays ) ;
		break ;
	    }
	    #if ( AFTER_FBXSDK_200611 )
	    case KFbxNodeAttribute::eNURBS_SURFACE : {
		KFbxNurbsSurface *nurb = (KFbxNurbsSurface *)node->GetNodeAttribute() ;
		arrays.load( nurb ) ;
		load_nurb( nurb, arrays ) ;
		load_shape( node, arrays ) ;
		load_link( node, arrays ) ;
		arrays.save( m_arrays ) ;
		break ;
	    }
	    #endif // AFTER_FBXSDK_200611
	    default :
		break ;
	}
}

//----------------------------------------------------------------
//  load mesh
//----------------------------------------------------------------

void fbx_loader::load_mesh( KFbxMesh *mesh, fbx_arrays &arrays )
{
	KFbxLayer *layer = mesh->GetLayer( 0 ) ;
	KFbxLayerElementMaterial *materials = ( layer == 0 ) ? 0 : layer->GetMaterials() ;
	#if ( AFTER_FBXSDK_200611 == 0 )
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetTextures() ;
	#elif ( AFTER_FBXSDK_201102 == 0 )
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetDiffuseTextures() ;
	#else // AFTER_FBXSDK_201102
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetTextures( KFbxLayerElement::eDIFFUSE_TEXTURES ) ;
	#endif // AFTER_FBXSDK_201102
	KFbxLayerElementNormal *normals = ( layer == 0 ) ? 0 : layer->GetNormals() ;
	KFbxLayerElementVertexColor *vcolors = ( layer == 0 ) ? 0 : layer->GetVertexColors() ;
	KFbxLayerElementUV *tcoords = ( layer == 0 ) ? 0 : layer->GetUVs() ;

	bool has_vcolor = ( vcolors != 0 ) ;
	if ( m_default_vcolor == MODE_OFF ) has_vcolor = false ;

	int mat_idx = -1 ;
	int vert_idx = 0 ;
	int n_polys = mesh->GetPolygonCount() ;
	for ( int poly_idx = 0 ; poly_idx < n_polys ; poly_idx ++ ) {
		int mat_idx2 = get_material_index( materials, textures, poly_idx, has_vcolor ) ;
		if ( mat_idx != mat_idx2 ) {
			if ( has_vcolor ) arrays.set_default_vcolor( m_vcolor_value ) ;
			mat_idx = mat_idx2 ;
			m_mesh = 0 ;
		}
		if ( m_mesh == 0 ) {
			string mat_name = get_material_name( mat_idx ) ;
			m_part->AttachChild( m_mesh = new MdxMesh ) ;
			m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;
			m_mesh->AttachChild( new MdxSetMaterial( mat_name.c_str() ) ) ;
		}
		int n_verts = mesh->GetPolygonSize( poly_idx ) ;
		MdxDrawArrays *cmd = new MdxDrawArrays ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetMode( MDX_PRIM_TRIANGLE_FAN ) ;
		cmd->SetVertexCount( n_verts ) ;
		cmd->SetPrimCount( 1 ) ;
		for ( int i = 0 ; i < n_verts ; i ++ ) {
			int ctrl_idx = mesh->GetPolygonVertex( poly_idx, i ) ;
			int nid = get_normal_index( normals, ctrl_idx, vert_idx, poly_idx ) ;
			int cid = get_vcolor_index( vcolors, ctrl_idx, vert_idx, poly_idx ) ;
			int tid = get_tcoord_index( tcoords, ctrl_idx, vert_idx, poly_idx ) ;
			int idx = arrays.set_index( ctrl_idx, nid, cid, tid ) ;
			cmd->SetIndex( i, idx ) ;
			vert_idx ++ ;
		}
	}
}

//----------------------------------------------------------------
//  load nurb
//----------------------------------------------------------------

void fbx_loader::load_nurb( KFbxNurb *nurb, fbx_arrays &arrays )
{
	KFbxLayer *layer = nurb->GetLayer( 0 ) ;
	KFbxLayerElementMaterial *materials = ( layer == 0 ) ? 0 : layer->GetMaterials() ;
	#if ( AFTER_FBXSDK_200611 == 0 )
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetTextures() ;
	#elif ( AFTER_FBXSDK_201102 == 0 )
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetDiffuseTextures() ;
	#else // AFTER_FBXSDK_201102
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetTextures( KFbxLayerElement::eDIFFUSE_TEXTURES ) ;
	#endif // AFTER_FBXSDK_201102
	int mat_idx = get_material_index( materials, textures ) ;
	string mat_name = get_material_name( mat_idx ) ;
	m_part->AttachChild( m_mesh = new MdxMesh ) ;
	m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;
	m_mesh->AttachChild( new MdxSetMaterial( mat_name.c_str() ) ) ;

	//  subdivision  (u,v) <- (v,u)

	int u_step = nurb->GetVStep() ;
	int v_step = nurb->GetUStep() ;
	if ( u_step != DEFAULT_PATCH_SUBDIV || v_step != DEFAULT_PATCH_SUBDIV ) {
		m_mesh->AttachChild( new MdxSubdivision( u_step, v_step ) ) ;
	}

	//  knot vector  (u,v) <- (v,u)

	int u_degree = nurb->GetVOrder() - 1 ;
	int v_degree = nurb->GetUOrder() - 1 ;
	kDouble *knots = nurb->GetVKnotVector() ;
	int n_knots = nurb->GetVKnotCount() ;
	int knot_type = get_knot_type( knots, n_knots, u_degree ) ;
	if ( knot_type & KNOT_TYPE_NONUNIFORM ) {
		MdxKnotVectorU *cmd = new MdxKnotVectorU ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetKnotCount( n_knots ) ;
		for ( int i = 0 ; i < n_knots ; i ++ ) {
			cmd->SetKnot( i, knots[ i ] ) ;
		}
	}
	knots = nurb->GetUKnotVector() ;
	n_knots = nurb->GetUKnotCount() ;
	knot_type = get_knot_type( knots, n_knots, v_degree ) ;
	if ( knot_type & KNOT_TYPE_NONUNIFORM ) {
		MdxKnotVectorV *cmd = new MdxKnotVectorV ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetKnotCount( n_knots ) ;
		for ( int i = 0 ; i < n_knots ; i ++ ) {
			cmd->SetKnot( i, knots[ i ] ) ;
		}
	}

	//  control points  (i,j) <- (-j,i)

	int mode = 0 ;
	if ( nurb->GetNurbVType() != KFbxNurb::eOPEN ) mode |= MDX_B_SPLINE_CLOSED_U ;
	if ( nurb->GetNurbUType() != KFbxNurb::eOPEN ) mode |= MDX_B_SPLINE_CLOSED_V ;
	bool u_periodic = ( nurb->GetNurbVType() == KFbxNurb::ePERIODIC ) ;
	bool v_periodic = ( nurb->GetNurbUType() == KFbxNurb::ePERIODIC ) ;

	int width = nurb->GetVCount() ;
	int height = nurb->GetUCount() ;
	int width2 = !u_periodic ? width : width + u_degree ;
	int height2 = !v_periodic ? height : height + v_degree ;

	MdxDrawBSpline *cmd = new MdxDrawBSpline ;
	m_mesh->AttachChild( cmd ) ;
	cmd->SetMode( mode ) ;
	cmd->SetWidth( width2 ) ;
	cmd->SetHeight( height2 ) ;
	for ( int i = 0 ; i < height2 ; i ++ ) {
		int i2 = !v_periodic ? height - 1 - i : ( height + 2 - i ) % height ;
		for ( int j = 0 ; j < width2 ; j ++ ) {
			int j2 = !u_periodic ? j : j % width ;
			int ctrl_idx = height * j2 + i2 ;
			int idx = arrays.set_index( ctrl_idx, -1, -1, -1, false ) ;
			cmd->SetIndex( width2 * i + j, idx ) ;
		}
	}
}

//----------------------------------------------------------------
//  load patch
//----------------------------------------------------------------

void fbx_loader::load_patch( KFbxPatch *patch, fbx_arrays &arrays )
{
	//  (>_<) ?
}

//----------------------------------------------------------------
//  load shape
//----------------------------------------------------------------

void fbx_loader::load_shape( KFbxNode *node, fbx_arrays &arrays )
{
	KFbxGeometry *geom = (KFbxGeometry *)node->GetNodeAttribute() ;
	if ( geom == 0 ) return ;

	int n_shapes = geom->GetShapeCount() ;
	if ( n_shapes == 0 ) return ;

	for ( int i = 0 ; i < n_shapes ; i ++ ) {
		arrays.load( geom->GetShape( i ) ) ;
	}

	//  animation

	for ( int i = 0 ; i < (int)m_motion_infos.size() ; i ++ ) {
		m_motion = (MdxMotion *)m_model->FindChild( MDX_MOTION, i ) ;
		if ( m_motion == 0 ) continue ;

		KFbxTakeInfo *take_info = m_motion_infos[ i ] ;
		if ( take_info == 0 ) continue ;
		KTime start = take_info->mLocalTimeSpan.GetStart() ;
		KTime end = take_info->mLocalTimeSpan.GetStop() ;
		KTime step = m_motion_step ;

		#if ( AFTER_FBXSDK_201102 == 0 )
		if ( !node->SetCurrentTakeNode( i + 1 ) ) continue ;
		KFbxTakeNode *take_node = node->GetCurrentTakeNode() ;
		#endif // AFTER_FBXSDK_201102

		fbx_fcurve fcurve ;
		fcurve.load( 0, 1.0f ) ;
		for ( int j = 0 ; j < n_shapes ; j ++ ) {
			#if ( AFTER_FBXSDK_200901 == 0 )
			fcurve.load( geom->GetShapeChannel( take_node, j ), 0.0f ) ;
			#elif ( AFTER_FBXSDK_201102 == 0 )
			fcurve.load( geom->GetShapeChannel( j, false, take_node->GetName() ), 0.0f ) ;
			#else // AFTER_FBXSDK_201102
			fcurve.load( geom->GetShapeChannel( j, false, "" ), 0.0f ) ;
			#endif // AFTER_FBXSDK_201102
		}
		if ( fcurve.is_active( m_filter_fcurve, start, end, step ) ) {
			m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
			fcurve.save( m_fcurve, start, end, step ) ;
			adjust_morph_fcurve( m_fcurve ) ;
			set_animation( m_bone, MDX_MORPH_WEIGHTS, 0, m_fcurve ) ;
		}
	}
}

//----------------------------------------------------------------
//  load link
//----------------------------------------------------------------

void fbx_loader::load_link( KFbxNode *node, fbx_arrays &arrays )
{
	KFbxGeometry *geom = (KFbxGeometry *)node->GetNodeAttribute() ;
	if ( geom == 0 ) return ;
	int n_points = geom->GetControlPointsCount() ;
	if ( n_points == 0 ) return ;

	//  check links

	vector<KFbxLink *> links ;
	#if ( AFTER_FBXSDK_200901 == 0 )
	int n_links = geom->GetLinkCount() ;
	for ( int i = 0 ; i < n_links ; i ++ ) {
		KFbxLink *link = geom->GetLink( i ) ;
		if ( link->GetLink() != 0 ) links.push_back( link ) ;
	}
	#else // AFTER_FBXSDK_200901
	int n_skins = geom->GetDeformerCount( KFbxDeformer::eSKIN ) ;
	for ( int i = 0 ; i < n_skins ; i ++ ) {
		KFbxSkin *skin = (KFbxSkin *)( geom->GetDeformer( i, KFbxDeformer::eSKIN ) ) ;
		int n_links = skin->GetClusterCount() ;
		for ( int j = 0 ; j < n_links ; j ++ ) {
			KFbxLink *link = skin->GetCluster( j ) ;
			if ( link->GetLink() != 0 ) links.push_back( link ) ;
		}
	}
	int n_links = links.size() ;
	#endif // AFTER_FBXSDK_200901
	if ( n_links == 0 ) return ;

	//  check weights

	vector<float> weights ;
	weights.resize( n_points * n_links ) ;
	for ( int i = 0 ; i < n_links ; i ++ ) {
		KFbxLink *link = links[ i ] ;
		int num = link->GetControlPointIndicesCount() ;
		kInt *ip = link->GetControlPointIndices() ;
		kDouble *wp = link->GetControlPointWeights() ;
		for ( int j = 0 ; j < num ; j ++ ) {
			weights[ n_points * i + ip[ j ] ] = (float)wp[ j ] ;
		}
	}

	//  additive mode

	if ( links[ 0 ]->GetLinkMode() == KFbxLink::eADDITIVE ) {
		vector<float> weights2 = weights ;
		for ( int i = 0 ; i < n_links ; i ++ ) {
			KFbxNode *node = links[ i ]->GetAssociateModel() ;
			if ( node == 0 ) continue ;
			for ( int j = 0 ; j < n_links ; j ++ ) {
				KFbxNode *node2 = links[ j ]->GetLink() ;
				if ( strcmp( node->GetName(), node2->GetName() ) != 0 ) continue ;
				for ( int k = 0 ; k < n_points ; k ++ ) {
					weights[ n_points * j + k ] -= weights2[ n_points * i + k ] ;
				}
				break ;
			}
		}
		for ( int i = 0 ; i < n_links ; i ++ ) {
			KFbxNode *node = links[ i ]->GetLink() ;
			for ( int j = i + 1 ; j < n_links ; j ++ ) {
				KFbxNode *node2 = links[ j ]->GetLink() ;
				if ( strcmp( node->GetName(), node2->GetName() ) != 0 ) continue ;
				for ( int k = 0 ; k < n_points ; k ++ ) {
					weights[ n_points * i + k ] += weights[ n_points * j + k ] ;
					weights[ n_points * j + k ] = 0.0f ;
				}
			}
		}
	}

	//  check activity

	int n_actives = 0 ;
	for ( int i = 0 ; i < n_links ; i ++ ) {
		bool active = false ;
		for ( int k = 0 ; k < n_points ; k ++ ) {
			active |= !equals( weights[ n_points * i + k ], 0.0f, WEIGHT_MARGIN ) ;
		}
		if ( !active ) continue ;
		if ( i > n_actives ) {
			links[ n_actives ] = links[ i ] ;
			for ( int k = 0 ; k < n_points ; k ++ ) {
				weights[ n_points * n_actives + k ] = weights[ n_points * i + k ] ;
			}
		}
		n_actives ++ ;
	}
	if ( n_actives == 0 ) return ;
	n_links = n_actives ;
	links.resize( n_links ) ;

	//  normalize weights

	switch ( links[ 0 ]->GetLinkMode() ) {
	    case KFbxLink::eNORMALIZE : {
		for ( int i = 0 ; i < n_points ; i ++ ) {
			float sum = 0.0f ;
			for ( int j = 0 ; j < n_links ; j ++ ) {
				sum += weights[ n_points * j + i ] ;
			}
			if ( sum == 0.0f ) {
				weights[ i ] = 1.0f ;
				sum = 1.0f ;
			}
			for ( int j = 0 ; j < n_links ; j ++ ) {
				weights[ n_points * j + i ] /= sum ;
			}
		}
		break ;
	    }
	    case KFbxLink::eTOTAL1 :
	    case KFbxLink::eADDITIVE : {
		weights.resize( n_points * ( n_links + 1 ) ) ;
		bool normalized = true ;
		for ( int i = 0 ; i < n_points ; i ++ ) {
			float sum = 0.0f ;
			for ( int j = 0 ; j < n_links ; j ++ ) {
				sum += weights[ n_points * j + i ] ;
			}
			float w = 1.0f - sum ;
			weights[ n_points * n_links + i ] = w ;
			normalized &= equals( w, 0.0f, WEIGHT_MARGIN ) ;
		}
		if ( !normalized ) {
			links.push_back( 0 ) ;
			n_links += 1 ;
		}
		break ;
	    }
	}

	//  set commands

	for ( int i = 0 ; i < n_links ; i ++ ) {
		KFbxLink *link = links[ i ] ;
		string bone = !link ? m_bone->GetName() : get_node_name( link->GetLink() ) ;
		mat4 offset = !link ? mat4_one : get_offset_matrix( link ) ;
		m_bone->AttachChild( new MdxBlendBone( bone.c_str(), offset ) ) ;
	}

	//  set weights

	for ( int i = 0 ; i < n_links ; i ++ ) {
		for ( int j = 0 ; j < n_points ; j ++ ) {
			arrays.set_weight( j, i, weights[ n_points * i + j ] ) ;
		}
	}
}

//----------------------------------------------------------------
//  load nurb ( FBX 6.0 and beyond ? )
//----------------------------------------------------------------

#if ( AFTER_FBXSDK_200611 )
void fbx_loader::load_nurb( KFbxNurbsSurface *nurb, fbx_arrays &arrays )
{
	KFbxLayer *layer = nurb->GetLayer( 0 ) ;
	KFbxLayerElementMaterial *materials = ( layer == 0 ) ? 0 : layer->GetMaterials() ;
	#if ( AFTER_FBXSDK_201102 == 0 )
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetDiffuseTextures() ;
	#else // AFTER_FBXSDK_201102
	KFbxLayerElementTexture *textures = ( layer == 0 ) ? 0 : layer->GetTextures( KFbxLayerElement::eDIFFUSE_TEXTURES ) ;
	#endif // AFTER_FBXSDK_201102
	int mat_idx = get_material_index( materials, textures ) ;
	string mat_name = get_material_name( mat_idx ) ;
	m_part->AttachChild( m_mesh = new MdxMesh ) ;
	m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;
	m_mesh->AttachChild( new MdxSetMaterial( mat_name.c_str() ) ) ;

	//  subdivision

	int u_step = nurb->GetUStep() ;
	int v_step = nurb->GetVStep() ;
	if ( u_step != DEFAULT_PATCH_SUBDIV || v_step != DEFAULT_PATCH_SUBDIV ) {
		m_mesh->AttachChild( new MdxSubdivision( u_step, v_step ) ) ;
	}

	//  knot vector

	int u_degree = nurb->GetUOrder() - 1 ;
	int v_degree = nurb->GetVOrder() - 1 ;
	kDouble *knots = nurb->GetUKnotVector() ;
	int n_knots = nurb->GetUKnotCount() ;
	int knot_type = get_knot_type( knots, n_knots, u_degree ) ;
	if ( knot_type & KNOT_TYPE_NONUNIFORM ) {
		MdxKnotVectorU *cmd = new MdxKnotVectorU ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetKnotCount( n_knots ) ;
		for ( int i = 0 ; i < n_knots ; i ++ ) {
			cmd->SetKnot( i, knots[ i ] ) ;
		}
	}
	knots = nurb->GetVKnotVector() ;
	n_knots = nurb->GetVKnotCount() ;
	knot_type = get_knot_type( knots, n_knots, v_degree ) ;
	if ( knot_type & KNOT_TYPE_NONUNIFORM ) {
		MdxKnotVectorV *cmd = new MdxKnotVectorV ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetKnotCount( n_knots ) ;
		for ( int i = 0 ; i < n_knots ; i ++ ) {
			cmd->SetKnot( i, knots[ i ] ) ;
		}
	}

	//  control points  (i,j) <- (-i,j)

	int mode = 0 ;
	if ( nurb->GetNurbUType() != KFbxNurb::eOPEN ) mode |= MDX_B_SPLINE_CLOSED_U ;
	if ( nurb->GetNurbVType() != KFbxNurb::eOPEN ) mode |= MDX_B_SPLINE_CLOSED_V ;
	bool u_periodic = ( nurb->GetNurbUType() == KFbxNurb::ePERIODIC ) ;
	bool v_periodic = ( nurb->GetNurbVType() == KFbxNurb::ePERIODIC ) ;

	int width = nurb->GetUCount() ;
	int height = nurb->GetVCount() ;
	int width2 = !u_periodic ? width : width + u_degree ;
	int height2 = !v_periodic ? height : height + v_degree ;

	MdxDrawBSpline *cmd = new MdxDrawBSpline ;
	m_mesh->AttachChild( cmd ) ;
	cmd->SetMode( mode ) ;
	cmd->SetWidth( width2 ) ;
	cmd->SetHeight( height2 ) ;
	for ( int i = 0 ; i < height2 ; i ++ ) {
		int i2 = !v_periodic ? height - 1 - i : ( height + 2 - i ) % height ;
		for ( int j = 0 ; j < width2 ; j ++ ) {
			int j2 = !u_periodic ? j : j % width ;
			int ctrl_idx = width * i2 + j2 ;
			int idx = arrays.set_index( ctrl_idx, -1, -1, -1, false ) ;
			cmd->SetIndex( width2 * i + j, idx ) ;
		}
	}
}
#endif // AFTER_FBXSDK_200611

//----------------------------------------------------------------
//  load all materials
//----------------------------------------------------------------

void fbx_loader::load_materials()
{
	for ( int i = 0 ; i < (int)m_material_infos.size() ; i ++ ) {
		KFbxMaterial *material = m_material_infos[ i ].material ;
		KFbxTexture *texture = m_material_infos[ i ].texture ;
		bool has_vcolor = m_material_infos[ i ].vcolor ;
		string mat_name = get_material_name( i ) ;
		m_model->AttachChild( m_material = new MdxMaterial( mat_name.c_str() ) ) ;

		if ( material != 0 ) {
			KFbxColor dc, sc, ec, ac ;
			float op = 1.0f, sh = 0.0f, re = 0.0f ;
			#if ( AFTER_FBXSDK_200611 == 0 )
			material->GetDefaultDiffuseColor( dc ) ;
			material->GetDefaultSpecularColor( sc ) ;
			material->GetDefaultEmissiveColor( ec ) ;		// (>_<) ?
			material->GetDefaultAmbientColor( ac ) ;
			op = material->GetDefaultOpacity() ;
			sh = material->GetDefaultShininess() ;			// (>_<) ?
			re = material->GetDefaultReflectivity() ;		// (>_<) ?
			sh = powf( 2.0f, sh * 10.0f ) * m_shiny_scale ;		// (>_<) ??
			#elif ( AFTER_FBXSDK_200901 == 0 )
			kFbxClassId id = material->GetNewFbxClassId() ;
			KFbxPropertyDouble3 prop3 ;
			KFbxPropertyDouble1 prop ;
			if ( id.Is( KFbxSurfacePhong::ClassId ) ) {
				KFbxSurfacePhong *phong = (KFbxSurfacePhong *)material ;
				phong->GetDefaultDiffuseColor( dc ) ;
				phong->GetDefaultSpecularColor( sc ) ;
				phong->GetDefaultEmissiveColor( ec ) ;		// (>_<) ?
				phong->GetDefaultAmbientColor( ac ) ;
				// phong->GetDefaultOpacity( op ) ;		// (>_<) ??
				// phong->GetDefaultShininess( sh ) ;		// (>_<) ?
				// phong->GetDefaultReflectivity( f ) ;		// (>_<) ?
				prop = phong->GetTransparencyFactor() ;
				op = 1.0f - prop.Get() ;
				prop = phong->GetShininess() ;
				sh = prop.Get() * m_shiny_scale ;
				prop = phong->GetReflectionFactor() ;
				prop3 = phong->GetReflectionColor() ;
				re = prop.Get() * prop3.Get()[ 0 ] ;
			} else if ( id.Is( KFbxSurfaceLambert::ClassId ) ) {
				KFbxSurfaceLambert *lambert = (KFbxSurfaceLambert *)material ;
				lambert->GetDefaultDiffuseColor( dc ) ;
				lambert->GetDefaultEmissiveColor( ec ) ;	// (>_<) ?
				lambert->GetDefaultAmbientColor( ac ) ;
				// lambert->GetDefaultOpacity( op ) ;		// (>_<) ??
				prop = lambert->GetTransparencyFactor() ;
				op = 1.0f - prop.Get() ;
			} else {
				KFbxPropertyString str = material->GetShadingModel() ;
				const char *cp = str.Get().Buffer() ;
				m_shell->Warning( "unknown shading model \"%s\"\n", cp ) ;
			}
			#else // AFTER_FBXSDK_200901
			kFbxClassId id = material->GetClassId() ;
			fbxDouble3 prop3 ;
			if ( id.Is( KFbxSurfacePhong::ClassId ) ) {
				KFbxSurfacePhong *phong = (KFbxSurfacePhong *)material ;
				prop3 = phong->GetDiffuseColor().Get() ;
				dc.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = phong->GetSpecularColor().Get() ;
				sc.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = phong->GetEmissiveColor().Get() ;
				ec.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = phong->GetAmbientColor().Get() ;
				ac.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = phong->GetTransparentColor().Get() ;
				op = max3( vec4( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ) ;
				op = 1.0f - op * phong->GetTransparencyFactor().Get() ;
				sh = phong->GetShininess().Get() * m_shiny_scale ;
				prop3 = phong->GetReflectionColor().Get() ;
				re = max3( vec4( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ) ;
				re *= phong->GetReflectionFactor().Get() ;
			} else if ( id.Is( KFbxSurfaceLambert::ClassId ) ) {
				KFbxSurfaceLambert *lambert = (KFbxSurfaceLambert *)material ;
				prop3 = lambert->GetDiffuseColor().Get() ;
				dc.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = lambert->GetEmissiveColor().Get() ;
				ec.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = lambert->GetAmbientColor().Get() ;
				ac.Set( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ;
				prop3 = lambert->GetTransparentColor().Get() ;
				op = max3( vec4( prop3[ 0 ], prop3[ 1 ], prop3[ 2 ] ) ) ;
				op = 1.0f - op * lambert->GetTransparencyFactor().Get() ;
			} else {
				KFbxPropertyString str = material->GetShadingModel() ;
				const char *cp = str.Get().Buffer() ;
				m_shell->Warning( "unknown shading model \"%s\"\n", cp ) ;
			}
			#endif // AFTER_FBXSDK_200901

			#if ( AFTER_FBXSDK_200901 )
			if ( m_filter_transp != MODE_OFF && op < 1.0f
			  && get_material_texture( material, KFbxMaterial::sTransparentColor ) != 0 ) {
				op = 1.0f ;
			}
			#endif // AFTER_FBXSDK_200901

			vec4 col, col2 ;
			col.set( dc.mRed, dc.mGreen, dc.mBlue, op ) ;
			if ( !equals3( col, 1.0f ) && !has_vcolor ) {
				m_material->AttachChild( new MdxDiffuse( col ) ) ;
			}
			col2.set( sc.mRed, sc.mGreen, sc.mBlue, 1.0f ) ;
			if ( !equals3( col2, 0.0f ) && sh > 0.0f ) {
				m_material->AttachChild( new MdxSpecular( col2 ) ) ;
			}
			col2.set( ec.mRed, ec.mGreen, ec.mBlue, 1.0f ) ;
			if ( !equals3( col2, 0.0f ) ) {
				m_material->AttachChild( new MdxEmission( col2 ) ) ;
			}
			col2.set( ac.mRed, ac.mGreen, ac.mBlue, op ) ;
			// if ( !equals3( col2, col ) ) {
			if ( !equals3( col2, col ) && !equals3( col2, 0.0f ) ) {
				m_material->AttachChild( new MdxAmbient( col2 ) ) ;
			}

			if ( op < 1.0f && !has_vcolor ) {
				m_material->AttachChild( new MdxOpacity( op ) ) ;
			}
			if ( !equals3( col2, 0.0f ) && sh > 0.0f ) {
				m_material->AttachChild( new MdxShininess( sh ) ) ;
			}
		}
		if ( texture != 0 ) {
			m_material->AttachChild( m_layer = new MdxLayer ) ;

			vector<KFbxTexture *>::iterator it ;
			it = find( m_texture_infos.begin(), m_texture_infos.end(), texture ) ;
			int tex_idx = it - m_texture_infos.begin() ;
			string tex_name = get_texture_name( tex_idx ) ;
			m_layer->AttachChild( new MdxSetTexture( tex_name.c_str() ) ) ;
		}
	}
	for ( int j = 0 ; j < (int)m_texture_infos.size() ; j ++ ) {
		KFbxTexture *texture = m_texture_infos[ j ] ;
		string tex_name = get_texture_name( j ) ;
		m_model->AttachChild( m_texture = new MdxTexture( tex_name.c_str() ) ) ;
		m_texture->AttachChild( new MdxFileName( texture->GetFileName() ) ) ;

		KFbxVector4 t, s ;
		texture->GetDefaultT( t ) ;
		texture->GetDefaultS( s ) ;
		rect crop( t[ 0 ], 1.0f - s[ 1 ] - t[ 1 ], s[ 0 ], s[ 1 ] ) ;
		if ( crop.xy() != 0.0f ) m_texture->AttachChild( new MdxUVTranslate( crop.xy() ) ) ;
		if ( crop.wh() != 1.0f ) m_texture->AttachChild( new MdxUVScale( crop.wh() ) ) ;
	}
}

//----------------------------------------------------------------
//  sub routine ( animation )
//----------------------------------------------------------------

void fbx_loader::set_animation( MdxBlock *block, int command, int mode, MdxFCurve *fcurve )
{
	MdxAnimate *cmd = new MdxAnimate ;
	m_motion->AttachChild( cmd ) ;
	cmd->SetScope( block->GetTypeID() ) ;
	cmd->SetBlock( block->GetName() ) ;
	cmd->SetCommand( command ) ;
	cmd->SetMode( mode ) ;
	cmd->SetFCurve( fcurve->GetName() ) ;
}

void fbx_loader::adjust_rotate_fcurve( MdxFCurve *fcurve, quat &prerot, quat (*quat_from_rot)( const vec4 &r ) )
{
	//  convert to quaternion and apply pre-rotation

	if ( fcurve->GetInterp() == MDX_FCURVE_LINEAR ) fcurve->SetInterp( MDX_FCURVE_SPHERICAL ) ;
	fcurve->SetDimCount( 4 ) ;
	int n_keys = fcurve->GetKeyFrameCount() ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = fcurve->GetKeyFrame( i ) ;
		vec4 r = key.GetValueVec4( 0 ) * RADIANS_PER_DEGREE ;
		key.SetValueQuat( 0, prerot * quat_from_rot( r ) ) ;
	}
}

void fbx_loader::adjust_morph_fcurve( MdxFCurve *fcurve )
{
	//  calculate 1st shape channel

	int n_dims = fcurve->GetDimCount() ;
	int n_keys = fcurve->GetKeyFrameCount() ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = fcurve->GetKeyFrame( i ) ;
		float sum = 1.0f ;
		for ( int j = 1 ; j < n_dims ; j ++ ) {
			float val = key.GetValue( j ) * 0.01f ;
			key.SetValue( j, val ) ;
			sum -= val ;
		}
		key.SetValue( 0, sum ) ;
	}
}

//----------------------------------------------------------------
//  subroutines ( layer elements )
//----------------------------------------------------------------

int fbx_loader::get_material_index( KFbxLayerElementMaterial *materials, KFbxLayerElementTexture *textures, int poly_idx, bool has_vcolor )
{
	KFbxMaterial *material = 0 ;
	KFbxTexture *texture = 0 ;
	if ( materials != 0 ) {
		int num = 0 ;
		if ( materials->GetMappingMode() == KFbxLayerElement::eBY_POLYGON ) {
			if ( materials->GetReferenceMode() == KFbxLayerElement::eINDEX_TO_DIRECT ) {
				num = materials->GetIndexArray().GetAt( poly_idx ) ;
			} else {
				num = poly_idx ;
			}
		}
		#if ( AFTER_FBXSDK_200901 == 0 )
		KArrayTemplate<KFbxMaterial *> &array = materials->GetDirectArray() ;
		if ( num >= 0 && num < array.GetCount() ) material = array.GetAt( num ) ;
		#else // AFTER_FBXSDK_200901
		// KFbxLayerElementArrayTemplate<KFbxMaterial *> &array = materials->GetDirectArray() ;
		// if ( num >= 0 && num < array.GetCount() ) material = array.GetAt( num ) ;
		if ( num >= 0 && num < m_node->GetMaterialCount() ) material = m_node->GetMaterial( num ) ;
		#endif // AFTER_FBXSDK_200901
	}
	if ( textures != 0 ) {
		int num = 0 ;
		if ( textures->GetMappingMode() == KFbxLayerElement::eBY_POLYGON ) {
			if ( textures->GetReferenceMode() == KFbxLayerElement::eINDEX_TO_DIRECT ) {
				num = textures->GetIndexArray().GetAt( poly_idx ) ;
			} else {
				num = poly_idx ;
			}
		}
		#if ( AFTER_FBXSDK_200901 == 0 )
		KArrayTemplate<KFbxTexture *> &array = textures->GetDirectArray() ;
		#else // AFTER_FBXSDK_200901
		KFbxLayerElementArrayTemplate<KFbxTexture *> &array = textures->GetDirectArray() ;
		#endif // AFTER_FBXSDK_200901
		if ( num >= 0 && num < array.GetCount() ) texture = array.GetAt( num ) ;
	}
	#if ( AFTER_FBXSDK_200901 )
	if ( material != 0 && texture == 0 ) {
		texture = get_material_texture( material, KFbxMaterial::sDiffuse ) ;
	}
	#endif // AFTER_FBXSDK_200901

	if ( has_vcolor ) {
		if ( material != 0 && texture == 0 ) {
			m_vcolor_value = get_diffuse_color( material ) ;
			has_vcolor = ( m_vcolor_value != 0xffffffff ) ;
		} else {
			m_vcolor_value = 0xffffffff ;
			has_vcolor = false ;
		}
	}

	material_info p( material, texture, has_vcolor ) ;
	vector<material_info>::iterator it ;
	it = find( m_material_infos.begin(), m_material_infos.end(), p ) ;
	int num = it - m_material_infos.begin() ;
	if ( it == m_material_infos.end() ) {
		m_material_infos.push_back( p ) ;
		if ( texture != 0 ) {
			vector<KFbxTexture *>::iterator it ;
			it = find( m_texture_infos.begin(), m_texture_infos.end(), texture ) ;
			if ( it == m_texture_infos.end() ) m_texture_infos.push_back( texture ) ;
		}
	}
	return num ;
}

int fbx_loader::get_normal_index( KFbxLayerElementNormal *normals, int ctrl_idx, int vert_idx, int poly_idx )
{
	if ( normals == 0 ) return -1 ;
	if ( normals->GetReferenceMode() == KFbxLayerElement::eINDEX ) return -1 ;

	int index = 0 ;
	switch ( normals->GetMappingMode() ) {
	    case KFbxLayerElement::eBY_CONTROL_POINT :	index = ctrl_idx ;	break ;
	    case KFbxLayerElement::eBY_POLYGON_VERTEX :	index = vert_idx ;	break ;
	    case KFbxLayerElement::eBY_POLYGON :	index = poly_idx ;	break ;
	    case KFbxLayerElement::eALL_SAME :		index = 0 ;		break ;
	    case KFbxLayerElement::eNONE :		return -1 ;
	}
	if ( normals->GetReferenceMode() == KFbxLayerElement::eINDEX_TO_DIRECT ) {
		index = normals->GetIndexArray().GetAt( index ) ;
	}
	return index ;
}

int fbx_loader::get_vcolor_index( KFbxLayerElementVertexColor *vcolors, int ctrl_idx, int vert_idx, int poly_idx )
{
	if ( vcolors == 0 ) return -1 ;
	if ( vcolors->GetReferenceMode() == KFbxLayerElement::eINDEX ) return -1 ;

	int index = 0 ;
	switch ( vcolors->GetMappingMode() ) {
	    case KFbxLayerElement::eBY_CONTROL_POINT :	index = ctrl_idx ;	break ;
	    case KFbxLayerElement::eBY_POLYGON_VERTEX :	index = vert_idx ;	break ;
	    case KFbxLayerElement::eBY_POLYGON :	index = poly_idx ;	break ;
	    case KFbxLayerElement::eALL_SAME :		index = 0 ;		break ;
	    case KFbxLayerElement::eNONE :		return -1 ;
	}
	if ( vcolors->GetReferenceMode() == KFbxLayerElement::eINDEX_TO_DIRECT ) {
		index = vcolors->GetIndexArray().GetAt( index ) ;
	}
	return index ;
}

int fbx_loader::get_tcoord_index( KFbxLayerElementUV *tcoords, int ctrl_idx, int vert_idx, int poly_idx )
{
	if ( tcoords == 0 ) return -1 ;
	if ( tcoords->GetReferenceMode() == KFbxLayerElement::eINDEX ) return -1 ;

	int index = 0 ;
	switch ( tcoords->GetMappingMode() ) {
	    case KFbxLayerElement::eBY_CONTROL_POINT :	index = ctrl_idx ;	break ;
	    case KFbxLayerElement::eBY_POLYGON_VERTEX :	index = vert_idx ;	break ;
	    case KFbxLayerElement::eBY_POLYGON :	index = poly_idx ;	break ;
	    case KFbxLayerElement::eALL_SAME :		index = 0 ;		break ;
	    case KFbxLayerElement::eNONE :		return -1 ;
	}
	if ( tcoords->GetReferenceMode() == KFbxLayerElement::eINDEX_TO_DIRECT ) {
		index = tcoords->GetIndexArray().GetAt( index ) ;
	}
	return index ;
}

rgba8888 fbx_loader::get_diffuse_color( KFbxMaterial *material )
{
	vec4 diffuse = 1.0f ;
	#if ( AFTER_FBXSDK_200611 == 0 )
	KFbxColor color ;
	material->GetDefaultDiffuseColor( color ) ;
	float alpha = material->GetDefaultOpacity() ;
	diffuse.set( color.mRed, color.mGreen, color.mBlue, alpha ) ;
	#elif ( AFTER_FBXSDK_200901 == 0 )
	kFbxClassId id = material->GetNewFbxClassId() ;
	if ( id.Is( KFbxSurfacePhong::ClassId ) || id.Is( KFbxSurfaceLambert::ClassId ) ) {
		KFbxSurfaceLambert *lambert = (KFbxSurfaceLambert *)material ;
		KFbxColor color ;
		lambert->GetDefaultDiffuseColor( color ) ;
		KFbxPropertyDouble1 prop = lambert->GetTransparencyFactor() ;
		diffuse.set( color.mRed, color.mGreen, color.mBlue, 1.0f - prop.Get() ) ;
	}
	#else // AFTER_FBXSDK_200901
	kFbxClassId id = material->GetClassId() ;
	if ( id.Is( KFbxSurfacePhong::ClassId ) || id.Is( KFbxSurfaceLambert::ClassId ) ) {
		KFbxSurfaceLambert *lambert = (KFbxSurfaceLambert *)material ;
		fbxDouble3 color = lambert->GetDiffuseColor().Get() ;
		fbxDouble3 trans = lambert->GetTransparentColor().Get() ;
		float alpha = max3( vec4( trans[ 0 ], trans[ 1 ], trans[ 2 ] ) ) ;
		alpha = 1.0f - alpha * lambert->GetTransparencyFactor().Get() ;
		diffuse.set( color[ 0 ], color[ 1 ], color[ 2 ], alpha ) ;
	}
	#endif // AFTER_FBXSDK_200901
	return diffuse ;
}

KFbxTexture *fbx_loader::get_material_texture( KFbxMaterial *material, const char *name )
{
	#if ( AFTER_FBXSDK_200901 )
	KFbxProperty prop = material->FindProperty( name ) ;
	if ( !prop.IsValid() ) return 0 ;
	KFbxLayeredTexture *layered = (KFbxLayeredTexture *)prop.GetSrcObject( KFbxLayeredTexture::ClassId, 0 ) ;
	if ( layered != 0 ) return (KFbxTexture *)layered->GetSrcObject( KFbxTexture::ClassId, 0 ) ;
	KFbxTexture *texture = (KFbxTexture *)prop.GetSrcObject( KFbxTexture::ClassId, 0 ) ;
	if ( texture != 0 ) return texture ;
	#endif // AFTER_FBXSDK_200901
	return 0 ;
}

//----------------------------------------------------------------
//  sub routine ( offset matrix )
//----------------------------------------------------------------

mat4 fbx_loader::get_offset_matrix( KFbxLink *link )
{
	KFbxXMatrix mat_s, mat_d, mat_o ;
	link->GetTransformMatrix( mat_s ) ;
	link->GetTransformLinkMatrix( mat_d ) ;
	mat_o = mat_d.Inverse() * mat_s ;
	kDouble *p = (kDouble *)mat_o ;
	return mat4( p[  0 ], p[  1 ], p[  2 ], p[  3 ],
	             p[  4 ], p[  5 ], p[  6 ], p[  7 ],
	             p[  8 ], p[  9 ], p[ 10 ], p[ 11 ],
	             p[ 12 ], p[ 13 ], p[ 14 ], p[ 15 ] ) ;
}

//----------------------------------------------------------------
//  sub routine ( node name )
//----------------------------------------------------------------

string fbx_loader::get_node_name( KFbxNode *node )
{
	string name = node->GetName() ;
	string::size_type colons = name.find( "::" ) ;
	return ( colons == string::npos ) ? name : name.substr( colons + 2 ) ;
}

string fbx_loader::get_material_name( int mat_idx )
{
	if ( m_use_material_name == MODE_OFF ) {
		return str_format( "material-%d", mat_idx ) ;
	}

	KFbxMaterial *material = m_material_infos[ mat_idx ].material ;
	string name = ( material == 0 ) ? "" : material->GetName() ;
	if ( name == "" ) name = str_format( "Material::material-%d", mat_idx ) ;
	int duplication = 0 ;
	for ( int i = 0 ; i < mat_idx ; i ++ ) {
		KFbxMaterial *material2 = m_material_infos[ i ].material ;
		if ( material2 != 0 && name == material2->GetName() ) duplication ++ ;
	}
	if ( duplication > 0 ) name = str_format( "%s-%04d", name.c_str(), duplication - 1 ) ;
	string::size_type colons = name.find( "::" ) ;
	return ( colons == string::npos ) ? name : name.substr( colons + 2 ) ;
}

string fbx_loader::get_texture_name( int tex_idx )
{
	if ( m_use_texture_name == MODE_OFF ) {
		return str_format( "texture-%d", tex_idx ) ;
	}

	KFbxTexture *texture = m_texture_infos[ tex_idx ] ;
	string name = ( texture == 0 ) ? "" : texture->GetName() ;
	if ( name == "" ) name = str_format( "Texture::texture-%d", tex_idx ) ;
	int duplication = 0 ;
	for ( int i = 0 ; i < tex_idx - 1 ; i ++ ) {
		KFbxTexture *texture2 = m_texture_infos[ i ] ;
		if ( texture2 != 0 && name == texture2->GetName() ) duplication ++ ;
	}
	if ( duplication > 0 ) name = str_format( "%s-%04d", name.c_str(), duplication ) ;
	string::size_type colons = name.find( "::" ) ;
	return ( colons == string::npos ) ? name : name.substr( colons + 2 ) ;
}

//----------------------------------------------------------------
//  sub routine ( knot type )
//----------------------------------------------------------------

int fbx_loader::get_knot_type( const kDouble *knots, int n_knots, int degree )
{
	int i ;

	if ( n_knots < degree * 2 + 2 ) {
		return KNOT_TYPE_CLOSE | KNOT_TYPE_NONUNIFORM ;
	}
	float D = knots[ degree + 1 ] - knots[ degree ] ;
	float E = fabsf( D ) * KNOT_REL_MARGIN ;
	if ( E < KNOT_ABS_MARGIN ) E = KNOT_ABS_MARGIN ;

	int type = KNOT_TYPE_OPEN ;
	int mult = degree ;
	const kDouble *knots2 = knots + n_knots - degree ;
	for ( i = 1 ; i < degree - 1 ; i ++ ) {
		if ( fabsf( knots[ i + 1 ] - knots[ i ] ) > E
		  || fabsf( knots2[ i ] - knots2[ i - 1 ] ) > E ) {
			type = KNOT_TYPE_CLOSE ;
			mult = 1 ;
			break ;
		}
	}

	for ( i = mult ; i < n_knots - mult - 1 ; i ++ ) {
		if ( fabsf( knots[ i + 1 ] - knots[ i ] - D ) > E ) {
			type |= KNOT_TYPE_NONUNIFORM ;
			break ;
		}
	}
	return type ;
}

//----------------------------------------------------------------
//  sub routine ( fcurve )
//----------------------------------------------------------------

float fbx_loader::eval_fcurve( KFCurve *src, KTime time, float val )
{
	return ( src != 0 ) ? src->Evaluate( time ) : val ;
}
