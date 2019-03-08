/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using DemoGame;

namespace ShootingDemo
{

/**
 * EnemyBossクラス
 */
public class EnemyBoss : Unit
{
    /// Z値
    public override float ZParam {
        get {return -3;}
    }

    /// コンストラクタ
    public EnemyBoss(Vector3 position,
                        Vector3 rotation,
                        int score,
                        GroupId groupId,
                        string name = null) : base(position,
                                                   rotation,
                                                   new EnemyBossUnitHandle(),
                                                   new EnemyBossUnitModel(),
                                                   score,
                                                   groupId,
                                                   name)
    {
    }

    /// 描画
    public override bool Render(MonoManager monoManager)
    {
        if (model != null) {
            model.Render(this);
        }

        return true;
    }

}

} // ShootingDemo
