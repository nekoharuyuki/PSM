#ifndef	FBX_MAYA_H_INCLUDE
#define	FBX_MAYA_H_INCLUDE

class fbx_loader ;

//----------------------------------------------------------------------------
//  fbx_maya
//----------------------------------------------------------------------------

class fbx_maya {
public:
	fbx_maya( MdxShell *shell = 0 ) ;
	~fbx_maya() ;

	void set_notes_transform( int mode, const string &prefix ) {
		m_notes_transform_mode = mode ;
		m_notes_transform_prefix = prefix ;
	}
	void set_notes_material( int mode, const string &prefix ) {
		m_notes_material_mode = mode ;
		m_notes_material_prefix = prefix ;
	}
	void set_notes_userdata( int mode, const string &prefix ) {
		m_notes_userdata_mode = mode ;
		m_notes_userdata_prefix = prefix ;
	}

	void clear() ;
	bool load( const string &filename ) ;
	bool save( fbx_loader &loader ) ;

	int get_time_mode() { return m_time_mode ; }
	float get_frame_rate() { return m_frame_rate ; }
	float get_frame_start() { return m_frame_start ; }
	float get_frame_end() { return m_frame_end ; }

private:
	struct curve {
		int tan ;
		vector<float> ktvs ;
		vector<float> kots ;
		int pre, pst ;
	public:
		curve() : tan( 0 ), pre( 0 ), pst( 0 ) {}
	} ;
	struct notes {
		string nts ;
		int line_no ;
	} ;

private:
	bool save_material_animation( fbx_loader &loader ) ;
	bool save_texture_animation( fbx_loader &loader ) ;
	bool save_extrapolation( fbx_loader &loader ) ;
	bool save_transform_notes( fbx_loader &loader ) ;
	bool save_material_notes( fbx_loader &loader ) ;
	bool save_frame_info( fbx_loader &loader ) ;

	string get_option( vector<string> &args, const string &name, int start = 0 ) ;
	int remove_options( vector<string> &args, const string &value_options ) ;
	bool parse_notes( MdxBlock *block, const notes &n ) ;
	bool parse_transform( MdxBlock *block, const string &line ) ;
	bool parse_material( MdxBlock *block, const string &line ) ;
	bool parse_userdata( MdxBlock *block, const string &line ) ;
	string sanitize( const string &str ) ;
	MdxFCurve *create_fcurve( const curve &ac, float (*modify)( float ) = 0 ) ;
	int get_extrap( const curve &ac ) ;

private:
	MdxShell *m_shell ;
	int m_notes_transform_mode ;
	int m_notes_material_mode ;
	int m_notes_userdata_mode ;
	string m_notes_transform_prefix ;
	string m_notes_material_prefix ;
	string m_notes_userdata_prefix ;

	map<string,rect> m_place_texs ;		//  place2dTexture ( offset/repeat )
	map<string,rect> m_place_texs2 ;	//  place2dTexture ( transform/coverage )
	map<string,curve> m_anim_curves ;	//  animCurveTL/TA/TU
	map<string,string> m_connect_attrs ;	//  connectAttr
	map<string,notes> m_transform_notes ;	//  transform
	map<string,notes> m_material_notes ;	//  lambert ... etc
	int m_time_mode ;			//  currentUnit
	float m_frame_rate ;
	float m_frame_start ;			//  playbackOptions
	float m_frame_end ;			//  playbackOptions

	string m_filename ;
	MdxModel *m_model ;
	MdxBone *m_bone ;
	MdxMaterial *m_material ;
	MdxMotion *m_motion ;
	MdxFCurve *m_fcurve ;
} ;


#endif
