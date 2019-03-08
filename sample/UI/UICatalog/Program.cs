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


namespace UICatalog
{
    class Program
    {
        static GraphicsContext graphics;

        static Stopwatch stopwatch;
        static int frameCount = 0;
        static int updateTicks = 0;
        static int renderTicks = 0;
        static int swapbufferTicks = 0;
        static float freqms;

        static float getPixelDensity(GraphicsContext graphics)
        {
            float pixelDensity;

            float dpiX = SystemParameters.DisplayDpiX;
            float dpiY = SystemParameters.DisplayDpiY;
            float w = graphics.Screen.Width / dpiX;
            float h = graphics.Screen.Height / dpiY;
            float inchDiagSq = w * w + h * h;

            if (inchDiagSq < 6 * 6)
            {
                // normal size display
                if (dpiX < 300)
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
                // large size display
                if (dpiX < 200)
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

            Console.WriteLine("Screen Size: {0}x{1} pixels, {2:.00}x{3:.00} inches, {4:.00} inch Diag.",
                              graphics.Screen.Width,
                              graphics.Screen.Height,
                              w, h, FMath.Sqrt(inchDiagSq));
            Console.WriteLine("DPI: {0}, {1}",
                              dpiX, dpiY);
            Console.WriteLine("PixelDensity: {0:.00}\nScaled screen size: {1:#.}x{2:#.}",
                              pixelDensity,
                              graphics.Screen.Width / pixelDensity,
                              graphics.Screen.Height / pixelDensity);

            return pixelDensity;
        }


        static void Main(string[] args)
        {
            // Initialize
            stopwatch = Stopwatch.StartNew();
            freqms = 1000f / Stopwatch.Frequency;
            
            int start = (int)stopwatch.ElapsedTicks;
            graphics = new GraphicsContext();
            
            Console.WriteLine("new GraphicsContext: " + ((int)stopwatch.ElapsedTicks - start) * freqms);
            
            
            start = (int)stopwatch.ElapsedTicks;

            float pixelDensity = getPixelDensity(graphics);
            UISystem.Initialize(graphics, pixelDensity);

            Console.WriteLine("Init UISystem: " + ((int)stopwatch.ElapsedTicks - start) * freqms);
            
            
            start = (int)stopwatch.ElapsedTicks;
            UISystem.SetScene(new LoadingScene());

            Console.WriteLine("Set LoadingScene: " + ((int)stopwatch.ElapsedTicks - start) * freqms);


            // main loop
            for (; ; )
            {
                start = (int)stopwatch.ElapsedTicks;

                // check system events
                SystemEvents.CheckEvents();

                // update
                {
                    // update UI
                    List<TouchData> touchDataList = Touch.GetData(0);
                    GamePadData gamePad = GamePad.GetData (0);
                    MotionData motionData = Motion.GetData(0);

                    UISystem.Update(touchDataList, ref gamePad, ref motionData);
                }
                updateTicks += (int)stopwatch.ElapsedTicks - start;

                // draw
                {
                    // clear
                    graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
                    graphics.SetClearColor(0.2f, 0.2f, 0.2f, 1.0f);
                    graphics.Clear();

                    // draw UI
                    UISystem.Render();
                    renderTicks += (int)stopwatch.ElapsedTicks - start;

                    graphics.SwapBuffers();
                    swapbufferTicks += (int)stopwatch.ElapsedTicks - start;
                }


                // Write time
                if ((frameCount++) % 100 == 0)
                {
                    // first loop
                    if (frameCount == 1)
                    {
                        updateTicks *= 100;
                        renderTicks *= 100;
                        swapbufferTicks *= 100;
                    }


                    float freq = (float)Stopwatch.Frequency;
                    float fps = freq * 100.0f / (float)swapbufferTicks;

                    float updateMs = (float)updateTicks / freq * 10.0f;
                    float renderMs = (float)(renderTicks - updateTicks) / freq * 10.0f;
                    float swapMs = (float)(swapbufferTicks - renderTicks) / freq * 10.0f;
                    Console.Write("Update {0:f3}ms / Render {1:f3}ms / Swap {2:f3}ms / {3:f2}fps\n",
                        updateMs, renderMs, swapMs, fps);

                    updateTicks = renderTicks = swapbufferTicks = 0;
                }
            }
        }

    }
}
