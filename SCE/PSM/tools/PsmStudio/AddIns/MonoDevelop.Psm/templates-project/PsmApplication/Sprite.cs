/* SCE CONFIDENTIAL
 * PlayStation(R) Mobile Software Development Kit 0.91.0
 * Copyright (C) 2011 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment; // FIXME : just for FMath

namespace ${Namespace}
{

/**
 * Spriteクラス
 */
public class Sprite : IDisposable
{
    private VertexBuffer vertices;
    private float positionX;
    private float positionY;
    private float centerX;
    private float centerY;
    private float degree;
    private float scaleX;
    private float scaleY;
    private Texture2D texture;

    /// コンストラクタ(Texture2D)
    public Sprite(Texture2D texture, float positionX, float positionY)
    {
        SetTexture(texture);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = 0.0f;

        ScaleX = 1.0f;
        ScaleY = 1.0f;
    }

    /// コンストラクタ(Texture2D)
    public Sprite(Texture2D texture, float positionX, float positionY, float degree, float scale)
    {
        SetTexture(texture);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = degree;

        ScaleX = scale;
        ScaleY = scale;
    }

    /// コンストラクタ(Texture2D)
    public Sprite(Texture2D texture,
                  float positionX, float positionY,
                  float centerX, float centerY, float degree,
                  float scaleX, float scaleY)
    {
        SetTexture(texture);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = centerX;
        CenterY = centerY;
        Degree = degree;

        ScaleX = scaleX;
        ScaleY = scaleY;
    }

    /// コンストラクタ(文字)
    public Sprite(string text, uint argb, Font font, int positionX, int positionY)
    {
        int width = font.GetTextWidth(text, 0, text.Length);
        int height = font.Metrics.Height;

        var image = new Image(new ImageSize(width, height),
                              new ImageColor(0, 0, 0, 0),
                              ImageMode.Rgba);

        image.DrawText(text,
                       new ImageColor((int)((argb >> 16) & 0xff),
                                      (int)((argb >> 8) & 0xff),
                                      (int)((argb >> 0) & 0xff),
                                      (int)((argb >> 24) & 0xff)),
                       font, new ImagePosition(0, 0));

        var texture = new Texture2D(width, height, false, PixelFormat.Rgba);
        texture.SetPixels(0, image.ToBuffer());
        image.Dispose();

        SetTexture(texture);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = 0.0f;

        ScaleX = 1.0f;
        ScaleY = 1.0f;
    }

    /// 破棄
    public void Dispose()
    {
        vertices.Dispose();
        texture.Dispose();
    }

    /// 座標 X
    public float PositionX
    {
        get {return positionX;}
        set {positionX = value;}
    }

    /// 座標 Y
    public float PositionY
    {
        get {return positionY;}
        set {positionY = value;}
    }

    /// 回転中心座標 X
    public float CenterX
    {
        get {return centerX;}
        set {centerX = value;}
    }

    /// 回転中心座標 Y
    public float CenterY
    {
        get {return centerY;}
        set {centerY = value;}
    }

    /// 角度(オイラー)
    public float Degree
    {
        get {return degree;}
        set {degree = value;}
    }

    /// 拡大率 X
    public float ScaleX
    {
        get {return scaleX;}
        set {scaleX = value;}
    }

    /// 拡大率 Y
    public float ScaleY
    {
        get {return scaleY;}
        set {scaleY = value;}
    }

    /// 頂点バッファ
    public VertexBuffer Vertices
    {
        get {return vertices;}
    }

    /// テクスチャ
    public Texture2D Texture
    {
        get {return texture;}
    }

    /// テクスチャのセット
    public void SetTexture(Texture2D texture)
    {
        this.texture = new Texture2D(texture);

        float l = 0;
        float t = 0;
        float r = texture.Width;
        float b = texture.Height;

        vertices = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float2);
        vertices.SetVertices(0, new float[]{l, t, 0,
                                            r, t, 0,
                                            r, b, 0,
                                            l, b, 0});
        vertices.SetVertices(1, new float[]{0.0f, 0.0f,
                                            1.0f, 0.0f,
                                            1.0f, 1.0f,
                                            0.0f, 1.0f});
    }

    /// モデル行列の生成
    public Matrix4 CreateModelMatrix()
    {
        var centerMatrix = Matrix4.Translation(new Vector3(-centerX, -centerY, 0.0f));
        var transMatrix = Matrix4.Translation(new Vector3(positionX + centerX, positionY + centerY, 0.0f));
        var rotMatrix = Matrix4.RotationZ((float)(degree / 180.0f * FMath.PI));
        var scaleMatrix = Matrix4.Scale(new Vector3(scaleX, scaleY, 1.0f));

        return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
    }
}

} // Sample
