#ifndef	FBX_FCURVE_H_INCLUDE
#define	FBX_FCURVE_H_INCLUDE

//----------------------------------------------------------------------------
//  fbx_fcurve
//----------------------------------------------------------------------------

class fbx_fcurve {
public:
	fbx_fcurve() ;
	~fbx_fcurve() ;

	void clear() ;
	void load( KFCurve *src, float val ) ;
	void save( MdxFCurve *dst, KTime start, KTime end, KTime step ) ;

	bool is_active( int mode, KTime start, KTime end, KTime step ) ;
	int get_extrap( KFCurve *fcurve ) ;

private:
	vector<KFCurve *> m_src ;
	vector<float> m_val ;
	MdxFCurve *m_dst ;
} ;


#endif
