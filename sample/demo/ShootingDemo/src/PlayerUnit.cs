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
 * PlayerUnitクラス
 */
public class PlayerUnit : Unit
{
    /// Z値
    public override float ZParam {
        get {return -1;}
    }

    /// コンストラクタ
    public PlayerUnit(Matrix4 matrix,
                        GroupId groupId,
                        string name = null) : base(matrix,
                                                   new PlayerUnitHandle(),
                                                   new PlayerUnitModel(),
                                                   groupId,
                                                   name)
    {
    }

    /// 衝突チェック
    public override bool IsCollision(Mono dstMono, Vector3 previousPosition, Vector3 nextPosition)
    {
        Vector3 collisionPos = new Vector3(0,0,0);

        /// 攻撃範囲と衝突範囲が異なる場合
        if (dstMono.CollisionRadius2 > 0) {
            return CommonCollision.CheckSphereAndSphere(GetCapsule(previousPosition, nextPosition), dstMono.GetSphere(dstMono.CollisionRadius2), ref collisionPos);
        }

        return CommonCollision.CheckSphereAndSphere(GetCapsule(previousPosition, nextPosition), dstMono.GetSphere(dstMono.CollisionRadius), ref collisionPos);
    }

}

} // ShootingDemo
