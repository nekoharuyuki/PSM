/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * NoneBulletHandle
 */
public class NoneBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public NoneBulletHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(19, 0, 0, 50, 0, 0, 0);

        return true;
    }
}

} // ShootingDemo
