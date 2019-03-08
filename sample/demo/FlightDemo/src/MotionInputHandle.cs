/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class MotionInputHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public MotionInputHandle()
    {
    }

    /// デストラクタ
    ~MotionInputHandle()
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
        MotionInputUnit myUnit = unit as MotionInputUnit;

        myUnit.Update();

        return true;
    }

    
}

} // end ns FlightDemo
//===
// EOF
//===
