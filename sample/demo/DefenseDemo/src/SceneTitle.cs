// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoModel;
using DemoGame;

namespace DefenseDemo{


public
class SceneTitle
    : IScene
{
    private Texture2D image;
    private SceneManager useSceneMgr;
	private ResourceDataContainer useResCont;
	private float camX = 0.0f;
	private float camY = 0.0f;
	private float camZ = 0.0f;
	private float camRotY = 0.0f;
	private float camRotX = 0.0f;
	private Ground ground = new Ground();
	private Grid grid = new Grid();
	private float alphaVal = 0.0f;
	private float alphaAdd = 0.1f;
			
//	private Image testImg;
//	private Texture2D testImgTexture;
			
//	private Grid grid = new Grid();
//	private bool unitTouchFlg = false;
//	private int selectUnitId = 0;
//	private Unit[] selectUnit = new Unit[4];
			
	private CameraInfo camInfo;
				
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
				
		camInfo = CameraInfo.Inst();

        image = new Texture2D("/Application/res/2d/2d_03.png", false);
				
		Graphics2D.AddSprite( "logo", new Sprite( image, 0, 352, 657, 277, 101, 52 ) );
		Graphics2D.AddSprite( "PressAnyKey", new Sprite( image, 0, 878, 414, 43, 220, 371 ) );
				
		Graphics2D.FindSprite( "PressAnyKey" ).Alpha = 1.0f;
				
//		Console.WriteLine( "[log][SceneTitle.cs]press any key/alpha:"+Graphics2D.FindSprite( "PressAnyKey" ).Alpha );
				
		useResCont = ResourceDataContainer.Inst();
				
		/// ルートデータ
		useResCont.Load3D( "ROUT", "/Application/res/3d/ground/rout_attr.mdx" );
		
		/// ユニット配置データ
		useResCont.Load3D( "UNITPOS", "/Application/res/3d/ground/unit_pos.mdx" );
				
		/// 背景
		useResCont.Load3D( "GROUND", "/Application/res/3d/ground/ground00.mdx" );
		useResCont.Load2D( "base_oc.png", "ground/base_oc.png" );
		useResCont.Load2D( "ground00_01.png", "ground/ground00_01.png" );
		useResCont.Load2D( "suna.png", "ground/suna.png" );
		/// ユニット配置箇所の蓋(アクション無し)
		useResCont.Load3D( "FUTA", "/Application/res/3d/ground/ground01.mdx" );
		/// ユニット配置箇所の蓋(アクション有り)
		useResCont.Load3D( "FUTA_E", "/Application/res/3d/ground/ground02.mdx" );

		useResCont.Load2D( "ground00_21.png", "ground/ground00_21.png" );
		useResCont.Load2D( "ground00_22.png", "ground/ground00_22.png" );
		useResCont.Load2D( "ground00_23.png", "ground/ground00_23.png" );
				
		useResCont.Load3D( "U00", "/Application/res/3d/ui/u00/u00.mdx" );
		useResCont.Load2D( "U00.png", "ui/u00/u00.png" );
				
		useResCont.modelContainer.BindTextures( useResCont.texContainer );
		
		useResCont.shaderContainer.LoadBasicProgram();
				
		AudioManager.AddBgm( "BGM_TITLE", "/Application/res/sound/S91.mp3" );
		AudioManager.AddSound( "SE_SELECT", "/Application/res/sound/S01.wav" ); 
		
		grid.Init();
				
		ground.Init();
				
		camY = 200.0f;
		camZ = 0.0f;
		camRotY = 3.0f;
		camRotX = -1.5f;

		camInfo.GetPosture().SetPosition( camX, camY, camZ );
		camInfo.GetPosture().AddYPR( camRotX, camRotY, 0.0f );
				
		camRotX = -20.0f;
		camRotY = 0.0f;
		camInfo.GetPosture().SetLookAt(
					new Vector3( camRotX, camRotY, 0.0f ),
					new Vector3( 0.0f, 0.0f, 0.0f ),
					-200.0f );

		EffectFade fade = EffectFade.GetInstance();
		fade.SetFadeIn( 0x000000, 10, true );
				
		alphaVal = 1.0f;
		alphaAdd = -0.1f;
				
		AudioManager.PlayBgm( "BGM_TITLE", false );
				
        return true;
    }

    /// シーンの破棄
    public void Term()
    {
		AudioManager.Clear();

        if( image != null ){
            image.Dispose();
            image = null;
        }
				
		grid.Term();
		grid = null;

		ground.Term();
		ground = null;

        // 所有権なし
        useSceneMgr = null;
        Graphics2D.ClearSprite();
		
		useResCont = null;

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
       	var touch = InputManager.InputTouch;
				
		EffectFade fade = EffectFade.GetInstance();
		
		if( fade.NowEffId == EffectFade.EffId.Non ){
			// タッチ押し時
			if( touch.GetInputNum() > 0 && touch.GetInputState(0) == InputTouchState.Down ){
			}
			// タッチ離し時
			if( touch.GetInputNum() > 0 && touch.GetInputState(0) == InputTouchState.Up ){
				AudioManager.PlaySound( "SE_SELECT" );
				fade.SetFadeOut( 0x000000, 10, true );
			}
		}
		fade.Frame();
				
		if( fade.NowEffId == EffectFade.EffId.FadeWait ){
			useSceneMgr.Next( new SceneGame(), false );
		}
				
		camRotY += 1.0f;
		if( camRotY >= 360.0f ){
			camRotY = 0.0f;
		}
		camInfo.GetPosture().SetLookAt(
					new Vector3( camRotX, camRotY, 0.0f ),
					new Vector3( 0.0f, 0.0f, 0.0f ),
					-200.0f );
				
		alphaVal += alphaAdd;
		if( alphaVal <= 0.0f ){
			alphaVal = 0.1f;
			alphaAdd = 0.1f;
		}
		if( alphaVal >= 1.0f ){
			alphaVal = 0.9f;
			alphaAdd = -0.1f;
		}
		Graphics2D.FindSprite( "PressAnyKey" ).Alpha = alphaVal;
		

				
		return true;
    }

    /// 描画処理
    public bool Render()
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		EffectFade fade = EffectFade.GetInstance();

        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();
				
		grid.Render();

		ground.Render();
#if DEBUG
		Graphics2D.AddSprite( "Ms", Fps.GetFpsTime()+"ms", 0xffffffff, 0, 0 );
#endif				
		Graphics2D.DrawSpritesUseAlpha();
				
		fade.Draw( Renderer.GetGraphicsDevice() );

        graphics.SwapBuffers();
#if DEBUG				
		Graphics2D.RemoveSprite( "Ms" );
#endif
        return true;
    }

}


} // end ns DefenseDemo

//===
// EOF
//===
