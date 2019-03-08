/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// OBJ:エフェクト
///***************************************************************************
public class ObjEffect : GameObjProduct
{
    private Common.ModelHandle        useMdlHdl;
    private Vector3                   baseScale;

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        useMdlHdl = new Common.ModelHandle();
        useMdlHdl.Init();
        baseScale = new Vector3( 1.0f, 1.0f, 1.0f );
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }
        useMdlHdl = null;
    }

    /// 開始
    public override bool DoStart()
    {
        DoUpdateMatrix();
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        useMdlHdl.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        useMdlHdl.UpdateAnim();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        Matrix4 worldMatrix = baseMtx * Matrix4.Scale( baseScale );
        useMdlHdl.Render( graphDev, worldMatrix );
        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
        boundingShape.SetMult( this.baseMtx );
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// モデルのセット
    public void SetMdlHandle( Data.EffTypeId effTypeId )
    {
        int effTypeIdx    = (int)effTypeId;
        int    mdlResId    = (int)Data.ModelResId.Eff00 + effTypeIdx;
        int    texResId    = (int)Data.ModelTexResId.EffA;
        int    shaResId    = (int)Data.ModelShaderReslId.Normal;

        switch( effTypeId ){
        case Data.EffTypeId.Eff00:    texResId    = (int)Data.ModelTexResId.EffA;    break;
        case Data.EffTypeId.Eff01:    texResId    = (int)Data.ModelTexResId.EffB;    break;
        case Data.EffTypeId.Eff02:    texResId    = (int)Data.ModelTexResId.EffB;    break;
        case Data.EffTypeId.Eff03:    texResId    = (int)Data.ModelTexResId.EffC;    break;
        case Data.EffTypeId.Eff04:    texResId    = (int)Data.ModelTexResId.EffC;    break;
        case Data.EffTypeId.Eff05:    texResId    = (int)Data.ModelTexResId.EffD;    break;
        case Data.EffTypeId.Eff06:    texResId    = (int)Data.ModelTexResId.EffE;    break;
        case Data.EffTypeId.Eff07:    texResId    = (int)Data.ModelTexResId.EffF;    break;
        case Data.EffTypeId.Eff08:    texResId    = (int)Data.ModelTexResId.EffE;    break;
        case Data.EffTypeId.Eff09:    texResId    = (int)Data.ModelTexResId.EffE;    break;
        case Data.EffTypeId.Eff10:    texResId    = (int)Data.ModelTexResId.EffG;    break;
        case Data.EffTypeId.Eff11:    texResId    = (int)Data.ModelTexResId.EffH;    break;
        case Data.EffTypeId.Eff12:    texResId    = (int)Data.ModelTexResId.EffI;    break;
        case Data.EffTypeId.Eff13:    texResId    = (int)Data.ModelTexResId.EffI;    break;
        case Data.EffTypeId.Eff14:    texResId    = (int)Data.ModelTexResId.EffI;    break;
        }


        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( mdlResId ), resMgr.GetTextureContainer( texResId ), resMgr.GetShaderContainer( shaResId )    );
        useMdlHdl.SetPlayAnim( 0, false );
    }

    /// アニメーションの終了確認
    public bool IsAnimation()
    {
        return useMdlHdl.IsAnimation();
    }

    /// スケールのセット
    public void SetScale( float scale )
    {
        baseScale.X = scale;
        baseScale.Y = scale;
        baseScale.Z = scale;
    }


    /// アニメーションのセット
    public void SetAnimation( int id, bool loop )
    {
        useMdlHdl.SetPlayAnim( id, loop );
    }


/// private メソッド
///---------------------------------------------------------------------------


/// 形状関連
///---------------------------------------------------------------------------

}

} // namespace
