/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using DemoModel;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * EnemyBUnitHandleクラス
 */
public class EnemyBUnitHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public EnemyBUnitHandle() : base()
    {
    }

    /// 更新
    public override bool Update(MonoManager monoManager, Mono mono)
    {
        if (mono.MonoLifeState == MonoLifeState.Explode) {
            if (mono.IsEndAction()) {
                monoManager.Remove(mono);
            }
            return true;
        }

        return base.Update(monoManager, mono);
    }

    /// 爆破
    public override bool CallExplode(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        mono.ChangeAction("Death");

        monoManager.Regist(new EnemyExplode(mono.WorldMatrix*Matrix4.Translation(new Vector3(0, -20.0f,  20.0f))));
        monoManager.Regist(new EnemyExplode(mono.WorldMatrix*Matrix4.Translation(new Vector3(0,  10.0f, -30.0f))));

        return true;
    }

}

} // ShootingDemo
