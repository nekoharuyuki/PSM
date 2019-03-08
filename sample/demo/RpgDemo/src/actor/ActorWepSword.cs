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
/// ACTOR : 剣の操作
///***************************************************************************
public class ActorWepSword : ActorWepBase
{
    private ObjEqpSword      objSword;
    private ObjChHero        objTrg;
    private int              boneId;

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        objSword = new ObjEqpSword();
        objSword.Init();

        objTrg = null;
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        objSword.Term();
        objSword    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        objSword.Start();

        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        objSword.End();
        objTrg = null;
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        objSword.Frame();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        if( objTrg != null ){

            BaseMtx        = objTrg.GetBoneMatrix( boneId );
            Common.VectorUtil.Set( ref BasePos, BaseMtx );

            objSword.SetMatrix( BaseMtx );

            objSword.Draw( graphDev );
        }
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objSword;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

/// public メソッド
///---------------------------------------------------------------------------

    /// 追従対象のOBJをセット
    public void SetTrgObj( ObjChHero trgObj, int boneId )
    {
        this.objTrg        = trgObj;
        this.boneId        = boneId;
    }

}
} // namespace
