/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

namespace DefenseDemo {

/// FPS計測クラス
/**
 * @version 0.1, 2011/06/23
 */
static public class Fps {

	/// 1フレーム前のFPS値
	static private float fpsTime;
	static private float fpsVal;

	/// FPS値の設定
	/**
	 * @param [in] time : 保持するFPS値
	 */
	static public void SetFpsTime( float time )
	{
		fpsTime = time;
	}

	/// FPS値の取得
	/**
	 * @return float : 保持しているFPS値
	 */
	static public float GetFpsTime()
	{
		return fpsTime;
	}

	/// FPS値の設定
	/**
	 * @param [in] time : 保持するFPS値
	 */
	static public void SetFpsVal( float val )
	{
		fpsVal = val;
	}

	/// FPS値の取得
	/**
	 * @return float : 保持しているFPS値
	 */
	static public float GetFpsVal()
	{
		return fpsVal;
	}
	
	/// Update処理時間の設定
	/**
	 * @param [in] time : 保持する処理時間
	 */
//	static public void SetUpdateFpsTime( float time )
//	{
//		fpsUpdateTime = time;
//	}
		
	/// Update処理時間の取得
	/**
	 * @return float : 保持している処理時間
	 */
//	static public float GetUpdateFpsTime()
//	{
//		return fpsUpdateTime;
//	}

	/// Render処理時間の設定
	/**
	 * @param [in] time : 保持する処理時間
	 */
//	static public void SetRenderFpsTime( float time )
//	{
//		fpsRenderTime = time;
//	}
		
	/// Render処理時間の取得
	/**
	 * @return float : 保持いている処理時間
	 */
//	static public float GetRenderFpsTime()
//	{
//		return fpsRenderTime;
//	}
		
}

} // end ns DefenseDemo
