#include "ModelTool.h"

namespace mdx {

//----------------------------------------------------------------
//  MdxProc
//----------------------------------------------------------------

MdxProc::MdxProc()
{
	Reset() ;
}

MdxProc::~MdxProc()
{
	;
}

MdxProc::MdxProc( const MdxProc &src )
{
	*this = src ;
}

MdxProc &MdxProc::operator=( const MdxProc &src )
{
	return *this ;
}

//----------------------------------------------------------------
//  invoking
//----------------------------------------------------------------

int MdxProc::Eval( MdxShell *shell, int argc, const char **argv )
{
	int res = MDX_EVAL_ERROR ;
	if ( Setup( shell, argc, argv ) ) {
		if ( Process() ) {
			res = MDX_EVAL_OK ;
		}
	}
	Reset() ;
	return res ;
}

bool MdxProc::Process()
{
	return false ;
}

//----------------------------------------------------------------
//  convenience functions
//----------------------------------------------------------------

bool MdxProc::Setup( MdxShell *shell, int argc, const char **argv, int min_argc )
{
	if ( shell == 0 ) {
		return false ;
	}
	if ( argc < min_argc ) {
		shell->Error( "wrong #args\n" ) ;
		return false ;
	}
	m_format = shell->GetFormat( MDX_FORMAT_ID ) ;
	if ( m_format == 0 ) {
		shell->Error( "MdxFormat not installed\n" ) ;
		return false ;
	}
	m_shell = shell ;
	m_argc = argc ;
	m_argv = argv ;
	return true ;
}

void MdxProc::Reset()
{
	m_shell = 0 ;
	m_format = 0 ;
	m_argc = 0 ;
	m_argv = 0 ;
}

MdxShell *MdxProc::GetShell() const
{
	return m_shell ;
}

MdxFormat *MdxProc::GetFormat() const
{
	return m_format ;
}

int MdxProc::GetArgc() const
{
	return m_argc ;
}

const char **MdxProc::GetArgv() const
{
	return m_argv ;
}

int MdxProc::GetArgCount() const
{
	return m_argc ;
}

const char *MdxProc::GetArg( int num, const char *def ) const
{
	return GetArgValue( num, def ) ;
}

const char *MdxProc::GetVar( const char *name, const char *def ) const
{
	if ( m_shell == 0 ) return def ;
	const char *var = m_shell->GetVar( name ) ;
	return ( var[ 0 ] == '\0' ) ? def : var ;
}

class MdxBlock *MdxProc::GetBlock( const char *name ) const
{
	if ( m_shell == 0 ) return 0 ;
	return m_shell->GetBlock( name ) ;
}

const char *MdxProc::GetArgName( int num, const char *def ) const
{
	//  argument name

	const char *arg = ( num < 0 || num >= m_argc ) ? def : m_argv[ num ] ;
	return ( arg[ 0 ] != '$' ) ? def : arg + 1 ;
}

const char *MdxProc::GetArgValue( int num, const char *def ) const
{
	//  argument value

	const char *arg = ( num < 0 || num >= m_argc ) ? def : m_argv[ num ] ;
	if ( arg[ 0 ] == '$' ) arg = m_shell->GetVar( arg + 1 ) ;
	return ( arg[ 0 ] == '\0' ) ? def : arg ;
}

void MdxProc::Error( const char *format, ... )
{
	if ( m_shell == 0 ) return ;
	va_list va ;
	va_start( va, format ) ;
	m_shell->ErrorVA( format, va ) ;
	va_end( va ) ;
}

void MdxProc::Warning( const char *format, ... )
{
	if ( m_shell == 0 ) return ;
	va_list va ;
	va_start( va, format ) ;
	m_shell->WarningVA( format, va ) ;
	va_end( va ) ;
}

void MdxProc::Message( const char *format, ... )
{
	if ( m_shell == 0 ) return ;
	va_list va ;
	va_start( va, format ) ;
	m_shell->MessageVA( format, va ) ;
	va_end( va ) ;
}

void MdxProc::DebugPrint( int level, const char *format, ... )
{
	if ( m_shell == 0 ) return ;
	va_list va ;
	va_start( va, format ) ;
	m_shell->DebugPrintVA( level, format, va ) ;
	va_end( va ) ;
}


//----------------------------------------------------------------
//  MdxCreatorProc
//----------------------------------------------------------------

int MdxCreatorProc::Eval( MdxShell *shell, int argc, const char *argv[] )
{
	int res = MDX_EVAL_ERROR ;
	if ( Setup( shell, argc, argv, 2 ) ) {
		MdxBlock *block = 0 ;
		if ( Create( block ) ) {
			shell->SetBlock( argv[ 1 ], block ) ;
			res = MDX_EVAL_OK ;
		}
	}
	Reset() ;
	return res ;
}

bool MdxCreatorProc::Create( MdxBlock *&block )
{
	return false ;
}

//----------------------------------------------------------------
//  MdxModifierProc
//----------------------------------------------------------------

int MdxModifierProc::Eval( MdxShell *shell, int argc, const char *argv[] )
{
	int res = MDX_EVAL_ERROR ;
	if ( Setup( shell, argc, argv, 2 ) ) {
		MdxBlock *block = shell->GetBlock( argv[ 1 ] ) ;
		if ( block == 0 ) {
			shell->Error( "wrong target \"%s\"\n", argv[ 1 ] ) ;
		} else if ( Modify( block ) ) {
			res = MDX_EVAL_OK ;
		}
	}
	Reset() ;
	return res ;
}

bool MdxModifierProc::Modify( MdxBlock *block )
{
	return false ;
}

//----------------------------------------------------------------
//  MdxImporterProc
//----------------------------------------------------------------

int MdxImporterProc::Eval( MdxShell *shell, int argc, const char *argv[] )
{
	int res = MDX_EVAL_ERROR ;
	if ( Setup( shell, argc, argv, 3 ) ) {
		MdxBlock *block = 0 ;
		if ( Import( block, argv[ 2 ] ) ) {
			shell->SetBlock( argv[ 1 ], block ) ;
			res = MDX_EVAL_OK ;
		}
	}
	Reset() ;
	return res ;
}

bool MdxImporterProc::Import( MdxBlock *&block, const char *filename )
{
	block = 0 ;
	bin_ifstream stream ;
	if ( !stream.open( filename ) ) {
		m_shell->Error( "open failed \"%s\"\n", filename ) ;
		return false ;
	}
	int size = stream.get_filesize() ;
	char *buf = new char[ size ] ;
	stream.read( buf, size ) ;
	stream.close() ;
	bool state = Import( block, buf, size ) ;
	delete[] buf ;
	return state ;
}

bool MdxImporterProc::Import( MdxBlock *&block, const void *buf, int size )
{
	return false ;
}

//----------------------------------------------------------------
//  MdxExporterProc
//----------------------------------------------------------------

int MdxExporterProc::Eval( MdxShell *shell, int argc, const char *argv[] )
{
	int res = MDX_EVAL_ERROR ;
	if ( Setup( shell, argc, argv, 3 ) ) {
		MdxBlock *block = shell->GetBlock( argv[ 1 ] ) ;
		if ( block == 0 ) {
			shell->Error( "wrong target \"%s\"\n", argv[ 1 ] ) ;
		} else if ( Export( block, argv[ 2 ] ) ) {
			res = MDX_EVAL_OK ;
		}
	}
	Reset() ;
	return res ;
}

bool MdxExporterProc::Export( MdxBlock *block, const char *filename )
{
	return false ;
}


} // namespace mdx
