/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class Timeover2DUnit
    : FlightUnit
{
    /// コンストラクタ
    public Timeover2DUnit()
    : base( new Timeover2DHandle(), new Timeover2DModel() )
    {
    }

    // デストラクタ
    ~Timeover2DUnit()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
    	AudioManager.StopBgm();
    	AudioManager.PlayBgm( "TimeOver", false );
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
