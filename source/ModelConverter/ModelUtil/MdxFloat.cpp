#include "ModelUtil.h"

namespace mdx {

static int round( float f )
{
	return (int)floorf( f + 0.5f ) ;
}

//----------------------------------------------------------------
//  float32
//----------------------------------------------------------------

float32::float32()
{
	this->bits = 0 ;
}

float32::float32( float val )
{
	this->val = val ;
}

float32 &float32::operator=( float val )
{
	this->val = val ;
	return *this ;
}

float32::operator float() const
{
	return this->val ;
}

//----------------------------------------------------------------
//  float16
//----------------------------------------------------------------

float16::float16()
{
	this->bits = 0 ;
}

float16::float16( float val )
{
	*this = val ;
}

float16 &float16::operator=( float val )
{
	float32 f32 ;
	f32.val = val ;
	int e = f32.e - ( 127 - 15 ) ;
	int f = f32.f >> 13 ;
	if ( e < 0 ) {
		e = 0 ;
		f = 0 ;
	}
	if ( e > 31 ) {
		e = 31 ;
		f = 0x3ff ;
	}
	this->f = f ;
	this->e = e ;
	this->s = f32.s ;
	return *this ;
}

float16::operator float() const
{
	float32 f32 ;
	int e = this->e ;
	if ( ( this->bits & 0x7fff ) != 0 ) {
		e += ( 127 - 15 ) ;
	}
	f32.f = this->f << 13 ;
	f32.e = e ;
	f32.s = this->s ;
	return f32.val ;
}

//----------------------------------------------------------------
//  fixed16		signed short / 32768
//----------------------------------------------------------------

fixed16::fixed16()
{
	this->bits = 0 ;
}

fixed16::fixed16( float val )
{
	*this = val ;
}

fixed16 &fixed16::operator=( float val )
{
	val *= 32768.0f ;
	if ( val < -32768.0f ) val = -32768.0f ;
	if ( val > 32767.0f ) val = 32767.0f ;
	this->bits = round( val ) ;
	return *this ;
}

fixed16::operator float() const
{
	return (float)( this->bits ) / 32768.0f ;
}

//----------------------------------------------------------------
//  ufixed16		unsigned short / 32768
//----------------------------------------------------------------

ufixed16::ufixed16()
{
	this->bits = 0 ;
}

ufixed16::ufixed16( float val )
{
	*this = val ;
}

ufixed16 &ufixed16::operator=( float val )
{
	val *= 32768.0f ;
	if ( val < 0.0f ) val = 0.0f ;
	if ( val > 65535.0f ) val = 65535.0f ;
	this->bits = round( val ) ;
	return *this ;
}

ufixed16::operator float() const
{
	return (float)( this->bits ) / 32768.0f ;
}

//----------------------------------------------------------------
//  fixed8		signed char / 128
//----------------------------------------------------------------

fixed8::fixed8()
{
	this->bits = 0 ;
}

fixed8::fixed8( float val )
{
	*this = val ;
}

fixed8 &fixed8::operator=( float val )
{
	val *= 128.0f ;
	if ( val < -128.0f ) val = -128.0f ;
	if ( val > 127.0f ) val = 127.0f ;
	this->bits = round( val ) ;
	return *this ;
}

fixed8::operator float() const
{
	return (float)( this->bits ) / 128.0f ;
}

//----------------------------------------------------------------
//  ufixed8		unsigned char / 128
//----------------------------------------------------------------

ufixed8::ufixed8()
{
	this->bits = 0 ;
}

ufixed8::ufixed8( float val )
{
	*this = val ;
}

ufixed8 &ufixed8::operator=( float val )
{
	val *= 128.0f ;
	if ( val < 0.0f ) val = 0.0f ;
	if ( val > 255.0f ) val = 255.0f ;
	this->bits = round( val ) ;
	return *this ;
}

ufixed8::operator float() const
{
	return (float)( this->bits ) / 128.0f ;
}

//----------------------------------------------------------------
//  fixed16n		signed short / 32767
//----------------------------------------------------------------

fixed16n::fixed16n()
{
	this->bits = 0 ;
}

fixed16n::fixed16n( float val )
{
	*this = val ;
}

fixed16n &fixed16n::operator=( float val )
{
	val *= 32767.0f ;
	if ( val < -32768.0f ) val = -32768.0f ;
	if ( val > 32767.0f ) val = 32767.0f ;
	this->bits = round( val ) ;
	return *this ;
}

fixed16n::operator float() const
{
	return (float)( this->bits ) / 32767.0f ;
}

//----------------------------------------------------------------
//  ufixed16n		unsigned short / 65535
//----------------------------------------------------------------

ufixed16n::ufixed16n()
{
	this->bits = 0 ;
}

ufixed16n::ufixed16n( float val )
{
	*this = val ;
}

ufixed16n &ufixed16n::operator=( float val )
{
	val *= 65535.0f ;
	if ( val < 0.0f ) val = 0.0f ;
	if ( val > 65535.0f ) val = 65535.0f ;
	this->bits = round( val ) ;
	return *this ;
}

ufixed16n::operator float() const
{
	return (float)( this->bits ) / 65535.0f ;
}

//----------------------------------------------------------------
//  fixed8n		signed char / 127
//----------------------------------------------------------------

fixed8n::fixed8n()
{
	this->bits = 0 ;
}

fixed8n::fixed8n( float val )
{
	*this = val ;
}

fixed8n &fixed8n::operator=( float val )
{
	val *= 127.0f ;
	if ( val < -128.0f ) val = -128.0f ;
	if ( val > 127.0f ) val = 127.0f ;
	this->bits = round( val ) ;
	return *this ;
}

fixed8n::operator float() const
{
	return (float)( this->bits ) / 127.0f ;
}

//----------------------------------------------------------------
//  ufixed8n		unsigned char / 255
//----------------------------------------------------------------

ufixed8n::ufixed8n()
{
	this->bits = 0 ;
}

ufixed8n::ufixed8n( float val )
{
	*this = val ;
}

ufixed8n &ufixed8n::operator=( float val )
{
	val *= 255.0f ;
	if ( val < 0.0f ) val = 0.0f ;
	if ( val > 255.0f ) val = 255.0f ;
	this->bits = round( val ) ;
	return *this ;
}

ufixed8n::operator float() const
{
	return (float)( this->bits ) / 255.0f ;
}


} // namespace mdx
