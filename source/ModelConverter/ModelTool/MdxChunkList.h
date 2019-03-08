#ifndef MDX_CHUNKS_H_INCLUDE
#define MDX_CHUNKS_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxChunkList
//----------------------------------------------------------------

template<class T>
class MdxChunkList : public vector<T *> {
public:
	//  override

	int size() const {
		return vector<T *>::size() ;
	}

	//  enumeration

	int EnumChild( const T *chunk, int type = 0 ) {
		if ( chunk == 0 ) return 0 ;
		int size_from = size() ;
		int count = chunk->GetChildCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			T *child = (T *)chunk->GetChild( i ) ;
			if ( type == 0 || child->GetTypeID() == type ) {
				push_back( child ) ;
			}
		}
		return size() - size_from ;
	}
	int EnumTree( const T *chunk, int type = 0 ) {
		if ( chunk == 0 ) return 0 ;
		int size_from = size() ;
		int count = chunk->GetChildCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			T *child = (T *)chunk->GetChild( i ) ;
			EnumTree( child, type ) ;
		}
		if ( type == 0 || chunk->GetTypeID() == type ) {
			push_back( (T *)chunk ) ;	// post-order, count itself
		}
		return size() - size_from ;
	}
	int EnumReferrer( const T *chunk, int type = 0, const T *parent = 0 ) {
		if ( chunk == 0 ) return 0 ;
		if ( parent == 0 ) parent = (const T *)chunk->GetParent() ;
		if ( parent == 0 ) parent = chunk ;
		int size_from = size() ;
		int count = parent->GetChildCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			T *child = (T *)parent->GetChild( i ) ;
			EnumReferrer( chunk, type, child ) ;
			if ( type != 0 && child->GetTypeID() != type ) continue ;
			if ( child->RefersTo( chunk ) ) {
				push_back( child ) ;
			}
		}
		return size() - size_from ;
	}

	//  invoking

	void AddRef() {
		MdxChunkList<T> &it = *this ;
		for ( int i = 0 ; i < (int)size() ; i ++ ) it[ i ]->AddRef() ;
	}
	void Release() {
		MdxChunkList<T> &it = *this ;
		for ( int i = 0 ; i < (int)size() ; i ++ ) it[ i ]->Release() ;
	}
	bool Equals( const MdxChunkList<T> &src ) const {
		if ( size() != src.size() ) return false ;
		const MdxChunkList<T> &it = *this ;
		for ( int i = 0 ; i < (int)size() ; i ++ ) {
			if ( !it[ i ]->Equals( src[ i ] ) ) return false ;
		}
		return true ;
	}
	void AttachTo( T *parent ) {
		if ( parent == 0 ) return ;
		MdxChunkList<T> &it = *this ;
		for ( int i = 0 ; i < (int)size() ; i ++ ) {
			parent->AttachChild( it[ i ] ) ;
		}
	}
	void Detach() {
		MdxChunkList<T> &it = *this ;
		for ( int i = 0 ; i < (int)size() ; i ++ ) {
			T *chunk = it[ i ] ;
			T *parent = (T *)chunk->GetParent() ;
			if ( parent != 0 ) parent->DetachChild( chunk ) ;
		}
	}
} ;


} // namespace mdx

#endif
