/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

#define EASY_SETUP
//#define EXTERNAL_INPUT

using System.Collections;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

static class HelloSprite
{
	static void Main( string[] args )
	{
		#if EASY_SETUP

		// initialize GameEngine2D's singletons
		Director.Initialize();

		#else // #if EASY_SETUP

		// create our own context
		Sce.PlayStation.Core.Graphics.GraphicsContext context = new Sce.PlayStation.Core.Graphics.GraphicsContext();

		// maximum number of sprites you intend to use (not including particles)
		uint sprites_capacity = 500;

		// maximum number of vertices that can be used in debug draws
		uint draw_helpers_capacity = 400;

		// initialize GameEngine2D's singletons, passing context from outside
		Director.Initialize( sprites_capacity, draw_helpers_capacity, context );

		#endif // #if EASY_SETUP 

		Director.Instance.GL.Context.SetClearColor( Colors.Grey20 );

		// set debug flags that display rulers to debug coordinates
//		Director.Instance.DebugFlags |= DebugFlags.DrawGrid;
		// set the camera navigation debug flag (press left alt + mouse to navigate in 2d space)
		Director.Instance.DebugFlags |= DebugFlags.Navigate; 

		// create a new scene
		var scene = new Scene();

		// set the camera so that the part of the word we see on screen matches in screen coordinates
		scene.Camera.SetViewFromViewport();

		// create a new TextureInfo object, used by sprite primitives
		var texture_info = new TextureInfo( new Texture2D("/Application/king_water_drop.png", false ) );

		// create a new sprite
		var sprite = new SpriteUV() { TextureInfo = texture_info};

		// make the texture 1:1 on screen
		sprite.Quad.S = texture_info.TextureSizef; 

		// center the sprite around its own .Position 
		// (by default .Position is the lower left bit of the sprite)
		sprite.CenterSprite();

		// put the sprite at the center of the screen
		sprite.Position = scene.Camera.CalcBounds().Center;

		// our scene only has 2 nodes: scene->sprite
		scene.AddChild( sprite );

		#if EASY_SETUP

		Director.Instance.RunWithScene( scene );

		#else // #if EASY_SETUP

		// handle the loop ourself

		Director.Instance.RunWithScene( scene, true );

		while ( !Input2.GamePad0.Cross.Press )
		{
			Sce.PlayStation.Core.Environment.SystemEvents.CheckEvents();

			#if EXTERNAL_INPUT

			// it is not needed but you can set external input data if you want

			List<TouchData> touch_data_list = Touch.GetData(0);
			Input2.Touch.SetData( 0, touch_data_list );

			GamePadData pad_data = GamePad.GetData(0);
			Input2.GamePad.SetData( 0, pad_data );

			#endif // #if EXTERNAL_INPUT
			
			Director.Instance.Update();
			Director.Instance.Render();

			Director.Instance.GL.Context.SwapBuffers();
			Director.Instance.PostSwap(); // you must call this after SwapBuffers

//			System.Console.WriteLine( "Director.Instance.DebugStats.DrawArraysCount " + Director.Instance.GL.DebugStats.DrawArraysCount );
		}

		#endif // #if EASY_SETUP

		Director.Terminate();

		System.Console.WriteLine( "Bye!" );
	}
}

