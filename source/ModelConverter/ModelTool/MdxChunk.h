#ifndef MDX_CHUNK_H_INCLUDE
#define MDX_CHUNK_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  MdxChunk
//----------------------------------------------------------------

class MdxChunk : public MdxBase {
public:
	MdxChunk() ;
	virtual ~MdxChunk() ;
	MdxChunk( const MdxChunk &src ) ;
	MdxChunk &operator=( const MdxChunk &src ) ;
	bool operator==( const MdxChunk &src ) const ;
	bool operator!=( const MdxChunk &src ) const ;

	//  type information

	virtual int GetFormatID() const { return 0 ; }
	virtual int GetTypeID() const { return 0 ; }
	virtual const char *GetTypeName() const { return "" ; }
	virtual bool IsCommand() const { return false ; }
	virtual float GetPriority() const { return (float)GetTypeID() ; }

	//  copy and compare

	virtual MdxChunk *Clone() const ;
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;
	MdxChunk *CloneTree() const ;
	MdxChunk *CopyTree( const MdxChunk *src ) ;
	bool EqualsTree( const MdxChunk *src ) const ;

	//  name

	void SetName( const char *name ) ;
	const char *GetName() const ;
	const char *UniqName( const char *prefix = 0 ) ;

	//  args

	virtual bool UpdateArgs() ;
	virtual bool FlushArgs() ;
	virtual bool SetArgsImage( const void *buf, int size, int style = 0, int option = 0 ) ;
	virtual bool GetArgsImage( const void *&buf, int &size, int style = 0, int option = 0 ) const ;

	virtual void SetArgsCount( int count ) ;
	virtual int GetArgsCount() const ;
	virtual void SetArgsDesc( int num, int desc ) ;		// MDX_WORD_XXXX
	virtual int GetArgsDesc( int num ) const ;		// MDX_WORD_XXXX
	virtual void SetArgsInt( int num, int val ) ;
	virtual int GetArgsInt( int num ) const ;
	virtual void SetArgsFloat( int num, float val ) ;
	virtual float GetArgsFloat( int num ) const ;
	virtual void SetArgsString( int num, const char *val ) ;
	virtual const char *GetArgsString( int num ) const ;

	void SetArgsVec2( int num, const vec2 &v ) ;
	vec2 GetArgsVec2( int num ) const ;
	void SetArgsVec3( int num, const vec3 &v ) ;
	vec3 GetArgsVec3( int num ) const ;
	void SetArgsVec4( int num, const vec4 &v ) ;
	vec4 GetArgsVec4( int num ) const ;
	void SetArgsMat4( int num, const mat4 &m ) ;
	mat4 GetArgsMat4( int num ) const ;
	void SetArgsQuat( int num, const quat &v ) ;
	quat GetArgsQuat( int num ) const ;
	void SetArgsRect( int num, const rect &v ) ;
	rect GetArgsRect( int num ) const ;
	void SetArgsRef( int num, const MdxChunk *ref ) ;
	MdxChunk *GetArgsRef( int num ) const ;
	bool HasArgsRef() const ;

	//  data

	virtual bool UpdateData() ;
	virtual bool FlushData() ;
	virtual bool SetDataImage( const void *buf, int size, int style = 0, int option = 0 ) ;
	virtual bool GetDataImage( const void *&buf, int &size, int style = 0, int option = 0 ) const ;

	virtual void SetDataCount( int count ) ;
	virtual int GetDataCount() const ;
	virtual void SetDataDesc( int num, int desc ) ;		// MDX_WORD_XXXX
	virtual int GetDataDesc( int num ) const ;		// MDX_WORD_XXXX
	virtual void SetDataInt( int num, int val ) ;
	virtual int GetDataInt( int num ) const ;
	virtual void SetDataFloat( int num, float val ) ;
	virtual float GetDataFloat( int num ) const ;
	virtual void SetDataString( int num, const char *val ) ;
	virtual const char *GetDataString( int num ) const ;

	vec2 GetDataVec2( int num ) const ;
	void SetDataVec2( int num, const vec2 &v ) ;
	vec3 GetDataVec3( int num ) const ;
	void SetDataVec3( int num, const vec3 &v ) ;
	vec4 GetDataVec4( int num ) const ;
	void SetDataVec4( int num, const vec4 &v ) ;
	mat4 GetDataMat4( int num ) const ;
	void SetDataMat4( int num, const mat4 &m ) ;
	quat GetDataQuat( int num ) const ;
	void SetDataQuat( int num, const quat &v ) ;
	rect GetDataRect( int num ) const ;
	void SetDataRect( int num, const rect &v ) ;
	void SetDataRef( int num, const MdxChunk *ref ) ;
	MdxChunk *GetDataRef( int num ) const ;
	bool HasDataRef() const ;

	//  hierarchy

	MdxChunk *GetParent() const ;
	MdxChunk *GetRoot( int type = 0 ) const ;
	void ClearChild( int type = 0 ) ;
	void ClearTree( int type = 0 ) ;
	int GetChildCount( int type = 0 ) const ;
	MdxChunk *GetChild( int num ) const ;
	void AttachChild( MdxChunk *chunk ) ;
	void DetachChild( int num ) ;
	void DetachChild( MdxChunk *chunk ) ;
	void InsertChild( int num, MdxChunk *chunk ) ;
	void DeleteChild( int num ) ;
	void DeleteChild( MdxChunk *chunk ) ;

	MdxChunk *FindChild( int type, const char *name ) const ;
	MdxChunk *FindChild( int type, int rank = 0 ) const ;
	MdxChunk *FindTree( int type, const char *name = 0 ) const ;
	int IndexOfChild( int type, const char *name ) const ;
	int IndexOfChild( int type, int rank = 0 ) const ;
	int IndexOfChild( const MdxChunk *chunk ) const ;
	int RankOfChild( int type, const char *name ) const ;
	int RankOfChild( const MdxChunk *chunk ) const ;
	int RankOfChild( int num ) const ;

	MdxChunk *FindRef( int type, const char *name ) const ;
	MdxChunk *FindRef( int index ) const ;			// MDX_REF_XXXX
	int IndexOfRef( int type, const char *name ) const ;	// MDX_REF_XXXX
	int IndexOfRef( const MdxChunk *chunk ) const ;		// MDX_REF_XXXX

	MdxChunk *FindReferrer( int type = 0, const MdxChunk *parent = 0 ) const ;
	bool RefersTo( const MdxChunk *chunk ) const ;

private:
	class impl ;
	impl *m_impl ;
} ;


} // namespace mdx

#endif
