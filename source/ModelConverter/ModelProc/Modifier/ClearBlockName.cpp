#include "ClearBlockName.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  ClearBlockName
//----------------------------------------------------------------

bool ClearBlockName::Modify( MdxBlock *block )
{
	//  check params

	int clear_level = 0 ;

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	if ( str_isdigit( arg ) ) {
		clear_level = str_atoi( arg ) ;
	} else {
		int mode = str_search( ModeNames, arg ) ;
		if ( mode < 0 ) {
			Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		if ( mode == MODE_OFF ) return true ;
	}

	//  process

	ModifyBlock( block, clear_level, -1000000 ) ;
	return true ;
}

void ClearBlockName::ModifyBlock( MdxBlock *block, int clear_level, int current_level )
{
	if ( block->GetTypeID() == MDX_FILE ) current_level = -1 ;
	current_level ++ ;

	for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
		MdxBlock *child = (MdxBlock *)block->GetChild( i ) ;
		if ( child->IsCommand() ) continue ;
		ModifyBlock( child, clear_level, current_level ) ;

		if ( current_level < clear_level ) continue ;
		int type = child->GetTypeID() ;
		string name = child->GetName() ;
		string base = str_untitle( child->GetTypeName() ) ;
		string name2 = str_format( "%s-%d", base.c_str(), block->RankOfChild( child ) ) ;
		if ( name == name2 ) continue ;

		MdxBlocks refs ;
		refs.EnumReferrer( child ) ;
		for ( int j = 0 ; j < refs.size() ; j ++ ) {
			MdxBlock *ref = refs[ j ] ;
			for ( int k = 0 ; k < ref->GetArgsCount() ; k ++ ) {
				int desc = ref->GetArgsDesc( k ) ;
				if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
				if ( MDX_WORD_SCOPE( desc ) != type ) continue ;
				if ( strcmp( ref->GetArgsString( k ), name.c_str() ) ) continue ;
				ref->SetArgsString( k, name2.c_str() ) ;
			}
		}
		child->SetName( name2.c_str() ) ;
	}
}


} // namespace mdx
