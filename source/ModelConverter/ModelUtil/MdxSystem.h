#ifndef	MDX_SYSTEM_H_INCLUDE
#define	MDX_SYSTEM_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  file
//----------------------------------------------------------------

bool sys_chmod( const string &filename, int mode ) ;
bool sys_unlink( const string &filename ) ;
bool sys_mkdir( const string &filename, int mode = 0775 ) ;
bool sys_rmdir( const string &filename ) ;

//----------------------------------------------------------------
//  stat
//----------------------------------------------------------------

bool sys_exists( const string &filename ) ;
bool sys_isfile( const string &filename ) ;
bool sys_isdirectory( const string &filename ) ;

//----------------------------------------------------------------
//  glob
//----------------------------------------------------------------

int sys_glob( vector<string> &filenames, const string &pattern ) ;

//----------------------------------------------------------------
//  exec
//----------------------------------------------------------------

bool sys_exec( const string &command, bool wait = true ) ;
bool sys_exec( const string &command, string &result, bool wait = true ) ;

//----------------------------------------------------------------
//  dlopen
//----------------------------------------------------------------

void *sys_dlopen( const string &filename ) ;
void sys_dlclose( void *handle ) ;
void *sys_dlsym( void *handle, const string &symbol ) ;

//----------------------------------------------------------------
//  time
//----------------------------------------------------------------

int sys_time() ;
string sys_asctime( int seconds ) ;

//----------------------------------------------------------------
//  getlogin
//----------------------------------------------------------------

string sys_getlogin() ;

//----------------------------------------------------------------
//  tmpfile
//----------------------------------------------------------------

string sys_tmpdir() ;
string sys_mktemp( const string &dir, const string &pre, const string &suf ) ;


} // namespace mdx

#endif
