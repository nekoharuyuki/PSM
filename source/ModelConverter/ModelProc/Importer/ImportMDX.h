#ifndef	IMPORTMDX_H_INCLUDE
#define	IMPORTMDX_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ImportMDX
//----------------------------------------------------------------

class ImportMDX : public MdxImporterProc {
public:
	virtual const char *GetProcName() const { return "ImportMDX" ; }
	virtual const char *GetProcUsage() const { return "ImportMDX $target $filename" ; }
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;

private:
	bool ImportHeader( MDXHeader *header, int size ) ;
	bool ImportBlock( MdxBlock *&block, MdxBlock *parent, MDXChunk *chunk ) ;
	bool CreateBlock( MdxBlock *&block, MDXChunk *chunk ) ;
	bool ImportSpecific( MdxBlock *block, MDXChunk *chunk ) ;
	bool ImportGeneric( MdxBlock *block, MDXChunk *chunk ) ;
	bool ImportArgs( MdxBlock *block, MDXChunk *chunk ) ;
	bool ImportData( MdxBlock *block, MDXChunk *chunk ) ;
	bool ImportChildren( MdxBlock *block, MDXChunk *chunk ) ;
	bool CheckDirective( MdxBlock *block ) ;

	bool PreProcess( const void *buf, int size ) ;
	bool PostProcess( MdxBlock *&block ) ;
	bool ResolveReference( MdxBlock *block ) ;

	bool ImportModelPSM( MdxModel *model, MDXChunk *chunk ) ;
	bool ImportVertexOffsetPSM( MdxVertexOffset *cmd, MDXChunk *chunk ) ;
	bool ImportArraysPSM( MdxArrays *arrays, MDXChunk *chunk ) ;
	bool ImportVertexPSM( MdxVertex &v, int format, void *data ) ;
	bool ImportFCurvePSM( MdxFCurve *fcurve, MDXChunk *chunk ) ;

private:
	int m_format_style ;
	int m_format_option ;

	vec4 m_vertex_offset ;
	vec4 m_vertex_scale ;
	vec4 m_tcoord_offset ;
	vec4 m_tcoord_scale ;

	int m_vertex_type ;
	int m_normal_type ;
	int m_vcolor_type ;
	int m_tcoord_type ;
	int m_weight_type ;
	int m_color_type ;
	int m_index_type ;
	int m_keyframe_type ;
} ;


} // namespace mdx

#endif
