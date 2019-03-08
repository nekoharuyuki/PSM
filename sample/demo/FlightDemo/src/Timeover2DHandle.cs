/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class Timeover2DHandle
    : FlightUnitHandle
{
    private float time;

    /// コンストラクタ
    public Timeover2DHandle()
    {
    }

    /// デストラクタ
    ~Timeover2DHandle()
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

        if( time >= 7.0f ){
        	return false;
        }
        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
