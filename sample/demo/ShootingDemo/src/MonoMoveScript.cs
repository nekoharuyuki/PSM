/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace ShootingDemo
{

public delegate void MonoScriptDelegate(MonoManager monoManager, Mono mono);

/**
 * MonoMoveScriptクラス
 */
public class MonoMoveScript
{
    private MonoScriptDelegate monoScriptDelegate;

    /// コンストラクタ
    public MonoMoveScript(long frame, MonoScriptDelegate monoScriptDelegate)
    {
        Frame = GameData.TargetFps * frame / 30;

        this.monoScriptDelegate = monoScriptDelegate;
    }

    /// デストラクタ
    ~MonoMoveScript()
    {
    }

    /// フレーム
    public long Frame {get; private set;}

    /// 実行処理
    public void Action(MonoManager monoManager, Mono mono)
    {
        monoScriptDelegate(monoManager, mono);
    }
}


} // ShootingDemo
