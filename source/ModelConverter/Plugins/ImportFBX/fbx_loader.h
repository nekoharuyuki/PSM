#ifndef	FBX_LOADER_H_INCLUDE
#define	FBX_LOADER_H_INCLUDE

#include <fbxsdk.h>
#include <fbxfilesdk_nsuse.h>

#ifndef _KFbxMaterial_h
#define AFTER_FBXSDK_200611 1
typedef KFbxSurfaceMaterial KFbxMaterial ;
#ifndef _FBXSDK_SCENE_INFO_H_
#define AFTER_FBXSDK_200901 1
#ifndef _FBXSDK_NODE_H_
#define AFTER_FBXSDK_200903 1
#ifndef FBXFILESDK_KFBXPLUGINS_KFBXTAKENODE_H
#define AFTER_FBXSDK_201102 1
#endif // FBXFILESDK_KFBXPLUGINS_KFBXTAKENODE_H
#endif // _FBXSDK_NODE_H_
#endif // _FBXSDK_SCENE_INFO_H_
#endif // _KFbxMaterial_h

#if ( AFTER_FBXSDK_200611 == 0 )
#pragma comment( lib, "fbxsdk_md.lib" )		// FBXSDK 2005.12a
#elif ( AFTER_FBXSDK_200901 == 0 )
#pragma comment( lib, "fbxsdk_md2005.lib" )	// FBXSDK 2006.11
#elif ( _MSC_VER < 1400 )
#pragma comment( lib, "fbxsdk_md2003.lib" )	// VC++ 7
#elif ( _MSC_VER < 1500 )
#pragma comment( lib, "fbxsdk_md2005.lib" )	// VC++ 8
#elif ( _MSC_VER < 1600 )
#pragma comment( lib, "fbxsdk_md2008.lib" )	// VC++ 9
#else // _MSC_VER >= 1600
#pragma comment( lib, "fbxsdk_md2010.lib" )	// VC++ 10
#endif

#define kDouble double
#define kInt int

#include "ImportFBX.h"
#include "fbx_arrays.h"
#include "fbx_fcurve.h"
#include "fbx_maya.h"

#define SET_TEX_XFORM_TO_LAYER	(0)

//----------------------------------------------------------------
//  fbx_loader
//----------------------------------------------------------------

class fbx_loader {
public:
	friend class fbx_maya ;
	enum {
		MODE_OFF	= 0,
		MODE_ON		= 1,
		MODE_AUTO	= 2
	} ;

public:
	fbx_loader( MdxShell *shell = 0 ) ;
	~fbx_loader() ;

	void set_time_mode( int mode ) { m_time_mode = mode ; }
	void set_output_root( int mode ) { m_output_root = mode ; }
	void set_filter_fcurve( int mode ) { m_filter_fcurve = mode ; }
	void set_filter_transp( int mode ) { m_filter_transp = mode ; }
	void set_use_material_name( int mode ) { m_use_material_name = mode ; }
	void set_use_texture_name( int mode ) { m_use_texture_name = mode ; }
	void set_default_vcolor( int mode, rgba8888 c ) { m_default_vcolor = mode ; m_vcolor_value = c ; }
	void set_check_maya_ascii( int mode ) { m_check_maya_ascii = mode ; }
	void set_maya_notes_transform( int mode, const string &prefix ) {
		m_maya_notes_transform_mode = mode ;
		m_maya_notes_transform_prefix = prefix ;
	}
	void set_maya_notes_material( int mode, const string &prefix ) {
		m_maya_notes_material_mode = mode ;
		m_maya_notes_material_prefix = prefix ;
	}
	void set_maya_notes_userdata( int mode, const string &prefix ) {
		m_maya_notes_userdata_mode = mode ;
		m_maya_notes_userdata_prefix = prefix ;
	}
	void set_shiny_scale( float scale ) { m_shiny_scale = scale ; }

	bool load( MdxBlock *&block, const string &filename ) ;

private:
	void load_geometries( KFbxNode *node, MdxBone *parent ) ;
	void load_bone( KFbxNode *node, MdxBone *parent ) ;
	void load_part( KFbxNode *node ) ;
	void load_mesh( KFbxMesh *mesh, fbx_arrays &arrays ) ;
	void load_nurb( KFbxNurb *nurb, fbx_arrays &arrays ) ;
	void load_patch( KFbxPatch *patch, fbx_arrays &arrays ) ;
	void load_shape( KFbxNode *node, fbx_arrays &arrays ) ;
	void load_link( KFbxNode *node, fbx_arrays &arrays ) ;
	#if ( AFTER_FBXSDK_200611 )
	void load_nurb( KFbxNurbsSurface *nurb, fbx_arrays &arrays ) ;
	#endif // AFTER_FBXSDK_200611

	void load_materials() ;

	void set_animation( MdxBlock *block, int command, int mode, MdxFCurve *fcurve ) ;
	void adjust_rotate_fcurve( MdxFCurve *fcurve, quat &prerot, quat (*quat_from_rot)( const vec4 &r ) ) ;
	void adjust_morph_fcurve( MdxFCurve *fcurve ) ;

	int get_material_index( KFbxLayerElementMaterial *materials, KFbxLayerElementTexture *textures, int poly_idx = 0, bool has_vcolor = false ) ;
	int get_normal_index( KFbxLayerElementNormal *normals, int ctrl_idx, int vert_idx, int poly_idx ) ;
	int get_vcolor_index( KFbxLayerElementVertexColor *vcolors, int ctrl_idx, int vert_idx, int poly_idx ) ;
	int get_tcoord_index( KFbxLayerElementUV *tcoords, int ctrl_idx, int vert_idx, int poly_idx ) ;
	rgba8888 get_diffuse_color( KFbxMaterial *material ) ;
	KFbxTexture *get_material_texture( KFbxMaterial *material, const char *name ) ;
	string get_node_name( KFbxNode *node ) ;
	string get_material_name( int mat_idx ) ;
	string get_texture_name( int tex_idx ) ;
	mat4 get_offset_matrix( KFbxLink *link ) ;
	int get_knot_type( const kDouble *knots, int n_knots, int degree ) ;
	float eval_fcurve( KFCurve *fcurve, KTime time, float val ) ;

private:
	struct material_info {
		KFbxMaterial *material ; KFbxTexture *texture ; bool vcolor ;
	public:
		material_info( KFbxMaterial *m = 0, KFbxTexture *t = 0, bool v = false ) {
			material = m ; texture = t ; vcolor = v ;
		}
		bool operator==( const material_info &m ) const {
			return ( material == m.material && texture == m.texture && vcolor == m.vcolor ) ;
		}
	} ;

private:

	MdxShell *m_shell ;

	int m_time_mode ;
	int m_output_root ;
	int m_filter_fcurve ;
	int m_filter_transp ;
	int m_use_material_name ;
	int m_use_texture_name ;
	int m_default_vcolor ;
	rgba8888 m_vcolor_value ;

	int m_check_maya_ascii ;
	int m_maya_notes_transform_mode ;
	int m_maya_notes_material_mode ;
	int m_maya_notes_userdata_mode ;
	string m_maya_notes_transform_prefix ;
	string m_maya_notes_material_prefix ;
	string m_maya_notes_userdata_prefix ;

	float m_shiny_scale ;

	string m_filename ;
	MdxFile *m_file ;
	MdxModel *m_model ;
	MdxBone *m_bone ;
	MdxPart *m_part ;
	MdxMesh *m_mesh ;
	MdxArrays *m_arrays ;
	MdxMaterial *m_material ;
	MdxLayer *m_layer ;
	MdxTexture *m_texture ;
	MdxMotion *m_motion ;
	MdxFCurve *m_fcurve ;

	KTime m_motion_step ;
	KFbxNode *m_node ;
	vector<KFbxTakeInfo *> m_motion_infos ;
	vector<material_info> m_material_infos ;
	vector<KFbxTexture *> m_texture_infos ;
} ;


#endif

