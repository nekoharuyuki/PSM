#include "ModelTool.h"

namespace mdx {

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

class MdxBase::impl {
public:
	int m_ref_count ;
} ;

//----------------------------------------------------------------
//  MdxBase
//----------------------------------------------------------------

MdxBase::MdxBase()
{
	m_impl = new impl ;
	m_impl->m_ref_count = 1 ;
}

MdxBase::~MdxBase()
{
	delete m_impl ;
}

MdxBase::MdxBase( const MdxBase &src )
{
	m_impl = new impl ;
	m_impl->m_ref_count = 1 ;
	*this = src ;
}

MdxBase &MdxBase::operator=( const MdxBase &src )
{
	return *this ;
}

int MdxBase::AddRef()
{
	if ( this == 0 ) return 0 ;
	int count = m_impl->m_ref_count ;
	if ( count > 0 ) {
		m_impl->m_ref_count = count + 1 ;
	}
	return count ;
}

int MdxBase::Release()
{
	if ( this == 0 ) return 0 ;
	int count = m_impl->m_ref_count - 1 ;
	if ( count >= 0 ) {
		m_impl->m_ref_count = count ;
		if ( count == 0 ) delete this ;
	}
	return count ;
}

//----------------------------------------------------------------
//  new and delete
//----------------------------------------------------------------

void *MdxBase::operator new( size_t size )
{
	return malloc( size ) ;
}

void MdxBase::operator delete( void *addr )
{
	free( addr ) ;
}

void *MdxBase::operator new[]( size_t size )
{
	return malloc( size ) ;
}

void MdxBase::operator delete[]( void *addr )
{
	free( addr ) ;
}

void *MdxBase::operator new( size_t, void *addr )
{
	return addr ;
}

void MdxBase::operator delete( void *, void * )
{
	;
}

void *MdxBase::operator new[]( size_t, void *addr )
{
	return addr ;
}

void MdxBase::operator delete[]( void *, void * )
{
	;
}


} // namespace mdx
