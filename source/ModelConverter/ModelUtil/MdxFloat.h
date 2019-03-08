#ifndef	MDX_FLOAT_H_INCLUDE
#define	MDX_FLOAT_H_INCLUDE

namespace mdx {

struct float32 ;	//  1:8:23
struct float16 ;	//  1:5:10

struct fixed16 ;	//  signed short / 32768
struct ufixed16 ;	//  unsigned short / 32768
struct fixed8 ;		//  signed char / 128
struct ufixed8 ;	//  unsigned char / 128

struct fixed16n ;	//  signed short / 32767
struct ufixed16n ;	//  unsigned short / 65535
struct fixed8n ;	//  signed char / 127
struct ufixed8n ;	//  unsigned char / 255

//----------------------------------------------------------------
//  floating point number
//----------------------------------------------------------------

struct float32 {
	union {
		unsigned int bits ;
		float val ;
		struct {
			unsigned int f: 23 ;
			unsigned int e: 8 ;
			unsigned int s: 1 ;
		} ;
	} ;
public:
	float32() ;
	float32( float val ) ;
	float32 &operator=( float val ) ;
	operator float() const ;
} ;

struct float16 {
	union {
		signed short bits ;
		struct {
			unsigned short f: 10 ;
			unsigned short e: 5 ;
			unsigned short s: 1 ;
		} ;
	} ;
public:
	float16() ;
	float16( float val ) ;
	float16 &operator=( float val ) ;
	operator float() const ;
} ;

//----------------------------------------------------------------
//  fixed point number ( n-1 bits )
//----------------------------------------------------------------

struct fixed16 {
	signed short bits ;
public:
	fixed16() ;
	fixed16( float val ) ;
	fixed16 &operator=( float val ) ;
	operator float() const ;
} ;

struct ufixed16 {
	unsigned short bits ;
public:
	ufixed16() ;
	ufixed16( float val ) ;
	ufixed16 &operator=( float val ) ;
	operator float() const ;
} ;

struct fixed8 {
	signed char bits ;
public:
	fixed8() ;
	fixed8( float val ) ;
	fixed8 &operator=( float val ) ;
	operator float() const ;
} ;

struct ufixed8 {
	unsigned char bits ;
public:
	ufixed8() ;
	ufixed8( float val ) ;
	ufixed8 &operator=( float val ) ;
	operator float() const ;
} ;

//----------------------------------------------------------------
//  fixed point number ( normalized )
//----------------------------------------------------------------

struct fixed16n {
	signed short bits ;
public:
	fixed16n() ;
	fixed16n( float val ) ;
	fixed16n &operator=( float val ) ;
	operator float() const ;
} ;

struct ufixed16n {
	unsigned short bits ;
public:
	ufixed16n() ;
	ufixed16n( float val ) ;
	ufixed16n &operator=( float val ) ;
	operator float() const ;
} ;

struct fixed8n {
	signed char bits ;
public:
	fixed8n() ;
	fixed8n( float val ) ;
	fixed8n &operator=( float val ) ;
	operator float() const ;
} ;

struct ufixed8n {
	unsigned char bits ;
public:
	ufixed8n() ;
	ufixed8n( float val ) ;
	ufixed8n &operator=( float val ) ;
	operator float() const ;
} ;


} // namespace mdx

#endif
