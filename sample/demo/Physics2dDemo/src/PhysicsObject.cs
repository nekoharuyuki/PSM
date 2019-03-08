/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// PhysicsObjectクラス
/// 衝突させるオブジェクト(設定が簡易なもの)
/// コンストラクタで全てレイアウト設定
///
public class PhysicsObject : Target
{	
	/// コンストラクタ
	/// @param [in] name
	/// @param [in] img テクスチャ
	/// @param [in] x   切り出し座標x
	/// @param [in] y   切り出し座標y
	/// @param [in] w   幅
	/// @param [in] h   高さ
	/// @param [in] drawX 描画座標ｘ
	/// @param [in] drawY 描画座標ｙ
	/// @param [in] p     描画優先度
	///
	public PhysicsObject(string name, Texture2D img, int x, int y, int w, int h, int drawX, int drawY, int p)
	{
		Name = name;
		action = new LayoutAction();
		LayoutAnimationList animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(img, x, y, w, h, 0,  0),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
		action.Add(name, animList);
		action.SetCurrent(name);
			
		// 初期位置設定
		baseX = drawX;
		baseY = drawY;
		Move(baseX, baseY);//初期位置設定
		action.ChangeActionPosition( baseX, baseY);
			
		priority = p;
	}

	/// 開始処理
	///
	/// @param [out]
	public override bool Start(TargetManager targetMnager)
	{
		return true;
	}
		
	/// 終了処理
	///
	/// @param [out]
	///
	public override bool End(TargetManager targetMnager)
	{
		return true;
	}
	
	/// 更新処理
	///
	/// @param [out]
	///
	public override bool Update(TargetManager targetMnager)
	{	
		action.ChangeActionPosition(positionX,positionY);
		return true;
	}
	
	/// 描画
	///
	/// @param [out]
	///
	public override bool Render(TargetManager targetMnager)
	{
		if(positionX-(854+GameData.WindowPosX) >= -1000 && positionX-(854+GameData.WindowPosX) < 1000)
			action.Render();
		return true;
	}
}

} // Physics2dDemo