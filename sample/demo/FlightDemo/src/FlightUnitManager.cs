/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using UnitSys;

namespace FlightDemo{

public
class FlightUnitManager
    : UnitManager
{
    public GameCommonData GameCommonData{
        get{ return (GameCommonData)base.commonData; }
    }

    /// コンストラクタ
    public FlightUnitManager( GameCommonData gameData )
    : base( gameData )
    {
    }
}

}

//===
// EOF
//===
