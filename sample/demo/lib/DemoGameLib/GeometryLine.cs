/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame
{

/// 線分
public class GeometryLine {

	public Vector3		StartPos;
	public Vector3		EndPos;
	public Vector3		Vec;
	public float		Length;

	/// コンストラクタ
	public GeometryLine()
    {
	}
	public GeometryLine( Vector3 sPos, Vector3 ePos )
    {
		Set( sPos, ePos );
    }

	/// デストラクタ
	~GeometryLine()
    {
    }


/// public メソッド
///---------------------------------------------------------------------------

	/// 生成
	/**
	 * sPosからePosへの点の軌道でラインを生成します
	 */
    public bool Set( Vector3 sPos, Vector3 ePos )
	{
		/// ベクトルの作成
		this.Vec.X		= ePos.X - sPos.X;
		this.Vec.Y		= ePos.Y - sPos.Y;
		this.Vec.Z		= ePos.Z - sPos.Z;
		this.Length	= FMath.Sqrt( this.Vec.Dot(this.Vec) );

		this.StartPos.X	= sPos.X;
		this.StartPos.Y	= sPos.Y;
		this.StartPos.Z	= sPos.Z;
		this.EndPos.X	= ePos.X;
		this.EndPos.Y	= ePos.Y;
		this.EndPos.Z	= ePos.Z;

		this.Vec = this.Vec.Normalize();
		return true;
	}

	/// 行列変換
	/**
	 * sPosからePosへの点の軌道でラインを生成します
	 */
	public void SetMult( Vector4 sPos, Vector4 ePos, Matrix4 mtx )
	{
		this.StartPos.X = (mtx.M11 * sPos.X) + (mtx.M21 * sPos.Y) + ( mtx.M31 * sPos.Z ) + ( mtx.M41 * sPos.W );
		this.StartPos.Y = (mtx.M12 * sPos.X) + (mtx.M22 * sPos.Y) + ( mtx.M32 * sPos.Z ) + ( mtx.M42 * sPos.W );
		this.StartPos.Z = (mtx.M13 * sPos.X) + (mtx.M23 * sPos.Y) + ( mtx.M33 * sPos.Z ) + ( mtx.M43 * sPos.W );

		this.EndPos.X = (mtx.M11 * ePos.X) + (mtx.M21 * ePos.Y) + ( mtx.M31 * ePos.Z ) + ( mtx.M41 * ePos.W );
		this.EndPos.Y = (mtx.M12 * ePos.X) + (mtx.M22 * ePos.Y) + ( mtx.M32 * ePos.Z ) + ( mtx.M42 * ePos.W );
		this.EndPos.Z = (mtx.M13 * ePos.X) + (mtx.M23 * ePos.Y) + ( mtx.M33 * ePos.Z ) + ( mtx.M43 * ePos.W );

		/// ベクトルの作成
		this.Vec.X		= this.EndPos.X - this.StartPos.X;
		this.Vec.Y		= this.EndPos.Y - this.StartPos.Y;
		this.Vec.Z		= this.EndPos.Z - this.StartPos.Z;
		this.Length	= FMath.Sqrt( this.Vec.Dot(this.Vec) );

		this.Vec = this.Vec.Normalize();
	}

	/// 座標取得
	public Vector3 GetPos( int idx )
	{
		if( idx == 0 )	return this.StartPos;
		return this.EndPos;
	}

} // GeometryLine

}
