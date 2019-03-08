#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  Part block
//----------------------------------------------------------------

MdxPart::MdxPart( const char *name )
{
	SetName( name ) ;
}

MdxPart::~MdxPart()
{
	;
}

MdxPart::MdxPart( const MdxPart &src )
{
	Copy( &src ) ;
}

MdxPart &MdxPart::operator=( const MdxPart &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxPart::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxPart::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  Mesh block
//----------------------------------------------------------------

MdxMesh::MdxMesh( const char *name )
{
	SetName( name ) ;
}

MdxMesh::~MdxMesh()
{
	;
}

MdxMesh::MdxMesh( const MdxMesh &src )
{
	Copy( &src ) ;
}

MdxMesh &MdxMesh::operator=( const MdxMesh &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxMesh::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxMesh::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  VertexOffset command
//----------------------------------------------------------------

MdxVertexOffset::MdxVertexOffset( int format, const float *offsets )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_ARRAYS_FORMAT ) ;
	SetArgsDesc( -1, MDX_WORD_FLOAT ) ;

	SetFormat( format ) ;
	if ( offsets == 0 ) return ;
	int count = GetArgsCount() - 1 ;
	for ( int i = 0 ; i < count ; i ++ ) {
		SetOffset( i, offsets[ i ] ) ;
	}
}

void MdxVertexOffset::SetArgsInt( int num, int val )
{
	if ( num == 0 ) {
		SetArgsCount( 1 + MdxVertex::GetFormatStride( val ) * 2 ) ;
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxVertexOffset::SetFormat( int format )
{
	SetArgsInt( 0, format ) ;
}

void MdxVertexOffset::SetOffset( int num, float offset )
{
	SetArgsFloat( 1 + num, offset ) ;
}

int MdxVertexOffset::GetFormat() const
{
	return GetArgsInt( 0 ) ;
}

float MdxVertexOffset::GetOffset( int num ) const
{
	return GetArgsFloat( 1 + num ) ;
}

//----------------------------------------------------------------
//  SetMaterial command
//----------------------------------------------------------------

MdxSetMaterial::MdxSetMaterial( const char *material )
{
	SetArgsDesc( 0, MDX_WORD_REF | MDX_MATERIAL ) ;

	SetMaterial( material ) ;
}

void MdxSetMaterial::SetMaterial( const char *material )
{
	SetArgsString( 0, material ) ;
}

const char *MdxSetMaterial::GetMaterial() const
{
	return GetArgsString( 0 ) ;
}

void MdxSetMaterial::SetMaterialRef( const MdxMaterial *material )
{
	SetArgsRef( 0, material ) ;
}

MdxMaterial *MdxSetMaterial::GetMaterialRef() const
{
	return (MdxMaterial *)GetArgsRef( 0 ) ;
}

//----------------------------------------------------------------
//  SetArrays command
//----------------------------------------------------------------

MdxSetArrays::MdxSetArrays( const char *arrays )
{
	SetArgsDesc( 0, MDX_WORD_REF | MDX_ARRAYS ) ;

	SetArrays( arrays ) ;
}

void MdxSetArrays::SetArrays( const char *arrays )
{
	SetArgsString( 0, arrays ) ;
}

const char *MdxSetArrays::GetArrays() const
{
	return GetArgsString( 0 ) ;
}

void MdxSetArrays::SetArraysRef( const MdxArrays *arrays )
{
	SetArgsRef( 0, arrays ) ;
}

MdxArrays *MdxSetArrays::GetArraysRef() const
{
	return (MdxArrays *)GetArgsRef( 0 ) ;
}

//----------------------------------------------------------------
//  BlendIndices command
//----------------------------------------------------------------

MdxBlendIndices::MdxBlendIndices( int n_indices, const int *indices )
{
	SetArgsDesc( 0, MDX_WORD_INT ) ;
	SetArgsDesc( -1, MDX_WORD_INT ) ;

	SetIndexCount( n_indices ) ;
	if ( indices == 0 ) return ;
	for ( int i = 0 ; i < n_indices ; i ++ ) {
		SetIndex( i, indices[ i ] ) ;
	}
}

void MdxBlendIndices::SetArgsInt( int num, int val )
{
	if ( num == 0 ) {
		SetArgsCount( 1 + val ) ;
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxBlendIndices::SetIndexCount( int num )
{
	SetArgsInt( 0, num ) ;
}

void MdxBlendIndices::SetIndex( int num, int index )
{
	SetArgsInt( 1 + num, index ) ;
}

int MdxBlendIndices::GetIndexCount() const
{
	return GetArgsInt( 0 ) ;
}

int MdxBlendIndices::GetIndex( int num ) const
{
	return GetArgsInt( 1 + num ) ;
}

//----------------------------------------------------------------
//  Subdivision command
//----------------------------------------------------------------

MdxSubdivision::MdxSubdivision( float div_u, float div_v )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;

	SetDivU( div_u ) ;
	SetDivV( div_v ) ;
}

void MdxSubdivision::SetDivU( float div_u )
{
	SetArgsFloat( 0, div_u ) ;
}

void MdxSubdivision::SetDivV( float div_v )
{
	SetArgsFloat( 1, div_v ) ;
}

float MdxSubdivision::GetDivU() const
{
	return GetArgsFloat( 0 ) ;
}

float MdxSubdivision::GetDivV() const
{
	return GetArgsFloat( 1 ) ;
}

//----------------------------------------------------------------
//  KnotVectorU command
//----------------------------------------------------------------

MdxKnotVectorU::MdxKnotVectorU( int n_knots, const float *knots )
{
	SetArgsDesc( 0, MDX_WORD_INT ) ;
	SetArgsDesc( -1, MDX_WORD_FLOAT ) ;

	SetKnotCount( n_knots ) ;
	if ( knots == 0 ) return ;
	for ( int i = 0 ; i < n_knots ; i ++ ) {
		SetKnot( i, knots[ i ] ) ;
	}
}

void MdxKnotVectorU::SetArgsInt( int num, int val )
{
	if ( num == 0 ) {
		SetArgsCount( 1 + val ) ;
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxKnotVectorU::SetKnotCount( int num )
{
	SetArgsInt( 0, num ) ;
}

void MdxKnotVectorU::SetKnot( int num, float knot )
{
	SetArgsFloat( 1 + num, knot ) ;
}

int MdxKnotVectorU::GetKnotCount() const
{
	return GetArgsInt( 0 ) ;
}

float MdxKnotVectorU::GetKnot( int num ) const
{
	return GetArgsFloat( 1 + num ) ;
}

//----------------------------------------------------------------
//  KnotVectorV command
//----------------------------------------------------------------

MdxKnotVectorV::MdxKnotVectorV( int n_knots, const float *knots )
{
	SetArgsDesc( 0, MDX_WORD_INT ) ;
	SetArgsDesc( -1, MDX_WORD_FLOAT ) ;

	SetKnotCount( n_knots ) ;
	if ( knots == 0 ) return ;
	for ( int i = 0 ; i < n_knots ; i ++ ) {
		SetKnot( i, knots[ i ] ) ;
	}
}

void MdxKnotVectorV::SetArgsInt( int num, int val )
{
	if ( num == 0 ) {
		SetArgsCount( 1 + val ) ;
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxKnotVectorV::SetKnotCount( int num )
{
	SetArgsInt( 0, num ) ;
}

void MdxKnotVectorV::SetKnot( int num, float knot )
{
	SetArgsFloat( 1 + num, knot ) ;
}

int MdxKnotVectorV::GetKnotCount() const
{
	return GetArgsInt( 0 ) ;
}

float MdxKnotVectorV::GetKnot( int num ) const
{
	return GetArgsFloat( 1 + num ) ;
}

//----------------------------------------------------------------
//  Base primitive command
//----------------------------------------------------------------

MdxPrimCommand::MdxPrimCommand( int mode, int n_verts, int n_prims, const int *indices )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_PRIM_MODE ) ;
	SetArgsDesc( 1, MDX_WORD_INT ) ;
	SetArgsDesc( 2, MDX_WORD_INT ) ;
	SetArgsDesc( -1, MDX_WORD_UINT16 ) ;

	SetMode( mode ) ;
	SetVertexCount( n_verts ) ;
	SetPrimCount( n_prims ) ;
	if ( indices == 0 ) return ;
	int count = n_verts * n_prims ;
	for ( int i = 0 ; i < count ; i ++ ) {
		SetIndex( i, indices[ i ] ) ;
	}
}

void MdxPrimCommand::SetArgsInt( int num, int val )
{
	if ( num == 1 || num == 2 ) {
		int val2 = GetArgsInt( 3 - num ) ;
		SetArgsCount( 3 + val * val2 ) ;
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxPrimCommand::SetMode( int mode )
{
	SetArgsInt( 0, mode ) ;
}

void MdxPrimCommand::SetVertexCount( int n_verts )
{
	SetArgsInt( 1, n_verts ) ;
}

void MdxPrimCommand::SetPrimCount( int n_prims )
{
	SetArgsInt( 2, n_prims ) ;
}

void MdxPrimCommand::SetIndex( int num, int index )
{
	SetArgsInt( 3 + num, index ) ;
}

int MdxPrimCommand::GetMode() const
{
	return GetArgsInt( 0 ) ;
}

int MdxPrimCommand::GetVertexCount() const
{
	return GetArgsInt( 1 ) ;
}

int MdxPrimCommand::GetPrimCount() const
{
	return GetArgsInt( 2 ) ;
}

int MdxPrimCommand::GetIndex( int num ) const
{
	return GetArgsInt( 3 + num ) ;
}

void MdxPrimCommand::SetSequentialIndices( int start )
{
	int count = GetVertexCount() * GetPrimCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		SetIndex( i, start + i ) ;
	}
}

bool MdxPrimCommand::HasSequentialIndices() const
{
	int start = GetIndex( 0 ) ;
	int count = GetVertexCount() * GetPrimCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		if ( GetIndex( i ) != start + i ) return false ;
	}
	return true ;
}

//----------------------------------------------------------------
//  DrawArrays command
//----------------------------------------------------------------

MdxDrawArrays::MdxDrawArrays( int mode, int n_verts, int n_prims, const int *indices )
: MdxPrimCommand( mode, n_verts, n_prims, indices )
{
	;
}

//----------------------------------------------------------------
//  DrawBSpline command
//----------------------------------------------------------------

MdxDrawBSpline::MdxDrawBSpline( int mode, int width, int height, const int *indices )
: MdxPrimCommand( mode, width, height, indices )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_B_SPLINE_MODE ) ;
}


} // namespace mdx
