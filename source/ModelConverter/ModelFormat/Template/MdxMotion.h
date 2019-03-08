#ifndef	MDX_MOTION_H_INCLUDE
#define MDX_MOTION_H_INCLUDE

namespace mdx {

//----------------------------------------------------------------
//  Motion block
//----------------------------------------------------------------

class MdxMotion : public MdxBlock {
public:
	MdxMotion( const char *name = 0 ) ;
	virtual ~MdxMotion() ;
	MdxMotion( const MdxMotion &src ) ;
	MdxMotion &operator=( const MdxMotion &src ) ;

	virtual int GetTypeID() const { return MDX_MOTION ; }
	virtual const char *GetTypeName() const { return "Motion" ; }
	virtual MdxChunk *Clone() const { return new MdxMotion( *this ) ; }
	virtual MdxChunk *Copy( const MdxChunk *src ) ;
	virtual bool Equals( const MdxChunk *src ) const ;

private:
	class impl ;
	impl *m_impl ;	
} ;

//----------------------------------------------------------------
//  FrameLoop command
//----------------------------------------------------------------

class MdxFrameLoop : public MdxCommand {
public:
	MdxFrameLoop( float start = 0.0f, float end = 0.0f ) ;
	virtual int GetTypeID() const { return MDX_FRAME_LOOP ; }
	virtual const char *GetTypeName() const { return "FrameLoop" ; }
	virtual MdxChunk *Clone() const { return new MdxFrameLoop( *this ) ; }

	void SetStart( float start ) ;
	void SetEnd( float end ) ;
	float GetStart() const ;
	float GetEnd() const ;
} ;

//----------------------------------------------------------------
//  FrameRate command
//----------------------------------------------------------------

class MdxFrameRate : public MdxCommand {
public:
	MdxFrameRate( float fps = 60.0f ) ;
	virtual int GetTypeID() const { return MDX_FRAME_RATE ; }
	virtual const char *GetTypeName() const { return "FrameRate" ; }
	virtual MdxChunk *Clone() const { return new MdxFrameRate( *this ) ; }

	void SetFPS( float fps ) ;
	float GetFPS() const ;
} ;

//----------------------------------------------------------------
//  FrameRepeat command
//----------------------------------------------------------------

class MdxFrameRepeat : public MdxCommand {
public:
	MdxFrameRepeat( int mode = MDX_REPEAT_CYCLE ) ;
	virtual int GetTypeID() const { return MDX_FRAME_REPEAT ; }
	virtual const char *GetTypeName() const { return "FrameRepeat" ; }
	virtual MdxChunk *Clone() const { return new MdxFrameRepeat( *this ) ; }

	void SetMode( int mode ) ;
	int GetMode() const ;
} ;

//----------------------------------------------------------------
//  Animate command
//----------------------------------------------------------------

class MdxAnimate : public MdxCommand {
public:
	MdxAnimate( int scope = MDX_BONE,
	            const char *block = 0,
	            int command = 0,
	            int mode = 0,
	            const char *fcurve = 0 ) ;
	virtual int GetTypeID() const { return MDX_ANIMATE ; }
	virtual const char *GetTypeName() const { return "Animate" ; }
	virtual MdxChunk *Clone() const { return new MdxAnimate( *this ) ; }

	void SetScope( int scope ) ;
	void SetBlock( const char *block ) ;
	void SetCommand( int cmd ) ;
	void SetMode( int mode ) ;
	void SetFCurve( const char *fcurve ) ;

	int GetScope() const ;
	const char *GetBlock() const ;
	int GetCommand() const ;
	int GetMode() const ;
	const char *GetFCurve() const ;

	//  convenience functions

	void SetBlockRef( const MdxBlock *block ) ;
	MdxBlock *GetBlockRef() const ;
	void SetFCurveRef( const MdxFCurve *fcurve ) ;
	MdxFCurve *GetFCurveRef() const ;

	void SetIndex( int index ) ;		// SetMode()
	int GetIndex() const ;			// GetMode()
	void SetOffset( int offset ) ;		// SetMode()
	int GetOffset() const ;			// GetMode()
} ;


} // namespace mdx

#endif
