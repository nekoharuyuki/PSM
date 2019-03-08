#include "RebuildBoundingBox.h"

namespace mdx {

//----------------------------------------------------------------
//  decls
//----------------------------------------------------------------

static const char ModeNames[] = "OFF ON MODEL BONE PART MESH" ;
enum {
	MODE_OFF,
	MODE_ON,
	MODE_MODEL,
	MODE_BONE,
	MODE_PART,
	MODE_MESH,
} ;

enum {
	SET_ALL		= ~0,
	SET_MODEL	= 1 << MODE_MODEL,
	SET_BONE	= 1 << MODE_BONE,
	SET_PART	= 1 << MODE_PART,
	SET_MESH	= 1 << MODE_MESH,
} ;

//----------------------------------------------------------------
//  RebuildBoundingBox
//----------------------------------------------------------------

bool RebuildBoundingBox::Modify( MdxBlock *block )
{
	//  check params

	m_flags = 0 ;

	int i ;

	string arg = str_toupper( GetArg( 2, "OFF" ) ) ;
	vector<string> args ;
	str_split( args, arg, ',' ) ;
	for ( i = 0 ; i < (int)args.size() ; i ++ ) {
		string arg = args[ i ] ;
		int mode = str_search( ModeNames, arg ) ;
		switch ( mode ) {
		    case MODE_OFF :
			m_flags = 0 ;
			break ;
		    case MODE_ON :
			m_flags = SET_ALL ;
			break ;
		    case MODE_MODEL :
			m_flags |= SET_MODEL ;
			break ;
		    case MODE_BONE :
			m_flags |= SET_BONE ;
			break ;
		    case MODE_PART :
			m_flags |= SET_PART ;
			break ;
		    case MODE_MESH :
			m_flags |= SET_MESH ;
			break ;
		    default :
			Error( "unknown mode \"%s\"\n", arg.c_str() ) ;
			return false ;
		}
	}
	if ( m_flags == 0 ) return true ;

	//  process

	MdxBlocks models ;
	models.EnumTree( block, MDX_MODEL ) ;
	for ( i = 0 ; i < models.size() ; i ++ ) {
		ModifyModel( (MdxModel *)models[ i ] ) ;
	}
	return true ;
}

void RebuildBoundingBox::ModifyModel( MdxModel *model )
{
	int i ;

	m_model_bbox.clear() ;
	m_bone_xforms.clear() ;
	m_part_bboxes.clear() ;
	m_mesh_bboxes.clear() ;

	MdxBlocks meshes ;
	meshes.EnumTree( model, MDX_MESH ) ;
	for ( i = 0 ; i < meshes.size() ; i ++ ) {
		ModifyMesh( (MdxMesh *)meshes[ i ] ) ;
	}
	MdxBlocks parts ;
	parts.EnumChild( model, MDX_PART ) ;
	for ( i = 0 ; i < parts.size() ; i ++ ) {
		ModifyPart( (MdxPart *)parts[ i ] ) ;
	}
	MdxBlocks bones ;
	bones.EnumChild( model, MDX_BONE ) ;
	for ( i = 0 ; i < bones.size() ; i ++ ) {
		ModifyBone( (MdxBone *)bones[ i ] ) ;
	}

	if ( m_model_bbox.valid() ) {
		if ( SET_MODEL & m_flags ) SetBounding( model, m_model_bbox ) ;
	}
}

void RebuildBoundingBox::ModifyMesh( MdxMesh *mesh )
{
	MdxSetArrays *cmd = (MdxSetArrays *)mesh->FindChild( MDX_SET_ARRAYS ) ;
	MdxArrays *arrays = ( cmd == 0 ) ? 0 : cmd->GetArraysRef() ;
	if ( arrays == 0 ) return ;

	bbox b ;

	for ( int i = 0 ; i < mesh->GetChildCount() ; i ++ ) {
		MdxPrimCommand *prim = dynamic_cast<MdxPrimCommand *>( mesh->GetChild( i ) ) ;
		if ( prim == 0 ) continue ;
		int count = prim->GetVertexCount() * prim->GetPrimCount() ;

		for ( int j = 0 ; j < arrays->GetMorphCount() ; j ++ ) {
			MdxArrays *morph = arrays->GetMorph( j ) ;
			if ( morph->GetVertexPositionCount() ) {
				for ( int k = 0 ; k < count ; k ++ ) {
					MdxVertex &v = morph->GetVertex( prim->GetIndex( k ) ) ;
					b.extend( v.GetPosition() ) ;
				}
			}
		}
	}

	if ( b.valid() ) {
		if ( SET_MESH & m_flags ) SetBounding( mesh, b ) ;
		m_mesh_bboxes[ mesh ] = b ;
	}
}

void RebuildBoundingBox::ModifyPart( MdxPart *part )
{
	bbox b ;

	MdxBlocks meshes ;
	meshes.EnumChild( part, MDX_MESH ) ;
	for ( int i = 0 ; i < meshes.size() ; i ++ ) {
		MdxMesh *mesh = (MdxMesh *)meshes[ i ] ;
		b.extend( m_mesh_bboxes[ mesh ] ) ;
	}

	if ( b.valid() ) {
		if ( SET_PART & m_flags ) SetBounding( part, b ) ;
		m_part_bboxes[ part ] = b ;
	}
}

void RebuildBoundingBox::ModifyBone( MdxBone *bone )
{
	bbox b ;

	MdxBlocks cmds ;
	cmds.EnumChild( bone, MDX_DRAW_PART ) ;
	for ( int i = 0 ; i < cmds.size() ; i ++ ) {
		MdxDrawPart *cmd = (MdxDrawPart *)cmds[ i ] ;
		MdxPart *part = cmd->GetPartRef() ;
		if ( part != 0 ) b.extend( m_part_bboxes[ part ] ) ;
	}

	if ( b.valid() ) {
		if ( SET_BONE & m_flags ) SetBounding( bone, b ) ;

		mat4 m = GetBoneMatrix( bone ) ;
		vec4 v ;
		for ( int j = 0 ; j < 8 ; j ++ ) {
			v.x = ( 1 & j ) ? b.lower.x : b.upper.x ;
			v.y = ( 2 & j ) ? b.lower.y : b.upper.y ;
			v.z = ( 4 & j ) ? b.lower.z : b.upper.z ;
			m_model_bbox.extend( transform3( m, v, 1.0f ) ) ;
		}
	}
}

const mat4 &RebuildBoundingBox::GetBoneMatrix( MdxBone *bone )
{
	map<MdxBone *,xform>::iterator it = m_bone_xforms.find( bone ) ;
	if ( it != m_bone_xforms.end() ) {
		return it->second.world_matrix ;
	}
	xform &x = m_bone_xforms[ bone ] ;

	MdxParentBone *cmd = (MdxParentBone *)bone->FindChild( MDX_PARENT_BONE ) ;
	if ( cmd != 0 ) {
		MdxBone *parent = cmd->GetBoneRef() ;
		GetBoneMatrix( parent ) ;
		x = m_bone_xforms[ parent ] ;
	}

	MdxPivot *pivot = (MdxPivot *)bone->FindChild( MDX_PIVOT ) ;
	MdxTranslate *trans = (MdxTranslate *)bone->FindChild( MDX_TRANSLATE ) ;
	MdxRotateZYX *rotzyx = (MdxRotateZYX *)bone->FindChild( MDX_ROTATE_ZYX ) ;
	MdxRotateYXZ *rotyxz = (MdxRotateYXZ *)bone->FindChild( MDX_ROTATE_YXZ ) ;
	MdxRotate *rot = (MdxRotate *)bone->FindChild( MDX_ROTATE ) ;
	MdxScale *scale = (MdxScale *)bone->FindChild( MDX_SCALE ) ;
	// MdxScale2 *scale2 = (MdxScale2 *)bone->FindChild( MDX_SCALE_2 ) ;
	// MdxScale3 *scale3 = (MdxScale3 *)bone->FindChild( MDX_SCALE_3 ) ;
	MdxScale *scale2 = 0, *scale3 = 0 ;

	mat4 m = 1.0f ;
	if ( trans != 0 ) {
		m *= mat4_from_translate( trans->GetTranslate() ) ;
	}
	if ( rotzyx != 0 ) {
		m *= mat4_from_rotzyx( rotzyx->GetRotate() * (3.1415926536f/180.0f) ) ;
	} else if ( rotyxz != 0 ) {
		m *= mat4_from_rotyxz( rotyxz->GetRotate() * (3.1415926536f/180.0f) ) ;
	} else if ( rot != 0 ) {
		m *= mat4_from_quat( rot->GetRotate() ) ;
	}
	if ( scale != 0 ) {
		m *= mat4_from_scale( scale->GetScale() ) ;
	}
	m.w.x *= x.stack_scale.x ;
	m.w.y *= x.stack_scale.y ;
	m.w.z *= x.stack_scale.z ;
	if ( pivot != 0 ) {
		vec4 p = pivot->GetPivot() * x.stack_scale ;
		m = mat4_from_translate( p ) * m * mat4_from_translate( -p ) ;
	}
	x.stack_matrix *= m ;
	x.stack_scale = x.comp_scale ;
	if ( scale == 0 ) {
		if ( scale2 != 0 ) {
			vec4 s = scale2->GetScale() ;
			x.stack_scale *= s ;
			x.comp_scale *= s ;
		} else if ( scale3 != 0 ) {
			x.stack_scale *= scale3->GetScale() ;
		}
	}
	mat4 &w = x.world_matrix ;
	w.x = x.stack_matrix.x * x.stack_scale.x ;
	w.y = x.stack_matrix.y * x.stack_scale.y ;
	w.z = x.stack_matrix.z * x.stack_scale.z ;
	w.w = x.stack_matrix.w ;
	return w ;
}

void RebuildBoundingBox::SetBounding( MdxBlock *block, const bbox &b )
{
	block->ClearChild( MDX_BOUNDING_BOX ) ;
	block->AttachChild( new MdxBoundingBox( b.lower, b.upper ) ) ;
}

//----------------------------------------------------------------
//  transform
//----------------------------------------------------------------

RebuildBoundingBox::xform::xform()
{
	clear() ;
}

void RebuildBoundingBox::xform::clear()
{
	stack_matrix = 1.0f ;
	stack_scale = 1.0f ;
	comp_scale = 1.0f ;
	world_matrix = 1.0f ;
}

void RebuildBoundingBox::xform::fix()
{
	world_matrix.x = stack_matrix.x * stack_scale.x ;
	world_matrix.y = stack_matrix.y * stack_scale.y ;
	world_matrix.z = stack_matrix.z * stack_scale.z ;
	world_matrix.w = stack_matrix.w ;
}

//----------------------------------------------------------------
//  bounding box
//----------------------------------------------------------------

RebuildBoundingBox::bbox::bbox()
{
	clear() ;
}

void RebuildBoundingBox::bbox::clear()
{
	lower.set( 1000000.0f, 1000000.0f, 1000000.0f, 1000000.0f ) ;
	upper.set( -1000000.0f, -1000000.0f, -1000000.0f, -1000000.0f ) ;
}

bool RebuildBoundingBox::bbox::valid() const
{
	return ( lower.x <= upper.x && lower.y <= upper.y && lower.z <= upper.z ) ;
}

void RebuildBoundingBox::bbox::extend( const vec4 &v )
{
	if ( v.x < lower.x ) lower.x = v.x ;
	if ( v.y < lower.y ) lower.y = v.y ;
	if ( v.z < lower.z ) lower.z = v.z ;
	if ( v.x > upper.x ) upper.x = v.x ;
	if ( v.y > upper.y ) upper.y = v.y ;
	if ( v.z > upper.z ) upper.z = v.z ;
}

void RebuildBoundingBox::bbox::extend( const bbox &b )
{
	if ( b.lower.x < lower.x ) lower.x = b.lower.x ;
	if ( b.lower.y < lower.y ) lower.y = b.lower.y ;
	if ( b.lower.z < lower.z ) lower.z = b.lower.z ;
	if ( b.upper.x > upper.x ) upper.x = b.upper.x ;
	if ( b.upper.y > upper.y ) upper.y = b.upper.y ;
	if ( b.upper.z > upper.z ) upper.z = b.upper.z ;
}


} // namespace mdx
