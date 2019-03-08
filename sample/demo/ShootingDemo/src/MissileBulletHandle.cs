/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * MissileBulletHandle
 */
public class MissileBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public MissileBulletHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(199, 0, 0, 14, 0, 0, 0);

        AudioManager.PlaySound("EnemyBulletShoot", false);

        return true;
    }
}

} // ShootingDemo
