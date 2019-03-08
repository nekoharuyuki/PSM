/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using DemoGame;

namespace FlightDemo{

public class EnemyBalloonUnit
    : EnemyUnit
{

    /// コンストラクタ
    public EnemyBalloonUnit( string pass )
    : base( pass, new EnemyHandle(), new EnemyModel( "BALLOON" ) )
    {
    }

    /// デストラクタ
    ~EnemyBalloonUnit()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
        this.addCollision( new GeometrySphere( new Vector3( 0.0f, 0.0f, 0.0f ), 0.5f ) );
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }



}

} // end ns FlightDemo
//===
// EOF
//===
