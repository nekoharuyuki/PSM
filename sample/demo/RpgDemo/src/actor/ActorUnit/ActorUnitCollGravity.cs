/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {


///***************************************************************************
/// 重力衝突判定(TreadObjを決定する)
///***************************************************************************
public class ActorUnitCollGravity
{
    private Data.CollTypeId         moveType;

    private CollisionSphereMove     calCollSphMove;
    private Vector3                 nextPos;
    private Vector3                 treadVec;
    DemoGame.GeometryCapsule        moveMoveCap;
    private bool                    touchGroundFlg;
    private int                     touchGroundType;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        calCollSphMove    = new CollisionSphereMove();
        moveMoveCap       = new DemoGame.GeometryCapsule();

        ResetGroudParam();

        moveType        = Data.CollTypeId.ChMove;
    }

    /// 破棄
    public void Term()
    {
        calCollSphMove    = null;
        moveMoveCap       = null;
    }


    /// 移動先のセット
    public void GetMovePos( GameActorCollManager collMgr, ref Vector3 movePos )
    {
        ShapeSphere moveSph = (ShapeSphere)collMgr.MoveShape;
        movePos       = moveSph.Sphre.Pos;
        movePos.Y    -= 0.5f;
    }

    /// チェック
    public bool Check( GameActorCollManager collMgr, Vector3 movePos )
    {
        ShapeSphere moveSph = (ShapeSphere)collMgr.MoveShape;

        moveMoveCap.Set( moveSph.Sphre.Pos, movePos, moveSph.Sphre.R );

        collMgr.StartCollision();

        for( int i=0; i<collMgr.TrgContainer.Num; i++ ){

            GameObjProduct trgObj = collMgr.TrgContainer.GetEntryObj( i );

            for( int x=0; x<trgObj.GetCollisionShapeMax(moveType); x++ ){

                GameShapeProduct trgShape = trgObj.GetCollisionShape( moveType, x );

                bool hitFlg = false;

                switch( trgShape.GetShapeType() ){

                /// 三角形
                case GameShapeProduct.TypeId.Triangles: 
                    hitFlg = calCollSphMove.Check( moveMoveCap.Line, (ShapeTriangles)trgShape );
                    break;
                }

                /// 衝突
                if( hitFlg ){
                    collMgr.SetCollParam( i, x, calCollSphMove.BestId, calCollSphMove.BestDis );
                }
            }
        }

        /// 衝突後の移動座標をセット
        setScrapedMovePos( collMgr, movePos );

        /// 衝突した
        return collMgr.CheckCollHit();
    }


    /// 地面を踏んでいるかチェック
    public bool CheckTouchGround()
    {
        return touchGroundFlg;
    }

    /// 地面の属性チェック
    public int GetTouchGroundType()
    {
        return touchGroundType;
    }

    /// 地面の情報初期化
    public void ResetGroudParam()
    {
        treadVec.X        = 0.0f;
        treadVec.Y        = 1.0f;
        treadVec.Z        = 0.0f;
        touchGroundFlg    = false;
        touchGroundType   = -1;
    }


/// private メソッド
///---------------------------------------------------------------------------

    /// 衝突後の移動座標をセット
    public void setScrapedMovePos( GameActorCollManager collMgr, Vector3 movePos )
    {
        /// 衝突あり
        if( collMgr.CheckCollHit() ){

            /// ソート
            collMgr.SortCollisionTrg();

            GameObjProduct     trgObj        = collMgr.TrgContainer.GetEntryObj( 0 );
            GameShapeProduct trgShape    = trgObj.GetCollisionShape( moveType, collMgr.TrgContainer.GetEntryShapeId( 0 ) );

            switch( trgShape.GetShapeType() ){

            /// 三角形
            case GameShapeProduct.TypeId.Triangles: 
                setScrapedMovePosTriangle( collMgr, movePos, (ShapeTriangles)trgShape );
                break;
            }
        }

        /// 衝突無し
        else{
            nextPos            = movePos;
            ResetGroudParam();
        }

        /// Y軸はマイナスまで行かない
        if( nextPos.Y < 0.0f ){
            nextPos.Y = 0.0f;
        }
    }


    /// 衝突後の移動座標をセット（三角形との衝突）
    public void setScrapedMovePosTriangle( GameActorCollManager collMgr, Vector3 movePos, ShapeTriangles shapeTri )
    {
        ShapeSphere        moveSph    = (ShapeSphere)collMgr.MoveShape;
        int                primId     = collMgr.TrgContainer.GetEntryPrimId( 0 );

        Vector3            collPos = new Vector3(0,0,0);
        float              scrapedPow = (moveSph.Sphre.R+0.001f);

        /// 弾く力の算出
        moveMoveCap.Set( moveSph.Sphre.Pos, movePos, moveSph.Sphre.R );
        DemoGame.CommonCollision.CheckSphereAndTriangle( moveMoveCap, shapeTri.Triangle[primId], ref collPos );

        /// 移動候補座標更新
        nextPos.X = moveSph.Sphre.Pos.X;
        nextPos.Y = collPos.Y + ( scrapedPow * (shapeTri.Triangle[primId].Plane.Nor.Y) );
        nextPos.Z = moveSph.Sphre.Pos.Z;
        
        touchGroundType   = shapeTri.CollisionType;
        touchGroundFlg    = true;

        treadVec = shapeTri.Triangle[primId].Plane.Nor;
    }



/// プロパティ
///---------------------------------------------------------------------------

    /// 踏んでいる対象の法線
    public Vector3 TreadVec
    {
        get {return treadVec;}
    }

    /// 移動候補座標
    public Vector3 NextPos
    {
        get {return nextPos;}
    }
}

} // namespace
