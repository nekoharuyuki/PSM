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
 * BossBulletModelクラス
 */
public class BossBulletModel : MonoModel
{
    /// コンストラクタ
    public BossBulletModel() : base()
    {
        modelInfo = new MonoModelInfo(20, CollisionLevel.Bullet);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("BossBullet")});

        SetAction("Default");
    }
}

} // ShootingDemo
