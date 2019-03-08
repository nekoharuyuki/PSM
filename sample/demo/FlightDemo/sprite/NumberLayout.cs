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
 * NumberLayoutクラス
 */
public class NumberLayout : IDisposable
{
    private SpriteAnimation[] anims;
    private SpriteAnimation[] signs;
    private int[] offsetX;
    private int[] offsetY;
    private int maxDigit;

    ///
    public NumberLayout(SpriteAnimation[] anims,
                        SpriteAnimation[] sings,
                        int[] offsetX, int[] offsetY)
    {
        this.anims = anims;
        this.signs = sings;
        this.offsetX = offsetX;
        this.offsetY = offsetY;

        if( this.signs == null ){
            maxDigit = offsetX.Length;
        }else{
            maxDigit = offsetX.Length - 1;
        }
    }

    ///
    public void Dispose()
    {
        Array.ForEach(anims, (anim) => anim.Dispose());
        anims = null;
    }

    public void Render(long number)
    {
        Render(number, 0, 0);
    }

    ///
    public void Render(long number, int ofstX, int ofstY, bool zero = true )
    {
        int digit = 0;

        long calcNumber = Math.Abs(number);
        while (calcNumber > 0) {
            calcNumber /= 10;
            digit++;
        }

        if (digit > maxDigit) {
            digit = maxDigit;
        }

        calcNumber = Math.Abs(number);
        for (int i = 0; i < digit; i++) {
            anims[calcNumber % 10].Render(0, offsetX[i]+ofstX, offsetY[i]+ofstY);
            calcNumber /= 10;
        }

        if( zero ){
            for (int i = digit; i < maxDigit; i++) {
                anims[0].Render(0, offsetX[i]+ofstX, offsetY[i]+ofstY);
                digit++;
            }
        }
        if( this.signs != null ){
            //            sings[0].Render(0, offsetX[maxDigit]+ofstX, offsetY[maxDigit]+ofstY);
            if( number > 0 && signs.Length >= 1 ){
                if( signs[ 0 ] != null ){
                    signs[0].Render(0, offsetX[digit]+ofstX, offsetY[digit]+ofstY);
                }
            }
            if( number < 0 && signs.Length >= 2 ){
                if( signs[ 1 ] != null ){
                    signs[1].Render(0, offsetX[digit]+ofstX, offsetY[digit]+ofstY);
                }
            }
            
        }

    }

}

} // ShootingDemo
