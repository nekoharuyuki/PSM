#include "ModelTool.h"

namespace mdx {

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

enum {
	HAS_ARGS_REF	= 0x0001,
	HAS_DATA_REF	= 0x0002,
} ;

class ChunkWord {
public:
	ChunkWord() ;
	~ChunkWord() ;
	ChunkWord( const ChunkWord &src ) ;
	ChunkWord &operator=( const ChunkWord &src ) ;
	bool operator==( const ChunkWord &src ) const ;

	void Clear() ;
	int GetDesc() const ;
	void SetDesc( int desc ) ;
	int GetInt() const ;
	void SetInt( int val ) ;
	float GetFloat() const ;
	void SetFloat( float val ) ;
	const char *GetString() const ;
	void SetString( const char *val ) ;
public:
	int m_desc ;
	union {
		unsigned int m_bits ;
		int m_int ;
		float m_float ;
		char *m_string ;
	} ;
} ;

class MdxChunk::impl {
public:
	int m_flags ;
	string m_name ;
	MdxChunk *m_parent ;
	vector<MdxChunk *> m_child ;
	vector<ChunkWord> m_args ;
	vector<ChunkWord> m_data ;
	int m_implicit_args_desc ;
	int m_implicit_data_desc ;
} ;

static vector<char> g_args_buf ;
static vector<char> g_data_buf ;

//----------------------------------------------------------------
//  MdxChunk
//----------------------------------------------------------------

MdxChunk::MdxChunk()
{
	m_impl = new impl ;
	m_impl->m_flags = 0 ;
	m_impl->m_parent = 0 ;
	m_impl->m_implicit_args_desc = 0 ;
	m_impl->m_implicit_data_desc = 0 ;
}

MdxChunk::~MdxChunk()
{
	ClearChild() ;
	if ( m_impl->m_parent != 0 ) {
		m_impl->m_parent->DetachChild( this ) ;
	}
	delete m_impl ;
}

MdxChunk::MdxChunk( const MdxChunk &src )
{
	m_impl = new impl ;
	m_impl->m_flags = 0 ;
	m_impl->m_parent = 0 ;
	m_impl->m_implicit_args_desc = 0 ;
	m_impl->m_implicit_data_desc = 0 ;
	*this = src ;
}

MdxChunk &MdxChunk::operator=( const MdxChunk &src )
{
	Copy( &src ) ;
	return *this ;
}

bool MdxChunk::operator==( const MdxChunk &src ) const
{
	return Equals( &src ) ;
}

bool MdxChunk::operator!=( const MdxChunk &src ) const
{
	return !Equals( &src ) ;
}

//----------------------------------------------------------------
//  copy and compare
//----------------------------------------------------------------

MdxChunk *MdxChunk::Clone() const
{
	return new MdxChunk( *this ) ;
}

MdxChunk *MdxChunk::Copy( const MdxChunk *src )
{
	if ( src == 0 ) return this ;
	if ( src == this ) return this ;
	//// if ( this->GetFormatID() != src->GetFormatID() ) return this ;
	//// if ( this->GetTypeID() != src->GetTypeID() ) return this ;
	m_impl->m_flags = src->m_impl->m_flags ;
	m_impl->m_name = src->m_impl->m_name ;
	//// m_parent
	//// m_child
	m_impl->m_args = src->m_impl->m_args ;
	m_impl->m_data = src->m_impl->m_data ;
	m_impl->m_implicit_args_desc = src->m_impl->m_implicit_args_desc ;
	m_impl->m_implicit_data_desc = src->m_impl->m_implicit_data_desc ;
	return this ;
}

bool MdxChunk::Equals( const MdxChunk *src ) const
{
	if ( src == 0 ) return false ;
	if ( src == this ) return true ;
	if ( GetFormatID() != src->GetFormatID() ) return false ;
	if ( GetTypeID() != src->GetTypeID() ) return false ;
	//// m_flags
	//// m_name
	//// m_parent
	//// m_child
	if ( m_impl->m_args != src->m_impl->m_args ) return false ;
	if ( m_impl->m_data != src->m_impl->m_data ) return false ;
	//// m_implicit_args_desc
	//// m_implicit_data_desc
	return true ;
}

MdxChunk *MdxChunk::CloneTree() const
{
	MdxChunk *chunk = Clone() ;
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		chunk->AttachChild( chunks[ i ]->CloneTree() ) ;
	}
	return chunk ;
}

MdxChunk *MdxChunk::CopyTree( const MdxChunk *src )
{
	if ( src == 0 ) return this ;
	if ( src == this ) return this ;
	Copy( src ) ;
	ClearChild() ;
	vector<MdxChunk *> &chunks = src->m_impl->m_child ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		AttachChild( chunks[ i ]->CloneTree() ) ;
	}
	return this ;
}

bool MdxChunk::EqualsTree( const MdxChunk *src ) const
{
	if ( src == 0 ) return false ;
	if ( src == this ) return true ;
	if ( !Equals( src ) ) return false ;
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	vector<MdxChunk *> &chunks2 = src->m_impl->m_child ;
	if ( chunks.size() != chunks2.size() ) return false ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		if ( !chunks[ i ]->EqualsTree( chunks2[ i ] ) ) return false ;
	}
	return true ;
}

//----------------------------------------------------------------
//  name
//----------------------------------------------------------------

void MdxChunk::SetName( const char *name )
{
	if ( name == 0 ) name = "" ;
	m_impl->m_name = name ;
}

const char *MdxChunk::GetName() const
{
	return m_impl->m_name.c_str() ;
}

const char *MdxChunk::UniqName( const char *prefix )
{
	MdxChunk *parent = m_impl->m_parent ;
	if ( parent == 0 ) return GetName() ;
	int type = GetTypeID() ;
	string name = GetName() ;
	string base = name ;
	SetName( "" ) ;
	if ( prefix != 0 && prefix[ 0 ] != '\0' ) {
		int index = parent->GetChildCount( type ) - 1 ;
		base = prefix ;
		name = str_format( "%s-%d", base.c_str(), index ) ;
	} else if ( name == "" ) {
		int index = parent->GetChildCount( type ) - 1 ;
		base = str_untitle( GetTypeName() ) ;
		name = str_format( "%s-%d", base.c_str(), index ) ;
	}
	int suffix = 0 ;
	while ( parent->IndexOfChild( type, name.c_str() ) >= 0 ) {
		name = str_format( "%s-%04d", base.c_str(), suffix ++ ) ;
	}
	SetName( name.c_str() ) ;
	return GetName() ;
}

//----------------------------------------------------------------
//  args
//----------------------------------------------------------------

bool MdxChunk::UpdateArgs()
{
	//  reflect internal information

	return true ;
}

bool MdxChunk::FlushArgs()
{
	//  modify internal information

	return true ;
}

bool MdxChunk::SetArgsImage( const void *buf, int size, int style, int option )
{
	if ( buf == 0 || size < 0 ) return false ;
	vector<char> tmp ;
	tmp.resize( ( size + 3 ) / 4 * 4 ) ;
	char *buf2 = tmp.empty() ? 0 : &( tmp[ 0 ] ) ;
	memcpy( buf2, buf, size ) ;

	int num = 0 ;
	const char *end = buf2 + size ;
	while ( buf2 < end ) {
		int desc = GetArgsDesc( num ) ;
		if ( num >= GetArgsCount() && !( MDX_WORD_VAR_COUNT & desc ) ) break ;
		if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) {
			buf2 = (char *)( ( (int)buf2 + 3 ) / 4 * 4 ) ;
			if ( buf2 >= end ) break ;
			int ref = *(int *)buf2 ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_REF_TYPE( ref )
			  && ( MDX_WORD_VAR_SCOPE & desc ) ) {
				desc &= ~MDX_WORD_SCOPE_MASK ;
				desc |= MDX_REF_TYPE( ref ) ;
				SetArgsDesc( num, desc ) ;
			}
			//  reference cannot be resolved here
			string name = ( ref < 0 ) ? "" : str_format( "0x%04x", ref & ~MDX_REF_TYPE_MASK ) ;
			SetArgsString( num, name.c_str() ) ;
			buf2 += 4 ;
			num ++ ;
			continue ;
		}
		switch ( MDX_WORD_VALUE_TYPE( desc ) ) {
		    case MDX_WORD_INT32 :
		    case MDX_WORD_UINT32 :
			buf2 = (char *)( ( (int)buf2 + 3 ) / 4 * 4 ) ;
			if ( buf2 >= end ) break ;
			SetArgsInt( num, *(int *)buf2 ) ;
			buf2 += 4 ;
			break ;
		    case MDX_WORD_INT16 :
		    case MDX_WORD_UINT16 :
			buf2 = (char *)( ( (int)buf2 + 1 ) / 2 * 2 ) ;
			if ( buf2 >= end ) break ;
			SetArgsInt( num, *(short *)buf2 ) ;
			buf2 += 2 ;
			break ;
		    case MDX_WORD_INT8 :
		    case MDX_WORD_UINT8 :
			SetArgsInt( num, *( buf2 ++ ) ) ;
			break ;
		    case MDX_WORD_FLOAT32 :
			buf2 = (char *)( ( (int)buf2 + 3 ) / 4 * 4 ) ;
			if ( buf2 >= end ) break ;
			SetArgsFloat( num, *(float *)buf2 ) ;
			buf2 += 4 ;
			break ;
		    case MDX_WORD_FLOAT16 :
			buf2 = (char *)( ( (int)buf2 + 1 ) / 2 * 2 ) ;
			if ( buf2 >= end ) break ;
			SetArgsFloat( num, *(float16 *)buf2 ) ;
			buf2 += 2 ;
			break ;
		    case MDX_WORD_STRING8 :
			SetArgsString( num, buf2 ) ;
			buf2 += strlen( buf2 ) + 1 ;
			break ;
		}
		num ++ ;
	}
	return true ;
}

bool MdxChunk::GetArgsImage( const void *&buf, int &size, int style, int option ) const
{
	g_args_buf.clear() ;
	int offset = 0 ;
	for ( int i = 0 ; i < GetArgsCount() ; i ++ ) {
		int desc = GetArgsDesc( i ) ;
		if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) {
			int scope = MDX_WORD_SCOPE( desc ) ;
			const char *name = GetArgsString( i ) ;
			int ref = IndexOfRef( scope, name ) ;
			offset = ( offset + 3 ) / 4 * 4 ;
			g_args_buf.resize( offset + 4 ) ;
			*(int *)&( g_args_buf[ offset ] ) = ref ;
			offset += 4 ;
			continue ;
		}
		switch ( MDX_WORD_VALUE_TYPE( desc ) ) {
		    case MDX_WORD_INT32 :
		    case MDX_WORD_UINT32 :
			offset = ( offset + 3 ) / 4 * 4 ;
			g_args_buf.resize( offset + 4 ) ;
			*(int *)&( g_args_buf[ offset ] ) = GetArgsInt( i ) ;
			offset += 4 ;
			break ;
		    case MDX_WORD_INT16 :
		    case MDX_WORD_UINT16 :
			offset = ( offset + 1 ) / 2 * 2 ;
			g_args_buf.resize( offset + 2 ) ;
			*(short *)&( g_args_buf[ offset ] ) = GetArgsInt( i ) ;
			offset += 2 ;
			break ;
		    case MDX_WORD_INT8 :
		    case MDX_WORD_UINT8 :
			g_args_buf.resize( offset + 1 ) ;
			*(short *)&( g_args_buf[ offset ] ) = GetArgsInt( i ) ;
			offset += 1 ;
			break ;
		    case MDX_WORD_FLOAT32 :
			offset = ( offset + 3 ) / 4 * 4 ;
			g_args_buf.resize( offset + 4 ) ;
			*(float *)&( g_args_buf[ offset ] ) = GetArgsFloat( i ) ;
			offset += 4 ;
			break ;
		    case MDX_WORD_FLOAT16 :
			offset = ( offset + 1 ) / 2 * 2 ;
			g_args_buf.resize( offset + 2 ) ;
			*(float16 *)&( g_args_buf[ offset ] ) = GetArgsFloat( i ) ;
			offset += 2 ;
			break ;
		    case MDX_WORD_STRING8 : {
			const char *val = GetArgsString( i ) ;
			int len = strlen( val ) + 1 ;
			g_args_buf.resize( offset + len ) ;
			memcpy( &( g_args_buf[ offset ] ), val, len ) ;
			offset += len ;
			break ;
		    }
		}
	}
	g_args_buf.resize( ( offset + 3 ) / 4 * 4 ) ;
	buf = g_args_buf.empty() ? 0 : (const void *)&( g_args_buf[ 0 ] ) ;
	size = offset ;
	return ( size > 0 ) ;
}

void MdxChunk::SetArgsCount( int count )
{
	if ( count < 0 ) return ;
	int from = m_impl->m_args.size() ;
	m_impl->m_args.resize( count ) ;
	for ( int i = from ; i < count ; i ++ ) {
		m_impl->m_args[ i ].SetDesc( m_impl->m_implicit_args_desc ) ;
	}
}

int MdxChunk::GetArgsCount() const
{
	return m_impl->m_args.size() ;
}

void MdxChunk::SetArgsDesc( int num, int desc )
{
	if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) m_impl->m_flags |= HAS_ARGS_REF ;
	if ( num < 0 || ( desc & MDX_WORD_VAR_COUNT ) != 0 ) {
		m_impl->m_implicit_args_desc = desc ;
		return ;
	}
	if ( num >= (int)m_impl->m_args.size() ) SetArgsCount( num + 1 ) ;
	m_impl->m_args[ num ].SetDesc( desc ) ;
}

int MdxChunk::GetArgsDesc( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_args.size() )  {
		return m_impl->m_implicit_args_desc ;
	}
	return m_impl->m_args[ num ].GetDesc() ;
}

void MdxChunk::SetArgsInt( int num, int val )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_impl->m_args.size() ) SetArgsCount( num + 1 ) ;
	m_impl->m_args[ num ].SetInt( val ) ;
}

int MdxChunk::GetArgsInt( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_args.size() ) return 0 ;
	return m_impl->m_args[ num ].GetInt() ;
}

void MdxChunk::SetArgsFloat( int num, float val )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_impl->m_args.size() ) SetArgsCount( num + 1 ) ;
	m_impl->m_args[ num ].SetFloat( val ) ;
}

float MdxChunk::GetArgsFloat( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_args.size() ) return 0.0f ;
	return m_impl->m_args[ num ].GetFloat() ;
}

void MdxChunk::SetArgsString( int num, const char *val )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_impl->m_args.size() ) SetArgsCount( num + 1 ) ;
	m_impl->m_args[ num ].SetString( val ) ;
}

const char *MdxChunk::GetArgsString( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_args.size() ) return "" ;
	return m_impl->m_args[ num ].GetString() ;
}

void MdxChunk::SetArgsVec2( int num, const vec2 &v )
{
	SetArgsFloat( num + 0, v.x ) ;
	SetArgsFloat( num + 1, v.y ) ;
}

vec2 MdxChunk::GetArgsVec2( int num ) const
{
	return vec2( GetArgsFloat( num + 0 ), GetArgsFloat( num + 1 ) ) ;
}

void MdxChunk::SetArgsVec3( int num, const vec3 &v )
{
	SetArgsFloat( num + 0, v.x ) ;
	SetArgsFloat( num + 1, v.y ) ;
	SetArgsFloat( num + 2, v.z ) ;
}

vec3 MdxChunk::GetArgsVec3( int num ) const
{
	return vec3( GetArgsFloat( num + 0 ), GetArgsFloat( num + 1 ),
	             GetArgsFloat( num + 2 ) ) ;
}

void MdxChunk::SetArgsVec4( int num, const vec4 &v )
{
	SetArgsFloat( num + 0, v.x ) ;
	SetArgsFloat( num + 1, v.y ) ;
	SetArgsFloat( num + 2, v.z ) ;
	SetArgsFloat( num + 3, v.w ) ;
}

vec4 MdxChunk::GetArgsVec4( int num ) const
{
	return vec4( GetArgsFloat( num + 0 ), GetArgsFloat( num + 1 ),
	             GetArgsFloat( num + 2 ), GetArgsFloat( num + 3 ) ) ;
}

void MdxChunk::SetArgsMat4( int num, const mat4 &m )
{
	const float *fp = (const float *)&m ;
	for ( int i = 0 ; i < 16 ; i ++ ) SetArgsFloat( num ++, *( fp ++ ) ) ;
}

mat4 MdxChunk::GetArgsMat4( int num ) const
{
	mat4 m ;
	float *fp = (float *)&m ;
	for ( int i = 0 ; i < 16 ; i ++ ) *( fp ++ ) = GetArgsFloat( num ++ ) ;
	return m ;
}

void MdxChunk::SetArgsQuat( int num, const quat &q )
{
	SetArgsFloat( num + 0, q.x ) ;
	SetArgsFloat( num + 1, q.y ) ;
	SetArgsFloat( num + 2, q.z ) ;
	SetArgsFloat( num + 3, q.w ) ;
}

quat MdxChunk::GetArgsQuat( int num ) const
{
	return quat( GetArgsFloat( num + 0 ), GetArgsFloat( num + 1 ),
	             GetArgsFloat( num + 2 ), GetArgsFloat( num + 3 ) ) ;
}

void MdxChunk::SetArgsRect( int num, const rect &r )
{
	SetArgsFloat( num + 0, r.x ) ;
	SetArgsFloat( num + 1, r.y ) ;
	SetArgsFloat( num + 2, r.w ) ;
	SetArgsFloat( num + 3, r.h ) ;
}

rect MdxChunk::GetArgsRect( int num ) const
{
	return rect( GetArgsFloat( num + 0 ), GetArgsFloat( num + 1 ),
	             GetArgsFloat( num + 2 ), GetArgsFloat( num + 3 ) ) ;
}

void MdxChunk::SetArgsRef( int num, const MdxChunk *ref )
{
	int desc = GetArgsDesc( num ) ;
	if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) return ;
	if ( ref == 0 ) {
		SetArgsString( num, 0 ) ;
		return ;
	}
	int scope = ref->GetTypeID() ;
	if ( MDX_WORD_SCOPE( desc ) != scope ) {
		if ( !( MDX_WORD_VAR_SCOPE & desc ) ) return ;
		SetArgsDesc( num, desc & ~MDX_WORD_SCOPE_MASK | scope ) ;
	}
	SetArgsString( num, ref->GetName() ) ;
}

MdxChunk *MdxChunk::GetArgsRef( int num ) const
{
	int desc = GetArgsDesc( num ) ;
	if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) return 0 ;
	int scope = MDX_WORD_SCOPE( desc ) ;
	const char *name = GetArgsString( num ) ;
	return FindRef( scope, name ) ;
}

bool MdxChunk::HasArgsRef() const
{
	return ( ( HAS_ARGS_REF & m_impl->m_flags ) != 0 ) ;
}

//----------------------------------------------------------------
//  data
//----------------------------------------------------------------

bool MdxChunk::UpdateData()
{
	//  reflect internal information

	return true ;
}

bool MdxChunk::FlushData()
{
	//  modify internal information

	return true ;
}

bool MdxChunk::SetDataImage( const void *buf, int size, int style, int option )
{
	if ( buf == 0 || size < 0 ) return false ;
	vector<char> tmp ;
	tmp.resize( ( size + 3 ) / 4 * 4 ) ;
	char *buf2 = tmp.empty() ? 0 : &( tmp[ 0 ] ) ;
	memcpy( buf2, buf, size ) ;

	int num = 0 ;
	const char *end = buf2 + size ;
	while ( buf2 < end ) {
		int desc = GetDataDesc( num ) ;
		if ( num >= GetDataCount() && !( MDX_WORD_VAR_COUNT & desc ) ) break ;
		if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) {
			buf2 = (char *)( ( (int)buf2 + 3 ) / 4 * 4 ) ;
			if ( buf2 >= end ) break ;
			int ref = *(int *)buf2 ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_REF_TYPE( ref )
			  && ( MDX_WORD_VAR_SCOPE & desc ) ) {
				desc &= ~MDX_WORD_SCOPE_MASK ;
				desc |= MDX_REF_TYPE( ref ) ;
				SetDataDesc( num, desc ) ;
			}
			//  reference cannot be resolved here
			string name = ( ref < 0 ) ? "" : str_format( "0x%04x", ref & ~MDX_REF_TYPE_MASK ) ;
			SetDataString( num, name.c_str() ) ;
			buf2 += 4 ;
			num ++ ;
			continue ;
		}
		switch ( MDX_WORD_VALUE_TYPE( desc ) ) {
		    case MDX_WORD_INT32 :
		    case MDX_WORD_UINT32 :
			buf2 = (char *)( ( (int)buf2 + 3 ) / 4 * 4 ) ;
			if ( buf2 >= end ) break ;
			SetDataInt( num, *(int *)buf2 ) ;
			buf2 += 4 ;
			break ;
		    case MDX_WORD_INT16 :
		    case MDX_WORD_UINT16 :
			buf2 = (char *)( ( (int)buf2 + 1 ) / 2 * 2 ) ;
			if ( buf2 >= end ) break ;
			SetDataInt( num, *(short *)buf2 ) ;
			buf2 += 2 ;
			break ;
		    case MDX_WORD_INT8 :
		    case MDX_WORD_UINT8 :
			SetDataInt( num, *( buf2 ++ ) ) ;
			break ;
		    case MDX_WORD_FLOAT32 :
			buf2 = (char *)( ( (int)buf2 + 3 ) / 4 * 4 ) ;
			if ( buf2 >= end ) break ;
			SetDataFloat( num, *(float *)buf2 ) ;
			buf2 += 4 ;
			break ;
		    case MDX_WORD_FLOAT16 :
			buf2 = (char *)( ( (int)buf2 + 1 ) / 2 * 2 ) ;
			if ( buf2 >= end ) break ;
			SetDataFloat( num, *(float16 *)buf2 ) ;
			buf2 += 2 ;
			break ;
		    case MDX_WORD_STRING8 :
			SetDataString( num, buf2 ) ;
			buf2 += strlen( buf2 ) + 1 ;
			break ;
		}
		num ++ ;
	}
	return true ;
}

bool MdxChunk::GetDataImage( const void *&buf, int &size, int style, int option ) const
{
	g_data_buf.clear() ;
	int offset = 0 ;
	for ( int i = 0 ; i < GetDataCount() ; i ++ ) {
		int desc = GetDataDesc( i ) ;
		if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) {
			int scope = MDX_WORD_SCOPE( desc ) ;
			const char *name = GetDataString( i ) ;
			int ref = IndexOfRef( scope, name ) ;
			offset = ( offset + 3 ) / 4 * 4 ;
			g_data_buf.resize( offset + 4 ) ;
			*(int *)&( g_data_buf[ offset ] ) = ref ;
			offset += 4 ;
			continue ;
		}
		switch ( MDX_WORD_VALUE_TYPE( desc ) ) {
		    case MDX_WORD_INT32 :
		    case MDX_WORD_UINT32 :
			offset = ( offset + 3 ) / 4 * 4 ;
			g_data_buf.resize( offset + 4 ) ;
			*(int *)&( g_data_buf[ offset ] ) = GetDataInt( i ) ;
			offset += 4 ;
			break ;
		    case MDX_WORD_INT16 :
		    case MDX_WORD_UINT16 :
			offset = ( offset + 1 ) / 2 * 2 ;
			g_data_buf.resize( offset + 2 ) ;
			*(short *)&( g_data_buf[ offset ] ) = GetDataInt( i ) ;
			offset += 2 ;
			break ;
		    case MDX_WORD_INT8 :
		    case MDX_WORD_UINT8 :
			g_data_buf.resize( offset + 1 ) ;
			*(short *)&( g_data_buf[ offset ] ) = GetDataInt( i ) ;
			offset += 1 ;
			break ;
		    case MDX_WORD_FLOAT32 :
			offset = ( offset + 3 ) / 4 * 4 ;
			g_data_buf.resize( offset + 4 ) ;
			*(float *)&( g_data_buf[ offset ] ) = GetDataFloat( i ) ;
			offset += 4 ;
			break ;
		    case MDX_WORD_FLOAT16 :
			offset = ( offset + 1 ) / 2 * 2 ;
			g_data_buf.resize( offset + 2 ) ;
			*(float16 *)&( g_data_buf[ offset ] ) = GetDataFloat( i ) ;
			offset += 2 ;
			break ;
		    case MDX_WORD_STRING8 : {
			const char *val = GetDataString( i ) ;
			int len = strlen( val ) + 1 ;
			g_data_buf.resize( offset + len ) ;
			memcpy( &( g_data_buf[ offset ] ), val, len ) ;
			offset += len ;
			break ;
		    }
		}
	}
	g_data_buf.resize( ( offset + 3 ) / 4 * 4 ) ;
	buf = g_data_buf.empty() ? 0 : (const void *)&( g_data_buf[ 0 ] ) ;
	size = offset ;
	return ( size > 0 ) ;
}

void MdxChunk::SetDataCount( int count )
{
	if ( count < 0 ) return ;
	int from = m_impl->m_data.size() ;
	m_impl->m_data.resize( count ) ;
	for ( int i = from ; i < count ; i ++ ) {
		m_impl->m_data[ i ].SetDesc( m_impl->m_implicit_data_desc ) ;
	}
}

int MdxChunk::GetDataCount() const
{
	return m_impl->m_data.size() ;
}

void MdxChunk::SetDataDesc( int num, int desc )
{
	if ( MDX_WORD_CLASS( desc ) == MDX_WORD_REF ) m_impl->m_flags |= HAS_DATA_REF ;
	if ( num < 0 || ( desc & MDX_WORD_VAR_COUNT ) != 0 ) {
		m_impl->m_implicit_data_desc = desc ;
		return ;
	}
	if ( num >= (int)m_impl->m_data.size() ) SetDataCount( num + 1 ) ;
	m_impl->m_data[ num ].SetDesc( desc ) ;
}

int MdxChunk::GetDataDesc( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_data.size() )  {
		return m_impl->m_implicit_data_desc ;
	}
	return m_impl->m_data[ num ].GetDesc() ;
}

void MdxChunk::SetDataInt( int num, int val )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_impl->m_data.size() ) SetDataCount( num + 1 ) ;
	m_impl->m_data[ num ].SetInt( val ) ;
}

int MdxChunk::GetDataInt( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_data.size() ) return 0 ;
	return m_impl->m_data[ num ].GetInt() ;
}

void MdxChunk::SetDataFloat( int num, float val )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_impl->m_data.size() ) SetDataCount( num + 1 ) ;
	m_impl->m_data[ num ].SetFloat( val ) ;
}

float MdxChunk::GetDataFloat( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_data.size() ) return 0.0f ;
	return m_impl->m_data[ num ].GetFloat() ;
}

void MdxChunk::SetDataString( int num, const char *val )
{
	if ( num < 0 ) return ;
	if ( num >= (int)m_impl->m_data.size() ) SetDataCount( num + 1 ) ;
	m_impl->m_data[ num ].SetString( val ) ;
}

const char *MdxChunk::GetDataString( int num ) const
{
	if ( num < 0 || num >= (int)m_impl->m_data.size() ) return "" ;
	return m_impl->m_data[ num ].GetString() ;
}

void MdxChunk::SetDataVec2( int num, const vec2 &v )
{
	SetDataFloat( num + 0, v.x ) ;
	SetDataFloat( num + 1, v.y ) ;
}

vec2 MdxChunk::GetDataVec2( int num ) const
{
	return vec2( GetDataFloat( num + 0 ), GetDataFloat( num + 1 ) ) ;
}

void MdxChunk::SetDataVec3( int num, const vec3 &v )
{
	SetDataFloat( num + 0, v.x ) ;
	SetDataFloat( num + 1, v.y ) ;
	SetDataFloat( num + 2, v.z ) ;
}

vec3 MdxChunk::GetDataVec3( int num ) const
{
	return vec3( GetDataFloat( num + 0 ), GetDataFloat( num + 1 ),
	             GetDataFloat( num + 2 ) ) ;
}

void MdxChunk::SetDataVec4( int num, const vec4 &v )
{
	SetDataFloat( num + 0, v.x ) ;
	SetDataFloat( num + 1, v.y ) ;
	SetDataFloat( num + 2, v.z ) ;
	SetDataFloat( num + 3, v.w ) ;
}

vec4 MdxChunk::GetDataVec4( int num ) const
{
	return vec4( GetDataFloat( num + 0 ), GetDataFloat( num + 1 ),
	             GetDataFloat( num + 2 ), GetDataFloat( num + 3 ) ) ;
}

void MdxChunk::SetDataMat4( int num, const mat4 &m )
{
	const float *fp = (const float *)&m ;
	for ( int i = 0 ; i < 16 ; i ++ ) SetDataFloat( num ++, *( fp ++ ) ) ;
}

mat4 MdxChunk::GetDataMat4( int num ) const
{
	mat4 m ;
	float *fp = (float *)&m ;
	for ( int i = 0 ; i < 16 ; i ++ ) *( fp ++ ) = GetDataFloat( num ++ ) ;
	return m ;
}

void MdxChunk::SetDataQuat( int num, const quat &q )
{
	SetDataFloat( num + 0, q.x ) ;
	SetDataFloat( num + 1, q.y ) ;
	SetDataFloat( num + 2, q.z ) ;
	SetDataFloat( num + 3, q.w ) ;
}

quat MdxChunk::GetDataQuat( int num ) const
{
	return quat( GetDataFloat( num + 0 ), GetDataFloat( num + 1 ),
	             GetDataFloat( num + 2 ), GetDataFloat( num + 3 ) ) ;
}

void MdxChunk::SetDataRect( int num, const rect &r )
{
	SetDataFloat( num + 0, r.x ) ;
	SetDataFloat( num + 1, r.y ) ;
	SetDataFloat( num + 2, r.w ) ;
	SetDataFloat( num + 3, r.h ) ;
}

rect MdxChunk::GetDataRect( int num ) const
{
	return rect( GetDataFloat( num + 0 ), GetDataFloat( num + 1 ),
	             GetDataFloat( num + 2 ), GetDataFloat( num + 3 ) ) ;
}

void MdxChunk::SetDataRef( int num, const MdxChunk *ref )
{
	int desc = GetDataDesc( num ) ;
	if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) return ;
	if ( ref == 0 ) {
		SetDataString( num, 0 ) ;
		return ;
	}
	int scope = ref->GetTypeID() ;
	if ( MDX_WORD_SCOPE( desc ) != scope ) {
		if ( !( MDX_WORD_VAR_SCOPE & desc ) ) return ;
		SetDataDesc( num, desc & ~MDX_WORD_SCOPE_MASK | scope ) ;
	}
	SetDataString( num, ref->GetName() ) ;
}

MdxChunk *MdxChunk::GetDataRef( int num ) const
{
	int desc = GetDataDesc( num ) ;
	if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) return 0 ;
	int scope = MDX_WORD_SCOPE( desc ) ;
	const char *name = GetDataString( num ) ;
	return FindRef( scope, name ) ;
}

bool MdxChunk::HasDataRef() const
{
	return ( ( HAS_DATA_REF & m_impl->m_flags ) != 0 ) ;
}

//----------------------------------------------------------------
//  hierarchy
//----------------------------------------------------------------

MdxChunk *MdxChunk::GetParent() const
{
	return m_impl->m_parent ;
}

MdxChunk *MdxChunk::GetRoot( int type ) const
{
	MdxChunk *chunk = (MdxChunk *)this ;
	if ( type == 0 ) {
		while ( chunk->m_impl->m_parent != 0 ) {
			chunk = chunk->m_impl->m_parent ;
		}
	} else {
		while ( chunk->GetTypeID() != type ) {
			chunk = chunk->m_impl->m_parent ;
			if ( chunk == 0 ) break ;
		}
	}
	return chunk ;
}

void MdxChunk::ClearChild( int type )
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = chunks.size() - 1 ; i >= 0 ; -- i ) {
		MdxChunk *chunk = chunks[ i ] ;
		if ( type == 0 || chunk->GetTypeID() == type ) {
			DetachChild( i ) ;
			chunk->Release() ;
		}
	}
}

void MdxChunk::ClearTree( int type )
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = chunks.size() - 1 ; i >= 0 ; -- i ) {
		MdxChunk *chunk = chunks[ i ] ;
		if ( type == 0 || chunk->GetTypeID() == type ) {
			DetachChild( i ) ;
			chunk->Release() ;
		} else {
			chunk->ClearTree( type ) ;
		}
	}
}

int MdxChunk::GetChildCount( int type ) const
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	if ( type == 0 ) return chunks.size() ;
	int count = 0 ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		if ( chunks[ i ]->GetTypeID() == type ) count ++ ;
	}
	return count ;
}

MdxChunk *MdxChunk::GetChild( int num ) const
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	if ( num < 0 || num >= (int)chunks.size() ) return 0 ;
	return chunks[ num ] ;
}

void MdxChunk::AttachChild( MdxChunk *chunk )
{
	InsertChild( GetChildCount(), chunk ) ;
}

void MdxChunk::DetachChild( int num )
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	if ( num < 0 || num >= (int)chunks.size() ) return ;
	MdxChunk *chunk = chunks[ num ] ;
	chunks.erase( chunks.begin() + num ) ;
	chunk->m_impl->m_parent = 0 ;
}

void MdxChunk::DetachChild( MdxChunk *chunk )
{
	DetachChild( IndexOfChild( chunk ) ) ;
}

void MdxChunk::InsertChild( int num, MdxChunk *chunk )
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	if ( num < 0 || num > (int)chunks.size() ) return ;
	MdxChunk *parent = chunk->m_impl->m_parent ;
	if ( parent == this ) return ;
	if ( parent != 0 ) parent->DetachChild( chunk ) ;
	chunks.insert( chunks.begin() + num, chunk ) ;
	chunk->m_impl->m_parent = this ;
	if ( !chunk->IsCommand() ) chunk->UniqName() ;
}

void MdxChunk::DeleteChild( int num )
{
	MdxChunk *chunk = GetChild( num ) ;
	DetachChild( num ) ;
	chunk->Release() ;
}

void MdxChunk::DeleteChild( MdxChunk *chunk )
{
	DeleteChild( IndexOfChild( chunk ) ) ;
}

MdxChunk *MdxChunk::FindChild( int type, const char *name ) const
{
	return GetChild( IndexOfChild( type, name ) ) ;
}

MdxChunk *MdxChunk::FindChild( int type, int rank ) const
{
	return GetChild( IndexOfChild( type, rank ) ) ;
}

MdxChunk *MdxChunk::FindTree( int type, const char *name ) const
{
	MdxChunk *chunk = FindChild( type, name ) ;
	if ( chunk != 0 ) return chunk ;

	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		chunk = chunks[ i ]->FindTree( type, name ) ;
		if ( chunk != 0 ) return chunk ;
	}
	return 0 ;
}

int MdxChunk::IndexOfChild( int type, const char *name ) const
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		MdxChunk *chunk = chunks[ i ] ;
		if ( chunk->GetTypeID() == type ) {
			if ( name == 0 || name[ 0 ] == '\0' ) return i ;
			if ( strcmp( chunk->GetName(), name ) == 0 ) return i ;
		}
	}
	return -1 ;
}

int MdxChunk::IndexOfChild( int type, int rank ) const
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		if ( chunks[ i ]->GetTypeID() == type ) {
			if ( -- rank < 0 ) return i ;
		}
	}
	return -1 ;
}

int MdxChunk::IndexOfChild( const MdxChunk *chunk ) const
{
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		if ( chunks[ i ] == chunk ) return i ;
	}
	return -1 ;
}

int MdxChunk::RankOfChild( int type, const char *name ) const
{
	return RankOfChild( FindChild( type, name ) ) ;
}

int MdxChunk::RankOfChild( const MdxChunk *chunk ) const
{
	if ( chunk == 0 ) return -1 ;
	vector<MdxChunk *> &chunks = m_impl->m_child ;
	int type = chunk->GetTypeID() ;
	int rank = 0 ;
	for ( int i = 0 ; i < (int)chunks.size() ; i ++ ) {
		MdxChunk *chunk2 = chunks[ i ] ;
		if ( chunk2 == chunk ) return rank ;
		if ( chunk2->GetTypeID() == type ) rank ++ ;
	}
	return -1 ;
}

int MdxChunk::RankOfChild( int num ) const
{
	return RankOfChild( GetChild( num ) ) ;
}

MdxChunk *MdxChunk::FindRef( int type, const char *name ) const
{
	return FindRef( IndexOfRef( type, name ) ) ;
}

MdxChunk *MdxChunk::FindRef( int index ) const
{
	int level = MDX_REF_LEVEL( index ) ;
	int type = MDX_REF_TYPE( index ) ;
	int rank = MDX_REF_RANK( index ) ;
	MdxChunk *curr = (MdxChunk *)this ;
	while ( level >= 0 ) {
		MdxChunk *parent = curr->GetParent() ;
		if ( parent == 0 ) {
			if ( level != 0 || rank != 0 ) return 0 ;
			if ( curr->GetTypeID() != type ) return 0 ;
			return curr ;
		}
		curr = parent ;
		level -- ;
	}
	return curr->FindChild( type, rank ) ;
}

int MdxChunk::IndexOfRef( int type, const char *name ) const
{
	if ( name == 0 ) name = "" ;

	MdxChunk *curr = (MdxChunk *)this ;
	int level = 0 ;
	for ( ; ; ) {
		MdxChunk *parent = curr->GetParent() ;
		if ( parent == 0 ) {
			if ( curr->GetTypeID() != type ) break ;
			if ( strcmp( curr->GetName(), name ) != 0 ) break ;
			return MDX_REF_INDEX( level, type, 0 ) ;
		}
		int rank = parent->RankOfChild( type, name ) ;
		if ( rank >= 0 ) return MDX_REF_INDEX( level, type, rank ) ;
		curr = parent ;
		level ++ ;
	}
	if ( str_isdigit( name ) ) {			//  unresolved reference
		int idx = str_atoi( name ) ;
		int level = MDX_REF_LEVEL( idx ) ;
		int rank = MDX_REF_RANK( idx ) ;
		return MDX_REF_INDEX( level, type, rank ) ;
	}
	return -1 ;
}

int MdxChunk::IndexOfRef( const MdxChunk *chunk ) const
{
	return IndexOfRef( chunk->GetTypeID(), chunk->GetName() ) ;
}

MdxChunk *MdxChunk::FindReferrer( int type, const MdxChunk *parent ) const
{
	if ( parent == 0 ) parent = GetParent() ;
	if ( parent == 0 ) parent = this ;
	int count = parent->GetChildCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		MdxChunk *child = parent->GetChild( i ) ;
		if ( type == 0 || child->GetTypeID() == type ) {	// pre-order
			if ( child->RefersTo( this ) ) return child ;
		}
		MdxChunk *referrer = FindReferrer( type, child ) ;
		if ( referrer != 0 ) return referrer ;
	}
	return 0 ;
}

bool MdxChunk::RefersTo( const MdxChunk *chunk ) const
{
	if ( chunk == 0 ) return false ;
	int scope = chunk->GetTypeID() ;
	if ( HasArgsRef() ) {
		int count = GetArgsCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int desc = GetArgsDesc( i ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != scope ) continue ;
			if ( strcmp( GetArgsString( i ), chunk->GetName() ) ) continue ;
			return true ;
		}
	}
	if ( HasDataRef() ) {
		int count = GetDataCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int desc = GetDataDesc( i ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != scope ) continue ;
			if ( strcmp( GetDataString( i ), chunk->GetName() ) ) continue ;
			return true ;
		}
	}
	return false ;
}

//----------------------------------------------------------------
//  ChunkWord
//----------------------------------------------------------------

ChunkWord::ChunkWord()
{
	m_desc = MDX_WORD_INT32 ;
	m_bits = 0 ;
}

ChunkWord::~ChunkWord()
{
	Clear() ;
}

ChunkWord::ChunkWord( const ChunkWord &src )
{
	m_desc = src.m_desc ;
	m_bits = src.m_bits ;
	if ( MDX_WORD_VALUE_CLASS( m_desc ) == MDX_WORD_STRING ) {
		m_bits = 0 ;
		SetString( src.m_string ) ;
	}
}

ChunkWord &ChunkWord::operator=( const ChunkWord &src )
{
	if ( &src == this ) return *this ;
	Clear() ;
	m_desc = src.m_desc ;
	m_bits = src.m_bits ;
	if ( MDX_WORD_VALUE_CLASS( m_desc ) == MDX_WORD_STRING ) {
		m_bits = 0 ;
		SetString( src.m_string ) ;
	}
	return *this ;
}

bool ChunkWord::operator==( const ChunkWord &src ) const
{
	if ( m_desc != src.m_desc ) return false ;
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT : return ( m_int == src.m_int ) ;
	    case MDX_WORD_FLOAT : return ( m_float == src.m_float ) ;
	    case MDX_WORD_STRING :
		return ( strcmp( m_string, src.m_string ) == 0 ) ;
	}
	return false ;
}

void ChunkWord::Clear()
{
	if ( MDX_WORD_VALUE_CLASS( m_desc ) == MDX_WORD_STRING ) delete[] m_string ;
	m_bits = 0 ;
}

int ChunkWord::GetDesc() const
{
	return m_desc ;
}

void ChunkWord::SetDesc( int desc )
{
	if ( MDX_WORD_CLASS( m_desc ) != MDX_WORD_CLASS( desc ) && m_bits != 0 ) Clear() ;
	m_desc = desc ;
}

void ChunkWord::SetInt( int val )
{
	if ( MDX_WORD_VAR_TYPE & m_desc ) {
		if ( MDX_WORD_VALUE_CLASS( m_desc ) != MDX_WORD_INT ) {
			int flags = MDX_WORD_FLAGS( m_desc ) ;
			SetDesc( MDX_WORD_INT32 | flags ) ;
		}
	}
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT : m_int = val ; break ;
	    case MDX_WORD_FLOAT : m_float = (float)val ; break ;
	    case MDX_WORD_STRING :
		SetString( str_itoa( val ).c_str() ) ; break ;
	}
}

int ChunkWord::GetInt() const
{
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT :
		switch ( MDX_WORD_VALUE_TYPE( m_desc ) ) {
		    case MDX_WORD_INT32 : return (signed int)m_int ;
		    case MDX_WORD_INT16 : return (signed short)m_int ;
		    case MDX_WORD_INT8 : return (signed char)m_int ;
		    case MDX_WORD_UINT32 : return (unsigned int)m_int ;
		    case MDX_WORD_UINT16 : return (unsigned short)m_int ;
		    case MDX_WORD_UINT8 : return (unsigned char)m_int ;
		}
		break ;
	    case MDX_WORD_FLOAT : return (int)m_float ;
	    case MDX_WORD_STRING :
		return ( m_string == 0 ) ? 0 : str_atoi( m_string ) ;
	}
	return 0 ;
}

void ChunkWord::SetFloat( float val )
{
	if ( MDX_WORD_VAR_TYPE & m_desc ) {
		if ( MDX_WORD_VALUE_CLASS( m_desc ) != MDX_WORD_FLOAT ) {
			int flags = MDX_WORD_FLAGS( m_desc ) ;
			SetDesc( MDX_WORD_FLOAT32 | flags ) ;
		}
	}
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT : m_int = (int)val ; break ;
	    case MDX_WORD_FLOAT : m_float = val ; break ;
	    case MDX_WORD_STRING :
		SetString( str_ftoa( val ).c_str() ) ; break ;
	}
}

float ChunkWord::GetFloat() const
{
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT : return (float)m_int ;
	    case MDX_WORD_FLOAT : return m_float ;
	    case MDX_WORD_STRING :
		return ( m_string == 0 ) ? 0.0f : str_atof( m_string ) ;
	}
	return 0.0f ;
}

void ChunkWord::SetString( const char *val )
{
	if ( MDX_WORD_VAR_TYPE & m_desc ) {
		if ( MDX_WORD_VALUE_CLASS( m_desc ) != MDX_WORD_STRING ) {
			int flags = MDX_WORD_FLAGS( m_desc ) ;
			SetDesc( MDX_WORD_STRING8 | flags ) ;
		}
	}
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT : m_int = str_atoi( val ) ; break ;
	    case MDX_WORD_FLOAT : m_float = str_atof( val ) ; break ;
	    case MDX_WORD_STRING :
		if ( m_string == val ) break ;
		delete[] m_string ;
		m_string = 0 ;
		if ( val != 0 && val[ 0 ] != '\0' ) {
			m_string = new char[ strlen( val ) + 1 ] ;
			strcpy( m_string, val ) ;
		}
		break ;
	}
}

const char *ChunkWord::GetString() const
{
	static char buf[ 32 ] ;
	switch ( MDX_WORD_VALUE_CLASS( m_desc ) ) {
	    case MDX_WORD_INT : sprintf( buf, "%d", m_int ) ; return buf ;
	    case MDX_WORD_FLOAT : sprintf( buf, "%f", m_float ) ; return buf ;
	    case MDX_WORD_STRING :
		return ( m_string == 0 ) ? "" : m_string ;
	}
	return "" ;
}


} // namespace mdx
