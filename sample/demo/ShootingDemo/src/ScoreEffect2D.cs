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
public class ScoreEffect2D : Mono
{
    private NumberLayout scoreLayout;

    /// Z値
    public override float ZParam
    {
        get {return -10001;}
    }

    /// コンストラクタ
    public ScoreEffect2D(string name = null) : base(name)
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        var image = ShootingData.Image;

        scoreLayout = new NumberLayout(
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
            new int[]{601, 575, 549, 523, 497, 471, 445},
            new int[]{2, 2, 2, 2, 2, 2, 2});

        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        if (scoreLayout != null) {
            scoreLayout.Dispose();
            scoreLayout = null;
        }

        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        return true;
    }

    /// 描画処理
    public override bool Render(MonoManager monoManager)
    {
        scoreLayout.Render(ShootingData.TotalScore);

        return true;
    }
}

} // ShootingDemo
