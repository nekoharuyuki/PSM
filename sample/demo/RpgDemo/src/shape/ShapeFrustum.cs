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
/// 錐台の形状
///***************************************************************************
public class ShapeFrustum : GameShapeProduct
{
    private int          objTriangleMax;
    private Vector4[]    objEntryPos;


/// 継承 メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init( int num )
    {
        objTriangleMax  = 1*6*2;

        Triangle        = new DemoGame.GeometryTriangle[ objTriangleMax ];
        objEntryPos     = new Vector4[ objTriangleMax*3 ];

        for( int i=0; i<objTriangleMax; i++ ){
            Triangle[i] = new DemoGame.GeometryTriangle();
        }

        EntryNum         = 0;
        this.ShapeType   = TypeId.Frustum;
        return( true );
    }

    /// 破棄
    public override void Term()
    {
        if( Triangle != null ){
            for( int i=0; i<objTriangleMax; i++ ){
                Triangle[i] = null;
            }
        }
        objTriangleMax  = 0;
        EntryNum        = 0;

        objEntryPos     = null;
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

    /// 登録
    public bool Set( Matrix4 mtx )
    {
        Vector4 nearTopL = new Vector4( -1.0f,  1.0f,  0.0f,  1.0f );
        Vector4 nearTopR = new Vector4(  1.0f,  1.0f,  0.0f,  1.0f );
        Vector4 nearBotL = new Vector4( -1.0f, -1.0f,  0.0f,  1.0f );
        Vector4 nearBotR = new Vector4(  1.0f, -1.0f,  0.0f,  1.0f );
        Vector4 farTopL  = new Vector4( -1.0f,  1.0f,  1.0f,  1.0f );
        Vector4 farTopR  = new Vector4(  1.0f,  1.0f,  1.0f,  1.0f );
        Vector4 farBotL  = new Vector4( -1.0f, -1.0f,  1.0f,  1.0f );
        Vector4 farBotR  = new Vector4(  1.0f, -1.0f,  1.0f,  1.0f );

        nearTopL = mtx * nearTopL;
        nearTopR = mtx * nearTopR;
        nearBotL = mtx * nearBotL;
        nearBotR = mtx * nearBotR;
        farTopL  = mtx * farTopL;
        farTopR  = mtx * farTopR;
        farBotL  = mtx * farBotL;
        farBotR  = mtx * farBotR;

        nearTopL /= nearTopL.W;
        nearTopR /= nearTopR.W;
        nearBotL /= nearBotL.W;
        nearBotR /= nearBotR.W;
        farTopL  /= farTopL.W;
        farTopR  /= farTopR.W;
        farBotL  /= farBotL.W;
        farBotR  /= farBotR.W;


        // near面
        setTriangle(  0, nearBotL, nearTopR, nearTopL );
        setTriangle(  1, nearBotR, nearTopR, nearBotL );

        // far面
        setTriangle(  2, farBotL, farTopL, farTopR );
        setTriangle(  3, farBotR, farBotL, farTopR );

        // 右面
        setTriangle(  4, nearTopR, farBotR, farTopR );
        setTriangle(  5, nearTopR, nearBotR, farBotR );

        // 左面
        setTriangle(  6, nearTopL, farTopL, nearBotL );
        setTriangle(  7, nearBotL, farTopL, farBotL );

        // 上面
        setTriangle(  8, nearTopL, nearTopR, farTopR );
        setTriangle(  9, nearTopL, farTopR, farTopL );

        // 下面
        setTriangle( 10, nearBotR, nearBotL, farBotL );
        setTriangle( 11, nearBotR, farBotL, farBotR );
            
        EntryNum = 12;
        return( true );
    }


    /// 最近接距離を求める
    public float CheckNearDis( Vector3 pos )
    {
        float isDis = 0.0f;
        float bestDis = 0.0f;

        /// 面単位で確認すればいいので１つ飛ばしで判定
        for( int i=0; i<EntryNum; i+=2 ){
            isDis = pos.Dot( Triangle[i].Plane.Nor ) + Triangle[i].Plane.D;
            if( i == 0 || isDis > bestDis ){
                bestDis = isDis;
            }
        }
        return bestDis;
    }



/// private メソッド
///---------------------------------------------------------------------------
    private void setTriangle( int idx, Vector4 pos1, Vector4 pos2, Vector4 pos3 )
    {
        Triangle[idx].Set( new Vector3(pos1.X,pos1.Y,pos1.Z),
                            new Vector3(pos2.X,pos2.Y,pos2.Z),
                            new Vector3(pos3.X,pos3.Y,pos3.Z) );

        objEntryPos[idx*3+0]    = pos1;
        objEntryPos[idx*3+1]    = pos2;
        objEntryPos[idx*3+2]    = pos3;
    }



/// デバック用
///---------------------------------------------------------------------------

    /// デバック用：描画
    public void Draw( DemoGame.GraphicsDevice graphDev, Rgba color )
    {
        DemoGame.RenderGeometry        drawTri = new DemoGame.RenderGeometry();
        drawTri.MakeTriangle();

        DemoGame.Camera camera = graphDev.GetCurrentCamera();

        for( int i=0; i<EntryNum; i++ ){
            drawTri.DrawTriangle( graphDev.Graphics, Triangle[i], camera, color );
        }

        drawTri        = null;
    }



/// プロパティ
///---------------------------------------------------------------------------
    public DemoGame.GeometryTriangle[]    Triangle;
    public int                            EntryNum;
    
}

} // namespace
