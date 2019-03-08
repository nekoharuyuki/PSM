#ifndef	UNIFYARRAYS_H_INCLUDE
#define	UNIFYARRAYS_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  UnifyArrays
//----------------------------------------------------------------

class UnifyArrays : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "UnifyArrays" ; }
	virtual const char *GetProcUsage() const { return "UnifyArrays $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyPart( MdxPart *part ) ;
} ;


} // namespace mdx

#endif
