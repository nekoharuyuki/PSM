#include "CompFCurveChannel.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  CompFCurveChannel
//----------------------------------------------------------------

bool CompFCurveChannel::Modify( MdxBlock *block )
{
	//  check params

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	int mode = str_search( ModeNames, arg ) ;
	if ( mode < 0 ) {
		Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
		return false ;
	}
	if ( mode == MODE_OFF ) return true ;

	//  process

	MdxBlocks motions ;
	motions.EnumTree( block, MDX_MOTION ) ;
	for ( int i = 0 ; i < motions.size() ; i ++ ) {
		ModifyMotion( (MdxMotion *)motions[ i ] ) ;
	}

	m_channels.clear() ;
	return true ;
}

void CompFCurveChannel::ModifyMotion( MdxMotion *motion )
{
	bool merged = false ;

	MdxBlocks cmds ;
	cmds.EnumChild( motion, MDX_ANIMATE ) ;
	for ( int i = 0 ; i < cmds.size() ; i ++ ) {
		MdxAnimate *anim = (MdxAnimate *)cmds[ i ] ;
		if ( anim == 0 ) continue ;
		if ( !CheckTarget( anim ) ) continue ;
		if ( !UnifyAnims( anim, cmds, i ) ) continue ;
		if ( !MergeFCurves( anim ) ) continue ;
		merged = true ;
	}

	if ( merged ) CleanFCurves( motion ) ;
}

bool CompFCurveChannel::CheckTarget( MdxAnimate *anim )
{
	MdxBlock *cmd = 0 ;
	MdxBlock *block = anim->GetBlockRef() ;
	if ( block != 0 ) cmd = (MdxBlock *)block->FindTree( anim->GetCommand() ) ;
	if ( cmd == 0 ) cmd = (MdxBlock *)m_format->GetTemplate( anim->GetCommand() ) ;
	if ( cmd == 0 ) return false ;

	int count = cmd->GetArgsCount() ;
	m_channels.clear() ;
	m_channels.resize( count ) ;
	for ( int i = 0 ; i < count ; i ++ ) {
		m_channels[ i ].value = cmd->GetArgsFloat( i ) ;
	}
	return true ;
}

bool CompFCurveChannel::UnifyAnims( MdxAnimate *anim, MdxBlocks &cmds, int num )
{
	int max_dims = 0 ;

	for ( int i = num ; i < cmds.size() ; i ++ ) {
		MdxAnimate *anim2 = (MdxAnimate *)cmds[ i ] ;
		if ( anim2 == 0 ) continue ;
		if ( anim2->GetScope() != anim->GetScope() ) continue ;
		if ( strcmp( anim2->GetBlock(), anim->GetBlock() ) ) continue ;
		if ( anim2->GetCommand() != anim->GetCommand() ) continue ;
		if ( anim2->GetIndex() != anim->GetIndex() ) continue ;

		MdxFCurve *fcurve = anim2->GetFCurveRef() ;
		if ( fcurve != 0 ) {
			int offset = anim2->GetOffset() ;
			int n_dims = fcurve->GetDimCount() ;
			if ( offset + n_dims > (int)m_channels.size() ) {
				m_channels.resize( offset + n_dims ) ;
			}
			for ( int j = 0 ; j < n_dims ; j ++ ) {
				m_channels[ offset + j ].fcurve = fcurve ;
				m_channels[ offset + j ].offset = j ;
			}
			if ( n_dims > max_dims ) max_dims = n_dims ;
		}

		if ( anim2 != anim ) {
			anim2->Release() ;
			cmds[ i ] = 0 ;
		}
	}
	anim->SetOffset( 0 ) ;
	return !( max_dims == (int)m_channels.size() ) ;
}

bool CompFCurveChannel::MergeFCurves( MdxAnimate *anim )
{
	int i, j ;

	//  check fcurves

	int interp = MDX_FCURVE_CONSTANT ;
	int extrap = MDX_FCURVE_HOLD ;
	vector<float> frames ;
	for ( i = 0 ; i < (int)m_channels.size() ; i ++ ) {
		MdxFCurve *fcurve = m_channels[ i ].fcurve ;
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

	MdxFCurve *fcurve2 = new MdxFCurve ;
	anim->GetParent()->AttachChild( fcurve2 ) ;
	anim->SetFCurveRef( fcurve2 ) ;
	// fcurve2->SetFormat( interp | extrap ) ;
	fcurve2->SetInterp( interp ) ;
	fcurve2->SetExtrap( extrap ) ;
	fcurve2->SetDimCount( m_channels.size() ) ;
	fcurve2->SetKeyFrameCount( frames.size() ) ;
	for ( i = 0 ; i < (int)frames.size() ; i ++ ) {
		fcurve2->GetKeyFrame( i ).SetFrame( frames[ i ] ) ;
	}

	//  merge fcurves

	for ( i = 0 ; i < (int)m_channels.size() ; i ++ ) {
		MdxFCurve *fcurve = m_channels[ i ].fcurve ;
		if ( fcurve == 0 ) {
			float value = m_channels[ i ].value ;
			for ( j = 0 ; j < fcurve2->GetKeyFrameCount() ; j ++ ) {
				MdxKeyFrame &key2 = fcurve2->GetKeyFrame( j ) ;
				key2.SetValue( i, value ) ;
				if ( j > 0 ) {
					MdxKeyFrame &key1 = fcurve2->GetKeyFrame( j - 1 ) ;
					float dx = ( key2.GetFrame() - key1.GetFrame() ) / 3.0f ;
					key1.SetOutDX( i, dx ) ;
					key2.SetInDX( i, -dx ) ;
				}
			}
		} else {
			MdxFCurve tmp = *fcurve ;
			for ( j = 0 ; j < fcurve2->GetKeyFrameCount() ; j ++ ) {
				float frame = fcurve2->GetKeyFrame( j ).GetFrame() ;
				if ( tmp.IndexOfKeyFrame( frame ) >= 0 ) continue ;
				MdxKeyFrame key ;
				int index = tmp.Eval( frame, key, true ) ;
				tmp.InsertKeyFrame( index + 1, key, true ) ;
			}
			tmp.SetFormat( fcurve2->GetFormat(), true ) ;

			int offset = m_channels[ i ].offset ;
			for ( j = 0 ; j < fcurve2->GetKeyFrameCount() ; j ++ ) {
				MdxKeyFrame &key = tmp.GetKeyFrame( j ) ;
				MdxKeyFrame &key2 = fcurve2->GetKeyFrame( j ) ;
				key2.SetValue( i, key.GetValue( offset ) ) ;
				key2.SetInDX( i, key.GetInDX( offset ) ) ;
				key2.SetInDY( i, key.GetInDY( offset ) ) ;
				key2.SetOutDX( i, key.GetOutDX( offset ) ) ;
				key2.SetOutDY( i, key.GetOutDY( offset ) ) ;
			}
		}
	}
	return true ;
}

void CompFCurveChannel::CleanFCurves( MdxMotion *motion )
{
	MdxBlocks fcurves ;
	fcurves.EnumChild( motion, MDX_FCURVE ) ;
	for ( int i = 0 ; i < fcurves.size() ; i ++ ) {
		MdxFCurve *fcurve = (MdxFCurve *)fcurves[ i ] ;
		if ( fcurve->FindReferrer() == 0 ) fcurve->Release() ;
	}
}


} // namespace mdx
