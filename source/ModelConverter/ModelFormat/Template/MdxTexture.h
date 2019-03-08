#ifndef	MDX_TEXTURE_H_INCLUDE
#define MDX_TEXTURE_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Texture block
//----------------------------------------------------------------

class MdxTexture : public MdxBlock {
public:
	MdxTexture( const char *name = 0 ) ;
	virtual ~MdxTexture() ;
	MdxTexture( const MdxTexture &src ) ;
	MdxTexture &operator=( const MdxTexture &src ) ;

	virtual int GetTypeID() const { return MDX_TEXTURE ; }
	virtual const char *GetTypeName() const { return "Texture" ; }
	virtual MdxChunk *Clone() const { return new MdxTexture( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  TexType command
//----------------------------------------------------------------

class MdxTexType : public MdxCommand {
public:	
	MdxTexType( int type = MDX_TEXTURE_2D ) ;
	virtual int GetTypeID() const { return MDX_TEX_TYPE ; }
	virtual const char *GetTypeName() const { return "TexType" ; }
	virtual MdxChunk *Clone() const { return new MdxTexType( *this ) ; }

	void SetType( int type ) ;
	int GetType() const ;
} ;

//----------------------------------------------------------------
//  TexFilter command
//----------------------------------------------------------------

class MdxTexFilter : public MdxCommand {
public:	
	MdxTexFilter( int mag = MDX_FILTER_LINEAR, int min = MDX_FILTER_LINEAR_MIPMAP_LINEAR ) ;
	virtual int GetTypeID() const { return MDX_TEX_FILTER ; }
	virtual const char *GetTypeName() const { return "TexFilter" ; }
	virtual MdxChunk *Clone() const { return new MdxTexFilter( *this ) ; }

	void SetMag( int mag ) ;
	void SetMin( int min ) ;
	int GetMag() const ;
	int GetMin() const ;
} ;

//----------------------------------------------------------------
//  TexWrap command
//----------------------------------------------------------------

class MdxTexWrap : public MdxCommand {
public:	
	MdxTexWrap( int wrap_u = MDX_WRAP_REPEAT, int wrap_v = MDX_WRAP_REPEAT ) ;
	virtual int GetTypeID() const { return MDX_TEX_WRAP ; }
	virtual const char *GetTypeName() const { return "TexWrap" ; }
	virtual MdxChunk *Clone() const { return new MdxTexWrap( *this ) ; }

	void SetWrapU( int wrap_u ) ;
	void SetWrapV( int wrap_v ) ;
	int GetWrapU() const ;
	int GetWrapV() const ;
} ;

//----------------------------------------------------------------
//  TexCrop command
//----------------------------------------------------------------

class MdxTexCrop : public MdxCommand {
public:
	MdxTexCrop( const rect &crop = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_TEX_CROP ; }
	virtual const char *GetTypeName() const { return "TexCrop" ; }
	virtual MdxChunk *Clone() const { return new MdxTexCrop( *this ) ; }

	void SetCrop( const rect &crop ) ;
	rect GetCrop() const ;
} ;

//----------------------------------------------------------------
//  UVPivot command
//----------------------------------------------------------------

class MdxUVPivot : public MdxCommand {
public:
	MdxUVPivot( const vec2 &pivot = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_UV_PIVOT ; }
	virtual const char *GetTypeName() const { return "UVPivot" ; }
	virtual MdxChunk *Clone() const { return new MdxUVPivot( *this ) ; }

	void SetPivot( const vec2 &pivot ) ;
	vec2 GetPivot() const ;
} ;

//----------------------------------------------------------------
//  UVTranslate command
//----------------------------------------------------------------

class MdxUVTranslate : public MdxCommand {
public:
	MdxUVTranslate( const vec2 &translate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_UV_TRANSLATE ; }
	virtual const char *GetTypeName() const { return "UVTranslate" ; }
	virtual MdxChunk *Clone() const { return new MdxUVTranslate( *this ) ; }

	void SetTranslate( const vec2 &translate ) ;
	vec2 GetTranslate() const ;
} ;

//----------------------------------------------------------------
//  UVRotate command
//----------------------------------------------------------------

class MdxUVRotate : public MdxCommand {
public:
	MdxUVRotate( float rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_UV_PIVOT ; }
	virtual const char *GetTypeName() const { return "UVRotate" ; }
	virtual MdxChunk *Clone() const { return new MdxUVRotate( *this ) ; }

	void SetRotate( float rotate ) ;
	float GetRotate() const ;
} ;

//----------------------------------------------------------------
//  UVScale command
//----------------------------------------------------------------

class MdxUVScale : public MdxCommand {
public:
	MdxUVScale( const vec2 &scale = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_UV_SCALE ; }
	virtual const char *GetTypeName() const { return "UVScale" ; }
	virtual MdxChunk *Clone() const { return new MdxUVScale( *this ) ; }

	void SetScale( const vec2 &scale ) ;
	vec2 GetScale() const ;
} ;


} // namespace mdx

#endif
