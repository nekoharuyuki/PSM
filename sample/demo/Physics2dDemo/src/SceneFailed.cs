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

/// SceneFailedクラス
public class SceneFailed : Scene
{
	private LayoutAction action;
	private bool isPress = false;
	
    /// コンストラクタ
    public SceneFailed()
    {
    }

    /// デストラクタ
    ~SceneFailed()
    {
    }
	
    /// シーンの初期化
    public override bool Start()
    {
		isPress = false;
		LayoutAnimationList animList = new LayoutAnimationList();
		var image = Resource2d.GetInstance().ImageLyt_ecE;

		AudioManager.AddBgm("Title", "/Application/res/data/snd/S32.mp3");//"Title"として一度だけ再生
        AudioManager.AddSound("Press", "/Application/res/data/snd/S01.wav");

		action = new LayoutAction();
		animList.Add(new SpriteAnimation(new Sprite(image,0,0,854,480,0,0),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
		animList.Add(new SoundAnimation("Title", 1000));
		action.Add("BG", animList);
		action.SetCurrent("BG");

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
    public override bool Frame()
    {
        var pad   = InputManager.InputGamePad;
        var touch = InputManager.InputTouch;
		if ( pad.Trig != 0 || touch.GetInputNum() > 0 ) {
			SetNextScene(new SceneTitle());
            if(!isPress)
				AudioManager.PlaySound("Press");
			isPress = true;
			return true;
		}
			
		action.Update(GameData.FrameTimeMillis);

		return true;
	}
	
    /// 描画
    public override bool Draw()
    {	
		action.Render();
		
		return true;
	}

}
	
} // Physics2dDemo