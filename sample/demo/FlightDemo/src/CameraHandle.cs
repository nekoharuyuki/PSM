/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using Sce.PlayStation.Core;
using DemoModel;
using UnitSys;

namespace FlightDemo{

public class CameraHandle
    : FlightUnitHandle
{
    private Vector4 prevCenterPos;   ///< 補間用の直前の注視点位置
    private Vector4 prevCamPos;      ///< 補間用の直前の

    /// コンストラクタ
    public CameraHandle()
    {
    }

    /// デストラクタ
    ~CameraHandle()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng, FlightUnit unit )
    {
        return true;
    }

    /// 時間差分更新処理
    protected override bool onUpdate( FlightUnitManager unitMng, FlightUnit unit, float delta )
    {
        GameCommonData gameData = unitMng.commonData as GameCommonData;
        
        PlaneUnit plane = unitMng.Find( "Plane", 0 ) as PlaneUnit;
        // メモ
        // 垂直画角 15.0
        // (飛行機相対の)注視点      : pos(0.0f, 0.1311f, -0.0251f) / 100.0f
        // (飛行機相対の)カメラ位置   : pos(0.0f, 0.2636f, -1.192.0f) / 100.0f 206?
        Matrix4 planePosture = plane.GetPosture();

        // 飛行機を基準にしたアップベクトル
        Vector3 up = new Vector3( planePosture.M21,
                                  planePosture.M22,
                                  planePosture.M23 );

        Vector4 localCenterPos = new Vector4( 0.0f, 0.1311f, 0.0251f, 1.0f );
        Vector4 worldCenterPos = planePosture * localCenterPos;
        // カメラアニメーション
        Vector4 localCamPos = new Vector4( 0.0f, 0.2636f, 1.192f, 1.0f );
        Vector4 worldCamPos = planePosture * localCamPos;

        prevCenterPos = worldCenterPos;
        prevCamPos = worldCamPos;
        
        Vector4 centerPos = prevCenterPos.Lerp( worldCenterPos, 0.5f );
        Vector4 camPos = prevCamPos.Lerp( worldCamPos, 0.5f );

        Matrix4 view = Matrix4.LookAt( new Vector3( camPos.X, camPos.Y, camPos.Z ),
                                       new Vector3( centerPos.X, centerPos.Y, centerPos.Z ),
                                       up );
        gameData.SetView( view );

        prevCenterPos = centerPos;
        prevCamPos = camPos;

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
