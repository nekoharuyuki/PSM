/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;

using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using Tutorial.Utility;


namespace Sample
{
	public class AppMain
	{
		static protected GraphicsContext graphics;
		
		static SimpleSprite spritePlayer;
		static Texture2D texture;
		static GamePadData gamePadData;
		
		
		public static void Main (string[] args)
		{
			Initialize ();

			while (true) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}

		public static void Initialize ()
		{
			graphics = new GraphicsContext();
			
			ImageRect rectScreen = graphics.Screen.Rectangle;
			
			texture = new Texture2D("/Application/resources/Player.png", false);
			spritePlayer = new SimpleSprite(graphics, texture);
			spritePlayer.Position.X = rectScreen.Width/2.0f;
			spritePlayer.Position.Y = rectScreen.Height/2.0f;
			spritePlayer.Position.Z = 0.0f;
		
		}

		public static void Update ()
		{
			gamePadData = GamePad.GetData(0);
			
			int speed = 4;
			
			if((gamePadData.Buttons & GamePadButtons.Left) != 0)
			{
				spritePlayer.Position.X -= speed;
			}
			if((gamePadData.Buttons & GamePadButtons.Right) != 0)
			{
				spritePlayer.Position.X += speed;
			}
			if((gamePadData.Buttons & GamePadButtons.Up) != 0)
			{
				spritePlayer.Position.Y -= speed;
			}
			if((gamePadData.Buttons & GamePadButtons.Down) != 0)
			{
				spritePlayer.Position.Y += speed;
			}
		}

		public static void Render ()
		{
			graphics.Clear();

			spritePlayer.Render();
			
			graphics.SwapBuffers();	
		}
	}
}
