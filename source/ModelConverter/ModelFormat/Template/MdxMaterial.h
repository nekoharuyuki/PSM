#ifndef	MDX_MATERIAL_H_INCLUDE
#define MDX_MATERIAL_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Material block
//----------------------------------------------------------------

class MdxMaterial : public MdxBlock {
public:
	MdxMaterial( const char *name = 0 ) ;
	virtual ~MdxMaterial() ;
	MdxMaterial( const MdxMaterial &src ) ;
	MdxMaterial &operator=( const MdxMaterial &src ) ;

	virtual int GetTypeID() const { return MDX_MATERIAL ; }
	virtual const char *GetTypeName() const { return "Material" ; }
	virtual MdxChunk *Clone() const { return new MdxMaterial( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  Layer block
//----------------------------------------------------------------

class MdxLayer : public MdxBlock {
public:
	MdxLayer( const char *name = 0 ) ;
	virtual ~MdxLayer() ;
	MdxLayer( const MdxLayer &src ) ;
	MdxLayer &operator=( const MdxLayer &src ) ;

	virtual int GetTypeID() const { return MDX_LAYER ; }
	virtual const char *GetTypeName() const { return "Layer" ; }
	virtual MdxChunk *Clone() const { return new MdxLayer( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  Base color command
//----------------------------------------------------------------

class MdxColorCommand : public MdxCommand {
public:
	MdxColorCommand( const vec3 &color = 1.0f ) ;

	void SetColor( const vec3 &color ) ;
	vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Diffuse command
//----------------------------------------------------------------

class MdxDiffuse : public MdxColorCommand {
public:
	MdxDiffuse( const vec3 &color = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_DIFFUSE ; }
	virtual const char *GetTypeName() const { return "Diffuse" ; }
	virtual MdxChunk *Clone() const { return new MdxDiffuse( *this ) ; }

	// void SetColor( const vec3 &color ) ;
	// vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Ambient command
//----------------------------------------------------------------

class MdxAmbient : public MdxColorCommand {
public:
	MdxAmbient( const vec3 &color = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_AMBIENT ; }
	virtual const char *GetTypeName() const { return "Ambient" ; }
	virtual MdxChunk *Clone() const { return new MdxAmbient( *this ) ; }

	// void SetColor( const vec3 &color ) ;
	// vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Specular command
//----------------------------------------------------------------

class MdxSpecular : public MdxColorCommand {
public:
	MdxSpecular( const vec3 &color = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_SPECULAR ; }
	virtual const char *GetTypeName() const { return "Specular" ; }
	virtual MdxChunk *Clone() const { return new MdxSpecular( *this ) ; }

	// void SetColor( const vec3 &color ) ;
	// vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Emission command
//----------------------------------------------------------------

class MdxEmission : public MdxColorCommand {
public:
	MdxEmission( const vec3 &color = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_EMISSION ; }
	virtual const char *GetTypeName() const { return "Emission" ; }
	virtual MdxChunk *Clone() const { return new MdxEmission( *this ) ; }

	// void SetColor( const vec3 &color ) ;
	// vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Reflection command
//----------------------------------------------------------------

class MdxReflection : public MdxColorCommand {
public:
	MdxReflection( const vec3 &color = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_REFLECTION ; }
	virtual const char *GetTypeName() const { return "Reflection" ; }
	virtual MdxChunk *Clone() const { return new MdxReflection( *this ) ; }

	// void SetColor( const vec3 &color ) ;
	// vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Refraction command
//----------------------------------------------------------------

class MdxRefraction : public MdxColorCommand {
public:
	MdxRefraction( const vec3 &color = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_REFRACTION ; }
	virtual const char *GetTypeName() const { return "Refraction" ; }
	virtual MdxChunk *Clone() const { return new MdxRefraction( *this ) ; }

	// void SetColor( const vec3 &color ) ;
	// vec3 GetColor() const ;
} ;

//----------------------------------------------------------------
//  Bump command
//----------------------------------------------------------------

class MdxBump : public MdxCommand {
public:
	MdxBump( float bump = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_BUMP ; }
	virtual const char *GetTypeName() const { return "Bump" ; }
	virtual MdxChunk *Clone() const { return new MdxBump( *this ) ; }

	void SetBump( float bump ) ;
	float GetBump() const ;
} ;

//----------------------------------------------------------------
//  Opacity command
//----------------------------------------------------------------

class MdxOpacity : public MdxCommand {
public:
	MdxOpacity( float opacity = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_OPACITY ; }
	virtual const char *GetTypeName() const { return "Opacity" ; }
	virtual MdxChunk *Clone() const { return new MdxOpacity( *this ) ; }

	void SetOpacity( float opacity ) ;
	float GetOpacity() const ;
} ;

//----------------------------------------------------------------
//  Shininess command
//----------------------------------------------------------------

class MdxShininess : public MdxCommand {
public:
	MdxShininess( float shininess = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_SHININESS ; }
	virtual const char *GetTypeName() const { return "Shininess" ; }
	virtual MdxChunk *Clone() const { return new MdxShininess( *this ) ; }

	void SetShininess( float shininess ) ;
	float GetShininess() const ;
} ;

//----------------------------------------------------------------
//  SetTexture command
//----------------------------------------------------------------

class MdxSetTexture : public MdxCommand {
public:
	MdxSetTexture( const char *texture = 0 ) ;
	virtual int GetTypeID() const { return MDX_SET_TEXTURE ; }
	virtual const char *GetTypeName() const { return "SetTexture" ; }
	virtual MdxChunk *Clone() const { return new MdxSetTexture( *this ) ; }

	void SetTexture( const char *texture ) ;
	const char *GetTexture() const ;

	void SetTextureRef( const MdxTexture *texture ) ;
	MdxTexture *GetTextureRef() const ;
} ;

//----------------------------------------------------------------
//  MapType command
//----------------------------------------------------------------

class MdxMapType : public MdxCommand {
public:
	MdxMapType( int type = MDX_DIFFUSE ) ;
	virtual int GetTypeID() const { return MDX_MAP_TYPE ; }
	virtual const char *GetTypeName() const { return "MapType" ; }
	virtual MdxChunk *Clone() const { return new MdxMapType( *this ) ; }

	void SetType( int type ) ;
	int GetType() const ;
} ;

//----------------------------------------------------------------
//  MapCoord command
//----------------------------------------------------------------

class MdxMapCoord : public MdxCommand {
public:
	MdxMapCoord( int coord = MDX_VF_TEXCOORD ) ;
	virtual int GetTypeID() const { return MDX_MAP_COORD ; }
	virtual const char *GetTypeName() const { return "MapCoord" ; }
	virtual MdxChunk *Clone() const { return new MdxMapCoord( *this ) ; }

	void SetCoord( int coord ) ;
	int GetCoord() const ;
} ;

//----------------------------------------------------------------
//  MapFactor command
//----------------------------------------------------------------

class MdxMapFactor : public MdxCommand {
public:
	MdxMapFactor( float factor = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_MAP_FACTOR ; }
	virtual const char *GetTypeName() const { return "MapFactor" ; }
	virtual MdxChunk *Clone() const { return new MdxMapFactor( *this ) ; }

	void SetFactor( float factor ) ;
	float GetFactor() const ;
} ;


} // namespace mdx

#endif
