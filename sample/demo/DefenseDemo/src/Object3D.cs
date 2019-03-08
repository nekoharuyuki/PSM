/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using DemoModel;

namespace DefenseDemo {

/// 3Dオブジェクト情報クラス
/**
 * モデルデータの参照ポインタ及び、アクションフレームの保持を行う。
 * @version 0.1, 2011/06/23
 */
public class Object3D {

	/// 3Dデータ
	public BasicModel model = null;
	/// 現在のアクションIndex値
	public int actIndex = 0;
	/// 現在のアクション値
	public int actFrame = 0;
}

} // end ns DefenseDemo
