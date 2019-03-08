/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
// 
// key
// 画面状態（Ready,Play,Result）
// Ready
//   画面タッチ,またはキー押下によりplay状態にスキップ可能
// Play
//   画面スライド	： タッチによるスライド
//   投擲       	： プレイヤにタッチした状態で画面をフリック,フリック解除時に投げる
//   遷移       	： 一定時間経過でResult状態に遷移
//   やり直し　　	： 画面をダブルタップ
//   ・デバック用キー操作 : デバッグビルド時のみ有効
//      上キー　	： やり直し
//      下キー　	： ステージクリアとして次画面に遷移
//      左右キー	： 画面スライド
//      Lボタン	： 左スライド
//      Rボタン	： 位置初期化  
//
// Result
//    得点を判定し,ステージクリア時は次画面へ,失敗時は失敗画面に遷移
// 
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Graphics;
using DemoGame;
using Sce.PlayStation.Core.Input;
using System.Collections.Generic;

// J: 2D Physicsフレームワークをインクルード
// E: Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2dDemo
{

/// SceneMainクラス
/// ゲーム中画面
public class SceneMain : Scene
{	
	private TargetManager targetManager     = null;
	private TargetManager scoreEffectMgr    = null;
	private LayoutAction  messageAction     = null; // throw him away
	private LayoutAction  requireAction     = null; // get more than xxx points!
	private LayoutAction  ArrowAction       = null; // フリック時の矢印(長さ可変)
	private LayoutAction  ArrowEndAction    = null; // フリック時の矢印(末端)
	private LayoutAction  ArrowTargetAction = null; // フリック時の矢印（先端）
	private NumberLayout  requireScoreLayout;
	private Stage         stage             = null; 	
	private Player        player;
	private Enemy         enemy;
	private Score         score;
	
	private int           stop_time   = 12;
	private float         border_line = 1.4f;   // 得点の閾値 衝突時に与える重力と合わせて設定
	private int           slide_cnt   = 0;      // 投げたあとの追従描画を制御		
	private int           ready_cnt   = 0;      // 投げる前の追従描画を制御
	private int           end_cnt     = 0;      // ゲームオーバー時のカウント
	private bool          backFlag    = false;  // 画面スクロールを戻るフラグ
	
	private Vector2       click_pos;
    private Vector2       diff_pos;
	private int           click_index = -1;

    private bool          first_touch_flag  = false;
    private bool          second_touch_flag = false;
    private bool          double_touch_flag = false;
    private int           double_touch_cnt  = 0;
    private bool          mouse_move_old    = false;
	private bool          mouse_move        = false;
	private bool          slide_move        = false; // 画面スライド用
	private bool          slide_old_move    = false; // 画面スライド用過去一度でも変えた場合
	private float         old_touchX        = 0;
	private int           old_screenX       = -854;
	private Vector2       touch_point       = new Vector2(0.0f, 0.0f);         // タッチいたポイント
	private Vector2       base_flickPoint   = new Vector2( -330.0f, -120.0f ); //投げる物体の中心
	private static int    window_width      = 0;    //854;
    private static int    window_height     = 0;    //480;
    private static float  rendering_scale   = 1.0f;
		
	private int           player_width      = 200;  //フリックの入力範囲に使用
	private int           player_height     = 200;
		
	private float[]       objOldLen         = null; // オブジェクトの移動量の距離
	private int[]         objCnt            = null; // オブジェクトの移動カウント
	private int[]         objActiveCnt      = null; // オブジェクトの動き続けた場合の移動カウント
	private int           windowX           = -854; // -854 <= window offset <= 0 
	private int           sceneBodiesNum         = 0;    // 剛体の数
	private int           scene             = 0;    // シーンの状態
	private int           main_state        = 0;    // ゲーム画面の状態
	private string        name              = null; // 検索用
	private bool          isFan             = false;// ファンファーレ中かどうか
	private bool          isfall            = false;// 作業員落下フラグ
	private bool          isfall_drum       = false;// ステージ3ドラム缶落下フラグ
	private bool          isfall_sack       = false;// ステージ1資材落下フラグ
	private bool          isCrash_shavel    = false;// ステージ2クレーン衝突フラグ
	private bool          isSignPartsCol    = false;// 看板の部品衝突フラグ
	private bool          isSignCol         = false;// 看板の衝突フラグ
	private int           s26_counter       = 0;
	private int           s27_counter       = 0;

	/// クリア条件のスコア
	public enum StageClearScore{
		Stage00 = 800,
		Stage01 = 400,
		Stage02 = 1200,
		Stage10 = 1400,
		Stage11 = 600,
		Stage12 = 1800,
	}
		
	/// シーンの状態ID
	public enum SceneId{
        Scene00 = 0,
        Scene01,
        Scene02,
	};	

	/// ゲーム画面の状態
	public enum MainState{
        Ready = 0,
        Play,
		Result,
	};
		
	/// コンストラクタ
    public SceneMain()
    {
    }

    /// デストラクタ
    ~SceneMain()
    {
    }
	
    /// シーンの初期化
    /// PhysicsSceneを設定する
	/// @param [out]
    public override bool Start()
    {
		main_state = (int)MainState.Ready;
		ready_cnt  = 0;
		slide_cnt  = 0;
		windowX    = -854;
		backFlag   = false;
		isFan      = false;
		isfall     = false;
		isfall_drum     = false;
		isfall_sack     = false;
		isCrash_shavel  = false;
		isSignPartsCol  = false;
		isSignCol       = false;
		slide_move      = false;
		slide_old_move  = false;
		end_cnt         = 0; 

		// 画面サイズを取得する
        window_width  = GameData.TargetScreenWidth;
        window_height = GameData.TargetScreenHeight;
		
		// メッセージ作成
		var image = Resource2d.GetInstance().ImageLyt;
		messageAction = new LayoutAction();
		LayoutAnimationList animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image,   0,   0, 644,  67, 102, 197),
				0, 500, true, 0, 0, 0x55, 0, 0, 0xff));
		messageAction.Add("In", animList);
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image,   0,   0, 644,  67, 102, 197),
				0, 500, true, 0, 0, 0xff, 0, 0, 0x55));
		messageAction.Add("Out", animList);
		messageAction.SetCurrent("In");
		
		// クリア条件メッセージ作成
		requireAction = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 516, 78, 411, 67, 38, 197),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
		animList.Add(new SpriteAnimation(new Sprite(image, 521, 154, 212, 67, 599, 197),
				0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
		requireAction.Add("Require", animList);
		requireAction.SetCurrent("Require");

		requireScoreLayout = new NumberLayout(
            new SpriteAnimation[] {
                new SpriteAnimation(new Sprite(image, 515, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 555, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 595, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 635, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 675, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 715, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 755, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 795, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 835, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 875, 230, 40, 67, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff)
            },
            new int[]{552, 520, 488, 456},
            new int[]{197, 197, 197, 197}
		);
		
		// 矢印作成
		image = Resource2d.GetInstance().ImageArrow;

		// 矢印先端
		ArrowTargetAction = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 49, 24, 36, 36, 0, 0),
				0, 0, true, 0, 0, 0x88, 0, 0, 0x88));
		ArrowTargetAction.Add("ArrowTarget", animList);
		ArrowTargetAction.SetCurrent("ArrowTarget");
		ArrowTargetAction.SetCenter("ArrowTarget", 36/2, 0);

		// 矢印末端
		ArrowEndAction = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 49, 260-4, 36, 4, 0, 0),
				0, 0, true, 0, 0, 0x88, 0, 0, 0x88));
		ArrowEndAction.Add("ArrowEnd", animList);
		ArrowEndAction.SetCurrent("ArrowEnd");
		ArrowEndAction.SetCenter("ArrowEnd", 36/2, 0);
		ArrowEndAction.ChangeActionPosition(427+(int)base_flickPoint.X-36/2, -((int)base_flickPoint.Y-240));			
			
		// 矢印棒作成
		image = Resource2d.GetInstance().ImageArrow;
		ArrowAction = new LayoutAction();
		animList = new LayoutAnimationList();
		animList.Add(new SpriteAnimation(new Sprite(image, 49, 60+1, 36, 200-5, 0, 0),
				0, 0, true, 0, 0, 0x88, 0, 0, 0x88));
		ArrowAction.Add("Arrow", animList);
		ArrowAction.SetCurrent("Arrow");
		ArrowAction.SetCenter("Arrow", 36/2,200-5);
		ArrowAction.ChangeActionPosition(427+(int)base_flickPoint.X-36/2, -((int)base_flickPoint.Y-240)-(200-5));			

		targetManager = new TargetManager();
		player        = new Player("Player");
		enemy         = new Enemy("Enemy");
		score         = new Score("Score");
		targetManager.Regist(player);
		targetManager.Regist(enemy);
		targetManager.Regist(score);
			
		scene = GameData.StageNum%3;

		// ステージ設定
		switch(scene)
		{
		case (int)SceneId.Scene00:
			stage  = new Stage00();
			break;
		case (int)SceneId.Scene01:
			stage = new Stage01();
			break;
		case (int)SceneId.Scene02:
			stage = new Stage02();		
			break;
		}
		
		stage.throwCnt = 0;	
		Stage.flickBasePos = base_flickPoint;
		sceneBodiesNum      = stage.NumBody;
		objOldLen      = new float[sceneBodiesNum];
		objCnt         = new int[sceneBodiesNum];
		objActiveCnt   = new int[sceneBodiesNum];

		// スコア表示マネージャー
		scoreEffectMgr = new TargetManager();

		for(int i=0; i<sceneBodiesNum; i++)	
		{
			name = "0"+i;
			scoreEffectMgr.Regist(new ScoreEffect(name));
			Vector2 pos = stage.GetsceneBodiesPosition(i);
			scoreEffectMgr.FindTarget(name).InitPos((int)pos.X - windowX, -((int)pos.Y - 240));	
			scoreEffectMgr.FindTarget(name).Score = 10;
			scoreEffectMgr.FindTarget(name).isCollision=false;
			scoreEffectMgr.FindTarget(name).isFirstCollision=false;
			scoreEffectMgr.FindTarget(name).isFirstScore=false;

			objOldLen[i]      = 0;
			objCnt[i]         = 0;
			objActiveCnt[i]   = 0;
		}
			
		// AudioManagerの設定
		InitAudioManager();

        AudioManager.PlayBgm("Stage");

		return true;
	}
	
    /// シーンの破棄
    public override void End()
    {
		Graphics2D.ClearSprite();

		if(messageAction != null){
			messageAction.Dispose();
			messageAction = null;
		}
		if(requireAction != null){
			requireAction.Dispose();
			requireAction = null;
		}
		if(ArrowAction != null){
			ArrowAction.Dispose();
			ArrowAction = null;
		}
		if(ArrowTargetAction != null){
			ArrowTargetAction.Dispose();
			ArrowTargetAction = null;
		}
		if(ArrowEndAction != null){
			ArrowEndAction.Dispose();
			ArrowEndAction = null;
		}

		if(player != null){ 
			player.Dispose();
			player = null;
		}
		if(enemy != null){
			enemy.Dispose();
			enemy = null;
		}
		if(score != null){
			score.Dispose();
			score = null;
		}

		if(requireScoreLayout != null){
			requireScoreLayout.Dispose();
			requireScoreLayout = null;
		}

		if(targetManager != null){
			targetManager.Dispose();
			targetManager = null;
		}
		if(scoreEffectMgr != null){
			scoreEffectMgr.Dispose();
            scoreEffectMgr = null;
		}
		if(stage != null){
			stage.ReleaseScene();
			stage = null;
		}

		AudioManager.Clear();

		GC.Collect();

	}
	
    /// フレーム処理
    public override bool Frame()
    {		
		// タッチ、パッド処理
		var pad = InputManager.InputGamePad;
		var touch = InputManager.InputTouch;
			
		if(main_state == (int)MainState.Ready)
		{
			if(backFlag){
				windowX -= 12;
				if(windowX < -854){
					windowX = -854;
					ready_cnt++;
					
					// 更新
					messageAction.Update(GameData.FrameTimeMillis);
			
			        if (messageAction.CurrentKey == "In" && messageAction.IsPlayEnd()) {
			            messageAction.ChangeCurrent("Out");
			        }else if(messageAction.CurrentKey == "Out" && messageAction.IsPlayEnd()){
			            messageAction.ChangeCurrent("In");
					}
		
					if(ready_cnt > 90){
						main_state=1;
						backFlag = false;
					}						
				}
			}else{
				windowX+=12;
				if(windowX >= 0){
			    	windowX = 0;
					backFlag =true;
				}
			}
			
			if ( pad.Trig != 0 || touch.GetInputNum() > 0 ) {
				if(backFlag){
					if(main_state==0 && ready_cnt > GameData.TargetFps/2){
						main_state=1;
						backFlag = false;
						AudioManager.PlaySound("Press");
					}
					else if(windowX > -854){
						windowX = -854;
						AudioManager.PlaySound("Press");
					}
				}else{
					if(main_state==0)
					{
						backFlag = true;
						windowX = -854;
						AudioManager.PlaySound("Press");
					}
				}
					
			}
		}
		else if(main_state == (int)MainState.Play)
		{
			#if DEBUG
			if(!mouse_move)
			{	
				// やり直し
				if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ||
					(pad.Scan & DemoGame.InputGamePadState.R) != 0	)
				{
					Retry();
				}
				// 次画面遷移
				else if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0)
				{
					if(!isFan){
						AudioManager.StopSound();
						AudioManager.StopBgm("Stage");
			        	AudioManager.PlayBgm("Fanfare", false, 1.0f);
						isFan = true;
					}

					SetNextScene(new SceneResult());
				}
				// 左スライド
				else if( stage.throwCnt == 0 && (pad.Scan & (DemoGame.InputGamePadState.Left )) != 0)
				{
					// スクリーンの移動
					windowX -= 12;
					if(windowX < -854){
						windowX = -854;
					}
				}
				// 右スライド
				else if( stage.throwCnt == 0 && (pad.Scan & (DemoGame.InputGamePadState.Right )) != 0)
				{
					// スクリーンの移動
					windowX += 12;
					if(windowX > 0){
						windowX = 0;
					}
				}
				// Lボタンで画面位置初期化
				else if( stage.throwCnt == 0 && (pad.Scan & DemoGame.InputGamePadState.L) != 0)
				{
					windowX = -854;
				}				
			}
			#endif
			
			// タッチパネルイベントを取る
			List<TouchData> touch_data_list = Touch.GetData(0);

			if ( touch_data_list.Count > 0 ){					
				// １回投げていたらもうフリックしない
				if( stage.throwCnt == 0 ){
					
					foreach (TouchData data in touch_data_list)
					{							
						old_touchX = touch_point.X;
						touch_point = GetClickPos((float)data.X, (float)data.Y);
							
						if(!slide_move)
						{
							// 指定位置（プレイヤ）の範囲からフリック開始
							if(windowX == -854){
								if(mouse_move){
									// プレイヤの回転設定
									float len = Math.Abs( base_flickPoint.X - touch_point.X);
									if(len < 1.0f)
										player.action_swing.ChangeActionPlayTimeMillis(200);
									else{
										int par = (int)(200 - len/2.5f);
										if(par<=1)par =1;
										player.action_swing.ChangeActionPlayTimeMillis(par);
									}
								}else{
									// フリック開始
									if((int)(window_width/2+window_width*data.X-(player_width/2)) < 100 &&
								   		(int)(window_height/2 + window_height*data.Y-(player_height/2)) > 200)
									{
										mouse_move = true;
										player.state = (int)Player.StateId.Swing;
										AudioManager.PlaySound("Swing",true);
									}
	
									// プレイヤの回転設定
									if(player.state == (int)Player.StateId.Swing)
									{
										float len = Math.Abs( base_flickPoint.X - touch_point.X);
																					
										if(len < 1.0f)
											player.action_swing.ChangeActionPlayTimeMillis(200);
										else{
											int par = (int)(200 - len/2.5f);
											if(par<=1)par =1;
											
											player.action_swing.ChangeActionPlayTimeMillis(par);
										}
									}
								}
							}
						}

						// スライド時は初回を記憶　フリック中は更新
						if(!slide_move){
							old_touchX = touch_point.X;
							if(!mouse_move){
								slide_move = true;
							}
						}

						// タッチによるスライド
						if(slide_move)
						{
							windowX = old_screenX + (int)(old_touchX - touch_point.X); 
							if(windowX < -854)
								windowX = -854;
							else if(windowX > 0)
								windowX = 0;

							old_screenX = windowX;
						}
					}
				}
				// 投げた後のタッチによるスライド
				else
				{
					foreach (TouchData data in touch_data_list)
					{
						old_touchX = touch_point.X;
						touch_point = GetClickPos((float)data.X, (float)data.Y);
						// スライド時は初回を記憶　フリック中は更新
						if(!slide_move){
							old_touchX = touch_point.X;
							if(!mouse_move){
								slide_move = true;
								slide_old_move = true; // 投げた後フリックした場合は追従させないフラグ
							}
						}

						// タッチによるスライド
						if(slide_move)
						{
							windowX = old_screenX + (int)(old_touchX - touch_point.X); 
							if(windowX < -854){
								windowX = -854;
							}else if(windowX > 0){
								windowX = 0;
							}
							old_screenX = windowX;
						}
					}
				}
				
				// ダブルタップ処理
				if(!mouse_move){		
					// ダブルタップ判定開始
					if(!first_touch_flag){
						first_touch_flag  = true;
						double_touch_cnt  = 0;
					}else{
						if(second_touch_flag)
							double_touch_flag  = true;
					}
				}

			}else{						
				// 投げる処理
				if(mouse_move){
					if(!stage.ThrowFrag && stage.ThrowCnt < 1){
						stage.ThrowCnt += 1;					
						stage.ThrowFrag = true;
						Stage.flickCurrPos = new Vector2(touch_point.X, touch_point.Y);
						enemy.state = (int)Enemy.StateId.Throw;
						AudioManager.StopSound("Swing");
						AudioManager.PlaySound("Throw");
					}
					player.state = (int)Player.StateId.Throw;			
				}

				if(!mouse_move)
				{		
					// タッチを離したフラグ取得
					if(first_touch_flag && !second_touch_flag){
						second_touch_flag = true;
					}
					// ダブルタップ判定
					else if(second_touch_flag && double_touch_flag)
					{
						if(double_touch_cnt>0 && double_touch_cnt < GameData.TargetFps/2)
						{
							// やり直し
							Retry();
							first_touch_flag  = false;
							second_touch_flag = false;
							double_touch_flag = false;
						}
					}
				}

				old_screenX = windowX;
				mouse_move  = false;
				slide_move  = false;
				
			}
			
			// ダブルタップカウント
			if(first_touch_flag || second_touch_flag || double_touch_flag){
				double_touch_cnt++;
				// 一定時間でフラグ初期化
				if(double_touch_cnt>=GameData.TargetFps/2){
					first_touch_flag  = false;
					second_touch_flag = false;
					double_touch_flag = false;
					double_touch_cnt = 0;	
				}	
			}

			click_pos = touch_point;
	
	        // Handle mouse interaction
	        if (mouse_move_old == false && mouse_move == true)
	        {
	            click_index = stage.CheckPicking(ref click_pos, ref diff_pos, false);
	        }
				
			// 投げている状態なら画面を自動的にスライド
			AutoSlideScreen();

		}
		// 結果判定状態
		else if(main_state == (int)MainState.Result)
		{
			// 次画面遷移
			if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0)
			{
				if(!isFan){
					AudioManager.StopSound();
					AudioManager.StopBgm("Stage");
		        	AudioManager.PlayBgm("Fanfare", false, 1.0f);
					isFan = true;
				}
				else
				{
					if(CheckClear(GameData.StageNum)){
						SetNextScene(new SceneResult());
					}else{
						SetNextScene(new SceneFailed());
					}
				}
			}

			// タッチパネルイベントを取る
			List<TouchData> touch_data_list = Touch.GetData(0);

			if ( touch_data_list.Count > 0 )
			{
				foreach (TouchData data in touch_data_list)
				{
					old_touchX = touch_point.X;
					touch_point = GetClickPos((float)data.X, (float)data.Y);
					// スライド時は初回を記憶　フリック中は更新
					if(!slide_move){
						old_touchX = touch_point.X;
						if(!mouse_move){
							slide_move = true;
							slide_old_move = true; // 投げた後フリックした場合は追従させないフラグ
						}
					}

					// タッチによるスライド
					if(slide_move)
					{
						windowX = old_screenX + (int)(old_touchX - touch_point.X); 
						if(windowX < -854){
							windowX = -854;
						}else if(windowX > 0){
							windowX = 0;
						}
						old_screenX = windowX;
					}
				}				
			}
			else
			{
				// 左までスライド
				if(isFan && !slide_old_move)
				{
					windowX -= 6;
					if(windowX <= -854)
					{
						windowX = -854;
					}
				}
					
				old_screenX = windowX;
				mouse_move  = false;
				slide_move  = false;
			}
		}

		// 画面のスクロールに応じて位置をスクロール
		stage.Scroll((-854 -windowX));
		targetManager.Scroll((-854 -windowX));
				
		// 剛体のシミュレーション
		stage.Simulate(click_index, click_pos, diff_pos);

		targetManager.Update();
		scoreEffectMgr.Update();
						
		// 作業員のレイアウトを合わせる
		if(stage.ThrowCnt != 0)
		{
			if(!stage.throwFlag)
			{
				Vector2 pos = stage.GetsceneBodiesPosition(stage.throwObjIdx);
				enemy.InitPos( 854 + (int)(pos.X*(1/stage.scene_scale)), -((int)(pos.Y*(1/stage.scene_scale)) - 240));
				enemy.action.SetDegree(stage.GetsceneBodiesRotation(stage.throwObjIdx)-0.5f);//斜めの作業員なので補正
			}
		}

		// 衝突してモノに速力が発生しているかチェック
		if(stage.ThrowCnt != 0)
			CollisionCheckAddScore();
				
		// スコアエフェクトの画面スクロール更新
		if(stage.ThrowCnt != 0)
			scoreEffectMgr.Scroll((-854 -windowX));


		// 静止させるかどうか
		if(main_state == (int)MainState.Play && player.state == (int)Player.StateId.Throw){
			CheckIsSetKinematic();
		}
		else if(main_state == (int)MainState.Result)
		{
			// 一定の閾値を下回ったら終了して次画面に遷移
			if(!isFan)
			{
				if(CheckIsSetKinematic())
				{	
					if(CheckClear(GameData.StageNum))
					{
						if(!isFan)
						{
							AudioManager.StopSound();
							AudioManager.StopBgm("Stage");
				        	AudioManager.PlayBgm("Fanfare", false, 1.0f);
							isFan = true;
						}
						SetNextScene(new SceneResult());
					}
					else
					{
						SetNextScene(new SceneFailed());
						isFan = true;
					}
				}
			}	
		}
			
		GameData.WindowPosX = windowX; // スライドの値記憶

		return true;
	}
	
	/// 衝突後、静態にするかどうか判定
	public bool CheckIsSetKinematic()
	{
		bool isNext = true;
		for(int i=0; i<stage.NumBody;i++)
		{
			name = "0"+i;			
			if(scoreEffectMgr.FindTarget(name).isFirstCollision || i==stage.throwObjIdx)
			{
				// 物体の重さがUtility.FLT_MAXのものは省く
				if(stage.sceneBodies[i].mass != PhysicsUtility.FltMax && objActiveCnt[i] <= GameData.TargetFps*7 || objCnt[i] <= GameData.TargetFps*7)
				{
					Vector2 pos = stage.GetsceneBodiesPosition(i);
					float len = Math.Abs(objOldLen[i]-pos.Length());
										
					objOldLen[i]=pos.Length();
					if(len < 2.8*stage.scene_scale && len >= 0.0f)
					{
						if(objActiveCnt[i] <= GameData.TargetFps*7){
							objActiveCnt[i] = 0;
						}

						// 静止タイミングカウント
						objCnt[i]++;
						if(objCnt[i] > GameData.TargetFps*7)
						{
							stage.SetsceneBodiesKinematic(i);
						}
						else{
							isNext = false;
						}
					}
					else if(len >= 15*stage.scene_scale)
					{
						if(objCnt[i] <= GameData.TargetFps*7){
							objCnt[i] = 0;
							isNext = false;							
						}

						// 動きつつけているものをカウント
						objActiveCnt[i]++;
						if(objActiveCnt[i] > GameData.TargetFps*7)
						{
							stage.SetsceneBodiesKinematic(i);
						}
					}
				}
			}
		}
		return isNext;
	}
		
		
	/// 衝突チェックとスコア加算
	public void CollisionCheckAddScore()
	{
		for(int i = 3; i<sceneBodiesNum; i++)
		{
			name = "0"+i;
			Vector2 vel = stage.GetsceneBodiesVelocity(i);

			// 作業員、一度衝突したものには重力を加算する
			if( i == stage.throwObjIdx ||
			    15*stage.scene_scale < vel.Length() || 
				scoreEffectMgr.FindTarget(name).isFirstCollision)
			{
				// 作業員
				if(i == stage.throwObjIdx)
				{
					if(main_state != (int)MainState.Result)
					{
						if(stage.sceneBodies[i].mass != PhysicsUtility.FltMax)
						{
							// 落下スピード加算
							stage.sceneBodies[i].force = new Vector2(0, -9.8f * 2.0f) * stage.sceneBodies[i].mass;
						}
					}
					// Stage3 落下時
					if(!isfall && scene == (int)SceneId.Scene02 && 
					    stage.sceneBodies[i].position.X*(1/stage.scene_scale) > (1350-854) && 
					    stage.sceneBodies[i].position.X*(1/stage.scene_scale) < (1515-854) &&
					    stage.sceneBodies[i].position.Y*(1/stage.scene_scale) < -(480-240))
					{
						AudioManager.PlaySound("Throw");
						scoreEffectMgr.FindTarget("03").Score = 300;
						score.AddScore(scoreEffectMgr.FindTarget("03").Score);//
						scoreEffectMgr.FindTarget("03").InitPos ( 854 + (int)(stage.sceneBodies[i].position.X*(1/stage.scene_scale)),-((int)(stage.sceneBodies[i].Position.Y*(1/stage.scene_scale)) - 240)-30);
						scoreEffectMgr.FindTarget("03").isCollision = true;
						scoreEffectMgr.FindTarget("03").isFirstCollision=true;

						isfall= true;
					}
				}
				else
				{	
						
					if(stage.sceneBodies[i].Position.Y*(1/stage.scene_scale) > -200 || 
					  (scene == (int)SceneId.Scene02 && 
						stage.sceneBodies[i].position.X*(1/stage.scene_scale) > (1350-854) && stage.sceneBodies[i].position.X*(1/stage.scene_scale) < (1515-854)))
					{
						// ジョイントいている剛体
						if((scene == (int)SceneId.Scene00 && (i==54 || i==55 || i==56)) ||
						   (scene == (int)SceneId.Scene01 && (i > 36 && i < 44 )) ||
						   (scene == (int)SceneId.Scene02 && (i==29 || i==30 || i==31)))
						{
							// シャベルのみ重力加算
							if(scene == (int)SceneId.Scene01 && (i > 36 && i < 44 ))
							{
								// 落下スピード加算		
								stage.sceneBodies[i].force = new Vector2(0, -9.8f * 2.0f) * stage.sceneBodies[i].mass;
									
							}		
						}
						// それ以外				
						else
						{
							if(stage.sceneBodies[i].mass != PhysicsUtility.FltMax)
							{
								// 落下スピード加算
								stage.sceneBodies[i].force = new Vector2(0, -9.8f * 2.0f) * stage.sceneBodies[i].mass;
							}
						}
					}

					//Stage3 ドラム缶落下時
					if( i==31 && !isfall_drum && scene == (int)SceneId.Scene02 && 
					    stage.sceneBodies[i].position.X*(1/stage.scene_scale) > (1350-854) && 
					    stage.sceneBodies[i].position.X*(1/stage.scene_scale) < (1515-854) &&
					    stage.sceneBodies[i].position.Y*(1/stage.scene_scale) < -(480-240))
					{
						AudioManager.PlaySound("021");
						scoreEffectMgr.FindTarget("031").Score = 200;
						score.AddScore(scoreEffectMgr.FindTarget("031").Score);//
						scoreEffectMgr.FindTarget("031").InitPos ( 854 + (int)(stage.sceneBodies[i].position.X*(1/stage.scene_scale)),-((int)(stage.sceneBodies[i].Position.Y*(1/stage.scene_scale)) - 240)-30);
						scoreEffectMgr.FindTarget("031").isCollision = true;
						scoreEffectMgr.FindTarget("031").isFirstCollision=true;

						isfall_drum = true;
					}
				}
			}
			// 貫通した際速力がなく判別できないため例外処理
			else if((stage.isCrane_st1 && scene == (int)SceneId.Scene00 && i == 57) ||
					(stage.isCrane_st3 && scene == (int)SceneId.Scene02 && i == 31))
			{
				// stage1資材 stage3ドラム缶
				if(stage.sceneBodies[i].mass != PhysicsUtility.FltMax)
				{
					// 落下スピード加算
					stage.sceneBodies[i].force = new Vector2(0, -9.8f * 2.0f) * stage.sceneBodies[i].mass;
				}
			}

			// スコア加算
			if( i != stage.throwObjIdx )
			{
				// レンガでは常に加算されてしまうと不具合	
				if((scene == (int)SceneId.Scene02) && (i>=12) && (i<=26))
				{
					if((border_line < vel.Length()) && scoreEffectMgr.FindTarget(name).Score < 20)
					{	
						if(!scoreEffectMgr.FindTarget(name).isCollision){
							CollisionSound(i);
	
							// 通常
							if(scoreEffectMgr.FindTarget(name).Score < 20)
							{
								if(scoreEffectMgr.FindTarget(name).isFirstScore){
									scoreEffectMgr.FindTarget(name).Score = 20;
								}else{
									scoreEffectMgr.FindTarget(name).Score = 10;
									
								}	
								score.AddScore(scoreEffectMgr.FindTarget(name).Score);//得点は仮、CollisionSoundで得点設定or加算
			
								scoreEffectMgr.FindTarget(name).InitPos ( 854 + (int)(stage.sceneBodies[i].position.X*(1/stage.scene_scale)),-((int)(stage.sceneBodies[i].position.Y*(1/stage.scene_scale)) - 240));
								scoreEffectMgr.FindTarget(name).isCollision=true;
								scoreEffectMgr.FindTarget(name).isFirstCollision=true;
								scoreEffectMgr.FindTarget(name).isFirstScore=true;
							}
							// CollisionSound()で高得点に変更した場合
							else
							{
									// Do nothing here
							}
						}
					}
				
			
				}
					
				// 一定値超えていれば得点 //壁(i=0,1)はむし
				else if( i>2 && (border_line < vel.Length()) && scoreEffectMgr.FindTarget(name).Score < 40)
				{
					if(!scoreEffectMgr.FindTarget(name).isCollision){
						CollisionSound(i);

						// 通常
						if(scoreEffectMgr.FindTarget(name).Score < 40)
						{
							if(scoreEffectMgr.FindTarget(name).isFirstScore && scoreEffectMgr.FindTarget(name).Score == 20){
								scoreEffectMgr.FindTarget(name).Score = 40;						
							}else if(scoreEffectMgr.FindTarget(name).isFirstScore){
								scoreEffectMgr.FindTarget(name).Score = 20;
							}else{
								scoreEffectMgr.FindTarget(name).Score = 10;
							}	
							score.AddScore(scoreEffectMgr.FindTarget(name).Score);//得点は仮、CollisionSoundで得点設定or加算
		
							scoreEffectMgr.FindTarget(name).InitPos ( 854 + (int)(stage.sceneBodies[i].position.X*(1/stage.scene_scale)),-((int)(stage.sceneBodies[i].position.Y*(1/stage.scene_scale)) - 240));
							scoreEffectMgr.FindTarget(name).isCollision=true;
							scoreEffectMgr.FindTarget(name).isFirstCollision=true;
							scoreEffectMgr.FindTarget(name).isFirstScore=true;
						}
						// CollisionSound()で高得点に変更した場合
						else
						{
							score.AddScore(scoreEffectMgr.FindTarget(name).Score);//得点は仮、CollisionSoundで得点設定or加算	
							scoreEffectMgr.FindTarget(name).InitPos ( 854 + (int)(stage.sceneBodies[i].position.X*(1/stage.scene_scale)),-((int)(stage.sceneBodies[i].position.Y*(1/stage.scene_scale)) - 240));
							scoreEffectMgr.FindTarget(name).isCollision=true;
							scoreEffectMgr.FindTarget(name).isFirstCollision=true;
						}
					}
				}else{
					// 特別な点数獲得が終了した場合	スコアを戻す
					if(!scoreEffectMgr.FindTarget(name).isCollision && scoreEffectMgr.FindTarget(name).Score > 40){
						if(scoreEffectMgr.FindTarget(name).isFirstScore){
							scoreEffectMgr.FindTarget(name).Score = 20;
						}else{
							scoreEffectMgr.FindTarget(name).Score = 10;
						}
					}							
				}	
			}
		}
			
		// 看板判定条件初期化
		isSignCol = false;
		isSignPartsCol = false;
	}
		
	/// 衝突時のサウンド
	/// このときidxによって特別な得点の設定を行なう場合あり
	/// @param idx budiesのindex
	private void CollisionSound(int idx)
	{
		if(scene == (int)SceneId.Scene00){
			if(4 <= idx && idx<9){// 資材
				s27_counter++;
				if(s27_counter > 9) s27_counter = 0;
				string key = "S27_"+s27_counter;
				AudioManager.PlaySound(key,false);
			}else if(9 <= idx && idx<16){// 看板
				if(!isSignPartsCol && (11 <= idx && idx<16)){// 看板部品は一回鳴らす
					AudioManager.PlaySound("S24");
					isSignPartsCol = true;
				}else if(!isSignCol && (9 <= idx && idx<11)){// 看板は２つでひとつ
					AudioManager.PlaySound("S24");
					isSignCol = true;
				}
			}else if(16 <= idx && idx<22){//トラック
				//AudioManager.PlaySound("S25"); Stage00クラスで鳴らす
			}else if(22 <= idx && idx<25+2){// トラック上の資材
				s27_counter++;
				if(s27_counter > 9) s27_counter = 0;
				string key = "S27_"+s27_counter;
				AudioManager.PlaySound(key,false);
			}else if(25 <= idx && idx < 27+2){// 三角コーン
				AudioManager.PlaySound("S23");
			}else if(27 <= idx && idx < 52+2){// 形鋼
				AudioManager.PlaySound("S25");
				string key = "0"+idx;
				scoreEffectMgr.FindTarget(key).isFirstCollision = true;
			}else if(52+2 <= idx && idx < 56+2){// クレーン
				if(idx == 57)
				{
					AudioManager.PlaySound("S27");
					if(!isfall_sack){
						scoreEffectMgr.FindTarget("057").Score = 200;
						isfall_sack = true;
					}
				}else{
					AudioManager.PlaySound("S22");	
				}
				//クレーンに当たったら資材落とす
				scoreEffectMgr.FindTarget("057").isFirstCollision = true;
			}
		}else if(scene == (int)SceneId.Scene01){
			if(7 <= idx && idx<23){//セメント//砂利
				s26_counter++;
				if(s26_counter > 5) s26_counter = 0;
				string key = "S26_"+s26_counter;
				AudioManager.PlaySound(key,false);
			}else if(23 <= idx && idx<33){//コンクリート
				s27_counter++;
				if(s27_counter > 9) s27_counter = 0;
				string key = "S27_"+s27_counter;
				AudioManager.PlaySound(key,false);
			}else if(33 <= idx && idx<36){//ドラム缶
				AudioManager.PlaySound("S21");
				// ドラム缶に衝突したらヘルメットも落下させる
				scoreEffectMgr.FindTarget("036").isFirstCollision = true;
			}else if(36 == idx ){//ヘルメット
				AudioManager.PlaySound("S23");
			}else if(37 <= idx && idx<44){//ショベル
				AudioManager.PlaySound("S25");
				if(!isCrash_shavel){
					scoreEffectMgr.FindTarget("043").Score = 200;
					isCrash_shavel = true;
				}
			}
		}else if(scene == (int)SceneId.Scene02){
			if(4 <= idx && idx<7){// ごみ箱 シャベル
				AudioManager.PlaySound("S23");				
			}else if(7 <= idx && idx<12){// ドラム缶
				AudioManager.PlaySound("S21");
			}else if(12 <= idx && idx<28){// 壁（レンガ）
				s27_counter++;
				if(s27_counter > 9) s27_counter = 0;
				string key = "S27_"+s27_counter;
				AudioManager.PlaySound(key,false);
			}else if(28 <= idx && idx<31){// クレーン
				AudioManager.PlaySound("S22");
				scoreEffectMgr.FindTarget("031").isFirstCollision = true;//クレーンに当たったら衝突とする
			}else if(31 == idx){// クレーンのドラム缶
				AudioManager.PlaySound("S21");
			}else if(32 <= idx && idx<48){// 足場
				AudioManager.PlaySound("S24");
				if(scoreEffectMgr.FindTarget("034").isFirstCollision)
					scoreEffectMgr.FindTarget("033").isFirstCollision = true;
				if(scoreEffectMgr.FindTarget("038").isFirstCollision)
					scoreEffectMgr.FindTarget("037").isFirstCollision = true;
			}
		}
	}	
	
	/// 投げたあと自動でスライド
	/// @param scene 現在のステージ番号
	private void AutoSlideScreen()
	{
		// 投げられてから判定開始
		if( stage.ThrowCnt == 1 && player.state == (int)Player.StateId.Throw){			
			if(backFlag){
				slide_cnt = 0;
				main_state = (int)MainState.Result;
			}else{
				Vector2 pos = stage.GetsceneBodiesPosition(stage.throwObjIdx);
				// 作業員を追従、一定時間でスライド
				slide_cnt++;
				if(slide_cnt > GameData.TargetFps*8)
				{
					// 右まで行ききってないならいく,タッチのスライドをしていた場合は移動はしない
					if(windowX < 0 && !slide_old_move)
					{
						windowX += 8;
						if(windowX >= 0)
							windowX = 0;
					}
					else if(slide_cnt > GameData.TargetFps*stop_time)
					{
						backFlag = true;
					}
				}else{
					if(pos.X*(1/stage.scene_scale) < -854/2){
						// 位置はそのままを維持
					}else if(pos.X*(1/stage.scene_scale) > -854/2 && pos.X*(1/stage.scene_scale) < 854/2){
						// タッチのスライドをしている場合は移動はしない
						if(!slide_old_move)
						{
							if(windowX < (int)(-854/2 + pos.X*(1/stage.scene_scale)))
								windowX = (int)(-854/2 + pos.X*(1/stage.scene_scale));
							if(windowX > 0)
								windowX = 0;
						}
					}
					else if(pos.X*(1/stage.scene_scale) > 854/2)
					{
						// タッチのスライドをしている場合は移動はしない
						if(!slide_old_move)
						{
							windowX = 0;
						}
					}
				}
					
				// 画面外なら判定して終了
				if(( pos.X*(1/stage.scene_scale) > 854+50  || pos.X*(1/stage.scene_scale) < -854-50))
				{
					end_cnt++;
					if(end_cnt > GameData.TargetFps*5)
					{
						main_state = (int)MainState.Result;// 結果判定へ
					}
				}
			}
		}
	}
		
    /// 描画
    public override bool Draw()
    {
		// ステージのオブジェクトレンダリング
		stage.ObjectDraw();
		
		// プレイヤ、作業員、総スコア表示
		targetManager.Render();
			
		// スコア表示
		if(stage.ThrowCnt != 0)
			scoreEffectMgr.Render();

		if(main_state == (int)MainState.Ready){
			if(backFlag && ready_cnt>0){
				messageAction.Render();
			}else{
				requireAction.Render();
				if(GameData.StageNum == 0)
					requireScoreLayout.Render((int)StageClearScore.Stage00);
				else if(GameData.StageNum == 1)
					requireScoreLayout.Render((int)StageClearScore.Stage01);
				else if(GameData.StageNum == 2)
					requireScoreLayout.Render((int)StageClearScore.Stage02);
				else if(GameData.StageNum == 3)
					requireScoreLayout.Render((int)StageClearScore.Stage10);
				else if(GameData.StageNum == 4)
					requireScoreLayout.Render((int)StageClearScore.Stage11);
				else if(GameData.StageNum == 5)
					requireScoreLayout.Render((int)StageClearScore.Stage12);
			}
		}
		
		// 矢印描画
		if(main_state == (int)MainState.Play && mouse_move)
		{
			//先端位置移動
			ArrowTargetAction.ChangeActionPosition(427+(int)touch_point.X-36/2, -((int)touch_point.Y-240));			
			//フリックの長さに応じてスケールを計算
			float len = CalcFlickLength(touch_point);
			ArrowAction.SetScaleY("Arrow", (len-36)/(200-5));
			
			//角度計算
			float degree = (float)CalcFlickAngle(touch_point);
			//角度設定
			ArrowAction.SetDegree(degree-1.572f);//-90度加える
			ArrowTargetAction.SetDegree(degree-1.572f);//-90度加える
			ArrowEndAction.SetDegree(degree-1.572f);//-90度加える
			
			if(len > 36.0f){//矢印より小さい場合は描画しない
				ArrowAction.Render();
				ArrowEndAction.Render();
			}
			ArrowTargetAction.Render();
		}
		
		mouse_move_old = mouse_move;
		return true;
	}
		
	/// 規程スコアをクリアできたか
	public bool CheckClear(int stageNum)
	{
		bool clearFlag = false;
		switch(stageNum)
		{
		case 0:
			if(score.TotalScore >= (int)StageClearScore.Stage00) clearFlag = true;
			break;
		case 1:
			if(score.TotalScore >= (int)StageClearScore.Stage01) clearFlag = true;
			break;
		case 2:
			if(score.TotalScore >= (int)StageClearScore.Stage02) clearFlag = true;
			break;
		case 3:
			if(score.TotalScore >= (int)StageClearScore.Stage10) clearFlag = true;
			break;
		case 4:
			if(score.TotalScore >= (int)StageClearScore.Stage11) clearFlag = true;
			break;
		case 5:
			if(score.TotalScore >= (int)StageClearScore.Stage12) clearFlag = true;
			break;
		}
		return clearFlag;
	}

	/// クリックの位置取得
	static Vector2 GetClickPos(float x, float y){
        return new Vector2(x * (window_width / 2.0f) / (0.5f * rendering_scale),
            -y * (window_height / 2.0f) / (0.5f * rendering_scale));
    }
		
	/// フリックの角度取得
	/// mouse_moveがtrueのとき計算すること
	public double CalcFlickAngle(Vector2 pos){
		float dx = (427 + pos.X) - (427 + base_flickPoint.X);
		float dy = (pos.Y+240) - (base_flickPoint.Y+240);
		double angle = Math.Atan2(dy, dx);
		return angle;
	}

	/// フリックの距離取得
	/// mouse_moveがtrueのとき計算すること
	public float CalcFlickLength(Vector2 pos){
		float dx = (427 + pos.X) - (427 + base_flickPoint.X);
		float dy = (pos.Y+240) - (base_flickPoint.Y+240);
		float len = (float)System.Math.Sqrt(dx * dx + dy * dy);
		return len;
	}
		
	/// やり直し
	public void Retry(){
		//画面初期化
		stage.ReleaseScene();
		score.ClearScore();
			
		windowX           = -854;
		ready_cnt         = 0;
		slide_cnt         = 0;
		backFlag          = false;
		player.state      = (int)Player.StateId.Stay;
		enemy.state       = (int)Enemy.StateId.Stay;
		main_state        = (int)MainState.Play;
		isFan             = false;
		isfall            = false;
		isfall_drum       = false;
		isfall_sack       = false;
		isCrash_shavel    = false;
		isSignPartsCol    = false;
		isSignCol         = false;
		slide_move        = false;
		slide_old_move    = false;
		end_cnt           = 0;
		first_touch_flag  = false;
	    double_touch_flag = false;
	    double_touch_cnt  = 0;

		GC.Collect();
				
		scene = GameData.StageNum%3;

		switch(scene)
		{
		case (int)SceneId.Scene00:
			stage = new Stage00();
			break;
		case (int)SceneId.Scene01:
			stage = new Stage01();
			break;
		case (int)SceneId.Scene02:
			stage = new Stage02();
			break;
		}
		stage.throwCnt = 0;
		Stage.flickBasePos = base_flickPoint;

		for(int i=0; i<stage.NumBody;i++)
		{
			name = "0"+i;
			Vector2 pos = stage.GetsceneBodiesPosition(i);
			scoreEffectMgr.FindTarget(name).InitPos((int)(pos.X*(1/stage.scene_scale)) - windowX,-((int)(pos.Y*(1/stage.scene_scale)) - 240));
			scoreEffectMgr.FindTarget(name).Score = 10;
			scoreEffectMgr.FindTarget(name).isCollision = false;
			scoreEffectMgr.FindTarget(name).isFirstCollision = false;
			scoreEffectMgr.FindTarget(name).isFirstScore = false;
			
			objOldLen[i] = pos.Length();
			objCnt[i] = 0;
			objActiveCnt[i] = 0;
		}
	}
		
	/// AudioManager設定
	private void InitAudioManager(){
        AudioManager.AddBgm("Fanfare", "/Application/res/data/snd/S31.mp3"); // ファンファーレ
        AudioManager.AddBgm("Stage", "/Application/res/data/snd/S42.mp3"); // ステージBGM
        AudioManager.AddSound("Swing", "/Application/res/data/snd/S11.wav"); // 振り回す
        AudioManager.AddSound("Throw", "/Application/res/data/snd/S12.wav"); // 振り飛ばす音
        AudioManager.AddSound("S21", "/Application/res/data/snd/S21.wav"); // ドラム缶
        AudioManager.AddSound("S22", "/Application/res/data/snd/S22.wav"); // クレーン
        AudioManager.AddSound("S23", "/Application/res/data/snd/S23.wav"); // 三角コーン,ヘルメット
        AudioManager.AddSound("S24", "/Application/res/data/snd/S24.wav"); // 看板
        AudioManager.AddSound("S25", "/Application/res/data/snd/S25.wav"); // トラック・形鋼、油圧ショベル
        AudioManager.AddSound("S26_0", "/Application/res/data/snd/S26.wav"); // 砂利,セメント
        AudioManager.AddSound("S26_1", "/Application/res/data/snd/S26.wav"); // 砂利,セメント
        AudioManager.AddSound("S26_2", "/Application/res/data/snd/S26.wav"); // 砂利,セメント
        AudioManager.AddSound("S26_3", "/Application/res/data/snd/S26.wav"); // 砂利,セメント
        AudioManager.AddSound("S26_4", "/Application/res/data/snd/S26.wav"); // 砂利,セメント
        AudioManager.AddSound("S26_5", "/Application/res/data/snd/S26.wav"); // 砂利,セメント
        AudioManager.AddSound("S27_0", "/Application/res/data/snd/S27.wav"); // コンクリート,資材
        AudioManager.AddSound("S27_1", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_2", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_3", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_4", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_5", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_6", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_7", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_8", "/Application/res/data/snd/S27.wav"); // コンクリート
        AudioManager.AddSound("S27_9", "/Application/res/data/snd/S27.wav"); // コンクリート
		AudioManager.AddSound("Press", "/Application/res/data/snd/S01.wav");
	}
	
}
	
} // Physics2dDemo
