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
 * MonoModelクラス
 */
public class MonoModel
{
    public float CollisionRadius {get{return modelInfo.CollisionRadius;}}
    public float CollisionRadius2 {get{return modelInfo.CollisionRadius2;}}
    public CollisionLevel CollisionLevel {get{return modelInfo.CollisionLevel;}}
    public int Hitpoint {get{return modelInfo.Hitpoint;}}
    public int Attack {get{return modelInfo.Attack;}}
    public int Defense {get{return modelInfo.Defense;}}

    protected ModelAction action = null;
    protected MonoModelInfo modelInfo = null;

    /// コンストラクタ
    public MonoModel()
    {
    }

    /// アニメーションリストの追加
    public void AddAnimList(string key, List<ModelAnim> animList)
    {
        if (action != null) {
            action.Add(key, animList);
        }
    }

    /// アクションのセット
    public virtual void SetAction(string key)
    {
        if (action != null) {
            action.SetCurrent(key);
        }
    }

    /// アクションの切り替え
    public virtual void ChangeAction(string key)
    {
        if (action != null) {
            action.ChangeCurrent(key);
        }
    }

    /// アクションの終了確認
    public virtual bool IsEndAction()
    {
        if (action != null) {
            return action.IsEndAction();
        }

        return true;
    }

    /// カレントのアクションの名前の取得
    public virtual string CurrentActionName()
    {
        if (action != null) {
            return action.CurrentKey;
        }

        return null;
    }

    /// 更新処理
    public virtual bool Update(Mono mono)
    {
        if (action != null) {
            action.Update(GameData.FrameTimeMillis);
        }

        return true;
    }

    /// 描画処理
    public virtual bool Render(Mono mono)
    {
        if (action != null) {
            Matrix4 matrix = mono.DrawMatrix;
            action.Render(ref matrix);
        }

        return true;
    }

    /// 描画処理
    public virtual bool Render(ref Matrix4 matrix)
    {
        if (action != null) {
            action.Render(ref matrix);
        }

        return true;
    }

    // アニメーション生成
    public static ModelAnim createAnim(string modelName,
                                       int animIndex,
                                       RepeatMode repeatMode,
                                       Matrix4 localMatrix,
                                       bool billboard,
                                       long startTimeMillis = 0,
                                       long playTimeMillis = 0)
    {
        return new ModelAnim(ModelManager.Find(modelName), animIndex, repeatMode, localMatrix, billboard, startTimeMillis, playTimeMillis);
    }

    // アニメーション生成
    public static ModelAnim createAnim(string modelName,
                                       int animIndex,
                                       RepeatMode repeatMode,
                                       Vector3 position,
                                       bool billboard,
                                       long startTimeMillis = 0,
                                       long playTimeMillis = 0)
    {
        Matrix4 matrix = Matrix4.Identity;
        matrix *= Matrix4.Translation(position);

        return new ModelAnim(ModelManager.Find(modelName), animIndex, repeatMode, matrix, billboard, startTimeMillis, playTimeMillis);
    }

    // オブジェクトのアニメーション生成
    public static ModelAnim createAnim1(string modelName,
                                        int animIndex = 0,
                                        RepeatMode repeatMode = RepeatMode.Loop,
                                        long startTimeMillis = 0,
                                        long playTimeMillis = 0)
    {
        Matrix4 matrix = Matrix4.Identity;
        return new ModelAnim(ModelManager.Find(modelName), animIndex, repeatMode, matrix, false, startTimeMillis, playTimeMillis);
    }

    // ビルボード型のアニメーション生成
    public static ModelAnim createAnim3(string modelName,
                                        int animIndex = 0,
                                        RepeatMode repeatMode = RepeatMode.Loop,
                                        long startTimeMillis = 0,
                                        long playTimeMillis = 0)
    {
        Matrix4 matrix = Matrix4.Identity;
        return new ModelAnim(ModelManager.Find(modelName), animIndex, repeatMode, matrix, true, startTimeMillis, playTimeMillis);
    }

}

} // ShootingDemo
