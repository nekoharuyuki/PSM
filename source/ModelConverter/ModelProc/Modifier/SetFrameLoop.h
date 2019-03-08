#ifndef	SETFRAMELOOP_H_INCLUDE
#define	SETFRAMELOOP_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  SetFrameLoop
//----------------------------------------------------------------

class SetFrameLoop : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "SetFrameLoop" ; }
	virtual const char *GetProcUsage() const { return "SetFrameLoop $target default/off/on/$start,$end" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	bool GetFrameRange( MdxMotion *motion, float &start, float &end ) ;
} ;


} // namespace mdx

#endif
