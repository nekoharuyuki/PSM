/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class AddSecEffect
    : FlightUnit
{
    private int sec; ///< 加算(減算)する秒数

    /// コンストラクタ
    public AddSecEffect( int sec )
    : base( new Effect2DHandle( 2.0f ), new AddSecEffectModel() )
    {
        this.sec = sec;
    }

    /// デストラクタ
    ~AddSecEffect()
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng )
    {
    	TimerUnit timer = unitMng.Find( "Timer", 0 ) as TimerUnit;
    	timer.Add( -sec );

        return true;
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

    public int GetTime()
    {
        return sec;
    }

}


} // end ns FlightDemo
//===
// EOF
//===
