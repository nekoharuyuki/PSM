using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace ProjectAction
{
	public class AppMain
	{
		private static GraphicsContext graphics;
		static int colorValue=0;    //<- here.
		
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
			// Set up the graphics system
			graphics = new GraphicsContext ();
		}

		public static void Update ()
		{
			colorValue++;       //<- here.
        	if(colorValue>255)  //<- here.
            colorValue=0;   //<- here.

        	graphics.SetClearColor(colorValue, colorValue, colorValue, 255);//<- here.
			// Query gamepad for current state
			//var gamePadData = GamePad.GetData (0);
		}

		public static void Render ()
		{
			// Clear the screen
			//graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear ();

			// Present the screen
			graphics.SwapBuffers ();
		}
	}
}
