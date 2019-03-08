/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame
{

/// カプセル
public class GeometryCapsule {

	public GeometryLine		Line;
	public float			Radius;


	/// コンストラクタ
	public GeometryCapsule()
    {
		this.Line		= new GeometryLine();
	}
	public GeometryCapsule( Vector3 sPos, Vector3 ePos, float radius )
	{
		this.Line		= new GeometryLine();
		Set( sPos, ePos, radius );
	}

	/// デストラクタ
	~GeometryCapsule()
	{
		this.Line		= null;
	}


/// public メソッド
///---------------------------------------------------------------------------

	/// 生成
	/**
	 * sPosからePosへの球の軌道でカプセルを生成します
	 */
    public bool Set( Vector3 sPos, Vector3 ePos, float radius )
	{
		this.Line.Set( sPos, ePos );
		this.Radius	= radius;
		return true;
	}

	/// 行列変換
	/**
	 * sPosからePosへの球の軌道でカプセルを生成します
	 */
	public void SetMult( Vector4 sPos, Vector4 ePos, float radius, Matrix4 mtx )
	{
		this.Line.SetMult( sPos, ePos, mtx );
		this.Radius	= radius;
	}


	/// 座標取得
	public Vector3 GetPos( int idx )
	{
		return( this.Line.GetPos(idx) );
	}

    /// ベクトル取得
    public Vector3 Vec
    {
        get {return this.Line.Vec;}
    }

    /// 座標取得
    public Vector3 StartPos
    {
        get {return this.Line.StartPos;}
    }
    public Vector3 EndPos
    {
        get {return this.Line.EndPos;}
    }
	/// 半径
    public float R
    {
        get {return this.Radius;}
    }

} // GeometryCapsule

}
