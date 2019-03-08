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
 * BossReflectionクラス
 */
public class BossReflection : Mono
{
    /// アクション
    private ModelAction action = null;

    /// Z値
    public override float ZParam {
        get {return -4;}
    }

    /// コンストラクタ
    public BossReflection(string name = null) : base(name)
    {
    }

    /// デストラクタ
    ~BossReflection()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager)
    {
        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {MonoModel.createAnim("Reflection", 0, RepeatMode.Constant, new Vector3(0f, 0f, -8.0f), true)});

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
