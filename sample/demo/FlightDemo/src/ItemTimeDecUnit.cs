/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class ItemTimeDecUnit
    : ItemUnit
{
    public const int DecreaseTime   = -10;

    /// コンストラクタ
    public ItemTimeDecUnit( float time )
    : base( time, new ItemHandle(), new ItemModel("TIME-DEC") )
    {
    }

    /// デストラクタ
    ~ItemTimeDecUnit()
    {
    }
    
    /// ItemUnit.Success() が呼ばれたときに通知される
    protected override void onSuccess()
    {
        this.ParentUnitMng.Regist( "Eff", -1, new AddSecEffect( ItemTimeDecUnit.DecreaseTime ) );
        AudioManager.PlaySound( "TimeDec" );

        PlaneUnit planeUnit = this.ParentUnitMng.Find( "Plane", 0 ) as PlaneUnit;
        planeUnit.EfxBad();

    }

}


} // end ns FlightDemo
//===
// EOF
//===
