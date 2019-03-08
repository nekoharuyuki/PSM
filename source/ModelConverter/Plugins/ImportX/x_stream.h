#ifndef X_STREAM_H_INCLUDE
#define X_STREAM_H_INCLUDE

#include "ImportX.h"

//----------------------------------------------------------------
//  x_ifstream
//----------------------------------------------------------------

class x_ifstream {

public:
	x_ifstream() {}
	virtual ~x_ifstream() {}

	virtual bool open( const string &filename ) = 0 ;
	virtual void close() = 0 ;
	virtual bool eof() = 0 ;

	virtual int get_pos() = 0 ;
	virtual int set_pos( int pos ) = 0 ;
	virtual int add_pos( int pos ) = 0 ;
	virtual int get_block_from() = 0 ;

	virtual bool open_block( string &type, string &name ) = 0 ;
	virtual bool open_block( string &type ) = 0 ;
	virtual bool open_block() = 0 ;
	virtual bool close_block() = 0 ;

	virtual bool get_string( string &buf ) = 0 ;
	virtual bool get_float( float &buf ) = 0 ;
	virtual bool get_int( int &buf ) = 0 ;

	virtual bool get_string( vector<string> &buf, int count ) = 0 ;
	virtual bool get_float( vector<float> &buf, int count ) = 0 ;
	virtual bool get_int( vector<int> &buf, int count ) = 0 ;

	virtual bool get_float( float *buf, int count ) = 0 ;
	virtual bool get_int( int *buf, int count ) = 0 ;
} ;

//----------------------------------------------------------------
//  x_txt_ifstream
//----------------------------------------------------------------

class x_txt_ifstream : public x_ifstream {
public:
	x_txt_ifstream() ;
	~x_txt_ifstream() ;

	bool open( const string &filename ) ;
	void close() ;
	bool eof() ;

	int get_pos() ;
	int set_pos( int pos ) ;
	int add_pos( int pos ) ;
	int get_block_from() ;
	int get_line_count() ;

	bool open_block( string &type, string &name ) ;
	bool open_block( string &type ) ;
	bool open_block() ;
	bool close_block() ;

	bool get_string( string &buf ) ;
	bool get_float( float &buf ) ;
	bool get_int( int &buf ) ;

	bool get_string( vector<string> &buf, int count ) ;
	bool get_float( vector<float> &buf, int count ) ;
	bool get_int( vector<int> &buf, int count ) ;

	bool get_float( float *buf, int count ) ;
	bool get_int( int *buf, int count ) ;

protected:
	txt_ifstream m_stream ;
	int m_block_from ;
} ;

//----------------------------------------------------------------
//  x_bin_ifstream
//----------------------------------------------------------------

class x_bin_ifstream : public x_ifstream {
public:
	x_bin_ifstream() ;
	~x_bin_ifstream() ;

	bool open( const string &filename ) ;
	void close() ;
	bool eof() ;

	int get_pos() ;
	int set_pos( int pos ) ;
	int add_pos( int pos ) ;
	int get_block_from() ;

	bool open_block( string &type, string &name ) ;
	bool open_block( string &type ) ;
	bool open_block() ;
	bool close_block() ;

	bool get_string( string &buf ) ;
	bool get_float( float &buf ) ;
	bool get_int( int &buf ) ;

	bool get_string( vector<string> &buf, int count ) ;
	bool get_float( vector<float> &buf, int count ) ;
	bool get_int( vector<int> &buf, int count ) ;

	bool get_float( float *buf, int count ) ;
	bool get_int( int *buf, int count ) ;

protected:
	bool open_data() ;
	bool open_token() ;
	void close_token() ;

protected:
	bin_ifstream m_stream ;
	int m_data_type ;
	int m_data_count ;
	int m_block_from ;
} ;



#endif
