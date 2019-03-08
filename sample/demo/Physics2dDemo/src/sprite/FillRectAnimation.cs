/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using DemoGame;

namespace Physics2dDemo
{

///
/// FillRectAnimationクラス
/// 
public class FillRectAnimation : LayoutAnimation
{
    private int rectW;
    private int rectH;

    /// コンストラクタ
    public FillRectAnimation(int rectW,
                             int rectH,
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
        if (rectW >= 0) {
            this.rectW = rectW;
        } else {
            this.rectW = GameData.TargetScreenWidth;
        }

        if (rectH >= 0) {
            this.rectH = rectH;
        } else {
            this.rectH = GameData.TargetScreenHeight;
        }
    }

    /// 破棄
    public override void Dispose()
    {
    }

    /// 描画処理
    public override void Render(long animTimeMillis, int offsetX = 0, int offsetY = 0)
    {
        long offsetTimeMillis = calcAnimPlayTimeMillis(animTimeMillis);

        if (offsetTimeMillis >= 0) {
            offsetX += getValue(offsetX1, offsetX2, offsetTimeMillis, playTimeMillis);
            offsetY += getValue(offsetY1, offsetY2, offsetTimeMillis, playTimeMillis);
            uint argb = getColor(color1, color2, offsetTimeMillis, playTimeMillis);

            Graphics2D.FillRect(argb, offsetX, offsetY, rectW, rectH);
        }
    }

    /// 指定時間の色の取得
    private static uint getColor(uint color1, uint color2, long nNowTime, long nMaxTime)
    {
        uint a = (uint)getValue((color1 & 0xff000000) >> 24,
                                (color2 & 0xff000000) >> 24, nNowTime, nMaxTime);
        uint r = (uint)getValue((color1 & 0x00ff0000) >> 16,
                                (color2 & 0x00ff0000) >> 16, nNowTime, nMaxTime);
        uint g = (uint)getValue((color1 & 0x0000ff00) >>  8,
                                (color2 & 0x0000ff00) >>  8, nNowTime, nMaxTime);
        uint b = (uint)getValue((color1 & 0x000000ff) >>  0,
                                (color2 & 0x000000ff) >>  0, nNowTime, nMaxTime);

        return ((a << 24) | (r << 16) | (g << 8) | (b << 0));
    }
}

} // Physics2dDemo
