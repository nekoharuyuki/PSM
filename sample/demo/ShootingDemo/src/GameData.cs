/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;

namespace ShootingDemo
{

/**
 * GameDataクラス
 */
public static class GameData
{
    private static FrameTime frameTime;
    private static float currentMs;
    private static LayoutAction fadeAction;
	private static int targetFps = 30;

    public static LayoutAction FadeAction
    {
        get {return fadeAction;}
    }

    /// ターゲットFPS
    public static int TargetFps
    {
		set {targetFps = value;}
        get {return targetFps;}
    }

    /// ターゲット画面横サイズ
    public static int TargetScreenWidth
    {
        get {return 854;}
    }

    /// ターゲット画面縦サイズ
    public static int TargetScreenHeight
    {
        get {return 480;}
    }

    /// 今回のフレーム処理にかかったミリ秒のセット
    public static void SetCurrentMs(float currentMs)
    {
        GameData.currentMs = currentMs;
    }

    /// 今回のフレーム処理にかかったミリ秒の取得
    public static float GetCurrentMs()
    {
        return currentMs;
    }

    /// 初期化
    public static void Init()
    {
        frameTime = new FrameTime();
        frameTime.Start();

        fadeAction = new LayoutAction();

        LayoutAnimationList animList;
        animList = new LayoutAnimationList();
        animList.Add(new FillRectAnimation(-1, -1, 0, 1000, false, 0, 0, 0xff000000, 0, 0, 0x00000000));
        fadeAction.Add("FadeIn", animList);

        animList = new LayoutAnimationList();
        animList.Add(new FillRectAnimation(-1, -1, 0, 1000, false, 0, 0, 0x00000000, 0, 0, 0xff000000));
        fadeAction.Add("FadeOut", animList);

        fadeAction.SetCurrent("FadeIn");
    }

    /// 解放
    public static void Term()
    {
        frameTime.Stop();
        frameTime = null;
    }

    /// FrameTimeの取得
    public static FrameTime FrameTime
    {
        get {return frameTime;}
    }

    /// フレーム時間の取得(ミリ秒)
    public static long FrameTimeMillis
    {
//        get {return frameTime.FrameMillis;}
        get {return 1000 / TargetFps;}
    }

    /// フレーム時間の取得(ミリ秒:float)
    public static float FrameTimeMillis_f32
    {
        get {return (float)FrameTimeMillis / 1000;}
    }
}

} // ShootingDemo
