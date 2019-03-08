/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;
using System;
using DemoGame;


namespace FlightDemo{

public class AltimeterModel
    : FlightUnitModel
{
    private Texture2D image03;
    private Sprite    baseSprite; ///< メーターの下地
	private Sprite    handSprite; ///< メーターの針

	/// コンストラクタ
    public AltimeterModel()
    {
    }

    /// デストラクタ
    ~AltimeterModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
		// イメージの取得/生成
//        image03 = new Texture2D( gameData.TexContainer.Find( "2d_3.png" ) );
        image03 = (gameData.TexContainer.Find("2d_3.png")).ShallowClone() as Texture2D;
		// スプライトの生成
        baseSprite = new Sprite( image03, 457, 134, 188, 184, Graphics2D.Width - 195, 13 );
        handSprite = new Sprite( image03, 262, 134, 194, 194, Graphics2D.Width - 198, 4 );
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
        AltimeterUnit myUnit = unit as AltimeterUnit;

		// 下地
        Graphics2D.DrawSprite( baseSprite, 0, 0 );
		// 針(3Dアニメーションは360度回転するがメーターの160を上限とする)
		handSprite.Degree = 40.0f + 280.0f * myUnit.HighDegree(); // (0.0 <= high <= 1.0) を (40.0 <= deg <= 320.0) にコンバート
        Graphics2D.DrawSprite( handSprite, 0, 0 );

		// デバッグ出力
        // Graphics2D.DrawText( String.Format("Hight : {0:0.000}", myUnit.HighDegree()), 0xffffffff, 0, 230);

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
