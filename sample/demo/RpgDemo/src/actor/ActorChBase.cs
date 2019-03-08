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
/// ACTOR : キャラクター操作基底
///***************************************************************************
public class ActorChBase : GameActorProduct
{

    ///---------------------------------------------------------------------------
    /// キャラクターの動作ID
    ///---------------------------------------------------------------------------
    public enum StateId{
        Stand = 0,
        Turn,
        Move,
        Attack,
        Damage,
        Dead,
        Jump,
        Victory,
    }

    protected StateId    stateIsPlayId;
    protected StateId    stateBackPlayId;
    protected int        statePlayTask;
    
    protected int        hpNow;

    /// 状態パラメータ
    protected Vector3    moveVec;
    protected float      movePow;
    protected float      moveTurn;
    protected bool       moveDashFlg;
    protected Data.AttackTypeId       attackType;

    protected ActorUnitCommon         unitCmnPlay;
    protected GameActorCollManager    moveCollMgr;
    protected GameActorContainer      interfereCntr;
    protected ActorUnitMvtHandle      mvtHdl;
    protected Data.ChTypeId           chTypeId;

    protected GameObjProduct          dmgTrgObj;



/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init()
    {
        unitCmnPlay = new ActorUnitCommon();
        unitCmnPlay.Init();

        moveCollMgr = new GameActorCollManager();
        moveCollMgr.Init();

        interfereCntr = new GameActorContainer();
        interfereCntr.Init();

        mvtHdl        = new ActorUnitMvtHandle();
        mvtHdl.Init();

        EventCntr = new GameActorEventContainer();
        EventCntr.Init();

        return( DoInit() );
    }

    /// 破棄
    public override void Term()
    {
        DoTerm();

        if( mvtHdl != null ){
            mvtHdl.Term();
        }
        if( moveCollMgr != null ){
            moveCollMgr.Term();
        }
        if( unitCmnPlay != null ){
            unitCmnPlay.Term();
        }
        if( EventCntr != null ){
            EventCntr.Clear();
            EventCntr.Term();
        }
        if( interfereCntr != null ){
            interfereCntr.Term();
        }

        interfereCntr = null;
        EventCntr     = null;
        mvtHdl        = null;
        moveCollMgr   = null;
        unitCmnPlay   = null;
        dmgTrgObj     = null;    
    }

    /// 開始
    public override bool Start()
    {
        stateIsPlayId        = StateId.Stand;
        SetStateStand();

        unitCmnPlay.Start( this, interfereCntr, moveCollMgr );
        EventCntr.Clear();

        Enable = DoStart();
        return( Enable );
    }

    /// 終了
    public override void End()
    {
        mvtHdl.End();

        DoEnd();
        unitCmnPlay.End();

        EventCntr.Clear();

        dmgTrgObj = null;    
        Enable = false;
    }

    /// フレーム
    public override bool Frame()
    {
        if( !Enable ){
            return Enable;
        }
        return( DoFrame() );
    }

    /// 描画
    public override bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        return( DoDraw(graphDev) );
    }




/// 状態セット
///---------------------------------------------------------------------------

    /// 状態の切り替え
    public bool ChangeState( StateId id )
    {
        if( DoChangeState( id ) == true ){
            stateBackPlayId      = stateIsPlayId;
            stateIsPlayId        = id;
            statePlayTask        = 0;
            return true;
        }
        return false;
    }

    /// 移動
    public void SetStateStand()
    {
        moveTurn        = 0.0f;
        ChangeState( StateId.Stand );
    }

    /// 旋回
    public void SetStateTurn( float turn )
    {
        moveTurn        = turn;

        if( stateIsPlayId == StateId.Stand ){
            ChangeState( StateId.Turn );
        }
    }

    /// 移動
    public void SetStateMove( Vector3 vec, float move, float turn, bool dashFlg )
    {
        moveVec         = vec;
        movePow         = move;
        moveTurn        = turn;
        moveDashFlg     = dashFlg;

        ChangeState( StateId.Move );
    }

    /// ジャンプ
    public void SetStateJump()
    {
        ChangeState( StateId.Jump );
    }

    /// ジャンプ移動
    public void SetStateJumpMove( Vector3 vec, float move, float turn )
    {
        moveVec         = vec;
        movePow         = move;
        moveTurn        = turn;
    }

    /// 攻撃
    public void SetStateAttack( Data.AttackTypeId type )
    {
        attackType = type;
        ChangeState( StateId.Attack );
    }

    /// 戦闘勝利
    public void SetStateVictory()
    {
        ChangeState( StateId.Victory );
    }



/// public メソッド
///---------------------------------------------------------------------------

    /// 移動衝突パラメータの取得
    public GameActorCollManager GetMoveCollManager()
    {
        return moveCollMgr;
    }

    /// 現在のステートIDを返す
    public StateId GetStateId()
    {
        return stateIsPlayId;
    }

    /// 基本向きを取得
    public Vector3 GetRot()
    {
        return unitCmnPlay.Rot;
    }

    /// キャラタイプIDの取得
    public Data.ChTypeId GetChTypeId()
    {
        return chTypeId;
    }

    /// 干渉するアクタのコンテナを取得
    public GameActorContainer GetInterfereCntr()
    {
        return interfereCntr;
    }

    /// 現在のHPを取得する
    public int GetHp()
    {
        return hpNow;
    }

    /// 地面の属性チェック
    public int GetTouchGroundType()
    {
        return unitCmnPlay.GetTouchGroundType();
    }



/// 仮想メソッド
///---------------------------------------------------------------------------

    protected virtual bool DoInit()
    {
        return true;
    }
    protected virtual void DoTerm()
    {
    }
    protected virtual bool DoStart()
    {
        return true;
    }
    protected virtual void DoEnd()
    {
    }
    protected virtual bool DoFrame()
    {
        return true;
    }
    protected virtual bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        return true;
    }

    protected virtual bool DoChangeState( StateId id )
    {
        return true;
    }

/// プロパティ
///---------------------------------------------------------------------------

}

} // namespace
