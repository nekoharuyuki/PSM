/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


namespace FlightDemo{

public class PlaneController
{
    bool Update( UnitManager unitMng, Plane plane )
    {
        var pad = InputManager.InputGamePad;

		if( (pad.Scan & InputGamePadState.Left) != 0 ){
            plane.Left( delta );
		}
		if( (pad.Scan & InputGamePadState.Right) != 0 ){
            plane.Right( delta );
		}
		if( (pad.Scan & InputGamePadState.Up) != 0 ){
            plane.Up( delta );
		}
		if( (pad.Scan & InputGamePadState.Down) != 0 ){
            plane.Down( delta );
		}

		if( (pad.Scan & InputGamePadState.Square) != 0 ){
            plane.SpeedUp( delta );
		}
		if( (pad.Scan & InputGamePadState.Triangle) != 0 ){
            plane.SpeedDown( delta );
		}

        plane.Move( unitCommonData.FlightRoute, delta );

        return true;
    }
}

class Plane
{
    private Matrix4 posture;    ///< 姿勢
    private float routeTime;    ///< ルート上の位置(時間)
    private float speed;        ///< 移動速度


    bool Upate()
    {
        var pad = InputManager.InputGamePad;

		if( (pad.Scan & InputGamePadState.Left) != 0 ){
            plane.Left( delta );
		}
		if( (pad.Scan & InputGamePadState.Right) != 0 ){
            plane.Right( delta );
		}
		if( (pad.Scan & InputGamePadState.Up) != 0 ){
            plane.Up( delta );
		}
		if( (pad.Scan & InputGamePadState.Down) != 0 ){
            plane.Down( delta );
		}

		if( (pad.Scan & InputGamePadState.Square) != 0 ){
            plane.SpeedUp( delta );
		}
		if( (pad.Scan & InputGamePadState.Triangle) != 0 ){
            plane.SpeedDown( delta );
		}

        plane.Move( unitCommonData.FlightRoute, delta );

        return true;

    }
}




}
//===
// EOF
//===
