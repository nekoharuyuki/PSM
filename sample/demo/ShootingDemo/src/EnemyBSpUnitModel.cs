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
 * EnemyBSpUnitModelクラス
 */
public class EnemyBSpUnitModel : MonoModel
{
    /// コンストラクタ
    public EnemyBSpUnitModel() : base()
    {
        modelInfo = new MonoModelInfo(32, CollisionLevel.EnemyUnit, 14);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("EnemyBSp")});
        action.Add("Default", new List<ModelAnim>() {createAnim1("EnemyBSp", 0)});
        action.Add("DefToStand", new List<ModelAnim>() {createAnim1("EnemyBSp", 1, RepeatMode.Constant)});
        action.Add("Stand", new List<ModelAnim>() {createAnim1("EnemyBSp", 2)});
        action.Add("StandToDef", new List<ModelAnim>() {createAnim1("EnemyBSp", 3, RepeatMode.Constant)});
        action.Add("Attack", new List<ModelAnim>() {createAnim1("EnemyBSp", 4, RepeatMode.Constant)});
        action.Add("Death", new List<ModelAnim>() {createAnim1("EnemyBSp", 5, RepeatMode.Constant)});
        action.Add("Damage", new List<ModelAnim>() {createAnim1("EnemyBSp"),
                                                    createAnim3("Reflection")});
        action.Add("Explode", new List<ModelAnim>() {createAnim3("EnemyExplode", 0, RepeatMode.Constant)});

        SetAction("Default");
    }
}

} // ShootingDemo
