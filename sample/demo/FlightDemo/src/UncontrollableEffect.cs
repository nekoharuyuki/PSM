/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class UncontrollableEffect
    : FlightUnit
{
    public UncontrollableEffect()
    : base( new Effect2DHandle( 2.0f ), new UncontrollableEffectModel() )
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng )
    {
		AudioManager.PlaySound( "Uncontrol" );

        return true;
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
