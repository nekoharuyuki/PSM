/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public class GoalGateModel
    : FlightUnitModel
{
    private float animTime = 0.0f;
    private BasicModel model;

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        model = gameData.ModelContainer.Find( "GOAL" );
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        animTime = 0.0f;
        // 所有権なし
        model = null;
        return true;
    }
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        model.SetAnimTime( 0, animTime );

        animTime += delta;
        float len = model.GetMotionLength( 0 );
        animTime %= len;

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        GoalGateUnit myUnit = unit as GoalGateUnit;
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        if( myUnit.State != GoalGateState.Sleep ){
            FlightRoute route = gameData.FlightRoute;

            model.WorldMatrix = route.BasePosture( route.GoalTime() );
            model.Update();
            model.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
            //            model.Render( "MODEL-GOAL", route.BasePosture( route.GoalTime() ) );
        }

        return true;
    }
}

} // end ns FlightDemo

//===
// EOF
//===
