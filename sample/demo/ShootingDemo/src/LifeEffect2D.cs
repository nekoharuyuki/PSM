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
 * LifeUpEffectクラス
 */
public class LifeEffect2D : Mono
{
    private NumberLayout lifeLayout;
    private LayoutAction action;
    private long previousScore;

    ///
    public override float ZParam
    {
        get {return -10001;}
    }

    /// コンストラクタ
    public LifeEffect2D(string name = null) : base(name)
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        var image = ShootingData.Image;
        action = new LayoutAction();

        var spr0 = new Sprite(image, 479,  68,  52,  44, 220,  -2);
        var spr1 = new Sprite(image, 562, 111, 149, 138, 172, -52);
        var spr2 = new Sprite(image, 562, 249, 149, 138, 172, -52);
        var spr3 = new Sprite(image, 562, 387, 149, 138, 172, -52);
        var spr4 = new Sprite(image, 562, 525, 149, 138, 172, -52);

        var animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr0, 0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
        action.Add("none", animList);

        animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr1,    0, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2,  100, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3,  200, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4,  300, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3,  400, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2,  500, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1,  600, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2,  700, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3,  800, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr4,  900, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr3, 1000, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr2, 1100, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr1, 1200, 100, true, 0, 0, 0xff, 0, 0, 0xff));
        action.Add("oneup", animList);

        action.SetCurrent("none", RepeatMode.Loop);


        lifeLayout = new NumberLayout(
            new SpriteAnimation[] {
                new SpriteAnimation(new Sprite(image, 624, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 660, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 696, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 732, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 768, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 804, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 840, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 876, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 912, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 948, 57, 36, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff)
            },
            new int[]{318, 292},
            new int[]{2, 2});


        previousScore = ShootingData.TotalScore;

        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        if (action != null) {
            action.Dispose();
            action = null;
        }

        if (lifeLayout != null) {
            lifeLayout.Dispose();
            lifeLayout = null;
        }

        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        long currentScore = ShootingData.TotalScore;

        if (currentScore - previousScore >= ShootingData.LifeUpScore) {
            previousScore = ((currentScore/ShootingData.LifeUpScore)*ShootingData.LifeUpScore);
            ShootingData.AddLifePoint();

            action.ChangeCurrent("oneup", RepeatMode.Constant);
            AudioManager.PlaySound("1UP");
        }


        action.Update(GameData.FrameTimeMillis);

        if (action.CurrentKey == "oneup" && action.IsPlayEnd()) {
            action.ChangeCurrent("none", RepeatMode.Loop);
        }

        return true;
    }

    /// 描画処理
    public override bool Render(MonoManager monoManager)
    {
        action.Render();
        lifeLayout.Render(ShootingData.LifePoint);

        return true;
    }
}

} // ShootingDemo
