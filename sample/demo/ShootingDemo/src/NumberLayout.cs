/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace ShootingDemo
{

/**
 * NumberLayoutクラス
 */
public class NumberLayout : IDisposable
{
    private SpriteAnimation[] anims;
    private int[] offsetX;
    private int[] offsetY;
    private int maxDigit;

    /// コンストラクタ
    public NumberLayout(SpriteAnimation[] anims, int[] offsetX, int[] offsetY)
    {
        this.anims = anims;
        this.offsetX = offsetX;
        this.offsetY = offsetY;

        maxDigit = offsetX.Length;
    }

    /// 破棄
    public void Dispose()
    {
        Array.ForEach(anims, (anim) => anim.Dispose());
        anims = null;
    }

    /// 描画
    public void Render(long number)
    {
        int digit = 0;

        long calcNumber = number;
        while (calcNumber > 0) {
            calcNumber /= 10;
            digit++;
        }

        if (digit > maxDigit) {
            digit = maxDigit;
        }

        calcNumber = number;
        for (int i = 0; i < digit; i++) {
            anims[calcNumber % 10].Render(0, offsetX[i], offsetY[i]);
            calcNumber /= 10;
        }

        for (int i = digit; i < maxDigit; i++) {
            anims[0].Render(0, offsetX[i], offsetY[i]);
        }
    }

}

} // ShootingDemo
