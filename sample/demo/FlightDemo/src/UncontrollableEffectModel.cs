/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo{

public class UncontrollableEffectModel
    : FlightUnitModel
{
    private Texture2D image03;
    private Sprite    alertSprite;  ///< "UNCONTROLLABLE"

    /// コンストラクタ
    public UncontrollableEffectModel()
    {
    }

    /// デストラクタ
    ~UncontrollableEffectModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
		var centerX = Graphics2D.Width / 2;

//        image03 = new Texture2D( gameData.TexContainer.Find( "2d_3.png" ) );
        image03 = (gameData.TexContainer.Find("2d_3.png")).ShallowClone() as Texture2D;
        alertSprite = new Sprite( image03, 417,  78, 490,  53, centerX - 244, Graphics2D.Height - 135 );

        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        if( image03 != null ){
            image03.Dispose();
            image03 = null;
        }
        if( alertSprite != null ){
            alertSprite.Dispose();
            alertSprite = null;
        }
        return true;
    }

    /// アニメーションの更新処理
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        Graphics2D.DrawSprite( alertSprite, 0, 0 );

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
