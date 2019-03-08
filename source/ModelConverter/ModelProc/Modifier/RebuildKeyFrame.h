#ifndef	REBUILDKEYFRAME_H_INCLUDE
#define	REBUILDKEYFRAME_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  RebuildKeyFrame
//----------------------------------------------------------------

class RebuildKeyFrame : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "RebuildKeyFrame" ; }
	virtual const char *GetProcUsage() const { return "RebuildKeyFrame $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyFCurve( MdxFCurve *fcurve ) ;
} ;


} // namespace mdx

#endif
