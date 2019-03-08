/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg
{

///***************************************************************************
/// 移動衝突処理
///***************************************************************************
public class CollisionPointMove
{
    private const float     collCheckDis = 0.5f;        /// 範囲補正値

    /// 計算結果
    private Vector3          calMovePos;       /// 移動候補座標
    private int              calBestId;        /// 衝突した登録ID
    private float            calBestDis;       /// 衝突した距離


/// public メソッド
///---------------------------------------------------------------------------

    /// ラインとの衝突
    public bool Check( DemoGame.GeometryLine moveLine, GameShapeProduct trgShape )
    {
        bool hitFlg = false;

        switch( trgShape.GetShapeType() ){

        /// 三角形
        case GameShapeProduct.TypeId.Triangles: 
            hitFlg = CheckTriangle( moveLine, (ShapeTriangles)trgShape );
            break;
        
        /// カプセル
        case GameShapeProduct.TypeId.Capsule: 
            hitFlg = CheckCapsule( moveLine, (ShapeCapsule)trgShape );
            break;
        }

        return hitFlg;
    }


    /// ラインと三角形との衝突
    public bool CheckTriangle( DemoGame.GeometryLine moveLine, ShapeTriangles trgShape )
    {
        Vector3 collPos = new Vector3(0,0,0);

        calMovePos = moveLine.EndPos;
        calBestDis = -1.0f;
        calBestId  = -1;

        float checDis = moveLine.Length + collCheckDis;

        for( int i=0; i<trgShape.EntryNum; i++ ){

            float a = (calMovePos.Dot( trgShape.Triangle[i].Plane.Nor ) + trgShape.Triangle[i].Plane.D);
            if( a >= checDis || a <= -checDis ){
                continue;
            }

            if( DemoGame.CommonCollision.CheckLineAndTriangle( moveLine, trgShape.Triangle[i], ref collPos ) == true ){
                float dis = Common.VectorUtil.Distance( collPos, moveLine.StartPos );
                if( dis < calBestDis || calBestId < 0 ){
                    calMovePos        = collPos;
                    calBestDis        = dis;
                    calBestId        = i;
                }
            }
        }

        if( calBestId >= 0 ){
            return true;
        }
        return false;
    }


    /// ラインとカプセルとの衝突
    public bool CheckCapsule( DemoGame.GeometryLine moveLine, ShapeCapsule trgCapsule )
    {
        Vector3 collPos = new Vector3(0,0,0);

        calMovePos = moveLine.EndPos;
        if( DemoGame.CommonCollision.CheckLineAndCapsule( moveLine, trgCapsule.Capsule, ref collPos ) == true ){
            calMovePos       = collPos;
            calBestDis       = Common.VectorUtil.Distance( collPos, moveLine.StartPos );
            calBestId        = 0;
            return true;
        }         
        return false;
    }




/// private メソッド
///---------------------------------------------------------------------------


/// プロパティ
///---------------------------------------------------------------------------

    public Vector3 MovePos
    {
        get {return calMovePos;}
    }
    public int BestId
    {
        get {return calBestId;}
    }
    public float BestDis
    {
        get {return calBestDis;}
    }
}

} // namespace
