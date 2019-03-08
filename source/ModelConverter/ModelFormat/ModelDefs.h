#ifndef	MODEL_DEFS_H_INCLUDE
#define	MODEL_DEFS_H_INCLUDE

#include "ModelUtil/ModelUtil.h"

namespace mdx {

//----------------------------------------------------------------
//  magic number
//----------------------------------------------------------------

enum {
	MDX_FORMAT_SIGNATURE		= (0x2e4d4458),		// '.MDX'
	MDX_FORMAT_VERSION			= (0x312e3030),		// '1.00'
	MDX_FORMAT_STYLE_PSM		= (0x0050534d)		// 'PSM'
} ;

//----------------------------------------------------------------
//  scope id
//----------------------------------------------------------------

enum {
	MDX_SCOPE_BLOCK_TYPE		= MDX_SCOPE_CHUNK_TYPE,		// alias
	MDX_SCOPE_COMMAND_TYPE		= MDX_SCOPE_CHUNK_TYPE,		// alias

	MDX_SCOPE_ARRAYS_FORMAT		= 0x0101,
	MDX_SCOPE_FCURVE_FORMAT		= 0x0102,
	MDX_SCOPE_FCURVE_EXTRAP		= 0x0103,
	MDX_SCOPE_PRIM_MODE			= 0x0104,
	MDX_SCOPE_B_SPLINE_MODE		= 0x0105,
	MDX_SCOPE_ANIM_MODE			= 0x0106,
	MDX_SCOPE_REPEAT_MODE		= 0x0107,
	MDX_SCOPE_TEXTURE_TYPE		= 0x0108,
	MDX_SCOPE_FILTER_MAG		= 0x0109,
	MDX_SCOPE_FILTER_MIN		= 0x010a,
	MDX_SCOPE_WRAP_U			= 0x010b,
	MDX_SCOPE_WRAP_V			= 0x010c
} ;

//----------------------------------------------------------------
//  chunk type
//----------------------------------------------------------------

enum {
	MDX_SHORT_CHUNK				= 0x8000,

	MDX_BLOCK					= 0x0001,
	MDX_FILE					= 0x0002,
	MDX_MODEL					= 0x0010,
	MDX_BONE					= 0x0011,
	MDX_PART					= 0x0012,
	MDX_MESH					= 0x0013,
	MDX_ARRAYS					= 0x0014,
	MDX_MATERIAL				= 0x0016,
	MDX_LAYER					= 0x0017,
	MDX_TEXTURE					= 0x0018,
	MDX_MOTION					= 0x001b,
	MDX_FCURVE					= 0x001c,
	MDX_BLIND_BLOCK				= 0x001f,

	MDX_COMMAND					= 0x0040,
	MDX_DEFINE_ENUM				= 0x0041,
	MDX_DEFINE_BLOCK			= 0x0042,
	MDX_DEFINE_COMMAND			= 0x0043,

	MDX_FILE_NAME				= 0x0080,
	MDX_FILE_IMAGE				= 0x0081,
	MDX_BOUNDING_BOX			= 0x0082,
	MDX_BOUNDING_SPHERE			= 0x0083,

	MDX_PARENT_BONE				= 0x0440,
	MDX_VISIBILITY				= 0x0441,
	MDX_PIVOT					= 0x0442,
	MDX_TRANSLATE				= 0x0443,
	MDX_ROTATE					= 0x0444,
	MDX_ROTATE_XYZ				= 0x0445,
	MDX_ROTATE_YZX				= 0x0446,
	MDX_ROTATE_ZXY				= 0x0447,
	MDX_ROTATE_XZY				= 0x0448,
	MDX_ROTATE_YXZ				= 0x0449,
	MDX_ROTATE_ZYX				= 0x044a,
	MDX_SCALE					= 0x044b,
	MDX_BLEND_BONE				= 0x0460,
	MDX_MORPH_INDEX				= 0x0461,
	MDX_MORPH_WEIGHTS			= 0x0462,
	MDX_DRAW_PART				= 0x047f,

	MDX_VERTEX_OFFSET			= 0x0480,

	MDX_SET_MATERIAL			= 0x04c0,
	MDX_SET_ARRAYS				= 0x04c1,
	MDX_BLEND_INDICES			= 0x04c2,
	MDX_SUBDIVISION				= 0x04c3,
	MDX_KNOT_VECTOR_U			= 0x04c4,
	MDX_KNOT_VECTOR_V			= 0x04c5,
	MDX_DRAW_ARRAYS				= 0x04e0,
	MDX_DRAW_B_SPLINE			= 0x04e1,

	MDX_SET_PROGRAM				= 0x0580,
	MDX_DIFFUSE					= 0x0581,
	MDX_AMBIENT					= 0x0582,
	MDX_SPECULAR				= 0x0583,
	MDX_EMISSION				= 0x0584,
	MDX_REFLECTION				= 0x0585,
	MDX_REFRACTION				= 0x0586,
	MDX_BUMP					= 0x0587,
	MDX_OPACITY					= 0x0588,
	MDX_SHININESS				= 0x0589,

	MDX_SET_TEXTURE				= 0x05c0,
	MDX_MAP_TYPE				= 0x05c1,
	MDX_MAP_COORD				= 0x05c2,
	MDX_MAP_FACTOR				= 0x05c3,

	MDX_TEX_TYPE				= 0x0600,
	MDX_TEX_FILTER				= 0x0601,
	MDX_TEX_WRAP				= 0x0602,
	MDX_TEX_CROP				= 0x0603,
	MDX_UV_PIVOT				= 0x0620,
	MDX_UV_TRANSLATE			= 0x0621,
	MDX_UV_ROTATE				= 0x0622,
	MDX_UV_SCALE				= 0x0623,

	MDX_FRAME_LOOP				= 0x06c0,
	MDX_FRAME_RATE				= 0x06c1,
	MDX_FRAME_REPEAT			= 0x06c2,
	MDX_ANIMATE					= 0x06e0,

	MDX_BLIND_DATA				= 0x07c0
} ;

//----------------------------------------------------------------
//  vertex format
//----------------------------------------------------------------

#define MDX_VF_MASK				(0x000fffff)
#define MDX_VT_MASK				(0xfff00000)

#define MDX_VF_POSITIONS(n)		(((n)&1)<<0)
#define MDX_VF_NORMALS(n)		(((n)&3)<<1)
#define MDX_VF_COLORS(n)		(((n)&3)<<3)
#define MDX_VF_TEXCOORDS(n)		(((n)&7)<<5)
#define MDX_VF_WEIGHTS(n)		(((n)&255)<<8)
#define MDX_VF_INDICES			(1<<16)

#define MDX_VF_POSITION_COUNT(f) (((f)>>0)&1)
#define MDX_VF_NORMAL_COUNT(f)	(((f)>>1)&3)
#define MDX_VF_COLOR_COUNT(f)	(((f)>>3)&3)
#define MDX_VF_TEXCOORD_COUNT(f) (((f)>>5)&7)
#define MDX_VF_WEIGHT_COUNT(f)	(((f)>>8)&255)
#define MDX_VF_HAS_INDICES(f)	(((f)>>16)&1)

static inline int MDX_VF_INDICES_COUNT( int f )
{
	if ( !MDX_VF_HAS_INDICES(f) ) return 0 ;
	int n_weights = MDX_VF_WEIGHT_COUNT( f ) ;
	return ( n_weights == 0 ) ? 1 : n_weights ;
}

#define MDX_VF_POSITION			MDX_VF_POSITIONS(1)
#define MDX_VF_NORMAL			MDX_VF_NORMALS(1)
#define MDX_VF_COLOR			MDX_VF_COLORS(1)
#define MDX_VF_TEXCOORD			MDX_VF_TEXCOORDS(1)

#define MDX_VT_NONE				(-1)
#define MDX_VT_FLOAT			(0)
#define MDX_VT_HALF				(1)
#define MDX_VT_BYTE				(2)
#define MDX_VT_UBYTE			(3)

#define MDX_VT_POSITIONS(t)		(((t)&3)<<20)
#define MDX_VT_NORMALS(t)		(((t)&3)<<22)
#define MDX_VT_COLORS(t)		(((t)&3)<<24)
#define MDX_VT_TEXCOORDS(t)		(((t)&3)<<26)
#define MDX_VT_WEIGHTS(t)		(((t)&3)<<28)

#define MDX_VT_POSITION_TYPE(f)	(((f)>>20)&3)
#define MDX_VT_NORMAL_TYPE(f)	(((f)>>22)&3)
#define MDX_VT_COLOR_TYPE(f)	(((f)>>24)&3)
#define MDX_VT_TEXCOORD_TYPE(f)	(((f)>>26)&3)
#define MDX_VT_WEIGHT_TYPE(f)	(((f)>>28)&3)

//----------------------------------------------------------------
//  fcurve format
//----------------------------------------------------------------

enum {
	MDX_FCURVE_FLOAT16			= 0x0080,

	MDX_FCURVE_CONSTANT			= 0x0000,
	MDX_FCURVE_LINEAR			= 0x0001,
	MDX_FCURVE_HERMITE			= 0x0002,
	MDX_FCURVE_CUBIC			= 0x0003,
	MDX_FCURVE_SPHERICAL		= 0x0004
} ;

enum {
	MDX_FCURVE_HOLD				= 0x0000,
	MDX_FCURVE_CYCLE			= 0x0011,
	MDX_FCURVE_SHUTTLE			= 0x0022,
	MDX_FCURVE_REPEAT			= 0x0033,
	MDX_FCURVE_EXTEND			= 0x0044,

	MDX_FCURVE_EXTRAP_IN_MASK	= 0x000f,
	MDX_FCURVE_HOLD_IN			= 0x0000,
	MDX_FCURVE_CYCLE_IN			= 0x0001,
	MDX_FCURVE_SHUTTLE_IN		= 0x0002,
	MDX_FCURVE_REPEAT_IN		= 0x0003,
	MDX_FCURVE_EXTEND_IN		= 0x0004,

	MDX_FCURVE_EXTRAP_OUT_MASK	= 0x00f0,
	MDX_FCURVE_HOLD_OUT			= 0x0000,
	MDX_FCURVE_CYCLE_OUT		= 0x0010,
	MDX_FCURVE_SHUTTLE_OUT		= 0x0020,
	MDX_FCURVE_REPEAT_OUT		= 0x0030,
	MDX_FCURVE_EXTEND_OUT		= 0x0040
} ;

//----------------------------------------------------------------
//  primitive type
//----------------------------------------------------------------

enum {
	MDX_PRIM_POINTS				= 0x0000,
	MDX_PRIM_LINES				= 0x0001,
	MDX_PRIM_LINE_STRIP			= 0x0002,
	MDX_PRIM_TRIANGLES			= 0x0003,
	MDX_PRIM_TRIANGLE_STRIP		= 0x0004,
	MDX_PRIM_TRIANGLE_FAN		= 0x0005
} ;

enum {
	MDX_B_SPLINE_CLOSED_U		= 0x0003,
	MDX_B_SPLINE_CLOSED_V		= 0x0030,
	MDX_B_SPLINE_CLOSED_U_IN	= 0x0001,
	MDX_B_SPLINE_CLOSED_U_OUT	= 0x0002,
	MDX_B_SPLINE_CLOSED_V_IN	= 0x0010,
	MDX_B_SPLINE_CLOSED_V_OUT	= 0x0020
} ;

//----------------------------------------------------------------
//  animate mode
//----------------------------------------------------------------

#define	MDX_ANIM_MODE(idx,ofs)	(((idx)&255)|(((ofs)&255)<<8))
#define	MDX_ANIM_INDEX(f)		((f)&255)
#define	MDX_ANIM_OFFSET(f)		(((f)>>8)&255)

//----------------------------------------------------------------
//  frame repeat
//----------------------------------------------------------------

enum {
	MDX_REPEAT_HOLD				= 0,
	MDX_REPEAT_CYCLE			= 1
} ;

//----------------------------------------------------------------
//  texture type
//----------------------------------------------------------------

enum {
	MDX_TEXTURE_2D				= 0,
	MDX_TEXTURE_CUBE			= 1
} ;

//----------------------------------------------------------------
//  texture filter
//----------------------------------------------------------------

enum {
	MDX_FILTER_NEAREST			= 0,
	MDX_FILTER_LINEAR			= 1,
	MDX_FILTER_NEAREST_MIPMAP_NEAREST = 2,
	MDX_FILTER_LINEAR_MIPMAP_NEAREST = 3,
	MDX_FILTER_NEAREST_MIPMAP_LINEAR = 4,
	MDX_FILTER_LINEAR_MIPMAP_LINEAR = 5
} ;

//----------------------------------------------------------------
//  texture wrap
//----------------------------------------------------------------

enum {
	MDX_WRAP_CLAMP				= 0,
	MDX_WRAP_REPEAT				= 1
} ;

//----------------------------------------------------------------
//  header structure
//----------------------------------------------------------------

struct MDXHeader {
	unsigned int signature ;
	unsigned int version ;
	unsigned int style ;
	unsigned int option ;
} ;

//----------------------------------------------------------------
//  chunk structure
//----------------------------------------------------------------

struct MDXChunk {
	unsigned short type ;
	unsigned short name_end ;
	unsigned int args_end ;
	unsigned int data_end ;
	unsigned int child_end ;
} ;

struct MDXShortChunk {
	unsigned short type ;
	unsigned short args_end ;
} ;

static inline bool MDX_CHUNK_IS_SHORT( const MDXChunk *chunk )
{
	return ( ( MDX_SHORT_CHUNK & chunk->type ) != 0 ) ;
}

static inline int MDX_CHUNK_TYPE( const MDXChunk *chunk )
{
	return ( ~MDX_SHORT_CHUNK & chunk->type ) ;
}

static inline MDXChunk *MDX_CHUNK_NEXT( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) {
		return (MDXChunk *)( (char *)chunk + ( ( chunk->name_end + 3 ) & ~3 ) ) ;
	} else {
		return (MDXChunk *)( (char *)chunk + ( ( chunk->child_end + 3 ) & ~3 ) ) ;
	}
}

static inline char *MDX_CHUNK_NAME( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return "" ;
	return (char *)( chunk + 1 ) ;
}

static inline void *MDX_CHUNK_ARGS( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return (char *)chunk + 4 ;
	return (MDXChunk *)( (char *)chunk + ( ( chunk->name_end + 3 ) & ~3 ) ) ;
}

static inline void *MDX_CHUNK_DATA( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return MDX_CHUNK_ARGS( chunk ) ;
	return (MDXChunk *)( (char *)chunk + ( ( chunk->args_end + 3 ) & ~3 ) ) ;
}

static inline MDXChunk *MDX_CHUNK_CHILD( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return MDX_CHUNK_NEXT( chunk ) ;
	return (MDXChunk *)( (char *)chunk + ( ( chunk->data_end + 3 ) & ~3 ) ) ;
}

static inline int MDX_CHUNK_SIZE( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) {
		return ( chunk->name_end + 3 ) & ~3 ;
	} else {
		return ( chunk->child_end + 3 ) & ~3 ;
	}
}

static inline int MDX_CHUNK_TAGSIZE( const MDXChunk *chunk )
{
	return ( MDX_SHORT_CHUNK & chunk->type ) ? 4 : 16 ;
}

static inline int MDX_CHUNK_NAMESIZE( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return 0 ;
	return chunk->name_end - 16 ;
}

static inline int MDX_CHUNK_ARGSSIZE( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return chunk->name_end - 4 ;
	return chunk->args_end - ( ( chunk->name_end + 3 ) & ~3 ) ;
}

static inline int MDX_CHUNK_DATASIZE( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return 0 ;
	return chunk->data_end - ( ( chunk->args_end + 3 ) & ~3 ) ;
}

static inline int MDX_CHUNK_CHILDSIZE( const MDXChunk *chunk )
{
	if ( MDX_SHORT_CHUNK & chunk->type ) return 0 ;
	return ( ( chunk->child_end + 3 ) & ~3 ) - ( ( chunk->data_end + 3 ) & ~3 ) ;
}

static inline void *MDX_CHUNK_SKIPSTRING( const char *str, int align )
{
	str += strlen( str ) + 1 ;
	align -= 1 ;
	return (void *)( ( (int)str + align ) & ~align ) ;
}

//----------------------------------------------------------------
//  block args
//----------------------------------------------------------------

struct MDXArrays {
	int format ;
	int stride ;
	int n_verts ;
} ;

struct MDXFCurve {
	int format ;
	int extrap ;
	int n_dims ;
	int n_keys ;
} ;

//----------------------------------------------------------------
//  common commands
//----------------------------------------------------------------

struct MDXFileName {
	char name[ 1 ] ;
} ;

struct MDXFileImage {
	int size ;
	int data[ 1 ] ;
} ;

struct MDXBoundingBox {
	vec3 lower ;
	vec3 upper ;
} ;

struct MDXBoundingSphere {
	vec3 center ;
	float radius ;
} ;

//----------------------------------------------------------------
//  bone commands
//----------------------------------------------------------------

struct MDXParentBone {
	int bone ;
} ;

struct MDXVisibility {
	int visibility ;
} ;

struct MDXPivot {
	vec3 pivot ;
} ;

struct MDXTranslate {
	vec3 translate ;
} ;

struct MDXRotate {
	quat rotate ;
} ;

struct MDXRotateXYZ {
	vec3 rotate ;
} ;

struct MDXRotateYZX {
	vec3 rotate ;
} ;

struct MDXRotateZXY {
	vec3 rotate ;
} ;

struct MDXRotateXZY {
	vec3 rotate ;
} ;

struct MDXRotateYXZ {
	vec3 rotate ;
} ;

struct MDXRotateZYX {
	vec3 rotate ;
} ;

struct MDXScale {
	vec3 scale ;
} ;

struct MDXBlendBone {
	int bone ;
	mat4 offset ;
} ;

struct MDXMorphIndex {
	float index ;
} ;

struct MDXMorphWeights {
	int n_weights ;
	float weights[ 1 ] ;
} ;

struct MDXDrawPart {
	int part ;
} ;

//----------------------------------------------------------------
//  part commands
//----------------------------------------------------------------

struct MDXVertexOffset {
	int format ;
	float offset[ 1 ] ;
} ;

//----------------------------------------------------------------
//  mesh commands
//----------------------------------------------------------------

struct MDXSetMaterial {
	int material ;
} ;

struct MDXSetArrays {
	int arrays ;
} ;

struct MDXBlendIndices {
	int n_indices ;
	int indices[ 1 ] ;
} ;

struct MDXSubdivision {
	float div_u ;
	float div_v ;
} ;

struct MDXKnotVectorU {
	int n_knots ;
	float knots[ 1 ] ;
} ;

struct MDXKnotVectorV {
	int n_knots ;
	float knots[ 1 ] ;
} ;

struct MDXDrawArrays {
	int mode ;
	int n_verts ;
	int n_prims ;
	unsigned short indices[ 1 ] ;
} ;

struct MDXDrawBSpline {
	int mode ;
	int width ;
	int height ;
	unsigned short indices[ 1 ] ;
} ;

//----------------------------------------------------------------
//  material commands
//----------------------------------------------------------------

struct MDXDiffuse {
	vec3 color ;
} ;

struct MDXAmbient {
	vec3 color ;
} ;

struct MDXSpecular {
	vec3 color ;
} ;

struct MDXEmission {
	vec3 color ;
} ;

struct MDXReflection {
	vec3 color ;
} ;

struct MDXRefraction {
	vec3 color ;
} ;

struct MDXBump {
	float bump ;
} ;

struct MDXOpacity {
	float opacity ;
} ;

struct MDXShininess {
	float shininess ;
} ;

//----------------------------------------------------------------
//  layer commands
//----------------------------------------------------------------

struct MDXSetTexture {
	int texture ;
} ;

struct MDXMapType {
	int type ;
} ;

struct MDXMapCoord {
	int coord ;
} ;

struct MDXMapFactor {
	float factor ;
} ;

//----------------------------------------------------------------
//  texture commands
//----------------------------------------------------------------

struct MDXTexType {
	int type ;
} ;

struct MDXTexFilter {
	int mag ;
	int min ;
} ;

struct MDXTexWrap {
	int wrap_u ;
	int wrap_v ;
} ;

struct MDXTexCrop {
	rect crop ;
} ;

struct MDXUVPivot {
	vec2 pivot ;
} ;

struct MDXUVTranslate {
	vec2 translate ;
} ;

struct MDXUVRotate {
	float rotate ;
} ;

struct MDXUVScale {
	vec2 scale ;
} ;

//----------------------------------------------------------------
//  motion commands
//----------------------------------------------------------------

struct MDXFrameLoop {
	float start ;
	float end ;
} ;

struct MDXFrameRate {
	float fps ;
} ;

struct MDXFrameRepeat {
	int mode ;
} ;

struct MDXAnimate {
	int block ;
	int cmd ;
	int index ;
	int fcurve ;
} ;

//----------------------------------------------------------------
//  other commands
//----------------------------------------------------------------

struct MDXBlindData {
	char name[ 1 ] ;
	// char pad[ n ] ;
	// void data[ m ] ;			// use MDX_CHUNK_SKIPSTRING()
} ;


} // namespace mdx

#endif
