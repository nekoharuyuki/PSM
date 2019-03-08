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
/// ACTOR : 武器操作基底
///***************************************************************************
public class ActorWepBase : GameActorProduct
{

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
        Enable = DoStart();
        return( Enable );
    }

    /// 破棄
    public override void End()
    {
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
