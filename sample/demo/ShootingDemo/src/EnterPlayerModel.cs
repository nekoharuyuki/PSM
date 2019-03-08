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
 * EnterPlayerModelクラス
 */
public class EnterPlayerModel : MonoModel
{
    /// コンストラクタ
    public EnterPlayerModel() : base()
    {
        modelInfo = new MonoModelInfo(16, CollisionLevel.None);

        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {createAnim1("Player", 0),
                                                     createAnim3("Enter", 0, RepeatMode.Constant)});

        SetAction("Default");
    }
}

} // ShootingDemo
