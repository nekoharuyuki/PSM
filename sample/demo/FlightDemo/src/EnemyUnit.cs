/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections;
using Sce.PlayStation.Core;
using DemoGame;

namespace FlightDemo{

public abstract class EnemyUnit
    : FlightUnit
{
    private ArrayList collisions = new ArrayList();
    protected FlightRoute route;

    /// コンストラクタ
    public EnemyUnit( string pass, EnemyHandle handle, FlightUnitModel model )
    : base( handle, model )
    {
        route = new FlightRoute();
        route.Load( pass );
    }

    /// デストラクタ()
    ~EnemyUnit()
    {
        if( collisions != null ){
            collisions = null;
        }
        if( route != null ){
            route.Dispose();
            route = null;
        }
    }

    /// ルート時間の設定
    public void SetRouteTime( float time )
    {
        Matrix4 posture = route.BasePosture( time );
        this.SetPosture( posture );
    }

    /// 自機とのあたり判定を取る
    public virtual bool IsHit( PlaneUnit plane )
    {
        for( int i = 0; i < GetCollisionCount(); i++ ){
            GeometrySphere sphere = GetCollision( i );
            Vector4 localPos = new Vector4( sphere.Pos.X, sphere.Pos.Y, sphere.Pos.Z, 1.0f );
            Vector4 pos = this.GetPosture() * localPos;

            Vector3 planePos = plane.GetPos();
            Vector3 dir = new Vector3( pos.X - planePos.X,
                                       pos.Y - planePos.Y,
                                       pos.Z - planePos.Z );

             float lenSq = dir.LengthSquared();
             float r = sphere.R + plane.GetCollision().R;

             if( (r * r) > lenSq ){
                 return true;
             }
        }
        
        return false;
    }

    public void addCollision( GeometrySphere sphere )
    {
        collisions.Add( sphere );
    }

    public int GetCollisionCount()
    {
        return collisions.Count;
    }

    public GeometrySphere GetCollision( int idx )
    {
        return collisions[ idx ] as GeometrySphere;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
