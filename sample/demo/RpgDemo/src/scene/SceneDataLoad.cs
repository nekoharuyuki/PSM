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
/// シーン：データの読み込み
///***************************************************************************
public class SceneDataLoad : DemoGame.IScene
{
    private DemoGame.SceneManager        useSceneMgr;
    private int                          taskId;
    

/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        taskId        = 0;
        useSceneMgr = sceneMgr;

        return true;
    }

    /// シーンの破棄
    public void Term()
    {
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
            taskId ++;
            break;

        case 1:
            /// リソース管理クラスの初期化
            Data.ModelDataManager.GetInstance().Init( (int)Data.ModelResId.Max,
                                                         (int)Data.ModelTexResId.Max, (int)Data.ModelShaderReslId.Max );
            Data.CharParamDataManager.GetInstance().Init( 5 );
            taskId ++;
            break;
            
        case 2:
            /// レイアウト＆２Ｄ素材の読み込み
            AppLyout.GetInstance().Load();

            /// サウンドの読み込み
            AppSound.GetInstance().Init();

            loadModelData();
            loadParamData();

            useSceneMgr.Next( ( new SceneTitle() ), false );
            break;
        }

        return true;
    }

    /// 描画処理
    public bool Render()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 0.0f, 0.025f, 0.25f, 0.0f ) ;
        useGraphDev.Graphics.Clear() ;

        DemoGame.Graphics2D.RemoveSprite( "mess" );
        DemoGame.Graphics2D.AddSprite( "mess", " Now Loading ...", 0xffffffff, 20, 120 );
        DemoGame.Graphics2D.DrawSprites();

        AppDispEff.GetInstance().Draw( useGraphDev );

        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "mess" );
        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    private bool loadModelData()
    {
        SetupModelData    setupData = new SetupModelData();

        if( setupData.SetStgData() == false ){
            return false;
        }
        if( setupData.SetCharData() == false ){
            return false;
        }
        if( setupData.SetEquipData() == false ){
            return false;
        }
        if( setupData.SetEffData() == false ){
            return false;
        }
        if( setupData.SetFixData() == false ){
            return false;
        }

        setupData = null;
        return true;
    }


    private bool loadParamData()
    {
        SetupChMvtData    setupData = new SetupChMvtData();

        if( setupData.SetHeroData() == false ){
            return false;
        }
        if( setupData.SetMonsterAData() == false ){
            return false;
        }
        if( setupData.SetMonsterBData() == false ){
            return false;
        }
        if( setupData.SetMonsterCData() == false ){
            return false;
        }
            
        setupData = null;
        return true;
    }


}

} // namespace
