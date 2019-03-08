#include "ModelUtil.h"

namespace mdx {

#if ( MDX_IGNORE_SJIS )
#define	SJIS_BIT	0x00
#else // MDX_IGNORE_SJIS
#define	SJIS_BIT	0x80
#endif // MDX_IGNORE_SJIS

//----------------------------------------------------------------
//  txt_ifstream
//----------------------------------------------------------------

txt_ifstream::txt_ifstream()
{
	m_filename = "" ;
	m_filesize = 0 ;
	m_line_count = 0 ;
	m_line_back = 0 ;
	m_line_top = 0 ;
	m_put_backs.clear() ;

	memset( m_char_types, MDX_STREAM_CHAR_NORMAL, sizeof( m_char_types ) ) ;
	m_char_types[ ' ' ] = MDX_STREAM_CHAR_BLANK ;
	m_char_types[ '\t' ] = MDX_STREAM_CHAR_BLANK ;
	m_char_types[ ',' ] = MDX_STREAM_CHAR_BLANK ;
	m_char_types[ ';' ] = MDX_STREAM_CHAR_BLANK ;
	m_char_types[ '"' ] = MDX_STREAM_CHAR_QUOTATION ;
	m_char_types[ '#' ] = MDX_STREAM_CHAR_COMMENT ;
	m_char_types[ '{' ] = MDX_STREAM_CHAR_SPECIAL ;
	m_char_types[ '}' ] = MDX_STREAM_CHAR_SPECIAL ;
}

txt_ifstream::~txt_ifstream()
{
	close() ;
}

bool txt_ifstream::open( const string &filename )
{
	close() ;
	ifstream::open( filename.c_str(), ios::in | ios::binary ) ;
	if ( !is_open() ) {
		ifstream::close() ;
		ifstream::clear() ;
		return false ;
	}
	m_filename = filename ;
	m_line_count = 1 ;
	m_line_back = 1 ;
	m_line_top = 1 ;
	seekg( 0, ios::end ) ;
	m_filesize = tellg() ;
	seekg( 0, ios::beg ) ;
	return true ;
}

void txt_ifstream::close()
{
	ifstream::close() ;
	ifstream::clear() ;
	m_put_backs.clear() ;
}

int txt_ifstream::get_char_type( int c ) const
{
	return m_char_types[ 255 & c ] ;
}

void txt_ifstream::set_char_type( int c, int type )
{
	m_char_types[ 255 & c ] = type ;
}

const string &txt_ifstream::get_filename() const
{
	return m_filename ;
}

int txt_ifstream::get_filesize() const
{
	return m_filesize ;
}

bool txt_ifstream::is_eof()
{
	if ( !m_put_backs.empty() ) return false ;
	return ( get_pos() < 0 || get_pos() >= get_filesize() ) ;
}

int txt_ifstream::get_pos()
{
	return tellg() ;
}

int txt_ifstream::set_pos( int pos )
{
	m_put_backs.clear() ;

	seekg( pos ) ;
	return tellg() ;
}

int txt_ifstream::get_line_count() const
{
	return m_line_count ;
}

void txt_ifstream::put_back( const string &word )
{
	if ( m_put_backs.empty() ) {
		int count = m_line_count ;
		m_line_count = m_line_back ;
		m_line_back = count ;
	}
	m_put_backs.push_back( word ) ;
}

txt_ifstream &txt_ifstream::get_line( vector<string> &data, bool unquote )
{
	data.clear() ;
	string word ;
	int first_line = -1 ;
	int prev_line = -1 ;
	while ( !is_eof() ) {
		get_string( word, unquote ) ;
		if ( prev_line < 0 ) prev_line = get_line_count() ;
		if ( m_line_count != prev_line ) {
			put_back( word ) ;
			break ;
		}
		if ( word == "\n" || word == "\r" ) {
			prev_line ++ ;
			continue ;
		}
		if ( data.empty() ) first_line = get_line_count() ;
		data.push_back( word ) ;
	}
	if ( first_line >= 0 ) {
		m_line_count = first_line ;
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_string( vector<string> &data, int count, bool unquote )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		get_word( data[ i ], unquote ) ;
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_string( string &data, bool unquote )
{
	get_word( data, unquote ) ;
	return *this ;
}

txt_ifstream &txt_ifstream::get_float( vector<float> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		string word ;
		if ( get_word( word ) ) {
			data.push_back( str_atof( word ) ) ;
		} else {
			data.push_back( 0.0f ) ;
		}
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_float( float *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		string word ;
		if ( get_word( word ) ) {
			data[ i ] = str_atof( word ) ;
		} else {
			data[ i ] = 0.0f ;
		}
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_float( float &data )
{
	string word ;
	if ( get_word( word ) ) {
		data = str_atof( word ) ;
	} else {
		data = 0.0f ;
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_int( vector<int> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		string word ;
		if ( get_word( word ) ) {
			data.push_back( str_atoi( word ) ) ;
		} else {
			data.push_back( 0 ) ;
		}
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_int( int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		string word ;
		if ( get_word( word ) ) {
			data[ i ] = str_atoi( word ) ;
		} else {
			data[ i ] = 0 ;
		}
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_int( int &data )
{
	string word ;
	if ( get_word( word ) ) {
		data = str_atoi( word ) ;
	} else {
		data = 0 ;
	}
	return *this ;
}

txt_ifstream &txt_ifstream::get_vec2( vec2 &v )
{
	return get_float( (float *)&v, 2 ) ;
}

txt_ifstream &txt_ifstream::get_vec3( vec3 &v )
{
	return get_float( (float *)&v, 3 ) ;
}

txt_ifstream &txt_ifstream::get_vec4( vec4 &v )
{
	return get_float( (float *)&v, 4 ) ;
}

txt_ifstream &txt_ifstream::get_mat4( mat4 &m )
{
	return get_float( (float *)&m, 16 ) ;
}

txt_ifstream &txt_ifstream::get_quat( quat &q )
{
	return get_float( (float *)&q, 4 ) ;
}

txt_ifstream &txt_ifstream::get_rect( rect &r )
{
	return get_float( (float *)&r, 4 ) ;
}

bool txt_ifstream::get_word( string &word, bool unquote )
{
	//  builtin rules
	//    linebreak	= cr lf cr+lf
	//    escape	= backslash
	//    comment	= // /* */

	if ( !m_put_backs.empty() ) {
		word = m_put_backs.back() ;
		m_put_backs.pop_back() ;
		m_line_count = m_line_back ;
		return true ;
	}

	int comment_mode = 0 ;

	word = "" ;
	char c, t ;
	for ( ; ; ) {
		if ( !get( c ) ) return false ;
		t = m_char_types[ 255 & c ] ;

		if ( comment_mode == 0 ) {
			if ( c == '\n' || c == '\r' ) {
				if ( c == '\r' ) {		// crlf -> cr
					char c2 ;
					if ( !get( c2 ) ) return false ;
					if ( c2 != '\n' ) ifstream::putback( c2 ) ;
				}
				m_line_top ++ ;

			} else if ( c == '/' ) {
				if ( !get( c ) ) break ;
				if ( c == '/' ) {
					comment_mode = 1 ;
				} else if ( c == '*' ) {
					comment_mode = 2 ;
				} else {
					ifstream::putback( c ) ;
					ifstream::putback( '/' ) ;
					break ;
				}

			} else if ( t == MDX_STREAM_CHAR_BLANK ) {
				;

			} else if ( t == MDX_STREAM_CHAR_COMMENT ) {
				comment_mode = 1 ;

			} else {
				ifstream::putback( c ) ;
				break ;
			}
		} else {
			if ( c == '\n' || c == '\r' ) {
				if ( c == '\r' ) {		// crlf -> cr
					char c2 ;
					if ( !get( c2 ) ) return false ;
					if ( c2 != '\n' ) ifstream::putback( c2 ) ;
				}
				if ( comment_mode == 1 ) {
					comment_mode = 0 ;
				}
				m_line_top ++ ;

			} else if ( c == '*' && comment_mode == 2 ) {
				if ( !get( c ) ) break ;
				if ( c == '/' ) {
					comment_mode = 0 ;
				} else {
					ifstream::putback( c ) ;
				}
			}
		}
	}

	m_line_back = m_line_count ;
	m_line_count = m_line_top ;

	int sjiskanji_mode = 0 ;
	int backslash_mode = 0 ;
	char q = '\0' ;

	for ( ; ; ) {
		if ( !get( c ) ) break ;
		t = m_char_types[ 255 & c ] ;

		if ( sjiskanji_mode ) {
			word += c ;
			sjiskanji_mode = 0 ;

		} else if ( SJIS_BIT & c ) {
			if ( backslash_mode ) {
				word += '\\' ;
				backslash_mode = 0 ;
			}
			word += c ;
			sjiskanji_mode = 1 ;

		} else if ( backslash_mode ) {
			if ( c == '\n' || c == '\r' ) {		// line-break
				if ( c == '\r' ) {		// crlf -> cr
					char c2 ;
					if ( !get( c2 ) ) return false ;
					if ( c2 != '\n' ) ifstream::putback( c2 ) ;
				}
				if ( word != "" ) {
					ifstream::putback( c ) ;
					ifstream::putback( '\\' ) ;
				} else {
					word += c ;
					m_line_top ++ ;
				}
				break ;	
			} else {
				if ( c != '\\' && c != '"' ) word += '\\' ;
				word += c ;
			}
			backslash_mode = 0 ;

		} else if ( c == '\\' ) {
			backslash_mode = 1 ;

		} else if ( q != '\0' ) {
			if ( c == q ) {
				if ( unquote == false ) word = str_quote( word, q ) ;
				q = '\0' ;
				break ;
			} else {
				word += c ;
			}

		} else if ( c == '\n' || c == '\r' ) {
			if ( c == '\r' ) {		// crlf -> cr
				char c2 ;
				if ( !get( c2 ) ) return false ;
				if ( c2 != '\n' ) ifstream::putback( c2 ) ;
			}
			m_line_top ++ ;
			break ;

		} else if ( t == MDX_STREAM_CHAR_BLANK ) {
			break ;

		} else if ( t == MDX_STREAM_CHAR_QUOTATION ) {
			q = c ;

		} else if ( t == MDX_STREAM_CHAR_SPECIAL ) {
			if ( word != "" ) {
				ifstream::putback( c ) ;
				break ;
			} else {
				word += c ;
				break ;
			}
		} else {
			word += c ;
		}
	}
	return ( word != "" ) ;
}

//----------------------------------------------------------------
//  txt_ofstream
//----------------------------------------------------------------

txt_ofstream::txt_ofstream()
{
	m_filename = "" ;
	m_indent = 0 ;
	m_first_word = true ;
	m_line_feed = "\n" ;
}

txt_ofstream::~txt_ofstream()
{
	close() ;
}

bool txt_ofstream::open( const string &filename )
{
	close() ;
	ofstream::open( filename.c_str(), ios::out | ios::binary ) ;
	if ( !is_open() ) {
		ofstream::close() ;
		ofstream::clear() ;
		return false ;
	}
	m_filename = filename ;
	setf( ios::fixed ) ;
	return true ;
}

void txt_ofstream::close()
{
	ofstream::close() ;
	ofstream::clear() ;
	m_indent = 0 ;
	m_first_word = true ;
}

const string &txt_ofstream::get_line_feed() const
{
	return m_line_feed ;
}

void txt_ofstream::set_line_feed( const string &line_feed )
{
	m_line_feed = line_feed ;
}

const string &txt_ofstream::get_filename() const
{
	return m_filename ;
}

int txt_ofstream::get_pos()
{
	return tellp() ;
}

int txt_ofstream::set_pos( int pos )
{
	seekp( pos ) ;
	return tellp() ;
}

int txt_ofstream::get_indent() const
{
	return m_indent ;
}

void txt_ofstream::set_indent( int indent )
{
	m_indent = indent ;
}

txt_ofstream &txt_ofstream::put_indent()
{
	for ( int i = 0 ; i < m_indent ; i ++ ) put( '\t' ) ;
	m_first_word = true ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_newline( bool backslash )
{
	if ( backslash ) *this << " \\" ;
	*this << m_line_feed ;
	m_first_word = true ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_string( vector<string> &data, bool quote )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_string( data[ i ], quote ) ;
	}
	return *this ;
}

txt_ofstream &txt_ofstream::put_string( const string &data, bool quote )
{
	put_word_separator() ;
	if ( quote ) {
		*this << "\"" << data << "\"" ;
	} else {
		*this << data ;
	}
	return *this ;
}

txt_ofstream &txt_ofstream::put_float( vector<float> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_float( data[ i ] ) ;
	}
	return *this ;
}

txt_ofstream &txt_ofstream::put_float( const float *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		put_float( data[ i ] ) ;
	}
	return *this ;
}

txt_ofstream &txt_ofstream::put_float( float data )
{
	put_word_separator() ;
	if ( data == -0.0f ) data = 0.0f ;
	*this << data ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_int( vector<int> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_int( data[ i ] ) ;
	}
	return *this ;
}

txt_ofstream &txt_ofstream::put_int( const int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		put_int( data[ i ] ) ;
	}
	return *this ;
}

txt_ofstream &txt_ofstream::put_int( int data )
{
	put_word_separator() ;
	*this << data ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_vec2( const vec2 &v )
{
	put_float( (const float *)&v, 2 ) ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_vec3( const vec3 &v )
{
	put_float( (const float *)&v, 3 ) ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_vec4( const vec4 &v )
{
	put_float( (const float *)&v, 4 ) ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_mat4( const mat4 &m )
{
	const float *fp = (const float *)&m ;
	put_float( fp + 0, 4 ) << " \\" << m_line_feed ;
	set_indent( get_indent() + 1 ) ;
	put_indent() ;
	put_float( fp + 4, 4 ) << " \\" << m_line_feed ;
	put_indent() ;
	put_float( fp + 8, 4 ) << " \\" << m_line_feed ;
	put_indent() ;
	put_float( fp + 12, 4 ) ;
	set_indent( get_indent() - 1 ) ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_quat( const quat &v )
{
	put_float( (const float *)&v, 4 ) ;
	return *this ;
}

txt_ofstream &txt_ofstream::put_rect( const rect &v )
{
	put_float( (const float *)&v, 4 ) ;
	return *this ;
}

void txt_ofstream::put_word_separator()
{
	if ( !m_first_word ) *this << ' ' ;
	m_first_word = false ;
}

//----------------------------------------------------------------
//  bin_ifstream
//----------------------------------------------------------------

bin_ifstream::bin_ifstream()
{
	m_filename = "" ;
	m_filesize = 0 ;
	m_flags = 0 ;
	m_swap_bytes = false ;
	m_chunk_stack.clear() ;
}

bin_ifstream::~bin_ifstream()
{
	close() ;
}

bool bin_ifstream::open( const string &filename, int flags )
{
	close() ;
	ifstream::open( filename.c_str(), ios::in | ios::binary ) ;
	if ( !is_open() ) {
		ifstream::close() ;
		ifstream::clear() ;
		return false ;
	}
	m_filename = filename ;
	set_flags( flags ) ;
	m_chunk_stack.clear() ;

	seekg( 0, ios::end ) ;
	m_filesize = tellg() ;
	seekg( 0, ios::beg ) ;
	return true ;
}

void bin_ifstream::close()
{
	ifstream::close() ;
	ifstream::clear() ;
	m_chunk_stack.clear() ;
}

int bin_ifstream::get_flags() const
{
	return m_flags ;
}

void bin_ifstream::set_flags( int flags )
{
	m_flags = flags ;
	m_swap_bytes = ( ( MDX_STREAM_BIG_ENDIAN & flags ) != 0 ) ;
}

const string &bin_ifstream::get_filename() const
{
	return m_filename ;
}

int bin_ifstream::get_filesize() const
{
	return m_filesize ;
}

bool bin_ifstream::is_eof()
{
	return ( get_pos() < 0 || get_pos() >= get_filesize() ) ;
}

int bin_ifstream::get_pos()
{
	return tellg() ;
}

int bin_ifstream::set_pos( int pos )
{
	seekg( pos ) ;
	return tellg() ;
}

int bin_ifstream::align_pos( int align, int offset )
{
	int pos = get_pos() ;
	if ( pos < 0 || align < 1 ) return pos ;
	int pad = ( offset - pos ) % align ;
	if ( pad < 0 ) pad += align ;
	set_pos( pos += pad ) ;
	return pos ;
}

bool bin_ifstream::open_chunk( int &type, string &name )
{
	if ( !is_open() ) return false ;

	int base = get_pos() ;
	int max = m_filesize ;
	if ( base % 4 != 0 ) {
		cout << "WARNING : chunk alignment is wrong\n" ;
	}
	if ( !m_chunk_stack.empty() ) {
		const chunk_info &info = m_chunk_stack.back() ;
		if ( base < info.m_child ) {
			cout << "ERROR : subchunk offset is wrong\n" ;
			return false ;
		}
		max = info.m_next ;
	}
	if ( base == max ) {
		return false ;
	}

	chunk_info info ;
	int name_size = 0 ;
	int tag_size = 8 ;

	if ( base + 8 > max ) {
		cout << "ERROR : unexpected eof\n" ;
		return false ;
	}
	get_uint16( type ) ;
	get_uint16( info.m_args ) ;
	get_uint32( info.m_next ) ;
	if ( 0x8000 & type ) {
		type &= ~0x8000 ;
		if ( info.m_args == 0 ) info.m_args = tag_size ;
		info.m_args += base ;
		info.m_next += base ;
		info.m_child = info.m_next ;	// no child-chunk
		info.m_data = info.m_child ;	// no chunk-data
	} else {
		tag_size = 16 ;
		if ( info.m_args == 0 ) info.m_args = tag_size ;
		name_size = info.m_args - tag_size ;
		if ( base + tag_size > max ) {
			cout << "ERROR : unexpected eof\n" ;
			set_pos( base ) ;
			return false ;
		}
		get_uint32( info.m_child ) ;
		get_uint32( info.m_data ) ;
		if ( info.m_child == 0 ) info.m_child = info.m_next ;
		if ( info.m_data == 0 ) info.m_data = info.m_child ;
		info.m_args += base ;
		info.m_next += base ;
		info.m_child += base ;
		info.m_data += base ;
	}
	if ( info.m_args < base + tag_size
	  || info.m_args > info.m_data
	  || info.m_data > info.m_child
	  || info.m_child > info.m_next
	  || info.m_next > max ) {
		cout << "ERROR : wrong offset\n" ;
		set_pos( base ) ;
		return false ;
	}
	if ( info.m_args % 4 != 0
	  || info.m_next % 4 != 0
	  || info.m_child % 4 != 0
	  || info.m_data % 4 != 0 ) {
		cout << "WARNING : wrong offset alignment\n" ;
	}

	m_chunk_stack.push_back( info ) ;

	name = "" ;
	if ( name_size > 0 ) {
		get_string( name, name_size ) ;
		if ( MDX_STREAM_NO_NAME & m_flags ) name = "" ;
	}

	set_pos( info.m_args ) ;
	return true ;
}

bool bin_ifstream::peek_chunk( int &type, string &name )
{
	int pos = get_pos() ;
	if ( open_chunk( type, name ) ) {
		close_chunk() ;
		set_pos( pos ) ;
		return true ;
	}
	return false ;
}

void bin_ifstream::close_chunk()
{
	if ( m_chunk_stack.empty() ) return ;
	set_pos( m_chunk_stack.back().m_next ) ;
	m_chunk_stack.pop_back() ;
}

bool bin_ifstream::open_chunk_args( int &size )
{
	if ( m_chunk_stack.empty() ) return false ;
	const chunk_info &info = m_chunk_stack.back() ;
	set_pos( info.m_args ) ;
	size = info.m_data - info.m_args ;
	return ( size > 0 ) ;
}

void bin_ifstream::close_chunk_args()
{
	if ( m_chunk_stack.empty() ) return ;
	set_pos( m_chunk_stack.back().m_data ) ;
}

bool bin_ifstream::open_chunk_data( int &size )
{
	if ( m_chunk_stack.empty() ) return false ;
	const chunk_info &info = m_chunk_stack.back() ;
	set_pos( info.m_data ) ;
	size = info.m_child - info.m_data ;
	return ( size > 0 ) ;
}

void bin_ifstream::close_chunk_data()
{
	if ( m_chunk_stack.empty() ) return ;
	set_pos( m_chunk_stack.back().m_child ) ;
}

bin_ifstream &bin_ifstream::get_string( vector<string> &data, int count, int limit )
{
	data.clear() ;
	if ( limit < 0 ) limit = 0x7fffffff ;
	for ( int i = 0 ; i < count ; i ++ ) {
		get_string( data[ i ], limit ) ;
		limit -= data[ i ].length() + 1 ;
		if ( limit < 0 ) limit = 0 ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_string( string &data, int limit )
{
	data = "" ;
	char c = '\0' ;
	if ( limit < 0 ) limit = 0x7fffffff ;
	while ( -- limit >= 0 ) {
		get( c ) ;
		if ( c == '\0' ) break ;
		data += c ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_float( vector<float> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		float val ;
		get_float( val ) ;
		data.push_back( val ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_float( float *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		get_float( data[ i ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_float( float &data )
{
	if ( !m_swap_bytes ) {
		read( (char *)&data, sizeof( float ) ) ;
	} else {
		char *cp = (char *)&data ;
		get( cp[ 3 ] ) ;
		get( cp[ 2 ] ) ;
		get( cp[ 1 ] ) ;
		get( cp[ 0 ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int32( vector<int> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		int val ;
		get_int32( val ) ;
		data.push_back( val ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int32( int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		get_int32( data[ i ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int32( int &data )
{
	if ( !m_swap_bytes ) {
		read( (char *)&data, sizeof( int ) ) ;
	} else {
		char *cp = (char *)&data ;
		get( cp[ 3 ] ) ;
		get( cp[ 2 ] ) ;
		get( cp[ 1 ] ) ;
		get( cp[ 0 ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int16( vector<int> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		int val ;
		get_int16( val ) ;
		data.push_back( val ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int16( int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		get_int16( data[ i ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int16( int &data )
{
	short tmp = data ;
	if ( !m_swap_bytes ) {
		read( (char *)&tmp, sizeof( short ) ) ;
	} else {
		char *cp = (char *)&tmp ;
		get( cp[ 1 ] ) ;
		get( cp[ 0 ] ) ;
	}
	data = tmp ;
	return *this ;
}

bin_ifstream &bin_ifstream::get_int8( vector<int> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		int val ;
		get_int8( val ) ;
		data.push_back( val ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int8( int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		get_int8( data[ i ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_int8( int &data )
{
	signed char tmp = data ;
	read( (char *)&tmp, sizeof( char ) ) ;
	data = tmp ;
	return *this ;
}

bin_ifstream &bin_ifstream::get_uint32( vector<int> &data, int count )
{
	return get_int32( data, count ) ;
}

bin_ifstream &bin_ifstream::get_uint32( int *data, int count )
{
	return get_int32( data, count ) ;
}

bin_ifstream &bin_ifstream::get_uint32( int &data )
{
	return get_int32( data ) ;
}

bin_ifstream &bin_ifstream::get_uint16( vector<int> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		int val ;
		get_uint16( val ) ;
		data.push_back( val ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_uint16( int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		get_uint16( data[ i ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_uint16( int &data )
{
	unsigned short tmp = data ;
	if ( !m_swap_bytes ) {
		read( (char *)&tmp, sizeof( short ) ) ;
	} else {
		char *cp = (char *)&tmp ;
		get( cp[ 1 ] ) ;
		get( cp[ 0 ] ) ;
	}
	data = tmp ;
	return *this ;
}

bin_ifstream &bin_ifstream::get_uint8( vector<int> &data, int count )
{
	data.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		int val ;
		get_uint8( val ) ;
		data.push_back( val ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_uint8( int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		get_uint8( data[ i ] ) ;
	}
	return *this ;
}

bin_ifstream &bin_ifstream::get_uint8( int &data )
{
	unsigned char tmp = data ;
	read( (char *)&tmp, sizeof( char ) ) ;
	data = tmp ;
	return *this ;
}

bin_ifstream &bin_ifstream::get_vec2( vec2 &v )
{
	return get_float( (float *)&v, 2 ) ;
}

bin_ifstream &bin_ifstream::get_vec3( vec3 &v )
{
	return get_float( (float *)&v, 3 ) ;
}

bin_ifstream &bin_ifstream::get_vec4( vec4 &v )
{
	return get_float( (float *)&v, 4 ) ;
}

bin_ifstream &bin_ifstream::get_mat4( mat4 &m )
{
	return get_float( (float *)&m, 16 ) ;
}

bin_ifstream &bin_ifstream::get_quat( quat &q )
{
	return get_float( (float *)&q, 4 ) ;
}

bin_ifstream &bin_ifstream::get_rect( rect &r )
{
	return get_float( (float *)&r, 4 ) ;
}

//----------------------------------------------------------------
//  bin_ofstream
//----------------------------------------------------------------

bin_ofstream::bin_ofstream()
{
	m_filename = "" ;
	m_flags = 0 ;
	m_swap_bytes = false ;
	m_chunk_stack.clear() ;
	m_chunk_level = 0 ;
}

bin_ofstream::~bin_ofstream()
{
	close() ;
}

bool bin_ofstream::open( const string &filename, int flags )
{
	close() ;
	ofstream::open( filename.c_str(), ios::out | ios::binary ) ;
	if ( !is_open() ) {
		ofstream::close() ;
		ofstream::clear() ;
		return false ;
	}
	m_filename = filename ;
	set_flags( flags ) ;
	m_chunk_stack.clear() ;
	m_chunk_level = 0 ;
	return true ;
}

void bin_ofstream::close()
{
	m_chunk_level = 0 ;
	fix_chunk() ;

	ofstream::close() ;
	ofstream::clear() ;
}

int bin_ofstream::get_flags() const
{
	return m_flags ;
}

void bin_ofstream::set_flags( int flags )
{
	m_flags = flags ;
	m_swap_bytes = ( ( MDX_STREAM_BIG_ENDIAN & flags ) != 0 ) ;
}

const string &bin_ofstream::get_filename() const
{
	return m_filename ;
}

int bin_ofstream::get_pos()
{
	return tellp() ;
}

int bin_ofstream::set_pos( int pos )
{
	seekp( pos ) ;
	return tellp() ;
}

int bin_ofstream::align_pos( int align, int offset )
{
	int pos = get_pos() ;
	if ( pos < 0 || align < 1 ) return pos ;
	int pad = ( offset - pos ) % align ;
	if ( pad < 0 ) pad += align ;
	while ( -- pad >= 0 ) {
		put( '\0' ) ;
		pos ++ ;
	}
	return pos ;
}

bool bin_ofstream::open_chunk( int type, const string &name )
{
	fix_chunk() ;

	chunk_info info ;
	info.m_short = ( ( 0x8000 & type ) != 0 ) ;
	info.m_base = get_pos() ;
	info.m_args = 0 ;
	info.m_data = 0 ;
	info.m_child = 0 ;
	m_chunk_stack.push_back( info ) ;
	m_chunk_level ++ ;

	if ( !info.m_short ) {
		put_int16( type ) ;		// chunk-type
		put_int16( 0 ) ;		// offset to name-end
		put_int32( 0 ) ;		// offset to args-end
		put_int32( 0 ) ;		// offset to data-end
		put_int32( 0 ) ;		// offset to child-end
		if ( !( MDX_STREAM_NO_NAME & m_flags ) && name != "" ) {
			put_string( name ) ;
			align_pos( 4 ) ;
		}
	} else {
		put_int16( type | 0x8000 ) ;	// chunk-type
		put_int16( 0 ) ;		// offset to args-end
	}
	return true ;
}

bool bin_ofstream::open_short_chunk( int type )
{
	return open_chunk( 0x8000 | type ) ;
}

void bin_ofstream::close_chunk()
{
	if ( m_chunk_level > 0 ) m_chunk_level -- ;
}

bool bin_ofstream::open_chunk_args()
{
	if ( m_chunk_stack.empty() ) {
		cout << "ERROR : cannot open chunk-args outof chunk\n" ;
		return false ;
	}
	chunk_info &parent = m_chunk_stack.back() ;
	if ( parent.m_args != 0 ) {
		cout << "ERROR : cannot open chunk-args redundantly\n" ;
		return false ;
	}
	align_pos( 4 ) ;
	parent.m_args = get_pos() ;
	return true ;
}

void bin_ofstream::close_chunk_args()
{
	;
}

bool bin_ofstream::open_chunk_data()
{
	if ( m_chunk_stack.empty() ) {
		cout << "ERROR : cannot open chunk-data outof chunk\n" ;
		return false ;
	}
	chunk_info &parent = m_chunk_stack.back() ;
	if ( parent.m_short ) {
		cout << "ERROR : cannot open chunk-data in short-chunk\n" ;
		return false ;
	}
	if ( parent.m_data != 0 ) {
		cout << "ERROR : cannot open chunk-data redundantly\n" ;
		return false ;
	}
	align_pos( 4 ) ;
	parent.m_data = get_pos() ;
	return true ;
}

void bin_ofstream::close_chunk_data()
{
	;
}

bin_ofstream &bin_ofstream::put_string( vector<string> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_string( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_string( const string &data )
{
	write( data.c_str(), data.length() + 1 ) ;
	return *this ;
}

bin_ofstream &bin_ofstream::put_float( vector<float> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_float( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_float( const float *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		put_float( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_float( float data )
{
	if ( !m_swap_bytes ) {
		write( (const char *)&data, sizeof( float ) ) ;
	} else {
		const char *cp = (const char *)&data ;
		put( cp[ 3 ] ) ;
		put( cp[ 2 ] ) ;
		put( cp[ 1 ] ) ;
		put( cp[ 0 ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int32( vector<int> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_int32( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int32( const int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		put_int32( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int32( int data )
{
	if ( !m_swap_bytes ) {
		write( (const char *)&data, sizeof( int ) ) ;
	} else {
		const char *cp = (const char *)&data ;
		put( cp[ 3 ] ) ;
		put( cp[ 2 ] ) ;
		put( cp[ 1 ] ) ;
		put( cp[ 0 ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int16( vector<int> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_int16( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int16( const int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		put_int16( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int16( int data )
{
	short tmp = data ;
	if ( !m_swap_bytes ) {
		write( (const char *)&tmp, sizeof( short ) ) ;
	} else {
		const char *cp = (const char *)&tmp ;
		put( cp[ 1 ] ) ;
		put( cp[ 0 ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int8( vector<int> &data )
{
	for ( int i = 0 ; i < (int)data.size() ; i ++ ) {
		put_int8( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int8( const int *data, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		put_int8( data[ i ] ) ;
	}
	return *this ;
}

bin_ofstream &bin_ofstream::put_int8( int data )
{
	char tmp = data ;
	write( (const char *)&tmp, sizeof( char ) ) ;
	return *this ;
}

bin_ofstream &bin_ofstream::put_uint32( vector<int> &data )
{
	return put_int32( data ) ;
}

bin_ofstream &bin_ofstream::put_uint32( const int *data, int count )
{
	return put_int32( data, count ) ;
}

bin_ofstream &bin_ofstream::put_uint32( int data )
{
	return put_int32( data ) ;
}

bin_ofstream &bin_ofstream::put_uint16( vector<int> &data )
{
	return put_int16( data ) ;
}

bin_ofstream &bin_ofstream::put_uint16( const int *data, int count )
{
	return put_int16( data, count ) ;
}

bin_ofstream &bin_ofstream::put_uint16( int data )
{
	return put_int16( data ) ;
}

bin_ofstream &bin_ofstream::put_uint8( vector<int> &data )
{
	return put_int8( data ) ;
}

bin_ofstream &bin_ofstream::put_uint8( const int *data, int count )
{
	return put_int8( data, count ) ;
}

bin_ofstream &bin_ofstream::put_uint8( int data )
{
	return put_int8( data ) ;
}

bin_ofstream &bin_ofstream::put_vec2( const vec2 &v )
{
	return put_float( (const float *)&v, 2 ) ;
}

bin_ofstream &bin_ofstream::put_vec3( const vec3 &v )
{
	return put_float( (const float *)&v, 3 ) ;
}

bin_ofstream &bin_ofstream::put_vec4( const vec4 &v )
{
	return put_float( (const float *)&v, 4 ) ;
}

bin_ofstream &bin_ofstream::put_mat4( const mat4 &m )
{
	return put_float( (const float *)&m, 16 ) ;
}

bin_ofstream &bin_ofstream::put_quat( const quat &q )
{
	return put_float( (const float *)&q, 4 ) ;
}

bin_ofstream &bin_ofstream::put_rect( const rect &r )
{
	return put_float( (const float *)&r, 4 ) ;
}

void bin_ofstream::fix_chunk()
{
	if ( m_chunk_stack.empty() ) return ;		// no chunk is opened

	align_pos( 4 ) ;
	int next = get_pos() ;
	if ( m_chunk_level > 0 ) {
		chunk_info &parent = m_chunk_stack[ m_chunk_level - 1 ] ;
		if ( parent.m_short ) {
			m_chunk_level -- ;		// close short-chunk
		} else if ( parent.m_child == 0 ) {
			parent.m_child = next ;		// first child-chunk
		}
	}
	for ( int i = m_chunk_level ; i < (int)m_chunk_stack.size() ; i ++ ) {
		chunk_info &info = m_chunk_stack[ i ] ;
		if ( !info.m_short ) {
			int base = info.m_base ;
			int args = info.m_args ;
			int child = info.m_child ;
			int data = info.m_data ;
			if ( child == 0 ) child = next ;
			if ( data == 0 ) data = child ;
			if ( args == 0 ) args = data ;
			set_pos( base + 2 ) ;
			put_int16( args - base ) ;	// offset to name-end
			put_int32( data - base ) ;	// offset to args-end
			put_int32( child - base ) ;	// offset to data-end
			put_int32( next - base ) ;	// offset to child-end
		} else {
			int base = info.m_base ;
			int args = info.m_args ;
			if ( args == 0 ) args = next ;
			set_pos( base + 2 ) ;
			put_int16( next - base ) ;	// offset to child-end
		}
	}
	m_chunk_stack.resize( m_chunk_level ) ;
	set_pos( next ) ;
}


} // namespace mdx
