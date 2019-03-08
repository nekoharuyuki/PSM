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
 * EnemyCSpUnitModelクラス
 */
public class EnemyCSpUnitModel : MonoModel
{
    /// コンストラクタ
    public EnemyCSpUnitModel() : base()
    {
        modelInfo = new MonoModelInfo(15, CollisionLevel.EnemyUnit, 3);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("EnemyCSp")});
        action.Add("Damage", new List<ModelAnim>() {createAnim1("EnemyCSp"),
                                                    createAnim3("Reflection")});
        action.Add("Explode", new List<ModelAnim>() {createAnim3("EnemyExplode", 0, RepeatMode.Constant)});

        SetAction("Default");
    }
}

} // ShootingDemo
