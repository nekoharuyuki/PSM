/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {


///***************************************************************************
/// シーン：ゲームメイン
///***************************************************************************
public class SceneGameMain : DemoGame.IScene
{
    private DemoGame.SceneManager        useSceneMgr;
	private int startMessCnt;

	private string strStartMess = "Defeat 10 enemies!";

/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        /// ゲーム制御開始
        GameCtrlManager.GetInstance().Start();

        /// 配置情報のセット
        SetupObjPlaceData.Load();

        setupLyout();

        AppSound.GetInstance().PlayBgm( AppSound.BgmId.Main, true );

        AppDispEff.GetInstance().SetFadeIn( 0xffffff, 10, true );

        useSceneMgr = sceneMgr;

		startMessCnt = 0;

        return true;
    }

    /// シーンの破棄
    public void Term()
    {
        /// ゲーム制御開始
        GameCtrlManager.GetInstance().End();

        useSceneMgr = null;
    }

    /// シーンの継続切り替え時の再開処理
    public bool Restart()
    {
        return true;
    }

    /// シーンの継続切り替え時の停止処理
    public bool Pause()
    {
        AppLyout.GetInstance().ClearSpriteAll();
        return true;
    }

    /// サスペンド＆レジューム処理
    public void Suspend()
    {
    }
    public void Resume()
    {
    }

    /// フレーム処理
    public bool Update()
    {
        AppDebug.CheckTimeStart();

#if DEBUG_MODE
        /// デバックモードへ
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;
        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            useSceneMgr.Next( ( new SceneDebugMenu() ), true );
            return true;
        }
#endif

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        /// ゲーム制御
        ctrlResMgr.Frame();

        setupLyout();

        AppDebug.CheckTimeEnd();

        /// ゲームの終了チェック
        if( ctrlResMgr.CtrlPl.GetHp() <= 0 || ctrlResMgr.CtrlEn.GetEntryNum() <= 0 ){
            useSceneMgr.Next( ( new SceneGameResult() ), true );
            return true;
        }
        return true;
    }

    /// 描画処理
    public bool Render()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 0.5f, 0.5f, 0.5f, 0.0f ) ;
        useGraphDev.Graphics.Clear() ;

        /// ゲーム制御
        ctrlResMgr.Draw();

///     /// デバック用ＦＰＳ表示
///     DemoGame.Graphics2D.AddSprite( "Fps", "ms : "+GameCtrlManager.GetInstance().GetMs()+
///                                                 " (Fps : "+((int)GameCtrlManager.GetInstance().GetFps())+")", 0xffffffff,
///                                                 2, useGraphDev.DisplayHeight-28 );
///
///     DemoGame.Graphics2D.AddSprite( "Mem", "CollNum : "+AppDebug.CollCnt+"(MS:"+AppDebug.TimeCal+")", 0xffffffff,
///                                                            2, useGraphDev.DisplayHeight-28*2 );
///     DemoGame.Graphics2D.AddSprite( "Wood", "("+AppDebug.WoodCnt+")", 0xffffffff, 2, useGraphDev.DisplayHeight-28*2 );
///     AppDebug.WoodCnt = 0;

#if DEBUG
        DemoGame.Graphics2D.AddSprite( "Fps", GameCtrlManager.GetInstance().GetMs()+ "ms", 0xffffffff, 0, 0 );
#endif

        AppDebug.CollCnt = 0;

		/// 開始からしばらくの間、メッセージを表示
		if( startMessCnt < 90 ){
			renderStartMess();
		}

        /// レイアウトの描画
        AppLyout.GetInstance().Render();

        AppDispEff.GetInstance().Draw( useGraphDev );

        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Fps" );
        DemoGame.Graphics2D.RemoveSprite( "Mess" );

#if DEBUG_MODE
///        DemoGame.Graphics2D.RemoveSprite( "Mem" );
///        DemoGame.Graphics2D.RemoveSprite( "Wood" );
#endif
        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// レイアウトのセット    
    public void setupLyout()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        AppLyout.GetInstance().ClearSpriteAll();

        setLyoutHp( ctrlResMgr.CtrlPl.GetHp() );
        setLyoutEnNum( ctrlResMgr.CtrlEn.GetEntryNum() );
    }

    /// プレイヤーＨＰセット
    public void setLyoutHp( int hp )
    {
        /// 現在のＨＰ
        if( hp == 0 ){
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life1_off );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life2_off );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life3_off );
        }
        else if( hp == 1 ){
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life1_on );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life2_off );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life3_off );
        }
        else if( hp == 2 ){
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life1_on );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life2_on );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life3_off );
        }
        else{
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life1_on );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life2_on );
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life3_on );
        }

        /// 下地
        AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Life );
    }

    /// 敵の数セット
    public void setLyoutEnNum( int num )
    {
        AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Num_life_0 );
        AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Num_life_1 );

        AppLyout.GetInstance().SetFocus( AppLyout.SpriteId.Num_life_0, num/10 );
        AppLyout.GetInstance().SetFocus( AppLyout.SpriteId.Num_life_1, num%10 );
    }


    /// ゲームスタート時のメッセージ
    public void renderStartMess()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;
			
		int strW = DemoGame.Graphics2D.CurrentFont.GetTextWidth( strStartMess ); 
		int strH = DemoGame.Graphics2D.CurrentFont.Size; 


        DemoGame.Graphics2D.FillRect( 0x80000000, 0, useGraphDev.DisplayHeight/2-strH/2-5, useGraphDev.DisplayWidth, strH+10 );
        DemoGame.Graphics2D.AddSprite( "Mess", strStartMess, 0xffffffff,
				useGraphDev.DisplayWidth/2-strW/2, useGraphDev.DisplayHeight/2-strH/2 );

		startMessCnt ++;
    }
}

} // namespace
