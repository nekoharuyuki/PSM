/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class GoalGateHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public GoalGateHandle()
    {
    }

    /// デストラクタ
    ~GoalGateHandle()
    {
    }


    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        GoalGateUnit myUnit = unit as GoalGateUnit;
        myUnit.Sleep();

        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    protected override bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta )
    {
        GoalGateUnit myUnit = unit as GoalGateUnit;
        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;
        GameCommonData gameData = unitMng.CommonData as GameCommonData;
        FlightRoute route = gameData.FlightRoute;

        if( plane.GetRouteTime() >= route.GoalTime() - 5.0f ){
            myUnit.Active();
        }

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
