#include "RebuildTriangle.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  RebuildTriangle
//----------------------------------------------------------------

bool RebuildTriangle::Modify( MdxBlock *block )
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

	MdxBlocks meshes ;
	meshes.EnumTree( block, MDX_MESH ) ;
	for ( int i = 0 ; i < meshes.size() ; i ++ ) {
		ModifyMesh( (MdxMesh *)meshes[ i ] ) ;
	}
	return true ;
}

void RebuildTriangle::ModifyMesh( MdxMesh *mesh )
{
	MdxBlocks src, dst ;
	vector<int> indices ;
	string arrays ;

	src.EnumChild( mesh ) ;
	src.Detach() ;
	for ( int i = 0 ; i < src.size() ; i ++ ) {
		MdxDrawArrays *cmd = (MdxDrawArrays *)src[ i ] ;
		if ( cmd->GetTypeID() != MDX_DRAW_ARRAYS ) {
			PutTriangles( indices, arrays, dst ) ;
			dst.push_back( cmd ) ;
			continue ;
		}
		if ( !GetTriangles( indices, cmd ) ) {
			PutTriangles( indices, arrays, dst ) ;
			dst.push_back( cmd ) ;
			continue ;
		}
	}
	PutTriangles( indices, arrays, dst ) ;
	dst.AttachTo( mesh ) ;
}

//----------------------------------------------------------------
//  triangles
//----------------------------------------------------------------

bool RebuildTriangle::GetTriangles( vector<int> &indices, MdxDrawArrays *cmd )
{
	int index = 0 ;

	switch ( cmd->GetMode() ) {
	    case MDX_PRIM_TRIANGLES : {
		int count = cmd->GetVertexCount() * cmd->GetPrimCount() / 3 ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int a = cmd->GetIndex( index ++ ) ;
			int b = cmd->GetIndex( index ++ ) ;
			int c = cmd->GetIndex( index ++ ) ;
			AppendTriangle( indices, a, b, c ) ;
		}
		return true ;
	    }
	    case MDX_PRIM_TRIANGLE_STRIP : {
		for ( int i = 0 ; i < cmd->GetPrimCount() ; i ++ ) {
			int flip = 0 ;
			for ( int j = 2 ; j < cmd->GetVertexCount() ; j ++ ) {
				int a = cmd->GetIndex( index ++ ) ;
				int b = cmd->GetIndex( index + flip ) ;
				flip = 1 - flip ;
				int c = cmd->GetIndex( index + flip ) ;
				AppendTriangle( indices, a, b, c ) ;
			}
			index += 2 ;
		}
		return true ;
	    }
	    case MDX_PRIM_TRIANGLE_FAN : {
		for ( int i = 0 ; i < cmd->GetPrimCount() ; i ++ ) {
			int pivot = cmd->GetIndex( index ++ ) ;
			for ( int j = 2 ; j < cmd->GetVertexCount() ; j ++ ) {
				int b = cmd->GetIndex( index ++ ) ;
				int c = cmd->GetIndex( index ) ;
				AppendTriangle( indices, pivot, b, c ) ;
			}
			index += 1 ;
		}
		return true ;
	    }
	}
	return false ;
}

bool RebuildTriangle::PutTriangles( vector<int> &indices, const string &arrays, MdxBlocks &blocks )
{
	if ( indices.empty() ) return false ;
	MdxDrawArrays *cmd = new MdxDrawArrays ;
	blocks.push_back( cmd ) ;
	cmd->SetMode( MDX_PRIM_TRIANGLES ) ;
	cmd->SetVertexCount( indices.size() ) ;
	cmd->SetPrimCount( 1 ) ;
	for ( int i = 0 ; i < (int)indices.size() ; i ++ ) {
		cmd->SetIndex( i, indices[ i ] ) ;
	}
	indices.clear() ;
	return true ;
}

bool RebuildTriangle::AppendTriangle( vector<int> &indices, int a, int b, int c )
{
	if ( a == b || b == c || c == a ) return false ;
	indices.push_back( a ) ;
	indices.push_back( b ) ;
	indices.push_back( c ) ;
	return true ;
}


} // namespace mdx
