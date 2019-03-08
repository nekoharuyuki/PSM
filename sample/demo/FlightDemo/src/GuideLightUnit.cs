/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core;

namespace FlightDemo{

/// 誘導灯
public class GuideLightUnit
    : FlightUnit
{
    private const int kPOINT_COUNT = 15;        // 
    private const float kPOINT_INTERVAL = 0.25f * 0.9f; // 単位：秒
    private float offsetTime = 0.0f;
    private Matrix4[] children = new Matrix4[ kPOINT_COUNT ];

    /// コンストラクタ
    public GuideLightUnit()
    : base( new GuideLightHandle(), new GuideLightModel() )
    {
    }

    /// デストラクタ
    ~GuideLightUnit()
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


    public int GetLightCount()
    {
        return children.Length;
    }

    public Matrix4 GetLight( int i )
    {
        return children[ i ];
    }

    /// 基準時間の設定
    public void SetBaseTime( FlightRoute route, float wt )
    {
        //float len = route.Length();
        offsetTime = wt - (wt % kPOINT_INTERVAL);

        children[ 0 ] = route.BasePosture( wt );

        for( int i = 1; i < kPOINT_COUNT; i++ ){
            children[ i ] = route.BasePosture( offsetTime + kPOINT_INTERVAL * i );
        }
    }
}



} // end ns FlightDemo
//===
// EOF
//===
