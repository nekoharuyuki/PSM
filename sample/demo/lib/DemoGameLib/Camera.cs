/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame{

public class Camera
{
	private const float pi = 3.141593f;

    //    private Matrix4 projMtx;
    //    private Matrix4 viewMtx;
 
	private Vector3 camPos;
	private Vector3 camUp;
	private Vector3 camLookVec;


	/// コンストラクタ 
    public Camera()
    {
	}


/// public メンバ
///---------------------------------------------------------------------------

	/// 射影行列の生成
    public void SetPerspective( int dspWidth, int dspHeight, float angle, float near, float far  )
    {
        float aspect = (float)dspWidth / (float)dspHeight;
        float fov = angle * (pi / 180.0f);
        this.Projection = Matrix4.Perspective( fov, aspect, near, far );
    }


	/// LookAtの指定からビュー行列の生成
	/**
	 * trgRotの回転値は１周を360.0fとする
	 */
    public void SetLookAt( Vector3 trgRot, Vector3 trgPos, float trgDis )
    {
		float a_Cal1, a_Cal2, a_Cal3;
		float a_Cal		= (float)(pi / 180.0);
		float angleX	= trgRot.X * a_Cal;
		float angleY	= trgRot.Y * a_Cal;
		float angleZ	= trgRot.Z * a_Cal;

		float a_sinx	= FMath.Sin( angleX );
		float a_cosx	= FMath.Cos( angleX );
		float a_siny	= FMath.Sin( angleY );
		float a_cosy	= FMath.Cos( angleY );
		float a_sinz	= FMath.Sin( angleZ );
		float a_cosz	= FMath.Cos( angleZ );
/*
		float a_sinx	= FMath.Sin( trgRot.X );
		float a_cosx	= FMath.Cos( trgRot.X );
		float a_siny	= FMath.Sin( trgRot.Y );
		float a_cosy	= FMath.Cos( trgRot.Y );
		float a_sinz	= FMath.Sin( trgRot.Z );
		float a_cosz	= FMath.Cos( trgRot.Z );
*/
		a_Cal1 = trgDis * a_cosx;
		a_Cal2 = trgDis * a_sinx;
		a_Cal3 = trgDis * a_cosx;

		camPos.X = trgPos.X + ( a_Cal1 * a_siny );
		camPos.Y = trgPos.Y + a_Cal2;
		camPos.Z = trgPos.Z + ( a_Cal3 * a_cosy );

		camUp.X =  ( a_sinz * a_cosy );
		camUp.Y =  a_cosz;
		camUp.Z = -( a_sinz * a_siny );

		this.View = Matrix4.LookAt( camPos, trgPos, camUp );

        ViewProjection = Projection * View;

		camLookVec = trgPos - camPos;
		camLookVec = camLookVec.Normalize();
	}


/// プロパティ
///---------------------------------------------------------------------------

    public Matrix4 Projection;
    public Matrix4 View;
    public Matrix4 ViewProjection;

    public Vector3 Pos
    {
        get{ return camPos; }
	}
    public Vector3 LookVec
    {
        get{ return camLookVec; }
	}

}
} // end ns DemoGame
