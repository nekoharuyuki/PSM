/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    public class ImageManager
    {
        static readonly string saveDir = "data";
        static readonly string fileprefix = "calender_";
        static readonly string fileExt = ".bmp";
        static int fileLastNumber = 0;

        public static List<ImageAsset> LoadCreatedImages()
        {
#if ENABLE_PB_SAVE
            var images = new List<ImageAsset>();
            string[] files;

            try
            {
                checkAndCreateSaveDir();

                files = Directory.GetFiles(saveDir, "*" + fileExt, SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed loading the image.\n", ex.ToString());
                return images;
            }


            foreach (var filename in files)
            {
                try
                {
                    images.Add(new ImageAsset(filename, false));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed loading the image:{0} ", filename);
                    Console.WriteLine(ex.ToString());
                }
            }
            return images;
#else
            return LoadMaterialImages();
#endif
        }

        public static List<ImageAsset> LoadMaterialImages()
        {
            var images = new List<ImageAsset>();

            string[] files = Directory.GetFiles("/Application/assets", "wallpaper*.jpg", SearchOption.TopDirectoryOnly);
			
            foreach (var filename in files)
            {
                images.Add(new ImageAsset(filename));
            }

            return images;
        }

        static string getFileName(int n)
        {
            return saveDir + "/" + fileprefix + fileLastNumber.ToString("0000") + fileExt;
        }

        public static void SaveImage(Panel panel)
        {
            try
            {
                checkAndCreateSaveDir();

                do
                {
                    fileLastNumber++;
                } while (File.Exists(getFileName(fileLastNumber)));


                var width = UISystem.FramebufferWidth;
                var height = UISystem.FramebufferHeight;

                ColorBuffer colorBuffer = new ColorBuffer(width, height, PixelFormat.Rgba);
                FrameBuffer frameBuffer = new FrameBuffer();
                frameBuffer.SetColorTarget(colorBuffer);

                float scale = 1f;// width / panel.Width;

                var mat = new Matrix4(
                    scale, 0.0f, 0.0f, 0.0f,
                    0.0f, -scale, 0.0f, 0.0f,
                    0.0f, 0.0f, 1.0f, 0.0f,
                    0.0f, height, 0.0f, 1.0f);


                panel.RenderToFrameBuffer(frameBuffer, mat);


                UISystem.GraphicsContext.SetFrameBuffer(frameBuffer);
                byte[] data = new byte[width * height * 4];
                UISystem.GraphicsContext.ReadPixels(data, PixelFormat.Rgba, 0, 0, width, height);
                var image = new Image(ImageMode.Rgba, new ImageSize(width, height), data);
                image.Export("CalendarMaker", fileprefix + fileLastNumber.ToString("0000") + ".png");

//                var thumbnail = image.Resize (new ImageSize(100*width/height, 100));
//                thumbnail.Export("CalendarMaker", fileprefix + fileLastNumber.ToString("0000") + "_thumb.png");
//                data = thumbnail.ToBuffer();
//                width = thumbnail.Size.Width;
//                height = thumbnail.Size.Height;
//                thumbnail.Dispose ();

                image.Dispose ();

                byte[] header = new byte[]{
                    // BITMAPFILEHEADER
                    0x42, 0x4d,             // WORD    bfType;
                    0x00, 0x00, 0x00, 0x00, // DWORD   bfSize;
                    0x00, 0x00,             // WORD    bfReserved1;
                    0x00, 0x00,             // WORD    bfReserved2;
                    0x00, 0x00, 0x00, 0x00, // DWORD   bfOffBits;
                    // BITMAPINFOHEADER
                    0x28, 0x00, 0x00, 0x00, // DWORD  biSize;
                    (byte)(width & 0xff),
                    (byte)((width>>8) & 0xff),
                    0x00, 0x00,             // LONG   biWidth;
                    (byte)(height & 0xff),
                    (byte)((height>>8) & 0xff),
                    0x00, 0x00,             // LONG   biHeight;
                    0x01, 0x00,             // WORD   biPlanes;
                    0x20, 0x00,             // WORD   biBitCount;
                    0x03, 0x00, 0x00, 0x00, // DWORD  biCompression;
                    0x20, 0x00, 0x00, 0x00, // DWORD  biSizeImage;
                    0x00, 0x00, 0x00, 0x00, // LONG   biXPelsPerMeter;
                    0x00, 0x00, 0x00, 0x00, // LONG   biYPelsPerMeter;
                    0x00, 0x00, 0x00, 0x00, // DWORD  biClrUsed;
                    0x00, 0x00, 0x00, 0x00, // DWORD  biClrImportant;
                    // BitField
                    0xFF, 0x00, 0x00, 0x00, // R
                    0x00, 0xFF, 0x00, 0x00, // G
                    0x00, 0x00, 0xFF, 0x00, // B
                };

                using (var fs = new FileStream(getFileName(fileLastNumber), FileMode.CreateNew, FileAccess.Write))
                {
                    fs.Write(header, 0, header.Length);
                    int pixSize = data.Length / 4;
                    for (int i = height - 1; i >= 0; i--)
                    {
                        fs.Write(data, i * width * 4, width * 4);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed saving the image.\n" + ex.ToString());
            }
        }

        public static void DeleteImages()
        {
            if (Directory.Exists(saveDir))
            {
                Directory.Delete(saveDir, true);
            }
        }

        static void checkAndCreateSaveDir()
        {
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
        }
    }
}

