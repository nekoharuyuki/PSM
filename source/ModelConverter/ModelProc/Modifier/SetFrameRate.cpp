#include "SetFrameRate.h"

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
//  SetFrameRate
//----------------------------------------------------------------

bool SetFrameRate::Modify( MdxBlock *block )
{
	//  check params

	int mode ;
	float fps ;

	string arg = str_toupper( GetArg( 2, "DEFAULT" ) ) ;
	if ( str_isdigit( arg ) ) {
		mode = MODE_NUMERIC ;
		fps = str_atof( arg ) ;
	} else {
		mode = str_search( ModeNames, arg ) ;
		fps = 60.0f ;
		if ( mode < 0 ) {
			Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		if ( mode == MODE_DEFAULT ) return true ;
	}

	//  process

	MdxBlocks motions ;
	motions.EnumTree( block, MDX_MOTION ) ;
	for ( int i = 0 ; i < motions.size() ; i ++ ) {
		MdxMotion *motion = (MdxMotion *)motions[ i ] ;
		motion->ClearChild( MDX_FRAME_RATE ) ;
		if ( mode != MODE_OFF ) {
			motion->AttachChild( new MdxFrameRate( fps ) ) ;
		}
	}
	return true ;
}


} // namespace mdx
