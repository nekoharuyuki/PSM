/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {


///***************************************************************************
/// 移動衝突判定
///***************************************************************************
public class ActorUnitCollMove
{
    private Data.CollTypeId         moveType;
    private CollisionSphereMove     calCollSphMove;
    private Vector3                 nextPos;
    DemoGame.GeometryCapsule        moveMoveCap;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        calCollSphMove  = new CollisionSphereMove();
        moveMoveCap     = new DemoGame.GeometryCapsule();

        moveType        = Data.CollTypeId.ChMove;
    }

    /// 破棄
    public void Term()
    {
        calCollSphMove    = null;
        moveMoveCap       = null;
    }


    /// 移動タイプのセット
    public void SetMoveType( Data.CollTypeId type )
    {
        moveType = type;
    }


    /// 移動先のセット
    /**
     * 坂道でも平面と同じ移動が出来るように踏んでいる地面の角度に合わせて移動先の座標に変換をかける
     * upVec  : 地面の法線
     * trgPos : 移動候補のオフセット位置
     */
    public void GetMovePos( GameActorCollManager collMgr, Vector3 upVec, Vector3 trgPos, ref Vector3 movePos )
    {
        ShapeSphere moveSph = (ShapeSphere)collMgr.MoveShape;
        Vector3 leftVec     = new Vector3( 1.0f, 0.0f, 0.0f );
        Matrix4 world       = new Matrix4();

        // Z軸のセット
        Vector3 calVecX = Common.VectorUtil.Cross2( leftVec, upVec );
        calVecX      = calVecX.Normalize();
        world.M31    = calVecX.X;
        world.M32    = calVecX.Y;
        world.M33    = calVecX.Z;
        world.M34    = 0;

        // X軸のセット
        world.M11    = leftVec.X;
        world.M12    = leftVec.Y;
        world.M13    = leftVec.Z;
        world.M14    = 0;

        // Y軸のセット
        world.M21    = upVec.X;
        world.M22    = upVec.Y;
        world.M23    = upVec.Z;
        world.M24    = 0;

        /// 移動先のセット
        movePos = Common.VectorUtil.Mult( ref trgPos, world );
        movePos += moveSph.Sphre.Pos;
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
                    hitFlg = calCollSphMove.Check( moveMoveCap, (ShapeTriangles)trgShape );
                    break;

                /// カプセル
                case GameShapeProduct.TypeId.Capsule: 
                    hitFlg = calCollSphMove.Check( moveMoveCap, (ShapeCapsule)trgShape );
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


/// private メソッド
///---------------------------------------------------------------------------

    /// 衝突後の移動座標をセット
    private void setScrapedMovePos( GameActorCollManager collMgr, Vector3 movePos )
    {
        /// 衝突あり
        if( collMgr.CheckCollHit() ){

            /// ソート
            collMgr.SortCollisionTrg();

            GameObjProduct     trgObj    = collMgr.TrgContainer.GetEntryObj( 0 );
            GameShapeProduct trgShape    = trgObj.GetCollisionShape( moveType, collMgr.TrgContainer.GetEntryShapeId( 0 ) );

            switch( trgShape.GetShapeType() ){

            /// 三角形
            case GameShapeProduct.TypeId.Triangles:
                setScrapedMovePosTriangle( collMgr, movePos, (ShapeTriangles)trgShape );
                break;

            /// カプセル
            case GameShapeProduct.TypeId.Capsule: 
                calCollSphMove.Check( moveMoveCap, (ShapeCapsule)trgShape );
                nextPos = calCollSphMove.MovePos;
                break;
            }
        }

        /// 衝突無し
        else{
            nextPos = movePos;
        }
    }


    /// 衝突後の移動座標をセット（三角形との衝突）
    private void setScrapedMovePosTriangle( GameActorCollManager collMgr, Vector3 movePos, ShapeTriangles shapeTri )
    {
        ShapeSphere      moveSph    = (ShapeSphere)collMgr.MoveShape;
        int              primId     = collMgr.TrgContainer.GetEntryPrimId( 0 );

        Vector3          collPos = new Vector3(0,0,0);
        float            scrapedPow = (moveSph.Sphre.R+0.001f);

        /// 点と面上の最近接点を求める
        DemoGame.CommonCollision.GetClosestPtPosPlane( movePos, shapeTri.Triangle[primId].Plane, ref collPos );

        /// 移動候補座標更新
        nextPos.X = collPos.X + ( scrapedPow * (shapeTri.Triangle[primId].Plane.Nor.X) );
        nextPos.Y = collPos.Y + ( scrapedPow * (shapeTri.Triangle[primId].Plane.Nor.Y) );
        nextPos.Z = collPos.Z + ( scrapedPow * (shapeTri.Triangle[primId].Plane.Nor.Z) );
    }


 
/// プロパティ
///---------------------------------------------------------------------------

    /// 移動候補座標
    public Vector3 NextPos
    {
        get {return nextPos;}
    }
}

} // namespace
