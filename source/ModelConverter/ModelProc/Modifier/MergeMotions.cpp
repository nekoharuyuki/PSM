#include "MergeMotions.h"

namespace mdx {

//----------------------------------------------------------------
//  MergeMotions
//----------------------------------------------------------------

bool MergeMotions::Modify( MdxBlock *block )
{
	MdxModel *model = (MdxModel *)block->FindTree( MDX_MODEL ) ;
	if ( model == 0 ) {
		Warning( "target has no model \"%s\"\n", GetArg( 0 ) ) ;
		return false ;
	}

	//  check vars

	bool match_target = ( str_toupper( GetVar( "match_motion_target", "OFF" ) ) == "ON" ) ;
	bool match_basepose = ( str_toupper( GetVar( "match_motion_basepose", "OFF" ) ) == "ON" ) ;

	//  process

	for ( int i = 2 ; i < GetArgCount() ; i ++ ) {
		MdxBlock *block2 = GetBlock( GetArg( i ) ) ;
		if ( block2 == 0 ) {
			Error( "wrong source \"%s\"\n", GetArg( i ) ) ;
			return false ;
		}
		MdxBlocks models ;
		models.EnumTree( block2, MDX_MODEL ) ;
		for ( int j = 0 ; j < models.size() ; j ++ ) {
			MdxModel *model2 = (MdxModel *)models[ j ] ;
			MdxBlocks motions ;
			motions.EnumTree( model2, MDX_MOTION ) ;
			for ( int k = 0 ; k < motions.size() ; k ++ ) {
				MdxMotion *motion = (MdxMotion *)motions[ k ]->CloneTree() ;
				model->AttachChild( motion ) ;
				if ( match_target ) MatchTarget( motion, model, model2 ) ;
				if ( match_basepose ) MatchBasePose( motion, model, model2 ) ;
			}
		}
	}
	m_tmp.Release() ;
	return true ;
}

//----------------------------------------------------------------
//  match target
//----------------------------------------------------------------

void MergeMotions::MatchTarget( MdxMotion *motion, MdxModel *model, MdxModel *model2 )
{
	MdxBlocks anims ;
	anims.EnumChild( motion, MDX_ANIMATE ) ;
	for ( int i = 0 ; i < anims.size() ; i ++ ) {
		MdxAnimate *anim = (MdxAnimate *)anims[ i ] ;
		string name = anim->GetBlock() ;

		//  check target

		int rank ;
		bool is_digit = str_isdigit( name ) ;
		if ( is_digit ) {
			rank = MDX_REF_RANK( str_atoi( name ) ) ;
		} else if ( anim->GetBlockRef() == 0 ) {
			rank = MDX_REF_RANK( model2->RankOfChild( anim->GetScope(), name.c_str() ) ) ;
		} else {
			continue ;
		}
		if ( rank == MDX_REF_RANK_MASK ) {
			if ( !is_digit ) continue ;
			if ( model->GetChildCount( anim->GetScope() ) == 0 ) continue ; // motion only ?
			Warning( "missing target ( \"%s\" )\n", name.c_str() ) ;
			continue ;
		}

		//  correct target

		MdxChunk *target = model->FindChild( anim->GetScope(), rank ) ;
		if ( target == 0 ) {
			if ( is_digit ) continue ;
			string name2 = str_format( "0x%04x", MDX_REF_INDEX( 1, 0, rank ) ) ;
			anim->SetBlock( name2.c_str() ) ;
			if ( model->GetChildCount( anim->GetScope() ) == 0 ) continue ; // motion only ?
			Warning( "unsolved target ( \"%s\" -> \"%s\" )\n", name.c_str(), name2.c_str() ) ;
			continue ;
		}
		if ( !is_digit ) {
			Warning( "diffrent target ( \"%s\" -> \"%s\" )\n", name.c_str(), target->GetName() ) ;
		}
		anim->SetBlock( target->GetName() ) ;
	}
}

//----------------------------------------------------------------
//  match basepose
//----------------------------------------------------------------

void MergeMotions::MatchBasePose( MdxMotion *motion, MdxModel *model, MdxModel *model2 )
{
	MdxBlocks bones ;
	bones.EnumChild( model, MDX_BONE ) ;

	for ( int i = 0 ; i < bones.size() ; i ++ ) {
		MdxBone *bone = (MdxBone *)bones[ i ] ;
		MdxBone *bone2 = (MdxBone *)model2->FindChild( MDX_BONE, bone->GetName() ) ;
		if ( bone2 == 0 ) bone2 = (MdxBone *)model2->FindChild( MDX_BONE, i ) ;
		if ( bone2 == 0 || bone->EqualsTree( bone2 ) ) continue ;

		if ( bone->GetChildCount() == 0 || bone2->GetChildCount() == 0 ) continue ;	// anchor ?

		MatchParam( motion, bone, bone2, MDX_VISIBILITY ) ;
		MatchParam( motion, bone, bone2, MDX_MORPH_WEIGHTS, MDX_MORPH_INDEX ) ;
		MatchParam( motion, bone, bone2, MDX_TRANSLATE ) ;
		MatchParam( motion, bone, bone2, MDX_ROTATE_ZYX, MDX_ROTATE_YXZ, MDX_ROTATE ) ;
		// MatchParam( motion, bone, bone2, MDX_SCALE, MDX_SCALE_2, MDX_SCALE_3 ) ;
		MatchParam( motion, bone, bone2, MDX_SCALE ) ;
	}
}

//----------------------------------------------------------------
//  subroutines
//----------------------------------------------------------------

bool MergeMotions::MatchParam( MdxMotion *motion, MdxBlock *block, MdxBlock *block2, int type, int type2, int type3 )
{
	MdxCommand *cmd = FindChild( block, type, type2, type3 ) ;
	MdxCommand *cmd2 = FindChild( block2, type, type2, type3 ) ;
	if ( cmd == 0 && cmd2 == 0 ) return false ;

	if ( cmd == 0 ) cmd = GetTemplate( cmd2 ) ;
	if ( cmd2 == 0 ) cmd2 = GetTemplate( cmd ) ;
	if ( cmd->Equals( cmd2 ) ) return true ;
	if ( FindAnim( motion, block, type, type2, type3 ) ) return true ;

	Warning( "diffrent basepose ( \"%s\" %s )\n", block->GetName(), cmd2->GetTypeName() ) ;

	MdxAnimate *anim = new MdxAnimate ;
	MdxFCurve *fcurve = new MdxFCurve ;
	motion->AttachChild( anim ) ;
	motion->AttachChild( fcurve ) ;

	anim->SetScope( block->GetTypeID() ) ;
	anim->SetBlock( block->GetName() ) ;
	anim->SetCommand( cmd2->GetTypeID() ) ;
	anim->SetFCurve( fcurve->GetName() ) ;

	int start = ( cmd2->GetTypeID() != MDX_MORPH_WEIGHTS ) ? 0 : 1 ;
	int n_dims = cmd2->GetArgsCount() - start ;
	fcurve->SetFormat( MDX_FCURVE_CONSTANT ) ;
	fcurve->SetDimCount( n_dims ) ;
	fcurve->SetKeyFrameCount( 1 ) ;

	MdxKeyFrame &key = fcurve->GetKeyFrame( 0 ) ;
	key.SetFrame( 0.0f ) ;
	for ( int i = 0 ; i < n_dims ; i ++ ) {
		key.SetValue( i, cmd2->GetArgsFloat( start + i ) ) ;
	}
	return true ;
}

MdxAnimate *MergeMotions::FindAnim( MdxMotion *motion, MdxBlock *block, int type, int type2, int type3 )
{
	MdxBlocks anims ;
	anims.EnumReferrer( block, MDX_ANIMATE, motion ) ;
	for ( int i = 0 ; i < anims.size() ; i ++ ) {
		int t = ( (MdxAnimate *)anims[ i ] )->GetCommand() ;
		if ( t == type || t == type2 || t == type3 ) return (MdxAnimate *)anims[ i ] ;
	}
	return 0 ;
}

MdxCommand *MergeMotions::FindChild( MdxBlock *block, int type, int type2, int type3 )
{
	MdxCommand *cmd = (MdxCommand *)block->FindChild( type ) ;
	if ( cmd == 0 && type2 >= 0 ) cmd = (MdxCommand *)block->FindChild( type2 ) ;
	if ( cmd == 0 && type3 >= 0 ) cmd = (MdxCommand *)block->FindChild( type3 ) ;
	return cmd ;
}

MdxCommand *MergeMotions::GetTemplate( MdxCommand *cmd )
{
	int type = cmd->GetTypeID() ;
	if ( type == MDX_MORPH_WEIGHTS ) {
		int n_weights = ( (MdxMorphWeights *)cmd )->GetWeightCount() ;
		MdxMorphWeights *weights = new MdxMorphWeights ;
		weights->SetWeightCount( n_weights ) ;
		weights->SetWeight( 0, 1.0f ) ;
		m_tmp.push_back( weights ) ;
		return weights ;
	}
	return (MdxCommand *)m_format->GetTemplate( type ) ;
}

} // namespace mdx
