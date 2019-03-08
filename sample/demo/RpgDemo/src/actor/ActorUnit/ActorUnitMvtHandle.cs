/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {


///***************************************************************************
/// 動作再生
///***************************************************************************
public class ActorUnitMvtHandle
{
    private ActorUnitMvtActPlayer  mvtActPlayer;
    private Data.CharParamData     useChParam;

    private Data.MvtData           playMvt;
    private int                    playMvtId;
    private int                    playMvtIdx;
    private int                    playMvtActIdx;
    private int                    playMvtActMax;
    private bool                   playActiveFlg;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        mvtActPlayer    = new ActorUnitMvtActPlayer();

        return( mvtActPlayer.Init() );
    }
    /// 破棄
    public void Term()
    {
        End();

        if( mvtActPlayer != null ){
            mvtActPlayer.Term();
        }
        mvtActPlayer  = null;
        playMvt       = null;
    }


    /// 開始
    public bool Start( ActorUnitCommon useCmn, Data.CharParamData useParam )
    {
        End();
        this.useChParam    = useParam;

        playMvt        = null;
        playMvtId      = -1;
        playMvtIdx     = -1;

        return( mvtActPlayer.Start( useCmn, useParam ) );
    }

    /// 終了
    public void End()
    {
        if( mvtActPlayer != null ){
            mvtActPlayer.End();
        }

        useChParam    = null;
        playMvt       = null;
    }


    /// 動作の再生セット
    public bool SetPlayMvt( int playId, bool updateFlg )
    {
        /// 再生チェック
        int index = getPlayMvtIndex( playId );
        if( index < 0 ){
            return false;
        }

        /// 同じ動作が再生することにった場合に更新するかどうか
        if( updateFlg == false && playActiveFlg == true &&
            playId == playMvtId && index == playMvtIdx ){
            return true;
        }

        setupMvtPlay( playId, index );
        return true;
    }


    /// フレーム処理
    public bool Frame()
    {
        if( playMvtId >= 0 && playActiveFlg ){

            /// アクション再生
            if( mvtActPlayer.Frame() == false ){
                if( playMvtActIdx+1 < playMvtActMax ){

                    playMvtActIdx ++;

                    /// アクション更新
                    playActiveFlg = setMvtActPlay( playMvtId, playMvtIdx, playMvtActIdx );
                }
                else{
                    playActiveFlg = false;
                }
            }
        }

        return playActiveFlg;
    }


    /// 動作再生中かのチェック
    public bool IsActive()
    {
        return playActiveFlg;
    }


/// private メソッド
///---------------------------------------------------------------------------

    /// 動作再生の準備
    private bool setupMvtPlay( int playId, int index )
    {
        playMvt          = useChParam.GetMvt( playId );
        playMvtId        = playId;
        playMvtIdx       = index;
        playMvtActIdx    = 0;
        playMvtActMax    = playMvt.GetActResNum( index );

        /// アクション再生登録
        playActiveFlg    = setMvtActPlay( playId, index, 0 );
        return true;
    }

    /// 動作アクション再生命令
    private bool setMvtActPlay( int playId, int mvtIdx, int actIdx )
    {
        Data.MvtData        mvtRes      = useChParam.GetMvt( playId );
        int                 mvtActId    = mvtRes.GetActResId( mvtIdx, actIdx );

        if( mvtActPlayer.SetPlay( mvtActId, 0 ) == false ){
            return false;
        }

        return true;
    }

    /// 再生チェック
    private int getPlayMvtIndex( int playId )
    {
        Data.MvtData            mvtRes    = useChParam.GetMvt( playId );
        if( mvtRes.Num <= 0 ){
            return -1;
        }

        return 0;
    }




/// プロパティ
///---------------------------------------------------------------------------

}

} // namespace
