#ifndef	MDX_UNKNOWN_H_INCLUDE
#define MDX_UNKNOWN_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  BlindBlock block
//----------------------------------------------------------------

class MdxBlindBlock : public MdxBlock {
public:
	MdxBlindBlock( const char *name = 0 ) ;
	virtual ~MdxBlindBlock() ;
	MdxBlindBlock( const MdxBlindBlock &src ) ;
	MdxBlindBlock &operator=( const MdxBlindBlock &src ) ;

	virtual int GetTypeID() const { return MDX_BLIND_BLOCK ; }
	virtual const char *GetTypeName() const { return "BlindBlock" ; }
	virtual MdxChunk *Clone() const { return new MdxBlindBlock( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  BlindData command
//----------------------------------------------------------------

class MdxBlindData : public MdxCommand {
public:
	MdxBlindData() ;
	// MdxBlindData( const MdxBlindData &src ) ;
	// MdxBlindData &operator=( const MdxBlindData &src ) ;

	virtual int GetTypeID() const { return MDX_BLIND_DATA ; }
	virtual const char *GetTypeName() const { return "BlindData" ; }
	virtual MdxChunk *Clone() const { return new MdxBlindData( *this ) ; }
} ;

//----------------------------------------------------------------
//  unknown block
//----------------------------------------------------------------

class MdxUnknownBlock : public MdxBlock {
public:
	MdxUnknownBlock( int type_id, const char *type_name = 0, const char *name = 0 ) ;
	virtual ~MdxUnknownBlock() ;
	MdxUnknownBlock( const MdxUnknownBlock &src ) ;
	MdxUnknownBlock &operator=( const MdxUnknownBlock &src ) ;

	virtual int GetTypeID() const ;
	virtual const char *GetTypeName() const ;
	virtual MdxChunk *Clone() const ;
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  unknown command
//----------------------------------------------------------------

class MdxUnknownCommand : public MdxCommand {
public:
	MdxUnknownCommand( int type_id, const char *type_name = 0 ) ;
	virtual ~MdxUnknownCommand() ;
	MdxUnknownCommand( const MdxUnknownCommand &src ) ;
	MdxUnknownCommand &operator=( const MdxUnknownCommand &src ) ;

	virtual int GetTypeID() const ;
	virtual const char *GetTypeName() const ;
	virtual MdxChunk *Clone() const ;
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;


} // namespace mdx

#endif
