/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo{

public
class AddSecEffectModel
    : FlightUnitModel
{
    private int   ofstY;
    private float offset;
    private Texture2D image03;
    private Sprite       signPlusSprite;  ///< +
    private Sprite       signMinusSprite; ///< -
    private NumberLayout secLayout;       ///< 秒
    private Sprite       secSprite;       ///< sec

    /// コンストラクタ
    public AddSecEffectModel()
    {
    }

    
    /// デストラクタ
    ~AddSecEffectModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        image03 = (gameData.TexContainer.Find("2d_3.png")).ShallowClone() as Texture2D;        
		SpriteAnimation[] numLSprites = new SpriteAnimation[] {
            new SpriteAnimation( new Sprite( image03,   0, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,  47, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,  94, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 141, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 188, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 235, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 282, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 329, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 376, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 423, 0, 47, 66, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
        };

        // + / - を利用する
        SpriteAnimation[] signSprites = new SpriteAnimation[] {
            new SpriteAnimation( new Sprite( image03, 470, 0, 39, 66, 0, 0 ), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03, 509, 0, 34, 66, 0, 0 ), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
        };
        
		var centerX = Graphics2D.Width / 2;

        secLayout = new NumberLayout( numLSprites,
                                      signSprites,
                                      new int[]{ centerX - 29, centerX - 72, centerX - 114, },
                                      new int[]{ 120, 120, 120, } );
        signPlusSprite = new Sprite( image03, 470, 0, 39, 66, centerX - 114, 120 );
        signMinusSprite = new Sprite( image03, 470 + 39, 0, 34, 66, centerX - 114, 120 );
        secSprite = new Sprite( image03, 680, 0,  98,  43, centerX + 16, 142 );

        offset = 0.0f;
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        if( image03 != null ){
            image03.Dispose();
            image03 = null;
        }

        if( signPlusSprite != null ){
            signPlusSprite.Dispose();
            signPlusSprite = null;
        }

        if( signMinusSprite != null ){
            signMinusSprite.Dispose();
            signMinusSprite = null;
        }

        if( secLayout != null ){
            secLayout.Dispose();
            secLayout = null;
        }
        if( secSprite != null ){
            secSprite.Dispose();
            secSprite = null;
        }

        return true;
    }


    /// アニメーションの更新処理
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        offset += delta;
        ofstY = (int)DemoUtil.EaseOut( 0, -20, offset / 2.0f );

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        AddSecEffect myUnit = (AddSecEffect)unit;

        int sec = myUnit.GetTime();
        //        if( sec >= 0 ){
        //            Graphics2D.DrawSprite( signPlusSprite, 0, ofstY );
        //        }else{
        //            Graphics2D.DrawSprite( signMinusSprite, 0, ofstY );
        //        }
        secLayout.Render( sec, 0, ofstY, false );
        Graphics2D.DrawSprite( secSprite, 0, ofstY );

        return true;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
