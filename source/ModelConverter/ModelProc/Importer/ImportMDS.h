#ifndef	IMPORTMDS_H_INCLUDE
#define	IMPORTMDS_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  ImportMDS
//----------------------------------------------------------------

class ImportMDS : public MdxImporterProc {
public:
	virtual const char *GetProcName() const { return "ImportMDS" ; }
	virtual const char *GetProcUsage() const { return "ImportMDS $target $filename" ; }
	virtual bool Import( MdxBlock *&block, const void *buf, int size ) ;
	virtual bool Import( MdxBlock *&block, const char *filename ) ;

private:
	bool ImportHeader() ;
	bool ImportBlock( MdxBlock *block ) ;
	bool ParseLine( MdxBlock *block, const vector<string> &line ) ;
	bool SkipBlock() ;
	bool ImportArgs( MdxBlock *block, const vector<string> &line, int start = 0 ) ;
	bool ImportData( MdxBlock *block, const vector<string> &line ) ;
	bool CheckWordDesc( string &word, int &desc ) ;
	bool CheckDirective( MdxBlock *block ) ;
	void Warning( const string &mesg, const string &arg ) ;

private:
	txt_ifstream m_stream ;
	int m_data_index ;

	bool m_source_define ;
	bool m_source_option ;
} ;


} // namespace mdx

#endif
