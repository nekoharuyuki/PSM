/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {

///***************************************************************************
/// エントリクラス
///***************************************************************************
static class RpgDemo
{
    static RpgMain appMain;

    /// エントリポイント
    static void Main( string[] args )
    {
        appMain = new RpgMain();
        appMain.SetUpperLimitFps( 32 );
        appMain.Run( args );
    }
}


///***************************************************************************
/// アプリケーション本体
///***************************************************************************
public class RpgMain : DemoGame.Application
{
    private DemoGame.SceneManager    sceneMgr;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        /// シーンマネージャの生成
        ///---------------------------------------------
        sceneMgr = new DemoGame.SceneManager();
        if( sceneMgr.Init() == false ){
            return false;
        }

        /// デバックパラメータの初期化
        ///---------------------------------------------
        AppDebug.Init();


        /// 入力クラスのセットアップ
        ///---------------------------------------------
        AppInput.GetInstance().Init( inputGPad, inputTouch, graphicsDevice );


        /// レイアウトクラスのセットアップ
        ///---------------------------------------------
        AppLyout.GetInstance().Init( graphicsDevice );


        /// シーンパラメータマネージャのセットアップ
        ///---------------------------------------------
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        ctrlResMgr.Init();
        ctrlResMgr.SetGraphicsDevice( graphicsDevice );


        /// 画面効果クラスのセットアップ
        ///---------------------------------------------
        AppDispEff.GetInstance().Init();


        /// デバック系のセットアップ
        ///---------------------------------------------
        DemoGame.RenderGeometry.Init( "/Application/shaders/AmbientColor.cgx", null );


        sceneMgr.Next( ( new SceneDataLoad() ), false );
        return true;
    }

    /// 破棄
    public override bool DoTerm()
    {
          if( sceneMgr != null ){
            sceneMgr.Term();
        }
        sceneMgr    = null;
            
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        ctrlResMgr.Term();

        AppDispEff.GetInstance().Term();

        AppDebug.Term();
        AppInput.GetInstance().Term();
        AppLyout.GetInstance().Term();
        AppSound.GetInstance().Term();
        DemoGame.RenderGeometry.Term();

        Data.ModelDataManager.GetInstance().Term();

        return true;
    }

    /// フレーム
    public override bool DoUpdate()
    {
        AppInput.GetInstance().Frame();

        if( AppDispEff.GetInstance().Frame() ){
            return true;
        }

        sceneMgr.Update();

        GameCtrlManager.GetInstance().SetFps( GetFps() );
        GameCtrlManager.GetInstance().SetMs( GetMs() );
        return true;
    }

    /// 描画
    public override bool DoRender()
    {
        sceneMgr.Render();
        return true;
    }


/// private メソッド
///---------------------------------------------------------------------------

}

} // namespace
