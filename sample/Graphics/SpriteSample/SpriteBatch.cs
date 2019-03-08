/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

#define RESIZE_VERTEX_BUFFER
#define ENABLE_SIN_TABLE

using System ;
using System.Threading ;
using System.Diagnostics ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.Core.Input ;

namespace Sample
{

//----------------------------------------------------------------
//  Sprite Batch
//----------------------------------------------------------------

public class SpriteBatch : IDisposable
{
	public SpriteBatch( GraphicsContext graphics, int maxSpriteCount )
	{
		int maxVertexCount = maxSpriteCount * 4 ;
		int maxIndexCount = maxSpriteCount * 6 ;

		graphicsContext = graphics ;
		#if !RESIZE_VERTEX_BUFFER
		vertexBuffer = new VertexBuffer( maxVertexCount, maxIndexCount, vertexFormats ) ;
		spriteCapacity = maxSpriteCount ;
		#endif // RESIZE_VERTEX_BUFFER
		vertexData = new Vertex[ maxVertexCount ] ;
		indexData = new ushort[ maxIndexCount ] ;

		spriteList = new Sprite[ maxSpriteCount ] ;
		sortedList = new Sprite[ maxSpriteCount ] ;

		#if ENABLE_SIN_TABLE
		if ( sinTable == null ) {
			sinTable = new float[ 4096 ] ;
			for ( int i = 0 ; i < 4096 ; i ++ ) {
				sinTable[ i ] = FMath.Sin( i * ( FMath.PI / 2048.0f ) ) ;
			}
		}
		#endif // ENABLE_SIN_TABLE
	}
	public void Dispose()
	{
		Dispose( true ) ;
	}
	protected virtual void Dispose( bool disposing )
	{
		if ( disposing ) {
			if ( vertexBuffer != null ) vertexBuffer.Dispose() ;
			vertexBuffer = null ;
			vertexData = null ;
			indexData = null ;
			spriteList = null ;
			sortedList = null ;
		}
	}

	//  Functions

	public void Draw()
	{
		#if RESIZE_VERTEX_BUFFER
		ResizeBuffer() ;
		#endif // RESIZE_VERTEX_BUFFER
		UpdateVertexData() ;
		UpdateIndexData() ;
		DrawSprites() ;
	}
	public void AddSprite( Sprite sprite )
	{
		AddToSpriteList( sprite ) ;
		AddToSortedList( sprite ) ;
		sprite.batch = this ;
		vertexCount += 4 ;
		indexCount += 6 ;
	}
	public void RemoveSprite( Sprite sprite )
	{
		RemoveFromSpriteList( sprite ) ;
		RemoveFromSortedList( sprite ) ;
		sprite.batch = null ;
		vertexCount -= 4 ;
		indexCount -= 6 ;
	}

	//  Subroutines

	#if RESIZE_VERTEX_BUFFER
	internal void ResizeBuffer()
	{
		int oldCapacity = spriteCapacity ;
		if ( spriteCount > spriteCapacity ) {
			int threshold = (int)( spriteCapacity * bufferResizeFactor ) ;
			spriteCapacity = spriteCount + bufferResizeMargin ;
			if ( spriteCapacity < threshold ) spriteCapacity = threshold ;
			bufferReducePending = 0 ;
			bufferReduceTarget = 0 ;
		} else {
			if ( spriteCount > bufferReduceTarget ) bufferReduceTarget = spriteCount ;
			if ( ++ bufferReducePending >= bufferReduceWait ) {
				int threshold = (int)( spriteCapacity / bufferResizeFactor ) ;
				if ( bufferReduceTarget + bufferResizeMargin * 2 <= threshold ) {
					spriteCapacity = bufferReduceTarget + bufferResizeMargin ;
				}
				bufferReducePending = 0 ;
				bufferReduceTarget = 0 ;
			}
		}
		if ( spriteCapacity != oldCapacity ) {
			if ( vertexBuffer != null ) vertexBuffer.Dispose() ;
			vertexBuffer = new VertexBuffer( spriteCapacity * 4, spriteCapacity * 6, vertexFormats ) ;
			needUpdateVertexData = true ;
			needUpdateIndexData = true ;
		}
	}
	#endif // RESIZE_VERTEX_BUFFER
	internal void UpdateVertexData()
	{
		if ( !needUpdateVertexData ) return ;
		needUpdateVertexData = false ;
		vertexBuffer.SetVertices( vertexData, 0, 0, vertexCount ) ;
	}
	internal void UpdateIndexData()
	{
		if ( !needUpdateIndexData ) return ;
		needUpdateIndexData = false ;

		int indexID = 0 ;
		SpriteMaterial material = null ;
		Sprite draw = null ;
		drawList = null ;
		drawCount = 0 ;
		for ( int i = 0 ; i < sortedCount ; i ++ ) {
			var head = sortedList[ i ] ;
			var sprite = head ;
			if ( material != sprite.Material ) {
				material = sprite.Material ;
				if ( draw == null ) drawList = sprite ;
				if ( draw != null ) draw.drawNext = sprite ;
				sprite.drawNext = null ;
				draw = sprite ;
				drawCount ++ ;
			}
			do {
				if ( sprite.indexID != indexID ) {
					sprite.indexID = indexID ;
					indexData[ indexID + 0 ] = (ushort)( sprite.vertexID + 0 ) ;
					indexData[ indexID + 1 ] = (ushort)( sprite.vertexID + 1 ) ;
					indexData[ indexID + 2 ] = (ushort)( sprite.vertexID + 2 ) ;
					indexData[ indexID + 3 ] = (ushort)( sprite.vertexID + 2 ) ;
					indexData[ indexID + 4 ] = (ushort)( sprite.vertexID + 1 ) ;
					indexData[ indexID + 5 ] = (ushort)( sprite.vertexID + 3 ) ;
				}
				indexID += 6 ;
				sprite = sprite.sortNext ;
			} while ( sprite != head ) ;
		}
		vertexBuffer.SetIndices( indexData, 0, 0, indexCount ) ;
	}
	internal void DrawSprites()
	{
		FrameBuffer fbuffer = graphicsContext.GetFrameBuffer() ;
		Matrix4 projection = Matrix4.Ortho( 0, fbuffer.Width, fbuffer.Height, 0, -100.0f, 100.0f ) ;
		graphicsContext.Enable( EnableMode.Blend ) ;

		graphicsContext.SetVertexBuffer( 0, vertexBuffer ) ;
		for ( var sprite = drawList ; sprite != null ; sprite = sprite.drawNext ) {
			var material = sprite.Material ;
			graphicsContext.SetShaderProgram( material.ShaderProgram ) ;
			material.ShaderProgram.SetUniformValue( 0, ref projection ) ;
			graphicsContext.SetBlendFunc( material.BlendFunc ) ;
			graphicsContext.SetTexture( 0, material.Texture ) ;
			int next = ( sprite.drawNext == null ) ? indexCount : sprite.drawNext.indexID ;
			graphicsContext.DrawArrays( DrawMode.Triangles, sprite.indexID, next - sprite.indexID ) ;
		}
	}

	//  Subroutines

	internal void AddToSpriteList( Sprite sprite )
	{
		sprite.spriteID = spriteCount ;
		sprite.vertexID = vertexCount ;
		spriteList[ spriteCount ++ ] = sprite ;
		needUpdateVertexData = true ;
	}
	internal void RemoveFromSpriteList( Sprite sprite )
	{
		var tail = spriteList[ -- spriteCount ] ;
		if ( sprite != tail ) {
			spriteList[ sprite.spriteID ] = tail ;
			for ( int i = 0 ; i < 4 ; i ++ ) {
				vertexData[ sprite.vertexID + i ] = vertexData[ tail.vertexID + i ] ;
			}
			tail.spriteID = sprite.spriteID ;
			tail.vertexID = sprite.vertexID ;
		}
		needUpdateVertexData = true ;
	}
	internal void AddToSortedList( Sprite sprite )
	{
		int index = FindSortedList( sprite ) ;
		if ( index >= 0 && sortedList[ index ].sortKey == sprite.sortKey ) {
			var head = sortedList[ index ] ;
			var tail = head.sortPrev ;
			head.sortPrev = sprite ;
			tail.sortNext = sprite ;
			sprite.sortPrev = tail ;
			sprite.sortNext = head ;
		} else {
			int count = ( sortedCount ++ ) - ( ++ index ) ;
			if ( count > 0 ) Array.Copy( sortedList, index, sortedList, index + 1, count ) ;
			sortedList[ index ] = sprite ;
			sprite.sortPrev = sprite ;
			sprite.sortNext = sprite ;
		}
		sprite.indexID = -1 ;
		needUpdateIndexData = true ;
	}
	internal void RemoveFromSortedList( Sprite sprite )
	{
		int index = FindSortedList( sprite ) ;
		if ( sortedList[ index ] == sprite ) {
			sortedList[ index ] = sprite.sortNext ;
			if ( sprite.sortNext == sprite ) {
				int count = ( -- sortedCount ) - index ;
				if ( count > 0 ) Array.Copy( sortedList, index + 1, sortedList, index, count ) ;
			}
		}
		sprite.sortPrev.sortNext = sprite.sortNext ;
		sprite.sortNext.sortPrev = sprite.sortPrev ;
		needUpdateIndexData = true ;
	}
	internal int FindSortedList( Sprite sprite )
	{
		int lower = -1 ;
		int upper = sortedCount ;
		while ( lower + 1 < upper ) {
			int middle = ( lower + upper ) / 2 ;
			if ( sortedList[ middle ].sortKey > sprite.sortKey ) {
				upper = middle ;
			} else {
				lower = middle ;
			}
		}
		return lower ;
	}

	//  Params

	internal GraphicsContext graphicsContext ;
	internal VertexBuffer vertexBuffer ;
	internal Vertex[] vertexData ;
	internal ushort[] indexData ;
	internal int vertexCount ;
	internal int indexCount ;
	internal bool needUpdateVertexData ;
	internal bool needUpdateIndexData ;

	internal Sprite[] spriteList ;
	internal Sprite[] sortedList ;
	internal Sprite drawList ;
	internal int spriteCapacity ;
	internal int spriteCount ;
	internal int sortedCount ;
	internal int drawCount ;

	#if RESIZE_VERTEX_BUFFER
	const float bufferResizeFactor = 1.5f ;
	const int bufferResizeMargin = 256 ;
	const int bufferReduceWait = 30 ;
	static int bufferReducePending ;
	static int bufferReduceTarget ;
	#endif // RESIZE_VERTEX_BUFFER

	#if ENABLE_SIN_TABLE
	internal static float[] sinTable ;
	#endif // ENABLE_SIN_TABLE

	//  Vertex format

	internal static VertexFormat[] vertexFormats = {
		VertexFormat.Float2, 
		VertexFormat.Float2, 
		VertexFormat.UByte4N
	} ;
	internal struct Vertex {
		public Vector2 Position ;
		public Vector2 TexCoord ;
		public Rgba Color ;
	}
}

//----------------------------------------------------------------
//  Sprite Material
//----------------------------------------------------------------

public class SpriteMaterial
{
	public SpriteMaterial( Texture texture = null )
	{
		MaterialID = nextMaterialID ++ ;
		ShaderProgram = DefaultShaderProgram ;
		BlendFunc = DefaultBlendFunc ;
		Texture = texture ?? DefaultTexture ;
	}

	//  Material params

	public readonly uint MaterialID ;
	public ShaderProgram ShaderProgram ;
	public BlendFunc BlendFunc ;
	public Texture Texture ;

	//  Default values

	public static SpriteMaterial DefaultMaterial {
		get {
			if ( defaultMaterial == null ) {
				defaultMaterial = new SpriteMaterial() ;
			}
			return defaultMaterial ;
		}
	}
	public static ShaderProgram DefaultShaderProgram {
		get {
			if ( defaultShaderProgram == null ) {
				var filename = "/Application/shaders/Sprite.cgx" ;
				defaultShaderProgram = new ShaderProgram( filename ) ;
			}
			return defaultShaderProgram ;
		}
	}
	public static BlendFunc DefaultBlendFunc {
		get {
			return new BlendFunc( BlendFuncMode.Add,
				BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
		}
	}
	public static Texture DefaultTexture {
		get {
			if ( defaultTexture == null ) {
				var texture = new Texture2D( 1, 1, false, PixelFormat.Rgba ) ;
				Rgba[] pixels = { new Rgba( 255, 255, 255, 255 ) } ;
				texture.SetPixels( 0, pixels ) ;
				defaultTexture = texture ;
			}
			return defaultTexture ;
		}
	}

	static uint nextMaterialID ;
	static SpriteMaterial defaultMaterial ;
	static ShaderProgram defaultShaderProgram ;
	static Texture defaultTexture ;
}

//----------------------------------------------------------------
//  Sprite
//----------------------------------------------------------------

public class Sprite
{
	public Sprite( SpriteBatch batch, SpriteMaterial material = null, int zIndex = 0 )
	{
		this.batch = batch ;
		this.material = material ?? SpriteMaterial.DefaultMaterial ;
		this.zIndex = zIndex ;
		vertexID = -1 ;
		indexID = -1 ;
		UpdateSortKey( false ) ;
		batch.AddSprite( this ) ;

		Center = new Vector2( 0.5f ) ;
		UVSize = new Vector2( 1.0f ) ;
		Color = new Rgba( 255, 255, 255, 255 ) ;
	}

	//  Functions

	public void UpdateAll()
	{
		UpdatePosition() ;
		UpdateTexCoord() ;
		UpdateColor() ;
	}
	public void UpdatePosition()
	{
		var vertexData = batch.vertexData ;
		#if !ENABLE_SIN_TABLE
		float c = (float)Math.Cos( Direction ) ;
		float s = (float)Math.Sin( Direction ) ;
		#else // ENABLE_SIN_TABLE
		int phase = (int)( Direction * ( 2048.0f / FMath.PI ) ) ;
		float c = SpriteBatch.sinTable[ ( phase + 1024 ) & 4095 ] ;
		float s = SpriteBatch.sinTable[ phase & 4095 ] ;
		#endif // ENABLE_SIN_TABLE
		float ux = c * Size.X ;	float vx = -s * Size.Y ;
		float uy = s * Size.X ;	float vy = c * Size.Y ;
		float x = Position.X - ux * Center.X - vx * Center.Y ;
		float y = Position.Y - uy * Center.X - vy * Center.Y ;
		vertexData[ vertexID + 0 ].Position.X = x ;
		vertexData[ vertexID + 0 ].Position.Y = y ;
		vertexData[ vertexID + 1 ].Position.X = x + vx ;
		vertexData[ vertexID + 1 ].Position.Y = y + vy ;
		vertexData[ vertexID + 2 ].Position.X = x + ux ;
		vertexData[ vertexID + 2 ].Position.Y = y + uy ;
		vertexData[ vertexID + 3 ].Position.X = x + ux + vx ;
		vertexData[ vertexID + 3 ].Position.Y = y + uy + vy ;
		batch.needUpdateVertexData = true ;
	}
	public void UpdateTexCoord()
	{
		var vertexData = batch.vertexData ;
		vertexData[ vertexID + 0 ].TexCoord.X = UVOffset.X ;
		vertexData[ vertexID + 0 ].TexCoord.Y = UVOffset.Y ;
		vertexData[ vertexID + 1 ].TexCoord.X = UVOffset.X ;
		vertexData[ vertexID + 1 ].TexCoord.Y = UVOffset.Y + UVSize.Y ;
		vertexData[ vertexID + 2 ].TexCoord.X = UVOffset.X + UVSize.X ;
		vertexData[ vertexID + 2 ].TexCoord.Y = UVOffset.Y ;
		vertexData[ vertexID + 3 ].TexCoord.X = UVOffset.X + UVSize.X ;
		vertexData[ vertexID + 3 ].TexCoord.Y = UVOffset.Y + UVSize.Y ;
		batch.needUpdateVertexData = true ;
	}
	public void UpdateColor()
	{
		var vertexData = batch.vertexData ;
		for ( int i = 0 ; i < 4 ; i ++ ) {
			vertexData[ vertexID + i ].Color = Color ;
		}
		batch.needUpdateVertexData = true ;
	}

	//  Subroutines

	void UpdateSortKey( bool sort )
	{
		long newKey = ( (long)zIndex << 32 ) | material.MaterialID ;
		if ( sort ) batch.RemoveFromSortedList( this ) ;
		sortKey = newKey ;
		if ( sort ) batch.AddToSortedList( this ) ;
	}

	//  Batch params

	internal SpriteBatch batch ;
	internal SpriteMaterial material ;
	internal Sprite sortPrev, sortNext ;
	internal Sprite drawNext ;
	internal int spriteID ;
	internal int vertexID ;
	internal int indexID ;
	internal int zIndex ;
	internal long sortKey ;

	//  Sprite params

	public SpriteMaterial Material {
		get { return material ; }
		set { material = value ; UpdateSortKey( batch != null ) ; }
	}
	public int ZIndex {
		get { return zIndex ; }
		set { zIndex = value ; UpdateSortKey( batch != null ) ; }
	}

	public Vector2 Position ;
	public Vector2 Velocity ;
	public float Direction ;
	public float Rotation ;
	public Vector2 Size ;
	public Vector2 Center ;
	public Vector2 UVOffset ;
	public Vector2 UVSize ;
	public Rgba Color ;
}


} //  end ns Sample
