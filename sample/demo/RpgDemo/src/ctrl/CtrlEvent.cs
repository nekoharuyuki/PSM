/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;


namespace AppRpg {

///***************************************************************************
/// アクタ間のイベント制御
///***************************************************************************
public class CtrlEvent
{

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
    }

    /// 開始
    public bool Start()
    {
        End();
        return true;
    }

    /// 終了
    public void End()
    {
    }

    /// フレーム処理
    public bool Frame()
    {
        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        return true;
    }


    /// イベントの再生
    public void Play( GameActorProduct trgActor, GameActorEventContainer eveCntr )
    {
        if( eveCntr.Num > 0 ){

            GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

            for( int i=0; i<eveCntr.Num; i++ ){

                GameObjProduct trgObj = eveCntr.GetEntryObj( i );

                /// ダメージ
                if( eveCntr.GetEventId( i ) == ActorEventId.Damage ){
                    trgActor.SetEventDamage( trgObj, (Data.AttackTypeId)eveCntr.GetEntryAtb( i ) );
                }

                /// エフェクト再生
                else if( eveCntr.GetEventId( i ) == ActorEventId.Effect ){
                    if( trgObj != null ){
                        ctrlResMgr.CtrlEffect.EntryEffect( (Data.EffTypeId)eveCntr.GetEntryAtb( i ), trgObj );
                    }
                    else{
                        ctrlResMgr.CtrlEffect.EntryEffect( (Data.EffTypeId)eveCntr.GetEntryAtb( i ), eveCntr.GetTrgPos( i ) );
                    }
                }

                /// 相手の方へ向く
                else if( eveCntr.GetEventId( i ) == ActorEventId.LookTrg ){
                    trgActor.SetLookTrgPos( eveCntr.GetTrgPos( i ) );
                }

                /// 相手の方へ振り向く
                else if( eveCntr.GetEventId( i ) == ActorEventId.TurnTrg ){
                    if( trgObj != null ){
                        trgActor.SetEventTurnPos( new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42, trgObj.Mtx.M43 ), eveCntr.GetEntryAtb( i ) );
                    }
                }

                /// スーパーアーマー化
                else if( eveCntr.GetEventId( i ) == ActorEventId.SuperArm ){
                    trgActor.SetEventSuperArm();
                }

                /// 動作キャンセル
                else if( eveCntr.GetEventId( i ) == ActorEventId.MvtCancel ){
                    trgActor.SetEventMvtCancel();
                }
            }
        }

        eveCntr.Clear();
    }

/// private メソッド
///---------------------------------------------------------------------------

}

} // namespace
