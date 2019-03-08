#ifndef	SCALESIZE_H_INCLUDE
#define	SCALESIZE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ScaleSize
//----------------------------------------------------------------

class ScaleSize : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "ScaleSize" ; }
	virtual const char *GetProcUsage() const { return "ScaleSize $target off/$scale" ; }
	virtual bool Modify( MdxBlock *block ) ;
} ;


} // namespace mdx

#endif
