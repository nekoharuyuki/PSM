/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace Sample
{

/**
 * ShellSample
 */
public static class ShellSample
{
    private static GraphicsContext graphics;
    private static SampleButton browserButton;

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

        int rectW = 256;
        int rectH = 64;
        int rectX = (SampleDraw.Width - rectW) / 2;
        int rectY = (SampleDraw.Height - 24 - rectH * 3) / 2;

        browserButton = new SampleButton(rectX, rectY, rectW, rectH);

        browserButton.SetText("Browser");

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

        List<TouchData> touchDataList = Touch.GetData(0);

        if (browserButton.TouchDown(touchDataList)) {
            Shell.Action action = Shell.Action.BrowserAction("http://www.playstation.com/psm/developer/");
            Shell.Execute(ref action);
            return true;
        }

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        browserButton.Draw();

        SampleDraw.DrawText("Shell Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
