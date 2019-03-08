#ifndef	SETFRAMEREPEAT_H_INCLUDE
#define	SETFRAMEREPEAT_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  SetFrameRepeat
//----------------------------------------------------------------

class SetFrameRepeat : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "SetFrameRepeat" ; }
	virtual const char *GetProcUsage() const { return "SetFrameRepeat $target default/off/on/hold/cycle" ; }
	virtual bool Modify( MdxBlock *block ) ;
} ;


} // namespace mdx

#endif
