/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * NormalBulletHandle
 */
public class NormalBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public NormalBulletHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(  1, 0, -7,  6, 0, 0, 0);
        MoveBasic(199, 0,  0, 50, 0, 0, 0);

        AudioManager.PlaySound("NormalBulletShoot", false);

        return true;
    }

    /// 攻撃呼び出し
    public override bool CallAttack (MonoManager monoManager, Mono bullet, Mono mono)
    {
        base.CallAttack (monoManager, bullet, mono);

        if( mono.Hitpoint > 1 )
        {
            bullet.Remove(monoManager);
        }

        return true;
    }

}

} // ShootingDemo
