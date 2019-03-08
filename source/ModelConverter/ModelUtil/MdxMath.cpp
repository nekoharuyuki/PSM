#include "ModelUtil.h"

namespace mdx {

//----------------------------------------------------------------
//  constants
//----------------------------------------------------------------

const vec2 vec2_zero = 0.0f ;
const vec2 vec2_one = 1.0f ;
const vec3 vec3_zero = 0.0f ;
const vec3 vec3_one = 1.0f ;
const vec4 vec4_zero = 0.0f ;
const vec4 vec4_one = 1.0f ;
const mat4 mat4_zero = 0.0f ;
const mat4 mat4_one = 1.0f ;
const quat quat_zero = 0.0f ;
const quat quat_one = 1.0f ;
const rect rect_zero = 0.0f ;
const rect rect_one = 1.0f ;

//----------------------------------------------------------------
//  vec2
//----------------------------------------------------------------

vec2::vec2( float f )
{
	set( f, f ) ;
}

vec2::vec2( float x, float y )
{
	set( x, y ) ;
}

vec2::vec2( const vec3 &v )
{
	set( v.x, v.y ) ;
}

vec2::vec2( const vec4 &v )
{
	set( v.x, v.y ) ;
}

vec2 &vec2::set( float x, float y )
{
	this->x = x ;
	this->y = y ;
	return *this ;
}

vec2 &vec2::operator=( float f )
{
	return set( f, f ) ;
}

vec2 &vec2::operator=( const vec3 &v )
{
	return set( v.x, v.y ) ;
}

vec2 &vec2::operator=( const vec4 &v )
{
	return set( v.x, v.y ) ;
}

float &vec2::operator[]( int num )
{
	return ( (float *)&x )[ num ] ;
}

const float &vec2::operator[]( int num ) const
{
	return ( (const float *)&x )[ num ] ;
}

bool vec2::operator==( const vec2 &v ) const
{
	return ( x == v.x && y == v.y ) ;
}

bool vec2::operator!=( const vec2 &v ) const
{
	return ( x != v.x || y != v.y ) ;
}

bool vec2::operator<( const vec2 &v ) const
{
	if ( x != v.x ) return ( x < v.x ) ;
	return ( y < v.y ) ;
}

//----------------------------------------------------------------
//  vec3
//----------------------------------------------------------------

vec3::vec3( float f )
{
	set( f, f, f ) ;
}

vec3::vec3( float x, float y, float z )
{
	set( x, y, z ) ;
}

vec3::vec3( const vec2 &v )
{
	set( v.x, v.y, 1.0f ) ;
}

vec3::vec3( const vec4 &v )
{
	set( v.x, v.y, v.z ) ;
}

vec3 &vec3::set( float x, float y, float z )
{
	this->x = x ;
	this->y = y ;
	this->z = z ;
	return *this ;
}

vec3 &vec3::operator=( float f )
{
	return set( f, f, f ) ;
}

vec3 &vec3::operator=( const vec2 &v )
{
	return set( v.x, v.y, 1.0f ) ;
}

vec3 &vec3::operator=( const vec4 &v )
{
	return set( v.x, v.y, v.z ) ;
}

float &vec3::operator[]( int num )
{
	return ( (float *)&x )[ num ] ;
}

const float &vec3::operator[]( int num ) const
{
	return ( (const float *)&x )[ num ] ;
}

bool vec3::operator==( const vec3 &v ) const
{
	return ( x == v.x && y == v.y && z == v.z ) ;
}

bool vec3::operator!=( const vec3 &v ) const
{
	return ( x != v.x || y != v.y || z != v.z ) ;
}

bool vec3::operator<( const vec3 &v ) const
{
	if ( x != v.x ) return ( x < v.x ) ;
	if ( y != v.y ) return ( y < v.y ) ;
	return ( z < v.z ) ;
}

//----------------------------------------------------------------
//  vec4
//----------------------------------------------------------------

vec4::vec4( float f )
{
	set( f, f, f, f ) ;
}

vec4::vec4( float x, float y, float z, float w )
{
	set( x, y, z, w ) ;
}

vec4::vec4( const vec2 &v )
{
	set( v.x, v.y, 1.0f, 1.0f ) ;
}

vec4::vec4( const vec3 &v )
{
	set( v.x, v.y, v.z, 1.0f ) ;
}

vec4::vec4( const rgba8888 &c )
{
	set( c.r / 255.0f, c.g / 255.0f, c.b / 255.0f, c.a / 255.0f ) ;
}

vec4 &vec4::set( float x, float y, float z, float w )
{
	this->x = x ;
	this->y = y ;
	this->z = z ;
	this->w = w ;
	return *this ;
}

vec4 &vec4::operator=( float f )
{
	return set( f, f, f, f ) ;
}

vec4 &vec4::operator=( const vec2 &v )
{
	return set( v.x, v.y, 1.0f, 1.0f ) ;
}

vec4 &vec4::operator=( const vec3 &v )
{
	return set( v.x, v.y, v.z, 1.0f ) ;
}

vec4 &vec4::operator=( const rgba8888 &c )
{
	return set( c.r / 255.0f, c.g / 255.0f, c.b / 255.0f, c.a / 255.0f ) ;
}

float &vec4::operator[]( int num )
{
	return ( (float *)&x )[ num ] ;
}

const float &vec4::operator[]( int num ) const
{
	return ( (const float *)&x )[ num ] ;
}

bool vec4::operator==( const vec4 &v ) const
{
	return ( x == v.x && y == v.y && z == v.z && w == v.w ) ;
}

bool vec4::operator!=( const vec4 &v ) const
{
	return ( x != v.x || y != v.y || z != v.z || w != v.w ) ;
}

bool vec4::operator<( const vec4 &v ) const
{
	if ( x != v.x ) return ( x < v.x ) ;
	if ( y != v.y ) return ( y < v.y ) ;
	if ( z != v.z ) return ( z < v.z ) ;
	return ( w < v.w ) ;
}

//----------------------------------------------------------------
//  mat4
//----------------------------------------------------------------

mat4::mat4( float f )
{
	x.set( f, 0, 0, 0 ) ;
	y.set( 0, f, 0, 0 ) ;
	z.set( 0, 0, f, 0 ) ;
	w.set( 0, 0, 0, f ) ;
}

mat4::mat4( float xx, float xy, float xz, float xw,
                   float yx, float yy, float yz, float yw,
                   float zx, float zy, float zz, float zw,
                   float wx, float wy, float wz, float ww )
{
	x.set( xx, xy, xz, xw ) ;
	y.set( yx, yy, yz, yw ) ;
	z.set( zx, zy, zz, zw ) ;
	w.set( wx, wy, wz, ww ) ;
}

mat4::mat4( const vec4 &x, const vec4 &y, const vec4 &z, const vec4 &w )
{
	set( x, y, z, w ) ;
}

mat4 &mat4::set( float xx, float xy, float xz, float xw,
                        float yx, float yy, float yz, float yw,
                        float zx, float zy, float zz, float zw,
                        float wx, float wy, float wz, float ww )
{
	x.set( xx, xy, xz, xw ) ;
	y.set( yx, yy, yz, yw ) ;
	z.set( zx, zy, zz, zw ) ;
	w.set( wx, wy, wz, ww ) ;
	return *this ;
}

mat4 &mat4::set( const vec4 &x, const vec4 &y, const vec4 &z, const vec4 &w )
{
	this->x = x ;
	this->y = y ;
	this->z = z ;
	this->w = w ;
	return *this ;
}

mat4 &mat4::operator=( float f )
{
	x.set( f, 0, 0, 0 ) ;
	y.set( 0, f, 0, 0 ) ;
	z.set( 0, 0, f, 0 ) ;
	w.set( 0, 0, 0, f ) ;
	return *this ;
}

vec4 &mat4::operator[]( int num )
{
	return ( (vec4 *)&x )[ num ] ;
}

const vec4 &mat4::operator[]( int num ) const
{
	return ( (const vec4 *)&x )[ num ] ;
}

bool mat4::operator==( const mat4 &m ) const
{
	return ( x == m.x && y == m.y && z == m.z && w == m.w ) ;
}

bool mat4::operator!=( const mat4 &m ) const
{
	return ( x != m.x || y != m.y || z != m.z || w != m.w ) ;
}

bool mat4::operator<( const mat4 &m ) const
{
	if ( x != m.x ) return ( x < m.x ) ;
	if ( y != m.y ) return ( y < m.y ) ;
	if ( z != m.z ) return ( z < m.z ) ;
	return ( w < m.w ) ;
}

//----------------------------------------------------------------
//  quat
//----------------------------------------------------------------

quat::quat( float f )
{
	set( 0, 0, 0, f ) ;
}

quat::quat( float x, float y, float z, float w )
{
	set( x, y, z, w ) ;
}

quat &quat::set( float x, float y, float z, float w )
{
	this->x = x ;
	this->y = y ;
	this->z = z ;
	this->w = w ;
	return *this ;
}

quat &quat::operator=( float f )
{
	return set( 0, 0, 0, f ) ;
}

float &quat::operator[]( int num )
{
	return ( (float *)&x )[ num ] ;
}

const float &quat::operator[]( int num ) const
{
	return ( (const float *)&x )[ num ] ;
}

bool quat::operator==( const quat &q ) const
{
	return ( x == q.x && y == q.y && z == q.z && w == q.w ) ;
}

bool quat::operator!=( const quat &q ) const
{
	return ( x != q.x || y != q.y || z != q.z || w != q.w ) ;
}

bool quat::operator<( const quat &q ) const
{
	if ( x != q.x ) return ( x < q.x ) ;
	if ( y != q.y ) return ( y < q.y ) ;
	if ( z != q.z ) return ( z < q.z ) ;
	return ( w < q.w ) ;
}

//----------------------------------------------------------------
//  rect
//----------------------------------------------------------------

rect::rect( float f )
{
	set( 0, 0, f, f ) ;
}

rect::rect( float x, float y, float w, float h )
{
	set( x, y, w, h ) ;
}

rect &rect::set( float x, float y, float w, float h )
{
	this->x = x ;
	this->y = y ;
	this->w = w ;
	this->h = h ;
	return *this ;
}

rect &rect::operator=( float f )
{
	return set( 0, 0, f, f ) ;
}

float &rect::operator[]( int num )
{
	return ( (float *)&x )[ num ] ;
}

const float &rect::operator[]( int num ) const
{
	return ( (const float *)&x )[ num ] ;
}

bool rect::operator==( const rect &r ) const
{
	return ( x == r.x && y == r.y && w == r.w && h == r.h ) ;
}

bool rect::operator!=( const rect &r ) const
{
	return ( x != r.x || y != r.y || w != r.w || h != r.h ) ;
}

bool rect::operator<( const rect &r ) const
{
	if ( x != r.x ) return ( x < r.x ) ;
	if ( y != r.y ) return ( y < r.y ) ;
	if ( w != r.w ) return ( w < r.w ) ;
	return ( h < r.h ) ;
}

vec2 rect::xy() const
{
	return vec2( x, y ) ;
}

vec2 rect::wh() const
{
	return vec2( w, h ) ;
}

//----------------------------------------------------------------
//  vector functions
//----------------------------------------------------------------

vec4 operator+( const vec4 &v1, const vec4 &v2 )
{
	return vec4( v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w ) ;
}

vec4 operator-( const vec4 &v1, const vec4 &v2 )
{
	return vec4( v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w ) ;
}

vec4 operator-( const vec4 &v1 )
{
	return vec4( - v1.x, - v1.y, - v1.z, - v1.w ) ;
}

vec4 operator*( const vec4 &v1, const vec4 &v2 )
{
	return vec4( v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w ) ;
}

vec4 operator*( const vec4 &v1, float f )
{
	return vec4( v1.x * f, v1.y * f, v1.z * f, v1.w * f ) ;
}

vec4 operator*( float f, const vec4 &v1 )
{
	return vec4( v1.x * f, v1.y * f, v1.z * f, v1.w * f ) ;
}

vec4 operator/( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v2.x == 0.0f ) ? 0.0f : v1.x / v2.x ;
	float y = ( v2.y == 0.0f ) ? 0.0f : v1.y / v2.y ;
	float z = ( v2.z == 0.0f ) ? 0.0f : v1.z / v2.z ;
	float w = ( v2.w == 0.0f ) ? 0.0f : v1.w / v2.w ;
	return vec4( x, y, z, w ) ;
}

vec4 operator/( const vec4 &v1, float f )
{
	if ( f == 0.0f ) return v1 ;
	f = 1.0f / f ;
	return vec4( v1.x * f, v1.y * f, v1.z * f, v1.w * f ) ;
}

vec4 operator/( float f, const vec4 &v1 )
{
	float x = ( v1.x == 0.0f ) ? 0.0f : f / v1.x ;
	float y = ( v1.y == 0.0f ) ? 0.0f : f / v1.y ;
	float z = ( v1.z == 0.0f ) ? 0.0f : f / v1.z ;
	float w = ( v1.w == 0.0f ) ? 0.0f : f / v1.w ;
	return vec4( x, y, z, w ) ;
}

vec4 &operator+=( vec4 &v1, const vec4 &v2 )
{
	v1.x += v2.x ;
	v1.y += v2.y ;
	v1.z += v2.z ;
	v1.w += v2.w ;
	return v1 ;
}

vec4 &operator-=( vec4 &v1, const vec4 &v2 )
{
	v1.x -= v2.x ;
	v1.y -= v2.y ;
	v1.z -= v2.z ;
	v1.w -= v2.w ;
	return v1 ;
}

vec4 &operator*=( vec4 &v1, const vec4 &v2 )
{
	v1.x *= v2.x ;
	v1.y *= v2.y ;
	v1.z *= v2.z ;
	v1.w *= v2.w ;
	return v1 ;
}

vec4 &operator*=( vec4 &v1, float f )
{
	v1.x *= f ;
	v1.y *= f ;
	v1.z *= f ;
	v1.w *= f ;
	return v1 ;
}

vec4 &operator/=( vec4 &v1, const vec4 &v2 )
{
	v1.x = ( v2.x == 0.0f ) ? 0.0f : v1.x / v2.x ;
	v1.y = ( v2.y == 0.0f ) ? 0.0f : v1.y / v2.y ;
	v1.z = ( v2.z == 0.0f ) ? 0.0f : v1.z / v2.z ;
	v1.w = ( v2.w == 0.0f ) ? 0.0f : v1.w / v2.w ;
	return v1 ;
}

vec4 &operator/=( vec4 &v1, float f )
{
	if ( f == 0.0f ) return v1 ;
	f = 1.0f / f ;
	v1.x *= f ;
	v1.y *= f ;
	v1.z *= f ;
	v1.w *= f ;
	return v1 ;
}

bool equals2( const vec4 &v1, const vec4 &v2, float e )
{
	if ( fabsf( v1.x - v2.x ) > e ) return false ;
	if ( fabsf( v1.y - v2.y ) > e ) return false ;
	return true ;
}

float dot2( const vec4 &v1, const vec4 &v2 )
{
	return v1.x * v2.x + v1.y * v2.y ;
}

float length2( const vec4 &v1 )
{
	return sqrtf( dot2( v1, v1 ) ) ;
}

vec4 normalize2( const vec4 &v1 )
{
	float f = sqrtf( dot2( v1, v1 ) ) ;
	if ( f == 0.0f ) return v1 ;
	f = 1.0f / f ;
	return vec4( v1.x * f, v1.y * f, v1.z, v1.w ) ;
}

vec4 lerp2( const vec4 &v1, const vec4 &v2, float rate )
{
	float x = ( v2.x - v1.x ) * rate + v1.x ;
	float y = ( v2.y - v1.y ) * rate + v1.y ;
	return vec4( x, y, v1.z, v1.w ) ;
}

vec4 clamp2( const vec4 &v1, float min, float max )
{
	float x = ( v1.x < min ) ? min : ( v1.x > max ) ? max : v1.x ;
	float y = ( v1.y < min ) ? min : ( v1.y > max ) ? max : v1.y ;
	return vec4( x, y, v1.z, v1.w ) ;
}

vec4 min2( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v1.x < v2.x ) ? v1.x : v2.x ;
	float y = ( v1.y < v2.y ) ? v1.y : v2.y ;
	return vec4( x, y, v1.z, v1.w ) ;
}

float min2( const vec4 &v1 )
{
	float f = v1.x ;
	if ( v1.y < f ) f = v1.y ;
	return f ;
}

vec4 max2( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v1.x > v2.x ) ? v1.x : v2.x ;
	float y = ( v1.y > v2.y ) ? v1.y : v2.y ;
	return vec4( x, y, v1.z, v1.w ) ;
}

float max2( const vec4 &v1 )
{
	float f = v1.x ;
	if ( v1.y > f ) f = v1.y ;
	return f ;
}

bool equals3( const vec4 &v1, const vec4 &v2, float e )
{
	if ( fabsf( v1.x - v2.x ) > e ) return false ;
	if ( fabsf( v1.y - v2.y ) > e ) return false ;
	if ( fabsf( v1.z - v2.z ) > e ) return false ;
	return true ;
}

float dot3( const vec4 &v1, const vec4 &v2 )
{
	return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z ;
}

vec4 cross3( const vec4 &v1, const vec4 &v2 )
{
	float x = v1.y * v2.z - v2.y * v1.z ;
	float y = v1.z * v2.x - v2.z * v1.x ;
	float z = v1.x * v2.y - v2.x * v1.y ;
	return vec4( x, y, z, v1.w ) ;
}

float length3( const vec4 &v1 )
{
	return sqrtf( dot3( v1, v1 ) ) ;
}

vec4 normalize3( const vec4 &v1 )
{
	float f = sqrtf( dot3( v1, v1 ) ) ;
	if ( f == 0.0f ) return v1 ;
	f = 1.0f / f ;
	return vec4( v1.x * f, v1.y * f, v1.z * f, v1.w ) ;
}

vec4 lerp3( const vec4 &v1, const vec4 &v2, float rate )
{
	float x = ( v2.x - v1.x ) * rate + v1.x ;
	float y = ( v2.y - v1.y ) * rate + v1.y ;
	float z = ( v2.z - v1.z ) * rate + v1.z ;
	return vec4( x, y, z, v1.w ) ;
}

vec4 clamp3( const vec4 &v1, float min, float max )
{
	float x = ( v1.x < min ) ? min : ( v1.x > max ) ? max : v1.x ;
	float y = ( v1.y < min ) ? min : ( v1.y > max ) ? max : v1.y ;
	float z = ( v1.z < min ) ? min : ( v1.z > max ) ? max : v1.z ;
	return vec4( x, y, z, v1.w ) ;
}

vec4 min3( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v1.x < v2.x ) ? v1.x : v2.x ;
	float y = ( v1.y < v2.y ) ? v1.y : v2.y ;
	float z = ( v1.z < v2.z ) ? v1.z : v2.z ;
	return vec4( x, y, z, v1.w ) ;
}

float min3( const vec4 &v1 )
{
	float f = v1.x ;
	if ( v1.y < f ) f = v1.y ;
	if ( v1.z < f ) f = v1.z ;
	return f ;
}

vec4 max3( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v1.x > v2.x ) ? v1.x : v2.x ;
	float y = ( v1.y > v2.y ) ? v1.y : v2.y ;
	float z = ( v1.z > v2.z ) ? v1.z : v2.z ;
	return vec4( x, y, z, v1.w ) ;
}

float max3( const vec4 &v1 )
{
	float f = v1.x ;
	if ( v1.y > f ) f = v1.y ;
	if ( v1.z > f ) f = v1.z ;
	return f ;
}

bool equals4( const vec4 &v1, const vec4 &v2, float e )
{
	if ( fabsf( v1.x - v2.x ) > e ) return false ;
	if ( fabsf( v1.y - v2.y ) > e ) return false ;
	if ( fabsf( v1.z - v2.z ) > e ) return false ;
	if ( fabsf( v1.w - v2.w ) > e ) return false ;
	return true ;
}

float dot4( const vec4 &v1, const vec4 &v2 )
{
	return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z ;
}

float length4( const vec4 &v1 )
{
	return sqrtf( dot4( v1, v1 ) ) ;
}

vec4 normalize4( const vec4 &v1 )
{
	float f = sqrtf( dot4( v1, v1 ) ) ;
	if ( f == 0.0f ) return v1 ;
	f = 1.0f / f ;
	return vec4( v1.x * f, v1.y * f, v1.z * f, v1.w ) ;
}

vec4 lerp4( const vec4 &v1, const vec4 &v2, float rate )
{
	float x = ( v2.x - v1.x ) * rate + v1.x ;
	float y = ( v2.y - v1.y ) * rate + v1.y ;
	float z = ( v2.z - v1.z ) * rate + v1.z ;
	float w = ( v2.w - v1.w ) * rate + v1.w ;
	return vec4( x, y, z, w ) ;
}

vec4 clamp4( const vec4 &v1, float min, float max )
{
	float x = ( v1.x < min ) ? min : ( v1.x > max ) ? max : v1.x ;
	float y = ( v1.y < min ) ? min : ( v1.y > max ) ? max : v1.y ;
	float z = ( v1.z < min ) ? min : ( v1.z > max ) ? max : v1.z ;
	float w = ( v1.w < min ) ? min : ( v1.w > max ) ? max : v1.w ;
	return vec4( x, y, z, w ) ;
}

vec4 min4( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v1.x < v2.x ) ? v1.x : v2.x ;
	float y = ( v1.y < v2.y ) ? v1.y : v2.y ;
	float z = ( v1.z < v2.z ) ? v1.z : v2.z ;
	float w = ( v1.w < v2.w ) ? v1.w : v2.w ;
	return vec4( x, y, z, w ) ;
}

float min4( const vec4 &v1 )
{
	float f = v1.x ;
	if ( v1.y < f ) f = v1.y ;
	if ( v1.z < f ) f = v1.z ;
	if ( v1.w < f ) f = v1.w ;
	return f ;
}

vec4 max4( const vec4 &v1, const vec4 &v2 )
{
	float x = ( v1.x > v2.x ) ? v1.x : v2.x ;
	float y = ( v1.y > v2.y ) ? v1.y : v2.y ;
	float z = ( v1.z > v2.z ) ? v1.z : v2.z ;
	float w = ( v1.w > v2.w ) ? v1.w : v2.w ;
	return vec4( x, y, z, w ) ;
}

float max4( const vec4 &v1 )
{
	float f = v1.x ;
	if ( v1.y > f ) f = v1.y ;
	if ( v1.z > f ) f = v1.z ;
	if ( v1.w > f ) f = v1.w ;
	return f ;
}

//----------------------------------------------------------------
//  matrix functions
//----------------------------------------------------------------

mat4 operator+( const mat4 &m1, const mat4 &m2 )
{
	return mat4( m1.x + m2.x, m1.y + m2.y, m1.z + m2.z, m1.w + m2.w ) ;
}

mat4 operator-( const mat4 &m1, const mat4 &m2 )
{
	return mat4( m1.x - m2.x, m1.y - m2.y, m1.z - m2.z, m1.w - m2.w ) ;
}

mat4 operator-( const mat4 &m1 )
{
	return mat4( - m1.x, - m1.y, - m1.z, - m1.w ) ;
}

mat4 operator*( const mat4 &m1, const mat4 &m2 )
{
	vec4 x = transform4( m1, m2.x ) ;
	vec4 y = transform4( m1, m2.y ) ;
	vec4 z = transform4( m1, m2.z ) ;
	vec4 w = transform4( m1, m2.w ) ;
	return mat4( x, y, z, w ) ;
}

mat4 operator*( const mat4 &m1, float f )
{
	return mat4( m1.x * f, m1.y * f, m1.z * f, m1.w * f ) ;
}

mat4 operator*( float f, const mat4 &m1 )
{
	return mat4( m1.x * f, m1.y * f, m1.z * f, m1.w * f ) ;
}

vec4 operator*( const mat4 &m1, const vec4 &v1 )
{
	return transform3( m1, v1 ) ;
}

mat4 operator/( const mat4 &m1, float f )
{
	if ( f == 0.0f ) return m1 ;
	f = 1.0f / f ;
	return mat4( m1.x * f, m1.y * f, m1.z * f, m1.w * f ) ;
}

mat4 &operator+=( mat4 &m1, const mat4 &m2 )
{
	m1.x += m2.x ;
	m1.y += m2.y ;
	m1.z += m2.z ;
	m1.w += m2.w ;
	return m1 ;
}

mat4 &operator-=( mat4 &m1, const mat4 &m2 )
{
	m1.x -= m2.x ;
	m1.y -= m2.y ;
	m1.z -= m2.z ;
	m1.w -= m2.w ;
	return m1 ;
}

mat4 &operator*=( mat4 &m1, const mat4 &m2 )
{
	mat4 m = m1 ;
	m1.x = transform4( m, m2.x ) ;
	m1.y = transform4( m, m2.y ) ;
	m1.z = transform4( m, m2.z ) ;
	m1.w = transform4( m, m2.w ) ;
	return m1 ;
}

mat4 &operator*=( mat4 &m1, float f )
{
	m1.x *= f ;
	m1.y *= f ;
	m1.z *= f ;
	m1.w *= f ;
	return m1 ;
}

mat4 &operator/=( mat4 &m1, float f )
{
	if ( f == 0.0f ) return m1 ;
	f = 1.0f / f ;
	m1.x *= f ;
	m1.y *= f ;
	m1.z *= f ;
	m1.w *= f ;
	return m1 ;
}

bool equals( const mat4 &m1, const mat4 &m2, float e )
{
	return equals( m1, m2, e, e ) ;
}

bool equals( const mat4 &m1, const mat4 &m2, float e, float ew )
{
	if ( !equals4( m1.x, m2.x, e ) ) return false ;
	if ( !equals4( m1.y, m2.y, e ) ) return false ;
	if ( !equals4( m1.z, m2.z, e ) ) return false ;
	if ( !equals4( m1.w, m2.w, ew ) ) return false ;
	return true ;
}

mat4 transpose( const mat4 &m1 )
{
	return mat4( m1.x.x, m1.y.x, m1.z.x, m1.w.x,
	             m1.x.y, m1.y.y, m1.z.y, m1.w.y,
	             m1.x.z, m1.y.z, m1.z.z, m1.w.z,
	             m1.x.w, m1.y.w, m1.z.w, m1.w.w ) ;
}

mat4 normalize( const mat4 &m1 )
{
	return mat4( normalize3( m1.x ),
	             normalize3( m1.y ),
	             normalize3( m1.z ),
	             m1.w ) ;
}

mat4 inverse( const mat4 &m1 )
{
	float source[ 4 ][ 4 ], inverse[ 4 ][ 4 ] ;
	int i, j, k ;
	float tmp, d ;

	for ( i = 0 ; i < 4 ; i ++ ) {
		for ( j = 0 ; j < 4 ; j ++ ) {
			source[ i ][ j ] = ( m1[ i ][ j ] ) ;
			inverse[ i ][ j ] = ( i == j ) ? 1.0f : 0.0f ;
		}
	}

	for ( i = 0 ; i < 4 ; i ++ ) {
		d = 0.0f ;
		j = -1 ;
		for ( k = i ; k < 4 ; k ++ ) {
			tmp = source[ k ][ i ] ;
			if ( fabsf( tmp ) > fabsf( d ) ) {
				d = tmp ;
				j = k ;
			}
		}
		if ( j < 0 ) return mat4_one ;

		if ( j != i ) {
			for ( k = 0 ; k < 4 ; k ++ ) {
				tmp = source[ i ][ k ] ;
				source[ i ][ k ] = source[ j ][ k ] ;
				source[ j ][ k ] = tmp ;
				tmp = inverse[ i ][ k ] ;
				inverse[ i ][ k ] = inverse[ j ][ k ] ;
				inverse[ j ][ k ] = tmp ;
			}
		}
		for ( j = 0 ; j < 4 ; j ++ ) {
			if ( j == i ) source[ i ][ j ] = 1.0f ;
			if ( j > i ) source[ i ][ j ] /= d ;
			inverse[ i ][ j ] /= d ;
		}
		for ( j = 0 ; j < 4 ; j ++ ) {
			if ( j == i ) continue ;
			d = source[ j ][ i ] ;
			for ( k = 0 ; k < 4 ; k ++ ) {
				if ( k == i ) source[ j ][ k ] = 0.0f ;
				if ( k > i ) source[ j ][ k ] -= source[ i ][ k ] * d ;
				inverse[ j ][ k ] -= inverse[ i ][ k ] * d ;
			}
		}
	}
	mat4 m ;
	for ( i = 0 ; i < 4 ; i ++ ) {
		for ( j = 0 ; j < 4 ; j ++ ) {
			m[ i ][ j ] = (float)( inverse[ i ][ j ] ) ;
		}
	}
	return m ;
}

mat4 inverse_ortho( const mat4 &m1 )
{
	float sx = 1.0f / dot3( m1.x, m1.x ) ;
	float sy = 1.0f / dot3( m1.y, m1.y ) ;
	float sz = 1.0f / dot3( m1.z, m1.z ) ;

	float x = - m1.w.x ;
	float y = - m1.w.y ;
	float z = - m1.w.z ;
	return mat4( m1.x.x * sx, m1.y.x * sy, m1.z.x * sz, 0,
	             m1.x.y * sx, m1.y.y * sy, m1.z.y * sz, 0,
	             m1.x.z * sx, m1.y.z * sy, m1.z.z * sz, 0,
	             ( m1.x.x * x + m1.x.y * y + m1.x.z * z ) * sx,
	             ( m1.y.x * x + m1.y.y * y + m1.y.z * z ) * sy,
	             ( m1.z.x * x + m1.z.y * y + m1.z.z * z ) * sz, 1 ) ;
}

mat4 inverse_orthonormal( const mat4 &m1 )
{
	float x = - m1.w.x ;
	float y = - m1.w.y ;
	float z = - m1.w.z ;
	return mat4( m1.x.x, m1.y.x, m1.z.x, 0,
	             m1.x.y, m1.y.y, m1.z.y, 0,
	             m1.x.z, m1.y.z, m1.z.z, 0,
	             m1.x.x * x + m1.x.y * y + m1.x.z * z,
	             m1.y.x * x + m1.y.y * y + m1.y.z * z,
	             m1.z.x * x + m1.z.y * y + m1.z.z * z, 1 ) ;
}

mat4 lerp( const mat4 &m1, const mat4 &m2, float rate )
{
	return mat4( lerp4( m1.x, m2.x, rate ),
	             lerp4( m1.y, m2.y, rate ),
	             lerp4( m1.z, m2.z, rate ),
	             lerp4( m1.w, m2.w, rate ) ) ;
}

vec4 transform2( const mat4 &m1, const vec4 &v1, float z, float w )
{
	return m1.x * v1.x + m1.y * v1.y + m1.z * z + m1.w * w ;
}

vec4 project2( const mat4 &m1, const vec4 &v1, float z, float w )
{
	vec4 v2 = transform2( m1, v1, z, w ) ;
	float w2 = v2.w ;
	if ( w2 == 0 ) return v2 ;
	w2 = 1.0f / w2 ;
	return vec4( v2.x * w2, v2.y * w2, v2.z * w2, w2 ) ;
}

vec4 transform3( const mat4 &m1, const vec4 &v1, float w )
{
	return m1.x * v1.x + m1.y * v1.y + m1.z * v1.z + m1.w * w ;
}

vec4 project3( const mat4 &m1, const vec4 &v1, float w )
{
	vec4 v2 = transform3( m1, v1, w ) ;
	float w2 = v2.w ;
	if ( w2 == 0 ) return v2 ;
	w2 = 1.0f / w2 ;
	return vec4( v2.x * w2, v2.y * w2, v2.z * w2, w2 ) ;
}

vec4 transform4( const mat4 &m1, const vec4 &v1 )
{
	return m1.x * v1.x + m1.y * v1.y + m1.z * v1.z + m1.w * v1.w ;
}

vec4 project4( const mat4 &m1, const vec4 &v1 )
{
	vec4 v2 = transform4( m1, v1 ) ;
	float w2 = v2.w ;
	if ( w2 == 0 ) return v2 ;
	w2 = 1.0f / w2 ;
	return vec4( v2.x * w2, v2.y * w2, v2.z * w2, w2 ) ;
}

mat4 mat4_identity()
{
	return mat4_one ;
}

mat4 mat4_from_translate( const vec4 &v1 )
{
	return mat4_from_translate( v1.x, v1.y, v1.z ) ;
}

mat4 mat4_from_translate( float x, float y, float z )
{
	return mat4( 1, 0, 0, 0,
	             0, 1, 0, 0,
	             0, 0, 1, 0,
	             x, y, z, 1 ) ;
}

mat4 mat4_from_rotate( const vec4 &axis, float angle )
{
	return mat4_from_quat( quat_from_rotate( axis, angle ) ) ;
}

mat4 mat4_from_rotate( float x, float y, float z, float angle )
{
	return mat4_from_quat( quat_from_rotate( x, y, z, angle ) ) ;
}

mat4 mat4_from_rotx( float x )
{
	float c = cosf( x ) ;
	float s = sinf( x ) ;
	return mat4( 1, 0, 0, 0,
	             0, c, s, 0,
	             0, -s, c, 0,
	             0, 0, 0, 1 ) ;
}

mat4 mat4_from_roty( float y )
{
	float c = cosf( y ) ;
	float s = sinf( y ) ;
	return mat4( c, 0, -s, 0,
	             0, 1, 0, 0,
	             s, 0, c, 0,
	             0, 0, 0, 1 ) ;
}

mat4 mat4_from_rotz( float z )
{
	float c = cosf( z ) ;
	float s = sinf( z ) ;
	return mat4( c, s, 0, 0,
	             -s, c, 0, 0,
	             0, 0, 1, 0,
	             0, 0, 0, 1 ) ;
}

mat4 mat4_from_rotzyx( const vec4 &rotate )
{
	return mat4_from_rotzyx( rotate.x, rotate.y, rotate.z ) ;
}

mat4 mat4_from_rotyxz( const vec4 &rotate )
{
	return mat4_from_rotyxz( rotate.x, rotate.y, rotate.z ) ;
}

mat4 mat4_from_rotxzy( const vec4 &rotate )
{
	return mat4_from_rotxzy( rotate.x, rotate.y, rotate.z ) ;
}

mat4 mat4_from_rotxyz( const vec4 &rotate )
{
	return mat4_from_rotxyz( rotate.x, rotate.y, rotate.z ) ;
}

mat4 mat4_from_rotyzx( const vec4 &rotate )
{
	return mat4_from_rotyzx( rotate.x, rotate.y, rotate.z ) ;
}

mat4 mat4_from_rotzxy( const vec4 &rotate )
{
	return mat4_from_rotzxy( rotate.x, rotate.y, rotate.z ) ;
}

mat4 mat4_from_rotzyx( float x, float y, float z )
{
	return mat4_from_quat( quat_from_rotzyx( x, y, z ) ) ;
}

mat4 mat4_from_rotyxz( float x, float y, float z )
{
	return mat4_from_quat( quat_from_rotyxz( x, y, z ) ) ;
}

mat4 mat4_from_rotxzy( float x, float y, float z )
{
	return mat4_from_quat( quat_from_rotxzy( x, y, z ) ) ;
}

mat4 mat4_from_rotxyz( float x, float y, float z )
{
	return mat4_from_quat( quat_from_rotxyz( x, y, z ) ) ;
}

mat4 mat4_from_rotyzx( float x, float y, float z )
{
	return mat4_from_quat( quat_from_rotyzx( x, y, z ) ) ;
}

mat4 mat4_from_rotzxy( float x, float y, float z )
{
	return mat4_from_quat( quat_from_rotzxy( x, y, z ) ) ;
}

mat4 mat4_from_scale( const vec4 &scale )
{
	return mat4_from_scale( scale.x, scale.y, scale.z ) ;
}

mat4 mat4_from_scale( float x, float y, float z )
{
	return mat4( x, 0, 0, 0,
	             0, y, 0, 0,
	             0, 0, z, 0,
	             0, 0, 0, 1 ) ;
}

mat4 mat4_from_viewport( float x, float y, float w, float h, float z_near, float z_far )
{
	w *= 0.5f ;	x += w ;
	h *= -0.5f ;	y -= h ;
	float d = ( z_far - z_near ) * 0.5f ;
	float z = ( z_far + z_near ) * 0.5f ;
	return mat4( w, 0, 0, 0,
	             0, h, 0, 0,
	             0, 0, d, 0,
	             x, y, z, 1 ) ;
}

mat4 mat4_from_perspective( float fovy, float aspect, float z_near, float z_far )
{
	float f = 1.0f / tanf( fovy * 0.5f ) ;
	float a = ( z_far + z_near ) / ( z_near - z_far ) ;
	float b = 2 * z_far * z_near / ( z_near - z_far ) ;
	return mat4( f / aspect, 0, 0, 0,
                     0, f, 0, 0,
                     0, 0, a, -1,
                     0, 0, b, 0 ) ;
}

mat4 mat4_from_frustum( float left, float right, float bottom, float top, float z_near, float z_far )
{
	float w = 1.0f / ( right - left ) ;
	float h = 1.0f / ( top - bottom ) ;
	float d = 1.0f / ( z_far - z_near ) ;
	float A = ( right + left ) * w ;
	float B = ( top + bottom ) * h ;
	float C = - ( z_far + z_near ) * d ;
	float D = - 2 * z_far * z_near * d ;
	return mat4( 2 * z_near * w, 0, 0, 0,
                     0, 2 * z_near * h, 0, 0,
                     A, B, C, -1,
                     0, 0, D, 0 ) ;
}

mat4 mat4_from_ortho( float left, float right, float bottom, float top, float z_near, float z_far )
{
	float w = 1.0f / ( right - left ) ;
	float h = 1.0f / ( top - bottom ) ;
	float d = 1.0f / ( z_far - z_near ) ;
	float A = - ( right + left ) * w ;
	float B = - ( top + bottom ) * h ;
	float C = - ( z_far + z_near ) * d ;
	return mat4( 2 * w, 0, 0, 0,
                     0, 2 * h, 0, 0,
                     0, 0, -2 * d, 0,
                     A, B, C, 1 ) ;
}

mat4 mat4_from_lookat( const vec4 &eye, const vec4 &center, const vec4 &up )
{
	vec4 f = normalize3( center - eye ) ;
	vec4 u = normalize3( up ) ;
	vec4 s = normalize3( cross3( f, u ) ) ;
	u = cross3( s, f ) ;
	return mat4( s.x, u.x, -f.x, 0,
	             s.y, u.y, -f.y, 0,
	             s.z, u.z, -f.z, 0,
	             - dot3( eye, s ), - dot3( eye, u ), dot3( eye, f ), 1 ) ;
}

mat4 mat4_from_lookat( float ex, float ey, float ez, float cx, float cy, float cz, float ux, float uy, float uz )
{
	return mat4_from_lookat( vec4( ex, ey, ez ), vec4( cx, cy, cz ), vec4( ux, uy, uz ) ) ;
}

mat4 mat4_from_quat( const quat &q1 )
{
	return mat4_from_quat( q1.x, q1.y, q1.z, q1.w ) ;
}

mat4 mat4_from_quat( float x, float y, float z, float w )
{
	return mat4( 1 - 2*y*y - 2*z*z, 2*x*y + 2*w*z, 2*z*x - 2*w*y, 0,
	             2*x*y - 2*w*z, 1 - 2*x*x - 2*z*z, 2*y*z + 2*w*x, 0,
	             2*z*x + 2*w*y, 2*y*z - 2*w*x, 1 - 2*x*x - 2*y*y, 0,
	             0, 0, 0, 1 ) ;
}

mat4 mat4_from_rect( const rect &q1 )
{
	return mat4_from_rect( q1.x, q1.y, q1.w, q1.h ) ;
}

mat4 mat4_from_rect( float x, float y, float w, float h )
{
	return mat4( w, 0, 0, 0,
	             0, h, 0, 0,
	             0, 0, 1, 0,
	             x, y, 0, 1 ) ;
}

//----------------------------------------------------------------
//  quaternion functions
//----------------------------------------------------------------

quat operator*( const quat &q1, const quat &q2 )
{
	float w1, x1, y1, z1 ;
	float w2, x2, y2, z2 ;

	x1 = q1.x ;	y1 = q1.y ;	z1 = q1.z ;	w1 = q1.w ;
	x2 = q2.x ;	y2 = q2.y ;	z2 = q2.z ;	w2 = q2.w ;

	return quat( w1 * x2 + w2 * x1 + y1 * z2 - z1 * y2,
	             w1 * y2 + w2 * y1 + z1 * x2 - x1 * z2,
	             w1 * z2 + w2 * z1 + x1 * y2 - y1 * x2,
	             w1 * w2 - x1 * x2 - y1 * y2 - z1 * z2 ) ;
}

vec4 operator*( const quat &q1, const vec4 &v1 )
{
	return transform3( q1, v1 ) ;
}

quat &operator*=( quat &q1, const quat &q2 )
{
	return ( q1 = q1 * q2 ) ;
}

bool equals( const quat &q1, const quat &q2, float e )
{
	return ( dot( q1, q2 ) >= cosf( e * 0.5f ) ) ;
}

float dot( const quat &q1, const quat &q2 )
{
	return q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w ;
}

float length( const quat &q1 )
{
	return sqrtf( dot( q1, q1 ) ) ;
}

quat normalize( const quat &q1 )
{
	float f = sqrtf( dot( q1, q1 ) ) ;
	if ( f == 0.0f ) return q1 ;
	f = 1.0f / f ;
	return quat( q1.x * f, q1.y * f, q1.z * f, q1.w * f ) ;
}

quat inverse( const quat &q1 )
{
	return quat( -q1.x, -q1.y, -q1.z, q1.w ) ;
}

quat slerp( const quat &q1, const quat &q2, float rate )
{
	float c = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w ;
	float r1 = 1 - rate ;
	float r2 = rate ;
	if ( c < 0.999999f ) {
		if ( c < 0.0f ) {
			c = -c ;
			r2 = -r2 ;
		}
		float angle = acosf( c ) ;
		float s = sinf( angle ) ;
		if ( s != 0.0f ) {
			r1 = sinf( r1 * angle ) / s ;
			r2 = sinf( r2 * angle ) / s ;
		}
	}
	return quat( q1.x * r1 + q2.x * r2,
	             q1.y * r1 + q2.y * r2,
	             q1.z * r1 + q2.z * r2,
	             q1.w * r1 + q2.w * r2 ) ;
}

vec4 transform2( const quat &q1, const vec4 &v1, float z, float w )
{
	return transform2( mat4_from_quat( q1 ), v1, z, w ) ;
}

vec4 transform3( const quat &q1, const vec4 &v1, float w )
{
	return transform3( mat4_from_quat( q1 ), v1, w ) ;
}

vec4 transform4( const quat &q1, const vec4 &v1 )
{
	return transform4( mat4_from_quat( q1 ), v1 ) ;
}

quat quat_from_mat4( const mat4 &m1 )
{
	float t = m1.x.x + m1.y.y + m1.z.z + 1.0f ;
	if ( t > 0.01f ) {
		float w = sqrtf( t ) * 0.5f ;
		float r = 0.25f / w ;
		float x = ( m1.y.z - m1.z.y ) * r ;
		float y = ( m1.z.x - m1.x.z ) * r ;
		float z = ( m1.x.y - m1.y.x ) * r ;
		return quat( x, y, z, w ) ;
	}
	if ( m1.x.x > m1.y.y ) {
		if ( m1.x.x > m1.z.z ) {
			float x = sqrtf( 1.0f + m1.x.x - m1.y.y - m1.z.z ) * 0.5f ;
			float r = 0.25f / x ;
			float y = ( m1.y.x + m1.x.y ) * r ;
			float z = ( m1.z.x + m1.x.z ) * r ;
			float w = ( m1.y.z - m1.z.y ) * r ;
			return quat( x, y, z, w ) ;
		}
	} else {
		if ( m1.y.y > m1.z.z ) {
			float y = sqrtf( 1.0f + m1.y.y - m1.z.z - m1.x.x ) * 0.5f ;
			float r = 0.25f / y ;
			float x = ( m1.y.x + m1.x.y ) * r ;
			float z = ( m1.z.y + m1.y.z ) * r ;
			float w = ( m1.z.x - m1.x.z ) * r ;
			return quat( x, y, z, w ) ;
		}
	}
	float z = sqrtf( 1.0f + m1.z.z - m1.x.x - m1.y.y ) * 0.5f ;
	float r = 0.25f / z ;
	float x = ( m1.z.x + m1.x.z ) * r ;
	float y = ( m1.z.y + m1.y.z ) * r ;
	float w = ( m1.x.y - m1.y.x ) * r ;
	return quat( x, y, z, w ) ;
}

quat quat_from_rotate( const vec4 &axis, float angle )
{
	angle *= 0.5f ;
	vec4 v = normalize3( axis ) * sinf( angle ) ;
	return quat( v.x, v.y, v.z, cosf( angle ) ) ;
}

quat quat_from_rotate( float x, float y, float z, float angle )
{
	return quat_from_rotate( vec4( x, y, z ), angle ) ;
}

quat quat_from_rotzyx( const vec4 &angle )
{
	return quat_from_rotzyx( angle.x, angle.y, angle.z ) ;
}

quat quat_from_rotyxz( const vec4 &angle )
{
	return quat_from_rotyxz( angle.x, angle.y, angle.z ) ;
}

quat quat_from_rotxzy( const vec4 &angle )
{
	return quat_from_rotxzy( angle.x, angle.y, angle.z ) ;
}

quat quat_from_rotxyz( const vec4 &angle )
{
	return quat_from_rotxyz( angle.x, angle.y, angle.z ) ;
}

quat quat_from_rotyzx( const vec4 &angle )
{
	return quat_from_rotyzx( angle.x, angle.y, angle.z ) ;
}

quat quat_from_rotzxy( const vec4 &angle )
{
	return quat_from_rotzxy( angle.x, angle.y, angle.z ) ;
}

quat quat_from_rotzyx( float x, float y, float z )
{
	x *= 0.5f ;	y *= 0.5f ;	z *= 0.5f ;
	float cx = cosf( x ) ;	float sx = sinf( x ) ;
	float cy = cosf( y ) ;	float sy = sinf( y ) ;
	float cz = cosf( z ) ;	float sz = sinf( z ) ;
	return quat( cz * cy * sx - cx * sz * sy,
	             cx * cz * sy + cy * sz * sx,
	             cx * cy * sz - cz * sy * sx,
	             cz * cy * cx + sz * sy * sx ) ;
}

quat quat_from_rotyxz( float x, float y, float z )
{
	x *= 0.5f ;	y *= 0.5f ;	z *= 0.5f ;
	float cx = cosf( x ) ;	float sx = sinf( x ) ;
	float cy = cosf( y ) ;	float sy = sinf( y ) ;
	float cz = cosf( z ) ;	float sz = sinf( z ) ;
	return quat( cz * cy * sx + cx * sy * sz,
	             cz * cx * sy - cy * sx * sz,
	             cy * cx * sz - cz * sy * sx,
	             cy * cx * cz + sy * sx * sz ) ;
}

quat quat_from_rotxzy( float x, float y, float z )
{
	x *= 0.5f ;	y *= 0.5f ;	z *= 0.5f ;
	float cx = cosf( x ) ;	float sx = sinf( x ) ;
	float cy = cosf( y ) ;	float sy = sinf( y ) ;
	float cz = cosf( z ) ;	float sz = sinf( z ) ;
	return quat( cy * cz * sx - cx * sz * sy,
	             cx * cz * sy - cy * sx * sz,
	             cy * cx * sz + cz * sx * sy,
	             cx * cz * cy + sx * sz * sy ) ;
}

quat quat_from_rotxyz( float x, float y, float z )
{
	x *= 0.5f ;	y *= 0.5f ;	z *= 0.5f ;
	float cx = cosf( x ) ;	float sx = sinf( x ) ;
	float cy = cosf( y ) ;	float sy = sinf( y ) ;
	float cz = cosf( z ) ;	float sz = sinf( z ) ;
	return quat( cz * cy * sx + cx * sy * sz,
	             cz * cx * sy - cy * sx * sz,
	             cx * cy * sz + cz * sx * sy,
	             cx * cy * cz - sx * sy * sz ) ;
}

quat quat_from_rotyzx( float x, float y, float z )
{
	x *= 0.5f ;	y *= 0.5f ;	z *= 0.5f ;
	float cx = cosf( x ) ;	float sx = sinf( x ) ;
	float cy = cosf( y ) ;	float sy = sinf( y ) ;
	float cz = cosf( z ) ;	float sz = sinf( z ) ;
	return quat( cy * cz * sx + cx * sy * sz,
	             cx * cz * sy + cy * sz * sx,
	             cx * cy * sz - cz * sy * sx,
	             cy * cz * cx - sy * sz * sx ) ;
}

quat quat_from_rotzxy( float x, float y, float z )
{
	x *= 0.5f ;	y *= 0.5f ;	z *= 0.5f ;
	float cx = cosf( x ) ;	float sx = sinf( x ) ;
	float cy = cosf( y ) ;	float sy = sinf( y ) ;
	float cz = cosf( z ) ;	float sz = sinf( z ) ;
	return quat( cy * cz * sx - cx * sz * sy,
	             cz * cx * sy + cy * sz * sx,
	             cy * cx * sz + cz * sx * sy,
	             cz * cx * cy - sz * sx * sy ) ;
}

vec4 quat_to_rotate( const quat &q1 )
{
	vec4 v( q1.x, q1.y, q1.z ) ;
	v = normalize3( v ) ;
	v.w = acosf( q1.w ) * 2.0f ;
	return v ;
}

vec4 quat_to_rotzyx( const quat &q1 )
{
	float x, y, z, w ;
	float cx, sx, cy, sy, cz, sz ;
	float rx, ry, rz ;

	x = q1.x ;	y = q1.y ;
	z = q1.z ;	w = q1.w ;

	sy = 2 * w * y - 2 * z * x ;
	if ( sy <= 0.99995f && sy >= -0.99995f ) {
		cz = 1 - 2 * y * y - 2 * z * z ;
		sz = 2 * x * y + 2 * w * z ;
		cx = 1 - 2 * x * x - 2 * y * y ;
		sx = 2 * y * z + 2 * w * x ;
		ry = asinf( sy ) ;
	} else {
		cz = 1 - 2 * x * x - 2 * z * z ;
		sz = 2 * w * z - 2 * x * y ;
		cx = 1 ;
		sx = 0 ;
		cy = 1 - 2 * x * x - 2 * y * y ;
		ry = atan2f( sy, cy ) ;
	}
	rx = atan2f( sx, cx ) ;
	rz = atan2f( sz, cz ) ;
	return vec4( rx, ry, rz ) ;
}

vec4 quat_to_rotyxz( const quat &q1 )
{
	float x, y, z, w ;
	float cx, sx, cy, sy, cz, sz ;
	float rx, ry, rz ;

	x = q1.x ;	y = q1.y ;
	z = q1.z ;	w = q1.w ;

	sx = 2 * w * x - 2 * y * z ;
	if ( sx <= 0.99995f && sx >= -0.99995f ) {
		cz = 1 - 2 * x * x - 2 * z * z ;
		sz = 2 * x * y + 2 * w * z ;
		cy = 1 - 2 * x * x - 2 * y * y ;
		sy = 2 * z * x + 2 * w * y ;
		rx = asinf( sx ) ;
	} else {
		cy = 1 - 2 * y * y - 2 * z * z ;
		sy = 2 * w * y - 2 * z * x ;
		cz = 1 ;
		sz = 0 ;
		cx = 1 - 2 * x * x - 2 * z * z ;
		rx = atan2f( sx, cx ) ;
	}
	ry = atan2f( sy, cy ) ;
	rz = atan2f( sz, cz ) ;
	return vec4( rx, ry, rz ) ;
}

vec4 quat_to_rotxyz( const quat &q1 )
{
	//  TODO: implement quaternion to xyz-angles
	return 0.0f ;
}

//----------------------------------------------------------------
//  rectangle functions
//----------------------------------------------------------------

rect operator*( const rect &r1, const rect &r2 )
{
	return rect( r1.w * r2.x + r1.x, r1.h * r2.y + r1.y, r1.w * r2.w, r1.h * r2.h ) ;
}

vec4 operator*( const rect &r1, const vec4 &v1 )
{
	return transform3( r1, v1 ) ;
}

rect &operator*=( rect &r1, const rect &r2 )
{
	return ( r1 = r1 * r2 ) ;
}

bool equals( const rect &r1, const rect &r2, float e )
{
	if ( fabsf( r1.x - r2.x ) > e ) return false ;
	if ( fabsf( r1.y - r2.y ) > e ) return false ;
	if ( fabsf( r1.w - r2.w ) > e ) return false ;
	if ( fabsf( r1.h - r2.h ) > e ) return false ;
	return true ;
}

rect inverse( const rect &r1 )
{
	if ( r1.w == 0.0f || r1.h == 0.0f ) return 1.0f ;
	float w = 1.0f / r1.w ;
	float h = 1.0f / r1.h ;
	return rect( - r1.x * w, - r1.y * h, w, h ) ;
}

rect lerp( const rect &r1, const rect &r2, float rate )
{
	float x = ( r2.x - r1.x ) * rate + r1.x ;
	float y = ( r2.y - r1.y ) * rate + r1.y ;
	float w = ( r2.w - r1.w ) * rate + r1.w ;
	float h = ( r2.h - r1.h ) * rate + r1.h ;
	return rect( x, y, w, h ) ;
}

vec4 transform2( const rect &r1, const vec4 &v1, float z, float w )
{
	return vec4( v1.x * r1.w + w * r1.x, v1.y * r1.h + w * r1.y, z, w ) ;
}

vec4 transform3( const rect &r1, const vec4 &v1, float w )
{
	return vec4( v1.x * r1.w + w * r1.x, v1.y * r1.h + w * r1.y, v1.z, w ) ;
}

vec4 transform4( const rect &r1, const vec4 &v1 )
{
	return vec4( v1.x * r1.w + v1.w * r1.x, v1.y * r1.h + v1.w * r1.y, v1.z, v1.w ) ;
}

rect rect_from_mat4( const mat4 &m1 )
{
	return rect( m1.w.x, m1.w.y, m1.x.x, m1.y.y ) ;
}

//----------------------------------------------------------------
//  float functions
//----------------------------------------------------------------

bool equals( float f1, float f2, float e )
{
	return ( fabsf( f1 - f2 ) <= e ) ;
}

bool equals( const float *f1, const float *f2, int count, float e )
{
	while ( -- count >= 0 ) {
		if ( fabsf( *( f1 ++ ) - *( f2 ++ ) ) > e ) return false ;
	}
	return true ;
}


} // namespace mdx


//----------------------------------------------------------------
//  debug print
//----------------------------------------------------------------

std::ostream &operator<<( std::ostream &os, const mdx::vec2 &v )
{
	return os << v.x << " " <<  v.y ;
}

std::ostream &operator<<( std::ostream &os, const mdx::vec3 &v )
{
	return os << v.x << " " <<  v.y << " " << v.z ;
}

std::ostream &operator<<( std::ostream &os, const mdx::vec4 &v )
{
	return os << v.x << " " <<  v.y << " " << v.z << " " << v.w ;
}

std::ostream &operator<<( std::ostream &os, const mdx::mat4 &m )
{
	return os << m.x << "\n" << m.y << "\n" << m.z << "\n" << m.w ;
}

std::ostream &operator<<( std::ostream &os, const mdx::rect &r )
{
	return os << r.x << " " <<  r.y << " " << r.w << " " << r.h ;
}

std::ostream &operator<<( std::ostream &os, const mdx::quat &q )
{
	return os << q.x << " " <<  q.y << " " << q.z << " " << q.w ;
}
