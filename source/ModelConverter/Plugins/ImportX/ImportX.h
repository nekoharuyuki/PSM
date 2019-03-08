#ifndef	IMPORTX_H_INCLUDE
#define	IMPORTX_H_INCLUDE

#include "ModelProc/ModelProc.h"
#include "SampleModule.h"

//----------------------------------------------------------------
//  ImportX
//----------------------------------------------------------------

class ImportX : public MdxImporterProc {
public:
	virtual const char *GetProcName() const { return "ImportX" ; }
	virtual const char *GetProcUsage() const { return "ImportX $target $filename" ; }
	virtual bool Import( MdxBlock *&block, const char *filename ) ;
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;
} ;

#endif
