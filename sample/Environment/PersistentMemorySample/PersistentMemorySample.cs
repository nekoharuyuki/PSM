/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;


namespace Sample
{


/**
 * PersistentMemorySample
 */
public static class PersistentMemorySample
{
    const string BUTTON_PRESS =   "ON";
    const string BUTTON_RELEASE = "OFF";
    const uint ENABLE_COLOR  = 0xffffffff;
    const uint DISABLE_COLOR = 0xff7f7f7f;
    const int BUTTON_NO = 4;
    const int MAX_INPUT_CHARACTOR = 256;

    // Offset from the beginning of PersistentMemory address.
    const int STATE_ADDR_BUTTON0 = 1;
    const int STATE_ADDR_BUTTON1 = 3;
    const int STATE_ADDR_BUTTON2 = 5;
    const int STATE_ADDR_BUTTON3 = 7;
    const int STATE_ADDR_SLIDER = 20;
    const int STATE_ADDR_STRING_NO = 1000;
    const int STATE_ADDR_STRING = STATE_ADDR_STRING_NO + 4;

    private static GraphicsContext graphics;
    private static int screenWidth;
    private static int screenHeight;

    private static SampleSlider slider;
    private static float oldRate;

    private static SampleButton inputTextButton;
    private static TextInputDialog dialog;

    private static SampleButton button0;
    private static SampleButton button1;
    private static SampleButton button2;
    private static SampleButton button3;

    private static byte[] persistentMemoryData;

    static bool loop = true;

    /// エントリーポイント
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

    /// 初期化
    public static bool Init()
    {
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        screenWidth = graphics.Screen.Width;
        screenHeight = graphics.Screen.Height;

        // Gets the state of the object from PersistentMemory
        persistentMemoryData = PersistentMemory.Read();

        // Set status of the object from the contents from PersistentMemory
        SetupObjects();

        return true;
    }

    /// terminate
    public static void Term()
    {
        button0.Dispose();
        button1.Dispose();
        button2.Dispose();
        button3.Dispose();

        slider.Dispose();

        inputTextButton.Dispose();
        if (dialog != null) {
            dialog.Dispose();
            dialog = null;
        }

        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        bool changeState;

        changeState = CheckChangeState();

        // If update the status of the object, the data reflect of PersistentMemory.
        if(true == changeState){
            PersistentMemory.Write(persistentMemoryData);
        }

        return true;
    }

    public static bool Render()
    {
        graphics.SetViewport(0, 0, screenWidth, screenHeight);
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        button0.Draw();
        button1.Draw();
        button2.Draw();
        button3.Draw();

        slider.Draw();

        inputTextButton.Draw();
			
        SampleDraw.DrawText("PersistentMemory Sample", 0xffffffff, 0, 0);
        SampleDraw.DrawText("Toggle Button", 0xffffffff, (((screenWidth - (BUTTON_NO * 128)) / 2) + 16), 96);
        SampleDraw.DrawText("TextDialog", 0xffffffff, ((screenWidth - 480) / 2), 224);
        SampleDraw.DrawText("Slider", 0xffffffff, ((screenWidth - 384) / 2), 330);
        graphics.SwapBuffers();

        return true;
    }

    // use objects intialize.
    private static void SetupObjects()
    {
        int sx = (screenWidth - (BUTTON_NO * 128)) / 2;
        int soff = (128 - 96) / 2;
        button0 = new SampleButton(sx + (128 * 0) + soff, 128, 96, 48);
        button1 = new SampleButton(sx + (128 * 1) + soff, 128, 96, 48);
        button2 = new SampleButton(sx + (128 * 2) + soff, 128, 96, 48);
        button3 = new SampleButton(sx + (128 * 3) + soff, 128, 96, 48);

        if (0 == persistentMemoryData[STATE_ADDR_BUTTON0]) {
            button0.SetText(BUTTON_RELEASE);
            button0.ButtonColor = DISABLE_COLOR;
        } else {
            button0.SetText(BUTTON_PRESS);
            button0.ButtonColor = ENABLE_COLOR;
        }

        if(0 == persistentMemoryData[STATE_ADDR_BUTTON1]) {
            button1.SetText(BUTTON_RELEASE);
            button1.ButtonColor = DISABLE_COLOR;
        } else {
            button1.SetText(BUTTON_PRESS);
            button1.ButtonColor = ENABLE_COLOR;
        }

        if(0 == persistentMemoryData[STATE_ADDR_BUTTON2]) {
            button2.SetText(BUTTON_RELEASE);
            button2.ButtonColor = DISABLE_COLOR;
        } else {
            button2.SetText(BUTTON_PRESS);
            button2.ButtonColor = ENABLE_COLOR;
        }

        if(0 == persistentMemoryData[STATE_ADDR_BUTTON3]) {
            button3.SetText(BUTTON_RELEASE);
            button3.ButtonColor = DISABLE_COLOR;
        } else {
            button3.SetText(BUTTON_PRESS);
            button3.ButtonColor = ENABLE_COLOR;
        }

        // for slider setting
        byte[] byteArray = new byte[4];
        for(int i = 0; i < 4; i++){
            byteArray[i] = persistentMemoryData[STATE_ADDR_SLIDER+i];
        }
        float rate = BitConverter.ToSingle(byteArray, 0);
        sx = (screenWidth - 384) / 2;
        slider = new SampleSlider(sx, 360, 384, 48);
        slider.Rate = rate;
        oldRate = rate;

        // for text dialog setting
        byte[] byteArrayD0 = new byte[4];
        for(int i = 0; i < 4; i++){
            byteArrayD0[i] = persistentMemoryData[STATE_ADDR_STRING_NO+i];
        }
        int len = BitConverter.ToInt32(byteArrayD0, 0);
        int readSize = len * 2;
        byte[] byteArrayD1 = new byte[readSize];
        for(int i = 0; i < readSize; i++){
            byteArrayD1[i] = persistentMemoryData[STATE_ADDR_STRING+i];
        }

        string label = "";

        if(len != 0){
            label = System.Text.Encoding.Unicode.GetString(byteArrayD1);
        }

        sx = (screenWidth - 480) / 2;
        inputTextButton = new SampleButton( sx, 256, 480, 32);
        inputTextButton.SetText(label, SampleButton.TextAlign.Left, SampleButton.VerticalAlign.Middle);
        dialog = null;
    }

    // check objects change state,
    private static bool CheckChangeState()
    {
        bool changeState = false;
        List<TouchData> touchDataList = Touch.GetData(0);

        // button.
        if (button0.ButtonColor == DISABLE_COLOR &&
            button0.TouchDown(touchDataList)) {
            button0.SetText(BUTTON_PRESS);
            button0.ButtonColor = ENABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON0] = 1;
            changeState = true;
        } else if (button0.ButtonColor == ENABLE_COLOR &&
            button0.TouchDown(touchDataList)) {
            button0.SetText(BUTTON_RELEASE);
            button0.ButtonColor = DISABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON0] = 0;
            changeState = true;
        }

        if (button1.ButtonColor == DISABLE_COLOR &&
            button1.TouchDown(touchDataList)) {
            button1.SetText(BUTTON_PRESS);
            button1.ButtonColor = ENABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON1] = 1;
            changeState = true;
        } else if (button1.ButtonColor == ENABLE_COLOR &&
            button1.TouchDown(touchDataList)) {
            button1.SetText(BUTTON_RELEASE);
            button1.ButtonColor = DISABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON1] = 0;
            changeState = true;
        }

        if (button2.ButtonColor == DISABLE_COLOR &&
            button2.TouchDown(touchDataList)) {
            button2.SetText(BUTTON_PRESS);
            button2.ButtonColor = ENABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON2] = 1;
            changeState = true;
        } else if (button2.ButtonColor == ENABLE_COLOR &&
            button2.TouchDown(touchDataList)) {
            button2.SetText(BUTTON_RELEASE);
            button2.ButtonColor = DISABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON2] = 0;
            changeState = true;
        }

        if (button3.ButtonColor == DISABLE_COLOR &&
            button3.TouchDown(touchDataList)) {
            button3.SetText(BUTTON_PRESS);
            button3.ButtonColor = ENABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON3] = 1;
            changeState = true;
        } else if (button3.ButtonColor == ENABLE_COLOR &&
            button3.TouchDown(touchDataList)) {
            button3.SetText(BUTTON_RELEASE);
            button3.ButtonColor = DISABLE_COLOR;
            persistentMemoryData[STATE_ADDR_BUTTON3] = 0;
            changeState = true;
        }

        // slider.
        slider.Update(touchDataList);
        float curRate = slider.Rate;

        if (curRate != oldRate) {
            byte[] byteArray;
            byteArray = BitConverter.GetBytes(curRate);
            for (int i = 0; i < byteArray.Length; i++) {
                persistentMemoryData[STATE_ADDR_SLIDER+i] = byteArray[i];
            }
            oldRate = curRate;
            changeState = true;
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
                    for (i = 0; i < byteArray0.Length; i++) {
                        persistentMemoryData[STATE_ADDR_STRING+i] = byteArray0[i];
                    }

                    byte[] byteArray1 = BitConverter.GetBytes(len);
                    for (i = 0; i < byteArray1.Length; i++) {
                        persistentMemoryData[STATE_ADDR_STRING_NO+i] = byteArray1[i];
                    }

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
