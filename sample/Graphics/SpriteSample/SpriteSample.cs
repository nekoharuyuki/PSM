/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System ;
using System.Threading ;
using System.Diagnostics ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.Core.Input ;

namespace Sample
{

/**
 * SpriteSample
 */
class SpriteSample
{
	static GraphicsContext graphics ;
	static Rectangle boundary ;
	static Random random ;

	static SpriteBatch batch ;
	static SpriteMaterial material ;
	static Sprite[] sprites ;
	static int spriteCount ;
	static float spriteSize = 24.0f ;
	static float spriteSpin = 1.0f ;

	static bool loop = true ;

	static void Main( string[] args )
	{
		Init() ;
		while ( loop ) {
			SystemEvents.CheckEvents() ;
			Update() ;
			Render() ;
		}
		Term() ;
	}

	static bool Init()
	{
		graphics = new GraphicsContext() ;
		boundary = new Rectangle( 32, 32, graphics.Screen.Width - 64, graphics.Screen.Height - 64 ) ;
		random = new Random() ;
		SampleDraw.Init( graphics ) ;
		SampleTimer.Init() ;

		InitSpriteBatch( 4096 ) ;
		SetSpriteCount( 4096 ) ;
		return true ;
	}

	static void Term()
	{
		SampleDraw.Term() ;
		SampleTimer.Term() ;
		graphics.Dispose() ;
	}

	static bool Update()
	{
		SampleDraw.Update() ;

		var padData = GamePad.GetData( 0 ) ;
		var press = padData.ButtonsDown ;
		if ( press != 0 ) {
			if ( ( GamePadButtons.Left & press ) != 0 ) SetSpriteCount( spriteCount * 2 ) ;
			if ( ( GamePadButtons.Right & press ) != 0 ) SetSpriteCount( spriteCount / 2 ) ;
			if ( ( GamePadButtons.Up & press ) != 0 ) SetSpriteSize( spriteSize + 4 ) ;
			if ( ( GamePadButtons.Down & press ) != 0 ) SetSpriteSize( spriteSize - 4 ) ;
			if ( ( GamePadButtons.Circle & press ) != 0 ) spriteSpin = 1 - spriteSpin ;
		}
		return true ;
	}

	static bool Render()
	{
		SampleTimer.StartFrame() ;

		graphics.SetViewport( 0, 0, graphics.Screen.Width, graphics.Screen.Height ) ;
		graphics.SetClearColor( 0.0f, 0.5f, 1.0f, 0.0f ) ;
		graphics.Clear() ;

		for ( int i = 0 ; i < spriteCount ; i ++ ) {
			MoveSprite( sprites[ i ], SampleTimer.DeltaTime ) ;
		}
		batch.Draw() ;

		SampleDraw.DrawText( "Sprite Sample", 0xffffffff, 0, 0 ) ;
		var msg = string.Format( "SpriteCount {0:D4} / FrameRate {1:F2} fps / FrameTime {2:F2} ms",
					spriteCount, SampleTimer.AverageFrameRate, SampleTimer.AverageFrameTime * 1000 ) ;
		SampleDraw.DrawText( msg, 0xffffffff, 0, graphics.Screen.Height - 24 ) ;

		SampleTimer.EndFrame() ;
		graphics.SwapBuffers() ;
		return true ;
	}

	//------------------------------------------------------------
	//  Sprite functions
	//------------------------------------------------------------

	static void InitSpriteBatch( int maxSpriteCount )
	{
		batch = new SpriteBatch( graphics, maxSpriteCount ) ;
		material = new SpriteMaterial( new Texture2D( "/Application/test.png", true ) ) ;
		material.Texture.SetFilter( TextureFilterMode.Linear, TextureFilterMode.Linear,
									TextureFilterMode.Nearest ) ;

		sprites = new Sprite[ maxSpriteCount ] ;
		spriteCount = 0 ;
	}

	static void SetSpriteCount( int count )
	{
		count = Math.Min( Math.Max( count, 1 ), sprites.Length ) ;
		if ( count > spriteCount ) {
			for ( int i = spriteCount ; i < count ; i ++ ) {
				sprites[ i ] = new Sprite( batch, material, 0 ) ;
				InitSprite( sprites[ i ] ) ;
			}
		} else {
			for ( int i = count ; i < spriteCount ; i ++ ) {
				batch.RemoveSprite( sprites[ i ] ) ;
			}
		}
		spriteCount = count ;
	}

	static void SetSpriteSize( float size )
	{
		size = FMath.Clamp( size, 4.0f, 512.0f ) ;
		for ( int i = 0 ; i < spriteCount ; i ++ ) {
			sprites[ i ].Size.X = sprites[ i ].Size.Y = size ;
		}
		spriteSize = size ;
	}

	static void InitSprite( Sprite s )
	{
		s.Position.X = boundary.X + boundary.Width * (float)random.NextDouble() ;
		s.Position.Y = boundary.Y + boundary.Height * (float)random.NextDouble() ;
		s.Velocity = Vector2.Rotation( FMath.Radians( random.Next( 360 ) ) ) * 100 ;
		s.Direction = FMath.Radians( random.Next( 360 ) ) ;
		s.Rotation = FMath.Radians( random.Next( -360, 360 ) ) ;
		s.Size = new Vector2( spriteSize, spriteSize ) ;
		s.Center = new Vector2( 0.5f, 0.5f ) ;
		s.UVOffset = new Vector2( random.Next( 4 ), random.Next( 4 ) ) * 0.25f ;
		s.UVSize = new Vector2( 0.25f, 0.25f ) ;
		s.Color = new Rgba( 255, 255, 255, 255 ) ;

		s.UpdateAll() ;
	}

	static void MoveSprite( Sprite s, float delta )
	{
		s.Position.X += s.Velocity.X * delta ;
		s.Position.Y += s.Velocity.Y * delta ;
		s.Direction += s.Rotation * delta * spriteSpin ;
		if ( s.Position.X < boundary.X ) s.Velocity.X = FMath.Abs( s.Velocity.X ) ;
		if ( s.Position.Y < boundary.Y ) s.Velocity.Y = FMath.Abs( s.Velocity.Y ) ;
		if ( s.Position.X > boundary.X + boundary.Width ) s.Velocity.X = - FMath.Abs( s.Velocity.X ) ;
		if ( s.Position.Y > boundary.Y + boundary.Height ) s.Velocity.Y = - FMath.Abs( s.Velocity.Y ) ;

		s.UpdatePosition() ;
	}
}


} // end ns Sample
