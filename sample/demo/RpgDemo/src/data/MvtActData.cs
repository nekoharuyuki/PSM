/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg { namespace Data {


///***************************************************************************
/// 動作アクションのリソースデータ
///***************************************************************************
public class MvtActData
{
    /// 動作データ構造体
    private struct cDataAct
    {
        public int      cmdId;          // 命令ID
        public float    startFrame;     // アクションの開始フレーム
        public float    endFrame;       // アクションの終了フレーム
        public int[]    Atb;            // アトリビュート（アクションによって使用用途が異なる）
    }

    private cDataAct[]    dataCmdTbl;
    private int           dataCmdMax;
    private int           dataCmdNum;


    /// コンストラクタ
    public MvtActData()
    {
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        dataCmdTbl        = null;
        dataCmdNum        = 0;
        dataCmdMax        = 0;
    }
    /// 破棄
    public void Term()
    {
        if( dataCmdTbl != null ){
            for( int i=0; i<dataCmdMax; i++ ){
                dataCmdTbl[i].Atb    = null;
            }
        }

        dataCmdTbl        = null;
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
        dataCmdTbl        = new cDataAct[max];
        for( int i=0; i<max; i++ ){
            dataCmdTbl[i].Atb    = new int[5];
        }
        dataCmdNum        = 0;
        dataCmdMax        = max;
    }

    /// リソースデータの作成
    public void AddParam( int cmdId, float startFrame, float endFrame,
                          int Atb1 = 0, int Atb2 = 0, int Atb3 = 0, int Atb4 = 0, int Atb5 = 0 )
    {
        dataCmdTbl[dataCmdNum].cmdId            = cmdId;
        dataCmdTbl[dataCmdNum].startFrame       = startFrame;
        dataCmdTbl[dataCmdNum].endFrame         = endFrame;
        dataCmdTbl[dataCmdNum].Atb[0]           = Atb1;
        dataCmdTbl[dataCmdNum].Atb[1]           = Atb2;
        dataCmdTbl[dataCmdNum].Atb[2]           = Atb3;
        dataCmdTbl[dataCmdNum].Atb[3]           = Atb4;
        dataCmdTbl[dataCmdNum].Atb[4]           = Atb5;
        dataCmdNum ++;
    }
    

/// データのパラメータ取得
///---------------------------------------------------------------------------
    
    public int GetCommandNum()
    {
        return dataCmdNum;
    }
    public int GetCommondId( int idx )
    {
        return dataCmdTbl[idx].cmdId;
    }
    public float GetStartFrame( int idx )
    {
        return dataCmdTbl[idx].startFrame;
    }
    public float GetEndFrame( int idx )
    {
        return dataCmdTbl[idx].endFrame;
    }
    public int GetAtb( int idx, int atb )
    {
        return dataCmdTbl[idx].Atb[atb];
    }

}

}} // namespace
