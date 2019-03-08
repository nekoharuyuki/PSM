/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using DemoGame;

namespace ShootingDemo
{

/**
 * GameOverEffectクラス
 */
public class GameOverEffect : Mono
{
    private LayoutAction action;
    private GameTimer soundTimer = new GameTimer();
    private bool playSound = false;

    ///
    public override float ZParam
    {
        get {return -10001;}
    }

    /// コンストラクタ
    public GameOverEffect(string name = "GameOver") : base(name)
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        var image = ShootingData.Image;
        action = new LayoutAction();

        Sprite spr0 = new Sprite(image, 2, 138, 547, 113, 154, 192);
        Sprite spr1 = new Sprite(image, 2, 251, 547, 113, 154, 192);
        Sprite spr2 = new Sprite(image, 2, 364, 547, 113, 154, 192);
        Sprite spr3 = new Sprite(image, 2, 477, 547, 113, 154, 192);
        Sprite spr4 = new Sprite(image, 2, 590, 547, 113, 154, 192);

        LayoutAnimationList animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr0,    0, 1000, true, 0, 0, 0x00, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 1000,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 1100,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 1200,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4, 1300,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 1400,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 1500,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 1600,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr0, 1700,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 1800,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 1900,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 2000,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4, 2100,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 2200,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 2300,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 2400,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr0, 2500,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 2600,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 2700,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 2800,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4, 2900,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 3000,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 3100,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 3200,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr0, 3300,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 3400,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 3500,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 3600,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4, 3700,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 3800,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 3900,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 4000,  100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr0, 4100, 1000, true, 0, 0, 0xff, 0, 0, 0x00));

        action.Add("default", animList);
        action.SetCurrent("default");

        AudioManager.StopBgm();

        soundTimer.Start();
        playSound = false;

        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        if (action != null) {
            action.Dispose();
            action = null;
        }

        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        action.Update(GameData.FrameTimeMillis);

        if (playSound == false) {
            soundTimer.Update();
            if (soundTimer.UptimeMillis > 1000) {
                AudioManager.PlaySound("Gameover", false);
                playSound = true;
            }
        }

        if (action.IsPlayEnd()) {
            monoManager.Remove(this);
        }

        return true;
    }

    /// 描画処理
    public override bool Render(MonoManager monoManager)
    {
        action.Render();

        return true;
    }
}

} // ShootingDemo
