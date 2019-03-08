/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * BossBulletHandle
 */
public class BossBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public BossBulletHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(199, 0, 0, 8, 0, 0, 0);

        return true;
    }
}

} // ShootingDemo
