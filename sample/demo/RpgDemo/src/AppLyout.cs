/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;


namespace AppRpg {


///***************************************************************************
/// アプリのレイアウト
///***************************************************************************
public class AppLyout
{
    ///---------------------------------------------------------------------------
    /// レイアウトデータ型
    ///---------------------------------------------------------------------------
    public enum SpriteId{
        Logo = 0,
        TouchScreen,
        Copyright,
        Mess_GAMEOVER,
        Mess_ENEMYALLCLEAR,
        Life,
        Life1_on,
        Life1_off,
        Life2_on,
        Life2_off,
        Life3_on,
        Life3_off,
        Num_life_1,
        Num_life_0,
		Play_on,
		Play_off,
		Tutorial_on,
		Tutorial_off,
		LeftA,
		RightA,
		Back_on,
		Back_off
    };



    ///---------------------------------------------------------------------------
    /// レイアウトデータ
    ///---------------------------------------------------------------------------
    private int[] lytRectData = {
		2,142,599,204,
		472,419,261,33,
		940,7,75,19,
		2,348,453,134,
		0,0,487,142,
		630,45,211,102,
		878,45,30,38,
		911,45,30,38,
		878,45,30,38,
		911,45,30,38,
		878,45,30,38,
		911,45,30,38,
		611,374,26,36,
		611,374,26,36,
		611,152,203,58,
		611,216,203,58,
		818,152,203,58,
		818,216,203,58,
		518,14,41,100,
		566,14,41,100,
		705,277,85,85,
		610,277,85,85,
    };

    private int[] lytScrData = {
		128,120,
		296,370,
		391,451,
		203,169,
		186,169,
		1,0,
		102,5,
		102,5,
		137,5,
		137,5,
		172,5,
		172,5,
		124,57,
		102,57,
		186,344,
		186,344,
		466,344,
		466,344,
		9,157,
		807,157,
		756,382,
		756,382,
    };

    private const int lyoutW = 854;            /// レイアウトデータの画面サイズW
    private const int lyoutH = 480;            /// レイアウトデータの画面サイズH

    private static AppLyout instance = new AppLyout();
        

    private DemoGame.Sprite[]        spritList;
    private Texture2D                image;

    private int        scrWidth;
    private int        scrHeight;
    private int        offsetW;
    private int        offsetH;


    /// インスタンスの取得
    public static AppLyout GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init( DemoGame.GraphicsDevice gDev )
    {
        scrWidth    = gDev.DisplayWidth;
        scrHeight   = gDev.DisplayHeight;

        offsetW     = (scrWidth-lyoutW) / 2;
        offsetH     = (scrHeight-lyoutH) / 2;

        DemoGame.Graphics2D.Init( gDev.Graphics );
        return true;
    }

    /// 破棄
    public void Term()
    {
        DemoGame.Graphics2D.ClearSprite();
        if( spritList != null ){
            for( int i=0; i<spritList.Length; i++ ){
                spritList[i] = null;
            }
        }

        spritList = null;
        image.Dispose();

        DemoGame.Graphics2D.Term();
    }

    /// レイアウト＆２Ｄ素材の読み込み
    public bool Load()
    {
        image = new Texture2D( "/Application/res/data/2D/2d_01.png", false);

        spritList = new DemoGame.Sprite[lytScrData.Length/2];

        for( int i=0; i<lytScrData.Length/2; i++ ){

            int scrX = lytScrData[i*2+0];
            int scrY = lytScrData[i*2+1];
            int rectX = lytRectData[i*4+0];
            int rectY = lytRectData[i*4+1];
            int rectW = lytRectData[i*4+2];
            int rectH = lytRectData[i*4+3];

            if( i < (int)SpriteId.Life || i >= (int)SpriteId.Play_on ){
                scrX += offsetW;
                scrY += offsetH;
            }

            spritList[i] = new DemoGame.Sprite( image, rectX, rectY, rectW, rectH, scrX, scrY );
        }

        for( int i=0; i<spritList.Length; i++ ){
            DemoGame.Graphics2D.AddSprite( spritList[spritList.Length-1-i] );
        }
        ClearSpriteAll();
        return true;
    }


    
/// private メソッド
///---------------------------------------------------------------------------

    /// スプライトの登録
    public void SetSprite( SpriteId id )
    {
        spritList[(int)id].Visible = true;
    }

    /// スプライトを削除
    public void ClearSprite( SpriteId id )
    {
        spritList[(int)id].Visible = false;
    }

    /// スプライトを全削除
    public void ClearSpriteAll()
    {
        for( int i=0; i<spritList.Length; i++ ){
            spritList[i].Visible = false;
        }
    }

    /// スプライトのα値を変更する
    public void SetAlpha( SpriteId sprId, float alpha )
    {
        int id = (int)sprId;
        spritList[id].Alpha = alpha;
    }

    /// スプライトのフォーカスを変更する
    public void SetFocus( SpriteId sprId, int idx )
    {
        int id = (int)sprId;
        spritList[id].SetDrawRect( lytRectData[id*4+0] + lytRectData[id*4+2] * idx,
                                   lytRectData[id*4+1], lytRectData[id*4+2], lytRectData[id*4+3] );
    }

    /// Rectの範囲に入っているかのチェック
    public bool CheckRect( SpriteId sprId, int pointX, int pointY )
    {
        if(spritList==null){
            return false;
        }
            
        int id = (int)sprId;
        int scrPosX = (int)spritList[id].PositionX;
        int scrPosY = (int)spritList[id].PositionY;

        if( ( scrPosX <= pointX && pointX <= (scrPosX + spritList[id].DrawRectWidth) ) &&
            ( scrPosY <= pointY && pointY <= (scrPosY + spritList[id].DrawRectHeight) ) ){
            return true;
        }
        return false;
    }

    /// 配置物の中心座標を返す
    public int GetCenterX( SpriteId sprId )
    {
        int id = (int)sprId;
        int scrPosX = (int)spritList[id].PositionX;
        return( scrPosX+(int)(spritList[id].DrawRectWidth/2) );
    }
    public int GetCenterY( SpriteId sprId )
    {
        int id = (int)sprId;
        int scrPosY = (int)spritList[id].PositionY;
        return( scrPosY+(int)(spritList[id].DrawRectHeight/2) );
    }


    /// 描画
    public void Render()
    {
        DemoGame.Graphics2D.DrawSpritesUseAlpha();
    }


/// プロパティ
///---------------------------------------------------------------------------

    /// 移動候補座標
    public int OffsetW
    {
        get {return offsetW;}
    }
    public int OffsetH
    {
        get {return offsetH;}
    }

}

} // namespace
