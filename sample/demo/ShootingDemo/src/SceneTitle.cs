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

namespace ShootingDemo
{

/**
 * SceneTitleクラス
 */
public class SceneTitle : Scene
{
    private MonoManager monoManager;
    private Texture2D image;
    private LayoutAction action;
    private LayoutAction buttonAction;

    /// コンストラクタ
    public SceneTitle()
    {
    }

    /// デストラクタ
    ~SceneTitle()
    {
    }

    /// シーンの初期化
    public override bool Start()
    {
        ShootingData.Init(Renderer.GetGraphicsContext());

        monoManager = new MonoManager();
        monoManager.Regist(new Stage("Stage"));
#if GRAPHICS_QUALITY_HIGH
		((Stage)(monoManager.FindMono("Stage"))).ShaderName = "BGShader";
#elif GRAPHICS_QUALITY_LOW
		((Stage)(monoManager.FindMono("Stage"))).ShaderName = "BGTexShader";
#else
		((Stage)(monoManager.FindMono("Stage"))).ShaderName = "BGShader";
#endif

        image = new Texture2D("/Application/res/2d/2d_1.png", false);

        Graphics2D.AddSprite(new Sprite(image,   2, 274, 502, 315, 176,  31));
        Graphics2D.AddSprite(new Sprite(image,   2,   0, 568, 271, 145,  42));
        Graphics2D.AddSprite(new Sprite(image, 572,   2, 285,  59, 285, 346));

        AudioManager.AddSound("TitleCall2", "/Application/res/snd/GAME_VO_01.wav");
        AudioManager.AddSound("TitleCall1", "/Application/res/snd/SYS_SE_02.wav");
        AudioManager.AddSound("Press", "/Application/res/snd/SYS_SE_01.wav");

        action = new LayoutAction();

        Sprite spr1 = new Sprite(image,   2, 274, 502, 315, 176,  31);
        Sprite spr2 = new Sprite(image,   2,   0, 568, 271, 145,  42);

        Sprite spr3 = new Sprite(image, 572,   2, 285,  59, 285, 346);
        Sprite spr4 = new Sprite(image, 572,  61, 285,  59, 285, 346);
        Sprite spr5 = new Sprite(image, 572, 120, 285,  59, 285, 346);
        Sprite spr6 = new Sprite(image, 572, 179, 285,  59, 285, 346);
        Sprite spr7 = new Sprite(image, 572, 238, 285,  59, 285, 346);

        LayoutAnimationList animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr1, 1000, 1000, false, 0, 0, 0x00, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 2000, 1000, false, 0, 0, 0x00, 0, 0, 0xff));
        animList.Add(new SoundAnimation("TitleCall1", 1000));
        animList.Add(new SoundAnimation("TitleCall2", 2000));
        action.Add("test", animList);
        action.SetCurrent("test");

        buttonAction = new LayoutAction();
        animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr3, 3000, 500, true, 0, 0, 0x00, 0, 0, 0xff));
        buttonAction.Add("In", animList);

        animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr3, 0, 500, true, 0, 0, 0xff, 0, 0, 0x00));
        buttonAction.Add("Out", animList);

        animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr3,   0, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4, 100, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr5, 200, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr6, 300, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr7, 400, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr6, 500, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr5, 600, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4, 700, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        buttonAction.Add("Idle", animList);
        buttonAction.SetCurrent("In");

        return true;
    }

    /// シーンの破棄
    public override void End()
    {
        ShootingData.Term();

        Graphics2D.ClearSprite();

        monoManager.Dispose();
        monoManager = null;

        action.Dispose();

        image.Dispose();

        AudioManager.Clear();
    }

    /// フレーム処理
    public override bool Frame()
    {
        var pad = InputManager.InputGamePad;
        if (pad.Trig != 0) {
            if (buttonAction.CurrentKey == "Idle") {
                buttonAction.ChangeCurrent("Out");
                SetNextScene(new SceneShooting());
                AudioManager.PlaySound("Press");
                System.GC.Collect();
            }
            return true;
        }

        monoManager.Update();

        action.Update(GameData.FrameTimeMillis);
        buttonAction.Update(GameData.FrameTimeMillis);

        if (buttonAction.CurrentKey == "In" && buttonAction.IsPlayEnd()) {
            buttonAction.ChangeCurrent("Idle", RepeatMode.Loop);
        }

        return true;
    }

    /// 描画処理
    public override bool Draw()
    {
        monoManager.Render();
        action.Render();
        buttonAction.Render();

        return true;
    }

}

} // ShootingDemo
