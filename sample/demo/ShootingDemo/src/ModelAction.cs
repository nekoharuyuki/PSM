/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace ShootingDemo
{

/**
 * ModelActionクラス
 */
public class ModelAction : IDisposable
{
    private string currentKey = null;
    private Dictionary<string, List<ModelAnim>> animTable = new Dictionary<string, List<ModelAnim>>();

    /// 破棄
    public void Dispose()
    {
    }

    /// アニメーションリストの追加
    public void Add(string key, List<ModelAnim> anim)
    {
        if (Find(key) == null) {
            animTable[key] = anim;

            if (currentKey == null) {
                currentKey = key;
            }
        }
    }

    /// アニメーションリストの検索
    public List<ModelAnim> Find(string key)
    {
        if (animTable.ContainsKey(key)) {
            return animTable[key];
        }

        return null;
    }

    /// アニメーションの切り替え
    public void ChangeCurrent(string key)
    {
        if (currentKey != key) {
            SetCurrent(key);
        }
    }

    /// アニメーションのセット
    public void SetCurrent(string key)
    {
        currentKey = key;
        Start();
    }

    /// カレントのアニメーションのキーの取得
    public string CurrentKey
    {
        get {return currentKey;}
    }

    /// 開始処理
    public void Start()
    {
        if (currentKey != null && animTable.ContainsKey(currentKey)) {
            foreach (var anim in animTable[currentKey]) {
                anim.Start();
            }
        }
    }

    /// 更新処理
    public void Update(long addTimeMillis)
    {
        if (currentKey != null && animTable.ContainsKey(currentKey)) {
            foreach (var anim in animTable[currentKey]) {
                anim.Update(addTimeMillis);
            }
        }
    }

    /// 描画処理
    public void Render(ref Matrix4 matrix)
    {
        if (currentKey != null && animTable.ContainsKey(currentKey)) {
            foreach (var anim in animTable[currentKey]) {
                anim.Render(ref matrix);
            }
        }
    }

    /// 再生終了確認
    public bool IsEndAction()
    {
        if (currentKey != null && animTable.ContainsKey(currentKey)) {
            foreach (var anim in animTable[currentKey]) {
                if (anim.IsEndAction() == false) {
                    return false;
                }
            }
        }

        return true;
    }
}

} // ShootingDemo
