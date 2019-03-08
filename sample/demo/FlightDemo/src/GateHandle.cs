/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace FlightDemo{

public
class GateHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public GateHandle()
    {
    }
    
    /// デストラクタ
    ~GateHandle()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        GateUnit gateUnit = unit as GateUnit;

        gateUnit.Sleep();

        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    protected override bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta )
    {
        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;
        GateUnit gateUnit = unit as GateUnit;

        switch( gateUnit.State ){
          case GateState.Sleep:
            if( gateUnit.BaseTime - plane.GetRouteTime() <= 10.0f ){
                gateUnit.Active();
            }
            break;
          case GateState.Normal:
            return onUpdateNormal( unitMng, gateUnit, delta );
          case GateState.Success:
          case GateState.Fail:
            break;
          case GateState.Destroy:
            unitMng.Remove( gateUnit );
            break;
        }
        return true;
    }

    private bool onUpdateNormal( FlightUnitManager unitMng, GateUnit gateUnit, float delta )
    {
        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;

        // 正しく通ることができた
        if( gateUnit.IsHit( plane ) ){
            gateUnit.Success();
            unitMng.Regist( "Eff", -1, new AddSecEffect( GateUnit.BonusTime ) );
        }

        // 通り過ぎてしまった
        if( gateUnit.BaseTime <= plane.GetRouteTime() ){
            gateUnit.Fail();
            unitMng.Regist( "Eff", -1, new AddSecEffect( GateUnit.PenaltyTime ) );
        }


        return true;
    }
    
}

}
//===
// EOF
//===
