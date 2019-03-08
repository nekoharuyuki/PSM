#ifndef	SCALETIME_H_INCLUDE
#define	SCALETIME_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ScaleTime
//----------------------------------------------------------------

class ScaleTime : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "ScaleTime" ; }
	virtual const char *GetProcUsage() const { return "ScaleTime $target off/$scale" ; }
	virtual bool Modify( MdxBlock *block ) ;
} ;


} // namespace mdx

#endif
