/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class StartGateUnit
    : FlightUnit
{
    private int idx;

    public StartGateUnit()
    : base( new StartGateHandle(), new StartGateModel() )
    {
    }

    /// デストラクタ
    ~StartGateUnit()
    {
    }
    
    protected override bool onStart( FlightUnitManager unitMng )
    {
        idx = 3;
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }


    public void SetIndex( int idx )
    {
        this.idx = idx;
    }

    public int GetIndex()
    {
        return idx;
    }
}

} // end ns FlightDemo
//===
// EOF
//===
