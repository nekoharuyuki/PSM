#ifndef	MDX_STRING_H_INCLUDE
#define	MDX_STRING_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  numeric
//----------------------------------------------------------------

bool str_isdigit( const string &str ) ;
int str_atoi( const string &str ) ;
string str_itoa( int val ) ;
float str_atof( const string &str ) ;
string str_ftoa( float val ) ;
int str_atom( const string &str ) ;
string str_mtoa( int val ) ;
string str_format( const char *format, ... ) ;
string str_vformat( const char *format, va_list va ) ;

//----------------------------------------------------------------
//  character
//----------------------------------------------------------------

string str_tolower( const string &src ) ;
string str_toupper( const string &src ) ;
string str_trim( const string &src, const string &chars = "" ) ;
string str_trimleft( const string &src, const string &chars = "" ) ;
string str_trimright( const string &src, const string &chars = "" ) ;
string str_quote( const string &src, char quotation = '"' ) ;
string str_unquote( const string &src, char quotation = '"' ) ;
string str_title( const string &str, char separator = '_' ) ;
string str_untitle( const string &str, char separator = '_' ) ;
string str_symbolname( const string &src, char separator = '_' ) ;
bool str_match( const string &pattern, const string &str, bool nocase = false ) ;

//----------------------------------------------------------------
//  list
//----------------------------------------------------------------

string str_append( const string &words, const string &word, char separator = 0 ) ;
string str_index( const string &words, int index, char separator = 0 ) ;
int str_search( const string &words, const string &word, char separator = 0 ) ;
string str_join( const vector<string> &buf, char separator = 0 ) ;
int str_split( vector<string> &buf, const string &words, char separator = 0 ) ;

//----------------------------------------------------------------
//  filename
//----------------------------------------------------------------

string str_extension( const string &filename ) ;
string str_rootname( const string &filename ) ;
string str_dirname( const string &filename ) ;
string str_tailname( const string &filename ) ;
string str_fullpath( const string &dirname, const string &filename ) ;


} // namespace mdx

#endif
