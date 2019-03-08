#ifndef	LIMITVERTEXBLEND_H_INCLUDE
#define	LIMITVERTEXBLEND_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  LimitVertexBlend
//----------------------------------------------------------------

class LimitVertexBlend : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "LimitVertexBlend" ; }
	virtual const char *GetProcUsage() const { return "LimitVertexBlend $target off/on/$count" ; }
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
		face_group() : n_verts( 0 ), n_weights( 0 ) {}
		int n_verts ;			//  vertex count
		int n_weights ;			//  weight count
	} ;

private:
	void ModifyPart( MdxPart *part ) ;
	void ModifyMesh( MdxMesh *mesh, int n_bones ) ;

	bool GetFaces( MdxDrawArrays *cmd ) ;
	bool PutFaces( MdxArrays *arrays, MdxMesh *mesh ) ;
	void CheckFaces( MdxArrays *arrays ) ;
	bool AppendFace( int prim, int n_verts, int a, int b = 0, int c = 0, int d = 0 ) ;

private:
	int m_limit_weight ;
	int m_limit_matrix ;

	vector<face> m_faces ;
	map<MdxMesh *, MdxMesh *> m_mesh_output ;
	map<MdxArrays *, MdxArrays *> m_arrays_output ;
} ;


} // namespace mdx

#endif
