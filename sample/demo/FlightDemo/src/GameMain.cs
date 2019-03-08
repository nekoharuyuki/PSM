/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using DemoGame;

namespace FlightDemo{


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
        //        GameData.Init(graphicsDevice.Graphics);
        Renderer.Init( graphicsDevice );
        Graphics2D.Init( graphicsDevice.Graphics );
        DbgSphere.Init();
        InputManager.Init(inputGPad, inputTouch);

		
        RenderGeometry.Init("/Application/shaders/AmbientColor.cgx", null);

        sceneManager = new SceneManager();
        if( sceneManager.Init() == false ){
            return false;
        }
        //sceneManager.Next( new SceneGameMain(), false );
        GameCommonData.Init();
		sceneManager.Next( new SceneTitle(), false );

        return true;
    }

    /// 解放
    public override bool DoTerm()
    {
		GameCommonData.Term();
		
        if( sceneManager != null ){
            sceneManager.Term();
            sceneManager = null;
        }

        Graphics2D.Term();
        DbgSphere.Term();
        
        return true;
    }

    /// 更新
    public override bool DoUpdate()
    {
		GameCommonData.Inst().SetMs( this.GetMs() );
        return sceneManager.Update();
    }

    /// 描画
    public override bool DoRender()
    {
        return sceneManager.Render();
    }



}

} // end ns FlightDemo
//===
// EOF
//===
