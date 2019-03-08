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

public class ChargeBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public ChargeBulletHandle()
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(20, 0, 0, 16, 0, 0, 0);

        AudioManager.PlaySound("ChargeBulletShoot", false);

        return true;
    }

}

public class ChargeBulletModel : MonoModel
{
    /// コンストラクタ
    public ChargeBulletModel() : base()
    {
        modelInfo = new MonoModelInfo(148, CollisionLevel.Bullet, 1, 8, 0);

        action = new ModelAction();
        action.Add("Attack", new List<ModelAnim>() {createAnim3("ChargeAttack", 0, RepeatMode.Constant)});

        SetAction("Attack");
    }
}

/**
 * ChargeBulletクラス
 */
public class ChargeBullet : Unit
{
    private Matrix4 drawMatrix;

    public override float ZParam {
        get {return WorldPosition.X + 1;}
    }

    /// コンストラクタ
    public ChargeBullet(Matrix4 matrix,
                        GroupId groupId,
                        string name = null) : base(matrix,
                                                   new ChargeBulletHandle(),
                                                   new ChargeBulletModel(),
                                                   groupId,
                                                   name)
    {
        drawMatrix = matrix;
    }

    /// 描画処理
    public override bool Render(MonoManager monoManager)
    {
        if (model != null) {
            model.Render(ref drawMatrix);
        }

        return true;
    }

}

} // ShootingDemo
