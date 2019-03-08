/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg { namespace Common {

///***************************************************************************
/// 行列の補佐クラス
///***************************************************************************
static public class MatrixUtil
{
    static private Vector3        calVec;

    /// 平行移動の代入
    static public void SetTranslate( ref Matrix4 mtx, Vector3 pos )
    {
        mtx.M41 = pos.X;
        mtx.M42 = pos.Y;
        mtx.M43 = pos.Z;
        mtx.M44 = 1.0f;
    }

    /// 行列にXYZ回転をかける
    static public void SetMtxRotateEulerXYZ( ref Matrix4 mtx, Vector3 rot )
    {
		float sinx	= FMath.Sin( rot.X );
		float cosx	= FMath.Cos( rot.X );
		float siny	= FMath.Sin( rot.Y );
		float cosy	= FMath.Cos( rot.Y );
		float sinz	= FMath.Sin( rot.Z );
		float cosz	= FMath.Cos( rot.Z );

		mtx = Matrix4.Identity;

        mtx.M11    = (cosz * cosy);
        mtx.M21    = (-sinz * cosx) + (cosz * siny * sinx);
        mtx.M31    = (sinz * sinx) + (cosz * siny * cosx);
        mtx.M12    = (sinz * cosy);
        mtx.M22    = (cosz * cosx) + (sinz * siny * sinx);
        mtx.M32    = (-cosz * sinx) + (sinz * siny * cosx);
        mtx.M13    = -siny;
        mtx.M23    = (cosy * sinx);
        mtx.M33    = (cosy * cosx);
    }

    /// 視線方向を向く（upベクトルがY軸プラス方向に固定版）
    static public void LookTrgVec( ref Matrix4 mtx, Vector3 lookVec )
    {
        Vector3 upVec = new Vector3( 0.0f, 1.0f, 0.0f );

        if( lookVec.X == 0.0f && lookVec.Z == 0.0f){
            lookVec.Z = 1.0f;
        }
        // Z軸のセット
        lookVec = lookVec.Normalize();
        mtx.M31 = lookVec.X;
        mtx.M32 = lookVec.Y;
        mtx.M33 = lookVec.Z;
        mtx.M34 = 0;

        // X軸のセット
        Vector3 calVecX = Common.VectorUtil.Cross2( upVec, lookVec );
        calVecX = calVecX.Normalize();
        mtx.M11 = calVecX.X;
        mtx.M12 = calVecX.Y;
        mtx.M13 = calVecX.Z;
        mtx.M14 = 0;

        // Y軸のセット
        Vector3 calVecY = VectorUtil.Cross2( lookVec, calVecX );
        calVecY = calVecY.Normalize();
        mtx.M21 = calVecY.X;
        mtx.M22 = calVecY.Y;
        mtx.M23 = calVecY.Z;
        mtx.M24 = 0;
    }


    /// ターゲットとのY軸の角度差を返す
    static public float GetPointRotY( Matrix4 myMtx, Vector3 myPos, Vector3 trgPos )
    {
        float myRot, trRot;

        calVec.X = trgPos.X - myPos.X;
        calVec.Y = 0;
        calVec.Z = trgPos.Z - myPos.Z;

        calVec = calVec.Normalize();

        myRot = (float)( Math.Atan2( myMtx.M31, myMtx.M33 ) / (3.141593f / 180.0) );
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
