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
using Sce.PlayStation.Core.Audio;

namespace Sample
{

/**
 * SoundPlayerSample
 */
public static class SoundPlayerSample
{
    private class SoundButton : IDisposable
    {
        private SampleSprite textSprite;
        private SampleButton playButton;
        private SampleButton stopButton;
        private SampleSlider volumeSlider;
        private Sound sound;
        private SoundPlayer soundPlayer;

        private readonly string[] soundName = {
            "/Application/SYS_SE_01.wav",
            "/Application/GAME_SE_01.wav",
            "/Application/GAME_SE_10.wav",
            "/Application/GAME_SE_11.wav",
        };

        public SoundButton(int id, int pixelX, int pixelY)
        {
            textSprite = new SampleSprite("SE " + id, 0xffffffff, SampleDraw.CurrentFont, pixelX - 64, pixelY + 12);
            playButton = new SampleButton(pixelX, pixelY, 96, 48);
            stopButton = new SampleButton(pixelX + 112, pixelY, 96, 48);
            volumeSlider = new SampleSlider(pixelX + 224, pixelY, 256, 48);

            playButton.SetText("Play");
            stopButton.SetText("Stop");

            sound = new Sound(soundName[id]);
            soundPlayer = sound.CreatePlayer();
        }

        public void Dispose()
        {
            soundPlayer.Stop();
            sound.Dispose();

            textSprite.Dispose();
            playButton.Dispose();
            stopButton.Dispose();
            volumeSlider.Dispose();
        }

        public void Update(List<TouchData> touchDataList)
        {
            if (playButton.TouchDown(touchDataList)) {
                soundPlayer.Play();
            }

            if (stopButton.TouchDown(touchDataList)) {
                soundPlayer.Stop();
            }

            volumeSlider.Update(touchDataList);
            soundPlayer.Volume = volumeSlider.Rate;
        }

        public void Draw()
        {
            SampleDraw.DrawSprite(textSprite);
            playButton.Draw();
            stopButton.Draw();
            volumeSlider.Draw();
        }
    }

    private static GraphicsContext graphics;
    private static List<SoundButton> soundButtonList;
    private static int screenWidth;
    private static int screenHeight;

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

        screenWidth = graphics.Screen.Width;
        screenHeight = graphics.Screen.Height;

        soundButtonList = new List<SoundButton>();
        soundButtonList.Add(new SoundButton(0, (graphics.Screen.Width / 2) - (480 / 2), (graphics.Screen.Height / 2) - 128+0));
        soundButtonList.Add(new SoundButton(1, (graphics.Screen.Width / 2) - (480 / 2), (graphics.Screen.Height / 2) - 128+64));
        soundButtonList.Add(new SoundButton(2, (graphics.Screen.Width / 2) - (480 / 2), (graphics.Screen.Height / 2) - 128+128));
        soundButtonList.Add(new SoundButton(3, (graphics.Screen.Width / 2) - (480 / 2), (graphics.Screen.Height / 2) - 128+192));

        return true;
    }

    /// Terminate
    public static void Term()
    {
        foreach (var soundButton in soundButtonList) {
            soundButton.Dispose();
        }

        SampleDraw.Term();
        graphics.Dispose();
    }

    public static bool Update()
    {
        SampleDraw.Update();

        List<TouchData> touchDataList = Touch.GetData(0);

        foreach (var soundButton in soundButtonList) {
            soundButton.Update(touchDataList);
        }

        return true;
    }

    public static bool Render()
    {
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        foreach (var soundButton in soundButtonList) {
            soundButton.Draw();
        }

        SampleDraw.DrawText("SoundPlayer Sample", 0xffffffff, 0, 0);
        SampleDraw.DrawText("Volume", 0xffffffff, (screenWidth / 2) + 72, (screenHeight / 2) - 168);

        graphics.SwapBuffers();

        return true;
    }
}

} // Sample
