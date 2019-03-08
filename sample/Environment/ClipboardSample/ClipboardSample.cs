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
 * ClipboardSample
 */
public static class ClipboardSample
{
    private static GraphicsContext graphics;
    private static SampleButton inputTextButton;
    private static SampleButton copyButton;
    private static SampleButton pasteButton;
    private static SampleButton copyTextButton;

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

        inputTextButton = new SampleButton( 32,  64, 480, 32);
        copyButton      = new SampleButton(544,  64,  96, 32);
        pasteButton     = new SampleButton( 32, 240,  96, 32);
        copyTextButton  = new SampleButton(160, 240, 480, 32);

        copyTextButton.ButtonColor = 0xff7f7f7f;

        inputTextButton.SetText("InputText", SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);
        copyButton.SetText("Copy");
        pasteButton.SetText("Paste");
        copyTextButton.SetText("CopyText", SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);

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

        if (pasteButton.TouchDown(touchDataList)) {
            copyTextButton.Label = Clipboard.GetText();
        }

        if (copyButton.TouchDown(touchDataList)) {
            Clipboard.SetText(inputTextButton.Label);
        }

        if (inputTextButton.TouchDown(touchDataList)) {
            if (dialog == null) {
                dialog = new TextInputDialog();
                dialog.Text = inputTextButton.Label;
                dialog.Open();
            }
            return true;
        }

        if (dialog != null) {
            if (dialog.State == CommonDialogState.Finished) {
                if (dialog.Result == CommonDialogResult.OK) {
                    inputTextButton.Label = dialog.Text;
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

        inputTextButton.Draw();
        copyButton.Draw();
        pasteButton.Draw();
        copyTextButton.Draw();

        SampleDraw.DrawText("Clipboard Sample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
