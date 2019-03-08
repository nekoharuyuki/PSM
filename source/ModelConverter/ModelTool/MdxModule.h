#ifndef	MDX_MODULE_H_INCLUDE
#define	MDX_MODULE_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxModule
//----------------------------------------------------------------

class MdxModule : public MdxBase {
public:
	MdxModule() ;
	virtual ~MdxModule() ;

	//  type information

	virtual const char *GetLibraryVersion() const { return "" ; }
	virtual const char *GetModuleName() const { return "" ; }
	virtual const char *GetModuleVersion() const { return "" ; }
	virtual const char *GetModuleCopyright() const { return "" ; }
	virtual const char *GetModuleDescription() const { return "" ; }

	//  actions

	virtual bool InitModule( MdxShell *shell, int argc = 0, const char **argv = 0 ) ;
	virtual void ExitModule() ;

private:
	class impl ;
	impl *m_impl ;
} ;


} // namespace mdx

#endif
