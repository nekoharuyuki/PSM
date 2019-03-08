/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using DemoGame;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * PlayerUnitHandleクラス
 */
public class PlayerUnitHandle : UnitHandle
{
    /// 押しっぱなし時のインターバル
    private int padInterval;

    /// 溜め中か否か
    private bool isChargeAttack;

    /// 自機のロール
    private float unitRotZ;

    /// コンストラクタ
    public PlayerUnitHandle()
    {
        padInterval = 0;
    }

    /// デストラクタ
    ~PlayerUnitHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono unit)
    {
        isChargeAttack = false;

        unitRotZ = 0;

        return true;
    }

    /// 終了
    public override bool End(MonoManager monoManager, Mono unit)
    {
        return true;
    }

    /// 更新
    public override bool Update(MonoManager monoManager, Mono unit)
    {
        if (unit.MonoLifeState == MonoLifeState.Explode) {
            return true;
        }

        var pad = InputManager.InputGamePad;

        float addY = 0;
        float addZ = 0;
        float rotZ = 0;

        float moveSpeed = 10.0f;
        float rotSpeed = 7.0f;

        /// 上下
        if ((pad.Scan & InputGamePadState.Down) != 0) {
            addY = -moveSpeed;
            if ( unitRotZ < 55.0f ) {
                rotZ = rotSpeed;
            }
        } else if ((pad.Scan & InputGamePadState.Up) != 0) {
            addY = moveSpeed;
            if ( unitRotZ > -55.0f ) {
                rotZ = -rotSpeed;
            }
        } else {
            if ( unitRotZ > 0 ) {
                rotZ = -rotSpeed;
            }
            else if ( unitRotZ < 0 ) {
                rotZ = rotSpeed;
            }
        }

        unitRotZ += rotZ;

        unit.AddRotate(0, 0, rotZ);

        /// 左右
        if ((pad.Scan & InputGamePadState.Left) != 0) {
            addZ = -moveSpeed;
            if ( unit.CurrentActionName() != "BoostSmall" ) {
                unit.ChangeAction("BoostSmall");
            }
        } else if ((pad.Scan & InputGamePadState.Right) != 0) {
            addZ = moveSpeed;
            if ( unit.CurrentActionName() != "BoostLarge" ) {
                unit.ChangeAction("BoostLarge");
            }
        }
        else {
            if ( unit.CurrentActionName() != "Default" ) {
                unit.ChangeAction("Default");
            }
        }

        /// 移動範囲
        int[] rect = new int[] { 70, 80, 140, 50 };

        if( unit.WorldPosition.Z + addZ < ((-832+rect[0])>>1) )
        {
            addZ = ((-832+rect[0])>>1) - unit.WorldPosition.Z;
        }
        else if( unit.WorldPosition.Z + addZ > ((832-rect[1])>>1) )
        {
            addZ = ((832-rect[1])>>1) - unit.WorldPosition.Z;
        }

        if( unit.WorldPosition.Y + addY > ((480-rect[2])>>1) )
        {
            addY = ((480-rect[2])>>1) - unit.WorldPosition.Y;
        }
        else if( unit.WorldPosition.Y + addY < ((-480+rect[3])>>1) )
        {
            addY = ((-480+rect[3])>>1) - unit.WorldPosition.Y;
        }

        unit.Translate(0, addY, addZ);

        /// 通常＋溜め
        if (((pad.Scan & InputGamePadState.Cross) != 0)||((pad.Scan & InputGamePadState.Circle) != 0)) {
            /// 溜め開始
            if ( !isChargeAttack ) {
                Mono mono = monoManager.FindMono("ChargeAttack");
                if (mono != null) {
                    mono.End(monoManager);
                    mono.Start(monoManager);
                } else {
                    monoManager.Regist(new ChargeAttack(), unit);
                }
                isChargeAttack = true;
            }
        }

        /// 連射
        else if (((pad.Scan & InputGamePadState.Square) != 0)||((pad.Scan & InputGamePadState.Triangle) != 0)) {
            /// 発射
            if ( padInterval++ == 0 ) {
                unit.ShootBullet1(monoManager);
            }
            padInterval %= 7;
        }

        /// 非連射
        else if (((pad.Scan & InputGamePadState.Square) == 0)&&((pad.Scan & InputGamePadState.Triangle) == 0)) {
            /// インターバル初期化
            padInterval = 0;
        }

        /// 溜め終了
        if ( isChargeAttack ) {
            if (((pad.Scan & InputGamePadState.Cross) == 0)&&((pad.Scan & InputGamePadState.Circle) == 0)) {
                isChargeAttack = false;
            }
        }

#if DEBUG
        /// 無敵（デバッグモード）
        if ((pad.Trig & InputGamePadState.Start) != 0) {
            if ( unit.UnrivaledTime > 0 ) {
                unit.PreCollisionLevel = CollisionLevel.None;
            }
            else {
               unit.CollisionLevel = (unit.CollisionLevel!=CollisionLevel.PlayerUnit?CollisionLevel.PlayerUnit:CollisionLevel.None);
            }
            AudioManager.PlaySound((unit.CollisionLevel==CollisionLevel.None?"1UP":"PlayerExplode"));
        }
#endif

        return true;
    }

    /// 爆破
    public override bool CallExplode(MonoManager monoManager, Mono mono, Mono collisionMono)
    {
        monoManager.Regist(new PlayerExplode(mono.WorldMatrix));
        monoManager.Remove(mono);

        return true;
    }

}

} // ShootingDemo
