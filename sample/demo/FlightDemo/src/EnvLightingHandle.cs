/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using UnitSys;

namespace FlightDemo{

public
class EnvLightingHandle
    : UnitHandle
{
    /// コンストラクタ
    public EnvLightingHandle()
    {
    }

    /// デストラクタ
    ~EnvLightingHandle()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    public override bool OnStart( UnitManager unitMng, Unit unit )
    {
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    public override bool OnEnd( UnitManager unitMng, Unit unit )
    {
        return true;
    }

    /// 時間差分更新処理
    public override bool OnUpdate( UnitManager unitMng, Unit unit, float delta )
    {
        EnvLightingUnit lightingUnit = unit as EnvLightingUnit;
        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;

        lightingUnit.Lighting( plane.GetRouteTime() );
        return true;
    }
}

} // end ns FlightDemo

//===
// EOF
//===
