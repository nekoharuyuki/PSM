#include "UnifyVertex.h"

namespace mdx {

static bool equals_vertex( MdxArrays *arrays, int a, int b ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  UnifyVertex
//----------------------------------------------------------------

bool UnifyVertex::Modify( MdxBlock *block )
{
	//  check params

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	int mode = str_search( ModeNames, arg ) ;
	if ( mode < 0 ) {
		Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
		return false ;
	}
	if ( mode == MODE_OFF ) return true ;

	//  process

	MdxBlocks parts ;
	parts.EnumTree( block, MDX_PART ) ;
	for ( int i = 0 ; i < parts.size() ; i ++ ) {
		ModifyPart( (MdxPart *)parts[ i ] ) ;
	}
	return true ;
}

void UnifyVertex::ModifyPart( MdxPart *part )
{
	int i, j, k ;
	map<string,vector<int> > arrays_renum ;	// arrays name -> renumber map

	MdxBlocks arrayz ;
	arrayz.EnumChild( part, MDX_ARRAYS ) ;
	for ( i = 0 ; i < arrayz.size() ; i ++ ) {
		MdxArrays *arrays = (MdxArrays *)arrayz[ i ] ;

		vector<int> &renum = arrays_renum[ arrays->GetName() ] ;

		int index = 0 ;
		int count = arrays->GetVertexCount() ;
		for ( j = 0 ; j < count ; j ++ ) {
			for ( k = 0 ; k < index ; k ++ ) {
				if ( equals_vertex( arrays, k, index ) ) break ;
			}
			if ( k < index ) {
				arrays->DeleteVertex( index ) ;
			} else {
				index ++ ;
			}
			renum.push_back( k ) ;
		}
	}

	#if 0
	MdxBlocks cmds ;
	cmds.EnumTree( part ) ;
	for ( i = 0 ; i < cmds.size() ; i ++ ) {
		MdxPrimCommand *cmd = dynamic_cast<MdxPrimCommand *>( cmds[ i ] ) ;
		if ( cmd == 0 ) continue ;

		const vector<int> &renum = arrays_renum[ cmd->GetArrays() ] ;

		int count = cmd->GetVertexCount() * cmd->GetPrimCount() ;
		for ( j = 0 ; j < count ; j ++ ) {
			cmd->SetIndex( j, renum[ cmd->GetIndex( j ) ] ) ;
		}
	}
	#else
	MdxBlocks meshes ;
	meshes.EnumTree( part, MDX_MESH ) ;
	for ( i = 0 ; i < meshes.size() ; i ++ ) {
		MdxMesh *mesh = (MdxMesh *)meshes[ i ] ;

		MdxSetArrays *cmd = (MdxSetArrays *)mesh->FindChild( MDX_SET_ARRAYS ) ;
		if ( cmd == 0 ) continue ;
		vector<int> &renum = arrays_renum[ cmd->GetArrays() ] ;

		for ( j = 0 ; j < mesh->GetChildCount() ; j ++ ) {
			MdxDrawArrays *prim = (MdxDrawArrays *)mesh->GetChild( j ) ;
			if ( prim->GetTypeID() != MDX_DRAW_ARRAYS
			  && prim->GetTypeID() != MDX_DRAW_B_SPLINE ) continue ;
			int count = prim->GetVertexCount() * prim->GetPrimCount() ;
			for ( k = 0 ; k < count ; k ++ ) {
				prim->SetIndex( k, renum[ prim->GetIndex( k ) ] ) ;
			}
		}
	}
	#endif
}

bool equals_vertex( MdxArrays *arrays, int a, int b )
{
	for ( int i = 0 ; i < arrays->GetMorphCount() ; i ++ ) {
		MdxArrays *morph = arrays->GetMorph( i ) ;
		if ( morph->GetVertex( a ) != morph->GetVertex( b ) ) return false ;
	}
	return true ;
}


} // namespace mdx
