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
 * ScriptUnitHandleクラス
 */
public class ScriptUnitHandle : UnitHandle, IDisposable
{
    protected List<MonoMoveScript> moveList = new List<MonoMoveScript>();
    protected List<MonoMoveScript> attackList = new List<MonoMoveScript>();
    protected List<MonoMoveScript> actionList = new List<MonoMoveScript>();
    protected int moveStep;
    protected int moveIndex;
    protected int attackStep;
    protected int attackIndex;
    protected int actionStep;
    protected int actionIndex;

    /// コンストラクタ
    public ScriptUnitHandle()
    {
    }

    /// 破棄
    public void Dispose()
    {
        moveList.Clear();
        attackList.Clear();
        actionList.Clear();
    }

    /// スクリプトの開始
    public void StartScript()
    {
        moveIndex = 0;
        moveStep = 0;

        attackIndex = 0;
        attackStep = 0;

        actionIndex = 0;
        actionStep = 0;
    }

    /// スクリプトリストのクリア
    public void ClearScript()
    {
        moveList.Clear();
        attackList.Clear();
        actionList.Clear();
    }

    /// 移動スクリプトのクリア
    public void ClearMoveScript()
    {
        moveList.Clear();
    }

    /// 攻撃スクリプトのクリア
    public void ClearAttackScript()
    {
        attackList.Clear();
    }

    /// アクションスクリプトのクリア
    public void ClearActionScript()
    {
        actionList.Clear();
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager, Mono unit)
    {
        StartScript();
        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager, Mono unit)
    {
        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager, Mono unit)
    {
        /// 移動
        if (moveIndex < moveList.Count) {
            if (moveStep < moveList[moveIndex].Frame) {
                moveList[moveIndex].Action(monoManager, unit);
                moveStep++;

                if (moveStep >= moveList[moveIndex].Frame) {
                    moveIndex++;
                    moveStep = 0;
                }
            }
        }

        // 攻撃
        if (attackIndex < attackList.Count) {
            if (attackStep == attackList[attackIndex].Frame) {
                attackList[attackIndex].Action(monoManager, unit);

                attackIndex++;
                attackStep = 0;
            } else {
                attackStep++;
            }
        }

        // モデルアクション
        if (actionIndex < actionList.Count) {
            if (actionStep == actionList[actionIndex].Frame) {
                actionList[actionIndex].Action(monoManager, unit);

                actionIndex++;
                actionStep = 0;
            } else {
                actionStep++;
            }
        }

        // 画面外に出たら消滅(仮)
        if (moveIndex >= moveList.Count - 1) {
            if (unit.WorldPosition.Z + unit.CollisionRadius < -832/2 || unit.WorldPosition.Z - unit.CollisionRadius > 832/2 ||
                unit.WorldPosition.Y + unit.CollisionRadius < -480/2 || unit.WorldPosition.Y - unit.CollisionRadius > 480/2) {

                unit.Remove(monoManager, false);
                return true;
            }
        }

        // 全てのリストが空になったら削除
        if (moveIndex >= moveList.Count &&
            attackIndex >= attackList.Count &&
            actionIndex >= actionList.Count) {

            callScriptEnd(monoManager, unit);
            return true;
        }

        return true;
    }

    /// スクリプト終了呼び出し
    protected virtual void callScriptEnd(MonoManager monoManager, Mono mono)
    {
        mono.Remove(monoManager, false);
    }

    /// 基本的な移動
    public void MoveBasic(long frame,
                          float transX, float transY, float transZ,
                          float rotX, float rotY, float rotZ)
    {
        moveList.Add(new MonoMoveScript(frame, (monoManager, mono) => {
                        mono.Rotate(rotX * 30 / GameData.TargetFps, rotY * 30 / GameData.TargetFps, rotZ * 30 / GameData.TargetFps);
                        mono.Translate(transX * 30 / GameData.TargetFps, transY * 30 / GameData.TargetFps, transZ * 30 / GameData.TargetFps);
                        mono.AddRotate(0, 0, 0);
                    }));
    }

    /// 基本的な移動
    public void MoveBasic(long frame,
                          float transX, float transY, float transZ,
                          float rotX, float rotY, float rotZ,
                          float addRotX, float addRotY, float addRotZ)
    {
        moveList.Add(new MonoMoveScript(frame, (monoManager, mono) => {
                        mono.Rotate(rotX * 30 / GameData.TargetFps, rotY * 30 / GameData.TargetFps, rotZ * 30 / GameData.TargetFps);
                        mono.Translate(transX * 30 / GameData.TargetFps, transY * 30 / GameData.TargetFps, transZ * 30 / GameData.TargetFps);
                        mono.AddRotate(addRotX * 30 / GameData.TargetFps, addRotY * 30 / GameData.TargetFps, addRotZ * 30 / GameData.TargetFps);
                    }));
    }

    /// 攻撃（通常弾）
    public void AttackEnemyNormal(long frame)
    {
        attackList.Add(new MonoMoveScript(frame, AttackEnemyNormal()));
    }

    /// 攻撃（方向弾）
    public void AttackEnemyDir(long frame)
    {
        attackList.Add(new MonoMoveScript(frame, AttackEnemyDir()));
    }

    /// 攻撃（ミサイル）
    public void AttackEnemyMissile(long frame)
    {
        attackList.Add(new MonoMoveScript(frame, AttackEnemyMissile()));
    }

    /// 攻撃（ボス：弾）
    public void AttackBossNormal(long frame)
    {
        attackList.Add(new MonoMoveScript(frame, AttackBossNormal()));
    }

    /// 攻撃（ボス：レーザー）
    public void AttackBossLaser(long frame)
    {
        attackList.Add(new MonoMoveScript(frame, AttackBossLaser()));
    }

    /// 攻撃（ボス：レーザー当たり）
    public void AttackBossNone(long frame, float radius)
    {
        attackList.Add(new MonoMoveScript(frame, AttackBossNone(radius)));
    }

    /// 行動変化
    public void ChangeAction(long frame, string name)
    {
        actionList.Add(new MonoMoveScript(frame, (monoManager, mono) => {
                    mono.ChangeAction(name);
                }));
    }

    /// 攻撃（通常弾）
    public static MonoScriptDelegate AttackEnemyNormal()
    {
        return (monoManager, unit) => {
            Unit bullet = new Unit(unit.WorldMatrix,
                                   new EnemyNormalBulletHandle(),
                                   new EnemyBulletModel(),
                                   unit.GroupId);

            monoManager.Regist(bullet);
        };
    }

    /// 攻撃（方向弾）
    public static MonoScriptDelegate AttackEnemyDir()
    {
        return (monoManager, unit) => {
            Unit bullet = new Unit(unit.WorldMatrix,
                                   new EnemyDirBulletHandle(),
                                   new EnemyBulletModel(),
                                   unit.GroupId);

            monoManager.Regist(bullet);
        };
    }

    /// 攻撃（ミサイル）
    public static MonoScriptDelegate AttackEnemyMissile()
    {
        return (monoManager, unit) => {
            MissileBullet bullet = new MissileBullet(unit.WorldMatrix,unit.GroupId);

            monoManager.Regist(bullet);
        };
    }

    /// 攻撃（ボス：弾）
    public static MonoScriptDelegate AttackBossNormal()
    {
        return (monoManager, unit) => {
            /// 0度
            BossBullet bullet = new BossBullet(unit.WorldMatrix,unit.GroupId);
            monoManager.Regist(bullet);

            /// 16度
            bullet = new BossBullet(unit.WorldMatrix*Matrix4.RotationX(FMath.Radians( 16)),unit.GroupId,0.1f);
            monoManager.Regist(bullet);

            /// -16度
            bullet = new BossBullet(unit.WorldMatrix*Matrix4.RotationX(FMath.Radians(-16)),unit.GroupId,0.1f);
            monoManager.Regist(bullet);

            /// 32度
            bullet = new BossBullet(unit.WorldMatrix*Matrix4.RotationX(FMath.Radians( 32)),unit.GroupId,0.2f);
            monoManager.Regist(bullet);

            /// -32度
            bullet = new BossBullet(unit.WorldMatrix*Matrix4.RotationX(FMath.Radians(-32)),unit.GroupId,0.2f);
            monoManager.Regist(bullet);
        };
    }

    /// 攻撃（ボス：レーザー）
    public static MonoScriptDelegate AttackBossLaser()
    {
        return (monoManager, unit) => {
            LaserBullet bullet = new LaserBullet(unit.WorldMatrix,
                                                 unit.GroupId,
                                                 "Laser");

            monoManager.Regist(bullet);
        };
    }

    /// 攻撃（ボス：レーザー当たり）
    public static MonoScriptDelegate AttackBossNone(float radius)
    {
        return (monoManager, unit) => {
            Unit bullet = new Unit(unit.WorldMatrix,
                                   new NoneBulletHandle(),
                                   new NoneBulletModel(),
                                   unit.GroupId);

            bullet.CollisionRadius = radius;

            monoManager.Regist(bullet);
        };
    }

}

} // ShootingDemo
