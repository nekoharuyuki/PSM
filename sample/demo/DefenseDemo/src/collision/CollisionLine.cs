/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

/// 当たり判定用線分情報クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionLine {

	/// 始点
	public Vector3 posStart;
	/// 終点
	public Vector3 posEnd;
	/// 向き
	public Vector3 dir;
	/// 長さ
	public float length;

	/// コンストラクタ
	/**
	 */
	public CollisionLine()
	{
		posStart = new Vector3();
		posEnd = new Vector3();
		dir = new Vector3();
		length = 0.0f;
	}

	/// デストラクタ
	/**
	 */
	~CollisionLine()
	{
		length = 0.0f;
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

	/// 始点、終点の設定
	/**
	 * @prame [in] posS : 始点
	 * @param [in] posE : 終点
	 */
	public void SetPos( Vector3 posS, Vector3 posE )
	{
		/// 始点→終点の向きを算出
		dir.X = posE.X - posS.X;
		dir.Y = posE.Y - posS.Y;
		dir.Z = posE.Z - posS.Z;
		length = FMath.Sqrt( dir.Dot( dir ) );

		posStart.X = posS.X;
		posStart.Y = posS.Y;
		posStart.Z = posS.Z;
		posEnd.X = posE.X;
		posEnd.Y = posE.Y;
		posEnd.Z = posE.Z;

		dir = dir.Normalize();
	}

}

} // end ns DefenseDemo
