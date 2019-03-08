#ifndef MODEL_TOOL_H_INCLUDE
#define MODEL_TOOL_H_INCLUDE

#include "ModelUtil/ModelUtil.h"
#include "MdxDefs.h"

#ifdef WIN32
#ifdef MODELTOOL_EXPORTS
#define MODELTOOL_API __declspec(dllexport)
#else // MODELTOOL_EXPORTS
#define MODELTOOL_API __declspec(dllimport)
#endif // MODELTOOL_EXPORTS
#else // WIN32
#define MODELTOOL_API
#endif // WIN32

namespace mdx {}
using namespace mdx ;

namespace mdx {

//----------------------------------------------------------------
//  classes
//----------------------------------------------------------------

class MODELTOOL_API MdxBase ;
class MODELTOOL_API MdxModule ;
class MODELTOOL_API MdxShell ;
class MODELTOOL_API MdxChunk ;
class MODELTOOL_API MdxProc ;
class MODELTOOL_API MdxCreatorProc ;
class MODELTOOL_API MdxModifierProc ;
class MODELTOOL_API MdxImporterProc ;
class MODELTOOL_API MdxExporterProc ;
class MODELTOOL_API MdxFormat ;
class MODELTOOL_API MdxBlock ;
class MODELTOOL_API MdxCommand ;

} // namespace mdx

//----------------------------------------------------------------
//  headers
//----------------------------------------------------------------

#include "MdxBase.h"
#include "MdxModule.h"
#include "MdxShell.h"
#include "MdxChunk.h"
#include "MdxChunkList.h"
#include "MdxProc.h"
#include "MdxFormat.h"

//----------------------------------------------------------------
//  typedefs
//----------------------------------------------------------------

typedef MdxChunkList<MdxChunk> MdxChunks ;
typedef MdxChunkList<MdxBlock> MdxBlocks ;
typedef MdxChunkList<MdxCommand> MdxCommands ;


#endif
