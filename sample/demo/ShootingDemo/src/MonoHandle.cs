/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

/**
 * MonoHandleクラス
 */
public abstract class MonoHandle
{
    /// 開始処理
    public abstract bool Start(MonoManager monoManager, Mono mono);

    /// 終了処理
    public abstract bool End(MonoManager monoManager, Mono mono);

    /// 更新処理
    public abstract bool Update(MonoManager monoManager, Mono mono);

    /// 攻撃呼び出し
    public abstract bool CallAttack(MonoManager monoManager, Mono mono, Mono collisionMono);

    /// ダメージ呼び出し
    public abstract bool CallDamage(MonoManager monoManager, Mono mono, Mono collisionMono);

    /// 破壊呼び出し
    public abstract bool CallExplode(MonoManager monoManager, Mono mono, Mono collisionMono);
}

} // ShootingDemo
