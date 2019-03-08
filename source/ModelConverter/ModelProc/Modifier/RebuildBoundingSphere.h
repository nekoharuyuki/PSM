#ifndef	REBUILDBOUNDINGSPHERE_H_INCLUDE
#define	REBUILDBOUNDINGSPHERE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  RebuildBoundingSphere
//----------------------------------------------------------------

class RebuildBoundingSphere : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "RebuildBoundingSphere" ; }
	virtual const char *GetProcUsage() const { return "RebuildBoundingSphere $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	struct xform {
		mat4 stack_matrix ;
		vec4 stack_scale ;
		vec4 comp_scale ;
		mat4 world_matrix ;
	public:
		xform() ;
		void clear() ;
		void fix() ;
	} ;

private:
	struct bbox {
		vec4 lower ;
		vec4 upper ;
		float radius ;
	public:
		bbox() ;
		void clear() ;
		bool valid() const ;
		void extend( const vec4 &v ) ;
		void extend( const bbox &b ) ;
	} ;

private:
	void ModifyModel( MdxModel *model ) ;
	void ModifyMesh( MdxMesh *mesh ) ;
	void ModifyPart( MdxPart *part ) ;
	void ModifyBone( MdxBone *bone ) ;

	void GetModelBox( bbox &b, MdxModel *model ) ;
	void GetModelSphere( bbox &b, MdxModel *model, const vec4 &center ) ;
	void GetBoneBox( bbox &b, MdxBone *bone ) ;
	void GetBoneSphere( bbox &b, MdxBone *bone, const vec4 &center, const mat4 *transform = 0 ) ;
	void GetPartBox( bbox &b, MdxPart *part ) ;
	void GetPartSphere( bbox &b, MdxPart *part, const vec4 &center, const mat4 *transform = 0 ) ;
	void GetMeshBox( bbox &b, MdxMesh *mesh ) ;
	void GetMeshSphere( bbox &b, MdxMesh *mesh, const vec4 &center, const mat4 *transform = 0 ) ;

	const mat4 &GetBoneMatrix( MdxBone *bone ) ;
	void SetBoundingSphere( MdxBlock *block, const bbox &b ) ;

private:
	int m_flags ;
	map<MdxBone *,xform> m_bone_xforms ;
	map<MdxBone *,bbox> m_bone_bboxes ;
	map<MdxPart *,bbox> m_part_bboxes ;
	map<MdxMesh *,bbox> m_mesh_bboxes ;
} ;


} // namespace mdx

#endif
