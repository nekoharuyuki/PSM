/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;

namespace AppRpg {


///***************************************************************************
/// 衝突判定用OBJをまとめるコンテナ
///***************************************************************************
public class ActorUnitCollObjParam
{

    public GameObjProduct        Obj;
    public GameActorProduct      ObjParent;

    public int                   ShapeId;
    public int                   PrimId;
    public float                 Dis;
    public bool                  CollFlg;

    public void Clear()
    {
        Obj          = null;
        ObjParent    = null;
        CollFlg      = false;
    }
    public void StartCollision()
    {
        CollFlg      = false;
    }
}


///***************************************************************************
/// 衝突判定用OBJをまとめるコンテナ
///***************************************************************************
public class GameActorCollObjContainer
{

    private List< ActorUnitCollObjParam >    objParamList;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        objParamList = new List< ActorUnitCollObjParam >();
        if( objParamList == null ){
            return false;
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        Clear();

        if( objParamList != null ){
            for( int i=0; i<objParamList.Count; i++ ){
                objParamList[i] = null;
            }
            objParamList.Clear();
        }
        objParamList    = null;
    }

    /// 登録削除
    public void Clear()
    {
        for( int i=0; i<objParamList.Count; i++ ){
            objParamList[i].Clear();
        }
        objParamList.Clear();
    }

    /// OBJの登録
    public bool Add( GameActorProduct parent, GameObjProduct obj )
    {
        objParamList.Add( new ActorUnitCollObjParam() );

        objParamList[(objParamList.Count-1)].ObjParent    = parent;
        objParamList[(objParamList.Count-1)].Obj          = obj;

        return true;
    }

    /// 衝突判定開始
    public void StartCollision()
    {
        for( int i=0; i<objParamList.Count; i++ ){
            objParamList[i].StartCollision();
        }
    }

    /// パラメータのセット
    public void SetCollParam( int entIdx, int shapeId, int primId, float dis )
    {
        objParamList[entIdx].ShapeId    = shapeId;
        objParamList[entIdx].PrimId     = primId;
        objParamList[entIdx].Dis        = dis;
        objParamList[entIdx].CollFlg    = true;
    }


    /// 登録情報の取得
    public GameObjProduct GetEntryObj( int idx )
    {
        return objParamList[idx].Obj;
    }
    public GameActorProduct GetEntryObjParent( int idx )
    {
        return objParamList[idx].ObjParent;
    }
    public int GetEntryShapeId( int idx )
    {
        return objParamList[idx].ShapeId;
    }
    public int GetEntryPrimId( int idx )
    {
        return objParamList[idx].PrimId;
    }
    public float GetEntryDis( int idx )
    {
        return objParamList[idx].Dis;
    }


    /// 衝突情報を近い順にソート
    public void SortNearDis()
    {
        objParamList.Sort( (x, y) => {
                if( x.CollFlg == false ){
                    return 0;
                }
                else if ( y.CollFlg == false || x.Dis < y.Dis) {
                    return -1;
                }
                else if (x.Dis > y.Dis) {
                    return 1;
                }
                else {
                    return 0;
                }
               } );
    }


/// private メソッド
///---------------------------------------------------------------------------


/// プロパティ
///---------------------------------------------------------------------------

    /// 登録数
    public int Num
    {
        get {return objParamList.Count;}
    }
}

} // namespace
