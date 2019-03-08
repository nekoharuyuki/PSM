#ifndef	MDX_PROC_H_INCLUDE
#define	MDX_PROC_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxProc
//----------------------------------------------------------------

class MdxProc : public MdxBase {
public:
	MdxProc() ;
	virtual ~MdxProc() ;
	MdxProc( const MdxProc &src ) ;
	MdxProc &operator=( const MdxProc &src ) ;

	//  type information

	virtual const char *GetProcName() const { return "" ; }
	virtual const char *GetProcUsage() const { return "" ; }
	virtual const char *GetProcDescription() const { return "" ; }

	//  namespace

	virtual const char *GetNameSpace() const { return MDX_PROC_NS ; }

	//  invoking

	virtual int Eval( MdxShell *shell, int argc, const char **argv ) ;
	virtual bool Process() ;

	//  convenience functions

	bool Setup( MdxShell *shell, int argc, const char **argv, int min_argc = 0 ) ;
	void Reset() ;
	MdxShell *GetShell() const ;
	MdxFormat *GetFormat() const ;
	int GetArgc() const ;
	const char **GetArgv() const ;
	int GetArgCount() const ;
	const char *GetArg( int num, const char *def = "" ) const ;
	const char *GetVar( const char *name, const char *def = "" ) const ;
	class MdxBlock *GetBlock( const char *name ) const ;
	const char *GetArgName( int num, const char *def = "" ) const ;
	const char *GetArgValue( int num, const char *def = "" ) const ;
	void Error( const char *format, ... ) ;
	void Warning( const char *format, ... ) ;
	void Message( const char *format, ... ) ;
	void DebugPrint( int level, const char *format, ... ) ;

private:
	class impl ;
	impl *m_impl ;

protected:
	MdxShell *m_shell ;
	MdxFormat *m_format ;
	int m_argc ;
	const char **m_argv ;
} ;

//----------------------------------------------------------------
//  MdxCreatorProc
//----------------------------------------------------------------

class MdxCreatorProc : public MdxProc {
public:
	virtual int Eval( MdxShell *shell, int argc, const char **argv ) ;
	virtual bool Create( MdxBlock *&block ) ;
} ;

//----------------------------------------------------------------
//  MdxModifierProc
//----------------------------------------------------------------

class MdxModifierProc : public MdxProc {
public:
	virtual int Eval( MdxShell *shell, int argc, const char **argv ) ;
	virtual bool Modify( MdxBlock *block ) ;
} ;

//----------------------------------------------------------------
//  MdxImporterProc
//----------------------------------------------------------------

class MdxImporterProc : public MdxProc {
public:
	virtual int Eval( MdxShell *shell, int argc, const char **argv ) ;
	virtual bool Import( MdxBlock *&block, const char *filename ) ;
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;
} ;

//----------------------------------------------------------------
//  MdxExporterProc
//----------------------------------------------------------------

class MdxExporterProc : public MdxProc {
public:
	virtual int Eval( MdxShell *shell, int argc, const char **argv ) ;
	virtual bool Export( MdxBlock *block, const char *filename ) ;
} ;


} // namespace mdx

#endif
