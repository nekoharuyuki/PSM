#ifndef	UNIFYMATERIAL_H_INCLUDE
#define	UNIFYMATERIAL_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  UnifyMaterial
//----------------------------------------------------------------

class UnifyMaterial : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "UnifyMaterial" ; }
	virtual const char *GetProcUsage() const { return "UnifyMaterial $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyReference( MdxMaterial *material, MdxMaterial *material2 ) ;
} ;


} // namespace mdx

#endif
