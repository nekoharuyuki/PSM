#include "ModelUtil.h"
#ifdef WIN32
#include <windows.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <time.h>
#include <io.h>
#include <direct.h>
#include <fcntl.h>
#else // WIN32
#include <glob.h>
#include <dlfcn.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>
#include <sys/time.h>
#include <time.h>
#include <fcntl.h>
#endif // WIN32

namespace mdx {

//----------------------------------------------------------------
//  file
//----------------------------------------------------------------

bool sys_chmod( const string &filename, int mode )
{
#ifdef WIN32
	return ( _chmod( filename.c_str(), mode ) == 0 ) ;
#else // WIN32
	return ( chmod( filename.c_str(), mode ) == 0 ) ;
#endif // WIN32
}

bool sys_unlink( const string &filename )
{
#ifdef WIN32
	return ( _unlink( filename.c_str() ) == 0 ) ;
#else // WIN32
	return ( unlink( filename.c_str() ) == 0 ) ;
#endif // WIN32
}

bool sys_mkdir( const string &filename, int mode )
{
#ifdef WIN32
	return ( _mkdir( filename.c_str() ) == 0 ) ;
#else // WIN32
	return ( mkdir( filename.c_str(), mode ) == 0 ) ;
#endif // WIN32
}

bool sys_rmdir( const string &filename )
{
#ifdef WIN32
	return ( _rmdir( filename.c_str() ) == 0 ) ;
#else // WIN32
	return ( rmdir( filename.c_str() ) == 0 ) ;
#endif // WIN32
}

//----------------------------------------------------------------
//  stat
//----------------------------------------------------------------

bool sys_exists( const string &filename )
{
#ifdef WIN32
	struct _stat buf ;
	return ( _stat( str_trimright( filename, "/\\" ).c_str(), &buf ) == 0 ) ;
#else // WIN32
	struct stat buf ;
	return ( stat( str_trimright( filename, "/\\" ).c_str(), &buf ) == 0 ) ;
#endif // WIN32
}

bool sys_isfile( const string &filename )
{
#ifdef WIN32
	struct _stat buf ;
	if ( _stat( str_trimright( filename, "/\\" ).c_str(), &buf ) != 0 ) return false ;
	return ( ( buf.st_mode & S_IFREG ) != 0 ) ;
#else // WIN32
	struct stat buf ;
	if ( stat( str_trimright( filename, "/\\" ).c_str(), &buf ) != 0 ) return false ;
	return ( ( buf.st_mode & S_IFREG ) != 0 ) ;
#endif // WIN32
}

bool sys_isdirectory( const string &filename )
{
#ifdef WIN32
	struct _stat buf ;
	if ( _stat( str_trimright( filename, "/\\" ).c_str(), &buf ) != 0 ) return false ;
	return ( ( buf.st_mode & S_IFDIR ) != 0 ) ;
#else // WIN32
	struct stat buf ;
	if ( stat( str_trimright( filename, "/\\" ).c_str(), &buf ) != 0 ) return false ;
	return ( ( buf.st_mode & S_IFDIR ) != 0 ) ;
#endif // WIN32
}

//----------------------------------------------------------------
//  glob
//----------------------------------------------------------------

int sys_glob( vector<string> &filenames, const string &pattern )
{
#ifdef WIN32
	filenames.clear() ;
	string dirname = str_dirname( pattern ) ;
	WIN32_FIND_DATA find_data ;
	HANDLE find_handle = FindFirstFile( pattern.c_str(), &find_data ) ;
	if ( find_handle != INVALID_HANDLE_VALUE ) {
		do {
			string filename = str_fullpath( dirname, find_data.cFileName ) ;
			filenames.push_back( filename ) ;
		} while ( FindNextFile( find_handle, &find_data ) ) ;
		FindClose( find_handle ) ;
	}
	return filenames.size() ;
#else // WIN32
	glob_t buf ;
	glob( pattern.c_str(), 0, 0, &buf ) ;
	filenames.clear() ;
	for ( int i = 0 ; i < (int)buf.gl_pathc ; i ++ ) {
		filenames.push_back( buf.gl_pathv[ i ] ) ;
	}
	globfree( &buf ) ;
	return filenames.size() ;
#endif // WIN32
}

//----------------------------------------------------------------
//  exec
//----------------------------------------------------------------

bool sys_exec( const string &command, bool wait )
{
	string tmp ;
	return sys_exec( command, tmp, wait ) ;
}

bool sys_exec( const string &command, string &result, bool wait )
{
#ifdef WIN32
	result = "" ;

	STARTUPINFO si ;
	memset( &si, 0, sizeof( si ) ) ;
	si.cb = sizeof( si ) ;
	si.dwFlags = STARTF_USESTDHANDLES ;
	si.hStdInput = INVALID_HANDLE_VALUE ;
	si.hStdOutput = GetStdHandle( STD_OUTPUT_HANDLE ) ;

	PROCESS_INFORMATION pi ;
	if ( !wait ) {
		char *cp = (char *)command.c_str() ;
		return ( CreateProcess( 0, cp, 0, 0, TRUE, 0, 0, 0, &si, &pi ) != 0 ) ;
	}

	SECURITY_ATTRIBUTES sa ;
	HANDLE pr, pw ;
	sa.nLength = sizeof( sa ) ;
	sa.lpSecurityDescriptor = 0 ;
	sa.bInheritHandle = TRUE ;
	CreatePipe( &pr, &pw, &sa, 0 ) ;
	si.hStdOutput = pw ;

	char *cp = (char *)command.c_str() ;
	if ( CreateProcess( 0, cp, 0, 0, TRUE, 0, 0, 0, &si, &pi ) == 0 ) {
		return false ;
	}
	WaitForInputIdle( pi.hProcess, INFINITE ) ;
	WaitForSingleObject( pi.hProcess, INFINITE ) ;
	DWORD code = 0 ;
	GetExitCodeProcess( pi.hProcess, &code ) ;
	CloseHandle( pi.hThread ) ;
	CloseHandle( pi.hProcess ) ;

	DWORD count ;
	while ( PeekNamedPipe( pr, NULL, 0, NULL, &count, NULL) && count > 0 ) {
		char buf[ 1024 ] ;
		if ( ReadFile( pr, buf, sizeof( buf ) - 1, &count, NULL ) ) {
			buf[ count ] = '\0' ;
			result += buf ;
		}
	}
	CloseHandle( pr ) ;
	CloseHandle( pw ) ;
	// return true ;
	return ( code == 0 ) ;
#else // WIN32
	// return ( system( command.c_str() ) == 0 ) ;
	int code = system( command.c_str() ) ;
	return ( WIFEXITED( code ) && ( WEXITSTATUS( code ) == 0 ) ) ;
#endif // WIN32
}

//----------------------------------------------------------------
//  dlopen
//----------------------------------------------------------------

void *sys_dlopen( const string &filename )
{
#ifdef WIN32
	// return (void *)LoadLibraryEx( filename.c_str(), 0, LOAD_WITH_ALTERED_SEARCH_PATH ) ;
	return (void *)LoadLibrary( filename.c_str() ) ;
#else // WIN32
	// return dlopen( filename.c_str(), RTLD_LAZY | RTLD_GLOBAL | RTLD_DEEPBIND ) ;
	return dlopen( filename.c_str(), RTLD_LAZY | RTLD_GLOBAL ) ;
#endif // WIN32
}

void sys_dlclose( void *handle )
{
#ifdef WIN32
	if ( handle == 0 ) return ;
	FreeLibrary( (HINSTANCE)handle ) ;
#else // WIN32
	if ( handle == 0 ) return ;
	dlclose( handle ) ;
#endif // WIN32
}

void *sys_dlsym( void *handle, const string &symbol )
{
#ifdef WIN32
	if ( handle == 0 ) return 0 ;
	return GetProcAddress( (HINSTANCE)handle, symbol.c_str() ) ;
#else // WIN32
	if ( handle == 0 ) return 0 ;
	return dlsym( handle, symbol.c_str() ) ;
#endif // WIN32
}

//----------------------------------------------------------------
//  time
//----------------------------------------------------------------

int sys_time()
{
	return time( 0 ) ;
}

string sys_asctime( int seconds )
{
	time_t t = seconds ;
	string a = asctime( localtime( &t ) ) ;
	if ( a != "" ) a.erase( a.length() - 1 ) ;
	return a ;
}

//----------------------------------------------------------------
//  getlogin
//----------------------------------------------------------------

string sys_getlogin()
{
#ifdef WIN32
	char name[ 256 ] ;
	DWORD size = sizeof( name ) ;
	return ( GetUserName( name, &size ) != 0 ) ? name : "nobody" ;
#else // WIN32
	const char *name = getlogin() ;
	return ( name != 0 ) ? name : "nobody" ;
#endif // WIN32
}

//----------------------------------------------------------------
//  tmpfile
//----------------------------------------------------------------

string sys_tmpdir()
{
#ifdef WIN32
	char path[ MAX_PATH ] ;
	GetTempPath( MAX_PATH, path ) ;
	return path ;
#else // WIN32
	return P_tmpdir ;
#endif // WIN32
}

string sys_mktemp( const string &dir, const string &pre, const string &suf )
{
#ifdef WIN32
	static bool initialized = false ;
	if ( !initialized ) srand( sys_time() ) ;
	initialized = true ;

	int id = rand() ^ ( rand() << 16 ) ;
	for ( int i = 0 ; i < 65536 ; i ++ ) {
		string name = str_format( "%s%08x%s", pre.c_str(), id ++, suf.c_str() ) ;
		string path = str_fullpath( dir, name ) ;
		int fd = _open( path.c_str(), O_CREAT | O_EXCL, 0664 ) ;
		if ( fd != -1 ) {
			_close( fd ) ;
			return path ;
		}
	}
	return "" ;
#else // WIN32
	static bool initialized = false ;
	if ( !initialized ) srand( sys_time() ) ;
	initialized = true ;

	int id = rand() ^ ( rand() << 16 ) ;
	for ( int i = 0 ; i < 65536 ; i ++ ) {
		string name = str_format( "%s%08x%s", pre.c_str(), id ++, suf.c_str() ) ;
		string path = str_fullpath( dir, name ) ;
		int fd = open( path.c_str(), O_CREAT | O_EXCL, 0664 ) ;
		if ( fd != -1 ) {
			close( fd ) ;
			return path ;
		}
	}
	return "" ;
#endif // WIN32
}

} // namespace mdx
