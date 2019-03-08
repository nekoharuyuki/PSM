/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using DemoGame;

namespace Physics2dDemo
{

/// BgmResourceクラス
public class BgmResource : IDisposable
{
    private Bgm bgm = null;
    private BgmPlayer bgmPlayer = null;

    /// コンストラクタ
    public BgmResource(string filePath)
    {
        bgm = new Bgm(filePath);
    }

    /// 破棄
    public void Dispose()
    {
        Stop();

        if (bgm != null) {
            bgm.Dispose();
            bgm = null;
        }
    }

    /// 再生
	///
	/// @param [in] loop   ループさせるか
	/// @param [in] volume ボリューム
    public void Play(bool loop = false, float volume = 1.0f)
    {
        Stop();

        bgmPlayer = bgm.CreatePlayer();
        bgmPlayer.Volume = volume;
        bgmPlayer.Loop = loop;
        bgmPlayer.Play();
    }

    /// 停止
    public void Stop()
    {
        if (bgmPlayer != null) {
            bgmPlayer.Stop();
            bgmPlayer.Dispose();
            bgmPlayer = null;
        }
    }
}

/// SoundResourceクラス
public class SoundResource : IDisposable
{
    private Sound sound = null;
    private SoundPlayer soundPlayer = null;

    /// コンストラクタ
    public SoundResource(string filePath)
    {
        sound = new Sound(filePath);
    }

    /// 破棄
    public void Dispose()
    {
        Stop();

        if (sound != null) {
            sound.Dispose();
            sound = null;
        }
    }

    /// 再生
	/// @param [in] loop   ループさせるか
	/// @param [in] volume ボリューム
	/// @param [in] pan    音の位置
    public void Play(bool loop = false, float volume = 1.0f, float pan = 0.0f)
    {
        Stop();

        soundPlayer = sound.CreatePlayer();
        soundPlayer.Loop = loop;
        soundPlayer.Volume = volume;
        soundPlayer.Pan = pan;
        soundPlayer.Play();
    }

    /// 停止
    public void Stop()
    {
        if (soundPlayer != null) {
            soundPlayer.Stop();
            soundPlayer = null;
        }
    }
}

/// AudioManagerクラス
///
public static class AudioManager
{
    private static Dictionary<string, BgmResource> bgmTable = new Dictionary<string, BgmResource>();
    private static Dictionary<string, SoundResource> soundTable = new Dictionary<string, SoundResource>();

    /// BGMの追加
	/// @param [in] key      BGM
	/// @param [in] filePath パス
    public static void AddBgm(string key, string filePath)
    {
        if (FindBgm(key) == null) {
            bgmTable[key] = new BgmResource(filePath);
        }
    }

    /// SEの追加
	/// @param [in] key      SE
	/// @param [in] filePath パス
    public static void AddSound(string key, string filePath)
    {
        if (FindSound(key) == null) {
            soundTable[key] = new SoundResource(filePath);
        }
    }

    /// クリア
    public static void Clear()
    {
        ClearSound();
        ClearBgm();
    }

    /// BGMのクリア
    public static void ClearBgm()
    {
        StopBgm();

        foreach (string key in bgmTable.Keys) {
            bgmTable[key].Dispose();
        }

        bgmTable.Clear();
    }

    /// 指定BGMの削除
	///
	/// @param [in] key      BGM
    public static void RemoveBgm(string key)
    {
        if (FindBgm(key) != null) {
            bgmTable[key].Stop();
            bgmTable[key].Dispose();

            bgmTable.Remove(key);
        }
    }

    /// SEのクリア
    public static void ClearSound()
    {
        StopSound();

        foreach (string key in soundTable.Keys) {
            soundTable[key].Dispose();
        }

        soundTable.Clear();
    }


    /// 指定SEの削除
	///
	/// @param [in] key      SE
    public static void RemoveSound(string key)
    {
        if (FindSound(key) != null) {
            soundTable[key].Stop();
            soundTable[key].Dispose();

            soundTable.Remove(key);
        }
    }

    /// BGMの検索
	///
	/// @param [in] key      BGM
    public static BgmResource FindBgm(string key)
    {
        if (bgmTable.ContainsKey(key)) {
            return bgmTable[key];
        }
        return null;
    }

    /// SEの検索
	///
	/// @param [in] key      SE
    public static SoundResource FindSound(string key)
    {
        if (soundTable.ContainsKey(key)) {
            return soundTable[key];
        }
        return null;
    }

    /// BGMの再生
	///
	/// @param [in] key      BGM
	/// @param [in] loop     ループさせるか
	/// @param [in] volume   ボリューム
    public static void PlayBgm(string key, bool loop = true, float volume = 1.0f)
    {
        var bgm = FindBgm(key);

        if (bgm != null) {
            bgm.Play(loop, volume);
        }
    }

    /// SEの再生
	///
	/// @param [in] key      SE
	/// @param [in] loop     ループさせるか
	/// @param [in] volume   ボリューム
    public static void PlaySound(string key, bool loop = false, float volume = 1.0f, float pan = 0.0f)
    {
        var sound = FindSound(key);

        if (sound != null) {
            sound.Play(loop, volume, pan);
        }
    }

    /// BGMの停止
    public static void StopBgm()
    {
        foreach (string key in bgmTable.Keys) {
            bgmTable[key].Stop();
        }
    }

    /// 指定BGMの停止
	///
	/// @param [in] key      BGM
    public static void StopBgm(string key)
    {
        var bgm = FindBgm(key);

        if (bgm != null) {
            bgm.Stop();
        }
    }

    /// SEの停止
    public static void StopSound()
    {
        foreach (string key in soundTable.Keys) {
            soundTable[key].Stop();
        }
    }

    /// 指定SEの停止
	///
	/// @param [in] key      SE
    public static void StopSound(string key)
    {
        var sound = FindSound(key);

        if (sound != null) {
            sound.Stop();
        }
    }
}

} // Physics2dDemo
