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
 * EnemyAUnitModelクラス
 */
public class EnemyAUnitModel : MonoModel
{
    /// コンストラクタ
    public EnemyAUnitModel() : base()
    {
        modelInfo = new MonoModelInfo(20, CollisionLevel.EnemyUnit);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("EnemyA")});
        action.Add("Damage", new List<ModelAnim>() {createAnim1("EnemyA"),
                                                    createAnim3("Reflection")});
        action.Add("Explode", new List<ModelAnim>() {createAnim3("EnemyExplode", 0, RepeatMode.Constant)});

        SetAction("Default");
    }
}

} // ShootingDemo
