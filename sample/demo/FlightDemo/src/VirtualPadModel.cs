/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;
using System;
using DemoGame;

namespace FlightDemo
{
public class VirtualPadModel
    : FlightUnitModel
{
    private Texture2D image03;
	private Sprite[]  padSprite = new Sprite[VirtualPadUnit.kButton_Max];

	/// コンストラクタ
	public VirtualPadModel()
	{
	}

    /// デストラクタ
    ~VirtualPadModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
		// イメージの取得/生成
//        image03 = new Texture2D( gameData.TexContainer.Find( "2d_3.png" ) );
        image03 = (gameData.TexContainer.Find("2d_3.png")).ShallowClone() as Texture2D;
		// スプライトの生成
		padSprite[VirtualPadUnit.kButton_SpeedUp]   = new Sprite( image03, 129, 125, 118, 178, Graphics2D.Width - 121, Graphics2D.Height - 175 );
		padSprite[VirtualPadUnit.kButton_SpeedDown] = new Sprite( image03,   7, 125, 118, 178,   3, Graphics2D.Height - 175 );
		return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
		// スプライトの解放
		int i;
		for( i=0;i<VirtualPadUnit.kButton_Max;i++ )
		{
			if( padSprite[i] != null ){
				padSprite[i].Dispose();
				padSprite[i] = null;
			}
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
		int i;
		for( i=0;i<VirtualPadUnit.kButton_Max;i++ )
		{
	        Graphics2D.DrawSprite( padSprite[i], 0, 0 );
		}

        return true;
    }

    public Sprite GetPadSprite( int button )
    {
    	return padSprite[button];
    }
}
}

