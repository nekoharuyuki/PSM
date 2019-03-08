// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using DemoGame;

namespace DefenseDemo{


/// GameMainクラス
class GameMain
    : Application
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
//		Console.WriteLine( "[log][GameMain.cs][DoInit]start" );
        Renderer.Init( graphicsDevice );
			
        Graphics2D.Init( graphicsDevice.Graphics, 854, 480 );
			
        InputManager.Init(inputGPad, inputTouch);

        RenderGeometry.Init("/Application/shaders/AmbientColor.cgx", null);

        sceneManager = new SceneManager();
			
        if( sceneManager.Init() == false ){
            return false;
        }
			
		sceneManager.Next( new SceneTitle(), false );

        return true;
    }

    /// 解放
    public override bool DoTerm()
    {
        if( sceneManager != null ){
            sceneManager.Term();
            sceneManager = null;
        }

        Graphics2D.Term();
        
        return true;
    }

    /// 更新
    public override bool DoUpdate()
    {
		Fps.SetFpsTime( this.GetMs() );
		Fps.SetFpsVal( this.GetFps() );
        return sceneManager.Update();
    }

    /// 描画
    public override bool DoRender()
    {
        return sceneManager.Render();
    }



}

} // end ns DefenseDemo
//===
// EOF
//===
