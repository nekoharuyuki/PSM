#ifndef	DAE_UTIL_H_INCLUDE
#define	DAE_UTIL_H_INCLUDE

#include "DAEDOCDocumentImporter.h"
#include "COLLADASaxFWLLoader.h"
#include "COLLADAFWScene.h"
#include "COLLADAFWVisualScene.h"
#include "COLLADAFWLibraryNodes.h"
#include "COLLADAFWMaterial.h"
#include "COLLADAFWCamera.h"
#include "COLLADAFWLight.h"
#include "COLLADAFWImage.h"
#include "COLLADAFWAnimationList.h"
#include "COLLADAFWController.h"
#include "COLLADAFWFormulas.h"
#include "COLLADAFWKinematicsScene.h"
#include "COLLADAFWGeometry.h"
#include "COLLADAFWEffect.h"
#include "COLLADAFWAnimation.h"
#include "COLLADAFWSkinControllerData.h"
#include "COLLADAFWMorphController.h"
#include "COLLADAFWMesh.h"

#include <hash_map>


typedef std::vector<COLLADAFW::Edge> EdgeList;
typedef stdext::hash_map<COLLADAFW::Edge,size_t> EdgeMap;

namespace DAEDOC
{

	double getDoubleValue ( const COLLADAFW::FloatOrDoubleArray &inputValuesArray, const size_t position );
	float getFloatValue ( const COLLADAFW::FloatOrDoubleArray &inputValuesArray, const size_t position );

}	// namespace DAEDOC
#endif