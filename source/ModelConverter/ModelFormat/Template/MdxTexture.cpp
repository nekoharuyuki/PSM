#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  Texture block
//----------------------------------------------------------------

MdxTexture::MdxTexture( const char *name )
{
	SetName( name ) ;
}

MdxTexture::~MdxTexture()
{
	;
}

MdxTexture::MdxTexture( const MdxTexture &src )
{
	Copy( &src ) ;
}

MdxTexture &MdxTexture::operator=( const MdxTexture &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxTexture::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxTexture::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  TexType command
//----------------------------------------------------------------

MdxTexType::MdxTexType( int type )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_TEXTURE_TYPE ) ;

	SetType( type ) ;
}

void MdxTexType::SetType( int type )
{
	SetArgsInt( 0, type ) ;
}

int MdxTexType::GetType() const
{
	return GetArgsInt( 0 ) ;
}

//----------------------------------------------------------------
//  TexFilter command
//----------------------------------------------------------------

MdxTexFilter::MdxTexFilter( int mag, int min )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_FILTER_MAG ) ;
	SetArgsDesc( 1, MDX_WORD_ENUM | MDX_SCOPE_FILTER_MIN ) ;

	SetMag( mag ) ;
	SetMin( min ) ;
}

void MdxTexFilter::SetMag( int mag )
{
	SetArgsInt( 0, mag ) ;
}

void MdxTexFilter::SetMin( int min )
{
	SetArgsInt( 1, min ) ;
}

int MdxTexFilter::GetMag() const
{
	return GetArgsInt( 0 ) ;
}

int MdxTexFilter::GetMin() const
{
	return GetArgsInt( 1 ) ;
}

//----------------------------------------------------------------
//  TexWrap command
//----------------------------------------------------------------

MdxTexWrap::MdxTexWrap( int wrap_u, int wrap_v )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_WRAP_U ) ;
	SetArgsDesc( 1, MDX_WORD_ENUM | MDX_SCOPE_WRAP_V ) ;

	SetWrapU( wrap_u ) ;
	SetWrapV( wrap_v ) ;
}

void MdxTexWrap::SetWrapU( int wrap_u )
{
	SetArgsInt( 0, wrap_u ) ;
}

void MdxTexWrap::SetWrapV( int wrap_v )
{
	SetArgsInt( 1, wrap_v ) ;
}

int MdxTexWrap::GetWrapU() const
{
	return GetArgsInt( 0 ) ;
}

int MdxTexWrap::GetWrapV() const
{
	return GetArgsInt( 1 ) ;
}

//----------------------------------------------------------------
//  TexCrop command
//----------------------------------------------------------------

MdxTexCrop::MdxTexCrop( const rect &crop )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 3, MDX_WORD_FLOAT ) ;

	SetCrop( crop ) ;
}

void MdxTexCrop::SetCrop( const rect &crop )
{
	SetArgsRect( 0, crop ) ;
}

rect MdxTexCrop::GetCrop() const
{
	return GetArgsRect( 0 ) ;
}

//----------------------------------------------------------------
//  UVPivot command
//----------------------------------------------------------------

MdxUVPivot::MdxUVPivot( const vec2 &pivot )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;

	SetPivot( pivot ) ;
}

void MdxUVPivot::SetPivot( const vec2 &pivot )
{
	SetArgsVec2( 0, pivot ) ;
}

vec2 MdxUVPivot::GetPivot() const
{
	return GetArgsVec2( 0 ) ;
}

//----------------------------------------------------------------
//  UVTranslate command
//----------------------------------------------------------------

MdxUVTranslate::MdxUVTranslate( const vec2 &translate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;

	SetTranslate( translate ) ;
}

void MdxUVTranslate::SetTranslate( const vec2 &translate )
{
	SetArgsVec2( 0, translate ) ;
}

vec2 MdxUVTranslate::GetTranslate() const
{
	return GetArgsVec2( 0 ) ;
}

//----------------------------------------------------------------
//  UVRotate command
//----------------------------------------------------------------

MdxUVRotate::MdxUVRotate( float rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxUVRotate::SetRotate( float rotate )
{
	SetArgsFloat( 0, rotate ) ;
}

float MdxUVRotate::GetRotate() const
{
	return GetArgsFloat( 0 ) ;
}

//----------------------------------------------------------------
//  UVScale command
//----------------------------------------------------------------

MdxUVScale::MdxUVScale( const vec2 &scale )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;

	SetScale( scale ) ;
}

void MdxUVScale::SetScale( const vec2 &scale )
{
	SetArgsVec2( 0, scale ) ;
}

vec2 MdxUVScale::GetScale() const
{
	return GetArgsVec2( 0 ) ;
}


} // namespace mdx
