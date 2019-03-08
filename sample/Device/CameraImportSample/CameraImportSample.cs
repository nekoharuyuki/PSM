/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Device;
using Sce.PlayStation.Core.Imaging;

namespace Sample
{

/**
 * CameraImportSample
 */
public static class CameraImportSample
{
    // for System
    private static GraphicsContext sm_GraphicsContext = null;
    private static bool sm_IsLoop = true;

    // for CameraImportDialog
    private static CameraImportDialog sm_CameraImportDialog = null; 
    private static int sm_CountOfCameraDevice = 0;
    private static string sm_Filename = string.Empty;

    // for UserInterface
    private static SampleButton sm_ButtonOpenDialog = null;
    private static SampleSprite sm_SpriteImportImage = null;

    public static void Main(string[] args)
    {
        Init();

        while (sm_IsLoop)
        {
            SystemEvents.CheckEvents();
            Update();
            Render();
        }

        Term();
    }

    public static bool Init()
    {
        // Initialize GraphicsContext
        sm_GraphicsContext = new GraphicsContext();

        // Initialize SampleDraw
        SampleDraw.Init(sm_GraphicsContext);

        // Initialize sm_ButtonOpenDialog
        {
            var rectW = SampleDraw.Width / 4;
            var rectH = SampleDraw.Height / 8;
            var rectX = (SampleDraw.Width / 2) - (rectW / 2);
            var rectY = (SampleDraw.Height * 5 / 6) - (rectH / 2);
            sm_ButtonOpenDialog = new SampleButton(rectX, rectY, rectW, rectH);
            sm_ButtonOpenDialog.SetText("Open Dialog", SampleButton.TextAlign.Center, SampleButton.VerticalAlign.Middle);
        }
        
        // Initialize sm_CountOfCameraDevice
        sm_CountOfCameraDevice = Camera.GetNumberOfCameras();
            
        // Initialize sm_Filename
        if (0 >= sm_CountOfCameraDevice)
        {
            sm_Filename = "ERROR - NO CAMERA DEVICE";
        }

        return true;
    }

    public static void Term()
    {
        if (null != sm_SpriteImportImage)
        {
            sm_SpriteImportImage.Dispose();
            sm_SpriteImportImage = null;
        }

        SampleDraw.Term();

        if (null != sm_CameraImportDialog)
        {
            sm_CameraImportDialog.Dispose();
            sm_CameraImportDialog = null;
        }

        if (null != sm_GraphicsContext)
        {
            sm_GraphicsContext.Dispose();
            sm_GraphicsContext = null;
        }
    }

    public static bool Update()
    {
        List<TouchData> touchDataList = Touch.GetData(0);
        
        SampleDraw.Update();

        if (0 >= sm_CountOfCameraDevice)
        {
            return true;
        }

        bool isDialogOpened = (null != sm_CameraImportDialog);

        if (!isDialogOpened &&
            sm_ButtonOpenDialog.TouchDown(touchDataList))
        {
            sm_CameraImportDialog = new CameraImportDialog();
            sm_CameraImportDialog.Open();
            return true;
        }

        if (isDialogOpened &&
            CommonDialogState.Finished == sm_CameraImportDialog.State)
        {
            OnDialogClosed(sm_CameraImportDialog.Result);
            return true;
        }

        return true;
    }

    public static bool Render()
    {
        sm_GraphicsContext.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        sm_GraphicsContext.Clear();

        if (null != sm_SpriteImportImage)
        {
            SampleDraw.DrawSprite(sm_SpriteImportImage);
        }

        SampleDraw.FillRect(0xFF505050, 0, 0, SampleDraw.Width, 30);
        SampleDraw.DrawText("Camera Import Sample", 0xFFFFFFFF, 0, 0);

        SampleDraw.FillRect(0xFF505050, 0, SampleDraw.Height - 30, SampleDraw.Width, 30);
        SampleDraw.DrawText(sm_Filename, 0xFF00FFFF, 0, SampleDraw.Height - 30);

        sm_ButtonOpenDialog.Draw();

        sm_GraphicsContext.SwapBuffers();

        return true;
    }

    private static void OnDialogClosed(CommonDialogResult result)
    {
        if (result == CommonDialogResult.OK)
        {
            UpdatePhotoImage();
        }

        if (null != sm_CameraImportDialog)
        {
            sm_CameraImportDialog.Dispose();
            sm_CameraImportDialog = null;
        }
    }

    private static void UpdatePhotoImage()
    {
        sm_Filename = sm_CameraImportDialog.Filename;

        using (var image = new Image(sm_Filename))
        {
            #region fix DecodeSize
            {
                var maxLength = 1024; // 1024 x 1024 px (Max 4 MB)

                var currentLength =
                    Math.Max(
                        image.Size.Width,
                        image.Size.Height);

                if (maxLength < currentLength)
                {
                    var scale =
                        (float)maxLength / currentLength;

                    var decodeSize =
                        new ImageSize(
                            (int)(image.Size.Width * scale),
                            (int)(image.Size.Height * scale));

                    image.DecodeSize = decodeSize;
                }
            }
            #endregion

            image.Decode();

            // Add sprite
            {
                if (null != sm_SpriteImportImage)
                {
                    sm_SpriteImportImage.Dispose();
                    sm_SpriteImportImage = null;
                }
                    
                int positionX = (SampleDraw.Width - image.Size.Width) / 2;
                int positionY = (SampleDraw.Height - image.Size.Height) / 2;
                    
                sm_SpriteImportImage = new SampleSprite(CreateTexture(image), positionX, positionY);
            }
        }// using
    }

    // Create Texture2D from Image
    private static Texture2D CreateTexture(Image image)
    {
        var texture = new Texture2D(image.Size.Width, image.Size.Height, false, PixelFormat.Rgba);
        texture.SetPixels(0, image.ToBuffer());

        return texture;
    }
}

} // Sample
