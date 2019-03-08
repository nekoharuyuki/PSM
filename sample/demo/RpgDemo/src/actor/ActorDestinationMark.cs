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
/// ACTOR : 目的地マーク
///***************************************************************************
public class ActorDestinationMark : GameActorProduct
{
    private ObjDestinationMark        objMark;
    protected GameActorCollManager    moveCollMgr;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init()
    {
        objMark = new ObjDestinationMark();
        objMark.Init();

        moveCollMgr = new GameActorCollManager();
        moveCollMgr.Init();
        return true;
    }

    /// 破棄
    public override void Term()
    {
        if( moveCollMgr != null ){
            moveCollMgr.Term();
        }
        objMark.Term();

        moveCollMgr   = null;
        objMark       = null;
    }

    /// 開始
    public override bool Start()
    {
        objMark.Start();
        Enable = true;
        return true;
    }

    /// 破棄
    public override void End()
    {
        objMark.End();
        Enable = false;
    }

    /// フレーム
    public override bool Frame()
    {
        objMark.Frame();
        return true;
    }

    /// 描画
    public override bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        if( objMark.TypeId == 0 ){
            SetBillboardMatrix( graphDev.GetCurrentCamera() );
        }
        else{
            SetBillboardMatrixY( graphDev.GetCurrentCamera() );
        }
        objMark.Draw( graphDev );
        return true;
    }

    /// 移動衝突パラメータの取得
    public GameActorCollManager GetMoveCollManager()
    {
        return moveCollMgr;
    }

    /// マークのタイプセット
    public void SetType( int type )
    {
        objMark.SetType( type );
    }


/// private
///---------------------------------------------------------------------------

    /// ビルボード用姿勢セット
    private void SetBillboardMatrix( DemoGame.Camera camera )
    {
        Common.MatrixUtil.LookTrgVec( ref BaseMtx, camera.Pos - BasePos );
        objMark.SetMatrix( BaseMtx );

        Matrix4 mtx = objMark.Mtx;
        mtx.M41 = BaseMtx.M41 + BaseMtx.M31 * 2.0f;
        mtx.M42 = BaseMtx.M42 + BaseMtx.M32 * 2.0f;
        mtx.M43 = BaseMtx.M43 + BaseMtx.M33 * 2.0f;
        objMark.SetMatrix( mtx );
   }

    /// Y軸ビルボード用姿勢セット
    private void SetBillboardMatrixY( DemoGame.Camera camera )
    {
        Vector3 look = camera.Pos - BasePos;
        look.Y = 0.0f;

        Common.MatrixUtil.LookTrgVec( ref BaseMtx, look );
        objMark.SetMatrix( BaseMtx );
    }


/// プロパティ
///---------------------------------------------------------------------------

}

} // namespace
