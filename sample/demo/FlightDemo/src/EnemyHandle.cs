/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public
class EnemyHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public EnemyHandle()
    {
    }

    /// デストラクタ
    ~EnemyHandle()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    protected override bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta )
    {
        EnemyUnit myUnit = unit as EnemyUnit;
        PlaneUnit planeUnit = unitMng.Find( "Plane", 0 ) as PlaneUnit;

        myUnit.SetRouteTime( planeUnit.GetRouteTime() );

        if( myUnit.IsHit( planeUnit ) ){
            planeUnit.ForceUncontrollable();
        }
        
        return true;
    }
}

} // end ns FlightDemo
//===
// EOF
//===
