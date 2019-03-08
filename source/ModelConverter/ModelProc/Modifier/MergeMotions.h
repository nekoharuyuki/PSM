#ifndef	MERGEMOTIONS_H_INCLUDE
#define	MERGEMOTIONS_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  MergeMotions
//----------------------------------------------------------------

class MergeMotions : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "MergeMotions" ; }
	virtual const char *GetProcUsage() const { return "MergeMotions $target $source ..." ; }
	virtual bool Modify( MdxBlock *block ) ;

protected:
	void MatchTarget( MdxMotion *motion, MdxModel *model, MdxModel *model2 ) ;
	void MatchBasePose( MdxMotion *motion, MdxModel *model, MdxModel *model2 ) ;

	bool MatchParam( MdxMotion *motion, MdxBlock *block, MdxBlock *block2, int type, int type2 = -1, int type3 = -1 ) ;
	MdxAnimate *FindAnim( MdxMotion *motion, MdxBlock *block, int type, int type2 = -1, int type3 = -1 ) ;
	MdxCommand *FindChild( MdxBlock *block, int type, int type2 = -1, int type3 = -1 ) ;
	MdxCommand *GetTemplate( MdxCommand *cmd ) ;

protected:
	MdxBlocks m_tmp ;
} ;


} // namespace mdx

#endif
