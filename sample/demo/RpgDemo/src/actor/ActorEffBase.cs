/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace AppRpg {

///***************************************************************************
/// ACTOR : エフェクト操作基底
///***************************************************************************
public class ActorEffBase : GameActorProduct
{

    protected Data.EffTypeId        effTypeId;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init()
    {
        return( DoInit() );
    }

    /// 破棄
    public override void Term()
    {
        DoTerm();
    }

    /// 開始
    public override bool Start()
    {
        effTypeId = 0;
        Enable = DoStart();
        return( Enable );
    }

    /// 終了
    public override void End()
    {
        effTypeId = 0;
        DoEnd();

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



/// protected メソッド
///---------------------------------------------------------------------------

    /// ビルボード用姿勢セット
    protected void SetBillboardMatrix( DemoGame.Camera camera )
    {
        Common.MatrixUtil.LookTrgVec( ref BaseMtx, (camera.Pos - BasePos) );
        SetPlace( BaseMtx );
    }


    /// Y軸ビルボード用姿勢セット
    protected void SetBillboardMatrixY( DemoGame.Camera camera )
    {
        Vector3 look = (camera.Pos - BasePos);
        look.Y = 0.0f;

        Common.MatrixUtil.LookTrgVec( ref BaseMtx, look );
        SetPlace( BaseMtx );
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
