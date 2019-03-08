#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  Motion block
//----------------------------------------------------------------

MdxMotion::MdxMotion( const char *name )
{
	SetName( name ) ;
}

MdxMotion::~MdxMotion()
{
	;
}

MdxMotion::MdxMotion( const MdxMotion &src )
{
	Copy( &src ) ;
}

MdxMotion &MdxMotion::operator=( const MdxMotion &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxMotion::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxMotion::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  FrameLoop command
//----------------------------------------------------------------

MdxFrameLoop::MdxFrameLoop( float start, float end )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;

	SetStart( start ) ;
	SetEnd( end ) ;
}

void MdxFrameLoop::SetStart( float start )
{
	SetArgsFloat( 0, start ) ;
}

void MdxFrameLoop::SetEnd( float end )
{
	SetArgsFloat( 1, end ) ;
}

float MdxFrameLoop::GetStart() const
{
	return GetArgsFloat( 0 ) ;
}

float MdxFrameLoop::GetEnd() const
{
	return GetArgsFloat( 1 ) ;
}

//----------------------------------------------------------------
//  FrameRate command
//----------------------------------------------------------------

MdxFrameRate::MdxFrameRate( float fps )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetFPS( fps ) ;
}

void MdxFrameRate::SetFPS( float fps )
{
	SetArgsFloat( 0, fps ) ;
}

float MdxFrameRate::GetFPS() const
{
	return GetArgsFloat( 0 ) ;
}

//----------------------------------------------------------------
//  FrameRepeat command
//----------------------------------------------------------------

MdxFrameRepeat::MdxFrameRepeat( int mode )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_REPEAT_MODE ) ;

	SetMode( mode ) ;
}

void MdxFrameRepeat::SetMode( int mode )
{
	SetArgsInt( 0, mode ) ;
}

int MdxFrameRepeat::GetMode() const
{
	return GetArgsInt( 0 ) ;
}

//----------------------------------------------------------------
//  Animate command
//----------------------------------------------------------------

MdxAnimate::MdxAnimate( int scope, const char *block, int command, int mode, const char *fcurve )
{
	SetArgsDesc( 0, MDX_WORD_REF | scope | MDX_WORD_VAR_SCOPE ) ;
	SetArgsDesc( 1, MDX_WORD_ENUM | MDX_SCOPE_COMMAND_TYPE ) ;
	SetArgsDesc( 2, MDX_WORD_ENUM | MDX_SCOPE_ANIM_MODE ) ;
	SetArgsDesc( 3, MDX_WORD_REF | MDX_FCURVE ) ;

	SetScope( scope ) ;
	SetBlock( block ) ;
	SetCommand( command ) ;
	SetMode( mode ) ;
	SetFCurve( fcurve ) ;
}

void MdxAnimate::SetScope( int scope )
{
	SetArgsDesc( 0, MDX_WORD_REF | scope | MDX_WORD_VAR_SCOPE ) ;
}

void MdxAnimate::SetBlock( const char *block )
{
	SetArgsString( 0, block ) ;
}

void MdxAnimate::SetCommand( int cmd )
{
	SetArgsInt( 1, cmd ) ;
}

void MdxAnimate::SetMode( int mode )
{
	SetArgsInt( 2, mode ) ;
}

void MdxAnimate::SetFCurve( const char *fcurve )
{
	SetArgsString( 3, fcurve ) ;
}

int MdxAnimate::GetScope() const
{
	return MDX_WORD_SCOPE( GetArgsDesc( 0 ) ) ;
}

const char *MdxAnimate::GetBlock() const
{
	return GetArgsString( 0 ) ;
}

int MdxAnimate::GetCommand() const
{
	return GetArgsInt( 1 ) ;
}

int MdxAnimate::GetMode() const
{
	return GetArgsInt( 2 ) ;
}

const char *MdxAnimate::GetFCurve() const
{
	return GetArgsString( 3 ) ;
}

void MdxAnimate::SetBlockRef( const MdxBlock *block )
{
	SetArgsRef( 0, block ) ;
}

MdxBlock *MdxAnimate::GetBlockRef() const
{
	return (MdxBlock *)GetArgsRef( 0 ) ;
}

void MdxAnimate::SetFCurveRef( const MdxFCurve *fcurve )
{
	SetArgsRef( 3, fcurve ) ;
}

MdxFCurve *MdxAnimate::GetFCurveRef() const
{
	return (MdxFCurve *)GetArgsRef( 3 ) ;
}

void MdxAnimate::SetIndex( int index )
{
	int offset = MDX_ANIM_OFFSET( GetMode() ) ;
	SetMode( MDX_ANIM_MODE( index, offset ) ) ;
}

int MdxAnimate::GetIndex() const
{
	return MDX_ANIM_INDEX( GetMode() ) ;
}

void MdxAnimate::SetOffset( int offset )
{
	int index = MDX_ANIM_INDEX( GetMode() ) ;
	SetMode( MDX_ANIM_MODE( index, offset ) ) ;
}

int MdxAnimate::GetOffset() const
{
	return MDX_ANIM_OFFSET( GetMode() ) ;
}


} // namespace mdx
