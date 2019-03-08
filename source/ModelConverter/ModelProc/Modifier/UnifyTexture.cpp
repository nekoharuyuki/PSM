#include "UnifyTexture.h"

namespace mdx {

static bool equals( MdxBlock *block, MdxBlock *block2 ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  UnifyTexture
//----------------------------------------------------------------

bool UnifyTexture::Modify( MdxBlock *block )
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

	MdxBlocks textures ;
	textures.EnumTree( block, MDX_TEXTURE ) ;
	for ( int i = 0 ; i < textures.size() ; i ++ ) {
		MdxTexture *texture = (MdxTexture *)textures[ i ] ;
		for ( int j = 0 ; j < i ; j ++ ) {
			MdxTexture *texture2 = (MdxTexture *)textures[ j ] ;
			if ( texture2 == 0 ) continue ;
			if ( equals( texture, texture2 ) ) {
				ModifyReference( texture, texture2 ) ;
				texture->Release() ;
				textures[ i ] = 0 ;
				break ;
			}
		}
	}
	return true ;
}

void UnifyTexture::ModifyReference( MdxTexture *texture, MdxTexture *texture2 )
{
	MdxBlocks ref ;
	ref.EnumReferrer( texture ) ;
	for ( int i = 0 ; i < ref.size() ; i ++ ) {
		MdxBlock *block = ref[ i ] ;
		for ( int j = 0 ; j < block->GetArgsCount() ; j ++ ) {
			int desc = block->GetArgsDesc( j ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_TEXTURE ) continue ;
			if ( strcmp( block->GetArgsString( j ), texture->GetName() ) ) continue ;
			block->SetArgsString( j, texture2->GetName() ) ;
		}
	}
}

bool equals( MdxBlock *block, MdxBlock *block2 )
{
	MdxFileName *filename = (MdxFileName *)block->FindChild( MDX_FILE_NAME ) ;
	MdxFileName *filename2 = (MdxFileName *)block2->FindChild( MDX_FILE_NAME ) ;
	if ( filename == 0 || filename2 == 0 ) return ( filename == filename2 ) ;
	return ( strcmp( filename->GetFileName(), filename2->GetFileName() ) == 0 ) ;
}


} // namespace mdx
