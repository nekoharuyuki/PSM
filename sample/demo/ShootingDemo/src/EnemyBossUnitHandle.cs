/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
///#define DEBUG_BOSS_CHECK
using System;
using System.Collections.Generic;
using DemoModel;
using DemoGame;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * EnemyBossUnitHandleクラス
 */
public class EnemyBossUnitHandle : ScriptUnitHandle
{
    /// 行動ステータス
    private enum UnitStatus
    {
        Start = 0,
        Bullet,
        Move,
        Laser,
        Death,
        End
    }

    private UnitStatus localStatus;

    /// レーザー回数
    private int laserNum;

    /// コンストラクタ
    public EnemyBossUnitHandle() : base()
    {
        localStatus = UnitStatus.Start;
        laserNum = 0;
    }

    /// デストラクタ
    ~EnemyBossUnitHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono unit)
    {
        /// 登場
        MoveBasic(150,  0,  0,  3, 0, 0, 0);
        /// ビット攻撃開始
        MoveBasic(1240,  0,  0,  0, 0, 0, 0);
        /// 退場
        MoveBasic( 99,  0, -4, -4, 0, 0, 0);
        MoveBasic(  1,  0, -4, -4, 0, 0, 0);

        return true;
    }

    /// 更新
    public override bool Update(MonoManager monoManager, Mono unit)
    {
        base.Update(monoManager,unit);

#if DEBUG_BOSS_CHECK
        var pad = InputManager.InputGamePad;

        /// デバッグ機能
        if ((pad.Trig & InputGamePadState.L) != 0) {
            /// ビット全滅
            if ( localStatus == UnitStatus.Start ) {
                /// ビット名称
                string[] ary = new string[] { "Bit0", "Bit45", "Bit90", "Bit135", "Bit180", "Bit225", "Bit270", "Bit315" };

                foreach( string i in ary ) {

                    var bit = monoManager.FindMono(i);

                    if ( bit != null) {
                        monoManager.Regist(new EnemyExplode(bit.WorldMatrix));
                        monoManager.Remove(bit);
                    }
                }
            }
            /// ボス全滅
            else if ( localStatus <= UnitStatus.Laser ) {
                CallExplode(monoManager, unit, null);
            }
        }
#endif

        /// 行動変化
        switch ( localStatus ) {

        /// ビット展開中
        case UnitStatus.Start:

            if (moveIndex == 1) {

                /// ビット名称
                string[] ary = new string[] { "Bit0", "Bit45", "Bit90", "Bit135", "Bit180", "Bit225", "Bit270", "Bit315" };

                foreach( string i in ary ) {
                    if ( monoManager.FindMono(i) != null) return true;
                }

                localStatus = UnitStatus.Bullet;

                /// 再初期化
                moveStep = moveIndex = attackStep = attackIndex = actionStep = actionIndex = 0;
                moveList.Clear();
                attackList.Clear();
                actionList.Clear();

                /// 手前
                MoveBasic(  16,  0, 0, 0, 0, 0, 0);
                /// 待機（専用弾）
                AttackBossNormal(0);
                MoveBasic( 200,  0, 0, 0, 0, 0, 0);
                /// 待機（専用弾）
                AttackBossNormal(100);
                /// 待機（更新用）
                MoveBasic( 199,  0, 0, 0, 0, 0, 0);

                /// 弱点出現
                ChangeAction(0,"PreWeak");
                ChangeAction(30,"Weak");
            }

            break;

        /// 弾・レーザー発射
        case UnitStatus.Bullet:
        case UnitStatus.Laser:

            if (moveIndex == 2) {

                var player = monoManager.FindMono("Player");

                if ( laserNum >= 3 ) {

                    localStatus = UnitStatus.End;

                    /// 再初期化
                    moveStep = moveIndex = attackStep = attackIndex = actionStep = actionIndex = 0;
                    moveList.Clear();
                    attackList.Clear();
                    actionList.Clear();

                    /// 退場
                    MoveBasic( 99,  0, -4, -4, 0, 0, 0);
                    MoveBasic(  1,  0, -4, -4, 0, 0, 0);

                    break;
                }

                if( ShootingData.LifePoint > 0 ) {
                    if ( player != null ) {

                        localStatus = UnitStatus.Move;

                        /// 再初期化
                        moveStep = moveIndex = attackStep = attackIndex = actionStep = actionIndex = 0;
                        moveList.Clear();
                        attackList.Clear();
                        actionList.Clear();

                        /// プレイヤーの位置を計算
                        int diffY = (int)( player.WorldPosition.Y - unit.WorldPosition.Y );

                        /// 移動
                        MoveBasic( ((diffY/4)!=0?Math.Abs(diffY/4):1), 0, (diffY>=0?4:-4), 0, 0, 0, 0);
                        /// 待機（更新用）
                        MoveBasic( 199,  0,  0, 0, 0, 0, 0);
                    }
                }

                /// 退場
                else {
                    localStatus = UnitStatus.End;

                    /// 再初期化
                    moveStep = moveIndex = attackStep = attackIndex = 0;
                    moveList.Clear();
                    attackList.Clear();

                    MoveBasic( 99,  0, -4, -4, 0, 0, 0);
                    MoveBasic(  1,  0, -4, -4, 0, 0, 0);
                }
            }

            break;

        /// 自機追いかけ
        case UnitStatus.Move:

            if (moveIndex == 1) {

                localStatus = UnitStatus.Laser;

                laserNum++;

                /// 再初期化
                moveStep = moveIndex = attackStep = attackIndex = actionStep = actionIndex = 0;
                moveList.Clear();
                attackList.Clear();
                actionList.Clear();

                /// 待機（発射準備）
                MoveBasic(  60, 0, 0, 0, 0, 0, 0);
                /// 待機（レーザー）
                MoveBasic( 100, 0, 0, 0, 0, 0, 0);
                /// 待機（更新用）
                MoveBasic( 199, 0, 0, 0, 0, 0, 0);

                /// レーザー
                AttackBossLaser(60);
                AttackBossNone( 8,60);
                AttackBossNone( 8,60);
                AttackBossNone( 8,60);
                AttackBossNone( 8,60);
                AttackBossNone( 8,60);
                AttackBossNone(10,40);
                AttackBossNone(10,24);
                AttackBossNone(10,6);

                /// レーザー（準備）
                ChangeAction(0,"PreLazer");
                /// レーザー（待機）
                ChangeAction(30,"DefLazer");
                /// レーザー（発射）
                ChangeAction(30,"Lazer");
                /// 待機
                ChangeAction(96,"Weak");
            }

            break;

        default:
            break;

        }

        return true;
    }

    /// ダメージ
    public override bool CallDamage(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        /// ビット破壊後＆ボス破壊前
        if ( localStatus != UnitStatus.Start && localStatus != UnitStatus.Death ) {
            monoManager.Regist(new BossReflection(), mono);
            return true;
        }

        /// ビット展開中
        else {
            mono.Hitpoint += collisionMono.Attack;
            mono.MonoLifeState = MonoLifeState.Normal;

            /// 通常弾時
            if ( collisionMono.Attack <= 1 ) {
                monoManager.Remove(collisionMono);
            }
        }

        return true;
    }

    /// 爆破
    public override bool CallExplode(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        localStatus = UnitStatus.Death;

        /// 再初期化
        ClearScript();

        /// レーザー削除
        var laser = monoManager.FindMono("Laser");

        if ( laser != null) {
            monoManager.Remove(laser);
        }

        /// 退場
        MoveBasic( 60, 0, 0, 0, 0, 0, 0);
        MoveBasic(  5, 0, 0, 0, 0, 0, 0);

        /// 死亡
        ChangeAction(0,"Death");

        StartScript();

        /// 爆破エフェクト再生
        monoManager.Regist(new BossExplode(mono.WorldMatrix));

        /// 爆破音再生
        AudioManager.PlaySound("BossExplode", false);

        return true;
    }

}

} // ShootingDemo
