#include "ImportX.h"
#include "x_loader.h"

//----------------------------------------------------------------
//  ImportX
//----------------------------------------------------------------

bool ImportX::Import( MdxBlock *&block, const void *buf, int size )
{
	return false ;	// not supported
}

bool ImportX::Import( MdxBlock *&block, const char *filename )
{
	x_loader loader( m_shell ) ;
	return loader.load( block, filename ) ;
}
