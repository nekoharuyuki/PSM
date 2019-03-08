/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace FlightDemo{

public class ItemHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public ItemHandle()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        ItemUnit itemUnit = unit as ItemUnit;

        itemUnit.Sleep();

        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// 時間差分更新処理
    protected override bool onUpdate( FlightUnitManager unitMng,
                                      FlightUnit unit,
                                      float delta )
    {
        ItemUnit itemUnit = unit as ItemUnit;
        PlaneUnit planeUnit = unitMng.Find( "Plane", 0 ) as PlaneUnit;

        switch( itemUnit.State ){
          case ItemState.Sleep:
            if( itemUnit.BaseTime - planeUnit.GetRouteTime() <= 5.0f ){
                itemUnit.Active();
            }
            break;
          case ItemState.Normal:
            if( planeUnit.State != PlaneState.Sleep ){
                // 正しく通ることができた
                if( itemUnit.IsHit( planeUnit ) ){
                    itemUnit.Success();
                }
                // 通り過ぎてしまった
                if( itemUnit.BaseTime <= planeUnit.GetRouteTime() ){
                    itemUnit.Fail();
                }
            }
            break;
          case ItemState.Success:
          case ItemState.Fail:
            break;
          case ItemState.Destroy:
            break;
        }

        return true;
    }
}


} // end ns FlightDemo
//===
// EOF
//===
