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
 * PlayerExplodeクラス
 */
public class PlayerExplode : Mono
{
    /// アクション
    private ModelAction action = null;

    /// Z値
    public override float ZParam {
        get {return -3;}
    }

    /// コンストラクタ
    public PlayerExplode(Matrix4 matrix, string name = null) : base(name)
    {
        WorldMatrix = matrix;
    }

    /// デストラクタ
    ~PlayerExplode()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager)
    {
        action = new ModelAction();
        action.Add("Default", new List<ModelAnim>() {MonoModel.createAnim3("PlayerExplode", 0, RepeatMode.Constant)});

        action.SetCurrent("Default");

        AudioManager.PlaySound("PlayerExplode", false);

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

        /// アクション終了
        if (action.IsEndAction()) {
            monoManager.Remove(this);

            ShootingData.LostLifePoint();

            /// 残機数あり
            if (ShootingData.LifePoint > 0) {
                EnterPlayer unit = new EnterPlayer(new Vector3(0, -20, -480),
                                                   new Vector3(0, 0, 0),
                                                   new EnterPlayerHandle(true),
                                                   new EnterPlayerModel(),
                                                   0,
                                                   GroupId.Player,
                                                   "Player");
                monoManager.Regist(unit);

            /// 残機数なし
            } else {
                monoManager.Regist(new GameOverEffect());
            }
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
