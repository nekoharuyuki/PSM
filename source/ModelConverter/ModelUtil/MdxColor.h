#ifndef	MDX_COLOR_H_INCLUDE
#define	MDX_COLOR_H_INCLUDE

namespace mdx {

struct rgba8888 ;
struct rgba8880 ;
struct rgba5650 ;
struct rgba5551 ;
struct rgba5550 ;
struct rgba4444 ;

struct vec4 ;		//  MdxMath.h

//----------------------------------------------------------------
//  rgba8888
//----------------------------------------------------------------

struct rgba8888 {
	union {
		unsigned int bits ;
		struct {
			unsigned int r: 8 ;
			unsigned int g: 8 ;
			unsigned int b: 8 ;
			unsigned int a: 8 ;
		} ;
	} ;
public:
	rgba8888( unsigned int bits = 0 ) ;
	rgba8888( int r, int g, int b, int a = 255 ) ;
	rgba8888( const vec4 &v ) ;
	rgba8888( const rgba8880 &c ) ;
	rgba8888( const rgba5650 &c ) ;
	rgba8888( const rgba5551 &c ) ;
	rgba8888( const rgba5550 &c ) ;
	rgba8888( const rgba4444 &c ) ;
	rgba8888 &set( int r, int g, int b, int a = 255 ) ;
	rgba8888 &operator=( unsigned int bits ) ;
	rgba8888 &operator=( const vec4 &v ) ;
	rgba8888 &operator=( const rgba8880 &c ) ;
	rgba8888 &operator=( const rgba5650 &c ) ;
	rgba8888 &operator=( const rgba5551 &c ) ;
	rgba8888 &operator=( const rgba5550 &c ) ;
	rgba8888 &operator=( const rgba4444 &c ) ;
	operator unsigned int() const ;
	rgba8888 bgra() const ;
} ;

//----------------------------------------------------------------
//  rgba8880
//----------------------------------------------------------------

struct rgba8880 {
	unsigned char r ;
	unsigned char g ;
	unsigned char b ;
public:
	rgba8880( unsigned int bits = 0 ) ;
	rgba8880( int r, int g, int b, int a = 0 ) ;
	rgba8880( const rgba8888 &c ) ;
	rgba8880 &set( int r, int g, int b, int a = 0 ) ;
	rgba8880 &operator=( unsigned int bits ) ;
	rgba8880 &operator=( const rgba8888 &c ) ;
	operator unsigned int() const ;
	rgba8880 bgra() const ;
} ;

//----------------------------------------------------------------
//  rgba5650
//----------------------------------------------------------------

struct rgba5650 {
	union {
		unsigned short bits ;
		struct {
			unsigned short r: 5 ;
			unsigned short g: 6 ;
			unsigned short b: 5 ;
		} ;
	} ;
public:
	rgba5650( unsigned short bits = 0 ) ;
	rgba5650( int r, int g, int b, int a = 0 ) ;
	rgba5650( const rgba8888 &c ) ;
	rgba5650 &set( int r, int g, int b, int a = 0 ) ;
	rgba5650 &operator=( unsigned short bits ) ;
	rgba5650 &operator=( const rgba8888 &c ) ;
	operator unsigned short() const ;
	rgba5650 bgra() const ;
} ;

//----------------------------------------------------------------
//  rgba5551
//----------------------------------------------------------------

struct rgba5551 {
	union {
		unsigned short bits ;
		struct {
			unsigned short r: 5 ;
			unsigned short g: 5 ;
			unsigned short b: 5 ;
			unsigned short a: 1 ;
		} ;
	} ;
public:
	rgba5551( unsigned short bits = 0 ) ;
	rgba5551( int r, int g, int b, int a = 1 ) ;
	rgba5551( const rgba8888 &c ) ;
	rgba5551( const rgba5550 &c ) ;
	rgba5551 &set( int r, int g, int b, int a = 1 ) ;
	rgba5551 &operator=( unsigned short bits ) ;
	rgba5551 &operator=( const rgba8888 &c ) ;
	rgba5551 &operator=( const rgba5550 &c ) ;
	operator unsigned short() const ;
	rgba5551 bgra() const ;
} ;

//----------------------------------------------------------------
//  rgba5550
//----------------------------------------------------------------

struct rgba5550 {
	union {
		unsigned short bits ;
		struct {
			unsigned short r: 5 ;
			unsigned short g: 5 ;
			unsigned short b: 5 ;
		} ;
	} ;
public:
	rgba5550( unsigned short bits = 0 ) ;
	rgba5550( int r, int g, int b, int a = 0 ) ;
	rgba5550( const rgba8888 &c ) ;
	rgba5550( const rgba5551 &c ) ;
	rgba5550 &set( int r, int g, int b, int a = 0 ) ;
	rgba5550 &operator=( unsigned short bits ) ;
	rgba5550 &operator=( const rgba8888 &c ) ;
	rgba5550 &operator=( const rgba5551 &c ) ;
	operator unsigned short() const ;
	rgba5550 bgra() const ;
} ;

//----------------------------------------------------------------
//  rgba4444
//----------------------------------------------------------------

struct rgba4444 {
	union {
		unsigned short bits ;
		struct {
			unsigned short r: 4 ;
			unsigned short g: 4 ;
			unsigned short b: 4 ;
			unsigned short a: 4 ;
		} ;
	} ;
public:
	rgba4444( unsigned short bits = 0 ) ;
	rgba4444( int r, int g, int b, int a = 15 ) ;
	rgba4444( const rgba8888 &c ) ;
	rgba4444 &set( int r, int g, int b, int a = 15 ) ;
	rgba4444 &operator=( unsigned short bits ) ;
	rgba4444 &operator=( const rgba8888 &c ) ;
	operator unsigned short() const ;
	rgba4444 bgra() const ;
} ;


} // namespace mdx

#endif
