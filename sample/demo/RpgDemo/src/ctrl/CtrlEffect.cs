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
/// エフェクト制御
///***************************************************************************
public class CtrlEffect
{
    private List< ActorEffBase >    actorEffList;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorEffList = new List< ActorEffBase >();
        if( actorEffList == null ){
            return false;
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        if( actorEffList != null ){
            for( int i=0; i<actorEffList.Count; i++ ){
                actorEffList[i].Term();
            }
            actorEffList.Clear();
        }
        actorEffList        = null;
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
        for( int i=0; i<actorEffList.Count; i++ ){
            actorEffList[i].End();
        }
        actorEffList.Clear();
    }


    /// フレーム処理
    public bool Frame()
    {
        for( int i=0; i<actorEffList.Count; i++ ){

            /// フレーム処理
            ///-------------------------------------
            if( actorEffList[i].Frame() == false ){
                actorEffList[i].End();
            }

            /// 登録削除
            ///-------------------------------------
            if( !actorEffList[i].Enable ){
                actorEffList[i].End();
                actorEffList[i].Term();
                actorEffList.RemoveAt(i);     // 要素の削除
                i --;
            }
        }
        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorEffList.Count; i++ ){
            if( actorEffList[i].Enable == true ){
                GameCtrlDrawManager.GetInstance().EntryEffect( actorEffList[i], true );
            }
        }

        return true;
    }


    /// 通常エフェクトの登録
    public bool EntryEffect( Data.EffTypeId effId, Vector3 pos )
    {
        ActorEffNormal actorEff = new ActorEffNormal();

        actorEff.Init();
        actorEff.Start();
        actorEff.SetMdlHandle( effId );

        Matrix4 mtx = Matrix4.RotationY( 0 );
        Common.MatrixUtil.SetTranslate( ref mtx, pos );
        actorEff.SetPlace( mtx );

        actorEffList.Add( actorEff );

        return true;
    }

    /// 追従エフェクトの登録
    public bool EntryEffect( Data.EffTypeId effId, GameObjProduct trgObj )
    {
        ActorEffAttach actorEff = new ActorEffAttach();

        actorEff.Init();
        actorEff.Start();

        actorEff.SetMdlHandle( effId );
        actorEff.SetTrgObj( trgObj );

        actorEffList.Add( actorEff );

        return true;
    }





/// private メソッド
///---------------------------------------------------------------------------

}

} // namespace
