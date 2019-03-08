/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

namespace ShootingDemo
{

/**
 * MonoManagerクラス
 */
public class MonoManager : IDisposable
{
    private List<Mono> updateList = new List<Mono>();
    private List<Mono> renderList = new List<Mono>();
    private int updateIndex = 0;

    /// コンストラクタ
    public MonoManager()
    {
    }

    /// 破棄
    public void Dispose()
    {
        updateList.ForEach(mono => {
            mono.End(this);
            mono.Dispose();
        });

        updateList.Clear();
        renderList.Clear();

        updateIndex = 0;
    }

    /// 登録
    public bool Regist(Mono mono, string parentName = null)
    {
        return Regist(mono, FindMono(parentName));
    }

    /// 登録
    public bool Regist(Mono mono, Mono parent)
    {
        //同一名が既にいる場合は登録不可
        if (FindMono(mono.Name) != null) {
            return false;
        }

        // 挿入位置の決定
        if (parent != null) {
            // 親の後ろに挿入
            updateList.Insert(updateList.IndexOf(parent) + 1, mono);
            renderList.Insert(renderList.IndexOf(parent) + 1, mono);
        } else {
            updateList.Add(mono);
            renderList.Add(mono);
        }

        // 現在より前に挿入する場合は1つ進める
        if (updateList.IndexOf(mono) <= updateIndex) {
            updateIndex++;
        }

        // 親をセット
        mono.SetParent(parent);

        // 開始
        mono.Start(this);

        mono.MonoLifeState = MonoLifeState.Normal;

        return true;
    }

    // リストから削除
    public void Remove(Mono mono)
    {
        if (updateList.IndexOf(mono) > 0) {
            // 現在より前のタスクを削除する場合は1つ戻す
            if (updateList.IndexOf(mono) <= updateIndex) {
                updateIndex--;
            }

            updateList.Remove(mono);
            renderList.Remove(mono);
            mono.End(this);
            mono.Dispose();
        }
    }

    /// 更新
    public bool Update()
    {
        for (updateIndex = 0; updateIndex < updateList.Count; updateIndex++) {
            updateList[updateIndex].Update(this);
        }

        return true;
    }

    /// 描画
    public bool Render()
    {
        renderList.Sort((x, y) => {
                if (x.ZParam > y.ZParam) {
                    return -1;
                } else if (x.ZParam < y.ZParam) {
                    return 1;
                } else {
                    return 0;
                }
            });

        renderList.ForEach(mono => mono.Render(this));

        return true;
    }

    /// 検索
    public Mono FindMono(string name)
    {
        return updateList.Find(mono => (name != null && mono.Name == name));
    }

    /// 登録確認
    public bool IsRegist(Mono mono)
    {
        return updateList.Contains(mono);
    }

    /// 更新用のリスト
    public List<Mono> UpdateList
    {
        get {return updateList;}
    }
}

} // ShootingDemo
