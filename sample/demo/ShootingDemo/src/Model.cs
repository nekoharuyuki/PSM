/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;
using DemoModel;

namespace ShootingDemo
{

/**
 * Modelクラス
 */
public class Model : IDisposable
{
    protected TexContainer textureContainer;
    protected BasicModel model = null;

    /// コンストラクタ
    public Model(string modelFilePath)
    {
        if (modelFilePath != null) {
            model = new BasicModel("/Application/res/data" + modelFilePath, 0);
            textureContainer = new TexContainer();
        }
    }

    /// 破棄
    public void Dispose()
    {
        textureContainer = null;
        model = null;
    }

    /// テクスチャの読み込み
    public void LoadTexture(String key, String filename)
    {
        textureContainer.Load(key, filename);
    }

    /// モデルにテクスチャを適応
    public void BindTexture()
    {
        if (model != null) {
            model.BindTextures(textureContainer);
        }
    }

    /// モデルの再生時間の取得
    public float GetMotionLength(int index)
    {
        return model.GetMotionLength(index);
    }

    /// 描画
    public bool Render(ref Matrix4 matrix, Camera camera, int animIndex, float animTime)
    {
        if (model != null) {
            GraphicsContext graphics = Renderer.GetGraphicsContext();

            model.SetAnimTime(animIndex, animTime);

            model.WorldMatrix = matrix;
            model.Update();

            model.Draw(graphics,
                       ShootingData.ShaderContainer,
                       camera.Projection * camera.View,
                       camera.View.Inverse() * new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        }

        return true;
    }
}

} // ShootingDemo
