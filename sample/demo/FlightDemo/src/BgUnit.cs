/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class BgUnit
    : FlightUnit
{
    /// コンストラクタ
    public BgUnit()
    : base( null, new BgModel() )
    {
    }

    /// デストラクタ
    ~BgUnit()
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng )
    {
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
