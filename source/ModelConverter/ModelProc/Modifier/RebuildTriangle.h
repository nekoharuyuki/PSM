#ifndef	REBUILDTRIANGLE_H_INCLUDE
#define	REBUILDTRIANGLE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  RebuildTriangle
//----------------------------------------------------------------

class RebuildTriangle : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "RebuildTriangle" ; }
	virtual const char *GetProcUsage() const { return "RebuildTriangle $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ModifyMesh( MdxMesh *mesh ) ;

	bool GetTriangles( vector<int> &indices, MdxDrawArrays *cmd ) ;
	bool PutTriangles( vector<int> &indices, const string &arrays, MdxBlocks &blocks ) ;
	bool AppendTriangle( vector<int> &indices, int a, int b, int c ) ;
} ;


} // namespace mdx

#endif
