#ifndef	X_LOADER_H_INCLUDE
#define	X_LOADER_H_INCLUDE

#define D3D_OVERLOADS
#include <d3d9.h>
#include <d3dx9.h>

#pragma comment( lib, "d3d9.lib" )
#pragma comment( lib, "d3dx9.lib" )
#pragma comment( lib, "d3dxof.lib")
#pragma comment( lib, "dxguid.lib")

#include "ImportX.h"

//----------------------------------------------------------------
//  x_loader
//----------------------------------------------------------------

class x_loader {
public:
	x_loader( MdxShell *shell = 0 ) ;
	~x_loader() ;

	bool load( MdxBlock *&block, const string &filename ) ;

private:
	void load_frame( ID3DXFileData *frame, ID3DXFileData *parent = 0 ) ;
	void load_mesh( ID3DXFileData *mesh, ID3DXFileData *parent = 0 ) ;
	void load_xform( ID3DXFileData *xform ) ;
	void load_vertices( ID3DXFileData *mesh ) ;
	void load_normals( ID3DXFileData *normals ) ;
	void load_tcoords( ID3DXFileData *tcoords ) ;
	void load_vcolors( ID3DXFileData *vcolors ) ;
	void load_weights( ID3DXFileData *weights ) ;
	void load_mat_list( ID3DXFileData *mat_list ) ;
	void load_material( ID3DXFileData *material ) ;
	MdxMaterial *find_material( MdxMaterial *material ) ;
	void save_normals() ;
	void save_meshes() ;
	void save_prims( int face_idx, int face_idx2 ) ;

	void load_anim_set( ID3DXFileData *anim_set ) ;
	void load_anim_key( ID3DXFileData *anim_key, ID3DXFileData *frame ) ;
	void load_matrix_anim_keys( ID3DXFileData *anim_key, ID3DXFileData *frame, int n_keys ) ;
	// void decompose_transforms() ;

	void load_frame( const vector<ID3DXFileData *> &frames, ID3DXFileData *parent = 0 ) ;
	void load_mesh( const vector<ID3DXFileData *> &meshes, ID3DXFileData *parent = 0 ) ;
	void load_weights( const vector<ID3DXFileData *> &weights ) ;
	void load_material( const vector<ID3DXFileData *> &materials ) ;

	ID3DXFileData *find_child( ID3DXFileData *xdata, const GUID &type ) ;
	vector<ID3DXFileData *> enum_child( ID3DXFileData *xdata, const GUID &type ) ;
	string get_name( ID3DXFileData *xdata ) ;
	string get_string( ID3DXFileData *xdata ) ;
	int get_int( ID3DXFileData *xdata ) ;
	float get_float( ID3DXFileData *xdata ) ;
	vector<int> get_int( ID3DXFileData *xdata, int count ) ;
	vector<float> get_float( ID3DXFileData *xdata, int count ) ;
	mat4 get_mat4( ID3DXFileData *xdata ) ;
	vec4 get_vec4( ID3DXFileData *xdata ) ;
	vec3 get_vec3( ID3DXFileData *xdata ) ;
	vec2 get_vec2( ID3DXFileData *xdata ) ;
	bool get_data( ID3DXFileData *xdata, void *data, int size ) ;
	void lock_data( ID3DXFileData *xdata ) ;
	void unlock_data() ;

	mat4 flip_z( const mat4 &m ) ;
	vec3 flip_z( const vec3 &v ) ;
	quat flip_z( const quat &v ) ;
	quat yzwx( const quat &v ) ;
	void decompose( const mat4 &m, vec4 &t, quat &r, vec4 &s ) ;
	float determinant3( const mat4 &m ) ;
	vec4 normalize3( const mat4 &m1, mat4 &m2 ) ;
	float normalize3( const vec4 &v1, vec4 &v2 ) ;

private:
	MdxShell *m_shell ;

	string m_filename ;
	MdxFile *m_file ;
	MdxModel *m_model ;
	MdxBone *m_bone ;
	MdxPart *m_part ;
	MdxMesh *m_mesh ;
	MdxArrays *m_arrays ;
	MdxMaterial *m_material ;
	MdxLayer *m_layer ;
	MdxTexture *m_texture ;
	MdxMotion *m_motion ;
	MdxFCurve *m_fcurve ;

	MdxMaterial *m_empty_material ;		//  model empty material
	map<string,MdxTexture *> m_textures ;	//  model textures ( by filename )
	set<string> m_matrix_targets ;		//  model matrix animation targets

	vector<vector<int> > m_vert_faces ;	//  mesh vertex indices
	vector<vector<int> > m_norm_faces ;	//  mesh normal indices
	vector<vec3> m_normals ;		//  mesh normal vectors
	vector<int> m_mat_idxs ;		//  mesh material indices
	vector<MdxMaterial *> m_mat_ptrs ;	//  mesh material pointers

	float m_anim_start ;			//  animation start
	float m_anim_end ;			//  animation end

	ID3DXFileData *m_lock_xdata ;		//  current x-file data
	const char *m_xdata_addr ;		//  current x-file data
	int m_xdata_size ;			//  current x-file data
} ;


#endif

