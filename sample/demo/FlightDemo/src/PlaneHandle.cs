/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using DemoGame;

namespace FlightDemo{

/// Plane の操作系
public class PlaneHandle
    : FlightUnitHandle
{
    /// コンストラクタ
    public PlaneHandle()
    {
    }

    /// デストラクタ
    ~PlaneHandle()
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



    /// 処理の更新
    protected override bool onUpdate( FlightUnitManager unitMng,
                                      FlightUnit unit, 
                                      float delta )
    {
        PlaneUnit plane = unit as PlaneUnit;

        
        // 自動的に roll を水平に保つ
        Matrix4 posture = plane.GetPosture();
        Vector4 up = new Vector4( 0.0f, 1.0f, 0.0f, 0.0f );
        Vector4 right = posture.ColumnX;

        if( up.Dot( right ) > 0.1f ){
            plane.Roll( -delta * 0.5f );
        }else if( up.Dot( right ) < -0.1f ){
            plane.Roll( delta * 0.5f );
        }

        var pad = InputManager.InputGamePad;
        VirtualPadUnit vpad = unitMng.Find( "Input", 0 ) as VirtualPadUnit;
        bool vpadSpeedDown = false;
        bool vpadSpeedUp = false;
        if( vpad != null ){
            vpadSpeedDown = vpad.IsTouch(VirtualPadUnit.kButton_SpeedDown);
            vpadSpeedUp = vpad.IsTouch(VirtualPadUnit.kButton_SpeedUp);
        }

        MotionInputUnit motion = unitMng.Find( "Input", 1 ) as MotionInputUnit;
        float motionLeft = 0.0f;
        float motionRight = 0.0f;
        float motionUp = 0.0f;
        float motionDown = 0.0f;
        if( motion != null ){
            motionLeft = motion.inputLeft;
            motionRight = motion.inputRight;
            motionUp = motion.inputUp;
            motionDown = motion.inputDown;
        }


		if( (pad.Scan & InputGamePadState.Left) != 0 ){
            plane.Yaw( 1.0f, delta );
            plane.Roll( delta );
		}else if( motionLeft > 0.0f ){
            plane.Yaw( motionLeft, delta );
            plane.Roll( motionLeft * delta );
        }

		if( (pad.Scan & InputGamePadState.Right) != 0 ){
            plane.Yaw( -1.0f, delta );
            plane.Roll( -delta );
		}else if( motionRight > 0.0f ){
            plane.Yaw( -motionRight, delta );
            plane.Roll( -motionRight * delta );
        }

		if( (pad.Scan & InputGamePadState.Up) != 0 ){
            plane.Pitch( -1.0f, delta );
		}else if( motionUp > 0.0f ){
            plane.Pitch( -motionUp, delta );
        }
            
		if( (pad.Scan & InputGamePadState.Down) != 0 ){
            plane.Pitch( 1.0f, delta );
		}else if( motionDown > 0.0f ){
            plane.Pitch( motionDown, delta );
        }

        if( (pad.Scan & InputGamePadState.Square) != 0 ){
            plane.Roll( -delta );
        }
        if( (pad.Scan & InputGamePadState.Triangle) != 0 ){
            plane.Roll( delta );
        }

		if( (pad.Scan & (InputGamePadState.Cross | InputGamePadState.L)) != 0 || vpadSpeedDown ){
            plane.AddSpeed( -delta );
		}
        if( (pad.Scan & (InputGamePadState.Circle | InputGamePadState.R)) != 0 || vpadSpeedUp ){
            plane.AddSpeed( delta );
		}
        GameCommonData commonData = (GameCommonData)unitMng.CommonData;

        //        PlaneState prevState = plane.State;
        plane.Move( commonData.FlightRoute, delta );

        //        if( prevState == PlaneState.Normal && plane.State == PlaneState.Uncontrollable ){
        //            unitMng.Regist( "Eff", -1, new UncontrollableEffect() );
        //        }

        return true;
    }
}
} // end ns FlightDemo
//===
// EOF
//===
