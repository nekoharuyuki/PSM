/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;
using System;
using DemoGame;

namespace FlightDemo{

/// 2D表示のスピードメーター
public class SpeedMeterModel
    : FlightUnitModel
{
    private Texture2D image03;
    private Sprite    baseSprite; ///< メーターの下地
	private Sprite    handSprite; ///< メーターの針
		
	/// コンストラクタ
    public SpeedMeterModel()
    {
    }

    /// デストラクタ
    ~SpeedMeterModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
		// イメージの取得/生成
//        image03 = new Texture2D( gameData.TexContainer.Find( "2d_3.png" ) );
        image03 = (gameData.TexContainer.Find("2d_3.png")).ShallowClone() as Texture2D;
		// スプライトの生成
        baseSprite = new Sprite( image03, 457, 319, 188, 189, 7, 8 );
        handSprite = new Sprite( image03, 646, 136, 194, 194, 4, 4 );
		handSprite.CenterX = 97; // 切り出し矩形内の座標(テクスチャ内の座標ではない)
		handSprite.CenterY = 97; // 切り出し矩形内の座標(テクスチャ内の座標ではない)
			
		return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
		// スプライトの解放
		if( handSprite != null ){
			handSprite.Dispose();
			handSprite = null;
		}
		if( baseSprite != null ){
			baseSprite.Dispose();
			baseSprite = null;
		}
		// イメージの解放
        if( image03 != null ){
            image03.Dispose();
            image03 = null;
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
        SpeedMeterUnit myUnit = unit as SpeedMeterUnit;

		// 下地
        Graphics2D.DrawSprite( baseSprite, 0, 0 );
		// 針(3Dアニメーションとの兼ね合いでメーター上の270が最速)
		handSprite.Degree = 330.0f * myUnit.Speed(); // (0.0 <= speed <= 1.0) を (0.0 <= deg <= 330.0) にコンバート
        Graphics2D.DrawSprite( handSprite, 0, 0 );

		// デバッグ出力
        // Graphics2D.DrawText( String.Format("SPEED : {0:0.000}", myUnit.Speed()), 0xffffffff, 0, 200);
        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
