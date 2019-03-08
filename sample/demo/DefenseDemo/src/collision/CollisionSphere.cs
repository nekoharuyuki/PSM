/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

///球状の当たり情報クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionSphere {

	/// 中心点
	public Vector3 position;
	/// 半径
	public float r;

	/// コンストラクタ
	/**
	 */
	public CollisionSphere()
	{
	}

	/// デストラクタ
	/**
	 */
	~CollisionSphere()
	{
	}

	/// 初期化
	/**
	 * @param [in] num 頂点数
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init( int num )
	{
		return true;
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	// 頂点の設定
	/**
	 * @param [in] pos : 頂点情報
	 * @param [in] radius : 球の半径情報
	 */
	public void SetPos( Vector3 pos, float radius )
	{
		position = pos;
		r = radius;
	}

} // end ns DefenseDemo

}