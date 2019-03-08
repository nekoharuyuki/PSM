/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// Resource2dクラス
/// 2D素材の管理
///
public class Resource2d
{			
    // インスタンス生成
	private static Resource2d instance = new Resource2d();

	private static Texture2D imageLyt;         /// Effect・スコア・レイアウト用Image
	private static Texture2D imageLyt_enemy;  /// Enemy用Image
	private static Texture2D imageLyt_player00;  /// Player用Image
	private static Texture2D imageLyt_player01;  /// Player用Image
	private static Texture2D imageLyt_ecA;     ///
	private static Texture2D imageLyt_ecB;     ///
	private static Texture2D imageLyt_ecC;     ///
	private static Texture2D imageLyt_ecD;     ///
	private static Texture2D imageLyt_ecE;     ///
	private static Texture2D imageStage00;     /// Stage1用Image
	private static Texture2D imageStage00_BG;  /// Stage1用Image背景
	private static Texture2D imageStage00_BGP; /// Stage1用Image背景パーツ
	private static Texture2D imageStage01;     /// Stage2用Image
	private static Texture2D imageStage01_BG;  /// Stage2用Image背景
	private static Texture2D imageStage01_BGP; /// Stage2用Image背景パーツ
	private static Texture2D imageStage02;     /// Stage3用Image
	private static Texture2D imageStage02_BG;  /// Stage3用Image背景
	private static Texture2D imageStage02_BGP; /// Stage3用Image背景パーツ
	private static Texture2D imageDummy;       /// 122*480の白画像
	private static Texture2D imageArrow;       /// 矢印画像

    /// インスタンスの取得
    public static Resource2d GetInstance()
    {
        return instance;
    }
	
	/// レイアウト用Imageの取得
    public Texture2D ImageLyt
    {
        get {return imageLyt;}
    }
    public Texture2D ImageLyt_enemy
    {
        get {return imageLyt_enemy;}
    }
    public Texture2D ImageLyt_player00
    {
        get {return imageLyt_player00;}
    }
    public Texture2D ImageLyt_player01
    {
        get {return imageLyt_player01;}
    }
    public Texture2D ImageLyt_ecA
    {
        get {return imageLyt_ecA;}
    }
    public Texture2D ImageLyt_ecB
    {
        get {return imageLyt_ecB;}
    }
    public Texture2D ImageLyt_ecC
    {
        get {return imageLyt_ecC;}
    }
    public Texture2D ImageLyt_ecD
    {
        get {return imageLyt_ecD;}
    }
    public Texture2D ImageLyt_ecE
    {
        get {return imageLyt_ecE;}
    }
    public Texture2D ImageStage00
    {
        get {return imageStage00;}
    }
    public Texture2D ImageStage00_BG
    {
        get {return imageStage00_BG;}
    }
    public Texture2D ImageStage00_BGP
    {
        get {return imageStage00_BGP;}
    }
    public Texture2D ImageStage01
    {
        get {return imageStage01;}
    }
    public Texture2D ImageStage01_BG
    {
        get {return imageStage01_BG;}
    }
    public Texture2D ImageStage01_BGP
    {
        get {return imageStage01_BGP;}
    }
    public Texture2D ImageStage02
    {
        get {return imageStage02;}
    }
    public Texture2D ImageStage02_BG
    {
        get {return imageStage02_BG;}
    }
    public Texture2D ImageStage02_BGP
    {
        get {return imageStage02_BGP;}
    }
    public Texture2D ImageDummy
    {
        get {return imageDummy;}
    }
    public Texture2D ImageArrow
    {
        get {return imageArrow;}
    }

    /// 初期化
    ///
    /// @param [in] gDev GraphicsDevice
    ///
    public bool Init( DemoGame.GraphicsDevice gDev )
    {
        DemoGame.Graphics2D.Init( gDev.Graphics, 854, 480 );
		
	    // レイアウト＆２Ｄ素材の読み込み
		imageLyt        = new Texture2D("/Application/res/data/2d/2d_01.png", false);
		imageLyt_enemy = new Texture2D("/Application/res/data/2d/obj_sagyo_00.png", false);
		imageLyt_player00 = new Texture2D("/Application/res/data/2d/panda_00.png", false);
		imageLyt_player01 = new Texture2D("/Application/res/data/2d/panda_01.png", false);
		imageLyt_ecA    = new Texture2D("/Application/res/data/2d/2d_ecA.png", false);
		imageLyt_ecB    = new Texture2D("/Application/res/data/2d/2d_ecB.png", false);
		imageLyt_ecC    = new Texture2D("/Application/res/data/2d/2d_ecC.png", false);
		imageLyt_ecD    = new Texture2D("/Application/res/data/2d/2d_ecD.png", false);
		imageLyt_ecE    = new Texture2D("/Application/res/data/2d/2d_ecE.png", false);
		imageStage00    = new Texture2D("/Application/res/data/2d/obj_st1_00.png", false);
		imageStage00_BG = new Texture2D("/Application/res/data/2d/bg_st1_00.png", false);
		imageStage00_BGP = new Texture2D("/Application/res/data/2d/bgp_bg_st1.png", false);
		imageStage01    = new Texture2D("/Application/res/data/2d/obj_st2_00.png", false);
		imageStage01_BG = new Texture2D("/Application/res/data/2d/bg_st2_00.png", false);
		imageStage01_BGP = new Texture2D("/Application/res/data/2d/bgp_bg_st2.png", false);
		imageStage02    = new Texture2D("/Application/res/data/2d/obj_st3_00.png", false);
		imageStage02_BG = new Texture2D("/Application/res/data/2d/bg_st3_00.png", false);
		imageStage02_BGP = new Texture2D("/Application/res/data/2d/bgp_bg_st3.png", false);
		imageDummy = new Texture2D("/Application/res/data/2d/dummy.png", false);
		imageArrow = new Texture2D("/Application/res/data/2d/arrow.png", false);
        return true;
    }

    /// 破棄
    public void Term()
    {
        DemoGame.Graphics2D.ClearSprite();

        imageLyt.Dispose();
        imageLyt_player00.Dispose();
        imageLyt_player01.Dispose();
        imageLyt_ecA.Dispose();
        imageLyt_ecB.Dispose();
        imageLyt_ecC.Dispose();
        imageLyt_ecD.Dispose();
        imageLyt_ecE.Dispose();
        imageStage00.Dispose();
        imageStage00_BG.Dispose();
        imageStage00_BGP.Dispose();
        imageStage01.Dispose();
        imageStage01_BG.Dispose();
        imageStage01_BGP.Dispose();
        imageStage02.Dispose();
        imageStage02_BG.Dispose();
        imageStage02_BGP.Dispose();
        imageDummy.Dispose();
        imageArrow.Dispose();

        DemoGame.Graphics2D.Term();
    }
	
}

}

