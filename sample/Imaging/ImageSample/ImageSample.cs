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
 * ImageSample
 */
public static class ImageSample
{
    private static GraphicsContext graphics;
    private static List<Texture2D> textureList;
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

        textureList = new List<Texture2D>();
        spriteList = new List<SampleSprite>();

        var image1 = new Image("/Application/test.jpg");
        image1.Decode();
        textureList.Add(createTexture(image1));
        image1.Dispose();

        var image2_1 = new Image("/Application/test.jpg");
        var image2_2 = new Image("/Application/test.png");
        image2_1.Decode();
        image2_2.Decode();
        image2_1.DrawImage(image2_2, new ImagePosition((image2_1.Size.Width - image2_2.Size.Width) / 2,
                                                       (image2_1.Size.Height - image2_2.Size.Height) / 2));
        textureList.Add(createTexture(image2_1));
        image2_2.Dispose();
        image2_1.Dispose();

        var image3_1 = new Image("/Application/test.jpg");
        var image3_2 = image3_1.Crop(new ImageRect(image3_1.Size.Width / 4,
                                                   image3_1.Size.Height / 4,
                                                   image3_1.Size.Width / 4,
                                                   image3_1.Size.Height / 4));
        var image3_3 = image3_2.Resize(new ImageSize(image3_1.Size.Width, image3_1.Size.Height));
        textureList.Add(createTexture(image3_3));
        image3_3.Dispose();
        image3_2.Dispose();
        image3_1.Dispose();

        int pixelX = (graphics.Screen.Width / 2) - ((125 * 3) + (8 * 2));
        int pixelY = (graphics.Screen.Height / 2) - (250 / 2);

        foreach (var texture in textureList) {
            spriteList.Add(new SampleSprite(texture, pixelX, pixelY));
            pixelX += texture.Width + 16;
        }

        return true;
    }

    /// Image -> Texture2D conversion
    private static Texture2D createTexture(Image image)
    {
        var texture = new Texture2D(image.Size.Width, image.Size.Height, false, PixelFormat.Rgba);
        texture.SetPixels(0, image.ToBuffer());

        return texture;
    }

    /// Terminate
    public static void Term()
    {
        foreach (var texture in textureList) {
            texture.Dispose();
        }

        foreach (var sprite in spriteList) {
            sprite.Dispose();
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
        graphics.SetViewport(0, 0, graphics.GetFrameBuffer().Width, graphics.GetFrameBuffer().Height);
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        int pixelX = (graphics.Screen.Width / 2) - ((125 * 3) + (8 * 2));
        int pixelY = (graphics.Screen.Height / 2) - (250 / 2);

        foreach (var sprite in spriteList) {
            SampleDraw.DrawSprite(sprite);
        }

        SampleDraw.DrawText("Image Sample", 0xffffffff, 0, 0);
        SampleDraw.DrawText("Simple Image", 0xffffffff, pixelX, pixelY-32);
        SampleDraw.DrawText("DrawImage", 0xffffffff, pixelX+266, pixelY-32);
        SampleDraw.DrawText("Crop", 0xffffffff, pixelX+532, pixelY-32);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
