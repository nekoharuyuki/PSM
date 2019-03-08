/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// SpriteAnimationクラス
/// スプライトのアニメーション
///
public class SpriteAnimation : LayoutAnimation
{
    private Sprite sprite;

    /// コンストラクタ
    /// 
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

    /// 破棄
    /// 
    public override void Dispose()
    {
        sprite.Dispose();
    }

    /// 描画処理
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

    /// 角度設定
    /// @param d 角度(float)
    public override void SetDegree(float d)
    {	
		this.sprite.Degree = -d * 180.0f / 3.14159265f;
   	}

	/// Center設定
    /// @param x 
    /// @param y
    public override void SetCenter(float x, float y)
    {
		this.sprite.CenterX = x;
		this.sprite.CenterY = y;
    }
		
	/// ScaleY設定
    /// @param scale
    public override void SetScaleY(float scale)
    {
		this.sprite.ScaleY = scale;
	}

}

} // Physics2dDemo
