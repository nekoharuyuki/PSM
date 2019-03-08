#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  Model block
//----------------------------------------------------------------

MdxModel::MdxModel( const char *name )
{
	SetName( name ) ;
}

MdxModel::~MdxModel()
{
	;
}

MdxModel::MdxModel( const MdxModel &src )
{
	Copy( &src ) ;
}

MdxModel &MdxModel::operator=( const MdxModel &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxModel::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxModel::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  FileName command
//----------------------------------------------------------------

MdxFileName::MdxFileName( const char *filename )
{
	SetArgsDesc( 0, MDX_WORD_STRING ) ;

	SetFileName( filename ) ;
}

void MdxFileName::SetFileName( const char *filename )
{
	SetArgsString( 0, filename ) ;
}

const char *MdxFileName::GetFileName() const
{
	return GetArgsString( 0 ) ;
}

//----------------------------------------------------------------
//  FileImage command
//----------------------------------------------------------------

MdxFileImage::MdxFileImage( const void *buf, int size )
{
	SetArgsDesc( 0, MDX_WORD_INT ) ;
	SetArgsDesc( -1, MDX_WORD_UINT32 | MDX_WORD_FMT_HEX ) ;

	SetFileImage( buf, size ) ;
}

void MdxFileImage::SetArgsInt( int num, int val )
{
	if ( num == 0 ) {
		int count = ( val + 3 ) / 4 ;
		SetArgsCount( 1 + count ) ;
		for ( int i = 0 ; i < count ; i += 4 ) {
			SetArgsDesc( 1 + i, MDX_WORD_UINT32 | MDX_WORD_FMT_HEX | MDX_WORD_FMT_NEWLINE ) ;
		}
	}
	MdxChunk::SetArgsInt( num, val ) ;
}

void MdxFileImage::SetFileImage( const void *buf, int size )
{
	if ( size < 0 ) size = 0 ;
	int size2 = ( size + 3 ) / 4 * 4 + 4 ;
	int *buf2 = (int *)malloc( size2 ) ;
	buf2[ size2 / 4 - 1 ] = 0 ;
	buf2[ 0 ] = size ;
	if ( buf != 0 ) memcpy( buf2 + 1, buf, size ) ;
	SetArgsImage( buf2, size2 ) ;
	free( buf2 ) ;
}

void MdxFileImage::GetFileImage( const void *&buf, int &size ) const
{
	GetArgsImage( buf, size ) ;
	buf = (int *)buf + 1 ;
	size = GetArgsInt( 0 ) ;
}

//----------------------------------------------------------------
//  BoundingBox command
//----------------------------------------------------------------

MdxBoundingBox::MdxBoundingBox( const vec3 &lower, const vec3 &upper )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 3, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 4, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 5, MDX_WORD_FLOAT ) ;

	SetLower( lower ) ;
	SetUpper( upper ) ;
}

void MdxBoundingBox::SetLower( const vec3 &lower )
{
	SetArgsVec3( 0, lower ) ;
}

void MdxBoundingBox::SetUpper( const vec3 &upper )
{
	SetArgsVec3( 3, upper ) ;
}

vec3 MdxBoundingBox::GetLower() const
{
	return GetArgsVec3( 0 ) ;
}

vec3 MdxBoundingBox::GetUpper() const
{
	return GetArgsVec3( 3 ) ;
}

//----------------------------------------------------------------
//  BoundingSphere command
//----------------------------------------------------------------

MdxBoundingSphere::MdxBoundingSphere( const vec3 &center, float radius )
{
	SetArgsDesc( 0, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 1, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 2, MDX_WORD_FLOAT ) ;
	SetArgsDesc( 3, MDX_WORD_FLOAT ) ;

	SetCenter( center ) ;
	SetRadius( radius ) ;
}

void MdxBoundingSphere::SetCenter( const vec3 &center )
{
	SetArgsVec3( 0, center ) ;
}

void MdxBoundingSphere::SetRadius( float radius )
{
	SetArgsFloat( 3, radius ) ;
}

vec3 MdxBoundingSphere::GetCenter() const
{
	return GetArgsVec3( 0 ) ;
}

float MdxBoundingSphere::GetRadius() const
{
	return GetArgsFloat( 3 ) ;
}


} // namespace mdx
