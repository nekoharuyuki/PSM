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
 * BossBulletクラス
 */
public class BossBullet : Unit
{
    /// Z値
    public override float ZParam {
        get {return WorldPosition.X - 4;}
    }

    /// コンストラクタ
    public BossBullet(Matrix4 matrix,
                        GroupId groupId,
                        float paramZ = 0f,
                        string name = null) : base(matrix,
                                                   new BossBulletHandle(),
                                                   new BossBulletModel(),
                                                   groupId,
                                                   name)
    {
        WorldMatrix *= Matrix4.Translation(new Vector3(-paramZ, 0.0f, 0.0f));
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
