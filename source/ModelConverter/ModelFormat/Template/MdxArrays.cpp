#include "ModelFormat.h"

namespace mdx {

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

class MdxArrays::impl {
public:
	int m_format ;
	int m_vertex_count ;
	int m_morph_format ;
	vector<MdxVertex> m_vertices ;
} ;

class MdxVertex::impl {
public:
	int m_format ;
	vec4 m_position ;
	vec4 m_normal ;
	rgba8888 m_color ;
	vector<vec4> m_texcoords ;
	vector<float> m_weights ;
	vector<unsigned char> m_blend_indices ;
} ;

//----------------------------------------------------------------
//  Arrays block
//----------------------------------------------------------------

MdxArrays::MdxArrays( const char *name )
{
	SetArgsDesc( 0, MDX_WORD_ENUM | MDX_SCOPE_ARRAYS_FORMAT ) ;
	SetArgsDesc( 1, MDX_WORD_INT ) ;
	SetArgsDesc( 2, MDX_WORD_INT ) ;

	m_impl = new impl ;
	m_impl->m_format = 0 ;
	m_impl->m_vertex_count = 0 ;
	m_impl->m_morph_format = 0 ;

	SetName( name ) ;
}

MdxArrays::~MdxArrays()
{
	delete m_impl ;
}

MdxArrays::MdxArrays( const MdxArrays &src )
{
	m_impl = new impl ;
	m_impl->m_format = 0 ;
	m_impl->m_vertex_count = 0 ;
	m_impl->m_morph_format = 0 ;

	Copy( &src ) ;
}

MdxArrays &MdxArrays::operator=( const MdxArrays &src )
{
	Copy( &src ) ;
	return *this ;
}

//----------------------------------------------------------------
//  chunk methods
//----------------------------------------------------------------

MdxChunk *MdxArrays::Copy( const MdxChunk *src )
{
	const MdxArrays *src2 = dynamic_cast<const MdxArrays *>( src ) ;
	if ( src2 == this ) return this ;
	if ( src2 == 0 ) return 0 ;

	MdxBlock::Copy( src2 ) ;
	*m_impl = *( src2->m_impl ) ;
	return this ;
}

bool MdxArrays::Equals( const MdxChunk *src ) const
{
	const MdxArrays *src2 = dynamic_cast<const MdxArrays *>( src ) ;
	if ( src2 == this ) return true ;
	if ( src2 == 0 ) return false ;

	if ( !MdxBlock::Equals( src2 ) ) return false ;
	if ( m_impl->m_format != src2->m_impl->m_format ) return false ;
	if ( m_impl->m_vertex_count != src2->m_impl->m_vertex_count ) return false ;
	if ( m_impl->m_vertices != src2->m_impl->m_vertices ) return false ;
	if ( m_impl->m_morph_format != src2->m_impl->m_morph_format ) return false ;
	return true ;
}

bool MdxArrays::UpdateArgs()
{
	SetArgsCount( 3 ) ;
	SetArgsInt( 0, GetFormat() ) ;
	SetArgsInt( 1, 0 ) ;
	SetArgsInt( 2, GetVertexCount() ) ;
	return true ;
}

bool MdxArrays::FlushArgs()
{
	SetFormat( GetArgsInt( 0 ) ) ;
	SetVertexCount( GetArgsInt( 2 ) ) ;
	return true ;
}

bool MdxArrays::UpdateData()
{
	int n_verts = GetVertexCount() ;

	SetDataCount( 0 ) ;
	SetDataDesc( -1, MDX_WORD_FLOAT ) ;	// default desc

	int num = 0 ;
	int j, k ;

	//  TODO: export NORMALn COLORn

	int format = GetVertexFormat() ;
	for ( j = 0 ; j < n_verts ; j ++ ) {
		SetDataDesc( num, MDX_WORD_FLOAT | MDX_WORD_FMT_NEWLINE ) ;

		const MdxVertex &vert = GetVertex( j ) ;
		if ( MDX_VF_POSITION & format ) {
			SetDataVec3( num, vert.GetPosition() ) ;
			num += 3 ;
		}
		if ( MDX_VF_NORMAL & format ) {
			SetDataVec3( num, vert.GetNormal() ) ;
			num += 3 ;
		}
		if ( MDX_VF_COLOR & format ) {
			SetDataVec4( num, vert.GetColor() ) ;
			num += 4 ;
		}
		for ( k = 0 ; k < MDX_VF_TEXCOORD_COUNT( format ) ; k ++ ) {
			SetDataVec2( num, vert.GetTexCoord( k ) ) ;
			num += 2 ;
		}
		for ( k = 0 ; k < MDX_VF_WEIGHT_COUNT( format ) ; k ++ ) {
			SetDataFloat( num, vert.GetWeight( k ) ) ;
			num += 1 ;
		}
		for ( k = 0 ; k < MDX_VF_INDICES_COUNT( format ) ; k ++ ) {
			SetDataDesc( num, MDX_WORD_UINT8 ) ;
			SetDataInt( num, vert.GetBlendIndex( k ) ) ;
			num += 1 ;
		}
	}

	return true ;
}

bool MdxArrays::FlushData()
{
	int n_verts = GetVertexCount() ;

	int num = 0 ;
	int j, k ;

	int format = GetVertexFormat() ;
	for ( j = 0 ; j < n_verts ; j ++ ) {
		MdxVertex &vert = GetVertex( j ) ;
		if ( MDX_VF_POSITION & format ) {
			vert.SetPosition( GetDataVec3( num ) ) ;
			num += 3 ;
		}
		if ( MDX_VF_NORMAL & format ) {
			vert.SetNormal( GetDataVec3( num ) ) ;
			num += 3 ;
		}
		if ( MDX_VF_COLOR & format ) {
			vert.SetColor( GetDataVec4( num ) ) ;
			num += 4 ;
		}
		for ( k = 0 ; k < MDX_VF_TEXCOORD_COUNT( format ) ; k ++ ) {
			vert.SetTexCoord( k, GetDataVec2( num ) ) ;
			num += 2 ;
		}
		for ( k = 0 ; k < MDX_VF_WEIGHT_COUNT( format ) ; k ++ ) {
			vert.SetWeight( k, GetDataFloat( num ) ) ;
			num += 1 ;
		}
		for ( k = 0 ; k < MDX_VF_INDICES_COUNT( format ) ; k ++ ) {
			vert.SetBlendIndex( k, GetDataInt( num ) ) ;
			num += 1 ;
		}
	}

	return true ;
}

//----------------------------------------------------------------
//  arrays methods
//----------------------------------------------------------------

void MdxArrays::SetFormat( int format )
{
	if ( m_impl->m_format == format ) return ;
	m_impl->m_format = format ;

	vector<MdxVertex> &verts = m_impl->m_vertices ;
	for ( int index = 0 ; index < m_impl->m_vertex_count ; index ++ ) {
		verts[ index ].SetFormat( format ) ;
	}

	MdxArrays *parent = (MdxArrays *)GetParent() ;
	if ( parent != 0 && parent->GetTypeID() == MDX_ARRAYS ) {
		parent->SetMorphFormat( format ) ;
	}
}

int MdxArrays::GetFormat() const
{
	return m_impl->m_format ;
}

void MdxArrays::SetVertexCount( int count )
{
	if ( m_impl->m_vertex_count == count || count < 0 ) return ;

		vector<MdxVertex> &verts = m_impl->m_vertices ;
		verts.resize( count ) ;

		int format = GetVertexFormat() ;
		for ( int index = m_impl->m_vertex_count ; index < count ; index ++ ) {
			verts[ index ].SetFormat( format ) ;
		}

	m_impl->m_vertex_count = count ;
}

int MdxArrays::GetVertexCount() const
{
	return m_impl->m_vertex_count ;
}

void MdxArrays::SetVertex( int index, const MdxVertex &vert )
{
	if ( index < 0 ) return ;
	if ( index >= m_impl->m_vertex_count ) SetVertexCount( index + 1 ) ;
	m_impl->m_vertices[ index ] = vert ;
}

const MdxVertex &MdxArrays::GetVertex( int index ) const
{
	static MdxVertex tmp ;
	if ( index < 0 ) return tmp ;
	if ( index >= m_impl->m_vertex_count ) return tmp ;
	return m_impl->m_vertices[ index ] ;
}

MdxVertex &MdxArrays::GetVertex( int index )
{
	static MdxVertex tmp ;
	if ( index < 0 ) return tmp ;
	if ( index >= m_impl->m_vertex_count ) SetVertexCount( index + 1 ) ;
	return m_impl->m_vertices[ index ] ;
}

void MdxArrays::InsertVertex( int index, int count )
{
	vector<MdxVertex> &verts = m_impl->m_vertices ;
	MdxVertex tmp = verts[ ( index == 0 ) ? 0 : index - 1 ] ;
	verts.insert( verts.begin() + index, count, tmp ) ;

	m_impl->m_vertex_count += count ;
}

void MdxArrays::DeleteVertex( int index, int count )
{
	vector<MdxVertex> &verts = m_impl->m_vertices ;
	verts.erase( verts.begin() + index, verts.begin() + index + count ) ;

	m_impl->m_vertex_count -= count ;
}

//----------------------------------------------------------------
//  morph methods
//----------------------------------------------------------------

void MdxArrays::SetMorphFormat( int format )
{
	if ( m_impl->m_morph_format == format ) return ;
	m_impl->m_morph_format = format ;

	for ( int i = 0 ; i < GetChildCount() ; i ++ ) {
		MdxArrays *morph = (MdxArrays *)GetChild( i ) ;
		if ( morph->GetTypeID() == MDX_ARRAYS ) morph->SetFormat( format ) ;
	}
}

int MdxArrays::GetMorphFormat() const
{
	return m_impl->m_morph_format ;
}

void MdxArrays::SetMorphCount( int count )
{
	int n_morphs = GetMorphCount() ;
	if ( count < 1 ) count = 1 ;
	if ( count < n_morphs ) {
		DeleteMorph( count, n_morphs - count ) ;
	} else if ( count > n_morphs ) {
		InsertMorph( n_morphs, count - n_morphs ) ;
	}
}

int MdxArrays::GetMorphCount() const
{
	return GetChildCount( MDX_ARRAYS ) + 1 ;
}

void MdxArrays::SetMorph( int index, const MdxArrays *morph )
{
	MdxArrays *morph2 = GetMorph( index ) ;
	if ( morph2 != 0 ) *morph2 = *morph ;
}

const MdxArrays *MdxArrays::GetMorph( int index ) const
{
	return ( index == 0 ) ? this : (const MdxArrays *)FindChild( MDX_ARRAYS, index - 1 ) ;
}

MdxArrays *MdxArrays::GetMorph( int index )
{
	return ( index == 0 ) ? this : (MdxArrays *)FindChild( MDX_ARRAYS, index - 1 ) ;
}

void MdxArrays::InsertMorph( int index, int count )
{
	if ( index == 0 ) return ;
	int index2 = IndexOfChild( MDX_ARRAYS, index - 1 ) ;
	if ( index2 < 0 ) index2 = GetChildCount() ;
	for ( int i = 0 ; i < count ; i ++ ) {
		MdxArrays *morph = new MdxArrays( str_format( "morph-%d", index + i ).c_str() ) ;
		InsertChild( index2 ++, morph ) ;
		morph->SetFormat( GetMorphFormat() ) ;
		morph->SetVertexCount( GetVertexCount() ) ;
	}
}

void MdxArrays::DeleteMorph( int index, int count )
{
	if ( index == 0 ) return ;
	for ( int i = 0 ; i < count ; i ++ ) {
		DeleteChild( GetMorph( index - 1 ) ) ;
	}
}

//----------------------------------------------------------------
//  convenience functions ( vertex format )
//----------------------------------------------------------------

void MdxArrays::SetVertexFormat( int format )
{
	SetFormat( format ) ;
}

int MdxArrays::GetVertexFormat() const
{
	return m_impl->m_format ;
}

void MdxArrays::SetVertexPositionCount( int count )
{
	int format = GetVertexFormat() ;
	int mask = MDX_VF_POSITION ;
	format = !count ? ( format & ~mask ) : ( format | mask ) ;
	SetVertexFormat( format ) ;
}

void MdxArrays::SetVertexNormalCount( int count )
{
	int format = GetVertexFormat() ;
	int mask = MDX_VF_NORMAL ;
	format = !count ? ( format & ~mask ) : ( format | mask ) ;
	SetVertexFormat( format ) ;
}

void MdxArrays::SetVertexColorCount( int count )
{
	int format = GetVertexFormat() ;
	int mask = MDX_VF_COLOR ;
	format = !count ? ( format & ~mask ) : ( format | mask ) ;
	SetVertexFormat( format ) ;
}

void MdxArrays::SetVertexTexCoordCount( int count )
{
	int format = GetVertexFormat() ;
	format &= ~MDX_VF_TEXCOORDS( ~0 ) ;
	format |= MDX_VF_TEXCOORDS( count ) ;
	SetVertexFormat( format ) ;
}

void MdxArrays::SetVertexWeightCount( int count )
{
	int format = GetVertexFormat() ;
	format &= ~MDX_VF_WEIGHTS( ~0 ) ;
	format |= MDX_VF_WEIGHTS( count ) ;
	SetVertexFormat( format ) ;
}

int MdxArrays::GetVertexPositionCount() const
{
	int format = GetVertexFormat() ;
	return !( MDX_VF_POSITION & format ) ? 0 : 1 ;
}

int MdxArrays::GetVertexNormalCount() const
{
	int format = GetVertexFormat() ;
	return !( MDX_VF_NORMAL & format ) ? 0 : 1 ;
}

int MdxArrays::GetVertexColorCount() const
{
	int format = GetVertexFormat() ;
	return !( MDX_VF_COLOR & format ) ? 0 : 1 ;
}

int MdxArrays::GetVertexTexCoordCount() const
{
	int format = GetVertexFormat() ;
	return MDX_VF_TEXCOORD_COUNT( format ) ;
}

int MdxArrays::GetVertexWeightCount() const
{
	int format = GetVertexFormat() ;
	return MDX_VF_WEIGHT_COUNT( format ) ;
}

int MdxArrays::GetVertexStride() const
{
	return MdxVertex::GetFormatStride( GetVertexFormat() ) ;
}

int MdxArrays::GetFormatStride( int format )
{
	return MdxVertex::GetFormatStride( format ) ;
}

void MdxArrays::SetVertexBlendIndicesMode( bool exists )
{
	int format = GetVertexFormat() ;
	format = !exists ? ( format & ~MDX_VF_INDICES ) : ( format | MDX_VF_INDICES ) ;
	SetVertexFormat( format ) ;
}

bool MdxArrays::GetVertexBlendIndicesMode() const
{
	int format = GetVertexFormat() ;
	return MDX_VF_HAS_INDICES( format ) ;
}

int MdxArrays::GetVertexBlendIndicesCount() const
{
	int format = GetVertexFormat() ;
	return MDX_VF_INDICES_COUNT( format ) ;
}

//----------------------------------------------------------------
//  Arrays vertex
//----------------------------------------------------------------

MdxVertex::MdxVertex()
{
	m_impl = new impl ;
	m_impl->m_format = 0 ;
	m_impl->m_position = 0.0f ;
	m_impl->m_normal = 0.0f ;
	m_impl->m_color = rgba8888( 255, 255, 255, 255 ) ;
}

MdxVertex::~MdxVertex()
{
	delete m_impl ;
}

MdxVertex::MdxVertex( const MdxVertex &src )
{
	m_impl = new impl ;
	*this = src ;
}

MdxVertex &MdxVertex::operator=( const MdxVertex &src )
{
	*m_impl = *( src.m_impl ) ;
	return *this ;
}

bool MdxVertex::operator==( const MdxVertex &src ) const
{
	if ( m_impl->m_format != src.m_impl->m_format ) return false ;
	if ( m_impl->m_position != src.m_impl->m_position ) return false ;
	if ( m_impl->m_normal != src.m_impl->m_normal ) return false ;
	if ( m_impl->m_color != src.m_impl->m_color ) return false ;
	if ( m_impl->m_texcoords != src.m_impl->m_texcoords ) return false ;
	if ( m_impl->m_weights != src.m_impl->m_weights ) return false ;
	if ( m_impl->m_blend_indices != src.m_impl->m_blend_indices ) return false ;
	return true ;
}

bool MdxVertex::operator!=( const MdxVertex &src ) const
{
	return !operator==( src ) ;
}

//----------------------------------------------------------------
//  vertex format
//----------------------------------------------------------------

void MdxVertex::SetFormat( int format )
{
	if ( m_impl->m_format == format ) return ;
	m_impl->m_format = format ;
	m_impl->m_texcoords.resize( MDX_VF_TEXCOORD_COUNT( format ) ) ;
	m_impl->m_weights.resize( MDX_VF_WEIGHT_COUNT( format ) ) ;
	m_impl->m_blend_indices.resize( MDX_VF_INDICES_COUNT( format ) ) ;
}

int MdxVertex::GetFormat() const
{
	return m_impl->m_format ;
}

//----------------------------------------------------------------
//  vertex data
//----------------------------------------------------------------

void MdxVertex::SetPosition( const vec4 &position )
{
	m_impl->m_position = position ;
}

const vec4 &MdxVertex::GetPosition() const
{
	return m_impl->m_position ;
}

void MdxVertex::SetNormal( const vec4 &normal )
{
	m_impl->m_normal = normal ;
}

const vec4 &MdxVertex::GetNormal() const
{
	return m_impl->m_normal ;
}

void MdxVertex::SetColor( rgba8888 color )
{
	m_impl->m_color = color ;
}

rgba8888 MdxVertex::GetColor() const
{
	return m_impl->m_color ;
}

void MdxVertex::SetTexCoord( const vec4 &coord )
{
	SetTexCoord( 0, coord ) ;
}

const vec4 &MdxVertex::GetTexCoord() const
{
	return GetTexCoord( 0 ) ;
}

void MdxVertex::SetWeight( float weight )
{
	SetWeight( 0, weight ) ;
}

float MdxVertex::GetWeight() const
{
	return GetWeight( 0 ) ;
}

void MdxVertex::SetTexCoord( int num, const vec4 &coord )
{
	if ( num < 0 || num >= MDX_VF_TEXCOORD_COUNT( m_impl->m_format ) ) return ;
	m_impl->m_texcoords[ num ] = coord ;
}

const vec4 &MdxVertex::GetTexCoord( int num ) const
{
	if ( num < 0 || num >= MDX_VF_TEXCOORD_COUNT( m_impl->m_format ) ) return vec4_zero ;
	return m_impl->m_texcoords[ num ] ;
}

void MdxVertex::SetWeight( int num, float weight )
{
	if ( num < 0 || num >= MDX_VF_WEIGHT_COUNT( m_impl->m_format ) ) return ;
	m_impl->m_weights[ num ] = weight ;
}

float MdxVertex::GetWeight( int num ) const
{
	if ( num < 0 || num >= MDX_VF_WEIGHT_COUNT( m_impl->m_format ) ) return 0.0f ;
	return m_impl->m_weights[ num ] ;
}

void MdxVertex::SetBlendIndex( int num, int index )
{
	if ( num < 0 || num >= MDX_VF_INDICES_COUNT( m_impl->m_format ) ) return ;
	m_impl->m_blend_indices[ num ] = index ;
}

int MdxVertex::GetBlendIndex( int num ) const
{
	if ( num < 0 || num >= MDX_VF_INDICES_COUNT( m_impl->m_format ) ) return 0 ;
	return m_impl->m_blend_indices[ num ] ;
}

//----------------------------------------------------------------
//  convenience functions ( vertex format )
//----------------------------------------------------------------

void MdxVertex::SetPositionCount( int count )
{
	int format = m_impl->m_format ;
	int mask = MDX_VF_POSITION ;
	format = !count ? ( format & ~mask ) : ( format | mask ) ;
	SetFormat( format ) ;
}

void MdxVertex::SetNormalCount( int count )
{
	int format = m_impl->m_format ;
	int mask = MDX_VF_NORMAL ;
	format = !count ? ( format & ~mask ) : ( format | mask ) ;
	SetFormat( format ) ;
}

void MdxVertex::SetColorCount( int count )
{
	int format = m_impl->m_format ;
	int mask = MDX_VF_COLOR ;
	format = !count ? ( format & ~mask ) : ( format | mask ) ;
	SetFormat( format ) ;
}

void MdxVertex::SetTexCoordCount( int count )
{
	int format = m_impl->m_format ;
	format &= ~MDX_VF_TEXCOORDS( ~0 ) ;
	format |= MDX_VF_TEXCOORDS( count ) ;
	SetFormat( format ) ;
}

void MdxVertex::SetWeightCount( int count )
{
	int format = m_impl->m_format ;
	format &= ~MDX_VF_WEIGHTS( ~0 ) ;
	format |= MDX_VF_WEIGHTS( count ) ;
	SetFormat( format ) ;
}

int MdxVertex::GetPositionCount() const
{
	return !( MDX_VF_POSITION & m_impl->m_format ) ? 0 : 1 ;
}

int MdxVertex::GetNormalCount() const
{
	return !( MDX_VF_NORMAL & m_impl->m_format ) ? 0 : 1 ;
}

int MdxVertex::GetColorCount() const
{
	return !( MDX_VF_COLOR & m_impl->m_format ) ? 0 : 1 ;
}

int MdxVertex::GetTexCoordCount() const
{
	return MDX_VF_TEXCOORD_COUNT( m_impl->m_format ) ;
}

int MdxVertex::GetWeightCount() const
{
	return MDX_VF_WEIGHT_COUNT( m_impl->m_format ) ;
}

int MdxVertex::GetStride() const
{
	return GetFormatStride( m_impl->m_format ) ;
}

void MdxVertex::SetBlendIndicesMode( bool exists )
{
	int format = m_impl->m_format ;
	format = !exists ? ( format & ~MDX_VF_INDICES ) : ( format | MDX_VF_INDICES ) ;
	SetFormat( format ) ;
}

bool MdxVertex::GetBlendIndicesMode() const
{
	return MDX_VF_HAS_INDICES( m_impl->m_format ) ;
}

int MdxVertex::GetFormatStride( int format )
{
	int count = 0 ;
	if ( MDX_VF_POSITION & format ) count += 3 ;
	if ( MDX_VF_NORMAL & format ) count += 3 ;
	if ( MDX_VF_COLOR & format ) count += 1 ;
	count += MDX_VF_TEXCOORD_COUNT( format ) * 2 ;
	count += MDX_VF_WEIGHT_COUNT( format ) ;
	return count ;
}


} // namespace mdx
