#include "x_parser.h"

//----------------------------------------------------------------
//  x_parser
//----------------------------------------------------------------

x_parser::x_parser()
{
	m_stream = 0 ;
	m_block_from[ 0 ] = '\0' ;
}

x_parser::~x_parser()
{
	delete m_stream ;
}

//----------------------------------------------------------------
//  load
//----------------------------------------------------------------

bool x_parser::load( const string &filename )
{
	m_filename = filename ;

	delete m_stream ;
	m_stream = 0 ;

	//  check header

	int header[ 4 ] ;
	bin_ifstream stream ;
	if ( !stream.open( m_filename ) ) {
		error( str_format( "open failed \"%s\"\n", m_filename.c_str() ) ) ;
		return false ;
	}
	stream.read( (char *)header, sizeof( header ) ) ;
	stream.close() ;

	if ( header[ 0 ] != 0x20666f78 ) {
		error( str_format( "wrong format \"%s\"\n", m_filename.c_str() ) ) ;
		return false ;
	}
	if ( header[ 2 ] == 0x20747874 ) {		// "txt "
		m_stream = new x_txt_ifstream ;
		m_is_binary = false ;
	} else if ( header[ 2 ] == 0x206e6962 ) {	// "bin "
		m_stream = new x_bin_ifstream ;
		m_is_binary = true ;
	} else {					// "bzip" ... etc
		error( str_format( "unsupported encoding \"%s\"\n", m_filename.c_str() ) ) ;
		return false ;
	}

	//  load

	if ( !m_stream->open( m_filename ) ) {
		error( str_format( "open failed \"%s\"\n", m_filename.c_str() ) ) ;
		return false ;
	}
	bool status = parse_recursive() ;
	m_stream->close() ;

	delete m_stream ;
	m_stream = 0 ;

	return status ;
}

bool x_parser::parse_recursive()
{
	string type, name ;

	while ( !m_stream->eof() ) {
		if ( !m_stream->open_block( type, name ) ) {
			break ;
		}
		if ( !m_is_binary ) {
			sprintf( m_block_from, "%d", m_stream->get_block_from() ) ;
		} else {
			sprintf( m_block_from, "0x%x", m_stream->get_block_from() ) ;
		}
		if ( !parse_block( type, name ) ) {
			return false ;
		}
		m_stream->close_block() ;
	}
	return true ;
}

bool x_parser::parse_block( const string &type, const string &name )
{
	int i, j ;

	string type2 = str_tolower( type ) ;
	if ( type2 == "" ) {
		// reference
		string name ;
		m_stream->get_string( name ) ;
		if ( !set_block_reference( name ) ) return false ;

	} else if ( type2 == "frame" ) {
		if ( !open_frame( name ) ) return false ;
		if ( !parse_recursive() ) return false ;
		if ( !close_frame() ) return false ;

	} else if ( type2 == "frametransformmatrix" ) {
		mat4 m ;
		m_stream->get_float( (float *)&m, 16 ) ;
		if ( !set_transform( m ) ) return false ;

	} else if ( type2 == "mesh" ) {
		vector<vec3> verts ;
		vector<vector<int> > faces ;
		int n_verts ;
		m_stream->get_int( n_verts ) ;
		for ( i = 0 ; i < n_verts ; i ++ ) {
			vec3 vert ;
			m_stream->get_float( (float *)&vert, 3 ) ;
			verts.push_back( vert ) ;
		}
		int n_faces ;
		m_stream->get_int( n_faces ) ;
		for ( i = 0 ; i < n_faces ; i ++ ) {
			vector<int> face ;
			int n_idxs ;
			m_stream->get_int( n_idxs ) ;
			m_stream->get_int( face, n_idxs ) ;
			faces.push_back( face ) ;
		}
		if ( !open_mesh( name ) ) return false ;
		if ( !set_vertices( verts, faces ) ) return false ;
		if ( !parse_recursive() ) return false ;
		if ( !close_mesh() ) return false ;

	} else if ( type2 == "meshnormals" ) {
		vector<vec3> norms ;
		vector<vector<int> > faces ;
		int n_norms ;
		m_stream->get_int( n_norms ) ;
		for ( i = 0 ; i < n_norms ; i ++ ) {
			vec3 norm ;
			m_stream->get_float( (float *)&norm, 3 ) ;
			norms.push_back( norm ) ;
		}
		int n_faces ;
		m_stream->get_int( n_faces ) ;
		for ( i = 0 ; i < n_faces ; i ++ ) {
			vector<int> face ;
			int n_idxs ;
			m_stream->get_int( n_idxs ) ;
			m_stream->get_int( face, n_idxs ) ;
			faces.push_back( face ) ;
		}
		if ( !set_normals( norms, faces ) ) return false ;

	} else if ( type2 == "meshtexturecoords" ) {
		vector<vec2> uvs ;
		int n_uvs ;
		m_stream->get_int( n_uvs ) ;
		for ( i = 0 ; i < n_uvs ; i ++ ) {
			vec2 uv ;
			m_stream->get_float( (float *)&uv, 2 ) ;
			uvs.push_back( uv ) ;
		}
		if ( !set_texture_coords( uvs ) ) return false ;

	} else if ( type2 == "meshvertexcolors" ) {
		vector<int> idxs ;
		vector<rgba8888> cols ;

		int n_cols ;
		m_stream->get_int( n_cols ) ;
		for ( i = 0 ; i < n_cols ; i ++ ) {
			int idx ;
			m_stream->get_int( idx ) ;

			vec4 col ;
			m_stream->get_float( (float *)&col, 4 ) ;
			idxs.push_back( idx ) ;
			cols.push_back( rgba8888( col ) ) ;
		}
		if ( !set_vertex_colors( idxs, cols ) ) return false ;

	} else if ( type2 == "skinweights" ) {
		string bone ;
		m_stream->get_string( bone ) ;
		bone = str_unquote( bone ) ;

		int n_weights ;
		m_stream->get_int( n_weights ) ;

		vector<int> idxs ;
		vector<float> wgts ;
		mat4 offset ;
		m_stream->get_int( idxs, n_weights ) ;
		m_stream->get_float( wgts, n_weights ) ;
		m_stream->get_float( (float *)&offset, 16 ) ;

		if ( !set_skin_weights( bone, idxs, wgts, offset ) ) return false ;

	} else if ( type2 == "meshmateriallist" ) {
		int n_mats, n_idxs ;
		m_stream->get_int( n_mats ) ;
		m_stream->get_int( n_idxs ) ;
		vector<int> idxs ;
		m_stream->get_int( idxs, n_idxs ) ;

		if ( !open_material_list() ) return false ;
		if ( !parse_recursive() ) return false ;
		if ( !set_material_indices( n_mats, idxs ) ) return false ;
		if ( !close_material_list() ) return false ;

	} else if ( type2 == "material" ) {
		vec4 diffuse ;
		float shininess ;
		vec3 specular, emissive ;
		m_stream->get_float( (float *)&diffuse, 4 ) ;
		m_stream->get_float( (float *)&shininess, 1 ) ;
		m_stream->get_float( (float *)&specular, 3 ) ;
		m_stream->get_float( (float *)&emissive, 3 ) ;
		if ( !open_material() ) return false ;
		if ( !set_material( diffuse, shininess, specular, emissive ) ) return false ;
		if ( !parse_recursive() ) return false ;
		if ( !close_material() ) return false ;

	} else if ( type2 == "texturefilename" ) {
		string filename ;
		m_stream->get_string( filename ) ;
		if ( !set_texture_filename( str_unquote( filename ) ) ) return false ;

	} else if ( type2 == "animationset" ) {
		if ( !open_animation_set( name ) ) return false ;
		if ( !parse_recursive() ) return false ;
		if ( !close_animation_set() ) return false ;

	} else if ( type2 == "animation" ) {
		if ( !open_animation() ) return false ;
		if ( !parse_recursive() ) return false ;
		if ( !close_animation() ) return false ;

	} else if ( type2 == "animationkey" ) {
		int type, n_keys ;
		m_stream->get_int( type ) ;
		m_stream->get_int( n_keys ) ;
		vector<int> frms ;
		vector<int> dims ;
		vector<float> vals ;
		for ( i = 0 ; i < n_keys ; i ++ ) {
			int frm, dim ;
			m_stream->get_int( frm ) ;
			m_stream->get_int( dim ) ;
			frms.push_back( frm ) ;
			dims.push_back( dim ) ;
			for ( j = 0 ; j < dim ; j ++ ) {
				float val ;
				m_stream->get_float( val ) ;
				vals.push_back( val ) ;
			}
		}
		if ( !set_animation_keys( type, frms, dims, vals ) ) return false ;

	} else if ( type2 == "template" ) {
	} else if ( type2 == "header" ) {
	} else if ( type2 == "vertexduplicationindices" ) {
	} else if ( type2 == "xskinmeshheader" ) {
		;

	} else {
		warning( str_format( "%s ( %d ) : unknown block-type \"%s\"\n",
			str_tailname( m_filename ).c_str(), m_block_from, type.c_str() ) ) ;
	}
	return true ;
}

//----------------------------------------------------------------
//  error callbacks
//----------------------------------------------------------------

void x_parser::error( const string &mesg )
{
	cout << "ERROR : " << mesg ;
}

void x_parser::warning( const string &mesg )
{
	cout << "WARNING : " << mesg ;
}

//----------------------------------------------------------------
//  geometry callbacks
//----------------------------------------------------------------

bool x_parser::open_frame( const string &name )
{
	cout << "open frame \"" << name << "\"\n" ;
	return true ;
}

bool x_parser::close_frame()
{
	cout << "close frame\n" ;
	return true ;
}

bool x_parser::set_transform( const mat4 &m )
{
	cout << "set transform\n" ;
	return true ;
}

bool x_parser::open_mesh( const string &name )
{
	cout << "open mesh \"" << name << "\"\n" ;
	return true ;
}

bool x_parser::close_mesh()
{
	cout << "close mesh\n" ;
	return true ;
}

bool x_parser::set_vertices( vector<vec3> &verts, vector<vector<int> > &faces )
{
	cout << "set vertices ( " << verts.size() << ", " << faces.size() << " )\n" ;
	return true ;
}

bool x_parser::set_normals( vector<vec3> &norms, vector<vector<int> > &faces )
{
	cout << "set normals ( " << norms.size() << ", " << faces.size() << " )\n" ;
	return true ;
}

bool x_parser::set_texture_coords( vector<vec2> &uvs )
{
	cout << "set texture coords ( " << uvs.size() << " )\n" ;
	return true ;
}

bool x_parser::set_vertex_colors( vector<int> &idxs, vector<rgba8888> &cols )
{
	cout << "set vertex colors ( " << idxs.size() << ", " << cols.size() << " )\n" ;
	return true ;
}

bool x_parser::set_skin_weights( const string &bone, vector<int> &idxs, vector<float> &wgts, const mat4 &offset )
{
	cout << "set skin weights \"" << bone << "\" ( " << idxs.size() << ", " << wgts.size() << " )\n" ;
	return true ;
}

//----------------------------------------------------------------
//  material callbacks
//----------------------------------------------------------------

bool x_parser::open_material_list()
{
	cout << "open material list\n" ;
	return true ;
}

bool x_parser::close_material_list()
{
	cout << "close material list\n" ;
	return true ;
}

bool x_parser::set_material_indices( int n_mats, vector<int> &idxs )
{
	cout << "set material indices ( " << n_mats << " )\n" ;
	return true ;
}

bool x_parser::open_material()
{
	cout << "open material\n" ;
	return true ;
}

bool x_parser::close_material()
{
	cout << "close material\n" ;
	return true ;
}

bool x_parser::set_material( const vec4 &diffuse, float shininess, const vec4 &specular, const vec4 &emissive )
{
	cout << "set material\n" ;
	return true ;
}

bool x_parser::set_texture_filename( const string &filename )
{
	cout << "set texture filename \"" << filename << "\"\n" ;
	return true ;
}

//----------------------------------------------------------------
//  animation callbacks
//----------------------------------------------------------------

bool x_parser::open_animation_set( const string &name )
{
	cout << "open animation set \"" << name << "\"\n" ;
	return true ;
}

bool x_parser::close_animation_set()
{
	cout << "close animation set\n" ;
	return true ;
}

bool x_parser::open_animation()
{
	cout << "open animation\n" ;
	return true ;
}

bool x_parser::close_animation()
{
	cout << "close animation\n" ;
	return true ;
}

bool x_parser::set_animation_keys( int type, vector<int> &frms, vector<int> &dims, vector<float> &vals )
{
	cout << "set animation keys ( " << type << " )\n" ;
	return true ;
}

bool x_parser::set_block_reference( const string &name )
{
	cout << "set block reference " << name << "\n" ;
	return true ;
}
