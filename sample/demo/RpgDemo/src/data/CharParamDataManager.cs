/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;


namespace AppRpg { namespace Data {

///***************************************************************************
/// キャラクタパラメータデータのコンテナ
///***************************************************************************
public class CharParamDataManager
{
    private CharParamData[]    dataTbl;
    private int                dataEntryMax;


    /// インスタンスの取得
    private static CharParamDataManager instance = new CharParamDataManager();

    public static CharParamDataManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init( int max )
    {
        dataTbl           = new CharParamData[max];
        for( int i=0; i<max; i++ ){
            dataTbl[i]    = new CharParamData();
            dataTbl[i].Init();
        }

        this.dataEntryMax = max;
        return true;
    }

    /// 破棄
    public void Term()
    {
        if( dataTbl != null ){
            for( int i=0; i<dataEntryMax; i++ ){
                dataTbl[i].Term();
                dataTbl[i]    = null;
            }
        }

        dataTbl    = null;
    }


    /// キャラクタパラメータデータの取得
    public CharParamData GetData( int idx )
    {
        return dataTbl[idx];
    }



/// private メソッド
///---------------------------------------------------------------------------

/// プロパティ
///---------------------------------------------------------------------------

}

}} // namespace
