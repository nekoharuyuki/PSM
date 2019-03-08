/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;

namespace ShootingDemo
{

/**
 * EnemyDirBulletHandle
 */
public class EnemyDirBulletHandle : ScriptUnitHandle
{
    /// コンストラクタ
    public EnemyDirBulletHandle()
    {
    }

    /// 開始
    public override bool Start(MonoManager monoManager, Mono bullet)
    {
        var player = monoManager.FindMono("Player");

        if ( player != null ) {

            /// 姿勢初期化
            Vector3 tempPosition = bullet.WorldPosition;
            bullet.WorldMatrix = Matrix4.Identity;
            tempPosition.X = 0f;
            bullet.WorldPosition = tempPosition;

            int diffZ = (int)( player.WorldPosition.Z - bullet.WorldPosition.Z );
            int diffY = (int)( player.WorldPosition.Y - bullet.WorldPosition.Y );

            int ang = bullet.MathAtan2( -diffZ, diffY );

            MoveBasic(  1, 0, 0,  0, ang, 0, 0);
            MoveBasic(199, 0, 0, 10,   0, 0, 0);
        }

        return true;
    }
}

} // ShootingDemo
