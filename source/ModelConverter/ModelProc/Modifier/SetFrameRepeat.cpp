#include "SetFrameRepeat.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "DEFAULT OFF ON HOLD CYCLE" ;
enum {
	MODE_DEFAULT,
	MODE_OFF,
	MODE_ON,
	MODE_HOLD,
	MODE_CYCLE,
} ;

//----------------------------------------------------------------
//  SetFrameRepeat
//----------------------------------------------------------------

bool SetFrameRepeat::Modify( MdxBlock *block )
{
	//  check params

	string arg = str_toupper( GetArg( 2, "DEFAULT" ) ) ;
	int mode = str_search( ModeNames, arg ) ;
	if ( mode < 0 ) {
		Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
		return false ;
	}
	if ( mode == MODE_DEFAULT ) return true ;

	//  process

	int repeat = ( mode == MODE_HOLD ) ? MDX_REPEAT_HOLD : MDX_REPEAT_CYCLE ;

	MdxBlocks motions ;
	motions.EnumTree( block, MDX_MOTION ) ;
	for ( int i = 0 ; i < motions.size() ; i ++ ) {
		MdxMotion *motion = (MdxMotion *)motions[ i ] ;
		motion->ClearChild( MDX_FRAME_REPEAT ) ;
		if ( mode != MODE_OFF ) {
			motion->AttachChild( new MdxFrameRepeat( repeat ) ) ;
		}
	}
	return true ;
}


} // namespace mdx
