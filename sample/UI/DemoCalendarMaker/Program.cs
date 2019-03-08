/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;


namespace CalendarMaker
{

    class Program
    {
        static GraphicsContext graphics = new GraphicsContext();

        static void Main(string[] args)
        {
            // Initialize
            UISystem.Initialize(graphics, getPixelDensity());

            // set first scene
            UISystem.SetScene(new MainScene());
            
            // main loop
            for (; ; )
            {
                // check system events
                SystemEvents.CheckEvents();

                // update
                {
                    // update UI
                    List<TouchData> touchDataList = Touch.GetData(0);
                    UISystem.Update(touchDataList);
                }
                
                // draw
                {
                    // clear
                    graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
                    graphics.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                    graphics.Clear();

                    // draw UI
                    UISystem.Render();

                    graphics.SwapBuffers();
                }
            }
        }

        static float getPixelDensity()
        {
            float pixelDensity;

            float w = graphics.Screen.Width / SystemParameters.DisplayDpiX;
            float h = graphics.Screen.Height / SystemParameters.DisplayDpiY;
            float inchDiagSq = w * w + h * h;

            if (inchDiagSq < 6 * 6)
            {
                // normal size display ( < 6 inch)
                if (SystemParameters.DisplayDpiX < 300)
                {
                    // normal resolution
                    pixelDensity = 1.0f;
                }
                else
                {
                    // high resolution
                    pixelDensity = 1.5f;
                }
            }
            else
            {
                // large size display ( > 6 inch)
                if (SystemParameters.DisplayDpiX < 200)
                {
                    // normal resolution
                    pixelDensity = 1.0f;
                }
                else
                {
                    // high resolution
                    pixelDensity = 1.5f;
                }
            }
            return pixelDensity;
        }
    }
}
