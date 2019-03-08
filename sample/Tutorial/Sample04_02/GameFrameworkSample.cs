/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.Framework;


namespace Sample
{
	public class GameFrameworkSample: GameFramework
	{
		public ImageRect rectScreen;
		public Random rand = new Random(123);
		
		Texture2D texturePlayer, textureStar;
		
		Int32 counter=0;
		
		public Actor root;
		
	    public override void Initialize()
		{
			base.Initialize();
			
			rectScreen = graphics.GetViewport();

			root = new Actor("root");

			texturePlayer = new Texture2D("/Application/resources/Player.png", false);
			root.AddChild(new Player(this, "Player", texturePlayer));
			
			textureStar = new Texture2D("/Application/resources/Star.png", false);			
			Actor starManager = new Actor("starManager");
			
			Vector4[] starColors ={
				new Vector4( 0.5f, 0.5f, 1.0f, 1.0f),	// blue
				new Vector4( 0.0f, 1.0f, 0.0f, 1.0f),	// green 
				new Vector4( 1.0f, 1.0f, 0.0f, 1.0f),	// yellow
				new Vector4( 1.0f, 0.5f, 0.0f, 1.0f),	// 
				new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),	// red
			};
			
			Star star;
			for( int i=0; i< 20; ++i)
			{
				star= new Star(this, "star"+i, textureStar, 
					new Vector3((float)(rectScreen.Width * rand.NextDouble()),(float)(rectScreen.Height* rand.NextDouble()),0.7f),
					starColors[ i % starColors.Length],
					(float)(1.0f * (rand.NextDouble() + 0.5f)));
				
				starManager.AddChild(star);
			}
			root.AddChild(starManager);
		}
		

	    public override void Update()
	    {
			base.Update();

#if DEBUG			
			debugString.WriteLine("counter "+counter);
			debugString.WriteLine("Buttons="+PadData.Buttons);
#endif
			
			root.Update();
			
			++counter;
	    }
		
	    public override void Render()
	    {
	        graphics.Clear();

			graphics.Enable(EnableMode.DepthTest);

			root.Render();
				
	        base.Render();
	    }
		
		public override void Terminate ()
		{
			texturePlayer.Dispose();
			textureStar.Dispose();
			
			base.Terminate ();
		}
		
	}
}
