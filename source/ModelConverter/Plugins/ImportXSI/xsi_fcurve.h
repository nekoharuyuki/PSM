#ifndef	XSI_FCURVE_H_INCLUDE
#define	XSI_FCURVE_H_INCLUDE

#include "SemanticLayer.h"	// XSI FTK
#include "ImportXSI.h"

//----------------------------------------------------------------------------
//  xsi_fcurve
//----------------------------------------------------------------------------

class xsi_fcurve {
public:
	xsi_fcurve() ;
	~xsi_fcurve() ;

	void unload() ;
	void load( CSLFCurve *src, float val ) ;
	void save( MdxFCurve *dst ) ;

	void set_scale( float scale ) { m_scale = scale ; }
	float get_scale() const { return m_scale ; }

private:
	void fixup_format() ;
	void fixup_keyframes() ;

private:
	vector<CSLFCurve *> m_src ;
	vector<float> m_val ;
	float m_scale ;

	MdxFCurve *m_dst ;
	int m_format ;
	int m_dim_count ;
	int m_key_count ;
	vector<float> m_frames ;
} ;


#endif
