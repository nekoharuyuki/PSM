#include "xsi_loader.h"

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

#define REL_MARGIN (0.01f)
#define ABS_MARGIN (0.0001f)
#define MAX_REPEAT (8)

static bool is_laddered( float y0, float y1, float y2, float y3 ) ;
static float find_root( float y0, float y1, float y2, float y3, float y ) ;

//----------------------------------------------------------------
//  xsi_fcurve
//----------------------------------------------------------------

xsi_fcurve::xsi_fcurve()
{
	m_scale = 1.0f ;

	m_format = MDX_FCURVE_CONSTANT ;
	m_dim_count = 0 ;
	m_key_count = 0 ;
}

xsi_fcurve::~xsi_fcurve()
{
	unload() ;
}

//----------------------------------------------------------------
//  load and save
//----------------------------------------------------------------

void xsi_fcurve::unload()
{
	m_src.clear() ;
	m_val.clear() ;
	m_dst = 0 ;
	m_frames.clear() ;
}

void xsi_fcurve::load( CSLFCurve *src, float val )
{
	m_src.push_back( src ) ;
	m_val.push_back( val ) ;
}

void xsi_fcurve::save( MdxFCurve *dst )
{
	if ( m_src.empty() || dst == 0 ) return ;

	m_dst = dst ;
	fixup_format() ;
	fixup_keyframes() ;
}

//----------------------------------------------------------------
//  fcurve format
//----------------------------------------------------------------

void xsi_fcurve::fixup_format()
{
	int i, j ;

	m_format = MDX_FCURVE_CONSTANT ;
	m_dim_count = m_src.size() ;
	m_frames.clear() ;

	for ( i = 0 ; i < m_dim_count ; i ++ ) {
		CSLFCurve *src = m_src[ i ] ;
		if ( src == 0 ) continue ;
		switch ( src->GetInterpolationType() ) {
		    case CSLTemplate::SI_CONSTANT : {
			CSLConstantKey *keys = src->GetConstantKeyListPtr() ;
			for ( j = 0 ; j < src->GetKeyCount() ; j ++ ) {
				m_frames.push_back( keys[ j ].m_fTime ) ;
			}
			break ;
		    }
		    case CSLTemplate::SI_LINEAR : {
			CSLLinearKey *keys = src->GetLinearKeyListPtr() ;
			for ( j = 0 ; j < src->GetKeyCount() ; j ++ ) {
				m_frames.push_back( keys[ j ].m_fTime ) ;
			}
			if ( m_format < MDX_FCURVE_LINEAR ) m_format = MDX_FCURVE_LINEAR ;
			break ;
		    }
		    case CSLTemplate::SI_HERMITE : {
			CSLHermiteKey *keys = src->GetHermiteKeyListPtr() ;
			for ( j = 0 ; j < src->GetKeyCount() ; j ++ ) {
				if ( m_format < MDX_FCURVE_HERMITE && j > 0 ) {
					float T = ( keys[ j - 0 ].m_fTime - keys[ j - 1 ].m_fTime ) / 3.0f ;
					float a = keys[ j - 1 ].m_fValue ;
					float b = keys[ j - 1 ].m_fOutTangent * T ;
					float c = - keys[ j + 0 ].m_fInTangent * T ;
					float d = keys[ j + 0 ].m_fValue ;
					if ( !is_laddered( a, a + b, c + d, d ) ) {
						m_format = MDX_FCURVE_HERMITE ;
					}
				}
				m_frames.push_back( keys[ j ].m_fTime ) ;
			}
			if ( m_format < MDX_FCURVE_LINEAR ) m_format = MDX_FCURVE_LINEAR ;
			break ;
		    }
		    case CSLTemplate::SI_CUBIC : {
			CSLCubicKey *keys = src->GetCubicKeyListPtr() ;
			for ( j = 0 ; j < src->GetKeyCount() ; j ++ ) {
				if ( m_format < MDX_FCURVE_CUBIC && j > 0 ) {
					float a = keys[ j - 1 ].m_fTime ;
					float b = keys[ j - 1 ].m_fRightTanX ;
					float c = keys[ j + 0 ].m_fLeftTanX ;
					float d = keys[ j + 0 ].m_fTime ;
					if ( !is_laddered( a, a + b, c + d, d ) ) {
						m_format = MDX_FCURVE_CUBIC ;
					}
				}
				if ( m_format < MDX_FCURVE_HERMITE && j > 0 ) {
					float a = keys[ j - 1 ].m_fValue ;
					float b = keys[ j - 1 ].m_fRightTanY ;
					float c = keys[ j + 0 ].m_fLeftTanY ;
					float d = keys[ j + 0 ].m_fValue ;
					if ( !is_laddered( a, a + b, c + d, d ) ) {
						m_format = MDX_FCURVE_HERMITE ;
					}
				}
				m_frames.push_back( keys[ j ].m_fTime ) ;
			}
			if ( m_format < MDX_FCURVE_LINEAR ) m_format = MDX_FCURVE_LINEAR ;
			break ;
		    }
		}
	}
	sort( m_frames.begin(), m_frames.end() ) ;
	m_frames.erase( unique( m_frames.begin(), m_frames.end() ), m_frames.end() ) ;
	m_key_count = m_frames.size() ;

	m_dst->SetFormat( m_format ) ;
	m_dst->SetDimCount( m_dim_count ) ;
	m_dst->SetKeyFrameCount( m_key_count ) ;

	for ( i = 0 ; i < m_key_count ; i ++ ) {
		m_dst->GetKeyFrame( i ).SetFrame( m_frames[ i ] ) ;
	}
}

//----------------------------------------------------------------
//  fcurve keyframes
//----------------------------------------------------------------

void xsi_fcurve::fixup_keyframes()
{
	if ( m_key_count == 0 ) return ;

	for ( int i = 0 ; i < m_dim_count ; i ++ ) {
		CSLFCurve *src = m_src[ i ] ;

		float y0, y1 ;
		if ( src == 0 ) {
			y1 = m_val[ i ] * m_scale ;
		} else {
			src->Evaluate( m_frames[ 0 ] ) ;
			y1 = src->GetLastEvaluation() * m_scale ;
		}
		m_dst->GetKeyFrame( 0 ).SetValue( i, y1 ) ;

		for ( int j = 0 ; j < m_key_count - 1 ; j ++ ) {
			MdxKeyFrame &k0 = m_dst->GetKeyFrame( j ) ;
			MdxKeyFrame &k1 = m_dst->GetKeyFrame( j + 1 ) ;

			float t0 = m_frames[ j + 0 ] ;
			float t1 = m_frames[ j + 1 ] ;
			float dx = ( t1 - t0 ) / 3.0f ;
			k0.SetOutDX( i, dx ) ;
			k0.SetOutDY( i, 0.0f ) ;
			k1.SetInDX( i, -dx ) ;
			k1.SetInDY( i, 0.0f ) ;

			if ( src == 0 ) {
				k1.SetValue( i, y1 ) ;
				continue ;
			} else {
				src->Evaluate( t1 ) ;
				y0 = y1 ;
				y1 = src->GetLastEvaluation() * m_scale ;
				k1.SetValue( i, y1 ) ;
			}

			switch ( src->GetInterpolationType() ) {
			    case CSLTemplate::SI_CONSTANT : {
				break ;
			    }
			    case CSLTemplate::SI_LINEAR : {
				float dy = ( y1 - y0 ) / 3.0f ;
				k0.SetOutDY( i, dy * m_scale ) ;
				k1.SetInDY( i, -dy * m_scale ) ;
				break ;
			    }
			    case CSLTemplate::SI_HERMITE : {
				if ( src->GetKeyCount() <= 1 ) break ;
				CSLHermiteKey *keys = src->GetHermiteKeyListPtr() ;
				CSLHermiteKey *key = keys + src->GetKeyCount() - 1 ;
				while ( -- key > keys ) {
					if ( key->m_fTime <= t0 ) break ;
				}

				float T0 = key[ 0 ].m_fTime ;
				float T1 = key[ 1 ].m_fTime ;
				float T = T1 - T0 ;
				if ( T == 0.0f ) break ;
				float f0 = ( t0 - T0 ) / T ;
				float f1 = ( t1 - T0 ) / T ;
				T /= 3.0f ;
				float a = key[ 0 ].m_fValue ;
				float b = key[ 0 ].m_fOutTangent * T ;
				float c = - key[ 1 ].m_fInTangent * T ;
				float d = key[ 1 ].m_fValue ;
				float A = a * 2 + b * 3 - c * 3 - d * 2 ;
				float B = ( - a - b * 2 + c + d ) * 2 ;
				float C = b ;
				float d0 = ( ( A * f0 + B ) * f0 + C ) * ( f1 - f0 ) ;
				float d1 = ( ( A * f1 + B ) * f1 + C ) * ( f1 - f0 ) ;
				if ( t0 >= T0 && t0 < T1 ) k0.SetOutDY( i, d0 * m_scale ) ;
				if ( t1 > T0 && t1 <= T1 ) k1.SetInDY( i, - d1 * m_scale ) ;
				break ;
			    }
			    case CSLTemplate::SI_CUBIC : {
				if ( src->GetKeyCount() <= 1 ) break ;
				CSLCubicKey *keys = src->GetCubicKeyListPtr() ;
				CSLCubicKey *key = keys + src->GetKeyCount() - 1 ;
				while ( -- key > keys ) {
					if ( key->m_fTime <= t0 ) break ;
				}

				float T0 = key[ 0 ].m_fTime ;
				float T1 = key[ 1 ].m_fTime ;
				float a = key[ 0 ].m_fTime ;
				float b = key[ 0 ].m_fRightTanX ;
				float c = key[ 1 ].m_fLeftTanX ;
				float d = key[ 1 ].m_fTime ;
				if ( a == d ) break ;
				float f0 = find_root( a, a + b, c + d, d, t0 ) ;
				float f1 = find_root( a, a + b, c + d, d, t1 ) ;
				float A = a * 2 + b * 3 - c * 3 - d * 2 ;
				float B = ( - a - b * 2 + c + d ) * 2 ;
				float C = b ;
				float d0 = ( ( A * f0 + B ) * f0 + C ) * ( f1 - f0 ) ;
				float d1 = ( ( A * f1 + B ) * f1 + C ) * ( f1 - f0 ) ;
				if ( t0 >= T0 && t0 < T1 ) k0.SetOutDX( i, d0 ) ;
				if ( t1 > T0 && t1 <= T1 ) k1.SetInDX( i, -d1 ) ;

				a = key[ 0 ].m_fValue ;
				b = key[ 0 ].m_fRightTanY ;
				c = key[ 1 ].m_fLeftTanY ;
				d = key[ 1 ].m_fValue ;
				A = a * 2 + b * 3 - c * 3 - d * 2 ;
				B = ( - a - b * 2 + c + d ) * 2 ;
				C = b ;
				d0 = ( ( A * f0 + B ) * f0 + C ) * ( f1 - f0 ) ;
				d1 = ( ( A * f1 + B ) * f1 + C ) * ( f1 - f0 ) ;
				if ( t0 >= T0 && t0 < T1 ) k0.SetOutDY( i, d0 * m_scale ) ;
				if ( t1 > T0 && t1 <= T1 ) k1.SetInDY( i, -d1 * m_scale ) ;
				break ;
			    }
			}
		}

		//  rewind

		if ( src != 0 ) src->Evaluate( m_frames[ 0 ] ) ;
	}
}

//----------------------------------------------------------------
//  sub routines
//----------------------------------------------------------------

bool is_laddered( float y0, float y1, float y2, float y3 )
{
	float E = fabsf( y3 - y0 ) * REL_MARGIN ;
	if ( E < ABS_MARGIN ) E = ABS_MARGIN ;
	y3 = y3 - y2 ;
	y2 = y2 - y1 ;
	y1 = y1 - y0 ;
	if ( fabsf( y3 - y2 ) > E ) return false ;
	if ( fabsf( y2 - y1 ) > E ) return false ;
	return true ;
}

float find_root( float y0, float y1, float y2, float y3, float y )
{
	if ( y == y0 ) return 0.0f ;
	if ( y == y3 ) return 1.0f ;

	float E = fabsf( y3 - y0 ) * REL_MARGIN ;
	if ( E < ABS_MARGIN ) E = ABS_MARGIN ;

	float t0 = 0.0f ;
	float t3 = 1.0f ;
	for ( int i = 0 ; i < MAX_REPEAT ; i ++ ) {
		float d = y3 - y0 ;
		if ( fabsf( d ) < E ) break ;
		float r = ( y2 - y1 ) / d - ( 1.0f / 3.0f ) ;
		if ( fabsf( r ) < REL_MARGIN ) break ;

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
	if ( fabsf( a ) < ABS_MARGIN ) {
		x = ( fabsf( b ) < ABS_MARGIN ) ? 0.5f : -c / b ;
	} else {
		float D2 = b * b - 4.0f * a * c ;
		if ( D2 < 0.0f ) D2 = 0.0f ;
		D2 = sqrtf( D2 ) ;
		if ( a + b < 0.0f ) D2 = -D2 ;
		x = ( -b + D2 ) / ( 2.0f * a ) ;
	}
	return ( t3 - t0 ) * x + t0 ;
}
