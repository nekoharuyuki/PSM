/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * GameTimerクラス
 */
public class GameTimer
{
    private long uptimeMillis = 0;

    /// 開始
    public void Start()
    {
        uptimeMillis = 0;
    }

    /// 更新
    public void Update()
    {
        uptimeMillis += GameData.FrameTimeMillis;
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
}

} // ShootingDemo
