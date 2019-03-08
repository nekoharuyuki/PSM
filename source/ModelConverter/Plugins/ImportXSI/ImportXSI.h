#ifndef	IMPORTXSI_H_INCLUDE
#define	IMPORTXSI_H_INCLUDE

#include "ModelProc/ModelProc.h"
#include "SampleModule.h"

//----------------------------------------------------------------
//  ImportXSI
//----------------------------------------------------------------

class ImportXSI : public MdxImporterProc {
public:
	virtual const char *GetProcName() const { return "ImportXSI" ; }
	virtual const char *GetProcUsage() const { return "ImportXSI $target $filename" ; }
	virtual bool Import( MdxBlock *&block, const char *filename ) ;
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;
} ;

#endif
