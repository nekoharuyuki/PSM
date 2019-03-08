/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using DemoGame;

namespace ShootingDemo
{

/**
 * SoundAnimationクラス
 */
public class SoundAnimation : LayoutAnimation
{
    private string soundKey;
    private bool loop;
    private bool play;

    /// コンストラクタ
    public SoundAnimation(string soundKey,
                          long startTimeMillis,
                          bool loop = false) : base(startTimeMillis, 1, true, 0, 0, 0, 0, 0, 0)
    {
        this.soundKey = soundKey;
        this.loop = loop;

        play = false;
    }

    /// 破棄
    public override void Dispose()
    {
    }

    /// 更新
    public override void Render(long animTimeMillis, int offsetX = 0, int offsetY = 0)
    {
        long animPlayTimeMillis = (animTimeMillis - startTimeMillis);

        if (animPlayTimeMillis >= 0 && play == false) {
            AudioManager.PlaySound(soundKey, loop);
            play = true;
        }
    }
}

} // ShootingDemo
