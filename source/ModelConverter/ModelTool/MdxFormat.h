#ifndef	MDX_FORMAT_H_INCLUDE
#define	MDX_FORMAT_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxFormat
//----------------------------------------------------------------

class MdxFormat : public MdxBase {
public:
	MdxFormat() ;
	virtual ~MdxFormat() ;
	MdxFormat( const MdxFormat &src ) ;
	MdxFormat &operator=( const MdxFormat &src ) ;

	//  type information

	virtual int GetFormatID() const { return MDX_FORMAT_ID ; }
	virtual const char *GetFormatName() const { return "MDX" ; }
	virtual const char *GetFormatVersion() const { return "1.00" ; }
	virtual const char *GetFormatDescription() const { return "Model format" ; }

	//  templates

	virtual void SetTemplate( int id, MdxChunk *chunk ) ;
	virtual MdxChunk *GetTemplate( int id ) const ;

	//  scopes

	virtual int SetScope( const char *name, int id = MDX_SCOPE_VOLATILE ) ;
	virtual int GetScope( const char *name ) const ;
	virtual const char *GetScopeName( int id ) const ;
	virtual bool GetScope( const char *name, int &id ) const ;
	virtual bool GetScopeName( int id, const char *&name ) const ;

	//  symbols

	virtual void SetSymbol( int scope, const char *name, int value ) ;
	virtual int GetSymbol( int scope, const char *name ) const ;
	virtual const char *GetSymbolName( int scope, int value ) const ;
	virtual bool GetSymbol( int scope, const char *name, int &value ) const ;
	virtual bool GetSymbolName( int scope, int value, const char *&name ) const ;

	//  flags ( symbol|symbol ... )

	virtual int GetFlags( int scope, const char *name ) const ;
	virtual const char *GetFlagsName( int scope, int value ) const ;
	virtual bool GetFlags( int scope, const char *name, int &value ) const ;
	virtual bool GetFlagsName( int scope, int value, const char *&name ) const ;

private:
	class impl ;
	impl *m_impl ;
} ;

//----------------------------------------------------------------
//  MdxBlock
//----------------------------------------------------------------

class MdxBlock : public MdxChunk {
public:
	virtual int GetFormatID() const { return MDX_FORMAT_ID ; }
	virtual bool IsCommand() const { return false ; }
} ;

//----------------------------------------------------------------
//  MdxCommand
//----------------------------------------------------------------

class MdxCommand : public MdxBlock {
public:
	virtual int GetFormatID() const { return MDX_FORMAT_ID ; }
	virtual bool IsCommand() const { return true ; }
} ;


} // namespace mdx

#endif
