/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;

namespace Sample
{

/**
 * HttpSample
 */
public static class HttpSample
{
    private static GraphicsContext graphics;
    private static SampleButton connectButton;
    private static SampleSprite httpText = null;

    private const int maxReadSize = 1024;
    private static int totalReadSize;
    private static MemoryStream readStream;
    private static byte[] readBuffer;
    private static ConnectState connectState = ConnectState.None;

    private static string statusCode;
    private static long contentLength;

    public enum ConnectState
    {
        None,
        Ready,
        Connect,
        Success,
        Failed
    }

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

        connectButton = new SampleButton(48, 48, 256, 64);
        connectButton.SetText("Connect");

        connectState = ConnectState.None;

        return true;
    }

    /// Terminate
    public static void Term()
    {
        if (httpText != null) {
            httpText.Dispose();
            httpText = null;
        }

        connectButton.Dispose();

        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        SampleDraw.Update();

        List<TouchData> touchDataList = Touch.GetData(0);

        switch (connectState) {
        case ConnectState.None:
            if (connectButton.TouchDown(touchDataList)) {
                connectState = ConnectState.Ready;
                if (httpText != null) {
                    httpText.Dispose();
                    httpText = null;
                }
                connectButton.ButtonColor = 0xff7f7f7f;
            }
            break;
        case ConnectState.Ready:
            connectHttpTest("http://www.scei.co.jp/index_e.html");
            break;
        case ConnectState.Success:
            connectButton.ButtonColor = 0xffffffff;

            if (readStream != null) {
                httpText = createTextSprite(readStream.ToArray(), statusCode, contentLength);

                readStream.Dispose();
                readStream = null;
                readBuffer = null;
            }

            connectState = ConnectState.None;
            break;
        case ConnectState.Failed:
            httpText = createTextSprite(null, statusCode, contentLength);
            connectState = ConnectState.None;
            break;
        }

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        if (httpText != null) {
            SampleDraw.DrawSprite(httpText);
        }

        connectButton.Draw();

        SampleDraw.DrawText("Http Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }

    /// Http connection test
    private static bool connectHttpTest(string url)
    {
        statusCode = "Unknown";
        contentLength = 0;

        try {
            var webRequest = HttpWebRequest.Create(url);
            // If you use web proxy, uncomment this and set appropriate address.
            //webRequest.Proxy = new System.Net.WebProxy("http://your_proxy.com:10080");
            webRequest.BeginGetResponse(new AsyncCallback(requestCallBack), webRequest);
            connectState = ConnectState.Connect;
        } catch (Exception e) {
            Console.WriteLine(e);
            connectState = ConnectState.Failed;
            return false;
        }

        return true;
    }

    private static void requestCallBack(IAsyncResult ar)
    {
        try {
            var webRequest = (HttpWebRequest)ar.AsyncState;
            var webResponse = (HttpWebResponse)webRequest.EndGetResponse(ar);

            statusCode = webResponse.StatusCode.ToString();
            contentLength = webResponse.ContentLength;
            readBuffer = new byte[1024];
            readStream = new MemoryStream();
            totalReadSize = 0;

            var stream = webResponse.GetResponseStream();
            stream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(readCallBack), stream);
        } catch (Exception e) {
            Console.WriteLine(e);
            connectState = ConnectState.Failed;
        }
    }

    private static void readCallBack(IAsyncResult ar)
    {
        try {
            var stream = (Stream)ar.AsyncState;
            int readSize = stream.EndRead(ar);

            if (readSize > 0) {
                totalReadSize += readSize;
                readStream.Write(readBuffer, 0, readSize);
            }

            if (readSize <= 0 || totalReadSize >= maxReadSize) {
                stream.Close();
                connectState = ConnectState.Success;
            } else {
                stream.BeginRead(readBuffer, 0, readBuffer.Length, new AsyncCallback(readCallBack), stream);
            }
        } catch (Exception e) {
            Console.WriteLine(e);
            connectState = ConnectState.Failed;
        }
    }

    /// Create a sprite from a string buffer
    private static SampleSprite createTextSprite(byte[] buffer, string statusCode, long contentLength)
    {
        int width = SampleDraw.Width;
        int height = SampleDraw.Height - 128;
        int positionX = 0;
        int positionY = 0;
        int fontHeight = SampleDraw.CurrentFont.Metrics.Height;

        var image = new Image(ImageMode.Rgba,
                              new ImageSize(width, height),
                              new ImageColor(0, 0, 0, 0));

        image.DrawText("ResponseCode : " + statusCode, new ImageColor(0xff, 0xff, 0xff, 0xff), SampleDraw.CurrentFont, new ImagePosition(positionX, positionY));
        positionY += fontHeight;
        image.DrawText("ContentLength : " + contentLength, new ImageColor(0xff, 0xff, 0xff, 0xff), SampleDraw.CurrentFont, new ImagePosition(positionX, positionY));
        positionY += fontHeight;
        positionY += fontHeight;

        if (buffer != null) {
            var stream = new MemoryStream(buffer);
            var reader = new StreamReader(stream);

            while (!reader.EndOfStream && positionY < height) {
                string text = reader.ReadLine();
                image.DrawText(text, new ImageColor(0xff, 0xff, 0xff, 0xff), SampleDraw.CurrentFont, new ImagePosition(positionX, positionY));
                positionY += fontHeight;
            }

            reader.Close();
            stream.Close();
        }

        var texture = new Texture2D(width, height, false, PixelFormat.Rgba);
        texture.SetPixels(0, image.ToBuffer());
        image.Dispose();

        var sprite = new SampleSprite(texture, 0, 128);

        return sprite;
    }
}

} // Sample
