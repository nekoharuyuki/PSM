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
 * MissileBulletModelクラス
 */
public class MissileBulletModel : MonoModel
{
    /// コンストラクタ
    public MissileBulletModel() : base()
    {
        modelInfo = new MonoModelInfo(8, CollisionLevel.Bullet);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("MissileBullet")});

        SetAction("Default");
    }
}

} // ShootingDemo
