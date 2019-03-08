/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

namespace DefenseDemo {

/// グリッドパーツクラス
/**
 * @version 0.1, 2011/06/23
 */
public class GridPerts {

	/// グリッドの頂点情報
	public float[] vertex;

	/// コンストラクタ
	/**
	 */
	public Grid()
	{
		vertex = new float[12];
	}

	/// デストラクタ
	/**
	 */
	~Grid()
	{
		vertex = null;
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

	/// グリッド、選択中グリッドの描画を行う
	/**
	 */
	public void Render()
	{
	}

}

} // end ns DefenseDemo
