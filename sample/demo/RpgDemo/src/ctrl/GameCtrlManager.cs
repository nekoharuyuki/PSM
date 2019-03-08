/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// シングルトン：CTRL間にて使用するリソースの管理
///***************************************************************************
public class GameCtrlManager
{

    private static GameCtrlManager instance = new GameCtrlManager();

    private DemoGame.GraphicsDevice        graphDev;

    private CtrlPlayer       ctrlPl;
    private CtrlEnemy        ctrlEn;
    private CtrlCamera       ctrlCam;
    private CtrlStage        ctrlStg;
    private CtrlFixture      ctrlFix;
    private CtrlEffect       ctrlEffect;
    private CtrlBullet       ctrlBullet;
    private CtrlEvent        ctrlEvent;
    private float            nowFps;
    private float            nowMs;



    /// コンストラクタ
    private GameCtrlManager()
    {
    }

    /// インスタンスの取得
    public static GameCtrlManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        ctrlPl = new CtrlPlayer();
        ctrlPl.Init();

        ctrlEn = new CtrlEnemy();
        ctrlEn.Init();

        ctrlCam = new CtrlCamera();
        ctrlCam.Init();

        ctrlStg = new CtrlStage();
        ctrlStg.Init();

        ctrlFix = new CtrlFixture();
        ctrlFix.Init();

        ctrlEffect = new CtrlEffect();
        ctrlEffect.Init();

        ctrlBullet = new CtrlBullet();
        ctrlBullet.Init();
        
        ctrlEvent = new CtrlEvent();
        ctrlEvent.Init();

        GameCtrlDrawManager.GetInstance().Init();

        nowFps = 0.0f;
        return true;
    }

    /// 破棄
    public void Term()
    {
        GameCtrlDrawManager.GetInstance().Term();

        ctrlPl.Term();
        ctrlEn.Term();
        ctrlCam.Term();
        ctrlStg.Term();
        ctrlFix.Term();
        ctrlEffect.Term();
        ctrlBullet.Term();
        ctrlEvent.Term();

        ctrlPl        = null;
        ctrlEn        = null;
        ctrlCam       = null;
        ctrlStg       = null;
        ctrlFix       = null;
        ctrlEffect    = null;
        ctrlEvent     = null;
        ctrlBullet    = null;
        graphDev      = null;
    }


    /// 開始
    public bool Start()
    {
        ctrlPl.Start();
        ctrlEn.Start();
        ctrlCam.Start();
        ctrlStg.Start();
        ctrlFix.Start();
        ctrlEffect.Start();
        ctrlBullet.Start();
        ctrlEvent.Start();

        GameCtrlDrawManager.GetInstance().Start();
        return true;
    }


    /// 終了
    public void End()
    {
        ctrlPl.End();
        ctrlEn.End();
        ctrlCam.End();
        ctrlStg.End();
        ctrlFix.End();
        ctrlEffect.End();
        ctrlBullet.End();
        ctrlEvent.End();
    }


    /// 使用するグラフィクスデバイスの登録
    public void SetGraphicsDevice( DemoGame.GraphicsDevice use )
    {
        graphDev    = use;
    }


    /// FPS
    public void SetFps( float fps )
    {
        nowFps    = fps;
    }
    public float GetFps()
    {
        return nowFps;
    }

    /// ミリ秒
    public void SetMs( float fps )
    {
        nowMs    = fps;
    }
    public float GetMs()
    {
        return nowMs;
    }


    /// 全コントロールのUpdate
    public void Frame()
    {
        ctrlPl.Frame();
        ctrlEn.Frame();
        ctrlCam.Frame();
        ctrlStg.Frame();
        ctrlFix.Frame();
        ctrlEffect.Frame();
        ctrlBullet.Frame();
        ctrlEvent.Frame();
    }


    /// 全コントロールの描画
    public void Draw()
    {
        ctrlCam.Draw( graphDev );

        /// 不透明の物を描画
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlEn.Draw( graphDev );
        ctrlFix.Draw( graphDev );

        ctrlStg.DrawDestinationMark( graphDev );
        ctrlPl.Draw( graphDev );

        GameCtrlDrawManager.GetInstance().SortNear();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlStg.Draw( graphDev );


        /// 半透明の物は奥から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlPl.DrawAlpha( graphDev );
        ctrlEn.DrawAlpha( graphDev );
        ctrlEffect.Draw( graphDev );
        ctrlBullet.Draw( graphDev );
        GameCtrlDrawManager.GetInstance().SortFar();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlEvent.Draw( graphDev );
    }


    /// タイトル用Update
    public void FrameTitle()
    {
        ctrlCam.FrameTitle();
        ctrlStg.FrameTitle();
        ctrlFix.Frame();
    }


    /// タイトル用の描画
    public void DrawTitle()
    {
        ctrlCam.Draw( graphDev );

        /// 不透明の物は手前から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlFix.Draw( graphDev );
        GameCtrlDrawManager.GetInstance().SortNear();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlStg.Draw( graphDev );
    }


    /// デバック用の描画
    public void DrawDebug()
    {
        ctrlCam.Draw( graphDev );

        /// 不透明の物は手前から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlEn.DrawDebug( graphDev );
        ctrlFix.Draw( graphDev );
        ctrlPl.Draw( graphDev );

        GameCtrlDrawManager.GetInstance().SortNear();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlStg.Draw( graphDev );

        /// 半透明の物は奥から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlPl.DrawAlpha( graphDev );
        ctrlEn.DrawAlpha( graphDev );
        ctrlEffect.Draw( graphDev );
        GameCtrlDrawManager.GetInstance().SortFar();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlEvent.Draw( graphDev );
    }


    /// リザルト用Update
    public void FrameResult()
    {
        ctrlPl.FrameResult();
        ctrlEn.Frame();
        ctrlCam.FrameResult();
        ctrlStg.FrameResult();
        ctrlFix.Frame();
        ctrlEffect.Frame();
        ctrlBullet.Frame();
        ctrlEvent.Frame();

    }


    /// 衝突対象をコンテナにセット
    public void SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        container.Clear();
        ctrlStg.SetCollisionActor( container, trgPos );
        ctrlFix.SetCollisionActor( container, trgPos );
        ctrlEn.SetCollisionActor( container, trgPos );
    }


    /// プレイヤーの干渉対象をコンテナにセット
    public void SetInterfereActorPl( GameActorContainer container )
    {
        container.Clear();
        ctrlStg.SetInterfereActor( container );
        ctrlFix.SetInterfereActor( container );
        ctrlEn.SetInterfereActor( container );
    }

    /// 敵の干渉対象をコンテナにセット
    public void SetInterfereActorEn( GameActorContainer container )
    {
        container.Clear();
        ctrlPl.SetInterfereActor( container );
    }



/// プロパティ
///---------------------------------------------------------------------------

    /// 
    public DemoGame.GraphicsDevice GraphDev
    {
        get {return graphDev;}
    }

    /// 
    public CtrlPlayer CtrlPl
    {
        get {return ctrlPl;}
    }
    public CtrlEnemy CtrlEn
    {
        get {return ctrlEn;}
    }
    public CtrlCamera CtrlCam
    {
        get {return ctrlCam;}
    }
    public CtrlStage CtrlStg
    {
        get {return ctrlStg;}
    }
    public CtrlFixture CtrlFix
    {
        get {return ctrlFix;}
    }
    public CtrlEffect CtrlEffect
    {
        get {return ctrlEffect;}
    }
    public CtrlBullet CtrlBullet
    {
        get {return ctrlBullet;}
    }
    public CtrlEvent CtrlEvent
    {
        get {return ctrlEvent;}
    }
}

} // namespace
