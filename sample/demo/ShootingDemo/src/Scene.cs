/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace ShootingDemo
{

/**
 * Sceneクラス
 */
public abstract class Scene : IScene
{
    private SceneManager sceneManager;
    private IScene nextScene = null;

    /// コンストラクタ
    public Scene()
    {
    }

    /// デストラクタ
    ~Scene()
    {
    }

    public abstract bool Start();
    public abstract void End();
    public abstract bool Frame();
    public abstract bool Draw();

    public bool Restart() {return true;}
    public bool Pause() {return true;}
    public void Suspend() {}
    public void Resume() {}

    /// シーンの初期化
    public bool Init(SceneManager useSceneMgr)
    {
        this.sceneManager = useSceneMgr;
        return Start();
    }

    /// シーンの破棄
    public void Term()
    {
        End();
    }

    /// 更新処理
    public bool Update()
    {
        bool result = Frame();

        if (nextScene != null && GameData.FadeAction.IsPlayEnd()) {
            sceneManager.Next(nextScene, false);
            GameData.FadeAction.SetCurrent("FadeIn");
            nextScene = null;
        }

        GameData.FadeAction.Update(GameData.FrameTimeMillis);

        return result;
    }

    /// 描画処理
    public bool Render()
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        bool result = Draw();

        GameData.FadeAction.Render();

#if DEBUG			
		// note: disabling because it creates too many PixelBuffer objects, causing gxm to run out of memory
        Graphics2D.RemoveSprite("Ms");
        Graphics2D.AddSprite("Ms", "" + GameData.GetCurrentMs() + "ms", 0xffffffff, 0, 0);
        Graphics2D.DrawSprite("Ms");
#endif				
        graphics.SwapBuffers();

        return result;
    }

    /// 次のシーンのセット
    public void SetNextScene(IScene nextScene)
    {
        if (this.nextScene == null) {
            this.nextScene = nextScene;
            GameData.FadeAction.SetCurrent("FadeOut");
        }
    }

}

} // ShootingDemo
