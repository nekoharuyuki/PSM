/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.IO ;
using System.Text ;
using System.Collections.Generic ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace Sce.PlayStation.HighLevel.Model {

	//------------------------------------------------------------
	//  MdxLoader
	//------------------------------------------------------------

	public class MdxLoader {

		public MdxLoader() {
			encoding = Encoding.ASCII ;
		}
		public void Load( BasicModel model, string fileName, int index ) {
			byte[] fileImage ;
			using ( var reader = new BinaryReader( File.OpenRead( fileName ) ) ) {
				fileImage = reader.ReadBytes( (int)reader.BaseStream.Length ) ;
			}
			Load( model, fileImage, index ) ;
		}
		public void Load( BasicModel model, byte[] fileImage, int index ) {
			this.model = model ;
			this.fileImage = fileImage ;
			ReadHeader() ;
			LoadFile( ReadChunk( 16 ), index ) ;
		}

		//  Loading Functions

		void LoadFile( Chunk chunk, int index ) {
			if ( chunk.Type != ChunkType.File ) index = -1 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				if ( index -- == 0 ) {
					LoadModel( child ) ;
					return ;
				}
			}
			throw new FileLoadException( "Index is out of range" ) ;
		}
		void LoadModel( Chunk chunk ) {
			model.Name = chunk.Name ;
			int nBones = CountChild( chunk, ChunkType.Bone ) ;
			int nParts = CountChild( chunk, ChunkType.Part ) ;
			int nMaterials = CountChild( chunk, ChunkType.Material ) ;
			int nTextures = CountChild( chunk, ChunkType.Texture ) ;
			int nMotions = CountChild( chunk, ChunkType.Motion ) ;
			model.Bones = new BasicBone[ nBones ] ;
			model.Parts = new BasicPart[ nParts ] ;
			model.Materials = new BasicMaterial[ nMaterials ] ;
			model.Textures = new BasicTexture[ nTextures ] ;
			model.Motions = new BasicMotion[ nMotions ] ;
			model.Programs = new BasicProgram[ 0 ] ;
			for ( int i = 0 ; i < nBones ; i ++ ) model.Bones[ i ] = new BasicBone() ;
			for ( int i = 0 ; i < nParts ; i ++ ) model.Parts[ i ] = new BasicPart() ;
			for ( int i = 0 ; i < nMaterials ; i ++ ) model.Materials[ i ] = new BasicMaterial() ;
			for ( int i = 0 ; i < nTextures ; i ++ ) model.Textures[ i ] = new BasicTexture() ;
			for ( int i = 0 ; i < nMotions ; i ++ ) model.Motions[ i ] = new BasicMotion() ;
			int iBone = 0, iPart = 0, iMaterial = 0, iTexture = 0, iMotion = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.Bone :
						LoadBone( child, model.Bones[ iBone ++ ] ) ;
						break ;
					case ChunkType.Part :
						LoadPart( child, model.Parts[ iPart ++ ] ) ;
						break ;
					case ChunkType.Material :
						LoadMaterial( child, model.Materials[ iMaterial ++ ] ) ;
						break ;
					case ChunkType.Texture :
						LoadTexture( child, model.Textures[ iTexture ++ ] ) ;
						break ;
					case ChunkType.Motion :
						LoadMotion( child, model.Motions[ iMotion ++ ] ) ;
						break ;
					case ChunkType.BoundingSphere :
						model.BoundingSphere = ReadVector4( args ) ;
						break ;
				}
			}
			if ( nMotions > 0 ) model.SetCurrentMotion( 0, 0.0f ) ;
		}
		void LoadBone( Chunk chunk, BasicBone bone ) {
			bone.Name = chunk.Name ;
			int nBlends = CountChild( chunk, ChunkType.BlendBone ) ;
			int nParts = CountChild( chunk, ChunkType.DrawPart ) ;
			if ( nBlends > 0 ) {
				bone.BlendBones = new int[ nBlends ] ;
				bone.BlendOffsets = new Matrix4[ nBlends ] ;
			}
			if ( nParts > 0 ) bone.DrawParts = new int[ nParts ] ;
			int iBlend = 0, iPart = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.BoundingSphere :
						bone.BoundingSphere = ReadVector4( args ) ;
						break ;
					case ChunkType.ParentBone :
						bone.ParentBone = ReadIndex( args ) ;
						break ;
					case ChunkType.Visibility :
						bone.Visibility = ReadInt32( args ) ;
						break ;
					case ChunkType.Pivot :
						bone.Pivot = ReadVector3( args ) ;
						break ;
					case ChunkType.Translate :
						bone.Translation = ReadVector3( args ) ;
						break ;
					case ChunkType.Rotate :
						bone.Rotation = ReadQuaternion( args ) ;
						break ;
					case ChunkType.RotateXYZ :
						bone.Rotation = Quaternion.RotationXyz( ReadVector3( args ) * FMath.DegToRad ) ;
						break ;
					case ChunkType.RotateYZX :
						bone.Rotation = Quaternion.RotationYzx( ReadVector3( args ) * FMath.DegToRad ) ;
						break ;
					case ChunkType.RotateZXY :
						bone.Rotation = Quaternion.RotationZxy( ReadVector3( args ) * FMath.DegToRad ) ;
						break ;
					case ChunkType.RotateXZY :
						bone.Rotation = Quaternion.RotationXzy( ReadVector3( args ) * FMath.DegToRad ) ;
						break ;
					case ChunkType.RotateYXZ :
						bone.Rotation = Quaternion.RotationYxz( ReadVector3( args ) * FMath.DegToRad ) ;
						break ;
					case ChunkType.RotateZYX :
						bone.Rotation = Quaternion.RotationZyx( ReadVector3( args ) * FMath.DegToRad ) ;
						break ;
					case ChunkType.Scale :
						bone.Scaling = ReadVector3( args ) ;
						break ;
					case ChunkType.BlendBone :
						bone.BlendBones[ iBlend ] = ReadIndex( args + 0 ) ;
						bone.BlendOffsets[ iBlend ] = ReadMatrix4( args + 4 ) ;
						iBlend ++ ;
						break ;
					case ChunkType.DrawPart :
						bone.DrawParts[ iPart ++ ] = ReadIndex( args ) ;
						break ;
				}
			}
			bone.AnimationBlendingData = new BasicAnimationBlendingData( bone ) ;
		}
		void LoadPart( Chunk chunk, BasicPart part ) {
			part.Name = chunk.Name ;
			int nMeshes = CountChild( chunk, ChunkType.Mesh ) ;
			int nArrays = CountChild( chunk, ChunkType.Arrays ) ;
			part.Meshes = new BasicMesh[ nMeshes ] ;
			part.Arrays = new BasicArrays[ nArrays ] ;
			var indexLists = new List<ushort>[ nArrays ] ;
			for ( int i = 0 ; i < nMeshes ; i ++ ) part.Meshes[ i ] = new BasicMesh() ;
			for ( int i = 0 ; i < nArrays ; i ++ ) part.Arrays[ i ] = new BasicArrays() ;
			for ( int i = 0 ; i < nArrays ; i ++ ) indexLists[ i ] = new List<ushort>() ;
			int iMesh = 0, iArrays = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				switch ( child.Type ) {
					case ChunkType.Mesh :
						LoadMesh( child, chunk, part.Meshes[ iMesh ++ ], indexLists ) ;
						break ;
				}
			}
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				switch ( child.Type ) {
					case ChunkType.Arrays :
						LoadArrays( child, chunk, part.Arrays[ iArrays ], indexLists[ iArrays ] ) ;
						iArrays ++ ;
						break ;
				}
			}
		}
		void LoadMesh( Chunk chunk, Chunk parent, BasicMesh mesh, List<ushort>[] indexLists ) {
			int pSetArrays = FindChild( chunk, ChunkType.SetArrays, 0 ) ;
			if ( pSetArrays == 0 ) return ;
			mesh.Arrays = ReadIndex( ReadChunk( pSetArrays ).Args ) ;
			var indexList = indexLists[ mesh.Arrays ] ;

			var prims = new List<Primitive>() ;
			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.SetMaterial :
						mesh.Material = ReadIndex( args ) ;
						break ;
					case ChunkType.BlendIndices :
						int nIndices = ReadInt32( args ) ;
						mesh.BlendIndices = new int[ nIndices ] ;
						for ( int i = 0 ; i < nIndices ; i ++ ) {
							mesh.BlendIndices[ i ] = ReadIndex( args + 4 + 4 * i ) ;
						}
						break ;
					case ChunkType.DrawArrays :
						int mode = ReadInt32( args + 0 ) ;
						int nVerts = ReadInt32( args + 4 ) ;
						int nPrims = ReadInt32( args + 8 ) ;
						int first = indexList.Count ;
						int total = nVerts * nPrims ;

						var prim = new Primitive( GetDrawMode( mode ), first, nVerts, 0 ) ;
						for ( int i = 0 ; i < nPrims ; i ++ ) {
							prims.Add( prim ) ;
							prim.First += (ushort)nVerts ;
						}
						for ( int i = 0 ; i < total ; i ++ ) {
							indexList.Add( ReadUInt16( args + 12 + i * 2 ) ) ;
						}
						break ;
				}
			}
			mesh.Primitives = prims.ToArray() ;
		}
		void LoadArrays( Chunk chunk, Chunk parent, BasicArrays arrays, List<ushort> indexList ) {
			int args = chunk.Args ;
			int data = chunk.Data ;
			int format = ReadInt32( args + 0 ) ;
			int stride = ReadInt32( args + 4 ) ;
			int count = ReadInt32( args + 8 ) ;
			var formats = GetVertexFormats( format ) ;
			var offsets = GetVertexOffsets( format ) ;

			arrays.VertexBuffer = new VertexBuffer( count, indexList.Count, formats ) ;
			for ( int i = 0 ; i < formats.Length ; i ++ ) {
				if ( formats[ i ] == VertexFormat.None ) continue ;
				arrays.VertexBuffer.SetVertices( i, fileImage, data + offsets[ i ], stride ) ;
			}
			if ( indexList.Count > 0 ) arrays.VertexBuffer.SetIndices( indexList.ToArray() ) ;
		}
		void LoadMaterial( Chunk chunk, BasicMaterial material ) {
			material.Name = chunk.Name ;
			int nLayers = CountChild( chunk, ChunkType.Layer ) ;
			material.Layers = new BasicLayer[ nLayers ] ;
			for ( int i = 0 ; i < nLayers ; i ++ ) material.Layers[ i ] = new BasicLayer() ;
			int iLayer = 0 ;
			bool hasAmbient = false ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.Layer :
						LoadLayer( child, chunk, material.Layers[ iLayer ++ ] ) ;
						break ;
					case ChunkType.Diffuse :
						material.Diffuse = ReadVector3( args ) ;
						if ( !hasAmbient ) material.Ambient = material.Diffuse ;
						break ;
					case ChunkType.Ambient :
						material.Ambient = ReadVector3( args ) ;
						hasAmbient = true ;
						break ;
					case ChunkType.Specular :
						material.Specular = ReadVector3( args ) ;
						break ;
					case ChunkType.Emission :
						material.Emission = ReadVector3( args ) ;
						break ;
					case ChunkType.Opacity :
						material.Opacity = ReadFloat( args ) ;
						break ;
					case ChunkType.Shininess :
						material.Shininess = ReadFloat( args ) ;
						break ;
					case ChunkType.FileName	:
						material.FileName = ReadString( args, args + child.ArgsSize ) ;
						break ;
				}
			}
			material.AnimationBlendingData = new BasicAnimationBlendingData( material ) ;
		}
		void LoadLayer( Chunk chunk, Chunk parent, BasicLayer layer ) {
			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.SetTexture :
						layer.Texture = ReadIndex( args ) ;
						break ;
				}
			}
		}
		void LoadTexture( Chunk chunk, BasicTexture texture ) {
			texture.Name = chunk.Name ;
			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.FileImage :
						int size = ReadInt32( args ) ;
						var image = new byte[ size ] ;
						Array.Copy( fileImage, args + 4, image, 0, size ) ;
						texture.Texture = new Texture2D( image, true ) ;
						texture.Texture.SetWrap( TextureWrapMode.Repeat ) ;
						break ;
					case ChunkType.FileName	:
						texture.FileName = ReadString( args, args + child.ArgsSize ) ;
						break ;
					case ChunkType.UVTranslate :
						texture.UVTranslation = ReadVector2( args ) ;
						break ;
					case ChunkType.UVScale :
						texture.UVScaling = ReadVector2( args ) ;
						break ;
				}
			}
			texture.AnimationBlendingData = new BasicAnimationBlendingData( texture ) ;
		}
		void LoadMotion( Chunk chunk, BasicMotion motion ) {
			motion.Name = chunk.Name ;
			int nFCurves = CountChild( chunk, ChunkType.Animate ) ;
			motion.FCurves = new BasicFCurve[ nFCurves ] ;
			for ( int i = 0 ; i < nFCurves ; i ++ ) motion.FCurves[ i ] = new BasicFCurve() ;
			int iFCurve = 0 ;

			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				child = ReadChunk( pos ) ;
				int args = child.Args ;
				switch ( child.Type ) {
					case ChunkType.Animate :
						LoadFCurve( child, chunk, motion.FCurves[ iFCurve ++ ] ) ;
						break ;
					case ChunkType.FrameRate :
						motion.FrameRate = ReadFloat( args ) ;
						break ;
					case ChunkType.FrameLoop :
						motion.FrameStart = ReadFloat( args + 0 ) ;
						motion.FrameEnd = ReadFloat( args + 4 ) ;
						break ;
					case ChunkType.FrameRepeat :
						motion.FrameRepeat = (BasicMotionRepeatMode)ReadInt32( args ) ;
						break ;
				}
			}
		}
		void LoadFCurve( Chunk chunk, Chunk parent, BasicFCurve fcurve ) {
			int args = chunk.Args ;
			int target = ReadInt32( args + 0 ) ;
			int channel = ReadInt32( args + 4 ) ;
			//int index = ReadInt32( args + 8 ) ;
			int iFCurve = ReadIndex( args + 12 ) ;
			int pFCurve = FindChild( parent, ChunkType.FCurve, iFCurve ) ;
			Chunk chunk2 = ReadChunk( pFCurve ) ;

			int args2 = chunk2.Args ;
			int interp = ReadInt32( args2 + 0 ) ;
			//int extrap = ReadInt32( args2 + 4 ) ;
			int nDims = ReadInt32( args2 + 8 ) ;
			int nKeys = ReadInt32( args2 + 12 ) ;
			int data = chunk2.Data ;
			int nData = chunk2.DataSize / 4 ;
			fcurve.KeyFrames = new float[ nData ] ;
			for ( int i = 0 ; i < nData ; i ++ ) {
				fcurve.KeyFrames[ i ] = ReadFloat( data + i * 4 ) ;
			}
			fcurve.DimCount = nDims ;
			fcurve.KeyCount = nKeys ;
			fcurve.InterpType = GetFCurveInterpType( interp ) ;
			fcurve.TargetType = GetFCurveTargetType( target ) ;
			fcurve.ChannelType = GetFCurveChannelType( channel ) ;
			fcurve.ChannelOption = GetFCurveChannelOption( channel ) ;
			fcurve.TargetIndex = GetReferenceIndex( target ) ;
		}

		//  Subroutines ( Enum )

		VertexFormat[] GetVertexFormats( int format ) {
			VertexFormat[] types = { VertexFormat.Float, VertexFormat.Half, VertexFormat.ByteN, VertexFormat.UByteN } ;
			int pCount = ( format >> 0 ) & 1 ;		int pType = ( format >> 20 ) & 3 ;
			int nCount = ( format >> 1 ) & 3 ;		int nType = ( format >> 22 ) & 3 ;
			int cCount = ( format >> 3 ) & 3 ;		int cType = ( format >> 24 ) & 3 ;
			int tCount = ( format >> 5 ) & 7 ;		int tType = ( format >> 26 ) & 3 ;
			int wCount = ( format >> 8 ) & 255 ;	int wType = ( format >> 28 ) & 3 ;
			bool hasIndices = ( ( ( format >> 16 ) & 1 ) != 0 ) ;
			var formats = new VertexFormat[ 6 ] ;
			if ( pCount > 0 ) formats[ 0 ] = (VertexFormat)( (int)types[ pType ] + 2 ) ;
			if ( nCount > 0 ) formats[ 1 ] = (VertexFormat)( (int)types[ nType ] + 2 ) ;
			if ( cCount > 0 ) formats[ 2 ] = (VertexFormat)( (int)types[ cType ] + 3 ) ;
			if ( tCount > 0 ) formats[ 3 ] = (VertexFormat)( (int)types[ tType ] + 1 ) ;
			if ( wCount > 0 ) formats[ 4 ] = (VertexFormat)( (int)types[ wType ] + wCount - 1 ) ;
			if ( hasIndices ) formats[ 5 ] = (VertexFormat)( (int)VertexFormat.UByte + wCount - 1 ) ;
			return formats ;
		}
		int[] GetVertexOffsets( int format ) {
			int[] masks = { 3, 1, 0, 0 } ;
			int pCount = ( format >> 0 ) & 1 ;		int pType = ( format >> 20 ) & 3 ;
			int nCount = ( format >> 1 ) & 3 ;		int nType = ( format >> 22 ) & 3 ;
			int cCount = ( format >> 3 ) & 3 ;		int cType = ( format >> 24 ) & 3 ;
			int tCount = ( format >> 5 ) & 7 ;		int tType = ( format >> 26 ) & 3 ;
			int wCount = ( format >> 8 ) & 255 ;	int wType = ( format >> 28 ) & 3 ;
			bool hasIndices = ( ( ( format >> 16 ) & 1 ) != 0 ) ;
			var offsets = new int[ 6 ] ;
			int offset = 0 ;
			int mask = masks[ pType ] ;
			offsets[ 0 ] = ( offset + mask ) & ~mask ;
			if ( pCount > 0 ) offset = offsets[ 0 ] + ( mask + 1 ) * 3 * pCount ;
			mask = masks[ nType ] ;
			offsets[ 1 ] = ( offset + mask ) & ~mask ;
			if ( nCount > 0 ) offset = offsets[ 1 ] + ( mask + 1 ) * 3 * nCount ;
			mask = masks[ cType ] ;
			offsets[ 2 ] = ( offset + mask ) & ~mask ;
			if ( cCount > 0 ) offset = offsets[ 2 ] + ( mask + 1 ) * 4 * cCount ;
			mask = masks[ tType ] ;
			offsets[ 3 ] = ( offset + mask ) & ~mask ;
			if ( tCount > 0 ) offset = offsets[ 3 ] + ( mask + 1 ) * 2 * tCount ;
			mask = masks[ wType ] ;
			offsets[ 4 ] = ( offset + mask ) & ~mask ;
			if ( wCount > 0 ) offset = offsets[ 4 ] + ( mask + 1 ) * wCount ;
			offsets[ 5 ] = offset ;
			if ( hasIndices ) offset = offsets[ 5 ] + wCount ;
			return offsets ;
		}
		int GetVertexStride( int format, int vType ) {
			return ( vType >> 16 ) & 0xffff ;
		}
		DrawMode GetDrawMode( int mode ) {
			DrawMode[] modes = {
				DrawMode.Points, DrawMode.Lines, DrawMode.LineStrip,
				DrawMode.Triangles, DrawMode.TriangleStrip, DrawMode.TriangleFan
			} ;
			return modes[ mode & 0x0f ] ;
		}
		BasicFCurveInterpType GetFCurveInterpType( int interp ) {
			return (BasicFCurveInterpType)( interp & 0x0f ) ;
		}
		BasicFCurveTargetType GetFCurveTargetType( int target ) {
			ChunkType type = GetReferenceType( target ) ;
			if ( type == ChunkType.Bone ) return BasicFCurveTargetType.Bone ;
			if ( type == ChunkType.Material ) return BasicFCurveTargetType.Material ;
			if ( type == ChunkType.Texture ) return BasicFCurveTargetType.Texture ;
			return BasicFCurveTargetType.None ;
		}
		BasicFCurveChannelType GetFCurveChannelType( int channel ) {
			ChunkType type = (ChunkType)channel ;
			if ( type == ChunkType.Visibility ) return BasicFCurveChannelType.Visibility ;
			if ( type == ChunkType.Translate ) return BasicFCurveChannelType.Translation ;
			if ( type >= ChunkType.Rotate && type <= ChunkType.RotateZYX ) return BasicFCurveChannelType.Rotation ;
			if ( type == ChunkType.Scale ) return BasicFCurveChannelType.Scaling ;
			if ( type == ChunkType.Diffuse ) return BasicFCurveChannelType.Diffuse ;
			if ( type == ChunkType.Ambient ) return BasicFCurveChannelType.Ambient ;
			if ( type == ChunkType.Specular ) return BasicFCurveChannelType.Specular ;
			if ( type == ChunkType.Emission ) return BasicFCurveChannelType.Emission ;
			if ( type == ChunkType.Opacity ) return BasicFCurveChannelType.Opacity ;
			if ( type == ChunkType.Shininess ) return BasicFCurveChannelType.Shininess ;
			if ( type == ChunkType.UVTranslate ) return BasicFCurveChannelType.UVTranslation ;
			if ( type == ChunkType.UVScale ) return BasicFCurveChannelType.UVScaling ;
			return BasicFCurveChannelType.None ;
		}
		int GetFCurveChannelOption( int channel ) {
			ChunkType type = (ChunkType)channel ;
			if ( type >= ChunkType.Rotate && type <= ChunkType.RotateZYX ) return type - ChunkType.Rotate ;
			return 0 ;
		}
		ChunkType GetReferenceType( int reference ) {
			return (ChunkType)( ( reference >> 16 ) & 0x7fff ) ;
		}
		int GetReferenceIndex( int reference ) {
			return ( reference & 0x0fff ) ;
		}

		//  Subroutines ( Format Reading )

		void ReadHeader() {
			header.Signature = BitConverter.ToUInt32( fileImage, 0 ) ;
			header.Version = BitConverter.ToUInt32( fileImage, 4 ) ;
			header.Style = BitConverter.ToUInt32( fileImage, 8 ) ;
			header.Option = BitConverter.ToUInt32( fileImage, 12 ) ;
			if ( header.Signature != 0x2e4d4458 ) throw new FileLoadException( "Unsupported format signature" ) ;
			if ( header.Version != 0x312e3030 ) throw new FileLoadException( "Unsupported format version" ) ;
			if ( header.Style != 0x0050534d ) throw new FileLoadException( "Unsupported format style" ) ;
		}
		Chunk ReadChunk( int pos ) {
			Chunk chunk ;
			ReadChunk( pos, out chunk ) ;
			return chunk ;
		}
		void ReadChunk( int pos, out Chunk chunk ) {
			chunk.Pos = pos ;
			chunk.Name = null ;
			int type = BitConverter.ToUInt16( fileImage, pos + 0 ) ;
			int size = BitConverter.ToUInt16( fileImage, pos + 2 ) ;
			if ( ( type & 0x8000 ) != 0 ) {
				chunk.NameEnd = pos + 4 ;
				chunk.ArgsEnd = pos + size ;
				chunk.DataEnd = chunk.ArgsEnd ;
				chunk.ChildEnd = chunk.DataEnd ;
			} else {
				chunk.NameEnd = pos + size ;
				chunk.ArgsEnd = pos + BitConverter.ToInt32( fileImage, pos + 4 ) ;
				chunk.DataEnd = pos + BitConverter.ToInt32( fileImage, pos + 8 ) ;
				chunk.ChildEnd = pos + BitConverter.ToInt32( fileImage, pos + 12 ) ;
				chunk.Name = ReadString( pos + 16, pos + size ) ;
			}
			chunk.Type = (ChunkType)( type & ~0x8000 ) ;
		}
		int CountChild( Chunk chunk, ChunkType type ) {
			Chunk child ;
			int count = 0 ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				ReadChunk( pos, out child ) ;
				if ( child.Type == type ) count ++ ;
			}
			return count ;
		}
		int FindChild( Chunk chunk, ChunkType type, int index ) {
			Chunk child ;
			for ( int pos = chunk.Child ; pos < chunk.Next ; pos = child.Next ) {
				ReadChunk( pos, out child ) ;
				if ( child.Type == type && index -- == 0 ) return pos ;
			}
			return 0 ;
		}

		//  Subroutines ( Data Reading )

		string ReadString( int pos, int end ) {
			int term = pos ;
			while ( term < end && fileImage[ term ] != 0 ) term ++ ;
			return ( term == pos ) ? "" : encoding.GetString( fileImage, pos, term - pos ) ;
		}
		int ReadIndex( int pos ) { return GetReferenceIndex( ReadInt32( pos ) ) ; }
		int ReadInt32( int pos ) { return BitConverter.ToInt32( fileImage, pos ) ; }
		uint ReadUInt32( int pos ) { return BitConverter.ToUInt32( fileImage, pos ) ; }
		short ReadInt16( int pos ) { return BitConverter.ToInt16( fileImage, pos ) ; }
		ushort ReadUInt16( int pos ) { return BitConverter.ToUInt16( fileImage, pos ) ; }
		sbyte ReadInt8( int pos ) { return (sbyte)fileImage[ pos ] ; }
		byte ReadUInt8( int pos ) { return fileImage[ pos ] ; }
		float ReadFloat( int pos ) { return BitConverter.ToSingle( fileImage, pos ) ; }
		Matrix4 ReadMatrix4( int pos ) {
			Vector4 x = ReadVector4( pos + 0 ) ;
			Vector4 y = ReadVector4( pos + 16 ) ;
			Vector4 z = ReadVector4( pos + 32 ) ;
			Vector4 w = ReadVector4( pos + 48 ) ;
			return new Matrix4( x, y, z, w ) ;
		}
		Vector4 ReadVector4( int pos ) {
			float x = BitConverter.ToSingle( fileImage, pos + 0 ) ;
			float y = BitConverter.ToSingle( fileImage, pos + 4 ) ;
			float z = BitConverter.ToSingle( fileImage, pos + 8 ) ;
			float w = BitConverter.ToSingle( fileImage, pos + 12 ) ;
			return new Vector4( x, y, z, w ) ;
		}
		Vector3 ReadVector3( int pos ) {
			float x = BitConverter.ToSingle( fileImage, pos + 0 ) ;
			float y = BitConverter.ToSingle( fileImage, pos + 4 ) ;
			float z = BitConverter.ToSingle( fileImage, pos + 8 ) ;
			return new Vector3( x, y, z ) ;
		}
		Vector2 ReadVector2( int pos ) {
			float x = BitConverter.ToSingle( fileImage, pos + 0 ) ;
			float y = BitConverter.ToSingle( fileImage, pos + 4 ) ;
			return new Vector2( x, y ) ;
		}
		Quaternion ReadQuaternion( int pos ) {
			Vector4 v = ReadVector4( pos ) ;
			return new Quaternion( v.X, v.Y, v.Z, v.W ) ;
		}

		//  Private Data

		Encoding encoding ;
		BasicModel model ;
		byte[] fileImage ;
		Header header ;

		//  Private Types

		struct Header {
			public uint Signature ;
			public uint Version ;
			public uint Style ;
			public uint Option ;
		} ;
		struct Chunk {
			public int Pos ;
			public ChunkType Type ;
			public string Name ;
			public int NameEnd ;
			public int ArgsEnd ;
			public int DataEnd ;
			public int ChildEnd ;
			public int Args { get { return ( NameEnd + 3 ) & ~3 ; } }
			public int Data { get { return ( ArgsEnd + 3 ) & ~3 ; } }
			public int Child { get { return ( DataEnd + 3 ) & ~3 ; } }
			public int Next { get { return ( ChildEnd + 3 ) & ~3 ; } }
			public int ArgsSize { get { return ArgsEnd - Args ; } }
			public int DataSize { get { return DataEnd - Data ; } }
			public int ChildSize { get { return ChildEnd - Child ; } }
			public int TotalSize { get { return Next - Pos ; } }
		} ;
		enum ChunkType {
			Block			= 0x0001,
			File			= 0x0002,
			Model			= 0x0010,
			Bone			= 0x0011,
			Part			= 0x0012,
			Mesh			= 0x0013,
			Arrays			= 0x0014,
			Material		= 0x0016,
			Layer			= 0x0017,
			Texture			= 0x0018,
			Motion			= 0x001b,
			FCurve			= 0x001c,
			BlindBlock		= 0x001f,

			Command			= 0x0040,
			DefineEnum		= 0x0041,
			DefineBlock		= 0x0042,
			DefineCommand	= 0x0043,

			FileName		= 0x0080,
			FileImage		= 0x0081,
			BoundingBox		= 0x0082,
			BoundingSphere	= 0x0083,

			ParentBone		= 0x0440,
			Visibility		= 0x0441,
			Pivot			= 0x0442,
			Translate		= 0x0443,
			Rotate			= 0x0444,
			RotateXYZ		= 0x0445,
			RotateYZX		= 0x0446,
			RotateZXY		= 0x0447,
			RotateXZY		= 0x0448,
			RotateYXZ		= 0x0449,
			RotateZYX		= 0x044a,
			Scale			= 0x044b,
			BlendBone		= 0x0460,
			MorphIndex		= 0x0461,
			MorphWeights	= 0x0462,
			DrawPart		= 0x047f,

			SetMaterial		= 0x04c0,
			SetArrays		= 0x04c1,
			BlendIndices	= 0x04c2,
			DrawArrays		= 0x04e0,
			DrawBSpline		= 0x04e1,

			SetProgram		= 0x0580,
			Diffuse			= 0x0581,
			Ambient			= 0x0582,
			Specular		= 0x0583,
			Emission		= 0x0584,
			Reflection		= 0x0585,
			Refraction		= 0x0586,
			Bump			= 0x0587,
			Opacity			= 0x0588,
			Shininess		= 0x0589,

			SetTexture		= 0x05c0,
			MapType			= 0x05c1,
			MapCoord		= 0x05c2,
			MapFactor		= 0x05c3,

			TexType			= 0x0600,
			TexFilter		= 0x0601,
			TexWrap			= 0x0602,
			TexCrop			= 0x0603,
			UVPivot			= 0x0620,
			UVTranslate		= 0x0621,
			UVRotate		= 0x0622,
			UVScale			= 0x0623,

			FrameLoop		= 0x06c0,
			FrameRate		= 0x06c1,
			FrameRepeat		= 0x06c2,
			Animate			= 0x06e0,

			BlindData		= 0x07c0
		} ;
	}
	
} // namespace
	
