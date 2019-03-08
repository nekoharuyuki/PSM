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
 * EnemyBossUnitModelクラス
 */
public class EnemyBossUnitModel : MonoModel
{
    /// コンストラクタ
    public EnemyBossUnitModel() : base()
    {
        modelInfo = new MonoModelInfo(32, CollisionLevel.EnemyUnit, 82, 1, 0, 148);

        Matrix4 matrix = Matrix4.Identity;
        matrix *= Matrix4.Translation(new Vector3(0f, -50.0f, -50.0f));

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("EnemyBoss", 0) });
        action.Add("PreWeak", new List<ModelAnim>() {createAnim1("EnemyBoss", 1, RepeatMode.Constant)});
        action.Add("Weak", new List<ModelAnim>() {createAnim1("EnemyBoss", 2)});
        action.Add("PreLazer", new List<ModelAnim>() {createAnim1("EnemyBoss", 3, RepeatMode.Constant)});
        action.Add("DefLazer", new List<ModelAnim>() {createAnim1("EnemyBoss", 4)});
        action.Add("Lazer", new List<ModelAnim>() {createAnim1("EnemyBoss", 5, RepeatMode.Constant)});
        action.Add("Death", new List<ModelAnim>() {createAnim1("EnemyBoss", 6, RepeatMode.Constant)});
        action.Add("Damage", new List<ModelAnim>() {createAnim3("Reflection")});
        action.Add("Explode", new List<ModelAnim>() {createAnim3("EnemyExplode", 0, RepeatMode.Constant)});

        SetAction("Default");
    }
}

} // ShootingDemo
