#ifndef MDX_STREAM_H_INCLUDE
#define MDX_STREAM_H_INCLUDE

#include "MdxMath.h"

namespace mdx {

class txt_ifstream ;
class txt_ofstream ;
class bin_ifstream ;
class bin_ofstream ;

//----------------------------------------------------------------
//  text char types
//----------------------------------------------------------------

enum {
	MDX_STREAM_CHAR_NORMAL		= 0,		// normal character
	MDX_STREAM_CHAR_BLANK		= 1,		// blank character
	MDX_STREAM_CHAR_QUOTATION	= 2,		// quotation character
	MDX_STREAM_CHAR_COMMENT		= 3,		// comment character
	MDX_STREAM_CHAR_SPECIAL		= 4,		// special character
} ;

//----------------------------------------------------------------
//  binary flags
//----------------------------------------------------------------

enum {
	MDX_STREAM_BIG_ENDIAN		= 0x0001,	// big endian
	MDX_STREAM_NO_NAME		= 0x0002,	// omit chunk name
} ;

//----------------------------------------------------------------
//  txt_ifstream
//----------------------------------------------------------------

class txt_ifstream : public ifstream {
public:
	txt_ifstream() ;
	~txt_ifstream() ;

	bool open( const string &filename ) ;
	void close() ;
	int get_char_type( int c ) const ;
	void set_char_type( int c, int type ) ;

	const string &get_filename() const ;
	int get_filesize() const ;
	bool is_eof() ;
	int get_pos() ;
	int set_pos( int pos ) ;

	int get_line_count() const ;
	void put_back( const string &word ) ;
	txt_ifstream &get_line( vector<string> &data, bool unquote = false ) ;

	txt_ifstream &get_string( vector<string> &data, int count, bool unquote = false ) ;
	txt_ifstream &get_string( string &data, bool unquote = false ) ;
	txt_ifstream &get_float( vector<float> &data, int count ) ;
	txt_ifstream &get_float( float *data, int count ) ;
	txt_ifstream &get_float( float &data ) ;
	txt_ifstream &get_int( vector<int> &data, int count ) ;
	txt_ifstream &get_int( int *data, int count ) ;
	txt_ifstream &get_int( int &data ) ;
	txt_ifstream &get_vec2( vec2 &v ) ;
	txt_ifstream &get_vec3( vec3 &v ) ;
	txt_ifstream &get_vec4( vec4 &v ) ;
	txt_ifstream &get_mat4( mat4 &m ) ;
	txt_ifstream &get_quat( quat &q ) ;
	txt_ifstream &get_rect( rect &r ) ;

	#if ( _MSC_VER >= 1400 )
	istream &read( void *buf, int size ) {
		if ( size == 0 ) return *this ;
		return ifstream::read( (char *)buf, size ) ;
	}
	#endif

protected:
	bool get_word( string &word, bool unquote = false ) ;

protected:
	string m_filename ;
	int m_filesize ;
	int m_line_count ;
	int m_line_back ;
	int m_line_top ;
	vector<string> m_put_backs ;
	char m_char_types[ 256 ] ;
} ;

//----------------------------------------------------------------
//  txt_ofstream
//----------------------------------------------------------------

class txt_ofstream : public ofstream {
public:
	txt_ofstream() ;
	~txt_ofstream() ;

	bool open( const string &filename ) ;
	void close() ;
	const string &get_line_feed() const ;
	void set_line_feed( const string &line_feed ) ;

	const string &get_filename() const ;
	int get_pos() ;
	int set_pos( int pos ) ;

	int get_indent() const ;
	void set_indent( int indent ) ;
	txt_ofstream &put_indent() ;
	txt_ofstream &put_newline( bool backslash = false ) ;

	txt_ofstream &put_string( vector<string> &data, bool quote = false ) ;
	txt_ofstream &put_string( const string &data, bool quote = false ) ;
	txt_ofstream &put_float( vector<float> &data ) ;
	txt_ofstream &put_float( const float *data, int count ) ;
	txt_ofstream &put_float( float data ) ;
	txt_ofstream &put_int( vector<int> &data ) ;
	txt_ofstream &put_int( const int *data, int count ) ;
	txt_ofstream &put_int( int data ) ;
	txt_ofstream &put_vec2( const vec2 &v ) ;
	txt_ofstream &put_vec3( const vec3 &v ) ;
	txt_ofstream &put_vec4( const vec4 &v ) ;
	txt_ofstream &put_mat4( const mat4 &m ) ;
	txt_ofstream &put_quat( const quat &q ) ;
	txt_ofstream &put_rect( const rect &r ) ;

	#if ( _MSC_VER >= 1400 )
	ostream &write( const void *buf, int size ) {
		if ( size == 0 ) return *this ;
		return ofstream::write( (const char *)buf, size ) ;
	}
	#endif

protected:
	void put_word_separator() ;

protected:
	string m_filename ;
	int m_indent ;
	bool m_first_word ;
	string m_line_feed ;
} ;

//----------------------------------------------------------------
//  bin_ifstream
//----------------------------------------------------------------

class bin_ifstream : public ifstream {
public:
	bin_ifstream() ;
	~bin_ifstream() ;

	bool open( const string &filename, int flags = 0 ) ;
	void close() ;
	int get_flags() const ;
	void set_flags( int flags ) ;

	const string &get_filename() const ;
	int get_filesize() const ;
	bool is_eof() ;
	int get_pos() ;
	int set_pos( int pos ) ;
	int align_pos( int align, int offset = 0 ) ;

	bool open_chunk( int &type, string &name ) ;
	bool peek_chunk( int &type, string &name ) ;
	void close_chunk() ;
	bool open_chunk_args( int &size ) ;
	void close_chunk_args() ;
	bool open_chunk_data( int &size ) ;
	void close_chunk_data() ;

	bin_ifstream &get_string( vector<string> &data, int count, int limit = -1 ) ;
	bin_ifstream &get_string( string &data, int limit = -1 ) ;
	bin_ifstream &get_float( vector<float> &data, int count ) ;
	bin_ifstream &get_float( float *data, int count ) ;
	bin_ifstream &get_float( float &data ) ;
	bin_ifstream &get_int32( vector<int> &data, int count ) ;
	bin_ifstream &get_int32( int *data, int count ) ;
	bin_ifstream &get_int32( int &data ) ;
	bin_ifstream &get_int16( vector<int> &data, int count ) ;
	bin_ifstream &get_int16( int *data, int count ) ;
	bin_ifstream &get_int16( int &data ) ;
	bin_ifstream &get_int8( vector<int> &data, int count ) ;
	bin_ifstream &get_int8( int *data, int count ) ;
	bin_ifstream &get_int8( int &data ) ;
	bin_ifstream &get_uint32( vector<int> &data, int count ) ;
	bin_ifstream &get_uint32( int *data, int count ) ;
	bin_ifstream &get_uint32( int &data ) ;
	bin_ifstream &get_uint16( vector<int> &data, int count ) ;
	bin_ifstream &get_uint16( int *data, int count ) ;
	bin_ifstream &get_uint16( int &data ) ;
	bin_ifstream &get_uint8( vector<int> &data, int count ) ;
	bin_ifstream &get_uint8( int *data, int count ) ;
	bin_ifstream &get_uint8( int &data ) ;
	bin_ifstream &get_vec2( vec2 &v ) ;
	bin_ifstream &get_vec3( vec3 &v ) ;
	bin_ifstream &get_vec4( vec4 &v ) ;
	bin_ifstream &get_mat4( mat4 &m ) ;
	bin_ifstream &get_quat( quat &q ) ;
	bin_ifstream &get_rect( rect &r ) ;

	#if ( _MSC_VER >= 1400 )
	istream &read( void *buf, int size ) {
		if ( size == 0 ) return *this ;
		return ifstream::read( (char *)buf, size ) ;
	}
	#endif

protected:
	struct chunk_info {
		int m_args ;		// offset to chunk-args
		int m_data ;		// offset to chunk-data
		int m_child ;		// offset to child-chunk
		int m_next ;		// offset to next-chunk
	} ;

protected:
	string m_filename ;
	int m_filesize ;
	int m_flags ;
	bool m_swap_bytes ;
	vector<chunk_info> m_chunk_stack ;
} ;

//----------------------------------------------------------------
//  bin_ofstream
//----------------------------------------------------------------

class bin_ofstream : public ofstream {
public:
	bin_ofstream() ;
	~bin_ofstream() ;

	bool open( const string &filename, int flags = 0 ) ;
	void close() ;
	int get_flags() const ;
	void set_flags( int flags ) ;

	const string &get_filename() const ;
	int get_pos() ;
	int set_pos( int pos ) ;
	int align_pos( int align, int offset = 0 ) ;

	bool open_chunk( int type, const string &name = "" ) ;
	bool open_short_chunk( int type ) ;
	void close_chunk() ;
	bool open_chunk_args() ;
	void close_chunk_args() ;
	bool open_chunk_data() ;
	void close_chunk_data() ;

	bin_ofstream &put_string( vector<string> &data ) ;
	bin_ofstream &put_string( const string &data ) ;
	bin_ofstream &put_float( vector<float> &data ) ;
	bin_ofstream &put_float( const float *data, int count ) ;
	bin_ofstream &put_float( float data ) ;
	bin_ofstream &put_int32( vector<int> &data ) ;
	bin_ofstream &put_int32( const int *data, int count ) ;
	bin_ofstream &put_int32( int data ) ;
	bin_ofstream &put_int16( vector<int> &data ) ;
	bin_ofstream &put_int16( const int *data, int count ) ;
	bin_ofstream &put_int16( int data ) ;
	bin_ofstream &put_int8( vector<int> &data ) ;
	bin_ofstream &put_int8( const int *data, int count ) ;
	bin_ofstream &put_int8( int data ) ;
	bin_ofstream &put_uint32( vector<int> &data ) ;
	bin_ofstream &put_uint32( const int *data, int count ) ;
	bin_ofstream &put_uint32( int data ) ;
	bin_ofstream &put_uint16( vector<int> &data ) ;
	bin_ofstream &put_uint16( const int *data, int count ) ;
	bin_ofstream &put_uint16( int data ) ;
	bin_ofstream &put_uint8( vector<int> &data ) ;
	bin_ofstream &put_uint8( const int *data, int count ) ;
	bin_ofstream &put_uint8( int data ) ;
	bin_ofstream &put_vec2( const vec2 &v ) ;
	bin_ofstream &put_vec3( const vec3 &v ) ;
	bin_ofstream &put_vec4( const vec4 &v ) ;
	bin_ofstream &put_mat4( const mat4 &m ) ;
	bin_ofstream &put_quat( const quat &q ) ;
	bin_ofstream &put_rect( const rect &r ) ;

	#if ( _MSC_VER >= 1400 )
	ostream &write( const void *buf, int size ) {
		if ( size == 0 ) return *this ;
		return ofstream::write( (const char *)buf, size ) ;
	}
	#endif

protected:
	void fix_chunk() ;

protected:
	struct chunk_info {
		bool m_short ;		// short-chunk flag
		int m_base ;		// offset to chunk-header
		int m_args ;		// offset to chunk-args
		int m_data ;		// offset to chunk-data
		int m_child ;		// offset to child-chunk
	} ;

protected:
	string m_filename ;
	int m_flags ;
	bool m_swap_bytes ;
	vector<chunk_info> m_chunk_stack ;
	int m_chunk_level ;
} ;


} // namespace mdx

#endif
