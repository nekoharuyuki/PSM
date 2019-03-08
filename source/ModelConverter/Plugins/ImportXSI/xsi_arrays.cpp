#include "xsi_loader.h"

//----------------------------------------------------------------
//  xsi_arrays
//----------------------------------------------------------------

xsi_arrays::xsi_arrays()
{
	m_dst = 0 ;
	m_tspace = 0 ;
}

xsi_arrays::~xsi_arrays()
{
	unload() ;
}

//----------------------------------------------------------------
//  load and save
//----------------------------------------------------------------

void xsi_arrays::unload()
{
	m_src.clear() ;
	m_vertices.clear() ;
	m_weights.clear() ;
	m_dst = 0 ;
	m_tspace = 0 ;
}

void xsi_arrays::load( CSLBaseShape *src )
{
	if ( m_src.empty() ) {
		if ( src->ShapeType() != CSLBaseShape::SI_ORDERED ) return ;
	} else {
		if ( src->ShapeType() != CSLBaseShape::SI_INDEXED ) return ;
	}
	m_src.push_back( src ) ;
}

void xsi_arrays::load( CSLNurbsSurface *src )
{
	if ( !m_src.empty() ) return ;
	m_src.push_back( src ) ;
}

#if ( AFTER_CROSSWALK_2_6 )
void xsi_arrays::load( CSLXSIShape *src )
{
	if ( m_src.empty() ) {
		if ( src->GetShapeType() != CSLXSIShape::XSI_ORDERED ) return ;
	} else {
		if ( src->GetShapeType() != CSLXSIShape::XSI_INDEXED ) return ;
	}
	m_src.push_back( src ) ;
}
#endif // AFTER_CROSSWALK_2_6

void xsi_arrays::save( MdxArrays *dst )
{
	if ( m_src.empty() || dst == 0 ) return ;

	m_dst = dst ;
	switch ( m_src[ 0 ]->Type() ) {
	    case CSLTemplate::SI_BASE_SHAPE :
	    case CSLTemplate::SI_SHAPE :
	    case CSLTemplate::SI_SHAPE35 :
		save_base_shape() ;
		save_base_morphs() ;
		break ;
	    case CSLTemplate::SI_NURBS_SURFACE :
		save_nurbs_surface() ;
		save_base_morphs() ;
		break ;
	    #if ( AFTER_CROSSWALK_2_6 )
	    case CSLTemplate::XSI_SHAPE :
		save_xsi_shape() ;
		save_xsi_morphs() ;
		break ;
	    #endif // AFTER_CROSSWALK_2_6
	    default :
		return ;
	}
}

//----------------------------------------------------------------
//  save shape
//----------------------------------------------------------------

void xsi_arrays::save_base_shape()
{
	int count = m_vertices.size() ;
	m_dst->SetVertexCount( count ) ;

	CSLBaseShape *shape = (CSLBaseShape *)m_src[ 0 ] ;
	CSIBCVector3D *vp = shape->GetVertexListPtr() ;
	if ( vp != 0 ) {
		m_dst->SetVertexPositionCount( 1 ) ;
		m_dst->SetVertexWeightCount( m_weights.size() ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int vid = m_vertices[ i ].vid ;
			if ( vid < 0 ) continue ;
			CSIBCVector3D &p = vp[ vid ] ;
			v.SetPosition( vec4( p.GetX(), p.GetY(), p.GetZ() ) ) ;

			for ( int j = 0 ; j < (int)m_weights.size() ; j ++ ) {
				if ( vid < (int)m_weights[ j ].size() ) {
					v.SetWeight( j, m_weights[ j ][ vid ] ) ;
				}
			}
		}
	}
	CSIBCVector3D *np = shape->GetNormalListPtr() ;
	if ( np != 0 ) {
		m_dst->SetVertexNormalCount( 1 ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int nid = m_vertices[ i ].nid ;
			if ( nid < 0 ) continue ;
			CSIBCVector3D &n = np[ nid ] ;
			v.SetNormal( vec4( n.GetX(), n.GetY(), n.GetZ() ) ) ;
		}
	}
	CSIBCColorf *cp = shape->GetColorListPtr() ;
	if ( cp != 0 ) {
		m_dst->SetVertexColorCount( 1 ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int cid = m_vertices[ i ].cid ;
			if ( cid < 0 ) continue ;
			CSIBCColorf &c = cp[ cid ] ;
			SI_Float r, g, b, a ;
			c.Get( &r, &g, &b, &a ) ;
			v.SetColor( rgba8888( vec4( r, g, b, a ) ) ) ;
		}
	}
	vector<CSIBCVector2D *> tps ;
	if ( shape->Type() == CSLTemplate::SI_SHAPE ) {
		CSLShape *shape30 = (CSLShape *)shape ;
		CSIBCVector2D *tp = shape30->GetUVCoordListPtr() ;
		if ( tp != 0 ) tps.push_back( tp ) ;
	} else if ( shape->Type() == CSLTemplate::SI_SHAPE35 ) {
		CSLShape_35 *shape35 = (CSLShape_35 *)shape ;
		CSLUVCoordArray **uvs = shape35->UVCoordArrays() ;
		for ( int i = 0 ; i < shape35->GetUVCoordArrayCount() ; i ++ ) {
			CSIBCVector2D *tp = uvs[ i ]->GetUVCoordListPtr() ;
			if ( tp != 0 ) tps.push_back( tp ) ;
		}
	}
	int tspace_count = tps.size() ;
	if ( tspace_count > 0 ) {
		m_dst->SetVertexTexCoordCount( 1 ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int tid = m_vertices[ i ].tid ;
			if ( tid < 0 ) continue ;
			int tspace = m_vertices[ i ].tspace ;
			if ( tspace > tspace_count ) tspace = 0 ;
			CSIBCVector2D *tp = tps[ tspace ] ;
			CSIBCVector2D &t = tp[ tid ] ;
			v.SetTexCoord( 0, vec4( t.GetX(), 1.0f - t.GetY() ) ) ;
		}
	}
}

void xsi_arrays::save_base_morphs()
{
	int i, j ;

	if ( m_src.size() == 1 || m_dst->GetMorphCount() == 0 ) return ;

	int format = 0 ;
	for ( i = 1 ; i < (int)m_src.size() ; i ++ ) {
		CSLBaseShape *shape2 = (CSLBaseShape *)m_src[ i ] ;
		if ( shape2->GetVertexCount() > 0 ) format |= MDX_VF_POSITION ;
		if ( shape2->GetNormalCount() > 0 ) format |= MDX_VF_NORMAL ;
		if ( shape2->GetColorCount() > 0 ) format |= MDX_VF_COLOR ;
	}
	format &= m_dst->GetFormat() ;
	if ( format == 0 ) return ;

	int count = m_src.size() - 1 ;
	m_dst->SetMorphCount( count ) ;
	m_dst->SetMorphFormat( format ) ;
	for ( i = 0 ; i < count ; i ++ ) {
		MdxArrays *morph = m_dst->GetMorph( i ) ;
 		CSLBaseShape *shape = (CSLBaseShape *)m_src[ i + 1 ] ;
		SLIndexedVector3D *vp = shape->GetIndexedVertexListPtr() ;
		for ( j = 0 ; j < shape->GetVertexCount() ; j ++ ) {
			int vid = (int)( vp[ j ].m_fIndex ) ;
			CSIBCVector3D &s = vp[ j ].m_Vector3D ;
			vec4 p( s.GetX(), s.GetY(), s.GetZ() ) ;
			for ( int k = 0 ; k < (int)m_vertices.size() ; k ++ ) {
				if ( m_vertices[ k ].vid != vid ) continue ;
				morph->GetVertex( k ).SetPosition( p ) ;
			}
		}
		SLIndexedVector3D *np = shape->GetIndexedNormalListPtr() ;
		for ( j = 0 ; j < shape->GetNormalCount() ; j ++ ) {
			int nid = (int)( np[ j ].m_fIndex ) ;
			CSIBCVector3D &s = np[ j ].m_Vector3D ;
			vec4 n( s.GetX(), s.GetY(), s.GetZ() ) ;
			for ( int k = 0 ; k < (int)m_vertices.size() ; k ++ ) {
				if ( m_vertices[ k ].nid != nid ) continue ;
				morph->GetVertex( k ).SetNormal( n ) ;
			}
		}
		SLIndexedColor *cp = shape->GetIndexedColorListPtr() ;
		for ( j = 0 ; j < shape->GetColorCount() ; j ++ ) {
			int cid = (int)( cp[ j ].m_fIndex ) ;
			CSIBCColorf &s = cp[ j ].m_Color ;
			SI_Float r, g, b, a ;
			s.Get( &r, &g, &b, &a ) ;
			rgba8888 c = vec4( r, g, b, a ) ;
			for ( int k = 0 ; k < (int)m_vertices.size() ; k ++ ) {
				if ( m_vertices[ k ].cid != cid ) continue ;
				morph->GetVertex( k ).SetColor( c ) ;
			}
		}
	}
}

void xsi_arrays::save_nurbs_surface()
{
	int count = m_vertices.size() ;
	m_dst->SetVertexCount( count ) ;

	CSLNurbsSurface *surf = (CSLNurbsSurface *)m_src[ 0 ] ;
	CSIBCVector4D *vp = surf->GetControlPointListPtr() ;
	if ( vp != 0 ) {
		m_dst->SetVertexPositionCount( 1 ) ;
		m_dst->SetVertexWeightCount( m_weights.size() ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;

			int vid = m_vertices[ i ].vid ;
			if ( vid < 0 ) continue ;
			CSIBCVector4D &p = vp[ vid ] ;
			v.SetPosition( vec4( p.m_fX, p.m_fY, p.m_fZ ) ) ;

			for ( int j = 0 ; j < (int)m_weights.size() ; j ++ ) {
				if ( vid < (int)m_weights[ j ].size() ) {
					v.SetWeight( j, m_weights[ j ][ vid ] ) ;
				}
			}
		}
	}
}

//----------------------------------------------------------------
//  save shape ( dotXSI 5.0 and beyond )
//----------------------------------------------------------------

#if ( AFTER_CROSSWALK_2_6 )
void xsi_arrays::save_xsi_shape()
{
	int count = m_vertices.size() ;
	m_dst->SetVertexCount( count ) ;

	CSLXSIShape *shape = (CSLXSIShape *)m_src[ 0 ] ;
	CSLXSISubComponentAttributeList *vp = shape->GetVertexPositionList() ;
	if ( vp != 0 ) {
		m_dst->SetVertexPositionCount( 1 ) ;
		m_dst->SetVertexWeightCount( m_weights.size() ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int vid = m_vertices[ i ].vid ;
			if ( vid < 0 ) continue ;
			SI_Float *fp = vp->GetAttributeArray()->ArrayPtr() + vid * 3 ;
			v.SetPosition( vec4( fp[ 0 ], fp[ 1 ], fp[ 2 ] ) ) ;
			for ( int j = 0 ; j < (int)m_weights.size() ; j ++ ) {
				if ( vid < (int)m_weights[ j ].size() ) {
					v.SetWeight( j, m_weights[ j ][ vid ] ) ;
				}
			}
		}
	}
	CSLXSISubComponentAttributeList *np = shape->GetFirstNormalList() ;
	if ( np != 0 ) {
		m_dst->SetVertexNormalCount( 1 ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int nid = m_vertices[ i ].nid ;
			if ( nid < 0 ) continue ;
			SI_Float *fp = np->GetAttributeArray()->ArrayPtr() + nid * 3 ;
			v.SetNormal( vec4( fp[ 0 ], fp[ 1 ], fp[ 2 ] ) ) ;
		}
	}
	CSLXSISubComponentAttributeList *cp = shape->GetFirstColorList() ;
	if ( cp != 0 ) {
		m_dst->SetVertexColorCount( 1 ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int cid = m_vertices[ i ].cid ;
			if ( cid < 0 ) continue ;
			SI_Float *fp = cp->GetAttributeArray()->ArrayPtr() + cid * 4 ;
			v.SetColor( rgba8888( vec4( fp[ 0 ], fp[ 1 ], fp[ 2 ], fp[ 3 ] ) ) ) ;
		}
	}
	CSLXSISubComponentAttributeList *tp = shape->GetFirstTexCoordList() ;
	if ( tp != 0 ) {
		vector<SI_Float *> fps ;
		while ( tp != 0 ) {
			fps.push_back( tp->GetAttributeArray()->ArrayPtr() ) ;
			tp = shape->GetNextTexCoordList() ;
		}
		m_dst->SetVertexTexCoordCount( 1 ) ;
		for ( int i = 0 ; i < count ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int tid = m_vertices[ i ].tid ;
			if ( tid < 0 ) continue ;
			int tspace = m_vertices[ i ].tspace ;
			if ( tspace >= (int)fps.size() ) tspace = 0 ;
			SI_Float *fp = fps[ tspace ] + tid * 2 ;
			v.SetTexCoord( 0, vec4( fp[ 0 ], 1.0f - fp[ 1 ] ) ) ;
		}
	}
}

void xsi_arrays::save_xsi_morphs()
{
	;
}
#endif // AFTER_CROSSWALK_2_6

//----------------------------------------------------------------
//  index and weight
//----------------------------------------------------------------

int xsi_arrays::set_index( SI_Int *vids, SI_Int *nids, SI_Int *cids, SI_Int *tids, int num, bool share )
{
	int vid = ( vids == 0 ) ? -1 : vids[ num ] ;
	int nid = ( nids == 0 ) ? -1 : nids[ num ] ;
	int cid = ( cids == 0 ) ? -1 : cids[ num ] ;
	int tid = ( tids == 0 ) ? -1 : tids[ num ] ;
	return set_index( vid, nid, cid, tid, share ) ;
}

int xsi_arrays::set_index( int vid, int nid, int cid, int tid, bool share )
{
	vertex tmp ;
	tmp.vid = vid ;
	tmp.nid = nid ;
	tmp.cid = cid ;
	tmp.tid = tid ;
	tmp.tspace = m_tspace ;

	int count = m_vertices.size() ;
	if ( share ) {
		for ( int i = 0 ; i < count ; i ++ ) {
			if ( match_vertex( m_vertices[ i ], tmp ) ) {
				define_vertex( m_vertices[ i ], tmp ) ;
				return i ;
			}
		}
	}
	m_vertices.push_back( tmp ) ;
	return count ;
}

void xsi_arrays::set_weight( int vid, int idx, float w )
{
	if ( vid < 0 || idx < 0 ) return ;
	if ( idx >= (int)m_weights.size() ) m_weights.resize( idx + 1 ) ;
	if ( vid >= (int)m_weights[ idx ].size() ) m_weights[ idx ].resize( vid + 1 ) ;
	m_weights[ idx ][ vid ] = w ;
}

bool xsi_arrays::match_vertex( const vertex &v1, const vertex &v2 )
{
	if ( v1.vid != v2.vid && v1.vid >= 0 && v2.vid >= 0 ) return false ;
	if ( v1.nid != v2.nid && v1.nid >= 0 && v2.nid >= 0 ) return false ;
	if ( v1.cid != v2.cid && v1.cid >= 0 && v2.cid >= 0 ) return false ;
	if ( v1.tid != v2.tid && v1.tid >= 0 && v2.tid >= 0 ) return false ;
	if ( v1.tspace != v2.tspace ) return false ;
	return true ;
}

void xsi_arrays::define_vertex( vertex &v1, const vertex &v2 )
{
	if ( v2.vid >= 0 ) v1.vid = v2.vid ;
	if ( v2.nid >= 0 ) v1.nid = v2.nid ;
	if ( v2.cid >= 0 ) v1.cid = v2.cid ;
	if ( v2.tid >= 0 ) v1.tid = v2.tid ;
	v1.tspace = v2.tspace ;
}

//----------------------------------------------------------------
//  tspace ( dotXSI 3.5 and beyond )
//----------------------------------------------------------------

int xsi_arrays::set_tspace( const string &name )
{
	if ( m_src.empty() || m_src[ 0 ] == 0 ) return ( m_tspace = 0 ) ;

	if ( m_src[ 0 ]->Type() == CSLTemplate::SI_SHAPE35 ) {
		CSLShape_35 *shape = (CSLShape_35 *)m_src[ 0 ] ;
		CSLUVCoordArray **uvs = shape->UVCoordArrays() ;
		for ( int i = 0 ; i < shape->GetUVCoordArrayCount() ; i ++ ) {
			if ( name == uvs[ i ]->GetTextureProjection() ) return ( m_tspace = i ) ;
		}
	}
	#if ( AFTER_CROSSWALK_2_6 )
	if ( m_src[ 0 ]->Type() == CSLTemplate::XSI_SHAPE ) {
		CSLXSIShape *shape = (CSLXSIShape *)m_src[ 0 ] ;
		CSLXSISubComponentAttributeList *tp = shape->GetFirstTexCoordList() ;
		while ( tp != 0 ) {
			if ( name == tp->GetName() ) return m_tspace ;
			tp = shape->GetNextTexCoordList() ;
			m_tspace ++ ;
		}
	}
	#endif // AFTER_CROSSWALK_2_6
	return ( m_tspace = 0 ) ;
}

//----------------------------------------------------------------
//  attribute type ( dotXSI 5.0 and beyond )
//----------------------------------------------------------------

#if ( AFTER_CROSSWALK_2_6 )
int xsi_arrays::get_attr_type( const string &name )
{
	if ( m_src.empty() || m_src[ 0 ] == 0 ) return -1 ;

	if ( m_src[ 0 ]->Type() == CSLTemplate::XSI_SHAPE ) {
		CSLXSIShape *shape = (CSLXSIShape *)m_src[ 0 ] ;
		CSIBCArray<CSLXSISubComponentAttributeList*> *attrs = shape->AttributeLists() ;
		for ( int i = 0 ; i < attrs->GetSize() ; i ++ ) {
			CSLXSISubComponentAttributeList *attr = (*attrs)[ i ] ;
			if ( name == attr->GetName() ) {
				const SI_Char *semantic = attr->GetSemantic() ;
				if ( strcmp( semantic, "POSITION" ) == 0 ) return 0 ;
				if ( strcmp( semantic, "NORMAL" ) == 0 ) return 1 ;
				if ( strcmp( semantic, "COLOR" ) == 0 ) return 2 ;
				if ( strcmp( semantic, "TEXCOORD" ) == 0 ) return 3 ;
				return -1 ;
			}
		}
	}
	return -1 ;
}
#endif // AFTER_CROSSWALK_2_6
