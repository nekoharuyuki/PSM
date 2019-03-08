/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace FlightDemo{

public class FlightPipeUnit
    : FlightUnit
{
    private Vector3 targetPos;

    /// コンストラクタ
    public FlightPipeUnit()
    : base( new FlightPipeHandle(), new FlightPipeModel() )
    {
    }

    /// デストラクタ
    ~FlightPipeUnit()
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng )
    {

        return true;
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

    /// パイプに近寄った時に色を変更するときの対象
    public void SetTargetPos( Vector3 pos )
    {
        targetPos = pos;
    }

    public Vector3 GetTargetPos()
    {
        return targetPos;
    }

}

} // end ns FlightDemo

//===
// EOF
//===
