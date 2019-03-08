#ifndef	COMPFCURVECHANNEL_H_INCLUDE
#define	COMPFCURVECHANNEL_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  CompFCurveChannel
//----------------------------------------------------------------

class CompFCurveChannel : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "CompFCurveChannel" ; }
	virtual const char *GetProcUsage() const { return "CompFCurveChannel $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	struct channel {
		MdxFCurve *fcurve ;
		int offset ;
		float value ;
	public:
		channel() {
			fcurve = 0 ;
			offset = 0 ;
			value = 0.0f ;
		}
	} ;

private:
	void ModifyMotion( MdxMotion *motion ) ;
	bool CheckTarget( MdxAnimate *anim ) ;
	bool UnifyAnims( MdxAnimate *anim, MdxBlocks &cmds, int num ) ;
	bool MergeFCurves( MdxAnimate *anim ) ;
	void CleanFCurves( MdxMotion *motion ) ;

private:
	vector<channel> m_channels ;
} ;


} // namespace mdx

#endif
