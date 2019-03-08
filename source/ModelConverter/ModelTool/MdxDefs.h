#ifndef	MDX_DEFS_H_INCLUDE
#define	MDX_DEFS_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxTool magic number
//----------------------------------------------------------------

#define MDX_LIBRARY_VERSION		"1.00"
#define MDX_FORMAT_ID			((int)'.MDX')
#define MDX_PROC_NS				"mdx"

//----------------------------------------------------------------
//  MdxShell output mask
//----------------------------------------------------------------

enum {
	MDX_OUTPUT_ALL		= 0xffff,
	MDX_OUTPUT_NONE		= 0x0000,
	MDX_OUTPUT_ERROR	= 0x0001,
	MDX_OUTPUT_WARNING	= 0x0002,
	MDX_OUTPUT_MESSAGE	= 0x0004,
	MDX_OUTPUT_DEBUG	= 0x0008,
} ;

//----------------------------------------------------------------
//  MdxProc return value
//----------------------------------------------------------------

enum {
	MDX_EVAL_ERROR		= 0,
	MDX_EVAL_OK		= 1,
	MDX_EVAL_RETURN		= 2,
	MDX_EVAL_BREAK		= 3,
	MDX_EVAL_CONTINUE	= 4,
} ;

//----------------------------------------------------------------
//  MdxFormat scope id
//----------------------------------------------------------------

enum {
	MDX_SCOPE_RESERVED	= 0x0000,	// 0000-0fff : reserved
	MDX_SCOPE_PRIVATE	= 0x1000,	// 1000-3fff : private use
	MDX_SCOPE_PUBLIC	= 0x4000,	// 4000-7fff : public use
	MDX_SCOPE_VOLATILE	= 0x8000,	// 8000-ffff : volatile

	//  reserved

	MDX_SCOPE_WORD_TYPE	= 0x0001,	// word types
	MDX_SCOPE_CHUNK_TYPE	= 0x0002,	// chunk types
} ;

//----------------------------------------------------------------
//  MdxChunk reference index
//----------------------------------------------------------------

#define MDX_REF_INDEX(level,type,rank) ((((level)<<12)&MDX_REF_LEVEL_MASK)|(((type)<<16)&MDX_REF_TYPE_MASK)|((rank)&MDX_REF_RANK_MASK))

#define MDX_REF_LEVEL(index) (((index)&MDX_REF_LEVEL_MASK)>>12)
#define MDX_REF_TYPE(index) (((index)&MDX_REF_TYPE_MASK)>>16)
#define MDX_REF_RANK(index) ((index)&MDX_REF_RANK_MASK)

enum {
	MDX_REF_LEVEL_MASK	= 0x0000f000,
	MDX_REF_TYPE_MASK	= 0x7fff0000,
	MDX_REF_RANK_MASK	= 0x00000fff,
} ;

//----------------------------------------------------------------
//  MdxChunk word description
//----------------------------------------------------------------

#define	MDX_WORD_DESC(type,scope,flags) ((type)|(scope)|(flags))

#define MDX_WORD_FLAGS(desc) ((desc)&MDX_WORD_FLAGS_MASK)
#define MDX_WORD_SCOPE(desc) ((desc)&MDX_WORD_SCOPE_MASK)
#define MDX_WORD_TYPE(desc) ((desc)&MDX_WORD_TYPE_MASK)

#define MDX_WORD_CLASS(desc) ((desc)&MDX_WORD_CLASS_MASK)
#define MDX_WORD_VALUE_TYPE(desc) ((desc)&MDX_WORD_VALUE_TYPE_MASK)
#define MDX_WORD_VALUE_CLASS(desc) ((desc)&MDX_WORD_VALUE_CLASS_MASK)

enum {
	//  masks

	MDX_WORD_FLAGS_MASK	= 0x7f000000,
	MDX_WORD_TYPE_MASK	= 0x00ff0000,
	MDX_WORD_SCOPE_MASK	= 0x0000ffff,

	MDX_WORD_CLASS_MASK	= 0x00f00000,
	MDX_WORD_VALUE_TYPE_MASK = 0x003f0000,
	MDX_WORD_VALUE_CLASS_MASK = 0x00300000,

	//  flags

	MDX_WORD_VAR_TYPE	= 0x01000000,
	MDX_WORD_VAR_SCOPE	= 0x02000000,
	MDX_WORD_VAR_COUNT	= 0x04000000,
	MDX_WORD_FMT_NEWLINE	= 0x10000000,
	MDX_WORD_FMT_HEX	= 0x20000000,

	//  class

	MDX_WORD_INT		= 0x00000000,
	MDX_WORD_FLOAT		= 0x00100000,
	MDX_WORD_STRING		= 0x00200000,
	MDX_WORD_ENUM		= 0x00400000,
	MDX_WORD_REF		= 0x00600000,

	//  value type

	MDX_WORD_INT32		= 0x00000000,
	MDX_WORD_INT16		= 0x00010000,
	MDX_WORD_INT8		= 0x00020000,
	MDX_WORD_UINT32		= 0x00040000,
	MDX_WORD_UINT16		= 0x00050000,
	MDX_WORD_UINT8		= 0x00060000,
	MDX_WORD_FLOAT32	= 0x00100000,
	MDX_WORD_FLOAT16	= 0x00110000,
	MDX_WORD_STRING8	= 0x00200000,
} ;


} // namespace mdx

#endif
