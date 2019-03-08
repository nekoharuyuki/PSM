/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using DemoGame;

namespace ShootingDemo
{

/**
 * PathCameraクラス
 */
public class PathCamera : Mono
{
    private Camera camera = new Camera();

    /// コンストラクタ
    public PathCamera(string name = null) : base(name)
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        int width = Renderer.DisplayWidth;
        int height = Renderer.DisplayHeight;
        float angle = 48.6f;
        float near = 100.0f;
        float far = 250000.0f;
        float dist = (height / 2) / FMath.Tan(FMath.Radians(angle / 2));

        camera.SetPerspective(width, height, angle, near, far);
        camera.SetLookAt(new Vector3(0.0f, 90.0f, 0.0f),
                         new Vector3(0, 0, 0),
                         -dist);

        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        Renderer.GetGraphicsDevice().SetCurrentCamera(camera);

        return true;
    }
}

} // ShootingDemo
