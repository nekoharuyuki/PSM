#ifndef	MDX_MODEL_H_INCLUDE
#define MDX_MODEL_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Model block
//----------------------------------------------------------------

class MdxModel : public MdxBlock {
public:
	MdxModel( const char *name = 0 ) ;
	virtual ~MdxModel() ;
	MdxModel( const MdxModel &src ) ;
	MdxModel &operator=( const MdxModel &src ) ;

	virtual int GetTypeID() const { return MDX_MODEL ; }
	virtual const char *GetTypeName() const { return "Model" ; }
	virtual MdxChunk *Clone() const { return new MdxModel( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;
} ;

//----------------------------------------------------------------
//  FileName command
//----------------------------------------------------------------

class MdxFileName : public MdxCommand {
public:
	MdxFileName( const char *filename = 0 ) ;
	virtual int GetTypeID() const { return MDX_FILE_NAME ; }
	virtual const char *GetTypeName() const { return "FileName" ; }
	virtual MdxChunk *Clone() const { return new MdxFileName( *this ) ; }

	void SetFileName( const char *name ) ;
	const char *GetFileName() const ;
} ;

//----------------------------------------------------------------
//  FileImage command
//----------------------------------------------------------------

class MdxFileImage : public MdxCommand {
public:
	MdxFileImage( const void *buf = 0, int size = 0 ) ;
	virtual int GetTypeID() const { return MDX_FILE_IMAGE ; }
	virtual const char *GetTypeName() const { return "FileImage" ; }
	virtual MdxChunk *Clone() const { return new MdxFileImage( *this ) ; }
	virtual void SetArgsInt( int num, int val ) ;

	void SetFileImage( const void *buf, int size ) ;
	void GetFileImage( const void *&buf, int &size ) const ;
} ;

//----------------------------------------------------------------
//  BoundingBox command
//----------------------------------------------------------------

class MdxBoundingBox : public MdxCommand {
public:
	MdxBoundingBox( const vec3 &lower = 0.0f, const vec3 &upper = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_BOUNDING_BOX ; }
	virtual const char *GetTypeName() const { return "BoundingBox" ; }
	virtual MdxChunk *Clone() const { return new MdxBoundingBox( *this ) ; }

	void SetLower( const vec3 &lower ) ;
	void SetUpper( const vec3 &upper ) ;
	vec3 GetLower() const ;
	vec3 GetUpper() const ;
} ;

//----------------------------------------------------------------
//  BoundingSphere command
//----------------------------------------------------------------

class MdxBoundingSphere : public MdxCommand {
public:
	MdxBoundingSphere( const vec3 &center = 0.0f, float radius = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_BOUNDING_SPHERE ; }
	virtual const char *GetTypeName() const { return "BoundingSphere" ; }
	virtual MdxChunk *Clone() const { return new MdxBoundingSphere( *this ) ; }

	void SetCenter( const vec3 &center ) ;
	void SetRadius( float radius ) ;
	vec3 GetCenter() const ;
	float GetRadius() const ;
} ;


} // namespace mdx

#endif
