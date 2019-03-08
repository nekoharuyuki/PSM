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
 * EnemyExplodeクラス
 */
public class EnemyExplode : Mono
{
    /// アクション
    private ModelAction action = null;

    /// Z値
    public override float ZParam {
        get {return -3;}
    }

    /// コンストラクタ
    public EnemyExplode(Matrix4 matrix, string name = null) : base(name)
    {
        WorldMatrix = matrix;
    }

    /// デストラクタ
    ~EnemyExplode()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager)
    {
        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {MonoModel.createAnim3("EnemyExplode", 0, RepeatMode.Constant)});

        action.SetCurrent("Default");

        AudioManager.PlaySound("EnemyExplode", false);

        return true;
    }

    /// 終了
    public override bool End(MonoManager monoManager)
    {
        action.Dispose();
        action = null;

        return true;
    }

    /// 更新
    public override bool Update(MonoManager monoManager)
    {
        action.Update(GameData.FrameTimeMillis);

        if (action.IsEndAction()) {
            monoManager.Remove(this);
        }

        return true;
    }

    /// 描画
    public override bool Render(MonoManager monoManager)
    {
        Matrix4 matrix = DrawMatrix;

        action.Render(ref matrix);

        return true;
    }
}

} // ShootingDemo
