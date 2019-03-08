/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg
{


///***************************************************************************
/// カプセル
///***************************************************************************
public class ShapeCapsule : GameShapeProduct {

    private Vector4[]        objEntryPos;


/// 継承 メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init( int num )
    {
        this.Capsule            = new DemoGame.GeometryCapsule();
        objEntryPos             = new Vector4[2];
        this.CollisionType      = 0;
        this.ShapeType          = TypeId.Capsule;
        return( true );
    }

    /// 破棄
    public override void Term()
    {
        this.Capsule        = null;
        objEntryPos         = null;
    }

    /// 行列変換
    public override void SetMult( Matrix4 mtx )
    {
        this.Capsule.SetMult( objEntryPos[0], objEntryPos[1], this.Capsule.R, mtx );
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 登録
    public bool Set( int type, Vector3 posS, Vector3 posE, float radius )
    {
        objEntryPos[0].X = posS.X;
        objEntryPos[0].Y = posS.Y;
        objEntryPos[0].Z = posS.Z;
        objEntryPos[0].W = 1.0f;
        objEntryPos[1].X = posE.X;
        objEntryPos[1].Y = posE.Y;
        objEntryPos[1].Z = posE.Z;
        objEntryPos[1].W = 1.0f;

        this.Capsule.Set( posS, posE, radius );
        this.CollisionType    = type;
        return( true );
    }

    /// 最近接距離を求める
    public float CheckNearDis( Vector3 pos )
    {
        Vector3 colPos = new Vector3();
        float isDis = DemoGame.CommonCollision.GetClosestPtPosLine( pos, this.Capsule.Line, ref colPos );
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
        drawSph.MakeCapsule();

        DemoGame.Camera camera = graphDev.GetCurrentCamera();

        if( hitIdx < 0 ){
            drawSph.DrawCapsule( graphDev.Graphics, this.Capsule, camera, color1 );
        }
        else{
            drawSph.DrawCapsule( graphDev.Graphics, this.Capsule, camera, color2 );
        }

        drawSph        = null;
    }



/// プロパティ
///---------------------------------------------------------------------------
    public DemoGame.GeometryCapsule       Capsule;
    public int                            CollisionType;
}

} // namespace
