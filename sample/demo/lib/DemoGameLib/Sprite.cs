/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace DemoGame
{

/**
 * Spriteクラス
 */
public class Sprite : IDisposable
{
    private VertexBuffer vertices;
    private float positionX;
    private float positionY;
    private float drawRectW;
    private float drawRectH;
    private float centerX;
    private float centerY;
    private float degree;
    private float scaleX;
    private float scaleY;
    private Texture2D texture;
    private bool visible;
    private float alpha;

    /// コンストラクタ(Texture2D)
    public Sprite(Texture2D texture, float positionX, float positionY, float degree = 0.0f, float scale = 1.0f)
    {
        SetTexture(texture);
        SetDrawRect(0, 0, texture.Width, texture.Height);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = degree;

        ScaleX = scale;
        ScaleY = scale;
		Visible = true;
		Alpha	= 1.0f;
    }

    /// コンストラクタ(Texture2D)
    public Sprite(Texture2D texture,
                  float positionX, float positionY,
                  float centerX, float centerY, float degree,
                  float scaleX, float scaleY)
    {
        SetTexture(texture);
        SetDrawRect(0, 0, texture.Width, texture.Height);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = centerX;
        CenterY = centerY;
        Degree = degree;

        ScaleX = scaleX;
        ScaleY = scaleY;
		Visible = true;
		Alpha	= 1.0f;
    }

    /// コンストラクタ(Texture2D)
    public Sprite(Texture2D texture, float drawX, float drawY, float drawW, float drawH, float positionX, float positionY)
    {
        SetTexture(texture);
        SetDrawRect(drawX, drawY, drawW, drawH);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = 0.0f;

        ScaleX = 1.0f;
        ScaleY = 1.0f;
		Visible = true;
		Alpha	= 1.0f;
    }

    /// コンストラクタ(文字)
    public Sprite(string text, uint argb, Font font, int positionX, int positionY)
    {
        int width = font.GetTextWidth(text, 0, text.Length);
        int height = font.Metrics.Height;

        var image = new Image(ImageMode.Rgba,
			                  new ImageSize(width, height),
                              new ImageColor(0, 0, 0, 0));

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
        SetDrawRect(0, 0, texture.Width, texture.Height);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = 0.0f;

        ScaleX = 1.0f;
        ScaleY = 1.0f;
		Visible = true;
		Alpha	= 1.0f;
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

    /// 描画矩形 W
    public float DrawRectWidth
    {
        get {return drawRectW;}
	}

    /// 描画矩形 H
    public float DrawRectHeight
    {
        get {return drawRectH;}
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

    /// 表示非表示
    public bool Visible
    {
        get {return visible;}
        set {visible = value;}
    }

    /// α値
    public float Alpha
    {
        get {return alpha;}
        set {alpha = value;}
    }

    /// テクスチャのセット
    public void SetTexture(Texture2D texture)
    {
//      this.texture = new Texture2D(texture);
        this.texture = texture.ShallowClone() as Texture2D;
    }

    /// 切り出し情報のセット
    public void SetDrawRect(float x, float y, float w, float h)
    {
        vertices = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float2);
        vertices.SetVertices(0, new float[]{0, 0, 0,
                                            w, 0, 0,
                                            w, h, 0,
                                            0, h, 0});

        float l = x / texture.Width;
        float t = y / texture.Height;
        float r = (x + w) / texture.Width;
        float b = (y + h) / texture.Height;

        vertices.SetVertices(1, new float[]{l, t,
                                            r, t,
                                            r, b,
                                            l, b});

		drawRectW = w;
		drawRectH = h;
    }

    /// モデル行列の生成
    public Matrix4 CreateModelMatrix(int offsetX = 0, int offsetY = 0)
    {
        var centerMatrix = Matrix4.Translation(-centerX, -centerY, 0.0f);
        var transMatrix = Matrix4.Translation(positionX + centerX + offsetX, positionY + centerY + offsetY, 0.0f);
        var rotMatrix = Matrix4.RotationZ((float)(degree / 180.0f * Math.PI));
        var scaleMatrix = Matrix4.Scale(new Vector3(scaleX, scaleY, 1.0f));

        return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
    }
}

} // Sample
