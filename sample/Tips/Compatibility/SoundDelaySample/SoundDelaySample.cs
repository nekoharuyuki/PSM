/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    public static class AppMain
    {
        private static GraphicsContext graphics;
        private static MainScene mainScene;

        public static void Main(string[] args)
        {
            Initialize();
            while (true) {
                SystemEvents.CheckEvents();
                Update();
                Render();
            }
        }
        public static void Initialize()
        {
            graphics = new GraphicsContext();

            UISystem.Initialize(graphics);
            mainScene = new MainScene();
            UISystem.SetScene(mainScene, null);
        }
        public static void Update()
        {
            List<TouchData> touchDataList = Touch.GetData(0);
            UISystem.Update(touchDataList);
        }
        public static void Render()
        {
            graphics.SetClearColor(0.3f, 0.5f, 1.0f, 0.0f);
            graphics.Clear();
            UISystem.Render();
            graphics.SwapBuffers();
        }
    }
}
