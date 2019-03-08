/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public class BgModel
    : FlightUnitModel
{
	private float animTime = 0.0f;
    private BasicModel model;

    /// コンストラクタ
    public BgModel()
    {
    }

    /// デストラクタ
    ~BgModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        model = gameData.ModelContainer.Find( "BG" );

        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        // 所有権なし
        model = null;
        return true;
    }

    /// アニメーションの更新処理
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
		animTime += delta;
		animTime %= model.GetMotionLength(0);
        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        model.WorldMatrix = unit.GetPosture();
		model.SetAnimTime( 0, animTime );
        model.Update();
        model.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
