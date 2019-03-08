/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {


///***************************************************************************
/// 動作再生
///***************************************************************************
public class ActorUnitMvtActPlayer
{
    private ActorUnitCommon          useUnitCmn;
    private Data.CharParamData       useChParam;
    private Data.MvtActData          usePlayAct;

    private float                    actFrame;
    private float                    actFrameMax;
    private int                      actLoopCnt;



/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        return true;
    }

    /// 破棄
    public void Term()
    {
        usePlayAct    = null;
        useUnitCmn    = null;
        useChParam    = null;
    }


    /// 開始
    public bool Start( ActorUnitCommon useCmn, Data.CharParamData useParam )
    {
        this.useUnitCmn    = useCmn;
        this.useChParam    = useParam;
        playEnd();
        return true;
    }

    /// 終了
    public void End()
    {
        useUnitCmn    = null;
        useChParam    = null;
        usePlayAct    = null;
    }


    /// 動作の再生セット
    public bool SetPlay( int actId, int loopCnt )
    {
        if( actId < 0 ){
            return false;
        }

        usePlayAct      = useChParam.GetMvtAct( actId );

        actFrame        = 0;
        actFrameMax     = 0;
        actLoopCnt      = loopCnt;
        return true;
    }


    /// 移動先のセット
    public bool Frame()
    {
        if( usePlayAct == null ){
            return false;
        }

        frameAction();
        frameUpdate();

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 再生終了
    private void playEnd()
    {
        usePlayAct    = null;
    }

    /// フレームのアクション命令制御
    private void frameAction()
    {
        int actCmdMax = usePlayAct.GetCommandNum();
        for( int i=0; i<actCmdMax; i++ ){

            /// 対象のフレーム内なら命令受付
            if( actFrame >= usePlayAct.GetStartFrame( i ) && actFrame <= usePlayAct.GetEndFrame( i ) ){
                cmdActProc( i );
            }
        }
    }

    /// フレーム更新
    private void frameUpdate()
    {
        actFrame ++;

        /// 終了判定
        if( actFrame >= actFrameMax ){

            /// ループ継続
            if( actLoopCnt > 0 ){
                actLoopCnt --;
                actFrame    = 0;
            }

            /// 無限ループ
            else if( actLoopCnt < 0 ){
                actFrame    = 0;
            }

            /// 終了
            else{
                playEnd();
            }
        }
    }


/// 命令処理
///---------------------------------------------------------------------------

    /// 命令受付分岐
    private void cmdActProc( int cmdIdx )
    {
        Data.ChMvtActCmdId cmdId = (Data.ChMvtActCmdId) usePlayAct.GetCommondId(cmdIdx);

        switch( cmdId ){
        case Data.ChMvtActCmdId.Animation:       cmdActSetAnimation( cmdIdx );       break;
        case Data.ChMvtActCmdId.Attack:          cmdActSetAttack( cmdIdx );          break;
        case Data.ChMvtActCmdId.SePlay:          cmdActSetSePlay( cmdIdx );          break;
        case Data.ChMvtActCmdId.LookNearTrg:     cmdActSetLookNearTrg( cmdIdx );     break;
        case Data.ChMvtActCmdId.TurnNearTrg:     cmdActSetTurnNearTrg( cmdIdx );     break;
        case Data.ChMvtActCmdId.EffPlay:         cmdActSetEffPlay( cmdIdx );         break;
        case Data.ChMvtActCmdId.SuperArm:        cmdActSetSuperArm( cmdIdx );        break;
        case Data.ChMvtActCmdId.MvtCancel:       cmdActSetMvtCancel( cmdIdx );       break;
        }
    }


    /// 命令：アニメーションのセット
    /**
     *  ATB[0]    : 再生するアニメーションNo.
     *  ATB[1]    : アニメーションをループするかのフラグ
     *  ATB[2]    : アクタに含まれる対象Objの登録No.
     **/
    private void cmdActSetAnimation( int cmdIdx )
    {
        int     animNo      = usePlayAct.GetAtb( cmdIdx, 0 );
        bool    animLoopFlg = (usePlayAct.GetAtb( cmdIdx, 1 )==0)?    false : true;
        int     trgObjId    = usePlayAct.GetAtb( cmdIdx, 2 );

        GameActorProduct    trgActor = useUnitCmn.GetUseActor();
        if( trgActor.GetUseObjNum() <= 0 ){
            return ;
        }

        Common.ModelHandle mdlHdl = trgActor.GetUseObj( trgObjId ).GetModelHdl();
        if( mdlHdl == null ){
            return ;
        }

        mdlHdl.SetPlayAnim( animNo, animLoopFlg );

        /// アニメーションの長さが終了フレームとする
        actFrameMax    = mdlHdl.GetAnimLength();
    }


    /// 命令：攻撃のセット
    /**
     *  ATB[0]    : 攻撃タイプ
     *  ATB[1]    : 攻撃対象との有効角度R
     *  ATB[2]    : 攻撃対象との有効角度L
     *  ATB[3]    : 攻撃対象との有効距離
     **/
    private void cmdActSetAttack( int cmdIdx )
    {
        int      attackType        = usePlayAct.GetAtb( cmdIdx, 0 );
        float    trgRotR           = (float) usePlayAct.GetAtb( cmdIdx, 1 );
        float    trgRotL           = (float) usePlayAct.GetAtb( cmdIdx, 2 );
        float    trgDis            = (float) usePlayAct.GetAtb( cmdIdx, 3 ) / 100.0f;;

        GameActorContainer  interfereCntr   = useUnitCmn.GetInterfereCntr();
        GameActorProduct    trgActor        = useUnitCmn.GetUseActor();

        /// 攻撃イベントを通達
        for( int i=0; i<interfereCntr.Num; i++ ){
            if( interfereCntr.GetActor(i).EventCntr != null ){
                if( attackAreaCheck( trgActor, interfereCntr.GetActor(i), trgRotR, trgRotL, trgDis ) ){
                    interfereCntr.GetActor(i).EventCntr.Add( ActorEventId.Damage, attackType, trgActor.GetUseObj(0) );
                }
            }
        }
    }


    /// 命令：SE再生
    /**
     *  ATB[0]    : SEのNo.
     **/
    private void cmdActSetSePlay( int cmdIdx )
    {
        int                    seId    = usePlayAct.GetAtb( cmdIdx, 0 );

        GameActorProduct    trgActor   = useUnitCmn.GetUseActor();

        AppSound.GetInstance().PlaySeCamDis( (AppSound.SeId)seId, trgActor.BasePos );
    }



    /// 命令：近くの対象の方向を向く
    /**
     *  ATB[0]    : 旋回する対象との有効距離
     **/
    private void cmdActSetLookNearTrg( int cmdIdx )
    {
        float    trgDis            = (float) usePlayAct.GetAtb( cmdIdx, 0 ) / 100.0f;

        GameActorContainer    interfereCntr   = useUnitCmn.GetInterfereCntr();
        GameActorProduct      trgActor        = useUnitCmn.GetUseActor();

        int  bestTrg;
        float dis, disMax;
        bool entryFlg;

        bestTrg = -1;
        disMax = 0.0f;
        entryFlg = false;

        for( int i=0; i<interfereCntr.Num; i++ ){

            if( interfereCntr.GetActor(i).EventCntr != null ){

                dis = Common.VectorUtil.Distance( trgActor.BasePos, interfereCntr.GetActor(i).BasePos );

                if( dis <= trgDis && (entryFlg == false || dis < disMax) ){
                    bestTrg     = i;
                    disMax      = dis;
                    entryFlg    = true;
                }
            }
        }

        /// 一番近くの対象の方向へ振り向く
        if( bestTrg >= 0 ){
            trgActor.EventCntr.Add( ActorEventId.LookTrg, 0, interfereCntr.GetActor(bestTrg).BasePos );
        }
    }


    /// 命令：近くの対象の方向を振り向く
    /**
     *  ATB[0]    : 旋回する対象との有効距離
     *  ATB[1]    : 旋回速度
     **/
    private void cmdActSetTurnNearTrg( int cmdIdx )
    {
        float    trgDis            = (float) usePlayAct.GetAtb( cmdIdx, 0 ) / 100.0f;
        int      rotVal            = usePlayAct.GetAtb( cmdIdx, 1 );

        GameActorContainer  interfereCntr   = useUnitCmn.GetInterfereCntr();
        GameActorProduct    trgActor        = useUnitCmn.GetUseActor();

        int  bestTrg;
        float dis, disMax;
        bool entryFlg;

        bestTrg = -1;
        disMax = 0.0f;
        entryFlg = false;

        for( int i=0; i<interfereCntr.Num; i++ ){

            if( interfereCntr.GetActor(i).EventCntr != null ){

                dis = Common.VectorUtil.Distance( trgActor.BasePos, interfereCntr.GetActor(i).BasePos );

                if( dis <= trgDis && (entryFlg == false || dis < disMax) ){
                    bestTrg     = i;
                    disMax      = dis;
                    entryFlg    = true;
                }
            }
        }

        /// 一番近くの対象の方向へ振り向く
        if( bestTrg >= 0 ){
            trgActor.EventCntr.Add( ActorEventId.TurnTrg, rotVal, interfereCntr.GetActor(bestTrg).GetUseObj(0) );
        }
    }


    /// 命令：エフェクト再生
    /**
     *  ATB[0]    : EffのNo.
     *  ATB[1]    : くっつける対象のOBJのIndex
     **/
    private void cmdActSetEffPlay( int cmdIdx )
    {
        int        effId    = usePlayAct.GetAtb( cmdIdx, 0 );
        int        objIdx   = usePlayAct.GetAtb( cmdIdx, 1 );
        
        GameActorProduct    trgActor        = useUnitCmn.GetUseActor();

        trgActor.EventCntr.Add( ActorEventId.Effect, effId, trgActor.GetUseObj(objIdx) );
    }


    /// 命令：スーパーアーマー化
    /**
     **/
    private void cmdActSetSuperArm( int cmdIdx )
    {
        GameActorProduct    trgActor        = useUnitCmn.GetUseActor();
        trgActor.EventCntr.Add( ActorEventId.SuperArm, 0, null );
    }


    /// 命令：動作キャンセル受付
    /**
     **/
    private void cmdActSetMvtCancel( int cmdIdx )
    {
        GameActorProduct    trgActor        = useUnitCmn.GetUseActor();
        trgActor.EventCntr.Add( ActorEventId.MvtCancel, 0, null );
    }



/// private
///---------------------------------------------------------------------------

    /// 攻撃範囲チェック
    private bool attackAreaCheck( GameActorProduct myActor, GameActorProduct trgActor, float rotR, float rotL, float disArea )
    {
        float dis = Common.VectorUtil.Distance( myActor.BasePos, trgActor.BasePos );

        if( dis <= disArea ){
            float rot = Common.MatrixUtil.GetPointRotY( myActor.BaseMtx, myActor.BasePos, trgActor.BasePos );

            if( rot >= rotR && rot <= rotL ){
                return true;
            }
        }
        return false;
    }

}

} // namespace
