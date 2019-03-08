/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class GateUnit
    : ItemUnit
{
    public const int PenaltyTime = -5;
    public const int BonusTime   = 5;

    /// コンストラクタ
    public GateUnit( float time )
    : base( time, new ItemHandle(), new GateModel() )
    {
    }

    /// デストラクタ
    ~GateUnit()
    {
    }

    /// ItemUnit.Fail() が呼ばれたときに通知される
    protected override void onFail()
    {
        this.ParentUnitMng.Regist( "Eff", -1, new AddSecEffect( GateUnit.PenaltyTime ) );
        AudioManager.PlaySound( "GateNG" );
    }

    /// ItemUnit.Success() が呼ばれたときに通知される
    protected override void onSuccess()
    {
        this.ParentUnitMng.Regist( "Eff", -1, new AddSecEffect( GateUnit.BonusTime ) );
        AudioManager.PlaySound( "GateOK" );
		PlaneUnit planeUnit = this.ParentUnitMng.Find( "Plane", 0 ) as PlaneUnit;
        planeUnit.EfxGoodA();
    }
}


} // end ns FlightDemo
//===
// EOF
//===
