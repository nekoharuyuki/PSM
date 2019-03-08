/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
namespace FlightDemo{

public 
class FlightPipeHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public FlightPipeHandle()
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
        FlightPipeUnit myUnit = unit as FlightPipeUnit;
        FlightRoute route = unitMng.GameCommonData.FlightRoute;

        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;

        
        myUnit.SetTargetPos( plane.GetPos() );
        myUnit.SetPosture( route.BasePosture( plane.GetRouteTime() + 0.25f ) );

        return true;
    }

}


} // end ns FlightDemo
//===
// EOF
//===
