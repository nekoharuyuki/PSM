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
 * MissileBulletクラス
 */
public class MissileBullet : Unit
{
    /// Z値
    public override float ZParam {
        get {return -4;}
    }

    /// コンストラクタ
    public MissileBullet(Matrix4 matrix,
                        GroupId groupId,
                        string name = null) : base(matrix,
                                                   new MissileBulletHandle(),
                                                   new MissileBulletModel(),
                                                   groupId,
                                                   name)
    {
    }

    /// 描画
    public override bool Render(MonoManager monoManager)
    {
        if (model != null) {
            model.Render(this);
        }

        return true;
    }

}

} // ShootingDemo
