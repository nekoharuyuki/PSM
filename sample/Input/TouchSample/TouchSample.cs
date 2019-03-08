/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace Sample
{

/**
 * TouchSample
 */
public static class TouchSample
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

        uint[] colorTable = {0xffff0000,
                             0xff00ff00,
                             0xff0000ff,
                             0xffffff00};

        foreach (var touchData in Touch.GetData(0)) {
            if (touchData.Status == TouchStatus.Down ||
                touchData.Status == TouchStatus.Move) {

                int pointX = (int)((touchData.X + 0.5f) * SampleDraw.Width);
                int pointY = (int)((touchData.Y + 0.5f) * SampleDraw.Height);
                int colorId = touchData.ID % colorTable.Length;

                SampleDraw.FillCircle(colorTable[colorId], pointX, pointY, 96);
            }
        }

        SampleDraw.DrawText("Touch Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
