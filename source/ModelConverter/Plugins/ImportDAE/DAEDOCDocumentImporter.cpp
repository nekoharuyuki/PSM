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
#include "COLLADAFWMeshVertexData.h"
#include "COLLADAFWFloatOrDoubleArray.h"
#include "COLLADAFWNode.h"
#include "COLLADAFWInstanceGeometry.h"
#include "COLLADAFWMaterialBinding.h"
#include "COLLADABUURI.h"
#include "COLLADAFWTextureCoordinateBinding.h"
#include "COLLADAFWUniqueId.h"
#include "COLLADAFWSampler.h"
#include "COLLADAFWSkinController.h"
#include "COLLADASaxFWLTypes.h"
#include "GeneratedSaxParserTypes.h"
#include "COLLADASaxFWLColladaParserAutoGen15.h"

#include "COLLADAFWTranslate.h"
#include "COLLADAFWRotate.h"
#include "COLLADAFWScale.h"


#define OLD_MDXCONV_SUPPORT

//#include "COLLADAFWTypes.h"
#include <hash_map>

namespace DAEDOC
{

DocumentImporter::DocumentImporter( std::string filename, MdxFile* outputMDX )
{
	m_filename = filename;
	
	m_mdxFile = outputMDX;
	

	m_mdxFile->AttachChild( m_mdxModel = new MdxModel ) ;

	m_daeDocMotionInfo.minTime   = DBL_MAX;
	m_daeDocMotionInfo.maxTime   = DBL_MIN;
	m_daeDocMotionInfo.frameRate = 60.0;

	m_ParseStep = NO_PARSING;
	m_DigitTolerance = FLOAT_TOLERANCE;

	//m_GeometryImporter = new GeometryImporter ( this );

}


bool DocumentImporter::importScene()
{

	m_ParseStep = FIRST_PARSING;

	COLLADASaxFWL::Loader	loader(NULL);
	
	loader.registerExtraDataCallbackHandler ( &m_ExtraDataCallbackHandler );
	
	
	if( loader.loadDocument( m_filename, this ) != true ){ 
		return false;
	}

	if ( !m_AnimationListsList.empty() )
	{
		m_mdxModel->AttachChild( m_mdxMotion = new MdxMotion() ) ;

		m_mdxMotion->AttachChild( new MdxFrameLoop( (float)m_daeDocMotionInfo.minTime, (float)m_daeDocMotionInfo.maxTime ) );
		m_mdxMotion->AttachChild( new MdxFrameRate( (float)m_daeDocMotionInfo.frameRate ) );
	}

	make_MdxMaterial();
	make_MdxScene();
	
	return true;


}


void DocumentImporter::make_MdxMaterial()
{
	std::vector<COLLADAFW::Material*>::iterator  materialsListIte = m_MaterialsList.begin() ;
	
	for( ; materialsListIte != m_MaterialsList.end(); materialsListIte++ ){
		MdxMaterial *inputMateria = new MdxMaterial;
		inputMateria->SetName((*materialsListIte)->getName().c_str() );
		
		m_mdxModel->AttachChild( inputMateria ) ;

		COLLADAFW::UniqueId materialUniqueId = (*materialsListIte)->getUniqueId();
		
		
		COLLADAFW::UniqueId matEffectUniqueId = (*materialsListIte)->getInstantiatedEffect();
		
		std::vector<COLLADAFW::Effect*>::iterator effectListIte = m_EffectsList.begin(); 

		int layerCnt = 0;
		std::stringstream wkLayerNo;

		for( ; effectListIte != m_EffectsList.end(); effectListIte++ ){
			COLLADAFW::UniqueId effectUniqueId  = (*effectListIte)->getUniqueId () ;


			if( matEffectUniqueId == effectUniqueId ){
				COLLADAFW::CommonEffectPointerArray getCommonEffect = (*effectListIte)->getCommonEffects();
				
				COLLADAFW::EffectCommon *getMaterialEffect = getCommonEffect[0];
				const COLLADAFW::SamplerPointerArray& getSamplerArray = getMaterialEffect->getSamplerPointerArray();

				//############ Diffuse ############
				const COLLADAFW::ColorOrTexture diffuseData = getMaterialEffect->getDiffuse();
				if ( diffuseData.isColor() ){
					
					vec4 setDiffuseColor;
					COLLADAFW::Color getDiffuseColor = diffuseData.getColor();
					setDiffuseColor.set ((float)getDiffuseColor.getRed(), (float)getDiffuseColor.getGreen(), (float)getDiffuseColor.getBlue(), (float)getDiffuseColor.getAlpha() ); 
					inputMateria->AttachChild(new MdxDiffuse(setDiffuseColor)  );

					const COLLADAFW::UniqueId&			diffuseAnimationListId = getDiffuseColor.getAnimationList();
					COLLADAFW::AnimationList*			diffuseAnimationList = findAnimationList( diffuseAnimationListId );
					/* get animated diffuse */
					if ( diffuseAnimationList != NULL )
					{
						const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = diffuseAnimationList->getAnimationBindings();
						size_t													numAnimationBindings = animationBindings.getCount();

						std::vector<MdxFCurve*>				diffuseFCurveList;
						std::vector<TransformVectorType>	diffuseAnimMergeOrder;

						for ( size_t anims=0; anims<numAnimationBindings; anims++ )
						{
							const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
							const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

							daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
							if ( !animCurveInfo )
							{
								continue;
							}

							switch ( animationBinding.animationClass )
							{
								case COLLADAFW::AnimationList::COLOR_R:
								{
									diffuseFCurveList.push_back( animCurveInfo->animationCurve );
									diffuseAnimMergeOrder.push_back( TRANSVECTYPE_R );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_G:
								{
									diffuseFCurveList.push_back( animCurveInfo->animationCurve );
									diffuseAnimMergeOrder.push_back( TRANSVECTYPE_G );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_B:
								{
									diffuseFCurveList.push_back( animCurveInfo->animationCurve );
									diffuseAnimMergeOrder.push_back( TRANSVECTYPE_B );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_A:
								{
									diffuseFCurveList.push_back( animCurveInfo->animationCurve );
									diffuseAnimMergeOrder.push_back( TRANSVECTYPE_A );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_RGBA:
								{
									diffuseFCurveList.push_back( animCurveInfo->animationCurve );
									diffuseAnimMergeOrder.push_back( TRANSVECTYPE_RGBA );
									break;
								}
								default:
								{
									continue;
								}
							}
						}

						MdxAnimate*		mdxAnimate = new MdxAnimate();
						MdxFCurve*		diffuseFCurve;

						mdxAnimate->SetScope( inputMateria->GetTypeID() );
						mdxAnimate->SetBlock( inputMateria->GetName() );
						mdxAnimate->SetCommand( MDX_DIFFUSE );
						mdxAnimate->SetMode( 0 );

						if ( diffuseAnimMergeOrder.size() == 1 && diffuseAnimMergeOrder[0] == TRANSVECTYPE_RGBA )
						{
							diffuseFCurve = diffuseFCurveList[0];
						} else
						{
							diffuseFCurve = new MdxFCurve;

							mergeMdxFCurves( diffuseFCurve, 4, diffuseFCurveList, diffuseAnimMergeOrder );
						}

						diffuseFCurve->SetName( ( std::string( inputMateria->GetName() ) + "_Diffuse" ).c_str() );

						mdxAnimate->SetFCurve( diffuseFCurve->GetName() );
						m_mdxMotion->AttachChild( mdxAnimate );
						m_mdxMotion->AttachChild( diffuseFCurve );
					
					}
				}
				if (diffuseData.isTexture() ){
					wkLayerNo.clear(); 
					wkLayerNo << "-" << layerCnt++ ;
					std::string	layerName = (*materialsListIte)->getName() + wkLayerNo.str();

					MdxLayer *inputLayer = new MdxLayer(  layerName.c_str()  );
					inputMateria->AttachChild( inputLayer );
					inputLayer->AttachChild( new MdxMapType( MDX_DIFFUSE ) );
					
					COLLADAFW::Texture diffuseTex = diffuseData.getTexture();
					
					MdxMaterialTextureMapInfo *inputTextureConnectInfo = new MdxMaterialTextureMapInfo;
			
					//diffuseTex (Texture Index Connect)
					inputTextureConnectInfo->materialUniqueId  = materialUniqueId;
					inputTextureConnectInfo->materialTextureID = diffuseTex.getTextureMapId();
					inputTextureConnectInfo->mdxLayerData	   = inputLayer;
					m_MdxMaterialTextureMap.push_back(inputTextureConnectInfo);
					
					set_MaterialLayer(inputLayer, getSamplerArray[diffuseTex.getSamplerId()], effectUniqueId);
				

				}

				//############ Emission ############
				const COLLADAFW::ColorOrTexture emissionData = getMaterialEffect->getEmission();
				if ( emissionData.isColor() ){
					
					vec4 setEmissionColor;
					COLLADAFW::Color getEmissionColor = emissionData.getColor();
					setEmissionColor.set ((float)getEmissionColor.getRed(), (float)getEmissionColor.getGreen(), (float)getEmissionColor.getBlue(), (float)getEmissionColor.getAlpha() ); 
					inputMateria->AttachChild(new MdxEmission(setEmissionColor)  );

					const COLLADAFW::UniqueId&			emissionAnimationListId = getEmissionColor.getAnimationList();
					COLLADAFW::AnimationList*			emissionAnimationList = findAnimationList( emissionAnimationListId );
					/* get animated diffuse */
					if ( emissionAnimationList != NULL )
					{
						const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = emissionAnimationList->getAnimationBindings();
						size_t													numAnimationBindings = animationBindings.getCount();

						std::vector<MdxFCurve*>				emissionFCurveList;
						std::vector<TransformVectorType>	emissionAnimMergeOrder;

						for ( size_t anims=0; anims < numAnimationBindings; anims++ )
						{
							const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
							const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

							daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
							if ( !animCurveInfo )
							{
								continue;
							}

							switch ( animationBinding.animationClass )
							{
								case COLLADAFW::AnimationList::COLOR_R:
								{
									emissionFCurveList.push_back( animCurveInfo->animationCurve );
									emissionAnimMergeOrder.push_back( TRANSVECTYPE_R );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_G:
								{
									emissionFCurveList.push_back( animCurveInfo->animationCurve );
									emissionAnimMergeOrder.push_back( TRANSVECTYPE_G );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_B:
								{
									emissionFCurveList.push_back( animCurveInfo->animationCurve );
									emissionAnimMergeOrder.push_back( TRANSVECTYPE_B );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_A:
								{
									emissionFCurveList.push_back( animCurveInfo->animationCurve );
									emissionAnimMergeOrder.push_back( TRANSVECTYPE_A );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_RGBA:
								{
									emissionFCurveList.push_back( animCurveInfo->animationCurve );
									emissionAnimMergeOrder.push_back( TRANSVECTYPE_RGBA );
									break;
								}
								default:
								{
									continue;
								}
							}
						}

						MdxAnimate*		mdxAnimate = new MdxAnimate();
						MdxFCurve*		emissionFCurve;

						mdxAnimate->SetScope( inputMateria->GetTypeID() );
						mdxAnimate->SetBlock( inputMateria->GetName() );
						mdxAnimate->SetCommand( MDX_EMISSION );
						mdxAnimate->SetMode( 0 );

						if ( emissionAnimMergeOrder.size() == 1 && emissionAnimMergeOrder[0] == TRANSVECTYPE_RGBA )
						{
							emissionFCurve = emissionFCurveList[0];
						} else
						{
							emissionFCurve = new MdxFCurve;
							mergeMdxFCurves( emissionFCurve, 4, emissionFCurveList, emissionAnimMergeOrder );
						}

						emissionFCurve->SetName( ( std::string( inputMateria->GetName() ) + "_Emission" ).c_str() );

						mdxAnimate->SetFCurve( emissionFCurve->GetName() );
						m_mdxMotion->AttachChild( mdxAnimate );
						m_mdxMotion->AttachChild( emissionFCurve );
					
					}
				}
				if (emissionData.isTexture() ){
					wkLayerNo.clear(); 
					wkLayerNo << "-" << layerCnt++ ;
					std::string	layerName = (*materialsListIte)->getName() + wkLayerNo.str();
					MdxLayer *inputLayer = new MdxLayer(  layerName.c_str()  );
					inputMateria->AttachChild( inputLayer );
					inputLayer->AttachChild( new MdxMapType( MDX_EMISSION ) );
					
					COLLADAFW::Texture emissionTex = emissionData.getTexture();
					MdxMaterialTextureMapInfo *inputTextureConnectInfo = new MdxMaterialTextureMapInfo;
			
					inputTextureConnectInfo->materialUniqueId  = materialUniqueId;
					inputTextureConnectInfo->materialTextureID = emissionTex.getTextureMapId();
					inputTextureConnectInfo->mdxLayerData	   = inputLayer;
					m_MdxMaterialTextureMap.push_back(inputTextureConnectInfo);

					set_MaterialLayer(inputLayer, getSamplerArray[emissionTex.getSamplerId()], effectUniqueId );

				}
				
				//############ Specular ############
				const COLLADAFW::ColorOrTexture specularData = getMaterialEffect->getSpecular();
				if ( specularData.isColor() ){
					vec4 setSpecularColor;
					COLLADAFW::Color getSpecularColor = specularData.getColor();
					setSpecularColor.set ((float)getSpecularColor.getRed(), (float)getSpecularColor.getGreen(), (float)getSpecularColor.getBlue(), (float)getSpecularColor.getAlpha() ); 
					inputMateria->AttachChild(new MdxSpecular(setSpecularColor)  );
					
					const COLLADAFW::UniqueId&			specularAnimationListId = getSpecularColor.getAnimationList();
					COLLADAFW::AnimationList*			specularAnimationList = findAnimationList( specularAnimationListId );
					/* get animated specular */
					if ( specularAnimationList != NULL )
					{
						const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = specularAnimationList->getAnimationBindings();
						size_t													numAnimationBindings = animationBindings.getCount();

						std::vector<MdxFCurve*>				specularFCurveList;
						std::vector<TransformVectorType>	specularAnimMergeOrder;

						for ( size_t anims=0; anims < numAnimationBindings; anims++ )
						{
							const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
							const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

							daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
							if ( !animCurveInfo )
							{
								continue;
							}

							switch ( animationBinding.animationClass )
							{
								case COLLADAFW::AnimationList::COLOR_R:
								{
									specularFCurveList.push_back( animCurveInfo->animationCurve );
									specularAnimMergeOrder.push_back( TRANSVECTYPE_R );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_G:
								{
									specularFCurveList.push_back( animCurveInfo->animationCurve );
									specularAnimMergeOrder.push_back( TRANSVECTYPE_G );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_B:
								{
									specularFCurveList.push_back( animCurveInfo->animationCurve );
									specularAnimMergeOrder.push_back( TRANSVECTYPE_B );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_A:
								{
									specularFCurveList.push_back( animCurveInfo->animationCurve );
									specularAnimMergeOrder.push_back( TRANSVECTYPE_A );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_RGBA:
								{
									specularFCurveList.push_back( animCurveInfo->animationCurve );
									specularAnimMergeOrder.push_back( TRANSVECTYPE_RGBA );
									break;
								}
								default:
								{
									continue;
								}
							}
						}

						MdxAnimate*		mdxAnimate = new MdxAnimate();
						MdxFCurve*		specularFCurve;

						mdxAnimate->SetScope( inputMateria->GetTypeID() );
						mdxAnimate->SetBlock( inputMateria->GetName() );
						mdxAnimate->SetCommand( MDX_SPECULAR );
						mdxAnimate->SetMode( 0 );

						if ( specularAnimMergeOrder.size() == 1 && specularAnimMergeOrder[0] == TRANSVECTYPE_RGBA )
						{
							specularFCurve = specularFCurveList[0];
						} else
						{
							specularFCurve = new MdxFCurve;
							mergeMdxFCurves( specularFCurve, 4, specularFCurveList, specularAnimMergeOrder );
						}

						specularFCurve->SetName( ( std::string( inputMateria->GetName() ) + "_Specular" ).c_str() );

						mdxAnimate->SetFCurve( specularFCurve->GetName() );
						m_mdxMotion->AttachChild( mdxAnimate );
						m_mdxMotion->AttachChild( specularFCurve );
					
					}
					
				}
				if (specularData.isTexture() ){
					wkLayerNo.clear(); 
					wkLayerNo << "-" << layerCnt++ ;
					std::string	layerName = (*materialsListIte)->getName() + wkLayerNo.str();

					MdxLayer *inputLayer = new MdxLayer(  layerName.c_str()  );
					inputMateria->AttachChild( inputLayer );
					inputLayer->AttachChild( new MdxMapType( MDX_SPECULAR ) );

					COLLADAFW::Texture specularTex = specularData.getTexture();
					MdxMaterialTextureMapInfo *inputTextureConnectInfo = new MdxMaterialTextureMapInfo;
			
					inputTextureConnectInfo->materialUniqueId  = materialUniqueId;
					inputTextureConnectInfo->materialTextureID = specularTex.getTextureMapId();
					inputTextureConnectInfo->mdxLayerData	   = inputLayer;
					m_MdxMaterialTextureMap.push_back(inputTextureConnectInfo);

					set_MaterialLayer(inputLayer, getSamplerArray[specularTex.getSamplerId()], effectUniqueId );

				}
				//############ Ambient ############
				const COLLADAFW::ColorOrTexture ambientData = getMaterialEffect->getAmbient();
				if ( ambientData.isColor() ){
					
					vec4 setAmbientColor;
					COLLADAFW::Color getAmbientColor = ambientData.getColor();
					setAmbientColor.set ((float)getAmbientColor.getRed(), (float)getAmbientColor.getGreen(), (float)getAmbientColor.getBlue(), (float)getAmbientColor.getAlpha() ); 
					inputMateria->AttachChild(new MdxAmbient(setAmbientColor)  );

					const COLLADAFW::UniqueId&			ambientAnimationListId = getAmbientColor.getAnimationList();
					COLLADAFW::AnimationList*			ambientAnimationList = findAnimationList( ambientAnimationListId );
					/* get animated diffuse */
					if ( ambientAnimationList != NULL )
					{
						const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = ambientAnimationList->getAnimationBindings();
						size_t													numAnimationBindings = animationBindings.getCount();

						std::vector<MdxFCurve*>				ambientFCurveList;
						std::vector<TransformVectorType>	ambientAnimMergeOrder;

						for ( size_t anims=0; anims < numAnimationBindings; anims++ )
						{
							const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
							const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

							daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
							if ( !animCurveInfo )
							{
								continue;
							}

							switch ( animationBinding.animationClass )
							{
								case COLLADAFW::AnimationList::COLOR_R:
								{
									ambientFCurveList.push_back( animCurveInfo->animationCurve );
									ambientAnimMergeOrder.push_back( TRANSVECTYPE_R );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_G:
								{
									ambientFCurveList.push_back( animCurveInfo->animationCurve );
									ambientAnimMergeOrder.push_back( TRANSVECTYPE_G );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_B:
								{
									ambientFCurveList.push_back( animCurveInfo->animationCurve );
									ambientAnimMergeOrder.push_back( TRANSVECTYPE_B );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_A:
								{
									ambientFCurveList.push_back( animCurveInfo->animationCurve );
									ambientAnimMergeOrder.push_back( TRANSVECTYPE_A );
									break;
								}
								case COLLADAFW::AnimationList::COLOR_RGBA:
								{
									ambientFCurveList.push_back( animCurveInfo->animationCurve );
									ambientAnimMergeOrder.push_back( TRANSVECTYPE_RGBA );
									break;
								}
								default:
								{
									continue;
								}
							}
						}

						MdxAnimate*		mdxAnimate = new MdxAnimate();
						MdxFCurve*		ambientFCurve;

						mdxAnimate->SetScope( inputMateria->GetTypeID() );
						mdxAnimate->SetBlock( inputMateria->GetName() );
						mdxAnimate->SetCommand( MDX_AMBIENT );
						mdxAnimate->SetMode( 0 );

						if ( ambientAnimMergeOrder.size() == 1 && ambientAnimMergeOrder[0] == TRANSVECTYPE_RGBA )
						{
							ambientFCurve = ambientFCurveList[0];
						} else
						{
							ambientFCurve = new MdxFCurve;
							mergeMdxFCurves( ambientFCurve, 4, ambientFCurveList, ambientAnimMergeOrder );
						}

						ambientFCurve->SetName( ( std::string( inputMateria->GetName() ) + "_Ambient" ).c_str() );

						mdxAnimate->SetFCurve( ambientFCurve->GetName() );
						m_mdxMotion->AttachChild( mdxAnimate );
						m_mdxMotion->AttachChild( ambientFCurve );
					
					}
						
				}
				if (ambientData.isTexture() ){
					wkLayerNo.clear(); 
					wkLayerNo << "-" << layerCnt++ ;
					std::string	layerName = (*materialsListIte)->getName() + wkLayerNo.str();
					MdxLayer *inputLayer = new MdxLayer(  layerName.c_str()  );
					inputMateria->AttachChild( inputLayer );
					inputLayer->AttachChild( new MdxMapType( MDX_AMBIENT ) );


					COLLADAFW::Texture ambientTex = ambientData.getTexture();
					MdxMaterialTextureMapInfo *inputTextureConnectInfo = new MdxMaterialTextureMapInfo;
			
					inputTextureConnectInfo->materialUniqueId  = materialUniqueId;
					inputTextureConnectInfo->materialTextureID = ambientTex.getTextureMapId();
					inputTextureConnectInfo->mdxLayerData	   = inputLayer;
					m_MdxMaterialTextureMap.push_back(inputTextureConnectInfo);

					set_MaterialLayer(inputLayer, getSamplerArray[ambientTex.getSamplerId()], effectUniqueId);
				}
				break;
			}
		}
		
		
		MdxMaterialMapInfo *wkMdxMaterial = new MdxMaterialMapInfo(  std::make_pair(materialUniqueId,inputMateria ));
		m_MdxMaterialMap.push_back( wkMdxMaterial );

	}
	
}

void DocumentImporter::set_MaterialLayer( MdxLayer *inputLayer, COLLADAFW::Sampler *sampler, COLLADAFW::UniqueId effectUniqueId )
{
	//  引数に unsigned ExtraIdx

	if( sampler == NULL )return;

	const COLLADAFW::UniqueId& imageUniqueId = sampler->getSourceImage(); 
	std::string textureName;
	if( findMdxTextureName(imageUniqueId, textureName )){
		inputLayer->AttachChild( new MdxSetTexture( textureName.c_str() ) );
	}
}
 


void DocumentImporter::make_MdxScene()
{

	for ( size_t i=0; i<m_VisualScenesList.size (); ++i )
	{
		const COLLADAFW::VisualScene* visualScene = m_VisualScenesList [i];
		if ( m_InstanceVisualScene->getInstanciatedObjectId () == visualScene->getUniqueId () )
		{
			m_wkvisualScene = m_VisualScenesList [i];
			const COLLADAFW::NodePointerArray& chiledNodes = visualScene->getRootNodes ();

			for ( size_t i = 0, count = chiledNodes.getCount(); i < count; ++i)
			{
				const COLLADAFW::Node* node = chiledNodes[i];
				make_MdxNode( node, NULL );
			}
		}
	}
}

void DocumentImporter::make_MdxNode( const COLLADAFW::Node *node, MdxBone *parentMdxNode)
{

	MdxBone* mdxNode;
	m_mdxModel->AttachChild( mdxNode = new MdxBone( node->getName().c_str() ) ) ;			

	if ( parentMdxNode != 0 ) {
		mdxNode->AttachChild( new MdxParentBone( parentMdxNode->GetName() ) ) ;
	}

	set_NodeInfo( node, mdxNode, parentMdxNode ); 

	const COLLADAFW::NodePointerArray& childNodes = node->getChildNodes();
	for ( size_t i = 0, count = childNodes.getCount(); i < count; ++i)
	{
		const COLLADAFW::Node* childNode = childNodes[i];
		
		make_MdxNode( childNode, mdxNode ); 
	}
	
}
void DocumentImporter::make_Mesh( const COLLADAFW::InstanceGeometry* instanceGeometry, MdxBone *setMdxNode )
{
	
	std::vector<daeDocGeometryInfo>::iterator geomMeshsIte = m_GeometryMeshList.begin() ;
	const COLLADAFW::UniqueId& getMeshUniqueId = instanceGeometry->getInstanciatedObjectId(); 
	
	
	for( ; geomMeshsIte != m_GeometryMeshList.end() ; geomMeshsIte++ ){
		
		if( getMeshUniqueId == geomMeshsIte->geometryUniqueId ){
			
			MdxPart* inputMdxPart = new MdxPart;
			std::string partName =  geomMeshsIte->geometryName  + "-Part";
			inputMdxPart->SetName( partName.c_str() );
				
			setMdxNode->AttachChild( new MdxDrawPart ( partName.c_str()  ));
			m_mdxModel->AttachChild( inputMdxPart );

			std::vector<daeDocMeshInfo>::iterator meshIte = geomMeshsIte->Mesh.begin();
			for( size_t meshNo = 0; meshIte != geomMeshsIte->Mesh.end() ; meshIte++, meshNo++ ){
				
				std::stringstream strstMeshNo;
				strstMeshNo.clear();
				strstMeshNo << "-" << meshNo;

				MdxMesh* inputMdxMesh = new MdxMesh;
				std::string	meshName = geomMeshsIte->geometryName  + strstMeshNo.str();
				inputMdxMesh->SetName( meshName.c_str() );
				
				inputMdxPart->AttachChild( inputMdxMesh );

				MdxDrawArrays *setMdxDrawArrays = new MdxDrawArrays ;
				MdxArrays *setMdxArrays = new MdxArrays ;
				
				
				std::string	arraysName = inputMdxMesh->GetName();
				arraysName += "-arrays";
				setMdxArrays->SetName( arraysName.c_str() ); 
				
				inputMdxMesh->AttachChild( new MdxSetArrays( setMdxArrays->GetName() ) );

				inputMdxMesh->AttachChild( setMdxDrawArrays );	
				inputMdxPart->AttachChild( setMdxArrays );

				// VertexBuffer
				setMdxArrays->SetVertexCount ( meshIte->vPositions.size() );
				
				// Postion Count
				if( meshIte->vPositions.size() != 0 ){
					setMdxArrays->SetVertexPositionCount(1) ;
				}else {
					setMdxArrays->SetVertexPositionCount(0) ;
				}

				// Normal Count
				if( meshIte->vNormals.size() != 0 ){
					setMdxArrays->SetVertexNormalCount(1) ;
					// tangent Count	
#ifndef OLD_MDXCONV_SUPPORT
					//Tangent 
					if( meshIte->vTangents.size() != 0  ){
						setMdxArrays->SetVertexTangentCount(1) ;
					}else {
						setMdxArrays->SetVertexTangentCount(0) ;
					}
					//BiNormal
					if( meshIte->vTangents.size() != 0  ){
						setMdxArrays->SetVertexBinormalCount(1) ;
					}else {
						setMdxArrays->SetVertexBinormalCount(0) ;
					}						
#endif
				}else {
					setMdxArrays->SetVertexNormalCount(0) ;
#ifndef OLD_MDXCONV_SUPPORT
					// tangent Count	
					setMdxArrays->SetVertexTangentCount(0) ;
					//BiNormal
					setMdxArrays->SetVertexBinormalCount(0) ;
#endif
				}
				
				// Color Count
				if( meshIte->vColors.size() != 0 ){
					setMdxArrays->SetVertexColorCount(1) ;
				}else {
					setMdxArrays->SetVertexColorCount(0) ;
				}

				// TextureCoord Count
				setMdxArrays->SetVertexTexCoordCount( meshIte->uvSetsNum  ) ;
								
				for( size_t vetexIdx = 0 ; vetexIdx < meshIte->vPositions.size(); vetexIdx++ )
				{
					MdxVertex &v = setMdxArrays->GetVertex( vetexIdx );

					// Postion
					if( meshIte->vPositions.size() != 0  )
					{
						v.SetPositionCount( 1 );
						v.SetPosition( meshIte->vPositions[vetexIdx] );
					}else {
						v.SetPositionCount( 0 );
					}
					
					// Normal
					if( meshIte->vNormals.size() != 0  )
					{
						v.SetNormalCount( 1 );
						v.SetNormal( meshIte->vNormals[vetexIdx] );

#ifndef OLD_MDXCONV_SUPPORT
						//Tangent 
						if( meshIte->vTangents.size() != 0  ){
							v.SetTangentCount( 1 );
							v.SetTangent( meshIte->vTangents[vetexIdx] );
						}else {
							v.SetTangentCount( 0 );
						}
						//BiNormal
						if( meshIte->vBinormals.size() != 0  ){
							v.SetBinormalCount( 1 );
							v.SetBinormal( meshIte->vBinormals[vetexIdx] );
						}else {
							v.SetBinormalCount( 0 );
						}
#endif
					}else {
						v.SetNormalCount( 0 );
#ifndef OLD_MDXCONV_SUPPORT
						v.SetTangentCount( 0 );
						v.SetBinormalCount( 0 );
#endif
					}
					
					// Color
					if( meshIte->vColors.size() != 0  )
					{
						v.SetColorCount( 1 );
						v.SetColor( meshIte->vColors[vetexIdx] );
					}else {
						v.SetColorCount( 0 );
					}
					
					// TextureCoord
					if( meshIte->vUVSet.size() != 0  )
					{
						v.SetTexCoordCount( (int)meshIte->uvSetsNum );
						for( size_t uvIdx= 0; uvIdx < meshIte->uvSetsNum ; uvIdx++ )
						{
							size_t getUVDataIdx = (meshIte->uvSetsNum * vetexIdx )+ uvIdx;
							v.SetTexCoord( uvIdx, meshIte->vUVSet[getUVDataIdx] );
						}
					}else {
						v.SetTexCoordCount( 0 );
					}
				}

				// IndexBuffer
				setMdxDrawArrays->SetMode( meshIte->mdxPrimType );
				setMdxDrawArrays->SetPrimCount( 1 );
				setMdxDrawArrays->SetVertexCount( meshIte->IndexBuffer.size() ) ;
				for( size_t ibIdx = 0 ; ibIdx < meshIte->IndexBuffer.size() ; ibIdx++ )
				{
					setMdxDrawArrays->SetIndex( ibIdx, meshIte->IndexBuffer[ibIdx]  );
				}


				// Material
				const COLLADAFW::MaterialBindingArray& wkMaiterialbindgArray =  instanceGeometry->getMaterialBindings();
				for ( size_t matbindingCnt = 0; matbindingCnt <  wkMaiterialbindgArray.getCount() ; matbindingCnt++ )
				{
					const COLLADAFW::MaterialId matId = wkMaiterialbindgArray[matbindingCnt].getMaterialId();
					const COLLADAFW::UniqueId& matUniqueId = wkMaiterialbindgArray[matbindingCnt].getReferencedMaterial() ;

					
					if( matId == meshIte->meshMaterialID ){
						std::string getMatelalName;
						if( findMdxMaterialName(matUniqueId, getMatelalName )){
							inputMdxMesh->AttachChild( new MdxSetMaterial( getMatelalName.c_str() ) ) ;
						}
					
						// Texture Index Connection？？
						const COLLADAFW::TextureCoordinateBindingArray& wkTexCoordinateBindingArray = wkMaiterialbindgArray[matbindingCnt].getTextureCoordinateBindingArray(); 
						for ( size_t texcdbdCnt = 0; texcdbdCnt <  wkTexCoordinateBindingArray.getCount() ; texcdbdCnt++ )
						{
							//COLLADAFW::TextureCoordinateBinding wkTextureCoordinateBinding = wkTexCoordinateBindingArray[texcdbdCnt];
							//
							//COLLADAFW::TextureMapId texMapId = wkTextureCoordinateBinding.getTextureMapId();
							//wkTextureCoordinateBinding.getTextureMapId();
							//wkTextureCoordinateBinding.getSetIndex()
						}
					}
				}
			}
			break;
		}
	}
}

void DocumentImporter::make_MorphingMesh( const COLLADAFW::InstanceController* instanceController, MdxBone *setMdxNode  )
{
	const COLLADAFW::UniqueId& controllerId = instanceController->getInstanciatedObjectId();

	std::vector<daeDocMorphCtrlMapInfo>::iterator daeDocMorphCtrlMapListIte = m_daeDocMorphCtrlMapList.begin() ;
	for( ; daeDocMorphCtrlMapListIte != m_daeDocMorphCtrlMapList.end() ; daeDocMorphCtrlMapListIte++ )
	{
		if( controllerId == daeDocMorphCtrlMapListIte->MorphCtrlUniqId ){
			std::vector<daeDocGeometryInfo>::iterator geomMeshsIte = m_GeometryMeshList.begin() ;

			for( ; geomMeshsIte != m_GeometryMeshList.end() ; geomMeshsIte++ ){
				if( daeDocMorphCtrlMapListIte->meshORcontrollerUniq == geomMeshsIte->geometryUniqueId ){
					
					geomMeshsIte->morphWeightAnimationListId = daeDocMorphCtrlMapListIte->morphWeightAnimationListId ;
				
					std::vector<morphTargetInfo>::iterator	morphTargetIte = daeDocMorphCtrlMapListIte->morphTarget.begin();  
					
					// TargetSet
					for (; morphTargetIte != daeDocMorphCtrlMapListIte->morphTarget.end() ; morphTargetIte++ ) 
					{
						
						std::vector<daeDocGeometryInfo>::iterator checkMeshsIte = m_GeometryMeshList.begin() ;
						for( ; checkMeshsIte != m_GeometryMeshList.end() ; checkMeshsIte++ ){
							if( morphTargetIte->geomUniqId == checkMeshsIte->geometryUniqueId ){

								geomMeshsIte->morphTaget_subMeshCount.push_back ( checkMeshsIte->Mesh.size() );
								vector<daeDocMeshInfo>::iterator targetSubMeshIte = checkMeshsIte->Mesh.begin();
								for( ; targetSubMeshIte != checkMeshsIte->Mesh.end() ; targetSubMeshIte++ ){ 
									geomMeshsIte->morphTagetMesh.push_back( *targetSubMeshIte );
								}
							}
						}		
					}

					
					MdxPart* inputMdxPart = new MdxPart;
					std::string partName =  geomMeshsIte->geometryName  + "-Part";
					inputMdxPart->SetName( partName.c_str() );
						
					setMdxNode->AttachChild( new MdxDrawPart ( partName.c_str()  ));
					m_mdxModel->AttachChild( inputMdxPart );

					std::vector<daeDocMeshInfo>::iterator meshIte = geomMeshsIte->Mesh.begin();
					for( size_t meshNo = 0; meshIte != geomMeshsIte->Mesh.end() ; meshIte++, meshNo++ ){
						
						std::stringstream strstMeshNo;
						strstMeshNo.clear();
						strstMeshNo << "-" << meshNo;

						MdxMesh* inputMdxMesh = new MdxMesh;
						std::string	meshName = geomMeshsIte->geometryName  + strstMeshNo.str();
						inputMdxMesh->SetName( meshName.c_str() );
						
						inputMdxPart->AttachChild( inputMdxMesh );

						MdxDrawArrays *setMdxDrawArrays = new MdxDrawArrays ;
						MdxArrays *setMdxArrays = new MdxArrays ;
						
						
						std::string	arraysName = inputMdxMesh->GetName();
						arraysName += "-arrays";
						setMdxArrays->SetName( arraysName.c_str() ); 
						
						inputMdxMesh->AttachChild( new MdxSetArrays( setMdxArrays->GetName() ) );

						inputMdxMesh->AttachChild( setMdxDrawArrays );	
						inputMdxPart->AttachChild( setMdxArrays );
						

						vector<daeDocMeshInfo>	inputMorphTaget_subMesh;
						inputMorphTaget_subMesh.clear();
						size_t getTagetIndex = 0;
						for( size_t targetsubMeshCnt= 0; targetsubMeshCnt < geomMeshsIte->morphTaget_subMeshCount.size() ; targetsubMeshCnt++ ){
						
							inputMorphTaget_subMesh.push_back( geomMeshsIte->morphTagetMesh[getTagetIndex+meshNo] ) ;
							getTagetIndex = getTagetIndex + geomMeshsIte->morphTaget_subMeshCount[targetsubMeshCnt];
							
						}
						
						set_MeshInfo( &(*meshIte), inputMdxMesh, setMdxArrays, setMdxDrawArrays, inputMorphTaget_subMesh );
						
						COLLADAFW::AnimationList*			animationList = findAnimationList( geomMeshsIte->morphWeightAnimationListId );
						if( animationList != 0 ){
							const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = animationList->getAnimationBindings();
							size_t													numAnimationBindings = animationBindings.getCount();
							std::vector<MdxFCurve*>  morph_animCurveList;
							morph_animCurveList.clear();

							for ( size_t anims=0; anims<numAnimationBindings; anims++ )
							{
								const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
								const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

								daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
								if ( !animCurveInfo )
								{
									continue;
								}
															
								morph_animCurveList.push_back( animCurveInfo->animationCurve );
								
							}
							
							if( morph_animCurveList.size() != 0 )
							{
								
								MdxAnimate*		mdxAnimate = new MdxAnimate();
								MdxFCurve*		morphWeightFCurve = new MdxFCurve() ;
								

								mdxAnimate->SetScope( setMdxNode->GetTypeID() );
								mdxAnimate->SetBlock( setMdxNode->GetName() );
								mdxAnimate->SetCommand( MDX_MORPH_WEIGHTS );
								mdxAnimate->SetMode( 0 );
								
								mergeMorphWeightMdxFCurves(morphWeightFCurve, morph_animCurveList);

								 
								morphWeightFCurve->SetName( ( std::string( setMdxNode->GetName() ) ).c_str() );

								mdxAnimate->SetFCurve( morphWeightFCurve->GetName() );
								m_mdxMotion->AttachChild( mdxAnimate );
								m_mdxMotion->AttachChild( morphWeightFCurve );
								
							}

						}
						
						// Material
						const COLLADAFW::MaterialBindingArray& wkMaiterialbindgArray =  instanceController->getMaterialBindings();
						for ( size_t matbindingCnt = 0; matbindingCnt <  wkMaiterialbindgArray.getCount() ; matbindingCnt++ )
						{
							const COLLADAFW::MaterialId matId = wkMaiterialbindgArray[matbindingCnt].getMaterialId();
							const COLLADAFW::UniqueId& matUniqueId = wkMaiterialbindgArray[matbindingCnt].getReferencedMaterial() ;

							
							if( matId == meshIte->meshMaterialID ){
								std::string getMatelalName;
								if( findMdxMaterialName(matUniqueId, getMatelalName )){
									inputMdxMesh->AttachChild( new MdxSetMaterial( getMatelalName.c_str() ) ) ;
								}
							
								// Texture Index Connection？？
								const COLLADAFW::TextureCoordinateBindingArray& wkTexCoordinateBindingArray = wkMaiterialbindgArray[matbindingCnt].getTextureCoordinateBindingArray(); 
								for ( size_t texcdbdCnt = 0; texcdbdCnt <  wkTexCoordinateBindingArray.getCount() ; texcdbdCnt++ )
								{
									//COLLADAFW::TextureCoordinateBinding wkTextureCoordinateBinding = wkTexCoordinateBindingArray[texcdbdCnt];
									//
									//COLLADAFW::TextureMapId texMapId = wkTextureCoordinateBinding.getTextureMapId();
									//wkTextureCoordinateBinding.getTextureMapId();
									//wkTextureCoordinateBinding.getSetIndex()
								}
							}
						}
					
					}
					break;
				}
			}
			break;
		}
	}
}

void DocumentImporter::set_MeshInfo( daeDocMeshInfo* meshdata,MdxMesh* setMdxMesh, MdxArrays* setMdxArrays, MdxDrawArrays *setMdxDrawArrays, std::vector<daeDocMeshInfo> morphTargetList )
{
	 
	
	// VertexBuffer
	setMdxArrays->SetVertexCount ( meshdata->vPositions.size() );

	// Postion Count
	if( meshdata->vPositions.size() != 0 ){
		setMdxArrays->SetVertexPositionCount(1) ;
	}else {
		setMdxArrays->SetVertexPositionCount(0) ;
	}
	
	// Normal Count
	if( meshdata->vNormals.size() != 0 ){
		setMdxArrays->SetVertexNormalCount(1) ;
#ifndef OLD_MDXCONV_SUPPORT
		// tangent Count
		if( meshdata->vTangents.size() != 0 ){
			setMdxArrays->SetVertexTangentCount(1) ;
		}else {
			setMdxArrays->SetVertexTangentCount(0) ;
		}
		//BiNormal
		if( meshdata->vTangents.size() != 0 ){
			setMdxArrays->SetVertexBinormalCount(1) ;
		}else {
			setMdxArrays->SetVertexBinormalCount(0) ;
		}
#endif
	}else {
		setMdxArrays->SetVertexNormalCount(0) ;
#ifndef OLD_MDXCONV_SUPPORT
		// tangent Count	
		setMdxArrays->SetVertexTangentCount(0) ;
		//BiNormal
		setMdxArrays->SetVertexBinormalCount(0) ;
#endif
	}
	
	// Color Count
	if( meshdata->vColors.size() != 0 ){
		setMdxArrays->SetVertexColorCount(1) ;
	}else {
		setMdxArrays->SetVertexColorCount(0) ;
	}
	
	// BlendIndices Count
	size_t jointUsingNum = 0;
	if( meshdata->jointUsingList.size() != 0 ){

		MdxBlendIndices *setMdxBlendIndices = new MdxBlendIndices;
		setMdxMesh->AttachChild( setMdxBlendIndices );	
		for( size_t jointIdx = 0 ; jointIdx < meshdata->jointUsingList.size() ; jointIdx++ )
		{
			if ( meshdata->jointUsingList[jointIdx] ){
				setMdxBlendIndices->SetIndex( jointUsingNum, jointIdx );
				jointUsingNum++;
			}
		}
		setMdxBlendIndices->SetIndexCount( jointUsingNum );
		setMdxArrays->SetVertexWeightCount( jointUsingNum ) ;
	}

	// Texture Coord Count
	setMdxArrays->SetVertexTexCoordCount( meshdata->uvSetsNum  ) ;
					
	for( size_t vetexIdx = 0 ; vetexIdx < meshdata->vPositions.size(); vetexIdx++ )
	{
		MdxVertex &v = setMdxArrays->GetVertex( vetexIdx );
		
		// Postion
		if( meshdata->vPositions.size() != 0  )
		{
			v.SetPositionCount( 1 );
			v.SetPosition( meshdata->vPositions[vetexIdx] );
		}else {
			v.SetPositionCount( 0 );
		}
		
		// Normal
		if( meshdata->vNormals.size() != 0  )
		{
			v.SetNormalCount( 1 );
			v.SetNormal( meshdata->vNormals[vetexIdx] );

#ifndef OLD_MDXCONV_SUPPORT
			//Tangent 
			if( meshdata->vTangents.size() != 0  ){
				v.SetTangentCount( 1 );
				v.SetTangent( meshdata->vTangents[vetexIdx] );
			}else {
				v.SetTangentCount( 0 );
			}
			//BiNormal
			if( meshdata->vBinormals.size() != 0  ){
				v.SetBinormalCount( 1 );
				v.SetBinormal( meshdata->vBinormals[vetexIdx] );
			}else {
				v.SetBinormalCount( 0 );
			}
#endif
		}else {
			v.SetNormalCount( 0 );
#ifndef OLD_MDXCONV_SUPPORT
			v.SetTangentCount( 0 );
			v.SetBinormalCount( 0 );
#endif
		}

		// Color
		if( meshdata->vColors.size() != 0  )
		{
			v.SetColorCount( 1 );
			v.SetColor( meshdata->vColors[vetexIdx] );
		}else {
			v.SetColorCount( 0 );
		}
		// UV
		if( meshdata->vUVSet.size() != 0  )
		{
			v.SetTexCoordCount( (int)meshdata->uvSetsNum );
			for( size_t uvIdx= 0; uvIdx < meshdata->uvSetsNum ; uvIdx++ )
			{
				size_t getUVDataIdx = (meshdata->uvSetsNum * vetexIdx )+ uvIdx;
				v.SetTexCoord( uvIdx, meshdata->vUVSet[getUVDataIdx] );
			}
		}else {
			v.SetTexCoordCount( 0 );
		}

		if( meshdata->jointUsingList.size() != 0 )
		{
			size_t jointUsingCount = 0;
			for( size_t jointIdx = 0 ; jointIdx < meshdata->jointUsingList.size() ; jointIdx++ )
			{
				if ( meshdata->jointUsingList[jointIdx] ){
					const std::vector<float>& weightList = meshdata->jointWeightList[jointIdx];
					v.SetWeight( jointUsingCount, weightList[vetexIdx]  );
					jointUsingCount++;
				}
			}
			v.SetWeightCount( jointUsingCount );
		}else {
			v.SetWeightCount( 0 );
		}
	}

	
	if( morphTargetList.size() != 0 )
	{
		int format = 0 ;
		setMdxArrays->SetMorphCount( morphTargetList.size()+1 );
		format = setMdxArrays->GetFormat() ;
		if ( format != 0 ){
			setMdxArrays->SetMorphFormat( format );
			
			std::vector<daeDocMeshInfo>::iterator morphTargetListIte = morphTargetList.begin();
			for( size_t morphCnt = 0 ; morphTargetListIte != morphTargetList.end() ; morphTargetListIte++, morphCnt++ ){

				for( size_t vetexIdx = 0 ; vetexIdx < morphTargetListIte->vPositions.size(); vetexIdx++ )
				{
					MdxVertex &v = setMdxArrays->GetMorph( morphCnt+1 )->GetVertex( vetexIdx );
					
					// Postion
					if( morphTargetListIte->vPositions.size() != 0  )
					{
						v.SetPositionCount( 1 );
						v.SetPosition( morphTargetListIte->vPositions[vetexIdx] );
					}else {
						v.SetPositionCount( 0 );
					}
					
					// Normal
					if( morphTargetListIte->vNormals.size() != 0  )
					{
						v.SetNormalCount( 1 );
						v.SetNormal( morphTargetListIte->vNormals[vetexIdx] );

#ifndef OLD_MDXCONV_SUPPORT
						//Tangent 
						if( morphTargetListIte->vTangents.size() != 0  ){
							v.SetTangentCount( 1 );
							v.SetTangent( morphTargetListIte->vTangents[vetexIdx] );
						}else {
							v.SetTangentCount( 0 );
						}
						//BiNormal
						if( morphTargetListIte->vBinormals.size() != 0  ){
							v.SetBinormalCount( 1 );
							v.SetBinormal( morphTargetListIte->vBinormals[vetexIdx] );
						}else {
							v.SetBinormalCount( 0 );
						}
#endif
					}else {
						v.SetNormalCount( 0 );
#ifndef OLD_MDXCONV_SUPPORT
						v.SetTangentCount( 0 );
						v.SetBinormalCount( 0 );
#endif
					}

					// Color
					if( morphTargetListIte->vColors.size() != 0  )
					{
						v.SetColorCount( 1 );
						v.SetColor( morphTargetListIte->vColors[vetexIdx] );
					}else {
						v.SetColorCount( 0 );
					}
					// UV
					if( morphTargetListIte->vUVSet.size() != 0  )
					{
						v.SetTexCoordCount( (int)morphTargetListIte->uvSetsNum );
						for( size_t uvIdx= 0; uvIdx < morphTargetListIte->uvSetsNum ; uvIdx++ )
						{
							size_t getUVDataIdx = (morphTargetListIte->uvSetsNum * vetexIdx )+ uvIdx;
							v.SetTexCoord( uvIdx, morphTargetListIte->vUVSet[getUVDataIdx] );
						}
					}else {
						v.SetTexCoordCount( 0 );
					}
				}
			}
		}
	}	

	// IndexBuffer
	setMdxDrawArrays->SetMode( meshdata->mdxPrimType );
	setMdxDrawArrays->SetPrimCount( 1 );
	setMdxDrawArrays->SetVertexCount( meshdata->IndexBuffer.size() ) ;
	for( size_t ibIdx = 0 ; ibIdx < meshdata->IndexBuffer.size() ; ibIdx++ )
	{
		setMdxDrawArrays->SetIndex( ibIdx, meshdata->IndexBuffer[ibIdx]  );
	}

	

}
void DocumentImporter::make_SkinedMesh( const COLLADAFW::InstanceController* instanceController, MdxBone *setMdxNode  )
{
	const COLLADAFW::UniqueId& controllerId = instanceController->getInstanciatedObjectId();

	std::vector<daeDocSkinCtrlMapInfo>::iterator daeDocSkinCtrlMapIte = m_daeDocSkinCtrlMapList.begin() ;

	for( ; daeDocSkinCtrlMapIte != m_daeDocSkinCtrlMapList.end() ; daeDocSkinCtrlMapIte++ )
	{
		if( controllerId == daeDocSkinCtrlMapIte->SkinCtrlUniqId ){

			daeDocSkinCtrlDataInfo daeDocSkinCtrlData;
			bool getSkinData = false;
		
			std::vector<daeDocSkinCtrlDataInfo>::iterator daeDocSkinCtrlDataListIte = m_daeDocSkinCtrlDataList.begin();
			for( ; daeDocSkinCtrlDataListIte != m_daeDocSkinCtrlDataList.end() ; daeDocSkinCtrlDataListIte++ ){
				if( daeDocSkinCtrlMapIte->skinCtrDataUniqId == daeDocSkinCtrlDataListIte->skinCtrDatalUniqId ){
					daeDocSkinCtrlData = *daeDocSkinCtrlDataListIte;
					getSkinData = true;

					for( size_t joinIdx = 0 ; joinIdx <  daeDocSkinCtrlData.jointNum ; joinIdx++ ){
						
						const COLLADAFW::NodePointerArray& rootChiledNodes = m_wkvisualScene->getRootNodes();
						const COLLADAFW::Node* jointBoneNode = findNode( daeDocSkinCtrlMapIte->jointUniqIdList[joinIdx], rootChiledNodes );

						COLLADABU::Math::Matrix4 jointBoneMat =	daeDocSkinCtrlData.jointData[joinIdx].InverseBindMatrix;
						jointBoneMat = jointBoneMat * daeDocSkinCtrlData.bindPoseMatrix ;
						mat4 setMat4 ;
												
						setMat4.x.x = (float)jointBoneMat.getElement( 0, 0);
						setMat4.x.y = (float)jointBoneMat.getElement( 1, 0);
						setMat4.x.z = (float)jointBoneMat.getElement( 2, 0);
						setMat4.x.w = (float)jointBoneMat.getElement( 3, 0);
						setMat4.y.x = (float)jointBoneMat.getElement( 0, 1);
						setMat4.y.y = (float)jointBoneMat.getElement( 1, 1);
						setMat4.y.z = (float)jointBoneMat.getElement( 2, 1);
						setMat4.y.w = (float)jointBoneMat.getElement( 3, 1);
						setMat4.z.x = (float)jointBoneMat.getElement( 0, 2);
						setMat4.z.y = (float)jointBoneMat.getElement( 1, 2);
						setMat4.z.z = (float)jointBoneMat.getElement( 2, 2);
						setMat4.z.w = (float)jointBoneMat.getElement( 3, 2);
						setMat4.w.x = (float)jointBoneMat.getElement( 0, 3);
						setMat4.w.y = (float)jointBoneMat.getElement( 1, 3);
						setMat4.w.z = (float)jointBoneMat.getElement( 2, 3);
						setMat4.w.w = (float)jointBoneMat.getElement( 3, 3);

						const char *bone = jointBoneNode->getName().c_str() ;
						setMdxNode->AttachChild( new MdxBlendBone( bone, setMat4 ) ) ;
					}
	
					break;
				}
			}
			
			std::vector<daeDocGeometryInfo>::iterator geomMeshsIte = m_GeometryMeshList.begin() ;
			bool b_skinMeshOnly = false;	// true:skinMesh  false:Morph + skinMesh

			for( ; geomMeshsIte != m_GeometryMeshList.end() ; geomMeshsIte++ ){
				if( daeDocSkinCtrlMapIte->meshORcontrollerUniq == geomMeshsIte->geometryUniqueId ){
					b_skinMeshOnly = true;
					
					MdxPart* inputMdxPart = new MdxPart;
					std::string partName =  geomMeshsIte->geometryName  + "-Part";
					inputMdxPart->SetName( partName.c_str() );
						
					setMdxNode->AttachChild( new MdxDrawPart ( partName.c_str() ));
					m_mdxModel->AttachChild( inputMdxPart );


					std::vector<daeDocMeshInfo>::iterator meshIte = geomMeshsIte->Mesh.begin();
					for( size_t meshNo = 0; meshIte != geomMeshsIte->Mesh.end() ; meshIte++, meshNo++ ){
						std::stringstream strstMeshNo;
						strstMeshNo.clear();
						strstMeshNo << "-" << meshNo;

						MdxMesh* inputMdxMesh = new MdxMesh;
						std::string	meshName = geomMeshsIte->geometryName  + strstMeshNo.str();
						inputMdxMesh->SetName( meshName.c_str() );
						
						inputMdxPart->AttachChild( inputMdxMesh );

						MdxDrawArrays *setMdxDrawArrays = new MdxDrawArrays ;
						MdxArrays *setMdxArrays = new MdxArrays ;
						
						
						std::string	arraysName = inputMdxMesh->GetName();
						arraysName += "-arrays";
						setMdxArrays->SetName( arraysName.c_str() ); 
						
						inputMdxMesh->AttachChild( new MdxSetArrays( setMdxArrays->GetName() ) );

						inputMdxMesh->AttachChild( setMdxDrawArrays );	
						inputMdxPart->AttachChild( setMdxArrays );

						// IndexBuffer + VertexBufferを作成
						for ( size_t jointIdx = 0; jointIdx < daeDocSkinCtrlData.jointNum ; jointIdx++ ){
							std::vector<float> weightVertexBuffer;
							bool isBoneUsing = false ;
							
							std::vector<size_t>::iterator vertexIdxConvMapIte =  meshIte->vertexIdxConvMap.begin();
							for( ; vertexIdxConvMapIte != meshIte->vertexIdxConvMap.end() ; vertexIdxConvMapIte++ )
							{
								weightVertexBuffer.push_back(daeDocSkinCtrlData.jointData[jointIdx].vertexWeight[*vertexIdxConvMapIte] );
								if( daeDocSkinCtrlData.jointData[jointIdx].vertexWeight[*vertexIdxConvMapIte] != 0.0f )
								{
									isBoneUsing = true ;
								}
							}
							meshIte->jointUsingList.push_back( isBoneUsing );
							meshIte->jointWeightList.push_back( weightVertexBuffer) ;
						}
						
						// VertexBuffer
						setMdxArrays->SetVertexCount ( meshIte->vPositions.size() );
				
						// Postion Count
						if( meshIte->vPositions.size() != 0 ){
							setMdxArrays->SetVertexPositionCount(1) ;
						}else {
							setMdxArrays->SetVertexPositionCount(0) ;
						}
						
						// Normal Count
						if( meshIte->vNormals.size() != 0 ){
							setMdxArrays->SetVertexNormalCount(1) ;
#ifndef OLD_MDXCONV_SUPPORT
							// tangent Count
							if( meshIte->vTangents.size() != 0 ){
								setMdxArrays->SetVertexTangentCount(1) ;
							}else {
								setMdxArrays->SetVertexTangentCount(0) ;
							}
							//BiNormal
							if( meshIte->vTangents.size() != 0 ){
								setMdxArrays->SetVertexBinormalCount(1) ;
							}else {
								setMdxArrays->SetVertexBinormalCount(0) ;
							}
#endif
						}else {
							setMdxArrays->SetVertexNormalCount(0) ;
#ifndef OLD_MDXCONV_SUPPORT
							// tangent Count	
							setMdxArrays->SetVertexTangentCount(0) ;
							//BiNormal
							setMdxArrays->SetVertexBinormalCount(0) ;
#endif
						}
						
						// Color Count
						if( meshIte->vColors.size() != 0 ){
							setMdxArrays->SetVertexColorCount(1) ;
						}else {
							setMdxArrays->SetVertexColorCount(0) ;
						}
						
						// BlendIndices Count
						size_t jointUsingNum = 0;
						MdxBlendIndices *setMdxBlendIndices = new MdxBlendIndices;
						inputMdxMesh->AttachChild( setMdxBlendIndices );	
						for( size_t jointIdx = 0 ; jointIdx < meshIte->jointUsingList.size() ; jointIdx++ )
						{
							if ( meshIte->jointUsingList[jointIdx] ){
								setMdxBlendIndices->SetIndex( jointUsingNum, jointIdx );
								jointUsingNum++;
							}
						}
						setMdxBlendIndices->SetIndexCount( jointUsingNum );
						setMdxArrays->SetVertexWeightCount( jointUsingNum ) ;

						// Texture Coord Count
						setMdxArrays->SetVertexTexCoordCount( meshIte->uvSetsNum  ) ;
										
						for( size_t vetexIdx = 0 ; vetexIdx < meshIte->vPositions.size(); vetexIdx++ )
						{
							MdxVertex &v = setMdxArrays->GetVertex( vetexIdx );
							
							// Postion
							if( meshIte->vPositions.size() != 0  )
							{
								v.SetPositionCount( 1 );
								v.SetPosition( meshIte->vPositions[vetexIdx] );
							}else {
								v.SetPositionCount( 0 );
							}
							
							// Normal
							if( meshIte->vNormals.size() != 0  )
							{
								v.SetNormalCount( 1 );
								v.SetNormal( meshIte->vNormals[vetexIdx] );

#ifndef OLD_MDXCONV_SUPPORT
								//Tangent 
								if( meshIte->vTangents.size() != 0  ){
									v.SetTangentCount( 1 );
									v.SetTangent( meshIte->vTangents[vetexIdx] );
								}else {
									v.SetTangentCount( 0 );
								}
								//BiNormal
								if( meshIte->vBinormals.size() != 0  ){
									v.SetBinormalCount( 1 );
									v.SetBinormal( meshIte->vBinormals[vetexIdx] );
								}else {
									v.SetBinormalCount( 0 );
								}
#endif
							}else {
								v.SetNormalCount( 0 );
#ifndef OLD_MDXCONV_SUPPORT
								v.SetTangentCount( 0 );
								v.SetBinormalCount( 0 );
#endif
							}

							// Color
							if( meshIte->vColors.size() != 0  )
							{
								v.SetColorCount( 1 );
								v.SetColor( meshIte->vColors[vetexIdx] );
							}else {
								v.SetColorCount( 0 );
							}
							// UV
							if( meshIte->vUVSet.size() != 0  )
							{
								v.SetTexCoordCount( (int)meshIte->uvSetsNum );
								for( size_t uvIdx= 0; uvIdx < meshIte->uvSetsNum ; uvIdx++ )
								{
									size_t getUVDataIdx = (meshIte->uvSetsNum * vetexIdx )+ uvIdx;
									v.SetTexCoord( uvIdx, meshIte->vUVSet[getUVDataIdx] );
								}
							}else {
								v.SetTexCoordCount( 0 );
							}

							if( meshIte->jointUsingList.size() != 0 )
							{
								size_t jointUsingCount = 0;
								for( size_t jointIdx = 0 ; jointIdx < meshIte->jointUsingList.size() ; jointIdx++ )
								{
									if ( meshIte->jointUsingList[jointIdx] ){
										const std::vector<float>& weightList = meshIte->jointWeightList[jointIdx];
										v.SetWeight( jointUsingCount, weightList[vetexIdx]  );
										jointUsingCount++;
									}
								}
								v.SetWeightCount( jointUsingCount );
							}else {
								v.SetWeightCount( 0 );
							}
						}
						

						// IndexBuffer
						setMdxDrawArrays->SetMode( meshIte->mdxPrimType );
						setMdxDrawArrays->SetPrimCount( 1 );
						setMdxDrawArrays->SetVertexCount( meshIte->IndexBuffer.size() ) ;
						for( size_t ibIdx = 0 ; ibIdx < meshIte->IndexBuffer.size() ; ibIdx++ )
						{
							setMdxDrawArrays->SetIndex( ibIdx, meshIte->IndexBuffer[ibIdx]  );
						}


						// Material
						const COLLADAFW::MaterialBindingArray& wkMaiterialbindgArray =  instanceController->getMaterialBindings();
						for ( size_t matbindingCnt = 0; matbindingCnt <  wkMaiterialbindgArray.getCount() ; matbindingCnt++ )
						{
							const COLLADAFW::MaterialId matId = wkMaiterialbindgArray[matbindingCnt].getMaterialId();
							const COLLADAFW::UniqueId& matUniqueId = wkMaiterialbindgArray[matbindingCnt].getReferencedMaterial() ;

							
							if( matId == meshIte->meshMaterialID ){
								std::string getMatelalName;
								if( findMdxMaterialName(matUniqueId, getMatelalName )){
									inputMdxMesh->AttachChild( new MdxSetMaterial( getMatelalName.c_str() ) ) ;
								}
							
								// Texture Index Connection？？
								const COLLADAFW::TextureCoordinateBindingArray& wkTexCoordinateBindingArray = wkMaiterialbindgArray[matbindingCnt].getTextureCoordinateBindingArray(); 
								for ( size_t texcdbdCnt = 0; texcdbdCnt <  wkTexCoordinateBindingArray.getCount() ; texcdbdCnt++ )
								{
									//COLLADAFW::TextureCoordinateBinding wkTextureCoordinateBinding = wkTexCoordinateBindingArray[texcdbdCnt];
									//
									//COLLADAFW::TextureMapId texMapId = wkTextureCoordinateBinding.getTextureMapId();
									//wkTextureCoordinateBinding.getTextureMapId();
									//wkTextureCoordinateBinding.getSetIndex()
								}
							}
						}
						
						
					}
					
					
					break;
				}		
				
			}

			if( b_skinMeshOnly )break;

			// Morph
			std::vector<daeDocMorphCtrlMapInfo>::iterator daeDocMorphCtrlMapListIte = m_daeDocMorphCtrlMapList.begin() ;
			for( ; daeDocMorphCtrlMapListIte != m_daeDocMorphCtrlMapList.end() ; daeDocMorphCtrlMapListIte++ )
			{
				if( daeDocSkinCtrlMapIte->meshORcontrollerUniq == daeDocMorphCtrlMapListIte->MorphCtrlUniqId ){
					std::vector<daeDocGeometryInfo>::iterator geomMeshsIte = m_GeometryMeshList.begin() ;

					for( ; geomMeshsIte != m_GeometryMeshList.end() ; geomMeshsIte++ ){
						if( daeDocMorphCtrlMapListIte->meshORcontrollerUniq == geomMeshsIte->geometryUniqueId ){
						
							geomMeshsIte->morphWeightAnimationListId = daeDocMorphCtrlMapListIte->morphWeightAnimationListId ;
							std::vector<morphTargetInfo>::iterator	morphTargetIte = daeDocMorphCtrlMapListIte->morphTarget.begin();  
						
							// TargetSet
							for (; morphTargetIte != daeDocMorphCtrlMapListIte->morphTarget.end() ; morphTargetIte++ ) 
							{
								std::vector<daeDocGeometryInfo>::iterator checkMeshsIte = m_GeometryMeshList.begin() ;
								for( ; checkMeshsIte != m_GeometryMeshList.end() ; checkMeshsIte++ ){
									if( morphTargetIte->geomUniqId == checkMeshsIte->geometryUniqueId ){
										geomMeshsIte->morphTaget_subMeshCount.push_back ( checkMeshsIte->Mesh.size() );
										vector<daeDocMeshInfo>::iterator targetSubMeshIte = checkMeshsIte->Mesh.begin();
										for( ; targetSubMeshIte != checkMeshsIte->Mesh.end() ; targetSubMeshIte++ ){ 
											geomMeshsIte->morphTagetMesh.push_back( *targetSubMeshIte );
										}
									}
								}		
							}

						
							MdxPart* inputMdxPart = new MdxPart;
							std::string partName =  geomMeshsIte->geometryName  + "-Part";
							inputMdxPart->SetName( partName.c_str() );
								
							setMdxNode->AttachChild( new MdxDrawPart ( partName.c_str()  ));
							m_mdxModel->AttachChild( inputMdxPart );

							std::vector<daeDocMeshInfo>::iterator meshIte = geomMeshsIte->Mesh.begin();
							for( size_t meshNo = 0; meshIte != geomMeshsIte->Mesh.end() ; meshIte++, meshNo++ ){
							
								std::stringstream strstMeshNo;
								strstMeshNo.clear();
								strstMeshNo << "-" << meshNo;

								MdxMesh* inputMdxMesh = new MdxMesh;
								std::string	meshName = geomMeshsIte->geometryName  + strstMeshNo.str();
								inputMdxMesh->SetName( meshName.c_str() );
								
								inputMdxPart->AttachChild( inputMdxMesh );

								MdxDrawArrays *setMdxDrawArrays = new MdxDrawArrays ;
								MdxArrays *setMdxArrays = new MdxArrays ;
								
								
								std::string	arraysName = inputMdxMesh->GetName();
								arraysName += "-arrays";
								setMdxArrays->SetName( arraysName.c_str() ); 
								
								inputMdxMesh->AttachChild( new MdxSetArrays( setMdxArrays->GetName() ) );

								inputMdxMesh->AttachChild( setMdxDrawArrays );	
								inputMdxPart->AttachChild( setMdxArrays );
								

								vector<daeDocMeshInfo>	inputMorphTaget_subMesh;
								inputMorphTaget_subMesh.clear();
								size_t getTagetIndex = 0;
								for( size_t targetsubMeshCnt= 0; targetsubMeshCnt < geomMeshsIte->morphTaget_subMeshCount.size() ; targetsubMeshCnt++ ){
								
									inputMorphTaget_subMesh.push_back( geomMeshsIte->morphTagetMesh[getTagetIndex+meshNo] ) ;
									getTagetIndex = getTagetIndex + geomMeshsIte->morphTaget_subMeshCount[targetsubMeshCnt];
									
								}
								
								set_MeshInfo( &(*meshIte), inputMdxMesh, setMdxArrays, setMdxDrawArrays, inputMorphTaget_subMesh );
								
								COLLADAFW::AnimationList*			animationList = findAnimationList( geomMeshsIte->morphWeightAnimationListId );
								if( animationList != 0 ){
									const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = animationList->getAnimationBindings();
									size_t													numAnimationBindings = animationBindings.getCount();
									std::vector<MdxFCurve*>  morph_animCurveList;
									morph_animCurveList.clear();

									for ( size_t anims=0; anims<numAnimationBindings; anims++ )
									{
										const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
										const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

										daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
										if ( !animCurveInfo )
										{
											continue;
										}
																	
										morph_animCurveList.push_back( animCurveInfo->animationCurve );
										
									}
									
									if( morph_animCurveList.size() != 0 )
									{
										
										MdxAnimate*		mdxAnimate = new MdxAnimate();
										MdxFCurve*		morphWeightFCurve = new MdxFCurve() ;
										

										mdxAnimate->SetScope( setMdxNode->GetTypeID() );
										mdxAnimate->SetBlock( setMdxNode->GetName() );
										mdxAnimate->SetCommand( MDX_MORPH_WEIGHTS );
										mdxAnimate->SetMode( 0 );
										
										mergeMorphWeightMdxFCurves(morphWeightFCurve, morph_animCurveList);

										 
										morphWeightFCurve->SetName( ( std::string( setMdxNode->GetName() ) ).c_str() );

										mdxAnimate->SetFCurve( morphWeightFCurve->GetName() );
										m_mdxMotion->AttachChild( mdxAnimate );
										m_mdxMotion->AttachChild( morphWeightFCurve );
										
									}
								}
								// Material
								const COLLADAFW::MaterialBindingArray& wkMaiterialbindgArray =  instanceController->getMaterialBindings();
								for ( size_t matbindingCnt = 0; matbindingCnt <  wkMaiterialbindgArray.getCount() ; matbindingCnt++ )
								{
									const COLLADAFW::MaterialId matId = wkMaiterialbindgArray[matbindingCnt].getMaterialId();
									const COLLADAFW::UniqueId& matUniqueId = wkMaiterialbindgArray[matbindingCnt].getReferencedMaterial() ;

									
									if( matId == meshIte->meshMaterialID ){
										std::string getMatelalName;
										if( findMdxMaterialName(matUniqueId, getMatelalName )){
											inputMdxMesh->AttachChild( new MdxSetMaterial( getMatelalName.c_str() ) ) ;
										}
									
										// Texture Index Connection？？
										const COLLADAFW::TextureCoordinateBindingArray& wkTexCoordinateBindingArray = wkMaiterialbindgArray[matbindingCnt].getTextureCoordinateBindingArray(); 
										for ( size_t texcdbdCnt = 0; texcdbdCnt <  wkTexCoordinateBindingArray.getCount() ; texcdbdCnt++ )
										{
											//COLLADAFW::TextureCoordinateBinding wkTextureCoordinateBinding = wkTexCoordinateBindingArray[texcdbdCnt];
											//
											//COLLADAFW::TextureMapId texMapId = wkTextureCoordinateBinding.getTextureMapId();
											//wkTextureCoordinateBinding.getTextureMapId();
											//wkTextureCoordinateBinding.getSetIndex()
										}
									}
								}
							}
						}	// GeoMetry ID Check
					}	// GeoMetry roop
				}	// MorphCtrl ID Check
			} // MorphCtrl roop
			break;	
		}
	}
}


void DocumentImporter::set_NodeInfo( const COLLADAFW::Node *node, MdxBone *mdxNode, MdxBone *parentMdxNode )
{
	// DrawPart
	const COLLADAFW::InstanceGeometryArray& geometryInstances = node->getInstanceGeometries ();
	
	size_t numInstances = geometryInstances.getCount ();
	for ( size_t i=0; i<numInstances; ++i )
	{
		MdxPart* inputMdxPart = NULL;
		const COLLADAFW::InstanceGeometry* instanceGeometry = geometryInstances [i];
		const COLLADAFW::UniqueId& geometryId = instanceGeometry->getInstanciatedObjectId ();
		
		// Mesh 
		make_Mesh( instanceGeometry, mdxNode );
	}

	// SkinedMesh or MorphingMesh Connection
	const COLLADAFW::InstanceControllerPointerArray& controllerInstances = node->getInstanceControllers();
	numInstances = controllerInstances.getCount ();
	for ( size_t i=0; i<numInstances; ++i )
	{
        const COLLADAFW::InstanceController* instanceController = controllerInstances [i];

		// Skin
		make_SkinedMesh( instanceController, mdxNode ) ;
		
		// Morph
		make_MorphingMesh( instanceController, mdxNode ) ;
        
	}

	// importTransformations
	std::vector<MdxFCurve*>				translateFCurveList;
	std::vector<MdxFCurve*>				rotateFCurveList;
	std::vector<MdxFCurve*>				scaleFCurveList;
	std::vector<vec4>					translateStaticVecList;
	std::vector<vec4>					rotateStaticAxisAngleVecList;
	std::vector<vec4>					scaleStaticVecList;
	std::vector<TransformVectorType>	translateAnimMergeOrder;
	std::vector<TransformVectorType>	translateStaticMergeOrder;
	std::vector<TransformVectorType>	rotateAnimMergeOrder;
	std::vector<TransformVectorType>	rotateStaticMergeOrder;
	std::vector<TransformVectorType>	scaleAnimMergeOrder;
	std::vector<TransformVectorType>	scaleStaticMergeOrder;
	bool								hasTranslateAnim = false;
	bool								hasRotateAnim = false;
	bool								hasScaleAnim = false;


	

	const COLLADAFW::TransformationArray& transforms = node->getTransformations ();
	
	size_t numTransforms = transforms.getCount();

	for ( size_t i=0; i < numTransforms ; ++i )
	{
		const COLLADAFW::Transformation*	transformation = transforms[i];
		const COLLADAFW::UniqueId&			animationListId = transformation->getAnimationList();
		COLLADAFW::AnimationList*			animationList = findAnimationList( animationListId );


		switch ( transformation->getTransformationType () )
		{
			/** Translate **/
			case COLLADAFW::Transformation::TRANSLATE:
			{
				/* get static translate */
				COLLADAFW::Translate*		translateTransform = ( COLLADAFW::Translate* )transformation;
				COLLADABU::Math::Vector3	translateVec = translateTransform->getTranslation ();
				vec4						mdxTranslateVec;

				mdxTranslateVec.x = (float)translateVec.x;
				mdxTranslateVec.y = (float)translateVec.y;
				mdxTranslateVec.z = (float)translateVec.z;

				translateStaticVecList.push_back( mdxTranslateVec );
				translateStaticMergeOrder.push_back( TRANSVECTYPE_XYZ );

				/* get animated translate */
				if ( animationList != NULL )
				{
					const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = animationList->getAnimationBindings();
					size_t													numAnimationBindings = animationBindings.getCount();


					for ( size_t anims=0; anims<numAnimationBindings; anims++ )
					{
						const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
						const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

						daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
						if ( !animCurveInfo )
						{
							continue;
						}

						switch ( animationBinding.animationClass )
						{
							case COLLADAFW::AnimationList::POSITION_X:
							{
								translateFCurveList.push_back( animCurveInfo->animationCurve );
								translateAnimMergeOrder.push_back( TRANSVECTYPE_X );
								break;
							}
							case COLLADAFW::AnimationList::POSITION_Y:
							{
								translateFCurveList.push_back( animCurveInfo->animationCurve );
								translateAnimMergeOrder.push_back( TRANSVECTYPE_Y );
								break;
							}
							case COLLADAFW::AnimationList::POSITION_Z:
							{
								translateFCurveList.push_back( animCurveInfo->animationCurve );
								translateAnimMergeOrder.push_back( TRANSVECTYPE_Z );
								break;
							}
							case COLLADAFW::AnimationList::POSITION_XYZ:
							{
								translateFCurveList.push_back( animCurveInfo->animationCurve );
								translateAnimMergeOrder.push_back( TRANSVECTYPE_XYZ );
								break;
							}
							default:
							{
								continue;
							}
						}
					}

					hasTranslateAnim = true;
				} else
				{
					translateFCurveList.push_back( NULL );
					translateAnimMergeOrder.push_back( TRANSVECTYPE_NONE );
				}


				break;
			}

			/** Rotate **/
			case COLLADAFW::Transformation::ROTATE:
			{
				COLLADAFW::Rotate*			rotateTransform = ( COLLADAFW::Rotate* )transformation;
				COLLADABU::Math::Vector3&	axisVec = rotateTransform->getRotationAxis();
				double						angleValue = rotateTransform->getRotationAngle ();


				/* get static rotation */
				if ( axisVec == COLLADABU::Math::Vector3::UNIT_X )
				{
					vec4	mdxRotateAxisAngleVec( 1.0f, 0.0f, 0.0f, (float)angleValue );

					rotateStaticAxisAngleVecList.push_back( mdxRotateAxisAngleVec );
					rotateStaticMergeOrder.push_back( TRANSVECTYPE_X );
				} else
				if ( axisVec == COLLADABU::Math::Vector3::UNIT_Y )
				{
					vec4	mdxRotateAxisAngleVec( 0.0f, 1.0f, 0.0f, (float)angleValue );

					rotateStaticAxisAngleVecList.push_back( mdxRotateAxisAngleVec );
					rotateStaticMergeOrder.push_back( TRANSVECTYPE_Y );
				} else
				if ( axisVec == COLLADABU::Math::Vector3::UNIT_Z )
				{
					vec4	mdxRotateAxisAngleVec( 0.0f, 0.0f, 1.0f, (float)angleValue );

					rotateStaticAxisAngleVecList.push_back( mdxRotateAxisAngleVec );
					rotateStaticMergeOrder.push_back( TRANSVECTYPE_Z );
				} else
				{
					quat	mdxRotateQuat;

					// AnglAxis to Quaternion
					mdxRotateQuat = quat_from_rotate( (float)axisVec.x, (float)axisVec.y, (float)axisVec.z, (float)angleValue );

					rotateStaticAxisAngleVecList.push_back( *(vec4*)&mdxRotateQuat );
					rotateStaticMergeOrder.push_back( TRANSVECTYPE_QUAT );
				}


				/* get animated rotation */
				if ( animationList != NULL )
				{
					const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = animationList->getAnimationBindings();
					size_t													numAnimationBindings = animationBindings.getCount();


					for ( size_t anims=0; anims<numAnimationBindings; anims++ )
					{
						const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
						const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

						daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
						if ( !animCurveInfo )
						{
							continue;
						}

						switch ( animationBinding.animationClass )
						{
							case COLLADAFW::AnimationList::ANGLE:
							{
								if ( axisVec == COLLADABU::Math::Vector3::UNIT_X )
								{
									rotateAnimMergeOrder.push_back( TRANSVECTYPE_X );
								} else
								if ( axisVec == COLLADABU::Math::Vector3::UNIT_Y )
								{
									rotateAnimMergeOrder.push_back( TRANSVECTYPE_Y );
								} else
								if ( axisVec == COLLADABU::Math::Vector3::UNIT_Z )
								{
									rotateAnimMergeOrder.push_back( TRANSVECTYPE_Z );
								} else
								{
									break;
								}

								rotateFCurveList.push_back( animCurveInfo->animationCurve );
								
								break;
							}
							case COLLADAFW::AnimationList::AXISANGLE:
							{
								rotateAnimMergeOrder.push_back( TRANSVECTYPE_AXISANGLE );
								rotateFCurveList.push_back( animCurveInfo->animationCurve );
								break;
							}
							default:
							{
								continue;
							}
						}
					}

					hasRotateAnim = true;
				} else
				{
					rotateFCurveList.push_back( NULL );
					rotateAnimMergeOrder.push_back( TRANSVECTYPE_NONE );
				}

				break;
			}

			/** Scale **/
			case COLLADAFW::Transformation::SCALE:
			{
				/* get static scale */
				COLLADAFW::Scale*			scaleTransform = ( COLLADAFW::Scale* )transformation;
				COLLADABU::Math::Vector3	scaleVec = scaleTransform->getScale() ;
				vec4						mdxScaleVec;

				mdxScaleVec.x = (float)scaleVec.x;
				mdxScaleVec.y = (float)scaleVec.y;
				mdxScaleVec.z = (float)scaleVec.z;

				scaleStaticVecList.push_back( mdxScaleVec );
				scaleStaticMergeOrder.push_back( TRANSVECTYPE_XYZ );


				/* get animated scale */
				if ( animationList != NULL )
				{
					const COLLADAFW::AnimationList::AnimationBindings&		animationBindings = animationList->getAnimationBindings();
					size_t													numAnimationBindings = animationBindings.getCount();


					for ( size_t anims=0; anims<numAnimationBindings; anims++ )
					{
						const COLLADAFW::AnimationList::AnimationBinding&	animationBinding = animationBindings[anims]; 
						const COLLADAFW::UniqueId&							animationId = animationBinding.animation;

						daeDocAnimCurveInfo*		animCurveInfo = findAnimCurveInfo( animationId );
						if ( !animCurveInfo )
						{
							continue;
						}

						switch ( animationBinding.animationClass )
						{
							case COLLADAFW::AnimationList::POSITION_X:
							{
								scaleFCurveList.push_back( animCurveInfo->animationCurve );
								scaleAnimMergeOrder.push_back( TRANSVECTYPE_X );
								break;
							}
							case COLLADAFW::AnimationList::POSITION_Y:
							{
								scaleFCurveList.push_back( animCurveInfo->animationCurve );
								scaleAnimMergeOrder.push_back( TRANSVECTYPE_Y );
								break;
							}
							case COLLADAFW::AnimationList::POSITION_Z:
							{
								scaleFCurveList.push_back( animCurveInfo->animationCurve );
								scaleAnimMergeOrder.push_back( TRANSVECTYPE_Z );
								break;
							}
							case COLLADAFW::AnimationList::POSITION_XYZ:
							{
								scaleFCurveList.push_back( animCurveInfo->animationCurve );
								scaleAnimMergeOrder.push_back( TRANSVECTYPE_XYZ );
								break;
							}
							default:
							{
								continue;
							}
						}
					}

					hasScaleAnim = true;
				} else
				{
					scaleFCurveList.push_back( NULL );
					scaleAnimMergeOrder.push_back( TRANSVECTYPE_NONE );
				}
				break;
			}
			case COLLADAFW::Transformation::MATRIX:
			case COLLADAFW::Transformation::LOOKAT:
			case COLLADAFW::Transformation::SKEW:
			default:
			{
				break;
			}
		}
	}


	if( translateStaticVecList.size() > 0 )
	{
		vec4		staticTranslateVec(0.0f, 0.0f, 0.0f, 1.0f);

		for ( size_t i=0; i<translateStaticVecList.size(); i++ )
		{
			staticTranslateVec += translateStaticVecList[i];
		}

		MdxTranslate*	staticMdxTranslate = new MdxTranslate( staticTranslateVec );

		mdxNode->AttachChild( staticMdxTranslate ); 
	}

	if( rotateStaticAxisAngleVecList.size() > 0 )
	{
		quat		rotQuat;

		for ( size_t i=0; i<rotateStaticAxisAngleVecList.size(); i++ )
		{
			quat		wkQuat;
			vec4*		v = &rotateStaticAxisAngleVecList[i];

			wkQuat = quat_from_rotate( v->x, v->y, v->z, DegToRad( v->w ) );
			rotQuat = rotQuat * wkQuat;
		}

		MdxRotate*		staticMdxRotate = ( new MdxRotate( rotQuat ) );
		mdxNode->AttachChild( staticMdxRotate ); 
	}


	if( scaleStaticVecList.size() > 0 )
	{
		vec4		staticScaleVec(0.0f, 0.0f, 0.0f, 1.0f);

		for ( size_t i=0; i<scaleStaticVecList.size(); i++ )
		{
			staticScaleVec += scaleStaticVecList[i];
		}

		MdxScale*		staticMdxScale = new MdxScale( staticScaleVec );

		mdxNode->AttachChild( staticMdxScale ); 
	}

	if ( hasTranslateAnim )
	{
		MdxAnimate*		mdxAnimate = new MdxAnimate();
		MdxFCurve*		translateFCurve;

		mdxAnimate->SetScope( mdxNode->GetTypeID() );
		mdxAnimate->SetBlock( mdxNode->GetName() );
		mdxAnimate->SetCommand( MDX_TRANSLATE );
		mdxAnimate->SetMode( 0 );

		if ( translateAnimMergeOrder.size() == 1 && translateAnimMergeOrder[0] == TRANSVECTYPE_XYZ )
		{
			translateFCurve = translateFCurveList[0];
		} else
		{
			translateFCurve = new MdxFCurve;

			mergeMdxFCurves( translateFCurve, 3, translateFCurveList, translateAnimMergeOrder );
		}

		translateFCurve->SetName( ( std::string( mdxNode->GetName() ) + "_Translate" ).c_str() );

		mdxAnimate->SetFCurve( translateFCurve->GetName() );
		m_mdxMotion->AttachChild( mdxAnimate );
		m_mdxMotion->AttachChild( translateFCurve );
	}

	if ( hasRotateAnim )
	{
		MdxAnimate*		mdxAnimate = new MdxAnimate();
		MdxFCurve*		rotateFCurve;

		mdxAnimate->SetScope( mdxNode->GetTypeID() );
		mdxAnimate->SetBlock( mdxNode->GetName() );
		mdxAnimate->SetMode( 0 );

		{
			mdxAnimate->SetCommand( MDX_ROTATE );

			rotateFCurve = new MdxFCurve();

			rotateFCurve->SetName( ( std::string( mdxNode->GetName() ) + "_Quat" ).c_str() );

			/* 回転のアニメーションカーブを全て合成して１つの Quaternion カーブに変換 */
			mergeMdxFCurveQuat( rotateFCurve, rotateFCurveList, rotateStaticAxisAngleVecList, rotateAnimMergeOrder );
		}

		mdxAnimate->SetFCurve( rotateFCurve->GetName() );

		m_mdxMotion->AttachChild( mdxAnimate );
		m_mdxMotion->AttachChild( rotateFCurve );
	}

	if ( hasScaleAnim )
	{
		MdxAnimate*		mdxAnimate = new MdxAnimate();
		MdxFCurve*		scaleFCurve;

		mdxAnimate->SetScope( mdxNode->GetTypeID() );
		mdxAnimate->SetBlock( mdxNode->GetName() );
		mdxAnimate->SetCommand( MDX_SCALE );
		mdxAnimate->SetMode( 0 );

		if ( ( scaleAnimMergeOrder.size() == 1 ) &&
			 ( scaleAnimMergeOrder[0] == TRANSVECTYPE_XYZ ) )
		{
			scaleFCurve = scaleFCurveList[0];
		} else
		{
			scaleFCurve = new MdxFCurve;

			mergeMdxFCurves( scaleFCurve, 3, scaleFCurveList, scaleAnimMergeOrder );
		}

		scaleFCurve->SetName( ( std::string( mdxNode->GetName() ) + "_Scale" ).c_str() );

		mdxAnimate->SetFCurve( scaleFCurve->GetName() );
		m_mdxMotion->AttachChild( mdxAnimate );
		m_mdxMotion->AttachChild( scaleFCurve );
	}
}



bool DocumentImporter::findMdxMaterialName( const COLLADAFW::UniqueId& findId, std::string& getMaterialName )
{
	
	bool findflg = false;

	std::vector<MdxMaterialMapInfo*>::iterator MdxMaterialMapIte = m_MdxMaterialMap.begin() ;
	
	
	for( ;MdxMaterialMapIte != m_MdxMaterialMap.end(); MdxMaterialMapIte++ )
	{
		const COLLADAFW::UniqueId& chekId = (*MdxMaterialMapIte)->first ;

		if ( findId == chekId ){
			MdxMaterial*  getfindMdxMaterial = (*MdxMaterialMapIte)->second ;
			getMaterialName = getfindMdxMaterial->GetName(); 
			findflg = true;
			break;
		}
	}

	return findflg;
}


bool DocumentImporter::findMdxTextureName( const COLLADAFW::UniqueId& findId, std::string& getTextureName )
{
	
	bool findflg = false;

	std::vector<MdxTextureMapInfo*>::iterator MdxTextureMapIte = m_MdxTextureMap.begin() ;
	
	
	for( ;MdxTextureMapIte != m_MdxTextureMap.end(); MdxTextureMapIte++ )
	{
		const COLLADAFW::UniqueId& chekId = (*MdxTextureMapIte)->imageUniqueId ;

		if ( findId == chekId ){
			MdxTexture*  getfindMdxTexture = (*MdxTextureMapIte)->mdxTextureData ;
			getTextureName = getfindMdxTexture->GetName(); 
			findflg = true;
			break;
		}
	}

	return findflg;
}

bool DocumentImporter::findMdxTextureName( const std::string  findName, std::string& getTextureName )
{	
	bool findflg = false;

	std::vector<MdxTextureMapInfo*>::iterator MdxTextureMapIte = m_MdxTextureMap.begin() ;
	
	
	for( ;MdxTextureMapIte != m_MdxTextureMap.end(); MdxTextureMapIte++ )
	{
		const std::string chekname = (*MdxTextureMapIte)->name ;

		if ( findName == chekname ){
			MdxTexture*  getfindMdxTexture = (*MdxTextureMapIte)->mdxTextureData ;
			getTextureName = getfindMdxTexture->GetName(); 
			findflg = true;
			break;
		}
	}

	return findflg;
}


DocumentImporter::~DocumentImporter()
{
	
	
}
void DocumentImporter::cancel( const COLLADAFW::String& errorMessage )
{
}

//--------------------------------------------------------------------
void DocumentImporter::start()
{
}

//--------------------------------------------------------------------
void DocumentImporter::finish()
{
}

bool DocumentImporter::writeGlobalAsset ( const COLLADAFW::FileInfo* asset )
{

	const COLLADAFW::FileInfo::Unit& unit = asset->getUnit ();

	
	switch ( unit.getLinearUnitUnit () )
	{
	case COLLADAFW::FileInfo::Unit::KILOMETER:
	case COLLADAFW::FileInfo::Unit::METER:
	case COLLADAFW::FileInfo::Unit::DECIMETER:
	case COLLADAFW::FileInfo::Unit::CENTIMETER:
	case COLLADAFW::FileInfo::Unit::MILLIMETER:
	case COLLADAFW::FileInfo::Unit::FOOT:
	case COLLADAFW::FileInfo::Unit::INCH:
	case COLLADAFW::FileInfo::Unit::YARD:
	default:
		break;
	}


	m_UpAxisType = asset->getUpAxisType ();

	return true;
}

bool DocumentImporter::writeScene ( const COLLADAFW::Scene* scene )
{

	if ( m_ParseStep <= COPY_ELEMENTS ) 
	{
		// Make a copy of the instantiated visual scene element.
		m_ParseStep = COPY_ELEMENTS;
		m_InstanceVisualScene = scene->getInstanceVisualScene ()->clone ();
		
	}
	return true;
}

bool DocumentImporter::writeVisualScene ( const COLLADAFW::VisualScene* visualScene )
{

	m_VisualScenesList.push_back ( new COLLADAFW::VisualScene ( *visualScene ) );
	return true;
}

bool DocumentImporter::writeLibraryNodes( const COLLADAFW::LibraryNodes* libraryNodes )
{
	

	if ( m_ParseStep <= COPY_ELEMENTS )
	{
		// Make a copy of the library nodes element and push it into the list of library nodes.
		m_ParseStep = COPY_ELEMENTS;
		m_LibraryNodesList.push_back ( new COLLADAFW::LibraryNodes ( *libraryNodes ) );
	}

	return true;
}

bool DocumentImporter::writeGeometry ( const COLLADAFW::Geometry* geometry )
{
	
	
	COLLADAFW::Geometry::GeometryType type = geometry->getType ();
	
	if( type == COLLADAFW::Geometry::GEO_TYPE_MESH ){
		daeDocGeometryInfo inputMesh;
		
		inputMesh.geometryUniqueId = geometry->getUniqueId();
		inputMesh.geometryName = geometry->getName();

		COLLADAFW::Mesh* mesh = ( COLLADAFW::Mesh* ) geometry;

		const COLLADAFW::MeshVertexData& vPositons( mesh->getPositions() );
		const COLLADAFW::MeshVertexData& vColors( mesh->getColors() );
		const COLLADAFW::MeshVertexData& vNormals( mesh->getNormals() ); 
		const COLLADAFW::MeshVertexData& vUVs( mesh->getUVCoords() );

		// VertexBuffer
		const COLLADAFW::MeshPrimitiveArray& primitiveElementsArray = mesh->getMeshPrimitives ();
		size_t count = primitiveElementsArray.getCount ();
		for ( size_t i=0; i<count; ++i )
		{
			const COLLADAFW::MeshPrimitive* primitiveElement = primitiveElementsArray [ i ];
			size_t vIdxSize = primitiveElement->getPositionIndices().getCount() ;



			daeDocMeshInfo setdaeDocSubMesh; 
			
			// indexbuf
			const COLLADAFW::UIntValuesArray& vPosIdx = primitiveElement->getPositionIndices();
			const COLLADAFW::UIntValuesArray& vNorIdx = primitiveElement->getNormalIndices();
			const COLLADAFW::IndexList*		  vColorIdx = primitiveElement->getColorIndices(0);
			
			for( size_t indexCnt = 0; indexCnt < vIdxSize ; indexCnt++ )
			{
					
				if( !vPositons.empty() ) {
					// Postion
					vec4 setvPos;
					
					unsigned int vertexIndex = vPosIdx[indexCnt];
					if( vPositons.getType() == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
						const COLLADAFW::FloatArray *getvPos = vPositons.getFloatValues();
						setvPos.x = (*getvPos)[0+ vertexIndex*3];
						setvPos.y = (*getvPos)[1+ vertexIndex*3];
						setvPos.z = (*getvPos)[2+ vertexIndex*3];
					}else {
						const COLLADAFW::DoubleArray *getvPos = vPositons.getDoubleValues();
						setvPos.x = (float)(*getvPos)[0+ vertexIndex*3];
						setvPos.y = (float)(*getvPos)[1+ vertexIndex*3];
						setvPos.z = (float)(*getvPos)[2+ vertexIndex*3];
					}
					
					setdaeDocSubMesh.vPositions.push_back( setvPos ) ;
					
					// skin Vertex Map [new index Map, old index]
					setdaeDocSubMesh.vertexIdxConvMap.push_back( vertexIndex ); 

				}
				
				if( !vNormals.empty() ) {
					// Normal
					vec4 setvNormal;
					
					unsigned int vertexIndex = vNorIdx[indexCnt];
					if( vNormals.getType() == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
						const COLLADAFW::FloatArray *getvNormal = vNormals.getFloatValues();
						setvNormal.x = (*getvNormal)[0+ vertexIndex*3];
						setvNormal.y = (*getvNormal)[1+ vertexIndex*3];
						setvNormal.z = (*getvNormal)[2+ vertexIndex*3];
					}else {
						const COLLADAFW::DoubleArray *getvNormal = vNormals.getDoubleValues();
						setvNormal.x = (float)(*getvNormal)[0+ vertexIndex*3];
						setvNormal.y = (float)(*getvNormal)[1+ vertexIndex*3];
						setvNormal.z = (float)(*getvNormal)[2+ vertexIndex*3];
					}
					
					setdaeDocSubMesh.vNormals.push_back( setvNormal ) ;
				}

				if( !vColors.empty()  ) {
					// color
					rgba8888 setvColor;
					
					unsigned int vertexIndex = vColorIdx->getIndex(indexCnt);
					if( vColors.getType() == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
						const COLLADAFW::FloatArray *getvColor = vColors.getFloatValues();
						setvColor.r  = (unsigned int)((*getvColor)[0+ vertexIndex*4]*255);
						setvColor.g  = (unsigned int)((*getvColor)[1+ vertexIndex*4]*255);
						setvColor.b  = (unsigned int)((*getvColor)[2+ vertexIndex*4]*255);
						setvColor.a  = (unsigned int)((*getvColor)[3+ vertexIndex*4]*255);
					}else {
						const COLLADAFW::DoubleArray *getvColor = vColors.getDoubleValues();
						setvColor.r  = (unsigned int)((*getvColor)[0+ vertexIndex*4]*255);
						setvColor.g  = (unsigned int)((*getvColor)[1+ vertexIndex*4]*255);
						setvColor.b  = (unsigned int)((*getvColor)[2+ vertexIndex*4]*255);
						setvColor.a  = (unsigned int)((*getvColor)[3+ vertexIndex*4]*255);
					}
					
					setdaeDocSubMesh.vColors.push_back( setvColor );
				}

				if( !vUVs.empty() ) {
					
					// UV texcoord
					const COLLADAFW::IndexListArray& uvSetIndicesArray = primitiveElement->getUVCoordIndicesArray ();
					size_t numUVSets = uvSetIndicesArray.getCount ();
					size_t uvSetsCnt;
					setdaeDocSubMesh.uvSetsNum = numUVSets;
									

					for( uvSetsCnt = 0 ; uvSetsCnt < numUVSets ; uvSetsCnt++ )
					{
						vec4 setvUV;

						const COLLADAFW::IndexList* vUVIdx = primitiveElement->getUVCoordIndices(uvSetsCnt);
						
						unsigned int vertexIndex = vUVIdx->getIndex(indexCnt);
						if( vUVs.getType() == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
							const COLLADAFW::FloatArray *getvUV = vUVs.getFloatValues();
							setvUV.x = (*getvUV)[0+ vertexIndex*2];
							setvUV.y = 1.0f - (*getvUV)[1+ vertexIndex*2];
						}else {
							const COLLADAFW::DoubleArray *getvUV = vUVs.getDoubleValues();
							setvUV.x = (float)(*getvUV)[0+ vertexIndex*2];
							setvUV.y = (float)(1.0 - (*getvUV)[1+ vertexIndex*2]);
						}
															
						setdaeDocSubMesh.vUVSet.push_back(setvUV);	
						
					}
					
				}

			}
			 
			// IndexBuffer
			const COLLADAFW::MeshPrimitive::PrimitiveType primitiveType = primitiveElement->getPrimitiveType ();
			int    idxCnt;
			int    numFaceVetex;
			size_t numFace;

			switch ( primitiveType )
			{
				case COLLADAFW::MeshPrimitive::TRIANGLE_FANS:
					setdaeDocSubMesh.mdxPrimType = MDX_PRIM_TRIANGLE_FAN ;
					for( idxCnt = 0; idxCnt < (int)vIdxSize ; idxCnt++ )
					{
						setdaeDocSubMesh.IndexBuffer.push_back( idxCnt );
					}
					break;
				case COLLADAFW::MeshPrimitive::TRIANGLE_STRIPS:
					setdaeDocSubMesh.mdxPrimType = MDX_PRIM_TRIANGLE_STRIP ;
					for( int idxCnt = 0; idxCnt < (int)vIdxSize ; idxCnt++ )
					{
						setdaeDocSubMesh.IndexBuffer.push_back( idxCnt );
					}
					break;
				case COLLADAFW::MeshPrimitive::POLYGONS:
				case COLLADAFW::MeshPrimitive::POLYLIST:
					setdaeDocSubMesh.mdxPrimType = MDX_PRIM_TRIANGLES ;
					numFace = primitiveElement->getGroupedVertexElementsCount();
					
					for( size_t faceIdx = 0, idxCnt= 0; faceIdx < numFace ; faceIdx++)
					{
						numFaceVetex = primitiveElement->getGroupedVerticesVertexCount( faceIdx ) ;	
						
						if( numFaceVetex >= 5 ){
							// not support
							
						}else if( numFaceVetex == 4 ){
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt   );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+1 );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+3 );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+1 );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+2 );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+3 );
							
						} else {	
							// triangles
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+1 );
							setdaeDocSubMesh.IndexBuffer.push_back( idxCnt+2 );
						}
						
						idxCnt+=numFaceVetex;
					}
					break;
				case COLLADAFW::MeshPrimitive::TRIANGLES:
					setdaeDocSubMesh.mdxPrimType = MDX_PRIM_TRIANGLES;
					for( int idxCnt = 0; idxCnt < (int)vIdxSize ; idxCnt++ )
					{
						setdaeDocSubMesh.IndexBuffer.push_back( idxCnt );
					}
					break;
				default:

					break;
			}

			setdaeDocSubMesh.meshMaterialID = primitiveElement->getMaterialId();
			make_MeshTangentBinormal( setdaeDocSubMesh );
			inputMesh.Mesh.push_back(setdaeDocSubMesh) ;
			
		}		
		m_GeometryMeshList.push_back(inputMesh);
    }
	return true;
}



void DocumentImporter::make_MeshTangentBinormal( daeDocMeshInfo& setMesh )
{
#ifndef OLD_MDXCONV_SUPPORT
	if( setMesh.mdxPrimType == MDX_PRIM_TRIANGLES ){
		
		if( setMesh.vPositions.size() != 0 && setMesh.vNormals.size() != 0 && setMesh.uvSetsNum != 0 ){
			for( size_t wkIdx = 0; wkIdx < setMesh.vPositions.size() ; wkIdx++ )
			{
				setMesh.vTangents.push_back ( vec4 (0.0f) );
				setMesh.vBinormals.push_back ( vec4 (0.0f) );
			}
			for( size_t indexBFidx = 0; indexBFidx < setMesh.IndexBuffer.size() ; indexBFidx+=3 )
			{
				vec4 vPos[3];
				vec4 vUv[3];
				vec4 vtan[3];
				vec4 vbinorm[3];

				vPos[0] = setMesh.vPositions[setMesh.IndexBuffer[indexBFidx  ]];
				vPos[1] = setMesh.vPositions[setMesh.IndexBuffer[indexBFidx+1]];
				vPos[2] = setMesh.vPositions[setMesh.IndexBuffer[indexBFidx+2]];
				vUv[0] = setMesh.vUVSet[setMesh.IndexBuffer[indexBFidx]   * setMesh.uvSetsNum ];
				vUv[1] = setMesh.vUVSet[setMesh.IndexBuffer[indexBFidx+1] * setMesh.uvSetsNum ];
				vUv[2] = setMesh.vUVSet[setMesh.IndexBuffer[indexBFidx+2] * setMesh.uvSetsNum ];


				make_VertexTangentBinormal( vPos, vUv, vtan, vbinorm );
				
				setMesh.vTangents[setMesh.IndexBuffer[indexBFidx  ]] = vtan[0];
				setMesh.vTangents[setMesh.IndexBuffer[indexBFidx+1 ]] = vtan[1]; 
				setMesh.vTangents[setMesh.IndexBuffer[indexBFidx+2 ]] = vtan[2];
				setMesh.vBinormals[setMesh.IndexBuffer[indexBFidx  ]] = vtan[0];
				setMesh.vBinormals[setMesh.IndexBuffer[indexBFidx+1 ]] = vtan[1];
				setMesh.vBinormals[setMesh.IndexBuffer[indexBFidx+2 ]] = vtan[2];
				
			}
		}
	}
#endif
}

void DocumentImporter::make_VertexTangentBinormal( const vec4 vPos[3], const vec4 vUv[3], vec4 vtan[3], vec4 vbinorm[3] )
{

	vec4  edge01, edge02;
    vec4  cp;


	vtan[0] = vec4( 0.0f );
    vtan[1] = vec4( 0.0f );
    vtan[2] = vec4( 0.0f );
    vbinorm[0] = vec4( 0.0f);
    vbinorm[1] = vec4( 0.0f);
    vbinorm[2] = vec4( 0.0f);

	/** x, tangent, binormal **/
	edge01 = vec4(  vPos[1].x - vPos[0].x, vUv[1].x - vUv[0].x, vUv[1].y - vUv[0].y );
    edge02 = vec4(  vPos[2].x - vPos[1].x, vUv[2].x - vUv[1].x,	vUv[2].y - vUv[1].y );
	cp = vec4( edge01.y * edge02.z - edge01.z * edge02.y, edge01.z * edge02.x - edge01.x * edge02.z, edge01.x * edge02.y - edge01.y * edge02.x );


	if ( fabs(cp.x) > 0.0 )
	{
		float dydx = cp.y / cp.x;
		float dzdx = cp.z / cp.x;
		vtan[0].x    = -dydx;
		vbinorm[0].x = -dzdx;
        vtan[1].x    = -dydx;
		vbinorm[1].x = -dzdx;
		vtan[2].x    = -dydx;
		vbinorm[2].x = -dzdx;
    }
	
	/** y, tangent, binormal **/
	edge01 = vec4( vPos[1].y -  vPos[0].y, vUv[1].x - vUv[0].x, vUv[1].y - vUv[0].y );
    edge02 = vec4( vPos[2].y -  vPos[1].y, vUv[2].x - vUv[1].x, vUv[2].y - vUv[1].y );
	cp = vec4( edge01.y * edge02.z - edge01.z * edge02.y, edge01.z * edge02.x - edge01.x * edge02.z, edge01.x * edge02.y - edge01.y * edge02.x );
	
	if ( fabs(cp.x) > 0.0 )
	{
		float	dydx = cp.y / cp.x;
		float	dzdx = cp.z / cp.x;
		vtan[0].y = -dydx;
		vbinorm[0].y = -dzdx;
        vtan[1].y = -dydx;
		vbinorm[1].y = -dzdx;
		vtan[2].y = -dydx;
		vbinorm[2].y = -dzdx;
    }

	/** z, u, v **/
	edge01 = vec4(  vPos[1].z -  vPos[0].z, vUv[1].x - vUv[0].x, vUv[1].y - vUv[0].y );
    edge02 = vec4(  vPos[2].z -  vPos[1].z, vUv[2].x - vUv[1].x, vUv[2].y - vUv[1].y );
	cp = vec4( edge01.y * edge02.z - edge01.z * edge02.y, edge01.z * edge02.x - edge01.x * edge02.z, edge01.x * edge02.y - edge01.y * edge02.x );

    if ( fabs(cp.x) > 0.0 )
	{
		float	dydx = cp.y / cp.x;
		float	dzdx = cp.z / cp.x;
		vtan[0].z    = -dydx;
		vbinorm[0].z = -dzdx;
        vtan[1].z    = -dydx;
		vbinorm[1].z = -dzdx;
		vtan[2].z    = -dydx;
		vbinorm[2].z = -dzdx;
    }

	vtan[0] = normalize4( vtan[0] );
	vtan[1] = normalize4( vtan[1] );
	vtan[2] = normalize4( vtan[2] );
	vbinorm[0] = normalize4( vbinorm[0] );
	vbinorm[1] = normalize4( vbinorm[1] );
	vbinorm[2] = normalize4( vbinorm[2] );

}
bool DocumentImporter::writeMaterial( const COLLADAFW::Material* material )
{
	
	if ( m_ParseStep <= COPY_ELEMENTS )
    {
        // Make a copy of the material element and push it into the list.
        m_ParseStep = COPY_ELEMENTS;
        m_MaterialsList.push_back ( new COLLADAFW::Material ( *material ) );
    }
	return true;
}

bool DocumentImporter::writeEffect( const COLLADAFW::Effect* effect )
{
    m_EffectsList.push_back ( new COLLADAFW::Effect ( *effect ) );
	return true;
}

bool DocumentImporter::writeImage( const COLLADAFW::Image* image )
{
	
	MdxTexture* inputTexture = new MdxTexture;
	COLLADAFW::UniqueId textureUniqueId = image->getUniqueId();
	COLLADAFW::String getFileName = image->getImageURI().getPathFile();

	inputTexture->SetName ( image->getName().c_str() );
	inputTexture->AttachChild( new MdxFileName( getFileName.c_str() ) );
	m_mdxModel->AttachChild( inputTexture ) ;
	
	MdxTextureMapInfo *inputTextureConnectInfo = new MdxTextureMapInfo;
	
	inputTextureConnectInfo->imageUniqueId = textureUniqueId;
	inputTextureConnectInfo->name = image->getName();
	inputTextureConnectInfo->mdxTextureData = inputTexture;

	m_MdxTextureMap.push_back(inputTextureConnectInfo); 

	return true;
}
DocumentImporter::daeDocAnimCurveInfo* DocumentImporter::findAnimCurveInfo( const COLLADAFW::UniqueId& animationId )
{
	std::vector<daeDocAnimCurveInfo*>::iterator		ite;

	for ( ite=m_daeDocAnimCurveList.begin(); ite!=m_daeDocAnimCurveList.end(); ite++ )
	{
		if ( (*ite)->animationId == animationId )
		{
			return (*ite);
		}
	}

	return NULL;
}

bool DocumentImporter::writeAnimation( const COLLADAFW::Animation* animation )
{
	daeDocAnimCurveInfo*			pAnimCurveInfo;

	pAnimCurveInfo = findAnimCurveInfo( animation->getUniqueId() );
	if ( !pAnimCurveInfo )
	{
		pAnimCurveInfo = new daeDocAnimCurveInfo;
		pAnimCurveInfo->animationId   = animation->getUniqueId();
		pAnimCurveInfo->animationName = animation->getName();
		pAnimCurveInfo->animationCurve = new MdxFCurve();

		if ( animation->getAnimationType() == COLLADAFW::Animation::ANIMATION_CURVE )
		{
			COLLADAFW::AnimationCurve*	animationCurve = ( COLLADAFW::AnimationCurve* )animation;

			if ( !writeAnimationCurve( animationCurve, pAnimCurveInfo ) )
			{
				delete pAnimCurveInfo;
				return false;
			}
		} else
		if ( animation->getAnimationType() == COLLADAFW::Animation::ANIMATION_FORMULA )
		{
			/* not supported yet */
			delete pAnimCurveInfo;
			return false;
		}

		m_daeDocAnimCurveList.push_back( pAnimCurveInfo );
	}



	return true;
}

bool DocumentImporter::writeAnimationCurve( COLLADAFW::AnimationCurve* animationCurve, daeDocAnimCurveInfo* outAnimCurveInfo )
{
	const COLLADAFW::PhysicalDimension&		physicalDimension = animationCurve->getInPhysicalDimension();

	if ( physicalDimension != COLLADAFW::PHYSICAL_DIMENSION_TIME )
	{
		/* not supported */
		return false;
	}

	const COLLADAFW::AnimationCurve::InterpolationType& interpolationType = animationCurve->getInterpolationType();

    switch ( interpolationType )
    {
		case COLLADAFW::AnimationCurve::INTERPOLATION_BEZIER:
		{
			if ( !setAnimCurveInfo( animationCurve, outAnimCurveInfo, MDX_FCURVE_HERMITE ) )
			{
				return false;
			}
			break;
		}
		case COLLADAFW::AnimationCurve::INTERPOLATION_LINEAR:
		{
			if ( !setAnimCurveInfo( animationCurve, outAnimCurveInfo, MDX_FCURVE_LINEAR ) )
			{
				return false;
			}
			break;
		}
		case COLLADAFW::AnimationCurve::INTERPOLATION_STEP:
		{
			if ( !setAnimCurveInfo( animationCurve, outAnimCurveInfo, MDX_FCURVE_CONSTANT ) )
			{
				return false;
			}
			break;
		}
		case COLLADAFW::AnimationCurve::INTERPOLATION_MIXED:
		case COLLADAFW::AnimationCurve::INTERPOLATION_UNKNOWN:
		case COLLADAFW::AnimationCurve::INTERPOLATION_BSPLINE:
		case COLLADAFW::AnimationCurve::INTERPOLATION_CARDINAL:
		case COLLADAFW::AnimationCurve::INTERPOLATION_HERMITE:
		default:
		{
			/* not supported */
			return false;
		}
    }

	return true;
}


bool DocumentImporter::setAnimCurveInfo( COLLADAFW::AnimationCurve* animationCurve, daeDocAnimCurveInfo* outAnimCurveInfo, int iFormat )
{
	const COLLADAFW::FloatOrDoubleArray&		inputValuesArray = animationCurve->getInputValues();
	size_t										numInputValues = inputValuesArray.getValuesCount();
	const COLLADAFW::FloatOrDoubleArray&		outputValuesArray = animationCurve->getOutputValues();
	size_t										outDimension = animationCurve->getOutDimension();
	size_t										numOutputValues = outputValuesArray.getValuesCount() / outDimension;
    const COLLADAFW::PhysicalDimensionArray&	outPhysicalDimensions = animationCurve->getOutPhysicalDimensions();
    size_t										outDimension2 = outPhysicalDimensions.getCount();
	mdx::MdxFCurve*								pMdxFCurve = outAnimCurveInfo->animationCurve;
	

    if ( numInputValues != numOutputValues || outDimension != outDimension2 )
    {
		/* invalid animation */
		return false;
    }

	pMdxFCurve->SetFormat( iFormat );
	pMdxFCurve->SetDimCount( outDimension );

	for ( size_t inputIndex=0; inputIndex<animationCurve->getKeyCount(); inputIndex++ )
	{
		double			inputValue = getDoubleValue ( inputValuesArray, inputIndex ) * m_daeDocMotionInfo.frameRate;
		MdxKeyFrame		mdxKey;

		mdxKey.SetDimCount( outDimension );
		mdxKey.SetFormat( iFormat );
		mdxKey.SetFrame( (float)inputValue );

		if ( inputValue < m_daeDocMotionInfo.minTime )
		{
			m_daeDocMotionInfo.minTime = inputValue;
		}
		if ( inputValue > m_daeDocMotionInfo.maxTime )
		{
			m_daeDocMotionInfo.maxTime = inputValue;
		}

		for ( size_t outputIndex=0; outputIndex<outDimension; outputIndex++ )
		{
			switch ( outPhysicalDimensions[ outputIndex ] )
			{
				case COLLADAFW::PHYSICAL_DIMENSION_LENGTH:
				case COLLADAFW::PHYSICAL_DIMENSION_ANGLE:
				case COLLADAFW::PHYSICAL_DIMENSION_COLOR:
				case COLLADAFW::PHYSICAL_DIMENSION_NUMBER:
				{
					size_t				currentOutputIndex = inputIndex*outDimension + outputIndex;
					double				outputValue = getDoubleValue ( outputValuesArray, currentOutputIndex );


					if ( outPhysicalDimensions[ outputIndex ] == COLLADAFW::PHYSICAL_DIMENSION_ANGLE )
					{
						outputValue = DegToRad( outputValue );
					}

					mdxKey.SetValue( outputIndex, (float)outputValue );

					if ( iFormat == MDX_FCURVE_HERMITE )
					{
						const COLLADAFW::FloatOrDoubleArray& inTangentValuesArray = animationCurve->getInTangentValues();
						const COLLADAFW::FloatOrDoubleArray& outTangentValuesArray = animationCurve->getOutTangentValues();
						size_t indexX = inputIndex*(outDimension*2) + (outputIndex*2);
						double inTangentValueX = getDoubleValue ( inTangentValuesArray, indexX ) * m_daeDocMotionInfo.frameRate;
						double inTangentValueY = getDoubleValue ( inTangentValuesArray, indexX+1 );
						double outTangentValueX = getDoubleValue ( outTangentValuesArray, indexX ) * m_daeDocMotionInfo.frameRate;
						double outTangentValueY = getDoubleValue ( outTangentValuesArray, indexX+1 );

						if ( outPhysicalDimensions[ outputIndex ] == COLLADAFW::PHYSICAL_DIMENSION_ANGLE )
						{
							inTangentValueY  = DegToRad( inTangentValueY );
							outTangentValueY = DegToRad( outTangentValueY );
						}

						double resultInY = ( inTangentValueY - outputValue ) ;
						double resultOutY = ( outTangentValueY - outputValue ) ;
						mdxKey.SetInDY( outputIndex, (float)resultInY ) ;
						mdxKey.SetOutDY( outputIndex, (float)resultOutY ) ;
					}
					break;
				}

				case COLLADAFW::PHYSICAL_DIMENSION_TIME:
				default:
				{
					/* not supported */
					return false;
				}
			}
		}

		pMdxFCurve->InsertKeyFrame( inputIndex, mdxKey );
	}

	return true;
}

bool DocumentImporter::mergeMorphWeightMdxFCurves( MdxFCurve* outFCurve, std::vector<MdxFCurve*> inFCurveList )
{
	float		fCurTime;
	int 		iStartTime, iEndTime, iTime;

	iStartTime = (int)floor( m_daeDocMotionInfo.minTime );
	iEndTime   = (int)ceil( m_daeDocMotionInfo.maxTime );

	for ( iTime=iStartTime; iTime<=iEndTime; iTime++ )
	{
		MdxKeyFrame		newKey;
		float			srcMeshWeight = 1.0f;

		fCurTime = (float)iTime;
		if ( fCurTime < m_daeDocMotionInfo.minTime )	fCurTime = (float)m_daeDocMotionInfo.minTime;
		if ( fCurTime > m_daeDocMotionInfo.maxTime )	fCurTime = (float)m_daeDocMotionInfo.maxTime;

		
		for ( size_t inC=0; inC < inFCurveList.size(); inC++ )
		{
			MdxFCurve*		inFCurve;
			MdxKeyFrame		workKey;

			newKey.SetFrame( fCurTime );
			newKey.SetDimCount( inFCurveList.size()+1 );
			newKey.SetFormat( MDX_FCURVE_LINEAR );
					

			inFCurve = inFCurveList[inC];
			if ( inFCurve != NULL )
			{
				inFCurve->Eval( fCurTime, workKey );
				
				float weightVal = workKey.GetValue( 0 );

				srcMeshWeight = srcMeshWeight - weightVal;
				newKey.SetValue( inC + 1 , weightVal );
				  		
			}
			
		}

		newKey.SetValue( 0 , srcMeshWeight );
		outFCurve->InsertKeyFrame( outFCurve->GetKeyFrameCount(), newKey );
	}

	outFCurve->SetFormat( MDX_FCURVE_LINEAR );
	outFCurve->SetDimCount( inFCurveList.size()+1 );

	return true;
}
bool DocumentImporter::mergeMdxFCurves( MdxFCurve* outFCurve, int outFCurveDim, std::vector<MdxFCurve*> inFCurveList, std::vector<TransformVectorType> mergeOrder )
{
	std::vector<float>		keyTimeList;
	int						iOutFCurveFormat = MDX_FCURVE_CONSTANT;

	if ( inFCurveList.size() <= 0 )
	{
		return false;
	}

	/* 入力の全カーブに存在するキーフレームの全時刻を収集 */
	for ( size_t inC=0; inC<inFCurveList.size(); inC++ )
	{
		MdxFCurve*		inFCurve;
		MdxKeyFrame		inKeyFrame;
		int				iNumKeys;


		inFCurve = inFCurveList[inC];
		if ( inFCurve == NULL )
		{
			continue;
		}

		if ( inFCurve->GetFormat() == MDX_FCURVE_HERMITE )
		{
			iOutFCurveFormat = MDX_FCURVE_HERMITE;
		} else
		if ( inFCurve->GetFormat() == MDX_FCURVE_LINEAR )
		{
			if ( iOutFCurveFormat == MDX_FCURVE_CONSTANT )
			{
				iOutFCurveFormat = MDX_FCURVE_LINEAR;
			}
		}

		iNumKeys = inFCurve->GetKeyFrameCount();
		for ( int i=0; i<iNumKeys; i++ )
		{
			inKeyFrame = inFCurve->GetKeyFrame( i );

			keyTimeList.push_back( inKeyFrame.GetFrame() );
		}
	}

	std::sort( keyTimeList.begin(), keyTimeList.end() );
	std::vector<float>::iterator	new_end = std::unique( keyTimeList.begin(), keyTimeList.end() );
	keyTimeList.erase( new_end, keyTimeList.end() );

	for ( size_t i=0; i<keyTimeList.size(); i++ )
	{
		float			fTime = keyTimeList[i];
		MdxKeyFrame		newKey;
		MdxKeyFrame		workKey;


		newKey.SetFrame( fTime );

		newKey.SetDimCount( outFCurveDim );
		newKey.SetFormat( iOutFCurveFormat );

		for ( size_t inC=0; inC<inFCurveList.size(); inC++ )
		{
			MdxFCurve*		inFCurve;
			int				iDim;

			inFCurve = inFCurveList[inC];
			
			switch ( mergeOrder.at(inC) )
			{
				case TRANSVECTYPE_X:
				case TRANSVECTYPE_R:
				{
					iDim=0;
					break;
				}
				case TRANSVECTYPE_Y:
				case TRANSVECTYPE_G:
				{
					iDim=1;
					break;
				}
				case TRANSVECTYPE_Z:
				case TRANSVECTYPE_B:
				{
					iDim=2;
					break;
				}
				case TRANSVECTYPE_W:
				case TRANSVECTYPE_A:
				{
					iDim=3;
					break;
				}
				default:
				{
					continue;
					break;
				}
			}

			if ( inFCurve )
			{
				inFCurve->Eval( fTime, workKey, true );
				newKey.SetValue( iDim, workKey.GetValue( 0 ) );
				if ( iOutFCurveFormat == MDX_FCURVE_HERMITE )
				{
					newKey.SetInDY( iDim, workKey.GetInDY( 0 ) );
					newKey.SetOutDY( iDim, workKey.GetOutDY( 0 ) );
				}
			}
		}

		outFCurve->InsertKeyFrame( i, newKey );
	}

	outFCurve->SetFormat( iOutFCurveFormat );
	outFCurve->SetDimCount( outFCurveDim );


	return true;
}

bool DocumentImporter::mergeMdxFCurveQuat( MdxFCurve* outFCurve, std::vector<MdxFCurve*> rotationFCurveList, std::vector<vec4> staticRotationAxisAngleList, std::vector<TransformVectorType> rotateOrder )
{
	float		fCurTime;
	int 		iStartTime, iEndTime, iTime;

	iStartTime = (int)floor( m_daeDocMotionInfo.minTime );
	iEndTime   = (int)ceil( m_daeDocMotionInfo.maxTime );

	for ( iTime=iStartTime; iTime<=iEndTime; iTime++ )
	{
		mdx::quat		keyQuat;
		MdxKeyFrame		newKey;

		fCurTime = (float)iTime;
		if ( fCurTime < m_daeDocMotionInfo.minTime )	fCurTime = (float)m_daeDocMotionInfo.minTime;
		if ( fCurTime > m_daeDocMotionInfo.maxTime )	fCurTime = (float)m_daeDocMotionInfo.maxTime;

		keyQuat = quat();
		for ( size_t inC=0; inC<rotationFCurveList.size(); inC++ )
		{
			MdxFCurve*		inFCurve;
			MdxKeyFrame		workKey;
			mdx::quat		workQuat;
			float			fAngle;

			inFCurve = rotationFCurveList[inC];
			if ( inFCurve != NULL )
			{
				inFCurve->Eval( fCurTime, workKey );

				fAngle = workKey.GetValue( 0 );
				switch ( rotateOrder.at(inC) )
				{
					case TRANSVECTYPE_X:
					{
						workQuat = quat_from_rotate( 1.0f, 0.0f, 0.0f, fAngle );
						break;
					}
					case TRANSVECTYPE_Y:
					{
						workQuat = quat_from_rotate( 0.0f, 1.0f, 0.0f, fAngle );
						break;
					}
					case TRANSVECTYPE_Z:
					{
						workQuat = quat_from_rotate( 0.0f, 0.0f, 1.0f, fAngle );
						break;
					}
					default:
					{
						workQuat = quat_from_rotate( 0.0f, 0.0f, 1.0f, 0.0f );
						break;
					}
				}
			} else
			{
				vec4*		staticAxisAngleVec = &staticRotationAxisAngleList[inC];

				workQuat = quat_from_rotate( staticAxisAngleVec->x, staticAxisAngleVec->y, staticAxisAngleVec->z, DegToRad( staticAxisAngleVec->w ) );
			}

			keyQuat = keyQuat * workQuat;
		}

		newKey.SetFrame( fCurTime );
		newKey.SetDimCount( 4 );
		newKey.SetFormat( MDX_FCURVE_SPHERICAL );
		newKey.SetValueQuat( 0, keyQuat );

		outFCurve->InsertKeyFrame( outFCurve->GetKeyFrameCount(), newKey );
	}

	outFCurve->SetFormat( MDX_FCURVE_SPHERICAL );
	outFCurve->SetDimCount( 4 );

	return true;
}


bool DocumentImporter::writeAnimationList( const COLLADAFW::AnimationList* animationList )
{
	m_AnimationListsList.push_back( new COLLADAFW::AnimationList ( *animationList ) );

	return true;
}

COLLADAFW::AnimationList* DocumentImporter::findAnimationList( const COLLADAFW::UniqueId& animationListId )
{
	std::vector<COLLADAFW::AnimationList*>::iterator		ite;

	for ( ite=m_AnimationListsList.begin(); ite!=m_AnimationListsList.end(); ite++ )
	{
		if ( (*ite)->getUniqueId() == animationListId )
		{
			return (*ite);
		}
	}

	return NULL;
}

bool DocumentImporter::writeSkinControllerData( const COLLADAFW::SkinControllerData* skinControllerData )
{
	
	daeDocSkinCtrlDataInfo	daeDocSkindata;
	
	daeDocSkindata.skinCtrDatalUniqId = skinControllerData->getUniqueId();
	daeDocSkindata.bindPoseMatrix = skinControllerData->getBindShapeMatrix();
	daeDocSkindata.jointNum = skinControllerData->getJointsCount();

	const COLLADAFW::Matrix4Array& jointsInversbindPoseList =  skinControllerData->getInverseBindMatrices() ;
	
	const COLLADAFW::IntValuesArray& jointIndicesList =  skinControllerData->getJointIndices();
	const COLLADAFW::UIntValuesArray& jointsPerVertexList = skinControllerData->getJointsPerVertex();
	
	// weight
	const COLLADAFW::UIntValuesArray& weightindecesList = skinControllerData->getWeightIndices();
	const COLLADAFW::FloatOrDoubleArray& wkWeightList = skinControllerData->getWeights();
	const COLLADAFW::FloatArray	 *fWeightList;
	const COLLADAFW::DoubleArray *dblWeightList;


	COLLADAFW::FloatOrDoubleArray::DataType weightDataType = wkWeightList.getType();

	if ( weightDataType == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
		fWeightList = wkWeightList.getFloatValues();
	}else {
		dblWeightList = wkWeightList.getDoubleValues();
	}
	
	size_t  vertexNum =  skinControllerData->getVertexCount();
	


	for( size_t jointIdx = 0; jointIdx < daeDocSkindata.jointNum ; jointIdx++ )
	{
		daeDocJointDataInfo	inputJointdata;
		size_t jointVertex = 0;
		inputJointdata.InverseBindMatrix =  jointsInversbindPoseList[jointIdx];

		int   wkjointsPerVertex = 0;
		
		for( size_t vertexIdx = 0; vertexIdx < vertexNum ; vertexIdx++ )
		{

			float setWeight;

			int		iNumjoints = jointsPerVertexList[ vertexIdx ];
			setWeight = 0.0f;
			for ( int i=0; i<iNumjoints; i++ )
			{
				int			weightJointIdx = jointIndicesList[ wkjointsPerVertex + i ];

				if ( weightJointIdx == jointIdx )
				{
					if( weightDataType == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
						setWeight = (*fWeightList)[weightindecesList[wkjointsPerVertex + i]];
					}else {
						setWeight = (float)(*dblWeightList)[weightindecesList[wkjointsPerVertex + i]];
					}
					break;
				}
			}
				
			inputJointdata.vertexWeight.push_back( setWeight );
			wkjointsPerVertex += jointsPerVertexList[vertexIdx];
			
		}
		
		daeDocSkindata.jointData.push_back( inputJointdata ); 
	}

	m_daeDocSkinCtrlDataList.push_back(daeDocSkindata); 
	
	return true;
}

bool DocumentImporter::writeController( const COLLADAFW::Controller* controller )
{
	COLLADAFW::Controller::ControllerType controllerType = controller->getControllerType ();
    switch ( controllerType )
    {
		case COLLADAFW::Controller::CONTROLLER_TYPE_SKIN:
			{
				COLLADAFW::SkinController* skinController = (COLLADAFW::SkinController*)controller;
				daeDocSkinCtrlMapInfo	inputSkinCtrlMap;

				inputSkinCtrlMap.SkinCtrlUniqId = controller->getUniqueId(); 
				inputSkinCtrlMap.skinCtrDataUniqId = skinController->getSkinControllerData();
				inputSkinCtrlMap.meshORcontrollerUniq = skinController->getSource(); 

				size_t jointNum	=  skinController->getJoints().getCount();
				const COLLADAFW::UniqueIdArray& jointsIds =  skinController->getJoints();
				for( size_t jointIdx =0 ; jointIdx < jointNum ; jointIdx++ )
				{
					inputSkinCtrlMap.jointUniqIdList.push_back( jointsIds[jointIdx] );
				}
				
				m_daeDocSkinCtrlMapList.push_back( inputSkinCtrlMap );
				
				

			}
			break;
		case COLLADAFW::Controller::CONTROLLER_TYPE_MORPH:
			{
				COLLADAFW::MorphController* morphController = (COLLADAFW::MorphController*)controller;
				daeDocMorphCtrlMapInfo	inputMorphCtrlMap;
				
				inputMorphCtrlMap.MorphCtrlUniqId = controller->getUniqueId(); 
				inputMorphCtrlMap.meshORcontrollerUniq = morphController->getSource();
				
				
				COLLADAFW::UniqueIdArray& morphTargets= morphController->getMorphTargets(); 
				COLLADAFW::FloatOrDoubleArray& morphWeights = morphController->getMorphWeights();
				const COLLADAFW::FloatArray	 *fMorphWeightList;
				const COLLADAFW::DoubleArray *dblMorphWeightList;
				
				inputMorphCtrlMap.morphWeightAnimationListId = morphWeights.getAnimationList(); 
				

				COLLADAFW::FloatOrDoubleArray::DataType morphWeightDataType = morphWeights.getType();
				 
				if ( morphWeightDataType == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
					fMorphWeightList = morphWeights.getFloatValues();
				}else {
					dblMorphWeightList = morphWeights.getDoubleValues();
				}


				for( size_t targetsCnt = 0 ; targetsCnt < morphTargets.getCount(); targetsCnt++ ){
					morphTargetInfo inputMorphTarget;
					float setMorphWeight = 0.0f;

					if( morphWeightDataType == COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT ){
						setMorphWeight = (*fMorphWeightList)[targetsCnt];
					}else {
						setMorphWeight = (float)(*dblMorphWeightList)[targetsCnt];
					}
					inputMorphTarget.initialMorphWeight = setMorphWeight;
					inputMorphTarget.geomUniqId = morphTargets[targetsCnt];
					inputMorphCtrlMap.morphTarget.push_back( inputMorphTarget ) ;
				}

				
				m_daeDocMorphCtrlMapList.push_back(inputMorphCtrlMap);
			}

			
			break;
	}
	
	return true;
}

bool DocumentImporter::writeCamera( const COLLADAFW::Camera* camera )
{
	// not used MDX/MDS
	return true;
}
bool DocumentImporter::writeLight( const COLLADAFW::Light* light )
{
	// not used MDX/MDS
	return true;
}


bool DocumentImporter::writeFormulas( const COLLADAFW::Formulas* formulas )
{

	return true;
}		
bool DocumentImporter::writeKinematicsScene( const COLLADAFW::KinematicsScene* kinematicsScene )
{
	return true;
}


const COLLADAFW::Node* DocumentImporter::findNode ( const COLLADAFW::UniqueId& nodeId, const COLLADAFW::NodePointerArray& nodes )
{
	size_t numNodes = nodes.getCount ();
	for ( size_t i=0; i<numNodes; ++i )
	{
		const COLLADAFW::Node* node = nodes [i];
		if ( nodeId != node->getUniqueId () ) 
		{
			// Recursive call
			const COLLADAFW::NodePointerArray& childNodes = node->getChildNodes ();
			const COLLADAFW::Node* searchedNode = findNode ( nodeId, childNodes );
			if ( searchedNode ) return searchedNode;
		}
		else return node;
	}
	return 0;
}

double DocumentImporter::getDoubleValue ( const COLLADAFW::FloatOrDoubleArray &inputValuesArray, const size_t position )
{
    double inputValue = 0;

    size_t numInputValues = inputValuesArray.getValuesCount ();
    if ( position > numInputValues - 1 )
        std::cerr << "Out of range error!" << std::endl;

    const COLLADAFW::FloatOrDoubleArray::DataType& inputDataType = inputValuesArray.getType ();
    switch ( inputDataType )
    {
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_DOUBLE:
        {
            const COLLADAFW::DoubleArray* inputValues = inputValuesArray.getDoubleValues ();
            inputValue = (*inputValues) [position];
        }
        break;
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT:
        {
            const COLLADAFW::FloatArray* inputValues = inputValuesArray.getFloatValues ();
            inputValue = (double)(*inputValues) [position];
        }
        break;
    default:
        std::cerr << "AnimationImporter::setInTangents(): inputDataType unknown data type!" << std::endl;
    }

    return inputValue;
}

float DocumentImporter::getFloatValue ( const COLLADAFW::FloatOrDoubleArray &inputValuesArray, const size_t position )
{
    float inputValue = 0;

    size_t numInputValues = inputValuesArray.getValuesCount ();
    if ( position > numInputValues - 1 )
        std::cerr << "Out of range error!" << std::endl;

    const COLLADAFW::FloatOrDoubleArray::DataType& inputDataType = inputValuesArray.getType ();
    switch ( inputDataType )
    {
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_DOUBLE:
        {
            const COLLADAFW::DoubleArray* inputValues = inputValuesArray.getDoubleValues ();
            inputValue = (float)(*inputValues) [position];
        }
        break;
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT:
        {
            const COLLADAFW::FloatArray* inputValues = inputValuesArray.getFloatValues ();
            inputValue = (*inputValues) [position];
        }
        break;
    default:
        std::cerr << "AnimationImporter::setInTangents(): inputDataType unknown data type!" << std::endl;
    }

    return inputValue;
}


} // namespace DAEDOC