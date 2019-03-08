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
/// 形状 球
///***************************************************************************
public class ShapeSphere : GameShapeProduct
{
    private Vector4            objEntryPos;


/// 継承 メソッド
///---------------------------------------------------------------------------
    /// 初期化
    public override bool Init( int num )
    {
        this.Sphre            = new DemoGame.GeometrySphere();
        this.CollisionType    = 0;
        this.ShapeType        = TypeId.Sphere;
        return( true );
    }

    /// 破棄
    public override void Term()
    {
        this.Sphre        = null;
    }

    /// 行列変換
    public override void SetMult( Matrix4 mtx )
    {
        this.Sphre.SetMult( objEntryPos, this.Sphre.R, mtx );
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 登録
    public bool Set( int type, Vector3 pos, float radius )
    {
        objEntryPos.X = pos.X;
        objEntryPos.Y = pos.Y;
        objEntryPos.Z = pos.Z;
        objEntryPos.W = 1.0f;

        this.Sphre.Set( pos, radius );
        this.CollisionType    = type;
        return( true );
    }

    /// 最近接距離を求める
    public float CheckNearDis( Vector3 pos )
    {
        float isDis = Common.VectorUtil.Distance( this.Sphre.Pos, pos );
        return isDis;
    }



/// private メソッド
///---------------------------------------------------------------------------



/// デバック用
///---------------------------------------------------------------------------

    /// デバック用：描画
    public void Draw( DemoGame.GraphicsDevice graphDev, int hitIdx, Rgba color1, Rgba color2 )
    {
        DemoGame.RenderGeometry        drawSph = new DemoGame.RenderGeometry();
        drawSph.MakeSphere();

        DemoGame.Camera camera = graphDev.GetCurrentCamera();

        if( hitIdx < 0 ){
            drawSph.DrawSphere( graphDev.Graphics, this.Sphre, camera, color1 );
        }
        else{
            drawSph.DrawSphere( graphDev.Graphics, this.Sphre, camera, color2 );
        }

        drawSph        = null;
    }



/// プロパティ
///---------------------------------------------------------------------------
    public DemoGame.GeometrySphere        Sphre;
    public int                            CollisionType;
    
}

} // namespace
