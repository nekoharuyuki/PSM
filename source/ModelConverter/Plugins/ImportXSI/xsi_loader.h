#ifndef	XSI_LOADER_H_INCLUDE
#define	XSI_LOADER_H_INCLUDE

#include "SemanticLayer.h"	// XSI FTK

#ifndef _XSISHAPE_H
#define AFTER_CROSSWALK_2_6 0
#else // _XSISHAPE_H
#define AFTER_CROSSWALK_2_6 1
#include "XSITransform.h"
#endif // _XSISHAPE_H

#include "ImportXSI.h"
#include "xsi_arrays.h"
#include "xsi_fcurve.h"
#include "xsi_fcurve2.h"

//----------------------------------------------------------------
//  xsi_loader
//----------------------------------------------------------------

class xsi_loader {
public:
	enum {
		MODE_OFF	= 0,
		MODE_ON		= 1,
		MODE_AUTO	= 2,
	} ;

public:
	xsi_loader( MdxShell *shell = 0 ) ;
	~xsi_loader() ;

	void set_vcolor_lighting( int mode ) { m_vcolor_lighting = mode ; }
	void set_vcolor_specular( int mode ) { m_vcolor_specular = mode ; }
	void set_ignore_textrans( int mode ) { m_ignore_textrans = mode ; }
	void set_ignore_prefix( int mode ) { m_ignore_prefix = mode ; }
	void set_check_custom_param( int mode, const string &prefix ) {
		m_check_custom_param_mode = mode ;
		m_check_custom_param_prefix = prefix ;
	}
	void set_custom_param_transform( int mode, const string &prefix ) {
		m_custom_param_transform_mode = mode ;
		m_custom_param_transform_prefix = prefix ;
	}
	void set_custom_param_material( int mode, const string &prefix ) {
		m_custom_param_material_mode = mode ;
		m_custom_param_material_prefix = prefix ;
	}
	void set_custom_param_userdata( int mode, const string &prefix ) {
		m_custom_param_userdata_mode = mode ;
		m_custom_param_userdata_prefix = prefix ;
	}
	void set_shiny_scale( float scale ) { m_shiny_scale = scale ; }

	bool load( MdxBlock *&block, const string &filename ) ;

private:
	void load_geometries( CSLModel *model, MdxBone *parent ) ;
	void load_bone( CSLModel *model, MdxBone *parent ) ;
	void load_transform( CSLTransform *trans, const string &name, const string &inst ) ;
	#if ( AFTER_CROSSWALK_2_6 )
	void load_transform( CSLXSITransform *trans, const string &name, const string &inst ) ;
	void load_transform( CSLXSIPolymatricks *trans, const string &name, const string &inst ) ;
	#endif // AFTER_CROSSWALK_2_6
	void load_part( CSLModel *model ) ;
	void load_triangles( CSLMesh *mesh, xsi_arrays &arrays ) ;
	void load_tristrips( CSLMesh *mesh, xsi_arrays &arrays ) ;
	void load_polygons( CSLMesh *mesh, xsi_arrays &arrays ) ;
	void load_surface( CSLNurbsSurface *surf, xsi_arrays &arrays, CSLBaseMaterial *material ) ;
	void load_shape_anim( CSLGeometry *geom, xsi_arrays &arrays ) ;
	#if ( AFTER_CROSSWALK_2_6 )
	void load_triangles( CSLXSIMesh *mesh, xsi_arrays &arrays ) ;		//  dotXSI 5.0 and beyond
	void load_tristrips( CSLXSIMesh *mesh, xsi_arrays &arrays ) ;		//  dotXSI 5.0 and beyond
	void load_polygons( CSLXSIMesh *mesh, xsi_arrays &arrays ) ;		//  dotXSI 5.0 and beyond
	void load_shape_anim( CSLXSIGeometry *geom, xsi_arrays &arrays ) ;	//  dotXSI 5.0 and beyond
	#endif // AFTER_CROSSWALK_2_6
	void load_envelope_weights( CSLModel *model, xsi_arrays &arrays ) ;
	void load_envelope_bones( CSLModel *model ) ;

	void load_materials( CSLMaterialLibrary *material_library ) ;
	void load_material( CSLMaterial *material ) ;
	void load_texture( CSLTexture2D *texture ) ;
	void load_material( CSLXSIMaterial *material ) ;	//  dotXSI 3.5 and beyond
	void load_texture( CSLXSIShader *texture ) ;		//  dotXSI 3.5 and beyond
	bool find_vector( CSLXSIShader *shader, const string &name, vec4 &value, CSLXSIShader **texture = 0 ) ;
	bool find_scalar( CSLXSIShader *shader, const string &name, float &value, CSLXSIShader **texture = 0 ) ;
	bool get_scalar( CSLXSIShader *shader, const string &name, float &value ) ;

	bool load_custom_param( MdxBlock *block, CSLTemplate *tmp ) ;
	bool load_transform_param( MdxBlock *block, const string &line ) ;
	bool load_material_param( MdxBlock *block, const string &line ) ;
	bool load_userdata_param( MdxBlock *block, const string &line ) ;
	string sanitize( const string &str ) ;

	void ignore_textrans() ;

	string get_node_name( CSLTemplate *tmp ) ;

	void set_animation( int scope, const string &block, int command, int mode, const string &fcurve ) ;
	bool copy_animation( int scope, const string &block, int command, int mode, const string &inst ) ;

	void set_transform_fcurves( const string &name, const string &inst, int type, const vec3 &v,
				CSLTemplate *tmp, int channel ) ;
	void set_transform_fcurves( const string &name, const string &inst, int type, const vec3 &v,
				CSLTemplate *tmp, const char **channels ) ;
	void set_transform_fcurves( const string &name, const string &inst, int type, const vec3 &v,
				CSLFCurve **anims ) ;
	void set_material_fcurves( const string &name, int type, const vec4 &v, CSLFCurve **anims, int n_anims ) ;
	void set_material_fcurves( const string &name, int type, const vec4 &v, CdotXSITemplate **anims, int n_anims ) ;
	void adjust_transparency_fcurve( MdxFCurve *fcurve ) ;

	bool get_fcurves( CSLFCurve **anims, CSLTemplate *tmp, int *channels, int n_channels ) ;
	bool get_fcurves( CSLFCurve **anims, CSLTemplate *tmp, const char **channels, int n_channels ) ;
	bool get_fcurves( CdotXSITemplate **anims, CSLTemplate *tmp, const char **channels, int n_channels ) ;
	CSLFCurve *get_fcurve( CSLTemplate *tmp, const char *channel ) ;
	CdotXSITemplate *get_fcurve( CdotXSITemplate *temp, const string &name ) ;

public:
	static int get_int( CdotXSITemplate *temp, int num, int idx = 0 ) ;
	static float get_float( CdotXSITemplate *temp, int num, int idx = 0 ) ;
	static string get_string( CdotXSITemplate *temp, int num, int idx = 0 ) ;

private:
	struct material_state {
		int flags ;
		rect crop2 ;
		string tspace ;
		map<CSLModel*,string> tspaces ;
	public:
		material_state() {
			flags = 0 ;
			crop2.set( 0, 0, 1, 1 ) ;
		}
		const string &find_tspace( CSLModel *object ) {
			map<CSLModel*,string>::iterator it = tspaces.find( object ) ;
			return ( it == tspaces.end() ) ? tspace : it->second ;
		}
	} ;

private:
	MdxShell *m_shell ;
	int m_vcolor_lighting ;
	int m_vcolor_specular ;
	int m_ignore_textrans ;
	int m_ignore_prefix ;

	int m_check_custom_param_mode ;
	int m_custom_param_transform_mode ;
	int m_custom_param_material_mode ;
	int m_custom_param_userdata_mode ;
	string m_check_custom_param_prefix ;
	string m_custom_param_transform_prefix ;
	string m_custom_param_material_prefix ;
	string m_custom_param_userdata_prefix ;

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

	int m_nurbs_width ;
	int m_nurbs_wrap ;
	map<string,material_state> m_material_states ;
	CSLModel *m_current_object ;

	map<string,string> m_instance_names ;
} ;


#endif
