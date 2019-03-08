/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class FinishTimeUnit
    : FlightUnit
{
    private float time;
    
    /// コンストラクタ
    public FinishTimeUnit()
    : base( new FinishTimeHandle(), new FinishTimeModel() )
    {
    }

    // デストラクタ
    ~FinishTimeUnit()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
        TimerUnit timer = unitMng.Find( "Timer", 0 ) as TimerUnit;

        time = timer.GetTime();

        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

    /// 分の取得
    public int Min()
    {
        return (int)(time / 60.0f);
    }

    /// 秒の取得
    public int Sec()
    {
        return (int)(time % 60.0f);
    }

    /// 10ミリ秒の取得
    public int TMsec()
    {
        return (int)((time * 100.0f) % 100.0f);

    }

	/// 残り分の取得
	public int RemainMin()
	{
		return (int)((TimerUnit.kMaxTime - time) / 60.0f);
	}
	/// 残り秒の取得
	public int RemainSec()
	{
		return (int)((TimerUnit.kMaxTime - time) % 60.0f);
	}
	/// 残り10ミリ秒の取得
	public int RemainTMsec()
	{
		return (int)(((TimerUnit.kMaxTime - time) * 100.0f) % 100.0f);
	}
}


} // end ns FlightDemo
//===
// EOF
//===
