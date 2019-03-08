/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;

namespace Physics2dDemo
{

///
/// GameDataクラス
///
public static class GameData
{
    private static FrameTime frameTime;
    private static float currentMs;
    private static float currentFps;
	private static LayoutAction fadeAction;
	public static int stage_num=0;
	public static int windowPosX;	

	public static LayoutAction FadeAction
    {
        get {return fadeAction;}
    }
	
	/// 現在のステージ番号
    public static int StageNum
    {
        get {return stage_num;}
		set {stage_num = value;}
    }
		
	/// windowのスライド位置
	/// -854<=x<=0
	public static int WindowPosX
    {
        get {return windowPosX;}
		set {windowPosX = value;}
    }

    /// ターゲットFPS
    public static int TargetFps
    {
        get {return 30;}
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

	/// 今回のフレーム処理にかかったFPSのセット
    public static void SetCurrentFps(float currentFps)
    {
        GameData.currentFps = currentFps;
    }

    /// 今回のフレーム処理にかかったミリ秒の取得
    public static float GetCurrentFps()
    {
        return currentFps;
    }

    /// FrameTimeの取得
    public static FrameTime FrameTime
    {
        get {return frameTime;}
    }

    /// フレーム時間の取得(ミリ秒)
    public static long FrameTimeMillis
    {
        get {return 1000 / TargetFps;}
    }

    /// フレーム時間の取得(ミリ秒:float)
    public static float FrameTimeMillis_f32
    {
        get {return (float)FrameTimeMillis / 1000;}
    }
		
	/// 初期化
    public static void Init()
    {
        frameTime = new FrameTime();
        frameTime.Start();
        fadeAction = new LayoutAction();

        LayoutAnimationList animList;
        animList = new LayoutAnimationList();
        animList.Add(new FillRectAnimation(-1, -1, 0, 1500, false, 0, 0, 0xff000000, 0, 0, 0x00000000));
        fadeAction.Add("FadeIn", animList);

        animList = new LayoutAnimationList();
        animList.Add(new FillRectAnimation(-1, -1, 0, 1500, false, 0, 0, 0x00000000, 0, 0, 0xff000000));
        fadeAction.Add("FadeOut", animList);

        fadeAction.SetCurrent("FadeIn");

	}
    
	/// 解放
    public static void Term()
    {
        frameTime.Stop();
        frameTime = null;
    }		
}

} // Physics2dDemo
