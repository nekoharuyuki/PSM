#include "EmbedImage.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char VarInputFilename[] = "input_filename" ;
static const char VarImageExtension[] = "image_extension" ;
static const char VarImageExtension2[] = "image_extension2" ;
static const char VarImageConverter[] = "image_converter" ;
//static const char PreferredExtension[] = ".gim" ;
static const char PreferredExtension[] = ".png" ;

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  EmbedImage
//----------------------------------------------------------------

bool EmbedImage::Modify( MdxBlock *block )
{
	//  check params

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	int mode = str_search( ModeNames, arg ) ;
	if ( mode < 0 ) {
		Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
		return false ;
	}
	if ( mode == MODE_OFF ) return true ;

	//  process

	str_split( m_exts, GetVar( VarImageExtension ), ',' ) ;
	str_split( m_exts2, GetVar( VarImageExtension2 ), ',' ) ;
	m_converter = str_trim( GetVar( VarImageConverter ) ) ;
	m_extension = !m_exts.empty() ? m_exts[ 0 ] : PreferredExtension ;

	MdxBlocks filenames ;
	filenames.EnumTree( block, MDX_FILE_NAME ) ;
	for ( int i = 0 ; i < filenames.size() ; i ++ ) {
		ImportFileImage( (MdxFileName *)filenames[ i ] ) ;
	}

	m_exts.clear() ;
	m_exts2.clear() ;
	m_converter = "" ;
	m_extension = "" ;
	return true ;
}

void EmbedImage::ImportFileImage( MdxFileName *filename )
{
	MdxBlock *parent = (MdxBlock *)filename->GetParent() ;
	if ( parent == 0 ) return ;

	//  open

	string dir = str_dirname( GetVar( VarInputFilename ) ) ;
	string name = filename->GetFileName() ;
	string full = str_fullpath( dir, name ) ;
	string curr = str_fullpath( dir, str_tailname( name ) ) ;

	bool exists = false ;
	for ( int i = 0 ; i < 2 ; i ++ ) {
		const string &path = ( i == 0 ) ? curr : full ;
		string root = str_rootname( path ) ;
		for ( int j = 0 ; j < (int)m_exts.size() ; j ++ ) {
			if ( ( exists = sys_exists( name = root + m_exts[ j ] ) ) ) break ;
		}
		if ( exists ) break ;
		for ( int k = 0 ; k < (int)m_exts2.size() ; k ++ ) {
			if ( ( exists = sys_exists( name = root + m_exts2[ k ] ) ) ) break ;
		}
		if ( exists ) break ;

		//  specified filename
		// if ( ( exists = sys_exists( name = path ) ) ) break ;
	}

	//  convert

	if ( exists && m_converter != "" ) {
		string ext = str_extension( name ) ;
		if ( find( m_exts2.begin(), m_exts2.end(), ext ) != m_exts2.end() ) {
			string name2 ;
			string command = MakeConvertCommand( name, name2 ) ;
			if ( !sys_exec( command, true ) ) {
				Warning( "cannot convert \"%s\"\n", name.c_str() ) ;
				exists = ( find( m_exts.begin(), m_exts.end(), ext ) != m_exts.end() ) ;
			} else {
				Message( "convert \"%s\"\n", name.c_str() ) ;
				name = name2 ;
			}
		}
	}

	//  import

	bin_ifstream stream ;
	if ( !exists || !stream.open( name ) ) {
		if ( parent->FindChild( MDX_FILE_IMAGE ) == 0 ) {
			if ( !exists ) name = filename->GetFileName() ;
			Warning( "cannot import \"%s\"\n", name.c_str() ) ;
		}
	} else {
		parent->ClearTree( MDX_FILE_IMAGE ) ;

		int size = stream.get_filesize() ;
		Message( "import \"%s\" ( %d bytes )\n", name.c_str(), size ) ;

		vector<char> buf ;
		if ( size > 0 ) {
			buf.resize( size ) ;
			stream.read( &( buf[ 0 ] ), size ) ;
		}
		stream.close() ;

		MdxFileImage *fileimage = new MdxFileImage ;
		char *ptr = buf.empty() ? 0 : &( buf[ 0 ] ) ;
		fileimage->SetFileImage( ptr, size ) ;
		parent->AttachChild( fileimage ) ;
	}

	//  tmpfile

	if ( m_tmpfile != "" ) {
		sys_unlink( m_tmpfile ) ;
		m_tmpfile = "" ;
	}
}

string EmbedImage::MakeConvertCommand( const string &name, string &name2 )
{
	string command = m_converter ;
	name2 = "" ;
	string::size_type pos = command.find( "%tmpfile" ) ;
	if ( pos != string::npos ) {
		name2 = m_tmpfile = sys_mktemp( sys_tmpdir(), "tmp", m_extension ) ;
		command = command.substr( 0, pos ) + str_quote( name2 ) + command.substr( pos + 8 ) ;
	}
	pos = command.find( "%curfile" ) ;
	if ( pos != string::npos ) {
		string dir = str_dirname( GetVar( VarInputFilename ) ) ;
		name2 = str_fullpath( dir, str_rootname( str_tailname( name ) ) + m_extension ) ;
		command = command.substr( 0, pos ) + str_quote( name2 ) + command.substr( pos + 8 ) ;
	}
	if ( name2 == "" ) name2 = str_rootname( name ) + m_extension ;
	return command + " " + str_quote( name ) ;
}


} // namespace mdx
