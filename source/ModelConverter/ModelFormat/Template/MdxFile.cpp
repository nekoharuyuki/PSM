#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  File block
//----------------------------------------------------------------

MdxFile::MdxFile( const char *name )
{
	SetName( name ) ;
}

MdxFile::~MdxFile()
{
	;
}

MdxFile::MdxFile( const MdxFile &src )
{
	Copy( &src ) ;
}

MdxFile &MdxFile::operator=( const MdxFile &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxFile::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxFile::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  DefineEnum command
//----------------------------------------------------------------

MdxDefineEnum::MdxDefineEnum( const char *scope, const char *name, int value )
{
	SetArgsDesc( 0, MDX_WORD_STRING ) ;
	SetArgsDesc( 1, MDX_WORD_STRING ) ;
	SetArgsDesc( 2, MDX_WORD_INT ) ;

	SetDefScope( scope ) ;
	SetDefName( name ) ;
	SetDefValue( value ) ;
}

void MdxDefineEnum::SetDefScope( const char *scope )
{
	SetArgsString( 0, scope ) ;
}

void MdxDefineEnum::SetDefName( const char *name )
{
	SetArgsString( 1, name ) ;
}

void MdxDefineEnum::SetDefValue( int value )
{
	SetArgsInt( 2, value ) ;
}

const char *MdxDefineEnum::GetDefScope() const
{
	return GetArgsString( 0 ) ;
}

const char *MdxDefineEnum::GetDefName() const
{
	return GetArgsString( 1 ) ;
}

int MdxDefineEnum::GetDefValue() const
{
	return GetArgsInt( 2 ) ;
}

//----------------------------------------------------------------
//  DefineBlock command
//----------------------------------------------------------------

MdxDefineBlock::MdxDefineBlock( int id, const char *name, int argc, const int *descs )
{
	SetArgsDesc( 0, MDX_WORD_UINT16 | MDX_WORD_FMT_HEX ) ;
	SetArgsDesc( 1, MDX_WORD_STRING ) ;
	SetArgsDesc( -1, MDX_WORD_ENUM | MDX_SCOPE_WORD_TYPE | MDX_WORD_VAR_COUNT ) ;

	SetDefTypeID( id ) ;
	SetDefTypeName( name ) ;
	SetDefArgsCount( argc ) ;
	if ( descs == 0 ) return ;
	for ( int i = 0 ; i < argc ; i ++ ) {
		SetDefArgsDesc( i, descs[ i ] ) ;
	}
}

void MdxDefineBlock::SetDefTypeID( int id )
{
	SetArgsInt( 0, id ) ;
}

void MdxDefineBlock::SetDefTypeName( const char *name )
{
	SetArgsString( 1, name ) ;
}

void MdxDefineBlock::SetDefArgsCount( int num )
{
	SetArgsCount( 2 + num ) ;
}

void MdxDefineBlock::SetDefArgsDesc( int num, int desc )
{
	SetArgsInt( 2 + num, desc ) ;
}

int MdxDefineBlock::GetDefTypeID() const
{
	return GetArgsInt( 0 ) ;
}

const char *MdxDefineBlock::GetDefTypeName() const
{
	return GetArgsString( 1 ) ;
}

int MdxDefineBlock::GetDefArgsCount() const
{
	return GetArgsCount() - 2 ;
}

int MdxDefineBlock::GetDefArgsDesc( int num ) const
{
	return GetArgsInt( 2 + num ) ;
}

//----------------------------------------------------------------
//  DefineCommand command
//----------------------------------------------------------------

MdxDefineCommand::MdxDefineCommand( int id, const char *name, int argc, const int *descs )
{
	SetArgsDesc( 0, MDX_WORD_UINT16 | MDX_WORD_FMT_HEX ) ;
	SetArgsDesc( 1, MDX_WORD_STRING ) ;
	SetArgsDesc( -1, MDX_WORD_ENUM | MDX_SCOPE_WORD_TYPE | MDX_WORD_VAR_COUNT ) ;

	SetDefTypeID( id ) ;
	SetDefTypeName( name ) ;
	SetDefArgsCount( argc ) ;
	if ( descs == 0 ) return ;
	for ( int i = 0 ; i < argc ; i ++ ) {
		SetDefArgsDesc( i, descs[ i ] ) ;
	}
}

void MdxDefineCommand::SetDefTypeID( int id )
{
	SetArgsInt( 0, id ) ;
}

void MdxDefineCommand::SetDefTypeName( const char *name )
{
	SetArgsString( 1, name ) ;
}

void MdxDefineCommand::SetDefArgsCount( int num )
{
	SetArgsCount( 2 + num ) ;
}

void MdxDefineCommand::SetDefArgsDesc( int num, int desc )
{
	SetArgsInt( 2 + num, desc ) ;
}

int MdxDefineCommand::GetDefTypeID() const
{
	return GetArgsInt( 0 ) ;
}

const char *MdxDefineCommand::GetDefTypeName() const
{
	return GetArgsString( 1 ) ;
}

int MdxDefineCommand::GetDefArgsCount() const
{
	return GetArgsCount() - 2 ;
}

int MdxDefineCommand::GetDefArgsDesc( int num ) const
{
	return GetArgsInt( 2 + num ) ;
}


} // namespace mdx
