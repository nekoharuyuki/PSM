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
/// ACTOR : 配置されているだけの備品
///***************************************************************************
public class ActorFixNormal : ActorFixBase
{


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    protected override bool DoInit()
    {
        objFix = new ObjFixNormal();
        objFix.Init();
        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        objFix.Term();
        objFix    = null;
    }

    /// 開始
    protected override bool DoStart()
    {
        objFix.Start();
        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        objFix.End();
    }

    /// フレーム処理
    protected override bool DoFrame()
    {
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        Visible = objFix.DrawFlg;
        objFix.Draw( graphDev );
        return true;
    }




/// private メソッド
///---------------------------------------------------------------------------

}

} // namespace
