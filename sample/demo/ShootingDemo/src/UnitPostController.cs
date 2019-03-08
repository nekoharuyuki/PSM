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
 * UnitPostControllerクラス
 */
public class UnitPostController : IDisposable
{
    private List<UnitPostInfo> unitScheduleList;
    private int index;
    private int step;

    /// コンストラクタ
    public UnitPostController(List<UnitPostInfo> unitScheduleList)
    {
        this.unitScheduleList = unitScheduleList;

        index = 0;
        step = 0;
    }

    /// デストラクタ
    ~UnitPostController()
    {
    }

    /// 破棄
    public void Dispose()
    {
        unitScheduleList = null;
    }

    /// 開始
    public void Start()
    {
        index = 0;
        step = 0;
    }

    /// 終了確認
    public bool IsPlayEnd()
    {
        return (index >= unitScheduleList.Count) ? true : false;
    }

    /// 更新
    public void Update(MonoManager monoManager)
    {
        while (true) {
            if (index >= unitScheduleList.Count) {
                break;
            }

            if (step == unitScheduleList[index].TimeMillis) {
                unitScheduleList[index].Action(monoManager);
                index++;
                step = 0;
            } else {
                step++;
                break;
            }
        }
    }
}

} // ShootingDemo
