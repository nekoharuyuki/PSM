#ifndef	EMBEDIMAGE_H_INCLUDE
#define	EMBEDIMAGE_H_INCLUDE

#include "ModelProc.h"

namespace mdx {

//----------------------------------------------------------------
//  EmbedImage
//----------------------------------------------------------------

class EmbedImage : public MdxModifierProc {
public:
	virtual const char *GetProcName() const { return "EmbedImage" ; }
	virtual const char *GetProcUsage() const { return "EmbedImage $target off/on" ; }
	virtual bool Modify( MdxBlock *block ) ;

private:
	void ImportFileImage( MdxFileName *filename ) ;
	string MakeConvertCommand( const string &name, string &name2 ) ;

private:
	vector<string> m_exts ;
	vector<string> m_exts2 ;
	string m_converter ;
	string m_extension ;
	string m_tmpfile ;
} ;


} // namespace mdx

#endif
