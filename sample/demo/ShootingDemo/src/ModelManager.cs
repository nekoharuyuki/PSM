/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;
using DemoModel;

namespace ShootingDemo
{

/**
 * ModelManagerクラス
 */
public static class ModelManager
{
    private static Dictionary<string, Model> modelTable = new Dictionary<string, Model>();

    /// クリア
    public static void Clear()
    {
        foreach(string key in modelTable.Keys) {
            modelTable[key].Dispose();
        }

        modelTable.Clear();
    }

    /// モデルの追加
    public static void Add(string key, Model model)
    {
        if (Find(key) == null) {
            modelTable[key] = model;
            modelTable[key].BindTexture();
        }
    }

    /// モデルの検索
    public static Model Find(string key)
    {
        if (modelTable.ContainsKey(key)) {
            return modelTable[key];
        }
        return null;
    }
}

} // ShootingDemo
