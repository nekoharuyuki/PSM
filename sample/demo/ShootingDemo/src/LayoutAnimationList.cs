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
 * LayoutAnimationListクラス
 */
public class LayoutAnimationList : IDisposable
{
    private List<LayoutAnimation> animList = new List<LayoutAnimation>();

    /// 破棄
    public void Dispose()
    {
        animList.ForEach(anim => anim.Dispose());
    }

    /// アニメーションの追加
    public void Add(LayoutAnimation anim)
    {
        animList.Add(anim);
    }

    /// 描画処理
    public void Render(long animTimeMillis, int offsetX = 0, int offsetY = 0)
    {
        foreach (var anim in animList) {
            anim.Render(animTimeMillis, offsetX, offsetY);
        }
    }

    /// 再生終了時間の算出
    public long EndTimeMillis()
    {
        long endTimeMillis = 0;

        foreach (var anim in animList) {
            if (endTimeMillis < anim.EndTimeMillis) {
                endTimeMillis = anim.EndTimeMillis;
            }
        }

        return endTimeMillis;
    }
}

} // ShootingDemo
