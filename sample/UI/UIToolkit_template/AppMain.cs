using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace UIToolkitApp
{
    public class AppMain
    {
        private static GraphicsContext graphics;
        
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
            
            // Initialize UI Toolkit
            UISystem.Initialize (graphics);

            // TODO: Create scenes and call UISystem.SetScene
            // Scene myScene = new Scene();
            // UISystem.SetScene(myScene, null);
        }

        public static void Update ()
        {
            // Query touch for current state
            List<TouchData> touchDataList = Touch.GetData (0);
            
            // Update UI Toolkit
            UISystem.Update(touchDataList);
        }

        public static void Render ()
        {
            // Clear the screen
            graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
            graphics.Clear ();
            
            // Render UI Toolkit
            UISystem.Render ();
            
            // Present the screen
            graphics.SwapBuffers ();
        }
    }
}
