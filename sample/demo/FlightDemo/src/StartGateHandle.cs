/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public
class StartGateHandle
    : FlightUnitHandle
{
	private float prevRouteTime;

    /// コンストラクタ
    public StartGateHandle()
    {
    }

    /// デストラクタ
    ~StartGateHandle()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
    	prevRouteTime = 0.0f;
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
        StartGateUnit myUnit = unit as StartGateUnit;
        FlightRoute route = unitMng.GameCommonData.FlightRoute;

        PlaneUnit planeUnit = unitMng.Find( "Plane", 0 ) as PlaneUnit;
				
		var currentRouteTime = planeUnit.GetRouteTime();

        float countTime03 = route.StartTime() - FlightRoute.SecToRouteSec( 3.0f );
        float countTime02 = route.StartTime() - FlightRoute.SecToRouteSec( 2.0f );
        float countTime01 = route.StartTime() - FlightRoute.SecToRouteSec( 1.0f );
        float countTime00 = route.StartTime() - FlightRoute.SecToRouteSec( 0.0f );

		if( prevRouteTime < countTime03 ){
			if( countTime03 <= currentRouteTime ){
				AudioManager.PlaySound( "Count" );
                myUnit.SetIndex( 2 );
			}
		}else if( prevRouteTime < countTime02 ){
			if( countTime02 <= currentRouteTime ){
				AudioManager.PlaySound( "Count" );
                myUnit.SetIndex( 1 );
			}
		}else if( prevRouteTime < countTime01 ){
			if( countTime01 <= currentRouteTime ){
				AudioManager.PlaySound( "Count" );
                myUnit.SetIndex( 0 );
			}
		}else if( prevRouteTime < countTime00 ){
			if( countTime00 <= currentRouteTime ){
				AudioManager.PlaySound( "Start" );
			}
		}else{
            myUnit.SetIndex( 3 );
        }

		prevRouteTime = currentRouteTime;
			
        if( route.StartTime() < currentRouteTime ){
            unitMng.Remove( unit );
        }

        return true;
    }


}

} // end ns FlightDemo

//===
// EOF
//===
