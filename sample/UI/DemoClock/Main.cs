/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

using System.Diagnostics;

namespace DemoClock
{
    class MainClass
    {
        static GraphicsContext graphics = new GraphicsContext();
        
        static Stopwatch stopwatch = Stopwatch.StartNew();
        static int frameCount = 0;
        static int updateTicks = 0;
        static int renderTicks = 0;
        static int swapbufferTicks = 0;
        static float freqms;
        
        const float filteringFactor = 0.1f;
        const float accelThreshold = 0.10f;
        static float[] accel = new float[3];
        static float[] preAccel = new float[3];
        static long preShake = Environment.TickCount;

        static void Main(string[] args)
        {
            freqms = 1000f / Stopwatch.Frequency;
            int start = (int)stopwatch.ElapsedTicks;
            Console.WriteLine("Start: " + ((int)stopwatch.ElapsedTicks - start) * freqms);

            // Initialize UI Toolkit
            UISystem.Initialize(graphics, getPixelDensity());
            Console.WriteLine("Init: " + ((int)stopwatch.ElapsedTicks - start) * freqms);

            // create scene
            MainScene scene = new MainScene();
            // set scene
            UISystem.SetScene(scene, null);
            Console.WriteLine("newScene: " + ((int)stopwatch.ElapsedTicks - start) * freqms);
            
            Console.WriteLine("x,y=" + graphics.Screen.Width + "," + graphics.Screen.Height);
            
            // main loop
            for (; ; )
            {
                start = (int)stopwatch.ElapsedTicks;
                // check system events
                SystemEvents.CheckEvents();

                // update
                {
                    try
                    {
                        List<TouchData> touchDataList = Touch.GetData(0);

                        // update UI Toolkit
                        UISystem.Update(touchDataList);
                        
                        MotionData motionData = Motion.GetData(0);
                        
                        // high-pass filter
                        accel[0] = motionData.Acceleration.X - preAccel[0];
                        accel[1] = motionData.Acceleration.Y - preAccel[1];
                        accel[2] = motionData.Acceleration.Z - preAccel[2];
                        preAccel[0] = motionData.Acceleration.X;
                        preAccel[1] = motionData.Acceleration.Y;
                        preAccel[2] = motionData.Acceleration.Z;
                    
                        if( accel[2] * accel[2] > accelThreshold && accel[2] > 0 )
                        {
                            Console.WriteLine( "acc z = " + accel[2] );
                            if( Environment.TickCount - preShake > 1000 )
                            {
                                preShake = Environment.TickCount;
                                
                                Console.WriteLine( "shake!!" );
                                //scene.OnShake(accel[2]);
                            }
                        }
                    }
                    catch
                    {
                        
                    }
                }
                updateTicks += (int)stopwatch.ElapsedTicks - start;

                // draw
                {
                    // clear
                    graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
                    graphics.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                    graphics.Clear();

                    // render UI Toolkit
                    UISystem.Render();
                    renderTicks += (int)stopwatch.ElapsedTicks - start;

                    graphics.SwapBuffers();
                    swapbufferTicks += (int)stopwatch.ElapsedTicks - start;
                    
                }

                // write frame rate
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
                    Console.Write("{0:f2}fps ({1:f3}ms) /up {2:f3}, ren {3:f3}, swap {4:f3}\n",
                        fps, 1000 / fps, updateMs, renderMs, swapMs);

                    updateTicks = renderTicks = swapbufferTicks = 0;
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
