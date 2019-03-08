#ifndef	MODEL_FORMAT_H_INCLUDE
#define	MODEL_FORMAT_H_INCLUDE

#include "ModelTool/ModelTool.h"
#include "ModelDefs.h"

#ifdef WIN32
#ifdef MODELFORMAT_EXPORTS
#define MODELFORMAT_API __declspec(dllexport)
#else // MODELFORMAT_EXPORTS
#define MODELFORMAT_API __declspec(dllimport)
#endif // MODELFORMAT_EXPORTS
#else // WIN32
#define MODELFORMAT_API
#endif // WIN32

namespace mdx {}
using namespace mdx ;

namespace mdx {

//----------------------------------------------------------------
//  module entry
//----------------------------------------------------------------

extern "C" MODELFORMAT_API class MdxModule_ModelFormat MdxModule_ModelFormat ;

class MdxModule_ModelFormat : public MdxModule {
public:
	virtual const char *GetLibraryVersion() const { return MDX_LIBRARY_VERSION ; }
	virtual const char *GetModuleName() const { return "ModelFormat" ; }
	virtual const char *GetModuleVersion() const { return "1.00" ; }
	virtual const char *GetModuleCopyright() const { return "(c) SCEI" ; }
	virtual const char *GetModuleDescription() const { return "Model format module" ; }
	virtual bool InitModule( MdxShell *shell, int argc, const char **argv ) ;
	virtual void ExitModule() ;
} ;

//----------------------------------------------------------------
//  classes
//----------------------------------------------------------------

class MODELFORMAT_API MdxFile ;
class MODELFORMAT_API MdxModel ;
class MODELFORMAT_API MdxBone ;
class MODELFORMAT_API MdxPart ;
class MODELFORMAT_API MdxMesh ;
class MODELFORMAT_API MdxArrays ;
class MODELFORMAT_API MdxVertex ;
class MODELFORMAT_API MdxMaterial ;
class MODELFORMAT_API MdxLayer ;
class MODELFORMAT_API MdxTexture ;
class MODELFORMAT_API MdxMotion ;
class MODELFORMAT_API MdxFCurve ;
class MODELFORMAT_API MdxKeyFrame ;
class MODELFORMAT_API MdxBlindBlock ;
class MODELFORMAT_API MdxUnknownBlock ;

class MODELFORMAT_API MdxDefineEnum ;
class MODELFORMAT_API MdxDefineBlock ;
class MODELFORMAT_API MdxDefineCommand ;

class MODELFORMAT_API MdxFileName ;
class MODELFORMAT_API MdxFileImage ;
class MODELFORMAT_API MdxBoundingBox ;
class MODELFORMAT_API MdxBoundingSphere ;

class MODELFORMAT_API MdxParentBone ;
class MODELFORMAT_API MdxVisibility ;
class MODELFORMAT_API MdxPivot ;
class MODELFORMAT_API MdxTranslate ;
class MODELFORMAT_API MdxRotate ;
class MODELFORMAT_API MdxRotateXYZ ;
class MODELFORMAT_API MdxRotateYZX ;
class MODELFORMAT_API MdxRotateZXY ;
class MODELFORMAT_API MdxRotateXZY ;
class MODELFORMAT_API MdxRotateYXZ ;
class MODELFORMAT_API MdxRotateZYX ;
class MODELFORMAT_API MdxScale ;
class MODELFORMAT_API MdxBlendBone ;
class MODELFORMAT_API MdxMorphIndex ;
class MODELFORMAT_API MdxMorphWeights ;
class MODELFORMAT_API MdxDrawPart ;

class MODELFORMAT_API MdxVertexOffset ;

class MODELFORMAT_API MdxSetMaterial ;
class MODELFORMAT_API MdxSetArrays ;
class MODELFORMAT_API MdxBlendIndices ;
class MODELFORMAT_API MdxSubdivision ;
class MODELFORMAT_API MdxKnotVectorU ;
class MODELFORMAT_API MdxKnotVectorV ;
class MODELFORMAT_API MdxPrimCommand ;
class MODELFORMAT_API MdxDrawArrays ;
class MODELFORMAT_API MdxDrawBSpline ;

class MODELFORMAT_API MdxColorCommand ;
class MODELFORMAT_API MdxDiffuse ;
class MODELFORMAT_API MdxSpecular ;
class MODELFORMAT_API MdxEmission ;
class MODELFORMAT_API MdxAmbient ;
class MODELFORMAT_API MdxReflection ;
class MODELFORMAT_API MdxRefraction ;
class MODELFORMAT_API MdxBump ;
class MODELFORMAT_API MdxOpacity ;
class MODELFORMAT_API MdxShininess ;

class MODELFORMAT_API MdxSetTexture ;
class MODELFORMAT_API MdxMapType ;
class MODELFORMAT_API MdxMapCoord ;
class MODELFORMAT_API MdxMapFactor ;

class MODELFORMAT_API MdxTexType ;
class MODELFORMAT_API MdxTexFilter ;
class MODELFORMAT_API MdxTexWrap ;
class MODELFORMAT_API MdxTexCrop ;
class MODELFORMAT_API MdxUVPivot ;
class MODELFORMAT_API MdxUVTranslate ;
class MODELFORMAT_API MdxUVRotate ;
class MODELFORMAT_API MdxUVScale ;

class MODELFORMAT_API MdxFrameLoop ;
class MODELFORMAT_API MdxFrameRate ;
class MODELFORMAT_API MdxFrameRepeat ;
class MODELFORMAT_API MdxAnimate ;

class MODELFORMAT_API MdxBlindData ;
class MODELFORMAT_API MdxUnknownCommand ;

} // namespace mdx

//----------------------------------------------------------------
//  headers
//----------------------------------------------------------------

#include "MdxFile.h"
#include "MdxModel.h"
#include "MdxBone.h"
#include "MdxPart.h"
#include "MdxArrays.h"
#include "MdxMaterial.h"
#include "MdxTexture.h"
#include "MdxMotion.h"
#include "MdxFCurve.h"
#include "MdxUnknown.h"


#endif
