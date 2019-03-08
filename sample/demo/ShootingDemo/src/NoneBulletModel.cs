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
 * NoneBulletModelクラス
 */
public class NoneBulletModel : MonoModel
{
    /// コンストラクタ
    public NoneBulletModel() : base()
    {
        modelInfo = new MonoModelInfo(48, CollisionLevel.Bullet);
    }

}

} // ShootingDemo
