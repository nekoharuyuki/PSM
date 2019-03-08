#ifndef	UNIFYTEXTURE_H_INCLUDE
#define	UNIFYTEXTURE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  UnifyTexture
//----------------------------------------------------------------

class UnifyTexture : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "UnifyTexture" ; }
	virtual const char *GetProcUsage() const { return "UnifyTexture $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyReference( MdxTexture *texture, MdxTexture *texture2 ) ;
} ;


} // namespace mdx

#endif
