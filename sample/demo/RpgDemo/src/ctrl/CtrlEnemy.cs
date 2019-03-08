/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;


namespace AppRpg {

///***************************************************************************
/// 敵制御
///***************************************************************************
public class CtrlEnemy
{
    private List< ActorChMonster >    actorChList;
    private List< ActorChMonster >    activeList;
    public  float                     EntryAreaDis;
    public  int                       EntryStayMax;
    private ObjEffect                 objShadow;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorChList = new List< ActorChMonster >();
        if( actorChList == null ){
            return false;
        }

        activeList = new List< ActorChMonster >();
        if( activeList == null ){
            return false;
        }

        objShadow = new ObjEffect();
        objShadow.Init();

        EntryAreaDis = 40.0f;
        EntryStayMax = 60;

        return true;
    }

    /// 破棄
    public void Term()
    {
        if( activeList != null ){
            activeList.Clear();
        }
        if( actorChList != null ){
            for( int i=0; i<actorChList.Count; i++ ){
                actorChList[i].Term();
            }
            actorChList.Clear();
        }
        if( objShadow != null ){
            objShadow.Term();
        }

        activeList       = null;
        actorChList      = null;
        objShadow        = null;
    }

    /// 開始
    public bool Start()
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Start();
        }

        objShadow.SetMdlHandle( Data.EffTypeId.Eff10 );
        objShadow.Start();

        return true;
    }

    /// 終了
    public void End()
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].End();
        }
        actorChList.Clear();
        activeList.Clear();
        objShadow.End();
    }


    /// フレーム処理
    public bool Frame()
    {
        setActiveChList();

        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        for( int i=0; i<activeList.Count; i++ ){

            /// 他アクタからのイベントをチェック
            ///-------------------------------------
            if( activeList[i].EventCntr.Num > 0 ){
                ctrlResMgr.CtrlEvent.Play( activeList[i], activeList[i].EventCntr );
            }

            /// フレーム処理
            ///-------------------------------------
            if( activeList[i].GetStateId() == ActorChBase.StateId.Stand || activeList[i].GetStateId() == ActorChBase.StateId.Move ||
                activeList[i].GetStateId() == ActorChBase.StateId.Turn ){
                frameMove( activeList[i] );
            }
            else if( activeList[i].GetStateId() == ActorChBase.StateId.Attack ){
                frameAttack( activeList[i] );
            }
            activeList[i].Frame();


            /// 自身発生のイベントをチェック
            ///-------------------------------------
            if( activeList[i].EventCntr.Num > 0 ){
                ctrlResMgr.CtrlEvent.Play( activeList[i], activeList[i].EventCntr );
            }
        }

        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<activeList.Count; i++ ){
            activeList[i].Draw( graphDev );
        }
    
        return true;
    }

    /// 描画処理
    public bool DrawAlpha( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<activeList.Count; i++ ){

            Matrix4 mtx = activeList[i].BaseMtx;
            mtx.M41 = activeList[i].GetBodyPos().X;
            mtx.M43 = activeList[i].GetBodyPos().Z;
            objShadow.SetMatrix( mtx );
            objShadow.SetScale( activeList[i].GetBodyWidth()/10.0f );

            objShadow.Draw( graphDev );
        }
        return true;
    }


    /// 描画処理（デバック用）
    public bool DrawDebug( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Frame();
            actorChList[i].Draw( graphDev );
        }
    
        return true;
    }


    /// 衝突対象をコンテナへ登録
    public bool SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        for( int i=0; i<activeList.Count; i++ ){
            for( int j=0; j<activeList[i].GetUseObjNum(); j++ ){
                container.Add( activeList[i], activeList[i].GetUseObj(j) );
            }
        }

        return true;
    }

    /// 移動目標対象をコンテナへ登録
    public bool SetDestinationActor( GameActorCollObjContainer container )
    {
        for( int i=0; i<activeList.Count; i++ ){
            for( int j=0; j<activeList[i].GetUseObjNum(); j++ ){
                container.Add( activeList[i], activeList[i].GetUseObj(j) );
            }
        }

        return true;
    }


    /// 外部から割り込みを受ける対象をコンテナへ登録
    public bool SetInterfereActor( GameActorContainer container )
    {
        for( int i=0; i<activeList.Count; i++ ){
            container.Add( activeList[i] );
        }
        return true;
    }


    /// ベースOBJの取得
    public GameObjProduct GetUseActorBaseObj( int idx )
    {
        return actorChList[idx].GetUseObj(0);
    }

    /// 敵の登録
    public void EntryAddEnemy( int chResId, float rotY, Vector3 pos )
    {
        ActorChMonster actorCh = new ActorChMonster();
        actorCh.Init();
        actorCh.Start();
        actorCh.SetMdlHandle( (Data.ChTypeId)chResId );

        actorChList.Add( actorCh );

        SetPlace( (actorChList.Count-1), rotY, pos );
    }

    /// 敵の登録削除
    public void DeleteEntryEnemy( int idx )
    {
        actorChList.RemoveAt( idx );
    }

    /// 敵の配置
    public void SetPlace( int idx, float rotY, Vector3 pos )
    {
        float a_Cal     = (float)(3.141593f / 180.0);
        float angleY    = rotY * a_Cal;

        Matrix4 mtx = Matrix4.RotationY( angleY );
        Common.MatrixUtil.SetTranslate( ref mtx, pos );

        actorChList[idx].SetPlace( mtx );
    }

    /// 敵の座標を取得
    public Vector3 GetPos( int idx )
    {
        return new Vector3( actorChList[idx].BaseMtx.M41, actorChList[idx].BaseMtx.M42, actorChList[idx].BaseMtx.M43 );
    }
    /// 敵の向きを取得
    public float GetRotY( int idx )
    {
        float angleY = (float)Math.Atan2( actorChList[idx].BaseMtx.M31, actorChList[idx].BaseMtx.M33 );
        float a_Cal  = (float)(angleY / (3.141593f / 180.0));
        return a_Cal;
    }
    /// 登録タイプの取得
    public int GetChTypeId( int idx )
    {
        return (int)actorChList[idx].GetChTypeId();
    }
    /// 登録数の取得
    public int GetEntryNum()
    {
        return actorChList.Count;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// フレーム：移動
    private bool frameMove( ActorChMonster actorCh )
    {
        Vector3 plPos = GameCtrlManager.GetInstance().CtrlPl.GetPos();
        float rot = Common.MatrixUtil.GetPointRotY( actorCh.BaseMtx, actorCh.BasePos, plPos );

        if( actorCh.AiAttackCnt <= 0 ){
            if( actorCh.ActiveDis < 10.0f && rot > -20.0f && rot < 20.0f ){
                actorCh.SetStateAttack( Data.AttackTypeId.Normal );
                actorCh.AiAttackCnt = Common.Rand.Get()%50 + 5;
                return true;
            }
        }
        else{
            actorCh.AiAttackCnt --;
        }

        if( actorCh.ActiveDis < 10.0f && (rot > 10.0f || rot < -10.0f) ){
            actorCh.SetStateTurn( rot );
            return true;
        }

        actorCh.SetStateStand();
        return true;
    }


    /// フレーム：攻撃
    private bool frameAttack( ActorChMonster actorCh )
    {
        GameCtrlManager.GetInstance().SetInterfereActorEn( actorCh.GetInterfereCntr() );
        return true;
    }


    /// アクティブなキャラクタのセット
    private void setActiveChList()
    {
        Vector3 plPos = GameCtrlManager.GetInstance().CtrlPl.GetPos();

        activeList.Clear();
        for( int i=0; i<actorChList.Count; i++ ){

            /// 登録削除
            ///-------------------------------------
            if( !actorChList[i].Enable ){
                actorChList[i].End();
                actorChList.RemoveAt(i);     // 要素の削除
                i --;
                continue ;
            }

            if( actorChList[i].ActiveFlg ){
                actorChList[i].ActiveCnt --;
                if( actorChList[i].ActiveCnt <= 0 ){
                    actorChList[i].ActiveFlg = false;
                }
                else{
                    activeList.Add( actorChList[i] );
                    continue ;
                }
            }

            if( actorChList[i].ActiveFlg == false ){
                float dis = Common.VectorUtil.Distance( actorChList[i].BasePos, plPos );
                if( dis < EntryAreaDis ){
                    actorChList[i].ActiveFlg    = true;
                    actorChList[i].ActiveCnt    = EntryStayMax;
                    actorChList[i].ActiveDis    = dis;
                    activeList.Add( actorChList[i] );
                }
            }
        }
    }


}

} // namespace
