#include "xsi_loader.h"

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

#define REL_MARGIN (0.01f)
#define ABS_MARGIN (0.0001f)

static bool is_laddered( float y0, float y1, float y2, float y3 ) ;

//----------------------------------------------------------------
//  xsi_fcurve2
//----------------------------------------------------------------

xsi_fcurve2::xsi_fcurve2()
{
	m_scale = 1.0f ;
}

xsi_fcurve2::~xsi_fcurve2()
{
	unload() ;
}

//----------------------------------------------------------------
//  load and save
//----------------------------------------------------------------

void xsi_fcurve2::unload()
{
	for ( int i = 0 ; i < (int)m_src.size() ; i ++ ) m_src[ i ]->Release() ;
	m_src.clear() ;
	m_val.clear() ;
}

void xsi_fcurve2::load( CdotXSITemplate *src, float val )
{
	load_fcurve( src, val ) ;
}

void xsi_fcurve2::save( MdxFCurve *dst )
{
	if ( m_src.empty() || dst == 0 ) return ;
	save_fcurve( dst ) ;
}

//----------------------------------------------------------------
//  load fcurve
//----------------------------------------------------------------

void xsi_fcurve2::load_fcurve( CdotXSITemplate *src, float val )
{
	MdxFCurve *fcurve = ( src == 0 ) ? 0 : new MdxFCurve ;
	m_src.push_back( fcurve ) ;
	m_val.push_back( val ) ;
	if ( src == 0 ) return ;

	string interp = xsi_loader::get_string( src, 2 ) ;
	// int n_dims = xsi_loader::get_int( src, 3 ) ;
	// int n_vals = xsi_loader::get_int( src, 4 ) ;
	int n_keys = xsi_loader::get_int( src, 5 ) ;

	int format = MDX_FCURVE_LINEAR ;
	if ( interp == "CONSTANT" ) format = MDX_FCURVE_CONSTANT ;
	if ( interp == "HERMITE" ) format = MDX_FCURVE_HERMITE ;
	if ( interp == "LINEAR" ) format = MDX_FCURVE_LINEAR ;
	if ( interp == "CUBIC" ) format = MDX_FCURVE_CUBIC ;
	fcurve->SetFormat( format ) ;
	fcurve->SetDimCount( 1 ) ;
	fcurve->SetKeyFrameCount( n_keys ) ;

	int format2 = ( format == MDX_FCURVE_CONSTANT ) ? MDX_FCURVE_CONSTANT : MDX_FCURVE_LINEAR ;
	int stride = ( format == MDX_FCURVE_CUBIC ) ? 6 : ( ( format == MDX_FCURVE_HERMITE ) ? 4 : 2 ) ;
	int idx2 = 0 ;

	MdxKeyFrame &k = fcurve->GetKeyFrame( 0 ) ;
	k.SetFrame( xsi_loader::get_float( src, 6, idx2 + 0 ) ) ;
	k.SetValue( 0, xsi_loader::get_float( src, 6, idx2 + 1 ) * m_scale ) ;

	for ( int i = 1 ; i < n_keys ; i ++ ) {
		MdxKeyFrame &k1 = fcurve->GetKeyFrame( i - 1 ) ;
		MdxKeyFrame &k2 = fcurve->GetKeyFrame( i ) ;
		int idx1 = idx2 ;
		idx2 += stride ;
		k2.SetFrame( xsi_loader::get_float( src, 6, idx2 + 0 ) ) ;
		k2.SetValue( 0, xsi_loader::get_float( src, 6, idx2 + 1 ) * m_scale ) ;
		if ( format == MDX_FCURVE_HERMITE ) {
			float dx = ( k2.GetFrame() - k1.GetFrame() ) / 3.0f ;
			k1.SetOutDY( 0, xsi_loader::get_float( src, 6, idx1 + 3 ) * dx * m_scale ) ;
			k2.SetInDY( 0, -xsi_loader::get_float( src, 6, idx2 + 2 ) * dx * m_scale ) ;
		}
		if ( format == MDX_FCURVE_CUBIC ) {
			k1.SetOutDX( 0, xsi_loader::get_float( src, 6, idx1 + 4 ) ) ;
			k1.SetOutDY( 0, xsi_loader::get_float( src, 6, idx1 + 5 ) * m_scale ) ;
			k2.SetInDX( 0, xsi_loader::get_float( src, 6, idx2 + 2 ) ) ;
			k2.SetInDY( 0, xsi_loader::get_float( src, 6, idx2 + 3 ) * m_scale ) ;
		}
		if ( format >= MDX_FCURVE_HERMITE && format2 < MDX_FCURVE_HERMITE ) {
			if ( !is_laddered( k1.GetValue( 0 ), k1.GetValue( 0 ) + k1.GetOutDY( 0 ),
					k2.GetInDY( 0 ) + k2.GetValue( 0 ), k2.GetValue( 0 ) ) ) {
				format2 = MDX_FCURVE_HERMITE ;
			}
		}
		if ( format >= MDX_FCURVE_CUBIC && format2 < MDX_FCURVE_CUBIC ) {
			if ( !is_laddered( k1.GetFrame(), k1.GetFrame() + k1.GetOutDX( 0 ),
					k2.GetInDX( 0 ) + k2.GetFrame(), k2.GetFrame() ) ) {
				format2 = MDX_FCURVE_CUBIC ;
			}
		}
	}
	if ( format2 < format ) fcurve->SetFormat( format2 ) ;
}

//----------------------------------------------------------------
//  save fcurve
//----------------------------------------------------------------

void xsi_fcurve2::save_fcurve( MdxFCurve *dst )
{
	int i, j ;

	//  check fcurves

	int interp = MDX_FCURVE_CONSTANT ;
	int extrap = MDX_FCURVE_HOLD ;
	vector<float> frames ;
	for ( i = 0 ; i < (int)m_src.size() ; i ++ ) {
		MdxFCurve *fcurve = m_src[ i ] ;
		if ( fcurve == 0 ) continue ;
		int interp2 = fcurve->GetInterp() ;
		int extrap2 = fcurve->GetExtrap() ;
		if ( interp2 > interp ) interp = interp2 ;
		if ( extrap2 > extrap ) extrap = extrap2 ;
		for ( j = 0 ; j < fcurve->GetKeyFrameCount() ; j ++ ) {
			frames.push_back( fcurve->GetKeyFrame( j ).GetFrame() ) ;
		}
	}
	sort( frames.begin(), frames.end() ) ;
	frames.erase( unique( frames.begin(), frames.end() ), frames.end() ) ;
	if ( frames.empty() ) frames.push_back( 0.0f ) ;

	//  create fcurve

	dst->SetInterp( interp ) ;
	dst->SetExtrap( extrap ) ;
	dst->SetDimCount( m_src.size() ) ;
	dst->SetKeyFrameCount( frames.size() ) ;
	for ( i = 0 ; i < (int)frames.size() ; i ++ ) {
		dst->GetKeyFrame( i ).SetFrame( frames[ i ] ) ;
	}

	//  merge fcurves

	for ( i = 0 ; i < (int)m_src.size() ; i ++ ) {
		MdxFCurve *fcurve = m_src[ i ] ;
		if ( fcurve == 0 ) {
			float value = m_val[ i ] ;
			for ( j = 0 ; j < dst->GetKeyFrameCount() ; j ++ ) {
				MdxKeyFrame &key2 = dst->GetKeyFrame( j ) ;
				key2.SetValue( i, value ) ;
				if ( j > 0 ) {
					MdxKeyFrame &key1 = dst->GetKeyFrame( j - 1 ) ;
					float dx = ( key2.GetFrame() - key1.GetFrame() ) / 3.0f ;
					key1.SetOutDX( i, dx ) ;
					key2.SetInDX( i, -dx ) ;
				}
			}
		} else {
			MdxFCurve tmp = *fcurve ;
			for ( j = 0 ; j < dst->GetKeyFrameCount() ; j ++ ) {
				float frame = dst->GetKeyFrame( j ).GetFrame() ;
				if ( tmp.IndexOfKeyFrame( frame ) >= 0 ) continue ;
				MdxKeyFrame key ;
				int index = tmp.Eval( frame, key, true ) ;
				tmp.InsertKeyFrame( index + 1, key, true ) ;
			}
			tmp.SetFormat( dst->GetFormat(), true ) ;

			for ( j = 0 ; j < dst->GetKeyFrameCount() ; j ++ ) {
				MdxKeyFrame &key = tmp.GetKeyFrame( j ) ;
				MdxKeyFrame &key2 = dst->GetKeyFrame( j ) ;
				key2.SetValue( i, key.GetValue( 0 ) ) ;
				key2.SetInDX( i, key.GetInDX( 0 ) ) ;
				key2.SetInDY( i, key.GetInDY( 0 ) ) ;
				key2.SetOutDX( i, key.GetOutDX( 0 ) ) ;
				key2.SetOutDY( i, key.GetOutDY( 0 ) ) ;
			}
		}
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
