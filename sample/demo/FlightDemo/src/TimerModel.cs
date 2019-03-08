/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo{

public 
class TimerModel
    : FlightUnitModel
{
    private Texture2D image03;
    private NumberLayout minLayout; ///< 分
    private Sprite       minSprite; ///< 分の区切り
    private NumberLayout secLayout; ///< 秒
    private Sprite       secSprite; ///< 秒の区切り
    private NumberLayout tmsLayout; ///< 10ms

    /// コンストラクタ
    public TimerModel()
    {

    }

    /// デストラクタ
    ~TimerModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        //		  this call is depricated and replaced by ShallowClone();				
//       image03 = new Texture2D( gameData.TexContainer.Find( "2d_3.png" ) );
        image03 = gameData.TexContainer.Find("2d_3.png").ShallowClone() as Texture2D;

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
        SpriteAnimation[] numSSprites = new SpriteAnimation[] {
            new SpriteAnimation( new Sprite( image03,0,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,40,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,80,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,120,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,160,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,200,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,240,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,280,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,320,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
            new SpriteAnimation( new Sprite( image03,360,66,40,54,0,0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff ),
        };
        
		var centerX = Graphics2D.Width / 2;

        minLayout = new NumberLayout( numLSprites,
                                      null,
                                      new int[]{ centerX - 111, centerX - 153, },
                                      new int[]{  30,  30, } );
        minSprite = new Sprite( image03, 791,  2,  19,  31, centerX - 63, 30 );


        secLayout = new NumberLayout( numLSprites,
                                      null,
                                      new int[]{ centerX + 1, centerX - 41, },
                                      new int[]{  30,  30, } );

        secSprite = new Sprite( image03, 822,   2,  34,   31, centerX + 48,  30 );

        tmsLayout = new NumberLayout( numSSprites,
                                      null,
                                      new int[]{ centerX + 114, centerX + 80, },
                                      new int[]{  41,  41, } );

        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        if( image03 != null ){
            image03.Dispose();
            image03 = null;
        }
        if( minLayout != null ){
            minLayout.Dispose();
            minLayout = null;
        }
        if( minSprite != null ){
            minSprite.Dispose();
            minSprite = null;
        }
        if( secLayout != null ){
            secLayout.Dispose();
            secLayout = null;
        }
        if( secSprite != null ){
            secSprite.Dispose();
            secSprite = null;
        }
        if( tmsLayout != null ){
            tmsLayout.Dispose();
            tmsLayout = null;
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
        TimerUnit myUnit = unit as TimerUnit;

        if( myUnit.IsShow() ){
            minLayout.Render( myUnit.RemainMin() );
            Graphics2D.DrawSprite( minSprite, 0, 0 );
            secLayout.Render( myUnit.RemainSec() );
            Graphics2D.DrawSprite( secSprite, 0, 0 );
            tmsLayout.Render( myUnit.RemainTMsec() );
        }

        return true;
    }
}

} // end ns FlightDemo
//===
// EOF
//===
