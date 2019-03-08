#include "ModelTool.h"

namespace mdx {

static string trim_namespace( const string &name, const string &space ) ;

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

class MdxShell::impl {
public:
	vector<MdxModule *> m_modules ;

	map<string,string> m_vars ;
	map<string,MdxProc *> m_procs ;
	map<string,MdxBlock *> m_blocks ;
	map<int,MdxFormat *> m_formats ;

	string m_result ;
	int m_error_count ;
	int m_warning_count ;
	int m_debug_level ;
	int m_output_mask ;

	map<string,int> m_namespaces ;
} ;

class ShellModules : public MdxBase {
public:
	ShellModules() ;
	~ShellModules() ;
	MdxModule *Load( const string &filename ) ;
private:
	map<void *,MdxModule *> m_modules ;
} ;

static ShellModules *g_module_manager = 0 ;

//----------------------------------------------------------------
//  MdxShell
//----------------------------------------------------------------

MdxShell::MdxShell()
{
	m_impl = new impl ;
	m_impl->m_error_count = 0 ;
	m_impl->m_warning_count = 0 ;
	m_impl->m_debug_level = 0 ;
	m_impl->m_output_mask = MDX_OUTPUT_ALL ;

	if ( g_module_manager == 0 ) {
		g_module_manager = new ShellModules ;
	} else {
		g_module_manager->AddRef() ;
	}
}

MdxShell::~MdxShell()
{
	map<string,MdxProc *>::iterator i ;
	map<string,MdxBlock *>::iterator j ;
	map<int,MdxFormat *>::iterator k ;
	for ( i = m_impl->m_procs.begin() ; i != m_impl->m_procs.end() ; i ++ ) {
		( i->second )->Release() ;
	}
	for ( j = m_impl->m_blocks.begin() ; j != m_impl->m_blocks.end() ; j ++ ) {
		( j->second )->Release() ;
	}
	for ( k = m_impl->m_formats.begin() ; k != m_impl->m_formats.end() ; k ++ ) {
		( k->second )->Release() ;
	}
	delete m_impl ;

	if ( g_module_manager->Release() == 0 ) {
		g_module_manager = 0 ;
	}
}

MdxShell::MdxShell( const MdxShell &src )
{
	m_impl = new impl ;
	*this = src ;
}

MdxShell &MdxShell::operator=( const MdxShell &src )
{
	m_impl->m_modules = src.m_impl->m_modules ;
	m_impl->m_vars = src.m_impl->m_vars ;
	m_impl->m_result = src.m_impl->m_result ;
	m_impl->m_error_count = src.m_impl->m_error_count ;
	m_impl->m_warning_count = src.m_impl->m_warning_count ;
	m_impl->m_debug_level = src.m_impl->m_debug_level ;
	m_impl->m_output_mask = src.m_impl->m_output_mask ;
	m_impl->m_namespaces = src.m_impl->m_namespaces ;

	map<string,MdxProc *>::iterator i ;
	map<string,MdxBlock *>::iterator j ;
	map<int,MdxFormat *>::iterator k ;
	for ( i = m_impl->m_procs.begin() ; i != m_impl->m_procs.end() ; i ++ ) {
		( i->second )->Release() ;
	}
	for ( j = m_impl->m_blocks.begin() ; j != m_impl->m_blocks.end() ; j ++ ) {
		( j->second )->Release() ;
	}
	for ( k = m_impl->m_formats.begin() ; k != m_impl->m_formats.end() ; k ++ ) {
		( k->second )->Release() ;
	}
	m_impl->m_procs = src.m_impl->m_procs ;
	m_impl->m_blocks = src.m_impl->m_blocks ;
	m_impl->m_formats = src.m_impl->m_formats ;
	for ( i = m_impl->m_procs.begin() ; i != m_impl->m_procs.end() ; i ++ ) {
		( i->second )->AddRef() ;
	}
	for ( j = m_impl->m_blocks.begin() ; j != m_impl->m_blocks.end() ; j ++ ) {
		( j->second )->AddRef() ;
	}
	for ( k = m_impl->m_formats.begin() ; k != m_impl->m_formats.end() ; k ++ ) {
		( k->second )->AddRef() ;
	}
	return *this ;
}

//----------------------------------------------------------------
//  modules
//----------------------------------------------------------------

bool MdxShell::LoadModule( const char *filename, int argc, const char **argv )
{
	if ( filename == 0 ) return false ;
	string fullpath = str_fullpath( str_dirname( GetAppFileName() ), filename ) ;
	#ifdef WIN32
	if ( str_extension( fullpath ) == "" ) fullpath += ".dll" ;
	#else // WIN32
	if ( str_extension( fullpath ) == "" ) fullpath += ".so" ;
	#endif // WIN32

	vector<string> filenames ;
	sys_glob( filenames, fullpath ) ;
	bool wildcard = ( strchr( filename, '*' ) != 0 ) ;
	if ( filenames.empty() && !wildcard ) {
		Error( "module not found \"%s\"\n", filename ) ;
		return false ;
	}

	bool status = true ;
	for ( int i = 0 ; i < (int)filenames.size() ; i ++ ) {
		const string &filename = filenames[ i ] ;
		MdxModule *module = g_module_manager->Load( filename ) ;
		if ( module == 0 ) {
			if ( wildcard ) continue ;
			Error( "load failed \"%s\"\n", filename.c_str() ) ;
			return false ;
		}
		if ( find( m_impl->m_modules.begin(), m_impl->m_modules.end(), module ) != m_impl->m_modules.end() ) {
			continue ;
		}
		m_impl->m_modules.push_back( module ) ;
		const char *version = module->GetLibraryVersion() ;
		if ( version == 0 || strcmp( version, MDX_LIBRARY_VERSION ) != 0 ) {
			Error( "wrong library version \"%s\"\n", filename.c_str() ) ;
			status = false ;
		} else if ( !module->InitModule( this, argc, argv ) ) {
			Error( "init failed \"%s\"\n", filename.c_str() ) ;
			status = false ;
		}
	}
	return status ;
}

//----------------------------------------------------------------
//  objects
//----------------------------------------------------------------

void MdxShell::SetVar( const char *name, const char *value )
{
	if ( name == 0 ) name = "" ;
	if ( value == 0 ) value = "" ;
	m_impl->m_vars[ name ] = value ;
}

const char *MdxShell::GetVar( const char *name ) const
{
	if ( name == 0 ) name = "" ;
	return m_impl->m_vars[ name ].c_str() ;
}

void MdxShell::SetProc( const char *name, MdxProc *proc )
{
	if ( ( name == 0 || name[ 0 ] == '\0' ) && proc != 0 ) name = proc->GetProcName() ;
	if ( name == 0 ) name = "" ;
	const char *space = ( proc == 0 ) ? 0 : proc->GetNameSpace() ;
	if ( space == 0 ) space = "" ;
	string sname = str_trimright( space, ":" ) ;

	//  qualified name

	string qname = ( sname == "" ) ? name : sname + "::" + name ;
	MdxProc *old = m_impl->m_procs[ qname ] ;
	if ( proc != old ) {
		m_impl->m_procs[ qname ] = proc ;
		old->Release() ;
	}

	//  aliased name

	map<string,int>::iterator it ;
	for ( it = m_impl->m_namespaces.begin() ; it != m_impl->m_namespaces.end() ; it ++ ) {
		string name = trim_namespace( qname, it->first ) ;
		if ( name == "" ) continue ;
		MdxProc *old2 = m_impl->m_procs[ name ] ;
		if ( old2 != 0 && old2 != old ) continue ;
		if ( proc != old2 ) {
			m_impl->m_procs[ name ] = proc ;
			old2->Release() ;
			proc->AddRef() ;
		}
	}
}

MdxProc *MdxShell::GetProc( const char *name ) const
{
	if ( name == 0 ) name = "" ;
	return m_impl->m_procs[ name ] ;
}

void MdxShell::SetBlock( const char *name, MdxBlock *block )
{
	if ( ( name == 0 || name[ 0 ] == '\0' ) && block != 0 ) name = block->GetName() ;
	if ( name == 0 ) name = "" ;
	m_impl->m_blocks[ name ]->Release() ;
	m_impl->m_blocks[ name ] = block ;
}

MdxBlock *MdxShell::GetBlock( const char *name ) const
{
	if ( name == 0 ) name = "" ;
	return m_impl->m_blocks[ name ] ;
}

void MdxShell::SetFormat( int id, MdxFormat *format )
{
	if ( id == 0 && format != 0 ) id = format->GetFormatID() ;
	m_impl->m_formats[ id ]->Release() ;
	m_impl->m_formats[ id ] = format ;
}

MdxFormat *MdxShell::GetFormat( int id ) const
{
	return m_impl->m_formats[ id ] ;
}

//----------------------------------------------------------------
//  invoking
//----------------------------------------------------------------

int MdxShell::EvalProc( const char *args, ... )
{
	va_list va ;
	va_start( va, args ) ;
	int code = EvalProcVA( args, va ) ;
	va_end( va ) ;
	return code ;
}

int MdxShell::EvalProcVA( const char *args, va_list va )
{
	if ( args == 0 ) return MDX_EVAL_OK ;
	if ( va == 0 ) return EvalProcArgv( 1, &args ) ;
	vector<const char *> buf ;
	buf.push_back( args ) ;
	const char *arg ;
	do {
		arg = va_arg( va, const char * ) ;
		buf.push_back( arg ) ;
	} while ( arg != 0 ) ;
	return EvalProcArgv( buf.size() - 1, &( buf[ 0 ] ) ) ;
}

int MdxShell::EvalProcArgv( int argc, const char **argv )
{
	SetResult( "" ) ;
	if ( argc < 1 ) return MDX_EVAL_OK ;
	MdxProc *proc = GetProc( argv[ 0 ] ) ;
	if ( proc == 0 ) {
		Error( "invalid proc name \"%s\"\n", argv[ 0 ] ) ;
		return MDX_EVAL_ERROR ;
	}
	return proc->Eval( this, argc, argv ) ;
}

const char *MdxShell::SetResult( const char *result )
{
	if ( result == 0 ) result = "" ;
	m_impl->m_result = result ;
	return result ;
}

const char *MdxShell::GetResult() const
{
	return m_impl->m_result.c_str() ;
}

//----------------------------------------------------------------
//  status
//----------------------------------------------------------------

void MdxShell::Error( const char *format, ... )
{
	va_list va ;
	va_start( va, format ) ;
	ErrorVA( format, va ) ;
	va_end( va ) ;
}

void MdxShell::Warning( const char *format, ... )
{
	va_list va ;
	va_start( va, format ) ;
	WarningVA( format, va ) ;
	va_end( va ) ;
}

void MdxShell::Message( const char *format, ... )
{
	va_list va ;
	va_start( va, format ) ;
	MessageVA( format, va ) ;
	va_end( va ) ;
}

void MdxShell::DebugPrint( int level, const char *format, ... )
{
	va_list va ;
	va_start( va, format ) ;
	DebugPrintVA( level, format, va ) ;
	va_end( va ) ;
}

void MdxShell::ErrorVA( const char *format, va_list va )
{
	m_impl->m_error_count ++ ;

	if ( m_impl->m_output_mask & MDX_OUTPUT_ERROR ) {
		if ( format == 0 ) format = "" ;
		printf( "ERROR : " ) ;
		vprintf( format, va ) ;
	}
}

void MdxShell::WarningVA( const char *format, va_list va )
{
	m_impl->m_warning_count ++ ;

	if ( m_impl->m_output_mask & MDX_OUTPUT_WARNING ) {
		if ( format == 0 ) format = "" ;
		printf( "WARNING : " ) ;
		vprintf( format, va ) ;
	}
}

void MdxShell::MessageVA( const char *format, va_list va )
{
	if ( m_impl->m_output_mask & MDX_OUTPUT_MESSAGE ) {
		if ( format == 0 ) format = "" ;
		vprintf( format, va ) ;
	}
}

void MdxShell::DebugPrintVA( int level, const char *format, va_list va )
{
	if ( m_impl->m_output_mask & MDX_OUTPUT_DEBUG ) {
		if ( m_impl->m_debug_level < level ) return ;
		if ( format == 0 ) format = "" ;
		vprintf( format, va ) ;
	}
}

void MdxShell::SetErrorCount( int count )
{
	m_impl->m_error_count = count ;
}

int MdxShell::GetErrorCount() const
{
	return m_impl->m_error_count ;
}

void MdxShell::SetWarningCount( int count )
{
	m_impl->m_warning_count = count ;
}

int MdxShell::GetWarningCount() const
{
	return m_impl->m_warning_count ;
}

void MdxShell::SetDebugLevel( int level )
{
	m_impl->m_debug_level = level ;
}

int MdxShell::GetDebugLevel() const
{
	return m_impl->m_debug_level ;
}

void MdxShell::SetOutputMask( int mask )
{
	m_impl->m_output_mask = mask ;
}

int MdxShell::GetOutputMask() const
{
	return m_impl->m_output_mask ;
}

//----------------------------------------------------------------
//  namespace
//----------------------------------------------------------------

void MdxShell::ImportNameSpace( const char *space )
{
	if ( space == 0 ) return ;
	string sname = str_trimright( space, ":" ) ;
	if ( sname == "" ) return ;
	if ( m_impl->m_namespaces.find( sname ) != m_impl->m_namespaces.end() ) return ;

	//  enable namespace

	m_impl->m_namespaces[ sname ] = 1 ;

	//  alias procedures

	map<string,MdxProc *>::iterator it ;
	for ( it = m_impl->m_procs.begin() ; it != m_impl->m_procs.end() ; it ++ ) {
		MdxProc *proc = it->second ;
		if ( proc == 0 ) continue ;

		string name = trim_namespace( it->first, sname ) ;
		if ( name == "" ) continue ;
		if ( m_impl->m_procs.find( name ) != m_impl->m_procs.end() ) continue ;
		m_impl->m_procs[ name ] = proc ;
		proc->AddRef() ;
	}
}

//----------------------------------------------------------------
//  ShellModules
//----------------------------------------------------------------

ShellModules::ShellModules()
{
	;
}

ShellModules::~ShellModules()
{
	map<void *,MdxModule *>::iterator i ;
	for ( i = m_modules.begin() ; i != m_modules.end() ; i ++ ) {
		MdxModule *module = i->second ;
		if ( module != 0 ) module->ExitModule() ;
		sys_dlclose( i->first ) ;
	}
	m_modules.clear() ;
}

MdxModule *ShellModules::Load( const string &filename )
{
	void *handle = sys_dlopen( filename ) ;
	if ( handle == 0 ) return 0 ;
	if ( m_modules.find( handle ) != m_modules.end() ) {
		sys_dlclose( handle ) ;
		return m_modules[ handle ] ;
	}
	string name = "MdxModule_" ;
	name += str_rootname( str_tailname( filename ) ) ;
	MdxModule *module = (MdxModule *)sys_dlsym( handle, name.c_str() ) ;
	m_modules[ handle ] = module ;
	return module ;
}

//----------------------------------------------------------------
//  subroutine
//----------------------------------------------------------------

string trim_namespace( const string &name, const string &space )
{
	int len = name.length() ;
	int slen = space.length() ;
	if ( len < slen + 2 ) return "" ;
	if ( name[ slen ] != ':' ) return "" ;
	if ( name.substr( 0, slen ) != space ) return "" ;
	return name.substr( slen + 2, string::npos ) ;
}


} // namespace mdx
