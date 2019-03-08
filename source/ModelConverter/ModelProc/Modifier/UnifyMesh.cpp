#include "UnifyMesh.h"

namespace mdx {

static bool equals( MdxBlock *block, MdxBlock *block2, int mode ) ;
static bool compare( MdxBlock *block1, MdxBlock *block2 ) ;
static void enum_prim( MdxBlocks &blocks, MdxBlock *block, bool not_prim = false ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON ARRAYS" ;
enum {
	MODE_OFF,
	MODE_ON,
	MODE_ARRAYS
} ;

//----------------------------------------------------------------
//  UnifyMesh
//----------------------------------------------------------------

bool UnifyMesh::Modify( MdxBlock *block )
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
		ModifyPart( (MdxPart *)parts[ i ], mode ) ;
	}
	return true ;
}

void UnifyMesh::ModifyPart( MdxPart *part, int mode )
{
	MdxBlocks meshes ;
	meshes.EnumChild( part, MDX_MESH ) ;
	for ( int i = 0 ; i < meshes.size() ; i ++ ) {
		MdxMesh *mesh = (MdxMesh *)meshes[ i ] ;
		for ( int j = 0 ; j < i ; j ++ ) {
			MdxMesh *mesh2 = (MdxMesh *)meshes[ j ] ;
			if ( mesh2 == 0 ) continue ;
			if ( equals( mesh, mesh2, mode ) ) {
				MdxBlocks prims ;
				enum_prim( prims, mesh ) ;
				prims.AttachTo( mesh2 ) ;
				mesh->Release() ;
				meshes[ i ] = 0 ;
				break ;
			}
		}
	}
}

bool equals( MdxBlock *block, MdxBlock *block2, int mode )
{
	//  compare attributes

	MdxBlocks props ;
	MdxBlocks props2 ;
	enum_prim( props, block, true ) ;
	enum_prim( props2, block2, true ) ;

	if ( props.size() != props2.size() ) return false ;
	stable_sort( props.begin(), props.end(), compare ) ;
	stable_sort( props2.begin(), props2.end(), compare ) ;
	return props.Equals( props2 ) ;
}

bool compare( MdxBlock *block1, MdxBlock *block2 )	// return ( block1 < block2 )
{
	return ( block1->GetTypeID() < block2->GetTypeID() ) ;
}

void enum_prim( MdxBlocks &blocks, MdxBlock *block, bool not_prim )
{
	for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
		MdxBlock *child = (MdxBlock *)block->GetChild( i ) ;
		bool is_prim = ( dynamic_cast<MdxPrimCommand *>( child ) != 0 ) ;
		if ( is_prim ^ not_prim ) blocks.push_back( child ) ;
	}
}


} // namespace mdx
