#ifndef	MDX_ARRAYS_H_INCLUDE
#define MDX_ARRAYS_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Arrays block
//----------------------------------------------------------------

class MdxArrays : public MdxBlock {
public:
	MdxArrays( const char *name = 0 ) ;
	~MdxArrays() ;
	MdxArrays( const MdxArrays &src ) ;
	MdxArrays &operator=( const MdxArrays &src ) ;

	//  chunk methods

	virtual int GetTypeID() const { return MDX_ARRAYS ; }
	virtual const char *GetTypeName() const { return "Arrays" ; }
	virtual MdxChunk *Clone() const { return new MdxArrays( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;
	virtual bool UpdateArgs() ;
	virtual bool FlushArgs() ;
	virtual bool UpdateData() ;
	virtual bool FlushData() ;

	//  arrays methods

	void SetFormat( int format ) ;
	int GetFormat() const ;
	void SetVertexCount( int count ) ;
	int GetVertexCount() const ;
	void SetVertex( int index, const MdxVertex &vert ) ;
	const MdxVertex &GetVertex( int index ) const ;
	MdxVertex &GetVertex( int index ) ;
	void InsertVertex( int index, int count = 1 ) ;
	void DeleteVertex( int index, int count = 1 ) ;

	//  morph methods

	void SetMorphFormat( int format ) ;
	int GetMorphFormat() const ;
	void SetMorphCount( int count ) ;
	int GetMorphCount() const ;
	void SetMorph( int index, const MdxArrays *morph ) ;
	const MdxArrays *GetMorph( int index ) const ;
	MdxArrays *GetMorph( int index ) ;
	void InsertMorph( int index, int count = 1 ) ;
	void DeleteMorph( int index, int count = 1 ) ;

	//  convenience functions

	void SetVertexFormat( int format ) ;
	int GetVertexFormat() const ;
	void SetVertexPositionCount( int count ) ;
	void SetVertexNormalCount( int count ) ;
	void SetVertexColorCount( int count ) ;
	void SetVertexTexCoordCount( int count ) ;
	void SetVertexWeightCount( int count ) ;
	int GetVertexPositionCount() const ;
	int GetVertexNormalCount() const ;
	int GetVertexColorCount() const ;
	int GetVertexTexCoordCount() const ;
	int GetVertexWeightCount() const ;
	int GetVertexStride() const ;
	void SetVertexBlendIndicesMode( bool exists ) ;
	bool GetVertexBlendIndicesMode() const ;
	int GetVertexBlendIndicesCount() const ;

public:
	static int GetFormatStride( int format ) ;

private:
	class impl ;
	impl *m_impl ;
} ;

//----------------------------------------------------------------
//  Arrays vertex
//----------------------------------------------------------------

class MdxVertex : public MdxBase {
public:
	MdxVertex() ;
	~MdxVertex() ;
	MdxVertex( const MdxVertex &src ) ;
	MdxVertex &operator=( const MdxVertex &src ) ;
	bool operator==( const MdxVertex &src ) const ;
	bool operator!=( const MdxVertex &src ) const ;

	void SetFormat( int format ) ;
	int GetFormat() const ;

	//  vertex elements

	void SetPosition( const vec4 &position ) ;
	const vec4 &GetPosition() const ;
	void SetNormal( const vec4 &normal ) ;
	const vec4 &GetNormal() const ;
	void SetColor( rgba8888 color ) ;
	rgba8888 GetColor() const ;
	void SetTexCoord( const vec4 &coord ) ;
	const vec4 &GetTexCoord() const ;
	void SetWeight( float weight ) ;
	float GetWeight() const ;

	void SetTexCoord( int num, const vec4 &coord ) ;
	const vec4 &GetTexCoord( int num ) const ;
	void SetWeight( int num, float weight ) ;
	float GetWeight( int num ) const ;
	void SetBlendIndex( int num, int index ) ;
	int GetBlendIndex( int num ) const ;

	//  convenience functions

	void SetPositionCount( int count ) ;
	void SetNormalCount( int count ) ;
	void SetColorCount( int count ) ;
	void SetTexCoordCount( int count ) ;
	void SetWeightCount( int count ) ;
	int GetPositionCount() const ;
	int GetNormalCount() const ;
	int GetColorCount() const ;
	int GetTexCoordCount() const ;
	int GetWeightCount() const ;
	int GetStride() const ;
	void SetBlendIndicesMode( bool exists ) ;
	bool GetBlendIndicesMode() const ;

public:
	static int GetFormatStride( int format ) ;

private:
	class impl ;
	impl *m_impl ;
} ;


} // namespace mdx

#endif
