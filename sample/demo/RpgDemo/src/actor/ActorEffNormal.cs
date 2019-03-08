/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// ACTOR : 通常エフェクトの操作
///***************************************************************************
public class ActorEffNormal : ActorEffBase
{
    private ObjEffect       objEff;
    private bool            billFlg;
    private bool            billYFlg;

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        objEff = new ObjEffect();
        objEff.Init();

        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        objEff.Term();
        objEff    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        objEff.Start();

        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        objEff.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        if( objEff.IsAnimation() == false ){
            Enable = false;
            return false;
        }

        objEff.Frame();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        if( billFlg == true ){
            SetBillboardMatrix( graphDev.GetCurrentCamera() );
        }
        else if( billYFlg == true ){
            SetBillboardMatrixY( graphDev.GetCurrentCamera() );
        }
        
        objEff.Draw( graphDev );
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objEff;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

    /// 姿勢の更新
    protected override void DoSetMatrix( Matrix4 mtx )
    {
        BaseMtx        = mtx;
        Common.VectorUtil.Set( ref BasePos, mtx );
        objEff.SetMatrix( mtx );
    }

    /// 境界ボリュームの取得
    protected override ShapeSphere DoGetBoundingShape()
    {
        return objEff.GetBoundSphere();
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// モデルの登録
    public void SetMdlHandle( Data.EffTypeId effTypeId )
    {
        objEff.SetMdlHandle( effTypeId );
        objEff.SetScale( 0.1f );

        this.effTypeId  = effTypeId;
        billFlg         = false;
        billYFlg        = false;

        /// 敵死亡エフェクトは大きめに表示
        if( effTypeId == Data.EffTypeId.Eff05 ){
            objEff.SetScale( 0.17f );
        }

        /// Y軸ビルボードで描画
        if( effTypeId == Data.EffTypeId.Eff03 || effTypeId == Data.EffTypeId.Eff04 || effTypeId == Data.EffTypeId.Eff05 || effTypeId == Data.EffTypeId.Eff11 ){
            billYFlg    = true;
        }

        /// ビルボードで描画
        else if( effTypeId == Data.EffTypeId.Eff08 || effTypeId == Data.EffTypeId.Eff09 || effTypeId == Data.EffTypeId.Eff12 || effTypeId == Data.EffTypeId.Eff13 ){
            billFlg    = true;
        }
    }

}

} // namespace
