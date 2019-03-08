/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using UnitSys;

namespace FlightDemo{

public class AltimeterUnit
    : FlightUnit
{
    private PlaneUnit plane;

    /// コンストラクタ
    public AltimeterUnit()
    : base( null, new AltimeterModel() )
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


    /// 高度の取得
    public float HighDegree()
    {
        if( plane != null ){
            return plane.HighDegree();
        }
        return 0.0f;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
