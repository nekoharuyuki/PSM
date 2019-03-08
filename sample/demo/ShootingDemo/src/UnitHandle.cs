/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * UnitHandleクラス
 */
public abstract class UnitHandle : MonoHandle
{
    /// 攻撃呼び出し
    public override bool CallAttack(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        return true;
    }

    /// ダメージ呼び出し
    public override bool CallDamage(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        monoManager.Regist(new EnemyReflection(), mono);

        return true;
    }

    /// 破壊呼び出し
    public override bool CallExplode(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        monoManager.Regist(new EnemyExplode(mono.WorldMatrix));
        monoManager.Remove(mono);

        return true;
    }
}

} // ShootingDemo
