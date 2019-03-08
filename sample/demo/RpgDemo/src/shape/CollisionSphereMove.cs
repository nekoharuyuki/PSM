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
public class CollisionSphereMove
{
    private const float     collCheckDis = 0.5f;        /// 範囲補正値

    /// 計算結果
    private Vector3          calMovePos;       /// 移動候補座標
    private int              calBestId;        /// 衝突した登録ID
    private float            calBestDis;       /// 衝突した距離


/// public メソッド
///---------------------------------------------------------------------------

    /// カプセルと三角形との衝突
    public bool Check( DemoGame.GeometryCapsule moveCap, ShapeTriangles trgShape )
    {
        Vector3 collPos = new Vector3(0,0,0);
        calMovePos = moveCap.EndPos;
        calBestDis = -1.0f;
        calBestId  = -1;

        float checDis = moveCap.Line.Length + moveCap.R + collCheckDis;

        if( AppDebug.CollLightFlg == false ){
            for( int i=0; i<trgShape.EntryNum; i++ ){

                float a = (calMovePos.Dot( trgShape.Triangle[i].Plane.Nor ) + trgShape.Triangle[i].Plane.D);
                if( a >= checDis || a <= -checDis ){
                    continue;
                }

                if( DemoGame.CommonCollision.CheckSphereAndTriangle( moveCap, trgShape.Triangle[i], ref collPos ) == true ){
                    float dis = Common.VectorUtil.Distance( collPos, moveCap.StartPos );
                    if( dis < calBestDis || calBestId < 0 ){
                        calMovePos        = collPos;
                        calBestDis        = dis;
                        calBestId        = i;
                    }
                }
                AppDebug.CollCnt ++;
            }
        }
        else{
            for( int i=0; i<trgShape.EntryNum; i++ ){

                float a = (calMovePos.Dot( trgShape.Triangle[i].Plane.Nor ) + trgShape.Triangle[i].Plane.D);
                if( a >= checDis || a <= -checDis ){
                    continue;
                }

                if( DemoGame.CommonCollision.CheckLineAndTriangle( moveCap.Line, trgShape.Triangle[i], ref collPos ) == true ){
                    float dis = Common.VectorUtil.Distance( collPos, moveCap.StartPos );
                    if( dis < calBestDis || calBestId < 0 ){
                        calMovePos        = collPos;
                        calBestDis        = dis;
                        calBestId        = i;
                    }
                }
                AppDebug.CollCnt ++;
            }
        }

        if( calBestId >= 0 ){
            return true;
        }
        return false;
    }


    /// カプセルと三角形との衝突
    public bool Check( DemoGame.GeometryLine moveLine, ShapeTriangles trgShape )
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
            AppDebug.CollCnt ++;
        }

        if( calBestId >= 0 ){
            return true;
        }
        return false;
    }



    /// カプセルとカプセルとの衝突
    public bool Check( DemoGame.GeometryCapsule moveCap, ShapeCapsule trgCap )
    {
        /// 同じ座標にいる場合はすり抜ける
        if( moveCap.StartPos == trgCap.Capsule.StartPos ){
            return false;
        }
            
        /// 対象と反対向きへ移動する際にはすり抜ける
        float rot = Common.VectorUtil.GetPointRotY( moveCap.Line.Vec, moveCap.StartPos, trgCap.Capsule.StartPos );
        if( rot <= -50.0f || rot >= 50.0f ){
            return false;
        }

        Vector3 collPos = new Vector3(0,0,0);

        calMovePos = moveCap.EndPos;
        if( DemoGame.CommonCollision.CheckCapsuleAndCapsule( moveCap, trgCap.Capsule, ref collPos ) == true ){
            calMovePos        = collPos;
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
