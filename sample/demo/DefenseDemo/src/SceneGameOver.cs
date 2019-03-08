/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;
using DemoGame;

namespace DefenseDemo{

/// ゲームオーバー画面クラス
/**
 * @version 0.1, 2011/06/23
 */
public
class SceneGameOver
    : IScene
{
	private SceneManager useSceneMgr;
	/// リソースデータ
	private ResourceDataContainer resourceDataContainer;

	/// コンストラクタ
	/**
	 */
    public SceneGameOver()
    {
    }

	/// デストラクタ
	/**
	 */
    ~SceneGameOver()
    {
    }

	/// シーンの初期化
	/**
	 * @param useSceneMgr シーン管理クラス
	 */
    public bool Init(SceneManager useSceneMgr)
    {
		this.useSceneMgr = useSceneMgr;
        return true;
    }

	/// シーンの破棄
	/**
	 */
    public void Term()
    {
		this.useSceneMgr = null;
    }

	/// シーンの継続切り替え時の再開処理
	/**
	 */
    public bool Restart()
    {
        return true;
    }

	/// シーンの継続切り替え時の停止処理
	/**
	 */
    public bool Pause()
    {
        return true;
    }

	/// サスペンド処理
	/**
	 */
    public void Suspend()
    {
    }

	/// レジューム処理
	/**
	 */
    public void Resume()
    {
    }

	/// フレーム処理
	/**
	 */
    public bool Update()
    {
		var pad = InputManager.InputGamePad;
		var touch = InputManager.InputTouch;
		if( pad.Trig != 0 || touch.GetInputNum() > 0 ){
			this.useSceneMgr.Next( new SceneTitle(), false );
		}
        return true;
    }

	/// 描画処理
	/**
	 */
    public bool Render()
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        graphics.SetClearColor(0.0f, 0.0f, 1.0f, 0.0f);
        graphics.Clear();
		
		Graphics2D.DrawText( "SceneGameOver", 0xffffffff, 0, 0 );
		
        graphics.SwapBuffers();
        return true;
    }
}


} // end ns DefenseDemo

//===
// EOF
//===
