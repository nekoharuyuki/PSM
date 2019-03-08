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
 * PlayerUnitModelクラス
 */
public class PlayerUnitModel : MonoModel
{
    /// コンストラクタ
    public PlayerUnitModel() : base()
    {
        modelInfo = new MonoModelInfo(16, CollisionLevel.PlayerUnit, 1, 0, 0);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("Player", 0),createAnim1("BoostPlayer", 0)});

        action.Add("BoostLarge", new List<ModelAnim>() {createAnim1("Player", 0),createAnim1("BoostPlayer", 1)});
        action.Add("BoostSmall", new List<ModelAnim>() {createAnim1("Player", 0),createAnim1("BoostPlayer", 2)});

        SetAction("Default");
    }
}

} // ShootingDemo
