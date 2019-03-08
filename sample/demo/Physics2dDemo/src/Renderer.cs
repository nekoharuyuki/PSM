/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// Rendererクラス
///
public class Renderer
{
    private static GraphicsDevice graphicsDevice;

    /// 初期化
	///
	/// @param [in] graphicsDevice
	/// @param [out]
	///
    public static bool Init(GraphicsDevice graphicsDevice)
    {
        Renderer.graphicsDevice = graphicsDevice;

        GetGraphicsContext().Enable(EnableMode.CullFace);
        GetGraphicsContext().SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);

        GetGraphicsContext().Enable(EnableMode.Blend);
        GetGraphicsContext().SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);

        return true;
    }

    /// 終了
    public static void Term()
    {
        Renderer.graphicsDevice = null;
    }

    /// GraphicsDeviceの取得
    public static GraphicsDevice GetGraphicsDevice()
    {
        return graphicsDevice;
    }

    /// GraphicsContextの取得
    public static GraphicsContext GetGraphicsContext()
    {
        return graphicsDevice.Graphics;
    }
}

} // Physics2dDemo
