/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * ShootingDemo
 */
public static class ShootingDemo
{
    private static GameMain gameMain;

    /// エントリーポイント
    public static void Main(string[] args)
    {
        gameMain = new GameMain();
        gameMain.SetUpperLimitFps(GameData.TargetFps);
        gameMain.Run(args);
    }
}

} // ShootingDemo
