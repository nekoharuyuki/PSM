/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using UnitSys;

namespace FlightDemo{

public class TimerHandle
    : FlightUnitHandle
{
    private RouteState prevRouteState;


    /// コンストラクタ
    public TimerHandle()
    {
    }

    ~TimerHandle()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        prevRouteState = RouteState.StartingWait;
        return true;
    }
    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }
    protected override bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta )
    {
        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;
        TimerUnit myUnit = unit as TimerUnit;
        FlightRoute route = unitMng.GameCommonData.FlightRoute;

        RouteState st = route.GetRouteState( plane.GetRouteTime() );

		// タイムオーバーになってしまっている
		if( myUnit.GetTime() >= TimerUnit.kMaxTime ){
            // タイムオーバー
            if( plane.State != PlaneState.Sleep ) {
                plane.Sleep();
                unitMng.Regist( "2DUI", -1, new Timeover2DUnit() );
            }
		}else{
            // タイムオーバーでない
            if( st == RouteState.StartingWait ){
                plane.Sleep();
            }else if( st == RouteState.EndingWait ){
                myUnit.Hide();
                plane.Sleep();

                // ゴールゲートをくぐった直後
                if( prevRouteState == RouteState.PlayTime ){
                    unitMng.Regist( "2DUI", -1, new Finish2DUnit() );
                }

            }else if( st == RouteState.EndOfRoute ){
                // 終わり
                return false;
            }else{
                // 通常時
                plane.Active();
                myUnit.Add( delta ); // 時間計測
			
                // スタートゲートをくぐった直後
                if( prevRouteState == RouteState.StartingWait ){
                    MotionInputUnit motion = unitMng.Find( "Input", 1 ) as MotionInputUnit;
                    motion.SetBaseAccel();
                }
            }
        }

        prevRouteState = st;

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
