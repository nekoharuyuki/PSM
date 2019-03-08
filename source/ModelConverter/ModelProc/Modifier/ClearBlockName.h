#ifndef	CLEARBLOCKNAME_H_INCLUDE
#define	CLEARBLOCKNAME_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ClearBlockName
//----------------------------------------------------------------

class ClearBlockName : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "ClearBlockName" ; }
	virtual const char *GetProcUsage() const { return "ClearBlockName $target off/on/$level" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyBlock( MdxBlock *block, int clear_level, int current_level ) ;
} ;


} // namespace mdx

#endif
