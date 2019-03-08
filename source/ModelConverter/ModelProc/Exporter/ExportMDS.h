#ifndef	EXPORTMDS_H_INCLUDE
#define	EXPORTMDS_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ExportMDS
//----------------------------------------------------------------

class ExportMDS : public MdxExporterProc {
public:
	virtual const char *GetProcName() const { return "ExportMDS" ; }
	virtual const char *GetProcUsage() const { return "ExportMDS $target $filename" ; }
	virtual bool Export( MdxBlock *block, const char *filename ) ;

private:
	bool ExportHeader() ;
	bool ExportBlock( MdxBlock *block ) ;
	bool ExportArgs( MdxBlock *block ) ;
	bool ExportData( MdxBlock *block ) ;
	bool WriteInt( int desc, int value ) ;
	bool WriteFloat( int desc, float value ) ;
	bool WriteString( int desc, const char *value ) ;
	bool WriteEnum( int desc, int value ) ;
	bool WriteRef( int desc, const char *value ) ;

private:
	txt_ofstream m_stream ;
	int m_root_state ;

	string m_line_feed ;
	int m_line_break ;
	string m_float_format ;
} ;


} // namespace mdx

#endif
