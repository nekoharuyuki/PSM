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
/// OBJ:基底
///***************************************************************************
public class GameObjProduct
{

    /// protected 変数    
    protected    Matrix4                        baseMtx;
    protected    ShapeSphere                    boundingShape;        /// 境界用形状


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        boundingShape    = new ShapeSphere();
        boundingShape.Init( 1 );
        boundingShape.Set( 0, new Vector3( 0.0f, 0.0f, 0.0f ), 1.0f );

        return( DoInit() );
    }

    /// 破棄
    public void Term()
    {
        DoTerm();

        boundingShape.Term();
    }

    /// 開始
    public bool Start()
    {
        baseMtx            = Matrix4.Identity;

        return( DoStart() );
    }

    /// 終了
    public void End()
    {
        DoEnd();
    }

    /// フレーム
    public bool Frame()
    {
        return( DoFrame() );
    }

    /// 描画
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        return( DoDraw(graphDev) );
    }

    /// 姿勢のセット
    public void SetMatrix( Matrix4 mtx )
    {
        baseMtx = mtx;
        DoUpdateMatrix();
    }

    /// 姿勢のセット（Objのアップデートは呼ばない）
    public void SetMatrixNoUpdate( Matrix4 mtx )
    {
        baseMtx = mtx;
    }

    /// 境界ボリュームの取得
    public ShapeSphere GetBoundSphere()
    {
        return boundingShape;
    }



/// 仮想メソッド
///---------------------------------------------------------------------------

    public virtual bool DoInit()
    {
        return true;
    }
    public virtual void DoTerm()
    {
    }
    public virtual bool DoStart()
    {
        return true;
    }
    public virtual void DoEnd()
    {
    }
    public virtual bool DoFrame()
    {
        return true;
    }
    public virtual bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        return true;
    }
    public virtual void DoUpdateMatrix()
    {
    }
    public virtual Common.ModelHandle GetModelHdl()
    {
        return null;
    }

    public virtual int CheckMoveTrgId()
    {
        return 0;
    }


/// 形状関連
///---------------------------------------------------------------------------

    /// 移動形状
    public virtual GameShapeProduct GetMoveShape()
    {
        return null;
    }
    public virtual int GetMoveShapeMax()
    {
        return 0;
    }

    /// 衝突形状
    public virtual GameShapeProduct GetCollisionShape( Data.CollTypeId type, int no )
    {
        return null;
    }
    public virtual int GetCollisionShapeMax( Data.CollTypeId type )
    {
        return 0;
    }


/// プロパティ
///---------------------------------------------------------------------------

    /// 姿勢の取得
    public Matrix4 Mtx
    {
        get {return baseMtx;}
    }
}

} // namespace
