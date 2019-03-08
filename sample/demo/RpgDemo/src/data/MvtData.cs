/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg { namespace Data {


///***************************************************************************
/// 動作リソースデータ
///***************************************************************************
public class MvtData
{
    private struct cData
    {
        /// 再生条件などがないため、内容はアクションのテーブルのみとなっています
        public int[]       ActResId;        // アクションデータの参照
        public int         ActResNum;
    }

    private cData[]        dataTbl;
    private int            dataMax;
    private int            dataNum;


    /// コンストラクタ
    public MvtData()
    {
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        dataTbl        = null;
        dataMax        = 0;
        dataNum        = 0;
    }
    /// 破棄
    public void Term()
    {
        if( dataTbl != null ){
            for( int i=0; i<dataMax; i++ ){
                dataTbl[i].ActResId    = null;
            }
        }
        dataTbl        = null;
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
    public void Make( int max )
    {
        dataTbl            = new cData[max];

        for( int i=0; i<dataMax; i++ ){
            dataTbl[i].ActResId    = null;
            dataTbl[i].ActResNum   = 0;
        }
        
        dataNum        = 0;
        dataMax        = max;
    }

    /// 動作アクションのパラメータ作成
    public int AddParam( int idx, int actResMax )
    {
        dataTbl[idx].ActResId    = new int[actResMax];
        dataTbl[idx].ActResNum   = 0;
        dataNum ++;
        return dataNum;
    }

    /// 動作アクションのパラメータを登録
    public void SetParamAddActionRes( int idx, int actResId )
    {
        dataTbl[idx].ActResId[dataTbl[idx].ActResNum]    = actResId;
        dataTbl[idx].ActResNum ++;
    }


/// データのパラメータ取得
///---------------------------------------------------------------------------
    
    public int GetActResId( int idx, int actIdx )
    {
        return dataTbl[idx].ActResId[actIdx];
    }
    public int GetActResNum( int idx )
    {
        return dataTbl[idx].ActResNum;
    }



/// プロパティ
///---------------------------------------------------------------------------

    /// 登録数の取得
    public int Num
    {
        get {return dataNum;}
    }


}

}} // namespace
