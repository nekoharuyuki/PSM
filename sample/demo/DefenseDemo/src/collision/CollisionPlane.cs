/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

/// 当たり判定用平面情報クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionPlane {

	/// 平面を表す公式：ax + by+ cz + d = 0
	/// a
	public float a;
	/// b
	public float b;
	/// c
	public float c;
	/// d
	public float d;
	/// 法線
	public Vector3 nor;

	/// コンストラクタ
	/**
	 */
	public CollisionPlane()
	{
	}

	/// デストラクタ
	/**
	 */
	~CollisionPlane()
	{
	}

	/// 初期化
	/**
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init()
	{
		return true;
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	/// 1点と法線から平面情報の作成
	/**
	 * @prame [in] pos : 点
	 * @param [in] normal : 法線
	 */
	public void CreatePlane( Vector3 pos, Vector3 normal )
	{
		a = normal.X;
		b = normal.Y;
		c = normal.Z;
		d = -(( normal.X*pos.X + normal.Y*pos.Y + normal.Z*pos.Z ));
			
		nor = normal;
	}

	/// 3点から平面情報の作成
	/**
	 * @param [in] pos1 : 頂点1の情報
	 * @param [in] pos2 : 頂点2の情報
	 * @param [in] pos3 : 頂点3の情報
	 */
	public void CreatePlane( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
	{
		Vector3 calVec1, calVec2, normal;

		/// 法線の作成
		calVec1 = pos2 - pos1;
		calVec2 = pos3 - pos1;

		calVec1 = calVec1.Normalize();
		calVec2 = calVec2.Normalize();

		normal.X = (calVec1.Y * calVec2.Z ) - calVec1.Z * calVec2.Y;
		normal.Y = (calVec1.Z * calVec2.X ) - calVec1.X * calVec2.Z;
		normal.Z = (calVec1.X * calVec2.Y ) - calVec1.Y * calVec2.X;
		a = normal.X;
		b = normal.Y;
		c = normal.Z;

		normal = normal.Normalize();
		d = normal.Dot( pos1 ) * -1;
			
		nor = normal;
	}


}

} // end ns DefenseDemo
