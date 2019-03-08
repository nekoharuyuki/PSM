#include "x_stream.h"

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

enum {
	X_TOKEN_NAME		= 1,
	X_TOKEN_STRING		= 2,
	X_TOKEN_INTEGER		= 3,
	X_TOKEN_GUID		= 5,
	X_TOKEN_INTEGER_LIST	= 6,
	X_TOKEN_FLOAT_LIST	= 7,

	X_TOKEN_OBRACE		= 10,
	X_TOKEN_CBRACE		= 11,
	X_TOKEN_OPAREN		= 12,
	X_TOKEN_CPAREN		= 13,
	X_TOKEN_OBRACKET	= 14,
	X_TOKEN_CBRACKET	= 15,
	X_TOKEN_OANGLE		= 16,
	X_TOKEN_CANGLE		= 17,
	X_TOKEN_DOT		= 18,
	X_TOKEN_COMMA		= 19,
	X_TOKEN_SEMICOLON	= 20,
	X_TOKEN_TEMPLATE	= 31,
	X_TOKEN_WORD		= 40,
	X_TOKEN_DWORD		= 41,
	X_TOKEN_FLOAT		= 42,
	X_TOKEN_DOUBLE		= 43,
	X_TOKEN_CHAR		= 44,
	X_TOKEN_UCHAR		= 45,
	X_TOKEN_SWORD		= 46,
	X_TOKEN_SDWORD		= 47,
	X_TOKEN_VOID		= 48,
	X_TOKEN_LPSTR		= 49,
	X_TOKEN_UNICODE		= 50,
	X_TOKEN_CSTRING		= 51,
	X_TOKEN_ARRAY		= 52,
} ;

//----------------------------------------------------------------
//  x_txt_ifstream
//----------------------------------------------------------------

x_txt_ifstream::x_txt_ifstream()
{
	;
}

x_txt_ifstream::~x_txt_ifstream()
{
	;
}

bool x_txt_ifstream::open( const string &filename )
{
	if ( !m_stream.open( filename ) ) return false ;
	vector<string> line ;
	m_stream.get_line( line ) ;
	m_block_from = 0 ;
	return true ;
}

void x_txt_ifstream::close()
{
	m_stream.close() ;
}

bool x_txt_ifstream::eof()
{
	return m_stream.eof() ;
}

int x_txt_ifstream::get_pos()
{
	return m_stream.get_pos() ;
}

int x_txt_ifstream::set_pos( int pos )
{
	return m_stream.set_pos( pos ) ;
}

int x_txt_ifstream::add_pos( int pos )
{
	return m_stream.set_pos( m_stream.get_pos() + pos ) ;
}

int x_txt_ifstream::get_block_from()
{
	return m_block_from ;
}

int x_txt_ifstream::get_line_count()
{
	return m_stream.get_line_count() ;
}

bool x_txt_ifstream::open_block( string &type, string &name )
{
	m_block_from = 0 ;
	type = "" ;
	name = "" ;
	while ( !m_stream.eof() ) {
		string word ;
		m_stream.get_string( word ) ;
		if ( word == "{" ) {
			m_block_from = m_stream.get_line_count() ;
			return true ;
		}
		if ( word == "}" ) {
			m_stream.put_back( word ) ;
			return false ;
		}
		if ( type == "" ) {
			type = word ;
		} else {
			if ( name != "" ) type = name ;
			name = word ;
		}
	}
	return false ;
}

bool x_txt_ifstream::open_block( string &type )
{
	string dummy ;
	return open_block( type, dummy ) ;
}

bool x_txt_ifstream::open_block()
{
	string dummy, dummy2 ;
	return open_block( dummy, dummy2 ) ;
}

bool x_txt_ifstream::close_block()
{
	int level = 0 ;
	while ( !m_stream.eof() ) {
		string word ;
		m_stream.get_string( word ) ;
		if ( word == "{" ) {
			level += 1 ;
		}
		if ( word == "}" ) {
			if ( level == 0 ) return true ;
			level -= 1 ;
		}
	}
	return false ;
}

bool x_txt_ifstream::get_string( string &buf )
{
	m_stream.get_string( buf ) ;
	return true ;
}

bool x_txt_ifstream::get_float( float &buf )
{
	string buf2 ;
	m_stream.get_string( buf2 ) ;
	buf = str_atof( buf2 ) ;
	return true ;
}

bool x_txt_ifstream::get_int( int &buf )
{
	string buf2 ;
	m_stream.get_string( buf2 ) ;
	buf = str_atoi( buf2 ) ;
	return true ;
}

bool x_txt_ifstream::get_string( vector<string> &buf, int count )
{
	buf.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		string buf2 ;
		m_stream.get_string( buf2 ) ;
		buf.push_back( buf2 ) ;
	}
	return true ;
}

bool x_txt_ifstream::get_float( vector<float> &buf, int count )
{
	buf.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		string buf2 ;
		m_stream.get_string( buf2 ) ;
		buf.push_back( str_atof( buf2 ) ) ;
	}
	return true ;
}

bool x_txt_ifstream::get_int( vector<int> &buf, int count )
{
	buf.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		string buf2 ;
		m_stream.get_string( buf2 ) ;
		buf.push_back( str_atoi( buf2 ) ) ;
	}
	return true ;
}

bool x_txt_ifstream::get_float( float *buf, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		string buf2 ;
		m_stream.get_string( buf2 ) ;
		buf[ i ] = str_atof( buf2 ) ;
	}
	return true ;
}

bool x_txt_ifstream::get_int( int *buf, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		string buf2 ;
		m_stream.get_string( buf2 ) ;
		buf[ i ] = str_atoi( buf2 ) ;
	}
	return true ;
}

//----------------------------------------------------------------
//  x_bin_ifstream
//----------------------------------------------------------------

x_bin_ifstream::x_bin_ifstream()
{
	;
}

x_bin_ifstream::~x_bin_ifstream()
{
	;
}

bool x_bin_ifstream::open( const string &filename )
{
	if ( !m_stream.open( filename ) ) return false ;
	set_pos( 16 ) ;
	m_block_from = 0 ;

	m_data_type = 0 ;
	m_data_count = 0 ;
	return true ;
}

void x_bin_ifstream::close()
{
	m_stream.close() ;
}

bool x_bin_ifstream::eof()
{
	return m_stream.eof() ;
}

int x_bin_ifstream::get_pos()
{
	return m_stream.get_pos() ;
}

int x_bin_ifstream::set_pos( int pos )
{
	return m_stream.set_pos( pos ) ;
}

int x_bin_ifstream::add_pos( int pos )
{
	return m_stream.set_pos( m_stream.get_pos() + pos ) ;
}

int x_bin_ifstream::get_block_from()
{
	return m_block_from ;
}

bool x_bin_ifstream::open_block( string &type, string &name )
{
	close_token() ;

	m_block_from = 0 ;
	type = "" ;
	name = "" ;
	while ( !eof() ) {
		open_token() ;
		if ( m_data_type == X_TOKEN_NAME ) {
			if ( type == "" ) {
				m_block_from = m_stream.get_pos() - 2 ;
				get_string( type ) ;
			} else {
				if ( name != "" ) {
					m_block_from = m_stream.get_pos() - 2 ;
					type = name ;
				}
				get_string( name ) ;
			}
		}
		if ( m_data_type == X_TOKEN_OBRACE ) {
			return true ;
		}
		if ( m_data_type == X_TOKEN_CBRACE ) {
			add_pos( -2 ) ;
			return false ;
		}
		close_token() ;
	}
	return false ;
}

bool x_bin_ifstream::open_block( string &type )
{
	string dummy ;
	return open_block( type, dummy ) ;
}

bool x_bin_ifstream::open_block()
{
	string dummy, dummy2 ;
	return open_block( dummy, dummy2 ) ;
}

bool x_bin_ifstream::close_block()
{
	close_token() ;

	int level = 0 ;
	while ( !eof() ) {
		open_token() ;
		if ( m_data_type == X_TOKEN_OBRACE ) {
			level += 1 ;
		}
		if ( m_data_type == X_TOKEN_CBRACE ) {
			if ( level == 0 ) return true ;
			level -= 1 ;
		}
		close_token() ;
	}
	return false ;
}

bool x_bin_ifstream::get_string( string &buf )
{
	if ( !open_data() ) return false ;

	if ( m_data_type == X_TOKEN_NAME ) {
		int len ;
		m_stream.get_int32( len ) ;
		m_stream.get_string( buf, len ) ;
		m_data_count -= 1 ;
		return true ;
	}
	if ( m_data_type == X_TOKEN_STRING ) {
		int len, term ;
		m_stream.get_int32( len ) ;
		m_stream.get_string( buf, len ) ;
		m_stream.get_int16( term ) ;
		m_data_count -= 1 ;
		return true ;
	}
	return false ;
}

bool x_bin_ifstream::get_float( float &buf )
{
	if ( !open_data() ) return false ;

	if ( m_data_type == X_TOKEN_FLOAT_LIST ) {
		m_stream.get_float( buf ) ;
		m_data_count -= 1 ;
		return true ;
	}
	return false ;
}

bool x_bin_ifstream::get_int( int &buf )
{
	if ( !open_data() ) return false ;

	if ( m_data_type == X_TOKEN_INTEGER_LIST ) {
		m_stream.get_int32( buf ) ;
		m_data_count -= 1 ;
		return true ;
	}
	if ( m_data_type == X_TOKEN_INTEGER ) {
		m_stream.get_int32( buf ) ;
		m_data_count -= 1 ;
		return true ;
	}
	return false ;
}

bool x_bin_ifstream::get_string( vector<string> &buf, int count )
{
	buf.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		string val ;
		if ( !get_string( val ) ) return false ;
		buf.push_back( val ) ;
	}
	return true ;
}

bool x_bin_ifstream::get_float( vector<float> &buf, int count )
{
	buf.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		float val ;
		if ( !get_float( val ) ) return false ;
		buf.push_back( val ) ;
	}
	return true ;
}

bool x_bin_ifstream::get_int( vector<int> &buf, int count )
{
	buf.clear() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		int val ;
		if ( !get_int( val ) ) return false ;
		buf.push_back( val ) ;
	}
	return true ;
}

bool x_bin_ifstream::get_float( float *buf, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		if ( !get_float( buf[ i ] ) ) return false ;
	}
	return true ;
}

bool x_bin_ifstream::get_int( int *buf, int count )
{
	for ( int i = 0 ; i < count ; i ++ ) {
		if ( !get_int( buf[ i ] ) ) return false ;
	}
	return true ;
}

bool x_bin_ifstream::open_data()
{
	while ( !eof() ) {
		open_token() ;
		if ( m_data_count > 0 ) return true ;
		if ( m_data_type == X_TOKEN_OBRACE || m_data_type == X_TOKEN_CBRACE ) {
			add_pos( -2 ) ;
			return false ;
		}
	}
	return false ;
}

bool x_bin_ifstream::open_token()
{
	if ( m_data_count > 0 ) return true ;
	m_data_type = 0 ;
	m_data_count = 0 ;
	if ( eof() ) return false ;

	m_stream.get_int16( m_data_type ) ;

	switch ( m_data_type ) {
	    case X_TOKEN_NAME :
	    case X_TOKEN_STRING :
	    case X_TOKEN_INTEGER :
		if ( eof() ) return false ;
		m_data_count = 1 ;
		break ;
	    case X_TOKEN_INTEGER_LIST :
	    case X_TOKEN_FLOAT_LIST :
		m_stream.get_int32( m_data_count ) ;
		if ( eof() ) return false ;
		break ;
	    case X_TOKEN_GUID :
		if ( eof() ) return false ;
		m_data_count = 1 ;
		break ;
	}
	return true ;
}

void x_bin_ifstream::close_token()
{
	int i ;

	if ( m_data_count <= 0 ) return ;

	switch ( m_data_type ) {
	    case X_TOKEN_NAME :
	    case X_TOKEN_STRING :
		for ( i = 0 ; i < m_data_count ; i ++ ) {
			string tmp ;
			get_string( tmp ) ;
		}
		break ;
	    case X_TOKEN_INTEGER :
	    case X_TOKEN_INTEGER_LIST :
		add_pos( sizeof( int ) * m_data_count ) ;
		break ;
	    case X_TOKEN_FLOAT_LIST :
		add_pos( sizeof( float ) * m_data_count ) ;
		break ;
	    case X_TOKEN_GUID :
		add_pos( sizeof( int ) * 4 * m_data_count ) ;
		break ;
	}
	m_data_type = 0 ;
	m_data_count = 0 ;
}
