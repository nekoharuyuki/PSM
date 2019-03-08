#include "ModelShell.h"

#include <cstring>
#include <cstdio>
#include <algorithm>

#ifdef WIN32
#include <windows.h>
#include <mmsystem.h>
#else // WIN32
#include <sys/time.h>
#endif // WIN32

//----------------------------------------------------------------
//  ModelShell
//----------------------------------------------------------------

ModelShell::ModelShell()
{
	;
}

ModelShell::~ModelShell()
{
	;
}

ModelShell::ModelShell( const ModelShell &src )
{
	*this = src ;
}

ModelShell &ModelShell::operator=( const ModelShell &src )
{
	MdxShell::operator=( src ) ;
	m_app_name = src.m_app_name ;
	m_app_version = src.m_app_version ;
	m_app_filename = src.m_app_filename ;
	m_scripts = src.m_scripts ;
	m_args = src.m_args ;
	m_known_vars = src.m_known_vars ;
	return *this ;
}

//----------------------------------------------------------------
//  application information
//----------------------------------------------------------------

const char *ModelShell::GetAppName() const
{
	return m_app_name.c_str() ;
}

const char *ModelShell::GetAppVersion() const
{
	return m_app_version.c_str() ;
}

const char *ModelShell::GetAppFileName() const
{
	return m_app_filename.c_str() ;
}

void ModelShell::SetAppName( const char *name )
{
	if ( name == 0 ) name = "" ;
	m_app_name = name ;
}

void ModelShell::SetAppVersion( const char *version )
{
	if ( version == 0 ) version = "" ;
	m_app_version = version ;
}

void ModelShell::SetAppFileName( const char *filename )
{
	if ( filename == 0 ) filename = "" ;
	m_app_filename = filename ;
}

//----------------------------------------------------------------
//  objects
//----------------------------------------------------------------

void ModelShell::SetVar( const char *name, const char *value )
{
	MdxShell::SetVar( name, value ) ;

	//  debug level

	if ( strcmp( name, "debug_mode" ) == 0 ) {
		int level = 0 ;
		if ( str_isdigit( value ) ) {
			level = str_atoi( value ) ;
		} else if ( strcmp( value, "on" ) == 0 ) {
			level = 1 ;
		}
		SetDebugLevel( level ) ;
	}

	//  var names

	m_known_vars.insert( name ) ;
}

//----------------------------------------------------------------
//  invoking
//----------------------------------------------------------------

int ModelShell::EvalProcArgv( int argc, const char **argv )
{
	//  "$var = $val"

	if ( argc >= 2 && strcmp( argv[ 1 ], "=" ) == 0 ) {
		string val ;
		for ( int i = 2 ; i < argc ; i ++ ) {
			if ( i > 2 ) val += ',' ;
			val += SubstVar( argv[ i ] ) ;
		}
		SetVar( argv[ 0 ], val.c_str() ) ;
		return MDX_EVAL_OK ;
	}

	//  "puts $message ..."

	if ( argc >= 1 && strcmp( argv[ 0 ], "puts" ) == 0 ) {
		for ( int i = 1 ; i < argc ; i ++ ) {
			Message( "%s ", SubstVar( argv[ i ] ) ) ;
		}
		Message( "\n" ) ;
		return MDX_EVAL_OK ;
	}

	//  "load $filename ..."

	if ( argc >= 1 && strcmp( argv[ 0 ], "load" ) == 0 ) {
		if ( argc < 2 ) return MDX_EVAL_OK ;
		const char *filename = SubstVar( argv[ 1 ] ) ;
		if ( !LoadModule( filename, argc - 2, argv + 2 ) ) return MDX_EVAL_ERROR ;
		return MDX_EVAL_OK ;
	}

	//  native procedure

	if ( GetDebugLevel() >= 1 ) {
		for ( int i = 0 ; i < argc ; i ++ ) DebugPrint( 1, "%s ", argv[ i ] ) ;
		DebugPrint( 1, "\n" ) ;
	}
	int start = GetTime() ;
	int code = MdxShell::EvalProcArgv( argc, argv ) ;
	DebugPrint( 1, "%s done ( %.2f sec )\n", argv[ 0 ], ( GetTime() - start ) / 1000.0f ) ;
	return code ;
}

//----------------------------------------------------------------
//  script file
//----------------------------------------------------------------

bool ModelShell::LoadScript( const string &filename )
{
	string filename2 = str_fullpath( str_dirname( GetAppFileName() ), filename ) ;

	txt_ifstream stream ;
	stream.open( filename2 ) ;
	if ( !stream.is_open() ) return false ;

	int current = -1 ;
	int n_names = 0 ;
	vector<string> line ;
	while ( !stream.is_eof() ) {
		stream.get_line( line, true ) ;
		if ( line.empty() ) continue ;
		if ( current < 0 ) {
			//  script header "type name ... %arg ... {"
			if ( line.back() != "{" || line.size() <= 1 ) continue ;
			line.pop_back() ;
			n_names = FindArgs( line ) - 1 ;
			if ( n_names == 0 ) {
				line.insert( line.begin() + 1, "" ) ;
				n_names = 1 ;
			}
			for ( int i = 0 ; i < n_names ; i ++ ) {
				int old = FindScript( line[ 0 ], line[ 1 + i ] ) ;
				if ( old >= 0 ) m_scripts.erase( m_scripts.begin() + old ) ;
			}
			current = m_scripts.size() ;
			m_scripts.resize( current + n_names ) ;
			for ( int j = 0 ; j < n_names ; j ++ ) {
				Script &s = m_scripts[ current + j ] ;
				s.type = line[ 0 ] ;
				s.name = line[ 1 + j ] ;
				s.args.assign( line.begin() + 1 + n_names, line.end() ) ;
				s.body.clear() ;
			}
		} else if ( line.front() == "}" ) {
			current = -1 ;
		} else {
			for ( int i = 0 ; i < n_names ; i ++ ) {
				m_scripts[ current + i ].body.push_back( line ) ;
			}
		}
	}
	stream.close() ;
	return true ;
}

bool ModelShell::LoadScript( const char **lines )
{
	if ( lines == 0 ) return false ;

	int current = -1 ;
	int n_names = 0 ;
	vector<string> line ;
	while ( *lines != 0 ) {
		str_split( line, *( lines ++ ) ) ;
		for ( int i = 0 ; i < (int)line.size() ; i ++ ) {
			line[ i ] = str_unquote( line[ i ] ) ;
		}
		if ( line.empty() ) continue ;
		if ( current < 0 ) {
			//  script header "type name ... %arg ... {"
			if ( line.back() != "{" || line.size() <= 1 ) continue ;
			line.pop_back() ;
			n_names = FindArgs( line ) - 1 ;
			if ( n_names == 0 ) {
				line.insert( line.begin() + 1, "" ) ;
				n_names = 1 ;
			}
			for ( int i = 0 ; i < n_names ; i ++ ) {
				int old = FindScript( line[ 0 ], line[ 1 + i ] ) ;
				if ( old >= 0 ) m_scripts.erase( m_scripts.begin() + old ) ;
			}
			current = m_scripts.size() ;
			m_scripts.resize( current + n_names ) ;
			for ( int j = 0 ; j < n_names ; j ++ ) {
				Script &s = m_scripts[ current + j ] ;
				s.type = line[ 0 ] ;
				s.name = line[ 1 + j ] ;
				s.args.assign( line.begin() + 1 + n_names, line.end() ) ;
				s.body.clear() ;
			}
		} else if ( line.front() == "}" ) {
			current = -1 ;
		} else {
			for ( int i = 0 ; i < n_names ; i ++ ) {
				m_scripts[ current + i ].body.push_back( line ) ;
			}
		}
	}
	return true ;
}

void ModelShell::UnloadScript()
{
	m_scripts.clear() ;
}

int ModelShell::EvalScript( const string &type, const string &name, const string &val )
{
	int num = FindScript( type, name ) ;
	if ( num < 0 ) {
		Error( "invalid script name \"%s %s\"\n", type.c_str(), name.c_str() ) ;
		return MDX_EVAL_ERROR ;
	}
	return EvalScript( num, val ) ;
}

int ModelShell::EvalScript( int num, const string &val )
{
	if ( num < 0 || num >= (int)m_scripts.size() ) return MDX_EVAL_ERROR ;

	const Script &s = m_scripts[ num ] ;
	for ( int i = 0 ; i < (int)s.body.size() ; i ++ ) {
		const vector<string> &line = s.body[ i ] ;
		vector<const char *> args ;
		bool error_ignore = false ;
		int error_count = GetErrorCount() ;
		for ( int j = 0 ; j < (int)line.size() ; j ++ ) {
			const char *arg = line[ j ].c_str() ;
			if ( j == 0 && arg[ 0 ] == '-' ) {
				error_ignore = true ;
				arg ++ ;
			}
			if ( arg[ 0 ] == '%' ) {
				bool match = ( !s.args.empty() && s.args[ 0 ] == arg ) ;
				arg = match ? val.c_str() : "" ;
			// } else if ( arg[ 0 ] == '$' ) {
			//	arg = GetVar( arg + 1 ) ;
			}
			args.push_back( arg ) ;
		}
		int argc = args.size() ;
		const char **argv = args.empty() ? 0 : &( args[ 0 ] ) ;
		int code = EvalProcArgv( argc, argv ) ;
		if ( error_ignore ) {
			if ( code == MDX_EVAL_ERROR ) DebugPrint( 0, "error ignored\n" ) ;
			SetErrorCount( error_count ) ;
			code = MDX_EVAL_OK ;
		}
		if ( code == MDX_EVAL_ERROR ) {
			DebugPrint( 0, "while executing \"" ) ;
			for ( int j = 0 ; j < (int)line.size() ; j ++ ) {
				const char *format = ( j == 0 ) ? "%s" : " %s" ;
				DebugPrint( 0, format, line[ j ].c_str() ) ;
			}
			DebugPrint( 0, "\"\n" ) ;
			return code ;
		}
	}
	return MDX_EVAL_OK ;
}

int ModelShell::FindScript( const string &type, const string &name )
{
	for ( int i = m_scripts.size() - 1 ; i >= 0 ; -- i ) {
		Script &s = m_scripts[ i ] ;
		if ( s.type == type && s.name == name ) return i ;
	}
	return -1 ;
}

int ModelShell::GetScriptCount() const
{
	return m_scripts.size() ;
}

const ModelShell::Script &ModelShell::GetScript( int num ) const
{
	return m_scripts[ num ] ;
}

int ModelShell::FindArgs( const vector<string> &line )
{
	for ( int i = 0 ; i < (int)line.size() ; i ++ ) {
		if ( line[ i ][ 0 ] == '%' ) return i ;
	}
	return line.size() ;
}

const char *ModelShell::SubstVar( const char *str )
{
	return ( str[ 0 ] != '$' ) ? str : GetVar( str + 1 ) ;
}

int ModelShell::GetTime()
{
#ifdef WIN32
	return timeGetTime() ;
#else // WIN32
	struct timeval t ;
	gettimeofday( &t, 0 ) ;
	return t.tv_sec * 1000 + t.tv_usec / 1000 ;
#endif // WIN32
}

//----------------------------------------------------------------
//  command line
//----------------------------------------------------------------

int ModelShell::EvalCommandLine( const vector<string> &args, bool set_args, bool set_vars )
{
	if ( set_args ) m_args.clear() ;

	int argc = args.size() ;
	for ( int i = 0 ; i < argc ; i ++ ) {
		const string &arg = args[ i ] ;
		int num = FindScript( "option", arg ) ;
		if ( num >= 0 ) {
			string val ;
			if ( !m_scripts[ num ].args.empty() ) {
				if ( ++ i >= argc ) {
					Error( "option value missing \"%s\"\n", arg.c_str() ) ;
					return MDX_EVAL_ERROR ;
				}
				val = args[ i ] ;
			}
			if ( set_vars ) {
				int code = EvalScript( "option", arg, val ) ;
				if ( code != MDX_EVAL_OK ) return code ;
			}
		} else if ( arg[ 0 ] == '-' ) {
			if ( arg[ 1 ] != '-' ) {
				Error( "unknown option \"%s\"\n", arg.c_str() ) ;
				return MDX_EVAL_ERROR ;
			}
			if ( ++ i >= argc ) {
				Error( "option value missing \"%s\"\n", arg.c_str() ) ;
				return MDX_EVAL_ERROR ;
			}
			if ( set_vars ) {
				const char *name = arg.c_str() + 2 ;
				if ( !CheckDirectOption( name ) ) return MDX_EVAL_ERROR ;
				SetVar( name, args[ i ].c_str() ) ;
			}
		} else {
			if ( set_args ) m_args.push_back( arg ) ;
		}
	}
	return MDX_EVAL_OK ;
}

int ModelShell::GetArgCount() const
{
	return m_args.size() ;
}

void ModelShell::SetArgCount( int num )
{
	m_args.resize( num ) ;
}

string ModelShell::GetArg( int num ) const
{
	if ( num < 0 || num >= (int)m_args.size() ) return "" ;
	return m_args[ num ].c_str() ;
}

void ModelShell::SetArg( int num, const string &arg )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_args.size() ) m_args.resize( num + 1 ) ;
	m_args[ num ] = arg ;
}

bool ModelShell::CheckDirectOption( const char *name )
{
	if ( m_known_vars.find( name ) != m_known_vars.end() ) return true ;
	m_known_vars.insert( name ) ;

	string opt = str_toupper( GetVar( "check_direct_option" ) ) ;
	switch ( str_search( "OFF ON WARNING ERROR", opt ) ) {
	    case 0 :
		break ;
	    case 2 :
		Warning( "unknown direct option \"--%s\"\n", name ) ;
		break ;
	    case 1 :
	    case 3 :
		Error( "unknown direct option \"--%s\"\n", name ) ;
		return false ;
	}
	return true ;
}
