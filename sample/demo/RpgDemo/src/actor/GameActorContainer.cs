/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Collections.Generic;

namespace AppRpg {


///***************************************************************************
/// アクターコンテナ
///***************************************************************************
public class GameActorContainer
{
    private List< GameActorProduct >    actorList;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorList = new List< GameActorProduct >();
        if( actorList == null ){
            return false;
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        Clear();
        actorList    = null;
    }

    /// 登録削除
    public void Clear()
    {
        if( actorList != null ){
            for( int i=0; i<actorList.Count; i++ ){
                actorList[i] = null;
            }
            actorList.Clear();
        }
    }

    /// イベントの登録
    public bool Add( GameActorProduct actor )
    {
        actorList.Add( actor );
        return true;
    }


    /// 登録情報の取得
    public GameActorProduct GetActor( int idx )
    {
        return actorList[idx];
    }




/// private メソッド
///---------------------------------------------------------------------------


/// プロパティ
///---------------------------------------------------------------------------

    /// 登録数
    public int Num
    {
        get {return actorList.Count;}
    }
}

} // namespace
