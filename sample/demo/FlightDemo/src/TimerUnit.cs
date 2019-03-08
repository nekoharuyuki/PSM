/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


namespace FlightDemo{

public
class TimerUnit
    : FlightUnit
{
    private float time;
    //    public const float kMaxTime = 600.0f; // 10min=600sec(暫定的に10分としている)
    public const float kMaxTime = 120.0f; // 2min=600sec
    private bool isShow;

    /// コンストラクタ
    public TimerUnit()
    : base( new TimerHandle(), new TimerModel() )
    {
    }

    ~TimerUnit()
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng )
    {
        isShow = true;
        time = 0.0f;
        return true;
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

    public void Add( float delta )
    {
        time += delta;
        if( time > kMaxTime ) time = kMaxTime;
    }

    public float GetTime()
    {
        return time;
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
		return (int)((kMaxTime - time) / 60.0f);
	}
	/// 残り秒の取得
	public int RemainSec()
	{
		return (int)((kMaxTime - time) % 60.0f);
	}
	/// 残り10ミリ秒の取得
	public int RemainTMsec()
	{
		return (int)(((kMaxTime - time) * 100.0f) % 100.0f);
	}

    // 表示を隠す
    public void Hide()
    {
        isShow = false;
    }

    /// 表示を行う
    public void Show()
    {
        isShow = true;
    }

    /// 表示を行うか？
    public bool IsShow()
    {
        return isShow;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
