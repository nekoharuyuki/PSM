/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo{

public class Timeover2DModel
    : FlightUnitModel
{
    private Texture2D image02;
    private Sprite sprite;

    /// コンストラクタ
    public Timeover2DModel()
    {
    }

    /// デストラクタ
    ~Timeover2DModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
		// イメージの取得/生成
		var centerX = Graphics2D.Width / 2;
		var centerY = Graphics2D.Height / 2;
//        image02 = new Texture2D( gameData.TexContainer.Find( "2d_2.png" ) );
        image02 = (gameData.TexContainer.Find("2d_2.png")).ShallowClone() as Texture2D;
        sprite = new Sprite( image02, 5, 5, 527, 86, centerX - 268, centerY - 43 );

		return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
		// スプライトの解放
		if( sprite != null ){
			sprite.Dispose();
			sprite = null;
		}
		// イメージの解放
        if( image02 != null ){
            image02.Dispose();
            image02 = null;
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
        Graphics2D.DrawSprite( sprite, 0, 0 );

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
