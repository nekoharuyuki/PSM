/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;

namespace Sample
{

/**
 * SystemEventsSample
 */
public static class SystemEventsSample
{
    private static GraphicsContext graphics;

    static bool loop = true;

    public static void Main(string[] args)
    {
        Init();

        while (loop) {
            SystemEvents.CheckEvents();
            Update();
            Render();
        }

        Term();
    }

    public static bool Init()
    {
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        SystemEvents.OnRestored += delegate(object sender, RestoredEventArgs e) {
            Console.WriteLine("Restored");
        };

		SystemEvents.OnMinimized += delegate(object sender, MinimizedEventArgs e) {
           Console.WriteLine("Minimized");
        };

        return true;
    }

    /// Terminate
    public static void Term()
    {
        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        SampleDraw.Update();

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        SampleDraw.DrawText("SystemEvents Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
