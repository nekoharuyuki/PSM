/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

/// 三角形状の当たり情報クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionTriangle {

	/// 頂点データ
	public Vector3[] vertex;
	/// 平面情報
	public CollisionPlane plane;

	/// コンストラクタ
	/**
	 */
	public CollisionTriangle()
	{
		vertex = new Vector3[3];
		for( int i=0; i<3; i++ ){
			vertex[i] = new Vector3();
		}
		plane = new CollisionPlane();
	}

	/// デストラクタ
	/**
	 */
	~CollisionTriangle()
	{
		plane = null;
		vertex = null;
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
	 * @param [in] pos1 : 頂点1の情報
	 * @param [in] pos2 : 頂点2の情報
	 * @param [in] pos3 : 頂点3の情報
	 */
	public void SetPos( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
	{
		vertex[0] = pos1;
		vertex[1] = pos2;
		vertex[2] = pos3;
		plane.CreatePlane( vertex[0], vertex[1], vertex[2] );
	}

} // end ns DefenseDemo

}