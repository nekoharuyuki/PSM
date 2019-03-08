/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using DemoGame;

namespace FlightDemo{

public class DemoUtil
{
    public static Vector3 ComputeYawPitchRoll( Matrix4 mtx )
	{
		Vector3 ret = new Vector3( 0.0f, 0.0f, 0.0f );

		if( DemoUtil.Equals( mtx.M23, 0.0f ) ){
			ret.X = 0.0f;
			ret.Y = (float)Math.Atan2( mtx.M13, mtx.M33 ) * -1.0f;
			ret.Z = (float)Math.Atan2( mtx.M21, mtx.M22 ) * -1.0f;

		}else if( DemoUtil.Equals( mtx.M23, -1.0f ) ){
			ret.X = (float)Math.PI / 2.0f * -1.0f;
			ret.Y = (float)Math.Atan2( mtx.M31, mtx.M11 );
			ret.Z = 0.0f;

		}else if( DemoUtil.Equals( mtx.M23, 1.0f ) ){
			ret.X = (float)Math.PI / 2.0f;
			ret.Y = (float)Math.Atan2( mtx.M31, mtx.M11 );
			ret.Z = 0.0f;

		}else{
			ret.X = (float)Math.Asin( mtx.M23 );
			ret.Y = (float)Math.Atan2( mtx.M13, mtx.M33 ) * -1.0f;
			ret.Z = (float)Math.Atan2( mtx.M21, mtx.M22 ) * -1.0f;
		}
		return ret;
	}

    public static bool Equals( float lhs, float rhs )
    {
		return Math.Abs(lhs - rhs) <= 0.000001;
	}

    public static float Rad2Deg( float r )
    {
        return r / ((float)Math.PI / 180.0f);
    }

    public static float Deg2Rad( float d )
    {
        return d * ((float)Math.PI / 180.0f);
    }

    /// 線形補間
    public static float Lerp( float val, float st, float ed )
    {
        float d = ed - st;

        return st + d * val;
    }

    /// レイ(pos + dir) と平面から交点を得る
    /**
     * @return : 交点が見つかった場合(ptが有効な場合)true
     */
    public static bool IntersectRayPlane( ref Vector3 pt, GeometryPlane plane, Vector3 pos, Vector3 dir)
    {
        dir = dir.Normalize();

        //     -(N・Q + D)
        // t = ------------
        //        (N・V)
        float den = plane.Nor.Dot( dir );

        // 面と向きが平行だったら交点は発生しない。
        if( DemoUtil.Equals( den, 0.0f ) ){
            return false;
        }

        float num = -(plane.Nor.Dot( pos ) + plane.D);
        float t = num  / den;

        // P(t) = Q + tV
        pt = pos + dir * t;
        return true;
    }



    /// Clamp 
    public static float Clamp( float value, float max, float  min )
    {     
        float result = value;
        if( value.CompareTo(max) > 0 ){
            result = max;
        }
        if( value.CompareTo(min) < 0 ){
            result = min;
        }
        return result;
    } 

    /// イーズアウト
    public static float EaseOut( float start, float end, float dt )
    {
        float len = end - start;
        float f = 1.0f - (float)Math.Exp(-6.0f * dt);
        return start + len * f;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
