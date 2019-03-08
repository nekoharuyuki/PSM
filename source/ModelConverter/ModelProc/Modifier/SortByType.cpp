#include "SortByType.h"

namespace mdx {

static bool compare( MdxBlock *block1, MdxBlock *block2 ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON BLOCK COMMAND PARTITION" ;
enum {
	MODE_OFF,
	MODE_ON,
	MODE_BLOCK,
	MODE_COMMAND,
	MODE_PARTITION,
} ;

enum {
	SORT_ALL	= ~0,
	SORT_BLOCK	= 1 << MODE_BLOCK,
	SORT_COMMAND	= 1 << MODE_COMMAND,
	SORT_PARTITION	= 1 << MODE_PARTITION,
} ;

static int g_flags ;

//----------------------------------------------------------------
//  SortByType
//----------------------------------------------------------------

bool SortByType::Modify( MdxBlock *block )
{
	//  check params

	m_flags = 0 ;

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	vector<string> args ;
	str_split( args, arg, ',' ) ;
	for ( int i = 0 ; i < (int)args.size() ; i ++ ) {
		string arg = args[ i ] ;
		int mode = str_search( ModeNames, arg ) ;
		switch ( mode ) {
		    case MODE_OFF :
			m_flags = 0 ;
			break ;
		    case MODE_ON :
			m_flags = SORT_ALL ;
			break ;
		    case MODE_BLOCK :
			m_flags |= SORT_BLOCK ;
			break ;
		    case MODE_COMMAND :
			m_flags |= SORT_COMMAND ;
			break ;
		    case MODE_PARTITION :
			m_flags |= SORT_PARTITION ;
			break ;
		    default :
			Error( "unknown block type \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
	}
	if ( m_flags == 0 ) return true ;

	g_flags = m_flags ;

	//  process

	ModifyBlocks( (MdxBlock *)block ) ;
	return true ;
}

void SortByType::ModifyBlocks( MdxBlock *block )
{
	MdxBlocks blocks ;
	blocks.EnumChild( block ) ;
	blocks.Detach() ;
	stable_sort( blocks.begin(), blocks.end(), compare ) ;
	blocks.AttachTo( block ) ;

	for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
		ModifyBlocks( (MdxBlock *)block->GetChild( i ) ) ;
	}
}

bool compare( MdxBlock *block1, MdxBlock *block2 )	// return ( block1 < block2 )
{
	bool is_cmd_1 = block1->IsCommand() ;
	bool is_cmd_2 = block2->IsCommand() ;
	if ( is_cmd_1 != is_cmd_2 ) return is_cmd_1 ;
	if ( !is_cmd_1 ) {
		if ( !( SORT_BLOCK & g_flags ) ) return false ;
	} else {
		if ( !( SORT_COMMAND & g_flags ) ) return false ;
	}
	return ( block1->GetPriority() < block2->GetPriority() ) ;
}


} // namespace mdx
