/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg { namespace Data {

///***************************************************************************
/// キャラパラメータデータ
///***************************************************************************
public class CharParamData
{
    private MvtData[]           mvtTbl;
    private MvtActData[]        actTbl;
    private int                 mvtEntryMax;
    private int                 actTblEntryMax;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        mvtTbl            = null;
        actTbl            = null;
        mvtEntryMax       = 0;
        actTblEntryMax    = 0;
        return true;
    }

    /// 破棄
    public void Term()
    {
        if( mvtTbl != null ){
            for( int i=0; i<mvtEntryMax; i++ ){
                mvtTbl[i].Term();
                mvtTbl[i]    = null;
            }
        }

        if( actTbl != null ){
            for( int i=0; i<actTblEntryMax; i++ ){
                actTbl[i].Term();
                actTbl[i]    = null;
            }
        }
        mvtTbl    = null;
        actTbl    = null;
    }


    /// リソースデータの読み込み
    /**
     * 今回は使用する予定なし
     */
    public bool Load( string filename )
    {
        return false;
    }


    /// リソースデータの作成
    public void Make( int mvtMax, int actMax )
    {
        mvtTbl        = new MvtData[mvtMax];
        for( int i=0; i<mvtMax; i++ ){
            mvtTbl[i]    = new MvtData();
            mvtTbl[i].Init();
        }

        actTbl        = new MvtActData[actMax];
        for( int i=0; i<actMax; i++ ){
            actTbl[i]    = new MvtActData();
            actTbl[i].Init();
        }

        mvtEntryMax       = mvtMax;
        actTblEntryMax    = actMax;
    }


    /// 動作データの取得
    public MvtData GetMvt( int idx )
    {
        return mvtTbl[idx];
    }

    /// 動作アクションデータの取得
    public MvtActData GetMvtAct( int idx )
    {
        return actTbl[idx];
    }



/// private メソッド
///---------------------------------------------------------------------------

/// プロパティ
///---------------------------------------------------------------------------

}

}} // namespace
