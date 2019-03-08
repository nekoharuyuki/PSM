/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {

///***************************************************************************
/// 視線衝突判定
///***************************************************************************
public class ActorUnitCollLook
{
    private Data.CollTypeId         moveType;
    private CollisionPointMove      calCollPointMove;
    private Vector3                 nextPos;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        calCollPointMove  = new CollisionPointMove();
        moveType          = Data.CollTypeId.ChMove;
    }

    /// 破棄
    public void Term()
    {
        calCollPointMove   = null;
    }


    /// 移動タイプのセット
    public void SetMoveType( Data.CollTypeId type )
    {
        moveType = type;
    }


    /// チェック
    public bool Check( GameActorCollManager collMgr, DemoGame.GeometryLine moveMoveLine )
    {
        collMgr.StartCollision();

        for( int i=0; i<collMgr.TrgContainer.Num; i++ ){

            GameObjProduct trgObj = collMgr.TrgContainer.GetEntryObj( i );

            for( int x=0; x<trgObj.GetCollisionShapeMax(moveType); x++ ){

                GameShapeProduct trgShape = trgObj.GetCollisionShape( moveType, x );

                /// 衝突
                if( calCollPointMove.Check( moveMoveLine, trgShape ) ){
                    collMgr.SetCollParam( i, x, calCollPointMove.BestId, calCollPointMove.BestDis );
                }
            }
        }

        /// 衝突座標をセット
        setScrapedMovePos( collMgr, moveMoveLine );

        /// 衝突した
        return collMgr.CheckCollHit();
    }


/// private メソッド
///---------------------------------------------------------------------------

    /// 衝突後の移動座標をセット
    private void setScrapedMovePos( GameActorCollManager collMgr, DemoGame.GeometryLine moveLine )
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
                setScrapedMovePosTriangle( collMgr, moveLine, (ShapeTriangles)trgShape );
                break;

            /// カプセル
            case GameShapeProduct.TypeId.Capsule: 
                calCollPointMove.Check( moveLine, trgShape );
                nextPos = calCollPointMove.MovePos;
                break;
            }
        }
    }


    /// 衝突後の移動座標をセット（三角形との衝突）
    private void setScrapedMovePosTriangle( GameActorCollManager collMgr, DemoGame.GeometryLine moveLine, ShapeTriangles shapeTri )
    {
        int              primId     = collMgr.TrgContainer.GetEntryPrimId( 0 );

        /// 衝突点を求める
        DemoGame.CommonCollision.CheckLineAndTriangle( moveLine, shapeTri.Triangle[primId], ref nextPos );
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
