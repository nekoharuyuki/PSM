#ifndef	MDX_BASE_H_INCLUDE
#define	MDX_BASE_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxBase
//----------------------------------------------------------------

class MdxBase {
public:
	MdxBase() ;
	virtual ~MdxBase() ;
	MdxBase( const MdxBase &src ) ;
	MdxBase &operator=( const MdxBase &src ) ;
	int AddRef() ;
	int Release() ;

	//  new and delete

	static void *operator new( size_t size ) ;
	static void operator delete( void *addr ) ;
	static void *operator new[]( size_t size ) ;
	static void operator delete[]( void *addr ) ;
	static void *operator new( size_t, void *addr ) ;
	static void operator delete( void *, void * ) ;
	static void *operator new[]( size_t, void *addr ) ;
	static void operator delete[]( void *, void * ) ;

private:
	class impl ;
	impl *m_impl ;
} ;


} // namespace mdx

#endif
