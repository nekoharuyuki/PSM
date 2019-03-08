/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using DemoGame;
using Sce.PlayStation.Core.Graphics;

namespace Physics2dDemo
{

///
/// Enemyクラス
/// 作業員(投げられる対象)
///
public class Enemy : Target
{
	public int state; // プレイヤーの状態
		
	/// 描画優先度(値が正のものから描画される)
    public override int Priority {
			get{return -6;}
	}

	/// 作業員の状態ID
	public enum StateId{
        Stay = 0,
        Throw,//投げられた状態
	};

	/// 作業員の状態
	public int State
	{
		get{return this.state;}
		set{state = value;}
	}

    /// コンストラクタ
	public Enemy(string name = null) : base(name)
	{
		state = 0;
	}
	
	/// 開始処理
	/// アニメーションの設定
	/// @param [out]
	///
	public override bool Start(TargetManager targetMnager)
	{
		var image = Resource2d.GetInstance().ImageLyt_enemy;
			
		action = new LayoutAction();
		LayoutAnimationList animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 180, 118, 0, 0),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//全身
		action.Add("0", animList);
	

		// 初期位置設定
		baseX = 10;
		baseY = 380;
		Move(baseX, baseY);//Target初期位置設定
		action.ChangeActionPosition(positionX,positionY);// 2Dの位置設定
		action.SetCenter("0", 180/2, 118/2);//2DのCenter設定				
		
		return true;
	}
	
	/// 終了処理
	/// @param [out]
	///
	public override bool End(TargetManager targetMnager)
	{
		Release();
		return true;
	}

	/// 破棄
    private void Release()
    {			
		if(action != null)
		{
			action.Dispose();
			action = null;
		}
	}

	/// 更新処理
	///
	/// @param [out]
	///
	public override bool Update(TargetManager targetMnager)
	{			
		switch(state)
		{
		case (int)StateId.Stay:
			break;
		case (int)StateId.Throw:
			action.ChangeActionPosition(positionX - 180/2,positionY - 118/2);
			break;
		}
		return true;
	}
	
	/// 描画
	///
	/// @param [out]
	///
	public override bool Render(TargetManager targetMnager)
	{
		switch(state)
		{
		case (int)StateId.Stay:
			break;
		case (int)StateId.Throw:
			action.ChangeCurrent("0");
			action.Render();
			break;
		}

		return true;
	}
}

} // Physics2dDemo