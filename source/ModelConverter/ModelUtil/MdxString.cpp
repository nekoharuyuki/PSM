#include "ModelUtil.h"

namespace mdx {

#if ( MDX_IGNORE_SJIS )
#define	SJIS_BIT	0x00
#else // MDX_IGNORE_SJIS
#define	SJIS_BIT	0x80
#endif // MDX_IGNORE_SJIS

#ifdef WIN32
static float hex_atof( const char *str ) ;
#endif // WIN32
static bool match( const char *pattern, const char *str, bool nocase ) ;

//----------------------------------------------------------------
//  numeric
//----------------------------------------------------------------

bool str_isdigit( const string &str )
{
	char c = str[ 0 ] ;
	if ( c >= '0' && c <= '9' ) return true ;
	if ( c == '-' || c == '+' || c == '.' ) {
		c = str[ 1 ] ;
		return ( c >= '0' && c <= '9' ) ;
	}
	return false ;
}

int str_atoi( const string &str )
{
	int base = 10 ;
	const char *cp = str.c_str() ;
	if ( cp[ 0 ] == '-' || cp[ 0 ] == '+' ) cp ++ ;
	if ( cp[ 0 ] == '0' ) base = ( cp[ 1 ] == 'x' || cp[ 1 ] == 'X' ) ? 16 : 8 ;
	return strtoul( str.c_str(), 0, base ) ;
}

string str_itoa( int val )
{
	char buf[ 32 ] ;
	sprintf( buf, "%d", val ) ;
	return string( buf ) ;
}

float str_atof( const string &str )
{
	#ifdef WIN32
	const char *cp = str.c_str() ;
	if ( cp[ 0 ] == '-' || cp[ 0 ] == '+' ) cp ++ ;
	if ( cp[ 0 ] == '0' && ( cp[ 1 ] == 'x' || cp[ 1 ] == 'X' ) ) {
		return hex_atof( str.c_str() ) ;
	}
	#endif // WIN32

	return (float)atof( str.c_str() ) ;
}

string str_ftoa( float val )
{
	char buf[ 32 ] ;
	sprintf( buf, "%f", val ) ;
	return string( buf ) ;
}

int str_atom( const string &str )
{
	//  ABCD -> 0x41424344

	const unsigned char *cp = (const unsigned char *)str.c_str() ;
	int val = 0 ;
	for ( ; ; ) {
		unsigned char c = *( cp ++ ) ;
		if ( c == '\0' ) break ;
		val = ( val << 8 ) | c ;
	}
	return val ;
}

string str_mtoa( int val )
{
	//  0x41424344 -> ABCD

	char buf[ 5 ] ;
	char *cp = buf + 4 ;
	*cp = '\0' ;
	for ( int i = 0 ; i < 4 ; i ++ ) {
		int c = 255 & val ;
		if ( c == 0 ) break ;
		*( -- cp ) = c ;
		val >>= 8 ;
	}
	return string( cp ) ;
}

string str_format( const char *format, ... )
{
	if ( format == 0 ) return "" ;
	va_list va ;
	va_start( va, format ) ;
	string tmp = str_vformat( format, va ) ;
	va_end( va ) ;
	return tmp ;
}

string str_vformat( const char *format, va_list va )
{
	if ( format == 0 ) return "" ;
	char buf[ 65536 ] ;
	#ifdef WIN32
	_vsnprintf( buf, sizeof( buf ), format, va ) ;
	#else // WIN32
	vsnprintf( buf, sizeof( buf ), format, va ) ;
	#endif // WIN32
	buf[ sizeof( buf ) - 1 ] = '\0' ;
	va_end( va ) ;
	return string( buf ) ;
}

//----------------------------------------------------------------
//  character
//----------------------------------------------------------------

string str_tolower( const string &str )
{
	//  Foo-Bar -> foo-bar

	string tmp ;
	for ( int i = 0 ; i < (int)str.length() ; i ++ ) {
		char c = str[ i ] ;
		if ( SJIS_BIT & c ) {
			tmp += c ;
			tmp += str[ ++ i ] ;
		} else {
			tmp += tolower( c ) ;
		}
	}
	return tmp ;
}

string str_toupper( const string &str )
{
	//  Foo-Bar -> FOO-BAR

	string tmp ;
	for ( int i = 0 ; i < (int)str.length() ; i ++ ) {
		char c = str[ i ] ;
		if ( SJIS_BIT & c ) {
			tmp += c ;
			tmp += str[ ++ i ] ;
		} else {
			tmp += toupper( c ) ;
		}
	}
	return tmp ;
}

string str_trim( const string &str, const string &chars )
{
	return str_trimleft( str_trimright( str, chars ), chars ) ;
}

string str_trimleft( const string &str, const string &chars )
{
	int len = str.length() ;
	if ( len == 0 ) return "" ;
	string trimmed = chars.empty() ? " \t\n\r" : chars ;
	if ( trimmed.find_first_of( str[ 0 ] ) == string::npos ) return str ;
	for ( int i = 0 ; i < len ; i ++ ) {
		char c = str[ i ] ;
		if ( trimmed.find_first_of( c ) == string::npos ) {
			return str.substr( i ) ;
		}
	}
	return "" ;
}

string str_trimright( const string &str, const string &chars )
{
	int len = str.length() ;
	if ( len == 0 ) return "" ;
	string trimmed = chars.empty() ? " \t\n\r" : chars ;
	if ( trimmed.find_first_of( str[ len - 1 ] ) == string::npos ) return str ;
	int min = 0 ;
	for ( int i = 0 ; i < len ; i ++ ) {
		if ( SJIS_BIT & str[ i ] ) {
			min = i + 2 ;
			i ++ ;
		}
	}
	if ( min >= len ) return "" ;
	for ( int j = len - 1 ; j >= min ; -- j ) {
		char c = str[ j ] ;
		if ( trimmed.find_first_of( c ) == string::npos ) {
			return str.substr( 0, j + 1 ) ;
		}
	}
	return str.substr( 0, min ) ;
}

string str_quote( const string &str, char quotation )
{
	//  Foo-Bar -> "Foo-Bar"

	if ( quotation == 0 ) quotation = '"' ;
	string tmp ;
	return tmp + quotation + str + quotation ;
}

string str_unquote( const string &str, char quotation )
{
	//  "Foo-Bar" -> Foo-Bar

	if ( str.length() >= 2 ) {
		if ( quotation == 0 ) quotation = '"' ;
		if ( str[ 0 ] == quotation && str[ str.length() - 1 ] == quotation ) {
			return str.substr( 1, str.length() - 2 ) ;
		}
	}
	return str ;
}

string str_title( const string &str, char separator )
{
	//  foo_bar -> FooBar

	if ( separator == 0 ) separator = '_' ;
	string tmp ;
	bool start = true ;
	for ( int i = 0 ; i < (int)str.length() ; i ++ ) {
		char c = str[ i ] ;
		if ( SJIS_BIT & c ) {
			tmp += c ;
			tmp += str[ ++ i ] ;
			start = false ;
			continue ;
		}
		if ( c == separator ) {
			start = true ;
			continue ;
		}
		tmp += ( start ? toupper( c ) : tolower( c ) ) ;
		start = false ;
	}
	return tmp ;
}

string str_untitle( const string &str, char separator )
{
	//  FooBar -> foo_bar

	if ( separator == 0 ) separator = '_' ;
	string tmp ;
	bool lower = false ;
	for ( int i = 0 ; i < (int)str.length() ; i ++ ) {
		char c = str[ i ] ;
		if ( SJIS_BIT & c ) {
			tmp += c ;
			tmp += str[ ++ i ] ;
			lower = false ;
			continue ;
		}
		if ( c >= 'A' && c <= 'Z' ) {
			if ( lower ) tmp += separator ;
		}
		lower = ( c >= 'a' && c <= 'z' ) ;
		tmp += tolower( c ) ;
	}
	return tmp ;
}

string str_symbolname( const string &str, char separator )
{
	//  Foo-Bar -> Foo_Bar

	if ( separator == 0 ) separator = '_' ;
	string tmp ;
	for ( int i = 0 ; i < (int)str.length() ; i ++ ) {
		char c = str[ i ] ;
		if ( SJIS_BIT & c ) {
			tmp += separator ;
			tmp += separator ;
			i ++ ;
		} else {
			if ( ( c < 'a' || c > 'z' ) && ( c < 'A' || c > 'Z' )
			  && ( c < '0' || c > '9' ) && ( c != '_' ) ) {
				c = separator ;
			}
			tmp += c ;
		}
	}
	return tmp ;
}

bool str_match( const string &pattern, const string &str, bool nocase )
{
	return match( pattern.c_str(), str.c_str(), nocase ) ;
}

//----------------------------------------------------------------
//  list
//----------------------------------------------------------------

string str_append( const string &words, const string &word, char separator )
{
	if ( separator == 0 ) separator = ' ' ;
	if ( words == "" ) return word ;
	return words + separator + word ;
}

string str_index( const string &words, int index, char separator )
{
	vector<string> tmp ;
	int count = str_split( tmp, words, separator ) ;
	if ( index < 0 || index >= count ) return "" ;
	return tmp[ index ] ;
}

int str_search( const string &words, const string &word, char separator )
{
	vector<string> tmp ;
	int count = str_split( tmp, words, separator ) ;
	for ( int i = 0 ; i < count ; i ++ ) {
		if ( tmp[ i ] == word ) return i ;
	}
	return -1 ;
}

string str_join( const vector<string> &buf, char separator )
{
	if ( separator == 0 ) separator = ' ' ;
	string tmp ;
	for ( int i = 0 ; i < (int)buf.size() ; i ++ ) {
		if ( buf[ i ] != "" ) {
			if ( i > 0 ) tmp += separator ;
			tmp += buf[ i ] ;
		}
	}
	return tmp ;
}

int str_split( vector<string> &buf, const string &words, char separator )
{
	buf.clear() ;
	char quote = 0 ;
	int from = 0 ;
	for ( int i = 0 ; i < (int)words.length() ; i ++ ) {
		char c = words[ i ] ;
		if ( quote == 0 ) {
			if ( c == '"' || c == '\'' ) {
				quote = c ;
				continue ;
			}
		} else {
			if ( c != quote ) continue ;
			quote = 0 ;
		}
		if ( separator == 0 ) {
			if ( c == ' ' || c == '\t' || c == '\n' || c == '\r' ) {
				if ( i > from ) buf.push_back( words.substr( from, i - from ) ) ;
				from = i + 1 ;
			}
		} else {
			if ( c == separator ) {
				buf.push_back( words.substr( from, i - from ) ) ;
				from = i + 1 ;
			}
		}
	}
	if ( from < (int)words.length() ) {
		buf.push_back( words.substr( from, words.length() - from ) ) ;
	}
	return buf.size() ;
}

//----------------------------------------------------------------
//  filename
//----------------------------------------------------------------

string str_extension( const string &filename )
{
	int dot = string::npos ;
	for ( int i = 0 ; i < (int)filename.length() ; i ++ ) {
		char c = filename[ i ] ;
		if ( c == '/' || c == '\\' ) {
			dot = string::npos ;
		} else if ( c == '.' ) {
			dot = i ;
		} else if ( SJIS_BIT & c ) {
			i ++ ;
		}
	}
	if ( dot != (int)string::npos ) {
		return filename.substr( dot, filename.length() - dot ) ;
	} else {
		return "" ;
	}
}

string str_rootname( const string &filename )
{
	int dot = string::npos ;
	for ( int i = 0 ; i < (int)filename.length() ; i ++ ) {
		char c = filename[ i ] ;
		if ( c == '/' || c == '\\' ) {
			dot = string::npos ;
		} else if ( c == '.' ) {
			dot = i ;
		} else if ( SJIS_BIT & c ) {
			i ++ ;
		}
	}
	if ( dot != (int)string::npos ) {
		return filename.substr( 0, dot ) ;
	} else {
		return filename ;
	}
}

string str_dirname( const string &filename )
{
	int slash = string::npos ;
	for ( int i = 0 ; i < (int)filename.length() ; i ++ ) {
		char c = filename[ i ] ;
		if ( c == '/' || c == '\\' ) {
			slash = i ;
		} else if ( SJIS_BIT & c ) {
			i ++ ;
		}
	}
	if ( slash == (int)string::npos ) return "." ;
	if ( slash == 0 ) return "/" ;
	return filename.substr( 0, slash ) ;
}

string str_tailname( const string &filename )
{
	int slash = string::npos ;
	for ( int i = 0 ; i < (int)filename.length() ; i ++ ) {
		char c = filename[ i ] ;
		if ( c == '/' || c == '\\' ) {
			slash = i ;
		} else if ( SJIS_BIT & c ) {
			i ++ ;
		}
	}
	if ( slash == (int)string::npos ) return filename ;
	return filename.substr( slash + 1, filename.length() - slash - 1 ) ;
}

string str_fullpath( const string &dirname, const string &filename )
{
	if ( filename[ 0 ] == '/' || filename[ 0 ] == '\\' ) return filename ;
	if ( filename.length() >= 2 && filename[ 1 ] == ':' ) return filename ;
	if ( dirname.empty() ) return filename ;
	return str_trimright( dirname, "/\\" ) + "/" + filename ;
}

//----------------------------------------------------------------
//  subroutines
//----------------------------------------------------------------

#ifdef WIN32
float hex_atof( const char *str )
{
	const char *cp = str ;
	if ( cp[ 0 ] == '-' || cp[ 0 ] == '+' ) cp ++ ;
	if ( cp[ 0 ] != '0' || ( cp[ 1 ] != 'x' && cp[ 1 ] != 'X' ) ) return 0.0f ;
	cp += 2 ;
	unsigned int bits = 0 ;
	int exp = 0, digit = 0 ;
	int shift = 28, point = 32 ;
	char c ;
	for ( ; ; ) {
		c = *( cp ++ ) ;
		if ( c >= '0' && c <= '9' ) {
			digit = ( c - '0' ) ;
		} else if ( c >= 'a' && c <= 'f' ) {
			digit = ( c - 'a' + 10 ) ;
		} else if ( c >= 'A' && c <= 'F' ) {
			digit = ( c - 'A' + 10 ) ;
		} else if ( c == '.' ) {
			point = shift ;
			continue ;
		} else if ( c == 'p' || c == 'P' ) {
			exp = atoi( cp ) ;
			break ;
		} else {
			break ;
		}
		if ( digit == 0 && shift == 28 ) continue ;
		if ( shift >= 0 ) bits |= ( digit << shift ) ;
		shift -= 4 ;
	}
	if ( bits == 0 ) return 0.0f ;
	if ( point == 32 ) point = shift ;
	exp = exp - point + 24 ;
	while ( bits & 0xe0000000 ) {
		bits >>= 1 ;
		exp += 1 ;
	}
	if ( bits & 0x10 ) {
		bits += 0x20 ;
		if ( bits & 0xe0000000 ) {
			bits >>= 1 ;
			exp += 1 ;
		}
	}
	float32 f32 ;
	f32.s = ( *str == '-' ) ? 1 : 0 ;
	f32.e = exp + 127 ;
	f32.f = bits >> 5 ;
	return f32.val ;
}
#endif

bool match( const char *pattern, const char *str, bool nocase )
{
	char c, d ;
	while ( ( c = *( pattern ++ ) ) != '\0' ) {
		if ( c == '*' ) {
			if ( ( c = *pattern ) == '\0' ) return true ;
			while ( ( d = *str ) != '\0' ) {
				if ( match( pattern, str, nocase ) ) return true ;
				if ( ( d & SJIS_BIT ) && *( ++ str ) == '\0' ) return false ;
				str ++ ;
			}
			return false ;
		} else if ( c == '?' ) {
			if ( ( *str & SJIS_BIT ) && *( ++ str ) == '\0' ) return false ;
			str ++ ;
		} else {
			if ( ( d = *( str ++ ) ) != c ) {
				c |= 0x20 ;
				d |= 0x20 ;
				if ( !nocase || c != d || c < 'a' || c > 'z' ) return false ;
			}
			if ( c & SJIS_BIT ) {
				c = *( pattern ++ ) ;
				if ( c == '\0' || c != *( str ++ ) ) return false ;
			}
		}
	}
	return ( *str == '\0' ) ;
}


} // namespace mdx
