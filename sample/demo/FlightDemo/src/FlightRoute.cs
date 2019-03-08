/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
//using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public enum RouteState{
    StartingWait,
    PlayTime,
    EndingWait,
    EndOfRoute,
}

/// Plane の予定航路を示す
public class FlightRoute
{
    public static float kROUTE_RADIUS = 4.5f;
    public BasicModel modelRoute; ///< Bone のパスがルートになっている

    /// コンストラクタ
    public FlightRoute()
    {
    }

    /// デストラクタ
    ~FlightRoute()
    {
    }

    public void Dispose()
    {
        if( modelRoute != null ){
            modelRoute.Dispose();
            modelRoute = null;
        }
    }

    /// ルートのロード
    public bool Load( string name )
    {
        modelRoute = new BasicModel( name, 0 );
        return false;
    }

    /// 全長
    public float Length()
    {
        return modelRoute.GetMotionLength( 0 );
    }


    /// 指定時間がスタート～ゴールの中にあるか？
    public RouteState GetRouteState( float time )
    {
        if( StartTime() >= time ){
            return RouteState.StartingWait;
        }else if( (Length() - 1.0f) <= time ){
            return RouteState.EndOfRoute;
        }else if( GoalTime() <= time ){
            return RouteState.EndingWait;
        }

        return RouteState.PlayTime;
    }

    /// スタート地点
    public float StartTime()
    {
        return 2.8f;
    }

    /// ゴール地点
    public float GoalTime()
    {
        return this.Length() - 5.0f;
    }

    /// 時間から基本姿勢の取得
    public Matrix4 BasePosture( float time )
    {
        modelRoute.SetAnimTime( 0, time );
        modelRoute.Update();

        return modelRoute.GetBoneMatrix( 0 );
    }

    /// 時間の位置
    public Vector3 BasePos( float time )
    {
        Matrix4 posture = BasePosture( time );
        return new Vector3( posture.M41, posture.M42, posture.M43 );
    }

    /// 指定時間のコース上の進行方向を得る
    /**
     * 進行方向は正規化されていないので、単位秒あたりの長さ取得が行える
     */
    public Vector3 Direction( float time, float delta )
    {
        Vector3 now  = this.BasePos( time );
        Vector3 next = this.BasePos( time + delta );

        Vector3 dir = next - now;

        return dir;
    }

    public static float SecToRouteSec( float sec )
    {
        return 0.50f * sec;
    }
}

} // end ns FlightDemo

//===
// EOF
//===
