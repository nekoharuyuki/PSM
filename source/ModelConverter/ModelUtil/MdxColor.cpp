#include "ModelUtil.h"

namespace mdx {

static int clamp( int num, int min, int max )
{
	if ( num < min ) return min ;
	if ( num > max ) return max ;
	return num ;
}

//----------------------------------------------------------------
//  rgba8888
//----------------------------------------------------------------

rgba8888::rgba8888( unsigned int bits )
{
	this->bits = bits ;
}

rgba8888::rgba8888( int r, int g, int b, int a )
{
	set( r, g, b, a ) ;
}

rgba8888::rgba8888( const vec4 &v )
{
	*this = v ;
}

rgba8888::rgba8888( const rgba8880 &c )
{
	*this = c ;
}

rgba8888::rgba8888( const rgba5650 &c )
{
	*this = c ;
}

rgba8888::rgba8888( const rgba5551 &c )
{
	*this = c ;
}

rgba8888::rgba8888( const rgba5550 &c )
{
	*this = c ;
}

rgba8888::rgba8888( const rgba4444 &c )
{
	*this = c ;
}

rgba8888 &rgba8888::set( int r, int g, int b, int a )
{
	this->r = clamp( r, 0, 255 ) ;
	this->g = clamp( g, 0, 255 ) ;
	this->b = clamp( b, 0, 255 ) ;
	this->a = clamp( a, 0, 255 ) ;
	return *this ;
}

rgba8888 &rgba8888::operator=( unsigned int bits )
{
	this->bits = bits ;
	return *this ;
}

rgba8888 &rgba8888::operator=( const vec4 &v )
{
	return set( (int)( v.x * 255 + 0.5f ), (int)( v.y * 255 + 0.5f ),
		(int)( v.z * 255 + 0.5f ), (int)( v.w * 255 + 0.5f ) ) ;
}

rgba8888 &rgba8888::operator=( const rgba8880 &c )
{
	return set( c.r , c.g, c.b, 255 ) ;
}

rgba8888 &rgba8888::operator=( const rgba5650 &c )
{
	return set( c.r * 0x21 / 4, c.g * 0x41 / 16, c.b * 0x21 / 4, 255 ) ;
}

rgba8888 &rgba8888::operator=( const rgba5551 &c )
{
	return set( c.r * 0x21 / 4, c.g * 0x21 / 4, c.b * 0x21 / 4, c.a * 255 ) ;
}

rgba8888 &rgba8888::operator=( const rgba5550 &c )
{
	return set( c.r * 0x21 / 4, c.g * 0x21 / 4, c.b * 0x21 / 4, 255 ) ;
}

rgba8888 &rgba8888::operator=( const rgba4444 &c )
{
	return set( c.r * 0x11, c.g * 0x11, c.b * 0x11, c.a * 0x11 ) ;
}

rgba8888::operator unsigned int() const
{
	return bits ;
}

rgba8888 rgba8888::bgra() const
{
	return rgba8888( b, g, r, a ) ;
}

//----------------------------------------------------------------
//  rgba8880
//----------------------------------------------------------------

rgba8880::rgba8880( unsigned int bits )
{
	r = bits ;
	g = bits >> 8 ;
	b = bits >> 16 ;
}

rgba8880::rgba8880( int r, int g, int b, int a )
{
	set( r, g, b, a ) ;
}

rgba8880::rgba8880( const rgba8888 &c )
{
	*this = c ;
}

rgba8880 &rgba8880::set( int r, int g, int b, int a )
{
	this->r = clamp( r, 0, 255 ) ;
	this->g = clamp( g, 0, 255 ) ;
	this->b = clamp( b, 0, 255 ) ;
	return *this ;
}

rgba8880 &rgba8880::operator=( unsigned int bits )
{
	r = bits ;
	g = bits >> 8 ;
	b = bits >> 16 ;
	return *this ;
}

rgba8880 &rgba8880::operator=( const rgba8888 &c )
{
	return set( c.r , c.g, c.b, 255 ) ;
}

rgba8880::operator unsigned int() const
{
	return r | ( g << 8 ) | ( b << 16 ) ;
}

rgba8880 rgba8880::bgra() const
{
	return rgba8880( b, g, r, 255 ) ;
}

//----------------------------------------------------------------
//  rgba5650
//----------------------------------------------------------------

rgba5650::rgba5650( unsigned short bits )
{
	this->bits = bits ;
}

rgba5650::rgba5650( int r, int g, int b, int a )
{
	set( r, g, b, a ) ;
}

rgba5650::rgba5650( const rgba8888 &c )
{
	*this = c ;
}

rgba5650 &rgba5650::set( int r, int g, int b, int a )
{
	this->r = clamp( r, 0, 31 ) ;
	this->g = clamp( g, 0, 63 ) ;
	this->b = clamp( b, 0, 31 ) ;
	return *this ;
}

rgba5650 &rgba5650::operator=( unsigned short bits )
{
	this->bits = bits ;
	return *this ;
}

rgba5650 &rgba5650::operator=( const rgba8888 &c )
{
	return set( c.r / 8, c.g / 4, c.b / 8, 255 ) ;
}

rgba5650::operator unsigned short() const
{
	return bits ;
}

rgba5650 rgba5650::bgra() const
{
	rgba5650 tmp ;
	tmp.r = b ;
	tmp.g = g ;
	tmp.b = r ;
	return tmp ;
}

//----------------------------------------------------------------
//  rgba5551
//----------------------------------------------------------------

rgba5551::rgba5551( unsigned short bits )
{
	this->bits = bits ;
}

rgba5551::rgba5551( int r, int g, int b, int a )
{
	set( r, g, b, a ) ;
}

rgba5551::rgba5551( const rgba8888 &c )
{
	*this = c ;
}

rgba5551::rgba5551( const rgba5550 &c )
{
	*this = c ;
}

rgba5551 &rgba5551::set( int r, int g, int b, int a )
{
	this->r = clamp( r, 0, 31 ) ;
	this->g = clamp( g, 0, 31 ) ;
	this->b = clamp( b, 0, 31 ) ;
	this->a = clamp( a, 0, 1 ) ;
	return *this ;
}

rgba5551 &rgba5551::operator=( unsigned short bits )
{
	this->bits = bits ;
	return *this ;
}

rgba5551 &rgba5551::operator=( const rgba8888 &c )
{
	return set( c.r / 8, c.g / 8, c.b / 8, c.a / 128 ) ;
}

rgba5551 &rgba5551::operator=( const rgba5550 &c )
{
	bits = c.bits | 0x8000 ;
	return *this ;
}

rgba5551::operator unsigned short() const
{
	return bits ;
}

rgba5551 rgba5551::bgra() const
{
	rgba5551 tmp ;
	tmp.r = b ;
	tmp.g = g ;
	tmp.b = r ;
	tmp.a = a ;
	return tmp ;
}

//----------------------------------------------------------------
//  rgba5550
//----------------------------------------------------------------

rgba5550::rgba5550( unsigned short bits )
{
	this->bits = bits ;
}

rgba5550::rgba5550( int r, int g, int b, int a )
{
	set( r, g, b, a ) ;
}

rgba5550::rgba5550( const rgba8888 &c )
{
	*this = c ;
}

rgba5550::rgba5550( const rgba5551 &c )
{
	*this = c ;
}

rgba5550 &rgba5550::set( int r, int g, int b, int a )
{
	this->r = clamp( r, 0, 31 ) ;
	this->g = clamp( g, 0, 31 ) ;
	this->b = clamp( b, 0, 31 ) ;
	return *this ;
}

rgba5550 &rgba5550::operator=( unsigned short bits )
{
	this->bits = bits & 0x7fff ;
	return *this ;
}

rgba5550 &rgba5550::operator=( const rgba8888 &c )
{
	return set( c.r / 8, c.g / 8, c.b / 8, 0 ) ;
}

rgba5550 &rgba5550::operator=( const rgba5551 &c )
{
	bits = c.bits & 0x7fff ;
	return *this ;
}

rgba5550::operator unsigned short() const
{
	return bits ;
}

rgba5550 rgba5550::bgra() const
{
	return rgba5550( b, g, r ) ;
}

//----------------------------------------------------------------
//  rgba4444
//----------------------------------------------------------------

rgba4444::rgba4444( unsigned short bits )
{
	this->bits = bits ;
}

rgba4444::rgba4444( int r, int g, int b, int a )
{
	set( r, g, b, a ) ;
}

rgba4444::rgba4444( const rgba8888 &c )
{
	*this = c ;
}

rgba4444 &rgba4444::set( int r, int g, int b, int a )
{
	this->r = clamp( r, 0, 15 ) ;
	this->g = clamp( g, 0, 15 ) ;
	this->b = clamp( b, 0, 15 ) ;
	this->a = clamp( a, 0, 15 ) ;
	return *this ;
}

rgba4444 &rgba4444::operator=( unsigned short bits )
{
	this->bits = bits ;
	return *this ;
}

rgba4444 &rgba4444::operator=( const rgba8888 &c )
{
	return set( c.r / 16, c.g / 16, c.b / 16, c.a / 16 ) ;
}

rgba4444::operator unsigned short() const
{
	return bits ;
}

rgba4444 rgba4444::bgra() const
{
	rgba4444 tmp ;
	tmp.r = b ;
	tmp.g = g ;
	tmp.b = r ;
	tmp.a = a ;
	return tmp ;
}


} // namespace mdx
