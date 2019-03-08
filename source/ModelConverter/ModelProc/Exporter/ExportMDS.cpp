#include "ExportMDS.h"

namespace mdx {

#define SUPPRESS_FILEBLOCK 1
#define SUPPRESS_FILEIMAGE 1

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char LineFeedNames[] = "CRLF CR LF" ;
static const char *LineFeedChars[] = { "\r\n", "\r", "\n" } ;

static const char LineBreakNames[] = "OFF ON" ;
enum { MODE_OFF, MODE_ON } ;

//----------------------------------------------------------------
//  ExportMDS
//----------------------------------------------------------------

bool ExportMDS::Export( MdxBlock *block, const char *filename )
{
	m_root_state = 0 ;
	m_line_feed = "\n" ;
	m_line_break = MODE_ON ;
	m_float_format = "%f" ;

	string var = str_toupper( GetVar( "text_line_feed" ) ) ;
	int mode = str_search( LineFeedNames, var ) ;
	if ( mode >= 0 ) m_line_feed = LineFeedChars[ mode ] ;
	var = str_toupper( GetVar( "text_line_break" ) ) ;
	mode = str_search( LineBreakNames, var ) ;
	if ( mode >= 0 ) m_line_break = mode ;
	var = GetVar( "text_float_format" ) ;
	if ( var != "" ) m_float_format = var ;

	if ( !m_stream.open( filename ) ) {
		Error( "open failed \"%s\"\n", filename ) ;
		return false ;
	}
	m_stream.set_line_feed( m_line_feed ) ;
	if ( !ExportHeader() ) {
		Error( "write failed \"%s\"\n", filename ) ;
		m_stream.close() ;
		return false ;
	}
	if ( !ExportBlock( block ) ) {
		Error( "write failed \"%s\"\n", filename ) ;
		m_stream.close() ;
		return false ;
	}
	m_stream.close() ;
	return true ;
}

bool ExportMDS::ExportHeader()
{
	m_stream.put_string( ".MDS 1.00" ) ;
	m_stream.put_newline() ;
	return true ;
}

bool ExportMDS::ExportBlock( MdxBlock *block )
{
	int type_id = block->GetTypeID() ;
	string type_name = m_format->GetSymbolName( MDX_SCOPE_CHUNK_TYPE, type_id ) ;
	if ( type_name == "" ) {
		Warning( "unknown block 0x%04x\n", type_id ) ;
		return true ;
	}

	#if ( SUPPRESS_FILEBLOCK )
	if ( type_id == MDX_FILE && m_stream.get_indent() == 0 ) {
		for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
			MdxBlock *child = (MdxBlock *)block->GetChild( i ) ;
			if ( !ExportBlock( child ) ) return false ;
		}
		return true ;
	}
	#endif // SUPPRESS_FILEBLOCK

	#if ( SUPPRESS_FILEIMAGE )
	if ( type_id == MDX_FILE_IMAGE ) return true ;
	#endif // SUPPRESS_FILEIMAGE

	if ( block->IsCommand() ) {
		if ( m_stream.get_indent() == 0 ) {
			if ( m_root_state != MDX_COMMAND ) m_stream.put_newline() ;
			m_root_state = MDX_COMMAND ;
		}
		m_stream.put_indent() ;
		m_stream.put_string( type_name ) ;
		m_stream.set_indent( m_stream.get_indent() + 1 ) ;
		if ( !ExportArgs( block ) ) return false ;
		m_stream.set_indent( m_stream.get_indent() - 1 ) ;
		m_stream.put_newline() ;

	} else {
		if ( m_stream.get_indent() == 0 ) {
			m_stream.put_newline() ;
			m_root_state = MDX_BLOCK ;
		}
		m_stream.put_indent() ;
		m_stream.put_string( type_name ) ;
		m_stream.put_string( block->GetName(), true ) ;
		m_stream.set_indent( m_stream.get_indent() + 1 ) ;
		if ( !ExportArgs( block ) ) return false ;
		m_stream.put_string( "{" ) ;
		m_stream.put_newline() ;
		if ( !ExportData( block ) ) return false ;
		for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
			MdxBlock *child = (MdxBlock *)block->GetChild( i ) ;
			if ( !ExportBlock( child ) ) return false ;
		}
		m_stream.set_indent( m_stream.get_indent() - 1 ) ;
		m_stream.put_indent() ;
		m_stream.put_string( "}" ) ;
		m_stream.put_newline() ;
	}
	return true ;
}

bool ExportMDS::ExportArgs( MdxBlock *block )
{
	block->UpdateArgs() ;
	for ( int i = 0 ; i < block->GetArgsCount() ; i ++ ) {
		int desc = block->GetArgsDesc( i ) ;
		if ( ( desc & MDX_WORD_FMT_NEWLINE ) && m_line_break != MODE_OFF ) {
			m_stream.put_newline( true ) ;
			m_stream.put_indent() ;
		}
		switch ( MDX_WORD_CLASS( desc ) ) {
		    case MDX_WORD_INT :
			WriteInt( desc, block->GetArgsInt( i ) ) ;
			break ;
		    case MDX_WORD_FLOAT :
			WriteFloat( desc, block->GetArgsFloat( i ) ) ;
			break ;
		    case MDX_WORD_STRING :
			WriteString( desc, block->GetArgsString( i ) ) ;
			break ;
		    case MDX_WORD_ENUM :
			WriteEnum( desc, block->GetArgsInt( i ) ) ;
			break ;
		    case MDX_WORD_REF :
			WriteRef( desc, block->GetArgsString( i ) ) ;
			break ;
		}
	}
	return true ;
}

bool ExportMDS::ExportData( MdxBlock *block )
{
	block->UpdateData() ;
	if ( block->GetDataCount() == 0 ) return true ;
	m_stream.put_indent() ;
	for ( int i = 0 ; i < block->GetDataCount() ; i ++ ) {
		int desc = block->GetDataDesc( i ) ;
		if ( ( desc & MDX_WORD_FMT_NEWLINE ) && i > 0 ) {
			m_stream.put_newline() ;
			m_stream.put_indent() ;
		}
		switch ( MDX_WORD_CLASS( desc ) ) {
		    case MDX_WORD_INT :
			WriteInt( desc, block->GetDataInt( i ) ) ;
			break ;
		    case MDX_WORD_FLOAT :
			WriteFloat( desc, block->GetDataFloat( i ) ) ;
			break ;
		    case MDX_WORD_STRING :
			WriteString( desc, block->GetDataString( i ) ) ;
			break ;
		    case MDX_WORD_ENUM :
			WriteEnum( desc, block->GetDataInt( i ) ) ;
			break ;
		    case MDX_WORD_REF :
			WriteRef( desc, block->GetDataString( i ) ) ;
			break ;
		}
	}
	m_stream.put_newline() ;
	return true ;
}

bool ExportMDS::WriteInt( int desc, int value )
{
	string word ;
	int type = MDX_WORD_TYPE( desc ) ;
	if ( desc & MDX_WORD_VAR_TYPE ) {
		if ( type != MDX_WORD_INT32 && type != MDX_WORD_UINT32 ) {
			word = m_format->GetFlagsName( MDX_SCOPE_WORD_TYPE, type ) ;
			word += "::" ;
		}
	}
	const char *format = "%d" ;
	if ( ( desc & MDX_WORD_FMT_HEX ) ) {
		switch ( type ) {
		    case MDX_WORD_UINT32 : format = "0x%08x" ; break ;
		    case MDX_WORD_INT32 : format = "0x%08x" ; break ;
		    case MDX_WORD_UINT16 : format = "0x%04x" ; break ;
		    case MDX_WORD_INT16 : format = "0x%04x" ; break ;
		    case MDX_WORD_UINT8 : format = "0x%02x" ; break ;
		    case MDX_WORD_INT8 : format = "0x%02x" ; break ;
		}
	}
	m_stream.put_string( word + str_format( format, value ) ) ;
	return true ;
}

bool ExportMDS::WriteFloat( int desc, float value )
{
	string word ;
	int type = MDX_WORD_TYPE( desc ) ;
	if ( desc & MDX_WORD_VAR_TYPE ) {
		if ( type != MDX_WORD_FLOAT32 ) {
			word = m_format->GetFlagsName( MDX_SCOPE_WORD_TYPE, type ) ;
			word += "::" ;
		}
	}
	const char *format = m_float_format.c_str() ;
	m_stream.put_string( word + str_format( format, value ) ) ;
	return true ;
}

bool ExportMDS::WriteString( int desc, const char *value )
{
	m_stream.put_string( value, true ) ;
	return true ;
}

bool ExportMDS::WriteEnum( int desc, int value )
{
	string word ;
	int scope = MDX_WORD_SCOPE( desc ) ;
	if ( ( desc & MDX_WORD_VAR_TYPE ) || ( desc & MDX_WORD_VAR_SCOPE ) ) {
		word = m_format->GetScopeName( scope ) ;
		word += "::" ;
	}
	word += m_format->GetFlagsName( scope, value ) ;
	m_stream.put_string( word ) ;
	return true ;
}

bool ExportMDS::WriteRef( int desc, const char *value )
{
	string word ;
	if ( ( desc & MDX_WORD_VAR_TYPE ) || ( desc & MDX_WORD_VAR_SCOPE ) ) {
		int scope = MDX_WORD_SCOPE( desc ) ;
		word = m_format->GetSymbolName( MDX_SCOPE_BLOCK_TYPE, scope ) ;
		word += "::" ;
	}
	word += value ;
	m_stream.put_string( word.c_str(), true ) ;
	return true ;
}


} // namespace mdx
