#ifndef	MDX_MATH_H_INCLUDE
#define	MDX_MATH_H_INCLUDE

namespace mdx {

struct vec2 ;
struct vec3 ;
struct vec4 ;
struct mat4 ;
struct quat ;
struct rect ;

struct rgba8888 ;	//  MdxColor.h

//----------------------------------------------------------------
//  vec2
//----------------------------------------------------------------

struct vec2 {
	float x, y ;
public:
	vec2( float f = 0.0f ) ;
	vec2( float x, float y ) ;
	vec2( const vec3 &v ) ;
	vec2( const vec4 &v ) ;
	vec2 &set( float x, float y ) ;
	vec2 &operator=( float f ) ;
	vec2 &operator=( const vec3 &v ) ;
	vec2 &operator=( const vec4 &v ) ;
	float &operator[]( int num ) ;
	const float &operator[]( int num ) const ;
	bool operator==( const vec2 &v ) const ;
	bool operator!=( const vec2 &v ) const ;
	bool operator<( const vec2 &v ) const ;
} ;

//----------------------------------------------------------------
//  vec3
//----------------------------------------------------------------

struct vec3 {
	float x, y, z ;
public:
	vec3( float f = 0.0f ) ;
	vec3( float x, float y, float z = 0.0f ) ;
	vec3( const vec2 &v ) ;
	vec3( const vec4 &v ) ;
	vec3 &set( float x, float y, float z = 0.0f ) ;
	vec3 &operator=( float f ) ;
	vec3 &operator=( const vec2 &v ) ;
	vec3 &operator=( const vec4 &v ) ;
	float &operator[]( int num ) ;
	const float &operator[]( int num ) const ;
	bool operator==( const vec3 &v ) const ;
	bool operator!=( const vec3 &v ) const ;
	bool operator<( const vec3 &v ) const ;
} ;

//----------------------------------------------------------------
//  vec4
//----------------------------------------------------------------

struct vec4 {
	float x, y, z, w ;
public:
	vec4( float f = 0.0f ) ;
	vec4( float x, float y, float z = 0.0f, float w = 0.0f ) ;
	vec4( const vec2 &v ) ;
	vec4( const vec3 &v ) ;
	vec4( const rgba8888 &c ) ;
	vec4 &set( float x, float y, float z = 0.0f, float w = 0.0f ) ;
	vec4 &operator=( float f ) ;
	vec4 &operator=( const vec2 &v ) ;
	vec4 &operator=( const vec3 &v ) ;
	vec4 &operator=( const rgba8888 &c ) ;
	float &operator[]( int num ) ;
	const float &operator[]( int num ) const ;
	bool operator==( const vec4 &v ) const ;
	bool operator!=( const vec4 &v ) const ;
	bool operator<( const vec4 &v ) const ;
} ;

//----------------------------------------------------------------
//  mat4
//----------------------------------------------------------------

struct mat4 {
	vec4 x, y, z, w ;
public:
	mat4( float f = 1.0f ) ;
	mat4( float xx, float xy, float xz, float xw,
	      float yx, float yy, float yz, float yw,
	      float zx, float zy, float zz, float zw,
	      float wx, float wy, float wz, float ww ) ;
	mat4( const vec4 &x, const vec4 &y, const vec4 &z, const vec4 &w ) ;
	mat4 &set( float xx, float xy, float xz, float xw,
	           float yx, float yy, float yz, float yw,
	           float zx, float zy, float zz, float zw,
	           float wx, float wy, float wz, float ww ) ;
	mat4 &set( const vec4 &x, const vec4 &y, const vec4 &z, const vec4 &w ) ;
	mat4 &operator=( float f ) ;
	vec4 &operator[]( int num ) ;
	const vec4 &operator[]( int num ) const ;
	bool operator==( const mat4 &m ) const ;
	bool operator!=( const mat4 &m ) const ;
	bool operator<( const mat4 &m ) const ;
} ;

//----------------------------------------------------------------
//  quat
//----------------------------------------------------------------

struct quat {
	float x, y, z, w ;
public:
	quat( float f = 1.0f ) ;
	quat( float x, float y, float z, float w ) ;
	quat &set( float x, float y, float z, float w ) ;
	quat &operator=( float f ) ;
	float &operator[]( int num ) ;
	const float &operator[]( int num ) const ;
	bool operator==( const quat &q ) const ;
	bool operator!=( const quat &q ) const ;
	bool operator<( const quat &q ) const ;
} ;

//----------------------------------------------------------------
//  rect
//----------------------------------------------------------------

struct rect {
	float x, y, w, h ;
public:
	rect( float f = 1.0f ) ;
	rect( float x, float y, float w, float h ) ;
	rect &set( float x, float y, float w, float h ) ;
	rect &operator=( float f ) ;
	float &operator[]( int num ) ;
	const float &operator[]( int num ) const ;
	bool operator==( const rect &r ) const ;
	bool operator!=( const rect &r ) const ;
	bool operator<( const rect &r ) const ;

	vec2 xy() const ;
	vec2 wh() const ;
} ;

//----------------------------------------------------------------
//  constants
//----------------------------------------------------------------

extern const vec2 vec2_zero ;
extern const vec2 vec2_one ;
extern const vec3 vec3_zero ;
extern const vec3 vec3_one ;
extern const vec4 vec4_zero ;
extern const vec4 vec4_one ;
extern const mat4 mat4_zero ;
extern const mat4 mat4_one ;
extern const quat quat_zero ;
extern const quat quat_one ;
extern const rect rect_zero ;
extern const rect rect_one ;

//----------------------------------------------------------------
//  vector functions
//----------------------------------------------------------------

vec4 operator+( const vec4 &v1, const vec4 &v2 ) ;
vec4 operator-( const vec4 &v1, const vec4 &v2 ) ;
vec4 operator-( const vec4 &v1 ) ;
vec4 operator*( const vec4 &v1, const vec4 &v2 ) ;
vec4 operator*( const vec4 &v1, float f ) ;
vec4 operator*( float f, const vec4 &v1 ) ;
vec4 operator/( const vec4 &v1, const vec4 &v2 ) ;
vec4 operator/( const vec4 &v1, float f ) ;
vec4 operator/( float f, const vec4 &v1 ) ;
vec4 &operator+=( vec4 &v1, const vec4 &v2 ) ;
vec4 &operator-=( vec4 &v1, const vec4 &v2 ) ;
vec4 &operator*=( vec4 &v1, const vec4 &v2 ) ;
vec4 &operator*=( vec4 &v1, float f ) ;
vec4 &operator/=( vec4 &v1, const vec4 &v2 ) ;
vec4 &operator/=( vec4 &v1, float f ) ;

bool equals2( const vec4 &v1, const vec4 &v2, float e = 0.0f ) ;
float dot2( const vec4 &v1, const vec4 &v2 ) ;
float length2( const vec4 &v1 ) ;
vec4 normalize2( const vec4 &v1 ) ;
vec4 lerp2( const vec4 &v1, const vec4 &v2, float f ) ;
vec4 clamp2( const vec4 &v1, float min, float max ) ;
vec4 min2( const vec4 &v1, const vec4 &v2 ) ;
float min2( const vec4 &v1 ) ;
vec4 max2( const vec4 &v1, const vec4 &v2 ) ;
float max2( const vec4 &v1 ) ;

bool equals3( const vec4 &v1, const vec4 &v2, float e = 0.0f ) ;
float dot3( const vec4 &v1, const vec4 &v2 ) ;
vec4 cross3( const vec4 &v1, const vec4 &v2 ) ;
float length3( const vec4 &v1 ) ;
vec4 normalize3( const vec4 &v1 ) ;
vec4 lerp3( const vec4 &v1, const vec4 &v2, float f ) ;
vec4 clamp3( const vec4 &v1, float min, float max ) ;
vec4 min3( const vec4 &v1, const vec4 &v2 ) ;
float min3( const vec4 &v1 ) ;
vec4 max3( const vec4 &v1, const vec4 &v2 ) ;
float max3( const vec4 &v1 ) ;

bool equals4( const vec4 &v1, const vec4 &v2, float e = 0.0f ) ;
float dot4( const vec4 &v1, const vec4 &v2 ) ;
float length4( const vec4 &v1 ) ;
vec4 normalize4( const vec4 &v1 ) ;
vec4 lerp4( const vec4 &v1, const vec4 &v2, float f ) ;
vec4 clamp4( const vec4 &v1, float min, float max ) ;
vec4 min4( const vec4 &v1, const vec4 &v2 ) ;
float min4( const vec4 &v1 ) ;
vec4 max4( const vec4 &v1, const vec4 &v2 ) ;
float max4( const vec4 &v1 ) ;

//----------------------------------------------------------------
//  matrix functions
//----------------------------------------------------------------

mat4 operator+( const mat4 &m1, const mat4 &m2 ) ;
mat4 operator-( const mat4 &m1, const mat4 &m2 ) ;
mat4 operator-( const mat4 &m1 ) ;
mat4 operator*( const mat4 &m1, const mat4 &m2 ) ;
mat4 operator*( const mat4 &m1, float f ) ;
mat4 operator*( float f, const mat4 &m1 ) ;
vec4 operator*( const mat4 &m1, const vec4 &v1 ) ;
mat4 operator/( const mat4 &m1, float f ) ;
mat4 &operator+=( mat4 &m1, const mat4 &m2 ) ;
mat4 &operator-=( mat4 &m1, const mat4 &m2 ) ;
mat4 &operator*=( mat4 &m1, const mat4 &m2 ) ;
mat4 &operator*=( mat4 &m1, float f ) ;
mat4 &operator/=( mat4 &m1, float f ) ;

bool equals( const mat4 &m1, const mat4 &m2, float e = 0.0f ) ;
bool equals( const mat4 &m1, const mat4 &m2, float e, float ew ) ;
mat4 transpose( const mat4 &m1 ) ;
mat4 normalize( const mat4 &m1 ) ;
mat4 inverse( const mat4 &m1 ) ;
mat4 inverse_ortho( const mat4 &m1 ) ;
mat4 inverse_orthonormal( const mat4 &m1 ) ;
mat4 lerp( const mat4 &m1, const mat4 &m2, float f ) ;
vec4 transform2( const mat4 &m1, const vec4 &v1, float z = 1.0f, float w = 1.0f ) ;
vec4 project2( const mat4 &m1, const vec4 &v1, float z = 1.0f, float w = 1.0f ) ;
vec4 transform3( const mat4 &m1, const vec4 &v1, float w = 1.0f ) ;
vec4 project3( const mat4 &m1, const vec4 &v1, float w = 1.0f ) ;
vec4 transform4( const mat4 &m1, const vec4 &v1 ) ;
vec4 project4( const mat4 &m1, const vec4 &v1 ) ;

mat4 mat4_identity() ;
mat4 mat4_from_translate( const vec4 &v1 ) ;
mat4 mat4_from_translate( float x, float y, float z ) ;
mat4 mat4_from_rotate( const vec4 &axis, float angle ) ;
mat4 mat4_from_rotate( float x, float y, float z, float angle ) ;
mat4 mat4_from_rotx( float x ) ;
mat4 mat4_from_roty( float y ) ;
mat4 mat4_from_rotz( float z ) ;
mat4 mat4_from_rotzyx( const vec4 &angle ) ;
mat4 mat4_from_rotyxz( const vec4 &angle ) ;
mat4 mat4_from_rotxzy( const vec4 &angle ) ;
mat4 mat4_from_rotxyz( const vec4 &angle ) ;
mat4 mat4_from_rotyzx( const vec4 &angle ) ;
mat4 mat4_from_rotzxy( const vec4 &angle ) ;
mat4 mat4_from_rotzyx( float x, float y, float z ) ;
mat4 mat4_from_rotyxz( float x, float y, float z ) ;
mat4 mat4_from_rotxzy( float x, float y, float z ) ;
mat4 mat4_from_rotxyz( float x, float y, float z ) ;
mat4 mat4_from_rotyzx( float x, float y, float z ) ;
mat4 mat4_from_rotzxy( float x, float y, float z ) ;
mat4 mat4_from_scale( const vec4 &scale ) ;
mat4 mat4_from_scale( float x, float y, float z ) ;
mat4 mat4_from_viewport( float x, float y, float w, float h, float z_near = 0.0f, float z_far = 1.0f ) ;
mat4 mat4_from_perspective( float fovy, float aspect, float z_near, float z_far ) ;
mat4 mat4_from_frustum( float left, float right, float bottom, float top, float z_near, float z_far ) ;
mat4 mat4_from_ortho( float left, float right, float bottom, float top, float z_near, float z_far ) ;
mat4 mat4_from_lookat( const vec4 &eye, const vec4 &center, const vec4 &up ) ;
mat4 mat4_from_lookat( float ex, float ey, float ez, float cx, float cy, float cz, float ux, float uy, float uz ) ;
mat4 mat4_from_quat( const quat &q1 ) ;
mat4 mat4_from_quat( float x, float y, float z, float w ) ;
mat4 mat4_from_rect( const rect &q1 ) ;
mat4 mat4_from_rect( float x, float y, float w, float h ) ;

//----------------------------------------------------------------
//  quaternion functions
//----------------------------------------------------------------

quat operator*( const quat &q1, const quat &q2 ) ;
vec4 operator*( const quat &q1, const vec4 &v1 ) ;
quat &operator*=( quat &q1, const quat &q2 ) ;

bool equals( const quat &q1, const quat &q2, float e = 0.0f ) ;
float dot( const quat &q1, const quat &q2 ) ;
float length( const quat &q1 ) ;
quat normalize( const quat &q1 ) ;
quat inverse( const quat &q1 ) ;
quat slerp( const quat &q1, const quat &q2, float f ) ;
vec4 transform2( const quat &q1, const vec4 &v1, float z = 1.0f, float w = 1.0f ) ;
vec4 transform3( const quat &q1, const vec4 &v1, float w = 1.0f ) ;
vec4 transform4( const quat &q1, const vec4 &v1 ) ;

quat quat_from_mat4( const mat4 &m1 ) ;
quat quat_from_rotate( const vec4 &axis, float angle ) ;
quat quat_from_rotate( float x, float y, float z, float angle ) ;
quat quat_from_rotzyx( const vec4 &angle ) ;
quat quat_from_rotyxz( const vec4 &angle ) ;
quat quat_from_rotxzy( const vec4 &angle ) ;
quat quat_from_rotxyz( const vec4 &angle ) ;
quat quat_from_rotyzx( const vec4 &angle ) ;
quat quat_from_rotzxy( const vec4 &angle ) ;
quat quat_from_rotzyx( float x, float y, float z ) ;
quat quat_from_rotyxz( float x, float y, float z ) ;
quat quat_from_rotxzy( float x, float y, float z ) ;
quat quat_from_rotxyz( float x, float y, float z ) ;
quat quat_from_rotyzx( float x, float y, float z ) ;
quat quat_from_rotzxy( float x, float y, float z ) ;
vec4 quat_to_rotate( const quat &q1 ) ;
vec4 quat_to_rotzyx( const quat &q1 ) ;
vec4 quat_to_rotyxz( const quat &q1 ) ;

//----------------------------------------------------------------
//  rectangle functions
//----------------------------------------------------------------

rect operator*( const rect &r1, const rect &r2 ) ;
vec4 operator*( const rect &r1, const vec4 &v1 ) ;
rect &operator*=( rect &r1, const rect &r2 ) ;

bool equals( const rect &r1, const rect &r2, float e = 0.0f ) ;
rect inverse( const rect &r1 ) ;
rect lerp( const rect &r1, const rect &r2, float f ) ;
vec4 transform2( const rect &q1, const vec4 &v1, float z = 1.0f, float w = 1.0f ) ;
vec4 transform3( const rect &q1, const vec4 &v1, float w = 1.0f ) ;
vec4 transform4( const rect &q1, const vec4 &v1 ) ;

rect rect_from_mat4( const mat4 &m1 ) ;

//----------------------------------------------------------------
//  float functions
//----------------------------------------------------------------

bool equals( float f1, float f2, float e = 0.0f ) ;
bool equals( const float *f1, const float *f2, int count, float e = 0.0f ) ;


} // namespace mdx


//----------------------------------------------------------------
//  debug print
//----------------------------------------------------------------

std::ostream &operator<<( std::ostream &os, const mdx::vec2 &v ) ;
std::ostream &operator<<( std::ostream &os, const mdx::vec3 &v ) ;
std::ostream &operator<<( std::ostream &os, const mdx::vec4 &v ) ;
std::ostream &operator<<( std::ostream &os, const mdx::mat4 &m ) ;
std::ostream &operator<<( std::ostream &os, const mdx::quat &q ) ;
std::ostream &operator<<( std::ostream &os, const mdx::rect &r ) ;

#endif
