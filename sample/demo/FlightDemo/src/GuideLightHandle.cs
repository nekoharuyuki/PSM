/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using UnitSys;

namespace FlightDemo{

public class GuideLightHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public GuideLightHandle()
    {
    }

    /// デストラクタ
    ~GuideLightHandle()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// 時間差分更新処理
    protected override bool onUpdate( FlightUnitManager unitMng,
                                      FlightUnit unit,
                                      float delta )
    {
        GameCommonData commonData = (GameCommonData)unitMng.CommonData;
        GuideLightUnit guide = (GuideLightUnit)unit;

        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;
        
        guide.SetBaseTime( commonData.FlightRoute, plane.GetRouteTime() );

        

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
