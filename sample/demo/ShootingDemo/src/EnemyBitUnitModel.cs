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
 * EnemyBitUnitModelクラス
 */
public class EnemyBitUnitModel : MonoModel
{
    /// コンストラクタ
    public EnemyBitUnitModel() : base()
    {
        modelInfo = new MonoModelInfo(5, CollisionLevel.EnemyUnit, 12);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("EnemyBit")});
        action.Add("Damage", new List<ModelAnim>() {createAnim1("EnemyBit"),
                                                    createAnim3("Reflection")});
        action.Add("Explode", new List<ModelAnim>() {createAnim3("EnemyExplode", 0, RepeatMode.Constant)});

        SetAction("Default");
    }
}

} // ShootingDemo
