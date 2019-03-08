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

namespace Sample
{

/**
 * DialogSample
 */
public static class DialogSample
{
    private static GraphicsContext graphics;
    private static SampleButton button;
    private static TextInputDialog dialog;

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

        int rectW = SampleDraw.Width / 2;
        int rectH = 32;
        int rectX = (SampleDraw.Width - rectW) / 2;
        int rectY = (SampleDraw.Height - 24) / 2;

        button = new SampleButton(rectX, rectY, rectW, rectH);
        button.SetText("InputText", SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);

        dialog = null;

        return true;
    }

    /// Terminate
    public static void Term()
    {
        if (dialog != null) {
            dialog.Dispose();
            dialog = null;
        }

        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        SampleDraw.Update();

        List<TouchData> touchDataList = Touch.GetData(0);

        if (button.TouchDown(touchDataList)) {
            if (dialog == null) {
                dialog = new TextInputDialog();
                dialog.Text = button.Label;
                dialog.Open();
            }
            return true;
        }

        if (dialog != null) {
            if (dialog.State == CommonDialogState.Finished) {
                if (dialog.Result == CommonDialogResult.OK) {
                    button.Label = dialog.Text;
                }
                dialog.Dispose();
                dialog = null;
            }
        }

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        button.Draw();

        SampleDraw.DrawText("Dialog Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
