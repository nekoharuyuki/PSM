/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
	
namespace Sample
{

/**
 * StorageSample
 */
public static class StorageSample
{
    const string EVENT_LABEL =   "LAST EVENT";
    const string EVENT_LOAD =    "File Data LOAD...";
    const string EVENT_WRITE =   "Write to File...";
    const string EVENT_DELETE =  "File Delete...";
    const string BUTTON_LOAD =   "Read";
    const string BUTTON_SAVE =   "Write";
    const string BUTTON_DELETE = "Delete";
    const string SAVE_FILE =     "/Documents/testdata.dat";

    const int BUTTON_NO = 3;
    const int MAX_INPUT_CHARACTOR = 256;

    private static GraphicsContext graphics;
    private static int screenWidth;
    private static int screenHeight;

    private static SampleButton inputTextButton;
    private static TextInputDialog dialog;
    private static int dialogXPos;

    private static SampleButton button0;
    private static SampleButton button1;
    private static SampleButton button2;
    private static string eventAction = "";
    private static bool isFileExist = false;
    private static byte[] inputData;
    private static int inputDataSize;

    static bool loop = true;

    /// Program Entry Point
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

    /// Initialize
    public static bool Init()
    {
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        screenWidth = graphics.Screen.Width;
        screenHeight = graphics.Screen.Height;

        isFileExist = System.IO.File.Exists(SAVE_FILE);

        // Set status of the object from the contents
        SetupObjects();

        return true;
    }

    /// terminate
    public static void Term()
    {
        button0.Dispose();
        button1.Dispose();
        button2.Dispose();

        inputTextButton.Dispose();
        if (dialog != null) {
            dialog.Dispose();
            dialog = null;
        }

        SampleDraw.Term();
        graphics.Dispose();
    }

    /// tick tack
    public static bool Update()
    {
        SampleDraw.Update();

        CheckChangeState();

        return true;
    }

    /// render
    public static bool Render()
    {
        graphics.SetViewport(0, 0, screenWidth, screenHeight);
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        button0.Draw();
        button1.Draw();
        button2.Draw();

        inputTextButton.Draw();

        SampleDraw.DrawText("Storage Sample", 0xffffffff, 0, 0);
        SampleDraw.DrawText("data", 0xffffffff, dialogXPos, 224);

        SampleDraw.DrawText(EVENT_LABEL, 0xffffffff, dialogXPos, 320);

        if ( eventAction.Length > 0 ) {
            SampleDraw.DrawText(eventAction, 0xffffffff, dialogXPos, 360);
        }

        graphics.SwapBuffers();

        return true;
    }

    // use objects intialize.
    private static void SetupObjects()
    {
        int sx = (screenWidth - (BUTTON_NO * 192)) / 2;
        int soff = (192 - 96) / 2;
        button0 = new SampleButton(sx + (192 * 0) + soff, 128, 128, 48);
        button1 = new SampleButton(sx + (192 * 1) + soff, 128, 128, 48);
        button2 = new SampleButton(sx + (192 * 2) + soff, 128, 128, 48);

        button0.SetText(BUTTON_LOAD);
        button1.SetText(BUTTON_SAVE);
        button2.SetText(BUTTON_DELETE);

        string label = "";

        dialogXPos = (screenWidth - 480) / 2;
        inputTextButton = new SampleButton( dialogXPos, 256, 480, 32);
        inputTextButton.SetText(label, SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);
        dialog = null;
    }

    // check objects change state,
    private static bool CheckChangeState()
    {
        bool changeState = false;
        List<TouchData> touchDataList = Touch.GetData(0);

        // button.
        if ( button0.TouchDown(touchDataList) ) {

            if ( true == System.IO.File.Exists(@SAVE_FILE) ) {
                using (System.IO.FileStream hStream = System.IO.File.Open(@SAVE_FILE, FileMode.Open)) {
                    if (hStream != null) {
                        long size = hStream.Length;
                        inputData = new byte[size];
                        hStream.Read(inputData, 0, (int)size);
                        inputDataSize = (int)size;
                        string label = System.Text.Encoding.Unicode.GetString(inputData);
                        inputTextButton.SetText(label, SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);
                        hStream.Close();
                        eventAction = EVENT_LOAD;
                    }
                }
            }
        }

        else if ( button1.TouchDown(touchDataList) ) {
            if ( inputDataSize > 0 ) {
                using (System.IO.FileStream hStream = System.IO.File.Open(@SAVE_FILE, FileMode.Create)) {
                    hStream.SetLength((int)inputDataSize);
                    hStream.Write(inputData, 0, (int)inputDataSize);
                    hStream.Close();
                    isFileExist = true;
                    eventAction = EVENT_WRITE;
                }
            }
        }

        else if ( button2.TouchDown(touchDataList) ) {
            if (true == System.IO.File.Exists(@SAVE_FILE) )
            {
                System.IO.File.Delete(@SAVE_FILE);
                inputTextButton.SetText("", SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);
                inputDataSize = 0;
                inputData = null;
                isFileExist = false;
                eventAction = EVENT_DELETE;
            }
        }


        // input dialog.
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
                    string setLabelStr;
                    int i;

                    int len = dialog.Text.Length;

                    if (len > MAX_INPUT_CHARACTOR) {
                        len = MAX_INPUT_CHARACTOR;
                    }

                    setLabelStr = dialog.Text.Substring(0, len);

                    inputTextButton.Label = setLabelStr;

                    byte[] byteArray0 = System.Text.Encoding.Unicode.GetBytes(setLabelStr);
                    inputData = new byte[byteArray0.Length];
                    for (i = 0; i < byteArray0.Length; i++) {
                        inputData[i] = byteArray0[i];
                    }

                    inputDataSize = byteArray0.Length;

                    changeState = true;
                }
                dialog.Dispose();
                dialog = null;
            }
        }

        return changeState;
    }
}

} // Sample
