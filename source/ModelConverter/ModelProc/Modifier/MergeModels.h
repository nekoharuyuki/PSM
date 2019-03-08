#ifndef	MERGEMODELS_H_INCLUDE
#define	MERGEMODELS_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  MergeModels
//----------------------------------------------------------------

class MergeModels : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "MergeModels" ; }
	virtual const char *GetProcUsage() const { return "MergeModels $target $source ..." ; }
	virtual bool Modify( MdxBlock *block ) ;
} ;


} // namespace mdx

#endif
