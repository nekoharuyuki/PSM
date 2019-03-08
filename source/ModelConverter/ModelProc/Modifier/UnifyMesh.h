#ifndef	UNIFYMESH_H_INCLUDE
#define	UNIFYMESH_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  UnifyMesh
//----------------------------------------------------------------

class UnifyMesh : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "UnifyMesh" ; }
	virtual const char *GetProcUsage() const { return "UnifyMesh $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyPart( MdxPart *part, int mode ) ;
} ;


} // namespace mdx

#endif
