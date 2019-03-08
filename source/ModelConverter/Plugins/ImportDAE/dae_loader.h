#ifndef	COLLADA_LOADER_H_INCLUDE
#define	COLLADA_LOADER_H_INCLUDE


#include "ImportDAE.h"
#include "DAEDOCDocumentImporter.h"
#include "COLLADASaxFWLLoader.h"
#include "COLLADAFWVisualScene.h"



class dae_loader {
public:
	enum {
		MODE_OFF	= 0,
		MODE_ON		= 1,
		MODE_AUTO	= 2,
	} ;

public:
	dae_loader( MdxShell *shell = 0 ) ;
	~dae_loader() ;


	bool load( MdxBlock *&block, const string &filename ) ;

private:
	MdxShell *m_shell ;
	float m_shiny_scale ;

	string		m_filename ;
	MdxFile		*m_OutputMdxfile ;
	


	DAEDOC::DocumentImporter* m_daeData;


};

#endif