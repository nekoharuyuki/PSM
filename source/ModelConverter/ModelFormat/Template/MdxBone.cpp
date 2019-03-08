#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  Bone block
//----------------------------------------------------------------

MdxBone::MdxBone( const char *name )
{
	SetName( name ) ;
}

MdxBone::~MdxBone()
{
	;
}

MdxBone::MdxBone( const MdxBone &src )
{
	Copy( &src ) ;
}

MdxBone &MdxBone::operator=( const MdxBone &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxBone::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxBone::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  ParentBone command
//----------------------------------------------------------------

MdxParentBone::MdxParentBone( const char *bone )
{
	SetArgsDesc( 0, MDX_WORD_REF | MDX_BONE ) ;

	SetBone( bone ) ;
}

void MdxParentBone::SetBone( const char *bone )
{
	SetArgsString( 0, bone ) ;
}

const char *MdxParentBone::GetBone() const
{
	return GetArgsString( 0 ) ;
}

void MdxParentBone::SetBoneRef( const MdxBone *bone )
{
	SetArgsRef( 0, bone ) ;
}

MdxBone *MdxParentBone::GetBoneRef() const
{
	return (MdxBone *)GetArgsRef( 0 ) ;
}

//----------------------------------------------------------------
//  Visibility command
//----------------------------------------------------------------

MdxVisibility::MdxVisibility( int visibility )
{
	SetArgsDesc( 0, MDX_WORD_INT ) ;

	SetVisibility( visibility ) ;
}

void MdxVisibility::SetVisibility( int visibility )
{
	SetArgsInt( 0, visibility ) ;
}

int MdxVisibility::GetVisibility() const
{
	return GetArgsInt( 0 ) ;
}

//----------------------------------------------------------------
//  Pivot command
//----------------------------------------------------------------

MdxPivot::MdxPivot( const vec3 &pivot )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetPivot( pivot ) ;
}

void MdxPivot::SetPivot( const vec3 &pivot )
{
	SetArgsVec3( 0, pivot ) ;
}

vec3 MdxPivot::GetPivot() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  Translate command
//----------------------------------------------------------------

MdxTranslate::MdxTranslate( const vec3 &translate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetTranslate( translate ) ;
}

void MdxTranslate::SetTranslate( const vec3 &translate )
{
	SetArgsVec3( 0, translate ) ;
}

vec3 MdxTranslate::GetTranslate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  Rotate command
//----------------------------------------------------------------

MdxRotate::MdxRotate( const quat &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 3, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotate::SetRotate( const quat &rotate )
{
	SetArgsQuat( 0, rotate ) ;
}

quat MdxRotate::GetRotate() const
{
	return GetArgsQuat( 0 ) ;
}

//----------------------------------------------------------------
//  RotateXYZ command
//----------------------------------------------------------------

MdxRotateXYZ::MdxRotateXYZ( const vec3 &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotateXYZ::SetRotate( const vec3 &rotate )
{
	SetArgsVec3( 0, rotate ) ;
}

vec3 MdxRotateXYZ::GetRotate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  RotateYZX command
//----------------------------------------------------------------

MdxRotateYZX::MdxRotateYZX( const vec3 &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotateYZX::SetRotate( const vec3 &rotate )
{
	SetArgsVec3( 0, rotate ) ;
}

vec3 MdxRotateYZX::GetRotate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  RotateZXY command
//----------------------------------------------------------------

MdxRotateZXY::MdxRotateZXY( const vec3 &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotateZXY::SetRotate( const vec3 &rotate )
{
	SetArgsVec3( 0, rotate ) ;
}

vec3 MdxRotateZXY::GetRotate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  RotateXZY command
//----------------------------------------------------------------

MdxRotateXZY::MdxRotateXZY( const vec3 &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotateXZY::SetRotate( const vec3 &rotate )
{
	SetArgsVec3( 0, rotate ) ;
}

vec3 MdxRotateXZY::GetRotate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  RotateYXZ command
//----------------------------------------------------------------

MdxRotateYXZ::MdxRotateYXZ( const vec3 &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotateYXZ::SetRotate( const vec3 &rotate )
{
	SetArgsVec3( 0, rotate ) ;
}

vec3 MdxRotateYXZ::GetRotate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  RotateZYX command
//----------------------------------------------------------------

MdxRotateZYX::MdxRotateZYX( const vec3 &rotate )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetRotate( rotate ) ;
}

void MdxRotateZYX::SetRotate( const vec3 &rotate )
{
	SetArgsVec3( 0, rotate ) ;
}

vec3 MdxRotateZYX::GetRotate() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  Scale command
//----------------------------------------------------------------

MdxScale::MdxScale( const vec3 &scale )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;

	SetScale( scale ) ;
}

void MdxScale::SetScale( const vec3 &scale )
{
	SetArgsVec3( 0, scale ) ;
}

vec3 MdxScale::GetScale() const
{
	return GetArgsVec3( 0 ) ;
}

//----------------------------------------------------------------
//  BlendBone command
//----------------------------------------------------------------

MdxBlendBone::MdxBlendBone( const char *bone, const mat4 &offset )
{
	SetArgsDesc( 0, MDX_WORD_REF | MDX_BONE ) ;
	for ( int i = 0 ; i < 16 ; i ++ ) {
		if ( i % 4 == 0 ) {
			SetArgsDesc( i + 1, MDX_WORD_FLOAT | MDX_WORD_FMT_NEWLINE ) ;
		} else {
			SetArgsDesc( i + 1, MDX_WORD_FLOAT ) ;
		}
	}

	SetBone( bone ) ;
	SetOffset( offset ) ;
}

void MdxBlendBone::SetBone( const char *bone )
{
	SetArgsString( 0, bone ) ;
}

const char *MdxBlendBone::GetBone() const
{
	return GetArgsString( 0 ) ;
}

void MdxBlendBone::SetOffset( const mat4 &offset )
{
	SetArgsMat4( 1, offset ) ;
}

mat4 MdxBlendBone::GetOffset() const
{
	return GetArgsMat4( 1 ) ;
}

void MdxBlendBone::SetBoneRef( const MdxBone *bone )
{
	SetArgsRef( 0, bone ) ;
}

MdxBone *MdxBlendBone::GetBoneRef() const
{
	return (MdxBone *)GetArgsRef( 0 ) ;
}

//----------------------------------------------------------------
//  MorphIndex command
//----------------------------------------------------------------

MdxMorphIndex::MdxMorphIndex( float index )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;

	SetIndex( index ) ;
}

void MdxMorphIndex::SetIndex( float index )
{
	SetArgsFloat( 0, index ) ;
}

float MdxMorphIndex::GetIndex() const
{
	return GetArgsFloat( 0 ) ;
}

//----------------------------------------------------------------
//  MorphWeights command
//----------------------------------------------------------------

MdxMorphWeights::MdxMorphWeights( int n_weights, const float *weights )
{
	SetArgsDesc( 0, MDX_WORD_INT ) ;
	SetArgsDesc( -1, MDX_WORD_FLOAT ) ;

	SetWeightCount( n_weights ) ;
	if ( weights == 0 ) return ;
	for ( int i = 0 ; i < n_weights ; i ++ ) {
		SetWeight( i, weights[ i ] ) ;
	}
}

void MdxMorphWeights::SetArgsInt( int num, int val )
{
	if ( num == 0 ) {
		SetArgsCount( 1 + val ) ;
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxMorphWeights::SetWeightCount( int num )
{
	SetArgsInt( 0, num ) ;
}

void MdxMorphWeights::SetWeight( int num, float weight )
{
	SetArgsFloat( 1 + num, weight ) ;
}

int MdxMorphWeights::GetWeightCount() const
{
	return GetArgsInt( 0 ) ;
}

float MdxMorphWeights::GetWeight( int num ) const
{
	return GetArgsFloat( 1 + num ) ;
}

//----------------------------------------------------------------
//  DrawPart command
//----------------------------------------------------------------

MdxDrawPart::MdxDrawPart( const char *part )
{
	SetArgsDesc( 0, MDX_WORD_REF | MDX_PART ) ;

	SetPart( part ) ;
}

void MdxDrawPart::SetPart( const char *part )
{
	SetArgsString( 0, part ) ;
}

const char *MdxDrawPart::GetPart() const
{
	return GetArgsString( 0 ) ;
}

void MdxDrawPart::SetPartRef( const MdxPart *part )
{
	SetArgsRef( 0, part ) ;
}

MdxPart *MdxDrawPart::GetPartRef() const
{
	return (MdxPart *)GetArgsRef( 0 ) ;
}


} // namespace mdx
