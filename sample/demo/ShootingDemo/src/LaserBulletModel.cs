/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * LaserBulletModelクラス
 */
public class LaserBulletModel : MonoModel
{
    /// コンストラクタ
    public LaserBulletModel() : base()
    {
        modelInfo = new MonoModelInfo(32, CollisionLevel.Bullet);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("BossLaser")});

        SetAction("Default");
    }
}

} // ShootingDemo
