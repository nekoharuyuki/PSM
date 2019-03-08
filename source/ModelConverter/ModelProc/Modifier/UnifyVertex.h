#ifndef	UNIFYVERTEX_H_INCLUDE
#define	UNIFYVERTEX_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  UnifyVertex
//----------------------------------------------------------------

class UnifyVertex : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "UnifyVertex" ; }
	virtual const char *GetProcUsage() const { return "UnifyVertex $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyPart( MdxPart *part ) ;
} ;


} // namespace mdx

#endif
