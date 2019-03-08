#ifndef	MDX_PART_H_INCLUDE
#define MDX_PART_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Part block
//----------------------------------------------------------------

class MdxPart : public MdxBlock {
public:
	MdxPart( const char *name = 0 ) ;
	virtual ~MdxPart() ;
	MdxPart( const MdxPart &src ) ;
	MdxPart &operator=( const MdxPart &src ) ;

	//  chunk methods

	virtual int GetTypeID() const { return MDX_PART ; }
	virtual const char *GetTypeName() const { return "Part" ; }
	virtual MdxChunk *Clone() const { return new MdxPart( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  Mesh block
//----------------------------------------------------------------

class MdxMesh : public MdxBlock {
public:
	MdxMesh( const char *name = 0 ) ;
	virtual ~MdxMesh() ;
	MdxMesh( const MdxMesh &src ) ;
	MdxMesh &operator=( const MdxMesh &src ) ;

	virtual int GetTypeID() const { return MDX_MESH ; }
	virtual const char *GetTypeName() const { return "Mesh" ; }
	virtual MdxChunk *Clone() const { return new MdxMesh( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  VertexOffset command
//----------------------------------------------------------------

class MdxVertexOffset : public MdxCommand {
public:
	MdxVertexOffset( int format = 0, const float *offsets = 0 ) ;
	virtual int GetTypeID() const { return MDX_VERTEX_OFFSET ; }
	virtual const char *GetTypeName() const { return "VertexOffset" ; }
	virtual MdxChunk *Clone() const { return new MdxVertexOffset( *this ) ; }
	virtual void SetArgsInt( int num, int val ) ;

	void SetFormat( int format ) ;
	void SetOffset( int num, float offset ) ;

	int GetFormat() const ;
	float GetOffset( int num ) const ;
} ;

//----------------------------------------------------------------
//  SetMaterial command
//----------------------------------------------------------------

class MdxSetMaterial : public MdxCommand {
public:
	MdxSetMaterial( const char *material = 0 ) ;
	virtual int GetTypeID() const { return MDX_SET_MATERIAL ; }
	virtual const char *GetTypeName() const { return "SetMaterial" ; }
	virtual MdxChunk *Clone() const { return new MdxSetMaterial( *this ) ; }

	void SetMaterial( const char *material ) ;
	const char *GetMaterial() const ;

	void SetMaterialRef( const MdxMaterial *material ) ;
	MdxMaterial *GetMaterialRef() const ;
} ;

//----------------------------------------------------------------
//  SetArrays command
//----------------------------------------------------------------

class MdxSetArrays : public MdxCommand {
public:
	MdxSetArrays( const char *arrays = 0 ) ;
	virtual int GetTypeID() const { return MDX_SET_ARRAYS ; }
	virtual const char *GetTypeName() const { return "SetArrays" ; }
	virtual MdxChunk *Clone() const { return new MdxSetArrays( *this ) ; }

	void SetArrays( const char *arrays ) ;
	const char *GetArrays() const ;

	void SetArraysRef( const MdxArrays *arrays ) ;
	MdxArrays *GetArraysRef() const ;
} ;

//----------------------------------------------------------------
//  BlendIndices command
//----------------------------------------------------------------

class MdxBlendIndices : public MdxCommand {
public:
	MdxBlendIndices( int n_indices = 0, const int *indices = 0 ) ;
	virtual int GetTypeID() const { return MDX_BLEND_INDICES ; }
	virtual const char *GetTypeName() const { return "BlendIndices" ; }
	virtual MdxChunk *Clone() const { return new MdxBlendIndices( *this ) ; }
	virtual void SetArgsInt( int num, int val ) ;

	void SetIndexCount( int num ) ;
	void SetIndex( int num, int index ) ;
	int GetIndexCount() const ;
	int GetIndex( int num ) const ;
} ;

//----------------------------------------------------------------
//  Subdivision command 
//----------------------------------------------------------------

class MdxSubdivision : public MdxCommand {
public:
	MdxSubdivision( float div_u = 4.0f, float div_v = 4.0f ) ;
	virtual int GetTypeID() const { return MDX_SUBDIVISION ; }
	virtual const char *GetTypeName() const { return "Subdivision" ; }
	virtual MdxChunk *Clone() const { return new MdxSubdivision( *this ) ; }

	void SetDivU( float div_u ) ;
	void SetDivV( float div_v ) ;
	float GetDivU() const ;
	float GetDivV() const ;
} ;

//----------------------------------------------------------------
//  KnotVectorU command
//----------------------------------------------------------------

class MdxKnotVectorU : public MdxCommand {
public:
	MdxKnotVectorU( int n_knots = 0, const float *knots = 0 ) ;
	virtual int GetTypeID() const { return MDX_KNOT_VECTOR_U ; }
	virtual const char *GetTypeName() const { return "KnotVectorU" ; }
	virtual MdxChunk *Clone() const { return new MdxKnotVectorU( *this ) ; }
	virtual void SetArgsInt( int num, int val ) ;

	void SetKnotCount( int num ) ;
	void SetKnot( int num, float knot ) ;
	int GetKnotCount() const ;
	float GetKnot( int num ) const ;
} ;

//----------------------------------------------------------------
//  KnotVectorV command
//----------------------------------------------------------------

class MdxKnotVectorV : public MdxCommand {
public:
	MdxKnotVectorV( int n_knots = 0, const float *knots = 0 ) ;
	virtual int GetTypeID() const { return MDX_KNOT_VECTOR_V ; }
	virtual const char *GetTypeName() const { return "KnotVectorV" ; }
	virtual MdxChunk *Clone() const { return new MdxKnotVectorV( *this ) ; }
	virtual void SetArgsInt( int num, int val ) ;

	void SetKnotCount( int num ) ;
	void SetKnot( int num, float knot ) ;
	int GetKnotCount() const ;
	float GetKnot( int num ) const ;
} ;

//----------------------------------------------------------------
//  Base primitive command
//----------------------------------------------------------------

class MdxPrimCommand : public MdxCommand {
public:
	MdxPrimCommand( int mode = 0, int n_verts = 0, int n_prims = 0, const int *indices = 0 ) ;
	virtual void SetArgsInt( int num, int val ) ;

	void SetMode( int mode ) ;
	void SetVertexCount( int n_verts ) ;
	void SetPrimCount( int n_prims ) ;
	void SetIndex( int num, int index ) ;

	int GetMode() const ;
	int GetVertexCount() const ;
	int GetPrimCount() const ;
	int GetIndex( int num ) const ;

	//  convenience functions

	void SetWidth( int width ) { SetVertexCount( width ) ; }
	void SetHeight( int height ) { SetPrimCount( height ) ; }
	int GetWidth() const { return GetVertexCount() ; }
	int GetHeight() const { return GetPrimCount() ; }

	int GetIndexCount() const { return GetVertexCount() * GetPrimCount() ; }
	void SetSequentialIndices( int start = 0 ) ;
	bool HasSequentialIndices() const ;
} ;

//----------------------------------------------------------------
//  DrawArrays command
//----------------------------------------------------------------

class MdxDrawArrays : public MdxPrimCommand {
public:
	MdxDrawArrays( int mode = 0, int n_verts = 0, int n_prims = 0, const int *indices = 0 ) ;
	virtual int GetTypeID() const { return MDX_DRAW_ARRAYS ; }
	virtual const char *GetTypeName() const { return "DrawArrays" ; }
	virtual MdxChunk *Clone() const { return new MdxDrawArrays( *this ) ; }

	// void SetMode( int mode ) ;
	// void SetVertexCount( int n_verts ) ;
	// void SetPrimCount( int n_prims ) ;
	// void SetIndex( int num, int index ) ;

	// int GetMode() const ;
	// int GetVertexCount() const ;
	// int GetPrimCount() const ;
	// int GetIndex( int num ) const ;
} ;

//----------------------------------------------------------------
//  DrawBSpline command
//----------------------------------------------------------------

class MdxDrawBSpline : public MdxPrimCommand {
public:
	MdxDrawBSpline( int mode = 0, int width = 0, int height = 0, const int *indices = 0 ) ;
	virtual int GetTypeID() const { return MDX_DRAW_B_SPLINE ; }
	virtual const char *GetTypeName() const { return "DrawBSpline" ; }
	virtual MdxChunk *Clone() const { return new MdxDrawBSpline( *this ) ; }

	// void SetMode( int mode ) ;
	// void SetWidth( int width ) ;
	// void SetHeight( int height ) ;
	// void SetIndex( int num, int index ) ;

	// int GetMode() const ;
	// int GetWidth() const ;
	// int GetHeight() const ;
	// int GetIndex( int num ) const ;
} ;


} // namespace mdx

#endif
