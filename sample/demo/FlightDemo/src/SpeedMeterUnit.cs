/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using UnitSys;

namespace FlightDemo{

/// 2D表示のスピードメーター
public class SpeedMeterUnit
    : FlightUnit
{
    private PlaneUnit plane;

    /// コンストラクタ
    public SpeedMeterUnit()
    : base( null, new SpeedMeterModel() )
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
        plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;
        return true;
    }
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        plane = null;
        return true;
    }


    public float Speed()
    {
        if( plane != null ){
            return plane.Speed();
        }
        return 0.0f;
    }
}

} // end ns FlightDemo
//===
// EOF
//===
