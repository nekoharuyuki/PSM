/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using UnitSys;

namespace FlightDemo{

public abstract class FlightUnitModel
    : UnitModel
{
    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    public bool OnStart( UnitCommonData gameData, Unit unit )
    {
        return onStart( (GameCommonData)gameData );
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    public bool OnEnd( UnitCommonData gameData, Unit unit )
    {
        return onEnd( (GameCommonData)gameData );
    }

    /// アニメーションの更新処理
    public bool OnUpdate( UnitCommonData gameData, Unit unit, float delta )
    {
        return onUpdate( (GameCommonData)gameData, (FlightUnit)unit, delta );
    }

    /// 描画処理
    public bool OnRender( UnitCommonData gameData, Unit unit )
    {
        return onRender( (GameCommonData)gameData, (FlightUnit)unit );
    }


    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected abstract bool onStart( GameCommonData gameData );

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected abstract bool onEnd( GameCommonData gameData );

    /// アニメーションの更新処理
    protected abstract bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta );

    /// 描画処理
    protected abstract bool onRender( GameCommonData gameData, FlightUnit unit );

}

} // end ns FlightDemo
//===
// EOF
//===
