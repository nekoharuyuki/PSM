/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{

    static class MainClass
    {

        static Stopwatch stopwatch;
        static GraphicsContext graphics;
        static GameLayer gameLayer;

        static int frameCount;
        static int cpuTicks;
        static int gpuTicks;

        static void Main(string[] args)
        {
            Init();
            while (true)
            {
                int start = (int)stopwatch.ElapsedTicks;

                SystemEvents.CheckEvents();

                List<TouchData> touchData = Touch.GetData(0);

                // Update UI
                UISystem.Update(touchData);
                // Update main model
                gameLayer.Update(touchData);

                // Clear graphics
                graphics.SetViewport(0, 0, graphics.Screen.Width, graphics.Screen.Height);
                graphics.SetClearColor(Globals.SettingColorRgba);
                graphics.SetClearDepth(1.0f);
                graphics.Clear();

                // Draw main model
                gameLayer.Draw();
                graphics.Enable(EnableMode.DepthTest, false);

                // Draw UI
                UISystem.Render();

                cpuTicks += (int)stopwatch.ElapsedTicks - start;
                graphics.SwapBuffers();
                gpuTicks += (int)stopwatch.ElapsedTicks - start;

                if ((++frameCount) % 100 == 0)
                {
                    float freq = (float)Stopwatch.Frequency;
                    float cpu = (float)cpuTicks * 60.0f / freq;
                    float gpu = (float)gpuTicks * 60.0f / freq;
                    float fps = freq * 100.0f / (float)gpuTicks;
                    Console.Write("CPU {0:f2}% / GPU {1:f2}% / {2:f2}fps\n", cpu, gpu, fps);
                    cpuTicks = gpuTicks = 0;
                }
            }
        }

        static void Init()
        {
            stopwatch = Stopwatch.StartNew();
            graphics = new GraphicsContext();

            Globals.GameState = GameState.Stop;

            gameLayer = new GameLayer();
            gameLayer.Init(graphics);

            UISystem.Initialize(graphics);
            //scene = new UIOverlayScene();
            var scene = new UITopMenu();
            UISystem.SetScene(scene);
        }
    }

}
