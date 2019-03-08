#include "xsi_loader.h"

#pragma warning( disable:4244 )

//----------------------------------------------------------------
//  config
//----------------------------------------------------------------

#define	DUPLICATIVE_SHADER_PARAM	1
#define	USE_ENABLE_LIGHTING			0
#define	USE_CUSTOM_PARAMETERS		0

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

#define RADIAN_PER_DEGREE (3.1415926536f/180.0f)

#define KNOT_TYPE_CLOSE (0)
#define KNOT_TYPE_OPEN (1)
#define KNOT_TYPE_UNIFORM (0)
#define KNOT_TYPE_NONUNIFORM (2)

#define KNOT_REL_MARGIN (0.01f)
#define KNOT_ABS_MARGIN (0.0001f)

#define MATERIAL_APPLIED_TO_POLYGON (1)
#define MATERIAL_APPLIED_TO_NURBS (2)
#define MATERIAL_MAPPED_BY_PROJECTION (4)

#if ( AFTER_CROSSWALK_2_6 )
static SI_Int *get_indices( CSLXSITriangleList *prim, xsi_arrays &arrays, int type ) ;
static SI_Int *get_indices( CSLXSITriangleStripList *prim, xsi_arrays &arrays, int type ) ;
static SI_Int *get_indices( CSLXSIPolygonList *prim, xsi_arrays &arrays, int type ) ;
static SI_Int *get_attr_indices( CSLXSISubComponentList *prim, xsi_arrays &arrays, int type ) ;
#endif // AFTER_CROSSWALK_2_6

static int knot_type( SI_Float *knots, int n_knots, int degree ) ;
static SI_Void on_warning( char* mesg, int level ) ;

static MdxShell *g_shell = 0 ;

//----------------------------------------------------------------
//  xsi_loader
//----------------------------------------------------------------

xsi_loader::xsi_loader( MdxShell *shell )
{
	m_shell = shell ;
	m_vcolor_lighting = MODE_OFF ;
	m_vcolor_specular = MODE_OFF ;
	m_ignore_textrans = MODE_AUTO ;
	m_ignore_prefix = MODE_OFF ;

	m_check_custom_param_mode = MODE_OFF ;
	m_custom_param_transform_mode = MODE_OFF ;
	m_custom_param_material_mode = MODE_OFF ;
	m_custom_param_userdata_mode = MODE_OFF ;
	m_check_custom_param_prefix = "" ;
	m_custom_param_transform_prefix = "" ;
	m_custom_param_material_prefix = "" ;
	m_custom_param_userdata_prefix = "" ;

	m_shiny_scale = 1.0f ;
}

xsi_loader::~xsi_loader()
{
	;
}

//----------------------------------------------------------------
//  load file
//----------------------------------------------------------------

bool xsi_loader::load( MdxBlock *&block, const string &filename )
{
	block = 0 ;

	//  reset

	m_filename = filename ;
	m_material_states.clear() ;
	m_current_object = 0 ;

	//  open scene

	CSLScene scene ;
	g_shell = m_shell ;
	scene.SetWarningCallback( on_warning ) ;
	if ( scene.Open( (char *)m_filename.c_str() ) != SI_SUCCESS ) {
		if ( m_shell != 0 ) {
			m_shell->Error( "load failed \"%s\"\n", m_filename.c_str() ) ;
		}
		return false ;
  	}
	scene.Read() ;

	//  load

	CSLSceneInfo *info = scene.SceneInfo() ;
	string name = get_node_name( info ) ;
	m_file = new MdxFile ;
	m_file->AttachChild( m_model = new MdxModel( name.c_str() ) ) ;
	m_model->AttachChild( m_motion = new MdxMotion( name.c_str() ) ) ;
	m_motion->AttachChild( new MdxFrameLoop( info->GetStart(), info->GetEnd() ) ) ;
	m_motion->AttachChild( new MdxFrameRate( info->GetFrameRate() ) ) ;

	load_materials( scene.GetMaterialLibrary() ) ;
	load_geometries( scene.Root(), 0 ) ;
	ignore_textrans() ;

	//  close scene

	scene.Close() ;

	block = m_file ;
	return true ;
}

//----------------------------------------------------------------
//  load all geometries
//----------------------------------------------------------------

void xsi_loader::load_geometries( CSLModel* model, MdxBone *parent )
{
	string name = get_node_name( model ) ;

	switch ( model->GetPrimitiveType() ) {
	    case CSLTemplate::SI_IK_EFFECTOR :
	    case CSLTemplate::SI_IK_JOINT :
	    case CSLTemplate::SI_IK_ROOT :
	    case CSLTemplate::SI_NULL_OBJECT :
	    case CSLTemplate::SI_MODEL :
		if ( model->Parent() == 0 ) break ;
		load_bone( model, parent ) ;
		parent = m_bone ;
		break ;
	    case CSLTemplate::SI_SURFACE_MESH :
	    case CSLTemplate::SI_MESH :
	    #if ( AFTER_CROSSWALK_2_6 )
	    case CSLTemplate::XSI_MESH :
	    #endif // AFTER_CROSSWALK_2_6
		load_bone( model, parent ) ;
		load_part( model ) ;
		parent = m_bone ;
		break ;
	    case CSLTemplate::SI_INSTANCE : {
		load_bone( model, parent ) ;
		parent = m_bone ;
		CSLInstance *inst = (CSLInstance *)model->Primitive() ;
		if ( inst == 0 ) return ;
		model = inst->GetReference() ;
		if ( model == 0 ) return ;

		load_part( model ) ;
		break ;
	    }
	    default :
		break ;
	}

	CSLModel **children = model->GetChildrenList() ;
	for ( int i = 0 ; i < model->GetChildrenCount() ; i ++ ) {
		load_geometries( children[ i ], parent ) ;
	}

	if ( parent != 0 ) {
		m_bone = parent ;
		load_envelope_bones( model ) ;
	}
}

//----------------------------------------------------------------
//  load bone
//----------------------------------------------------------------

void xsi_loader::load_bone( CSLModel *model, MdxBone *parent )
{
	string name = get_node_name( model ) ;	// original name
	m_model->AttachChild( m_bone = new MdxBone( name.c_str() ) ) ;
	string inst = m_bone->GetName() ;	// instance name

	m_instance_names[ name ] = inst ;

	if ( parent != 0 ) {
		m_bone->AttachChild( new MdxParentBone( parent->GetName() ) ) ;
	}

	//  visibility

	CSLVisibility *vis = model->Visibility() ;
	if ( vis != 0 ) {
		int val = vis->GetVisibility() ;
		if ( val == 0 ) {
			m_bone->AttachChild( new MdxVisibility( val ) ) ;
		}
		CSLFCurve *anim = vis->GetSpecificFCurve( CSLTemplate::SI_NODEVIS ) ;
		if ( anim != 0 ) {
			if ( !copy_animation( MDX_BONE, name, MDX_VISIBILITY, 0, inst ) ) {
				m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
				xsi_fcurve fcurve ;
				fcurve.load( anim, (float)val ) ;
				fcurve.save( m_fcurve ) ;
				set_animation( MDX_BONE, inst, MDX_VISIBILITY, 0, m_fcurve->GetName() ) ;
			}
		}
	}

	//  transform

	if ( model->Transform() != 0 ) {
		load_transform( model->Transform(), name, inst ) ;
	} else {
		#if ( AFTER_CROSSWALK_2_6 )
		CSLXSITransform *trans = model->XSITransform() ;
		if ( trans != 0 ) {
			if ( trans->GetPolymatricks() == 0 ) {
				load_transform( trans, name, inst ) ;
			} else {
				load_transform( trans->GetPolymatricks(), name, inst ) ;
			}
		}
		#endif // AFTER_CROSSWALK_2_6
	}

	//  custom param

	load_custom_param( m_bone, model ) ;
}

void xsi_loader::load_transform( CSLTransform *trans, const string &name, const string &inst )
{
	CSIBCVector3D vt = trans->GetTranslation() ;
	CSIBCVector3D vr = trans->GetEulerRotation() ;
	CSIBCVector3D vs = trans->GetScale() ;
	vec3 t( vt.GetX(), vt.GetY(), vt.GetZ() ) ;
	vec3 r( vr.GetX(), vr.GetY(), vr.GetZ() ) ;
	vec3 s( vs.GetX(), vs.GetY(), vs.GetZ() ) ;
	if ( t != 0.0f ) m_bone->AttachChild( new MdxTranslate( t ) ) ;
	if ( r != 0.0f ) m_bone->AttachChild( new MdxRotateZYX( r ) ) ;
	if ( s != 1.0f ) m_bone->AttachChild( new MdxScale( s ) ) ;

	set_transform_fcurves( name, inst, MDX_TRANSLATE, t, trans, CSLTemplate::SI_TRANSLATION_X ) ;
	set_transform_fcurves( name, inst, MDX_ROTATE_ZYX, r, trans, CSLTemplate::SI_ROTATION_X ) ;
	set_transform_fcurves( name, inst, MDX_SCALE, s, trans, CSLTemplate::SI_SCALING_X ) ;
}

#if ( AFTER_CROSSWALK_2_6 )
void xsi_loader::load_transform( CSLXSITransform *trans, const string &name, const string &inst )
{
	CSIBCVector3D vp = trans->GetPivotPosition() ;
	CSIBCVector3D vt = trans->GetTranslation() ;
	CSIBCVector3D vr = trans->GetEulerRotation() ;
	CSIBCVector3D vs = trans->GetScale() ;
	vec3 p( vp.GetX(), vp.GetY(), vp.GetZ() ) ;
	vec3 t( vt.GetX(), vt.GetY(), vt.GetZ() ) ;
	vec3 r( vr.GetX(), vr.GetY(), vr.GetZ() ) ;
	vec3 s( vs.GetX(), vs.GetY(), vs.GetZ() ) ;
	if ( p != 0.0f ) m_bone->AttachChild( new MdxPivot( p ) ) ;
	if ( t != 0.0f ) m_bone->AttachChild( new MdxTranslate( t ) ) ;
	if ( r != 0.0f ) m_bone->AttachChild( new MdxRotateZYX( r ) ) ;
	if ( s != 1.0f ) m_bone->AttachChild( new MdxScale( s ) ) ;

	static const char *Positions[] = { "posx", "posy", "posz" } ;
	static const char *Rotations[] = { "rotx", "roty", "rotz" } ;
	static const char *Scalings[] = { "sclx", "scly", "sclz" } ;
	set_transform_fcurves( name, inst, MDX_TRANSLATE, t, trans, Positions ) ;
	set_transform_fcurves( name, inst, MDX_ROTATE_ZYX, r, trans, Rotations ) ;
	set_transform_fcurves( name, inst, MDX_SCALE, s, trans, Scalings ) ;
}

void xsi_loader::load_transform( CSLXSIPolymatricks *poly, const string &name, const string &inst )
{
	CSLXSITranslate *trans = 0, *pivot = 0 ;
	CSLXSIRotate *rots[ 6 ] = { 0, 0, 0, 0, 0, 0 } ;
	CSLXSIScale *scl = 0 ;
	int n_rots = 0 ;

	CSLTemplate **nodes = poly->GetTransformNodes() ;
	for ( int i = 0 ; i < poly->GetTransformNodeCount() ; i ++ ) {
		CSLTemplate *node = nodes[ i ] ;
		switch ( node->Type() ) {
		    case CSLTemplate::XSI_TRANSLATE :
			if ( trans == 0 ) trans = (CSLXSITranslate *)node ;
			if ( n_rots == 0 ) pivot = (CSLXSITranslate *)node ;
			break ;
		    case CSLTemplate::XSI_ROTATE :
			if ( n_rots < 6 ) rots[ n_rots ] = (CSLXSIRotate *)node ;
			n_rots ++ ;
			break ;
		    case CSLTemplate::XSI_SCALE :
			if ( scl == 0 ) scl = (CSLXSIScale *)node ;
			break ;
		}
	}

	if ( pivot != 0 && pivot != trans ) {
		CSIBCVector3D vp = pivot->GetTranslation() ;
		vec3 p( vp.GetX(), vp.GetY(), vp.GetZ() ) ;
		if ( p != 0.0f ) m_bone->AttachChild( new MdxPivot( p ) ) ;
	}
	if ( trans != 0 ) {
		CSIBCVector3D vt = trans->GetTranslation() ;
		vec3 t( vt.GetX(), vt.GetY(), vt.GetZ() ) ;
		if ( t != 0.0f ) m_bone->AttachChild( new MdxTranslate( t ) ) ;

		static const char *XYZ[] = { "x", "y", "z" } ;
		set_transform_fcurves( name, inst, MDX_TRANSLATE, t, trans, XYZ ) ;
	}
	if ( n_rots > 0 ) {
		CSLFCurve *anims[ 3 ] = { 0, 0, 0 } ;
		vec3 r ;
		int idx = ( n_rots <= 6 ) ? 0 : 3 ;
		for ( int i = 0 ; i < 3 ; i ++ ) {
			CSLXSIRotate *rot = rots[ idx + i ] ;
			if ( rot == 0 ) continue ;
			CSIBCVector3D va = rot->GetAxis() ;
			int axis = ( va.GetX() != 0.0f ) ? 0 : ( va.GetY() != 0.0f ) ? 1 : 2 ;
			r[ axis ] = rot->GetAngle() ;
			anims[ axis ] = get_fcurve( rot, "angle" ) ;
		}
		if ( r != 0.0f ) m_bone->AttachChild( new MdxRotateZYX( r ) ) ;
		set_transform_fcurves( name, inst, MDX_ROTATE_ZYX, r, anims ) ;
	}
	if ( scl != 0 ) {
		CSIBCVector3D vs = scl->GetScale() ;
		vec3 s( vs.GetX(), vs.GetY(), vs.GetZ() ) ;
		if ( s != 1.0f ) m_bone->AttachChild( new MdxScale( s ) ) ;

		static const char *XYZ[] = { "x", "y", "z" } ;
		set_transform_fcurves( name, inst, MDX_SCALE, s, scl, XYZ ) ;
	}
}
#endif // AFTER_CROSSWALK_2_6

//----------------------------------------------------------------
//  load part
//----------------------------------------------------------------

void xsi_loader::load_part( CSLModel* model )
{
	m_current_object = model ;

	switch ( model->GetPrimitiveType() ) {
	    case CSLTemplate::SI_MESH : {
		string name = get_node_name( model ) ;

		//  instanced ?
		m_part = (MdxPart *)m_model->FindChild( MDX_PART, name.c_str() ) ;
		if ( m_part != 0 ) {
			m_bone->AttachChild( new MdxDrawPart( name.c_str() ) ) ;
			break ;
		}

		m_model->AttachChild( m_part = new MdxPart( name.c_str() ) ) ;
		m_bone->AttachChild( new MdxDrawPart( m_part->GetName() ) ) ;

		m_part->AttachChild( m_arrays = new MdxArrays ) ;
		CSLMesh *mesh = (CSLMesh *)model->Primitive() ;
		xsi_arrays arrays ;
		arrays.load( mesh->Shape() ) ;
		load_triangles( mesh, arrays ) ;
		load_tristrips( mesh, arrays ) ;
		load_polygons( mesh, arrays ) ;
		load_shape_anim( mesh, arrays ) ;
		m_nurbs_wrap = 0 ;
		load_envelope_weights( model, arrays ) ;
		arrays.save( m_arrays ) ;
		break ;
	    }
	    #if ( AFTER_CROSSWALK_2_6 )
	    case CSLTemplate::XSI_MESH : {
		string name = get_node_name( model ) ;

		//  instanced ?
		m_part = (MdxPart *)m_model->FindChild( MDX_PART, name.c_str() ) ;
		if ( m_part != 0 ) {
			m_bone->AttachChild( new MdxDrawPart( name.c_str() ) ) ;
			break ;
		}

		m_model->AttachChild( m_part = new MdxPart( name.c_str() ) ) ;
		m_bone->AttachChild( new MdxDrawPart( m_part->GetName() ) ) ;

		m_part->AttachChild( m_arrays = new MdxArrays ) ;
		CSLXSIMesh *mesh = (CSLXSIMesh *)model->Primitive() ;
		xsi_arrays arrays ;
		arrays.load( mesh->XSIShape() ) ;
		load_triangles( mesh, arrays ) ;
		load_tristrips( mesh, arrays ) ;
		load_polygons( mesh, arrays ) ;
		load_shape_anim( mesh, arrays ) ;
		m_nurbs_wrap = 0 ;
		load_envelope_weights( model, arrays ) ;
		arrays.save( m_arrays ) ;
		break ;
	    }
	    #endif // AFTER_CROSSWALK_2_6
	    case CSLTemplate::SI_SURFACE_MESH : {
		string name = get_node_name( model ) ;
		m_model->AttachChild( m_part = new MdxPart( name.c_str() ) ) ;
		m_bone->AttachChild( new MdxDrawPart( m_part->GetName() ) ) ;

		CSLBaseMaterial *material = 0 ;
		if ( model->GlobalMaterial() != 0 ) {
			material = model->GlobalMaterial()->GetMaterial() ;
		}
		CSLSurfaceMesh *mesh = (CSLSurfaceMesh *)model->Primitive() ;
		CSLNurbsSurface **surfs = mesh->Surfaces() ;
		for ( int i = 0 ; i < mesh->GetSurfaceCount() ; i ++ ) {
			m_part->AttachChild( m_arrays = new MdxArrays ) ;
			CSLNurbsSurface *surf = surfs[ i ] ;
			xsi_arrays arrays ;
			arrays.load( surf ) ;
			load_surface( surf, arrays, material ) ;
			load_shape_anim( surf, arrays ) ;
			if ( i == 0 ) load_envelope_weights( model, arrays ) ;
			arrays.save( m_arrays ) ;
		}
		break ;
	    }
	    default :
		break ;
	}
}

//----------------------------------------------------------------
//  load meshes
//----------------------------------------------------------------

void xsi_loader::load_triangles( CSLMesh *mesh, xsi_arrays &arrays )
{
	CSLTriangleList **prims = (CSLTriangleList **)mesh->TriangleLists() ;
	for ( int i = 0 ; i < mesh->GetTriangleListCount() ; i ++ ) {
		m_part->AttachChild( m_mesh = new MdxMesh ) ;
		m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

		CSLTriangleList *prim = prims[ i ] ;
		if ( prim->GetMaterial() != 0 ) {
			string name = get_node_name( prim->GetMaterial() ) ;
			m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
			m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_POLYGON ;
			arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;
		} else {
			arrays.set_tspace( "" ) ;
		}

		MdxDrawArrays *cmd = new MdxDrawArrays ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetMode( MDX_PRIM_TRIANGLES ) ;
		cmd->SetVertexCount( prim->GetTriangleCount() * 3 ) ;
		cmd->SetPrimCount( 1 ) ;
		SI_Int *vids = prim->GetVertexIndicesPtr() ;
		SI_Int *nids = prim->GetNormalIndicesPtr() ;
		SI_Int *cids = prim->GetColorIndicesPtr() ;
		SI_Int *tids = prim->GetUVIndicesPtr( 0 ) ;
		for ( int j = 0 ; j < prim->GetTriangleCount() * 3 ; j ++ ) {
			int idx = arrays.set_index( vids, nids, cids, tids, j ) ;
			cmd->SetIndex( j, idx ) ;
		}
	}
}

void xsi_loader::load_tristrips( CSLMesh *mesh, xsi_arrays &arrays )
{
	CSLTriangleStripList **prims = (CSLTriangleStripList **)mesh->TriangleStripLists() ;
	for ( int i = 0 ; i < mesh->GetTriangleStripListCount() ; i ++ ) {
		m_part->AttachChild( m_mesh = new MdxMesh ) ;
		m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

		CSLTriangleStripList *prim = prims[ i ] ;
		if ( prim->GetMaterial() != 0 ) {
			string name = get_node_name( prim->GetMaterial() ) ;
			m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
			m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_POLYGON ;
			arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;
		} else {
			arrays.set_tspace( "" ) ;
		}

		CSLTriangleStrip **strs = prim->TriangleStrips() ;
		for ( int j = 0 ; j < prim->GetTriangleStripCount() ; j ++ ) {
			CSLTriangleStrip *str = strs[ j ] ;
			int n_verts = str->GetVertexCount() ;
			MdxDrawArrays *cmd = new MdxDrawArrays ;
			m_mesh->AttachChild( cmd ) ;
			cmd->SetMode( MDX_PRIM_TRIANGLE_STRIP ) ;
			cmd->SetVertexCount( n_verts ) ;
			cmd->SetPrimCount( 1 ) ;
			SI_Int *vids = str->GetVertexIndicesPtr() ;
			SI_Int *nids = str->GetNormalIndicesPtr() ;
			SI_Int *cids = str->GetColorIndicesPtr() ;
			SI_Int *tids = str->GetUVIndicesPtr( 0 ) ;
			for ( int k = 0 ; k < n_verts ; k ++ ) {
				int idx = arrays.set_index( vids, nids, cids, tids, k ) ;
				cmd->SetIndex( k, idx ) ;
			}
		}
	}
}

void xsi_loader::load_polygons( CSLMesh *mesh, xsi_arrays &arrays )
{
	CSLPolygonList **prims = (CSLPolygonList **)mesh->PolygonLists() ;
	for ( int i = 0 ; i < mesh->GetPolygonListCount() ; i ++ ) {
		m_part->AttachChild( m_mesh = new MdxMesh ) ;
		m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

		CSLPolygonList *prim = prims[ i ] ;
		if ( prim->GetMaterial() != 0 ) {
			string name = get_node_name( prim->GetMaterial() ) ;
			m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
			m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_POLYGON ;
			arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;
		} else {
			arrays.set_tspace( "" ) ;
		}

		SI_Int *counts = prim->GetPolygonVertexCountListPtr() ;
		if ( counts == 0 ) continue ;
		SI_Int *vids = prim->GetVertexIndicesPtr() ;
		SI_Int *nids = prim->GetNormalIndicesPtr() ;
		SI_Int *cids = prim->GetColorIndicesPtr() ;
		SI_Int *tids = prim->GetUVIndicesPtr( 0 ) ;
		int num = 0 ;
		for ( int j = 0 ; j < prim->GetPolygonCount() ; j ++ ) {
			int n_verts = counts[ j ] ;
			MdxDrawArrays *cmd = new MdxDrawArrays ;
			m_mesh->AttachChild( cmd ) ;
			cmd->SetMode( MDX_PRIM_TRIANGLE_FAN ) ;
			cmd->SetVertexCount( n_verts ) ;
			cmd->SetPrimCount( 1 ) ;
			for ( int k = 0 ; k < n_verts ; k ++ ) {
				int idx = arrays.set_index( vids, nids, cids, tids, num ++ ) ;
				cmd->SetIndex( k, idx ) ;
			}
		}
	}
}

void xsi_loader::load_surface( CSLNurbsSurface *surf, xsi_arrays &arrays, CSLBaseMaterial *material )
{
	m_part->AttachChild( m_mesh = new MdxMesh ) ;
	m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

	string name = ( material == 0 ) ? "Default" : get_node_name( material ) ;
	m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
	m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_NURBS ;
	arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;

	int mode = 0 ;
	int type = knot_type( surf->GetUKnotListPtr(), surf->GetUKnotCount(), surf->GetUDegree() ) ;
	if ( !( KNOT_TYPE_OPEN & type ) ) mode |= MDX_B_SPLINE_CLOSED_U ;
	if ( KNOT_TYPE_NONUNIFORM & type ) {
		MdxKnotVectorU *cmd = new MdxKnotVectorU ;
		m_mesh->AttachChild( cmd ) ;
		SI_Float *knots = surf->GetUKnotListPtr() ;
		cmd->SetKnotCount( surf->GetUKnotCount() ) ;
		for ( int i = 0 ; i < surf->GetUKnotCount() ; i ++ ) {
			cmd->SetKnot( i, knots[ i ] ) ;
		}
	}
	type = knot_type( surf->GetVKnotListPtr(), surf->GetVKnotCount(), surf->GetVDegree() ) ;
	if ( !( KNOT_TYPE_OPEN & type ) ) mode |= MDX_B_SPLINE_CLOSED_V ;
	if ( KNOT_TYPE_NONUNIFORM & type ) {
		MdxKnotVectorV *cmd = new MdxKnotVectorV ;
		m_mesh->AttachChild( cmd ) ;
		SI_Float *knots = surf->GetVKnotListPtr() ;
		cmd->SetKnotCount( surf->GetVKnotCount() ) ;
		for ( int i = 0 ; i < surf->GetVKnotCount() ; i ++ ) {
			cmd->SetKnot( i, knots[ i ] ) ;
		}
	}
	int width = surf->GetUControlPointCount() ;
	int height = surf->GetVControlPointCount() ;

	MdxDrawBSpline *cmd = new MdxDrawBSpline ;
	m_mesh->AttachChild( cmd ) ;
	cmd->SetMode( mode ) ;
	cmd->SetWidth( width ) ;
	cmd->SetHeight( height ) ;
	int width2 = width - ( !( MDX_B_SPLINE_CLOSED_U & mode ) ? 0 : surf->GetUDegree() ) ;
	int height2 = height - ( !( MDX_B_SPLINE_CLOSED_V & mode ) ? 0 : surf->GetVDegree() ) ;
	int num = 0 ;
	for ( int i = height - 1 ; i >= 0 ; -- i ) {
		for ( int j = 0 ; j < width ; j ++ ) {
			int vid = width * ( i % height2 ) + ( j % width2 ) ;
			int idx = arrays.set_index( vid, -1, -1, -1, false ) ;
			cmd->SetIndex( num ++, idx ) ;
		}
	}
	m_nurbs_width = width2 ;
	m_nurbs_wrap = width - width2 ;
}

void xsi_loader::load_shape_anim( CSLGeometry *geom, xsi_arrays &arrays )
{
	CSLShapeAnimation *anim = geom->ShapeAnimation() ;
	if ( ( 0xffff0000 & (int)anim ) == 0 ) return ;		// (>_<)?

	CSLBaseShape **shapes = anim->Shapes() ;
	for ( int i = 0 ; i < anim->GetShapeCount() ; i ++ ) {
		arrays.load( shapes[ i ] ) ;
	}

	if ( anim->Animation() != 0 ) {
		m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
		xsi_fcurve fcurve ;
		fcurve.load( anim->Animation(), 1.0f ) ;
		fcurve.save( m_fcurve ) ;
		set_animation( MDX_BONE, m_bone->GetName(), MDX_MORPH_INDEX, 0, m_fcurve->GetName() ) ;
	}
}

//----------------------------------------------------------------
//  load meshes ( dotXSI 5.0 and beyond )
//----------------------------------------------------------------

#if ( AFTER_CROSSWALK_2_6 )
void xsi_loader::load_triangles( CSLXSIMesh *mesh, xsi_arrays &arrays )
{
	CSLXSITriangleList **prims = (CSLXSITriangleList **)mesh->XSITriangleLists() ;
	for ( int i = 0 ; i < mesh->GetXSITriangleListCount() ; i ++ ) {
		m_part->AttachChild( m_mesh = new MdxMesh ) ;
		m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

		CSLXSITriangleList *prim = prims[ i ] ;
		if ( prim->GetMaterial() != 0 ) {
			string name = get_node_name( prim->GetMaterial() ) ;
			m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
			m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_POLYGON ;
			arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;
		} else {
			arrays.set_tspace( "" ) ;
		}

		SI_Int *vids = get_indices( prim, arrays, 0 ) ;
		SI_Int *nids = get_indices( prim, arrays, 1 ) ;
		SI_Int *cids = get_indices( prim, arrays, 2 ) ;
		SI_Int *tids = get_indices( prim, arrays, 3 ) ;

		MdxDrawArrays *cmd = new MdxDrawArrays ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetMode( MDX_PRIM_TRIANGLES ) ;
		cmd->SetVertexCount( prim->GetTriangleCount() * 3 ) ;
		cmd->SetPrimCount( 1 ) ;
		for ( int j = 0 ; j < prim->GetTriangleCount() * 3 ; j ++ ) {
			int idx = arrays.set_index( vids, nids, cids, tids, j ) ;
			cmd->SetIndex( j, idx ) ;
		}
	}
}

void xsi_loader::load_tristrips( CSLXSIMesh *mesh, xsi_arrays &arrays )
{
	CSLXSITriangleStripList **prims = (CSLXSITriangleStripList **)mesh->XSITriangleStripLists() ;
	for ( int i = 0 ; i < mesh->GetXSITriangleStripListCount() ; i ++ ) {
		m_part->AttachChild( m_mesh = new MdxMesh ) ;
		m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

		CSLXSITriangleStripList *prim = prims[ i ] ;
		if ( prim->GetMaterial() != 0 ) {
			string name = get_node_name( prim->GetMaterial() ) ;
			m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
			m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_POLYGON ;
			arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;
		} else {
			arrays.set_tspace( "" ) ;
		}

		SI_Int *counts = get_indices( prim, arrays, -1 ) ;
		if ( counts == 0 ) continue ;
		SI_Int *vids = get_indices( prim, arrays, 0 ) ;
		SI_Int *nids = get_indices( prim, arrays, 1 ) ;
		SI_Int *cids = get_indices( prim, arrays, 2 ) ;
		SI_Int *tids = get_indices( prim, arrays, 3 ) ;
		int num = 0 ;
		for ( int j = 0 ; j < prim->GetTriangleStripCount() ; j ++ ) {
			int n_verts = counts[ j ] ;
			MdxDrawArrays *cmd = new MdxDrawArrays ;
			m_mesh->AttachChild( cmd ) ;
			cmd->SetMode( MDX_PRIM_TRIANGLE_STRIP ) ;
			cmd->SetVertexCount( n_verts ) ;
			cmd->SetPrimCount( 1 ) ;
			for ( int k = 0 ; k < n_verts ; k ++ ) {
				int idx = arrays.set_index( vids, nids, cids, tids, num ++ ) ;
				cmd->SetIndex( k, idx ) ;
			}
		}
	}
}

void xsi_loader::load_polygons( CSLXSIMesh *mesh, xsi_arrays &arrays )
{
	CSLXSIPolygonList **prims = (CSLXSIPolygonList **)mesh->XSIPolygonLists() ;
	for ( int i = 0 ; i < mesh->GetXSIPolygonListCount() ; i ++ ) {
		m_part->AttachChild( m_mesh = new MdxMesh ) ;
		m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;

		CSLXSIPolygonList *prim = prims[ i ] ;
		if ( prim->GetMaterial() != 0 ) {
			string name = get_node_name( prim->GetMaterial() ) ;
			m_mesh->AttachChild( new MdxSetMaterial( name.c_str() ) ) ;
			m_material_states[ name ].flags |= MATERIAL_APPLIED_TO_POLYGON ;
			arrays.set_tspace( m_material_states[ name ].find_tspace( m_current_object ) ) ;
		} else {
			arrays.set_tspace( "" ) ;
		}

		SI_Int *counts = get_indices( prim, arrays, -1 ) ;
		if ( counts == 0 ) continue ;
		SI_Int *vids = get_indices( prim, arrays, 0 ) ;
		SI_Int *nids = get_indices( prim, arrays, 1 ) ;
		SI_Int *cids = get_indices( prim, arrays, 2 ) ;
		SI_Int *tids = get_indices( prim, arrays, 3 ) ;
		int num = 0 ;
		for ( int j = 0 ; j < prim->GetPolygonCount() ; j ++ ) {
			int n_verts = counts[ j ] ;
			MdxDrawArrays *cmd = new MdxDrawArrays ;
			m_mesh->AttachChild( cmd ) ;
			cmd->SetMode( MDX_PRIM_TRIANGLE_FAN ) ;
			cmd->SetVertexCount( n_verts ) ;
			cmd->SetPrimCount( 1 ) ;
			for ( int k = 0 ; k < n_verts ; k ++ ) {
				int idx = arrays.set_index( vids, nids, cids, tids, num ++ ) ;
				cmd->SetIndex( k, idx ) ;
			}
		}
	}
}

void xsi_loader::load_shape_anim( CSLXSIGeometry *geom, xsi_arrays &arrays )
{
	CSLXSIShapeAnimation *anim = geom->XSIShapeAnimation() ;
	if ( ( 0xffff0000 & (int)anim ) == 0 ) return ;

	CSLXSIShape **shapes = anim->XSIShapes() ;
	for ( int i = 0 ; i < anim->GetXSIShapeCount() ; i ++ ) {
		arrays.load( shapes[ i ] ) ;
	}

	if ( anim->Animation() != 0 ) {
		m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
		xsi_fcurve fcurve ;
		fcurve.load( anim->Animation(), 1.0f ) ;
		fcurve.save( m_fcurve ) ;
		set_animation( MDX_BONE, m_bone->GetName(), MDX_MORPH_INDEX, 0, m_fcurve->GetName() ) ;
	}
}
#endif // AFTER_CROSSWALK_2_6

//----------------------------------------------------------------
//  load envelopes
//----------------------------------------------------------------

void xsi_loader::load_envelope_weights( CSLModel *model, xsi_arrays &arrays )
{
	int num = 0 ;
	CSLEnvelope **envs = model->GetEnvelopeList() ;
	for ( int i = 0 ; i < model->GetEnvelopeCount() ; i ++ ) {
		CSLEnvelope *env = envs[ i ] ;
		if ( env->GetVertexWeightCount() == 0 ) continue ;

		SLVertexWeight *weights = env->GetVertexWeightListPtr() ;
		for ( int j = 0 ; j < env->GetVertexWeightCount() ; j ++ ) {
			int vid = (int)( weights[ j ].m_fVertexIndex ) ;
			if ( m_nurbs_wrap ) {
				vid += vid / m_nurbs_width * m_nurbs_wrap ;
			}
			float w = weights[ j ].m_fWeight * 0.01f ;
			arrays.set_weight( vid, num, w ) ;
		}
		num ++ ;
	}
}

void xsi_loader::load_envelope_bones( CSLModel *model )
{
	CSLEnvelope **envs = model->GetEnvelopeList() ;
	for ( int i = 0 ; i < model->GetEnvelopeCount() ; i ++ ) {
		CSLEnvelope *env = envs[ i ] ;
		if ( env->GetVertexWeightCount() == 0 ) continue ;

		CSLModel *deformer = env->GetDeformer() ;	//  maybe instanced
		string name = get_node_name( deformer ) ;
		if ( m_instance_names.find( name ) != m_instance_names.end() ) {
			name = m_instance_names[ name ] ;
		}

		mat4 mat_d = 1.0f, mat_s = 1.0f ;
		if ( model->GetBasePose() != 0 ) {
			model->GetBasePose()->GetMatrix().Get( (float *)&mat_s ) ;
		}
		if ( deformer->GetBasePose() != 0 ) {
			deformer->GetBasePose()->GetMatrix().Get( (float *)&mat_d ) ;
		}
		#if ( AFTER_CROSSWALK_2_6 )
		if ( model->GetXSIBasePose() != 0 ) {
			model->GetXSIBasePose()->GetMatrix().Get( (float *)&mat_s ) ;
		}
		if ( deformer->GetXSIBasePose() != 0 ) {
			deformer->GetXSIBasePose()->GetMatrix().Get( (float *)&mat_d ) ;
		}
		#endif // AFTER_CROSSWALK_2_6

		m_bone->AttachChild( new MdxBlendBone( name.c_str(), inverse( mat_d ) * mat_s ) ) ;
	}
}

//----------------------------------------------------------------
//  load all materials
//----------------------------------------------------------------

void xsi_loader::load_materials( CSLMaterialLibrary *material_library )
{
	CSLBaseMaterial **materials = material_library->GetMaterialList() ;
	for ( int i = 0 ; i < material_library->GetMaterialCount() ; i ++ ) {
		CSLBaseMaterial *material = materials[ i ] ;
		switch( material->Type() ) {
		    case CSLTemplate::SI_MATERIAL :
			load_material( (CSLMaterial *)material ) ;
			break ;
		    case CSLTemplate::XSI_MATERIAL : {
			load_material( (CSLXSIMaterial *)material ) ;
			break ;
		    }
		    default :
			break ;
		}
	}
}

//----------------------------------------------------------------
//  load material
//----------------------------------------------------------------

void xsi_loader::load_material( CSLMaterial *material )
{
	string name = get_node_name( material ) ;
	if ( name == "" ) return ;	// mdxconv 1.47 and beyond
	m_model->AttachChild( m_material = new MdxMaterial( name.c_str() ) ) ;

	//  shading model

	bool do_lighting = true ;
	bool do_specular = true ;
	bool explicit_ambient = false ;
	switch ( material->GetShadingModel() ) {
	    case CSLMaterial::CONSTANT :
		do_lighting = false ;
		do_specular = false ;
		break ;
	    case CSLMaterial::LAMBERT :
		do_lighting = true ;
		do_specular = false ;
		break ;
	    case CSLMaterial::PHONG :
	    case CSLMaterial::BLINN :
		do_lighting = true ;
		do_specular = true ;
		break ;
	    case CSLMaterial::SHADOW_OBJECT :
		// (>_<)
		break ;
	    case CSLMaterial::VERTEX_COLOR :
		do_lighting = ( m_vcolor_lighting != MODE_OFF ) ;
		do_specular = ( m_vcolor_specular != MODE_OFF ) ;
		break ;
	}

	//  colors

	// (>_<) reflection refraction bump

	CSLTexture2D *diffuse_map = material->Texture2D() ;
	vec4 diffuse = 1.0f, specular = 0.0f, emission = 0.0f, ambient = 1.0f ;
	float shiny = 0.0f ;
	material->GetDiffuseColor().Get( &diffuse.x, &diffuse.y, &diffuse.z, &diffuse.w ) ;
	material->GetSpecularColor().Get( &specular.x, &specular.y, &specular.z ) ;
	material->GetEmissiveColor().Get( &emission.x, &emission.y, &emission.z ) ;
	material->GetAmbientColor().Get( &ambient.x, &ambient.y, &ambient.z ) ;
	shiny = material->GetSpecularDecay() * m_shiny_scale ;

	if ( !do_lighting ) {
		#if ( USE_ENABLE_LIGHTING )
		m_material->AttachChild( new MdxEnableLighting( 0 ) ) ;
		#endif // USE_ENABLE_LIGHTING
		if ( diffuse_map != 0 ) {
			if ( diffuse.x == 0.0f && diffuse.y == 0.0f && diffuse.z == 0.0f ) {
				diffuse.x = diffuse.y = diffuse.z = 1.0f ;
			}
		}
		ambient = diffuse ;
	}
	if ( !do_specular ) specular = 0.0f ;
	ambient.w = diffuse.w ;
	specular.w = 1.0f ;
	emission.w = 1.0f ;

	if ( (vec3)diffuse != 1.0f ) {
		m_material->AttachChild( new MdxDiffuse( diffuse ) ) ;
	}
	if ( specular != vec4( 0, 0, 0, 1 ) && shiny != 0.0f ) {
		m_material->AttachChild( new MdxSpecular( specular ) ) ;
	}
	if ( emission != vec4( 0, 0, 0, 1 ) ) {
		m_material->AttachChild( new MdxEmission( emission ) ) ;
	}
	if ( ambient != diffuse ) {
		m_material->AttachChild( new MdxAmbient( ambient ) ) ;
		explicit_ambient = true ;
	}

	if ( diffuse.w != 1.0f ) {
		m_material->AttachChild( new MdxOpacity( diffuse.w ) ) ;
	}
	if ( specular != vec4( 0, 0, 0, 1 ) && shiny != 0.0f ) {
		m_material->AttachChild( new MdxShininess( shiny ) ) ;
	}

	//  animation

	// (>_<) reflection animation
	// (>_<) refraction animation

	static int Diffuses[] = { CSLTemplate::SI_DIFFUSE_R, CSLTemplate::SI_DIFFUSE_G,
				CSLTemplate::SI_DIFFUSE_B } ;
	static int Ambients[] = { CSLTemplate::SI_AMBIENT_R, CSLTemplate::SI_AMBIENT_G,
				CSLTemplate::SI_AMBIENT_B } ;
	static int Speculars[] = { CSLTemplate::SI_SPECULAR_R, CSLTemplate::SI_SPECULAR_G,
				CSLTemplate::SI_SPECULAR_B } ;
	static int Emissions[] = { CSLTemplate::SI_EMISSIVE_R, CSLTemplate::SI_EMISSIVE_G,
				CSLTemplate::SI_EMISSIVE_B } ;
	static int Shininess[] = { CSLTemplate::SI_POWER } ;

	CSLFCurve *anims[ 4 ] ;
	if ( get_fcurves( anims, material, Diffuses, 3 ) ) {
		set_material_fcurves( name, MDX_DIFFUSE, diffuse, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Ambients, 3 ) ) {
		set_material_fcurves( name, MDX_AMBIENT, ambient, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Speculars, 3 ) ) {
		set_material_fcurves( name, MDX_SPECULAR, specular, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Emissions, 3 ) ) {
		set_material_fcurves( name, MDX_EMISSION, emission, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Shininess, 1 ) ) {
		set_material_fcurves( name, MDX_SHININESS, shiny, anims, 1 ) ;
	}
	// (>_<) template only
	CdotXSITemplate *anim = get_fcurve( material->Template(), "TRANSPARENCY-A" ) ;
	if ( anim != 0 ) {
		set_material_fcurves( name, MDX_OPACITY, diffuse.w, &anim, 1 ) ;
	}

	//  texture

	// (>_<) texture scroll
	// (>_<) multi-texture

	if ( diffuse_map != 0 ) {
		load_texture( diffuse_map ) ;
	}

	//  custom param

	load_custom_param( m_material, material ) ;
}

void xsi_loader::load_texture( CSLTexture2D *texture )
{
	string name = get_node_name( texture ) ;
	m_model->AttachChild( m_texture = new MdxTexture( name.c_str() ) ) ;
	m_texture->AttachChild( new MdxFileName( texture->GetImageFileName() ) );

	m_material->AttachChild( m_layer = new MdxLayer ) ;
	m_layer->AttachChild( new MdxSetTexture( m_texture->GetName() ) ) ;

	CSLTexture2D::EMappingType mapping = texture->GetMappingType() ;
	if ( mapping == CSLTexture2D::SI_REFLECTION_MAP ) {
		m_layer->AttachChild( new MdxMapType( MDX_REFLECTION ) ) ;
		m_material->AttachChild( new MdxReflection( 1.0f ) ) ;
	}

	//  crop

	// (>_<) alternate flag
	// (>_<) transform matrix

	rect crop( 0, 0, 1, 1 ) ;
	rect crop2( 0, 0, 1, 1 ) ;
	float n = texture->GetImageWidth() ;
	float a = texture->GetCropUMin() ;
	float b = texture->GetCropUMax() ;
	float o = texture->GetUOffset() ;
	float s = texture->GetUScale() ;
	float r = texture->GetURepeat() ;
	if ( s != 0.0f && n != 0.0f ) {
		crop.w = 1.0f / s * r * ( b - a + 1 ) / n ;
		crop.x = crop.w * ( -o ) + a / n ;
		crop2.w = r * ( b - a + 1 ) / n ;
		crop2.x = a / n ;
	}
	n = texture->GetImageHeight() ;
	a = texture->GetCropVMin() ;
	b = texture->GetCropVMax() ;
	o = texture->GetVOffset() ;
	s = texture->GetVScale() ;
	r = texture->GetVRepeat() ;
	if ( s != 0.0f && n != 0.0f ) {
		crop.h = 1.0f / s * r * ( b - a + 1 ) / n ;
		crop.y = crop.h * ( -o ) + ( n - b - 1 ) / n ;
		crop2.h = r * ( b - a + 1 ) / n ;
		crop2.y = ( n - b - 1 ) / n ;
	}
	MdxBlock *block = m_texture ;
	if ( crop.xy() != 0.0f ) block->AttachChild( new MdxUVTranslate( crop.xy() ) ) ;
	if ( crop.wh() != 1.0f ) block->AttachChild( new MdxUVScale( crop.wh() ) ) ;

	material_state &state = m_material_states[ m_material->GetName() ] ;
	if ( mapping != CSLTexture2D::SI_UV_MAP && mapping != CSLTexture2D::SI_UV_MAP_WRAPPED ) {
		state.flags |= MATERIAL_MAPPED_BY_PROJECTION ;
	}
	state.crop2 = crop2 ;
}

//----------------------------------------------------------------
//  load material ( dotXSI 3.5 and beyond )
//----------------------------------------------------------------

void xsi_loader::load_material( CSLXSIMaterial *material )
{
	string name = get_node_name( material ) ;
	m_model->AttachChild( m_material = new MdxMaterial( name.c_str() ) ) ;

	//  find shader ( "surface" or "RealTime" )

	CSLXSIShader *shader = 0 ;
	CSLConnectionPoint **connects = material->GetConnectionPointList() ;
	for ( int i = 0 ; i < material->GetConnectionPointCount() ; i ++ ) {
		CSLConnectionPoint *connect = connects[ i ] ;
		string name = connect->GetName() ;
		if ( ( name == "surface" && shader == 0 ) || name == "RealTime" ) {
			if ( connect->GetShader() != 0 ) shader = connect->GetShader() ;
		}
	}
	if ( shader == 0 ) return ;

	//  colors

	bool do_lighting = true ;
	bool explicit_ambient = false ;

	CSLXSIShader *diffuse_map = 0 ;
	vec4 diffuse = 1.0f, specular = 0.0f, emission = 0.0f, ambient = 1.0f ;
	vec4 trans = 0.0f, reflect = 0.0f, bump = 0.0f ;
	float shiny = 1.25f, refract = 1.0f ;
	float use_alpha_trans = 1.0f, use_alpha_refl = 1.0f ;

	if ( find_vector( shader, "color", diffuse, &diffuse_map ) ) {
		do_lighting = false ;
	} else {
		find_vector( shader, "diffuse", diffuse, &diffuse_map ) ;
	}
	find_vector( shader, "specular", specular ) ;
	find_scalar( shader, "shiny", shiny ) ;
	find_vector( shader, "incandescence", emission ) ;
	find_scalar( shader, "inc_inten", emission.w ) ;
	find_vector( shader, "ambient", ambient ) ;
	find_vector( shader, "transparency", trans ) ;
	find_vector( shader, "reflectivity", reflect ) ;
	find_scalar( shader, "index_of_refraction", refract ) ;
	find_vector( shader, "bump", bump ) ;
	find_scalar( shader, "usealphatrans", use_alpha_trans ) ;
	find_scalar( shader, "usealpharefl", use_alpha_refl ) ;
	shiny *= m_shiny_scale ;

	diffuse.w = 1.0f - trans.w ;
	if ( use_alpha_trans == 0.0f ) {
		diffuse.w = 1.0f - ( trans.x + trans.y + trans.z ) / 3.0f ;
	}
	if ( use_alpha_refl == 0.0f ) {
		reflect.w = ( reflect.x + reflect.y + reflect.z ) / 3.0f ;
	}
	ambient.w = diffuse.w ;
	specular.w = 1.0f ;
	emission *= emission.w ;
	emission.w = 1.0f ;

	if ( !do_lighting ) {
		#if ( USE_ENABLE_LIGHTING )
		m_material->AttachChild( new MdxEnableLighting( 0 ) ) ;
		#endif // USE_ENABLE_LIGHTING
		if ( diffuse_map != 0 ) {
			if ( diffuse.x == 0.0f && diffuse.y == 0.0f && diffuse.z == 0.0f ) {
				diffuse.x = diffuse.y = diffuse.z = 1.0f ;
			}
		}
		ambient = diffuse ;
	}
	if ( (vec3)diffuse != 1.0f ) {
		m_material->AttachChild( new MdxDiffuse( diffuse ) ) ;
	}
	if ( specular != vec4( 0, 0, 0, 1 ) && shiny != 0.0f ) {
		m_material->AttachChild( new MdxSpecular( specular ) ) ;
	}
	if ( emission != vec4( 0, 0, 0, 1 ) ) {
		m_material->AttachChild( new MdxEmission( emission ) ) ;
	}
	if ( ambient != diffuse ) {
		m_material->AttachChild( new MdxAmbient( ambient ) ) ;
		explicit_ambient = true ;
	}
	if ( reflect.w != 0.0f ) {
		m_material->AttachChild( new MdxReflection( reflect.w ) ) ;
	}
	if ( refract != 1.0f ) {
		m_material->AttachChild( new MdxRefraction( refract ) ) ;
	}
	if ( bump.z != 0.0f ) {
		m_material->AttachChild( new MdxBump( bump.z ) ) ;
	}
	if ( diffuse.w != 1.0f ) {
		m_material->AttachChild( new MdxOpacity( diffuse.w ) ) ;
	}
	if ( specular != vec4( 0, 0, 0, 1 ) && shiny != 0.0f ) {
		m_material->AttachChild( new MdxShininess( shiny ) ) ;
	}

	//  animation

	static const char *Ambients[] = { "ambient.red", "ambient.green", "ambient.blue" } ;
	static const char *Diffuses[] = { "diffuse.red", "diffuse.green", "diffuse.blue" } ;
	static const char *Colors[] = { "color.red", "color.green", "color.blue" } ;
	static const char *Speculars[] = { "specular.red", "specular.green", "specular.blue" } ;
	static const char *Emissions[] = { "incandescence.red", "incandescence.green", "incandescence.blue" } ;
	// (>_<) "inc_inten"
	static const char *Shininess[] = { "shiny" } ;

	CSLFCurve *anims[ 4 ] ;
	const char **channels = do_lighting ? Diffuses : Colors ;
	if ( get_fcurves( anims, material, channels, 3 ) ) {
		set_material_fcurves( name, MDX_DIFFUSE, diffuse, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Ambients, 3 ) ) {
		set_material_fcurves( name, MDX_AMBIENT, ambient, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Speculars, 3 ) ) {
		set_material_fcurves( name, MDX_SPECULAR, specular, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Emissions, 3 ) ) {
		set_material_fcurves( name, MDX_EMISSION, emission, anims, 3 ) ;
	}
	if ( get_fcurves( anims, material, Shininess, 1 ) ) {
		set_material_fcurves( name, MDX_SHININESS, shiny, anims, 1 ) ;
	}
	const char *channel = ( use_alpha_trans == 0.0f ) ? "transparency.red" : "transparency.alpha" ;
	anims[ 0 ] = get_fcurve( shader, channel ) ;
	if ( anims[ 0 ] != 0 ) {
		set_material_fcurves( name, MDX_OPACITY, diffuse.w, anims, 1 ) ;
	}

	//  texture

	// (>_<) texture scroll
	// (>_<) multi-texture

	if ( diffuse_map != 0 ) {
		load_texture( diffuse_map ) ;
	}

	//  custom param

	load_custom_param( m_material, material ) ;
}

void xsi_loader::load_texture( CSLXSIShader *texture )
{
	int i, j ;

	//  find image

	char *name = 0 ;
	CSLShaderConnectionPoint **connects = texture->GetConnectionPointList() ;
	for ( i = 0 ; i < texture->GetConnectionPointCount(); i ++ ) {
		if ( ( name = connects[ i ]->GetImage() ) != 0 ) break ;
	}
	CSLImageLibrary *images = texture->Scene()->GetImageLibrary() ;
	if ( images == 0 ) return ;
	CSLImage *image = images->FindImage( name ) ;
	if ( image == 0 ) return ;

	//  create texture

	m_texture = (MdxTexture *)m_model->FindChild( MDX_TEXTURE, name ) ;
	if ( m_texture == 0 ) {
		m_model->AttachChild( m_texture = new MdxTexture( name ) ) ;
		m_texture->AttachChild( new MdxFileName( image->GetSourceFile() ) ) ;
	}

	m_material->AttachChild( m_layer = new MdxLayer ) ;
	m_layer->AttachChild( new MdxSetTexture( m_texture->GetName() ) ) ;

	//  crop

	vec4 repeat, map_min, map_max ;
	find_vector( texture, "repeats", repeat ) ;
	find_vector( texture, "min", map_min ) ;
	find_vector( texture, "max", map_max ) ;

	rect crop( 0, 0, 1, 1 ) ;
	float a = image->GetCropMinX() ;
	float b = image->GetCropMaxX() ;
	if ( map_min.x != map_max.x ) {
		crop.w = 1.0f / ( map_max.x - map_min.x ) * repeat.x * ( b - a ) ;
		crop.x = crop.w * ( -map_min.x ) + a ;
	}
	a = 1.0f - image->GetCropMaxY() ;
	b = 1.0f - image->GetCropMinY() ;
	if ( map_min.y != map_max.y ) {
		crop.h = 1.0f / ( map_max.y - map_min.y ) * repeat.y * ( b - a ) ;
		crop.y = crop.h * ( -map_min.y ) + a ;
	}
	MdxBlock *block = m_texture ;
	m_texture->ClearChild( MDX_UV_TRANSLATE ) ;
	m_texture->ClearChild( MDX_UV_SCALE ) ;
	if ( crop.xy() != 0.0f ) block->AttachChild( new MdxUVTranslate( crop.xy() ) ) ;
	if ( crop.wh() != 1.0f ) block->AttachChild( new MdxUVScale( crop.wh() ) ) ;

	//  tspace_id

	const char *tspace = "" ;
	CSLVariantParameter **params = texture->GetParameterList() ;
	for ( i = 0 ; i < texture->GetParameterCount() ; i ++ ) {
		if ( strcmp( params[ i ]->GetName(), "tspace_id" ) == 0 ) {
			SI_TinyVariant *var = params[ i ]->GetValue() ;
			if ( var->variantType == SI_VT_PCHAR ) tspace = var->p_cVal ;
		}
	}
	material_state &state = m_material_states[ m_material->GetName() ] ;
	state.tspace = tspace ;

	//  tspace_id ( instance )

	CSLShaderInstanceData **insts = texture->GetInstanceDataList() ;
	for ( i = 0 ; i < texture->GetInstanceDataCount() ; i ++ ) {
		CSLShaderInstanceData *inst = insts[ i ] ;
		CSLModel *object = inst->GetReference() ;
		if ( object == 0 ) continue ;

		CSLVariantParameter **params = inst->GetParameterList() ;
		for ( j = 0 ; j < inst->GetParameterCount() ; j ++ ) {
			if ( strcmp( params[ j ]->GetName(), "tspace_id" ) == 0 ) {
				SI_TinyVariant *var = params[ j ]->GetValue() ;
				if ( var->variantType == SI_VT_PCHAR ) {
					state.tspaces[ object ] = var->p_cVal ;
				}
			}
		}
	}
}

bool xsi_loader::find_vector( CSLXSIShader *shader, const string &name, vec4 &value, CSLXSIShader **texture )
{
	// find vector parameter

	bool found = false ;
	found |= get_scalar( shader, name + ".red", value.x ) ;
	found |= get_scalar( shader, name + ".green", value.y ) ;
	found |= get_scalar( shader, name + ".blue", value.z ) ;
	found |= get_scalar( shader, name + ".alpha", value.w ) ;
	found |= get_scalar( shader, name + ".x", value.x ) ;
	found |= get_scalar( shader, name + ".y", value.y ) ;
	found |= get_scalar( shader, name + ".z", value.z ) ;

	CSLShaderConnectionPoint **connects = shader->GetConnectionPointList() ;
	for ( int i = 0 ; i < shader->GetConnectionPointCount(); i ++ ) {
		CSLShaderConnectionPoint *connect = connects[ i ] ;
		switch ( connect->GetConnectionType() ) {
		    case CSLShaderConnectionPoint::SI_SHADER :
			if ( strcmp( connect->GetName(), name.c_str() ) == 0 ) {
				found |= find_vector( connect->GetShader(), name, value, texture ) ;
			}
			break ;
		    case CSLShaderConnectionPoint::SI_IMAGE :
			if ( texture != 0 ) {
				*texture = shader ;
				found = true ;
			}
			break ;
		    case CSLShaderConnectionPoint::SI_NONE :
			break ;
		}
	}
	return found ;
}

bool xsi_loader::find_scalar( CSLXSIShader *shader, const string &name, float &value, CSLXSIShader **texture )
{
	bool found = get_scalar( shader, name, value ) ;

	CSLShaderConnectionPoint **connects = shader->GetConnectionPointList() ;
	for ( int i = 0 ; i < shader->GetConnectionPointCount(); i ++ ) {
		CSLShaderConnectionPoint *connect = connects[ i ] ;
		switch ( connect->GetConnectionType() ) {
		    case CSLShaderConnectionPoint::SI_SHADER :
			if ( strcmp( connect->GetName(), name.c_str() ) == 0 ) {
				found |= find_scalar( connect->GetShader(), name, value, texture ) ;
			}
			break ;
		    case CSLShaderConnectionPoint::SI_IMAGE :
			if ( texture != 0 ) *texture = shader ;
			break ;
		    case CSLShaderConnectionPoint::SI_NONE :
			break ;
		}
	}
	return found ;
}

bool xsi_loader::get_scalar( CSLXSIShader *shader, const string &name, float &value )
{
	#if ( DUPLICATIVE_SHADER_PARAM == 0 )
	CSLAnimatableType *param = shader->ParameterFromName( (char *)name.c_str() ) ;
	if ( param == 0 ) return false ;
	value = param->GetFloatValue() ;
	return true ;
	#else // DUPLICATIVE_SHADER_PARAM
	CSLVariantParameter **params = shader->GetParameterList() ;
	int n_params = shader->GetParameterCount() ;
	bool found = false ;
	for ( int i = 0 ; i < n_params ; i ++ ) {
		if ( name != params[ i ]->GetName() ) continue ;
		value = params[ i ]->GetFloatValue() ;
		found = true ;
	}
	return found ;
	#endif
}

//----------------------------------------------------------------
//  load custom param
//----------------------------------------------------------------

#if ( USE_CUSTOM_PARAMETERS == 0 )

bool xsi_loader::load_custom_param( MdxBlock *block, CSLTemplate *tmp )
{
	return true ;
}

#else // USE_CUSTOM_PARAMETERS

bool xsi_loader::load_custom_param( MdxBlock *block, CSLTemplate *tmp )
{
	if ( m_check_custom_param_mode == xsi_loader::MODE_OFF ) return false ;

	CSLCustomPSet **cps = tmp->GetCustomPSetList() ;
	for ( int i = 0 ; i < tmp->GetCustomPSetCount() ; i ++ ) {
		CSLCustomPSet *cp = cps[ i ] ;
		CSLVariantParameter **vps = cp->GetParameterList() ;
		for ( int j = 0 ; j < cp->GetParameterCount() ; j ++ ) {
			CSLVariantParameter *vp = vps[ j ] ;
			SI_TinyVariant *var = vp->GetValue() ;
			if ( var->variantType != SI_VT_PCHAR ) continue ;

			const string &prefix = m_check_custom_param_prefix ;
			if ( strnicmp( vp->GetName(), prefix.c_str(), prefix.length() ) != 0 ) continue ;

			const char *cur_ptr = var->p_cVal ;
			const char *end_ptr = cur_ptr + strlen( cur_ptr ) + 1 ;
			for ( const char *cp = cur_ptr ; cp < end_ptr ; cp ++ ) {
				if ( cp[ 0 ] != '\0' ) {
					if ( cp[ 0 ] != '\\' ) continue ;
					if ( cp[ 1 ] != 'r' && cp[ 1 ] != 'n' ) continue ;
				}
				string line = str_trim( string( cur_ptr, cp - cur_ptr ) ) ;
				cur_ptr = cp + 2 ;
				if ( line == "" ) continue ;
				if ( load_transform_param( block, line ) ) continue ;
				if ( load_material_param( block, line ) ) continue ;
				if ( load_userdata_param( block, line ) ) continue ;
			}
		}
	}
	return true ;
}

bool xsi_loader::load_transform_param( MdxBlock *block, const string &line )
{
	if ( block->GetTypeID() != MDX_BONE ) return false ;
	if ( m_custom_param_transform_mode == xsi_loader::MODE_OFF ) return false ;
	const string &prefix = m_custom_param_transform_prefix ;
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

bool xsi_loader::load_material_param( MdxBlock *block, const string &line )
{
	if ( block->GetTypeID() != MDX_MATERIAL ) return false ;
	if ( m_custom_param_material_mode == xsi_loader::MODE_OFF ) return false ;
	const string &prefix = m_custom_param_material_prefix ;
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

bool xsi_loader::load_userdata_param( MdxBlock *block, const string &line )
{
	if ( m_custom_param_userdata_mode == xsi_loader::MODE_OFF ) return false ;
	const string &prefix = m_custom_param_userdata_prefix ;
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

string xsi_loader::sanitize( const string &str )
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

#endif // USE_CUSTOM_PARAMETERS

//----------------------------------------------------------------
//  sub routine ( fixup "TexCrop" )
//----------------------------------------------------------------

void xsi_loader::ignore_textrans()
{
	if ( m_ignore_textrans == MODE_OFF ) return ;

	MdxChunks materials ;
	materials.EnumChild( m_model, MDX_MATERIAL ) ;
	for ( int i = 0 ; i < (int)materials.size() ; i ++ ) {
		MdxMaterial *material = (MdxMaterial *)materials[ i ] ;

		material_state &state = m_material_states[ material->GetName() ] ;
		int flags = state.flags ;
		if ( m_ignore_textrans == MODE_AUTO ) {
			if ( !( MATERIAL_MAPPED_BY_PROJECTION & flags ) ) continue ;
			if ( !( MATERIAL_APPLIED_TO_POLYGON & flags ) ) continue ;
		}

		MdxChunks layers ;
		layers.EnumChild( material, MDX_MATERIAL ) ;
		for ( int j = 0 ; j < (int)layers.size() ; j ++ ) {
			MdxLayer *layer = (MdxLayer *)layers[ j ] ;

			MdxBlock *block = layer ;
			MdxSetTexture *cmd = (MdxSetTexture *)layer->FindChild( MDX_SET_TEXTURE ) ;
			if ( cmd == 0 ) continue ;
			block = cmd->GetTextureRef() ;
			if ( block == 0 ) continue ;

			block->ClearChild( MDX_UV_TRANSLATE ) ;
			block->ClearChild( MDX_UV_SCALE ) ;
			if ( state.crop2.xy() != 0.0f ) {
				block->AttachChild( new MdxUVTranslate( state.crop2.xy() ) ) ;
			}
			if ( state.crop2.wh() != 1.0f ) {
				block->AttachChild( new MdxUVScale( state.crop2.wh() ) ) ;
			}
		}
	}
}

//----------------------------------------------------------------
//  sub routine ( get node name )
//----------------------------------------------------------------

string xsi_loader::get_node_name( CSLTemplate *tmp )
{
	string name = tmp->GetName() ;
	if ( m_ignore_prefix != MODE_OFF ) {
		string::size_type dot = name.find_first_of( '.' ) ;
		if ( dot != string::npos ) name = name.substr( dot + 1 ) ;
	}
	return name ;
}

//----------------------------------------------------------------
//  sub routine ( set animation )
//----------------------------------------------------------------

void xsi_loader::set_animation( int scope, const string &block, int command, int mode, const string &fcurve )
{
	MdxAnimate *cmd = new MdxAnimate ;
	m_motion->AttachChild( cmd ) ;
	cmd->SetScope( scope ) ;
	cmd->SetBlock( block.c_str() ) ;
	cmd->SetCommand( command ) ;
	cmd->SetMode( mode ) ;
	cmd->SetFCurve( fcurve.c_str() ) ;
}

bool xsi_loader::copy_animation( int scope, const string &block, int command, int mode, const string &inst )
{
	if ( inst == block ) return false ;

	//  reuse original fcurve

	for ( int i = 0 ; i < m_motion->GetChildCount() ; i ++ ) {
		MdxAnimate *cmd = (MdxAnimate *)m_motion->GetChild( i ) ;
		if ( cmd->GetTypeID() != MDX_ANIMATE ) continue ;
		if ( cmd->GetScope() != scope ) continue ;
		if ( block != cmd->GetBlock() ) continue ;
		if ( cmd->GetCommand() != command ) continue ;
		if ( cmd->GetMode() != mode ) continue ;
		set_animation( scope, inst, command, mode, cmd->GetFCurve() ) ;
		return true ;
	}
	return false ;
}

//----------------------------------------------------------------
//  sub routine ( set transform fcurves )
//----------------------------------------------------------------

void xsi_loader::set_transform_fcurves( const string &name, const string &inst, int type, const vec3 &v, CSLTemplate *tmp, int channel )
{
	CSLFCurve *anims[ 3 ] ;
	int channels[ 3 ] = { channel + 0, channel + 1, channel + 2 } ;
	if ( get_fcurves( anims, tmp, channels, 3 ) ) set_transform_fcurves( name, inst, type, v, anims ) ;
}

void xsi_loader::set_transform_fcurves( const string &name, const string &inst, int type, const vec3 &v, CSLTemplate *tmp, const char **channels )
{
	CSLFCurve *anims[ 3 ] ;
	if ( get_fcurves( anims, tmp, channels, 3 ) ) set_transform_fcurves( name, inst, type, v, anims ) ;
}

void xsi_loader::set_transform_fcurves( const string &name, const string &inst, int type, const vec3 &v, CSLFCurve **anims )
{
	if ( copy_animation( MDX_BONE, name, type, 0, inst ) ) return ;

	m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
	xsi_fcurve fcurve ;
	fcurve.load( anims[ 0 ], v.x ) ;
	fcurve.load( anims[ 1 ], v.y ) ;
	fcurve.load( anims[ 2 ], v.z ) ;
	fcurve.save( m_fcurve ) ;
	set_animation( MDX_BONE, inst, type, 0, m_fcurve->GetName() ) ;
}

//----------------------------------------------------------------
//  sub routine ( set material fcurves )
//----------------------------------------------------------------

void xsi_loader::set_material_fcurves( const string &name, int type, const vec4 &v, CSLFCurve **anims, int n_anims )
{
	m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
	xsi_fcurve fcurve ;
	if ( n_anims >= 1 ) fcurve.load( anims[ 0 ], v.x ) ;
	if ( n_anims >= 2 ) fcurve.load( anims[ 1 ], v.y ) ;
	if ( n_anims >= 3 ) fcurve.load( anims[ 2 ], v.z ) ;
	fcurve.save( m_fcurve ) ;
	if ( type == MDX_OPACITY ) adjust_transparency_fcurve( m_fcurve ) ;
	set_animation( MDX_MATERIAL, name, type, 0, m_fcurve->GetName() ) ;
}

void xsi_loader::set_material_fcurves( const string &name, int type, const vec4 &v, CdotXSITemplate **anims, int n_anims )
{
	m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
	xsi_fcurve2 fcurve ;
	if ( n_anims >= 1 ) fcurve.load( anims[ 0 ], v.x ) ;
	if ( n_anims >= 2 ) fcurve.load( anims[ 1 ], v.y ) ;
	if ( n_anims >= 3 ) fcurve.load( anims[ 2 ], v.z ) ;
	fcurve.save( m_fcurve ) ;
	if ( type == MDX_OPACITY ) adjust_transparency_fcurve( m_fcurve ) ;
	set_animation( MDX_MATERIAL, name, type, 0, m_fcurve->GetName() ) ;
}

void xsi_loader::adjust_transparency_fcurve( MdxFCurve *fcurve )
{
	//  convert transparency to opacity

	int n_keys = fcurve->GetKeyFrameCount() ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = fcurve->GetKeyFrame( i ) ;
		key.SetValue( 0, 1.0f - key.GetValue( 0 ) ) ;
		key.SetInDY( 0, -key.GetInDY( 0 ) ) ;
		key.SetOutDY( 0, -key.GetOutDY( 0 ) ) ;
	}
}

//----------------------------------------------------------------
//  sub routine ( getting fcurves )
//----------------------------------------------------------------

bool xsi_loader::get_fcurves( CSLFCurve **anims, CSLTemplate *tmp, int *channels, int n_channels )
{
	bool exists = false ;
	for ( int i = 0 ; i < n_channels ; i ++ ) {
		int c = channels[ i ] ;
		anims[ i ] = tmp->GetSpecificFCurve( (CSLTemplate::EFCurveType)c ) ;
		if ( anims[ i ] != 0 ) exists = true ;
	}
	return exists ;
}

bool xsi_loader::get_fcurves( CSLFCurve **anims, CSLTemplate *tmp, const char **channels, int n_channels )
{
	bool exists = false ;
	for ( int i = 0 ; i < n_channels ; i ++ ) {
		anims[ i ] = get_fcurve( tmp, channels[ i ] ) ;
		if ( anims[ i ] != 0 ) exists = true ;
	}
	return exists ;
}

bool xsi_loader::get_fcurves( CdotXSITemplate **anims, CSLTemplate *tmp, const char **channels, int n_channels )
{
	bool exists = false ;
	for ( int i = 0 ; i < n_channels ; i ++ ) {
		anims[ i ] = get_fcurve( tmp->Template(), channels[ i ] ) ;
		if ( anims[ i ] != 0 ) exists = true ;
	}
	return exists ;
}

CSLFCurve *xsi_loader::get_fcurve( CSLTemplate *tmp, const char *channel )
{
	for ( int i = 0 ; i < tmp->GetFCurveCount() ; i ++ ) {
		CSLFCurve *fcurve = tmp->FCurves()[ i ] ;
		const SI_Char *channel2 = fcurve->GetFCurveTypeAsString() ;
		if ( channel2 != 0 && strcmp( channel, channel2 ) == 0 ) return fcurve ;
	}
	return 0 ;
}

CdotXSITemplate *xsi_loader::get_fcurve( CdotXSITemplate *temp, const string &name )
{
	if ( temp == 0 ) return 0 ;
	CdotXSITemplates &children = temp->Children() ;
	for ( int i = 0 ; i < children.GetCount() ; i ++ ) {
		CdotXSITemplate *anim ;
		children.Item( i, &anim ) ;
		if ( strcmp( anim->Name().GetText(), "SI_FCurve" ) == 0 ) {
			if ( get_string( anim, 1 ) == name ) return anim ;
		}
	}
	return 0 ;
}

//----------------------------------------------------------------
//  sub routine ( dotXSI template )
//----------------------------------------------------------------

int xsi_loader::get_int( CdotXSITemplate *temp, int num, int idx )
{
	if ( temp == 0 ) return 0 ;
	CdotXSIParams &params = temp->Params() ;
	if ( num < 0 || num >= params.GetCount() ) return 0 ;
	CdotXSIParam *param ;
	SI_TinyVariant value ;
	params.Item( num, &param ) ;
	param->GetValue( &value ) ;
	return value.nVal ;
}

float xsi_loader::get_float( CdotXSITemplate *temp, int num, int idx )
{
	if ( temp == 0 ) return 0.0f ;
	CdotXSIParams &params = temp->Params() ;
	if ( num < 0 || num >= params.GetCount() ) return 0.0f ;
	CdotXSIParam *param ;
	SI_TinyVariant value ;
	params.Item( num, &param ) ;
	param->GetValue( &value ) ;
	if ( value.variantType == SI_VT_PFLOAT && value.p_fVal != 0 ) {
		if ( idx >= 0 && idx < value.numElems ) return value.p_fVal[ idx ] ;
	}
	return value.fVal ;
}

string xsi_loader::get_string( CdotXSITemplate *temp, int num, int idx )
{
	if ( temp == 0 ) return "" ;
	CdotXSIParams &params = temp->Params() ;
	if ( num < 0 || num >= params.GetCount() ) return "" ;
	CdotXSIParam *param ;
	SI_TinyVariant value ;
	params.Item( num, &param ) ;
	param->GetValue( &value ) ;
	return ( value.variantType != SI_VT_PCHAR ) ? "" : value.p_cVal ;
}

//----------------------------------------------------------------
//  sub routine ( dotXSI 5.0 and beyond )
//----------------------------------------------------------------

#if ( AFTER_CROSSWALK_2_6 )
SI_Int *get_indices( CSLXSITriangleList *prim, xsi_arrays &arrays, int type )
{
	if ( type == 0 ) {
		CSLXSITriangleList::CSLIntArray *vp = prim->GetVertexIndices() ;
		return ( vp == 0 ) ? 0 : vp->ArrayPtr() ;
	}
	return get_attr_indices( prim, arrays, type ) ;
}

SI_Int *get_indices( CSLXSITriangleStripList *prim, xsi_arrays &arrays, int type )
{
	if ( type == -1 ) {
		CSLXSITriangleStripList::CSLIntArray *cp = prim->GetTriangleStripNodeCountArray() ;
		return ( cp == 0 ) ? 0 : cp->ArrayPtr() ;
	}
	if ( type == 0 ) {
		CSLXSITriangleStripList::CSLIntArray *vp = prim->GetVertexIndices() ;
		return ( vp == 0 ) ? 0 : vp->ArrayPtr() ;
	}
	return get_attr_indices( prim, arrays, type ) ;
}

SI_Int *get_indices( CSLXSIPolygonList *prim, xsi_arrays &arrays, int type )
{
	if ( type == -1 ) {
		CSLXSIPolygonList::CSLIntArray *cp = prim->GetPolygonNodeCountArray() ;
		return ( cp == 0 ) ? 0 : cp->ArrayPtr() ;
	}
	if ( type == 0 ) {
		CSLXSIPolygonList::CSLIntArray *vp = prim->GetVertexIndices() ;
		return ( vp == 0 ) ? 0 : vp->ArrayPtr() ;
	}
	return get_attr_indices( prim, arrays, type ) ;
}

SI_Int *get_attr_indices( CSLXSISubComponentList *prim, xsi_arrays &arrays, int type )
{
	CSLXSISubComponentList::CSLStringArray *names = prim->GetAttributeNameArray() ;
	for ( int i = 0 ; i < names->GetSize() ; i ++ ) {
		if ( arrays.get_attr_type( (*names)[ i ] ) == type ) {
			CSLXSISubComponentList::CSLIntArray *ap = prim->GetAttributeIndices( i ) ;
			return ( ap == 0 ) ? 0 : ap->ArrayPtr() ;
		}
	}
	return 0 ;
}

#endif // AFTER_CROSSWALK_2_6

//----------------------------------------------------------------
//  sub routine ( check knot type )
//----------------------------------------------------------------

int knot_type( SI_Float *knots, int n_knots, int degree )
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
	SI_Float *knots2 = knots + n_knots - degree ;
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
//  callback ( XSI FTK warning )
//----------------------------------------------------------------

SI_Void on_warning( char *mesg, int level )
{
	// if ( g_shell != 0 ) g_shell->Warning( mesg ) ;
	if ( g_shell != 0 ) g_shell->DebugPrint( 1, mesg ) ;
}

