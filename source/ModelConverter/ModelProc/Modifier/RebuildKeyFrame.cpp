#include "RebuildKeyFrame.h"

namespace mdx {

typedef bool compare_func( MdxKeyFrame &key1, MdxKeyFrame &key2 ) ;

static bool need_fcurve( MdxFCurve *fcurve, MdxAnimate *anim, MdxFormat *format ) ;
static bool need_keyframe( MdxFCurve *fcurve, int idx, MdxFCurve *org, int curr, int prev, compare_func *compare ) ;
static compare_func compare_translate ;
static compare_func compare_rotate ;
static compare_func compare_scale ;
static compare_func compare_misc ;
static compare_func compare_cos ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char VarEpsilonTranslate[] = "epsilon_translate" ;
static const char VarEpsilonRotate[] = "epsilon_rotate" ;
static const char VarEpsilonScale[] = "epsilon_scale" ;
static const char VarEpsilonMisc[] = "epsilon_misc" ;
static const char VarEpsilonStatic[] = "epsilon_static" ;

static float g_epsilon_translate = 0.001f ;
static float g_epsilon_rotate = 0.001f ;
static float g_epsilon_scale = 0.001f ;
static float g_epsilon_misc = 0.001f ;
static float g_epsilon_cos = 0.999f ;	// for quaternion
static float g_epsilon_static = -0.001f ;

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  RebuildKeyFrame
//----------------------------------------------------------------

bool RebuildKeyFrame::Modify( MdxBlock *block )
{
	//  check params

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	int mode = str_search( ModeNames, arg ) ;
	if ( mode < 0 ) {
		Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
		return false ;
	}
	if ( mode == MODE_OFF ) return true ;

	g_epsilon_translate = str_atof( GetVar( VarEpsilonTranslate, "0.0" ) ) ;
	g_epsilon_rotate = str_atof( GetVar( VarEpsilonRotate, "0.0" ) ) ;
	g_epsilon_scale = str_atof( GetVar( VarEpsilonScale, "0.0" ) ) ;
	g_epsilon_misc = str_atof( GetVar( VarEpsilonMisc, "0.0" ) ) ;
	g_epsilon_cos = cosf( g_epsilon_rotate * 0.5f ) ;
	g_epsilon_static = str_atof( GetVar( VarEpsilonStatic, "-1.0" ) ) ;

	//  process

	MdxBlocks fcurves ;
	fcurves.EnumTree( block, MDX_FCURVE ) ;
	for ( int i = 0 ; i < fcurves.size() ; i ++ ) {
		ModifyFCurve( (MdxFCurve *)fcurves[ i ] ) ;
	}
	return true ;
}

void RebuildKeyFrame::ModifyFCurve( MdxFCurve *fcurve )
{
	MdxAnimate *anim = (MdxAnimate *)fcurve->FindReferrer( MDX_ANIMATE ) ;
	int type = ( anim == 0 ) ? 0 : anim->GetCommand() ;

	compare_func *compare = compare_misc ;
	switch ( type ) {
	    case MDX_TRANSLATE :   compare = compare_translate ; break ;
	    case MDX_ROTATE_ZYX :  compare = compare_rotate ; break ;
	    case MDX_ROTATE_YXZ :  compare = compare_rotate ; break ;
	    case MDX_ROTATE :      compare = compare_cos ; break ;
	    case MDX_SCALE :       compare = compare_scale ; break ;
	}

	MdxFCurve copy = *fcurve ;
	int prev = 0, idx = 1 ;
	for ( int curr = 1 ; curr < copy.GetKeyFrameCount() ; curr ++ ) {
		if ( !need_keyframe( fcurve, idx, &copy, curr, prev, compare ) ) {
			fcurve->DeleteKeyFrame( idx ) ;
		} else {
			prev = curr ;
			idx ++ ;
		}
	}
	if ( fcurve->GetKeyFrameCount() == 1 ) {
		fcurve->SetFormat( MDX_FCURVE_CONSTANT ) ;
	}

	//  remove fcurve

	if ( fcurve->GetKeyFrameCount() == 1 && g_epsilon_static >= 0.0f ) {
		MdxBlocks anims ;
		anims.EnumReferrer( fcurve, MDX_ANIMATE ) ;
		for ( int i = 0 ; i < anims.size() ; i ++ ) {
			MdxAnimate *anim = (MdxAnimate *)anims[ i ] ;
			if ( !need_fcurve( fcurve, anim, m_format ) ) anim->Release() ;
		}
		if ( fcurve->FindReferrer() == 0 ) fcurve->Release() ;
	}
}

bool need_fcurve( MdxFCurve *fcurve, MdxAnimate *anim, MdxFormat *format )
{
	if ( fcurve->GetKeyFrameCount() != 1 ) return true ;
	MdxKeyFrame &key = fcurve->GetKeyFrame( 0 ) ;

	MdxBlock *cmd = 0 ;
	MdxBlock *block = anim->GetBlockRef() ;
	if ( block != 0 ) cmd = (MdxBlock *)block->FindTree( anim->GetCommand() ) ;
	if ( cmd == 0 ) cmd = (MdxBlock *)format->GetTemplate( anim->GetCommand() ) ;
	if ( cmd == 0 ) return true ;

	float e = g_epsilon_static ;
	if ( key.GetDimCount() != cmd->GetArgsCount() ) return true ;
	for ( int i = 0 ; i < cmd->GetArgsCount() ; i ++ ) {
		if ( fabsf( key.GetValue( i ) - cmd->GetArgsFloat( i ) ) > e ) return true ;
	}
	return false ;
}

bool need_keyframe( MdxFCurve *fcurve, int idx, MdxFCurve *org, int curr, int prev, compare_func *compare )
{
	MdxFCurve tmp ;
	tmp.SetFormat( fcurve->GetFormat() ) ;
	tmp.SetDimCount( fcurve->GetDimCount() ) ;
	tmp.SetKeyFrame( 0, fcurve->GetKeyFrame( idx - 1 ) ) ;
	tmp.SetKeyFrame( 1, fcurve->GetKeyFrame( idx + 0 ) ) ;
	if ( idx + 1 < fcurve->GetKeyFrameCount() ) {
		tmp.SetKeyFrame( 2, fcurve->GetKeyFrame( idx + 1 ) ) ;
	}
	tmp.DeleteKeyFrame( 1 ) ;

	MdxKeyFrame val ;
	for ( int i = curr ; i > prev ; -- i ) {
		MdxKeyFrame &key = org->GetKeyFrame( i ) ;
		tmp.Eval( key.GetFrame(), val ) ;
		if ( compare( key, val ) ) return true ;
	}
	return false ;
}

bool compare_translate( MdxKeyFrame &key1, MdxKeyFrame &key2 )
{
	float e = g_epsilon_translate ;
	for ( int i = 0 ; i < 3 ; i ++ ) {
		if ( fabsf( key1.GetValue( i ) - key2.GetValue( i ) ) > e ) return true ;
	}
	return false ;
}

bool compare_rotate( MdxKeyFrame &key1, MdxKeyFrame &key2 )
{
	float e = g_epsilon_rotate ;
	for ( int i = 0 ; i < 3 ; i ++ ) {
		if ( fabsf( key1.GetValue( i ) - key2.GetValue( i ) ) > e ) return true ;
	}
	return false ;
}

bool compare_scale( MdxKeyFrame &key1, MdxKeyFrame &key2 )
{
	float e = g_epsilon_scale ;
	for ( int i = 0 ; i < 3 ; i ++ ) {
		if ( fabsf( key1.GetValue( i ) - key2.GetValue( i ) ) > e ) return true ;
	}
	return false ;
}

bool compare_misc( MdxKeyFrame &key1, MdxKeyFrame &key2 )
{
	float e = g_epsilon_misc ;
	int count = key1.GetDimCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		if ( fabsf( key1.GetValue( i ) - key2.GetValue( i ) ) > e ) return true ;
	}
	return false ;
}

bool compare_cos( MdxKeyFrame &key1, MdxKeyFrame &key2 )
{
	float c = 0.0f ;
	for ( int i = 0 ; i < 4 ; i ++ ) {
		c += key1.GetValue( i ) * key2.GetValue( i ) ;
	}
	return ( c < g_epsilon_cos ) ;
}


} // namespace mdx
