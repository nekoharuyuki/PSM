/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using DemoGame;

namespace Physics2dDemo
{

///
/// Truckクラス
/// トラック
///
public class Truck : Target
{
	/// 描画優先度(値が正のものから描画される)
    public override int Priority {
			get{return -10;}
	}
	
    /// コンストラクタ
	public Truck(string name = null) : base(name)
	{
	}
	
	/// 開始処理
	/// LayoutActionの設定
	/// @param [out]
	///
	public override bool Start(TargetManager targetMnager)
	{
		var image = Resource2d.GetInstance().ImageStage00;
		action = new LayoutAction();
		LayoutAnimationList animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 227, 504, 96, 0,  0),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//荷台
		animList.Add(new SpriteAnimation(new Sprite(image, 122, 120, 192, 104, 661-661, 223-327+1.0f),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//本体　切れ目分補正
		animList.Add(new SpriteAnimation(new Sprite(image, 254, 325, 54, 98, 676-661, 226-327),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//バックミラー
		animList.Add(new SpriteAnimation(new Sprite(image, 90, 325, 88, 88, 1029-661, 375-327),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//後輪
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 325, 88, 88, 717-661, 375-327),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//前輪
		action.Add("truck", animList);
		action.SetCurrent("truck");
			
		// 初期位置設定
		baseX = 661;//荷台の位置基準
		baseY = 327;
		Move(baseX, baseY);//初期位置設定
		action.ChangeActionPosition( baseX, baseY);

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
		action.Render();
		return true;
	}
}

} // Physics2dDemo