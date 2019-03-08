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
/// フィールド内を飛び交う弾の制御
///***************************************************************************
public class CtrlBullet
{
    private List< ActorBulletBase >    actorList;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorList = new List< ActorBulletBase >();
        if( actorList == null ){
            return false;
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        if( actorList != null ){
            for( int i=0; i<actorList.Count; i++ ){
                actorList[i].Term();
            }
            actorList.Clear();
        }
        actorList        = null;
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
        for( int i=0; i<actorList.Count; i++ ){
            actorList[i].End();
        }
        actorList.Clear();
    }


    /// フレーム処理
    public bool Frame()
    {
        GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();
            
        for( int i=0; i<actorList.Count; i++ ){

            setMoveCollTrgObj( actorList[i] );
            setInterfereActor( actorList[i] );

            /// フレーム処理
            ///-------------------------------------
            if( actorList[i].Frame() == false ){
                actorList[i].End();
            }

            /// 自身発生のイベントをチェック
            ///-------------------------------------
            if( actorList[i].EventCntr.Num > 0 ){
                ctrlResMgr.CtrlEvent.Play( actorList[i], actorList[i].EventCntr );
            }

            /// 登録削除
            ///-------------------------------------
            if( !actorList[i].Enable ){
                actorList[i].End();
                actorList[i].Term();
                actorList.RemoveAt(i);     // 要素の削除
                i --;
            }
        }
        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorList.Count; i++ ){
            if( actorList[i].Enable == true ){
                GameCtrlDrawManager.GetInstance().EntryEffect( actorList[i], true );
            }
        }

        return true;
    }


    /// 弾の登録
    public bool EntryEffect( Data.BulletTypeId bulletId, Vector3 pos, Vector3 vec )
    {
        ActorBulletFireBall actor = new ActorBulletFireBall();

        actor.Init();
        actor.Start();

		Vector4 x = new Vector4(0,0,0,0);
		Vector4 y = new Vector4(0,0,0,0);
		Vector4 z = new Vector4(0,0,0,0);
		Vector4 w = new Vector4(0,0,0,0);
        Matrix4 mtx = new Matrix4(x ,y, z, w);
        Common.MatrixUtil.LookTrgVec( ref mtx, vec );
        Common.MatrixUtil.SetTranslate( ref mtx, pos );

        actor.SetPlace( mtx );

        actorList.Add( actor );
        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 移動衝突対象OBJの登録
    private void setMoveCollTrgObj( ActorBulletBase actor )
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();
        GameActorCollManager   useCollMgr    = actor.GetMoveCollManager();

        /// 移動する自身のOBJを登録
        useCollMgr.SetMoveShape( actor.GetUseObj(0).GetMoveShape() ); 

        /// 対象を登録
        ctrlResMgr.SetCollisionActor( useCollMgr.TrgContainer, actor.BasePos );
    }

    /// 干渉対象の登録
    private void setInterfereActor( ActorBulletBase actor )
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        /// 対象を登録
        ctrlResMgr.SetInterfereActorPl( actor.GetInterfereCntr() );
    }

}

} // namespace
