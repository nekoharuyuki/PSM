/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace FlightDemo{

public class ItemSpeedUpUnit
    : ItemUnit
{
    /// コンストラクタ
    public ItemSpeedUpUnit( float time )
    : base( time, new ItemHandle(), new ItemModel("SPEED-UP") )
    {
    }

    /// デストラクタ
    ~ItemSpeedUpUnit()
    {
    }
    
    /// ItemUnit.Success() が呼ばれたときに通知される
    protected override void onSuccess()
    {
        //        this.ParentUnitMng.Regist( "Eff", -1, new AddSecEffect( GateUnit.BonusTime ) );

        PlaneUnit planeUnit = this.ParentUnitMng.Find( "Plane", 0 ) as PlaneUnit;
        if( planeUnit != null ){
            planeUnit.SpeedUp();
        }
        AudioManager.PlaySound( "ItemGood" );
    }

}


} // end ns FlightDemo
//===
// EOF
//===
