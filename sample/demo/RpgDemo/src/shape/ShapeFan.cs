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
/// 扇型の形状
///***************************************************************************
public class ShapeFan : GameShapeProduct
{
    private int          objTriangleMax;
    private int          objDivisionNum;
    private Vector4[]    objEntryPos;


/// 継承 メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init( int num )
    {
        objTriangleMax  = num*4+4;

        Triangle        = new DemoGame.GeometryTriangle[ objTriangleMax ];
        objEntryPos     = new Vector4[ objTriangleMax*3 ];

        for( int i=0; i<objTriangleMax; i++ ){
            Triangle[i] = new DemoGame.GeometryTriangle();
        }

        objDivisionNum  = num;
        EntryNum        = 0;

        this.ShapeType  = TypeId.Fan;
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
    public bool Set( float dis, float radR, float radL, float bottomY, float upY )
    {
        EntryNum = 0;

        float calRad = (radL-radR)/objDivisionNum;
        float a_Cal        = (float)(3.141593f / 180.0);

        for( int i=0; i<objDivisionNum; i++ ){

            float calR = radR + calRad*i;
            float calL = radR + calRad*(i+1);

            if( i+1 >= objDivisionNum ){
                calL = radL;
            }
            calR *= a_Cal;
            calL *= a_Cal;

            /// 下面
            setIntoAreaTri( EntryNum+0, dis, calR, calL, bottomY );

            /// 上面
            setIntoAreaTri( EntryNum+1, dis, calL, calR, upY );

            /// 遠面
            setTriangle( EntryNum+2, Triangle[EntryNum+0].Pos3, Triangle[EntryNum+1].Pos3, Triangle[EntryNum+0].Pos1 );
            setTriangle( EntryNum+3, Triangle[EntryNum+0].Pos3, Triangle[EntryNum+1].Pos1, Triangle[EntryNum+1].Pos3 );
    
            EntryNum += 4;
        }


        if( !(radR == -180.0f && radL == 180.0f) ){
            /// 右側面
            setTriangle( EntryNum+0, Triangle[0].Pos1, Triangle[1].Pos3, Triangle[0].Pos2 );
            setTriangle( EntryNum+1, Triangle[1].Pos3, Triangle[1].Pos2, Triangle[0].Pos2 );

            /// 左側面
            setTriangle( EntryNum+2, Triangle[EntryNum-4].Pos2, Triangle[EntryNum-3].Pos2, Triangle[EntryNum-4].Pos3 );
            setTriangle( EntryNum+3, Triangle[EntryNum-3].Pos2, Triangle[EntryNum-3].Pos1, Triangle[EntryNum-4].Pos3 );

            EntryNum += 4;
        }
        return( true );
    }



    /// 最近接距離を求める
    public float CheckNearDis( Vector3 pos )
    {
        float isDis = 0.0f;
        float bestDis = 0.0f;

        /// 上面
        bestDis = pos.Dot( Triangle[0].Plane.Nor ) + Triangle[0].Plane.D;

        /// 下面
        bestDis = pos.Dot( Triangle[1].Plane.Nor ) + Triangle[1].Plane.D;
        if( isDis > bestDis ){
            bestDis = isDis;
        }        

        /// 右側面
        bestDis = pos.Dot( Triangle[EntryNum-4].Plane.Nor ) + Triangle[EntryNum-4].Plane.D;
        if( isDis > bestDis ){
            bestDis = isDis;
        }        

        /// 左側面
        bestDis = pos.Dot( Triangle[EntryNum-2].Plane.Nor ) + Triangle[EntryNum-2].Plane.D;
        if( isDis > bestDis ){
            bestDis = isDis;
        }        
        
        /// 遠面
        for( int i=0; i<objDivisionNum; i++ ){
            isDis = pos.Dot( Triangle[i*4+2].Plane.Nor ) + Triangle[i*4+2].Plane.D;
            if( i == 0 || isDis > bestDis ){
                bestDis = isDis;
            }
        }

        return bestDis;
    }



/// private メソッド
///---------------------------------------------------------------------------
    private void setTriangle( int idx, Vector3 pos1, Vector3 pos2, Vector3 pos3 )
    {
        Triangle[idx].Set( pos1, pos2, pos3 );

        objEntryPos[idx*3+0].X    = pos1.X;
        objEntryPos[idx*3+0].Y    = pos1.Y;
        objEntryPos[idx*3+0].Z    = pos1.Z;
        objEntryPos[idx*3+0].W    = 1.0f;
        objEntryPos[idx*3+1].X    = pos2.X;
        objEntryPos[idx*3+1].Y    = pos2.Y;
        objEntryPos[idx*3+1].Z    = pos2.Z;
        objEntryPos[idx*3+1].W    = 1.0f;
        objEntryPos[idx*3+2].X    = pos3.X;
        objEntryPos[idx*3+2].Y    = pos3.Y;
        objEntryPos[idx*3+2].Z    = pos3.Z;
        objEntryPos[idx*3+2].W    = 1.0f;
    }

    private void setIntoAreaTri( int idx, float dis, float rotR, float rotL, float height )
    {
        Vector3 movePos;
        Vector3 trgPos, trgPos2, trgPos3;
        Matrix4    mtx;

        movePos.X = 0.0f;
        movePos.Y = height;
        movePos.Z = 1.0f * dis;

        /// 右座標
        mtx        = Matrix4.RotationY( rotR );
        trgPos = Common.VectorUtil.Mult( ref movePos, mtx );

        /// 左座標
        mtx        = Matrix4.RotationY( rotL );
        trgPos3 = Common.VectorUtil.Mult( ref movePos, mtx );

        /// 基点座標
        trgPos2.X = 0.0f;
        trgPos2.Y = 0.0f + height;
        trgPos2.Z = 0.0f;

        /// 登録
        setTriangle( idx, trgPos, trgPos2, trgPos3 );
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
