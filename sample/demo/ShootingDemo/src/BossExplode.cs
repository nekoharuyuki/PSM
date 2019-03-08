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
 * BossExplodeクラス
 */
public class BossExplode : Mono
{
    /// アクション
    private ModelAction action = null;

    /// Z値
    public override float ZParam {
        get {return -3;}
    }

    /// コンストラクタ
    public BossExplode(Matrix4 matrix, string name = null) : base(name)
    {
        WorldMatrix = matrix;
    }

    /// デストラクタ
    ~BossExplode()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager)
    {
        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f, -50.0f, -50.0f), true,    0),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f,  50.0f,  50.0f), true,  800),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f,  50.0f, -50.0f), true, 1200),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f, -50.0f,  50.0f), true, 1500),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f, -50.0f, -50.0f), true, 1600),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f,  50.0f,  50.0f), true, 1700),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f,  30.0f,  30.0f), true, 1800),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f, -30.0f, -30.0f), true, 1900),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f,  50.0f, -50.0f), true, 2000),
                                                     MonoModel.createAnim("EnemyExplode", 0, RepeatMode.Constant, new Vector3(0f, -50.0f,  50.0f), true, 2100)});

        action.SetCurrent("Default");

        AudioManager.PlaySound("BossExplode", false);

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
