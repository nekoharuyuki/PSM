/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using DemoModel;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * EnterPlayerHandleクラス
 */
public class EnterPlayerHandle : ScriptUnitHandle
{
    /// 無敵中か否か
    private bool Unrivaled;

    /// コンストラクタ
    public EnterPlayerHandle(bool unrivaled = false) : base()
    {
        Unrivaled = unrivaled;
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono mono)
    {
        MoveBasic(20, 0, 0, 16,    0, 0, 0);
        MoveBasic( 1, 0, 0, 12.8f, 0, 0, 0);
        MoveBasic( 1, 0, 0, 10.2f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  8.2f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  6.5f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  5.2f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  4.2f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  3.3f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  2.7f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  2.1f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  1.7f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  1.4f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  1.1f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  0.9f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  0.7f, 0, 0, 0);
        MoveBasic( 1, 0, 0,  0.5f, 0, 0, 0);

        AudioManager.PlaySound("Enter", false);

        return true;
    }

    /// スクリプト呼び出し終了
    protected override void callScriptEnd(MonoManager monoManager, Mono mono)
    {
        mono.Remove(monoManager, false);

        PlayerUnit unit = new PlayerUnit(mono.WorldMatrix,
                                         GroupId.Player,
                                         "Player");

        monoManager.Regist(unit);

        if ( Unrivaled ) {
            unit.SetUnrivaledTime(60);
        }
    }
}

} // ShootingDemo
