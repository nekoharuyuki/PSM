/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;


namespace Sample
{
	public class AppMain
	{
		static protected GraphicsContext graphics;
		static int colorValue=0;	//<- here.
		
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
		}

		public static void Update ()
		{
			colorValue++;		//<- here.
			if(colorValue>255)	//<- here.
				colorValue=0;	//<- here.
			
			graphics.SetClearColor(colorValue, colorValue, colorValue, 255);
		}
		
		public static void Render ()
		{
			graphics.Clear();
			
			graphics.SwapBuffers();	
		}
	}
}
