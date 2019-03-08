#ifndef	XSI_FCURVE2_H_INCLUDE
#define	XSI_FCURVE2_H_INCLUDE

#include "SemanticLayer.h"	// XSI FTK
#include "ImportXSI.h"

//----------------------------------------------------------------------------
//  xsi_fcurve2
//----------------------------------------------------------------------------

class xsi_fcurve2 {
public:
	xsi_fcurve2() ;
	~xsi_fcurve2() ;

	void unload() ;
	void load( CdotXSITemplate *src, float val ) ;
	void save( MdxFCurve *dst ) ;

	void set_scale( float scale ) { m_scale = scale ; }
	float get_scale() const { return m_scale ; }

private:
	void load_fcurve( CdotXSITemplate *src, float val ) ;
	void save_fcurve( MdxFCurve *dst ) ;

private:
	vector<MdxFCurve *> m_src ;
	vector<float> m_val ;
	float m_scale ;
} ;


#endif
