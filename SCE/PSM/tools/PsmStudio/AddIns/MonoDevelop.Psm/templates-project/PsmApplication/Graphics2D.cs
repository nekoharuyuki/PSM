/* SCE CONFIDENTIAL
 * PlayStation(R) Mobile Software Development Kit 0.91.0
 * Copyright (C) 2011 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace ${Namespace}
{

/**
 * Graphics2Dクラス
 */
public static class Graphics2D
{
    private static GraphicsContext graphics;

    private static ShaderProgram textureShaderProgram;
    private static ShaderProgram colorShaderProgram;

    private static Matrix4 projectionMatrix;
    private static Matrix4 viewMatrix;

    private static VertexBuffer rectVertices;
    private static VertexBuffer circleVertices;

    private static Dictionary<string, Sprite> spriteDict;
    private static int spriteNamelessCount;

    private static Font defaultFont;
    private static Font currentFont;


    /// 初期化
    public static bool Init(GraphicsContext graphicsContext)
    {
        graphics = graphicsContext;

        textureShaderProgram = createSimpleTextureShader();
        colorShaderProgram = createSimpleColorShader();

        projectionMatrix = Matrix4.Ortho(0, Width,
                                         0, Height,
                                         0.0f, 32768.0f);

        viewMatrix = Matrix4.LookAt(new Vector3(0, Height, 0),
                                    new Vector3(0, Height, 1),
                                    new Vector3(0, -1, 0));

        rectVertices =  new VertexBuffer(4, VertexFormat.Float3);
        rectVertices.SetVertices(0, new float[]{0, 0, 0,
                                                1, 0, 0,
                                                1, 1, 0,
                                                0, 1, 0});

        circleVertices =  new VertexBuffer(36, VertexFormat.Float3);
        float[] circleVertex = new float[3 * 36];
        for (int i = 0; i < 36; i++) {
            float radian = ((i * 10) / 180.0f * FMath.PI);
            circleVertex[3 * i + 0] = FMath.Cos(radian);
            circleVertex[3 * i + 1] = FMath.Sin(radian);
            circleVertex[3 * i + 2] = 0.0f;
        }
        circleVertices.SetVertices(0, circleVertex);

        defaultFont = new Font(FontAlias.System, 24, FontStyle.Regular);
        SetDefaultFont();

        spriteDict = new Dictionary<string, Sprite>();
        spriteNamelessCount = 0;

        return true;
    }


    /// 解放
    public static void Term()
    {
        ClearSprite();

        defaultFont.Dispose();
        circleVertices.Dispose();
        rectVertices.Dispose();
        colorShaderProgram.Dispose();
        textureShaderProgram.Dispose();
        graphics = null;
    }

    /// 横幅
    public static int Width
    {
        get {return graphics.GetFrameBuffer().Width;}
    }

    /// 縦幅
    public static int Height
    {
        get {return graphics.GetFrameBuffer().Height;}
    }

    /// タッチ座標 -> 画面座標 : X
    public static int TouchPixelX(TouchData touchData)
    {
        return (int)((touchData.X + 0.5f) * Width);
    }

    /// タッチ座標 -> 画面座標 : Y
    public static int TouchPixelY(TouchData touchData)
    {
        return (int)((touchData.Y + 0.5f) * Height);
    }

    /// スプライトの追加
    public static void AddSprite(Sprite sprite)
    {
        AddSprite("[nameless]:" + spriteNamelessCount, sprite);
        spriteNamelessCount++;
    }

    /// スプライトの追加
    public static void AddSprite(string key, Sprite sprite)
    {
        if (spriteDict.ContainsKey(key) == false) {
            spriteDict.Add(key, sprite);
        }
    }

    /// 文字スプライト登録
    public static void AddSprite(string key, string text, uint argb, Font font, int positionX, int positionY)
    {
        if (spriteDict.ContainsKey(text) == false) {
            AddSprite(key, new Sprite(text, argb, font, positionX, positionY));
        }
    }

    /// 文字スプライト登録
    public static void AddSprite(string text, uint argb, int positionX, int positionY)
    {
        AddSprite(text, text, argb, currentFont, positionX, positionY);
    }

    /// スプライトの削除
    public static void RemoveSprite(string key)
    {
        if (spriteDict.ContainsKey(key)) {
            spriteDict[key].Dispose();
            spriteDict.Remove(key);
        }
    }

    /// スプライトのクリア
    public static void ClearSprite()
    {
        foreach (string key in spriteDict.Keys) {
            spriteDict[key].Dispose();
        }

        spriteDict.Clear();
        spriteNamelessCount = 0;
    }

    /// 登録されたスプライトをまとめて描画
    public static void DrawSprites()
    {
        foreach (string key in spriteDict.Keys) {
            DrawSprite(spriteDict[key]);
        }
    }

    /// スプライト描画
    public static void DrawSprite(string key)
    {
        if (spriteDict.ContainsKey(key)) {
            DrawSprite(spriteDict[key]);
        }
    }

    /// スプライト描画
    public static void DrawSprite(Sprite sprite)
    {
        var modelMatrix = sprite.CreateModelMatrix();
        var worldViewProj = projectionMatrix * viewMatrix * modelMatrix;

        textureShaderProgram.SetUniformValue(0, ref worldViewProj);

        graphics.SetShaderProgram(textureShaderProgram);
        graphics.SetVertexBuffer(0, sprite.Vertices);
        graphics.SetTexture(0, sprite.Texture);

        graphics.Enable(EnableMode.Blend);
        graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);

        graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
    }

    /// カレントフォント
    public static Font CurrentFont
    {
        get {return currentFont;}
    }

    /// フォント設定
    public static Font SetDefaultFont()
    {
        return SetFont(defaultFont);
    }

    /// フォント設定
    public static Font SetFont(Font font)
    {
        Font previousFont = currentFont;
        currentFont = font;
        return previousFont;
    }

    /// 簡易文字列描画
    public static void DrawText(string text, uint argb, Font font, int positionX, int positionY)
    {
        AddSprite(text, text, argb, font, positionX, positionY);
        DrawSprite(text);
    }

    /// 簡易文字列描画
    public static void DrawText(string text, uint argb, int positionX, int positionY)
    {
        AddSprite(text, text, argb, currentFont, positionX, positionY);
        DrawSprite(text);
    }

    /// 矩形塗りつぶし
    public static void FillRect(uint argb, int positionX, int positionY, int rectW, int rectH)
    {
        FillVertices(rectVertices, argb, positionX, positionY, rectW, rectH);
    }

    // 円塗りつぶし
    public static void FillCircle(uint argb, int positionX, int positionY, int radius)
    {
        FillVertices(circleVertices, argb, positionX, positionY, radius, radius);
    }

    // 頂点バッファ塗りつぶし
    public static void FillVertices(VertexBuffer vertices, uint argb, float positionX, float positionY, float scaleX, float scaleY)
    {
        var transMatrix = Matrix4.Translation(new Vector3(positionX, positionY, 0.0f));
        var scaleMatrix = Matrix4.Scale(new Vector3(scaleX, scaleY, 1.0f));
        var modelMatrix = transMatrix * scaleMatrix;

        var worldViewProj = projectionMatrix * viewMatrix * modelMatrix;

        colorShaderProgram.SetUniformValue(0, ref worldViewProj);

        Vector4 color = new Vector4((float)((argb >> 16) & 0xff) / 0xff,
                                    (float)((argb >> 8) & 0xff) / 0xff,
                                    (float)((argb >> 0) & 0xff) / 0xff,
                                    (float)((argb >> 24) & 0xff) / 0xff);
        colorShaderProgram.SetUniformValue(colorShaderProgram.FindUniform("MaterialColor"), ref color);

        graphics.SetShaderProgram(colorShaderProgram);
        graphics.SetVertexBuffer(0, vertices);

        graphics.DrawArrays(DrawMode.TriangleFan, 0, vertices.VertexCount);
    }

    /// 画像ファイルからテクスチャを生成
    public static Texture2D CreateTexture(string fileName)
    {
        var image = new Image(fileName);
        image.Decode();

        var texture = new Texture2D(image.Size.Width, image.Size.Height, false, PixelFormat.Rgba);
        texture.SetPixels(0, image.ToBuffer());

        image.Dispose();

        return texture;
    }

    private static ShaderProgram createSimpleTextureShader()
    {
        var shaderProgram = new ShaderProgram("shaders/Texture.cgx");

        shaderProgram.SetAttributeBinding(0, "a_Position");
        shaderProgram.SetAttributeBinding(1, "a_TexCoord");

        shaderProgram.SetUniformBinding(0, "WorldViewProj");

        return shaderProgram;
    }

    private static ShaderProgram createSimpleColorShader()
    {
        var shaderProgram = new ShaderProgram("shaders/Simple.cgx");

        shaderProgram.SetAttributeBinding(0, "a_Position");
        //shaderProgram.SetAttributeBinding(1, "a_Color0");

        shaderProgram.SetUniformBinding(0, "WorldViewProj");

        return shaderProgram;
    }
}

} // Sample
