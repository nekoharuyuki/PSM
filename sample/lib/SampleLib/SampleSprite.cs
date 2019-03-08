/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace Sample
{

/**
 * SampleSprite class
 */
public class SampleSprite : IDisposable
{
    private const int spriteLifeTimer = 60;
    private VertexBuffer vertices;
    private float positionX;
    private float positionY;
    private float centerX;
    private float centerY;
    private float degree;
    private float scaleX;
    private float scaleY;
    private int lifeTimer = 0;
    private Texture2D texture;

    /// Construct from Texture2D
    public SampleSprite(Texture2D texture, float positionX, float positionY)
    {
        SetTexture(texture);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = 0.0f;

        ScaleX = 1.0f;
        ScaleY = 1.0f;

        SetLifeTimer();
    }

    /// Construct from Texture2D
    public SampleSprite(Texture2D texture, float positionX, float positionY, float degree, float scale)
    {
        SetTexture(texture);

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = degree;

        ScaleX = scale;
        ScaleY = scale;

        SetLifeTimer();
    }

    /// Construct from Texture2D
    public SampleSprite(Texture2D texture,
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

        SetLifeTimer();
    }

    /// Construct from text
    public SampleSprite(string text, uint argb, Font font, int positionX, int positionY)
    {
        int width = font.GetTextWidth(text, 0, text.Length);
        int height = font.Metrics.Height;

        // clamp to max size
        width = Math.Min( width, 2048 );

        var image = new Image(ImageMode.Rgba,
                              new ImageSize(width, height),
                              new ImageColor(0, 0, 0, 0) );

        image.DrawText(text,
                       new ImageColor((int)((argb >> 16) & 0xff),
                                      (int)((argb >> 8) & 0xff),
                                      (int)((argb >> 0) & 0xff),
                                      (int)((argb >> 24) & 0xff)),
                       font, new ImagePosition(0, 0));

        var texture = new Texture2D(width, height, false, PixelFormat.Rgba);
        texture.SetPixels(0, image.ToBuffer(), 0, 0, width, height);
        image.Dispose();

        SetTexture(texture);
        texture.Dispose();

        PositionX = positionX;
        PositionY = positionY;

        CenterX = texture.Width / 2;
        CenterY = texture.Height / 2;
        Degree = 0.0f;

        ScaleX = 1.0f;
        ScaleY = 1.0f;

        SetLifeTimer();
    }

    public void Dispose()
    {
        vertices.Dispose();
        texture.Dispose();
    }

    public float PositionX
    {
        get {return positionX;}
        set {positionX = value;}
    }

    public float PositionY
    {
        get {return positionY;}
        set {positionY = value;}
    }

    /// Rotation center X
    public float CenterX
    {
        get {return centerX;}
        set {centerX = value;}
    }

    /// Rotation center Y
    public float CenterY
    {
        get {return centerY;}
        set {centerY = value;}
    }

    /// Angle (Euler)
    public float Degree
    {
        get {return degree;}
        set {degree = value;}
    }

    public float ScaleX
    {
        get {return scaleX;}
        set {scaleX = value;}
    }

    public float ScaleY
    {
        get {return scaleY;}
        set {scaleY = value;}
    }

    public VertexBuffer Vertices
    {
        get {return vertices;}
    }

    public Texture2D Texture
    {
        get {return texture;}
    }

    public void SetTexture(Texture2D texture)
    {
        this.texture = texture.ShallowClone() as Texture2D;
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

    public Matrix4 CreateModelMatrix()
    {
        var centerMatrix = Matrix4.Translation(new Vector3(-centerX, -centerY, 0.0f));
        var transMatrix = Matrix4.Translation(new Vector3(positionX + centerX, positionY + centerY, 0.0f));
        var rotMatrix = Matrix4.RotationZ((float)(degree / 180.0f * FMath.PI));
        var scaleMatrix = Matrix4.Scale(new Vector3(scaleX, scaleY, 1.0f));

        return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
    }

    public void SetLifeTimer()
    {
        lifeTimer = spriteLifeTimer;
    }

    public bool CheckLifeTimer()
    {
        lifeTimer--;
        if(0 > lifeTimer)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}

} // Sample
