/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using UnitSys;

namespace FlightDemo{

public abstract class FlightUnitHandle
    : UnitHandle
{
    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    public override bool OnStart( UnitManager unitMng, Unit unit )
    {
        return onStart( (FlightUnitManager)unitMng, (FlightUnit)unit );
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    public override bool OnEnd( UnitManager unitMng, Unit unit )
    {
        return onEnd( (FlightUnitManager)unitMng, (FlightUnit)unit );
    }

    /// 時間差分更新処理
    public override bool OnUpdate( UnitManager unitMng, Unit unit, float delta )
    {
        return onUpdate( (FlightUnitManager)unitMng, (FlightUnit)unit, delta );
    }

    protected abstract bool onStart( FlightUnitManager unitMng, FlightUnit unit );
    protected abstract bool onEnd( FlightUnitManager unitMng, FlightUnit unit );
    protected abstract bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta );

}

} // end ns FlightDemo
//===
// EOF
//===
