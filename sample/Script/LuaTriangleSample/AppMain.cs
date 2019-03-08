/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;

using LuaInterface;

namespace Sample
{

/**
 * LuaTriangleSample
 */
class LuaTriangleSample
{   
    public static void Main(string[] args)
    {
        var dummy = typeof(Sample.SampleDraw);    // require SampleLib.dll

        var lua = new Lua();
        lua.DoFile("/Application/triangle.lua");
        lua.DoString("Main()");
        lua.Dispose();
    }
}

} // end ns Sample
