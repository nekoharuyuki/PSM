#ifndef	XSI_ARRAYS_H_INCLUDE
#define	XSI_ARRAYS_H_INCLUDE

#include "SemanticLayer.h"	// XSI FTK
#include "ImportXSI.h"

//----------------------------------------------------------------------------
//  xsi_arrays
//----------------------------------------------------------------------------

class xsi_arrays {
public:
	xsi_arrays() ;
	~xsi_arrays() ;

	void unload() ;
	void load( CSLBaseShape *src ) ;
	void load( CSLNurbsSurface *src ) ;
	#if ( AFTER_CROSSWALK_2_6 )
	void load( CSLXSIShape *src ) ;			//  dotXSI 5.0 and beyond
	#endif // AFTER_CROSSWALK_2_6
	void save( MdxArrays *dst ) ;

	int set_index( SI_Int *vids, SI_Int *nids, SI_Int *cids, SI_Int *tids, int num, bool share = true ) ;
	int set_index( int vid, int nid, int cid, int tid, bool share = true ) ;
	void set_weight( int vid, int idx, float w ) ;

	int set_tspace( const string &name ) ;		//  dotXSI 3.5 and beyond
	int get_attr_type( const string &name ) ;	//  dotXSI 5.0 and beyond

private:
	struct vertex {
		int vid, nid, cid, tid ;
		int tspace ;
	} ;

private:
	void save_base_shape() ;
	void save_base_morphs() ;
	void save_nurbs_surface() ;
	void save_xsi_shape() ;
	void save_xsi_morphs() ;
	bool match_vertex( const vertex &v1, const vertex &v2 ) ;
	void define_vertex( vertex &v1, const vertex &v2 ) ;

private:
	vector<CSLTemplate *> m_src ;
	vector<vertex> m_vertices ;
	vector<vector<float> > m_weights ;
	MdxArrays *m_dst ;
	int m_tspace ;
} ;


#endif
