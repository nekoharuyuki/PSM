#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

class MdxUnknownBlock::impl {
public:
	int m_type_id ;
	string m_type_name ;
} ;

class MdxUnknownCommand::impl {
public:
	int m_type_id ;
	string m_type_name ;
} ;

//----------------------------------------------------------------
//  BlindBlock block
//----------------------------------------------------------------

MdxBlindBlock::MdxBlindBlock( const char *name )
{
	SetName( name ) ;
}

MdxBlindBlock::~MdxBlindBlock()
{
	;
}

MdxBlindBlock::MdxBlindBlock( const MdxBlindBlock &src )
{
	Copy( &src ) ;
}

MdxBlindBlock &MdxBlindBlock::operator=( const MdxBlindBlock &src )
{
	Copy( &src ) ;
	return *this ;
}

MdxChunk *MdxBlindBlock::Copy( const MdxChunk *src )
{
	return MdxBlock::Copy( src ) ;
}

bool MdxBlindBlock::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  BlindData command
//----------------------------------------------------------------

MdxBlindData::MdxBlindData()
{
	SetArgsDesc( 0, MDX_WORD_STRING ) ;
	SetArgsDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
}

//----------------------------------------------------------------
//  unknown block
//----------------------------------------------------------------

MdxUnknownBlock::MdxUnknownBlock( int type_id, const char *type_name, const char *name )
{
	if ( type_name == 0 ) type_name = "Unknown" ;

	m_impl = new impl ;
	m_impl->m_type_id = type_id ;
	m_impl->m_type_name = type_name ;

	SetName( name ) ;
	SetArgsDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
	SetDataDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
}

MdxUnknownBlock::~MdxUnknownBlock()
{
	delete m_impl ;
}

MdxUnknownBlock::MdxUnknownBlock( const MdxUnknownBlock &src )
{
	m_impl = new impl ;
	SetArgsDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
	SetDataDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
	Copy( &src ) ;
}

MdxUnknownBlock &MdxUnknownBlock::operator=( const MdxUnknownBlock &src )
{
	Copy( &src ) ;
	return *this ;
}

int MdxUnknownBlock::GetTypeID() const
{
	return m_impl->m_type_id ;
}

const char *MdxUnknownBlock::GetTypeName() const
{
	return m_impl->m_type_name.c_str() ;
}

MdxChunk *MdxUnknownBlock::Clone() const
{
	return new MdxUnknownBlock( *this ) ;
}

MdxChunk *MdxUnknownBlock::Copy( const MdxChunk *src )
{
	const MdxUnknownBlock *src2 = dynamic_cast<const MdxUnknownBlock *>( src ) ;
	if ( src2 == this ) return this ;
	if ( src2 == 0 ) return 0 ;

	MdxBlock::Copy( src2 ) ;
	*m_impl = *( src2->m_impl ) ;
	return this ;
}

bool MdxUnknownBlock::Equals( const MdxChunk *src ) const
{
	return MdxBlock::Equals( src ) ;
}

//----------------------------------------------------------------
//  unknown command
//----------------------------------------------------------------

MdxUnknownCommand::MdxUnknownCommand( int type_id, const char *type_name )
{
	if ( type_name == 0 ) type_name = "Unknown" ;

	m_impl = new impl ;
	m_impl->m_type_id = type_id ;
	m_impl->m_type_name = type_name ;

	SetArgsDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
}

MdxUnknownCommand::~MdxUnknownCommand()
{
	delete m_impl ;
}

MdxUnknownCommand::MdxUnknownCommand( const MdxUnknownCommand &src )
{
	m_impl = new impl ;
	SetArgsDesc( -1, MDX_WORD_VAR_TYPE | MDX_WORD_VAR_COUNT ) ;
	Copy( &src ) ;
}

MdxUnknownCommand &MdxUnknownCommand::operator=( const MdxUnknownCommand &src )
{
	Copy( &src ) ;
	return *this ;
}

int MdxUnknownCommand::GetTypeID() const
{
	return m_impl->m_type_id ;
}

const char *MdxUnknownCommand::GetTypeName() const
{
	return m_impl->m_type_name.c_str() ;
}

MdxChunk *MdxUnknownCommand::Clone() const
{
	return new MdxUnknownCommand( *this ) ;
}

MdxChunk *MdxUnknownCommand::Copy( const MdxChunk *src )
{
	const MdxUnknownCommand *src2 = dynamic_cast<const MdxUnknownCommand *>( src ) ;
	if ( src2 == this ) return this ;
	if ( src2 == 0 ) return 0 ;

	MdxCommand::Copy( src2 ) ;
	*m_impl = *( src2->m_impl ) ;
	return this ;
}

bool MdxUnknownCommand::Equals( const MdxChunk *src ) const
{
	return MdxCommand::Equals( src ) ;
}


} // namespace mdx
