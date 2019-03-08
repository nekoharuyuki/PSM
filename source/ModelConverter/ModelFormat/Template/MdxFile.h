#ifndef	MDX_FILE_H_INCLUDE
#define MDX_FILE_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  File block
//----------------------------------------------------------------

class MdxFile : public MdxBlock {
public:
	MdxFile( const char *name = 0 ) ;
	virtual ~MdxFile() ;
	MdxFile( const MdxFile &src ) ;
	MdxFile &operator=( const MdxFile &src ) ;

	virtual int GetTypeID() const { return MDX_FILE ; }
	virtual const char *GetTypeName() const { return "File" ; }
	virtual MdxChunk *Clone() const { return new MdxFile( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;
} ;

//----------------------------------------------------------------
//  DefineEnum command
//----------------------------------------------------------------

class MdxDefineEnum : public MdxCommand {
public:
	MdxDefineEnum( const char *scope = 0, const char *name = 0, int value = 0 ) ;
	virtual int GetTypeID() const { return MDX_DEFINE_ENUM ; }
	virtual const char *GetTypeName() const { return "DefineEnum" ; }
	virtual MdxChunk *Clone() const { return new MdxDefineEnum( *this ) ; }

	void SetDefScope( const char *scope ) ;
	void SetDefName( const char *name ) ;
	void SetDefValue( int value ) ;

	const char *GetDefScope() const ;
	const char *GetDefName() const ;
	int GetDefValue() const ;
} ;

//----------------------------------------------------------------
//  DefineBlock command
//----------------------------------------------------------------

class MdxDefineBlock : public MdxCommand {
public:
	MdxDefineBlock( int id = 0, const char *name = 0, int argc = 0, const int *descs = 0 ) ;
	virtual int GetTypeID() const { return MDX_DEFINE_BLOCK ; }
	virtual const char *GetTypeName() const { return "DefineBlock" ; }
	virtual MdxChunk *Clone() const { return new MdxDefineBlock( *this ) ; }

	void SetDefTypeID( int id ) ;
	void SetDefTypeName( const char *name ) ;
	void SetDefArgsCount( int num ) ;
	void SetDefArgsDesc( int num, int type ) ;

	int GetDefTypeID() const ;
	const char *GetDefTypeName() const ;
	int GetDefArgsCount() const ;
	int GetDefArgsDesc( int num ) const ;
} ;

//----------------------------------------------------------------
//  DefineCommand command
//----------------------------------------------------------------

class MdxDefineCommand : public MdxCommand {
public:
	MdxDefineCommand( int id = 0, const char *name = 0, int argc = 0, const int *descs = 0 ) ;
	virtual int GetTypeID() const { return MDX_DEFINE_COMMAND ; }
	virtual const char *GetTypeName() const { return "DefineCommand" ; }
	virtual MdxChunk *Clone() const { return new MdxDefineCommand( *this ) ; }

	void SetDefTypeID( int id ) ;
	void SetDefTypeName( const char *name ) ;
	void SetDefArgsCount( int num ) ;
	void SetDefArgsDesc( int num, int type ) ;

	int GetDefTypeID() const ;
	const char *GetDefTypeName() const ;
	int GetDefArgsCount() const ;
	int GetDefArgsDesc( int num ) const ;
} ;


} // namespace mdx

#endif
