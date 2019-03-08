#include "ModelFormat.h"

namespace mdx {

static float find_root( float y0, float y1, float y2, float y3, float y ) ;

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

class MdxFCurve::impl {
public:
	int m_format ;
	int m_extrap ;
	int m_dim_count ;
	int m_key_count ;
	vector<MdxKeyFrame> m_keys ;
} ;

class MdxKeyFrame::impl {
public:
	int m_format ;
	int m_dim_count ;
	float m_frame ;
	vector<float> m_values ;
	vector<float> m_in_dys ;
	vector<float> m_out_dys ;
	vector<float> m_in_dxs ;
	vector<float> m_out_dxs ;
} ;

//----------------------------------------------------------------
//  FCurve block
//----------------------------------------------------------------

MdxFCurve::MdxFCurve( const char *name )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_FCURVE_FORMAT ) ;
	SetArgsDesc( 1, MDX_WORD_ENUM | MDX_SCOPE_FCURVE_EXTRAP ) ;
	SetArgsDesc( 2, MDX_WORD_INT ) ;
	SetArgsDesc( 3, MDX_WORD_INT ) ;

	m_impl = new impl ;
	m_impl->m_format = MDX_FCURVE_LINEAR ;
	m_impl->m_extrap = MDX_FCURVE_HOLD ;
	m_impl->m_dim_count = 0 ;
	m_impl->m_key_count = 0 ;

	SetName( name ) ;
}

MdxFCurve::~MdxFCurve()
{
	delete m_impl ;
}

MdxFCurve::MdxFCurve( const MdxFCurve &src )
{
	m_impl = new impl ;
	m_impl->m_format = MDX_FCURVE_LINEAR ;
	m_impl->m_extrap = MDX_FCURVE_HOLD ;
	m_impl->m_dim_count = 0 ;
	m_impl->m_key_count = 0 ;
	Copy( &src ) ;
}

MdxFCurve &MdxFCurve::operator=( const MdxFCurve &src )
{
	Copy( &src ) ;
	return *this ;
}

//----------------------------------------------------------------
//  chunk methods
//----------------------------------------------------------------

MdxChunk *MdxFCurve::Copy( const MdxChunk *src )
{
	const MdxFCurve *src2 = dynamic_cast<const MdxFCurve *>( src ) ;
	if ( src2 == this ) return this ;
	if ( src2 == 0 ) return 0 ;

	MdxBlock::Copy( src2 ) ;
	*m_impl = *( src2->m_impl ) ;
	return this ;
}

bool MdxFCurve::Equals( const MdxChunk *src ) const
{
	const MdxFCurve *src2 = dynamic_cast<const MdxFCurve *>( src ) ;
	if ( src2 == this ) return true ;
	if ( src2 == 0 ) return false ;

	if ( !MdxBlock::Equals( src2 ) ) return false ;
	if ( m_impl->m_format != src2->m_impl->m_format ) return false ;
	if ( m_impl->m_extrap != src2->m_impl->m_extrap ) return false ;
	if ( m_impl->m_dim_count != src2->m_impl->m_dim_count ) return false ;
	if ( m_impl->m_keys != src2->m_impl->m_keys ) return false ;
	return true ;
}

bool MdxFCurve::UpdateArgs()
{
	SetArgsCount( 4 ) ;
	SetArgsInt( 0, GetFormat() ) ;
	SetArgsInt( 1, GetExtrap() ) ;
	SetArgsInt( 2, GetDimCount() ) ;
	SetArgsInt( 3, GetKeyFrameCount() ) ;
	return true ;
}

bool MdxFCurve::FlushArgs()
{
	SetFormat( GetArgsInt( 0 ) ) ;
	SetExtrap( GetArgsInt( 1 ) ) ;
	SetDimCount( GetArgsInt( 2 ) ) ;
	SetKeyFrameCount( GetArgsInt( 3 ) ) ;
	return true ;
}

bool MdxFCurve::UpdateData()
{
	int format = GetFormat() ;
	int n_dims = GetDimCount() ;
	int n_keys = GetKeyFrameCount() ;

	SetDataCount( 0 ) ;
	SetDataDesc( -1, MDX_WORD_FLOAT ) ;	// default desc

	int num = 0 ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		SetDataDesc( num, MDX_WORD_FLOAT | MDX_WORD_FMT_NEWLINE ) ;

		const MdxKeyFrame &key = GetKeyFrame( i ) ;
		SetDataFloat( num ++, key.GetFrame() ) ;
		for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetValue( j ) ) ;
		if ( format == MDX_FCURVE_HERMITE ) {
			for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetInDY( j ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetOutDY( j ) ) ;
		}
		if ( format == MDX_FCURVE_CUBIC ) {
			for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetInDY( j ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetOutDY( j ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetInDX( j ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) SetDataFloat( num ++, key.GetOutDX( j ) ) ;
		}
	}
	return true ;
}

bool MdxFCurve::FlushData()
{
	int format = GetFormat() ;
	int n_dims = GetDimCount() ;
	int n_keys = GetKeyFrameCount() ;

	int num = 0 ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &key = GetKeyFrame( i ) ;
		key.SetFrame( GetDataFloat( num ++ ) ) ;
		for ( int j = 0 ; j < n_dims ; j ++ ) key.SetValue( j, GetDataFloat( num ++ ) ) ;
		if ( format == MDX_FCURVE_HERMITE ) {
			for ( int j = 0 ; j < n_dims ; j ++ ) key.SetInDY( j, GetDataFloat( num ++ ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) key.SetOutDY( j, GetDataFloat( num ++ ) ) ;
		}
		if ( format == MDX_FCURVE_CUBIC ) {
			for ( int j = 0 ; j < n_dims ; j ++ ) key.SetInDY( j, GetDataFloat( num ++ ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) key.SetOutDY( j, GetDataFloat( num ++ ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) key.SetInDX( j, GetDataFloat( num ++ ) ) ;
			for ( int j = 0 ; j < n_dims ; j ++ ) key.SetOutDX( j, GetDataFloat( num ++ ) ) ;
		}
	}
	return true ;
}

//----------------------------------------------------------------
//  fcurve methods
//----------------------------------------------------------------

void MdxFCurve::SetFormat( int format, bool adjust_handles )
{
	if ( m_impl->m_format == format ) return ;
	int format1 = m_impl->m_format ;
	int format2 = format ;
	m_impl->m_format = format ;

	for ( int i = 0 ; i < m_impl->m_key_count ; i ++ ) {
		m_impl->m_keys[ i ].SetFormat( format ) ;
	}

	if ( !adjust_handles ) return ;

	if ( format1 < MDX_FCURVE_HERMITE && format2 >= MDX_FCURVE_HERMITE ) {
		for ( int i = 0 ; i < m_impl->m_key_count - 1 ; i ++ ) {
			MdxKeyFrame &key1 = m_impl->m_keys[ i ] ;
			MdxKeyFrame &key2 = m_impl->m_keys[ i + 1 ] ;
			for ( int j = 0 ; j < m_impl->m_dim_count ; j ++ ) {
				float dy = ( key2.GetValue( j ) - key1.GetValue( j ) ) / 3.0f ;
				key1.SetOutDY( j, dy ) ;
				key2.SetInDY( j, -dy ) ;
			}
		}
	}
	if ( format1 < MDX_FCURVE_CUBIC && format2 >= MDX_FCURVE_CUBIC ) {
		for ( int i = 0 ; i < m_impl->m_key_count - 1 ; i ++ ) {
			MdxKeyFrame &key1 = m_impl->m_keys[ i ] ;
			MdxKeyFrame &key2 = m_impl->m_keys[ i + 1 ] ;
			float dx = ( key2.GetFrame() - key1.GetFrame() ) / 3.0f ;
			for ( int j = 0 ; j < m_impl->m_dim_count ; j ++ ) {
				key1.SetOutDX( j, dx ) ;
				key2.SetInDX( j, -dx ) ;
			}
		}
	}
}

int MdxFCurve::GetFormat() const
{
	return m_impl->m_format ;
}

void MdxFCurve::SetInterp( int interp, bool adjust_handle )
{
	SetFormat( interp, adjust_handle ) ;
}

int MdxFCurve::GetInterp() const
{
	return m_impl->m_format ;
}

void MdxFCurve::SetExtrap( int extrap, bool adjust_handle )
{
	m_impl->m_extrap = extrap ;
}

int MdxFCurve::GetExtrap() const
{
	return m_impl->m_extrap ;
}

void MdxFCurve::SetDimCount( int count )
{
	// if ( count <= 0 ) count = 1 ;
	if ( m_impl->m_dim_count == count ) return ;
	m_impl->m_dim_count = count ;

	for ( int i = 0 ; i < m_impl->m_key_count ; i ++ ) {
		m_impl->m_keys[ i ].SetDimCount( count ) ;
	}
}

int MdxFCurve::GetDimCount() const
{
	return m_impl->m_dim_count ;
}

void MdxFCurve::SetKeyFrameCount( int count )
{
	// if ( count <= 0 ) count = 1 ;
	if ( m_impl->m_key_count == count ) return ;

	m_impl->m_keys.resize( count ) ;
	for ( int i = m_impl->m_key_count ; i < count ; i ++ ) {
		MdxKeyFrame &key = m_impl->m_keys[ i ] ;
		key.SetFormat( m_impl->m_format ) ;
		key.SetDimCount( m_impl->m_dim_count ) ;
	}
	m_impl->m_key_count = count ;
}

int MdxFCurve::GetKeyFrameCount() const
{
	return m_impl->m_key_count ;
}

void MdxFCurve::SetKeyFrame( int index, const MdxKeyFrame &key )
{
	// if ( index < 0 ) return ;
	if ( index >= m_impl->m_key_count ) SetKeyFrameCount( index + 1 ) ;
	m_impl->m_keys[ index ] = key ;
}

const MdxKeyFrame &MdxFCurve::GetKeyFrame( int index ) const
{
	static MdxKeyFrame tmp ;
	if ( index < 0 ) return tmp ;
	if ( index >= m_impl->m_key_count ) return tmp ;
	return m_impl->m_keys[ index ] ;
}

MdxKeyFrame &MdxFCurve::GetKeyFrame( int index )
{
	static MdxKeyFrame tmp ;
	if ( index < 0 ) return tmp ;
	if ( index >= m_impl->m_key_count ) SetKeyFrameCount( index + 1 ) ;
	return m_impl->m_keys[ index ] ;
}

void MdxFCurve::InsertKeyFrame( int index, const MdxKeyFrame &key, bool adjust_handles )
{
	int n_keys = m_impl->m_key_count ;
	if ( index < 0 || index > n_keys ) return ;

	m_impl->m_keys.insert( m_impl->m_keys.begin() + index, key ) ;
	m_impl->m_key_count = n_keys + 1 ;

	int format = m_impl->m_format ;
	if ( !adjust_handles || index == 0 || index == n_keys
	  || ( format != MDX_FCURVE_HERMITE && format != MDX_FCURVE_CUBIC ) ) {
		return ;
	}

	//  adjust handles

	MdxKeyFrame &key1 = m_impl->m_keys[ index - 1 ] ;
	MdxKeyFrame &key2 = m_impl->m_keys[ index ] ;
	MdxKeyFrame &key3 = m_impl->m_keys[ index + 1 ] ;
	float f1 = key1.GetFrame() ;
	float f2 = key2.GetFrame() ;
	float f3 = key3.GetFrame() ;
	float t = ( f3 == f1 ) ? 0.5f : ( f2 - f1 ) / ( f3 - f1 ) ;
	float s = 1.0f - t ;
	for ( int i = 0 ; i < m_impl->m_dim_count ; i ++ ) {
		if ( format == MDX_FCURVE_CUBIC ) {
			float dx1 = key1.GetOutDX( i ) ;
			float dx3 = key3.GetInDX( i ) ;
			t = find_root( f1, f1 + dx1, f3 + dx3, f3, f2 ) ;
			s = 1.0f - t ;
			key1.SetOutDX( i, key1.GetOutDX( i ) * t ) ;
			key2.SetInDX( i, key2.GetInDX( i ) * t ) ;
			key2.SetOutDX( i, key2.GetOutDX( i ) * s ) ;
			key3.SetInDX( i, key3.GetInDX( i ) * s ) ;
		}
		key1.SetOutDY( i, key1.GetOutDY( i ) * t ) ;
		key2.SetInDY( i, key2.GetInDY( i ) * t ) ;
		key2.SetOutDY( i, key2.GetOutDY( i ) * s ) ;
		key3.SetInDY( i, key3.GetInDY( i ) * s ) ;
	}
}

void MdxFCurve::DeleteKeyFrame( int index, bool adjust_handles )
{
	int n_keys = m_impl->m_key_count ;
	if ( index < 0 || index >= n_keys ) return ;

	int format = m_impl->m_format ;
	if ( !adjust_handles || index == 0 || index == n_keys - 1
	  || ( format != MDX_FCURVE_HERMITE && format != MDX_FCURVE_CUBIC ) ) {
		m_impl->m_keys.erase( m_impl->m_keys.begin() + index ) ;
		m_impl->m_key_count = n_keys - 1 ;
		return ;
	}

	//  adjust handles

	MdxKeyFrame &key1 = m_impl->m_keys[ index - 1 ] ;
	MdxKeyFrame &key2 = m_impl->m_keys[ index ] ;
	MdxKeyFrame &key3 = m_impl->m_keys[ index + 1 ] ;
	float f1 = key1.GetFrame() ;
	float f2 = key2.GetFrame() ;
	float f3 = key3.GetFrame() ;
	float r = ( f3 == f1 ) ? 0.5f : ( f2 - f1 ) / ( f3 - f1 ) ;
	float t = r ;
	float s = 1.0f - r ;
	for ( int i = 0 ; i < m_impl->m_dim_count ; i ++ ) {
		float v ;
		if ( format == MDX_FCURVE_CUBIC ) {
			float a = length2( vec4( key2.GetInDX( i ), key2.GetInDY( i ) ) ) ;
			float b = a + length2( vec4( key2.GetOutDX( i ), key2.GetOutDY( i ) ) ) ;
			t = ( b == 0.0f ) ? r : a / b ;
			s = 1.0f - t ;
			v = ( t == 0.0f ) ? key2.GetOutDX( i ) : key1.GetOutDX( i ) / t ;
			key1.SetOutDX( i, v ) ;
			v = ( s == 0.0f ) ? key2.GetInDX( i ) : key3.GetInDX( i ) / s ;
			key3.SetInDX( i, v ) ;
		}
		v = ( t == 0.0f ) ? key2.GetOutDY( i ) : key1.GetOutDY( i ) / t ;
		key1.SetOutDY( i, v ) ;
		v = ( s == 0.0f ) ? key2.GetInDY( i ) : key3.GetInDY( i ) / s ;
		key3.SetInDY( i, v ) ;
	}
	m_impl->m_keys.erase( m_impl->m_keys.begin() + index ) ;
	m_impl->m_key_count = n_keys - 1 ;
}

int MdxFCurve::IndexOfKeyFrame( float frame )
{
	int n_keys = m_impl->m_key_count ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		if ( m_impl->m_keys[ i ].GetFrame() == frame ) return i ;
	}
	return -1 ;
}

int MdxFCurve::IndexOfInterval( float frame )
{
	int n_keys = m_impl->m_key_count ;
	for ( int i = 0 ; i < n_keys ; i ++ ) {
		if ( m_impl->m_keys[ i ].GetFrame() > frame ) return i - 1 ;
	}
	return n_keys - 1 ;
}

int MdxFCurve::Eval( float frame, MdxKeyFrame &key, bool eval_handles )
{
	int n_keys = m_impl->m_key_count ;
	if ( n_keys == 0 ) {
		key.SetFrame( frame ) ;
		return -1 ;
	}
	int index = IndexOfInterval( frame ) ;
	if ( index < 0 ) {
		key = m_impl->m_keys[ 0 ] ;
		key.SetFrame( frame ) ;
		return index ;
	}
	if ( index >= n_keys - 1 ) {
		key = m_impl->m_keys[ n_keys - 1 ] ;
		key.SetFrame( frame ) ;
		return index ;
	}

	MdxKeyFrame &key1 = m_impl->m_keys[ index ] ;
	MdxKeyFrame &key2 = m_impl->m_keys[ index + 1 ] ;
	float f1 = key1.GetFrame() ;
	float f2 = key2.GetFrame() ;
	int format = m_impl->m_format ;
	if ( format == MDX_FCURVE_CONSTANT || f1 == f2 || f1 == frame ) {
		key = key1 ;
		key.SetFrame( frame ) ;
		return index ;
	}

	key.SetFormat( m_impl->m_format ) ;
	key.SetDimCount( m_impl->m_dim_count ) ;
	key.SetFrame( frame ) ;

	switch ( format ) {
	    case MDX_FCURVE_LINEAR : {
		float t = ( frame - f1 ) / ( f2 - f1 ) ;
		for ( int i = 0 ; i < m_impl->m_dim_count ; i ++ ) {
			float v1 = key1.GetValue( i ) ;
			float v2 = key2.GetValue( i ) ;
			key.SetValue( i, ( v2 - v1 ) * t + v1 ) ;
		}
		break ;
	    }
	    case MDX_FCURVE_HERMITE : {
		float t = ( frame - f1 ) / ( f2 - f1 ) ;
		float s = 1.0f - t ;
		float t2 = t * t ;
		float s2 = s * s ;
		float B1 = s2 * t * 3.0f ;
		float B2 = t2 * s * 3.0f ;
		float B0 = s2 * s + B1 ;
		float B3 = t2 * t + B2 ;
		float b0 = t * s * 2.0f ;
		float b1 = s2 - b0 ;
		float b2 = b0 - t2 ;
		for ( int i = 0 ; i < m_impl->m_dim_count ; i ++ ) {
			float v1 = key1.GetValue( i ) ;
			float v2 = key2.GetValue( i ) ;
			float dy1 = key1.GetOutDY( i ) ;
			float dy2 = key2.GetInDY( i ) ;
			key.SetValue( i, B0 * v1 + B1 * dy1 + B2 * dy2 + B3 * v2 ) ;
			if ( eval_handles ) {
				float dy = b0 * ( v2 - v1 ) + b1 * dy1 + b2 * dy2 ;
				key.SetInDY( i, -dy ) ;
				key.SetOutDY( i, dy ) ;
			}
		}
		break ;
	    }
	    case MDX_FCURVE_CUBIC : {
		for ( int i = 0 ; i < m_impl->m_dim_count ; i ++ ) {
			float dx1 = key1.GetOutDX( i ) ;
			float dx2 = key2.GetInDX( i ) ;
			float t = find_root( f1, f1 + dx1, f2 + dx2, f2, frame ) ;
			float s = 1.0f - t ;
			float t2 = t * t ;
			float s2 = s * s ;
			float B1 = s2 * t * 3.0f ;
			float B2 = t2 * s * 3.0f ;
			float B0 = s2 * s + B1 ;
			float B3 = t2 * t + B2 ;
			float v1 = key1.GetValue( i ) ;
			float v2 = key2.GetValue( i ) ;
			float dy1 = key1.GetOutDY( i ) ;
			float dy2 = key2.GetInDY( i ) ;
			key.SetValue( i, B0 * v1 + B1 * dy1 + B2 * dy2 + B3 * v2 ) ;
			if ( eval_handles ) {
				float b0 = t * s * 2.0f ;
				float b1 = s2 - b0 ;
				float b2 = b0 - t2 ;
				float dy = b0 * ( v2 - v1 ) + b1 * dy1 + b2 * dy2 ;
				key.SetInDY( i, -dy ) ;
				key.SetOutDY( i, dy ) ;
				float dx = b0 * ( f2 - f1 ) + b1 * dx1 + b2 * dx2 ;
				key.SetInDX( i, -dx ) ;
				key.SetOutDX( i, dx ) ;
			}
		}
		break ;
	    }
	    case MDX_FCURVE_SPHERICAL : {
		quat q1( key1.GetValue( 0 ), key1.GetValue( 1 ), key1.GetValue( 2 ), key1.GetValue( 3 ) ) ;
		quat q2( key2.GetValue( 0 ), key2.GetValue( 1 ), key2.GetValue( 2 ), key2.GetValue( 3 ) ) ;
		float t = ( frame - f1 ) / ( f2 - f1 ) ;
		quat q = slerp( q1, q2, t ) ;
		key.SetValue( 0, q.x ) ;
		key.SetValue( 1, q.y ) ;
		key.SetValue( 2, q.z ) ;
		key.SetValue( 3, q.w ) ;
		break ;
	    }
	}
	return index ;
}

//----------------------------------------------------------------
//  convenience functions ( keyframe format )
//----------------------------------------------------------------

int MdxFCurve::GetKeyFrameStride() const
{
	return MdxKeyFrame::GetFormatStride( m_impl->m_format, m_impl->m_dim_count ) ;
}

bool MdxFCurve::HasKeyFrameDY() const
{
	int format = m_impl->m_format ;
	return ( format == MDX_FCURVE_HERMITE || format == MDX_FCURVE_CUBIC ) ;
}

bool MdxFCurve::HasKeyFrameDX() const
{
	int format = m_impl->m_format ;
	return ( format == MDX_FCURVE_CUBIC ) ;
}

int MdxFCurve::GetFormatStride( int format, int dim_count )
{
	return MdxKeyFrame::GetFormatStride( format, dim_count ) ;
}

//----------------------------------------------------------------
//  FCurve keyframe
//----------------------------------------------------------------

MdxKeyFrame::MdxKeyFrame()
{
	m_impl = new impl ;
	m_impl->m_format = MDX_FCURVE_LINEAR ;
	m_impl->m_dim_count = 0 ;
	m_impl->m_frame = 0.0f ;
}

MdxKeyFrame::~MdxKeyFrame()
{
	delete m_impl ;
}

MdxKeyFrame::MdxKeyFrame( const MdxKeyFrame &src )
{
	m_impl = new impl ;
	m_impl->m_format = MDX_FCURVE_LINEAR ;
	m_impl->m_dim_count = 0 ;
	m_impl->m_frame = 0.0f ;
	*this = src ;
}

MdxKeyFrame &MdxKeyFrame::operator=( const MdxKeyFrame &src )
{
	*m_impl = *( src.m_impl ) ;
	return *this ;
}

bool MdxKeyFrame::operator==( const MdxKeyFrame &src ) const
{
	if ( m_impl->m_dim_count != src.m_impl->m_dim_count ) return false ;
	if ( m_impl->m_frame != src.m_impl->m_frame ) return false ;
	if ( m_impl->m_values != src.m_impl->m_values ) return false ;
	if ( m_impl->m_in_dys != src.m_impl->m_in_dys ) return false ;
	if ( m_impl->m_out_dys != src.m_impl->m_out_dys ) return false ;
	if ( m_impl->m_in_dxs != src.m_impl->m_in_dxs ) return false ;
	if ( m_impl->m_out_dxs != src.m_impl->m_out_dxs ) return false ;
	return true ;
}

bool MdxKeyFrame::operator!=( const MdxKeyFrame &src ) const
{
	return !operator==( src ) ;
}

//----------------------------------------------------------------
//  keyframe format
//----------------------------------------------------------------

void MdxKeyFrame::SetFormat( int format )
{
	if ( m_impl->m_format == format ) return ;
	m_impl->m_format = format ;

	int count = m_impl->m_dim_count ;
	m_impl->m_values.resize( count ) ;
	if ( format != MDX_FCURVE_HERMITE && format != MDX_FCURVE_CUBIC ) count = 0 ;
	m_impl->m_in_dys.resize( count ) ;
	m_impl->m_out_dys.resize( count ) ;
	if ( format != MDX_FCURVE_CUBIC ) count = 0 ;
	m_impl->m_in_dxs.resize( count ) ;
	m_impl->m_out_dxs.resize( count ) ;
}

int MdxKeyFrame::GetFormat() const
{
	return m_impl->m_format ;
}

void MdxKeyFrame::SetInterp( int interp )
{
	SetFormat( interp ) ;
}

int MdxKeyFrame::GetInterp() const
{
	return m_impl->m_format ;
}

void MdxKeyFrame::SetDimCount( int count )
{
//	if ( count <= 0 ) count = 1 ;
	if ( m_impl->m_dim_count == count ) return ;
	m_impl->m_dim_count = count ;

	int format = m_impl->m_format ;
	m_impl->m_values.resize( count ) ;
	if ( format != MDX_FCURVE_HERMITE && format != MDX_FCURVE_CUBIC ) count = 0 ;
	m_impl->m_in_dys.resize( count ) ;
	m_impl->m_out_dys.resize( count ) ;
	if ( format != MDX_FCURVE_CUBIC ) count = 0 ;
	m_impl->m_in_dxs.resize( count ) ;
	m_impl->m_out_dxs.resize( count ) ;
}

int MdxKeyFrame::GetDimCount() const
{
	return m_impl->m_dim_count ;
}

//----------------------------------------------------------------
//  keyframe data
//----------------------------------------------------------------

void MdxKeyFrame::SetFrame( float frame )
{
	m_impl->m_frame = frame ;
}

float MdxKeyFrame::GetFrame() const
{
	return m_impl->m_frame ;
}

void MdxKeyFrame::SetValue( int num, float value )
{
	if ( num < 0 || num >= (int)m_impl->m_values.size() ) return ;
	m_impl->m_values[ num ] = value ;
}

float MdxKeyFrame::GetValue( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_values.size() ) return 0.0f ;
	return m_impl->m_values[ num ] ;
}

void MdxKeyFrame::SetInDY( int num, float dy )
{
	if ( num < 0 || num >= (int)m_impl->m_in_dys.size() ) return ;
	m_impl->m_in_dys[ num ] = dy ;
}

float MdxKeyFrame::GetInDY( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_in_dys.size() ) return 0.0f ;
	return m_impl->m_in_dys[ num ] ;
}

void MdxKeyFrame::SetOutDY( int num, float dy )
{
	if ( num < 0 || num >= (int)m_impl->m_out_dys.size() ) return ;
	m_impl->m_out_dys[ num ] = dy ;
}

float MdxKeyFrame::GetOutDY( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_out_dys.size() ) return 0.0f ;
	return m_impl->m_out_dys[ num ] ;
}

void MdxKeyFrame::SetInDX( int num, float dx )
{
	if ( num < 0 || num >= (int)m_impl->m_in_dxs.size() ) return ;
	m_impl->m_in_dxs[ num ] = dx ;
}

float MdxKeyFrame::GetInDX( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_in_dxs.size() ) return 0.0f ;
	return m_impl->m_in_dxs[ num ] ;
}

void MdxKeyFrame::SetOutDX( int num, float dx )
{
	if ( num < 0 || num >= (int)m_impl->m_out_dxs.size() ) return ;
	m_impl->m_out_dxs[ num ] = dx ;
}

float MdxKeyFrame::GetOutDX( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_out_dxs.size() ) return 0.0f ;
	return m_impl->m_out_dxs[ num ] ;
}

//----------------------------------------------------------------
//  convenience functions ( keyframe elements )
//----------------------------------------------------------------

void MdxKeyFrame::SetValueVec2( int num, const vec2 &v )
{
	if ( num < 0 || num + 2 > (int)m_impl->m_values.size() ) return ;
	*(vec2 *)&( m_impl->m_values[ num ] ) = v ;
}

const vec2 &MdxKeyFrame::GetValueVec2( int num ) const
{
	if ( num < 0 || num + 2 > (int)m_impl->m_values.size() ) return vec2_zero ;
	return *(const vec2 *)&( m_impl->m_values[ num ] ) ;
}

void MdxKeyFrame::SetValueVec3( int num, const vec3 &v )
{
	if ( num < 0 || num + 3 > (int)m_impl->m_values.size() ) return ;
	*(vec3 *)&( m_impl->m_values[ num ] ) = v ;
}

const vec3 &MdxKeyFrame::GetValueVec3( int num ) const
{
	if ( num < 0 || num + 3 > (int)m_impl->m_values.size() ) return vec3_zero ;
	return *(const vec3 *)&( m_impl->m_values[ num ] ) ;
}

void MdxKeyFrame::SetValueVec4( int num, const vec4 &v )
{
	if ( num < 0 || num + 4 > (int)m_impl->m_values.size() ) return ;
	*(vec4 *)&( m_impl->m_values[ num ] ) = v ;
}

const vec4 &MdxKeyFrame::GetValueVec4( int num ) const
{
	if ( num < 0 || num + 4 > (int)m_impl->m_values.size() ) return vec4_zero ;
	return *(const vec4 *)&( m_impl->m_values[ num ] ) ;
}

void MdxKeyFrame::SetValueMat4( int num, const mat4 &v )
{
	if ( num < 0 || num + 16 > (int)m_impl->m_values.size() ) return ;
	*(mat4 *)&( m_impl->m_values[ num ] ) = v ;
}

const mat4 &MdxKeyFrame::GetValueMat4( int num ) const
{
	if ( num < 0 || num + 16 > (int)m_impl->m_values.size() ) return mat4_one ;
	return *(const mat4 *)&( m_impl->m_values[ num ] ) ;
}

void MdxKeyFrame::SetValueQuat( int num, const quat &v )
{
	if ( num < 0 || num + 4 > (int)m_impl->m_values.size() ) return ;
	*(quat *)&( m_impl->m_values[ num ] ) = v ;
}

const quat &MdxKeyFrame::GetValueQuat( int num ) const
{
	if ( num < 0 || num + 4 > (int)m_impl->m_values.size() ) return quat_one ;
	return *(const quat *)&( m_impl->m_values[ num ] ) ;
}

void MdxKeyFrame::SetValueRect( int num, const rect &v )
{
	if ( num < 0 || num + 4 > (int)m_impl->m_values.size() ) return ;
	*(rect *)&( m_impl->m_values[ num ] ) = v ;
}

const rect &MdxKeyFrame::GetValueRect( int num ) const
{
	if ( num < 0 || num + 4 > (int)m_impl->m_values.size() ) return rect_one ;
	return *(const rect *)&( m_impl->m_values[ num ] ) ;
}

//----------------------------------------------------------------
//  convenience functions ( keyframe format )
//----------------------------------------------------------------

int MdxKeyFrame::GetStride() const
{
	return GetFormatStride( m_impl->m_format, m_impl->m_dim_count ) ;
}

bool MdxKeyFrame::HasDY() const
{
	int format = m_impl->m_format ;
	return ( format == MDX_FCURVE_HERMITE || format == MDX_FCURVE_CUBIC ) ;
}

bool MdxKeyFrame::HasDX() const
{
	int format = m_impl->m_format ;
	return ( format == MDX_FCURVE_CUBIC ) ;
}

int MdxKeyFrame::GetFormatStride( int format, int dim_count )
{
	int count = 1 ;
	if ( format == MDX_FCURVE_HERMITE ) count += 2 ;
	if ( format == MDX_FCURVE_CUBIC ) count += 2 ;
	return count * dim_count ;
}

//----------------------------------------------------------------
//  subroutine
//----------------------------------------------------------------

float find_root( float y0, float y1, float y2, float y3, float y )
{
	float E = ( y3 - y0 ) * 0.01f ;
	if ( E < 0.0001f ) E = 0.0001f ;
	float t0 = 0.0f ;
	float t3 = 1.0f ;
	for ( int i = 0 ; i < 8 ; i ++ ) {
		float d = y3 - y0 ;
		if ( d > -E && d < E ) break ;
		float r = ( y2 - y1 ) / d - ( 1.0f / 3.0f ) ;
		if ( r > -0.01f && r < 0.01f ) break ;
		float yc = ( y0 + y1 * 3.0f + y2 * 3.0f + y3 ) / 8.0f ;
		if ( y < yc ) {
			y3 = yc ;
			y2 = ( y1 + y2 ) * 0.5f ;
			y1 = ( y0 + y1 ) * 0.5f ;
			y2 = ( y1 + y2 ) * 0.5f ;
			t3 = ( t0 + t3 ) * 0.5f ;
		} else {
			y0 = yc ;
			y1 = ( y1 + y2 ) * 0.5f ;
			y2 = ( y2 + y3 ) * 0.5f ;
			y1 = ( y1 + y2 ) * 0.5f ;
			t0 = ( t0 + t3 ) * 0.5f ;
		}
	}
	float c = y0 - y ;
	float b = 3.0f * ( y1 - y0 ) ;
	float a = y3 - y0 - b ;
	float x ;
	if ( fabsf( a ) < 0.0001f ) {
		x = ( fabsf( b ) < 0.0001f ) ? 0.5f : -c / b ;
	} else {
		float D2 = b * b - 4.0f * a * c ;
		if ( D2 < 0.0f ) D2 = 0.0f ;
		D2 = sqrtf( D2 ) ;
		if ( a + b < 0.0f ) D2 = -D2 ;
		x = ( -b + D2 ) / ( 2.0f * a ) ;
	}
	return ( t3 - t0 ) * x + t0 ;
}


} // namespace mdx
