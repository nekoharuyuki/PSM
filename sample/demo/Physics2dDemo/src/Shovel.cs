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
/// Shovelクラス
/// 油圧ショベル
///
public class Shovel : Target
{
    /// コンストラクタ
	public Shovel(string name = null) : base(name)
	{
	}
	
	/// 開始処理
	/// LayoutActionの設定
	/// @param [out]
	///
	public override bool Start(TargetManager targetMnager)
	{
		var image = Resource2d.GetInstance().ImageStage01;
		action = new LayoutAction();
		LayoutAnimationList animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 378, 322, 71, 188, 0+1, 0+1),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//1 回転時に切れ目がわからにように少しかぶせる
		action.Add("0", animList);
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 452, 322+1, 58, 190, 1239-1241+1, 157-(-31)),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//2
		action.Add("1", animList);
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 395, 181, 115, 140, 1259-1241, 208-(-31)),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//バケット
		action.Add("2", animList);
			
		// 初期位置設定
		// アーム2_00に合わせる
		baseX = 1241;//位置基準
		baseY = -31;
		Move(baseX, baseY);//初期位置設定
		action.ChangeActionPosition( baseX, baseY);
		//動かすため別々のCenterを設定
		action.SetCenter("0", 71/2, 188/2);//2DのCenter設定				
		action.SetCenter("1", 58/2+(71-58)/2, -(188/2-1));//2DのCenter設定 被せている分を補正				
		action.SetCenter("2", 115/2+(71-115)/2, -(190-140+188/2));//2DのCenter設定				
		
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
		{
			action.ChangeCurrent("0");
			action.Render();
			action.ChangeCurrent("1");
			action.Render();
			action.ChangeCurrent("2");
			action.Render();
		}
		return true;
	}
}

} // Physics2dDemo