/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// ACTOR : 弾基底
///***************************************************************************
public class ActorBulletBase : GameActorProduct
{
    protected GameActorCollManager  moveCollMgr;
    protected GameActorContainer    interfereCntr;
    protected ActorUnitCollMove     calCollMove;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init()
    {
        moveCollMgr = new GameActorCollManager();
        moveCollMgr.Init();

        interfereCntr = new GameActorContainer();
        interfereCntr.Init();

        calCollMove    = new ActorUnitCollMove();
        calCollMove.Init();

        EventCntr = new GameActorEventContainer();
        EventCntr.Init();

        return( DoInit() );
    }

    /// 破棄
    public override void Term()
    {
        DoTerm();

        if( moveCollMgr != null ){
            moveCollMgr.Term();
        }
        if( calCollMove != null ){
            calCollMove.Term();
        }
        if( interfereCntr != null ){
            interfereCntr.Term();
        }
        if( EventCntr != null ){
            EventCntr.Clear();
            EventCntr.Term();
        }

        interfereCntr    = null;
        EventCntr        = null;
        moveCollMgr      = null;
        calCollMove      = null;
    }

    /// 開始
    public override bool Start()
    {
        EventCntr.Clear();
        Enable = DoStart();
        return( Enable );
    }

    /// 終了
    public override void End()
    {
        DoEnd();

        EventCntr.Clear();
        Enable = false;
    }

    /// フレーム
    public override bool Frame()
    {
        return( DoFrame() );
    }

    /// 描画
    public override bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        return( DoDraw(graphDev) );
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 移動衝突パラメータの取得
    public GameActorCollManager GetMoveCollManager()
    {
        return moveCollMgr;
    }

    /// 干渉するアクタのコンテナを取得
    public GameActorContainer GetInterfereCntr()
    {
        return interfereCntr;
    }


/// 仮想メソッド
///---------------------------------------------------------------------------

    public virtual bool DoInit()
    {
        return true;
    }
    public virtual void DoTerm()
    {
    }
    public virtual bool DoStart()
    {
        return true;
    }
    public virtual void DoEnd()
    {
    }
    public virtual bool DoFrame()
    {
        return true;
    }
    public virtual bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        return true;
    }


/// プロパティ
///---------------------------------------------------------------------------

}

} // namespace
