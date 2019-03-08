/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public class StartGateModel
    : FlightUnitModel
{
    private float animTime;
    private BasicModel modelStart03;
    private BasicModel modelStart02;
    private BasicModel modelStart01;
    private BasicModel modelStart00;

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        modelStart03 = gameData.ModelContainer.Find( "START-3" );
        modelStart02 = gameData.ModelContainer.Find( "START-2" );
        modelStart01 = gameData.ModelContainer.Find( "START-1" );
        modelStart00 = gameData.ModelContainer.Find( "START-0" );

        animTime = 0.0f;
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        // 所有権なし
        modelStart03 = null;
        modelStart02 = null;
        modelStart01 = null;
        modelStart00 = null;

        return true;
    }
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        modelStart03.SetAnimTime( 0, animTime );
        modelStart02.SetAnimTime( 0, animTime );
        modelStart01.SetAnimTime( 0, animTime );
        modelStart00.SetAnimTime( 0, animTime );

        animTime += delta;
        animTime %= modelStart00.GetMotionLength( 0 );

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        FlightRoute route = gameData.FlightRoute;
        Matrix4 rot = Matrix4.RotationY( (float)Math.PI / 2.0f );

        GraphicsContext graphics = Renderer.GetGraphicsContext();

        StartGateUnit myUnit = unit as StartGateUnit;

        //        float ofst = 0.33f; //
        //        FlightRoute route = unitMng.GameCommonData.FlightRoute;

        float countTime03 = route.StartTime() - FlightRoute.SecToRouteSec( 3.0f );
        float countTime02 = route.StartTime() - FlightRoute.SecToRouteSec( 2.0f );
        float countTime01 = route.StartTime() - FlightRoute.SecToRouteSec( 1.0f );
        float countTime00 = route.StartTime() - FlightRoute.SecToRouteSec( 0.0f );
        //        float countTime03 = FlightRoute.SecToRouteSec( 1.0f );
        //        float countTime02 = FlightRoute.SecToRouteSec( 2.0f );
        //        float countTime01 = FlightRoute.SecToRouteSec( 3.0f );
        //        float countTime00 = FlightRoute.SecToRouteSec( 4.0f );

        switch( myUnit.GetIndex() ){
          case 0:
            modelStart00.WorldMatrix = route.BasePosture( countTime00 ) * rot;
            modelStart00.Update();
            modelStart00.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
            break;
          case 1:
            modelStart01.WorldMatrix = route.BasePosture( countTime01 ) * rot;
            modelStart01.Update();
            modelStart01.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
            break;
          case 2:
            modelStart02.WorldMatrix = route.BasePosture( countTime02 ) * rot;
            modelStart02.Update();
            modelStart02.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
            break;
          case 3:
            modelStart03.WorldMatrix = route.BasePosture( countTime03 ) * rot;
            modelStart03.Update();
            modelStart03.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
            break;
        }


        return true;
    }
}

} // end ns FlightDemo

//===
// EOF
//===
