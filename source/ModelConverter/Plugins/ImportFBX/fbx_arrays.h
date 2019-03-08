#ifndef	FBX_ARRAYS_H_INCLUDE
#define	FBX_ARRAYS_H_INCLUDE

//----------------------------------------------------------------------------
//  fbx_arrays
//----------------------------------------------------------------------------

class fbx_arrays {
public:
	fbx_arrays() ;
	~fbx_arrays() ;

	void set_default_vcolor( rgba8888 c ) { m_default_vcolor = c ; }

	void clear() ;
	void load( KFbxMesh *src ) ;
	void load( KFbxNurb *src ) ;
	void load( KFbxPatch *src ) ;
	void load( KFbxShape *shape ) ;
	#if ( AFTER_FBXSDK_200611 )
	void load( KFbxNurbsSurface *src ) ;
	#endif // AFTER_FBXSDK_200611
	void save( MdxArrays *dst ) ;

	int set_index( int pid, int nid, int cid, int tid, bool share = true ) ;
	void set_weight( int pid, int idx, float w ) ;

private:
	struct vertex {
		int pid, nid, cid, tid ;
	} ;

private:
	void save_base_shape() ;
	void save_morph_shapes() ;

	bool match_vertex( const vertex &v1, const vertex &v2 ) ;
	void define_vertex( vertex &v1, const vertex &v2 ) ;

private:
	rgba8888 m_default_vcolor ;

	KFbxGeometry *m_src ;
	vector<KFbxShape *> m_shapes ;
	vector<vertex> m_vertices ;
	vector<vector<float> > m_weights ;
	MdxArrays *m_dst ;
} ;


#endif
