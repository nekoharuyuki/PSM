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
 * EnemyReflectionクラス
 */
public class EnemyReflection : Mono
{
    /// アクション
    private ModelAction action = null;

    /// Z値
    public override float ZParam {
        get {return -2;}
    }

    /// コンストラクタ
    public EnemyReflection(string name = null) : base(name)
    {
    }

    /// デストラクタ
    ~EnemyReflection()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager)
    {
        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {MonoModel.createAnim3("Reflection", 0, RepeatMode.Constant)});

        action.SetCurrent("Default");

        AudioManager.PlaySound("Reflection", false);

        return true;
    }

    /// 終了
    public override bool End(MonoManager monoManager)
    {
        action.Dispose();
        action = null;

        if (Parent != null && Parent.MonoLifeState == MonoLifeState.Damage) {
            Parent.MonoLifeState = MonoLifeState.Normal;
        }

        return true;
    }

    /// 更新
    public override bool Update(MonoManager monoManager)
    {
        action.Update(GameData.FrameTimeMillis);

        if (Parent == null || action.IsEndAction()) {
            monoManager.Remove(this);
        }

        return true;
    }

    /// 描画
    public override bool Render(MonoManager monoManager)
    {
        if (Parent != null) {
            Matrix4 matrix = Parent.DrawMatrix;
            action.Render(ref matrix);
        }

        return true;
    }
}

} // ShootingDemo
