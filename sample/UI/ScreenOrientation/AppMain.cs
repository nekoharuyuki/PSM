using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace ScreenOrientationSample
{
    public class AppMain
    {
        private static GraphicsContext graphics;

        public static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                SystemEvents.CheckEvents();
                Update();
                Render();
            }
        }

        public static void Initialize()
        {
            // Set up the graphics system
            graphics = new GraphicsContext();

            // Initialize UI Toolkit
            UISystem.Initialize(graphics);

            var topScene = new TopScene();
            UISystem.SetScene(topScene, null);
        }

        public static void Update()
        {
            var touchData = Touch.GetData(0);
            var gamePadData = GamePad.GetData(0);

            // Update UI Toolkit
            UISystem.Update(touchData, ref gamePadData);
        }

        public static void Render()
        {
            // Clear the screen
            graphics.SetClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            graphics.Clear();

            // Render UI Toolkit
            UISystem.Render();

            // Present the screen
            graphics.SwapBuffers();
        }
    }
}
