/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// OBJ:モンスター
///***************************************************************************
public class ObjChMonster : GameObjProduct
{
    public  DemoGame.GeometryCapsule    objBody;
    public  Vector3                     BodyPos;

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;

    private Common.ModelHandle          useMdlHdl;
        

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        shapeMove = null;

        shapeMove = new ShapeSphere();
        shapeMove.Init(1);
        shapeMove.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), 0.001f );

        shapeColl = new ShapeCapsule();
        shapeColl.Init(1);
        shapeColl.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );

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
        if( shapeColl != null ){
            shapeColl.Term();
        }
        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }

        shapeMove = null;
        shapeColl = null;
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
        useMdlHdl.Render( graphDev, baseMtx * Matrix4.Scale(new Vector3(0.15f, 0.15f, 0.15f)) );

        Matrix4 mtx = GetBoneMatrix( 2 );
        Common.VectorUtil.Set( ref BodyPos, mtx.M41, mtx.M42, mtx.M43 );
        boundingShape.SetMult( mtx );
        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
        shapeMove.SetMult( this.baseMtx );
        shapeColl.SetMult( this.baseMtx );
        boundingShape.SetMult( this.baseMtx );
    }

    /// モデルハンドルを返す
    public override Common.ModelHandle GetModelHdl()
    {
        return useMdlHdl;
    }

    /// 移動対象かどうか
    public override int CheckMoveTrgId()
    {
        return 2;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// ボーンの姿勢を取得    
    public Matrix4 GetBoneMatrix( int boneId )
    {
        return useMdlHdl.GetBoneMatrix( boneId );
    }

    /// モデルのセット
    public void SetMdlHandle( Data.ChTypeId chTypeId )
    {
        int chTypeIdx    = (int)chTypeId;
        int mdlResIdx    = (int)Data.ModelResId.Hero + chTypeIdx;
        int texResIdx    = (int)Data.ModelTexResId.Hero + chTypeIdx;
        int shaResIdx    = (int)Data.ModelShaderReslId.Normal;

        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( mdlResIdx ), resMgr.GetTextureContainer( texResIdx ), resMgr.GetShaderContainer( shaResIdx )    );

        switch( chTypeId ){
        case Data.ChTypeId.MonsterA: shapeColl.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        case Data.ChTypeId.MonsterB: shapeColl.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.75f, 0.0f), 0.5f );    break;
        case Data.ChTypeId.MonsterC: shapeColl.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        }
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

    /// 衝突形状
    public override GameShapeProduct GetCollisionShape( Data.CollTypeId type, int no )
    {
        return shapeColl;
    }
    public override int GetCollisionShapeMax( Data.CollTypeId type )
    {
        return 1;
    }
}

} // namespace
