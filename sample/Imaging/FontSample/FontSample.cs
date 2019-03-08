/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;

namespace Sample
{

/**
 * FontSample
 */
public static class FontSample
{
    private static GraphicsContext graphics;
    private static List<SampleSprite> spriteList;

    static bool loop = true;

    public static void Main(string[] args)
    {
        Init();

        while (loop) {
            SystemEvents.CheckEvents();
            Update();
            Render();
        }

        Term();
    }

    public static bool Init()
    {
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        spriteList = new List<SampleSprite>();

        int positionX = 0;
        int positionY = 24;
        int line = 0;

        while (positionY < SampleDraw.Height) {
            int size = 8 + line * 3;

            var fonts = new Font[] {
                new Font(FontAlias.System, size, FontStyle.Regular),
                new Font(FontAlias.System, size, FontStyle.Bold),
                new Font(FontAlias.System, size, FontStyle.Italic),
                new Font(FontAlias.System, size, FontStyle.Bold | FontStyle.Italic),
            };

            var textSets = new[] {
                new {text = System.String.Format("{0}pt", size), font = fonts[0]},
                new {text = "/",          font = fonts[0]},
                new {text = "Regular",    font = fonts[0]},
                new {text = "/",          font = fonts[0]},
                new {text = "Bold",       font = fonts[1]},
                new {text = "/",          font = fonts[0]},
                new {text = "Italic",     font = fonts[2]},
                new {text = "/",          font = fonts[0]},
                new {text = "BoldItalic", font = fonts[3]},
                new {text = "/",          font = fonts[0]},
                new {text = "こんにちは", font = fonts[0]},
            };

            positionX = 0;
            foreach (var textSet in textSets) {
                var texture = createTexture(textSet.text, textSet.font, 0xffffffff);
                spriteList.Add(new SampleSprite(texture, positionX, positionY));

                positionX += textSet.font.GetTextWidth(textSet.text, 0, textSet.text.Length);
            }
            positionY += size;
            line++;

            foreach (var font in fonts) {
                font.Dispose();
            }
        }

        return true;
    }

    /// Terminate
    public static void Term()
    {
        foreach (var texture in spriteList) {
            texture.Dispose();
        }

        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        SampleDraw.Update();

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        foreach (var textSet in spriteList) {
            SampleDraw.DrawSprite(textSet);
        }

        SampleDraw.DrawText("Font Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }

    /// Given a string, create a texture
    private static Texture2D createTexture(string text, Font font, uint argb)
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

        return texture;
    }
}

} // Sample
