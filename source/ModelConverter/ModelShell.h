#ifndef MODEL_SHELL_H_INCLUDE
#define MODEL_SHELL_H_INCLUDE

#include "ModelTool/ModelTool.h"

#include <string>
#include <vector>
#include <set>
using namespace std ;

//----------------------------------------------------------------
//  ModelShell
//----------------------------------------------------------------

class ModelShell : public MdxShell {
public:
	ModelShell() ;
	virtual ~ModelShell() ;
	ModelShell( const ModelShell &src ) ;
	ModelShell &operator=( const ModelShell &src ) ;

	//  application information

	virtual const char *GetAppName() const ;
	virtual const char *GetAppVersion() const ;
	virtual const char *GetAppFileName() const ;
	void SetAppName( const char *name ) ;
	void SetAppVersion( const char *version ) ;
	void SetAppFileName( const char *filename ) ;

	//  objects

	virtual void SetVar( const char *name, const char *value ) ;

	//  invoking

	virtual int EvalProcArgv( int argc, const char **argv ) ;

public:
	//  script file

	struct Script {
		string type ;
		string name ;
		vector<string> args ;
		vector<vector<string> > body ;
	} ;

	bool LoadScript( const string &filename ) ;
	bool LoadScript( const char **lines ) ;
	void  UnloadScript() ;
	int EvalScript( const string &type, const string &name = "", const string &val = "" ) ;
	int EvalScript( int num, const string &val = "" ) ;
	int FindScript( const string &type, const string &name = "" ) ;
	int GetScriptCount() const ;
	const Script &GetScript( int num ) const ;
	int FindArgs( const vector<string> &line ) ;
	const char *SubstVar( const char *str ) ;
	int GetTime() ;

	//  command line

	int EvalCommandLine( const vector<string> &args, bool set_args = true, bool set_vars = true ) ;
	int GetArgCount() const ;
	void SetArgCount( int num ) ;
	string GetArg( int num ) const ;
	void SetArg( int num, const string &arg ) ;
	bool CheckDirectOption( const char *name ) ;

protected:
	string m_app_name ;
	string m_app_version ;
	string m_app_filename ;
	vector<Script> m_scripts ;
	vector<string> m_args ;
	set<string> m_known_vars ;
} ;


#endif
