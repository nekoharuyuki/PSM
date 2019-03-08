#ifndef	SORTBYTYPE_H_INCLUDE
#define	SORTBYTYPE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  SortByType
//----------------------------------------------------------------

class SortByType : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "SortByType" ; }
	virtual const char *GetProcUsage() const { return "SortByType $target off/on/block/command/partition" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyBlocks( MdxBlock *block ) ;

private:
	int m_flags ;
} ;


} // namespace mdx

#endif
