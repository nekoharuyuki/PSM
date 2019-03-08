#include "ModelConverter.h"

#ifdef WIN32
#include <windows.h>
#include <conio.h>
#endif // WIN32

//----------------------------------------------------------------
//  configuration
//----------------------------------------------------------------

static const char ModelConverter_AppName[] = "ModelConverter" ;
static const char ModelConverter_AppVersion[] = "1.2X" ;
static const char ModelConverter_AppLogo[] = 
	"ModelConverter ver 1.2X --- model file converter\n"
	" Copyright (C) 2013 Sony Computer Entertainment Inc.\n"
	"  All Rights Reserved.\n" ;

static const char ModelConverter_ConfigFile[] = "ModelConverter.cfg" ;
static const char ModelConverter_FormatModule[] = "./module/ModelFormat" ;
static const char ModelConverter_ProcModule[] = "./module/ModelProc" ;

//----------------------------------------------------------------
//  variables
//----------------------------------------------------------------

static vector<string> g_command_line ;
static vector<string> g_output_files ;

//----------------------------------------------------------------
//  main
//----------------------------------------------------------------

int main( int argc, const char **argv )
{
	setlocale( LC_CTYPE, "" ) ;

	ModelConverter *conv = new ModelConverter ;
	int error = conv->Run( argc, argv ) ;
	conv->Release() ;
	return error ;
}

//----------------------------------------------------------------
//  ModelConverter
//----------------------------------------------------------------

ModelConverter::ModelConverter()
{
	;
}

ModelConverter::~ModelConverter()
{
	;
}

ModelConverter::ModelConverter( const ModelConverter &src )
{
	*this = src ;
}

ModelConverter &ModelConverter::operator=( const ModelConverter &src )
{
	ModelShell::operator=( src ) ;
	return *this ;
}

//----------------------------------------------------------------
//  execution
//----------------------------------------------------------------

int ModelConverter::Run( int argc, const char **argv )
{
	Startup( argc, argv ) ;
	Process() ;
	Finish() ;
	return GetErrorCount() ;
}

bool ModelConverter::Startup( int argc, const char **argv )
{
	//  application information

	char filename[ 1024 ] ;
	#ifdef WIN32
	GetModuleFileName( 0, filename, sizeof( filename ) ) ;
	#else // WIN32
	strncpy( filename, str_fullpath( INSTDIR, str_tailname( argv[ 0 ] ) ).c_str(), 1024 ) ;
	filename[ 1023 ] = '\0' ;
	#endif // WIN32

	SetAppName( ModelConverter_AppName ) ;
	SetAppVersion( ModelConverter_AppVersion ) ;
	SetAppFileName( filename ) ;

	//  module and script

	ImportNameSpace( MDX_PROC_NS ) ;
	if ( !LoadModule( ModelConverter_FormatModule )
	  || !LoadModule( ModelConverter_ProcModule ) ) {
		return false ;
	}

	if ( !LoadScript( ModelConverter_ConfigFile ) ) {
		Message( "NOTICE : open failed \"%s\"\n", ModelConverter_ConfigFile ) ;
		LoadScript( ModelConverter_BuiltinConfig ) ;
	}
	if ( FindScript( "default" ) >= 0 ) {
		if ( EvalScript( "default" ) == MDX_EVAL_ERROR ) return false ;
	}

	//  command line

	for ( int i = 0 ; i < argc ; i ++ ) {
		g_command_line.push_back( argv[ i ] ) ;
	}
	if ( EvalCommandLine( g_command_line ) == MDX_EVAL_ERROR ) return false ;

	//  interact mode

	string opt = GetVar( "interact_mode" ) ;
	#ifdef WIN32
	if ( opt == "ctrl" ) {
		if ( ( GetKeyState( VK_CONTROL ) & 0x8000 ) != 0 ) opt = "on" ;
	}
	#endif // WIN32
	if ( opt == "on" ) {
		SetVar( "prompt_mode", "on" ) ;
		string command = "ModelConverter" ;
		for ( int i = 1 ; i < argc ; i ++ ) {
			command += " " ;
			command += argv[ i ] ;
		}
		Message( "%s\n", command.c_str() ) ;
		Message( "more options ? " ) ;
		char buf[ 4096 ] ;
		fgets( buf, sizeof( buf ), stdin ) ;
		vector<string> args ;
		str_split( args, buf ) ;
		g_command_line.insert( g_command_line.end(), args.begin(), args.end() ) ;

		if ( EvalCommandLine( g_command_line ) == MDX_EVAL_ERROR ) {
			return false ;
		}
	}

	//  usage

	if ( GetArgCount() <= 1 ) {
		Message( "%s\n", ModelConverter_AppLogo ) ;
		if ( FindScript( "usage" ) >= 0 ) {
			EvalScript( "usage" ) ;
		}
		#ifdef WIN32
		Message( "Press any key to continue" ) ;
		getch() ;
		#endif // WIN32
		Release() ;
		exit( 0 ) ;
	}

	//  definition file

	string def = GetVar( "definition_file" ) ;
	if ( def != "" ) {
		def = str_fullpath( str_dirname( filename ), def ) ;
		Message( "load \"%s\"\n", def.c_str() ) ;
		EvalProc( "ImportMDS", "file-1", def.c_str(), 0 ) ;
		SetBlock( "file-1", 0 ) ;	//  delete temporary
		SetErrorCount( 0 ) ;		//  ignore errors
	}
	return true ;
}

bool ModelConverter::Process()
{
	if ( GetErrorCount() != 0 ) return false ;

	//  import, merge, filter, export

	string opt = GetVar( "merge_mode" ) ;
	if ( opt == "" || opt == "off" ) {
		for ( int i = 1 ; i < GetArgCount() ; i ++ ) {
			string infile = GetArg( i ) ;
			if ( Batch( "file-1", infile ) ) continue ;
			if ( !Import( "file-1", infile ) ) return false ;
			if ( !Filter( "file-1" ) ) return false ;
			if ( !Filter2( "file-1" ) ) return false ;
			if ( !Export( "file-1", infile, i ) ) return false ;
		}
	} else {
		string infile = GetArg( 1 ) ;
		if ( Batch( "file-1", infile, false ) ) return false ;		//  disabled
		if ( !Import( "file-1", infile ) ) return false ;
		if ( !Filter( "file-1" ) ) return false ;
		for ( int i = 2 ; i < GetArgCount() ; i ++ ) {
			string infile2 = GetArg( i ) ;
			if ( Batch( "file-2", infile2, false ) ) return false ;	//  disabled
			if ( !Import( "file-2", GetArg( i ) ) ) return false ;
			if ( !Filter( "file-2" ) ) return false ;
			if ( !Merge( "file-1", "file-2" ) ) return false ;
		}
		if ( !Filter2( "file-1" ) ) return false ;
		if ( !Export( "file-1", infile, 0 ) ) return false ;
	}
	return true ;
}

void ModelConverter::Finish()
{
	#ifdef WIN32
	//  launch viewer

	if ( GetErrorCount() == 0 && !g_output_files.empty() ) {
		string viewer = GetVar( "object_viewer" ) ;
		if ( viewer != "" ) {
			string command = viewer ;
			for ( int i = 0 ; i < (int)g_output_files.size() ; i ++ ) {
				command += " \"" ;
				command += g_output_files[ i ] ;
				command += "\"" ;
			}
			Message( "exec \"%s\"\n", viewer.c_str() ) ;
			if ( !sys_exec( command, true ) ) {
				Warning( "cannot exec \"%s\"\n", viewer.c_str() ) ;
				Message( "Press any key to continue" ) ;
				getch() ;
			}
			return ;
		}
	}

	//  prompt mode

	string opt = GetVar( "prompt_mode" ) ;
	if ( opt == "" ) opt = "on" ;
	int mode = str_search( "off on warning error", opt ) ;
	switch ( mode ) {
	    case 0 :
		return ;
	    case 1 :
		break ;
	    case 2 :
		if ( GetErrorCount() == 0 && GetWarningCount() == 0 ) return ;
		break ;
	    case 3 :
		if ( GetErrorCount() == 0 ) return ;
		break ;
	}
	Message( "Press any key to continue" ) ;
	getch() ;
	#endif // WIN32
}

//----------------------------------------------------------------
//  processing
//----------------------------------------------------------------

bool ModelConverter::Batch( const string &target, const string &infile, bool enabled )
{
	//  check

	string ext = str_toupper( str_extension( infile ) ) ;
	string ext2 = str_toupper( GetVar( "batch_extension" ) ) ;
	if ( ext != ext2 ) return false ;
	if ( !enabled ) {
		Error( "batch-files cannot be merged\n" ) ;
		return true ;
	}

	//  process

	Message( "load \"%s\"\n", infile.c_str() ) ;

	txt_ifstream stream ;
	stream.open( infile ) ;
	if ( !stream.is_open() ) {
		Error( "open failed \"%s\"\n", infile.c_str() ) ;
		return true ;
	}
	string pattern = str_format( "%s*", ModelConverter_AppName ) ;
	string dirname = str_dirname( infile ) ;
	vector<string> line ;
	while ( !stream.is_eof() ) {
		stream.get_line( line, true ) ;
		if ( line.empty() ) continue ;
		string cmd = str_tailname( line[ 0 ] ) ;
		if ( !str_match( pattern, cmd, true ) ) continue ;

		ModelConverter copy = *this ;
		copy.EvalCommandLine( line ) ;
		copy.EvalCommandLine( g_command_line, false, true ) ;	//  override options
		int n_args = 0 ;
		for ( int i = 0 ; i < copy.GetArgCount() ; i ++ ) {	//  convert to fullpath
			string arg = copy.GetArg( i ) ;
			if ( arg == "" ) continue ;	//  work around ( empty line before EOF )
			copy.SetArg( n_args ++, str_fullpath( dirname, arg ) ) ;
		}
		copy.SetArgCount( n_args ) ;
		if ( !copy.Process() ) break ;
	}
	stream.close() ;

	return true ;
}

bool ModelConverter::Import( const string &target, const string &infile )
{
	//  format specific settings

	string tailname = str_tailname( infile ) ;
	for ( int i = 0 ; i < GetScriptCount() ; i ++ ) {
		const ModelShell::Script &s = GetScript( i ) ;
		if ( s.type == "input" && str_match( s.name, tailname, true ) ) {
			if ( EvalScript( i ) == MDX_EVAL_ERROR ) return false ;
		}
	}
	// if ( EvalCommandLine( g_command_line ) == MDX_EVAL_ERROR ) return false ;
	EvalCommandLine( g_command_line, false, true ) ;	//  override options

	//  invoke "ImportXXX"

	Message( "load \"%s\"\n", infile.c_str() ) ;
	SetVar( "input_filename", infile.c_str() ) ;

	string ext = str_extension( infile ) ;
	if ( ext != "" ) ext.erase( 0, 1 ) ;
	string procname = "Import" ;
	procname += str_toupper( ext ) ;
	if ( GetProc( procname.c_str() ) == 0 ) {
		Error( "unsupported format \"%s\"\n", infile.c_str() ) ;
		return false ;
	}
	int code = EvalProc( procname.c_str(), target.c_str(), infile.c_str(), 0 ) ;
	return ( code != MDX_EVAL_ERROR ) ;
}

bool ModelConverter::Merge( const string &target, const string &source )
{
	string opt = GetVar( "merge_mode" ) ;
	if ( opt == "" ) opt = "off" ;
	int mode = str_search( "off on model motion", opt ) ;
	string procname ;
	switch ( mode ) {
	    case 0 :	return true ;
	    case 1 :	procname = "MergeModels" ; break ;
	    case 2 :	procname = "MergeModels" ; break ;
	    case 3 :	procname = "MergeMotions" ; break ;
	    default :
		Warning( "unknown merge mode \"%s\"\n", opt.c_str() ) ;
		return true ;
	}

	//  invoke "MergeXXX"

	int code = EvalProc( procname.c_str(), target.c_str(), source.c_str(), 0 ) ;
	return ( code != MDX_EVAL_ERROR ) ;
}

bool ModelConverter::Filter( const string &target )
{
	//  filter script

	string opt = GetVar( "filter_script" ) ;
	if ( opt == "" ) return true ;
	int code = EvalScript( "script", opt, target ) ;
	return ( code != MDX_EVAL_ERROR ) ;
}

bool ModelConverter::Filter2( const string &target )
{
	//  filter script

	string opt = GetVar( "filter_script2" ) ;
	if ( opt == "" ) return true ;
	int code = EvalScript( "script", opt, target ) ;
	return ( code != MDX_EVAL_ERROR ) ;
}

bool ModelConverter::Export( const string &target, const string &infile, int suffix )
{
	//  check filename

	string outfile = GetVar( "output_filename" ) ;

	string opt = GetVar( "output_directory" ) ;
	int mode = str_search( "current input auto", opt ) ;
	if ( mode < 0 || mode == 2 ) {
		mode = ( outfile[ 0 ] == '.' ) ? 0 : 1 ;
	}

	bool auto_extension = false ;
	if ( outfile == "" ) {
		outfile = ( mode == 0 ) ? str_tailname( infile ) : infile ;
		auto_extension = true ;
	} else {
		if ( str_extension( outfile ) == "" ) auto_extension = true ;
		if ( mode != 0 ) outfile = str_fullpath( str_dirname( infile ), outfile ) ;
		char c = outfile[ outfile.length() - 1 ] ;
		if ( c == '/' || c == '\\' || sys_isdirectory( outfile ) ) {
			outfile = str_fullpath( outfile, str_tailname( infile ) ) ;
			auto_extension = true ;
		} else if ( suffix > 1 ) {
			string root = str_rootname( outfile ) ;
			string ext = str_extension( outfile ) ;
			string num = str_format( "(%d)", suffix ) ;
			outfile = root + num + ext ;
		}
	}

	//  export files

	string output = GetVar( "output_object" ) ;
	if ( output == "on" ) {
		if ( auto_extension ) {
			outfile = str_rootname( outfile ) + GetVar( "object_extension" ) ;
		}
		if ( !Export( target, outfile ) ) return false ;
	}

	output = GetVar( "output_script" ) ;
	if ( output == "on" ) {
		outfile = str_rootname( outfile ) + GetVar( "script_extension" ) ;
		if ( !Export( target, outfile ) ) return false ;
	}
	return true ;
}

bool ModelConverter::Export( const string &target, const string &outfile )
{
	Message( "save \"%s\"\n", outfile.c_str() ) ;
	g_output_files.push_back( outfile ) ;

	string ext = str_extension( outfile ) ;
	if ( ext != "" ) ext.erase( 0, 1 ) ;
	string procname = "Export" ;
	procname += str_toupper( ext ) ;
	int code = EvalProc( procname.c_str(), target.c_str(), outfile.c_str(), 0 ) ;
	return ( code != MDX_EVAL_ERROR ) ;
}
