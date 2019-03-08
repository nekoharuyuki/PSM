/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using System.Collections.Generic;
using DemoGame;

namespace Physics2dDemo
{

///
/// SceneTitleクラス
///
public class SceneTitle : Scene
{	
	private TargetManager targetManager  = null;
	private LayoutAction  buttonAction;
	private LayoutAction  bgAction;
	private bool          isPress        = false;
	private Stage         stage          = null; 	
	private Player        player;
	private LayoutAction  fillterAction;
	private bool          backFlag       = false;  //画面スクロールを戻るフラグ
	private bool          slideFlag      = true;   //画面スクロールフラグ
	private int           windowX        = -854; // -854 <= window offset <= 0 
	private Vector2       click_pos	     = new Vector2(0, 0);	
    private Vector2       diff_pos	     = new Vector2(0, 0);
	private int           click_index    = -1;

    /// コンストラクタ
    public SceneTitle()
    {
    }

    /// デストラクタ
    ~SceneTitle()
    {
    }
	
    /// シーンの初期化
	/// 2D読み込み設定
	/// @param [out]
	///
    public override bool Start()
    {
		windowX    = -854;
		backFlag   = false;
		isPress    = false;
		slideFlag  = true;
		LayoutAnimationList animList = new LayoutAnimationList();
		var image = Resource2d.GetInstance().ImageLyt;

		AudioManager.AddBgm("Title", "/Application/res/data/snd/S41.mp3");
        AudioManager.AddSound("Press", "/Application/res/data/snd/S01.wav");

		bgAction = new LayoutAction();
		animList.Add(new SpriteAnimation(new Sprite(image, 0,80,503,311,176,25),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//タイトル
//		animList.Add(new SpriteAnimation(new Sprite(image, 647,45,75,18,390,451),
//				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));//コピーライト
        animList.Add(new SoundAnimation("Title", 1000));
		bgAction.Add("title",animList);						
		bgAction.SetCurrent("title");

		//ボタン
		buttonAction = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 648,   2, 290, 35, 281, 376),
				0, 1500, true, 0, 0, 0x33, 0, 0, 0xff));
		buttonAction.Add("In", animList);
		
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 648, 2, 290, 35, 281, 376),
				0, 1500, true, 0, 0, 0xff, 0, 0, 0x33));
		buttonAction.Add("Out", animList);
		buttonAction.SetCurrent("In");
			
        GameData.StageNum = 0;//ステージ番号初期化

		targetManager = new TargetManager();
		player        = new Player("Player");
		targetManager.Regist(player);
		stage  = new Stage00();
		
		image = Resource2d.GetInstance().ImageDummy;
		fillterAction = new LayoutAction();
		animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 0, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 122, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 244, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 366, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 488, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 610, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        animList.Add(new SpriteAnimation(new Sprite(image, 0, 0, 122, 480, 732, 0),
				0, 0, true, 0, 0, 0x80, 0, 0, 0x80));
        fillterAction.Add("Fillter", animList);
        fillterAction.SetCurrent("Fillter");
		
		return true;
	}
	
    /// シーンの破棄
    public override void End()
    {
		Graphics2D.ClearSprite();
			
		if(fillterAction != null)
		{
			fillterAction.Dispose();
			fillterAction = null;
		}
		if(bgAction != null)
		{
			bgAction.Dispose();
			bgAction = null;
		}
		
		if(buttonAction != null)
		{
			buttonAction.Dispose();
			buttonAction = null;
		}

		if(player != null){ 
			player.Dispose();
			player = null;
		}
			
		if(stage != null){
			stage.ReleaseScene();
			stage = null;
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
			SetNextScene(new SceneStart());
            if(!isPress)
				AudioManager.PlaySound("Press");
			isPress = true;
		}
					
		// スライド
		{
			if(backFlag){
				windowX -= 6;
				if(windowX < -854){
					windowX = -854;
				}
			}else{
				windowX+=6;
				if(windowX >= 0){
			    	windowX = 0;
					backFlag =true;
				}
			}
		}

		// 画面のスクロールに応じて位置をスクロール
		if(slideFlag)
		{
			stage.Scroll((-854 -windowX));
			targetManager.Scroll((-854 -windowX));
		}
		if(backFlag && windowX == -854)
			slideFlag = false;

		// 剛体のシミュレーション
		stage.Simulate(click_index, click_pos, diff_pos);

		//ボタン描画の更新
		targetManager.Update();
		buttonAction.Update(GameData.FrameTimeMillis);
		bgAction.Update(GameData.FrameTimeMillis);
			
        if (buttonAction.CurrentKey == "In" && buttonAction.IsPlayEnd()) {
            buttonAction.ChangeCurrent("Out");
        }else if(buttonAction.CurrentKey == "Out" && buttonAction.IsPlayEnd()){
            buttonAction.ChangeCurrent("In");
		}
			
		GameData.WindowPosX = windowX;//スライドの値記憶

		return true;
	}
	
    /// 描画
    public override bool Draw()
    {
		// ステージのオブジェクトレンダリング
		stage.ObjectDraw();
		
		// プレイヤ、作業員、総スコア表示
		targetManager.Render();
		
		fillterAction.Render();
		
		bgAction.Render();

		buttonAction.Render();
		
		return true;
	}
}
	
} // Physics2dDemo
