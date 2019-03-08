#ifndef	MDX_BONE_H_INCLUDE
#define MDX_BONE_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Bone block
//----------------------------------------------------------------

class MdxBone : public MdxBlock {
public:
	MdxBone( const char *name = 0 ) ;
	virtual ~MdxBone() ;
	MdxBone( const MdxBone &src ) ;
	MdxBone &operator=( const MdxBone &src ) ;

	virtual int GetTypeID() const { return MDX_BONE ; }
	virtual const char *GetTypeName() const { return "Bone" ; }
	virtual MdxChunk *Clone() const { return new MdxBone( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  ParentBone command
//----------------------------------------------------------------

class MdxParentBone : public MdxCommand {
public:
	MdxParentBone( const char *bone = 0 ) ;
	virtual int GetTypeID() const { return MDX_PARENT_BONE ; }
	virtual const char *GetTypeName() const { return "ParentBone" ; }
	virtual MdxChunk *Clone() const { return new MdxParentBone( *this ) ; }

	void SetBone( const char *bone ) ;
	const char *GetBone() const ;

	void SetBoneRef( const MdxBone *bone ) ;
	MdxBone *GetBoneRef() const ;
} ;

//----------------------------------------------------------------
//  Visibility command
//----------------------------------------------------------------

class MdxVisibility : public MdxCommand {
public:
	MdxVisibility( int visibility = 1 ) ;
	virtual int GetTypeID() const { return MDX_VISIBILITY ; }
	virtual const char *GetTypeName() const { return "Visibility" ; }
	virtual MdxChunk *Clone() const { return new MdxVisibility( *this ) ; }

	void SetVisibility( int visibility ) ;
	int GetVisibility() const ;
} ;

//----------------------------------------------------------------
//  Pivot command
//----------------------------------------------------------------

class MdxPivot : public MdxCommand {
public:
	MdxPivot( const vec3 &pivot = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_PIVOT ; }
	virtual const char *GetTypeName() const { return "Pivot" ; }
	virtual MdxChunk *Clone() const { return new MdxPivot( *this ) ; }

	void SetPivot( const vec3 &pivot ) ;
	vec3 GetPivot() const ;
} ;

//----------------------------------------------------------------
//  Translate command
//----------------------------------------------------------------

class MdxTranslate : public MdxCommand {
public:
	MdxTranslate( const vec3 &translate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_TRANSLATE ; }
	virtual const char *GetTypeName() const { return "Translate" ; }
	virtual MdxChunk *Clone() const { return new MdxTranslate( *this ) ; }

	void SetTranslate( const vec3 &translate ) ;
	vec3 GetTranslate() const ;
} ;

//----------------------------------------------------------------
//  Rotate command
//----------------------------------------------------------------

class MdxRotate : public MdxCommand {
public:
	MdxRotate( const quat &rotate = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE ; }
	virtual const char *GetTypeName() const { return "Rotate" ; }
	virtual MdxChunk *Clone() const { return new MdxRotate( *this ) ; }

	void SetRotate( const quat &rotate ) ;
	quat GetRotate() const ;
} ;

//----------------------------------------------------------------
//  RotateXYZ command
//----------------------------------------------------------------

class MdxRotateXYZ : public MdxCommand {
public:
	MdxRotateXYZ( const vec3 &rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE_XYZ ; }
	virtual const char *GetTypeName() const { return "RotateXYZ" ; }
	virtual MdxChunk *Clone() const { return new MdxRotateXYZ( *this ) ; }

	void SetRotate( const vec3 &rotate ) ;
	vec3 GetRotate() const ;
} ;

//----------------------------------------------------------------
//  RotateYZX command
//----------------------------------------------------------------

class MdxRotateYZX : public MdxCommand {
public:
	MdxRotateYZX( const vec3 &rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE_YZX ; }
	virtual const char *GetTypeName() const { return "RotateYZX" ; }
	virtual MdxChunk *Clone() const { return new MdxRotateYZX( *this ) ; }

	void SetRotate( const vec3 &rotate ) ;
	vec3 GetRotate() const ;
} ;

//----------------------------------------------------------------
//  RotateZXY command
//----------------------------------------------------------------

class MdxRotateZXY : public MdxCommand {
public:
	MdxRotateZXY( const vec3 &rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE_ZXY ; }
	virtual const char *GetTypeName() const { return "RotateZXY" ; }
	virtual MdxChunk *Clone() const { return new MdxRotateZXY( *this ) ; }

	void SetRotate( const vec3 &rotate ) ;
	vec3 GetRotate() const ;
} ;

//----------------------------------------------------------------
//  RotateXZY command
//----------------------------------------------------------------

class MdxRotateXZY : public MdxCommand {
public:
	MdxRotateXZY( const vec3 &rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE_XZY ; }
	virtual const char *GetTypeName() const { return "RotateXZY" ; }
	virtual MdxChunk *Clone() const { return new MdxRotateXZY( *this ) ; }

	void SetRotate( const vec3 &rotate ) ;
	vec3 GetRotate() const ;
} ;

//----------------------------------------------------------------
//  RotateYXZ command
//----------------------------------------------------------------

class MdxRotateYXZ : public MdxCommand {
public:
	MdxRotateYXZ( const vec3 &rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE_YXZ ; }
	virtual const char *GetTypeName() const { return "RotateYXZ" ; }
	virtual MdxChunk *Clone() const { return new MdxRotateYXZ( *this ) ; }

	void SetRotate( const vec3 &rotate ) ;
	vec3 GetRotate() const ;
} ;

//----------------------------------------------------------------
//  RotateZYX command
//----------------------------------------------------------------

class MdxRotateZYX : public MdxCommand {
public:
	MdxRotateZYX( const vec3 &rotate = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_ROTATE_ZYX ; }
	virtual const char *GetTypeName() const { return "RotateZYX" ; }
	virtual MdxChunk *Clone() const { return new MdxRotateZYX( *this ) ; }

	void SetRotate( const vec3 &rotate ) ;
	vec3 GetRotate() const ;
} ;

//----------------------------------------------------------------
//  Scale command
//----------------------------------------------------------------

class MdxScale : public MdxCommand {
public:
	MdxScale( const vec3 &scale = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_SCALE ; }
	virtual const char *GetTypeName() const { return "Scale" ; }
	virtual MdxChunk *Clone() const { return new MdxScale( *this ) ; }

	void SetScale( const vec3 &scale ) ;
	vec3 GetScale() const ;
} ;

//----------------------------------------------------------------
//  BlendBone command
//----------------------------------------------------------------

class MdxBlendBone : public MdxCommand {
public:
	MdxBlendBone( const char *bone = 0, const mat4 &offset = 1.0f ) ;
	virtual int GetTypeID() const { return MDX_BLEND_BONE ; }
	virtual const char *GetTypeName() const { return "BlendBone" ; }
	virtual MdxChunk *Clone() const { return new MdxBlendBone( *this ) ; }

	void SetBone( const char *bone ) ;
	const char *GetBone() const ;
	void SetOffset( const mat4 &offset ) ;
	mat4 GetOffset() const ;

	void SetBoneRef( const MdxBone *bone ) ;
	MdxBone *GetBoneRef() const ;
} ;

//----------------------------------------------------------------
//  MorphIndex command
//----------------------------------------------------------------

class MdxMorphIndex : public MdxCommand {
public:
	MdxMorphIndex( float index = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_MORPH_INDEX ; }
	virtual const char *GetTypeName() const { return "MorphIndex" ; }
	virtual MdxChunk *Clone() const { return new MdxMorphIndex( *this ) ; }

	void SetIndex( float index ) ;
	float GetIndex() const ;
} ;

//----------------------------------------------------------------
//  MorphWeights command
//----------------------------------------------------------------

class MdxMorphWeights : public MdxCommand {
public:
	MdxMorphWeights( int n_weights = 0, const float *weights = 0 ) ;
	virtual int GetTypeID() const { return MDX_MORPH_WEIGHTS ; }
	virtual const char *GetTypeName() const { return "MorphWeights" ; }
	virtual MdxChunk *Clone() const { return new MdxMorphWeights( *this ) ; }
	virtual void SetArgsInt( int num, int val ) ;

	void SetWeightCount( int num ) ;
	void SetWeight( int num, float weight ) ;
	int GetWeightCount() const ;
	float GetWeight( int num ) const ;
} ;

//----------------------------------------------------------------
//  DrawPart command
//----------------------------------------------------------------

class MdxDrawPart : public MdxCommand {
public:
	MdxDrawPart( const char *part = 0 ) ;
	virtual int GetTypeID() const { return MDX_DRAW_PART ; }
	virtual const char *GetTypeName() const { return "DrawPart" ; }
	virtual MdxChunk *Clone() const { return new MdxDrawPart( *this ) ; }

	void SetPart( const char *part ) ;
	const char *GetPart() const ;

	void SetPartRef( const MdxPart *part ) ;
	MdxPart *GetPartRef() const ;
} ;


} // namespace mdx

#endif
