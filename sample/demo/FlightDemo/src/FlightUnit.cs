/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using UnitSys;

namespace FlightDemo{

public abstract class FlightUnit
    : Unit
{
    protected Matrix4 posture = Matrix4.Identity;
    protected Vector3 pos = new Vector3();

    public FlightUnit( FlightUnitHandle handle, FlightUnitModel model )
    : base( handle, model )
    {
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    public override bool OnStart( UnitManager unitMng )
    {
        return onStart( (FlightUnitManager)unitMng );
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    public override bool OnEnd( UnitManager unitMng )
    {
        return onEnd( (FlightUnitManager)unitMng );
    }

    protected abstract bool onStart( FlightUnitManager unitMng );
    protected abstract bool onEnd( FlightUnitManager unitMng );

    /// 姿勢の設定
    public void SetPosture( Matrix4 posture )
    {
        this.posture = posture;
        pos.X = this.posture.M41;
        pos.Y = this.posture.M42;
        pos.Z = this.posture.M43;
    }

    /// (ワールド上の)姿勢の取得
    public Matrix4 GetPosture()
    {
        return posture;
    }

    public Vector3 GetPos()
    {
        return pos;
    }
    

}

} // end ns FlightDemo
//===
// EOF
//===
