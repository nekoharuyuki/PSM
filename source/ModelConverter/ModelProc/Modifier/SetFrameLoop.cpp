#include "SetFrameLoop.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "DEFAULT OFF ON" ;
enum {
	MODE_DEFAULT,
	MODE_OFF,
	MODE_ON,
	MODE_NUMERIC,
} ;

//----------------------------------------------------------------
//  SetFrameLoop
//----------------------------------------------------------------

bool SetFrameLoop::Modify( MdxBlock *block )
{
	//  check params

	int mode ;
	float start ;
	float end ;

	string arg = str_toupper( GetArg( 2, "DEFAULT" ) ) ;
	if ( str_isdigit( arg ) ) {
		mode = MODE_NUMERIC ;
		start = str_atof( str_index( arg, 0, ',' ) ) ;
		end = str_atof( str_index( arg, 1, ',' ) ) ;
	} else {
		mode = str_search( ModeNames, arg ) ;
		start = 0.0f ;
		end = 0.0f ;
		if ( mode < 0 ) {
			Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		if ( mode == MODE_DEFAULT ) return true ;
	}

	MdxBlocks motions ;
	motions.EnumTree( block, MDX_MOTION ) ;
	for ( int i = 0 ; i < motions.size() ; i ++ ) {
		MdxMotion *motion = (MdxMotion *)motions[ i ] ;
		motion->ClearChild( MDX_FRAME_LOOP ) ;
		if ( mode != MODE_OFF ) {
			if ( mode == MODE_ON ) GetFrameRange( motion, start, end ) ;
			motion->AttachChild( new MdxFrameLoop( start, end ) ) ;
		}
	}
	return true ;
}

bool SetFrameLoop::GetFrameRange( MdxMotion *motion, float &start, float &end )
{
	start = 0.0f ;
	end = 0.0f ;

	bool is_set = false ;
	MdxBlocks fcurves ;
	fcurves.EnumTree( motion, MDX_FCURVE ) ;
	for ( int i = 0 ; i < fcurves.size() ; i ++ ) {
		MdxFCurve *fcurve = (MdxFCurve *)fcurves[ i ] ;
		int n_keys = fcurve->GetKeyFrameCount() ;
		if ( n_keys > 0 ) {
			float frame = fcurve->GetKeyFrame( 0 ).GetFrame() ;
			if ( !is_set || start > frame ) start = frame ;
			frame = fcurve->GetKeyFrame( n_keys - 1 ).GetFrame() ;
			if ( !is_set || end < frame ) end = frame ;
			is_set = true ;
		}
	}
	return is_set ;
}


} // namespace mdx
