/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;


namespace AppRpg {


///***************************************************************************
/// 形状 三角形で構成
///***************************************************************************
public class ShapeTriangles : GameShapeProduct
{

    private List< Vector4 >    objEntryPos;

/// 継承 メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init( int num )
    {
        Triangle        = new List< DemoGame.GeometryTriangle >();
        objEntryPos     = new List< Vector4 >();

        CollisionType   = 0;
        EntryNum        = 0;
        this.ShapeType  = TypeId.Triangles;
        return( true );
    }

    /// 破棄
    public override void Term()
    {
        if( Triangle != null ){
            for( int i=0; i<Triangle.Count; i++ ){
                Triangle[i] = null;
            }
            Triangle.Clear();
        }
        if( objEntryPos != null ){
            objEntryPos.Clear();
        }
        
        EntryNum        = 0;        objEntryPos     = null;
        Triangle        = null;
    }

    /// 行列変換
    public override void SetMult( Matrix4 mtx )
    {
        for( int i=0; i<EntryNum; i++ ){
            Triangle[i].SetMult( objEntryPos[i*3+0], objEntryPos[i*3+1], objEntryPos[i*3+2], mtx );
        }
    }



/// public メソッド
///---------------------------------------------------------------------------

    /// 衝突タイプのセット
    public void SetType( int type )
    {
        CollisionType    = type;
    }

    /// 登録
    public bool Add( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
    {
        setTriangle( pos1, pos2, pos3 );

           EntryNum = Triangle.Count;
        return( true );
    }

    /// 登録
    public bool AddLight( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
    {
        Triangle.Add( new DemoGame.GeometryTriangle( pos1, pos2, pos3 ) );
           EntryNum = Triangle.Count;
        return( true );
    }

    /// 最近接距離を求める
    public float CheckNearDis( Vector3 pos )
    {
        float isDis = 0.0f;
        float bestDis = 0.0f;

        for( int i=0; i<EntryNum; i++ ){
            isDis = pos.Dot( Triangle[i].Plane.Nor ) + Triangle[i].Plane.D;
            if( i == 0 || isDis > bestDis ){
                bestDis = isDis;
            }
        }

        return bestDis;
    }



/// private メソッド
///---------------------------------------------------------------------------
    private void setTriangle( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
    {
        Triangle.Add( new DemoGame.GeometryTriangle( pos1, pos2, pos3 ) );

        objEntryPos.Add( new Vector4( pos1.X, pos1.Y, pos1.Z, 1.0f ) );
        objEntryPos.Add( new Vector4( pos2.X, pos2.Y, pos2.Z, 1.0f ) );
        objEntryPos.Add( new Vector4( pos3.X, pos3.Y, pos3.Z, 1.0f ) );
    }





/// デバック用
///---------------------------------------------------------------------------

    /// デバック用：描画
    public void Draw( DemoGame.GraphicsDevice graphDev, int hitIdx, Rgba color1, Rgba color2 )
    {
        DemoGame.RenderGeometry        drawTri = new DemoGame.RenderGeometry();
        drawTri.MakeTriangle();

        DemoGame.Camera camera = graphDev.GetCurrentCamera();

        for( int i=0; i<EntryNum; i++ ){
            if( i != hitIdx ){
                drawTri.DrawTriangle( graphDev.Graphics, Triangle[i], camera, color1 );
            }
            else{
                drawTri.DrawTriangle( graphDev.Graphics, Triangle[i], camera, color2 );
            }
        }

        drawTri        = null;
    }



/// プロパティ
///---------------------------------------------------------------------------
    public List< DemoGame.GeometryTriangle >    Triangle;
    public int                                    CollisionType;
    public int                                    EntryNum;
    
}

} // namespace
