#include "LimitVertexBlend.h"
#include "LimitVertexBlend100.h"

namespace mdx {

static void rename_arrays( MdxArrays *arrays, MdxArrays *arrays2 ) ;
static int count_common( const vector<int> &v1, const vector<int> &v2 ) ;
static int merge_union( vector<int> &dst, const vector<int> &v1, const vector<int> &v2 ) ;
static int cost_of_face( int prim, int n_verts ) ;
static bool compare_float( float *ptr1, float *ptr2 ) ;

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

#define MAX_WEIGHTS		(256)
#define DEFAULT_LIMIT_WEIGHT	(4)
#define DEFAULT_LIMIT_MATRIX	(16)
#define REMOVE_SUFFIX		(1)	//  remove unnecessary suffixes
#define REMOVE_DEGEN		(1)	//  remove degenerated faces

static const char ModeNames[] = "OFF ON" ;
enum {
	MODE_OFF,
	MODE_ON,
} ;

//----------------------------------------------------------------
//  decls ( costs )
//----------------------------------------------------------------

#define MAX_COST (1000000)

static const char VarCostMesh[] = "blend_cost_mesh" ;
static const char VarCostMatrix[] = "blend_cost_matrix" ;

static bool g_optimize = false ;
static int g_cost_mesh = MAX_COST ;
static int g_cost_matrix = 10 ;

//----------------------------------------------------------------
//  LimitVertexBlend
//----------------------------------------------------------------

bool LimitVertexBlend::Modify( MdxBlock *block )
{
	//  check params

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	if ( str_isdigit( arg ) ) {
		m_limit_weight = str_atoi( arg ) ;
		if ( m_limit_weight < 0 || m_limit_weight > MAX_WEIGHTS ) {
			Error( "illegal weight limit \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
	} else {
		m_limit_weight = DEFAULT_LIMIT_WEIGHT ;
		int mode = str_search( ModeNames, arg ) ;
		if ( mode < 0 ) {
			Error( "unknown weight mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		// if ( mode == MODE_OFF ) return true ;
		if ( mode == MODE_OFF ) m_limit_weight = MAX_WEIGHTS ;
	}
	arg = str_toupper( GetArg( 3, "OFF" ) ) ;
	if ( str_isdigit( arg ) ) {
		m_limit_matrix = str_atoi( arg ) ;
		if ( m_limit_matrix < m_limit_weight || m_limit_matrix > MAX_WEIGHTS ) {
			Error( "illegal matrix limit \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
	} else {
		m_limit_matrix = DEFAULT_LIMIT_MATRIX ;
		int mode = str_search( ModeNames, arg ) ;
		if ( mode < 0 ) {
			Error( "unknown matrix mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
		// if ( mode == MODE_OFF ) return true ;
		// if ( mode == MODE_OFF ) m_limit_matrix = MAX_WEIGHTS ;
		if ( mode == MODE_OFF ) m_limit_matrix = 0 ;	//  use old version
	}
	if ( m_limit_weight >= MAX_WEIGHTS && m_limit_matrix >= MAX_WEIGHTS ) return true ;

	if ( m_limit_matrix <= m_limit_weight ) {
		MdxProc *proc = GetShell()->GetProc( "LimitVertexBlend100" ) ;
		if ( proc != 0 ) return ( proc->Eval( GetShell(), GetArgc(), GetArgv() ) == MDX_EVAL_OK ) ;
	}

	//  check vars

	arg = GetVar( VarCostMesh ) ;
	if ( str_isdigit( arg ) ) g_cost_mesh = str_atoi( arg ) ;
	g_cost_mesh = str_isdigit( arg ) ? str_atoi( arg ) : MAX_COST ;
	arg = GetVar( VarCostMatrix ) ;
	if ( str_isdigit( arg ) ) g_cost_matrix = str_atoi( arg ) ;
	g_cost_matrix = str_isdigit( arg ) ? str_atoi( arg ) : 10 ;

	if ( g_cost_mesh >= MAX_COST ) g_cost_mesh = MAX_COST ;
	g_optimize = ( g_cost_mesh < MAX_COST ) ;

	//  process

	MdxBlocks parts ;
	parts.EnumTree( block, MDX_PART ) ;
	for ( int i = 0 ; i < parts.size() ; i ++ ) {
		ModifyPart( (MdxPart *)parts[ i ] ) ;
	}
	return true ;
}

void LimitVertexBlend::ModifyPart( MdxPart *part )
{
	int i ;

	MdxBlocks meshes ;
	meshes.EnumChild( part, MDX_MESH ) ;
	MdxBlocks arrayz ;
	arrayz.EnumChild( part, MDX_ARRAYS ) ;

	//  check arrays

	int max_weights = 0 ;
	bool has_indices = false ;
	for ( i = 0 ; i < arrayz.size() ; i ++ ) {
		MdxArrays *arrays = (MdxArrays *)arrayz[ i ] ;
		int n_weights = arrays->GetVertexWeightCount() ;
		if ( max_weights < n_weights ) max_weights = n_weights ;
		if ( arrays->GetVertexBlendIndicesMode() ) has_indices = true ;
	}
	if ( max_weights <= 1 ) return ;

	//  check bones

	int n_bones = 0 ;
	MdxBlocks draws ;
	draws.EnumReferrer( part, MDX_DRAW_PART ) ;
	for ( i = 0 ; i < draws.size() ; i ++ ) {
		MdxBone *bone = (MdxBone *)draws[ i ]->GetParent() ;
		int count = bone->GetChildCount( MDX_BLEND_BONE ) ;
		if ( n_bones < count ) n_bones = count ;
	}
	if ( n_bones <= 1 ) return ;

	//  check meshes

	int max_matrices = 0 ;
	for ( i = 0 ; i < meshes.size() ; i ++ ) {
		MdxMesh *mesh = (MdxMesh *)meshes[ i ] ;
		MdxBlendIndices *blend = (MdxBlendIndices *)mesh->FindChild( MDX_BLEND_INDICES ) ;
		int n_matrices = ( blend == 0 ) ? n_bones : blend->GetIndexCount() ;
		if ( max_matrices < n_matrices ) max_matrices = n_matrices ;
	}

	//  need conversion ?

	if ( !g_optimize && max_matrices <= m_limit_matrix && max_weights <= m_limit_weight ) return ;

	//  TODO: current version cannot reconvert BLENDINDICES
	//  TODO: BLENDINDICES should be removed before conversion
	if ( has_indices ) return ;

	//  check meshes

	for ( i = 0 ; i < meshes.size() ; i ++ ) {
		ModifyMesh( (MdxMesh *)meshes[ i ], n_bones ) ;
	}

	//  cleanup

	for ( i = 0 ; i < arrayz.size() ; i ++ ) {
		MdxArrays *arrays = (MdxArrays *)arrayz[ i ] ;
		if ( arrays->FindReferrer() == 0 ) {
			#if ( REMOVE_SUFFIX )
			MdxArrays *arrays2 = m_arrays_output[ arrays ] ;
			if ( arrays2 != 0 && arrays2 != arrays ) rename_arrays( arrays2, arrays ) ;
			#endif // REMOVE_SUFFIX
			arrays->Release() ;
		}
	}

	#if ( REMOVE_SUFFIX )
	m_mesh_output.clear() ;
	m_arrays_output.clear() ;
	#endif // REMOVE_SUFFIX
}

void LimitVertexBlend::ModifyMesh( MdxMesh *mesh, int n_bones )
{
	MdxSetArrays *set_arrays = (MdxSetArrays *)mesh->FindChild( MDX_SET_ARRAYS ) ;
	MdxArrays *arrays = ( set_arrays == 0 ) ? 0 : set_arrays->GetArraysRef() ;
	int n_weights = ( arrays == 0 ) ? 0 : arrays->GetVertexWeightCount() ;
	MdxBlendIndices *blend = (MdxBlendIndices *)mesh->FindChild( MDX_BLEND_INDICES ) ;
	int n_matrices = ( blend == 0 ) ? n_bones : blend->GetIndexCount() ;

	//  need conversion ?

	// if ( !g_optimize && n_matrices <= m_limit_matrix && n_weights <= m_limit_weight ) arrays = 0 ;
	if ( !g_optimize && n_matrices <= m_limit_matrix && n_weights <= m_limit_weight ) return ;

	int n_prims = 0 ;

	MdxBlocks cmds ;
	cmds.EnumChild( mesh ) ;
	for ( int i = 0 ; i < cmds.size() ; i ++ ) {
		MdxDrawArrays *cmd = (MdxDrawArrays *)cmds[ i ] ;
		if ( dynamic_cast<MdxPrimCommand *>( cmds[ i ] ) != 0 ) n_prims ++ ;
		if ( cmd->GetTypeID() != MDX_DRAW_ARRAYS ) continue ;
		if ( arrays != 0 ) {
			GetFaces( cmd ) ;
			cmd->Release() ;
			-- n_prims ;
		}
	}
	PutFaces( arrays, mesh ) ;

	if ( n_prims == 0 ) {
		#if ( REMOVE_SUFFIX )
		MdxMesh *mesh2 = m_mesh_output[ mesh ] ;
		if ( mesh2 != 0 ) mesh2->SetName( mesh->GetName() ) ;
		#endif // REMOVE_SUFFIX
		mesh->Release() ;
	}
}

//----------------------------------------------------------------
//  faces
//----------------------------------------------------------------

bool LimitVertexBlend::GetFaces( MdxDrawArrays *cmd )
{
	int index = 0 ;

	switch ( cmd->GetMode() ) {
	    case MDX_PRIM_TRIANGLES : {
		int count = cmd->GetVertexCount() * cmd->GetPrimCount() / 3 ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int a = cmd->GetIndex( index ++ ) ;
			int b = cmd->GetIndex( index ++ ) ;
			int c = cmd->GetIndex( index ++ ) ;
			AppendFace( MDX_PRIM_TRIANGLES, 3, a, b, c ) ;
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
				AppendFace( MDX_PRIM_TRIANGLES, 3, a, b, c ) ;
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
				AppendFace( MDX_PRIM_TRIANGLES, 3, pivot, b, c ) ;
			}
			index += 1 ;
		}
		return true ;
	    }
	    case MDX_PRIM_LINES : {
		int count = cmd->GetVertexCount() * cmd->GetPrimCount() / 2 ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int a = cmd->GetIndex( index ++ ) ;
			int b = cmd->GetIndex( index ++ ) ;
			AppendFace( MDX_PRIM_LINES, 2, a, b ) ;
		}
		return true ;
	    }
	    case MDX_PRIM_LINE_STRIP : {
		for ( int i = 0 ; i < cmd->GetPrimCount() ; i ++ ) {
			for ( int j = 1 ; j < cmd->GetVertexCount() ; j ++ ) {
				int a = cmd->GetIndex( index ++ ) ;
				int b = cmd->GetIndex( index ) ;
				AppendFace( MDX_PRIM_LINES, 2, a, b ) ;
			}
			index += 1 ;
		}
		return true ;
	    }
	    case MDX_PRIM_POINTS : {
		int count = cmd->GetVertexCount() * cmd->GetPrimCount() ;
		for ( int i = 0 ; i < count ; i ++ ) {
			int a = cmd->GetIndex( index ++ ) ;
			AppendFace( MDX_PRIM_POINTS, 1, a ) ;
		}
		return true ;
	    }
	}
	return false ;
}

bool LimitVertexBlend::PutFaces( MdxArrays *arrays, MdxMesh *mesh )
{
	if ( arrays == 0 || mesh == 0 || mesh->GetParent() == 0 ) return false ;
	if ( m_faces.empty() ) return false ;

	int i, j, k ;
	int n_weights = arrays->GetVertexWeightCount() ;
	if ( n_weights > m_limit_weight ) CheckFaces( arrays ) ;

	//  make groups

	map<widx_group, face_group> groups ;		//  weight indices -> face indices
	for ( i = 0 ; i < (int)m_faces.size() ; i ++ ) {
		face &f = m_faces[ i ] ;

		//  count matrices per face
		widx_group widxs ;
		for ( j = 0 ; j < n_weights ; j ++ ) {
			for ( k = 0 ; k < f.n_verts ; k ++ ) {
				MdxVertex &v = arrays->GetVertex( f.verts[ k ] ) ;
				if ( v.GetWeight( j ) > 0.0f ) {
					widxs.push_back( j ) ;
					break ;
				}
			}
		}

		//  count weights per vertex
		int max_weights = 0 ;
		for ( k = 0 ; k < f.n_verts ; k ++ ) {
			int count = 0 ;
			for ( j = 0 ; j < n_weights ; j ++ ) {
				MdxVertex &v = arrays->GetVertex( f.verts[ k ] ) ;
				if ( v.GetWeight( j ) > 0.0f ) count ++ ;
			}
			if ( max_weights < count ) max_weights = count ;
		}

		face_group &faces = groups[ widxs ] ;
		faces.push_back( i ) ;
		faces.n_verts += f.n_verts ;
		// faces.n_weights = widxs.size() ;
		if ( faces.n_weights < max_weights ) faces.n_weights = max_weights ;
	}

	//  merge groups

	while ( groups.size() > 1 ) {
		map<widx_group, face_group>::iterator mit1 = groups.end(), mit2 = groups.end() ;
		map<widx_group, face_group>::iterator it1, it2 ;
		int min_cost = 1 ;
		for ( it1 = groups.begin() ; it1 != groups.end() ; it1 ++ ) {
			for ( it2 = it1, it2 ++ ; it2 != groups.end() ; it2 ++ ) {
				const widx_group &widxs1 = it1->first, &widxs2 = it2->first ;
				const face_group &faces1 = it1->second, &faces2 = it2->second ;
				int common = count_common( widxs1, widxs2 ) ;
				int n_matrices = widxs1.size() + widxs2.size() - common ;
				if ( n_matrices > m_limit_matrix ) continue ;
				int n_weights = ( faces1.n_weights > faces2.n_weights ) ? faces1.n_weights : faces2.n_weights ;
				int cost = 0 ;
				cost -= g_cost_mesh ;
				cost -= g_cost_matrix * common ;
				cost += faces1.n_verts * ( n_weights - faces1.n_weights ) ;
				cost += faces2.n_verts * ( n_weights - faces2.n_weights ) ;
				if ( min_cost > cost ) {
					min_cost = cost ;
					mit1 = it1 ;
					mit2 = it2 ;
				}
			}
		}
		if ( min_cost > 0 ) break ;
		widx_group widxs ;
		face_group faces ;
		merge_union( widxs, mit1->first, mit2->first ) ;
		merge_union( faces, mit1->second, mit2->second ) ;
		faces.n_verts = mit1->second.n_verts + mit2->second.n_verts ;
		faces.n_weights = ( mit1->second.n_weights > mit2->second.n_weights ) ? mit1->second.n_weights : mit2->second.n_weights ;
		groups.erase( mit1 ) ;
		groups.erase( mit2 ) ;
		face_group &faces2 = groups[ widxs ] ;
		if ( faces2.empty() ) {
			groups[ widxs ] = faces ;
		} else {
			face_group faces3 ;
			merge_union( faces3, faces2, faces ) ;
			faces3.n_verts = faces2.n_verts + faces.n_verts ;
			faces3.n_weights = ( faces2.n_weights > faces.n_weights ) ? faces2.n_weights : faces.n_weights ;
			groups[ widxs ] = faces3 ;
		}
	}

	//  output groups

	map<widx_group, face_group>::iterator it ;
	for ( it = groups.begin() ; it != groups.end() ; it ++ ) {
		const widx_group &widxs = it->first ;
		const face_group &faces = it->second ;
		int n_matrices = widxs.size() ;
		int n_faces = faces.size() ;

		//  create mesh and arrays

		MdxMesh *mesh2 = new MdxMesh( mesh->GetName() ) ;
		MdxArrays *arrays2 = new MdxArrays( arrays->GetName() ) ;
		MdxPart *parent = (MdxPart *)mesh->GetParent() ;
		parent->AttachChild( mesh2 ) ;
		parent->AttachChild( arrays2 ) ;
		#if ( REMOVE_SUFFIX )
		m_mesh_output[ mesh ] = ( m_mesh_output[ mesh ] != 0 ) ? mesh : mesh2 ;
		m_arrays_output[ arrays ] = ( m_arrays_output[ arrays ] != 0 ) ? arrays : arrays2 ;
		#endif // REMOVE_SUFFIX

		mesh2->AttachChild( new MdxSetArrays( arrays2->GetName() ) ) ;

		arrays2->SetFormat( arrays->GetFormat() ) ;
		arrays2->SetMorphCount( arrays->GetMorphCount() ) ;
		arrays2->SetMorphFormat( arrays->GetMorphFormat() ) ;
		arrays2->SetVertexWeightCount( faces.n_weights ) ;
		arrays2->SetVertexBlendIndicesMode( true ) ;

		MdxSetMaterial *material = (MdxSetMaterial *)mesh->FindChild( MDX_SET_MATERIAL ) ;
		if ( material != 0 ) mesh2->AttachChild( material->Clone() ) ;

		MdxBlendIndices *blend = (MdxBlendIndices *)mesh->FindChild( MDX_BLEND_INDICES ) ;
		MdxBlendIndices *blend2 = new MdxBlendIndices ;
		mesh2->AttachChild( blend2 ) ;
		blend2->SetIndexCount( n_matrices ) ;
		for ( i = 0 ; i < n_matrices ; i ++ ) {
			int index = widxs[ i ] ;
			if ( blend != 0 ) index = blend->GetIndex( index ) ;
			blend2->SetIndex( i, index ) ;
		}

		//  output prims

		map<int,int> vids ;		// old vertex -> new vertex
		MdxDrawArrays *cmd = 0 ;
		int num = 0 ;
		int prim = -1 ;

		for ( i = 0 ; i < n_faces ; i ++ ) {
			face &f = m_faces[ faces[ i ] ] ;

			if ( prim != f.prim ) {
				prim = f.prim ;
				cmd = 0 ;
				num = 0 ;
			}
			if ( cmd == 0 ) {
				mesh2->AttachChild( cmd = new MdxDrawArrays ) ;
				cmd->SetMode( prim ) ;
				if ( f.n_verts <= 3 ) {
					cmd->SetPrimCount( 1 ) ;
				} else {
					cmd->SetVertexCount( f.n_verts ) ;
				}
			}
			if ( f.n_verts <= 3 ) {
				cmd->SetVertexCount( num + f.n_verts ) ;
			} else {
				cmd->SetPrimCount( ( num + f.n_verts ) / f.n_verts ) ;
			}
			for ( j = 0 ; j < f.n_verts ; j ++ ) {
				int vid = ( f.verts[ j ] & ~0x80000000 ) ;
				map<int,int>::iterator it = vids.find( vid ) ;
				if ( it != vids.end() ) {
					cmd->SetIndex( num ++, it->second ) ;
					continue ;
				}

				//  append vertex

				int vid2 = arrays2->GetVertexCount() ;
				arrays2->SetVertexCount( vid2 + 1 ) ;
				cmd->SetIndex( num ++, vid2 ) ;
				vids[ vid ] = vid2 ;

				int n_morphs = arrays->GetMorphCount() ;
				for ( k = 0 ; k < n_morphs ; k ++ ) {
					MdxVertex &v = arrays->GetMorph( k )->GetVertex( vid ) ;
					MdxVertex &v2 = arrays2->GetMorph( k )->GetVertex( vid2 ) ;
					v2 = v ;
					//  BLENDWEIGHT are not copied
					v2.SetFormat( arrays2->GetFormat() ) ;
				}
				MdxVertex &v = arrays->GetVertex( vid ) ;
				MdxVertex &v2 = arrays2->GetVertex( vid2 ) ;
				int num2 = 0 ;
				for ( k = 0 ; k < n_matrices ; k ++ ) {
					float weight = v.GetWeight( widxs[ k ] ) ;
					if ( weight == 0.0f ) continue ;
					v2.SetWeight( num2, weight ) ;
					v2.SetBlendIndex( num2, k ) ;
					if ( ++ num2 >= faces.n_weights ) break ;
				}
				for ( ; num2 < faces.n_weights ; num2 ++ ) {
					v2.SetWeight( num2, 0.0f ) ;
					v2.SetBlendIndex( num2, 0 ) ;
				}
			}
		}
	}

	m_faces.clear() ;
	return true ;
}

void LimitVertexBlend::CheckFaces( MdxArrays *arrays )
{
	if ( arrays == 0 ) return ;

	float priorities[ MAX_WEIGHTS ] ;
	int n_weights = arrays->GetVertexWeightCount() ;
	int i, j, k ;

	for ( i = 0 ; i < (int)m_faces.size() ; i ++ ) {
		memset( priorities, 0, sizeof( float ) * n_weights ) ;
		face &f = m_faces[ i ] ;

		int nonzero = 0 ;
		for ( j = 0 ; j < f.n_verts ; j ++ ) {
			MdxVertex &v = arrays->GetVertex( f.verts[ j ] ) ;
			float max_w = 0.0f ;
			int max_k = -1 ;
			for ( k = 0 ; k < n_weights ; k ++ ) {
				float w = v.GetWeight( k ) ;
				if ( w == 0.0f ) continue ;
				if ( priorities[ k ] == 0.0f ) nonzero ++ ;
				priorities[ k ] += w ;
				if ( w > max_w ) {
					max_w = w ;
					max_k = k ;
				}
			}
			if ( max_k >= 0 ) priorities[ max_k ] += (float)f.n_verts ;
		}
		if ( nonzero <= m_limit_weight ) continue ;

		vector<float *> pointers ;
		for ( j = 0 ; j < n_weights ; j ++ ) pointers.push_back( priorities + j ) ;
		stable_sort( pointers.begin(), pointers.end(), compare_float ) ;
		for ( j = m_limit_weight ; j < n_weights ; j ++ ) *( pointers[ j ] ) = 0.0f ;

		for ( j = 0 ; j < f.n_verts; j ++ ) {
			MdxVertex &v = arrays->GetVertex( f.verts[ j ] ) ;
			float sum = 0.0f ;
			for ( k = 0 ; k < n_weights ; k ++ ) {
				if ( priorities[ k ] > 0.0f ) sum += v.GetWeight( k ) ;
			}
			if ( sum == 0.0f ) continue ;
			for ( k = 0 ; k < n_weights ; k ++ ) {
				float w = 0.0f ;
				if ( priorities[ k ] > 0.0f ) w = v.GetWeight( k ) / sum ;
				v.SetWeight( k, w ) ;
			}
		}
	}
}

bool LimitVertexBlend::AppendFace( int prim, int n_verts, int a, int b, int c, int d )
{
	#if ( REMOVE_DEGEN )
	if ( n_verts == 2 ) if ( a == b ) return false ;
	if ( n_verts == 3 ) if ( a == b || b == c || c == a ) return false ;
	if ( n_verts == 4 ) {
		if ( a == b ) return AppendFace( MDX_PRIM_TRIANGLES, 3, b, c, d ) ;
		if ( b == c ) return AppendFace( MDX_PRIM_TRIANGLES, 3, c, d, a ) ;
		if ( c == d ) return AppendFace( MDX_PRIM_TRIANGLES, 3, d, a, b ) ;
		if ( d == a ) return AppendFace( MDX_PRIM_TRIANGLES, 3, a, b, c ) ;
	}
	#endif // REMOVE_DEGEN

	face f ;
	f.prim = prim ;
	f.n_verts = n_verts ;
	f.verts[ 0 ] = a ;
	f.verts[ 1 ] = b ;
	f.verts[ 2 ] = c ;
	f.verts[ 3 ] = d ;
	m_faces.push_back( f ) ;
	return true ;
}

//----------------------------------------------------------------
//  subroutines
//----------------------------------------------------------------

void rename_arrays( MdxArrays *arrays, MdxArrays *arrays2 )
{
	MdxBlocks ref ;
	ref.EnumReferrer( arrays ) ;
	for ( int i = 0 ; i < ref.size() ; i ++ ) {
		MdxBlock *block = ref[ i ] ;
		for ( int j = 0 ; j < block->GetArgsCount() ; j ++ ) {
			int desc = block->GetArgsDesc( j ) ;
			if ( MDX_WORD_CLASS( desc ) != MDX_WORD_REF ) continue ;
			if ( MDX_WORD_SCOPE( desc ) != MDX_ARRAYS ) continue ;
			if ( strcmp( block->GetArgsString( j ), arrays->GetName() ) ) continue ;
			block->SetArgsString( j, arrays2->GetName() ) ;
		}
	}
	arrays->SetName( arrays2->GetName() ) ;
}

int count_common( const vector<int> &v1, const vector<int> &v2 )
{
	int j = 0, common = 0 ;
	for ( int i = 0 ; i < (int)v1.size() ; i ++ ) {
		int v = v1[ i ] ;
		while ( j < (int)v2.size() && v2[ j ] < v ) j ++ ;
		if ( j >= (int)v2.size() ) break ;
		if ( v2[ j ] == v ) common ++ ;
	}
	return common ;
}

int merge_union( vector<int> &dst, const vector<int> &v1, const vector<int> &v2 )
{
	int j = 0 ;
	for ( int i = 0 ; i < (int)v1.size() ; i ++ ) {
		int v = v1[ i ] ;
		while ( j < (int)v2.size() && v2[ j ] < v ) dst.push_back( v2[ j ++ ] ) ;
		if ( j < (int)v2.size() && v2[ j ] == v ) j ++ ;
		dst.push_back( v ) ;
	}
	while ( j < (int)v2.size() ) dst.push_back( v2[ j ++ ] ) ;
	return dst.size() ;
}

bool compare_float( float *ptr1, float *ptr2 )
{
	return ( *ptr1 > *ptr2 ) ;
}


} // namespace mdx
