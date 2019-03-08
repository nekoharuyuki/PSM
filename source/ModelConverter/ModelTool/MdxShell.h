#ifndef	MDX_SHELL_H_INCLUDE
#define	MDX_SHELL_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxShell
//----------------------------------------------------------------

class MdxShell : public MdxBase {
public:
	MdxShell() ;
	virtual ~MdxShell() ;
	MdxShell( const MdxShell &src ) ;
	MdxShell &operator=( const MdxShell &src ) ;

	//  type information

	virtual const char *GetAppName() const { return "" ; }
	virtual const char *GetAppVersion() const { return "" ; }
	virtual const char *GetAppFileName() const { return "" ; }

	//  modules

	virtual bool LoadModule( const char *filename, int argc = 0, const char **argv = 0 ) ;

	//  objects

	virtual void SetVar( const char *name, const char *value ) ;
	virtual const char *GetVar( const char *name ) const ;
	virtual void SetProc( const char *name, class MdxProc *proc ) ;
	virtual class MdxProc *GetProc( const char *name ) const ;
	virtual void SetBlock( const char *name, MdxBlock *block ) ;
	virtual MdxBlock *GetBlock( const char *name ) const ;
	virtual void SetFormat( int id, MdxFormat *format ) ;
	virtual MdxFormat *GetFormat( int id ) const ;

	//  invoking

	int EvalProc( const char *args, ... ) ;
	int EvalProcVA( const char *args, va_list va ) ;
	virtual int EvalProcArgv( int argc, const char **argv ) ;
	virtual const char *SetResult( const char *result ) ;
	virtual const char *GetResult() const ;

	//  status

	void Error( const char *format, ... ) ;
	void Warning( const char *format, ... ) ;
	void Message( const char *format, ... ) ;
	void DebugPrint( int level, const char *format, ... ) ;
	virtual void ErrorVA( const char *format, va_list va ) ;
	virtual void WarningVA( const char *format, va_list va ) ;
	virtual void MessageVA( const char *format, va_list va ) ;
	virtual void DebugPrintVA( int level, const char *format, va_list va ) ;

	virtual void SetErrorCount( int count ) ;
	virtual int GetErrorCount() const ;
	virtual void SetWarningCount( int count ) ;
	virtual int GetWarningCount() const ;
	virtual void SetDebugLevel( int level ) ;
	virtual int GetDebugLevel() const ;
	virtual void SetOutputMask( int mask ) ;
	virtual int GetOutputMask() const ;

	//  namespace

	virtual void ImportNameSpace( const char *name ) ;

private:
	class impl ;
	impl *m_impl ;
} ;


} // namespace mdx

#endif
