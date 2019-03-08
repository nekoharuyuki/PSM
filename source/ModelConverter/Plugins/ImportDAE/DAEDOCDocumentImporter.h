#include "ModelProc/ModelProc.h"
#include "COLLADAFWIWriter.h"
#include "COLLADAFWFileInfo.h"
#include "COLLADAFWInstanceVisualScene.h"
#include "COLLADAFWFormula.h"
#include "COLLADAFWEffect.h"
#include "COLLADAFWAnimationCurve.h"


#include "COLLADASaxFWLLoader.h"
#include "COLLADABUPrerequisites.h"
#include "COLLADAFWMesh.h"
#include "COLLADAFWNode.h"

#include "DAEDOCExtraDataCallbackHandler.h"
#include "DAEUtil.h"



#ifndef __DAEDOCUMENTIMPORTER_H__
#define __DAEDOCUMENTIMPORTER_H__



#define FLOAT_TOLERANCE 0.0001f
#define RADIANS_PER_DEGREE	(3.1415926536f/180.0f)

namespace DAEDOC
{


class DocumentImporter : public COLLADAFW::IWriter
{

private:
	std::string		m_filename ;	// Input FileName(.dae)
	MdxFile*		m_mdxFile;
	MdxModel*		m_mdxModel;
	MdxMotion*		m_mdxMotion;
	
	COLLADAFW::VisualScene* m_wkvisualScene;
	std::vector<std::pair<COLLADAFW::UniqueId, std::string>> m_RefNodeNameList;	// Material Map
	
	typedef std::pair<COLLADAFW::UniqueId, MdxMaterial*> MdxMaterialMapInfo;	// (materialUniqueId, wrietMdxMaterial )
	std::vector<MdxMaterialMapInfo*> m_MdxMaterialMap;	// Material Map


	typedef struct {
		COLLADAFW::UniqueId     imageUniqueId;
		std::string				name;
		MdxTexture*				mdxTextureData;

	} MdxTextureMapInfo;
	std::vector<MdxTextureMapInfo*> m_MdxTextureMap;	// Texture Map
	
	typedef struct {
		COLLADAFW::UniqueId		materialUniqueId;
		COLLADAFW::TextureMapId	materialTextureID;
		MdxLayer*				mdxLayerData;
	} MdxMaterialTextureMapInfo;

	std::vector<MdxMaterialTextureMapInfo*> m_MdxMaterialTextureMap;	// Material - Texture

	
	//###### MeshInfo #####
	typedef struct {
		COLLADAFW::MaterialId					meshMaterialID;
		int										mdxPrimType;
		std::vector<size_t>						IndexBuffer;
		std::vector<vec4>						vPositions;
		std::vector<vec4>						vNormals;
		std::vector<rgba8888>					vColors;
		size_t									uvSetsNum;
		std::vector<vec4>						vUVSet;      // ( UVSet1.uv1, UVSet2.uv1,UVSet1.uv2, UVSet2.uv2 ....)
		std::vector<vec4>						vTangents;
		std::vector<vec4>						vBinormals;
		std::vector<size_t>						vertexIdxConvMap;
		std::vector<bool>						jointUsingList;
		std::vector<std::vector<float>>			jointWeightList;
	} daeDocMeshInfo ;
	
	typedef struct {
		COLLADAFW::UniqueId				geometryUniqueId;
		std::string						geometryName;
		COLLADAFW::UniqueId				morphWeightAnimationListId;
		vector<daeDocMeshInfo>			Mesh;
		vector<size_t>					morphTaget_subMeshCount;
		vector<daeDocMeshInfo>			morphTagetMesh;
	} daeDocGeometryInfo ;
	
	std::vector<daeDocGeometryInfo> m_GeometryMeshList;


	typedef struct {
		COLLADAFW::UniqueId					SkinCtrlUniqId;
		COLLADAFW::UniqueId					skinCtrDataUniqId;
		COLLADAFW::UniqueId					meshORcontrollerUniq;
		std::vector<COLLADAFW::UniqueId>	jointUniqIdList;
	} daeDocSkinCtrlMapInfo;

	std::vector<daeDocSkinCtrlMapInfo> m_daeDocSkinCtrlMapList;

	typedef struct {
		COLLADAFW::UniqueId		geomUniqId;
		float					initialMorphWeight;
	} morphTargetInfo ;

	typedef struct {
		COLLADAFW::UniqueId					MorphCtrlUniqId;
		COLLADAFW::UniqueId					meshORcontrollerUniq;
		COLLADAFW::UniqueId					morphWeightAnimationListId;
		std::vector<morphTargetInfo>		morphTarget;
	} daeDocMorphCtrlMapInfo;

	std::vector<daeDocMorphCtrlMapInfo>  m_daeDocMorphCtrlMapList;

	typedef struct {
		COLLADABU::Math::Matrix4		InverseBindMatrix;
		std::vector<float>				vertexWeight;
	} daeDocJointDataInfo;
	typedef struct {
		COLLADAFW::UniqueId					skinCtrDatalUniqId;
		COLLADABU::Math::Matrix4			bindPoseMatrix;
		size_t								jointNum;							
		std::vector<daeDocJointDataInfo>	jointData;
	} daeDocSkinCtrlDataInfo;

	std::vector<daeDocSkinCtrlDataInfo> m_daeDocSkinCtrlDataList;
	
	typedef struct {
		double									minTime;
		double									maxTime;
		double									frameRate;
	} daeDocMotionInfo;
	typedef struct {
		COLLADAFW::UniqueId						animationId;
		std::string								animationName;
		MdxFCurve*								animationCurve;
	} daeDocAnimCurveInfo;

	daeDocMotionInfo					m_daeDocMotionInfo;
	std::vector<daeDocAnimCurveInfo*>	m_daeDocAnimCurveList;



/*	çÏã∆óp
	MdxFile
		MdxModel
			MdxBone ÅÀ MdxPart(MdxDrawPart)
	       
			MdxPart 
				MdxMesh  ÅÀ MdxArrays, MdxMaterial
				MdxArrays
			MdxMaterial
				MdxLayer ÅÀ MdxTexture
			MdxTexture
			MdxMotion  ÅÀ MdxFCurve, TergetAnime
				MdxFCurve
*/
	
	COLLADAFW::FileInfo::UpAxisType m_UpAxisType;
	double	m_DigitTolerance;

	
	enum ParseSteps
    {
        NO_PARSING = 0,
        FIRST_PARSING,
        IMPORT_ASSET,
        COPY_ELEMENTS, // no order: scene, visual scene, library nodes, materials, animationLists, writeController
        ELEMENTS_COPIED,
        VISUAL_SCENE_IMPORTED,
        SECOND_PARSING, 
        ANIMATIONS_IMPORTED,
        GEOMETRY_IMPORTED,
        MAKE_CONNECTIONS
    };

	enum TransformVectorType
	{
		TRANSVECTYPE_NONE,
		TRANSVECTYPE_X,
		TRANSVECTYPE_Y,
		TRANSVECTYPE_Z,
		TRANSVECTYPE_W,
		TRANSVECTYPE_XYZ,
		TRANSVECTYPE_R,
		TRANSVECTYPE_G,
		TRANSVECTYPE_B,
		TRANSVECTYPE_A,
		TRANSVECTYPE_RGB,
		TRANSVECTYPE_RGBA,
		TRANSVECTYPE_AXISANGLE,
		TRANSVECTYPE_QUAT,
	};

	ParseSteps m_ParseStep;

    /**
    * The list of all unique ids of maya nodes (dag nodes and depend nodes). 
    * A list of names which are either used up to multiple times for dag nodes in the scene 
    * graph or just once for any other maya depend object (materials, shading groups, material 
    * infos, animations, blend shapes, skin clusters, textures ). Used to avoid dublicate names. 
    */
    COLLADABU::IDList mGlobalNodeIdList;

    /**
    * The list of unique ids of maya depend nodes. Depend nodes are: materials, shading groups, 
    * material infos, animations, blend shapes, skin clusters, textures. Used to avoid dublicate 
    * names. 
    */
    COLLADABU::IDList mDependNodeIdList;

public:


	/** The callback handler to parse the extra data elements. */
    ExtraDataCallbackHandler m_ExtraDataCallbackHandler;


	COLLADAFW::InstanceVisualScene*			m_InstanceVisualScene;
	std::vector<COLLADAFW::VisualScene*>	m_VisualScenesList;
   
	std::vector<COLLADAFW::Material*>	m_MaterialsList;
	std::vector<COLLADAFW::Effect*>		m_EffectsList;
	std::vector<COLLADAFW::Image*>		m_ImageList;
	


	std::vector<COLLADAFW::LibraryNodes*>	m_LibraryNodesList;
	
	std::vector<COLLADAFW::Camera*>		m_CameraList;
	std::vector<COLLADAFW::Light*>		m_LightList;
	
	std::vector<COLLADAFW::AnimationList*>	m_AnimationListsList;
	std::vector<COLLADAFW::Controller*>	m_ControllerList;
	std::vector<COLLADAFW::Formulas*>	m_FormulasList;
	std::vector<COLLADAFW::KinematicsScene*>	m_KinematicsSceneList;

	
	// The map holds the skin controller objects for every source (mesh or skin controller).
	std::map<COLLADAFW::UniqueId, std::vector<COLLADAFW::SkinController*> > m_SkinControllersMap;
	// The map holds the morph controller objects for every source (mesh).
	std::map<COLLADAFW::UniqueId, std::vector<COLLADAFW::MorphController*> > m_MorphControllersMap;
	// The map holds the morph controller objects for every morph target object (mesh).
	std::map<COLLADAFW::UniqueId, std::vector<COLLADAFW::MorphController*> > m_MorphTargetsMap;


public:
	/** Constructor. */
	DocumentImporter( std::string filename, MdxFile* outputMDX );
	

	bool importScene();

public:
	
	

	virtual ~DocumentImporter();

	virtual void cancel(const COLLADAFW::String& errorMessage);

	/** Prepare to receive data.*/
	virtual void start();

	/** Remove all objects that don't have an object. Deletes unused visual scenes.*/
	virtual void finish();


	/** When this method is called, the writer must write the global document asset.
	@return The writer should return true, if writing succeeded, false otherwise.*/
	virtual bool writeGlobalAsset ( const COLLADAFW::FileInfo* asset );

	/** When this method is called, the writer must write the scene.
	@return The writer should return true, if writing succeeded, false otherwise.*/
	virtual bool writeScene ( const COLLADAFW::Scene* scene );

	/** Writes the entire visual scene.
	@return True on succeeded, false otherwise.*/
	virtual bool writeVisualScene ( const COLLADAFW::VisualScene* visualScene );

	/** Handles all nodes in the library nodes.
	@return True on succeeded, false otherwise.*/
	virtual bool writeLibraryNodes( const COLLADAFW::LibraryNodes* libraryNodes );

	/** Writes the geometry.
	@return True on succeeded, false otherwise.*/
	virtual bool writeGeometry ( const COLLADAFW::Geometry* geometry );

	/** Writes the material.
	@return True on succeeded, false otherwise.*/
	virtual bool writeMaterial( const COLLADAFW::Material* material );

	/** Writes the effect.
	@return True on succeeded, false otherwise.*/
	virtual bool writeEffect( const COLLADAFW::Effect* effect );

	/** Writes the camera.
	@return True on succeeded, false otherwise.*/
	virtual bool writeCamera( const COLLADAFW::Camera* camera );

	/** Writes the image.
	@return True on succeeded, false otherwise.*/
	virtual bool writeImage( const COLLADAFW::Image* image );

	/** Writes the light.
	@return True on succeeded, false otherwise.*/
	virtual bool writeLight( const COLLADAFW::Light* light );

	/** Writes the animation.
	@return True on succeeded, false otherwise.*/
	virtual bool writeAnimation( const COLLADAFW::Animation* animation );

	/** Writes the animation.
	@return True on succeeded, false otherwise.*/
	virtual bool writeAnimationList( const COLLADAFW::AnimationList* animationList );

	/** When this method is called, the writer must write the skin controller data.
	@return The writer should return true, if writing succeeded, false otherwise.*/
	virtual bool writeSkinControllerData( const COLLADAFW::SkinControllerData* skinControllerData );

	/** When this method is called, the writer must write the controller.
	@return The writer should return true, if writing succeeded, false otherwise.*/
	virtual bool writeController( const COLLADAFW::Controller* controller );

	/** When this method is called, the writer must write the formulas. All the formulas of the entire
	COLLADA file are contained in @a formulas.
	@return The writer should return true, if writing succeeded, false otherwise.*/
	virtual bool writeFormulas( const COLLADAFW::Formulas* formulas );

	/** When this method is called, the writer must write the kinematics scene. 
	@return The writer should return true, if writing succeeded, false otherwise.*/
	virtual bool writeKinematicsScene( const COLLADAFW::KinematicsScene* kinematicsScene );


	const double getTolerance () const { return m_DigitTolerance; }
private:
	void make_MdxScene();
	void make_MdxNode( const COLLADAFW::Node *node, MdxBone *parentNode);
	void set_NodeInfo( const COLLADAFW::Node *node, MdxBone *mdxNode, MdxBone *parentMdxNode );
	void set_NodeAnimationInfo( const COLLADAFW::Node *node, MdxBone *mdxNode, MdxBone *parentMdxNode );

	void set_MaterialLayer( MdxLayer *inputLayer, COLLADAFW::Sampler *sampler, COLLADAFW::UniqueId effectUniqueId);
	void set_MeshInfo( daeDocMeshInfo* meshdata,MdxMesh* setMdxMesh, MdxArrays* setMdxArrays, MdxDrawArrays *setMdxDrawArrays, std::vector<daeDocMeshInfo> morphTargetList );

	void make_MdxMaterial();
	void make_Mesh( const COLLADAFW::InstanceGeometry* instanceGeometry, MdxBone *setMdxNode  );
	void make_SkinedMesh( const COLLADAFW::InstanceController* instanceController, MdxBone *setMdxNode  );
	void make_MorphingMesh( const COLLADAFW::InstanceController* instanceController, MdxBone *setMdxNode  );

	void make_MeshTangentBinormal( daeDocMeshInfo& setMesh );
	void make_VertexTangentBinormal( const vec4 vPos[3], const vec4 vUv[3], vec4 vtan[3], vec4 vbinorm[3] );

	bool findMdxMaterialName( const COLLADAFW::UniqueId& findId, std::string& getMaterialName );
	bool findMdxTextureName( const COLLADAFW::UniqueId& findId, std::string& getTextureName );
	bool findMdxTextureName( const std::string  findName, std::string& getTextureName );
	const COLLADAFW::Node* findNode ( const COLLADAFW::UniqueId& nodeId, const COLLADAFW::NodePointerArray &nodes );

	double getDoubleValue ( const COLLADAFW::FloatOrDoubleArray &inputValuesArray, const size_t position );
	float getFloatValue ( const COLLADAFW::FloatOrDoubleArray &inputValuesArray, const size_t position );

	bool mergeMdxFCurves( MdxFCurve* outFCurve, int outFCurveDim, std::vector<MdxFCurve*> inFCurveList, std::vector<TransformVectorType> mergeOrder );
	bool mergeMdxFCurveQuat( MdxFCurve* outFCurve, std::vector<MdxFCurve*> rotationFCurveList, std::vector<vec4> staticRotationAxisAngleList, std::vector<TransformVectorType> rotateOrder );
	bool mergeMorphWeightMdxFCurves( MdxFCurve* outFCurve, std::vector<MdxFCurve*> inFCurveList );

	daeDocAnimCurveInfo*	findAnimCurveInfo( const COLLADAFW::UniqueId& animationId );
	COLLADAFW::AnimationList*	DocumentImporter::findAnimationList( const COLLADAFW::UniqueId& animationListId );
	bool writeAnimationCurve( COLLADAFW::AnimationCurve* animationCurve, daeDocAnimCurveInfo* outAnimCurveInfo );
	bool setAnimCurveInfo( COLLADAFW::AnimationCurve* animationCurve, daeDocAnimCurveInfo* outAnimCurveInfo, int iFormat );

	double DegToRad( double Deg )	{ return Deg * 3.1415927 / 180.0; }
	float  DegToRad( float Deg )	{ return Deg * 3.1415927f / 180.0f; }
};

}	// DAEDOC

#endif //__DAEDOCUMENTIMPORTER_H__