/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// OBJ:装備品 剣
///***************************************************************************
public class ObjEqpSword : GameObjProduct
{
    private Common.ModelHandle        useMdlHdl;
    private Common.ModelHandle        useMdlEffHdl;
        

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        useMdlHdl = new Common.ModelHandle();
        useMdlHdl.Init();
        
        useMdlEffHdl = new Common.ModelHandle();
        useMdlEffHdl.Init();
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }
        if( useMdlEffHdl != null ){
            useMdlEffHdl.Term();
        }

        useMdlEffHdl    = null;
        useMdlHdl       = null;
    }

    /// 開始
    public override bool DoStart()
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Sword ),
                         resMgr.GetTextureContainer( (int)Data.ModelTexResId.Sword ),
                         resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                        );

        useMdlEffHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Eff00 ),
                         resMgr.GetTextureContainer( (int)Data.ModelTexResId.EffA ),
                         resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                        );
        useMdlEffHdl.SetPlayAnim( 0, true );
        DoUpdateMatrix();
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        useMdlHdl.End();
        useMdlEffHdl.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        useMdlEffHdl.UpdateAnim();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        useMdlHdl.Render( graphDev, baseMtx );
        useMdlEffHdl.Render( graphDev, baseMtx );
        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
    }


/// private メソッド
///---------------------------------------------------------------------------


/// 形状関連
///---------------------------------------------------------------------------

}

} // namespace
