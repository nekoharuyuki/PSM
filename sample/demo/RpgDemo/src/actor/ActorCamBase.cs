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
/// ACTOR : カメラ操作基底
///***************************************************************************
public class ActorCamBase : GameActorProduct
{

    protected ObjCamera        objCam;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init()
    {
        objCam = new ObjCamera();
        objCam.Init();

        return( DoInit() );
    }

    /// 破棄
    public override void Term()
    {
        DoTerm();

        objCam.Term();
        objCam    = null;
    }

    /// 開始
    public override bool Start()
    {
        objCam.Start();
        Enable = DoStart();
        return( Enable );
    }

    /// 終了
    public override void End()
    {
        DoEnd();
        objCam.End();
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

    /// OBJ
    public ObjCamera Obj
    {
        get {return objCam;}
    }

}

} // namespace
