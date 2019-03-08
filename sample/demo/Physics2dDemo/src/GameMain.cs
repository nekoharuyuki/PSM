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

namespace Physics2dDemo
{

///
/// GameMainクラス
///
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
	
	/// 開始処理
	public override bool DoInit()
	{
		Renderer.Init( graphicsDevice );
        Graphics2D.Init(graphicsDevice.Graphics,854,480);
		InputManager.Init(inputGPad, inputTouch);
		
		/// リソース２Dクラスのセットアップ
        Resource2d.GetInstance().Init( graphicsDevice );
			
		RenderGeometry.Init("/Application/shaders/AmbientColor.cgx", null);
		GameData.Init();
			
		sceneManager = new SceneManager();
        if( sceneManager.Init() == false ){
            return false;
        }
        sceneManager.Next( new SceneTitle(), false );	
		
		return true;
	}
	
	/// 終了処理
	public override bool DoTerm()
	{
        if( sceneManager != null ){
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
	
	/// 更新処理
	public override bool DoUpdate()
	{
        GameData.FrameTime.Update();			
        GameData.SetCurrentMs(GetMs());
//		GameData.SetCurrentFps(GetFps());

		return sceneManager.Update();
	}
	
	/// 描画
	public override bool DoRender()
	{
		return sceneManager.Render();
	}
}
	
} // Physics2dDemo
