/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;
using DemoGame;

namespace FlightDemo{


public
class SceneTitle
    : IScene
{
    private Texture2D image;
    private SceneManager useSceneMgr;

	private const int kState_FadeIn		= 0;  // タイトル開始時フェードイン			
	private const int kState_Title	    = 1;  // タイトルメイン状態
	private const int kState_FadeOut    = 2;  // ゲーム画面への遷移用フェード
	private const int kState_AnimFadeOut= 3;  // アニメーションループ用のフェード
	private const int kState_AnimFadeIn = 4;  // アニメーションループ用のフェード
	private const int kFade_MaxTime = 20;
	private int state;
	private int fadeTime;
    private float animTime = 0.0f;
    private BasicModel passOpening;

    private GameCommonData unitCommonData;

    /// コンストラクタ
    public SceneTitle()
    {
    }

    /// デストラクタ
    ~SceneTitle()
    {
    }

    /// シーンの初期化
    public bool Init(SceneManager useSceneMgr)
    {
        this.useSceneMgr = useSceneMgr;

        image = new Texture2D("/Application/res/2d/2d_1.png", false);
				
		var centerX = Graphics2D.Width / 2;
		var centerY = Graphics2D.Height / 2;
        Graphics2D.AddSprite(new Sprite(image,   2,	2, 541, 239, centerX - 271,  centerY - 183));
        Graphics2D.AddSprite(new Sprite(image, 561,	6, 297,  47, centerX - 148, centerY + 111));
        //Graphics2D.AddSprite(new Sprite(image, 877,	9,  78,  22, 389, 451));

		// サウンドの読み込み
		AudioManager.AddSound( "Press", "/Application/res/snd/F_SYS_SE_001.wav" );
				
		state = kState_FadeIn;
		fadeTime = 20;

        // 管理は考える
        unitCommonData = GameCommonData.Inst();
        initBg();
        passOpening = unitCommonData.ModelContainer.Find( "OPENING" );


        return true;
    }

    /// シーンの破棄
    public void Term()
    {
        unitCommonData = null;
				
        if( image != null ){
            image.Dispose();
            image = null;
        }

        // 所有権なし
        useSceneMgr = null;
        Graphics2D.ClearSprite();

		// サウンド解放
		AudioManager.Clear();
    }

    /// シーンの継続切り替え時の再開処理
    public bool Restart()
    {
        return true;
    }

    /// シーンの継続切り替え時の停止処理
    public bool Pause()
    {
        return true;
    }

    /// サスペンド＆レジューム処理
    public void Suspend()
    {
    }

    public void Resume()
    {
    }

    /// フレーム処理
    public bool Update()
    {
    	switch( state )
    	{
	    case kState_AnimFadeOut:
	    	if( fadeTime++ > kFade_MaxTime )
	    	{
                animTime = 0.0f;
                state = kState_AnimFadeIn;
            }
            break;
          case kState_AnimFadeIn:
	    	if( fadeTime-- <= 0 )
	    	{
                state = kState_Title;
            }
            break;

        case kState_FadeIn:
	    	if( fadeTime-- <= 0 )
	    	{
	        	state = kState_Title;
            }
        	break;            
    	case kState_Title:
	        var pad = InputManager.InputGamePad;
        	var touch = InputManager.InputTouch;
	        if( pad.Trig != 0 || touch.GetInputNum() > 0 ){
	        	state = kState_FadeOut;
	        	fadeTime = 0;
				AudioManager.PlaySound("Press");
	        }
            if( animTime >= (passOpening.GetMotionLength( 0 ) - 1.0f)){
	        	fadeTime = 0;
                state = kState_AnimFadeOut;
            }
	        break;
	    case kState_FadeOut:
	    	if( fadeTime++ > kFade_MaxTime )
	    	{
	            useSceneMgr.Next( new SceneGameMain(), false );
	            return false;
            }
	    	break;

	    }
        animTime += 1.0f / 30.0f;

        return true;
    }

    private void fade()
    {
		uint alpha;
        if( fadeTime > kFade_MaxTime ) alpha = 0xff;
        else alpha = (uint)(0xff * fadeTime / kFade_MaxTime);
        if( state != kState_FadeIn )
	        Graphics2D.FillRect( (alpha << 24) | 0xffffff, 0, 0, Graphics2D.Width, Graphics2D.Height );
	    else
	        Graphics2D.FillRect( (alpha << 24) | 0x000000, 0, 0, Graphics2D.Width, Graphics2D.Height );
    }

    /// 描画処理
    public bool Render()
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        renderBg( graphics );

        

        if( state == kState_AnimFadeIn || state == kState_AnimFadeOut ){
            fade();
	    }

        Graphics2D.DrawSprites();

        if( state == kState_FadeIn || state == kState_FadeOut ){
            fade();
	    }

        graphics.SwapBuffers();


        return true;
    }


    public void initBg()
    {
        unitCommonData.SetProjection( Matrix4.Perspective( DemoUtil.Deg2Rad( 20.0f ),
                                                           Graphics2D.Width / (float)Graphics2D.Height,
                                                           0.25f, 1200000.0f ) );
    }

    public void renderBg( GraphicsContext graphics )
    {
        // 環境用のライト
        Light light = new Light();
        light.Position  = new Vector4( 0.0f, 120.0f, 0.0f, 1.0f );
        light.KDiffuse  = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        light.KSpecular = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );

        unitCommonData.ModelContainer.SetLightCount( 1 );
        unitCommonData.ModelContainer.SetLight( 0, light );

        // デモ用のパス決定
        Matrix4 eye = passOpening.Bones[1].WorldMatrix;
        Matrix4 plane = passOpening.Bones[0].WorldMatrix;
        passOpening.SetAnimTime( 0, animTime );
        passOpening.Update();

        // 地形
        BasicModel modelBg = unitCommonData.ModelContainer.Find( "BG" );

        modelBg.SetAnimTime( 0 , animTime % modelBg.GetMotionLength( 0 ) );
        modelBg.Update();
        modelBg.Draw( graphics,
                      unitCommonData.ShaderContainer,
                      unitCommonData.GetViewProj(),
                      unitCommonData.GetEyePos() );

        // 飛行機
        unitCommonData.SetView( eye.Inverse() );

        drawPlane( graphics, plane );

    }

    public void drawPlane( GraphicsContext graphics, Matrix4 plane )
    {
        BasicModel modelPlane = unitCommonData.ModelContainer.Find( "PLANE" );
        BasicModel modelTurn = unitCommonData.ModelContainer.Find( "TURN" );
        BasicModel modelUpDownR = unitCommonData.ModelContainer.Find( "UPDOWN-R" );
        BasicModel modelUpDownL = unitCommonData.ModelContainer.Find( "UPDOWN-L" );
        BasicModel modelPropera = unitCommonData.ModelContainer.Find( "PROPERA" );

        modelPlane.WorldMatrix = plane;
        modelPlane.Update();
        modelPlane.Draw( graphics, unitCommonData.ShaderContainer, unitCommonData.GetViewProj(), unitCommonData.GetEyePos() );

        modelTurn.WorldMatrix = plane;
        modelTurn.SetAnimTime( 0, 0.5f );
        modelTurn.Update();
        modelTurn.Draw( graphics, unitCommonData.ShaderContainer, unitCommonData.GetViewProj(), unitCommonData.GetEyePos() );

        modelUpDownR.WorldMatrix = plane;
        modelUpDownR.SetAnimTime( 0, 0.5f );
        modelUpDownR.Update();
        modelUpDownR.Draw( graphics, unitCommonData.ShaderContainer, unitCommonData.GetViewProj(), unitCommonData.GetEyePos() );

        modelUpDownL.SetAnimTime( 0, 0.5f );
        modelUpDownL.WorldMatrix = plane;
        modelUpDownL.Update();
        modelUpDownL.Draw( graphics, unitCommonData.ShaderContainer, unitCommonData.GetViewProj(), unitCommonData.GetEyePos() );


        modelPropera.SetAnimTime( 0, animTime % modelPropera.GetMotionLength( 0 ) );
        modelPropera.WorldMatrix = plane;
        modelPropera.Update();
        modelPropera.Draw( graphics, unitCommonData.ShaderContainer, unitCommonData.GetViewProj(), unitCommonData.GetEyePos() );
    }

}


} // end ns FlightDemo

//===
// EOF
//===
