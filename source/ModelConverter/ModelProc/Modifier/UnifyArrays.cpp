#include "UnifyArrays.h"

namespace mdx {

static bool equals( MdxArrays *arrays, MdxArrays *arrays2 ) ;
static void unify( MdxArrays *dst, MdxArrays *src ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  UnifyArrays
//----------------------------------------------------------------

bool UnifyArrays::Modify( MdxBlock *block )
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

void UnifyArrays::ModifyPart( MdxPart *part )
{
	MdxBlocks arrayz ;
	arrayz.EnumChild( part, MDX_ARRAYS ) ;
	for ( int i = 0 ; i < arrayz.size() ; i ++ ) {
		MdxArrays *arrays = (MdxArrays *)arrayz[ i ] ;
		for ( int j = 0 ; j < i ; j ++ ) {
			MdxArrays *arrays2 = (MdxArrays *)arrayz[ j ] ;
			if ( arrays2 == 0 ) continue ;
			if ( equals( arrays, arrays2 ) ) {
				unify( arrays, arrays2 ) ;
				arrays->Release() ;
				arrayz[ i ] = 0 ;
				break ;
			}
		}
	}
}

bool equals( MdxArrays *arrays, MdxArrays *arrays2 )
{
	if ( arrays->GetFormat() != arrays2->GetFormat() ) return false ;
	if ( arrays->GetMorphCount() != arrays2->GetMorphCount() ) return false ;
	if ( arrays->GetMorphCount() > 1 ) {
		if ( arrays->GetMorphFormat() != arrays2->GetMorphFormat() ) return false ;
	}
	return true ;
}

#if 0
void unify( MdxArrays *arrays, MdxArrays *arrays2 )
{
	int i, j ;

	int offset = arrays2->GetVertexCount() ;

	MdxBlocks ref ;
	ref.EnumReferrer( arrays ) ;
	for ( i = 0 ; i < ref.size() ; i ++ ) {
		MdxBlock *block = ref[ i ] ;
		MdxPrimCommand *prim = dynamic_cast<MdxPrimCommand *>( block ) ;
		if ( prim != 0 ) {
			prim->SetArrays( arrays2->GetName() ) ;
			int count = prim->GetVertexCount() * prim->GetPrimCount() ;
			for ( j = 0 ; j < count ; j ++ ) {
				prim->SetIndex( j, prim->GetIndex( j ) + offset ) ;
			}
			continue ;
		}
		for ( j = 0 ; j < block->GetArgsCount() ; j ++ ) {
			int desc = block->GetArgsDesc( j ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_ARRAYS ) continue ;
			if ( strcmp( block->GetArgsString( j ), arrays->GetName() ) ) continue ;
			block->SetArgsString( j, arrays2->GetName() ) ;
		}
	}

	arrays2->SetVertexCount( arrays->GetVertexCount() + offset ) ;
	for ( i = 0 ; i < arrays->GetVertexCount() ; i ++ ) {
		for ( j = 0 ; j < arrays->GetMorphCount() ; j ++ ) {
			arrays2->SetVertex( i + offset, j, arrays->GetVertex( i, j ) ) ;
		}
	}
}
#else
void unify( MdxArrays *arrays, MdxArrays *arrays2 )
{
	int i, j, k ;

	int offset = arrays2->GetVertexCount() ;

	MdxBlocks ref ;
	ref.EnumReferrer( arrays ) ;
	for ( i = 0 ; i < ref.size() ; i ++ ) {
		MdxSetArrays *cmd = (MdxSetArrays *)ref[ i ] ;
		if ( cmd->GetTypeID() == MDX_SET_ARRAYS ) {
			MdxMesh *mesh = (MdxMesh *)cmd->GetParent() ;
			for ( j = 0 ; j < mesh->GetChildCount() ; j ++ ) {
				MdxDrawArrays *prim = (MdxDrawArrays *)mesh->GetChild( j ) ;
				if ( prim->GetTypeID() != MDX_DRAW_ARRAYS
				  && prim->GetTypeID() != MDX_DRAW_B_SPLINE ) continue ;
				int count = prim->GetVertexCount() * prim->GetPrimCount() ;
				for ( k = 0 ; k < count ; k ++ ) {
					prim->SetIndex( k, prim->GetIndex( k ) + offset ) ;
				}
			}
			cmd->SetArrays( arrays2->GetName() ) ;
			continue ;
		}
		MdxBlock *block = ref[ i ] ;
		for ( j = 0 ; j < block->GetArgsCount() ; j ++ ) {
			int desc = block->GetArgsDesc( j ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_ARRAYS ) continue ;
			if ( strcmp( block->GetArgsString( j ), arrays->GetName() ) ) continue ;
			block->SetArgsString( j, arrays2->GetName() ) ;
		}
	}
	arrays2->SetVertexCount( arrays->GetVertexCount() + offset ) ;
	for ( i = 0 ; i < arrays->GetMorphCount() ; i ++ ) {
		MdxArrays *morph = arrays->GetMorph( i ) ;
		MdxArrays *morph2 = arrays2->GetMorph( i ) ;
		for ( j = 0 ; j < arrays->GetVertexCount() ; j ++ ) {
			morph2->SetVertex( j + offset, morph->GetVertex( j ) ) ;
		}
	}
}
#endif

} // namespace mdx
