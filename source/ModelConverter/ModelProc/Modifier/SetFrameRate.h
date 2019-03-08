#ifndef	SETFRAMERATE_H_INCLUDE
#define	SETFRAMERATE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  SetFrameRate
//----------------------------------------------------------------

class SetFrameRate : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "SetFrameRate" ; }
	virtual const char *GetProcUsage() const { return "SetFrameRate $target default/off/on/$fps" ; }
	virtual bool Modify( MdxBlock *block ) ;
} ;


} // namespace mdx

#endif
