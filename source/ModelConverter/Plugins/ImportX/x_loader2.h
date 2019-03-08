#ifndef	X_LOADER2_H_INCLUDE
#define	X_LOADER2_H_INCLUDE

#include "x_parser.h"

//----------------------------------------------------------------
//  x_loader2
//----------------------------------------------------------------

class x_loader2 : public x_parser {
public:
	x_loader2( MdxShell *shell = 0 ) ;
	virtual ~x_loader2() ;

	bool load( MdxBlock *&block, const string &filename ) ;

protected:
	virtual void error( const string &mesg ) ;
	virtual void warning( const string &mesg ) ;

	virtual bool open_frame( const string &name ) ;
	virtual bool close_frame() ;
	virtual bool set_transform( const mat4 &m ) ;

	virtual bool open_mesh( const string &name ) ;
	virtual bool close_mesh() ;
	virtual bool set_vertices( vector<vec3> &verts, vector<vector<int> > &faces ) ;
	virtual bool set_normals( vector<vec3> &norms, vector<vector<int> > &faces ) ;
	virtual bool set_texture_coords( vector<vec2> &uvs ) ;
	virtual bool set_vertex_colors( vector<int> &idxs, vector<rgba8888> &cols ) ;
	virtual bool set_skin_weights( const string &bone, vector<int> &idxs, vector<float> &wgts, const mat4 &offset ) ;

	virtual bool open_material_list() ;
	virtual bool close_material_list() ;
	virtual bool set_material_indices( int n_mats, vector<int> &idxs ) ;
	virtual bool open_material() ;
	virtual bool close_material() ;
	virtual bool set_material( const vec4 &diffuse, float shininess, const vec4 &specular, const vec4 &emissive ) ;
	virtual bool set_texture_filename( const string &filename ) ;

	virtual bool open_animation_set( const string &name ) ;
	virtual bool close_animation_set() ;
	virtual bool open_animation() ;
	virtual bool close_animation() ;
	virtual bool set_animation_keys( int type, vector<int> &frms, vector<int> &dims, vector<float> &vals ) ;
	virtual bool set_block_reference( const string &name ) ;

private:
	bool set_matrix_animation_keys( vector<int> &frms, vector<int> &dims, vector<float> &vals ) ;

private:
	void reset_context() ;
	void store_normals() ;
	void create_prims() ;
	// void finish_transforms() ;
	MdxMaterial *find_material( MdxMaterial *material ) ;

protected:
	MdxShell *m_shell ;

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

	vector<MdxBone *> m_bone_stack ;
	vector<vector<int> > m_vert_faces ;	//  mesh vertex indices
	vector<vector<int> > m_norm_faces ;	//  mesh normal indices
	vector<vec3> m_normals ;		//  mesh normal vectors
	vector<int> m_material_idxs ;		//  mesh material indices
	vector<MdxMaterial *> m_material_list ;	//  mesh material pointers
	bool m_texture_exists ;
	string m_texture_filename ;

	float m_anim_start ;			//  animation start
	float m_anim_end ;			//  animation end
	string m_anim_bone ;			//  animation target
	vector<MdxAnimate *> m_anim_cmds ;	//  animation commands
} ;

#endif
