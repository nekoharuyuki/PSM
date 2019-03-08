/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// OBJ: 目的地のマーカー
///***************************************************************************
public class ObjDestinationMark : GameObjProduct
{
    private Common.ModelHandle        useMdlEffHdl;
    private Vector3                   baseScale;
    public  int                       TypeId;

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        useMdlEffHdl = new Common.ModelHandle();
        useMdlEffHdl.Init();
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( useMdlEffHdl != null ){
            useMdlEffHdl.Term();
        }
        useMdlEffHdl    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        DoUpdateMatrix();
        SetType( 0 );
        SetScale( 0.1f );
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
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
        Matrix4 worldMatrix = baseMtx * Matrix4.Scale( baseScale );
        useMdlEffHdl.Render( graphDev, worldMatrix );
        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
    }

    /// スケールのセット
    public void SetScale( float scale )
    {
        baseScale.X = scale;
        baseScale.Y = scale;
        baseScale.Z = scale;
    }

    /// モデルタイプのセット
    public void SetType( int type )
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        if( type == 0 ){
            useMdlEffHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Eff13 ),
                             resMgr.GetTextureContainer( (int)Data.ModelTexResId.EffD ),
                             resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                            );
        }
        else{
            useMdlEffHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Eff14 ),
                             resMgr.GetTextureContainer( (int)Data.ModelTexResId.EffD ),
                             resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                            );
        }
        TypeId = type;
        useMdlEffHdl.SetPlayAnim( 0, true );
    }

/// private メソッド
///---------------------------------------------------------------------------


/// 形状関連
///---------------------------------------------------------------------------

}

} // namespace