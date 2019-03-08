/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using DemoGame;

namespace DefenseDemo
{

public class BgmResource : IDisposable
{
    private Bgm bgm = null;
    private BgmPlayer bgmPlayer = null;

    ///
    public BgmResource(string filePath)
    {
        bgm = new Bgm(filePath);
    }

    ///
    public void Dispose()
    {
        Stop();

        if (bgm != null) {
            bgm.Dispose();
            bgm = null;
        }
    }

    ///
    public void Play(bool loop = false, float volume = 1.0f)
    {
        Stop();

        bgmPlayer = bgm.CreatePlayer();
        bgmPlayer.Volume = volume;
        bgmPlayer.Loop = loop;
        bgmPlayer.Play();
    }

    ///
    public void Stop()
    {
        if (bgmPlayer != null) {
            if (bgmPlayer.Status == BgmStatus.Playing) {
                bgmPlayer.Stop();
            }
            bgmPlayer.Dispose();
            bgmPlayer = null;
        }
    }
}

public class SoundResource : IDisposable
{
    private Sound sound = null;
    private SoundPlayer soundPlayer = null;

    ///
    public SoundResource(string filePath)
    {
        sound = new Sound(filePath);
    }

    ///
    public void Dispose()
    {
        Stop();

        if (sound != null) {
            sound.Dispose();
            sound = null;
        }
    }

    ///
    public void Play(bool loop = false, float volume = 1.0f, float pan = 0.0f)
    {
        Stop();

        soundPlayer = sound.CreatePlayer();
        soundPlayer.Loop = loop;
        soundPlayer.Volume = volume;
        soundPlayer.Pan = pan;
        soundPlayer.Play();
    }

    ///
    public void Stop()
    {
        if (soundPlayer != null) {
            if (soundPlayer.Status == SoundStatus.Playing) {
                soundPlayer.Stop();
            }
            soundPlayer = null;
        }
    }
}

/**
 * AudioManagerクラス
 */
public static class AudioManager
{
    private static Dictionary<string, BgmResource> bgmTable = new Dictionary<string, BgmResource>();
    private static Dictionary<string, SoundResource> soundTable = new Dictionary<string, SoundResource>();

    ///
    public static void AddBgm(string key, string filePath)
    {
        if (FindBgm(key) == null) {
            bgmTable[key] = new BgmResource(filePath);
        }
    }

    ///
    public static void AddSound(string key, string filePath)
    {
        if (FindSound(key) == null) {
            soundTable[key] = new SoundResource(filePath);
        }
    }

    ///
    public static void Clear()
    {
        ClearSound();
        ClearBgm();
    }

    ///
    public static void ClearBgm()
    {
        StopBgm();

        foreach (string key in bgmTable.Keys) {
            bgmTable[key].Dispose();
        }

        bgmTable.Clear();
    }

    ///
    public static void RemoveBgm(string key)
    {
        if (FindBgm(key) != null) {
            bgmTable[key].Stop();
            bgmTable[key].Dispose();

            bgmTable.Remove(key);
        }
    }

    ///
    public static void ClearSound()
    {
        StopSound();

        foreach (string key in soundTable.Keys) {
            soundTable[key].Dispose();
        }

        soundTable.Clear();
    }


    ///
    public static void RemoveSound(string key)
    {
        if (FindSound(key) != null) {
            soundTable[key].Stop();
            soundTable[key].Dispose();

            soundTable.Remove(key);
        }
    }

    ///
    public static BgmResource FindBgm(string key)
    {
        if (bgmTable.ContainsKey(key)) {
            return bgmTable[key];
        }
        return null;
    }

    ///
    public static SoundResource FindSound(string key)
    {
        if (soundTable.ContainsKey(key)) {
            return soundTable[key];
        }
        return null;
    }

    ///
    public static void PlayBgm(string key, bool loop = true, float volume = 1.0f)
    {
        var bgm = FindBgm(key);

        if (bgm != null) {
            bgm.Play(loop, volume);
        }
    }

    ///
    public static void PlaySound(string key, bool loop = false, float volume = 1.0f, float pan = 0.0f)
    {
        var sound = FindSound(key);

        if (sound != null) {
            sound.Play(loop, volume, pan);
        }
    }

    ///
    public static void StopBgm()
    {
        foreach (string key in bgmTable.Keys) {
            bgmTable[key].Stop();
        }
    }

    ///
    public static void StopBgm(string key)
    {
        var bgm = FindBgm(key);

        if (bgm != null) {
            bgm.Stop();
        }
    }

    ///
    public static void StopSound()
    {
        foreach (string key in soundTable.Keys) {
            soundTable[key].Stop();
        }
    }

    ///
    public static void StopSound(string key)
    {
        var sound = FindSound(key);

        if (sound != null) {
            sound.Stop();
        }
    }
}

} // ShootingDemo
