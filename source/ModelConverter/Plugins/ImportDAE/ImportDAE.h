#ifndef	IMPORTCOLLADA_H_INCLUDE
#define	IMPORTCOLLADA_H_INCLUDE

#include "ModelProc/ModelProc.h"
#include "SampleModule.h"

//----------------------------------------------------------------
//  ImportCollada
//----------------------------------------------------------------

class ImportCollada : public MdxImporterProc {
public:
	virtual const char *GetProcName() const { return "ImportDAE" ; }
	virtual const char *GetProcUsage() const { return "ImportDAE $target $filename" ; }
	virtual bool Import( MdxBlock *&block, const char *filename ) ;
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;
} ;

#endif
