#ifndef	IMPORTFBX_H_INCLUDE
#define	IMPORTFBX_H_INCLUDE

#include "ModelProc/ModelProc.h"
#include "SampleModule.h"

//----------------------------------------------------------------
//  ImportFBX
//----------------------------------------------------------------

class ImportFBX : public MdxImporterProc {
public:
	virtual const char *GetProcName() const { return "ImportFBX" ; }
	virtual const char *GetProcUsage() const { return "ImportFBX $target $filename" ; }
	virtual bool Import( MdxBlock *&block, const char *filename ) ;
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;
} ;

#endif
