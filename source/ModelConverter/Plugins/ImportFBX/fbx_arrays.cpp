#include "fbx_loader.h"

#pragma warning( disable:4244 )
#pragma warning( disable:4996 )

//----------------------------------------------------------------
//  fbx_arrays
//----------------------------------------------------------------

fbx_arrays::fbx_arrays()
{
	m_default_vcolor = 0xffffffff ;

	m_src = 0 ;
	m_dst = 0 ;
}

fbx_arrays::~fbx_arrays()
{
	clear() ;
}

//----------------------------------------------------------------
//  load and save
//----------------------------------------------------------------

void fbx_arrays::clear()
{
	m_src = 0 ;
	m_shapes.clear() ;
	m_vertices.clear() ;
	m_weights.clear() ;
	m_dst = 0 ;
}

void fbx_arrays::load( KFbxMesh *mesh )
{
	m_src = mesh ;
}

void fbx_arrays::load( KFbxNurb *nurb )
{
	m_src = nurb ;
}

void fbx_arrays::load( KFbxPatch *patch )
{
	m_src = patch ;
}

void fbx_arrays::load( KFbxShape *shape )
{
	m_shapes.push_back( shape ) ;
}

#if ( AFTER_FBXSDK_200611 )
void fbx_arrays::load( KFbxNurbsSurface *nurb )
{
	m_src = nurb ;
}
#endif // AFTER_FBXSDK_200611

void fbx_arrays::save( MdxArrays *dst )
{
	if ( m_src == 0 || dst == 0 ) return ;
	m_dst = dst ;

	save_base_shape() ;
	save_morph_shapes() ;
}

void fbx_arrays::save_base_shape()
{
	int n_verts = m_vertices.size() ;
	m_dst->SetVertexCount( n_verts ) ;

	KFbxVector4 *points = m_src->GetControlPoints() ;
	KFbxLayer *layer = m_src->GetLayer( 0 ) ;
	KFbxLayerElementNormal *normals = ( layer == 0 ) ? 0 : layer->GetNormals() ;
	KFbxLayerElementVertexColor *vcolors = ( layer == 0 ) ? 0 : layer->GetVertexColors() ;
	KFbxLayerElementUV *tcoords = ( layer == 0 ) ? 0 : layer->GetUVs() ;

	if ( points != 0 ) {
		int count = m_src->GetControlPointsCount() ;
		m_dst->SetVertexPositionCount( 1 ) ;
		m_dst->SetVertexWeightCount( m_weights.size() ) ;
		for ( int i = 0 ; i < n_verts ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int pid = m_vertices[ i ].pid ;
			if ( pid < 0 || pid >= count ) continue ;
			// const KFbxVector4 &p = points[ pid ] ;
			const double *p = points[ pid ].mData ;
			v.SetPosition( vec4( p[ 0 ], p[ 1 ], p[ 2 ] ) ) ;

			for ( int j = 0 ; j < (int)m_weights.size() ; j ++ ) {
				if ( pid < (int)m_weights[ j ].size() ) {
					v.SetWeight( j, m_weights[ j ][ pid ] ) ;
				}
			}
		}
	}
	if ( normals != 0 ) {
		#if ( AFTER_FBXSDK_200901 == 0 )
		KArrayTemplate<KFbxVector4> &array = normals->GetDirectArray() ;
		#else // AFTER_FBXSDK_200901
		KFbxLayerElementArrayTemplate<KFbxVector4> &array = normals->GetDirectArray() ;
		#endif // AFTER_FBXSDK_200901
		int count = array.GetCount() ;
		m_dst->SetVertexNormalCount( 1 ) ;
		for ( int i = 0 ; i < n_verts ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int nid = m_vertices[ i ].nid ;
			if ( nid < 0 || nid >= count ) continue ;
			// const KFbxVector4 &n = array.GetAt( nid ) ;
			const double *n = array.GetAt( nid ).mData ;
			v.SetNormal( vec4( n[ 0 ], n[ 1 ], n[ 2 ] ) ) ;
		}
	}
	if ( vcolors != 0 ) {
		#if ( AFTER_FBXSDK_200901 == 0 )
		KArrayTemplate<KFbxColor> &array = vcolors->GetDirectArray() ;
		#else // AFTER_FBXSDK_200901
		KFbxLayerElementArrayTemplate<KFbxColor> &array = vcolors->GetDirectArray() ;
		#endif // AFTER_FBXSDK_200901
		int count = array.GetCount() ;
		m_dst->SetVertexColorCount( 1 ) ;
		for ( int i = 0 ; i < n_verts ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int cid = m_vertices[ i ].cid ;
			if ( cid < 0 || cid >= count ) continue ;
			const KFbxColor &c = array.GetAt( cid ) ;
			vec4 c2( c.mRed, c.mGreen, c.mBlue, c.mAlpha ) ;
			v.SetColor( ( c2 != 0.0f ) ? rgba8888( c2 ) : m_default_vcolor ) ;
		}
	}
	if ( tcoords != 0 ) {
		#if ( AFTER_FBXSDK_200901 == 0 )
		KArrayTemplate<KFbxVector2> &array = tcoords->GetDirectArray() ;
		#else // AFTER_FBXSDK_200901
		KFbxLayerElementArrayTemplate<KFbxVector2> &array = tcoords->GetDirectArray() ;
		#endif // AFTER_FBXSDK_200901
		int count = array.GetCount() ;
		m_dst->SetVertexTexCoordCount( 1 ) ;
		for ( int i = 0 ; i < n_verts ; i ++ ) {
			MdxVertex &v = m_dst->GetVertex( i ) ;
			int tid = m_vertices[ i ].tid ;
			if ( tid < 0 || tid >= count ) continue ;
			// const KFbxVector2 &t = array.GetAt( tid ) ;
			const double *t = array.GetAt( tid ).mData ;
			v.SetTexCoord( 0, vec4( t[ 0 ], 1.0 - t[ 1 ] ) ) ;
		}
	}
}

void fbx_arrays::save_morph_shapes()
{
	int i, j ;

	int n_verts = m_vertices.size() ;
	int n_shapes = m_shapes.size() ;
	if ( n_verts == 0 || n_shapes == 0 ) return ;

	int format = 0 ;
	for ( i = 0 ; i < n_shapes ; i ++ ) {
		KFbxShape *shape = m_shapes[ i ] ;
		if ( shape->GetControlPoints() != 0 ) format |= MDX_VF_POSITION ;
		#if ( AFTER_FBXSDK_200903 == 0 )
		if ( shape->GetNormals() != 0 ) format |= MDX_VF_NORMAL ;
		#else // AFTER_FBXSDK_200903
		KFbxLayerElementArrayTemplate<KFbxVector4> *normals = 0 ;
		if ( shape->GetNormals( &normals ) ) format |= MDX_VF_NORMAL ;
		#endif // AFTER_FBXSDK_200903
	}
	format &= m_dst->GetFormat() ;
	if ( format == 0 ) return ;

	m_dst->SetMorphCount( n_shapes + 1 ) ;
	m_dst->SetMorphFormat( format ) ;

	for ( i = 0 ; i < n_shapes ; i ++ ) {
		MdxArrays *morph = m_dst->GetMorph( i + 1 ) ;
		KFbxShape *shape = m_shapes[ i ] ;
		KFbxVector4 *points = shape->GetControlPoints() ;
		int count = shape->GetControlPointsCount() ;
		if ( points != 0 ) {
			for ( j = 0 ; j < n_verts ; j ++ ) {
				MdxVertex &v = morph->GetVertex( j ) ;
				int pid = m_vertices[ j ].pid ;
				if ( pid < 0 || pid >= count ) continue ;
				KFbxVector4 &p = points[ pid ] ;
				v.SetPosition( vec4( p[ 0 ], p[ 1 ], p[ 2 ] ) ) ;
			}
		}

		#if ( AFTER_FBXSDK_200901 == 0 )
		KFbxVector4 *normals = shape->GetNormals() ;
		#else // AFTER_FBXSDK_200901
		KFbxLayerElementArrayTemplate<KFbxVector4> *normals = 0 ;
		shape->GetNormals( &normals ) ;
		#endif // AFTER_FBXSDK_200901

		if ( normals != 0 ) {
			for ( j = 0 ; j < n_verts ; j ++ ) {
				MdxVertex &v = morph->GetVertex( j ) ;
				int nid = m_vertices[ j ].nid ;
				#if ( AFTER_FBXSDK_200901 == 0 )
				if ( nid < 0 || nid >= count ) continue ;
				KFbxVector4 &n = normals[ nid ] ;
				#else // AFTER_FBXSDK_200901
				if ( nid < 0 || nid >= normals->GetCount() ) continue ;
				KFbxVector4 &n = normals->GetAt( nid ) ;
				#endif // AFTER_FBXSDK_200901
				v.SetNormal( vec4( n[ 0 ], n[ 1 ], n[ 2 ] ) ) ;
			}
		}
	}
}

//----------------------------------------------------------------
//  index and weight
//----------------------------------------------------------------

int fbx_arrays::set_index( int pid, int nid, int cid, int tid, bool share )
{
	vertex tmp ;
	tmp.pid = pid ;
	tmp.nid = nid ;
	tmp.cid = cid ;
	tmp.tid = tid ;

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

void fbx_arrays::set_weight( int pid, int idx, float w )
{
	if ( pid < 0 || idx < 0 ) return ;
	if ( idx >= (int)m_weights.size() ) m_weights.resize( idx + 1 ) ;
	if ( pid >= (int)m_weights[ idx ].size() ) m_weights[ idx ].resize( pid + 1 ) ;
	m_weights[ idx ][ pid ] = w ;
}

bool fbx_arrays::match_vertex( const vertex &v1, const vertex &v2 )
{
	if ( v1.pid != v2.pid && v1.pid >= 0 && v2.pid >= 0 ) return false ;
	if ( v1.nid != v2.nid && v1.nid >= 0 && v2.nid >= 0 ) return false ;
	if ( v1.cid != v2.cid && v1.cid >= 0 && v2.cid >= 0 ) return false ;
	if ( v1.tid != v2.tid && v1.tid >= 0 && v2.tid >= 0 ) return false ;
	return true ;
}

void fbx_arrays::define_vertex( vertex &v1, const vertex &v2 )
{
	if ( v2.pid >= 0 ) v1.pid = v2.pid ;
	if ( v2.nid >= 0 ) v1.nid = v2.nid ;
	if ( v2.cid >= 0 ) v1.cid = v2.cid ;
	if ( v2.tid >= 0 ) v1.tid = v2.tid ;
}
