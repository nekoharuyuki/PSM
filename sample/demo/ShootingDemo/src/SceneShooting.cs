/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Audio;
using DemoGame;

namespace ShootingDemo
{

/**
 * SceneShootingクラス
 */
public class SceneShooting : Scene
{
    private MonoManager monoManager;
    private UnitPostController unitPostManager;
    private StageInfo stageInfo;

    /// コンストラクタ
    public SceneShooting()
    {
    }

    /// デストラクタ
    ~SceneShooting()
    {
    }

    /// シーンの初期化
    public override bool Start()
    {
        ShootingData.Init(Renderer.GetGraphicsContext());

        ModelManager.Clear();

        stageInfo = new StageInfo();
        stageInfo.Setup();

        unitPostManager = new UnitPostController(stageInfo.GetUnitPostSchedule().GetScheduleList());

        monoManager = new MonoManager();
        monoManager.Regist(new PathCamera("Camera"));
        monoManager.Regist(new Stage("Stage"));
        monoManager.Regist(new BaseEffect2D());
        monoManager.Regist(new ScoreEffect2D());
        monoManager.Regist(new LifeEffect2D());
        EnterPlayer unit = new EnterPlayer(new Vector3(0, -20, -480),
                                           new Vector3(0, 0, 0),
                                           new EnterPlayerHandle(),
                                           new EnterPlayerModel(),
                                           0,
                                           GroupId.Player,
                                           "Player");
        monoManager.Regist(unit);

#if GRAPHICS_QUALITY_HIGH
		((Stage)(monoManager.FindMono("Stage"))).ShaderName = "BGShader";
#elif GRAPHICS_QUALITY_LOW
		((Stage)(monoManager.FindMono("Stage"))).ShaderName = "BGTexShader";
#else
		((Stage)(monoManager.FindMono("Stage"))).ShaderName = "BGShader";
#endif

        AudioManager.AddBgm("Stage", "/Application/res/snd/GAME_BGM_01.mp3"); // ステージBGM
        AudioManager.AddSound("NormalBulletShoot", "/Application/res/snd/GAME_SE_01.wav"); // 通常の弾を撃った時
        AudioManager.AddSound("ChargeBulletShoot", "/Application/res/snd/GAME_SE_02.wav"); // 溜め撃ちをした時
        AudioManager.AddSound("ChargeStart", "/Application/res/snd/GAME_SE_03.wav"); // 溜めている間にループ再生
        AudioManager.AddSound("ChargeEnd", "/Application/res/snd/GAME_SE_04.wav"); // 溜めが完了した際に再生
        AudioManager.AddSound("Reflection", "/Application/res/snd/GAME_SE_05.wav"); // 耐久度の高い敵に当たった時
        AudioManager.AddSound("PlayerExplode", "/Application/res/snd/GAME_SE_06.wav"); // 自機がやられた時
        AudioManager.AddSound("EnemyBulletShoot", "/Application/res/snd/GAME_SE_07.wav"); // ザコ敵が特定の攻撃をした時
        AudioManager.AddSound("LaserBulletShoot", "/Application/res/snd/GAME_SE_08.wav"); // ボスがレーザーを撃った時
        AudioManager.AddSound("Enter", "/Application/res/snd/GAME_SE_09.wav"); // ステージ開始時や復活時に
        AudioManager.AddSound("1UP", "/Application/res/snd/GAME_SE_10.wav"); // １UP時に
        AudioManager.AddSound("EnemyExplode", "/Application/res/snd/GAME_SE_11.wav"); // ザコ敵がやられた時
        AudioManager.AddSound("BossExplode", "/Application/res/snd/GAME_SE_12.wav"); // ボスがやられた時ｏｒやっつけた時（の場合は爆発音ではない）
        AudioManager.AddSound("Gameover", "/Application/res/snd/GAME_VO_02.wav"); // ゲームオーバー時

        AudioManager.PlayBgm("Stage");

        return true;
    }

    /// シーンの破棄
    public override void End()
    {
        ShootingData.Term();

        stageInfo.Dispose();
        stageInfo = null;

        Graphics2D.ClearSprite();

        monoManager.Dispose();
        monoManager = null;

        unitPostManager.Dispose();
        unitPostManager = null;

        AudioManager.Clear();
    }

    /// フレーム処理
    public override bool Frame()
    {
        unitPostManager.Update(monoManager);
        monoManager.Update();

        // ステージループテスト
        if (unitPostManager.IsPlayEnd()) {
            // ボスがいなかったら初期化
            if (monoManager.FindMono("Boss") == null) {
                unitPostManager.Start();
            }
        }

        // ゲームオーバー判定
        if (ShootingData.LifePoint <= 0 && monoManager.FindMono("GameOver") == null) {
            SetNextScene(new SceneTitle());
            return true;
        }

        return true;
    }

    /// 描画処理
    public override bool Draw()
    {
        monoManager.Render();

        return true;
    }
}

} // ShootingDemo
