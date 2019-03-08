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
public class BaseEffect2D : Mono
{
    private LayoutAction action;

    ///
    public override float ZParam
    {
        get {return -10000;}
    }

    /// コンストラクタ
    public BaseEffect2D(string name = null) : base(name)
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        var image = ShootingData.Image;
        action = new LayoutAction();

        Sprite spr03 = new Sprite(image,   2,  57, 435,  53, 211,   2); // score
        Sprite spr13 = new Sprite(image,   2,   2, 900,  49, -22,   0); // shitaji

        LayoutAnimationList animList = new LayoutAnimationList();
        animList.Add(new SpriteAnimation(spr13, 0, 0, false, 0, 0, 0xff, 0, 0, 0xff));
        animList.Add(new SpriteAnimation(spr03, 0, 0, false, 0, 0, 0xff, 0, 0, 0xff));

        action.Add("Base", animList);
        action.SetCurrent("Base");

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
