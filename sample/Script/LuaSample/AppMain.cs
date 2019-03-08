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

using LuaInterface;

namespace Sample
{

/**
 * LuaSample
 */
public static class LuaSample
{
    static GraphicsContext graphics;
    static bool loop = true;

    static Lua lua;

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

    public static void Init()
    {
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        // Set up LuaInterface
        lua = new Lua();

        // Register C# function
        lua.RegisterFunction("FillCircle", null, typeof(LuaSample).GetMethod("FillCircle"));

        // Read Lua file
        lua.DoFile("/Application/touch.lua");
    }

    public static void Term()
    {
        // Close LuaInterface
        lua.Dispose();

        SampleDraw.Term();
        graphics.Dispose();
    }

    public static void Update()
    {
        SampleDraw.Update();
    }

    public static void Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        foreach (var touchData in Touch.GetData(0)) {
            if (touchData.Status == TouchStatus.Down ||
                touchData.Status == TouchStatus.Move) {

                // Call Lua function
                int pointX = (int)((touchData.X + 0.5f) * SampleDraw.Width);
                int pointY = (int)((touchData.Y + 0.5f) * SampleDraw.Height);
                bool isDown = touchData.Status == TouchStatus.Down;
                lua.GetFunction("OnTouch").Call(pointX, pointY, isDown);
            }
        }

        // Get Lua variables
        var titleString = Convert.ToString(lua["TitleString"]);
        var colorIndex = Convert.ToInt32(lua["ColorIndex"]);
        var message = string.Format("Color {0}", colorIndex);

        SampleDraw.DrawText(titleString, 0xffffffff, 0, 0);
        SampleDraw.DrawText(message, 0xffffffff, 0, graphics.Screen.Height - 24);

        graphics.SwapBuffers();
    }

    public static void FillCircle(uint argb, int pointX, int pointY, int radius)
    { 
        SampleDraw.FillCircle(argb, pointX, pointY, radius); 
    }
}

} // Sample
