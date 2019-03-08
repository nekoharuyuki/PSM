/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;

namespace AppRpg {


///***************************************************************************
/// アクタ間のイベントID
///***************************************************************************

public enum ActorEventId {
    Effect = 0,      /// エフェクト再生
    Damage,          /// ダメージを受ける
    LookTrg,         /// 対象に向く
    TurnTrg,         /// 対象に振り向く
    SuperArm,        /// スーパーアーマー化
    MvtCancel,       /// 動作キャンセル
}



///***************************************************************************
/// アクタ間のイベントパラメータ
///***************************************************************************
public class GameActorEventParam
{
    public ActorEventId          EventId;
    public int                   EventAtb;
    public GameObjProduct        TrgObj;
    public Vector3               TrgPos;

    public void Clear()
    {
        TrgObj = null;
    }
}



///***************************************************************************
/// アクタ間のイベントコンテナ
///***************************************************************************
public class GameActorEventContainer
{
    private const int entryMax = 10;
    private List< GameActorEventParam >    eveParamList;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        eveParamList = new List< GameActorEventParam >();
        if( eveParamList == null ){
            return false;
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        Clear();

        if( eveParamList != null ){
            for( int i=0; i<eveParamList.Count; i++ ){
                eveParamList[i] = null;
            }
            eveParamList.Clear();
        }
        eveParamList    = null;
    }

    /// 登録削除
    public void Clear()
    {
        for( int i=0; i<eveParamList.Count; i++ ){
            eveParamList[i].Clear();
        }
        eveParamList.Clear();
    }

    /// イベントの登録
    public bool Add( ActorEventId eveId, int eveAtb, GameObjProduct obj )
    {
        /// 登録許容量OVER
        if( eveParamList.Count >= entryMax ){
            return false;
        }

        eveParamList.Add( new GameActorEventParam() );

        eveParamList[(eveParamList.Count-1)].EventId    = eveId;
        eveParamList[(eveParamList.Count-1)].EventAtb   = eveAtb;
        eveParamList[(eveParamList.Count-1)].TrgObj     = obj;
        return true;
    }

    /// イベントの登録
    public bool Add( ActorEventId eveId, int eveAtb, Vector3 pos )
    {
        /// 登録許容量OVER
        if( eveParamList.Count >= entryMax ){
            return false;
        }

        eveParamList.Add( new GameActorEventParam() );

        eveParamList[(eveParamList.Count-1)].EventId    = eveId;
        eveParamList[(eveParamList.Count-1)].EventAtb   = eveAtb;
        eveParamList[(eveParamList.Count-1)].TrgPos     = pos;
        return true;
    }


    /// 登録情報の取得
    public ActorEventId GetEventId( int idx )
    {
        return eveParamList[idx].EventId;
    }
    public int GetEntryAtb( int idx )
    {
        return eveParamList[idx].EventAtb;
    }
    public GameObjProduct GetEntryObj( int idx )
    {
        return eveParamList[idx].TrgObj;
    }
    public Vector3 GetTrgPos( int idx )
    {
        return eveParamList[idx].TrgPos;
    }




/// private メソッド
///---------------------------------------------------------------------------


/// プロパティ
///---------------------------------------------------------------------------

    /// 登録数
    public int Num
    {
        get {return eveParamList.Count;}
    }
}

} // namespace
