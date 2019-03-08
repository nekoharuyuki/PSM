/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;
using DemoGame;

namespace FlightDemo{


public class SceneGameMain
    : IScene
{
    private SceneManager useSceneMgr;
    private FlightUnitManager unitMng;
    private GameCommonData unitCommonData;

    private TextureRenderer texRenderer;


    /// コンストラクタ
    public SceneGameMain()
    {
    }

    /// デストラクタ
    ~SceneGameMain()
    {
    }

    public bool Restart() {return true;}
    public bool Pause() {return true;}
    public void Suspend() {}
    public void Resume() {}





    private void initUnits()
    {
        unitMng = new FlightUnitManager( unitCommonData );

        unitMng.Regist( "Env", 0, new EnvLightingUnit() );

        unitMng.Regist( "Bg", 0, new BgUnit() );
        unitMng.Regist( "Bg", 1, new GuideLightUnit() );
        unitMng.Regist( "Bg", 2, new StartGateUnit() );
        unitMng.Regist( "Bg", 3, new GoalGateUnit() );
        unitMng.Regist( "Bg", 4, new FlightPipeUnit() );


        // アイテム類の配置
        // 表示順を兼ねる
        unitMng.Regist( "Gate", -1, new GateUnit( 82.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 80.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 79.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 77.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 75.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 74.0f ) );
        unitMng.Regist( "SpeedDown", -1, new ItemSpeedDownUnit( 72.0f ) );
        unitMng.Regist( "TimeDec", -1, new ItemTimeDecUnit( 71.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 70.0f ) );
        unitMng.Regist( "TimeInc", -1, new ItemTimeIncUnit( 68.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 67.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 65.0f ) );
        unitMng.Regist( "TimeInc", -1, new ItemTimeIncUnit( 64.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 62.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 60.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 58.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 56.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 55.0f ) );
        unitMng.Regist( "TimeInc", -1, new ItemTimeIncUnit( 54.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 53.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 52.0f ) );
        unitMng.Regist( "TimeInc", -1, new ItemTimeIncUnit( 51.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 50.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 49.0f ) );
        unitMng.Regist( "TimeDec", -1, new ItemTimeDecUnit( 48.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 47.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 46.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 44.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 42.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 36.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 32.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 38.0f ) );
        unitMng.Regist( "SpeedDown", -1, new ItemSpeedDownUnit( 37.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 29.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 26.0f ) );
        unitMng.Regist( "SpeedUp", -1, new ItemSpeedUpUnit( 23.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 20.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 16.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 14.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 11.0f ) );
        unitMng.Regist( "Gate", -1, new GateUnit( 9.0f ) );


        // 敵の配置
        unitMng.Regist( "AirShip", 0, new EnemyAirShipUnit( "/Application/res/data/airship/pass_airship.mdx" ) );
        unitMng.Regist( "Balloon", 0, new EnemyBalloonUnit( "/Application/res/data/balloon/pass_ballon1.mdx" ) );
        unitMng.Regist( "Balloon", 1, new EnemyBalloonUnit( "/Application/res/data/balloon/pass_ballon2.mdx" ) );
        unitMng.Regist( "EnemyPlane", 0, new EnemyPlaneUnit( "/Application/res/data/plane_enemy/pass_enemy.mdx" ) );

        unitMng.Regist( "Plane", 0, new PlaneUnit( new PlaneHandle(), new PlaneModel() ) );
        unitMng.Regist( "Camera", 0, new CameraUnit() );


        // 2D
        unitMng.Regist( "Timer", 0, new TimerUnit() );       // ゲームの進行役も兼ねる
        unitMng.Regist( "2DUI", 1, new SpeedMeterUnit() );
        unitMng.Regist( "2DUI", 2, new AltimeterUnit() );
		// バーチャルパッド
        unitMng.Regist( "Input", 0, new VirtualPadUnit() );
        unitMng.Regist( "Input", 1, new MotionInputUnit() );
    }
		
	private void initSound()
	{
		// サウンドの読み込み
		AudioManager.AddBgm( "Stage", 		"/Application/res/snd/F_GAME_BGM_001.mp3" );	// ステージBGM
		AudioManager.AddBgm( "Finish",		"/Application/res/snd/F_GAME_SE_010.mp3" );	// フィニッシュファンファーレ
		AudioManager.AddBgm( "TimeOver",	"/Application/res/snd/F_GAME_SE_011.mp3" );	// タイムオーバーファンファーレ
		AudioManager.AddSound( "Decide",	"/Application/res/snd/F_SYS_SE_001.wav" );	// 決定音
		AudioManager.AddSound( "Count",		"/Application/res/snd/F_GAME_SE_000.wav" );	// カウント音
		AudioManager.AddSound( "Start",		"/Application/res/snd/F_GAME_SE_001.wav" );	// スタート音
		AudioManager.AddSound( "EngineL",	"/Application/res/snd/F_GAME_SE_020.wav" );	// 低回転エンジン＋プロペラ
		AudioManager.AddSound( "EngineM",	"/Application/res/snd/F_GAME_SE_021.wav" );	// 中回転エンジン＋プロペラ
		AudioManager.AddSound( "EngineH",	"/Application/res/snd/F_GAME_SE_022.wav" );	// 高回転エンジン＋プロペラ
		AudioManager.AddSound( "GateOK",	"/Application/res/snd/F_GAME_SE_040.wav" );	// 旗門通過
		AudioManager.AddSound( "GateNG",	"/Application/res/snd/F_GAME_SE_041.wav" );	// 旗門不正通過
		AudioManager.AddSound( "ItemGood",	"/Application/res/snd/F_GAME_SE_050.wav" );	// 有利アイテム取得
		AudioManager.AddSound( "ItemBad",	"/Application/res/snd/F_GAME_SE_051.wav" );	// 不利アイテム取得
		AudioManager.AddSound( "SpeedUp",	"/Application/res/snd/F_GAME_SE_060.wav" );	// 加速
		AudioManager.AddSound( "SpeedDown",	"/Application/res/snd/F_GAME_SE_061.wav" );	// 減速
		AudioManager.AddSound( "TimeInc",	"/Application/res/snd/F_GAME_SE_070.wav" );	// タイム加算
		AudioManager.AddSound( "TimeDec",	"/Application/res/snd/F_GAME_SE_071.wav" );	// タイム減算
		AudioManager.AddSound( "Crash",		"/Application/res/snd/F_GAME_SE_080.wav" );	// 航空機衝突
		AudioManager.AddSound( "Boundary",	"/Application/res/snd/F_GAME_SE_090.wav" );	// 境界面接触
		AudioManager.AddSound( "Uncontrol",	"/Application/res/snd/F_GAME_SE_100.wav" );	// Uncontrollable
	}

    /// 初期化処理
    public bool Init( SceneManager sceneMng )
    {
        this.useSceneMgr = sceneMng;

        GraphicsContext graphics = Renderer.GetGraphicsContext();

        texRenderer = new TextureRenderer();
        texRenderer.BindGraphicsContext( graphics );

        unitCommonData = GameCommonData.Inst();
		//unitCommonData = GameCommonData.Inst();
		
        unitCommonData.SetProjection( Matrix4.Perspective( DemoUtil.Deg2Rad( 20.0f ),
                                                           Graphics2D.Width / (float)Graphics2D.Height,
                                                           1.0f, 1200000.0f ) );
        initUnits();

		initSound();
		AudioManager.PlayBgm( "Stage" );

        return true;
    }

    /// 解放処理
    public void Term()
    {
        unitCommonData = null;
        
        // サウンド解放
        AudioManager.Clear();
    }

    /// フレーム処理

    public bool Update()
    {
        float delta = 1.0f / 30.0f;

        if( unitMng.Update( delta ) == false ){
            useSceneMgr.Next( new SceneTitle(), false );
        }

        return true;
    }

    /// 描画処理
    public bool Render()
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();


        unitMng.Render();
			
#if DEBUG			
        Graphics2D.RemoveSprite("Ms");
        Graphics2D.AddSprite("Ms", "" + unitCommonData.GetMs() + "ms", 0xffffffff, 0, 0);
        Graphics2D.DrawSprite("Ms");
#endif				
        graphics.SwapBuffers();
        Graphics2D.ClearSprite();

        return true;
    }

}


} // end ns FlightDemo
//===
// EOF
//===
