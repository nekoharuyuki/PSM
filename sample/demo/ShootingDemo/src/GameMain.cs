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
using DemoGame;

namespace ShootingDemo
{

/**
 * GameMainクラス
 */
public class GameMain : Application
{
    private SceneManager sceneManager;

    /// コンストラクタ
    public GameMain()
    {
    }

    /// デストラクタ
    ~GameMain()
    {
    }

    /// 初期化
    public override bool DoInit()
    {
        Graphics2D.Init(graphicsDevice.Graphics, GameData.TargetScreenWidth, GameData.TargetScreenHeight);
        Renderer.Init(graphicsDevice);
        InputManager.Init(inputGPad, inputTouch);

        RenderGeometry.Init("/Application/shaders/AmbientColor.cgx", null);
        GameData.Init();
#if TARGET_FRAME_HIGH
		GameData.TargetFps = 60;
#elif TARGET_FRAME_LOW
		GameData.TargetFps = 30;
#else
		GameData.TargetFps = 30;
#endif
		SetUpperLimitFps( GameData.TargetFps );
			
        sceneManager = new SceneManager();
        if (sceneManager.Init() == false) {
            return false;
        }
        sceneManager.Next(new SceneTitle(), false);

        return true;
    }

    /// 解放
    public override bool DoTerm()
    {
        if (sceneManager != null) {
            sceneManager.Term();
            sceneManager = null;
        }

        AudioManager.Clear();

        RenderGeometry.Term();

        Graphics2D.Term();
        InputManager.Term();
        Renderer.Term();
        GameData.Term();

        return true;
    }

    /// 更新
    public override bool DoUpdate()
    {
        GameData.FrameTime.Update();

        GameData.SetCurrentMs(GetMs());

        return sceneManager.Update();
    }

    /// 描画
    public override bool DoRender()
    {
        return sceneManager.Render();
    }
}

} // ShootingDemo
