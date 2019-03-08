/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{


public class GateModel
    : FlightUnitModel
{
    private BasicModel model;
    private float animTime;

    /// コンストラクタ
    public GateModel()
    {
    }

    /// デストラクタ
    ~GateModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        model = gameData.ModelContainer.Find( "GATE" );
        animTime = 0.0f;
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        // 所有権なし
        model = null;
        return true;
    }
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        GateUnit gateUnit = unit as GateUnit;

        switch( gateUnit.State ){
          case ItemState.Normal:
            animTime = 0.0f;
            break;
          case ItemState.Success:
            animTime += delta;
            break;
          case ItemState.Fail:
            animTime += delta;
            break;
          case ItemState.Destroy:
            animTime = 0.0f;
            break;
        }

        if( animTime >= model.GetMotionLength( 0 ) ){
            gateUnit.Destroy();
        }

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();
        GateUnit myUnit = unit as GateUnit;

        if( myUnit.State != ItemState.Sleep ){
            model.WorldMatrix = unit.GetPosture();
            model.SetAnimTime( 0, animTime );
            model.Update();
            model.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
            DbgSphere.Draw( graphics, gameData.GetViewProj(), myUnit.GetPosture(), myUnit.GetCollision() );
        }

        return true;
    }



}
} // end ns FlightDemo
//===
// EOF
//===
