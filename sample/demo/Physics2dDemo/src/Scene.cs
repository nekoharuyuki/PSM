/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// Sceneクラス
///
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
	///
	/// @param [out]
	///
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
	
	/// 更新
	///
	/// @param [out]
	///
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

    /// 描画
	///
	/// @param [out]
	///
	public bool Render()
	{
		GraphicsContext graphics = Renderer.GetGraphicsContext();
		graphics.SetClearColor(1.0f, 1.0f, 1.0f, 0.0f);
        graphics.Clear();

        bool result = Draw();		
		GameData.FadeAction.Render();

#if DEBUG
        /// デバック用ＦＰＳ表示
		Graphics2D.DrawText("ms:"+GameData.GetCurrentMs(), 0xff000000, 0, 0);
		//Graphics2D.DrawText("fps:"+GameData.GetCurrentFps(), 0xff000000, 0, 20);
#endif			
			
		graphics.SwapBuffers();	
		return result;
	}
		
	/// 次のシーンのセット
	///
	/// @param [in] nextScene 次のシーン
	///
    public void SetNextScene(IScene nextScene)
    {
        if (this.nextScene == null) {
            this.nextScene = nextScene;
			GameData.FadeAction.SetCurrent("FadeOut");
        }
    }
}
	
} // Physics2dDemo
