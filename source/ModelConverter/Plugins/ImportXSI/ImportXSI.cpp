#include "ImportXSI.h"
#include "xsi_loader.h"

//----------------------------------------------------------------
//  ImportXSI
//----------------------------------------------------------------

bool ImportXSI::Import( MdxBlock *&block, const void *buf, int size )
{
	return false ;	// not supported
}

bool ImportXSI::Import( MdxBlock *&block, const char *filename )
{
	xsi_loader loader( m_shell ) ;

	//  xsi options

	string var = str_toupper( GetVar( "xsi_vcolor_lighting" ) ) ;
	int mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_vcolor_lighting( mode ) ;

	var = str_toupper( GetVar( "xsi_vcolor_specular" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_vcolor_specular( mode ) ;

	var = str_toupper( GetVar( "xsi_ignore_textrans" ) ) ;
	mode = str_search( "OFF ON AUTO", var ) ;
	if ( mode >= 0 ) loader.set_ignore_textrans( mode ) ;

	var = str_toupper( GetVar( "xsi_ignore_prefix" ) ) ;
	mode = str_search( "OFF ON", var ) ;
	if ( mode >= 0 ) loader.set_ignore_prefix( mode ) ;

	//  custom param options

	var = str_toupper( GetVar( "xsi_check_custom_param" ) ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_check_custom_param( xsi_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	var = GetVar( "xsi_custom_param_transform" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_custom_param_transform( xsi_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	var = GetVar( "xsi_custom_param_material" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_custom_param_material( xsi_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	var = GetVar( "xsi_custom_param_userdata" ) ;
	mode = str_search( "OFF ON", str_toupper( var ) ) ;
	if ( mode != 0 ) loader.set_custom_param_userdata( xsi_loader::MODE_ON, ( mode >= 0 ) ? "" : var ) ;

	//  xsi options ( temporary )

	var = GetVar( "xsi_shiny_scale" ) ;
	if ( str_isdigit( var ) ) loader.set_shiny_scale( str_atof( var ) ) ;

	return loader.load( block, filename ) ;
}
