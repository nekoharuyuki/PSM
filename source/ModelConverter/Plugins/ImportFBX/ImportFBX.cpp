#include "ImportFBX.h"
#include "fbx_loader.h"

//----------------------------------------------------------------
//  ImportFBX
//----------------------------------------------------------------

bool ImportFBX::Import( MdxBlock *&block, const void *buf, int size )
{
	return false ;	// not supported
}

bool ImportFBX::Import( MdxBlock *&block, const char *filename )
{
	fbx_loader loader( m_shell ) ;

	//  fbx options

	string var = str_toupper( GetVar( "fbx_time_mode" ) ) ;
	int mode = !str_isdigit( var ) ? str_search( "OFF ON", var ) : str_atoi( var ) ;
	if ( mode >= 0 ) loader.set_time_mode( mode ) ;

	var = str_toupper( GetVar( "fbx_output_root" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_output_root( mode ) ;

	var = str_toupper( GetVar( "fbx_filter_fcurve" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_filter_fcurve( mode ) ;

	var = str_toupper( GetVar( "fbx_filter_transp" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_filter_transp( mode ) ;

	var = str_toupper( GetVar( "fbx_use_material_name" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_use_material_name( mode ) ;

	var = str_toupper( GetVar( "fbx_use_texture_name" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_use_texture_name( mode ) ;

	var = GetVar( "fbx_default_vcolor" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode >= 0 ) {
		loader.set_default_vcolor( mode, 0xffffffff ) ;
	} else if ( str_isdigit( var ) ) {
		loader.set_default_vcolor( fbx_loader::MODE_OFF, str_atoi( var ) ) ;
	}

	//  maya ascii options

	var = str_toupper( GetVar( "fbx_check_maya_ascii" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_check_maya_ascii( mode ) ;

	var = GetVar( "fbx_maya_notes_transform" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_maya_notes_transform( fbx_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	var = GetVar( "fbx_maya_notes_material" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_maya_notes_material( fbx_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	var = GetVar( "fbx_maya_notes_userdata" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_maya_notes_userdata( fbx_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	//  fbx options ( temporary )

	var = GetVar( "fbx_shiny_scale" ) ;
	if ( str_isdigit( var ) ) loader.set_shiny_scale( str_atof( var ) ) ;

	return loader.load( block, filename ) ;
}
