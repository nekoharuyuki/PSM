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
/// シーン：ゲームリザルト
///***************************************************************************
public class SceneGameResult : DemoGame.IScene
{

    private DemoGame.SceneManager        useSceneMgr;
    private int                          taskId;
    private int                          alphaCnt;
    private float                        alpha;


/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        taskId        = 0;
        useSceneMgr = sceneMgr;

        AppLyout.GetInstance().ClearSpriteAll();

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        if( ctrlResMgr.CtrlPl.GetHp() <= 0 ){
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Mess_GAMEOVER );
            AppSound.GetInstance().PlayBgm( AppSound.BgmId.Gameover, false );
        }
        else{
            AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Mess_ENEMYALLCLEAR );
            AppSound.GetInstance().PlayBgm( AppSound.BgmId.Clear, false );
        }

        alphaCnt = 0;
        alpha = 0.0f;

        return true;
    }

    /// シーンの破棄
    public void Term()
    {
        AppLyout.GetInstance().ClearSpriteAll();
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
        switch( taskId ){

        case 0:
            if( AppSound.GetInstance().IsBgmPlaing() == false ){
                AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.TouchScreen );
                taskId ++;
            }
            break;

        case 1:
            if( (AppInput.GetInstance().Event & AppInput.EventId.SpelAtk) != 0 || AppInput.GetInstance().TouchRelease == true ){
                alphaCnt    = 0;
                AppDispEff.GetInstance().SetFadeOut( 0xffffff, 5, true );
                taskId ++;
                break;
            }

            alphaCnt ++;
            if( alphaCnt < 100 ){
                alpha += 0.02f;
                if( alpha >= 1.0f ){
                    alpha        = 1.0f;
                    alphaCnt    = 100;
                }
            }
            else{
                alpha -= 0.02f;
                if( alpha < 0.25f ){
                    alpha        = 0.25f;
                    alphaCnt    = 0;
                }
            }
            break;

        case 2:
            if( AppDispEff.GetInstance().NowEffId != AppDispEff.EffId.FadeOut ){
                useSceneMgr.Next( ( new SceneTitle() ), false );
            }
            alpha        = 0.0f;
            break;
        }

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        ctrlResMgr.FrameResult();

        AppLyout.GetInstance().SetAlpha( AppLyout.SpriteId.TouchScreen, alpha );
        return true;
    }

    /// 描画処理
    public bool Render()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 0.5f, 0.5f, 0.5f, 0.0f ) ;
        useGraphDev.Graphics.Clear() ;

        ctrlResMgr.Draw();

        /// レイアウトの描画
        AppLyout.GetInstance().Render();

        AppDispEff.GetInstance().Draw( useGraphDev );

         useGraphDev.Graphics.SwapBuffers();

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

}

} // namespace
