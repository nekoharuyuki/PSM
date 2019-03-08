/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

namespace ShootingDemo
{

/**
 * EnemyCNormalUnitHandleクラス
 */
public class EnemyCNormalUnitHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public EnemyCNormalUnitHandle() : base()
    {
    }

    /// デストラクタ
    ~EnemyCNormalUnitHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono unit)
    {
        var player = monoManager.FindMono("Player");
        int ang = 0;

        if ( player != null ) {
            int diffZ = (int)( player.WorldPosition.Z - unit.WorldPosition.Z );
            int diffY = (int)( player.WorldPosition.Y - unit.WorldPosition.Y );

            ang = unit.MathAtan2( diffZ, -diffY );
        }

        MoveBasic(  1,  0, 0,  8,   0, 0, ang);
        MoveBasic( 20,  0, 0, 16,   0, 0, 0, 0, 0, 9);
        MoveBasic( 18,  0, 0,  8,   0, 5, 0);
        MoveBasic( 99,  0, 0, 10,   0, 0, 0);

        return true;
    }
}

} // ShootingDemo
