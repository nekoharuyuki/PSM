#include "UnifyMaterial.h"

namespace mdx {

static bool equals( MdxBlock *block, MdxBlock *block2 ) ;
static bool compare( MdxBlock *block1, MdxBlock *block2 ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  UnifyMaterial
//----------------------------------------------------------------

bool UnifyMaterial::Modify( MdxBlock *block )
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

	MdxBlocks materials ;
	materials.EnumTree( block, MDX_MATERIAL ) ;
	for ( int i = 0 ; i < materials.size() ; i ++ ) {
		MdxMaterial *material = (MdxMaterial *)materials[ i ] ;
		if ( material->FindReferrer( MDX_ANIMATE ) != 0 ) {
			materials[ i ] = 0 ;
			continue ;
		}
		for ( int j = 0 ; j < i ; j ++ ) {
			MdxMaterial *material2 = (MdxMaterial *)materials[ j ] ;
			if ( material2 == 0 ) continue ;
			if ( equals( material, material2 ) ) {
				ModifyReference( material, material2 ) ;
				material->Release() ;
				materials[ i ] = 0 ;
				break ;
			}
		}
	}
	return true ;
}

void UnifyMaterial::ModifyReference( MdxMaterial *material, MdxMaterial *material2 )
{
	MdxBlocks ref ;
	ref.EnumReferrer( material ) ;
	for ( int i = 0 ; i < ref.size() ; i ++ ) {
		MdxBlock *block = ref[ i ] ;
		for ( int j = 0 ; j < block->GetArgsCount() ; j ++ ) {
			int desc = block->GetArgsDesc( j ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_MATERIAL ) continue ;
			if ( strcmp( block->GetArgsString( j ), material->GetName() ) ) continue ;
			block->SetArgsString( j, material2->GetName() ) ;
		}
	}
}

bool equals( MdxBlock *block, MdxBlock *block2 )
{
	MdxBlocks blocks ;
	MdxBlocks blocks2 ;
	blocks.EnumChild( block ) ;
	blocks2.EnumChild( block2 ) ;
	if ( blocks.size() != blocks2.size() ) return false ;
	stable_sort( blocks.begin(), blocks.end(), compare ) ;
	stable_sort( blocks2.begin(), blocks2.end(), compare ) ;
	for ( int i = 0 ; i < blocks.size() ; i ++ ) {
		MdxBlock *child = blocks[ i ] ;
		MdxBlock *child2 = blocks2[ i ] ;
		if ( !child->Equals( child2 ) ) return false ;
		if ( !equals( child, child2 ) ) return false ;
	}
	return true ;
}

bool compare( MdxBlock *block1, MdxBlock *block2 )	// return ( block1 < block2 )
{
	return block1->GetTypeID() < block2->GetTypeID() ;
}


} // namespace mdx
