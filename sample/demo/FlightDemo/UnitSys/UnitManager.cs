/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;


namespace UnitSys{

public class UnitData
{
    private string groupName;
    public int id;
    public Unit unit;

    public UnitData( string groupName, int id, Unit unit )
    {
        this.groupName = groupName;
        this.id = id;
        this.unit = unit;
    }

    public string GroupName
    {
        get{ return groupName; }
    }

    public int ID
    {
        get{ return id; }
    }

    public Unit Unit
    {
        get{ return unit; }
    }
}

/// Unit へメッセージを送るためのマネージャ
public class UnitManager
{
    public UnitCommonData commonData;
    private List< UnitData > unitList = new List< UnitData >();
    private Dictionary< string, int > groupHash = new Dictionary< string, int >();

    /// コンストラクタ
    public UnitManager( UnitCommonData commonData )
    {
        this.commonData = commonData;
    }

    /// デストラクタ
    ~UnitManager()
    {
    }

    public UnitCommonData CommonData{
        get{ return commonData; }
    }

    /// Unit の登録
    /**
     * Group + ID が一意になる
     */
    public UnitData Regist( string groupName, int id, Unit unit )
    {
        if( id < 0 ){
            if( groupHash.ContainsKey( groupName ) ){
                id = groupHash[ groupName ];
                id++;
            }else{
                id = 0;
            }
        }

        if( Find( groupName, id ) == null ){
            groupHash[ groupName ] = id;
            UnitData data = new UnitData( groupName, id, unit );
            unitList.Add( data );
            unit.Start( this );
            return data;
        }

        return null;
    }

    /// Unit から登録を削除
    public void Remove( Unit removeUnit )
    {
        foreach( UnitData unitData in unitList ){
            if( unitData.Unit == removeUnit ){
                if( unitData.ID == groupHash[ unitData.GroupName ] ){
                    groupHash[ unitData.GroupName ]--;
                }
                unitList.Remove( unitData );
                unitData.Unit.End( this );
                break;
            }
        }
        
        //removeUnit.End( this );
    }

    public List<Unit> FindByGroup( string groupName )
    {
        List<Unit> list = new List<Unit>();

        foreach( UnitData unitData in unitList ){
            if( unitData.GroupName == groupName ){
                list.Add( unitData.Unit );
            }
        }
        return list;
    }

    public Unit Find( string groupName, int id )
    {
        foreach( UnitData unitData in unitList ){
            if( unitData.GroupName == groupName && unitData.ID == id ){
                return unitData.Unit;
            }
        }
        return null;
    }

    /// 時間差分更新
    public bool Update( float delta )
    {
        for( int i = 0; i < unitList.Count; i++ ){
            UnitData unitData = unitList[ i ];
            if( unitData.Unit.Update( this, delta ) == false ){
                return false;
            }
            if( unitList.IndexOf( unitData ) < 0 ){
                i--;
                if( i < 0 ){
                    break;
                }
            }
        }
        //        foreach( UnitData unitData in unitList ){
        //            unitData.Unit.Update( this, gameGraph, unitCommonData, delta );
        //        }
        return true;
    }

    /// 描画処理
    public bool Render()
    {
        foreach( UnitData unitData in unitList ){
            if( unitData.Unit.Render( this ) == false ){
                return false;
            }
        }

        return true;
    }
}

} // end ns FlightDemo

//===
// EOF
//===
