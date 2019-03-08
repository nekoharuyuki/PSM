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
 * SystemParametersSample
 */
public static class SystemParametersSample
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

        int fontHeight = SampleDraw.CurrentFont.Metrics.Height;
        int positionX = (graphics.Screen.Width / 2) - 192;
        int positionY = (graphics.Screen.Height / 2) - (fontHeight * 3);

        positionY += fontHeight * 2;
        SampleDraw.DrawText("GamePadButtonMeaning : " + SystemParameters.GamePadButtonMeaning.ToString(), 0xffffffff, positionX, positionY);
        positionY += fontHeight;
        SampleDraw.DrawText("Language : " + SystemParameters.Language, 0xffffffff, positionX, positionY);
        positionY += fontHeight;
        SampleDraw.DrawText("YesNoLayout : " + SystemParameters.YesNoLayout.ToString(), 0xffffffff, positionX, positionY);

        SampleDraw.DrawText("SystemParameters Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
