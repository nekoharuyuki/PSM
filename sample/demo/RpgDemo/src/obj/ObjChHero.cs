/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// OBJ:英雄
///***************************************************************************
public class ObjChHero : GameObjProduct
{
    private const float objWidth = 0.1f;

    public  Vector3                 BodyPos;

    /// 形状
    private ShapeSphere             shapeMove;

    private Common.ModelHandle      useMdlHdl;
	private int                     boneIdHand;      

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
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Hero ),
                         resMgr.GetTextureContainer( (int)Data.ModelTexResId.Hero ),
                         resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                        );
			
        useMdlHdl.SetPlayAnim( 2, true );
		boneIdHand = useMdlHdl.GetBoneId( "RHand_sword_hand" );
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
        Matrix4 worldMatrix = baseMtx * Matrix4.Scale(new Vector3(0.15f, 0.15f, 0.15f));
        worldMatrix.M42 -= objWidth;

        useMdlHdl.Render( graphDev, worldMatrix );

        Matrix4 mtx = GetBoneMatrix( 1 );
        Common.VectorUtil.Set( ref BodyPos, mtx.M41, mtx.M42, mtx.M43 );

        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
        shapeMove.SetMult( this.baseMtx );
        boundingShape.SetMult( this.baseMtx );
    }

    /// モデルハンドルを返す
    public override Common.ModelHandle GetModelHdl()
    {
        return useMdlHdl;
    }



/// public メソッド
///---------------------------------------------------------------------------

    /// ボーンの姿勢を取得    
    public Matrix4 GetBoneMatrix( int boneId )
    {
        return useMdlHdl.GetBoneMatrix( boneId );
    }

    /// ボーンの姿勢を取得
    public int GetHandBoneId()
    {
        return boneIdHand;
    }

    /// モデルのセット
    public void SetMdlHandle( int mdlResId, int texResId, int shaResId )
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( mdlResId ), resMgr.GetTextureContainer( texResId ), resMgr.GetShaderContainer( shaResId )    );
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
