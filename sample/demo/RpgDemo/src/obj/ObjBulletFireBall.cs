/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// OBJ:魔法弾
///***************************************************************************
public class ObjBulletFireBall : GameObjProduct
{
    private const float objWidth = 0.25f;

    private Common.ModelHandle      useMdlHdl;
    private Vector3                 baseScale;

    /// 形状
    private ShapeSphere             shapeMove;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        shapeMove = null;

        shapeMove = new ShapeSphere();
        shapeMove.Init(1);
        shapeMove.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), objWidth );

        useMdlHdl = new Common.ModelHandle();
        useMdlHdl.Init();
        baseScale = new Vector3( 1.0f, 1.0f, 1.0f );
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( shapeMove != null ){
            shapeMove.Term();
        }
        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }

        shapeMove = null;
        useMdlHdl = null;
    }

    /// 開始
    public override bool DoStart()
    {
        int    mdlResId    = (int)Data.ModelResId.Eff07;
        int    texResId    = (int)Data.ModelTexResId.EffF;
        int    shaResId    = (int)Data.ModelShaderReslId.Normal;

        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( mdlResId ), resMgr.GetTextureContainer( texResId ), resMgr.GetShaderContainer( shaResId )    );
        useMdlHdl.SetPlayAnim( 0, true );

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
        shapeMove.SetMult( this.baseMtx );
        boundingShape.SetMult( this.baseMtx );
    }


/// public メソッド
///---------------------------------------------------------------------------

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

    /// 移動形状
    public override GameShapeProduct GetMoveShape()
    {
        return shapeMove;
    }
    public override int GetMoveShapeMax()
    {
        return 1;
    }

}

} // namespace
