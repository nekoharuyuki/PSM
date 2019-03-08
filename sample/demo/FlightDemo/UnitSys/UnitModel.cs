/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


namespace UnitSys{

public interface UnitModel
{
    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    bool OnStart( UnitCommonData commonData, Unit unit );

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    bool OnEnd( UnitCommonData commonData, Unit unit );

    /// アニメーションの更新処理
    bool OnUpdate( UnitCommonData gameData, Unit unit, float delta );

    /// 描画処理
    bool OnRender( UnitCommonData commonData, Unit unit );
}

} // end ns FlightDemo
//===
// EOF
//===
