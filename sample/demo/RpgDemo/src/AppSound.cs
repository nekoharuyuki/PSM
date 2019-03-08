/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Audio;


namespace AppRpg {


///***************************************************************************
/// サウンド再生と管理
///***************************************************************************
public class AppSound
{
    ///---------------------------------------------------------------------------
    /// BGMとSEの再生ID
    ///---------------------------------------------------------------------------
    public enum BgmId{
        Main = 0,
        Clear,
        Gameover,
        Max
    };
    public enum SeId{
        PlDamage = 0,
        PlAtk,
        PlSpelAtk,
        PlFoot,
        EnHit,
        EnSpelHit,
        EnDead,
        EnFly,
        ObjBreak,
        Max
    };

    private static AppSound instance = new AppSound();

    private Bgm[]          bgmList;
    private BgmPlayer      bgmPlayer;
    private Sound[]        seList;
    private SoundPlayer[]  sePlayer;

    /// インスタンスの取得
    public static AppSound GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        bgmList = new Bgm[(int)BgmId.Max];
        bgmList[(int)BgmId.Main]        = new Bgm("/Application/res/data/Sound/bgm_001.mp3");
        bgmList[(int)BgmId.Clear]       = new Bgm("/Application/res/data/Sound/jingle_clear.mp3");
        bgmList[(int)BgmId.Gameover]    = new Bgm("/Application/res/data/Sound/jingle_gameover.mp3");

        seList = new Sound[(int)SeId.Max];
        seList[(int)SeId.EnDead]        = new Sound("/Application/res/data/Sound/enemy_dead.wav");
        seList[(int)SeId.EnFly]         = new Sound("/Application/res/data/Sound/enemy_fly.wav");
        seList[(int)SeId.EnHit]         = new Sound("/Application/res/data/Sound/enemy_hit.wav");
        seList[(int)SeId.EnSpelHit]     = new Sound("/Application/res/data/Sound/enemy_m_hit.wav");
        seList[(int)SeId.PlAtk]         = new Sound("/Application/res/data/Sound/player_attack.wav");
        seList[(int)SeId.PlSpelAtk]     = new Sound("/Application/res/data/Sound/player_m_attack.wav");
        seList[(int)SeId.PlDamage]      = new Sound("/Application/res/data/Sound/player_damage.wav");
        seList[(int)SeId.PlFoot]        = new Sound("/Application/res/data/Sound/player_foot.wav");
        seList[(int)SeId.ObjBreak]      = new Sound("/Application/res/data/Sound/obj_break.wav");


        sePlayer = new SoundPlayer[(int)SeId.Max];
        for( int i=0; i<(int)SeId.Max; i++ ){
            sePlayer[i] = seList[i].CreatePlayer();
        }
        bgmPlayer = null;

        return true;
    }

    /// 破棄
    public void Term()
    {
        StopBgm();
        for( int i=0; i<(int)BgmId.Max; i++ ){
            bgmList[i].Dispose();
            bgmList[i] = null;
        }
        for( int i=0; i<(int)SeId.Max; i++ ){
            if( sePlayer[i].Status == SoundStatus.Playing ){
                sePlayer[i].Stop();
            }
            sePlayer[i] = null;

            seList[i].Dispose();
            seList[i] = null;
        }
        bgmList   = null;
        seList    = null;
    }

    /// BGMの再生
    public void PlayBgm( BgmId id, bool loop )
    {
        StopBgm();

        bgmPlayer = bgmList[(int)id].CreatePlayer();
        bgmPlayer.Loop = loop;
        bgmPlayer.Play();
        bgmPlayer.Volume = 0.5f;
    }

    /// BGMの停止
    public void StopBgm()
    {
        if( bgmPlayer != null ){
            if( bgmPlayer.Status == BgmStatus.Playing ){
                bgmPlayer.Stop();
            }
            bgmPlayer.Dispose();
        }
        bgmPlayer = null;
    }

    /// SEの再生
    public void PlaySe( SeId id )
    {
        sePlayer[(int)id].Play();
        sePlayer[(int)id].Volume = 1.0f;
    }

    /// SEの再生（カメラからの距離に応じて音量が変化）
    public void PlaySeCamDis( SeId id, Vector3 pos )
    {
        float dis = Common.VectorUtil.Distance( pos, GameCtrlManager.GetInstance().CtrlCam.GetCamPos() );

        float vol = 1.0f;
        if( dis > 8.0f ){
            vol = 1.0f - (dis-8.0f) / 32.0f;
            if( vol < 0.1f ){
                vol = 0.1f;
            }
        }

        sePlayer[(int)id].Play();
        sePlayer[(int)id].Volume = vol;
    }

    /// BGMが再生中か調べる
    public bool IsBgmPlaing()
    {
        if( bgmPlayer.Status == BgmStatus.Playing ){
            return true;
        }
        return false;
    }

}

} // namespace
