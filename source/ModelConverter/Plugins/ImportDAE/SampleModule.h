#ifndef	SAMPLEMODULE_H_INCLUDE
#define	SAMPLEMODULE_H_INCLUDE

#include "ModelProc/ModelProc.h"

#ifdef WIN32
#ifdef SAMPLEMODULE_EXPORTS
#define SAMPLEMODULE_API __declspec(dllexport)
#else // SAMPLEMODULE_EXPORTS
#define SAMPLEMODULE_API __declspec(dllimport)
#endif // SAMPLEMODULE_EXPORTS
#else // WIN32
#define SAMPLEMODULE_API
#endif // WIN32

//----------------------------------------------------------------
//  module entry
//----------------------------------------------------------------

#define SAMPLEMODULE	MdxModule_ImportDAE

extern "C" SAMPLEMODULE_API class SAMPLEMODULE SAMPLEMODULE ;

class SAMPLEMODULE : public MdxModule {
	virtual const char *GetLibraryVersion() const { return MDX_LIBRARY_VERSION ; }
	virtual const char *GetModuleName() const { return "ImportDAE" ; }
	virtual const char *GetModuleVersion() const { return "0.1" ; }
	virtual const char *GetModuleCopyright() const { return "(c) HI" ; }
	virtual const char *GetModuleDescription() const { return "DAE Importer" ; }
	virtual bool InitModule( MdxShell *shell, int argc, const char **argv ) ;
	virtual void ExitModule() ;
} ;

#endif
