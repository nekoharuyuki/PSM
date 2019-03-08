#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  MdxMaterial
//----------------------------------------------------------------

MdxMaterial::MdxMaterial( const char *name )
{
	SetName( name ) ;
}

MdxMaterial::~MdxMaterial()
{
	;
}

MdxMaterial::MdxMaterial( const MdxMaterial &src )
{
	Copy( &src ) ;
}

MdxMaterial &MdxMaterial::operator=( const MdxMaterial &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxMaterial::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxMaterial::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  MdxLayer
//----------------------------------------------------------------

MdxLayer::MdxLayer( const char *name )
{
	SetName( name ) ;
}

MdxLayer::~MdxLayer()
{
	;
}

MdxLayer::MdxLayer( const MdxLayer &src )
{
	Copy( &src ) ;
}

MdxLayer &MdxLayer::operator=( const MdxLayer &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxLayer::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxLayer::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  Base color command
//----------------------------------------------------------------

MdxColorCommand::MdxColorCommand( const vec3 &color )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetColor( color ) ;
}

void MdxColorCommand::SetColor( const vec3 &color )
{
	SetArgsVec3( 0, color ) ;
}

vec3 MdxColorCommand::GetColor() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  Diffuse command
//----------------------------------------------------------------

MdxDiffuse::MdxDiffuse( const vec3 &color ) : MdxColorCommand( color )
{
	;
}

//----------------------------------------------------------------
//  Ambient command
//----------------------------------------------------------------

MdxAmbient::MdxAmbient( const vec3 &color ) : MdxColorCommand( color )
{
	;
}

//----------------------------------------------------------------
//  Specular command
//----------------------------------------------------------------

MdxSpecular::MdxSpecular( const vec3 &color ) : MdxColorCommand( color )
{
	;
}

//----------------------------------------------------------------
//  Emission command
//----------------------------------------------------------------

MdxEmission::MdxEmission( const vec3 &color ) : MdxColorCommand( color )
{
	;
}

//----------------------------------------------------------------
//  Reflection command
//----------------------------------------------------------------

MdxReflection::MdxReflection( const vec3 &color ) : MdxColorCommand( color )
{
	;
}

//----------------------------------------------------------------
//  Refraction command
//----------------------------------------------------------------

MdxRefraction::MdxRefraction( const vec3 &color ) : MdxColorCommand( color )
{
	;
}

//----------------------------------------------------------------
//  Bump command
//----------------------------------------------------------------

MdxBump::MdxBump( float bump )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetBump( bump ) ;
}

void MdxBump::SetBump( float bump )
{
	SetArgsFloat( 0, bump ) ;
}

float MdxBump::GetBump() const
{
	return GetArgsFloat( 0 ) ;
}

//----------------------------------------------------------------
//  Opacity command
//----------------------------------------------------------------

MdxOpacity::MdxOpacity( float bump )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetOpacity( bump ) ;
}

void MdxOpacity::SetOpacity( float bump )
{
	SetArgsFloat( 0, bump ) ;
}

float MdxOpacity::GetOpacity() const
{
	return GetArgsFloat( 0 ) ;
}

//----------------------------------------------------------------
//  Shininess command
//----------------------------------------------------------------

MdxShininess::MdxShininess( float bump )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetShininess( bump ) ;
}

void MdxShininess::SetShininess( float bump )
{
	SetArgsFloat( 0, bump ) ;
}

float MdxShininess::GetShininess() const
{
	return GetArgsFloat( 0 ) ;
}

//----------------------------------------------------------------
//  SetTexture command
//----------------------------------------------------------------

MdxSetTexture::MdxSetTexture( const char *texture )
{
	SetArgsDesc( 0, MDX_WORD_REF | MDX_TEXTURE ) ;

	SetTexture( texture ) ;
}

void MdxSetTexture::SetTexture( const char *texture )
{
	SetArgsString( 0, texture ) ;
}

const char *MdxSetTexture::GetTexture() const
{
	return GetArgsString( 0 ) ;
}

void MdxSetTexture::SetTextureRef( const MdxTexture *texture )
{
	SetArgsRef( 0, texture ) ;
}

MdxTexture *MdxSetTexture::GetTextureRef() const
{
	return (MdxTexture *)GetArgsRef( 0 ) ;
}

//----------------------------------------------------------------
//  MapType command
//----------------------------------------------------------------

MdxMapType::MdxMapType( int type )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_COMMAND_TYPE ) ;

	SetType( type ) ;
}

void MdxMapType::SetType( int type )
{
	SetArgsInt( 0, type ) ;
}

int MdxMapType::GetType() const
{
	return GetArgsInt( 0 ) ;
}

//----------------------------------------------------------------
//  MapCoord command
//----------------------------------------------------------------

MdxMapCoord::MdxMapCoord( int coord )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_ARRAYS_FORMAT ) ;

	SetCoord( coord ) ;
}

void MdxMapCoord::SetCoord( int coord )
{
	SetArgsInt( 0, coord ) ;
}

int MdxMapCoord::GetCoord() const
{
	return GetArgsInt( 0 ) ;
}

//----------------------------------------------------------------
//  MapFactor command
//----------------------------------------------------------------

MdxMapFactor::MdxMapFactor( float factor )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetFactor( factor ) ;
}

void MdxMapFactor::SetFactor( float factor )
{
	SetArgsFloat( 0, factor ) ;
}

float MdxMapFactor::GetFactor() const
{
	return GetArgsFloat( 0 ) ;
}


} // namespace mdx
