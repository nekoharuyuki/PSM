/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace ShootingDemo
{

/// 繰り返しモード
public enum RepeatMode
{
    Constant,
    Loop
}

/**
 * ModelAnimクラス
 */
public class ModelAnim
{
    private Model model;
    private int animIndex;
    private long startTimeMillis;
    private long playTimeMillis;
    private RepeatMode repeatMode;
    private bool animEnd;
    private Matrix4 localMatrix;
    private long animTimeMillis;
    private bool billboard;

    /// コンストラクタ
    public ModelAnim(Model model,
                     int animIndex,
                     RepeatMode repeatMode,
                     Matrix4 localMatrix,
                     bool billboard,
                     long startTimeMillis = 0,
                     long playTimeMillis = 0)
    {
        this.model = model;
        this.animIndex = animIndex;
        this.repeatMode = repeatMode;

        this.localMatrix = localMatrix;
        this.billboard = billboard;

        this.startTimeMillis = startTimeMillis;
        if (playTimeMillis > 0) {
            this.playTimeMillis = playTimeMillis;
        } else {
            this.playTimeMillis = (long)(model.GetMotionLength(animIndex) * 1000);
        }
    }

    /// 開始処理
    public void Start()
    {
        animTimeMillis = 0;
        animEnd = false;
    }

    /// 更新処理
    public void Update(long addTimeMillis)
    {
        long endTimeMillis = startTimeMillis + playTimeMillis;
        if (endTimeMillis <= 0) {
            animTimeMillis = 0;
            return;
        }

        animTimeMillis += addTimeMillis;

        if (animTimeMillis > endTimeMillis) {
            switch (repeatMode) {
            case RepeatMode.Constant:
                animTimeMillis = endTimeMillis;
                break;
            case RepeatMode.Loop:
                animTimeMillis %= endTimeMillis;
                break;
            }

            animEnd = true;
        }
    }

    /// 描画処理
    public void Render(ref Matrix4 matrix)
    {
        var camera = Renderer.GetGraphicsDevice().GetCurrentCamera();

        Matrix4 worldMatrix;
        if (billboard) {
            worldMatrix = camera.View.Inverse();
            worldMatrix *= Matrix4.RotationY(FMath.Radians(90));

            worldMatrix.M41 = matrix.M41;
            worldMatrix.M42 = matrix.M42;
            worldMatrix.M43 = matrix.M43;
        } else {
            worldMatrix = matrix;
        }

        worldMatrix *= localMatrix;

        /// 全体的に倍
        worldMatrix *= Matrix4.Scale(new Vector3(2.0f, 2.0f, 2.0f));

        model.Render(ref worldMatrix, camera, animIndex, (float)(calcAnimPlayTimeMillis(animTimeMillis)) / 1000);
    }

    /// 再生終了確認
    public bool IsEndAction()
    {
        if (repeatMode == RepeatMode.Constant && animEnd == false) {
            return false;
        }

        return true;
    }

    /// 再生時間の算出
    protected long calcAnimPlayTimeMillis(long animTimeMillis)
    {
        long animPlayTimeMillis = (animTimeMillis - startTimeMillis);

        if (animPlayTimeMillis >= 0) {
            if (playTimeMillis <= 0) {
                animPlayTimeMillis = 0;
            } else if (animPlayTimeMillis >= playTimeMillis) {
                animPlayTimeMillis = playTimeMillis;
            }
        }

        if (animPlayTimeMillis < 0 || animPlayTimeMillis > playTimeMillis) {
            animPlayTimeMillis = -1;
        }

        return animPlayTimeMillis;
    }
}

} // ShootingDemo
