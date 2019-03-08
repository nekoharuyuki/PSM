#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  module entry
//----------------------------------------------------------------

extern "C" { class MdxModule_ModelFormat MdxModule_ModelFormat ; }	// alias

bool MdxModule_ModelFormat::InitModule( MdxShell *shell, int argc, const char **argv )
{
	MdxFormat *format = new MdxFormat() ;
	shell->SetFormat( 0, format ) ;

	//  templates

	format->SetTemplate( 0, new MdxFile ) ;
	format->SetTemplate( 0, new MdxModel ) ;
	format->SetTemplate( 0, new MdxBone ) ;
	format->SetTemplate( 0, new MdxPart ) ;
	format->SetTemplate( 0, new MdxMesh ) ;
	format->SetTemplate( 0, new MdxArrays ) ;
	format->SetTemplate( 0, new MdxMaterial ) ;
	format->SetTemplate( 0, new MdxLayer ) ;
	format->SetTemplate( 0, new MdxTexture ) ;
	format->SetTemplate( 0, new MdxMotion ) ;
	format->SetTemplate( 0, new MdxFCurve ) ;
	format->SetTemplate( 0, new MdxBlindBlock ) ;

	format->SetTemplate( 0, new MdxDefineEnum ) ;
	format->SetTemplate( 0, new MdxDefineBlock ) ;
	format->SetTemplate( 0, new MdxDefineCommand ) ;

	format->SetTemplate( 0, new MdxFileName ) ;
	format->SetTemplate( 0, new MdxFileImage ) ;
	format->SetTemplate( 0, new MdxBoundingBox ) ;
	format->SetTemplate( 0, new MdxBoundingSphere ) ;

	format->SetTemplate( 0, new MdxParentBone ) ;
	format->SetTemplate( 0, new MdxVisibility ) ;
	format->SetTemplate( 0, new MdxPivot ) ;
	format->SetTemplate( 0, new MdxTranslate ) ;
	format->SetTemplate( 0, new MdxRotate ) ;
	format->SetTemplate( 0, new MdxRotateXYZ ) ;
	format->SetTemplate( 0, new MdxRotateYZX ) ;
	format->SetTemplate( 0, new MdxRotateZXY ) ;
	format->SetTemplate( 0, new MdxRotateXZY ) ;
	format->SetTemplate( 0, new MdxRotateYXZ ) ;
	format->SetTemplate( 0, new MdxRotateZYX ) ;
	format->SetTemplate( 0, new MdxScale ) ;
	format->SetTemplate( 0, new MdxBlendBone ) ;
	format->SetTemplate( 0, new MdxMorphIndex ) ;
	format->SetTemplate( 0, new MdxMorphWeights ) ;
	format->SetTemplate( 0, new MdxDrawPart ) ;

	format->SetTemplate( 0, new MdxVertexOffset ) ;

	format->SetTemplate( 0, new MdxSetMaterial ) ;
	format->SetTemplate( 0, new MdxSetArrays ) ;
	format->SetTemplate( 0, new MdxBlendIndices ) ;
	format->SetTemplate( 0, new MdxSubdivision ) ;
	format->SetTemplate( 0, new MdxKnotVectorU ) ;
	format->SetTemplate( 0, new MdxKnotVectorV ) ;
	format->SetTemplate( 0, new MdxDrawArrays ) ;
	format->SetTemplate( 0, new MdxDrawBSpline ) ;

	format->SetTemplate( 0, new MdxDiffuse ) ;
	format->SetTemplate( 0, new MdxSpecular ) ;
	format->SetTemplate( 0, new MdxEmission ) ;
	format->SetTemplate( 0, new MdxAmbient ) ;
	format->SetTemplate( 0, new MdxReflection ) ;
	format->SetTemplate( 0, new MdxRefraction ) ;
	format->SetTemplate( 0, new MdxBump ) ;
	format->SetTemplate( 0, new MdxOpacity ) ;
	format->SetTemplate( 0, new MdxShininess ) ;

	format->SetTemplate( 0, new MdxSetTexture ) ;
	format->SetTemplate( 0, new MdxMapType ) ;
	format->SetTemplate( 0, new MdxMapCoord ) ;
	format->SetTemplate( 0, new MdxMapFactor ) ;

	format->SetTemplate( 0, new MdxTexType ) ;
	format->SetTemplate( 0, new MdxTexFilter ) ;
	format->SetTemplate( 0, new MdxTexWrap ) ;
	format->SetTemplate( 0, new MdxTexCrop ) ;
	format->SetTemplate( 0, new MdxUVPivot ) ;
	format->SetTemplate( 0, new MdxUVTranslate ) ;
	format->SetTemplate( 0, new MdxUVRotate ) ;
	format->SetTemplate( 0, new MdxUVScale ) ;

	format->SetTemplate( 0, new MdxFrameLoop ) ;
	format->SetTemplate( 0, new MdxFrameRate ) ;
	format->SetTemplate( 0, new MdxFrameRepeat ) ;
	format->SetTemplate( 0, new MdxAnimate ) ;

	format->SetTemplate( 0, new MdxBlindData ) ;

	//  scopes

	format->SetScope( "ARRAYS_FORMAT", MDX_SCOPE_ARRAYS_FORMAT ) ;
	format->SetScope( "FCURVE_FORMAT", MDX_SCOPE_FCURVE_FORMAT ) ;
	format->SetScope( "FCURVE_EXTRAP", MDX_SCOPE_FCURVE_EXTRAP ) ;
	format->SetScope( "PRIM_MODE", MDX_SCOPE_PRIM_MODE ) ;
	format->SetScope( "B_SPLINE_MODE", MDX_SCOPE_B_SPLINE_MODE ) ;
	format->SetScope( "ANIM_MODE", MDX_SCOPE_ANIM_MODE ) ;
	format->SetScope( "REPEAT_MODE", MDX_SCOPE_REPEAT_MODE ) ;
	format->SetScope( "TEXTURE_TYPE", MDX_SCOPE_TEXTURE_TYPE ) ;
	format->SetScope( "FILTER_MAG", MDX_SCOPE_FILTER_MAG ) ;
	format->SetScope( "FILTER_MIN", MDX_SCOPE_FILTER_MIN ) ;
	format->SetScope( "WRAP_U", MDX_SCOPE_WRAP_U ) ;
	format->SetScope( "WRAP_V", MDX_SCOPE_WRAP_V ) ;

	//  symbols

	int scope = MDX_SCOPE_ARRAYS_FORMAT ;
	format->SetSymbol( scope, "POSITION", MDX_VF_POSITION ) ;
	format->SetSymbol( scope, "NORMAL", MDX_VF_NORMAL ) ;
	format->SetSymbol( scope, "COLOR", MDX_VF_COLOR ) ;

	int num ;
	format->SetSymbol( scope, "TEXCOORD", MDX_VF_TEXCOORDS( 1 ) ) ;
	for ( num = 1 ; num <= 3 ; num ++ ) {
		string name = str_format( "TEXCOORD%d", num ) ;
		format->SetSymbol( scope, name.c_str(), MDX_VF_TEXCOORDS( num ) ) ;
	}
	format->SetSymbol( scope, "WEIGHT", MDX_VF_WEIGHTS( 1 ) ) ;
	for ( num = 1 ; num <= 255 ; num ++ ) {
		string name = str_format( "WEIGHT%d", num ) ;
		format->SetSymbol( scope, name.c_str(), MDX_VF_WEIGHTS( num ) ) ;
	}
	format->SetSymbol( scope, "INDICES", MDX_VF_INDICES ) ;

	scope = MDX_SCOPE_FCURVE_FORMAT ;
	format->SetSymbol( scope, "CONSTANT", MDX_FCURVE_CONSTANT ) ;
	format->SetSymbol( scope, "LINEAR", MDX_FCURVE_LINEAR ) ;
	format->SetSymbol( scope, "HERMITE", MDX_FCURVE_HERMITE ) ;
	format->SetSymbol( scope, "CUBIC", MDX_FCURVE_CUBIC ) ;
	format->SetSymbol( scope, "SLERP", MDX_FCURVE_SPHERICAL ) ;

	scope = MDX_SCOPE_FCURVE_EXTRAP ;
	format->SetSymbol( scope, "HOLD", MDX_FCURVE_HOLD ) ;
	format->SetSymbol( scope, "CYCLE", MDX_FCURVE_CYCLE ) ;
	format->SetSymbol( scope, "SHUTTLE", MDX_FCURVE_SHUTTLE ) ;
	format->SetSymbol( scope, "HOLD_HOLD", MDX_FCURVE_HOLD_IN | MDX_FCURVE_HOLD_OUT ) ;
	format->SetSymbol( scope, "HOLD_CYCLE", MDX_FCURVE_HOLD_IN | MDX_FCURVE_CYCLE_OUT ) ;
	format->SetSymbol( scope, "HOLD_SHUTTLE", MDX_FCURVE_HOLD_IN | MDX_FCURVE_SHUTTLE_OUT ) ;
	format->SetSymbol( scope, "CYCLE_HOLD", MDX_FCURVE_CYCLE_IN | MDX_FCURVE_HOLD_OUT ) ;
	format->SetSymbol( scope, "CYCLE_CYCLE", MDX_FCURVE_CYCLE_IN | MDX_FCURVE_CYCLE_OUT ) ;
	format->SetSymbol( scope, "CYCLE_SHUTTLE", MDX_FCURVE_CYCLE_IN | MDX_FCURVE_SHUTTLE_OUT ) ;
	format->SetSymbol( scope, "SHUTTLE_HOLD", MDX_FCURVE_SHUTTLE_IN | MDX_FCURVE_HOLD_OUT ) ;
	format->SetSymbol( scope, "SHUTTLE_CYCLE", MDX_FCURVE_SHUTTLE_IN | MDX_FCURVE_CYCLE_OUT ) ;
	format->SetSymbol( scope, "SHUTTLE_SHUTTLE", MDX_FCURVE_SHUTTLE_IN | MDX_FCURVE_SHUTTLE_OUT ) ;

	scope = MDX_SCOPE_PRIM_MODE ;
	format->SetSymbol( scope, "POINTS", MDX_PRIM_POINTS ) ;
	format->SetSymbol( scope, "LINES", MDX_PRIM_LINES ) ;
	format->SetSymbol( scope, "LINE_STRIP", MDX_PRIM_LINE_STRIP ) ;
	format->SetSymbol( scope, "TRIANGLES", MDX_PRIM_TRIANGLES ) ;
	format->SetSymbol( scope, "TRIANGLE_STRIP", MDX_PRIM_TRIANGLE_STRIP ) ;
	format->SetSymbol( scope, "TRIANGLE_FAN", MDX_PRIM_TRIANGLE_FAN ) ;

	scope = MDX_SCOPE_B_SPLINE_MODE ;
	format->SetSymbol( scope, "OPEN_OPEN", 0 ) ;
	format->SetSymbol( scope, "CLOSED_CLOSED", MDX_B_SPLINE_CLOSED_U | MDX_B_SPLINE_CLOSED_V ) ;
	format->SetSymbol( scope, "OPEN_CLOSED", MDX_B_SPLINE_CLOSED_V ) ;
	format->SetSymbol( scope, "CLOSED_OPEN", MDX_B_SPLINE_CLOSED_U ) ;

	scope = MDX_SCOPE_ANIM_MODE ;
	format->SetSymbol( scope, "0", 0 ) ;	//  for "INDEX0" "OFFSET0"
	for ( num = 0 ; num <= 15 ; num ++ ) {
		string name = str_format( "INDEX%d", num ) ;
		format->SetSymbol( scope, name.c_str(), MDX_ANIM_MODE( num, 0 ) ) ;
	}
	for ( num = 0 ; num <= 15 ; num ++ ) {
		string name = str_format( "OFFSET%d", num ) ;
		format->SetSymbol( scope, name.c_str(), MDX_ANIM_MODE( 0, num ) ) ;
	}

	scope = MDX_SCOPE_REPEAT_MODE ;
	format->SetSymbol( scope, "HOLD", MDX_REPEAT_HOLD ) ;
	format->SetSymbol( scope, "CYCLE", MDX_REPEAT_CYCLE ) ;

	scope = MDX_SCOPE_TEXTURE_TYPE ;
	format->SetSymbol( scope, "2D", MDX_TEXTURE_2D ) ;
	format->SetSymbol( scope, "CUBE", MDX_TEXTURE_CUBE ) ;

	scope = MDX_SCOPE_FILTER_MAG ;
	format->SetSymbol( scope, "NEAREST", MDX_FILTER_NEAREST ) ;
	format->SetSymbol( scope, "LINEAR", MDX_FILTER_LINEAR ) ;

	scope = MDX_SCOPE_FILTER_MIN ;
	format->SetSymbol( scope, "NEAREST", MDX_FILTER_NEAREST ) ;
	format->SetSymbol( scope, "LINEAR", MDX_FILTER_LINEAR ) ;
	format->SetSymbol( scope, "NEAREST_MIPMAP_NEAREST", MDX_FILTER_NEAREST_MIPMAP_NEAREST ) ;
	format->SetSymbol( scope, "LINEAR_MIPMAP_NEAREST", MDX_FILTER_LINEAR_MIPMAP_NEAREST ) ;
	format->SetSymbol( scope, "NEAREST_MIPMAP_LINEAR", MDX_FILTER_NEAREST_MIPMAP_LINEAR ) ;
	format->SetSymbol( scope, "LINEAR_MIPMAP_LINEAR", MDX_FILTER_LINEAR_MIPMAP_LINEAR ) ;

	scope = MDX_SCOPE_WRAP_U ;
	format->SetSymbol( scope, "CLAMP", MDX_WRAP_CLAMP ) ;
	format->SetSymbol( scope, "REPEAT", MDX_WRAP_REPEAT ) ;

	scope = MDX_SCOPE_WRAP_V ;
	format->SetSymbol( scope, "CLAMP", MDX_WRAP_CLAMP ) ;
	format->SetSymbol( scope, "REPEAT", MDX_WRAP_REPEAT ) ;
	return true ;
}

void MdxModule_ModelFormat::ExitModule()
{
	;
}


} // namespace mdx
