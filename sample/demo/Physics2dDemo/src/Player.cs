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
/// Playerクラス
///
public class Player : Target
{
	public LayoutAction action_swing;
	public LayoutAction action_throw;
	public int state; // プレイヤーの状態
		
	/// プレイヤーの状態ID
	public enum StateId{
        Stay = 0,
        Swing,
        Throw
	};
	
	/// プレイヤーの状態
	public int State
	{
		get{return this.state;}
		set{state = value;}
	}
	
    /// コンストラクタ
	public Player(string name = null) : base(name)
	{
	}
	
	/// 開始処理
	///
	/// 2D素材読み込む
	/// @param [out]
	///
	public override bool Start(TargetManager targetMnager)
	{
		var image = Resource2d.GetInstance().ImageLyt_player00;
		
		//待機モーション
		action = new LayoutAction();
		LayoutAnimationList animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 90, 210, 49,  260),
				0, 1000, false, 0, 0, 0xff, 0, 0, 0xff));
		action.Add("stay", animList);
		action.SetCurrent("stay");
		
		//投げるモーション
		action_swing = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 90, 0, 203, 210, 51, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("first", animList);
		
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 210, 209, 210, 55, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("second", animList);

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 293, 0, 198, 210, 45, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("forth", animList);

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 209, 210, 112, 210, 20, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("fifth", animList);

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 321, 210, 175, 210, -32, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("eighth", animList);

		image = Resource2d.GetInstance().ImageLyt_player01;

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 235, 210, 55, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("third", animList);

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 235, 0, 202, 210, -89, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("sixth", animList);

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 0, 210, 227, 210, -107, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("seventh", animList);

		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 227, 210, 110, 210, 43, 260),
				0, 150, true, 0, 0, 0xff, 0, 0, 0xff));
		action_swing.Add("nineth", animList);
		action_swing.SetCurrent("first");

		//投げたあとのモーション
		action_throw = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 337, 210, 148, 210, 38, 260),
				0, 1000, false, 0, 0, 0xff, 0, 0, 0xff));
		action_throw.Add("throw", animList);
		action_throw.SetCurrent("throw");
		
		// 初期位置設定
		baseX = 0;
		baseY = 0;
		Move(baseX, baseY);//初期位置設定
		action.ChangeActionPosition( baseX, baseY);
		action_swing.ChangeActionPosition( baseX, baseY);
		action_throw.ChangeActionPosition( baseX, baseY);
		
		action.ChangeActionPosition(positionX,positionY);
		action_swing.ChangeActionPosition(positionX,positionY);
		action_throw.ChangeActionPosition(positionX,positionY);
			
		return true;
	}
	
	/// 終了処理
	///
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

		if(action_throw != null)
		{
			action_throw.Dispose();
			action_throw = null;
		}

		if(action_swing != null)
		{
			action_swing.Dispose();
			action_swing = null;
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
		case (int)StateId.Swing:
			action_swing.Update(GameData.FrameTimeMillis);
	        if (action_swing.CurrentKey == "first" && action_swing.IsPlayEnd()) {
	            action_swing.ChangeCurrent("second");
	        }else if(action_swing.CurrentKey == "second" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("third");
			}else if(action_swing.CurrentKey == "third" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("forth");
			}else if(action_swing.CurrentKey == "forth" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("fifth");
			}else if(action_swing.CurrentKey == "fifth" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("sixth");
			}else if(action_swing.CurrentKey == "sixth" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("seventh");
			}else if(action_swing.CurrentKey == "seventh" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("eighth");
			}else if(action_swing.CurrentKey == "eighth" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("nineth");
			}else if(action_swing.CurrentKey == "nineth" && action_swing.IsPlayEnd()){
    	        action_swing.ChangeCurrent("first");
			}
			break;
		case (int)StateId.Stay:
		case (int)StateId.Throw:
			action.ChangeActionPosition(positionX,positionY);
			action.Update(GameData.FrameTimeMillis);
			action_throw.ChangeActionPosition(positionX,positionY);
			action_throw.Update(GameData.FrameTimeMillis);
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
		if(positionX-(854+GameData.WindowPosX) >= -1000 && positionX-(854+GameData.WindowPosX) < 1000)
		{
			switch(state)
			{
			case (int)StateId.Swing:
				action_swing.Render();
				break;
			case (int)StateId.Stay:
				action.Render();
				break;
			case (int)StateId.Throw:
				action_throw.Render();
				break;
			}
		}
		return true;
	}

}

} // Physics2dDemo