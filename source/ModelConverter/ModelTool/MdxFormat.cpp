#include "ModelTool.h"

namespace mdx {

//----------------------------------------------------------------
//  impls
//----------------------------------------------------------------

struct FormatSymbol {
	string name ;
	int value ;
} ;

class MdxFormat::impl {
public:
	map<int,MdxChunk *> m_templates ;
	map<string,int> m_scopes ;
	map<int,vector<FormatSymbol> > m_symbols ;
	int m_volatile_scope ;
} ;

//----------------------------------------------------------------
//  MdxFormat
//----------------------------------------------------------------

MdxFormat::MdxFormat()
{
	m_impl = new impl ;
	m_impl->m_volatile_scope = MDX_SCOPE_VOLATILE ;

	//  scopes

	SetScope( "WORD_TYPE", MDX_SCOPE_WORD_TYPE ) ;
	SetScope( "CHUNK_TYPE", MDX_SCOPE_CHUNK_TYPE ) ;

	//  symbols

	int scope = MDX_SCOPE_WORD_TYPE ;
	SetSymbol( scope, "int", MDX_WORD_INT32 ) ;
	SetSymbol( scope, "short", MDX_WORD_INT16 ) ;
	SetSymbol( scope, "char", MDX_WORD_INT8 ) ;
	SetSymbol( scope, "u_int", MDX_WORD_UINT32 ) ;
	SetSymbol( scope, "u_short", MDX_WORD_UINT16 ) ;
	SetSymbol( scope, "u_char", MDX_WORD_UINT8 ) ;
	SetSymbol( scope, "float", MDX_WORD_FLOAT32 ) ;
	SetSymbol( scope, "half", MDX_WORD_FLOAT16 ) ;
	SetSymbol( scope, "string", MDX_WORD_STRING8 ) ;
	SetSymbol( scope, "void", MDX_WORD_VAR_TYPE ) ;
	SetSymbol( scope, "ref", MDX_WORD_VAR_SCOPE | MDX_WORD_REF ) ;
	SetSymbol( scope, "...", MDX_WORD_VAR_COUNT ) ;
}

MdxFormat::~MdxFormat()
{
	map<int,MdxChunk *>::iterator i ;
	for ( i = m_impl->m_templates.begin() ; i != m_impl->m_templates.end() ; i ++ ) {
		( i->second )->Release() ;
	}
	delete m_impl ;
}

MdxFormat::MdxFormat( const MdxFormat &src )
{
	m_impl = new impl ;
	m_impl->m_volatile_scope = MDX_SCOPE_VOLATILE ;
	*this = src ;
}

MdxFormat &MdxFormat::operator=( const MdxFormat &src )
{
	m_impl->m_volatile_scope = src.m_impl->m_volatile_scope ;
	m_impl->m_scopes = src.m_impl->m_scopes ;
	m_impl->m_symbols = src.m_impl->m_symbols ;

	map<int,MdxChunk *>::iterator i ;
	for ( i = m_impl->m_templates.begin() ; i != m_impl->m_templates.end() ; i ++ ) {
		( i->second )->Release() ;
	}
	m_impl->m_templates = src.m_impl->m_templates ;
	for ( i = m_impl->m_templates.begin() ; i != m_impl->m_templates.end() ; i ++ ) {
		( i->second )->AddRef() ;
	}
	return *this ;
}

//----------------------------------------------------------------
//  templates
//----------------------------------------------------------------

void MdxFormat::SetTemplate( int id, MdxChunk *chunk )
{
	if ( id == 0 && chunk != 0 ) id = chunk->GetTypeID() ;
	m_impl->m_templates[ id ]->Release() ;
	m_impl->m_templates[ id ] = chunk ;

	if ( chunk != 0 ) {
		const char *type_name = chunk->GetTypeName() ;
		int type_id = chunk->GetTypeID() ;
		SetSymbol( MDX_SCOPE_CHUNK_TYPE, type_name, type_id ) ;
		SetSymbol( MDX_SCOPE_WORD_TYPE, type_name, MDX_WORD_REF | type_id ) ;
	}
}

MdxChunk *MdxFormat::GetTemplate( int id ) const
{
	return m_impl->m_templates[ id ] ;
}

//----------------------------------------------------------------
//  scopes
//----------------------------------------------------------------

int MdxFormat::SetScope( const char *name, int id )
{
	if ( name == 0 ) name = "" ;
	if ( id >= MDX_SCOPE_VOLATILE ) {
		map<string,int>::iterator i = m_impl->m_scopes.find( name ) ;
		if ( i != m_impl->m_scopes.end() ) return i->second ;
		id = m_impl->m_volatile_scope ++ ;
	}
	m_impl->m_scopes[ name ] = id ;

	SetSymbol( MDX_SCOPE_WORD_TYPE, name, MDX_WORD_ENUM | id ) ;
	return id ;
}

int MdxFormat::GetScope( const char *name ) const
{
	int id ;
	GetScope( name, id ) ;
	return id ;
}

const char *MdxFormat::GetScopeName( int id ) const
{
	const char *name ;
	GetScopeName( id, name ) ;
	return name ;
}

bool MdxFormat::GetScope( const char *name, int &id ) const
{
	if ( name == 0 ) name = "" ;
	map<string,int>::iterator i = m_impl->m_scopes.find( name ) ;
	id = ( i == m_impl->m_scopes.end() ) ? 0 : i->second ;
	return ( i != m_impl->m_scopes.end() ) ;
}

bool MdxFormat::GetScopeName( int id, const char *&name ) const
{
	map<string,int>::iterator i = m_impl->m_scopes.begin() ;
	while ( i != m_impl->m_scopes.end() ) {
		if ( i->second == id ) {
			name = i->first.c_str() ;
			return true ;
		}
		i ++ ;
	}
	name = "" ;
	return false ;
}

//----------------------------------------------------------------
//  symbols
//----------------------------------------------------------------

void MdxFormat::SetSymbol( int scope, const char *name, int value )
{
	if ( name == 0 ) name = "" ;
	vector<FormatSymbol> &symbols = m_impl->m_symbols[ scope ] ;
	for ( int i = 0 ; i < (int)symbols.size() ; i ++ ) {
		if ( symbols[ i ].name == name ) {
			symbols[ i ].value = value ;
			return ;
		}
	}
	FormatSymbol tmp ;
	tmp.name = name ;
	tmp.value = value ;
	symbols.push_back( tmp ) ;
}

int MdxFormat::GetSymbol( int scope, const char *name ) const
{
	int value ;
	GetSymbol( scope, name, value ) ;
	return value ;
}

const char *MdxFormat::GetSymbolName( int scope, int value ) const
{
	const char *name ;
	GetSymbolName( scope, value, name ) ;
	return name ;
}

bool MdxFormat::GetSymbol( int scope, const char *name, int &value ) const
{
	if ( name == 0 ) name = "" ;
	const vector<FormatSymbol> &symbols = m_impl->m_symbols[ scope ] ;
	for ( int i = 0 ; i < (int)symbols.size() ; i ++ ) {
		if ( symbols[ i ].name == name ) {
			value = symbols[ i ].value ;
			return true ;
		}
	}
	value = 0 ;
	return false ;
}

bool MdxFormat::GetSymbolName( int scope, int value, const char *&name ) const
{
	if ( name == 0 ) name = "" ;
	const vector<FormatSymbol> &symbols = m_impl->m_symbols[ scope ] ;
	for ( int i = 0 ; i < (int)symbols.size() ; i ++ ) {
		if ( symbols[ i ].value == value ) {
			name = symbols[ i ].name.c_str() ;
			return true ;
		}
	}
	name = "" ;
	return false ;
}

int MdxFormat::GetFlags( int scope, const char *name ) const
{
	int flags ;
	GetFlags( scope, name, flags ) ;
	return flags ;
}

const char *MdxFormat::GetFlagsName( int scope, int flags ) const
{
	const char *name ;
	GetFlagsName( scope, flags, name ) ;
	return name ;
}

bool MdxFormat::GetFlags( int scope, const char *name, int &flags ) const
{
	if ( name == 0 ) name = "" ;
	flags = 0 ;

	string str = name ;
	bool result = true ;
	for ( int next = -1 ; next < (int)str.length() ; ) {
		int from = next + 1 ;
		next = str.find( "|", from ) ;
		if ( next == (int)string::npos ) next = str.length() ;
		if ( next <= from ) continue ;
		string word = str.substr( from, next - from ) ;
		if ( str_isdigit( word ) ) {
			flags |= str_atoi( word ) ;
			continue ;
		}
		int value ;
		if ( !GetSymbol( scope, word.c_str(), value ) ) result = false ;
		flags |= value ;
	}
	return result ;
}

bool MdxFormat::GetFlagsName( int scope, int flags, const char *&name ) const
{
	static string g_str ;
	g_str = "" ;

	vector<FormatSymbol> &symbols = m_impl->m_symbols[ scope ] ;
	vector<int> indices ;
	while ( flags != 0 ) {
		int best_index = -1 ;
		int best_error = 32 ;
		int best_flags = 0 ;
		for ( int i = 0 ; i < (int)symbols.size() ; i ++ ) {
			int flags2 = symbols[ i ].value ;
			if ( flags2 != 0 && ( ~flags & flags2 ) == 0 ) {
				if ( flags == flags2 ) {
					best_index = i ;
					best_flags = flags2 ;
					break ;
				}
				int error = 0 ;
				unsigned int diff = flags ^ flags2 ;
				while ( diff != 0 ) {
					error += ( 1 & diff ) ;
					diff >>= 1 ;
				}
				if ( error < best_error ) {
					best_index = i ;
					best_error = error ;
					best_flags = flags2 ;
				}
			}
		}
		if ( best_flags == 0 ) break ;
		indices.push_back( best_index ) ;
		flags &= ~best_flags ;
	}

	sort( indices.begin(), indices.end() ) ;
	for ( int i = 0 ; i < (int)indices.size() ; i ++ ) {
		if ( i != 0 ) g_str += "|" ;
		g_str += symbols[ indices[ i ] ].name ;
	}

	bool result = true ;
	if ( flags != 0 ) {
		if ( g_str != "" ) g_str += "|" ;
		g_str += str_format( "0x%04x", flags ) ;
		result = false ;
	} else if ( g_str == "" ) {
		for ( int j = 0 ; j < (int)symbols.size() ; j ++ ) {
			if ( symbols[ j ].value == 0 ) {
				g_str = symbols[ j ].name ;
				break ;
			}
		}
		if ( g_str == "" ) g_str = "0" ;
	}
	name = g_str.c_str() ;
	return result ;
}


} // namespace mdx
