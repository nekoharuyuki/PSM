/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

#define SHARED_RESOURCES

using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace ShootingDemo
{

/**
 * ShootingDataクラス
 */
public static class ShootingData
{
    public static readonly int DefaultLifePoint = 3;
    public static readonly int MaxLife = 99;
    public static readonly long LifeUpScore = 100000;
    public static readonly long MaxScore = 9999999;

    private static int lifePoint;
    private static long totalScore;
    private static Texture2D image;
    private static ShaderContainer shaderContainer;

    /// コンストラクタ
    static ShootingData()
    {
        Clear();
    }

    /// ShaderContainer
    public static ShaderContainer ShaderContainer
    {
        get {return shaderContainer;}
    }

    /// 汎用Image
    public static Texture2D Image
    {
        get {return image;}
    }

    /// 初期化
    public static void Init(GraphicsContext graphics)
    {
#if SHARED_RESOURCES
		if (shaderContainer == null)
#endif
        shaderContainer = new ShaderContainer();

        image = new Texture2D("/Application/res/2d/2d_2.png", false);

        Clear();
    }

    /// 解放
    public static void Term()
    {
#if SHARED_RESOURCES
		// unneeded
#else
        if (shaderContainer != null) {
            shaderContainer.Dispose();
            shaderContainer = null;
        }
#endif

        image.Dispose();
    }

    /// クリア
    public static void Clear()
    {
        lifePoint = DefaultLifePoint;
        totalScore = 0;
    }

    /// 残機
    public static int LifePoint
    {
        get {return lifePoint;}
    }

    /// スコア
    public static long TotalScore
    {
        get {return totalScore;}
    }

    /// 残機の繰り下げ
    public static void LostLifePoint()
    {
        lifePoint--;
        if (lifePoint < 0) {
            lifePoint = 0;
        }
    }

    /// 残機の繰り上げ
    public static void AddLifePoint()
    {
        lifePoint++;

        if (lifePoint > MaxLife) {
            lifePoint = MaxLife;
        }
    }

    /// スコアの足しこみ
    public static void AddScore(long score)
    {
        totalScore += score;

        if (totalScore > MaxScore) {
            totalScore = MaxScore;
        }
    }
}

} // ShootingDemo
