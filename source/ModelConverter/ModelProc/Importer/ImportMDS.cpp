#include "ImportMDS.h"

namespace mdx {

//----------------------------------------------------------------
//  ImportMDS
//----------------------------------------------------------------

bool ImportMDS::Import( MdxBlock *&block, const void *buf, int size )
{
	return false ;	// not supported
}

bool ImportMDS::Import( MdxBlock *&block, const char *filename )
{
	string var = m_shell->GetVar( "source_define" ) ;
	m_source_define = !( var == "off" ) ;
	var = m_shell->GetVar( "source_option" ) ;
	m_source_option = !( var == "off" ) ;

	block = 0 ;
	if ( !m_stream.open( filename ) ) {
		Error( "open failed \"%s\"\n", filename ) ;
		return false ;
	}
	if ( !ImportHeader() ) {
		m_stream.close() ;
		return false ;
	}
	block = new MdxFile ;
	if ( !ImportBlock( block ) ) {
		block->Release() ;
		block = 0 ;
		m_stream.close() ;
		return false ;
	}
	m_stream.close() ;
	return true ;
}

bool ImportMDS::ImportHeader()
{
	vector<string> line ;
	m_stream.get_line( line, true ) ;
	if ( line.size() < 2 ) {
		Error( "incomplete header \"%s\"\n", m_stream.get_filename().c_str() ) ;
		return false ;
	}
	if ( line[ 0 ] == ".MDS" ) {
		if ( line[ 1 ] != "1.00" ) {
			Error( "wrong version \"%s\"\n", m_stream.get_filename().c_str() ) ;
			return false ;
		}
	} else {
		Error( "wrong format \"%s\"\n", m_stream.get_filename().c_str() ) ;
		return false ;
	}
	return true ;
}

bool ImportMDS::ImportBlock( MdxBlock *block )
{
	vector<string> line ;
	while ( !m_stream.is_eof() ) {
		m_stream.get_line( line, false ) ;
		if ( line.empty() ) continue ;
		string cmd = line.front() ;
		if ( cmd == "}" ) return true ;
		if ( !ParseLine( block, line ) ) return false ;
	}
	return true ;
}

bool ImportMDS::ParseLine( MdxBlock *block, const vector<string> &line )
{
	//  skipping

	if ( block == 0 ) {
		if ( line.back() == "{" ) SkipBlock() ;
		return true ;
	}

	//  check type

	string cmd = line.front() ;

	if ( str_isdigit( cmd ) ) {
		ImportData( block, line ) ;
		return true ;
	}

	int type_id ;
	if ( !m_format->GetSymbol( MDX_SCOPE_CHUNK_TYPE, cmd.c_str(), type_id ) ) {
		Warning( "unknown command or block", cmd.c_str() ) ;
		if ( line.back() == "{" ) SkipBlock() ;
		return true ;
	}

	//  check template

	MdxBlock *block2 ;
	MdxChunk *templ = m_format->GetTemplate( type_id ) ;
	if ( templ != 0 ) {
		block2 = (MdxBlock *)templ->Clone() ;		// clone template
	} else if ( line.back() == "{" ) {
		block2 = new MdxUnknownBlock( type_id ) ;	// unknown block
	} else {
		block2 = new MdxUnknownCommand( type_id ) ;	// unknown command
	}

	//  import body

	if ( block2->IsCommand() ) {
		block->AttachChild( block2 ) ;
		if ( !ImportArgs( block2, line, 1 ) ) return false ;
		if ( line.back() == "{" ) SkipBlock() ;
		CheckDirective( block2 ) ;
	} else {
		if ( line.size() >= 2 && line[ 1 ] != "{" ) {
			block2->SetName( str_unquote( line[ 1 ] ).c_str() ) ;
		}
		block->AttachChild( block2 ) ;
		if ( !ImportArgs( block2, line, 2 ) ) return false ;

		if ( line.back() == "{" ) {
			block2->UpdateData() ;
			m_data_index = 0 ;
			if ( !ImportBlock( block2 ) ) return false ;
			if ( m_data_index >= 0 && m_data_index < block2->GetDataCount() ) {
				int desc = block->GetArgsDesc( m_data_index ) ;
				if ( !( desc & MDX_WORD_VAR_COUNT ) ) {
					Warning( "too few data", block2->GetTypeName() ) ;
				}
			}
			block2->FlushData() ;
		}
	}
	return true ;
}

bool ImportMDS::SkipBlock()
{
	vector<string> line ;
	while ( !m_stream.is_eof() ) {
		m_stream.get_line( line, true ) ;
		if ( line.empty() ) continue ;
		string cmd = line.front() ;
		if ( cmd == "}" ) return true ;
		if ( !ParseLine( 0, line ) ) return false ;
	}
	return true ;
}

bool ImportMDS::ImportArgs( MdxBlock *block, const vector<string> &line, int start )
{
	block->UpdateArgs() ;

	int num = 0 ;
	for ( int i = start ; i < (int)line.size() ; i ++ ) {
		string word = line[ i ] ;
		if ( word == "{" ) break ;

		int desc = block->GetArgsDesc( num ) ;
		if ( num >= block->GetArgsCount() && !( desc & MDX_WORD_VAR_COUNT ) ) {
			Warning( "too many args", line[ 0 ] ) ;
			break ;
		}

		if ( CheckWordDesc( word, desc ) ) {
			block->SetArgsDesc( num, desc ) ;
		}

		switch ( MDX_WORD_CLASS( desc ) ) {
		    case MDX_WORD_INT : {
			block->SetArgsInt( num, str_atoi( word ) ) ;
			break ;
		    }
		    case MDX_WORD_FLOAT : {
			block->SetArgsFloat( num, str_atof( word ) ) ;
			break ;
		    }
		    case MDX_WORD_STRING : {
			block->SetArgsString( num, word.c_str() ) ;
			break ;
		    }
		    case MDX_WORD_ENUM : {
			int value = 0 ;
			if ( !m_format->GetFlags( MDX_WORD_SCOPE( desc ), word.c_str(), value ) ) {
				Warning( "unknown symbol", word.c_str() ) ;
			}
			block->SetArgsInt( num, value ) ;
			break ;
		    }
		    case MDX_WORD_REF : {
			block->SetArgsString( num, word.c_str() ) ;
			break ;
		    }
		}
		num ++ ;
	}
	if ( num < block->GetArgsCount() ) {
		int desc = block->GetArgsDesc( num ) ;
		if ( !( desc & MDX_WORD_VAR_COUNT ) ) Warning( "too few args", line[ 0 ] ) ;
	}
	block->FlushArgs() ;
	return true ;
}

bool ImportMDS::ImportData( MdxBlock *block, const vector<string> &line )
{
	int num = m_data_index ;
	if ( num < 0 ) return true ;	// "too many data"

	for ( int i = 0 ; i < (int)line.size() ; i ++ ) {
		string word = line[ i ] ;
		if ( word == "{" ) break ;

		int desc = block->GetDataDesc( num ) ;
		if ( num >= block->GetDataCount() && !( desc & MDX_WORD_VAR_COUNT ) ) {
			Warning( "too many data", block->GetTypeName() ) ;
			num = -1 ;
			break ;
		}

		if ( CheckWordDesc( word, desc ) ) {
			block->SetArgsDesc( num, desc ) ;
		}

		switch ( MDX_WORD_CLASS( desc ) ) {
		    case MDX_WORD_INT : {
			block->SetDataInt( num, str_atoi( word ) ) ;
			break ;
		    }
		    case MDX_WORD_FLOAT : {
			block->SetDataFloat( num, str_atof( word ) ) ;
			break ;
		    }
		    case MDX_WORD_STRING : {
			block->SetDataString( num, word.c_str() ) ;
			break ;
		    }
		    case MDX_WORD_ENUM : {
			int value = 0 ;
			if ( !m_format->GetFlags( MDX_WORD_SCOPE( desc ), word.c_str(), value ) ) {
				Warning( "unknown symbol", word.c_str() ) ;
			}
			block->SetDataInt( num, value ) ;
			break ;
		    }
		    case MDX_WORD_REF : {
			block->SetDataString( num, word.c_str() ) ;
			break ;
		    }
		}
		num ++ ;
	}
	m_data_index = num ;
	return true ;
}

bool ImportMDS::CheckWordDesc( string &word, int &desc )
{
	bool quoted = false ;
	if ( word[ 0 ] == '"' ) {
		word = str_unquote( word ) ;
		quoted = true ;
	}

	int colons = word.find( "::" ) ;
	if ( colons == (int)string::npos ) {
		if ( !( MDX_WORD_VAR_TYPE & desc ) ) return false ;
		desc &= ~MDX_WORD_TYPE_MASK ;
		if ( quoted || !str_isdigit( word ) ) {
			desc |= MDX_WORD_STRING ;
		} else if ( word.find_first_of( '.' ) != string::npos ) {
			desc |= MDX_WORD_FLOAT ;
		} else {
			desc |= MDX_WORD_INT ;
		}
		return true ;
	}

	string qualifier = word.substr( 0, colons ) ;
	word = word.substr( colons + 2 ) ;

	int scope ;
	if ( !m_format->GetFlags( MDX_SCOPE_WORD_TYPE, qualifier.c_str(), scope ) ) {
		Warning( "unknown qualifier", qualifier ) ;
		return false ;
	}
	if ( MDX_WORD_VAR_TYPE & desc ) {
		scope &= ~MDX_WORD_FLAGS_MASK ;
		desc &= MDX_WORD_FLAGS_MASK ;
		desc |= scope ;
		return true ;
	} else if ( MDX_WORD_VAR_SCOPE & desc ) {
		scope &= MDX_WORD_SCOPE_MASK ;
		desc &= ~MDX_WORD_SCOPE_MASK ;
		desc |= scope ;
		return true ;
	}
	return false ;
}

bool ImportMDS::CheckDirective( MdxBlock *block )
{
	switch ( block->GetTypeID() ) {
	    case MDX_DEFINE_ENUM : {
		if ( !m_source_define ) return true ;
		MdxDefineEnum *def = (MdxDefineEnum *)block ;
		int scope = m_format->SetScope( def->GetDefScope(), MDX_SCOPE_VOLATILE ) ;
		m_format->SetSymbol( scope, def->GetDefName(), def->GetDefValue() ) ;
		return true ;
	    }
	    case MDX_DEFINE_BLOCK : {
		if ( !m_source_define ) return true ;
		MdxDefineBlock *def = (MdxDefineBlock *)block ;
		int type_id = def->GetDefTypeID() ;
		const char *type_name = def->GetDefTypeName() ;
		MdxUnknownBlock *tmp = new MdxUnknownBlock( type_id, type_name ) ;
		tmp->SetArgsDesc( -1, 0 ) ;	// default = VAR_TYPE | VAR_COUNT
		for ( int i = 0 ; i < def->GetDefArgsCount() ; i ++ ) {
			int desc = def->GetDefArgsDesc( i ) ;
			if ( desc & MDX_WORD_VAR_COUNT ) {
				desc = def->GetDefArgsDesc( i - 1 ) ;
				tmp->SetArgsDesc( -1, desc | MDX_WORD_VAR_COUNT ) ;
				tmp->SetArgsCount( i - 1 ) ;
				break ;
			}
			tmp->SetArgsDesc( i, desc ) ;
		}
		m_format->SetTemplate( 0, tmp ) ;
		return true ;
	    }
	    case MDX_DEFINE_COMMAND : {
		if ( !m_source_define ) return true ;
		MdxDefineCommand *def = (MdxDefineCommand *)block ;
		int type_id = def->GetDefTypeID() ;
		const char *type_name = def->GetDefTypeName() ;
		MdxUnknownCommand *tmp = new MdxUnknownCommand( type_id, type_name ) ;
		tmp->SetArgsDesc( -1, 0 ) ;	// default = VAR_TYPE | VAR_COUNT
		for ( int i = 0 ; i < def->GetDefArgsCount() ; i ++ ) {
			int desc = def->GetDefArgsDesc( i ) ;
			if ( desc & MDX_WORD_VAR_COUNT ) {
				desc = def->GetDefArgsDesc( i - 1 ) ;
				tmp->SetArgsDesc( -1, desc | MDX_WORD_VAR_COUNT ) ;
				tmp->SetArgsCount( i - 1 ) ;
				break ;
			}
			tmp->SetArgsDesc( i, desc ) ;
		}
		m_format->SetTemplate( 0, tmp ) ;
		return true ;
	    }
	}
	return false ;
}

void ImportMDS::Warning( const string &mesg, const string &arg )
{
	string name = str_tailname( m_stream.get_filename() ) ;
	int line = m_stream.get_line_count() ;
	MdxProc::Warning( "%s ( %d ) : %s \"%s\"\n",
				name.c_str(), line, mesg.c_str(), arg.c_str() ) ;
}


} // namespace mdx
