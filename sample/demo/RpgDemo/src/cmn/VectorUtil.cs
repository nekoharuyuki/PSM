/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg { namespace Common {

///***************************************************************************
/// ベクトルの補佐クラス
///***************************************************************************
static public class VectorUtil
{
    static private Vector3 calPos;
    static private Vector3 calVec;
    static private Vector4 calPos4;

    /// ２点間の距離を返す
    static public float Distance( Vector3 pos1, Vector3 pos2 )
    {
        calPos = pos1 - pos2;
		float dis = FMath.Sqrt( calPos.Dot(calPos) );
        return dis;
    }

    /// ２点間の距離を返す（XとZのみ）
    static public float DistanceXZ( Vector3 pos1, Vector3 pos2 )
    {
        calPos = pos1 - pos2;
        calPos.Y = 0;
		float dis = FMath.Sqrt( calPos.Dot(calPos) );
        return dis;
    }

    /// 代入
    static public void Set( ref Vector3 pos, float x, float y, float z )
    {
        pos.X = x;
        pos.Y = y;
        pos.Z = z;
    }

    /// 行列の平行移動成分を代入
    static public void Set( ref Vector3 pos, Matrix4 mtx )
    {
        pos.X = mtx.M41;
        pos.Y = mtx.M42;
        pos.Z = mtx.M43;
    }

    /// 行列との掛け算
    static public Vector3 Mult( ref Vector3 pos, Matrix4 mtx )
    {
        calPos.X = (mtx.M11 * pos.X) + (mtx.M21 * pos.Y) + ( mtx.M31 * pos.Z ) + ( mtx.M41 * 0.0f );
        calPos.Y = (mtx.M12 * pos.X) + (mtx.M22 * pos.Y) + ( mtx.M32 * pos.Z ) + ( mtx.M42 * 0.0f );
        calPos.Z = (mtx.M13 * pos.X) + (mtx.M23 * pos.Y) + ( mtx.M33 * pos.Z ) + ( mtx.M43 * 0.0f );
        return calPos;
    }

    /// 行列との掛け算
    static public Vector4 Mult( ref Vector4 pos, Matrix4 mtx )
    {
        calPos4.X = (mtx.M11 * pos.X) + (mtx.M21 * pos.Y) + ( mtx.M31 * pos.Z ) + ( mtx.M41 * pos.W );
        calPos4.Y = (mtx.M12 * pos.X) + (mtx.M22 * pos.Y) + ( mtx.M32 * pos.Z ) + ( mtx.M42 * pos.W );
        calPos4.Z = (mtx.M13 * pos.X) + (mtx.M23 * pos.Y) + ( mtx.M33 * pos.Z ) + ( mtx.M43 * pos.W );
        return calPos4;
    }

    static public Vector3 Cross2( Vector3 u, Vector3 v )
    {
        Vector3 res = new Vector3();
        res.X = (u.Y * v.Z) - (u.Z * v.Y);
        res.Y = (u.Z * v.X) - (u.X * v.Z);
        res.Z = (u.X * v.Y) - (u.Y * v.X);
        return res;
    }

    /// ターゲットとのY軸の角度差を返す
    static public float GetPointRotY( Vector3 lookVec, Vector3 myPos, Vector3 trgPos )
    {
        float myRot, trRot;

        calVec.X = trgPos.X - myPos.X;
        calVec.Y = 0;
        calVec.Z = trgPos.Z - myPos.Z;

        calVec = calVec.Normalize();

        myRot = (float)( Math.Atan2( lookVec.X, lookVec.Z ) / (3.141593f / 180.0) );
        trRot = (float)( Math.Atan2( calVec.X, calVec.Z ) / (3.141593f / 180.0) );

        float rot = (trRot-myRot);
        if( rot < -180.0f ){
            rot = 360.0f + rot;
        }
        else if( rot > 180.0f ){
            rot = -360.0f + rot;
        }
        return( rot );
    }
}

}} // namespace
