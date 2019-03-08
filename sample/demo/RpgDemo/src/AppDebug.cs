/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Diagnostics;

namespace AppRpg {


///***************************************************************************
/// デバックパラメータ
///***************************************************************************
public static class AppDebug
{
    /// デバック用
    public static bool   PlDrawFlg        = true;
    public static bool   ToonFlg          = true;
    public static bool   AtkStepPlay      = false;
    public static bool   GravityFlg       = true;
    public static bool   CollLightFlg     = false;
    public static int    CollCnt          = 0;
    public static long   TimeCal          = 0;
    public static int    WoodCnt          = 0;

    private static long timeStart;
    private static long timeEnd;
    private static Stopwatch stopwatch = new Stopwatch();


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public static bool Init()
    {
        PlDrawFlg     = true;
        ToonFlg       = false;
        AtkStepPlay   = false;
        GravityFlg    = true;

        TimeCal       = 0;
        timeStart     = 0;
        timeEnd       = 0;

        stopwatch.Start();
        return true;
    }

    /// 破棄
    public static void Term()
    {
        stopwatch.Stop();
    }

    /// 
    public static void CheckTimeStart()
    {
        timeStart    = stopwatch.ElapsedMilliseconds;
    }
    public static void CheckTimeEnd()
    {
        timeEnd      = stopwatch.ElapsedMilliseconds;
        TimeCal      = timeEnd - timeStart;
    }
}

} // namespace
