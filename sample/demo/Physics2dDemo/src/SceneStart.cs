/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// SceneStartクラス
///
public class SceneStart : Scene
{
	private LayoutAction action;
	private bool isPress = false;
	
    /// コンストラクタ
    public SceneStart()
    {
    }

    /// デストラクタ
    ~SceneStart()
    {
    }
	
    /// シーンの初期化
    public override bool Start()
    {
		isPress = false;
		LayoutAnimationList animList = new LayoutAnimationList();
		var image = Resource2d.GetInstance().ImageLyt_ecA;
		action = new LayoutAction();
		animList.Add(new SpriteAnimation(new Sprite(image,0,0,854,480,0,0),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
		action.Add("BG", animList);
		action.SetCurrent("BG");

		AudioManager.AddBgm("Stage", "/Application/res/data/snd/S42.mp3");
        AudioManager.AddSound("Press", "/Application/res/data/snd/S01.wav");
			
		AudioManager.PlayBgm("Stage");

		return true;
	}
	
    /// シーンの破棄
    public override void End()
    {
		Graphics2D.ClearSprite();

		if(action != null)
		{
			action.Dispose();
			action = null;
		}

		AudioManager.Clear();
	}
	
    /// フレーム処理
	///
	/// @param [out]
	///
    public override bool Frame()
    {
        var pad   = InputManager.InputGamePad;
        var touch = InputManager.InputTouch;
		if ( pad.Trig != 0 || touch.GetInputNum() > 0 ) {
			SetNextScene(new SceneMain());
            if(!isPress)
				AudioManager.PlaySound("Press");
			isPress = true;
			return true;
		}

		
		return true;
	}
	
    /// 描画
	///
	/// @param [out]
	///
    public override bool Draw()
    {			
		action.Render();
		
		return true;
	}

}
	
} // Physics2dDemo