/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class Finish2DHandle
    : FlightUnitHandle
{
    private float time;

    /// コンストラクタ
    public Finish2DHandle()
    {
    }

    /// デストラクタ
    ~Finish2DHandle()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        time = 0.0f;
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    protected override bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta )
    {
        time += delta;

        if( time >= 3.0f ){
            unitMng.Remove( unit );
            unitMng.Regist( "2DUI", -1, new FinishTimeUnit() );
        }
        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
