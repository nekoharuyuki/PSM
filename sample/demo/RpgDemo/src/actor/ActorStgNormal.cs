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
/// ACTOR : 通常ステージの操作
///***************************************************************************
public class ActorStgNormal : ActorStgBase
{

    private ObjStage        objStage;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        objStage = new ObjStage();
        objStage.Init();

        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        objStage.Term();
        objStage    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        objStage.Start();

        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        objStage.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        objStage.Frame();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objStage.Draw( graphDev );
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objStage;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

}

} // namespace
