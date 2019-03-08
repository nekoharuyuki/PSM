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
/// 画面効果
///***************************************************************************
public class AppDispEff
{
    private static AppDispEff instance = new AppDispEff();
        
    ///---------------------------------------------------------------------------
    /// エフェクトID
    ///---------------------------------------------------------------------------
    public enum EffId{
        Non = 0,
        FadeIn,          /// フェードイン
        FadeOut,         /// フェードアウト
        FadeWait,        /// フェードアウト待機中
    };

    private EffId       effId;
    private int         fadeSpeed;
    private uint        fadeColor;
    private int         fadeAlpha;
    private bool        effActiveFlg;


    /// インスタンスの取得
    public static AppDispEff GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        effId           = EffId.Non;
        effActiveFlg    = false;
        return true;
    }

    /// 破棄
    public void Term()
    {
    }

    /// フレーム処理
    public bool Frame()
    {
        if( effId == EffId.Non ){
            effActiveFlg = false;
            return false;
        }

        switch( effId ){
        case EffId.FadeIn:
            fadeAlpha -= fadeSpeed;
            if( fadeAlpha < 0 ){
                effId = EffId.Non;
            }
            break;

        case EffId.FadeOut:
            fadeAlpha += fadeSpeed;
            if( fadeAlpha > 0xff ){
                fadeAlpha = 0xff;
                effId = EffId.FadeWait;
                effActiveFlg = true;
            }
            break;
        }

        return !effActiveFlg;
    }

    /// 描画
    public void Draw( DemoGame.GraphicsDevice useGraphDev )
    {
        if( effId == EffId.Non ){
            return ;
        }

        switch( effId ){
        case EffId.FadeIn:
        case EffId.FadeOut:
        case EffId.FadeWait:
            uint col = fadeColor | ((uint)fadeAlpha << 24);
            DemoGame.Graphics2D.FillRect( col, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );
            break;
        }
    }


    /// フェードインのセット
    public void SetFadeIn( uint col, int speed, bool active )
    {
        effId            = EffId.FadeIn;
        fadeColor        = col;
        fadeAlpha        = 0xff;
        fadeSpeed        = speed;
        effActiveFlg     = active;
    }

    /// フェードアウトのセット
    public void SetFadeOut( uint col, int speed, bool active )
    {
        effId            = EffId.FadeOut;
        fadeColor        = col;
        fadeAlpha        = 0x0;
        fadeSpeed        = speed;
        effActiveFlg     = active;
    }

/// プロパティ
///---------------------------------------------------------------------------

    /// 演出IDの取得
    public EffId NowEffId
    {
        get{return effId;}
    }
}

} // namespace
