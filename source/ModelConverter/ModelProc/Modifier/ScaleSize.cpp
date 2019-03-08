#include "ScaleSize.h"

namespace mdx {

static mat4 scale_mat4_trans( const mat4 &m, float scale ) ;
static int check_anim_type( MdxFCurve *fcurve ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF" ;
enum {
	MODE_OFF,
	MODE_NUMERIC,
} ;

//----------------------------------------------------------------
//  ScaleSize
//----------------------------------------------------------------

bool ScaleSize::Modify( MdxBlock *block )
{
	//  check params

	int mode ;
	float scale ;

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	if ( str_isdigit( arg ) ) {
		mode = MODE_NUMERIC ;
		scale = str_atof( arg ) ;
		if ( scale == 1.0f ) return true ;
	} else {
		mode = str_search( ModeNames, arg ) ;
		scale = 1.0f ;
		if ( mode < 0 ) {
			Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		if ( mode == MODE_OFF ) return true ;
	}

	//  process

	int i, j, k ;

	MdxBlocks blocks ;
	blocks.EnumTree( block, MDX_BOUNDING_BOX ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxBoundingBox *cmd = (MdxBoundingBox *)blocks[ i ] ;
		cmd->SetLower( cmd->GetLower() * scale ) ;
		cmd->SetUpper( cmd->GetUpper() * scale ) ;
	}
	blocks.clear() ;
	blocks.EnumTree( block, MDX_BOUNDING_SPHERE ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxBoundingSphere *cmd = (MdxBoundingSphere *)blocks[ i ] ;
		cmd->SetCenter( cmd->GetCenter() * scale ) ;
		cmd->SetRadius( cmd->GetRadius() * scale ) ;
	}
	blocks.clear() ;
	blocks.EnumTree( block, MDX_BLEND_BONE ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxBlendBone *cmd = (MdxBlendBone *)blocks[ i ] ;
		cmd->SetOffset( scale_mat4_trans( cmd->GetOffset(), scale ) ) ;
	}
	blocks.clear() ;
	blocks.EnumTree( block, MDX_PIVOT ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxPivot *cmd = (MdxPivot *)blocks[ i ] ;
		cmd->SetPivot( cmd->GetPivot() * scale ) ;
	}
	blocks.clear() ;
	blocks.EnumTree( block, MDX_TRANSLATE ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxTranslate *cmd = (MdxTranslate *)blocks[ i ] ;
		cmd->SetTranslate( cmd->GetTranslate() * scale ) ;
	}
	blocks.clear() ;
	blocks.EnumTree( block, MDX_ARRAYS ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxArrays *arrays = (MdxArrays *)blocks[ i ] ;
		if ( arrays->GetVertexPositionCount() ) {
			for ( k = 0 ; k < arrays->GetVertexCount() ; k ++ ) {
				MdxVertex &v = arrays->GetVertex( k ) ;
				v.SetPosition( v.GetPosition() * scale ) ;
			}
		}
	}
	blocks.clear() ;
	blocks.EnumTree( block, MDX_FCURVE ) ;
	for ( i = 0 ; i < blocks.size() ; i ++ ) {
		MdxFCurve *fcurve = (MdxFCurve *)blocks[ i ] ;
		switch ( check_anim_type( fcurve ) ) {
		    case MDX_TRANSLATE :	break ;
		    case MDX_PIVOT :		break ;
		    default :			continue ;
		}
		for ( j = 0 ; j < fcurve->GetKeyFrameCount() ; j ++ ) {
			MdxKeyFrame &key = fcurve->GetKeyFrame( j ) ;
			for ( k = 0 ; k < 3 ; k ++ ) {
				key.SetValue( k, key.GetValue( k ) * scale ) ;
				key.SetInDY( k, key.GetInDY( k ) * scale ) ;
				key.SetOutDY( k, key.GetOutDY( k ) * scale ) ;
			}
		}
	}
	return true ;
}

//----------------------------------------------------------------
//  subroutines
//----------------------------------------------------------------

mat4 scale_mat4_trans( const mat4 &m, float scale )
{
	mat4 m2 = m ;
	m2.w.x *= scale ;
	m2.w.y *= scale ;
	m2.w.z *= scale ;
	return m2 ;
}

int check_anim_type( MdxFCurve *fcurve )
{
	MdxMotion *motion = (MdxMotion *)fcurve->GetParent() ;
	if ( motion == 0 ) return 0 ;

	string name = fcurve->GetName() ;

	MdxBlocks anims ;
	anims.EnumChild( motion, MDX_ANIMATE ) ;
	for ( int i = 0 ; i < anims.size() ; i ++ ) {
		MdxAnimate *anim = (MdxAnimate *)anims[ i ] ;
		if ( name == anim->GetFCurve() ) return anim->GetCommand() ;
	}
	return 0 ;
}


} // namespace mdx
