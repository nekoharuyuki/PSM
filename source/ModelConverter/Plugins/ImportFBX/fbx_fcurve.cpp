#include "fbx_loader.h"

#pragma warning( disable:4244 )

//----------------------------------------------------------------
//  fbx_fcurve
//----------------------------------------------------------------

fbx_fcurve::fbx_fcurve()
{
	;
}

fbx_fcurve::~fbx_fcurve()
{
	clear() ;
}

//----------------------------------------------------------------
//  load and save
//----------------------------------------------------------------

void fbx_fcurve::clear()
{
	m_src.clear() ;
	m_val.clear() ;
	m_dst = 0 ;
}

void fbx_fcurve::load( KFCurve *src, float val )
{
	m_src.push_back( src ) ;
	m_val.push_back( val ) ;
}

void fbx_fcurve::save( MdxFCurve *dst, KTime start, KTime end, KTime step )
{
	if ( m_src.empty() || dst == 0 ) return ;

	//  plot fcurve

	int n_dims = m_src.size() ;
	dst->SetInterp( MDX_FCURVE_LINEAR ) ;
	dst->SetDimCount( n_dims ) ;

	bool defined = false ;
	int extrap_l = 0, extrap_r = 0 ;
	for ( int i = 0 ; i < n_dims ; i ++ ) {
		KFCurve *src = m_src[ i ] ;
		if ( src == 0 || src->KeyGetCount() == 0 ) continue ;
		KTime s = src->KeyGetTime( 0 ) ;
		KTime e = src->KeyGetTime( src->KeyGetCount() - 1 ) ;
		if ( !defined ) {
			start = s ;
			end = e ;
			defined = true ;
		} else {
			if ( start > s ) start = s ;
			if ( end < e ) end = e ;
		}

		int extrap = get_extrap( src ) ;
		int l = extrap & MDX_FCURVE_EXTRAP_IN_MASK ;
		int r = extrap & MDX_FCURVE_EXTRAP_OUT_MASK ;
		if ( extrap_l < l ) extrap_l = l ;
		if ( extrap_r < r ) extrap_r = r ;
	}
	end = ( end - start + step - KTIME_EPSILON ) / step * step + start ;
	dst->SetExtrap( extrap_l | extrap_r ) ;

	int n_keys = 0 ;
	unsigned int changes = 0 ;
	for ( KTime t = start ; t <= end ; t += step ) {
		MdxKeyFrame &key = dst->GetKeyFrame( n_keys ++ ) ;
		MdxKeyFrame &key0 = dst->GetKeyFrame( 0 ) ;
		key.SetFrame( t.GetFrame( true ) ) ;

		for ( int i = 0 ; i < n_dims ; i ++ ) {
			KFCurve *src = m_src[ i ] ;
			float val = ( src == 0 ) ? m_val[ i ] : src->Evaluate( t ) ;
			key.SetValue( i, val ) ;

			if ( val != key0.GetValue( i ) ) changes |= ( 1 << i ) ;
		}
	}

	//  reduce constant key

	for ( int i = 0 ; i < n_dims ; i ++ ) {
		if ( ( changes & ( 1 << i ) ) == 0 ) continue ;
		KFCurve *src = m_src[ i ] ;
		if ( src == 0 ) continue ;
		int n_keys = src->KeyGetCount() - 1 ;
		int flags = KFCURVE_INTERPOLATION_CONSTANT ;
		for ( int j = 0 ; j < n_keys ; j ++ ) {
			KFCurveKey key = src->KeyGet( j ) ;
			if ( ( key.GetInterpolation() & flags ) == 0 ) return ;
		}
	}
	dst->SetInterp( MDX_FCURVE_CONSTANT ) ;
	if ( changes == 0 ) dst->SetKeyFrameCount( 1 ) ;
}

//----------------------------------------------------------------
//  check activity
//----------------------------------------------------------------

bool fbx_fcurve::is_active( int mode, KTime start, KTime end, KTime step )
{
	for ( int i = 0 ; i < (int)m_src.size() ; i ++ ) {
		KFCurve *src = m_src[ i ] ;
		if ( src == 0 || src->KeyGetCount() == 0 ) continue ;

		switch ( mode ) {
		    case fbx_loader::MODE_OFF : return true ;
		    case fbx_loader::MODE_ON : break ;
		    case fbx_loader::MODE_AUTO :
			if ( src->KeyGetCount() >= 2 ) return true ;
			break ;
		}
		float val = m_val[ i ] ;
		for ( KTime t = start ; t <= end ; t += step ) {
			if ( (float)src->Evaluate( t ) != val ) return true ;
		}
	}
	return false ;
}

//----------------------------------------------------------------
//  check format
//----------------------------------------------------------------

int fbx_fcurve::get_extrap( KFCurve *fcurve )
{
	static int Flags[] = {
		MDX_FCURVE_HOLD,
		MDX_FCURVE_HOLD,		// 1 = KFCURVE_EXTRAPOLATION_CONST
		MDX_FCURVE_CYCLE,		// 2 = KFCURVE_EXTRAPOLATION_REPETITION
		MDX_FCURVE_SHUTTLE,		// 3 = KFCURVE_EXTRAPOLATION_MIRROR_REPETITION
		MDX_FCURVE_EXTEND,		// 4 = KFCURVE_EXTRAPOLATION_KEEP_SLOPE
	} ;
	if ( fcurve == 0 ) return 0 ;
	int extrap_l = Flags[ fcurve->GetPreExtrapolation() % 5 ] & MDX_FCURVE_EXTRAP_IN_MASK ;
	int extrap_r = Flags[ fcurve->GetPostExtrapolation() % 5 ] & MDX_FCURVE_EXTRAP_OUT_MASK ;
	return extrap_l | extrap_r ;
}

