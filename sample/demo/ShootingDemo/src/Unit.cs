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
 * Unitクラス
 */
public class Unit : Mono
{
    protected MonoHandle handle = null;
    private List<Mono> attackUnitList = new List<Mono>();

    /// コンストラクタ
    public Unit(Vector3 position,
                Vector3 rotation,
                MonoHandle handle,
                MonoModel model,
                int score,
                GroupId groupId,
                string name = null) : base(name)
    {
        SetLocalMatrix(ref position, ref rotation);

        this.handle = handle;
        this.model = model;

        Score = score;
        GroupId = groupId;

        CollisionRadius = model.CollisionRadius;
        CollisionRadius2 = model.CollisionRadius2;
        CollisionLevel = model.CollisionLevel;
        Hitpoint = model.Hitpoint;
        Attack = model.Attack;
        Defense = model.Defense;
    }

    /// コンストラクタ
    public Unit(Matrix4 matrix,
                MonoHandle handle,
                MonoModel model,
                GroupId groupId,
                string name = null) : base(name)
    {
        WorldMatrix = matrix;

        this.handle = handle;
        this.model = model;

        Score = 0;
        GroupId = groupId;

        CollisionRadius = model.CollisionRadius;
        CollisionLevel = model.CollisionLevel;
        Hitpoint = model.Hitpoint;
        Attack = model.Attack;
        Defense = model.Defense;
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        attackUnitList.Clear();
        return handle.Start(monoManager, this);
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        attackUnitList.Clear();
        return handle.End(monoManager, this);
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        previousPosition = WorldPosition;

        handle.Update(monoManager, this);

        switch (MonoLifeState) {
        case MonoLifeState.Normal:
            UpdateNormal(monoManager);
            break;
        case MonoLifeState.Damage:
            UpdateDamage(monoManager);
            break;
        case MonoLifeState.Explode:
            UpdateExplode(monoManager);
            break;
        }

        return model.Update(this);
    }

    /// 攻撃呼び出し
    protected override bool CallAttack(MonoManager monoManager, Mono mono)
    {
        return handle.CallAttack(monoManager, this, mono);
    }

    /// 防御呼び出し
    protected override bool CallDefense(MonoManager monoManager, Mono mono)
    {
        // 同一ユニットの攻撃は受け付けない
        if (attackUnitList.Contains(mono)) {
            return true;
        }

        Hitpoint -= (mono.Attack - mono.Defense);

        if (Hitpoint <= 0) {
            attackUnitList.Clear();
            MonoLifeState = MonoLifeState.Explode;

            Hitpoint = 0;

            ShootingData.AddScore(Score);

            return handle.CallExplode(monoManager, this, mono);
        } else {
            // 非登録の要素の削除
            for (int i = attackUnitList.Count - 1; i >= 0; i--) {
                if (monoManager.IsRegist(attackUnitList[i]) == false) {
                    attackUnitList.Remove(attackUnitList[i]);
                }
            }

            attackUnitList.Add(mono);
            MonoLifeState = MonoLifeState.Damage;

            return handle.CallDamage(monoManager, this, mono);
        }
    }


}

} // ShootingDemo
