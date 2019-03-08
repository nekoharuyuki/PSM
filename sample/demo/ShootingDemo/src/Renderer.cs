/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace ShootingDemo
{

/**
 * Rendererクラス
 */
public static class Renderer
{
    private static GraphicsDevice graphicsDevice;

    /// 初期化
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

    /// ディスプレイの横サイズ
    public static int DisplayWidth
    {
        get {return GameData.TargetScreenWidth;}
    }

    /// ディスプレイの縦サイズ
    public static int DisplayHeight
    {
        get {return GameData.TargetScreenHeight;}
    }
}

} // ShootingDemo
