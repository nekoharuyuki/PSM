#include "ImportMDX.h"

namespace mdx {

#define SUPPORT_PSMSTYLE 1
#define PRESERVE_DATATYPE 1

//----------------------------------------------------------------
//  ImportMDX
//----------------------------------------------------------------

bool ImportMDX::Import( MdxBlock *&block, const void *buf, int size )
{
	block = 0 ;

	if ( !PreProcess( buf, size ) ) {
		return false ;
	}
	MDXHeader *header = (MDXHeader *)buf ;
	if ( !ImportHeader( header, size ) ) {
		return false ;
	}
	MDXChunk *chunk = (MDXChunk *)( header + 1 ) ;
	if ( !ImportBlock( block, 0, chunk ) ) {
		block->Release() ;
		block = 0 ;
		return false ;
	}
	if ( !PostProcess( block ) ) {
		block->Release() ;
		block = 0 ;
		return false ;
	}
	return true ;
}

bool ImportMDX::ImportHeader( MDXHeader *header, int size )
{
	if ( size < sizeof( MDXHeader ) + sizeof( MDXChunk ) ) {
		Error( "incomplete header\n" ) ;
		return false ;
	}
	if ( header->signature == MDX_FORMAT_SIGNATURE ) {
		if ( header->version != MDX_FORMAT_VERSION ) {
			Error( "wrong version\n" ) ;
			return false ;
		}
	} else {
		Error( "wrong format\n" ) ;
		return false ;
	}
	m_format_style = header->style ;
	m_format_option = header->option ;

	#if ( SUPPORT_PSMSTYLE )
	if ( m_format_style != 0 ) m_format_style = MDX_FORMAT_STYLE_PSM ;
	#endif // SUPPORT_PSMSTYLE
	return true ;
}

bool ImportMDX::ImportBlock( MdxBlock *&block, MdxBlock *parent, MDXChunk *chunk )
{
	if ( !CreateBlock( block, chunk ) ) return false ;
	if ( block == 0 ) return ( parent != 0 ) ;
	if ( !block->IsCommand() && MDX_CHUNK_NAMESIZE( chunk ) > 0 ) {
		block->SetName( MDX_CHUNK_NAME( chunk ) ) ;
	}
	if ( parent != 0 ) parent->AttachChild( block ) ;
	if ( !ImportSpecific( block, chunk ) ) return false ;
	return true ;
}

bool ImportMDX::CreateBlock( MdxBlock *&block, MDXChunk *chunk )
{
	block = 0 ;
	int type_id = MDX_CHUNK_TYPE( chunk ) ;
	if ( m_format != 0 ) {
		MdxChunk *templ = m_format->GetTemplate( type_id ) ;
		if ( templ != 0 ) {
			block = (MdxBlock *)templ->Clone() ;
		} else if ( !MDX_CHUNK_IS_SHORT( chunk ) ) {
			block = new MdxUnknownBlock( type_id ) ;
		} else {
			block = new MdxUnknownCommand( type_id ) ;
		}
	}
	return true ;
}

bool ImportMDX::ImportSpecific( MdxBlock *block, MDXChunk *chunk )
{
	#if ( SUPPORT_PSMSTYLE )
	if ( m_format_style == MDX_FORMAT_STYLE_PSM ) {
		switch ( block->GetTypeID() ) {
		    case MDX_MODEL :
			return ImportModelPSM( (MdxModel *)block, chunk ) ;
		    case MDX_VERTEX_OFFSET :
			return ImportVertexOffsetPSM( (MdxVertexOffset *)block, chunk ) ;
		    case MDX_ARRAYS :
			return ImportArraysPSM( (MdxArrays *)block, chunk ) ;
		    case MDX_FCURVE :
			return ImportFCurvePSM( (MdxFCurve *)block, chunk ) ;
		}
	}
	#endif // SUPPORT_PSMSTYLE

	return ImportGeneric( block, chunk ) ;
}

bool ImportMDX::ImportGeneric( MdxBlock *block, MDXChunk *chunk )
{
	if ( block->IsCommand() ) {
		if ( !ImportArgs( block, chunk ) ) return false ;
		if ( !CheckDirective( block ) ) return false ;
	} else {
		if ( !ImportArgs( block, chunk ) ) return false ;
		if ( !ImportData( block, chunk ) ) return false ;
		if ( !ImportChildren( block, chunk ) ) return false ;
		if ( !CheckDirective( block ) ) return false ;
	}
	return true ;
}

bool ImportMDX::ImportArgs( MdxBlock *block, MDXChunk *chunk )
{
	void *buf = MDX_CHUNK_ARGS( chunk ) ;
	int size = MDX_CHUNK_ARGSSIZE( chunk ) ;
	block->UpdateArgs() ;
	if ( !block->SetArgsImage( buf, size, m_format_style, m_format_option )
	  || !block->FlushArgs() ) {
		Error( "wrong chunk args\n" ) ;
		return false ;
	}
	return true ;
}

bool ImportMDX::ImportData( MdxBlock *block, MDXChunk *chunk )
{
	void *buf = MDX_CHUNK_DATA( chunk ) ;
	int size = MDX_CHUNK_DATASIZE( chunk ) ;
	block->UpdateData() ;
	if ( !block->SetDataImage( buf, size, m_format_style, m_format_option )
	  || !block->FlushData() ) {
		Error( "wrong chunk data\n" ) ;
		return false ;
	}
	return true ;
}

bool ImportMDX::ImportChildren( MdxBlock *block, MDXChunk *chunk )
{
	MDXChunk *end = MDX_CHUNK_NEXT( chunk ) ;
	for ( chunk = MDX_CHUNK_CHILD( chunk ) ; chunk < end ; chunk = MDX_CHUNK_NEXT( chunk ) ) {
		MdxBlock *child ;
		if ( !ImportBlock( child, block, chunk ) ) return false ;
	}
	return true ;
}

bool ImportMDX::CheckDirective( MdxBlock *block )
{
	if ( m_format == 0 ) return true ;

	switch ( block->GetTypeID() ) {
	    case MDX_DEFINE_ENUM : {
		MdxDefineEnum *def = (MdxDefineEnum *)block ;
		int scope = m_format->SetScope( def->GetDefScope(), MDX_SCOPE_VOLATILE ) ;
		m_format->SetSymbol( scope, def->GetDefName(), def->GetDefValue() ) ;
		return true ;
	    }
	    case MDX_DEFINE_BLOCK : {
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
	return true ;
}

//----------------------------------------------------------------
//  pre-process / post-process
//----------------------------------------------------------------

bool ImportMDX::PreProcess( const void *buf, int size )
{
	m_format_style = 0 ;
	m_format_option = 0 ;
	m_vertex_offset = 0.0f ;
	m_vertex_scale = 1.0f ;
	m_tcoord_offset = 0.0f ;
	m_tcoord_scale = 1.0f ;

	#if ( PRESERVE_DATATYPE )
	m_vertex_type = -1 ;
	m_normal_type = -1 ;
	m_vcolor_type = -1 ;
	m_tcoord_type = -1 ;
	m_weight_type = -1 ;
	m_color_type = -1 ;
	m_index_type = -1 ;
	m_keyframe_type = -1 ;
	#endif // PRESERVE_DATATYPE

	return true ;
}

bool ImportMDX::PostProcess( MdxBlock *&block )
{
	ResolveReference( block ) ;

	#if ( PRESERVE_DATATYPE )
	static const char *Types[] = { "float", "half", "byte" } ;
	if ( strcmp( GetVar( "format_vertex" ), "default" ) == 0 && m_vertex_type >= 0 ) {
		m_shell->SetVar( "format_vertex", Types[ m_vertex_type ] ) ;
	}
	if ( strcmp( GetVar( "format_normal" ), "default" ) == 0 && m_normal_type >= 0 ) {
		m_shell->SetVar( "format_normal", Types[ m_normal_type ] ) ;
	}
	if ( strcmp( GetVar( "format_vcolor" ), "default" ) == 0 && m_vcolor_type >= 0 ) {
		m_shell->SetVar( "format_vcolor", Types[ m_vcolor_type ] ) ;
	}
	if ( strcmp( GetVar( "format_tcoord" ), "default" ) == 0 && m_tcoord_type >= 0 ) {
		m_shell->SetVar( "format_tcoord", Types[ m_tcoord_type ] ) ;
	}
	if ( strcmp( GetVar( "format_weight" ), "default" ) == 0 && m_weight_type >= 0 ) {
		m_shell->SetVar( "format_weight", Types[ m_weight_type ] ) ;
	}
	if ( strcmp( GetVar( "format_color" ), "default" ) == 0 && m_color_type >= 0 ) {
		m_shell->SetVar( "format_color", Types[ m_color_type ] ) ;
	}
	if ( strcmp( GetVar( "format_keyframe" ), "default" ) == 0 && m_keyframe_type >= 0 ) {
		m_shell->SetVar( "format_keyframe", Types[ m_keyframe_type ] ) ;
	}
	#endif // PRESERVE_DATATYPE

	return true ;
}

bool ImportMDX::ResolveReference( MdxBlock *block )
{
	for ( int i = 0 ; i < block->GetChildCount() ; i ++ ) {
		ResolveReference( (MdxBlock *)block->GetChild( i ) ) ;
	}
	if ( block->HasArgsRef() ) {
		for ( int i = 0 ; i < block->GetArgsCount() ; i ++ ) {
			MdxBlock *ref = (MdxBlock *)block->GetArgsRef( i ) ;
			if ( ref != 0 && str_isdigit( block->GetArgsString( i ) ) ) {
				block->SetArgsString( i, ref->GetName() ) ;
			}
		}
		block->FlushArgs() ;
	}
	if ( block->HasDataRef() ) {
		for ( int i = 0 ; i < block->GetDataCount() ; i ++ ) {
			MdxBlock *ref = (MdxBlock *)block->GetDataRef( i ) ;
			if ( ref != 0 && str_isdigit( block->GetDataString( i ) ) ) {
				block->SetDataString( i, ref->GetName() ) ;
			}
		}
		block->FlushData() ;
	}
	return true ;
}

//----------------------------------------------------------------
//  "PSM" specific input
//----------------------------------------------------------------

#if ( SUPPORT_PSMSTYLE )

bool ImportMDX::ImportModelPSM( MdxModel *model, MDXChunk *chunk )
{
	m_vertex_offset = 0.0f ;
	m_vertex_scale = 1.0f ;
	m_tcoord_offset = 0.0f ;
	m_tcoord_scale = 1.0f ;
	return ImportGeneric( model, chunk ) ;
}

bool ImportMDX::ImportVertexOffsetPSM( MdxVertexOffset *cmd, MDXChunk *chunk )
{
	MDXVertexOffset *args = (MDXVertexOffset *)MDX_CHUNK_ARGS( chunk ) ;
	int format = args->format ;
	float *offset = args->offset ;
	switch ( format ) {
	    case MDX_VF_POSITION :
		m_vertex_offset.set( offset[ 0 ], offset[ 1 ], offset[ 2 ] ) ;
		m_vertex_scale.set( offset[ 3 ], offset[ 4 ], offset[ 5 ] ) ;
		break ;
	    case MDX_VF_TEXCOORD :
		m_tcoord_offset.set( offset[ 0 ], offset[ 1 ] ) ;
		m_tcoord_scale.set( offset[ 2 ], offset[ 3 ] ) ;
		break ;
	}
	cmd->Release() ;	// removed
	return true ;
}

bool ImportMDX::ImportArraysPSM( MdxArrays *arrays, MDXChunk *chunk )
{
	MDXArrays *args = (MDXArrays *)MDX_CHUNK_ARGS( chunk ) ;
	int format = args->format ;
	int stride = args->stride ;
	int n_verts = args->n_verts ;

	arrays->SetFormat( format & MDX_VF_MASK ) ;
	arrays->SetVertexCount( n_verts ) ;

	void *data = MDX_CHUNK_DATA( chunk ) ;
	for ( int j = 0 ; j < n_verts ; j ++ ) {
		MdxVertex &v = arrays->GetVertex( j ) ;
		if ( !ImportVertexPSM( v, format, data ) ) return false ;
		data = (char *)data + stride ;
	}

	#if ( PRESERVE_DATATYPE )
	if ( MDX_VF_POSITION_COUNT( format ) ) m_vertex_type = MDX_VT_POSITION_TYPE( format ) ;
	if ( MDX_VF_NORMAL_COUNT( format ) )   m_normal_type = MDX_VT_NORMAL_TYPE( format ) ;
	if ( MDX_VF_COLOR_COUNT( format ) )    m_vcolor_type = MDX_VT_COLOR_TYPE( format ) ;
	if ( MDX_VF_TEXCOORD_COUNT( format ) ) m_tcoord_type = MDX_VT_TEXCOORD_TYPE( format ) ;
	if ( MDX_VF_WEIGHT_COUNT( format ) )   m_weight_type = MDX_VT_WEIGHT_TYPE( format ) ;
	#endif // PRESERVE_DATATYPE

	if ( !ImportChildren( arrays, chunk ) ) return false ;
	if ( !CheckDirective( arrays ) ) return false ;
	return true ;
}

bool ImportMDX::ImportVertexPSM( MdxVertex &v, int format, void *data )
{
	static int Aligns[] = { 3, 1, 0, 0 } ;
	int align = 0 ;
	vec4 d ;

	if ( MDX_VF_POSITION_COUNT( format ) ) {
		int ptype = MDX_VT_POSITION_TYPE( format ) ;
		align |= Aligns[ ptype ] ;
		switch ( ptype ) {
		    case MDX_VT_FLOAT : {
			float *dp = (float *)( ( (int)data + 3 ) & ~3 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ] ) ;
			data = dp + 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			float16 *dp = (float16 *)( ( (int)data + 1 ) & ~1 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ] ) ;
			data = dp + 3 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			fixed8n *dp = (fixed8n *)data ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ] ) * m_vertex_scale + m_vertex_offset ;
			data = dp + 3 ;
			break ;
		    }
		}
		v.SetPosition( d ) ;
	}
	if ( MDX_VF_NORMAL_COUNT( format ) ) {
		int ntype = MDX_VT_NORMAL_TYPE( format ) ;
		align |= Aligns[ ntype ] ;
		switch ( ntype ) {
		    case MDX_VT_FLOAT : {
			float *dp = (float *)( ( (int)data + 3 ) & ~3 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ] ) ;
			data = dp + 3 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			float16 *dp = (float16 *)( ( (int)data + 1 ) & ~1 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ] ) ;
			data = dp + 3 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			fixed8n *dp = (fixed8n *)data ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ] ) ;
			data = dp + 3 ;
			break ;
		    }
		}
		v.SetNormal( d ) ;
	}
	if ( MDX_VF_COLOR_COUNT( format ) ) {
		int ctype = MDX_VT_COLOR_TYPE( format ) ;
		align |= Aligns[ ctype ] ;
		switch ( ctype ) {
		    case MDX_VT_FLOAT : {
			float *dp = (float *)( ( (int)data + 3 ) & ~3 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ], dp[ 3 ] ) ;
			data = dp + 4 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			float16 *dp = (float16 *)( ( (int)data + 1 ) & ~1 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ], dp[ 3 ] ) ;
			data = dp + 4 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			ufixed8n *dp = (ufixed8n *)data ;
			d = vec4( dp[ 0 ], dp[ 1 ], dp[ 2 ], dp[ 3 ] ) ;
			data = dp + 4 ;
			break ;
		    }
		}
		v.SetColor( d ) ;
	}
	if ( MDX_VF_TEXCOORD_COUNT( format ) ) {
		int ttype = MDX_VT_TEXCOORD_TYPE( format ) ;
		align |= Aligns[ ttype ] ;
		switch ( ttype ) {
		    case MDX_VT_FLOAT : {
			float *dp = (float *)( ( (int)data + 3 ) & ~3 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ] ) ;
			data = dp + 2 ;
			break ;
		    }
		    case MDX_VT_HALF : {
			float16 *dp = (float16 *)( ( (int)data + 1 ) & ~1 ) ;
			d = vec4( dp[ 0 ], dp[ 1 ] ) ;
			data = dp + 2 ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			ufixed8n *dp = (ufixed8n *)data ;
			d = vec4( dp[ 0 ], dp[ 1 ] ) * m_tcoord_scale + m_tcoord_offset ;
			data = dp + 2 ;
			break ;
		    }
		}
		v.SetTexCoord( d ) ;
	}
	if ( MDX_VF_WEIGHT_COUNT( format ) ) {
		int count = MDX_VF_WEIGHT_COUNT( format ) ;
		int wtype = MDX_VT_WEIGHT_TYPE( format ) ;
		align |= Aligns[ wtype ] ;
		switch ( wtype ) {
		    case MDX_VT_FLOAT : {
			float *dp = (float *)( ( (int)data + 3 ) & ~3 ) ;
			for ( int i = 0 ; i < count ; i ++ ) {
				v.SetWeight( i, *( dp ++ ) ) ;
			}
			data = dp ;
			break ;
		    }
		    case MDX_VT_HALF : {
			float16 *dp = (float16 *)( ( (int)data + 1 ) & ~1 ) ;
			for ( int i = 0 ; i < count ; i ++ ) {
				v.SetWeight( i, *( dp ++ ) ) ;
			}
			data = dp ;
			break ;
		    }
		    case MDX_VT_BYTE :
		    case MDX_VT_UBYTE : {
			ufixed8n *dp = (ufixed8n *)data ;
			for ( int i = 0 ; i < count ; i ++ ) {
				v.SetWeight( i, *( dp ++ ) ) ;
			}
			data = dp ;
			break ;
		    }
		}
	}
	int count = MDX_VF_INDICES_COUNT( format ) ;
	unsigned char *dp = (unsigned char *)data ;
	for ( int i = 0 ; i < count ; i ++ ) {
		v.SetBlendIndex( i, *( dp ++ ) ) ;
	}
	data = dp ;

	data = (void *)( ( (int)data + align ) & ~align ) ;
	return true ;
}

bool ImportMDX::ImportFCurvePSM( MdxFCurve *fcurve, MDXChunk *chunk )
{
	MDXFCurve *args = (MDXFCurve *)MDX_CHUNK_ARGS( chunk ) ;
	if ( args->format & MDX_FCURVE_FLOAT16 ) {
		int format = args->format & ~MDX_FCURVE_FLOAT16 ;
		fcurve->SetFormat( format ) ;
		fcurve->SetExtrap( args->extrap ) ;
		fcurve->SetDimCount( args->n_dims ) ;
		fcurve->SetKeyFrameCount( args->n_keys ) ;

		float16 *data = (float16 *)MDX_CHUNK_DATA( chunk ) ;
		for ( int i = 0 ; i < args->n_keys ; i ++ ) {
			MdxKeyFrame &key = fcurve->GetKeyFrame( i ) ;
			key.SetFrame( *( data ++ ) ) ;
			for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetValue( j, *( data ++ ) ) ;
			if ( format == MDX_FCURVE_HERMITE ) {
				for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetInDY( j, *( data ++ ) ) ;
				for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetOutDY( j, *( data ++ ) ) ;
			}
			if ( format == MDX_FCURVE_CUBIC ) {
				for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetInDY( j, *( data ++ ) ) ;
				for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetOutDY( j, *( data ++ ) ) ;
				for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetInDX( j, *( data ++ ) ) ;
				for ( int j = 0 ; j < args->n_dims ; j ++ ) key.SetOutDX( j, *( data ++ ) ) ;
			}
		}
		#if ( PRESERVE_DATATYPE )
		m_keyframe_type = MDX_VT_HALF ;
		#endif // PRESERVE_DATATYPE
		return true ;
	}
	#if ( PRESERVE_DATATYPE )
	if ( m_keyframe_type < 0 ) m_keyframe_type = MDX_VT_FLOAT ;
	#endif // PRESERVE_DATATYPE
	return ImportGeneric( fcurve, chunk ) ;
}

#endif // SUPPORT_PSMSTYLE


} // namespace mdx
