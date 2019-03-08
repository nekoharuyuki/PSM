// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoModel;
using DemoGame;

namespace DefenseDemo{


public
class SceneGame
    : IScene
{
    private Texture2D image;
    private SceneManager useSceneMgr;
			
	private float tmpCamX = 0.0f;
	private float tmpCamY = 0.0f;
	private float tmpCamZ = 0.0f;
	private float camLookAtX = 0.0f;
	private float camLookAtY = 0.0f;
	private float camLookAtZ = 0.0f;
	private float testCamRotY = 0.0f;
	private float testCamRotX = 0.0f;
	private float testCamRotZ = 0.0f;
	private float tesetDist = 0.0f;
	private Vector3 camTrgPos = new Vector3(0.0f,0.0f,0.0f);
	private Ground ground = new Ground();
	private UnitManager unitManager = new UnitManager();
			
	private Grid grid = new Grid();
	private bool unitTouchFlg = false;
	private int selectUnitId = 0;
	private int keyWait = 0;
	private Stopwatch setEnemyTimer;
	private int waveCnt = 0;
	private int waveEnemyCnt = 0;
			
	private CameraInfo camInfo;
	private CommonUtil comUtil;
			
	private Random rand;
			
	private bool load2dFlg = false;
			
	private Stopwatch sceneTimer;
			
	private int mainPoints = 0;

	/// ゲーム画面内の状態遷移値
	private int gameState = 0;
	private int gameStatePrev = 0;
	private bool gameStateInitFlg = false;
	/// カメラを中心に向ける
	private static int GAMESTATE_CAM_CENTER = 0;
	/// アラートメッセージ表示
	private static int GAMESTATE_MESS_ALERT = 1;
	/// カメラを進入口に向ける
	private static int GAMESTATE_CAM_MOVE_START = 2;
	/// 戦闘目的メッセージ表示
	private static int GAMESTATE_MESS_DESTROY_ENEMY = 3;
	/// カメラを最終防衛ラインに向ける
	private static int GAMESTATE_CAM_MOVE_GOAL = 4;
	/// カメラが最終防衛ラインを映し、一時停止
	private static int GAMESTATE_CAM_MOVE_GOAL_WAIT = 5;
	/// カメラを再度進入口に向ける
	private static int GAMESTATE_CAM_MOVE_START2 = 6;
	/// 戦闘開始メッセージ表示
	private static int GAMESTATE_MESS_FIRE = 7;
	/// 敵侵入開始
	private static int GAMESTATE_ENEMY_START = 8;
	/// ボスユニット登場メッセージ表示
	private static int GAMESTATE_MESS_LARGE = 9;
	/// ボスユニット侵入開始
	private static int GAMESTATE_BOSS_START = 10;
	/// ボスユニット破壊メッセージ表示
	private static int GAMESTATE_MESS_DESTROYED = 11;
	/// 最終防衛ライン突破メッセージ表示
	private static int GAMESTATE_MESS_BROKEN = 12;
	/// ゲームクリアメッセージ表示
	private static int GAMESTATE_MESS_GAMECLEAR = 13;
	/// ゲームオーバーメッセージ表示
	private static int GAMESTATE_MESS_GAMEOVER = 14;
	private static int GAMESTATE_FADE_IN = 15;
	private static int GAMESTATE_FADE_OUT = 16;
	private static int GAMESTATE_FADE_OUT_TO_TITLE = 17;
	/// その他
	private static int GAMESTATE_NONE = 20;
	/// カメラ自動移動フラグ
	private bool gameStateCameraMoveFlg = false;
	/// ゲーム画面内の状態遷移時に使用する時間用一時領域
	private long gameStateTimeStart = 0;
	/// カメラ自動移動時の開始点
	private Vector3 gameStateCameraStart = new Vector3( 0.0f, 0.0f, 0.0f );
	/// カメラ自動移動時の終了点
	private Vector3 gameStateCameraEnd = new Vector3( 0.0f, 0.0f, 0.0f );
	/// カメラ自動移動に掛ける時間
	private long gameStateCameraMoveTime = 0;
			
	private int touchScrOldPosX = 0;
	private int touchScrOldPosY = 0;
	private float touchScrOldDistance = 0;
			
//	Matrix4 testLookAt;
	Posture testPosR;
			
	private bool gameResult = false;
	private EffectFade fade;
			
	private int cameraMoveFrame = 0;
	private bool cameraMoveFadeOut = false;
			
	Rout rout;
			
	private float setEnemyTimerCurrent;
				
    /// コンストラクタ
    public SceneGame()
    {
    }

    /// デストラクタ
    ~SceneGame()
    {
    }

    /// シーンの初期化
    public bool Init(SceneManager useSceneMgr)
    {
        this.useSceneMgr = useSceneMgr;
				
		sceneTimer = new Stopwatch();
		sceneTimer.Start();
				
		camInfo = CameraInfo.Inst();
		comUtil = CommonUtil.Inst();
				
		rand = new Random( DateTime.Now.Millisecond );

		load2dFlg = false;
				
		load2dData();
				
		load3dData();
				
		loadSoundData();

		grid.Init();
				
		unitManager.Init();
		unitManager.UnitInit();
		ground.Init();
				
		testCamRotX = -30.0f;
		testCamRotY = 0.0f;
		testCamRotZ = 0.0f;
		tesetDist = -200.0f;
		camTrgPos.X = camLookAtX;
		camTrgPos.Y = camLookAtY;
		camTrgPos.Z = camLookAtZ;
//		camInfo.GetPosture().SetLookAt(
		camInfo.GetPosture().SetLookAt2(
					new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
//					new Vector3( camLookAtX, camLookAtY, camLookAtZ ),
					camTrgPos,
					tesetDist );
				
//		triPos00.Init( CollisionStageData.Positions00 );
//		triPos50.Init( CollisionStageData.Positions50 );
				
		fade = EffectFade.GetInstance();
		fade.SetFadeIn( 0x000000, 10, true );
				
		setEnemyTimer = new Stopwatch();
		setEnemyTimer.Start();
				
		mainPoints = Unit.INIT_MAIN_POINT;
				
				
		setImageNumber( "num_main", mainPoints );
		setImageNumber( "num_y", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_yw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_g", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_WIDE_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_gw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_WIDE_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_r", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_MISSILE*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_rw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_MISSILE*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_b", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_HIGH_EXPLOSIVE*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_bw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_HIGH_EXPLOSIVE*Unit.DEF_PARAM_NUM)] );
				
		gameState = GAMESTATE_CAM_CENTER;
//		gameState = GAMESTATE_ENEMY_START;
//		gameState = GAMESTATE_NONE;
		gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
				
		fade = EffectFade.GetInstance();
				
		gameResult = false;
				
		AudioManager.PlayBgm( "BGM_GAME" );
				
		rout = new Rout();
		rout.Init();
				
		checkUnitImg();
				
		setEnemyTimerCurrent = -1.0f;
				
//		cameraMoveFrame = -1;
		cameraMoveFrame = 0;
		cameraMoveFadeOut = false;
				
        return true;
    }

    /// シーンの破棄
    public void Term()
    {
		AudioManager.Clear();
				
		setEnemyTimer.Stop();
		setEnemyTimer = null;

		unitManager.Term();
		unitManager = null;

		sceneTimer.Stop();
		sceneTimer = null;
				
		rout.Term();
		rout = null;

        // 所有権なし
        useSceneMgr = null;
        Graphics2D.ClearSprite();
		
//		useResCont = null;

    }

    /// シーンの継続切り替え時の再開処理
    public bool Restart()
    {
        return true;
    }

    /// シーンの継続切り替え時の停止処理
    public bool Pause()
    {
        return true;
    }

    /// サスペンド＆レジューム処理
    public void Suspend()
    {
    }

    public void Resume()
    {
    }

    /// フレーム処理
    public bool Update()
    {
        var pad = InputManager.InputGamePad;
       	var touch = InputManager.InputTouch;
				
		/// ゲーム画面内の状態遷移処理
		if( gameState >= 0 ){
			if( gameStateInitFlg ){
				/// ボスユニット登場メッセージ表示の準備
				if( gameState == GAMESTATE_MESS_LARGE ){
					AudioManager.StopBgm();
					AudioManager.PlayBgm( "BGM_BOSS" );
					SetMessageUI( "large" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
				}
				/// ボスユニット破壊メッセージ表示の準備
				else if( gameState == GAMESTATE_MESS_DESTROYED ){
					SetMessageUI( "destroyed" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
				}
				/// 最終防衛ライン突破メッセージ表示の準備
				else if( gameState == GAMESTATE_MESS_BROKEN ){
					SetMessageUI( "broken" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
				}
				/// ゲームクリアメッセージ表示の準備
				else if( gameState == GAMESTATE_MESS_GAMECLEAR ){
					AudioManager.StopSound();
					AudioManager.StopBgm();
					AudioManager.PlayBgm( "BGM_GAMECLEAR", false );
					SetMessageUI( "gameclear" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
				}
				/// ゲームオーバーメッセージ表示の準備
				else if( gameState == GAMESTATE_MESS_GAMEOVER ){
					AudioManager.StopSound();
					AudioManager.StopBgm();
					AudioManager.PlayBgm( "BGM_GAMEOVER", false );
					SetMessageUI( "gameover" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
				}
				else if( gameState == GAMESTATE_FADE_OUT ){
					fade.SetFadeOut( 0x000000, 10, true );
				}
				else if( gameState == GAMESTATE_FADE_IN ){
					fade.SetFadeIn( 0x000000, 10, true );
				}
				else if( gameState == GAMESTATE_FADE_OUT_TO_TITLE ){
					fade.SetFadeOut( 0x000000, 10, true );
				}
						
				gameStateInitFlg = false;
			}
			/// 状態：その他
			if( gameState == GAMESTATE_NONE ){
				gameStateInitFlg = true;
			}
			/// カメラを中心に向ける
			if( gameState == GAMESTATE_CAM_CENTER ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
					AudioManager.PlaySound( "SE_ALERT", true );
					SetMessageUI( "alert" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_MESS_ALERT;
					SetGameState( GAMESTATE_MESS_ALERT );
				}
			}
			/// アラートメッセージ表示
			else if( gameState == GAMESTATE_MESS_ALERT ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
					clearMessageUI();
					SetCameraMove(
//							new Vector3( camLookAtX, camLookAtY, camLookAtZ ),
							camTrgPos,
//							new Vector3( camLookAtX, camLookAtY, camLookAtZ + 100.0f ),
							new Vector3( camTrgPos.X, camTrgPos.Y, camTrgPos.Z - 200.0f ),
							5000 );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_CAM_MOVE_START;
					SetGameState( GAMESTATE_CAM_MOVE_START );
				}
			}
			/// カメラを進入口に向ける
			else if( gameState == GAMESTATE_CAM_MOVE_START ){
				runCameraMove();
				if( !gameStateCameraMoveFlg ){
					SetMessageUI( "destroy" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_MESS_DESTROY_ENEMY;
					SetGameState( GAMESTATE_MESS_DESTROY_ENEMY );
				}
			}
			/// 戦闘目的メッセージ表示
			else if( gameState == GAMESTATE_MESS_DESTROY_ENEMY ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
					clearMessageUI();
					SetCameraMove(
//							new Vector3( camLookAtX, camLookAtY, camLookAtZ ),
							camTrgPos,
//							new Vector3( camLookAtX, camLookAtY, camLookAtZ - 200.0f ),
							new Vector3( camTrgPos.X, camTrgPos.Y, camTrgPos.Z + 400.0f ),
							5000 );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_CAM_MOVE_GOAL;
					SetGameState( GAMESTATE_CAM_MOVE_GOAL );
				}
			}
			/// カメラを最終防衛ラインに向ける
			else if( gameState == GAMESTATE_CAM_MOVE_GOAL ){
				runCameraMove();
				if( !gameStateCameraMoveFlg ){
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_CAM_MOVE_GOAL_WAIT;
					SetGameState( GAMESTATE_CAM_MOVE_GOAL_WAIT );
				}
			}
			/// カメラが最終防衛ラインを映し、一時停止
			else if( gameState == GAMESTATE_CAM_MOVE_GOAL_WAIT ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
					clearMessageUI();
					SetCameraMove(
//							new Vector3( camLookAtX, camLookAtY, camLookAtZ ),
							camTrgPos,
//							new Vector3( camLookAtX, camLookAtY, camLookAtZ + 200.0f ),
							new Vector3( camTrgPos.X, camTrgPos.Y, camTrgPos.Z - 300.0f ),
							5000 );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_CAM_MOVE_START2;
					SetGameState( GAMESTATE_CAM_MOVE_START2 );
				}
			}
			/// カメラを再度進入口に向ける
			else if( gameState == GAMESTATE_CAM_MOVE_START2 ){
				runCameraMove();
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 7000 ){
//				if( !gameStateCameraMoveFlg ){
					SetMessageUI( "firing" );
					gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
//					gameState = GAMESTATE_MESS_FIRE;
					SetGameState( GAMESTATE_MESS_FIRE );
				}
			}
			/// 戦闘開始メッセージ表示
			else if( gameState == GAMESTATE_MESS_FIRE ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
					AudioManager.StopSound( "SE_ALERT" );
					clearMessageUI();
//					gameState = GAMESTATE_ENEMY_START;
					SetGameState( GAMESTATE_ENEMY_START );
				}
			}
			/// 敵侵入開始
			else if( gameState == GAMESTATE_ENEMY_START ){
					setEnemyTimerCurrent = 0.0f;
//					gameState = GAMESTATE_NONE;
					SetGameState( GAMESTATE_NONE );
			}
			/// ボスユニット登場メッセージ表示
			else if( gameState == GAMESTATE_MESS_LARGE ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
					clearMessageUI();
//					gameState = GAMESTATE_BOSS_START;
					SetGameState( GAMESTATE_BOSS_START );
				}
			}
			/// ボスユニット侵入開始
			else if( gameState == GAMESTATE_BOSS_START ){
//					gameState = GAMESTATE_NONE;
					SetGameState( GAMESTATE_NONE );
			}
			/// ボスユニット破壊メッセージ表示
			else if( gameState == GAMESTATE_MESS_DESTROYED ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 5000 ){
					clearMessageUI();
//					gameState = GAMESTATE_NONE;
					SetGameState( GAMESTATE_FADE_OUT );
					gameStateInitFlg = true;
				}
			}
			/// 最終防衛ライン突破メッセージ表示
			else if( gameState == GAMESTATE_MESS_BROKEN ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 5000 ){
					clearMessageUI();
//					gameState = GAMESTATE_NONE;
					SetGameState( GAMESTATE_FADE_OUT );
					gameStateInitFlg = true;
				}
			}
			/// ゲームクリアメッセージ表示
			else if( gameState == GAMESTATE_MESS_GAMECLEAR ){
//				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
//					clearMessageUI();
//					gameState = GAMESTATE_NONE;
//					SetGameState( GAMESTATE_NONE );
					SetGameState( GAMESTATE_FADE_IN );
					gameStateInitFlg = true;
//				}
			}
			/// ゲームオーバーメッセージ表示
			else if( gameState == GAMESTATE_MESS_GAMEOVER ){
//				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 2000 ){
//					clearMessageUI();
//					gameState = GAMESTATE_FADE_IN;
					SetGameState( GAMESTATE_FADE_IN );
					gameStateInitFlg = true;
//				}
			}
			else if( gameState == GAMESTATE_FADE_OUT ){
				if( fade.NowEffId == EffectFade.EffId.FadeWait ){
					if( gameStatePrev == GAMESTATE_MESS_DESTROYED ){
						SetGameState( GAMESTATE_MESS_GAMECLEAR );
						gameStateInitFlg = true;
					}
					else if( gameStatePrev == GAMESTATE_MESS_BROKEN ){
//						gameState = GAMESTATE_MESS_GAMEOVER;
						SetGameState( GAMESTATE_MESS_GAMEOVER );
						gameStateInitFlg = true;
					}
					else if( cameraMoveFadeOut ){
						SetMessageUI( "firing" );
						gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
						SetGameState( GAMESTATE_MESS_FIRE );
						SetGameState( GAMESTATE_FADE_IN );
						gameStateInitFlg = true;
								
						testCamRotX = -30.0f;
						testCamRotY = 180.0f;
						testCamRotZ = 0.0f;
						tesetDist = -204.0f;
						camTrgPos.X = 0.0f;
						camTrgPos.Y = 0.0f;
						camTrgPos.Z = -31.82887f;
						camInfo.GetPosture().SetLookAt2(
									new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
									camTrgPos,
									tesetDist );
						cameraMoveFrame = -1;
					}
				}
			}
			else if( gameState == GAMESTATE_FADE_IN ){
				if( (sceneTimer.ElapsedMilliseconds - gameStateTimeStart) > 5000 ){
					clearMessageUI();
					if( gameStatePrev == GAMESTATE_MESS_GAMECLEAR ){
						SetGameState( GAMESTATE_FADE_OUT_TO_TITLE );
						gameStateInitFlg = true;
					}
					else if( gameStatePrev == GAMESTATE_MESS_GAMEOVER ){
						SetGameState( GAMESTATE_FADE_OUT_TO_TITLE );
						gameStateInitFlg = true;
					}
					else if( gameStatePrev == GAMESTATE_MESS_FIRE ){
						gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
						SetGameState( GAMESTATE_MESS_FIRE );
					}
					else{
//						gameState = GAMESTATE_NONE;
						SetGameState( GAMESTATE_NONE );
					}
				}
			}
			else if( gameState == GAMESTATE_FADE_OUT_TO_TITLE ){
				if( fade.NowEffId == EffectFade.EffId.FadeWait ){
					useSceneMgr.Next( new SceneTitle(), false );
					SetGameState( GAMESTATE_NONE );
				}
			}
				
		}
				
		if( cameraMoveFrame >= 0 ){
			Matrix4 tmpCameraMat;
/*			
			testCamRotX = (float)(RoutDataPattern.CameraRot00[(cameraMoveFrame*3)+0] * 180.0f/Math.PI);
			testCamRotY = -(float)(RoutDataPattern.CameraRot00[(cameraMoveFrame*3)+1] * 180.0f/Math.PI);
			testCamRotZ = (float)(RoutDataPattern.CameraRot00[(cameraMoveFrame*3)+2] * 180.0f/Math.PI);
			camTrgPos.X = (float)RoutDataPattern.CameraPos00[(cameraMoveFrame*3)+0];
			camTrgPos.Y = (float)RoutDataPattern.CameraPos00[(cameraMoveFrame*3)+1];
			camTrgPos.Z = (float)RoutDataPattern.CameraPos00[(cameraMoveFrame*3)+2];
			tesetDist = -150.0f;
			camInfo.GetPosture().SetLookAt2(
						new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
						camTrgPos,
						tesetDist );
					
//			tmpCameraMat = camInfo.GetPosture().GetPosture();
//			camInfo.GetPosture().SetPosture( tmpCameraMat );
*/						
			
					
//			camInfo.GetPosture().SetPosture( rout.GetRoutPointMatrix( Rout.ROUT_CAMERA_0, cameraMoveFrame ) );
//			tmpCameraMat = camInfo.GetPosture().GetPosture();
//			camInfo.GetPosture().SetPosture( tmpCameraMat.Inverse() );
			testCamRotX = (float)(RoutDataPattern.CameraRot00[(cameraMoveFrame*3)+0]);
			testCamRotY = (float)(RoutDataPattern.CameraRot00[(cameraMoveFrame*3)+1]);
			testCamRotZ = (float)(RoutDataPattern.CameraRot00[(cameraMoveFrame*3)+2]);
			camTrgPos.X = (float)RoutDataPattern.CameraPos00[(cameraMoveFrame*3)+0];
			camTrgPos.Y = (float)RoutDataPattern.CameraPos00[(cameraMoveFrame*3)+1];
			camTrgPos.Z = (float)RoutDataPattern.CameraPos00[(cameraMoveFrame*3)+2];

//			tmpCameraMat = Matrix4.Translation( camTrgPos.X, camTrgPos.Y, camTrgPos.Z );
			tmpCameraMat = Matrix4.RotationYxz( testCamRotX, testCamRotY, testCamRotZ );
//			tmpCameraMat.Multiply( Matrix4.RotationYxz( testCamRotX, testCamRotY, testCamRotZ ) );
//			tmpCameraMat.Multiply( Matrix4.Translation( camTrgPos.X, camTrgPos.Y, camTrgPos.Z ) );
			tmpCameraMat.M41 = camTrgPos.X;
			tmpCameraMat.M42 = camTrgPos.Y;
			tmpCameraMat.M43 = camTrgPos.Z;
//			tmpCameraMat.Multiply( Matrix4.RotationYxz( 0.0f, 180.0f*(float)(Math.PI/180.0f), 0.0f ) );
//			camInfo.GetPosture().SetPosture( tmpCameraMat.Inverse() );
			camInfo.GetPosture().SetPosture( tmpCameraMat );
					
			cameraMoveFrame++;
			if( cameraMoveFrame >= rout.GetRoutPointMaxNum( Rout.ROUT_CAMERA_0 ) ){
				cameraMoveFrame = rout.GetRoutPointMaxNum( Rout.ROUT_CAMERA_0 )-1;
				if( !cameraMoveFadeOut ){
					SetGameState( GAMESTATE_FADE_OUT );
					gameStateInitFlg = true;
					cameraMoveFadeOut = true;
				}
/*						
				testCamRotX = -30.0f;
				testCamRotY = 0.0f;
				testCamRotZ = 0.0f;
				tesetDist = -200.0f;
				camTrgPos.X = camLookAtX;
				camTrgPos.Y = camLookAtY;
				camTrgPos.Z = camLookAtZ;
				camInfo.GetPosture().SetLookAt2(
							new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
							camTrgPos,
							tesetDist );
*/							
			}
		}
			
//		if( touch.GetInputNum() > 0 ){
//			comUtil.SetLog( "[log][SceneGame.cs]touch.GetInputId:"+touch.GetInputId(0) );
//		}
		
		int touchScrPosX = touch.GetScrPosX(0) * 854 / Renderer.GetGraphicsDevice().Graphics.GetFrameBuffer().Width;
		int touchScrPosY = touch.GetScrPosY(0) * 854 / Renderer.GetGraphicsDevice().Graphics.GetFrameBuffer().Width;
		// タッチ押し時
		if( touch.GetInputNum() > 0 && touch.GetInputState(0) == InputTouchState.Down ){
			touchScrOldPosX = touchScrPosX;
			touchScrOldPosY = touchScrPosY;
			touchScrOldDistance = 0;

			if( gameState == GAMESTATE_NONE ){
			// ユニット1選択
			if( touchScrPosX >= 4 && touchScrPosX < (4+162) &&
				touchScrPosY >= 357 && touchScrPosY < (357+114) ){
				if( mainPoints >= Unit.DEF_PARAM_DATA[(Unit.KIND_DEF_LASER*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_COST_POINT] ){
					unitTouchFlg = true;
					selectUnitId = 0;
					grid.gridIndex = -1;
					Graphics2D.FindSprite( "btn_LASER_on" ).Visible = true;
					AudioManager.PlaySound( "SE_SELECT" );
				}
			}
			// ユニット2選択
			if( touchScrPosX >= 170 && touchScrPosX < (170+162) &&
				touchScrPosY >= 357 && touchScrPosY < (357+114) ){
				if( mainPoints >= Unit.DEF_PARAM_DATA[(Unit.KIND_DEF_WIDE_LASER*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_COST_POINT] ){
					unitTouchFlg = true;
					selectUnitId = 1;
					grid.gridIndex = -1;
					Graphics2D.FindSprite( "btn_WIDE_on" ).Visible = true;
					AudioManager.PlaySound( "SE_SELECT" );
				}
			}
			// ユニット3選択
			if( touchScrPosX >= 522 && touchScrPosX < (522+162) &&
				touchScrPosY >= 357 && touchScrPosY < (357+114) ){
				if( mainPoints >= Unit.DEF_PARAM_DATA[(Unit.KIND_DEF_HIGH_EXPLOSIVE*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_COST_POINT] ){
					unitTouchFlg = true;
					selectUnitId = 2;
					grid.gridIndex = -1;
					Graphics2D.FindSprite( "btn_HIGH_on" ).Visible = true;
					AudioManager.PlaySound( "SE_SELECT" );
				}
			}
			// ユニット4選択
			if( touchScrPosX >= 688 && touchScrPosX < (688+162) &&
				touchScrPosY >= 357 && touchScrPosY < (357+114) ){
				if( mainPoints >= Unit.DEF_PARAM_DATA[(Unit.KIND_DEF_MISSILE*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_COST_POINT] ){
					unitTouchFlg = true;
					selectUnitId = 3;
					grid.gridIndex = -1;
					Graphics2D.FindSprite( "btn_MISSILE_on" ).Visible = true;
					AudioManager.PlaySound( "SE_SELECT" );
				}
			}
			}
		}
		// タッチ離し時
		if( touch.GetInputNum() > 0 && touch.GetInputState(0) == InputTouchState.Up ){

			if( gameState == GAMESTATE_NONE ){
			
			/// タッチフラグON時
			if( unitTouchFlg ){
				if( touchScrPosX >= 4 && touchScrPosX < (4+162) &&
					touchScrPosY >= 357 && touchScrPosY < (357+114) ){
					unitTouchFlg = false;
				}
				else if( touchScrPosX >= 170 && touchScrPosX < (170+162) &&
					touchScrPosY >= 357 && touchScrPosY < (357+114) ){
					unitTouchFlg = false;
				}
				else if( touchScrPosX >= 522 && touchScrPosX < (522+162) &&
					touchScrPosY >= 357 && touchScrPosY < (357+114) ){
					unitTouchFlg = false;
				}
				else if( touchScrPosX >= 688 && touchScrPosX < (688+162) &&
					touchScrPosY >= 357 && touchScrPosY < (357+114) ){
					unitTouchFlg = false;
				}
				else if( grid.gridIndex < 0 ){
					unitTouchFlg = false;
				}
				else if( grid.gridList[grid.gridIndex].unitType >= 0 ){
//					Console.WriteLine( "[log][SceneGame.cs]====unitType is 0 over" );
					unitTouchFlg = false;
				}
				else if( grid.gridIndex >= 0 ){
					unitTouchFlg = false;
					int getIndex = 0;
					if( selectUnitId == 0 ){
						getIndex = unitManager.AddUnit( new UnitDefense1() );
					}else if( selectUnitId == 1 ){
						getIndex = unitManager.AddUnit( new UnitDefense2() );
					}else if( selectUnitId == 2 ){
						getIndex = unitManager.AddUnit( new UnitDefense3() );
					}else if( selectUnitId == 3 ){
						getIndex = unitManager.AddUnit( new UnitDefense4() );
					}
						
					grid.gridList[grid.gridIndex].unitType = selectUnitId;
					unitManager.GetUnit( getIndex ).Init();
					unitManager.GetUnit( getIndex ).SetPosture(
								grid.gridList[grid.gridIndex].GetPosture() );
					buyUnit( selectUnitId );
						
					checkUnitImg();
							
					AudioManager.PlaySound( "SE_SETTING" );
				}
				Graphics2D.FindSprite( "btn_LASER_on" ).Visible = false;
				Graphics2D.FindSprite( "btn_WIDE_on" ).Visible = false;
				Graphics2D.FindSprite( "btn_MISSILE_on" ).Visible = false;
				Graphics2D.FindSprite( "btn_HIGH_on" ).Visible = false;
			}
						
			}
		}

		/// タッチ移動時
		if( touch.GetInputNum() > 0 && touch.GetInputState(0) == InputTouchState.Move && unitTouchFlg ){
			if( gameState == GAMESTATE_NONE ){
				grid.GetTouchGridIndex( touchScrPosX, touchScrPosY );
			}
		}
				
		if( touch.GetInputNum() > 0 && touch.GetInputState(0) == InputTouchState.Move && !unitTouchFlg ){

			if( gameState == GAMESTATE_NONE ){
			/// シングルタッチ
			if( touch.GetInputNum() == 1 ){
						
				testPosR = new Posture();
//				testPosR.SetPosture( camInfo.GetPosture().GetPosture() );
//				testPosR.AddYPR( testCamRotX*(float)(Math.PI/180.0f), 0.0f, 0.0f );
				testPosR.AddYPR( 0.0f, testCamRotY*(float)(Math.PI/180.0f), 0.0f*(float)(Math.PI/180.0f) );
				testPosR.SetPosition( camTrgPos.X, camTrgPos.Y, camTrgPos.Z );
				testPosR.AddPosition(
//						((touchScrOldPosX-touchScrPosX)*0.1f),
//						((touchScrOldPosY-touchScrPosY)*-0.1f),
//						0.0f );
						((touchScrOldPosX-touchScrPosX)*0.1f),
						0.0f,
						((touchScrOldPosY-touchScrPosY)*-0.1f) );
				
				camTrgPos.X = testPosR.GetPosition().X;
				camTrgPos.Y = testPosR.GetPosition().Y;
				camTrgPos.Z = testPosR.GetPosition().Z;
						
//				comUtil.SetLog( "[log][SceneGame.cs]camTrgPos.X:"+camTrgPos.X+"/y"+camTrgPos.Y+"/Z"+camTrgPos.Z );
						
				if( camTrgPos.X >= 100.0f ){
					camTrgPos.X = 100.0f;
				}else if( camTrgPos.X <= -100.0f ){
					camTrgPos.X = -100.0f;
				}
				if( camTrgPos.Z >= 180.0f ){
					camTrgPos.Z = 180.0f;
				}else if( camTrgPos.Z <= -110.0f ){
					camTrgPos.Z = -110.0f;
				}
/*						
				if( camTrgPos.Y >= 30.0f ){
					camTrgPos.Y = 30.0f;
				}else if( camTrgPos.Y <= 0.0f ){
					camTrgPos.Y = 0.0f;
				}
*/						
//				camInfo.GetPosture().SetLookAt(
				camInfo.GetPosture().SetLookAt2(
						new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
						camTrgPos,
						tesetDist );						
						
				touchScrOldPosX = touchScrPosX;
				touchScrOldPosY = touchScrPosY;
			}
			/// マルチタッチ
			else{
				Vector3 vec1 = new Vector3(
					(float)touchScrPosX,
					(float)touchScrPosY,
					0.0f );
				Vector3 vec2 = new Vector3(
					(float)touch.GetScrPosX(1) * 854 / Renderer.GetGraphicsDevice().Graphics.GetFrameBuffer().Width,
					(float)touch.GetScrPosY(1) * 854 / Renderer.GetGraphicsDevice().Graphics.GetFrameBuffer().Width,
					0.0f );
				float dis = comUtil.GetDistance( vec1, vec2 );

				/// 一つ前のタッチ時との距離を算出する為、一つ前のタッチ情報が無い場合は処理を行わない
				if( touchScrOldDistance != 0 ){
					/// 一つ前のタッチ情報との距離変化が少ない場合、指2本での移動とみなす
					/// ピンチアウト、ピンチイン
					if( (dis > touchScrOldDistance && (dis-touchScrOldDistance) > 1.0f) ||
						(dis < touchScrOldDistance && (touchScrOldDistance-dis) > 1.0f) ){

						tesetDist += (float)(touchScrOldDistance-dis)*0.25f*-1.0f;
//						comUtil.SetLog( "[log][SceneGame.cs]tesetDist:"+tesetDist );

						if( tesetDist >= -150.0f ){
							tesetDist = -150.0f;
						}
						else if( tesetDist <= -300.0f ){
							tesetDist = -300.0f;
						}
						camInfo.GetPosture().SetLookAt(
								new Vector3( testCamRotX, testCamRotY, 0.0f ),
								camTrgPos,
								tesetDist );

					}
					if( (dis > touchScrOldDistance && (dis-touchScrOldDistance) <= 2.0f) ||
						(dis < touchScrOldDistance && (touchScrOldDistance-dis) <= 2.0f) ){
						testCamRotY += (float)(touchScrOldPosX-touchScrPosX)*0.5f;
						testCamRotX += (float)(touchScrOldPosY-touchScrPosY)*0.5f;
								
//						comUtil.SetLog( "[log][SceneGame.cs]====testCamRotX/testCamRotY:"+testCamRotX+"/"+testCamRotY );
										
						if( testCamRotX < -89.0f ){
							testCamRotX = -89.0f;
						}
						else if( testCamRotX > -30.0f ){
							testCamRotX = -30.0f;
						}
						camInfo.GetPosture().SetLookAt(
									new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
									camTrgPos,
									tesetDist );
					}
				}
						
				touchScrOldDistance = dis;
				touchScrOldPosX = touchScrPosX;
				touchScrOldPosY = touchScrPosY;
			}
			}
		}
				
		/// 上
		if( gameState == GAMESTATE_NONE ){
			if((pad.Scan & InputGamePadState.Up) != 0){
				testCamRotX += 0.5f;

				if( testCamRotX < -89.0f ){
					testCamRotX = -89.0f;
				}
				else if( testCamRotX > -15.0f ){
					testCamRotX = -15.0f;
				}
	//			camInfo.GetPosture().SetLookAt(
				camInfo.GetPosture().SetLookAt2(
						new Vector3( testCamRotX, testCamRotY, 0.0f ),
						camTrgPos,
						tesetDist );
			}
			/// 下
			if((pad.Scan & InputGamePadState.Down) != 0){
				testCamRotX -= 0.5f;

				if( testCamRotX < -89.0f ){
					testCamRotX = -89.0f;
				}
				else if( testCamRotX > -15.0f ){
					testCamRotX = -15.0f;
				}
	//			camInfo.GetPosture().SetLookAt(
				camInfo.GetPosture().SetLookAt2(
						new Vector3( testCamRotX, testCamRotY, 0.0f ),
						camTrgPos,
						tesetDist );
			}
			/// 右
			if((pad.Scan & InputGamePadState.Right) != 0){
					
				testCamRotY += 0.5f;

				if( testCamRotX < -89.0f ){
					testCamRotX = -89.0f;
				}
				else if( testCamRotX > -15.0f ){
					testCamRotX = -15.0f;
				}
	//			camInfo.GetPosture().SetLookAt(
				camInfo.GetPosture().SetLookAt2(
						new Vector3( testCamRotX, testCamRotY, 0.0f ),
						camTrgPos,
						tesetDist );
			}
			/// 左
			if((pad.Scan & InputGamePadState.Left) != 0){
					
				testCamRotY -= 0.5f;

				if( testCamRotX < -89.0f ){
					testCamRotX = -89.0f;
				}
				else if( testCamRotX > -15.0f ){
					testCamRotX = -15.0f;
				}
	//			camInfo.GetPosture().SetLookAt(
				camInfo.GetPosture().SetLookAt2(
						new Vector3( testCamRotX, testCamRotY, 0.0f ),
						camTrgPos,
						tesetDist );
			}
			if((pad.Scan & InputGamePadState.Triangle) != 0 ){
	//			testCamZ += 1.0f;
	//			camInfo.GetPosture().AddPosition( 0.0f, 0.0f, 1.0f );
	/*					
				tesetDist += 5.0f;
				camInfo.GetPosture().SetLookAt(
							new Vector3( testCamRotX, testCamRotY, 0.0f ),
							new Vector3( 0.0f, 0.0f, 0.0f ),
							tesetDist );
	*/						
	//			camInfo.GetPosture().AddPosition( 0.0f, 0.0f, -1.0f );
				tesetDist += (float)(1.0f);
				comUtil.SetLog( "[log][SceneGame.cs]tesetDist:"+tesetDist );
					
				if( tesetDist >= -150.0f ){
					tesetDist = -150.0f;
				}
				else if( tesetDist <= -400.0f ){
					tesetDist = -400.0f;
				}
				camInfo.GetPosture().SetLookAt(
						new Vector3( testCamRotX, testCamRotY, 0.0f ),
						camTrgPos,
						tesetDist );
					
			}
			if((pad.Scan & InputGamePadState.Cross) != 0 ){
	//			testCamZ -= 1.0f;
	//			camInfo.GetPosture().AddPosition( 0.0f, 0.0f, -1.0f );
	/*					
				tesetDist -= 5.0f;
				camInfo.GetPosture().SetLookAt(
							new Vector3( testCamRotX, testCamRotY, 0.0f ),
							new Vector3( 0.0f, 0.0f, 0.0f ),
							tesetDist );
	*/						
	//			camInfo.GetPosture().AddPosition( 0.0f, 0.0f, 1.0f );
				tesetDist += (float)(-1.0f);
				comUtil.SetLog( "[log][SceneGame.cs]tesetDist:"+tesetDist );
					
				if( tesetDist >= -150.0f ){
					tesetDist = -150.0f;
				}
				else if( tesetDist <= -300.0f ){
					tesetDist = -300.0f;
				}
				camInfo.GetPosture().SetLookAt(
						new Vector3( testCamRotX, testCamRotY, 0.0f ),
						camTrgPos,
						tesetDist );
					
			}
			if((pad.Scan & InputGamePadState.Square) != 0  && keyWait == 0 ){
				unitManager.changeUnitModelHigh = true;
					
				keyWait = 10;
			}
			if((pad.Scan & InputGamePadState.Circle ) != 0 && keyWait == 0 ){
				unitManager.changeUnitModelLow = true;
					
				keyWait = 10;			
			}
		}
/*				
		if((pad.Scan & InputGamePadState.L ) != 0 && keyWait == 0 ){
			comUtil.SetLog( "[log][SceneGame.cs]push L key" );
			AudioManager.StopSound( "SE_ALERT" );
			clearMessageUI();
			SetGameState( GAMESTATE_NONE );
			setEnemyTimerCurrent = 0.0f;
			testCamRotX = -30.0f;
			testCamRotY = 180.0f;
			testCamRotZ = 0.0f;
			tesetDist = -204.0f;
			camTrgPos.X = 0.0f;
			camTrgPos.Y = 0.0f;
			camTrgPos.Z = -31.82887f;
			camInfo.GetPosture().SetLookAt2(
					new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
					camTrgPos,
					tesetDist );
			cameraMoveFrame = -1;					
		}
*/				
/*				
		if((pad.Scan & InputGamePadState.R ) != 0 && keyWait == 0 ){
			comUtil.SetLog( "[log][SceneGame.cs]push R key" );
			if( mainPoints <= 9999 ){
				mainPoints += 10;
				if( mainPoints >= 9999 ){
					mainPoints = 9999;
				}
				setImageNumber( "num_main", mainPoints );
				checkUnitImg();
			}
		}
*/
		ground.Update();
		grid.Update( unitTouchFlg );
				
		unitManager.UnitUpdate();
				
		// enemy goul check				
		if( !gameResult ){
			if( unitManager.enemyArrive ){
				SetGameState( GAMESTATE_MESS_BROKEN );
				gameResult = true;
			}
		}
				
		if( unitManager.GetPoints() > 0 ){
			mainPoints += unitManager.GetPoints();
			unitManager.ClearPoints();
			if( load2dFlg ){
				setImageNumber( "num_main", mainPoints );
				checkUnitImg();
			}
		}
				
		fade.Frame();
								
		keyWait --;
		if( keyWait < 0 ){
			keyWait = 0;
		}
				
		/// 時間取得
		if( setEnemyTimerCurrent >= 0.0f ){
			setEnemyTimerCurrent += 0.033f;
		}
		int i = 0;
		Vector3 tmp;
				
		if( waveCnt < Unit.ENEMY_START_DATA.Length &&
			(waveEnemyCnt*3) < Unit.ENEMY_START_DATA[waveCnt].Length ){
			/// 経過時間から敵ユニットの出現を判定
			if( setEnemyTimerCurrent >= (Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+0]) ){
						
				int pattern = rand.Next( 5 );
						
				if( waveCnt == Unit.ENEMY_START_DATA.Length -1 &&
					waveEnemyCnt == (Unit.ENEMY_START_DATA[waveCnt].Length/3)-1){
					gameState = GAMESTATE_MESS_LARGE;
					gameStateInitFlg = true;
				}
				while( i < Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+2] ){
					/// ユニット管理クラスにユニット追加
					int setEnemyIndex = 0;
					if( Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+1] == 0 ){
						setEnemyIndex = unitManager.AddUnit( new UnitOffense1() );
					}else if( Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+1] == 1 ){
						setEnemyIndex = unitManager.AddUnit( new UnitOffense2() );
					}else if( Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+1] == 2 ){
						setEnemyIndex = unitManager.AddUnit( new UnitOffense3() );
					}else if( Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+1] == 3 ){
						setEnemyIndex = unitManager.AddUnit( new UnitOffense4() );
					}else{
						setEnemyIndex = unitManager.AddUnit( new UnitOffenseBoss() );
					}
					/// ユニットの初期化
					unitManager.GetUnit(setEnemyIndex).Init();
					/// ユニットの初期位置設定
					tmp = GetUnitSettingPos( Unit.ENEMY_START_DATA[waveCnt][(waveEnemyCnt*3)+2], i );
								
					unitManager.GetUnit(setEnemyIndex).SetPosition( tmp.X, tmp.Y, tmp.Z );
							
					if( tmp.X == 0.0f ){
						unitManager.GetUnit(setEnemyIndex).nowRouteLine = Unit.ROUTE_LINE_CENTER;
					}else if( tmp.X == -3.0f ){
						unitManager.GetUnit(setEnemyIndex).nowRouteLine = Unit.ROUTE_LINE_LEFT;
					}else{
						unitManager.GetUnit(setEnemyIndex).nowRouteLine = Unit.ROUTE_LINE_RIGHT;
					}
								
					unitManager.GetUnit(setEnemyIndex).wave = waveCnt;
					if( waveEnemyCnt >= (Unit.ENEMY_START_DATA[waveCnt].Length/3) -1 ){
						unitManager.GetUnit(setEnemyIndex).waveLast = true;
					}
								
					if( waveCnt == Unit.ENEMY_START_DATA.Length-1 ){
						unitManager.GetUnit(setEnemyIndex).nowRouteLine = Unit.ROUTE_LINE_CENTER;
						unitManager.GetUnit(setEnemyIndex).routePattern = Unit.ROUTE_PATTERN_C;
					}
					else {
						unitManager.GetUnit(setEnemyIndex).routePattern = pattern;
					}
								
					unitManager.GetUnit(setEnemyIndex).routNum = 0;
					unitManager.GetUnit(setEnemyIndex).SetPosture(
									rout.GetRoutPointMatrix(
										unitManager.GetUnit(setEnemyIndex).routePattern,
										unitManager.GetUnit(setEnemyIndex).routNum )
									);
								
					unitManager.nowAllEnemyDead = false;
						
					i++;
				}
				/// 出現するユニット番号を進める
				waveEnemyCnt++;
			}
		}
					
//		}
				
		if( (waveCnt < Unit.ENEMY_START_DATA.Length && (waveEnemyCnt*3) >= Unit.ENEMY_START_DATA[waveCnt].Length) &&
			unitManager.nowAllEnemyDead ){
//		if( unitManager.nowAllEnemyDead ){
			if( waveCnt < Unit.ENEMY_START_DATA.Length -1){
				waveCnt++;
				waveEnemyCnt = 0;
				setEnemyTimerCurrent = 0.0f;
				unitManager.nowAllEnemyDead = false;
			}
					
			else if( waveCnt == Unit.ENEMY_START_DATA.Length -1 ){
				waveCnt++;
				gameState = GAMESTATE_MESS_DESTROYED;
				gameStateInitFlg = true;
			}		
		}

		return true;
    }

    /// 描画処理
    public bool Render()
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		EffectFade fade = EffectFade.GetInstance();

        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		
        graphics.Clear();
				
#if DEBUG				
		Graphics2D.AddSprite( "Ms", Fps.GetFpsTime()+"ms", 0xffffffff, 0, 0 );
#endif				
		grid.Render();
				
		unitManager.UnitRender();
				
		ground.Render();

		unitManager.UnitRenderAlpha();
				
		grid.RenderAlpha();
				
		Graphics2D.DrawSprites();
				
		fade.Draw( Renderer.GetGraphicsDevice() );
				
        graphics.SwapBuffers();

#if DEBUG
		Graphics2D.RemoveSprite( "Ms" );
#endif

		return true;
    }

	/// 隊列内のキャラ位置取得
	/**
	 * @param [in] all ; 隊列ユニット数
	 * @param [in] num : 何番目のユニットか
	 * @warning 隊列ユニット数が1～6までを対応、7以上はコードの追記が必要
	 */
	public Vector3 GetUnitSettingPos( int all, int num )
	{
		Vector3 result = new Vector3( 0.0f, 0.0f, 0.0f );
				
		int lineValue = rand.Next( 3 );
				
		if( lineValue == 0 ){
			result.X = 0.0f;
		}
		else if( lineValue == 1 ){
			result.X = -3.0f;
		}
		else if( lineValue == 2 ){
			result.X = 3.0f;
		}
				
		switch( all ){
		case 1:
			result.Y = -30.0f;
			result.Z = -110.0f;
			break;
		case 2:
			if( num == 0 ){
				result.X = 3.0f;
				result.Y = -30.0f;
				result.Z = -110.0f;
			}else{
				result.X = -3.0f;
				result.Y = -30.0f;
				result.Z = -110.0f;
			}
			break;
		case 3:
			if( num == 0 ){
				result.X = 0.0f;
				result.Y = -30.0f;
				result.Z = -110.0f;
			}else if( num == 1 ){
				result.X = -3.0f;
				result.Y = -30.0f;
				result.Z = -110.0f;
			}else{
				result.X = 3.0f;
				result.Y = -30.0f;
				result.Z = -110.0f;
			}
			break;
		default:
			result.X = 0.0f;
			result.Y = -30.0f;
			result.Z = -110.0f;
			break;
		}
				
		return result;
	}

	/// 2Dリソースの読込み
	/**
	 */
	public void load2dData()
	{
        image = new Texture2D("/Application/res/2d/2d_03.png", false);

		/// UI素材読込み
		Graphics2D.AddSprite( "base", new Sprite( image, 0, 0, 854, 53, 0, 427 ) );

		/// 「レーザー」アイコン
		Graphics2D.AddSprite("btn_LASER_off",new Sprite(image,700,537,162,115,4,356));
		Graphics2D.AddSprite("btn_LASER_on",new Sprite(image,700,421,162,115,4,356));
		Graphics2D.AddSprite("btn_LASER_no",new Sprite(image,538,909,162,115,4,356));
		Graphics2D.FindSprite( "btn_LASER_no" ).Visible = false;
		Graphics2D.FindSprite( "btn_LASER_on" ).Visible = false;

		/// 「ワイドレーザー」アイコン
		Graphics2D.AddSprite("btn_WIDE_off",new Sprite(image,862,531,161.5f,124,170,347));
		Graphics2D.AddSprite("btn_WIDE_on",new Sprite(image,862,407,161.5f,124,170,347));
		Graphics2D.AddSprite("btn_WIDE_no",new Sprite(image,862,283,161.5f,124,170,347));
		Graphics2D.FindSprite( "btn_WIDE_no" ).Visible = false;
		Graphics2D.FindSprite( "btn_WIDE_on" ).Visible = false;

		/// 「榴弾砲」アイコン
		Graphics2D.AddSprite("btn_HIGH_off",new Sprite(image,862,902,162,122,522,349));
		Graphics2D.AddSprite("btn_HIGH_on",new Sprite(image,862,779,162,122,522,349));
		Graphics2D.AddSprite("btn_HIGH_no",new Sprite(image,862,656,162,122,522,349));
		Graphics2D.FindSprite( "btn_HIGH_no" ).Visible = false;
		Graphics2D.FindSprite( "btn_HIGH_on" ).Visible = false;

		/// 「ミサイル」アイコン
		Graphics2D.AddSprite("btn_MISSILE_off",new Sprite(image,700,900,162,124,688,347));
		Graphics2D.AddSprite("btn_MISSILE_on",new Sprite(image,700,776,162,124,688,347));
		Graphics2D.AddSprite("btn_MISSILE_no",new Sprite(image,700,652,162,124,688,347));
		Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible = false;
		Graphics2D.FindSprite( "btn_MISSILE_on" ).Visible = false;

		/// 「レーザー」コスト用数字
		Graphics2D.AddSprite("pt_sy",new Sprite(image,886,101,23,21,99,405));
		Graphics2D.AddSprite("num_y_100",new Sprite(image,0,944,21,23,47,402));
		Graphics2D.AddSprite("num_y_10",new Sprite(image,0,944,21,23,63,402));
		Graphics2D.AddSprite("num_y_1",new Sprite(image,0,944,21,23,79,402));
		Graphics2D.AddSprite("pt_syw",new Sprite(image,963,99,23,21,99,405));
		Graphics2D.AddSprite("num_yw_100",new Sprite(image,0,921,21,23,47,402));
		Graphics2D.AddSprite("num_yw_10",new Sprite(image,0,921,21,23,63,402));
		Graphics2D.AddSprite("num_yw_1",new Sprite(image,0,921,21,23,79,402));
		Graphics2D.FindSprite( "pt_syw" ).Visible = false;
		Graphics2D.FindSprite( "num_yw_100" ).Visible = false;
		Graphics2D.FindSprite( "num_yw_10" ).Visible = false;
		Graphics2D.FindSprite( "num_yw_1" ).Visible = false;			

		/// 「ワイドレーザー」コスト用数字
		Graphics2D.AddSprite("pt_sg",new Sprite(image,886,101,23,21,265,405));
		Graphics2D.AddSprite("num_g_100",new Sprite(image,0,967,21,23,213,402));
		Graphics2D.AddSprite("num_g_10",new Sprite(image,0,967,21,23,229,402));
		Graphics2D.AddSprite("num_g_1",new Sprite(image,0,967,21,23,245,402));
		Graphics2D.AddSprite("pt_sgw",new Sprite(image,963,99,23,21,265,405));
		Graphics2D.AddSprite("num_gw_100",new Sprite(image,0,921,21,23,213,402));
		Graphics2D.AddSprite("num_gw_10",new Sprite(image,0,921,21,23,229,402));
		Graphics2D.AddSprite("num_gw_1",new Sprite(image,0,921,21,23,245,402));
		Graphics2D.FindSprite( "pt_sgw" ).Visible = false;
		Graphics2D.FindSprite( "num_gw_100" ).Visible = false;
		Graphics2D.FindSprite( "num_gw_10" ).Visible = false;
		Graphics2D.FindSprite( "num_gw_1" ).Visible = false;			

		/// 「榴弾砲」コスト用数字
		Graphics2D.AddSprite("pt_sr",new Sprite(image,886,101,23,21,783,405));
		Graphics2D.AddSprite("num_r_100",new Sprite(image,0,990,21,23,731,402));
		Graphics2D.AddSprite("num_r_10",new Sprite(image,0,990,21,23,747,402));
		Graphics2D.AddSprite("num_r_1",new Sprite(image,0,990,21,23,763,402));
		Graphics2D.AddSprite("pt_srw",new Sprite(image,963,99,23,21,783,405));
		Graphics2D.AddSprite("num_rw_100",new Sprite(image,0,921,21,23,731,402));
		Graphics2D.AddSprite("num_rw_10",new Sprite(image,0,921,21,23,747,402));
		Graphics2D.AddSprite("num_rw_1",new Sprite(image,0,921,21,23,763,402));
		Graphics2D.FindSprite( "pt_srw" ).Visible = false;
		Graphics2D.FindSprite( "num_rw_100" ).Visible = false;
		Graphics2D.FindSprite( "num_rw_10" ).Visible = false;
		Graphics2D.FindSprite( "num_rw_1" ).Visible = false;			

		/// 「ミサイル」コスト用数字
		Graphics2D.AddSprite("pt_sb",new Sprite(image,886,101,23,21,617,405));
		Graphics2D.AddSprite("num_b_100",new Sprite(image,210,921,21,23,565,402));
		Graphics2D.AddSprite("num_b_10",new Sprite(image,210,921,21,23,581,402));
		Graphics2D.AddSprite("num_b_1",new Sprite(image,210,921,21,23,597,402));
		Graphics2D.AddSprite("pt_sbw",new Sprite(image,963,99,23,21,617,405));
		Graphics2D.AddSprite("num_bw_100",new Sprite(image,0,921,21,23,565,402));
		Graphics2D.AddSprite("num_bw_10",new Sprite(image,0,921,21,23,581,402));
		Graphics2D.AddSprite("num_bw_1",new Sprite(image,0,921,21,23,597,402));
		Graphics2D.FindSprite( "pt_sbw" ).Visible = false;
		Graphics2D.FindSprite( "num_bw_100" ).Visible = false;
		Graphics2D.FindSprite( "num_bw_10" ).Visible = false;
		Graphics2D.FindSprite( "num_bw_1" ).Visible = false;			

		/// 所持ポイント用数字
		Graphics2D.AddSprite("pt_B",new Sprite(image,924,98,32,26,463,448));
		Graphics2D.AddSprite("num_main_1000",new Sprite(image,210,944,27,29,361,443));
		Graphics2D.AddSprite("num_main_100",new Sprite(image,210,944,27,29,386,443));
		Graphics2D.AddSprite("num_main_10",new Sprite(image,210,944,27,29,412,443));
		Graphics2D.AddSprite("num_main_1",new Sprite(image,210,944,27,29,437,443));

		/// メッセージ
		Graphics2D.AddSprite("base",new Sprite(image,0,0,854,53,0,427));
		Graphics2D.AddSprite("gameclear",new Sprite(image,0,630,569,94,142,157));
		Graphics2D.AddSprite("gameover",new Sprite(image,0,724,549,94,152,157));
		Graphics2D.AddSprite("alert",new Sprite(image,0,233,829,60,12,173));
		Graphics2D.AddSprite("destroy",new Sprite(image,0,53,854,60,0,173));
		Graphics2D.AddSprite("firing",new Sprite(image,0,818,409,60,222,173));
		Graphics2D.AddSprite("large",new Sprite(image,0,293,729,60,62,173));
		Graphics2D.AddSprite("destroyed",new Sprite(image,0,173,849,60,2,173));
		Graphics2D.AddSprite("broken",new Sprite(image,0,113,854,60,0,173));
		Graphics2D.FindSprite( "firing" ).Visible = false;
		Graphics2D.FindSprite( "alert" ).Visible = false;
		Graphics2D.FindSprite( "destroy" ).Visible = false;
		Graphics2D.FindSprite( "large" ).Visible = false;
		Graphics2D.FindSprite( "destroyed" ).Visible = false;
		Graphics2D.FindSprite( "broken" ).Visible = false;
		Graphics2D.FindSprite( "gameclear" ).Visible = false;
		Graphics2D.FindSprite( "gameover" ).Visible = false;
				
		load2dFlg = true;
				
		setImageNumber( "num_main", mainPoints );
		setImageNumber( "num_y", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_yw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_g", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_WIDE_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_gw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_WIDE_LASER*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_r", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_MISSILE*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_rw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_MISSILE*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_b", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_HIGH_EXPLOSIVE*Unit.DEF_PARAM_NUM)] );
		setImageNumber( "num_bw", Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(Unit.KIND_DEF_HIGH_EXPLOSIVE*Unit.DEF_PARAM_NUM)] );

	}

	/// 2Dリソースの解放
	/**
	 */
	void release2dData()
	{
		Graphics2D.ClearSprite();
				
		load2dFlg = false;
	}

	/// 3Dリソースの読込み
	/**
	 */
	void load3dData()
	{
		ResourceDataContainer useResCont = ResourceDataContainer.Inst();
				
		/// 味方ユニット
		/// モデル
		/// レーザー砲台
		useResCont.Load3D( "A01L0", "/Application/res/3d/weapon/a01/A01_l0.mdx" );
		useResCont.Load3D( "A01L2", "/Application/res/3d/weapon/a01/A01_l2.mdx" );
		useResCont.Load2D( "A01.png", "weapon/a01/A01.png" );
		useResCont.Load2D( "A01_AO.png", "weapon/a01/A01_AO.png" );
		/// ワイドレーザー砲台
		useResCont.Load3D( "A02L0", "/Application/res/3d/weapon/a02/A02_l0.mdx" );
		useResCont.Load3D( "A02L2", "/Application/res/3d/weapon/a02/A02_l2.mdx" );
		useResCont.Load2D( "A02.png", "weapon/a02/A02.png" );
		useResCont.Load2D( "A02_AO.png", "weapon/a02/A02_AO.png" );
		/// 榴弾砲
		useResCont.Load3D( "A03L0", "/Application/res/3d/weapon/a03/A03_l0.mdx" );
		useResCont.Load3D( "A03L2", "/Application/res/3d/weapon/a03/A03_l2.mdx" );
		useResCont.Load2D( "A03.png", "weapon/a03/A03.png" );
		useResCont.Load2D( "A03_AO.png", "weapon/a03/A03_AO.png" );
		/// ミサイル砲台
		useResCont.Load3D( "A04L0", "/Application/res/3d/weapon/a04/A04_l0.mdx" );
		useResCont.Load3D( "A04L2", "/Application/res/3d/weapon/a04/A04_l2.mdx" );
		useResCont.Load2D( "A04.png", "weapon/a04/A04.png" );
		useResCont.Load2D( "A04_AO.png", "weapon/a04/A04_AO.png" );
				
		/// 味方ユニットエフェクト
		/// レーザー砲台
		/// 弾
		useResCont.Load3D( "E00", "/Application/res/3d/effect/e00/e00.mdx" );
		useResCont.Load2D( "e00.png", "effect/e00/e00.png" );
		/// 弾
		useResCont.Load3D( "E01", "/Application/res/3d/effect/e01/E01.mdx" );
		useResCont.Load2D( "E01.png", "effect/e01/E01.png" );
		/// 発射
		useResCont.Load3D( "E02", "/Application/res/3d/effect/e02/E02.mdx" );
		useResCont.Load2D( "E02.png", "effect/e02/E02.png" );
		/// 着弾(敵)
		useResCont.Load3D( "E03", "/Application/res/3d/effect/e03/E03.mdx" );
		useResCont.Load2D( "E03.png", "effect/e03/E03.png" );
		/// 着弾(地面)
		useResCont.Load3D( "E04", "/Application/res/3d/effect/e04/E04.mdx" );
		useResCont.Load2D( "E04.png", "effect/e04/E04.png" );
		/// ワイドレーザー砲台
		useResCont.Load3D( "E11", "/Application/res/3d/effect/e11/E11.mdx" );
		useResCont.Load2D( "E11.png", "effect/e11/E11.png" );
				
		useResCont.Load3D( "E12", "/Application/res/3d/effect/e12/E12.mdx" );
		useResCont.Load2D( "E12.png", "effect/e12/E12.png" );
				
		useResCont.Load3D( "E13", "/Application/res/3d/effect/e13/E13.mdx" );
		useResCont.Load2D( "E13.png", "effect/e13/E13.png" );
				
		useResCont.Load3D( "E14", "/Application/res/3d/effect/e14/E14.mdx" );
		useResCont.Load2D( "E14.png", "effect/e14/E14.png" );
		/// 榴弾砲
		useResCont.Load3D( "E21", "/Application/res/3d/effect/e21/E21.mdx" );
		useResCont.Load2D( "E21.png", "effect/e21/E21.png" );
				
		useResCont.Load3D( "E22", "/Application/res/3d/effect/e22/E22.mdx" );
		useResCont.Load2D( "E22.png", "effect/e22/E22.png" );
				
		useResCont.Load3D( "E23", "/Application/res/3d/effect/e23/E23.mdx" );
		useResCont.Load2D( "E23.png", "effect/e23/E23.png" );
				
		useResCont.Load3D( "E24", "/Application/res/3d/effect/e24/E24.mdx" );
		useResCont.Load2D( "E24.png", "effect/e24/E24.png" );
				
		useResCont.Load3D( "E25", "/Application/res/3d/effect/e25/E25.mdx" );
		useResCont.Load2D( "E25.png", "effect/e25/E25.png" );
				
		/// ミサイル砲台
		useResCont.Load3D( "E31", "/Application/res/3d/effect/e31/E31.mdx" );
		useResCont.Load2D( "E31.png", "effect/e31/E31.png" );
				
		useResCont.Load3D( "E32", "/Application/res/3d/effect/e32/E32.mdx" );
		useResCont.Load2D( "E32.png", "effect/e32/E32.png" );
				
		useResCont.Load3D( "E33", "/Application/res/3d/effect/e33/E33.mdx" );
		useResCont.Load2D( "E33.png", "effect/e33/E33.png" );
				
		useResCont.Load3D( "E34", "/Application/res/3d/effect/e34/E34.mdx" );
		useResCont.Load2D( "E34.png", "effect/e34/E34.png" );
				
		useResCont.Load3D( "E35", "/Application/res/3d/effect/e35/E35.mdx" );
		useResCont.Load2D( "E35.png", "effect/e35/E35.png" );
				
		/// 攻撃により敵爆発
		useResCont.Load3D( "E41", "/Application/res/3d/effect/e41/E41.mdx" );
		useResCont.Load2D( "E41.png", "effect/e41/E41.png" );
				
		useResCont.Load3D( "E42", "/Application/res/3d/effect/e42/E42.mdx" );
		useResCont.Load2D( "E42.png", "effect/e42/E42.png" );

		/// 敵ユニット
		/// トラック
		useResCont.Load3D( "ENE01L0", "/Application/res/3d/enemy/ene01/ene01_l0.mdx" );
		useResCont.Load2D( "ene01_l0.png", "enemy/ene01/ene01_l0.png" );
		useResCont.Load3D( "ENE01L2", "/Application/res/3d/enemy/ene01/ene01_l2.mdx" );
		useResCont.Load2D( "ene01_l2.png", "enemy/ene01/ene01_l2.png" );
		/// 装甲車
		useResCont.Load3D( "ENE02L0", "/Application/res/3d/enemy/ene02/ene02_l0.mdx" );
		useResCont.Load2D( "ene02_l0.png", "enemy/ene02/ene02_l0.png" );
		useResCont.Load3D( "ENE02L2", "/Application/res/3d/enemy/ene02/ene02_l2.mdx" );
		useResCont.Load2D( "ene02_l2.png", "enemy/ene02/ene02_l2.png" );
		/// 戦車
		useResCont.Load3D( "ENE03L0", "/Application/res/3d/enemy/ene03/ene03_l0.mdx" );
		useResCont.Load2D( "ene03_l0.png", "enemy/ene03/ene03_l0.png" );
		useResCont.Load3D( "ENE03L2", "/Application/res/3d/enemy/ene03/ene03_l2.mdx" );
		useResCont.Load2D( "ene03_l2.png", "enemy/ene03/ene03_l2.png" );
		/// 浮遊ポット
		useResCont.Load3D( "ENE04L0", "/Application/res/3d/enemy/ene04/ene04_l0.mdx" );
		useResCont.Load2D( "ene04_l0.png", "enemy/ene04/ene04_l0.png" );
		useResCont.Load3D( "ENE04L2", "/Application/res/3d/enemy/ene04/ene04_l2.mdx" );
		useResCont.Load2D( "ene04_l2.png", "enemy/ene04/ene04_l2.png" );
		/// ボス
		useResCont.Load3D( "ENE05L0", "/Application/res/3d/enemy/ene05/ene05_l0.mdx" );
		useResCont.Load2D( "ene05_l0.png", "enemy/ene05/ene05_l0.png" );
		useResCont.Load3D( "ENE05L2", "/Application/res/3d/enemy/ene05/ene05_l2.mdx" );
		useResCont.Load2D( "ene05_l2.png", "enemy/ene05/ene05_l2.png" );

		/// ルートデータ
		useResCont.Load3D( "ROUT", "/Application/res/3d/ground/rout_attr.mdx" );
		
		/// ユニット配置データ
		useResCont.Load3D( "UNITPOS", "/Application/res/3d/ground/unit_pos.mdx" );

		/// 背景
		useResCont.Load3D( "GROUND", "/Application/res/3d/ground/ground00.mdx" );
		useResCont.Load2D( "base_oc.png", "ground/base_oc.png" );
		useResCont.Load2D( "ground00_01.png", "ground/ground00_01.png" );
		useResCont.Load2D( "ground00_02.png", "ground/ground00_02.png" );
		useResCont.Load2D( "suna.png", "ground/suna.png" );
		/// ユニット配置箇所の蓋(アクション無し)
		useResCont.Load3D( "FUTA", "/Application/res/3d/ground/ground01.mdx" );
		/// ユニット配置箇所の蓋(アクション有り)
		useResCont.Load3D( "FUTA_E", "/Application/res/3d/ground/ground02.mdx" );

		useResCont.Load2D( "ground00_21.png", "ground/ground00_21.png" );
		useResCont.Load2D( "ground00_22.png", "ground/ground00_22.png" );
		useResCont.Load2D( "ground00_23.png", "ground/ground00_23.png" );
				
		useResCont.Load3D( "U00", "/Application/res/3d/ui/u00/u00.mdx" );
		useResCont.Load2D( "U00.png", "ui/u00/u00.png" );
				
		useResCont.Load3D( "U01", "/Application/res/3d/ui/u01/u01.mdx" );
		useResCont.Load2D( "U01.png", "ui/u01/u01.png" );
		

		useResCont.modelContainer.BindTextures( useResCont.texContainer );

		useResCont.shaderContainer.LoadBasicProgram();
	}
			
	public void loadSoundData()
	{
		AudioManager.AddBgm( "BGM_GAME", "/Application/res/sound/S92.mp3" );
		AudioManager.AddBgm( "BGM_BOSS", "/Application/res/sound/S93.mp3" );
				
		AudioManager.AddBgm( "BGM_GAMECLEAR", "/Application/res/sound/S61.mp3" );
		AudioManager.AddBgm( "BGM_GAMEOVER", "/Application/res/sound/S62.mp3" );
				
		AudioManager.AddSound( "SE_SELECT", "/Application/res/sound/S01.wav" );
		AudioManager.AddSound( "SE_SETTING", "/Application/res/sound/S11.wav" );
		AudioManager.AddSound( "SE_ALERT", "/Application/res/sound/S21.wav" );
				
		AudioManager.AddSound( "SE_LASER", "/Application/res/sound/S31.wav" );
		AudioManager.AddSound( "SE_WIDE", "/Application/res/sound/S32.wav" );
		AudioManager.AddSound( "SE_EXPLOSIVE", "/Application/res/sound/S33.wav" );
		AudioManager.AddSound( "SE_MISSILE", "/Application/res/sound/S34.wav" );

		AudioManager.AddSound( "SE_TRACK", "/Application/res/sound/S41.wav" );
		AudioManager.AddSound( "SE_CAR", "/Application/res/sound/S42.wav" );
		AudioManager.AddSound( "SE_TANK", "/Application/res/sound/S43.wav" );
		AudioManager.AddSound( "SE_FLOATING", "/Application/res/sound/S44.wav" );
		AudioManager.AddSound( "SE_BOSS", "/Application/res/sound/S45.wav" );
				
		AudioManager.AddSound( "SE_DEAD", "/Application/res/sound/S51.wav" );
		AudioManager.AddSound( "SE_DEAD_BOSS", "/Application/res/sound/S52.wav" );
	}

	/// 数字画像で、表示する画像を変更する
	/**
	 * @param tag : 変更する数字画像スクリプトのタグ
	 * @param num : 画像で表示する数値
	 */
	public void setImageNumber( String tag, int num )
	{
		int numBuf;
		int useNumber;
		int rectId = 0;
		int i = 0;
		int[][] drawRectXYWH = {
				new[]{ 210, 944, 27, 29 },
				new[]{   0, 944, 21, 23 },
				new[]{   0, 921, 21, 23 },
				new[]{   0, 967, 21, 23 },
				new[]{   0, 921, 21, 23 },
				new[]{   0, 990, 21, 23 },
				new[]{   0, 921, 21, 23 },
				new[]{ 210, 921, 21, 23 },
				new[]{   0, 921, 21, 23 }
		};
		String[] tagRectId= {
			"num_main",
			"num_y",
			"num_yw",
			"num_g",
			"num_gw",
			"num_r",
			"num_rw",
			"num_b",
			"num_bw"
		};
				
		if( !load2dFlg ){
			return;
		}
			
		i = 0;
		while( i < tagRectId.Length ){
			if( tag == tagRectId[i] ){
				break;
			}
			i++;
		}
		if( i >= tagRectId.Length ){
			rectId = 0;
		}else{
			rectId = i;
		}
				
		useNumber = num/1000;
		numBuf = num%1000;
		if( rectId == 0 ){
		if( useNumber >= 0 ){
//			Graphics2D.FindSprite( tag+"_1000" ).SetDrawRect( 210+(27*useNumber), 944, 27, 29 );
			Graphics2D.FindSprite( tag+"_1000" ).SetDrawRect(
						drawRectXYWH[rectId][0]+(drawRectXYWH[rectId][2]*useNumber),
						drawRectXYWH[rectId][1],
						drawRectXYWH[rectId][2],
						drawRectXYWH[rectId][3] );
						
		}else{
		}
		}
				
		useNumber = numBuf/100;
		numBuf = numBuf%100;
		if( useNumber >= 0 ){
//			Graphics2D.FindSprite( tag+"_100" ).SetDrawRect( 210+(27*useNumber), 944, 27, 29 );
			Graphics2D.FindSprite( tag+"_100" ).SetDrawRect(
						drawRectXYWH[rectId][0]+(drawRectXYWH[rectId][2]*useNumber),
						drawRectXYWH[rectId][1],
						drawRectXYWH[rectId][2],
						drawRectXYWH[rectId][3] );
		}else{
		}
		
		useNumber = numBuf/10;
		numBuf = numBuf%10;
		if( useNumber >= 0 ){
//			Graphics2D.FindSprite( tag+"_10" ).SetDrawRect( 210+(27*useNumber), 944, 27, 29 );
			Graphics2D.FindSprite( tag+"_10" ).SetDrawRect(
						drawRectXYWH[rectId][0]+(drawRectXYWH[rectId][2]*useNumber),
						drawRectXYWH[rectId][1],
						drawRectXYWH[rectId][2],
						drawRectXYWH[rectId][3] );
		}
				
		useNumber = numBuf;
		if( useNumber >= 0 ){
//			Graphics2D.FindSprite( tag+"_1" ).SetDrawRect( 210+(27*useNumber), 944, 27, 29 );
			Graphics2D.FindSprite( tag+"_1" ).SetDrawRect(
						drawRectXYWH[rectId][0]+(drawRectXYWH[rectId][2]*useNumber),
						drawRectXYWH[rectId][1],
						drawRectXYWH[rectId][2],
						drawRectXYWH[rectId][3] );
		}
	}

	/// ユニットの購入
	/**
	 * @param [in] kind : ユニットの種別
	 * @return bool : 購入出来た場合true、購入出来なかった場合false
	 */
	public bool buyUnit( int kind )
	{
		int unitCost = Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(kind*Unit.DEF_PARAM_NUM)];
//		if( mainPoints < unitCost ){
//			return false;
//		}
		if( !checkUnitBuyPossible( kind ) ){
			return false;
		}
				
		mainPoints -= unitCost;
				
		setImageNumber( "num_main", mainPoints );
				
		return true;
	}

	/// ユニットの購入可否の確認
	/**
	 * @param [in] kind : ユニットの種別
	 * @return bool : 購入可能：true、購入不可：false
	 */
	public bool checkUnitBuyPossible( int kind )
	{
		int unitCost = Unit.DEF_PARAM_DATA[Unit.DEF_PARAM_COST_POINT+(kind*Unit.DEF_PARAM_NUM)];
		if( mainPoints < unitCost ){
			return false;
		}
				
		return true;
	}

	/// 各ユニットの購入可否を確認し、表示するアイコン画像を入れ替える
	/**
	 */
	public void checkUnitImg()
	{
		/// レーザー
		if( checkUnitBuyPossible( Unit.KIND_DEF_LASER ) ){
			if( Graphics2D.FindSprite( "btn_LASER_no" ).Visible ){
				Graphics2D.FindSprite( "btn_LASER_no" ).Visible = !Graphics2D.FindSprite( "btn_LASER_no" ).Visible;
				Graphics2D.FindSprite( "pt_syw" ).Visible = !Graphics2D.FindSprite( "pt_syw" ).Visible;
				Graphics2D.FindSprite( "num_yw_100" ).Visible = !Graphics2D.FindSprite( "num_yw_100" ).Visible;
				Graphics2D.FindSprite( "num_yw_10" ).Visible = !Graphics2D.FindSprite( "num_yw_10" ).Visible;
				Graphics2D.FindSprite( "num_yw_1" ).Visible = !Graphics2D.FindSprite( "num_yw_1" ).Visible;
			}
		}else{
			if( !Graphics2D.FindSprite( "btn_LASER_no" ).Visible ){
				Graphics2D.FindSprite( "btn_LASER_no" ).Visible = !Graphics2D.FindSprite( "btn_LASER_no" ).Visible;
				Graphics2D.FindSprite( "pt_syw" ).Visible = !Graphics2D.FindSprite( "pt_syw" ).Visible;
				Graphics2D.FindSprite( "num_yw_100" ).Visible = !Graphics2D.FindSprite( "num_yw_100" ).Visible;
				Graphics2D.FindSprite( "num_yw_10" ).Visible = !Graphics2D.FindSprite( "num_yw_10" ).Visible;
				Graphics2D.FindSprite( "num_yw_1" ).Visible = !Graphics2D.FindSprite( "num_yw_1" ).Visible;
			}
		}

		/// ワイドレーザー
		if( checkUnitBuyPossible( Unit.KIND_DEF_WIDE_LASER ) ){
			if( Graphics2D.FindSprite( "btn_WIDE_no" ).Visible ){
				Graphics2D.FindSprite( "btn_WIDE_no" ).Visible = !Graphics2D.FindSprite( "btn_WIDE_no" ).Visible;
				Graphics2D.FindSprite( "pt_sgw" ).Visible = !Graphics2D.FindSprite( "pt_sgw" ).Visible;
				Graphics2D.FindSprite( "num_gw_100" ).Visible = !Graphics2D.FindSprite( "num_gw_100" ).Visible;
				Graphics2D.FindSprite( "num_gw_10" ).Visible = !Graphics2D.FindSprite( "num_gw_10" ).Visible;
				Graphics2D.FindSprite( "num_gw_1" ).Visible = !Graphics2D.FindSprite( "num_gw_1" ).Visible;			
			}
		}else{
			if( !Graphics2D.FindSprite( "btn_WIDE_no" ).Visible ){
				Graphics2D.FindSprite( "btn_WIDE_no" ).Visible = !Graphics2D.FindSprite( "btn_WIDE_no" ).Visible;
				Graphics2D.FindSprite( "pt_sgw" ).Visible = !Graphics2D.FindSprite( "pt_sgw" ).Visible;
				Graphics2D.FindSprite( "num_gw_100" ).Visible = !Graphics2D.FindSprite( "num_gw_100" ).Visible;
				Graphics2D.FindSprite( "num_gw_10" ).Visible = !Graphics2D.FindSprite( "num_gw_10" ).Visible;
				Graphics2D.FindSprite( "num_gw_1" ).Visible = !Graphics2D.FindSprite( "num_gw_1" ).Visible;			
			}
		}

		/// 榴弾砲
		if( checkUnitBuyPossible( Unit.KIND_DEF_HIGH_EXPLOSIVE ) ){
			if( Graphics2D.FindSprite( "btn_HIGH_no" ).Visible ){
				Graphics2D.FindSprite( "btn_HIGH_no" ).Visible = !Graphics2D.FindSprite( "btn_HIGH_no" ).Visible;
				Graphics2D.FindSprite( "pt_sbw" ).Visible = !Graphics2D.FindSprite( "pt_sbw" ).Visible;
				Graphics2D.FindSprite( "num_bw_100" ).Visible = !Graphics2D.FindSprite( "num_bw_100" ).Visible;
				Graphics2D.FindSprite( "num_bw_10" ).Visible = !Graphics2D.FindSprite( "num_bw_10" ).Visible;
				Graphics2D.FindSprite( "num_bw_1" ).Visible = !Graphics2D.FindSprite( "num_bw_1" ).Visible;			
			}
		}else{
			if( !Graphics2D.FindSprite( "btn_HIGH_no" ).Visible ){
				Graphics2D.FindSprite( "btn_HIGH_no" ).Visible = !Graphics2D.FindSprite( "btn_HIGH_no" ).Visible;
				Graphics2D.FindSprite( "pt_sbw" ).Visible = !Graphics2D.FindSprite( "pt_sbw" ).Visible;
				Graphics2D.FindSprite( "num_bw_100" ).Visible = !Graphics2D.FindSprite( "num_bw_100" ).Visible;
				Graphics2D.FindSprite( "num_bw_10" ).Visible = !Graphics2D.FindSprite( "num_bw_10" ).Visible;
				Graphics2D.FindSprite( "num_bw_1" ).Visible = !Graphics2D.FindSprite( "num_bw_1" ).Visible;			
			}
		}

		/// ミサイル
		if( checkUnitBuyPossible( Unit.KIND_DEF_MISSILE ) ){
			if( Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible ){
				Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible = !Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible;
				Graphics2D.FindSprite( "pt_srw" ).Visible = !Graphics2D.FindSprite( "pt_srw" ).Visible;
				Graphics2D.FindSprite( "num_rw_100" ).Visible = !Graphics2D.FindSprite( "num_rw_100" ).Visible;
				Graphics2D.FindSprite( "num_rw_10" ).Visible = !Graphics2D.FindSprite( "num_rw_10" ).Visible;
				Graphics2D.FindSprite( "num_rw_1" ).Visible = !Graphics2D.FindSprite( "num_rw_1" ).Visible;
			}
		}else{
			if( !Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible ){
				Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible = !Graphics2D.FindSprite( "btn_MISSILE_no" ).Visible;
				Graphics2D.FindSprite( "pt_srw" ).Visible = !Graphics2D.FindSprite( "pt_srw" ).Visible;
				Graphics2D.FindSprite( "num_rw_100" ).Visible = !Graphics2D.FindSprite( "num_rw_100" ).Visible;
				Graphics2D.FindSprite( "num_rw_10" ).Visible = !Graphics2D.FindSprite( "num_rw_10" ).Visible;
				Graphics2D.FindSprite( "num_rw_1" ).Visible = !Graphics2D.FindSprite( "num_rw_1" ).Visible;
			}
		}
	}

	/// 指定したメッセージ画像を表示
	/**
	 * @param mess : 指定画像の登録名
	 */
	public void SetMessageUI( String mess )
	{
		clearMessageUI();
				
		Graphics2D.FindSprite( mess ).Visible = true;
	}

	/// すべてのメッセージ画像を非表示とする
	/**
	 */
	public void clearMessageUI()
	{
		Graphics2D.FindSprite( "firing" ).Visible = false;
		Graphics2D.FindSprite( "alert" ).Visible = false;
		Graphics2D.FindSprite( "destroy" ).Visible = false;
		Graphics2D.FindSprite( "large" ).Visible = false;
		Graphics2D.FindSprite( "destroyed" ).Visible = false;
		Graphics2D.FindSprite( "broken" ).Visible = false;
		Graphics2D.FindSprite( "gameclear" ).Visible = false;
		Graphics2D.FindSprite( "gameover" ).Visible = false;
	}
	
	/// カメラ自動移動の開始点、終了点を指定
	/**
	 * @param start : 開始点
	 * @param end : 終了点
	 * @param time : 移動に掛ける時間(ミリ秒）
	 */
	public void SetCameraMove( Vector3 start, Vector3 end, long time )
	{
		gameStateCameraStart = start;
		gameStateCameraEnd = end;
		gameStateCameraMoveTime = time;
		gameStateTimeStart = sceneTimer.ElapsedMilliseconds;
		gameStateCameraMoveFlg = true;
	}

	/// カメラの自動移動処理
	/**
	 * @warning 先にSetCameraMove()で開始点、終了点、時間を設定する必要がある。
	 */
	public void runCameraMove()
	{
		if( !gameStateCameraMoveFlg ){
			return;
		}
				
		long cnt = sceneTimer.ElapsedMilliseconds - gameStateTimeStart;
		Vector3 dir = new Vector3(
					(gameStateCameraEnd.X - gameStateCameraStart.X),
					(gameStateCameraEnd.Y - gameStateCameraStart.Y),
					(gameStateCameraEnd.Z - gameStateCameraStart.Z) );
		dir = dir.Normalize();
		float dis = comUtil.GetDistance( gameStateCameraStart, gameStateCameraEnd );
				
		if( cnt > gameStateCameraMoveTime ){
/*					
			camLookAtX = tmpCamX;
			camLookAtY = tmpCamY;
			camLookAtZ = tmpCamZ;
			camTrgPos.X = tmpCamX;
			camTrgPos.Y = tmpCamY;
			camTrgPos.Z = tmpCamZ;
*/			
			gameStateCameraMoveFlg = false;
			return;
		}
				
		float disOne = dis/(float)gameStateCameraMoveTime;
				
		tmpCamX = gameStateCameraStart.X + (dir.X * (disOne*cnt));
		tmpCamY = gameStateCameraStart.Y + (dir.Y * (disOne*cnt));
		tmpCamZ = gameStateCameraStart.Z + (dir.Z * (disOne*cnt));
/*				
		camInfo.GetPosture().SetLookAt2(
			new Vector3( testCamRotX, testCamRotY, testCamRotZ ),
			new Vector3( tmpCamX, tmpCamY, tmpCamZ ),
			tesetDist );
*/				
		comUtil.SetLog( "[log][SceneGame.cs]tmpCamX:"+tmpCamX );
		comUtil.SetLog( "[log][SceneGame.cs]tmpCamY:"+tmpCamY );
		comUtil.SetLog( "[log][SceneGame.cs]tmpCamZ:"+tmpCamZ );
	}
			
	public Vector3 GetRotatePos( Vector3 basePos, Vector3 trgPos, float angle )
	{
		Vector3 result = new Vector3( 0.0f, 0.0f, 0.0f );

		Posture tmpPosture = new Posture();
		Matrix4 baseMat;
		Matrix4 trgMat;
		Matrix4 baseMatInverse;
		Matrix4 resMat;
					
		CameraInfo camInfo = CameraInfo.Inst();
		tmpPosture.SetPosition( camInfo.GetPosture().GetPosition().X,
								camInfo.GetPosture().GetPosition().Y,
								camInfo.GetPosture().GetPosition().Z );
		baseMat = tmpPosture.GetPosture();
		tmpPosture.SetPosition( 0.0f, 0.0f, 0.0f );
		trgMat = tmpPosture.GetPosture();
		baseMatInverse = Matrix4.Inverse( baseMat );
					
		baseMat = baseMat * baseMatInverse;
		trgMat = baseMatInverse * trgMat;	
				
		float x = (float)(trgMat.M41*Math.Cos((double)angle)) - (float)(trgMat.M43*Math.Sin((double)angle));
		float y = (float)(trgMat.M41*Math.Sin((double)angle)) + (float)(trgMat.M43*Math.Cos((double)angle));
				
		tmpPosture.SetPosition( x, 0.0f, y );
		resMat = tmpPosture.GetPosture();
		resMat = baseMatInverse * resMat;

		result.X = resMat.M41;
		result.Y = 0.0f;
		result.Z = resMat.M43;
				
		return result;
	}
			
	public void SetGameState( int state )
	{
		gameStatePrev = gameState;
		gameState = state;
	}

}


} // end ns DefenseDemo

//===
// EOF
//===
