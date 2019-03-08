#include "ImportDAE.h"
#include "dae_loader.h"

//----------------------------------------------------------------
//  ImportCollada
//----------------------------------------------------------------

bool ImportCollada::Import( MdxBlock *&block, const char *filename )
{

	
	dae_loader loader( m_shell ) ;

	return loader.load( block, filename );


	
}

bool ImportCollada::Import( MdxBlock *&block, const void *buf, int size )
{
	return false ;	// not supported ( To use OpenCollada ) 
	
}

