#include "MergeModels.h"

namespace mdx {

//----------------------------------------------------------------
//  MergeModels
//----------------------------------------------------------------

bool MergeModels::Modify( MdxBlock *block )
{
	for ( int i = 2 ; i < GetArgCount() ; i ++ ) {
		MdxBlock *block2 = GetBlock( GetArg( i ) ) ;
		if ( block2 == 0 ) {
			Error( "wrong source \"%s\"\n", GetArg( i ) ) ;
			return false ;
		}

		MdxBlocks models ;
		models.EnumTree( block2, MDX_MODEL ) ;
		for ( int j = 0 ; j < models.size() ; j ++ ) {
			MdxModel *model = (MdxModel *)models[ j ] ;
			block->AttachChild( model->CloneTree() ) ;
		}
	}
	return true ;
}


} // namespace mdx
