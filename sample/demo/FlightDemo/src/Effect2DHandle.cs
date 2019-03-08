/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public
class Effect2DHandle
    : FlightUnitHandle
{
    private float time;

    /// コンストラクタ
    public Effect2DHandle( float drawTime )
    {
        ;
    }

    /// デストラクタ
    ~Effect2DHandle()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// 処理の更新
    protected override bool onUpdate( FlightUnitManager unitMng,
                                 FlightUnit unit, 
                                 float delta )
    {
        time += delta;

        if( time >= 2.0f ){
            unitMng.Remove( unit );
        }

        return true;
    }
}

} // end ns FlightDemo
//===
// EOF
//===
