#ifndef	MDX_FCURVE_H_INCLUDE
#define MDX_FCURVE_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  FCurve block
//----------------------------------------------------------------

class MdxFCurve : public MdxBlock {
public:
	MdxFCurve( const char *name = 0 ) ;
	~MdxFCurve() ;
	MdxFCurve( const MdxFCurve &src ) ;
	MdxFCurve &operator=( const MdxFCurve &src ) ;

	//  chunk methods

	virtual int GetTypeID() const { return MDX_FCURVE ; }
	virtual const char *GetTypeName() const { return "FCurve" ; }
	virtual MdxChunk *Clone() const { return new MdxFCurve( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;
	virtual bool UpdateArgs() ;
	virtual bool FlushArgs() ;
	virtual bool UpdateData() ;
	virtual bool FlushData() ;

	//  fcurve methods

	void SetFormat( int format, bool adjust_handles = false ) ;	// obsolete
	int GetFormat() const ;
	void SetInterp( int interp, bool adjust_handle = false ) ;
	int GetInterp() const ;
	void SetExtrap( int extrap, bool adjust_handle = false ) ;
	int GetExtrap() const ;
	void SetDimCount( int count ) ;
	int GetDimCount() const ;
	void SetKeyFrameCount( int count ) ;
	int GetKeyFrameCount() const ;

	void SetKeyFrame( int index, const MdxKeyFrame &key ) ;
	const MdxKeyFrame &GetKeyFrame( int index ) const ;
	MdxKeyFrame &GetKeyFrame( int index ) ;

	void InsertKeyFrame( int index, const MdxKeyFrame &key, bool adjust_handles = false ) ;
	void DeleteKeyFrame( int index, bool adjust_handles = false ) ;
	int IndexOfKeyFrame( float frame ) ;	// returns n, where key[n] == frame
	int IndexOfInterval( float frame ) ;	// returns n, where key[n] <= frame < key[n+1]
	int Eval( float frame, MdxKeyFrame &key, bool eval_handles = false ) ;

	//  convenience functions

	int GetKeyFrameStride() const ;
	bool HasKeyFrameDY() const ;
	bool HasKeyFrameDX() const ;

public:
	static int GetFormatStride( int format, int dim_count ) ;

private:
	class impl ;
	impl *m_impl ;
} ;

//----------------------------------------------------------------
//  FCurve keyframe
//----------------------------------------------------------------

class MdxKeyFrame : public MdxBase {
public:
	MdxKeyFrame() ;
	virtual ~MdxKeyFrame() ;
	MdxKeyFrame( const MdxKeyFrame &src ) ;
	MdxKeyFrame &operator=( const MdxKeyFrame &src ) ;
	bool operator==( const MdxKeyFrame &src ) const ;
	bool operator!=( const MdxKeyFrame &src ) const ;

	void SetFormat( int format ) ;	// obsolete
	int GetFormat() const ;
	void SetInterp( int interp ) ;
	int GetInterp() const ;
	void SetDimCount( int count ) ;
	int GetDimCount() const ;

	//  keyframe elements

	void SetFrame( float frame ) ;
	float GetFrame() const ;
	void SetValue( int index, float value ) ;
	float GetValue( int index ) const ;
	void SetInDY( int index, float dy ) ;
	float GetInDY( int index ) const ;
	void SetOutDY( int index, float dy ) ;
	float GetOutDY( int index ) const ;
	void SetInDX( int index, float dx ) ;
	float GetInDX( int index ) const ;
	void SetOutDX( int index, float dx ) ;
	float GetOutDX( int index ) const ;

	//  convenience functions

	void SetValueVec2( int num, const vec2 &v ) ;
	const vec2 &GetValueVec2( int num ) const ;
	void SetValueVec3( int num, const vec3 &v ) ;
	const vec3 &GetValueVec3( int num ) const ;
	void SetValueVec4( int num, const vec4 &v ) ;
	const vec4 &GetValueVec4( int num ) const ;
	void SetValueMat4( int num, const mat4 &m ) ;
	const mat4 &GetValueMat4( int num ) const ;
	void SetValueQuat( int num, const quat &v ) ;
	const quat &GetValueQuat( int num ) const ;
	void SetValueRect( int num, const rect &v ) ;
	const rect &GetValueRect( int num ) const ;

	int GetStride() const ;
	bool HasDY() const ;
	bool HasDX() const ;

public:
	static int GetFormatStride( int format, int dim_count ) ;

private:
	class impl ;
	impl *m_impl ;
} ;


} // namespace mdx

#endif
