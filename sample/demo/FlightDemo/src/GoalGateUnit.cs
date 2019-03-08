/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public enum GoalGateState{
    Sleep,
    Active
}

public class GoalGateUnit
    : FlightUnit
{
    private GoalGateState state;

    public GoalGateState State{
        get{ return state; }
    }


    public GoalGateUnit()
    : base( new GoalGateHandle(), new GoalGateModel() )
    {
    }

    /// デストラクタ
    ~GoalGateUnit()
    {
    }
    
    protected override bool onStart( FlightUnitManager unitMng )
    {
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

    public void Sleep()
    {
        state = GoalGateState.Sleep;
    }

    public void Active()
    {
        state = GoalGateState.Active;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
