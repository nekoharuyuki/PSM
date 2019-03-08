/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * LaserBulletHandle
 */
public class LaserBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public LaserBulletHandle()
    {
    }

    ///
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(96, 0, 0, 0, 0, 0, 0);

        AudioManager.PlaySound("LaserBulletShoot", false);

        return true;
    }
}

} // ShootingDemo
