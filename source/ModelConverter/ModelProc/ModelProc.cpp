#include "ModelProc.h"

#include "ImportMDX.h"
#include "ImportMDS.h"
#include "ExportMDX.h"
#include "ExportMDS.h"
#include "MergeModels.h"
#include "MergeMotions.h"

#include "ScaleSize.h"
#include "ScaleTime.h"
#include "SetFrameLoop.h"
#include "SetFrameRate.h"
#include "SetFrameRepeat.h"
#include "UnifyTexture.h"
#include "UnifyMaterial.h"
#include "UnifyVertex.h"
#include "UnifyArrays.h"
#include "UnifyMesh.h"
#include "RebuildBoundingBox.h"
#include "RebuildBoundingSphere.h"
#include "RebuildTriangle.h"
#include "RebuildKeyFrame.h"
#include "ClearBlockName.h"
#include "EmbedImage.h"
#include "SortByType.h"

#include "CompFCurveChannel.h"
#include "LimitVertexBlend.h"
#include "LimitVertexBlend100.h"

namespace mdx {

//----------------------------------------------------------------
//  module entry
//----------------------------------------------------------------

extern "C" { class MdxModule_ModelProc MdxModule_ModelProc ; }

bool MdxModule_ModelProc::InitModule( MdxShell *shell, int argc, const char **argv )
{
	if ( shell == 0 ) return false ;

	shell->SetProc( 0, new ImportMDX ) ;
	shell->SetProc( 0, new ImportMDS ) ;
	shell->SetProc( 0, new ExportMDX ) ;
	shell->SetProc( 0, new ExportMDS ) ;
	shell->SetProc( 0, new MergeModels ) ;
	shell->SetProc( 0, new MergeMotions ) ;

	shell->SetProc( 0, new ScaleSize ) ;
	shell->SetProc( 0, new ScaleTime ) ;
	shell->SetProc( 0, new SetFrameLoop ) ;
	shell->SetProc( 0, new SetFrameRate ) ;
	shell->SetProc( 0, new SetFrameRepeat ) ;
	shell->SetProc( 0, new UnifyTexture ) ;
	shell->SetProc( 0, new UnifyMaterial ) ;
	shell->SetProc( 0, new UnifyVertex ) ;
	shell->SetProc( 0, new UnifyArrays ) ;
	shell->SetProc( 0, new UnifyMesh ) ;
	shell->SetProc( 0, new RebuildBoundingBox ) ;
	shell->SetProc( 0, new RebuildBoundingSphere ) ;
	shell->SetProc( 0, new RebuildTriangle ) ;
	shell->SetProc( 0, new RebuildKeyFrame ) ;
	shell->SetProc( 0, new ClearBlockName ) ;
	shell->SetProc( 0, new EmbedImage ) ;
	shell->SetProc( 0, new SortByType ) ;

	shell->SetProc( 0, new CompFCurveChannel ) ;
	shell->SetProc( 0, new LimitVertexBlend ) ;
	shell->SetProc( 0, new LimitVertexBlend100 ) ;
	return true ;
}

void MdxModule_ModelProc::ExitModule()
{
	;
}

} // namespace mdx
