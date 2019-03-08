/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo
{

/**
 * SpriteAnimationクラス
 */
public class SpriteAnimation : LayoutAnimation
{
    private Sprite sprite;

    /// コンストラクタ
    public SpriteAnimation(Sprite sprite,
                           long startTimeMillis,
                           long playTimeMillis,
                           bool killEnd,
                           int offsetX1,
                           int offsetY1,
                           uint color1,
                           int offsetX2,
                           int offsetY2,
                           uint color2) : base(startTimeMillis,
                                               playTimeMillis,
                                               killEnd,
                                               offsetX1,
                                               offsetY1,
                                               color1,
                                               offsetX2,
                                               offsetY2,
                                               color2)
    {
        this.sprite = sprite;
    }

    ///
    public override void Dispose()
    {
        sprite.Dispose();
    }

    ///
    public override void Render(long animTimeMillis, int offsetX = 0, int offsetY = 0)
    {
        long offsetTimeMillis = calcAnimPlayTimeMillis(animTimeMillis);

        if (offsetTimeMillis >= 0) {
            offsetX += getValue(offsetX1, offsetX2, offsetTimeMillis, playTimeMillis);
            offsetY += getValue(offsetY1, offsetY2, offsetTimeMillis, playTimeMillis);
            float alpha = (float)getValue(color1, color2, offsetTimeMillis, playTimeMillis) / 0xff;

            Graphics2D.DrawSprite(sprite, offsetX, offsetY, alpha);
        }
    }
}

} // ShootingDemo
