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
/// シーン：デバックメニュー
///***************************************************************************
public class SceneDebugMenu : DemoGame.IScene
{
    private enum debugMenuTaskId{
        MenuTop,
        EnemyEntry,
        EnemyPlace,
        FixEntry,
        FixPlace,
        LodParam,
        EnemyParam,
        EffCheck,
        SoundCheck,
        GameSetup,
        UpdatePlace,
        SavePlaceParam,
    };

    private DemoGame.SceneManager        useSceneMgr;
    private debugMenuTaskId              nowTaskId;
    private debugMenuTaskId              nextTaskId;

    private int                          nowSubTask;
    private int                          topMenuId;
    private int                          enemyId;
    private int                          soundBgmNo;
    private int                          soundSeNo;
    private int                          soundTrg;
    private int                          lodNo;
    private int                          effNo;
    private int                          gameSetupId;


    private string[] menuTopStringList = {
        "Enemy Entry",
        "Enemy Place",
        "Fix Entry",
        "Fix Place",
        "Lod Param",
        "Enemy Param",
        "Eff Check",
        "Sound Check",
        "Game Setup",
        "Update Place",
        "Save Place Data"
    };
    private string[] enemyEntryStringList = {
        "Hero",
        "Monster A",
        "Monster B",
        "Monster C"
    };
    private string[] fixEntryStringList = {
        "Box",
        "Barrel",
        "Fence",
        "Grass",
        "Board",
        "Pillar",
        "House A",
        "House B",
        "House C",
        "Cart",
        "Firewood",
        "Ship",
        "Arch",
        "Tree A",
        "Tree B",
        "Pillar A",
        "Pillar B",
        "Pillar C",
        "House D",
        "House E",
    };

    private string[] gameSetupStringList = {
        "Pl Non Draw",
        "Toon",
        "AtkStepPlay",
        "Gravity",
        "CollLight",
    };


/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        useSceneMgr = sceneMgr;

        soundBgmNo    = 0;
        soundSeNo    = 0;
        lodNo        = 0;
        effNo        = 0;

        nowTaskId    = debugMenuTaskId.MenuTop;
        changeTask( debugMenuTaskId.MenuTop );

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
        nowTaskId    = debugMenuTaskId.MenuTop;
        changeTask( debugMenuTaskId.MenuTop );
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
        switch( nowTaskId ){
        case debugMenuTaskId.MenuTop:            frameMenuTop();         break;
        case debugMenuTaskId.EnemyEntry:         frameEnemyEntry();      break;
        case debugMenuTaskId.FixEntry:           frameFixEntry();        break;

        case debugMenuTaskId.EnemyPlace:         frameObjPlace();        break;
        case debugMenuTaskId.FixPlace:           frameObjPlace();        break;
        case debugMenuTaskId.LodParam:           frameLodParam();        break;
        case debugMenuTaskId.EnemyParam:         frameEnemyParam();      break;
        case debugMenuTaskId.EffCheck:           frameEffCheck();        break;
        case debugMenuTaskId.SoundCheck:         frameSoundCheck();      break;
        case debugMenuTaskId.GameSetup:          frameGameSetup();       break;
        case debugMenuTaskId.UpdatePlace:        frameUpdatePlace();     break;
        case debugMenuTaskId.SavePlaceParam:     frameSavePlaceParam();  break;
        }

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        ctrlResMgr.CtrlCam.FramePlace( ctrlResMgr.CtrlPl.GetPos() );

        return true;
    }


    /// 描画処理
    public bool Render()
    {
        if( nowTaskId != nextTaskId ){
            nowTaskId    = nextTaskId;
            return true;
        }

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 1.0f, 0.025f, 0.25f, 1.0f ) ;
        useGraphDev.Graphics.Clear() ;

        ctrlResMgr.Draw();


        /// デバック用ＦＰＳ表示
        DemoGame.Graphics2D.AddSprite( "Fps", "MS : "+GameCtrlManager.GetInstance().GetMs()+
                                                    "  (Fps : "+GameCtrlManager.GetInstance().GetFps()+")", 0xffffffff,
                                                    2, useGraphDev.DisplayHeight-28 );

        switch( nowTaskId ){
        case debugMenuTaskId.MenuTop:            renderMenuTop();        break;
        case debugMenuTaskId.EnemyEntry:        renderEnemyEntry();        break;
        case debugMenuTaskId.FixEntry:            renderFixEntry();        break;
        case debugMenuTaskId.LodParam:            renderLodParam();        break;
        case debugMenuTaskId.EnemyParam:        renderEnemyParam();        break;
        case debugMenuTaskId.EffCheck:            renderEffCheck();        break;
        case debugMenuTaskId.SoundCheck:        renderSoundCheck();        break;
        case debugMenuTaskId.GameSetup:            renderGameSetup();        break;
        case debugMenuTaskId.UpdatePlace:        renderUpdatePlace();    break;
        case debugMenuTaskId.SavePlaceParam:    renderSavePlaceParam();    break;
        }
        DemoGame.Graphics2D.RemoveSprite( "Fps" );

        return true;
    }



    private void changeTask( debugMenuTaskId task)
    {
        nextTaskId        = task;
        nowSubTask        = 0;
    }

/// メニュートップ
///---------------------------------------------------------------------------

    public bool frameMenuTop()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        /// デバックモードを抜ける
        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            useSceneMgr.Prev();
            return true;
        }

        
        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            topMenuId --;
            if( topMenuId < 0 ){
                topMenuId = menuTopStringList.Length-1;
            }
        }
        if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            topMenuId = (topMenuId+1)%menuTopStringList.Length;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            switch( topMenuId ){
            case 0:        changeTask( debugMenuTaskId.EnemyEntry );        break;
            case 1:        changeTask( debugMenuTaskId.EnemyPlace );        break;
            case 2:        changeTask( debugMenuTaskId.FixEntry );            break;
            case 3:        changeTask( debugMenuTaskId.FixPlace );            break;
            case 4:        changeTask( debugMenuTaskId.LodParam );            break;
            case 5:        changeTask( debugMenuTaskId.EnemyParam );        break;
            case 6:        changeTask( debugMenuTaskId.EffCheck );            break;
            case 7:        changeTask( debugMenuTaskId.SoundCheck );        break;
            case 8:        changeTask( debugMenuTaskId.GameSetup );        break;
            case 9:        changeTask( debugMenuTaskId.UpdatePlace );        break;
            case 10:    changeTask( debugMenuTaskId.SavePlaceParam );    break;
            }
        }

        return true;
    }
    public bool renderMenuTop()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.AddSprite( "Mess", "DebugMenu", 0xff40ff40, 2, 2 );
        DemoGame.Graphics2D.DrawSprites();

        for( int i=0; i<menuTopStringList.Length; i++ ){
            if( topMenuId == i ){
                DemoGame.Graphics2D.AddSprite( "Mess"+i, menuTopStringList[i], 0xffff0000, 30, 50+i*28 );
            }
            else{
                DemoGame.Graphics2D.AddSprite( "Mess"+i, menuTopStringList[i], 0xffffffff, 30, 50+i*28 );
            }
            DemoGame.Graphics2D.DrawSprites();
        }

        useGraphDev.Graphics.SwapBuffers();

        for( int i=0; i<menuTopStringList.Length; i++ ){
            DemoGame.Graphics2D.RemoveSprite( "Mess"+i );
        }
        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        return true;
    }



/// 敵の登録
///---------------------------------------------------------------------------

    public bool frameEnemyEntry()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        /// 初期化
        if( nowSubTask == 0 ){
            enemyId = 0;
            nowSubTask ++;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            enemyId --;
            if( enemyId < 0 ){
                enemyId = enemyEntryStringList.Length-1;
            }
        }
        if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            enemyId = (enemyId+1)%enemyEntryStringList.Length;
        }


        /// 敵の登録
        if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){

            GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
            ctrlResMgr.CtrlEn.EntryAddEnemy( enemyId, 0.0f, ctrlResMgr.CtrlPl.GetPos() );
            
            changeTask( debugMenuTaskId.MenuTop );
        }

        /// 戻る
        else if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
        }

        return true;
    }
    public bool renderEnemyEntry()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.AddSprite( "Mess", "Entry Enemy", 0xff40ff40, 2, 2 );
        DemoGame.Graphics2D.DrawSprites();

        for( int i=0; i<enemyEntryStringList.Length; i++ ){
            if( enemyId == i ){
                DemoGame.Graphics2D.AddSprite( "Mess"+i, enemyEntryStringList[i], 0xffff0000, 30, 50+i*28 );
            }
            else{
                DemoGame.Graphics2D.AddSprite( "Mess"+i, enemyEntryStringList[i], 0xffffffff, 30, 50+i*28 );
            }
            DemoGame.Graphics2D.DrawSprites();
        }

        useGraphDev.Graphics.SwapBuffers();

        for( int i=0; i<enemyEntryStringList.Length; i++ ){
            DemoGame.Graphics2D.RemoveSprite( "Mess"+i );
        }
        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        return true;
    }



/// 備品の登録
///---------------------------------------------------------------------------

    public bool frameFixEntry()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        /// 初期化
        if( nowSubTask == 0 ){
            enemyId = 0;
            nowSubTask ++;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            enemyId --;
            if( enemyId < 0 ){
                enemyId = fixEntryStringList.Length-1;
            }
        }
        if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            enemyId = (enemyId+1)%fixEntryStringList.Length;
        }


        /// 備品の登録
        if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
            ctrlResMgr.CtrlFix.EntryAddFix( enemyId, new Vector3(0.0f,0.0f,0.0f),
                    new Vector3(1.0f,1.0f,1.0f),
                    ctrlResMgr.CtrlPl.GetPos() );

            changeTask( debugMenuTaskId.MenuTop );
        }

        /// 戻る
        else if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
        }

        return true;
    }
    public bool renderFixEntry()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.AddSprite( "Mess", "Entry Fixture", 0xff40ff40, 2, 2 );
        DemoGame.Graphics2D.DrawSprites();


        for( int i=0; i<fixEntryStringList.Length; i++ ){
            string str = String.Format( "{0:D2} : "+fixEntryStringList[i], i );
            int x = (i<12)?     30 : 300;
            int y = (i<12)?     (50+i*28) : (50+(i-12)*28);

            if( enemyId == i ){
                DemoGame.Graphics2D.AddSprite( "Mess"+i, str, 0xffff0000, x, y );
            }
            else{
                DemoGame.Graphics2D.AddSprite( "Mess"+i, str, 0xffffffff, x, y );
            }
            DemoGame.Graphics2D.DrawSprites();
        }

        useGraphDev.Graphics.SwapBuffers();

        for( int i=0; i<fixEntryStringList.Length; i++ ){
            DemoGame.Graphics2D.RemoveSprite( "Mess"+i );
        }
        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        return true;
    }



/// オブジェクトの配置
///---------------------------------------------------------------------------

    public bool frameObjPlace()
    {
        SceneDebugObjPlace nextScene = new SceneDebugObjPlace();

        if( nowTaskId == debugMenuTaskId.EnemyPlace ){
            nextScene.SetTrgType( 0 );
        }
        else{
            nextScene.SetTrgType( 1 );
        }

        useSceneMgr.Next( nextScene, true );
        return true;
    }


/// LODパラメータのセット
///---------------------------------------------------------------------------

    public bool frameLodParam()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
            return true;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            lodNo --;
            if( lodNo < 0 ){
                lodNo = GameCtrlDrawManager.GetInstance().LodLevMax - 1;
            }
        }
        else if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            lodNo = (lodNo+1)%GameCtrlDrawManager.GetInstance().LodLevMax;
        }

        pad.SetRepeatParam( 10, 2 ); 
        if( (pad.Repeat & DemoGame.InputGamePadState.Left) != 0 ){
            GameCtrlDrawManager.GetInstance().SetLodParam( lodNo, (GameCtrlDrawManager.GetInstance().GetLodParam( lodNo ) -1.0f) );
        }
        else if( (pad.Repeat & DemoGame.InputGamePadState.Right) != 0 ){
            GameCtrlDrawManager.GetInstance().SetLodParam( lodNo, (GameCtrlDrawManager.GetInstance().GetLodParam( lodNo ) +1.0f) );
        }
    
        return true;
    }
    public bool renderLodParam()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, 300, 100 );

        DemoGame.Graphics2D.AddSprite( "Mess", "Lod Level Set", 0xff40ff40, 2, 2 );

        for( int i=0; i<GameCtrlDrawManager.GetInstance().LodLevMax; i++ ){
            if( lodNo == i ){
                DemoGame.Graphics2D.AddSprite( "Mess"+i,
                                         "Lev "+i+" : "+GameCtrlDrawManager.GetInstance().GetLodParam( i ) ,
                                         0xffff0000, 30, 50+i*28 );
            }
            else{
                DemoGame.Graphics2D.AddSprite( "Mess"+i,
                                         "Lev "+i+" : "+GameCtrlDrawManager.GetInstance().GetLodParam( i ) ,
                                         0xffffffff, 30, 50+i*28 );
            }
            DemoGame.Graphics2D.DrawSprites();
        }

        DemoGame.Graphics2D.DrawSprites();
        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Mess" );

        for( int i=0; i<GameCtrlDrawManager.GetInstance().LodLevMax; i++ ){
            DemoGame.Graphics2D.RemoveSprite( "Mess"+i );
        }
        return true;
    }


/// 敵パラメータのセット
///---------------------------------------------------------------------------

    public bool frameEnemyParam()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
            return true;
        }

        if( nowSubTask == 0 ){
            enemyId = 0;
            nowSubTask ++;
        }


        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            enemyId --;
            if( enemyId < 0 ){
                enemyId = 1;
            }
        }
        if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            enemyId = (enemyId+1)%2;
        }

        pad.SetRepeatParam( 10, 2 ); 
        if( enemyId == 0 ){
            if( (pad.Repeat & DemoGame.InputGamePadState.Left) != 0 ){
                ctrlResMgr.CtrlEn.EntryAreaDis -= 0.5f;
            }
            else if( (pad.Repeat & DemoGame.InputGamePadState.Right) != 0 ){
                ctrlResMgr.CtrlEn.EntryAreaDis += 0.5f;
            }
        }
        else{
            if( (pad.Repeat & DemoGame.InputGamePadState.Left) != 0 ){
                ctrlResMgr.CtrlEn.EntryStayMax --;
            }
            else if( (pad.Repeat & DemoGame.InputGamePadState.Right) != 0 ){
                ctrlResMgr.CtrlEn.EntryStayMax ++;
            }
        }
        

        ctrlResMgr.CtrlEn.Frame();
        return true;
    }
    public bool renderEnemyParam()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, 300, 100 );

        DemoGame.Graphics2D.AddSprite( "Mess", "Lod Level Set", 0xff40ff40, 2, 2 );

        uint col;
        col = (enemyId == 0)?    0xffff0000    : 0xffffffff;
        DemoGame.Graphics2D.AddSprite( "Mess0",
                                     "EntryDis : "+ctrlResMgr.CtrlEn.EntryAreaDis,
                                     col, 30, 50+0*28 );
        col = (enemyId == 1)?    0xffff0000    : 0xffffffff;
        DemoGame.Graphics2D.AddSprite( "Mess1",
                                     "StayCnt  : "+ctrlResMgr.CtrlEn.EntryStayMax,
                                     col, 30, 50+1*28 );

        DemoGame.Graphics2D.DrawSprites();
        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.RemoveSprite( "Mess0" );
        DemoGame.Graphics2D.RemoveSprite( "Mess1" );
        return true;
    }




/// エフェクトチェック
///---------------------------------------------------------------------------

    public bool frameEffCheck()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
            return true;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Left) != 0 ){
            effNo --;
            if( effNo < 0 ){
                effNo = (int)Data.EffTypeId.Max - 1;
            }
        }
        else if( (pad.Trig & DemoGame.InputGamePadState.Right) != 0 ){
            effNo = (effNo+1)%((int)Data.EffTypeId.Max);
        }
    
        /// 再生
        if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            Vector3 pos = ctrlResMgr.CtrlPl.GetPos();
            pos.Y += 2.0f;
            ctrlResMgr.CtrlEffect.EntryEffect( (Data.EffTypeId)effNo, pos );
        }
    
        ctrlResMgr.CtrlEffect.Frame();

        return true;
    }
    public bool renderEffCheck()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, 300, 100 );

        DemoGame.Graphics2D.AddSprite( "Mess", "EffectTest", 0xff40ff40, 2, 2 );

        DemoGame.Graphics2D.AddSprite( "Mess1", "Eff "+effNo, 0xffff0040, 30, 50+0*28 );
        DemoGame.Graphics2D.DrawSprites();

        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.RemoveSprite( "Mess1" );
        return true;
    }




/// サウンドチェック
///---------------------------------------------------------------------------

    public bool frameSoundCheck()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
            return true;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            soundTrg --;
            if( soundTrg < 0 ){
                soundTrg = 1;
            }
        }
        else if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            soundTrg = (soundTrg+1)%2;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Left) != 0 ){
            if( soundTrg == 0 ){
                soundBgmNo --;
                if( soundBgmNo < 0 ){
                    soundBgmNo = (int)AppSound.BgmId.Max - 1;
                }
            }
            else{
                soundSeNo --;
                if( soundSeNo < 0 ){
                    soundSeNo = (int)AppSound.SeId.Max - 1;
                }
            }
        }
        else if( (pad.Trig & DemoGame.InputGamePadState.Right) != 0 ){
            if( soundTrg == 0 ){
                soundBgmNo = (soundBgmNo+1)%((int)AppSound.BgmId.Max);
            }
            else{
                soundSeNo = (soundSeNo+1)%((int)AppSound.SeId.Max);
            }
        }
    
        /// 再生
        if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            if( soundTrg == 0 ){
                AppSound.GetInstance().PlayBgm( (AppSound.BgmId)soundBgmNo, false );
            }
            else{
                AppSound.GetInstance().PlaySe( (AppSound.SeId)soundSeNo );
            }
        }
    
        return true;
    }
    public bool renderSoundCheck()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.AddSprite( "Mess", "SoundTest", 0xff40ff40, 2, 2 );

        if( soundTrg == 0 ){
            DemoGame.Graphics2D.AddSprite( "Mess1", "Bgm "+soundBgmNo, 0xffff0040, 30, 50+0*28 );
            DemoGame.Graphics2D.AddSprite( "Mess2", "Se "+soundSeNo,   0xff40ff40, 30, 50+1*28 );
        }
        else{
            DemoGame.Graphics2D.AddSprite( "Mess1", "Bgm "+soundBgmNo, 0xff40ff40, 30, 50+0*28 );
            DemoGame.Graphics2D.AddSprite( "Mess2", "Se "+soundSeNo,   0xffffff40, 30, 50+1*28 );
        }
        DemoGame.Graphics2D.DrawSprites();

        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.RemoveSprite( "Mess1" );
        DemoGame.Graphics2D.RemoveSprite( "Mess2" );
        return true;
    }



/// ゲーム制御系セットアップ
///---------------------------------------------------------------------------

    public bool frameGameSetup()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        /// 初期化
        if( nowSubTask == 0 ){
            gameSetupId = 0;
            nowSubTask ++;
        }

        if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            gameSetupId --;
            if( gameSetupId < 0 ){
                gameSetupId = gameSetupStringList.Length-1;
            }
        }
        if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            gameSetupId = (gameSetupId+1)%gameSetupStringList.Length;
        }


        /// セットアップ
        if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            switch( gameSetupId ){
            case 0:        AppDebug.PlDrawFlg    = ( AppDebug.PlDrawFlg == false )? true : false;            break;
            case 1:        AppDebug.ToonFlg        = ( AppDebug.ToonFlg == false )? true : false;            break;
            case 2:        AppDebug.AtkStepPlay    = ( AppDebug.AtkStepPlay == false )? true : false;            break;
            case 3:        AppDebug.GravityFlg    = ( AppDebug.GravityFlg == false )? true : false;            break;
            case 4:        AppDebug.CollLightFlg    = ( AppDebug.CollLightFlg == false )? true : false;            break;
            }
        }

        /// 戻る
        else if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
        }

        return true;
    }
    public bool renderGameSetup()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.AddSprite( "Mess", "Game Setup", 0xff40ff40, 2, 2 );
        DemoGame.Graphics2D.DrawSprites();

        for( int i=0; i<gameSetupStringList.Length; i++ ){
            uint col = 0xffffffff;
            if( gameSetupId == i ){
                col = 0xffff0000;
            }

            switch(i){
            case 0:
                DemoGame.Graphics2D.AddSprite( "Mess"+i, gameSetupStringList[i] +
                                                    "  : "+AppDebug.PlDrawFlg, col, 30, 50+i*28 );
                break;
            case 1:
                DemoGame.Graphics2D.AddSprite( "Mess"+i, gameSetupStringList[i] +
                                                    "  : "+AppDebug.ToonFlg, col, 30, 50+i*28 );
                break;
            case 2:
                DemoGame.Graphics2D.AddSprite( "Mess"+i, gameSetupStringList[i] +
                                                    "  : "+AppDebug.AtkStepPlay, col, 30, 50+i*28 );
                break;
            case 3:
                DemoGame.Graphics2D.AddSprite( "Mess"+i, gameSetupStringList[i] +
                                                    "  : "+AppDebug.GravityFlg, col, 30, 50+i*28 );
                break;
            case 4:
                DemoGame.Graphics2D.AddSprite( "Mess"+i, gameSetupStringList[i] +
                                                    "  : "+AppDebug.CollLightFlg, col, 30, 50+i*28 );
                break;
            }

            DemoGame.Graphics2D.DrawSprites();
        }

        useGraphDev.Graphics.SwapBuffers();

        for( int i=0; i<gameSetupStringList.Length; i++ ){
            DemoGame.Graphics2D.RemoveSprite( "Mess"+i );
        }
        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        return true;
    }


/// 配置情報の保存
///---------------------------------------------------------------------------

    public bool frameUpdatePlace()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        /// 初期化
        if( nowSubTask == 0 ){
            nowSubTask ++;
        }
        else{
            ctrlResMgr.CtrlFix.UpdateFixEntry();
            changeTask( debugMenuTaskId.MenuTop );
        }
        return true;
    }
    public bool renderUpdatePlace()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.AddSprite( "Mess", "Now Update ...", 0xff40ff40, 2, 2 );
        DemoGame.Graphics2D.DrawSprites();

        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        return true;
    }



/// 配置情報の保存
///---------------------------------------------------------------------------

    public bool frameSavePlaceParam()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;
        /// 初期化
        if( nowSubTask == 0 ){
            SetupObjPlaceData.Save();
            nowSubTask ++;
        }

        if( pad.Trig != 0 ){
            changeTask( debugMenuTaskId.MenuTop );
        }
        return true;
    }
    public bool renderSavePlaceParam()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.AddSprite( "Mess", "Save OK !", 0xff40ff40, 2, 2 );
        DemoGame.Graphics2D.DrawSprites();

        useGraphDev.Graphics.SwapBuffers();

        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        return true;
    }



}

} // namespace
