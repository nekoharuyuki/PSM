/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class ItemTimeIncUnit
    : ItemUnit
{
    public const int IncreaseTime   = 10;

    /// コンストラクタ
    public ItemTimeIncUnit( float time )
    : base( time, new ItemHandle(), new ItemModel("TIME-ADD") )
    {
    }

    /// デストラクタ
    ~ItemTimeIncUnit()
    {
    }
    
    /// ItemUnit.Success() が呼ばれたときに通知される
    protected override void onSuccess()
    {
        this.ParentUnitMng.Regist( "Eff", -1, new AddSecEffect( ItemTimeIncUnit.IncreaseTime ) );
        AudioManager.PlaySound( "TimeInc" );

        PlaneUnit planeUnit = this.ParentUnitMng.Find( "Plane", 0 ) as PlaneUnit;
        planeUnit.EfxGoodB();
    }

}


} // end ns FlightDemo
//===
// EOF
//===
