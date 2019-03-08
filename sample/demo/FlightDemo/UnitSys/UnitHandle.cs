/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace UnitSys{

public abstract class UnitHandle
{
    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    public abstract bool OnStart( UnitManager unitMng, Unit unit );

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    public abstract bool OnEnd( UnitManager unitMng, Unit unit );

    /// 時間差分更新処理
    public abstract bool OnUpdate( UnitManager unitMng, Unit unit, float delta );
}

} // end ns FlightDemo

//===
// EOF
//===

