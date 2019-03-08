/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class Finish2DUnit
    : FlightUnit
{
    /// コンストラクタ
    public Finish2DUnit()
    : base( new Finish2DHandle(), new Finish2DModel() )
    {
    }

    // デストラクタ
    ~Finish2DUnit()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
    	AudioManager.StopBgm();
    	AudioManager.PlayBgm( "Finish", false );
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

}


} // end ns FlightDemo
//===
// EOF
//===
