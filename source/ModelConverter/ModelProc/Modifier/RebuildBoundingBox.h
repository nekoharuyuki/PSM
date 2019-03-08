#ifndef	REBUILDBOUNDINGBOX_H_INCLUDE
#define	REBUILDBOUNDINGBOX_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  RebuildBoundingBox
//----------------------------------------------------------------

class RebuildBoundingBox : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "RebuildBoundingBox" ; }
	virtual const char *GetProcUsage() const { return "RebuildBoundingBox $target off/on" ; }
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
	const mat4 &GetBoneMatrix( MdxBone *bone ) ;
	void SetBounding( MdxBlock *block, const bbox &b ) ;

private:
	int m_flags ;
	bbox m_model_bbox ;
	map<MdxBone *,xform> m_bone_xforms ;
	map<MdxPart *,bbox> m_part_bboxes ;
	map<MdxMesh *,bbox> m_mesh_bboxes ;
} ;


} // namespace mdx

#endif
