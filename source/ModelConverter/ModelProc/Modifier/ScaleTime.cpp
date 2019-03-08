#include "ScaleTime.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF" ;
enum {
	MODE_OFF,
	MODE_NUMERIC,
} ;

//----------------------------------------------------------------
//  ScaleTime
//----------------------------------------------------------------

bool ScaleTime::Modify( MdxBlock *block )
{
	//  check params

	int mode ;
	float scale ;

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	if ( str_isdigit( arg ) ) {
		mode = MODE_NUMERIC ;
		scale = str_atof( arg ) ;
		if ( scale == 1.0f ) return true ;
	} else {
		mode = str_search( ModeNames, arg ) ;
		scale = 1.0f ;
		if ( mode < 0 ) {
			Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		if ( mode == MODE_OFF ) return true ;
	}
	scale = 1.0f / scale ;

	//  process

	int i, j, k ;

	MdxBlocks blocks ;
	blocks.EnumTree( block, MDX_FRAME_LOOP ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxFrameLoop *cmd = (MdxFrameLoop *)blocks[ i ] ;
		cmd->SetStart( cmd->GetStart() * scale ) ;
		cmd->SetEnd( cmd->GetEnd() * scale ) ;
	}

	blocks.clear() ;
	blocks.EnumTree( block, MDX_FCURVE ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxFCurve *fcurve = (MdxFCurve *)blocks[ i ] ;
		for ( j = 0 ; j < fcurve->GetKeyFrameCount() ; j ++ ) {
			MdxKeyFrame &key = fcurve->GetKeyFrame( j ) ;
			key.SetFrame( key.GetFrame() * scale ) ;
			for ( k = 0 ; k < key.GetDimCount() ; k ++ ) {
				key.SetInDX( k, key.GetInDX( k ) * scale ) ;
				key.SetOutDX( k, key.GetOutDX( k ) * scale ) ;
			}
		}
	}
	return true ;
}


} // namespace mdx
