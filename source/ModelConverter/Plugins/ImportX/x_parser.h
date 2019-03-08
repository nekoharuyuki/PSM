#ifndef	X_PARSER_H_INCLUDE
#define	X_PARSER_H_INCLUDE

#include "x_stream.h"

//----------------------------------------------------------------
//  x_parser
//----------------------------------------------------------------

class x_parser {
public:
	x_parser() ;
	virtual ~x_parser() ;

	bool load( const string &filename ) ;

private:
	bool parse_recursive() ;
	bool parse_block( const string &type, const string &name ) ;

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


protected:
	string m_filename ;
	x_ifstream *m_stream ;
	bool m_is_binary ;
	char m_block_from[ 32 ] ;
} ;

#endif
