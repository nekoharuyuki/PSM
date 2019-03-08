/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;

namespace ShootingDemo
{

/**
 * FrameTimeクラス
 */
public class FrameTime
{
    private long uptimeMillis = 0;
    private long previousElapsedMillis = 0;
    private long currentElapsedMillis = 0;
    private long frameMillis = 0;
    private Stopwatch timer = null;

    /// 開始
    public void Start()
    {
        timer = new Stopwatch();
        timer.Start();

        previousElapsedMillis = timer.ElapsedMilliseconds;
        currentElapsedMillis = previousElapsedMillis;

        frameMillis = 0;
        uptimeMillis = 0;
    }

    /// 終了
    public void Stop()
    {
        timer = null;

        previousElapsedMillis = 0;
        currentElapsedMillis = 0;

        frameMillis = 0;
        uptimeMillis = 0;
    }

    /// 更新
    public void Update()
    {
        if (timer != null) {
            previousElapsedMillis = currentElapsedMillis;
            currentElapsedMillis = timer.ElapsedMilliseconds;

            frameMillis = currentElapsedMillis - previousElapsedMillis;
            uptimeMillis += frameMillis;
        }
    }

    /// 稼働時間をミリ秒(1/1000)で取得
    public long UptimeMillis
    {
        get {return uptimeMillis;}
    }

    /// 稼働時間を秒で取得
    public long UptimeSeconds
    {
        get {return UptimeMillis / 1000;}
    }

    /// フレーム時間をミリ秒(1/1000)で取得
    public long FrameMillis
    {
        get {return frameMillis;}
    }

    /// フレーム時間を秒で取得
    public float FrameMillis_f32
    {
        get {return (float)FrameMillis / 1000;}
    }
}

} // ShootingDemo
