#include "x_loader2.h"

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

#define OUTPUT_ERROR_MESSAGE (0)
#define ADJUST_VERTEX_COLOR (0)

static mat4 flip( const mat4 &m ) ;
static void decompose( const mat4 &m, vec4 &t, quat &r, vec4 &s ) ;
static float determinant3( const mat4 &m ) ;
static vec4 normalize3( const mat4 &m1, mat4 &m2 ) ;
static float normalize3( const vec4 &v1, vec4 &v2 ) ;

//----------------------------------------------------------------
//  x_loader2
//----------------------------------------------------------------

x_loader2::x_loader2( MdxShell *shell )
{
	m_shell = shell ;
}

x_loader2::~x_loader2()
{
	;
}

//----------------------------------------------------------------
//  load
//----------------------------------------------------------------

bool x_loader2::load( MdxBlock *&block, const string &filename )
{
	block = 0 ;

	reset_context() ;

	m_file = new MdxFile ;
	string name = str_tolower( str_rootname( str_tailname( filename ) ) ) ;
	m_file->AttachChild( m_model = new MdxModel( name.c_str() ) ) ;

	if ( !x_parser::load( filename ) ) {
		m_file->Release() ;
		return false ;
	}

	block = m_file ;
	return true ;
}

//----------------------------------------------------------------
//  error callbacks
//----------------------------------------------------------------

void x_loader2::error( const string &mesg )
{
	#if ( OUTPUT_ERROR_MESSAGE )
	if ( m_shell != 0 ) m_shell->Error( "%s", mesg.c_str() ) ;
	#endif // OUTPUT_ERROR_MESSAGE
}

void x_loader2::warning( const string &mesg )
{
	#if ( OUTPUT_ERROR_MESSAGE )
	if ( m_shell != 0 ) m_shell->Warning( "%s", mesg.c_str() ) ;
	#endif // OUTPUT_ERROR_MESSAGE
}

//----------------------------------------------------------------
//  geometry callbacks
//----------------------------------------------------------------

bool x_loader2::open_frame( const string &name )
{
	MdxBone *parent = m_bone ;
	m_bone_stack.push_back( parent ) ;

	m_model->AttachChild( m_bone = new MdxBone( name.c_str() ) ) ;
	if ( parent != 0 ) {
		m_bone->AttachChild( new MdxParentBone( parent->GetName() ) ) ;
	}
	return true ;
}

bool x_loader2::close_frame()
{
	if ( m_bone == 0 ) return true ;

	if ( !m_bone_stack.empty() ) {
		m_bone = m_bone_stack.back() ;
		m_bone_stack.pop_back() ;
	} else {
		m_bone = 0 ;
	}
	return true ;
}

bool x_loader2::set_transform( const mat4 &m )
{
	if ( m_bone == 0 ) return true ;

	vec4 t, s ;
	quat r ;
	decompose( flip( m ), t, r, s ) ;
	if ( !equals3( t, 0.0f ) ) m_bone->AttachChild( new MdxTranslate( t ) ) ;
	if ( !equals( r, 1.0f ) ) m_bone->AttachChild( new MdxRotate( r ) ) ;
	if ( !equals3( s, 1.0f ) ) m_bone->AttachChild( new MdxScale( s ) ) ;
	return true ;
}

bool x_loader2::open_mesh( const string &name )
{
	if ( m_bone == 0 ) {
		open_frame( "" ) ;	// implicit bone
	}

	m_model->AttachChild( m_part = new MdxPart( name.c_str() ) ) ;
	m_bone->AttachChild( new MdxDrawPart( m_part->GetName() ) ) ;

	m_mesh = 0 ;
	m_arrays = 0 ;
	m_material_idxs.clear() ;
	m_material_list.clear() ;
	m_vert_faces.clear() ;
	m_norm_faces.clear() ;
	return true ;
}

bool x_loader2::close_mesh()
{
	if ( m_part == 0 ) return true ;

	store_normals() ;
	create_prims() ;

	m_part = 0 ;
	m_mesh = 0 ;
	m_arrays = 0 ;
	return true ;
}

bool x_loader2::set_vertices( vector<vec3> &verts, vector<vector<int> > &faces )
{
	if ( m_part == 0 ) return true ;

	m_part->AttachChild( m_arrays = new MdxArrays ) ;
	m_arrays->SetFormat( MDX_VF_POSITION ) ;
	m_arrays->SetVertexCount( verts.size() ) ;
	for ( int i = 0 ; i < (int)verts.size() ; i ++ ) {
		vec4 vert = verts[ i ] ;
		vert.z = -vert.z ;
		m_arrays->GetVertex( i ).SetPosition( vert ) ;
	}
	m_vert_faces = faces ;
	return true ;
}

bool x_loader2::set_normals( vector<vec3> &norms, vector<vector<int> > &faces )
{
	if ( m_arrays == 0 ) return true ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_NORMAL ) ;

	int count = norms.size() ;
	m_normals = norms ;
	for ( int i = 0 ; i < count ; i ++ ) {
		vec3 &norm = m_normals[ i ] ;
		norm.z = -norm.z ;
	}
	m_norm_faces = faces ;
	return true ;
}

bool x_loader2::set_texture_coords( vector<vec2> &uvs )
{
	if ( m_arrays == 0 ) return true ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_TEXCOORD ) ;
	int count = uvs.size() ;
	if ( count >= m_arrays->GetVertexCount() ) count = m_arrays->GetVertexCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		m_arrays->GetVertex( i ).SetTexCoord( 0, uvs[ i ] ) ;
	}
	return true ;
}

bool x_loader2::set_vertex_colors( vector<int> &idxs, vector<rgba8888> &cols )
{
	if ( m_arrays == 0 ) return true ;

	m_arrays->SetFormat( m_arrays->GetFormat() | MDX_VF_COLOR ) ;
	int count = ( idxs.size() < cols.size() ) ? idxs.size() : cols.size() ;
	if ( count >= m_arrays->GetVertexCount() ) count = m_arrays->GetVertexCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		rgba8888 col = cols[ i ] ;
		#if ( ADJUST_VERTEX_COLOR )
		col.r = 255 - ( 255 - col.r ) * col.a ;
		col.g = 255 - ( 255 - col.g ) * col.a ;
		col.b = 255 - ( 255 - col.b ) * col.a ;
		col.a = 255 ;
		#endif // ADJUST_VERTEX_COLOR
		m_arrays->GetVertex( idxs[ i ] ).SetColor( col ) ;
	}
	return true ;
}

bool x_loader2::set_skin_weights( const string &bone, vector<int> &idxs, vector<float> &wgts, const mat4 &offset )
{
	if ( m_arrays == 0 ) return true ;

	int num = m_arrays->GetVertexWeightCount() ;
	m_arrays->SetVertexWeightCount( num + 1 ) ;

	int count = ( idxs.size() < wgts.size() ) ? idxs.size() : wgts.size() ;
	if ( count >= m_arrays->GetVertexCount() ) count = m_arrays->GetVertexCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		m_arrays->GetVertex( idxs[ i ] ).SetWeight( num, wgts[ i ] ) ;
	}

	if ( m_bone == 0 ) return true ;
	m_bone->AttachChild( new MdxBlendBone( bone.c_str(), flip( offset ) ) ) ;
	return true ;
}

//----------------------------------------------------------------
//  material callbacks
//----------------------------------------------------------------

bool x_loader2::open_material_list()
{
	return true ;
}

bool x_loader2::close_material_list()
{
	return true ;
}

bool x_loader2::set_material_indices( int n_mats, vector<int> &idxs )
{
	m_material_idxs = idxs ;
	return true ;
}

bool x_loader2::open_material()
{
	m_material = new MdxMaterial ;
	return true ;
}

bool x_loader2::close_material()
{
	if ( m_material == 0 ) return true ;

	MdxMaterial *exists = find_material( m_material ) ;
	if ( exists != 0 ) {
		m_material->Release() ;
		m_material = exists ;
	}
	m_model->AttachChild( m_material ) ;
	m_material_list.push_back( m_material ) ;

	m_material = 0 ;
	m_layer = 0 ;
	m_texture = 0 ;
	return true ;
}

bool x_loader2::set_material( const vec4 &diffuse, float shininess, const vec4 &specular, const vec4 &emissive )
{
	if ( m_material == 0 ) return true ;

	vec4 diffuse2 = diffuse ;
	vec4 specular2 = specular ;
	if ( diffuse2.w == 0.0f ) diffuse2.w = 1.0f ;		// (>_<) heuristic opacity
	if ( equals3( specular, 0.0f ) ) shininess = 0.0f ;	// (>_<) heuristic shininess
	if ( shininess == 0.0f ) specular2 = 0.0f ;		// (>_<) heuristic sepcular

	if ( !equals3( diffuse2, 1.0f ) ) m_material->AttachChild( new MdxDiffuse( diffuse2 ) ) ;
	if ( !equals3( specular2, 0.0f ) ) m_material->AttachChild( new MdxSpecular( specular2 ) ) ;
	if ( !equals3( emissive, 0.0f ) ) m_material->AttachChild( new MdxEmission( emissive ) ) ;
	if ( diffuse2.w != 1.0f ) m_material->AttachChild( new MdxOpacity( diffuse2.w ) ) ;
	if ( shininess != 0.0f ) m_material->AttachChild( new MdxShininess( shininess ) ) ;
	return true ;
}

bool x_loader2::set_texture_filename( const string &filename )
{
	if ( m_material == 0 ) return true ;

	m_texture = m_textures[ filename ] ;
	if ( m_texture == 0 ) {
		m_model->AttachChild( m_texture = new MdxTexture ) ;
		m_texture->AttachChild( new MdxFileName( filename.c_str() ) ) ;
		m_textures[ filename ] = m_texture ;
	}
	m_material->AttachChild( m_layer = new MdxLayer ) ;
	m_layer->AttachChild( new MdxSetTexture( m_texture->GetName() ) ) ;
	return true ;
}

//----------------------------------------------------------------
//  animation callbacks
//----------------------------------------------------------------

bool x_loader2::open_animation_set( const string &name )
{
	m_model->AttachChild( m_motion = new MdxMotion( name.c_str() ) ) ;
	m_fcurve = 0 ;
	m_anim_start = 1000000.0f ;
	m_anim_end = -1000000.0f ;
	return true ;
}

bool x_loader2::close_animation_set()
{
	if ( m_motion == 0 ) return true ;

	if ( m_anim_start <= m_anim_end ) {
		m_motion->AttachChild( new MdxFrameLoop( m_anim_start, m_anim_end ) ) ;
	}

	m_motion = 0 ;
	m_fcurve = 0 ;
	return true ;
}

bool x_loader2::open_animation()
{
	if ( m_motion == 0 ) return true ;

	m_anim_bone = "" ;
	m_anim_cmds.clear() ;
	return true ;
}

bool x_loader2::close_animation()
{
	if ( m_motion == 0 ) return true ;

	m_anim_bone = "" ;
	m_anim_cmds.clear() ;
	return true ;
}

bool x_loader2::set_animation_keys( int type, vector<int> &frms, vector<int> &dims, vector<float> &vals )
{
	if ( m_motion == 0 ) return true ;

	//  fcurve

	static int CmdTypes[] = { MDX_ROTATE, MDX_SCALE, MDX_TRANSLATE, 0 } ;
	static int CmdDims[] = { 4, 3, 3, 16 } ;

	if ( type < 0 ) return true ;
	if ( type > 3 ) type = 3 ;

	if ( type == 3 ) return set_matrix_animation_keys( frms, dims, vals ) ;

	int n_keys = frms.size() ;
	m_motion->AttachChild( m_fcurve = new MdxFCurve ) ;
	m_fcurve->SetFormat( ( type == 0 ) ? MDX_FCURVE_SPHERICAL : MDX_FCURVE_LINEAR ) ;
	m_fcurve->SetDimCount( CmdDims[ type ] ) ;
	m_fcurve->SetKeyFrameCount( n_keys ) ;

	int idx = 0 ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = m_fcurve->GetKeyFrame( i ) ;

		float frame = (float)frms[ i ] ;
		key.SetFrame( frame ) ;
		if ( frame < m_anim_start ) m_anim_start = frame ;
		if ( frame > m_anim_end ) m_anim_end = frame ;

		switch ( type ) {
		    case 0 : {
			key.SetValue( 0, vals[ idx + 1 ] ) ;
			key.SetValue( 1, vals[ idx + 2 ] ) ;
			key.SetValue( 2, -vals[ idx + 3 ] ) ;
			key.SetValue( 3, vals[ idx + 0 ] ) ;
			break ;
		    }
		    case 1 : {
			key.SetValue( 0, vals[ idx + 0 ] ) ;
			key.SetValue( 1, vals[ idx + 1 ] ) ;
			key.SetValue( 2, vals[ idx + 2 ] ) ;
			break ;
		    }
		    case 2 : {
			key.SetValue( 0, vals[ idx + 0 ] ) ;
			key.SetValue( 1, vals[ idx + 1 ] ) ;
			key.SetValue( 2, -vals[ idx + 2 ] ) ;
			break ;
		    }
		    default : {
			mat4 *src = (mat4 *)&vals[ idx ] ;
			key.SetValueMat4( 0, flip( *src ) ) ;
			break ;
		    }
		}
		idx += dims[ i ] ;
	}

	//  animation

	MdxAnimate *cmd = new MdxAnimate ;
	m_motion->AttachChild( cmd ) ;
	cmd->SetScope( MDX_BONE ) ;
	cmd->SetBlock( m_anim_bone.c_str() ) ;
	cmd->SetCommand( CmdTypes[ type ] ) ;
	cmd->SetMode( 0 ) ;
	cmd->SetFCurve( m_fcurve->GetName() ) ;

	if ( m_anim_bone == "" ) m_anim_cmds.push_back( cmd ) ;
	return true ;
}

bool x_loader2::set_matrix_animation_keys( vector<int> &frms, vector<int> &dims, vector<float> &vals )
{
	int n_keys = frms.size() ;

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
	int idx = 0 ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		float frame = (float)frms[ i ] ;
		if ( frame < m_anim_start ) m_anim_start = frame ;
		if ( frame > m_anim_end ) m_anim_end = frame ;

		decompose( flip( *(mat4 *)&vals[ idx ] ), t_val, r_val, s_val ) ;

		MdxKeyFrame &t_key = translate->GetKeyFrame( i ) ;
		t_key.SetFrame( frame ) ;
		t_key.SetValueVec3( 0, t_val ) ;

		MdxKeyFrame &r_key = rotate->GetKeyFrame( i ) ;
		r_key.SetFrame( frame ) ;
		r_key.SetValueQuat( 0, r_val ) ;

		MdxKeyFrame &s_key = scale->GetKeyFrame( i ) ;
		s_key.SetFrame( frame ) ;
		s_key.SetValueVec3( 0, s_val ) ;

		idx += dims[ i ] ;
	}

	//  animation

	MdxAnimate *t_cmd = new MdxAnimate ;
	m_motion->AttachChild( t_cmd ) ;
	t_cmd->SetScope( MDX_BONE ) ;
	t_cmd->SetBlock( m_anim_bone.c_str() ) ;
	t_cmd->SetCommand( MDX_TRANSLATE ) ;
	t_cmd->SetMode( 0 ) ;
	t_cmd->SetFCurve( translate->GetName() ) ;

	MdxAnimate *r_cmd = new MdxAnimate ;
	m_motion->AttachChild( r_cmd ) ;
	r_cmd->SetScope( MDX_BONE ) ;
	r_cmd->SetBlock( m_anim_bone.c_str() ) ;
	r_cmd->SetCommand( MDX_ROTATE ) ;
	r_cmd->SetMode( 0 ) ;
	r_cmd->SetFCurve( rotate->GetName() ) ;

	MdxAnimate *s_cmd = new MdxAnimate ;
	m_motion->AttachChild( s_cmd ) ;
	s_cmd->SetScope( MDX_BONE ) ;
	s_cmd->SetBlock( m_anim_bone.c_str() ) ;
	s_cmd->SetCommand( MDX_SCALE ) ;
	s_cmd->SetMode( 0 ) ;
	s_cmd->SetFCurve( scale->GetName() ) ;

	if ( m_anim_bone == "" ) {
		m_anim_cmds.push_back( t_cmd ) ;
		m_anim_cmds.push_back( r_cmd ) ;
		m_anim_cmds.push_back( s_cmd ) ;
	}
	return true ;
}

bool x_loader2::set_block_reference( const string &name )
{
	if ( m_motion == 0 ) return true ;

	m_anim_bone = name ;
	for ( int i = 0 ; i < (int)m_anim_cmds.size() ; i ++ ) {
		m_anim_cmds[ i ]->SetBlock( name.c_str() ) ;
	}
	return true ;
}

//----------------------------------------------------------------
//  subroutines
//----------------------------------------------------------------

void x_loader2::reset_context()
{
	m_file = 0 ;
	m_model = 0 ;
	m_bone = 0 ;
	m_part = 0 ;
	m_mesh = 0 ;
	m_arrays = 0 ;
	m_material = 0 ;
	m_layer = 0 ;
	m_texture = 0 ;
	m_motion = 0 ;
	m_fcurve = 0 ;

	m_empty_material = 0 ;
	m_textures.clear() ;

	m_bone_stack.clear() ;
	m_vert_faces.clear() ;
	m_norm_faces.clear() ;
	m_normals.clear() ;

	m_material_idxs.clear() ;
	m_material_list.clear() ;
	m_texture_exists = false ;
	m_texture_filename = "" ;

	m_anim_start = 0.0f ;
	m_anim_end = 0.0f ;
	m_anim_bone = "" ;
	m_anim_cmds.clear() ;
}

void x_loader2::store_normals()
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

	vector<int> vids ;
	vector<int> nids ;
	int n_verts = m_arrays->GetVertexCount() ;
	for ( i = 0 ; i < n_verts ; i ++ ) {
		vids.push_back( i ) ;
		nids.push_back( -1 ) ;
	}

	int n_verts2 = n_verts ;
	for ( i = 0 ; i < (int)m_vert_faces.size() ; i ++ ) {
		vector<int> &vert_face = m_vert_faces[ i ] ;
		vector<int> &norm_face = m_norm_faces[ i ] ;
		for ( j = 0 ; j < (int)vert_face.size() ; j ++ ) {
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

	//  store normals

	m_arrays->SetVertexCount( n_verts2 ) ;
	for ( i = n_verts ; i < n_verts2 ; i ++ ) {
		MdxVertex &v = m_arrays->GetVertex( vids[ i ] ) ;
		m_arrays->SetVertex( i, v ) ;
	}
	for ( i = 0 ; i < n_verts2 ; i ++ ) {
		int nid = nids[ i ] ;
		if ( nid < 0 || nid >= (int)m_normals.size() ) continue ;
		m_arrays->GetVertex( i ).SetNormal( m_normals[ nids[ i ] ] ) ;
	}
}

void x_loader2::create_prims()
{
	m_mesh = 0 ;
	int mat_idx = -1 ;

	for ( int i = 0 ; i < (int)m_vert_faces.size() ; i ++ ) {

		//  check material

		if ( i < (int)m_material_idxs.size() ) {
			int idx = m_material_idxs[ i ] ;
			if ( idx != mat_idx && idx < (int)m_material_list.size() ) {
				m_mesh = 0 ;
				mat_idx = idx ;
			}
		}
		if ( m_mesh == 0 ) {
			m_part->AttachChild( m_mesh = new MdxMesh ) ;
			if ( mat_idx >= 0 && mat_idx < (int)m_material_list.size() ) {
				m_material = m_material_list[ mat_idx ] ;
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

		//  create prim

		vector<int> &face = m_vert_faces[ i ] ;
		int n_verts = face.size() ;

		MdxDrawArrays *cmd = new MdxDrawArrays ;
		m_mesh->AttachChild( cmd ) ;
		cmd->SetMode( MDX_PRIM_TRIANGLE_FAN ) ;
		cmd->SetVertexCount( n_verts ) ;
		cmd->SetPrimCount( 1 ) ;
		for ( int j = 0 ; j < n_verts ; j ++ ) {
			cmd->SetIndex( j, face[ n_verts - 1 - j ] ) ;
		}
	}
}

MdxMaterial *x_loader2::find_material( MdxMaterial *material )
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

//----------------------------------------------------------------
//  subroutine		vector functions
//----------------------------------------------------------------

static mat4 flip( const mat4 &m )
{
	mat4 m2 = m ;
	m2[0][2] = -m2[0][2] ;
	m2[1][2] = -m2[1][2] ;
	m2[2][2] = -m2[2][2] ;
	m2[3][2] = -m2[3][2] ;
	m2[2][0] = -m2[2][0] ;
	m2[2][1] = -m2[2][1] ;
	m2[2][2] = -m2[2][2] ;
	m2[2][3] = -m2[2][3] ;
	return m2 ;
}

static void decompose( const mat4 &m, vec4 &t, quat &r, vec4 &s )
{
	mat4 norm ;
	s = normalize3( m, norm ) ;
	if ( determinant3( norm ) < 0.0f ) {
		s.x = - s.x ;
		norm.x.x = -norm.x.x ;
		norm.x.y = -norm.x.y ;
		norm.x.z = -norm.x.z ;
	}
	r = quat_from_mat4( norm ) ;
	t = norm.w ;
	// if ( equals3( t, 0.0f, 0.000001f ) ) t = 0.0f ;
	// if ( equals( r, 1.0f, 0.000001f ) ) r = 1.0f ;
	// if ( equals3( s, 1.0f, 0.000001f ) ) s = 1.0f ;
}

static float determinant3( const mat4 &m )
{
	return dot3( cross3( m.x, m.y ), m.z ) ;
}

static vec4 normalize3( const mat4 &m1, mat4 &m2 )
{
	float x = normalize3( m1.x, m2.x ) ;
	float y = normalize3( m1.y, m2.y ) ;
	float z = normalize3( m1.z, m2.z ) ;
	m2.w = m1.w ;
	return vec4( x, y, z, 1.0f ) ;
}

static float normalize3( const vec4 &v1, vec4 &v2 )
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
