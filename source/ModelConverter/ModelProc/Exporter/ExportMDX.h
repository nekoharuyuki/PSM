#ifndef	EXPORTMDX_H_INCLUDE
#define	EXPORTMDX_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ExportMDX
//----------------------------------------------------------------

class ExportMDX : public MdxExporterProc {
public:
	virtual const char *GetProcName() const { return "ExportMDX" ; }
	virtual const char *GetProcUsage() const { return "ExportMDX $target $filename" ; }
	virtual bool Export( MdxBlock *block, const char *filename ) ;

private:
	bool ExportHeader( MdxBlock *block ) ;
	bool ExportBlock( MdxBlock *block ) ;
	bool ExportSpecific( MdxBlock *block ) ;
	bool ExportGeneric( MdxBlock *block ) ;
	bool ExportArgs( MdxBlock *block ) ;
	bool ExportData( MdxBlock *block ) ;
	bool ExportChildren( MdxBlock *block ) ;

	bool PreProcess( MdxBlock *&block ) ;
	bool PostProcess( MdxBlock *&block ) ;
	bool CheckOptions( MdxBlock *&block ) ;
	void PrintStat() ;
	void CheckLimit() ;

	bool ExportModelPSM( MdxModel *model ) ;
	bool ExportArraysPSM( MdxArrays *arrays ) ;
	bool ExportVertexPSM( MdxArrays *arrays, int index ) ;
	int GetArraysFormatPSM( MdxArrays *arrays ) ;
	int GetArraysStridePSM( MdxArrays *arrays ) ;
	bool ExportFCurvePSM( MdxFCurve *fcurve ) ;
	bool ExportFileNamePSM( MdxFileName *cmd ) ;

private:
	bin_ofstream m_stream ;
	const void *m_args_buf ;
	const void *m_data_buf ;
	int m_args_size ;
	int m_data_size ;

	bool m_stat_mode ;
	bool m_debug_mode ;
	bool m_output_name ;
	bool m_output_anchor ;
	bool m_output_define ;
	bool m_output_option ;
	bool m_fixed_voffs ;
	bool m_fixed_toffs ;

	int m_format_style ;
	int m_format_option ;
	int m_format_color ;
	int m_format_index ;
	int m_format_keyframe ;

	int m_vertex_type ;
	int m_normal_type ;
	int m_vcolor_type ;
	int m_tcoord_type ;
	int m_weight_type ;

	vec4 m_vertex_offset ;
	vec4 m_vertex_scale ;
	vec4 m_tcoord_offset ;
	vec4 m_tcoord_scale ;

	map<int,int> m_output_filters ;
	map<int,int> m_output_anchors ;
	map<int,int> m_output_limits ;
	map<int,int> m_output_counts ;
	map<int,int> m_output_sizes ;
} ;


} // namespace mdx

#endif
