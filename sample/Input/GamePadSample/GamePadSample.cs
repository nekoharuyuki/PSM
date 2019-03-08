/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace Sample
{

/**
 * GamePadSample
 */
public static class GamePadSample
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
        var gamePadData = GamePad.GetData(0);

        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        int fontHeight = SampleDraw.CurrentFont.Metrics.Height;
        int positionX = (SampleDraw.Width / 5);
        int positionY = ((SampleDraw.Height / 2) - (428 / 2)) + (fontHeight * 3);

        foreach (GamePadButtons item in Enum.GetValues(typeof(GamePadButtons))) {
            drawPadState(positionX, positionY, item.ToString(), (gamePadData.Buttons & item) != 0);
            positionY += fontHeight;
        }

        positionX = (SampleDraw.Width / 5) * 3;
        positionY = ((SampleDraw.Height / 2) - (428 / 2)) + (fontHeight * 3);

        SampleDraw.DrawText("AnalogLeftX : " + gamePadData.AnalogLeftX, 0xffffffff, positionX, positionY);
        positionY += fontHeight;
        SampleDraw.DrawText("AnalogLeftY : " + gamePadData.AnalogLeftY, 0xffffffff, positionX, positionY);
        positionY += fontHeight;
        SampleDraw.DrawText("AnalogRightX : " + gamePadData.AnalogRightX, 0xffffffff, positionX, positionY);
        positionY += fontHeight;
        SampleDraw.DrawText("AnalogRightY : " + gamePadData.AnalogRightY, 0xffffffff, positionX, positionY);

        SampleDraw.DrawText("GamePad Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }

    ///
    private static void drawPadState(int positionX, int positionY, string buttonName, bool isButtonDown)
    {
        uint argb;
        string buttonState;

        if (isButtonDown) {
            argb = 0xffff0000;
            buttonState = "ON";
        } else {
            argb = 0xffffffff;
            buttonState = "OFF";
        }

        SampleDraw.DrawText("" + buttonName + " : " + buttonState, argb, positionX, positionY);
    }
}

} // Sample
