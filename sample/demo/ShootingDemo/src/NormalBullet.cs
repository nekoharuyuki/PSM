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
 * NormalBulletクラス
 */
public class NormalBullet : Unit
{
    /// Z値
    public override float ZParam {
        get {return -1;}
    }

    /// コンストラクタ
    public NormalBullet(Matrix4 matrix,
                        GroupId groupId,
                        string name = null) : base(matrix,
                                                   new NormalBulletHandle(),
                                                   new NormalBulletModel(),
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
