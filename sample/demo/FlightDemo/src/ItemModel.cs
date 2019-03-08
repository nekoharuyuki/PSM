/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public class ItemModel
    : FlightUnitModel
{
    private BasicModel model;
    private string nameModel;

    public ItemModel( string nameModel )
    {
        this.nameModel = nameModel;
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        model = gameData.ModelContainer.Find( nameModel );
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
        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        ItemUnit myUnit = unit as ItemUnit;

        switch( myUnit.State ){
          case ItemState.Sleep:
            break;
          case ItemState.Normal:
          case ItemState.Success:
          case ItemState.Fail:
            GraphicsContext graphics = Renderer.GetGraphicsContext();

            model.WorldMatrix = unit.GetPosture();
            model.Update();
            model.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );

            DbgSphere.Draw( graphics, gameData.GetViewProj(), myUnit.GetPosture(), myUnit.GetCollision() );
            break;
          case ItemState.Destroy:
            break;
        }



        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
