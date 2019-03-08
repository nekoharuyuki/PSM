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
 * EnterPlayerクラス
 */
public class EnterPlayer : Unit
{
    /// Z値
    public override float ZParam {
        get {return 4;}
    }

    /// コンストラクタ
    public EnterPlayer(Vector3 position,
                       Vector3 rotation,
                       MonoHandle handle,
                       MonoModel model,
                       int score,
                       GroupId groupId,
                       string name = null) : base(position,
                                                  rotation,
                                                  handle,
                                                  model,
                                                  score,
                                                  groupId,
                                                  name)
    {
    }

}

} // ShootingDemo
