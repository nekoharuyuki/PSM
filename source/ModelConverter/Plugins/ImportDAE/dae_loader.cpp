#include "dae_loader.h"
#include "DAEDOCDocumentImporter.h"
#include "COLLADAFWInstanceGeometry.h"
#include "COLLADAFWInstanceController.h"


dae_loader::dae_loader( MdxShell *shell )
{
	m_shell = shell ;

	m_shiny_scale = 1.0f ;
}

dae_loader::~dae_loader()
{
	;
}


bool dae_loader::load( MdxBlock *&block, const string &filename )
{
	block = NULL;
	m_filename = filename;
	m_OutputMdxfile = new MdxFile ;
	

	DAEDOC::DocumentImporter	daeData(m_filename, m_OutputMdxfile );


	if ( daeData.importScene() != true ) return false;	
	

	
	
	block = m_OutputMdxfile ;
	return true;
}

