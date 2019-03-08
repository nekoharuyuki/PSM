#ifndef MODEL_PROC_H_INCLUDE
#define MODEL_PROC_H_INCLUDE

#include "ModelTool/ModelTool.h"
#include "ModelFormat/ModelFormat.h"

#ifdef WIN32
#ifdef MODELPROC_EXPORTS
#define MODELPROC_API __declspec(dllexport)
#else // MODELPROC_EXPORTS
#define MODELPROC_API __declspec(dllimport)
#endif // MODELPROC_EXPORTS
#else // WIN32
#define MODELPROC_API
#endif // WIN32

namespace mdx {}
using namespace mdx ;

namespace mdx {

//----------------------------------------------------------------
//  module entry
//----------------------------------------------------------------

extern "C" MODELPROC_API class MdxModule_ModelProc MdxModule_ModelProc ;

class MdxModule_ModelProc : public MdxModule {
public:
	virtual const char *GetLibraryVersion() const { return MDX_LIBRARY_VERSION ; }
	virtual const char *GetModuleName() const { return "ModelProc" ; }
	virtual const char *GetModuleVersion() const { return "1.00" ; }
	virtual const char *GetModuleCopyright() const { return "(c) SCEI" ; }
	virtual const char *GetModuleDescription() const { return "Model procedure module" ; }
	virtual bool InitModule( MdxShell *shell, int argc, const char **argv ) ;
	virtual void ExitModule() ;
} ;


} // namespace mdx

#endif
