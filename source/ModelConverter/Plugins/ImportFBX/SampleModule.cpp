#include "SampleModule.h"
#include "ImportFBX.h"

//----------------------------------------------------------------
//  module entry
//----------------------------------------------------------------

extern "C" { class SAMPLEMODULE SAMPLEMODULE ; }

bool SAMPLEMODULE::InitModule( MdxShell *shell, int argc, const char **argv )
{
	if ( shell == 0 ) return false ;

	shell->SetProc( 0, new ImportFBX ) ;

	if ( shell->GetProc( "ImportOBJ" ) == 0 ) {
		shell->SetProc( "ImportOBJ", new ImportFBX ) ;
	}
	if ( shell->GetProc( "Import3DS" ) == 0 ) {
		shell->SetProc( "Import3DS", new ImportFBX ) ;
	}
	if ( shell->GetProc( "ImportDXF" ) == 0 ) {
		shell->SetProc( "ImportDXF", new ImportFBX ) ;
	}
	if ( shell->GetProc( "ImportDAE" ) == 0 ) {
		shell->SetProc( "ImportDAE", new ImportFBX ) ;
	}

	return true ;
}

void SAMPLEMODULE::ExitModule()
{
	return ;
}

