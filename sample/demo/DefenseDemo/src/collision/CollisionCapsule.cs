/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

/// カプセル形状の当たり情報クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionCapsule {

	/// 線分
	public CollisionLine colLine;
	/// 半径
	public float r;

	/// コンストラクタ
	/**
	 */
	public CollisionCapsule()
	{
		colLine = new CollisionLine();
	}

	/// デストラクタ
	/**
	 */
	~CollisionCapsule()
	{
		colLine = null;
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
		
	/// 球の軌道の始点と終点を設定し、カプセルを生成
	/**
	 * @param [in] sPos : 球の軌道開始点
	 * @param [in] ePos : 球の軌道終了点
	 * @param [in] radius : 球の半径
	 */
	public void CreateCapsule( Vector3 sPos, Vector3 ePos, float radius )
	{
		colLine.SetPos( sPos, ePos );
		r = radius;
	}

}

} // end ns DefenseDemo
