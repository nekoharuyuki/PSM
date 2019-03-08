#ifndef	LIMITVERTEXBLEND100_H_INCLUDE
#define	LIMITVERTEXBLEND100_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  LimitVertexBlend100
//----------------------------------------------------------------

class LimitVertexBlend100 : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "LimitVertexBlend100" ; }
	virtual const char *GetProcUsage() const { return "LimitVertexBlend100 $target off/on/$count" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	struct face {
		unsigned short prim ;		//  prim type
		unsigned short n_verts ;	//  vertex count
		unsigned short verts[ 4 ] ;	//  vertex indices
	} ;
	class widx_group : public vector<int> {} ;
	class face_group : public vector<int> {
	public:
		face_group() : cost( 0 ) {}
		int cost ;			//  total cost
	} ;

private:
	void ModifyPart( MdxPart *part ) ;
	void ModifyMesh( MdxMesh *mesh ) ;

	bool GetFaces( MdxDrawArrays *cmd ) ;
	bool PutFaces( MdxArrays *arrays, MdxMesh *mesh ) ;
	void CheckFaces( MdxArrays *arrays ) ;
	bool AppendFace( int prim, int n_verts, int a, int b = 0, int c = 0, int d = 0 ) ;

private:
	int m_limit ;

	vector<face> m_faces ;
	bool m_need_subset ;

	map<MdxMesh *, MdxMesh *> m_mesh_output ;
	map<MdxArrays *, MdxArrays *> m_arrays_output ;
} ;


} // namespace mdx

#endif
