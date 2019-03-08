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
 * UnitPostScheduleクラス
 */
public class UnitPostSchedule : IDisposable
{
    private List<UnitPostInfo> scheduleList;

    private List<ScriptUnitHandle> bitHandleList;

    /// コンストラクタ
    public UnitPostSchedule()
    {
        scheduleList = new List<UnitPostInfo>();

        bitHandleList = new List<ScriptUnitHandle>();

        setupBit();
    }

    /// デストラクタ
    ~UnitPostSchedule()
    {
        Dispose();
    }

    /// 後処理
    public void Dispose()
    {
        if (bitHandleList != null) {
            bitHandleList.Clear();
            bitHandleList = null;
        }

        if (scheduleList != null) {
            scheduleList.Clear();
            scheduleList = null;
        }
    }

    /// スケジュールリスト取得
    public List<UnitPostInfo> GetScheduleList()
    {
        return scheduleList;
    }

    /// 敵Ａ：ストレート
    public void AddEnemyAStraight(long timeMillis,
                                  Vector3 position,
                                  Vector3 rotation,
                                  int score,
                                  int interval = 0,
                                  string unitName = null,
                                  string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic(45, 0, 0, 8, 0, 0, 0, 0, 0, 8);
                    handle.MoveBasic(99, 0, 0, 8, 0, 0, 0, 0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(30+interval);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyAUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ａ：カーブ（上）
    public void AddEnemyACurveU(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                int interval = 0,
                                bool typeSp = false,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 45,  0, 0, 8,  2, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 99,  0, 0, 8,  0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(50+interval);

                    /// 通常タイプ
                    if( !typeSp ) {
                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyAUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                    /// 特殊
                    } else {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyASpUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    }
                }));
    }

    /// 敵Ａ：カーブ（下）
    public void AddEnemyACurveD(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                int interval = 0,
                                bool typeSp = false,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 45,  0, 0, 8, -2, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 99,  0, 0, 8,  0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(46+interval);

                    /// 通常タイプ
                    if( !typeSp ) {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyAUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    /// 特殊
                    } else {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyASpUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    }
                }));
    }

    /// 敵Ａ：ターン（上）
    public void AddEnemyATurnU(long timeMillis,
                               Vector3 position,
                               Vector3 rotation,
                               int score,
                               int interval = 0,
                               bool typeSp = false,
                               string unitName = null,
                               string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 90, 4, 0, 8,  0, 0, 0, 0, 0, 8);
                    handle.MoveBasic(  1, 0, 0, 8, -2, 0, 0);
                    handle.MoveBasic( 22, 0, 0, 8, -8, 0, 0);
                    handle.MoveBasic( 45, 0, 0, 8,  0, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 99, 0, 0, 8,  0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(100+interval);

                    /// 通常タイプ
                    if( !typeSp ) {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyAUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    /// 特殊
                    } else {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyASpUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    }
                }));
    }

    /// 敵Ａ：ターン（下）
    public void AddEnemyATurnD(long timeMillis,
                               Vector3 position,
                               Vector3 rotation,
                               int score,
                               int interval = 0,
                               bool typeSp = false,
                               string unitName = null,
                               string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 90, 4, 0, 8,  0, 0, 0, 0, 0, 8);
                    handle.MoveBasic(  1, 0, 0, 8,  2, 0, 0);
                    handle.MoveBasic( 22, 0, 0, 8,  8, 0, 0);
                    handle.MoveBasic( 45, 0, 0, 8,  0, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 99, 0, 0, 8,  0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(100+interval);

                    /// 通常タイプ
                    if( !typeSp ) {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyAUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    /// 特殊
                    } else {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    handle,
                                                    new EnemyASpUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    unitName), parentName);
                    }
                }));
    }

    /// 敵Ａ：旋回（上）
    public void AddEnemyAArcU(long timeMillis,
                              Vector3 position,
                              Vector3 rotation,
                              int score,
                              string unitName = null,
                              string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 50,  0, 0, 8,  0, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 72,  0, 0, 8,  5, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 99,  0, 0, 8,  0, 0, 0, 0, 0, 8);

                    /// 攻撃
                    handle.AttackEnemyDir(50);
                    handle.AttackEnemyDir(72);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyASpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ａ：旋回（下）
    public void AddEnemyAArcD(long timeMillis,
                              Vector3 position,
                              Vector3 rotation,
                              int score,
                              string unitName = null,
                              string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 50,  0, 0, 8,  0, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 72,  0, 0, 8, -5, 0, 0, 0, 0, 8);
                    handle.MoveBasic( 99,  0, 0, 8,  0, 0, 0, 0, 0, 8);

                    /// 攻撃
                    handle.AttackEnemyDir(50);
                    handle.AttackEnemyDir(72);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyASpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ａ：ジグザグ（上）
    public void AddEnemyAZigzagU(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 int interval = 0,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9,  3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9, -3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9,  3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9, -3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(45+interval);
                    handle.AttackEnemyDir(60+interval);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyASpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ａ：ジグザグ（下）
    public void AddEnemyAZigzagD(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 int interval = 0,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9, -3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9,  3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9, -3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9, -3, 0, 0);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);
                    handle.MoveBasic( 30,  0, 0, 9,  3, 0, 0, 0, 0, 12);
                    handle.MoveBasic( 15,  0, 0, 9,  3, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyDir(45+interval);
                    handle.AttackEnemyDir(60+interval);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyASpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：ストレート
    public void AddEnemyBStraight(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                bool wait = false,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic( 27,  0,  0,  6,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f/2f, 0,   0, 0);
                    if ( wait ) {
                        handle.MoveBasic( 30,  0,  0,   0,       0,   0, 0);
                    }
                    handle.MoveBasic(  1,  0,  0,   0.5f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f/2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f/2f, 0,   0, 0);
                    handle.MoveBasic( 35,  0,  0,  6,    0,   0, 0);

                    /// アクション
                    handle.ChangeAction(22,"DefToStand");
                    if ( wait ) {
                        handle.ChangeAction(30,"Stand");
                    }
                    handle.ChangeAction(30,"StandToDef");
                    handle.ChangeAction(20,"Default");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                0,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：Ｌ字（大回り・上）
    public void AddEnemyBRectLU(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 45,  0,  0,  16,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0,    0, -15, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic( 30,  0,  5,   0,    0,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic( 99,  0,  5,   0,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0,    0,   0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(72);
                    handle.AttackEnemyMissile(60);

                    /// アクション
                    handle.ChangeAction(30,"DefToStand");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(30,"Attack");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：Ｌ字（大回り・下）
    public void AddEnemyBRectLD(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 45,  0,  0,  16,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0,    0, -15, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic( 30,  0, -5,   0,    0,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic( 99,  0, -5,   0,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0,    0,   0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(72);
                    handle.AttackEnemyMissile(60);

                    /// アクション
                    handle.ChangeAction(30,"DefToStand");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(30,"Attack");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：Ｌ字（中回り・上）
    public void AddEnemyBRectMU(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 15,  0,  0,  16,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic( 15,  0,  0,  16,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic( 15,  0,  0,   0,    0, -12, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic( 99,  0, -5,   0,    0,   0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(30);
                    handle.AttackEnemyMissile(90);

                    /// アクション
                    handle.ChangeAction(0,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"StandToDef");
                    handle.ChangeAction(20,"Default");
                    handle.ChangeAction(10,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：Ｌ字（中回り・下）
    public void AddEnemyBRectMD(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 15,  0,  0,  16,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic( 15,  0,  0,  16,    0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f, 0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f, 0,   0, 0);
                    handle.MoveBasic( 15,  0,  0,   0,    0, -12, 0);
                    handle.MoveBasic( 30,  0,  0,   0,    0,   0, 0);
                    handle.MoveBasic( 99,  0,  5,   0,    0,   0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(30);
                    handle.AttackEnemyMissile(90);

                    /// アクション
                    handle.ChangeAction(0,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"StandToDef");
                    handle.ChangeAction(20,"Default");
                    handle.ChangeAction(10,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：垂直（上）
    public void AddEnemyBVerticalU(long timeMillis,
                                   Vector3 position,
                                   Vector3 rotation,
                                   int score,
                                   string unitName = null,
                                   string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 30,  0,  4, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0,  0, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0,  4, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0,  4, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0,  0, 0, 0, 0, 0);
                    handle.MoveBasic( 99,  0,  4, 0, 0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(30);
                    handle.AttackEnemyMissile(90);

                    /// アクション
                    handle.ChangeAction(0,"Stand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(60,"Attack");
                    handle.ChangeAction(30,"Stand");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：垂直（下）
    public void AddEnemyBVerticalD(long timeMillis,
                                   Vector3 position,
                                   Vector3 rotation,
                                   int score,
                                   string unitName = null,
                                   string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 30,  0, -4, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0,  0, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0, -4, 0, 0, 0, 0);
                    handle.MoveBasic( 30,  0,  0, 0, 0, 0, 0);
                    handle.MoveBasic( 99,  0, -4, 0, 0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(30);
                    handle.AttackEnemyMissile(60);

                    /// アクション
                    handle.ChangeAction(0,"Stand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：急旋回（大回り・上）
    public void AddEnemyBQuickLU(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic(170,  0, 0,   5,   0, 0, 0);
                    handle.MoveBasic( 36,  0, 0,   0,   0, 5, 0);
                    handle.MoveBasic( 30,  0, 0,   0,   0, 0, 0);
                    handle.MoveBasic(  2,  0, 0, -10,   0, 0, 0);
                    handle.MoveBasic( 20,  0, 0,   0,   0, 0, 0);
                    handle.MoveBasic(  1,  0, 0,   0, -45, 0, 0);
                    handle.MoveBasic(299,  0, 0,   5,   0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(206);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：急旋回（大回り・下）
    public void AddEnemyBQuickLD(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic(170,  0, 0,   5,   0, 0, 0);
                    handle.MoveBasic( 36,  0, 0,   0,   0, 5, 0);
                    handle.MoveBasic( 30,  0, 0,   0,   0, 0, 0);
                    handle.MoveBasic(  2,  0, 0, -10,   0, 0, 0);
                    handle.MoveBasic( 20,  0, 0,   0,   0, 0, 0);
                    handle.MoveBasic(  1,  0, 0,   0,  45, 0, 0);
                    handle.MoveBasic(299,  0, 0,   5,   0, 0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(206);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：急旋回（中回り・上）
    public void AddEnemyBQuickMU(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 20,  0, 0,  16,    0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,  12.8f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,  10.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   8.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   6.5f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   5.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   4.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   3.3f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   2.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   2.1f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.4f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.1f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.9f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.5f, 0,  0, 0);
                    handle.MoveBasic( 30,  0, 0,   0,    0,  0, 0);
                    handle.MoveBasic( 12,  0, 0,   0,    0, 15, 0);
                    handle.MoveBasic(  6,  0, 0,   0,    5,  0, 0);
                    handle.MoveBasic( 12,  0, 0,   0,    0,  0, 0);
                    handle.MoveBasic(  1,  0,  0.5f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  0.7f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  0.9f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  1.1f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  1.4f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  1.7f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  2.1f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  2.7f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  3.3f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  4.2f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  5.2f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  6.5f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0,  8.2f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 10.2f/2f, 0, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 12.8f/2f, 0, 0,  0, 0);
                    handle.MoveBasic( 39,  0, 8,   0,    0,  0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(35);

                    /// アクション
                    handle.ChangeAction(5,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(30,"StandToDef");
                    handle.ChangeAction(20,"Default");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：急旋回（中回り・下）
    public void AddEnemyBQuickMD(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic( 20,  0, 0,  16,    0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,  12.8f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,  10.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   8.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   6.5f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   5.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   4.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   3.3f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   2.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   2.1f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.4f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.1f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.9f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.5f, 0,  0, 0);
                    handle.MoveBasic( 30,  0, 0,   0,    0,  0, 0);
                    handle.MoveBasic( 12,  0, 0,   0,    0, 15, 0);
                    handle.MoveBasic(  6,  0, 0,   0,    2,  0, 0);
                    handle.MoveBasic( 12,  0, 0,   0,    2,  0, 0);
                    handle.MoveBasic(  6,  0, 0,   0,    2,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.5f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   0.9f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.1f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.4f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   1.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   2.1f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   2.7f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   3.3f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   4.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   5.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   6.5f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,   8.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,  10.2f, 0,  0, 0);
                    handle.MoveBasic(  1,  0, 0,  12.8f, 0,  0, 0);
                    handle.MoveBasic( 19,  0, 0,  16,    0,  0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(35);

                    /// アクション
                    handle.ChangeAction(5,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(30,"StandToDef");
                    handle.ChangeAction(20,"Default");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：連射（上）
    public void AddEnemyBSerialU(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic(  8,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic(  7,  0,  0,  16,     0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f,  0,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0,  2.5f,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0,  2.5f,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0,     0, -15, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f,  0,   0, 0);
                    handle.MoveBasic( 10,  0, 0,   16,     0,   0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(30);
                    handle.AttackEnemyMissile(42);
                    handle.AttackEnemyMissile(42);

                    /// アクション
                    handle.ChangeAction(0,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"StandToDef");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｂ：連射（下）
    public void AddEnemyBSerialD(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyBUnitHandle();

                    /// 移動
                    handle.MoveBasic(  8,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic(  7,  0,  0,  16,     0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f,  0,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0, -2.5f,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0, -2.5f,   0, 0);
                    handle.MoveBasic( 30,  0,  0,   0,     0,   0, 0);
                    handle.MoveBasic( 12,  0,  0,   0,     0, -15, 0);
                    handle.MoveBasic(  1,  0,  0,   0.5f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   0.9f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.4f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   1.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.1f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   2.7f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   3.3f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   4.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   5.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   6.5f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,   8.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  10.2f,  0,   0, 0);
                    handle.MoveBasic(  1,  0,  0,  12.8f,  0,   0, 0);
                    handle.MoveBasic( 10,  0,  0,  16,     0,   0, 0);

                    /// 攻撃
                    handle.AttackEnemyMissile(30);
                    handle.AttackEnemyMissile(42);
                    handle.AttackEnemyMissile(42);

                    /// アクション
                    handle.ChangeAction(0,"DefToStand");
                    handle.ChangeAction(30,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"Attack");
                    handle.ChangeAction(30,"Stand");
                    handle.ChangeAction(12,"StandToDef");

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyBSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｃ：直進
    public void AddEnemyCNormal(long timeMillis,
                                Vector3 position,
                                Vector3 rotation,
                                int score,
                                string unitName = null,
                                string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new EnemyCNormalUnitHandle();

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyCUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｃ：分裂
    public void AddEnemyCSplit(long timeMillis,
                               Vector3 position,
                               Vector3 rotation,
                               int score,
                               float iniRot,
                               string unitName = null,
                               string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic(  1,  0, 0,  8,  0,  0, iniRot);
                    handle.MoveBasic( 40,  0, 0,  8,  0,  0, 0, 0, 0, 9);
                    handle.MoveBasic( 18,  0, 0,  8,  0, -5, 0);
                    handle.MoveBasic(199,  0, 0, 10,  0,  0, 0);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyCSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｃ：ジグザグ（上）
    public void AddEnemyCZigzagU(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic(  5,  0, 0,  0, -15, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 15,  0, 0, 10,   0, 0, 0, 0, 0, 9);
                    handle.MoveBasic(  5,  0, 0,  0,  25, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 45,  0, 0, 10,   0, 0, 0, 0, 0, 9);
                    handle.MoveBasic(  5,  0, 0,  0, -20, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 40,  0, 0, 10,   0, 0, 0, 0, 0, 9);
                    handle.MoveBasic(  5,  0, 0,  0,  15, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 99,  0, 0, 10,   0, 0, 0, 0, 0, 9);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyCSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// 敵Ｃ：ジグザグ（下）
    public void AddEnemyCZigzagD(long timeMillis,
                                 Vector3 position,
                                 Vector3 rotation,
                                 int score,
                                 string unitName = null,
                                 string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    var handle = new ScriptUnitHandle();

                    /// 移動
                    handle.MoveBasic(  5,  0, 0,  0,  10, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 20,  0, 0, 10,   0, 0, 0, 0, 0, 9);
                    handle.MoveBasic(  5,  0, 0,  0, -20, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 40,  0, 0, 10,   0, 0, 0, 0, 0, 9);
                    handle.MoveBasic(  5,  0, 0,  0,  20, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 40,  0, 0, 10,   0, 0, 0, 0, 0, 9);
                    handle.MoveBasic(  5,  0, 0,  0, -15, 0, 0, 0, 0, 9);
                    handle.MoveBasic( 99,  0, 0, 10,   0, 0, 0, 0, 0, 9);

                    monoManager.Regist(new Unit(position,
                                                rotation,
                                                handle,
                                                new EnemyCSpUnitModel(),
                                                score,
                                                GroupId.Enemy,
                                                unitName), parentName);
                }));
    }

    /// ボス：本体
    public void AddEnemyBoss(long timeMillis,
                             Vector3 position,
                             Vector3 rotation,
                             int score,
                             string unitName = null,
                             string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    EnemyBoss unit = new EnemyBoss(position,
                                                   rotation,
                                                   score,
                                                   GroupId.Enemy,
                                                   "Boss");
                    monoManager.Regist(unit, parentName);
                }));
    }

    /// ビットの行動設定
    private void setupBit()
    {
        int[] ary = new int[] { 0, 45, 90, 135, 180, 225, 270, 315 };

        foreach( int i in ary ) {
            var handle = new ScriptUnitHandle();

            double nowZ = 0;
            double nowY = 0;
            double nowX = 0;
            double preZ = 0;
            double preY = 0;
            double preX = 0;
            double rr = 0;
            double speed = 1;
            int roll = 80;

            /// 移動
            for (double r=0; r<1440+50; r+=speed) {
                /// 回転タイミング
                if ( ( ( r >=  150 ) && ( r <  150+roll ) ) || 
                     ( ( r >=  450 ) && ( r <  450+roll ) ) || 
                     ( ( r >=  750 ) && ( r <  750+roll ) ) || 
                     ( ( r >= 1050 ) && ( r < 1050+roll ) ) ) {
                    rr += Math.PI/(roll>>1);
                }
                nowX = Math.Sin(((r+i)%360)*Math.PI/180)*(Math.Sin(rr)*190);
                nowZ = Math.Cos(((r+i)%360)*Math.PI/180)*190;
                nowY = Math.Sin(((r+i)%360)*Math.PI/180)*(Math.Cos(rr)*190);
                handle.MoveBasic( 1,  (float)(preX-nowX), (float)(preY-nowY), (float)(preZ-nowZ), 0, 0, 0, 0, 0, 25f );
                preZ = nowZ;
                preY = nowY;
                preX = nowX;
            }

            /// 正面時攻撃
            if (i%90==0) {
                handle.AttackEnemyDir((360-(int)i)%360-4);
                handle.AttackEnemyDir(360);
                handle.AttackEnemyDir(360);
                handle.AttackEnemyDir(360);
            /// 対角時攻撃
            } else {
                handle.AttackEnemyDir(((315-(int)i)%360)+8);
                handle.AttackEnemyDir( 90);
                handle.AttackEnemyDir(270);
                handle.AttackEnemyDir( 90);
                handle.AttackEnemyDir(270);
                handle.AttackEnemyDir( 90);
                handle.AttackEnemyDir(270);
                handle.AttackEnemyDir( 90);
            }

            bitHandleList.Add(handle);
        }
    }

    /// ビット
    public void AddEnemyBit(long timeMillis,
                            Vector3 position,
                            Vector3 rotation,
                            int score,
                            string unitName = null,
                            string parentName = null)
    {
        scheduleList.Add(new UnitPostInfo(timeMillis, (monoManager) => {
                    for (int i = 0; i < bitHandleList.Count; i++) {
                        monoManager.Regist(new Unit(position,
                                                    rotation,
                                                    bitHandleList[i],
                                                    new EnemyBitUnitModel(),
                                                    score,
                                                    GroupId.Enemy,
                                                    "Bit"+(i*45)), "Boss");
                    }
                }));
    }

}

} // ShootingDemo
