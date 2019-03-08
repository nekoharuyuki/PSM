/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {


///***************************************************************************
/// 基本的な姿勢の動き制御を行う
///***************************************************************************
public class ActorUnitCommon
{
    /// 列挙子
    private enum PlayId{
        Turn        = (1 << 0),        /// 旋回
        Move        = (1 << 1),        /// 移動
    }


    /// private 変数
    private Matrix4              baseMtx;
    private Vector3              basePos;
    private Vector3              baseRot;
    private PlayId               playId;
    private bool                 isUpdateMtx;

    private Vector3              moveVec;
    private float                movePow;

    private float                rotPow;

    private GameActorProduct     useActor;
    private GameActorContainer   useInterfereCntr;
    private GameActorCollManager useCollMgr;

    private ActorUnitCollMove    calCollMove;
    private ActorUnitCollGravity calCollGrav;



/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        calCollMove    = new ActorUnitCollMove();
        calCollMove.Init();
        calCollGrav    = new ActorUnitCollGravity();
        calCollGrav.Init();
    }

    /// 破棄
    public void Term()
    {
        if( calCollMove != null ){
            calCollMove.Term();
        }
        if( calCollGrav != null ){
            calCollGrav.Term();
        }
        calCollGrav         = null;
        calCollMove         = null;
        useCollMgr          = null;
        useActor            = null;
        useInterfereCntr    = null;
    }

    /// 開始
    public void Start( GameActorProduct useActor, GameActorContainer useInterfereCntr, GameActorCollManager useCollMgr )
    {
        basePos        = new Vector3( 0.0f, 0.0f, 0.0f );
        baseRot        = new Vector3( 0.0f, 0.0f, 0.0f );

        setMtxRotateEulerXYZ( baseRot );

        Common.MatrixUtil.SetTranslate( ref baseMtx, basePos );

        playId        = 0;

        this.useActor            = useActor;
        this.useInterfereCntr    = useInterfereCntr;
        this.useCollMgr          = useCollMgr;
    }

    /// クリア
    public void End()
    {
        playId              = 0;
        useActor            = null;
        useInterfereCntr    = null;
        useCollMgr          = null;
    }

    /// フレーム
    public void Frame()
    {
        isUpdateMtx = false;

        if( (playId & PlayId.Turn) != 0 ){
            frameTurn();
        }
        if( (playId & PlayId.Move) != 0 ){
            frameMove();
        }
    }


    /// 重力処理
    public void FrameGravity()
    {
        if( useCollMgr != null ){
            useCollMgr.MoveShape.SetMult(baseMtx);

            Vector3 movePos = new Vector3(0,0,0);
            calCollGrav.GetMovePos( useCollMgr, ref movePos );

            if( calCollGrav.Check( useCollMgr, movePos ) == true ){
                /// OBJに接地
            }
            basePos = calCollGrav.NextPos;
        }
        Common.MatrixUtil.SetTranslate( ref baseMtx, basePos );

        playId        &= ~PlayId.Move;
        isUpdateMtx    = true;
    }



    /// 旋回
    public void SetTurn( float rot, int endFrame )
    {
        this.rotPow             = rot;
        this.playId            |= PlayId.Turn;
    }

    /// 指定の向きに向く
    public void SetRot( float rot )
    {
        this.rotPow            = 0.0f;
        this.baseRot.Y         = rot;
        this.playId           |= PlayId.Turn;
    }

    /// 移動
    public void SetMove( Vector3 vec, float pow )
    {
        this.moveVec         = vec;
        this.movePow         = pow;
        this.playId         |= PlayId.Move;
    }

    /// 配置
    public void SetPlace( Matrix4 mtx )
    {
        baseMtx            = mtx;
        Common.VectorUtil.Set( ref basePos, baseMtx );

        float angleY = (float)Math.Atan2( mtx.M31, mtx.M33 );
        this.baseRot.Y    = (float)(angleY / (3.141593f / 180.0));

        isUpdateMtx       = true;
    }

    /// 姿勢の更新があるかの確認
    public bool IsUpdateMtx()
    {
        return isUpdateMtx;
    }

    /// 制御対象のアクターを返す
    public GameActorProduct GetUseActor()
    {
        return useActor;
    }

    /// 干渉するアクターのコンテナを返す
    public GameActorContainer GetInterfereCntr()
    {
        return useInterfereCntr;
    }

    /// 地面を踏んでいるかチェック
    public bool CheckTouchGround()
    {
        return calCollGrav.CheckTouchGround();
    }

    /// 地面の属性を取得
    public int GetTouchGroundType()
    {
        return calCollGrav.GetTouchGroundType();
    }

    /// 地面の情報を初期化
    public void ResetGroudParam()
    {
        calCollGrav.ResetGroudParam();
    }


/// private メソッド
///---------------------------------------------------------------------------

    /// フレーム処理：旋回
    private void frameTurn()
    {
        baseRot.Y += rotPow;
        setMtxRotateEulerXYZ( baseRot );

        playId        &= ~PlayId.Turn;
        isUpdateMtx    = true;
    }

    /// フレーム処理：移動
    private void frameMove()
    {

        if( useCollMgr != null ){

            useCollMgr.MoveShape.SetMult(baseMtx);
            Vector3 movePos = new Vector3(0,0,0);
            calCollMove.GetMovePos( useCollMgr, calCollGrav.TreadVec, (moveVec * movePow), ref movePos );

            for( int checkCnt=0; checkCnt<4; checkCnt++ ){
                if( calCollMove.Check( useCollMgr, movePos ) == true ){
                    movePos = calCollMove.NextPos;
                }
                else{
                    basePos = calCollMove.NextPos;
                    break;
                }
            }
        }
        else{
            basePos    = (moveVec * movePow) + basePos;
        }

        Common.MatrixUtil.SetTranslate( ref baseMtx, basePos );

        playId        &= ~PlayId.Move;
        isUpdateMtx    = true;
    }



    /// 行列にXYZ回転をかける
    private void setMtxRotateEulerXYZ( Vector3 rot )
    {
        float a_Cal       = (float)(3.141593f / 180.0);
        Vector3 calRot    = new Vector3( rot.X * a_Cal, rot.Y * a_Cal, rot.Z * a_Cal );

        Common.MatrixUtil.SetMtxRotateEulerXYZ( ref baseMtx, calRot );
        Common.MatrixUtil.SetTranslate( ref baseMtx, basePos );
    }



/// プロパティ
///---------------------------------------------------------------------------

    /// 姿勢
    public Matrix4 Mtx
    {
        get {return baseMtx;}
    }

    /// 座標
    public Vector3 Pos
    {
        get {return basePos;}
    }

    /// 向き
    public Vector3 Rot
    {
        get {return baseRot;}
    }
}

} // namespace
