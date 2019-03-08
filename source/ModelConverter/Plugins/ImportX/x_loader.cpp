#include "x_loader.h"
#include <rmxftmpl.h>
#include <rmxfguid.h>

#pragma warning( disable:4018 )

#define TRY_LEGACY_LOADER (1)
#if ( TRY_LEGACY_LOADER )
#include "x_loader2.h"
#endif // TRY_LEGACY_LOADER

//----------------------------------------------------------------
//  x_loader
//----------------------------------------------------------------

x_loader::x_loader( MdxShell *shell )
{
	m_shell = shell ;
}

x_loader::~x_loader()
{
	;
}

//----------------------------------------------------------------
//  load file
//----------------------------------------------------------------

bool x_loader::load( MdxBlock *&block, const string &filename )
{
	#if ( TRY_LEGACY_LOADER )
	x_loader2 loader2( m_shell ) ;
	if ( loader2.load( block, filename ) ) return true ;
	#endif // TRY_LEGACY_LOADER

	block = 0 ;

	//  reset

	m_filename = filename ;

	m_empty_material = 0 ;
	m_textures.clear() ;
	m_matrix_targets.clear() ;

	m_lock_xdata = 0 ;
	m_xdata_addr = 0 ;
	m_xdata_size = 0 ;

	//  open scene

	ID3DXFile *xfile = 0 ;
	D3DXFileCreate( &xfile );
	if ( xfile == 0 ) {
		m_shell->Error( "unable to create the x-file interface" ) ;
		return false ;
	}
	xfile->RegisterTemplates( (void *)D3DRM_XTEMPLATES, D3DRM_XTEMPLATE_BYTES ) ;
	xfile->RegisterTemplates( (void *)XSKINEXP_TEMPLATES, strlen( XSKINEXP_TEMPLATES ) ) ;
	xfile->RegisterTemplates( (void *)XEXTENSIONS_TEMPLATES, strlen( XEXTENSIONS_TEMPLATES ) ) ;

	ID3DXFileEnumObject *xenum = 0 ;
	xfile->CreateEnumObject( (LPVOID)filename.c_str(), D3DXF_FILELOAD_FROMFILE, &xenum ) ;
	if ( xenum == 0 ) {
		m_shell->Error( "load failed \"%s\"\n", m_filename.c_str() ) ;
		xfile->Release() ;
		return false ;
	}

	//  load

	m_file = new MdxFile ;
	string name = str_tolower( str_rootname( str_tailname( filename ) ) ) ;
	m_file->AttachChild( m_model = new MdxModel( name.c_str() ) ) ;

	SIZE_T n_child = 0 ;
	xenum->GetChildren( &n_child ) ;
	for ( int i = 0 ; i < n_child ; i ++ ) {
		ID3DXFileData *child = 0 ;
		xenum->GetChild( i, &child ) ;
		GUID type ;
		child->GetType( &type ) ;
		if ( type == TID_D3DRMFrame ) load_frame( child ) ;
		if ( type == TID_D3DRMMesh ) load_mesh( child ) ;
		if ( type == TID_D3DRMAnimationSet ) load_anim_set( child ) ;
	}

	//  close scene

	xenum->Release() ;
	xfile->Release() ;

	block = m_file ;
	return true ;
}

//----------------------------------------------------------------
//  load geometries
//----------------------------------------------------------------

void x_loader::load_frame( ID3DXFileData *frame, ID3DXFileData *parent )
{
	if ( frame == 0 ) return ;

	m_model->AttachChild( m_bone = new MdxBone( get_name( frame ).c_str() ) ) ;
	if ( parent != 0 ) m_bone->AttachChild( new MdxParentBone( get_name( parent ).c_str() ) ) ;

	load_xform( find_child( frame, TID_D3DRMFrameTransformMatrix ) ) ;
	load_mesh( enum_child( frame, TID_D3DRMMesh ), frame ) ;
	load_frame( enum_child( frame, TID_D3DRMFrame ), frame ) ;
}

void x_loader::load_mesh( ID3DXFileData *mesh, ID3DXFileData *parent )
{
	if ( mesh == 0 ) return ;

	m_vert_faces.clear() ;
	m_norm_faces.clear() ;
	m_normals.clear() ;
	m_mat_idxs.clear() ;
	m_mat_ptrs.clear() ;

	if ( parent == 0 ) m_model->AttachChild( m_bone = new MdxBone ) ;
	m_model->AttachChild( m_part = new MdxPart( get_name( mesh ).c_str() ) ) ;
	m_bone->AttachChild( new MdxDrawPart( m_part->GetName() ) ) ;
	m_part->AttachChild( m_arrays = new MdxArrays ) ;

	load_vertices( mesh ) ;
	load_normals( find_child( mesh, TID_D3DRMMeshNormals ) ) ;
	load_tcoords( find_child( mesh, TID_D3DRMMeshTextureCoords ) ) ;
	load_vcolors( find_child( mesh, TID_D3DRMMeshVertexColors ) ) ;
	load_weights( enum_child( mesh, DXFILEOBJ_SkinWeights ) ) ;
	load_mat_list( find_child( mesh, TID_D3DRMMeshMaterialList ) ) ;

	save_normals() ;
	save_meshes() ;
}

void x_loader::load_xform( ID3DXFileData *xform )
{
	if ( xform == 0 ) return ;

	mat4 m = get_mat4( xform ) ;
	vec4 t, s ;
	quat r ;
	decompose( flip_z( m ), t, r, s ) ;
	if ( !equals3( t, 0.0f ) ) m_bone->AttachChild( new MdxTranslate( t ) ) ;
	if ( !equals( r, 1.0f ) ) m_bone->AttachChild( new MdxRotate( r ) ) ;
	if ( !equals3( s, 1.0f ) ) m_bone->AttachChild( new MdxScale( s ) ) ;
}

void x_loader::load_vertices( ID3DXFileData *mesh )
{
	if ( mesh == 0 ) return ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_POSITION ) ;
	int n_verts = get_int( mesh ) ;
	m_arrays->SetVertexCount( n_verts ) ;
	for ( int i = 0 ; i < n_verts ; i ++ ) {
		m_arrays->GetVertex( i ).SetPosition( flip_z( get_vec3( mesh ) ) ) ;
	}
	int n_faces = get_int( mesh ) ;
	for ( int i = 0 ; i < n_faces ; i ++ ) {
		int n_idxs = get_int( mesh ) ;
		m_vert_faces.push_back( get_int( mesh, n_idxs ) ) ;
	}
}

void x_loader::load_normals( ID3DXFileData *normals )
{
	if ( normals == 0 ) return ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_NORMAL ) ;
	int n_normals = get_int( normals ) ;
	for ( int i = 0 ; i < n_normals ; i ++ ) {
		m_normals.push_back( flip_z( get_vec3( normals ) ) ) ;
	}
	int n_faces = get_int( normals ) ;
	for ( int i = 0 ; i < n_faces ; i ++ ) {
		int n_idxs = get_int( normals ) ;
		m_norm_faces.push_back( get_int( normals, n_idxs ) ) ;
	}
}

void x_loader::load_tcoords( ID3DXFileData *tcoords )
{
	if ( tcoords == 0 ) return ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_TEXCOORD ) ;
	int n_tcoords = get_int( tcoords ) ;
	for ( int i = 0 ; i < n_tcoords ; i ++ ) {
		m_arrays->GetVertex( i ).SetTexCoord( 0, get_vec2( tcoords ) ) ;
	}
}

void x_loader::load_vcolors( ID3DXFileData *vcolors )
{
	if ( vcolors == 0 ) return ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_COLOR ) ;
	int n_vcolors = get_int( vcolors ) ;
	for ( int i = 0 ; i < n_vcolors ; i ++ ) {
		int idx = get_int( vcolors ) ;
		rgba8888 col = get_vec4( vcolors ) ;
		m_arrays->GetVertex( idx ).SetColor( col ) ;
	}
}

void x_loader::load_weights( ID3DXFileData *weights )
{
	if ( weights == 0 ) return ;

	string bone = get_string( weights ) ;
	int n_weights = get_int( weights ) ;
	vector<int> idxs = get_int( weights, n_weights ) ;
	vector<float> wgts = get_float( weights, n_weights ) ;
	mat4 offset = flip_z( get_mat4( weights ) ) ;

	int num = m_arrays->GetVertexWeightCount() ;
	m_arrays->SetVertexWeightCount( num + 1 ) ;
	for ( int i = 0 ; i < n_weights ; i ++ ) {
		m_arrays->GetVertex( idxs[ i ] ).SetWeight( num, wgts[ i ] ) ;
	}
	m_bone->AttachChild( new MdxBlendBone( bone.c_str(), offset ) ) ;
}

void x_loader::load_mat_list( ID3DXFileData *mat_list )
{
	if ( mat_list == 0 ) return ;

	int n_materials = get_int( mat_list ) ;
	int n_mat_idxs = get_int( mat_list ) ;
	m_mat_idxs = get_int( mat_list, n_mat_idxs ) ;

	load_material( enum_child( mat_list, TID_D3DRMMaterial ) ) ;
}

void x_loader::load_material( ID3DXFileData *material )
{
	if ( material == 0 ) return ;

	m_material = new MdxMaterial ;
	vec4 diffuse = get_vec4( material ) ;
	float shininess = get_float( material ) ;
	vec3 specular = get_vec3( material ) ;
	vec3 emissive = get_vec3( material ) ;

	if ( diffuse.w == 0.0f ) diffuse.w = 1.0f ;		// (>_<) heuristic opacity
	if ( specular == 0.0f ) shininess = 0.0f ;		// (>_<) heuristic shininess
	if ( shininess == 0.0f ) specular = 0.0f ;		// (>_<) heuristic specular
	if ( !equals3( diffuse, 1.0f ) ) m_material->AttachChild( new MdxDiffuse( diffuse ) ) ;
	if ( !equals3( specular, 0.0f ) ) m_material->AttachChild( new MdxSpecular( specular ) ) ;
	if ( !equals3( emissive, 0.0f ) ) m_material->AttachChild( new MdxEmission( emissive ) ) ;
	if ( diffuse.w != 1.0f ) m_material->AttachChild( new MdxOpacity( diffuse.w ) ) ;
	if ( shininess != 0.0f ) m_material->AttachChild( new MdxShininess( shininess ) ) ;

	ID3DXFileData *texture = find_child( material, TID_D3DRMTextureFilename ) ;
	if ( texture != 0 ) {
		string filename = get_string( texture ) ;
		m_texture = m_textures[ filename ] ;
		if ( m_texture == 0 ) {
			m_model->AttachChild( m_texture = new MdxTexture ) ;
			m_texture->AttachChild( new MdxFileName( filename.c_str() ) ) ;
			m_textures[ filename ] = m_texture ;
		}
		m_material->AttachChild( m_layer = new MdxLayer ) ;
		m_layer->AttachChild( new MdxSetTexture( m_texture->GetName() ) ) ;
	}

	MdxMaterial *exists = find_material( m_material ) ;
	if ( exists != 0 ) {
		m_material->Release() ;
		m_material = exists ;
	}
	m_model->AttachChild( m_material ) ;
	m_mat_ptrs.push_back( m_material ) ;
}

MdxMaterial *x_loader::find_material( MdxMaterial *material )
{
	if ( material == 0 ) return 0 ;

	MdxBlocks materials ;
	materials.EnumChild( m_model, MDX_MATERIAL ) ;
	for ( int i = 0 ; i < materials.size() ; i ++ ) {
		MdxMaterial *material2 = (MdxMaterial *)materials[ i ] ;
		if ( material2 == material ) continue ;
		if ( material2->EqualsTree( material ) ) return material2 ;
	}
	return 0 ;
}

void x_loader::save_normals()
{
	int i, j, k ;

	//  compare indices

	if ( m_vert_faces.empty() || m_norm_faces.empty() ) return ;
	if ( m_vert_faces == m_norm_faces ) {
		int count = m_normals.size() ;
		if ( count >= m_arrays->GetVertexCount() ) count = m_arrays->GetVertexCount() ;
		for ( i = 0 ; i < count ; i ++ ) {
			m_arrays->GetVertex( i ).SetNormal( m_normals[ i ] ) ;
		}
		return ;
	}

	//  combine indices

	vector<int> vids, nids ;
	int n_verts = m_arrays->GetVertexCount() ;
	for ( i = 0 ; i < n_verts ; i ++ ) {
		vids.push_back( i ) ;
		nids.push_back( -1 ) ;
	}

	int n_verts2 = n_verts ;
	for ( i = 0 ; i < m_vert_faces.size() ; i ++ ) {
		vector<int> &vert_face = m_vert_faces[ i ] ;
		vector<int> &norm_face = m_norm_faces[ i ] ;
		for ( j = 0 ; j < vert_face.size() ; j ++ ) {
			int vid = vert_face[ j ] ;
			int nid = norm_face[ j ] ;
			if ( nids[ vid ] == nid || nids[ vid ] < 0 ) {
				nids[ vid ] = nid ;
				continue ;
			}
			for ( k = n_verts ; k < n_verts2 ; k ++ ) {
				if ( vids[ k ] == vid && nids[ k ] == nid ) break ;
			}
			if ( k == n_verts2 ) {
				vids.push_back( vid ) ;
				nids.push_back( nid ) ;
				n_verts2 ++ ;
			}
			vert_face[ j ] = k ;
		}
	}

	//  output normals

	m_arrays->SetVertexCount( n_verts2 ) ;
	for ( i = n_verts ; i < n_verts2 ; i ++ ) {
		m_arrays->SetVertex( i, m_arrays->GetVertex( vids[ i ] ) ) ;
	}
	for ( i = 0 ; i < n_verts2 ; i ++ ) {
		if ( nids[ i ] < 0 || nids[ i ] >= m_normals.size() ) continue ;
		m_arrays->GetVertex( i ).SetNormal( m_normals[ nids[ i ] ] ) ;
	}
}

void x_loader::save_meshes()
{
	m_mesh = 0 ;
	int mat_idx = -1 ;
	int face_idx = 0 ;
	int n_verts = 0 ;

	for ( int i = 0 ; i < m_vert_faces.size() ; i ++ ) {

		//  output mesh

		if ( i < m_mat_idxs.size() ) {
			int idx = m_mat_idxs[ i ] ;
			if ( idx != mat_idx ) {
				save_prims( face_idx, i ) ;
				face_idx = i ;
				mat_idx = idx ;
				m_mesh = 0 ;
			}
		}
		if ( m_mesh == 0 ) {
			m_part->AttachChild( m_mesh = new MdxMesh ) ;
			if ( mat_idx >= 0 && mat_idx < m_mat_ptrs.size() ) {
				m_material = m_mat_ptrs[ mat_idx ] ;
			} else {
				if ( m_empty_material == 0 ) {
					m_empty_material = new MdxMaterial ;
					m_model->AttachChild( m_empty_material ) ;
				}
				m_material = m_empty_material ;
			}
			m_mesh->AttachChild( new MdxSetMaterial( m_material->GetName() ) ) ;
			m_mesh->AttachChild( new MdxSetArrays( m_arrays->GetName() ) ) ;
		}

		//  output prims

		int n_verts2 = m_vert_faces[ i ].size() ;
		if ( n_verts != n_verts2 ) {
			n_verts = n_verts2 ;
			save_prims( face_idx, i ) ;
			face_idx = i ;
		}
	}
	save_prims( face_idx, m_vert_faces.size() ) ;
}

void x_loader::save_prims( int face_idx, int face_idx2 )
{
	if ( m_mesh == 0 || face_idx == face_idx2 ) return ;

	int n_verts = m_vert_faces[ face_idx ].size() ;
	int n_prims = face_idx2 - face_idx ;

	MdxDrawArrays *cmd = new MdxDrawArrays ;
	m_mesh->AttachChild( cmd ) ;
	if ( n_verts == 3 ) {
		cmd->SetMode( MDX_PRIM_TRIANGLES ) ;
		cmd->SetVertexCount( n_verts * n_prims ) ;
		cmd->SetPrimCount( 1 ) ;
	} else {
		cmd->SetMode( MDX_PRIM_TRIANGLE_FAN ) ;
		cmd->SetVertexCount( n_verts ) ;
		cmd->SetPrimCount( n_prims ) ;
	}
	for ( int i = 0 ; i < n_prims ; i ++ ) {
		vector<int> &face = m_vert_faces[ face_idx + i ] ;
		for ( int j = 0 ; j < n_verts ; j ++ ) {
			cmd->SetIndex( n_verts * i + j, face[ n_verts - 1 - j ] ) ;
		}
	}
}

//----------------------------------------------------------------
//  load animations
//----------------------------------------------------------------

void x_loader::load_anim_set( ID3DXFileData *anim_set )
{
	if ( anim_set == 0 ) return ;

	m_anim_start = 1000000.0f ;
	m_anim_end = -1000000.0f ;

	m_model->AttachChild( m_motion = new MdxMotion( get_name( anim_set ).c_str() ) ) ;

	vector<ID3DXFileData *> anims = enum_child( anim_set, TID_D3DRMAnimation ) ;
	for ( int i = 0 ; i < anims.size() ; i ++ ) {
		ID3DXFileData *anim = anims[ i ] ;
		ID3DXFileData *target = find_child( anim, TID_D3DRMFrame ) ;
		if ( target == 0 ) continue ;
		vector<ID3DXFileData *> anim_keys = enum_child( anim, TID_D3DRMAnimationKey ) ;
		for ( int j = 0 ; j < anim_keys.size() ; j ++ ) {
			load_anim_key( anim_keys[ j ], target ) ;
		}
	}

	if ( m_anim_start <= m_anim_end ) {
		m_motion->AttachChild( new MdxFrameLoop( m_anim_start, m_anim_end ) ) ;
	}
}

void x_loader::load_anim_key( ID3DXFileData *anim_key, ID3DXFileData *target )
{
	if ( anim_key == 0 || target == 0 ) return ;

	static int CmdTypes[] = { MDX_ROTATE, MDX_SCALE, MDX_TRANSLATE, 0 } ;
	static int CmdDims[] = { 4, 3, 3, 16 } ;

	int type = get_int( anim_key ) ;
	int n_keys = get_int( anim_key ) ;
	if ( type > 3 ) type = 3 ;
	int n_dims = CmdDims[ type ] ;

	if ( type == 3 ) {
		load_matrix_anim_keys( anim_key, target, n_keys ) ;
		return ;
	}

	m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
	m_fcurve->SetFormat( ( type == 0 ) ? MDX_FCURVE_SPHERICAL : MDX_FCURVE_LINEAR ) ;
	m_fcurve->SetDimCount( CmdDims[ type ] ) ;
	m_fcurve->SetKeyFrameCount( n_keys ) ;

	float vals[ 16 ] ;
	memset( vals, 0, sizeof( vals ) ) ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = m_fcurve->GetKeyFrame( i ) ;
		float frame = (float)get_int( anim_key ) ;
		int n_dims2 = get_int( anim_key ) ;
		get_data( anim_key, vals, sizeof( float ) * n_dims ) ;

		key.SetFrame( frame ) ;
		if ( type == 0 ) key.SetValueQuat( 0, flip_z( yzwx( ( *(quat *)vals ) ) ) ) ;
		if ( type == 1 ) key.SetValueVec3( 0, *(vec3 *)vals ) ;
		if ( type == 2 ) key.SetValueVec3( 0, flip_z( *(vec3 *)vals ) ) ;
		if ( type == 3 ) key.SetValueMat4( 0, flip_z( *(mat4 *)vals ) ) ;

		if ( frame < m_anim_start ) m_anim_start = frame ;
		if ( frame > m_anim_end ) m_anim_end = frame ;
	}

	//  animation

	MdxAnimate *cmd = new MdxAnimate ;
	m_motion->AttachChild( cmd ) ;
	cmd->SetScope( MDX_BONE ) ;
	cmd->SetBlock( get_name( target ).c_str() ) ;
	cmd->SetCommand( CmdTypes[ type ] ) ;
	cmd->SetMode( 0 ) ;
	cmd->SetFCurve( m_fcurve->GetName() ) ;
}

void x_loader::load_matrix_anim_keys( ID3DXFileData *anim_key, ID3DXFileData *target, int n_keys )
{
	MdxFCurve *translate = new MdxFCurve ;
	m_motion->AttachChild( translate ) ;
	translate->SetFormat( MDX_FCURVE_LINEAR ) ;
	translate->SetDimCount( 3 ) ;
	translate->SetKeyFrameCount( n_keys ) ;

	MdxFCurve *rotate = new MdxFCurve ;
	m_motion->AttachChild( rotate ) ;
	rotate->SetFormat( MDX_FCURVE_SPHERICAL ) ;
	rotate->SetDimCount( 4 ) ;
	rotate->SetKeyFrameCount( n_keys ) ;

	MdxFCurve *scale = new MdxFCurve ;
	m_motion->AttachChild( scale ) ;
	scale->SetFormat( MDX_FCURVE_LINEAR ) ;
	scale->SetDimCount( 3 ) ;

	scale->SetKeyFrameCount( n_keys ) ;

	vec4 t_val, s_val ;
	quat r_val ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		float frame = (float)get_int( anim_key ) ;
		int n_dims2 = get_int( anim_key ) ;
		mat4 m_val = get_mat4( anim_key ) ;

		if ( frame < m_anim_start ) m_anim_start = frame ;
		if ( frame > m_anim_end ) m_anim_end = frame ;

		decompose( flip_z( m_val ), t_val, r_val, s_val ) ;

		MdxKeyFrame &t_key = translate->GetKeyFrame( i ) ;
		t_key.SetFrame( frame ) ;
		t_key.SetValueVec3( 0, t_val ) ;

		MdxKeyFrame &r_key = rotate->GetKeyFrame( i ) ;
		r_key.SetFrame( frame ) ;
		r_key.SetValueQuat( 0, r_val ) ;

		MdxKeyFrame &s_key = scale->GetKeyFrame( i ) ;
		s_key.SetFrame( frame ) ;
		s_key.SetValueVec3( 0, s_val ) ;
	}

	//  animation

	MdxAnimate *t_cmd = new MdxAnimate ;
	m_motion->AttachChild( t_cmd ) ;
	t_cmd->SetScope( MDX_BONE ) ;
	t_cmd->SetBlock( get_name( target ).c_str() ) ;
	t_cmd->SetCommand( MDX_TRANSLATE ) ;
	t_cmd->SetMode( 0 ) ;
	t_cmd->SetFCurve( translate->GetName() ) ;

	MdxAnimate *r_cmd = new MdxAnimate ;
	m_motion->AttachChild( r_cmd ) ;
	r_cmd->SetScope( MDX_BONE ) ;
	r_cmd->SetBlock( get_name( target ).c_str() ) ;
	r_cmd->SetCommand( MDX_ROTATE ) ;
	r_cmd->SetMode( 0 ) ;
	r_cmd->SetFCurve( rotate->GetName() ) ;

	MdxAnimate *s_cmd = new MdxAnimate ;
	m_motion->AttachChild( s_cmd ) ;
	s_cmd->SetScope( MDX_BONE ) ;
	s_cmd->SetBlock( get_name( target ).c_str() ) ;
	s_cmd->SetCommand( MDX_SCALE ) ;
	s_cmd->SetMode( 0 ) ;
	s_cmd->SetFCurve( scale->GetName() ) ;
}

//----------------------------------------------------------------
//  subroutines ( convenience functions )
//----------------------------------------------------------------

void x_loader::load_frame( const vector<ID3DXFileData *> &frames, ID3DXFileData *parent )
{
	for ( int i = 0 ; i < frames.size() ; i ++ ) load_frame( frames[ i ], parent ) ;
}

void x_loader::load_mesh( const vector<ID3DXFileData *> &meshes, ID3DXFileData *parent )
{
	for ( int i = 0 ; i < meshes.size() ; i ++ ) load_mesh( meshes[ i ], parent ) ;
}

void x_loader::load_weights( const vector<ID3DXFileData *> &weights )
{
	for ( int i = 0 ; i < weights.size() ; i ++ ) load_weights( weights[ i ] ) ;
}

void x_loader::load_material( const vector<ID3DXFileData *> &materials )
{
	for ( int i = 0 ; i < materials.size() ; i ++ ) load_material( materials[ i ] ) ;
}

//----------------------------------------------------------------
//  subroutines ( x-file node )
//----------------------------------------------------------------

ID3DXFileData *x_loader::find_child( ID3DXFileData *xdata, const GUID &type )
{
	SIZE_T n_child = 0 ;
	xdata->GetChildren( &n_child ) ;
	for ( int i = 0 ; i < n_child ; i ++ ) {
		ID3DXFileData *child = 0 ;
		xdata->GetChild( i, &child ) ;
		if ( child == 0 ) continue ;
		GUID type2 ;
		child->GetType( &type2 ) ;
		if ( type2 == type ) return child ;
	}
	return 0 ;
}

vector<ID3DXFileData *> x_loader::enum_child( ID3DXFileData *xdata, const GUID &type )
{
	vector<ID3DXFileData *> result ;
	SIZE_T n_child = 0 ;
	xdata->GetChildren( &n_child ) ;
	for ( int i = 0 ; i < n_child ; i ++ ) {
		ID3DXFileData *child = 0 ;
		xdata->GetChild( i, &child ) ;
		if ( child == 0 ) continue ;
		GUID type2 ;
		child->GetType( &type2 ) ;
		if ( type2 == type ) result.push_back( child ) ;
	}
	return result ;
}

string x_loader::get_name( ID3DXFileData *xdata )
{
	char buf[ 256 ] ;
	SIZE_T len ;
	len = sizeof( buf ) - 1 ;
	xdata->GetName( buf, &len ) ;
	buf[ len ] = '\0' ;
	return buf ;
}

//----------------------------------------------------------------
//  subroutines ( x-file data )
//----------------------------------------------------------------

string x_loader::get_string( ID3DXFileData *xdata )
{
	lock_data( xdata ) ;
	if ( m_xdata_addr == 0 ) return "" ;
	int size = strlen( m_xdata_addr ) + 1 ;
	if ( size > m_xdata_size ) {
		m_xdata_addr = 0 ;
		m_xdata_size = 0 ;
		return "" ;
	}
	const char *buf = m_xdata_addr ;
	m_xdata_addr += size ;
	m_xdata_size -= size ;
	return buf ;
}

int x_loader::get_int( ID3DXFileData *xdata )
{
	int n = 0 ;
	get_data( xdata, &n, sizeof( n ) ) ;
	return n ;
}

float x_loader::get_float( ID3DXFileData *xdata )
{
	float f = 0.0f ;
	get_data( xdata, &f, sizeof( f ) ) ;
	return f ;
}

vector<int> x_loader::get_int( ID3DXFileData *xdata, int count )
{
	vector<int> result ;
	result.resize( count ) ;
	get_data( xdata, &( result[ 0 ] ), sizeof( int ) * count ) ;
	return result ;
}

vector<float> x_loader::get_float( ID3DXFileData *xdata, int count )
{
	vector<float> result ;
	result.resize( count ) ;
	get_data( xdata, &( result[ 0 ] ), sizeof( float ) * count ) ;
	return result ;
}

mat4 x_loader::get_mat4( ID3DXFileData *xdata )
{
	mat4 m ;
	get_data( xdata, &m, sizeof( m ) ) ;
	return m ;
}

vec4 x_loader::get_vec4( ID3DXFileData *xdata )
{
	vec4 v ;
	get_data( xdata, &v, sizeof( v ) ) ;
	return v ;
}

vec3 x_loader::get_vec3( ID3DXFileData *xdata )
{
	vec3 v ;
	get_data( xdata, &v, sizeof( v ) ) ;
	return v ;
}

vec2 x_loader::get_vec2( ID3DXFileData *xdata )
{
	vec2 v ;
	get_data( xdata, &v, sizeof( v ) ) ;
	return v ;
}

bool x_loader::get_data( ID3DXFileData *xdata, void *data, int size )
{
	lock_data( xdata ) ;
	if ( m_xdata_addr == 0 || size > m_xdata_size ) {
		m_xdata_addr = 0 ;
		m_xdata_size = 0 ;
		return false ;
	}
	memcpy( data, m_xdata_addr, size ) ;
	m_xdata_addr += size ;
	m_xdata_size -= size ;
	return true ;
}

void x_loader::lock_data( ID3DXFileData *xdata )
{
	if ( m_lock_xdata == xdata ) return ;
	unlock_data() ;

	if ( xdata != 0 ) {
		const VOID *addr = 0 ;
		SIZE_T size = 0 ;
		xdata->Lock( &size, &addr ) ;
		m_lock_xdata = xdata ;
		m_xdata_addr = (const char *)addr ;
		m_xdata_size = size ;
	}
}

void x_loader::unlock_data()
{
	if ( m_lock_xdata != 0 ) m_lock_xdata->Unlock() ;
	m_lock_xdata = 0 ;
	m_xdata_addr = 0 ;
	m_xdata_size = 0 ;
}

//----------------------------------------------------------------
//  subroutines ( calculation )
//----------------------------------------------------------------

mat4 x_loader::flip_z( const mat4 &m )
{
	mat4 m2 = m ;
	m2.x.z = -m2.x.z ;
	m2.y.z = -m2.y.z ;
	m2.z.z = -m2.z.z ;
	m2.w.z = -m2.w.z ;
	m2.z.x = -m2.z.x ;
	m2.z.y = -m2.z.y ;
	m2.z.z = -m2.z.z ;
	m2.z.w = -m2.z.w ;
	return m2 ;
}

vec3 x_loader::flip_z( const vec3 &v )
{
	return vec3( v.x, v.y, -v.z ) ;
}

quat x_loader::flip_z( const quat &q )
{
	return quat( q.x, q.y, -q.z, q.w ) ;
}

quat x_loader::yzwx( const quat &q )
{
	return quat( q.y, q.z, q.w, q.x ) ;
}

void x_loader::decompose( const mat4 &m, vec4 &t, quat &r, vec4 &s )
{
	mat4 norm ;
	s = normalize3( m, norm ) ;
	if ( determinant3( norm ) < 0.0f ) {
		norm.x = -norm.x ;
		s.x = -s.x ;
	}
	r = quat_from_mat4( norm ) ;
	t = norm.w ;
	// if ( equals3( t, 0.0f, 0.000001f ) ) t = 0.0f ;
	// if ( equals( r, 1.0f, 0.000001f ) ) r = 1.0f ;
	// if ( equals3( s, 1.0f, 0.000001f ) ) s = 1.0f ;
}

float x_loader::determinant3( const mat4 &m )
{
	return dot3( cross3( m.x, m.y ), m.z ) ;
}

vec4 x_loader::normalize3( const mat4 &m1, mat4 &m2 )
{
	float x = normalize3( m1.x, m2.x ) ;
	float y = normalize3( m1.y, m2.y ) ;
	float z = normalize3( m1.z, m2.z ) ;
	m2.w = m1.w ;
	return vec4( x, y, z, 1.0f ) ;
}

float x_loader::normalize3( const vec4 &v1, vec4 &v2 )
{
	float f = sqrtf( v1.x * v1.x + v1.y * v1.y + v1.z * v1.z ) ;
	if ( f == 0.0f ) {
		v2 = v1 ;
		return 0.0f ;
	}
	float r = 1.0f / f ;
	v2.set( v1.x * r, v1.y * r, v1.z * r, v1.w ) ;
	return f ;
}
