/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

public delegate void UnitPostDelegate(MonoManager monoManager);

/**
 * UnitPostInfoクラス
 */
public class UnitPostInfo
{
    public long TimeMillis {get; protected set;}
    private UnitPostDelegate action;


    /// コンストラクタ
    public UnitPostInfo(long timeMillis, UnitPostDelegate action)
    {
        TimeMillis = GameData.TargetFps * timeMillis / 30;
        this.action = action;
    }

    /// 実行処理
    public void Action(MonoManager monoManager)
    {
        action(monoManager);
    }
}

} // ShootingDemo
