/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//#define SINGLE_SCENE

using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;	// Font

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using FeatureCatalogPlanetCute;
using FeatureCatalogIsometric;

static class FeatureCatalog
{
	static Camera2D UICamera;
	static FontMap UIFontMap;
	static FontMap LargeFontMap;
	
	const int NUM_SHARED_MENU_BUTTONS = 11;
	static Button[] CurrentSharedMenuButtons;

	public class Button
	: Label
	{
		public SpriteUV Background;
		public System.Action Action;

		static int count = 0;

		/// <summary>Return the Bounds2 object containing the text, in local space.</summary>
		public override bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			bounds = GetlContentLocalBounds();
			return true;
		}

		/// <summary>Return the Bounds2 object containing the text, in local space.</summary>
		new public Bounds2 GetlContentLocalBounds()
		{
			Bounds2 ret;
			if ( FontMap == null ) ret = Director.Instance.SpriteRenderer.DrawTextDebug( Text, Math._00, CharWorldHeight, false );
			else ret = Director.Instance.SpriteRenderer.DrawTextWithFontMap( Text, Math._00, CharWorldHeight, false, FontMap, Shader );
			ret.Max.X = ret.Min.X + 4.5f; // make all buttons the same size
			return ret;
		}

		public Button( Camera2D UICamera, FontMap fontmap )
		{
			Camera = (ICamera)UICamera;
			Color = Colors.White;
			HeightScale = Camera.GetPixelSize();
			BlendMode = BlendMode.Normal;
			FontMap = fontmap;

			Background = new SpriteUV(); // background rectangle
			Background.BlendMode = BlendMode.None;
			Background.TextureInfo = Director.Instance.GL.WhiteTextureInfo;

			++count;

			Schedule( ( dt ) =>

				{
					Color = Input2.Touch00.Down && IsWorldPointInsideContentLocalBounds( GetTouchPos() ) ? Colors.Cyan : Colors.White;

//					System.Console.WriteLine( Text + " " + Common.FrameCount + " " + count );
					
					if ( Input2.Touch00.Release && IsWorldPointInsideContentLocalBounds( GetTouchPos() ) )
					{
						if ( Action != null )
							Action();
					}

					Background.Color = Math.Lerp( Colors.Grey30, Colors.LightBlue, 0.3f );

					// sync background sprite position with text size
					Background.Position = Position;
					Background.Quad = new TRS( GetlContentLocalBounds() );
					Background.Camera = Camera;
				}
			);
		}
	}

	public static Button AddButton( Node scene, string text, ref Vector2 pos, System.Action action )
	{
		var b = new Button( UICamera, UIFontMap ) { Text = text};

		pos -= new Vector2( 0.0f, b.CharWorldHeight + ( 1.0f * b.HeightScale ) );

		b.Position = UICamera.CalcBounds().Point01 + pos;
		b.Action = action;
		scene.AddChild( b.Background );
		scene.AddChild( b );
		return b;
	}
	
	public static void StopCurrentSharedMenuButtonsAction()
	{
		for( int i = 0; i < NUM_SHARED_MENU_BUTTONS; i++ )
		{
			CurrentSharedMenuButtons[i].Action = null;	
		}
	}

	static int transition_mode = 0;

	static void ReplaceScene( Scene next_scene )
	{
		next_scene.RegisterDisposeOnExitRecursive();

		switch ( transition_mode )
		{
			case 0: Director.Instance.ReplaceScene( next_scene ); break;
			case 1: Director.Instance.ReplaceScene( new TransitionCrossFade( next_scene ){ Duration = 2.0f, Tween = (x) => Math.PowEaseOut( x, 3.0f )} ); break;
			case 2: Director.Instance.ReplaceScene( new TransitionSolidFade( next_scene ){ Duration = 1.0f, Tween = (x) => Math.PowEaseOut( x, 3.0f )} ); break;
			case 3: Director.Instance.ReplaceScene( new TransitionDirectionalFade( next_scene ){ Duration = 2.0f, Tween = (x) => Math.PowEaseOut( x, 3.0f )} ); break;
		}
	}

	static string get_transition_mode_string()
	{
		switch ( transition_mode )
		{
			case 0: return "transition: off";
			case 1: return "transition: fade";
			case 2: return "transition: solid";
			case 3: return "transition: directional";
		}

		return "?";
	}

	public static void AddSharedMenu( Scene scene )
	{
//		System.Console.WriteLine( "AddSharedMenu: ignored" );
//		return;

		var pos = new Vector2( 11.0f, 0.0f );

		CurrentSharedMenuButtons[0] = AddButton( scene, "test: Action", ref pos, () => { var next_scene = MakeTestActionsScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[1] = AddButton( scene, "test: Pivot", ref pos, () => { var next_scene = MakeTestPivotScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[2] = AddButton( scene, "test: SpriteList", ref pos, () => { var next_scene = MakeManySpritesInSpriteListScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[3] = AddButton( scene, "test: Sprites ", ref pos, () => { var next_scene = MakeManySpritesScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[4] = AddButton( scene, "test: Plane3D", ref pos, () => { var next_scene = MakeTestPlane3D(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[5] = AddButton( scene, "test: Game scene", ref pos, () => { var next_scene = MakeTestGameScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[6] = AddButton( scene, "test: Forest scene", ref pos, () => { var next_scene = MakeForestScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[7] = AddButton( scene, "test: Isometric scene", ref pos, () => { var next_scene = MakeIsometricScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[8] = AddButton( scene, "test: Particles", ref pos, () => { var next_scene = MakeParticlesScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		CurrentSharedMenuButtons[9] = AddButton( scene, "test: Multitouch", ref pos, () => { var next_scene = MakeMultiTouchScene(); StopCurrentSharedMenuButtonsAction(); AddSharedMenu( next_scene ); ReplaceScene( next_scene ); });
		
		{
			CurrentSharedMenuButtons[10] = AddButton( scene, get_transition_mode_string(), ref pos, () => { transition_mode=(transition_mode+1)%4; });
			CurrentSharedMenuButtons[10].Action += () => { CurrentSharedMenuButtons[10].Text = get_transition_mode_string(); };
		}
	}

	public static Scene MakeTestPivotScene()
	{
		var scene = new Scene() { Name = "Pivot test" };

		scene.Camera2D.SetViewFromHeightAndCenter( 8.0f, new Vector2(3,3) );

		var texture_info = Director.Instance.GL.WhiteTextureInfo;

		var sprite = new SpriteUV() { TextureInfo = texture_info};

		sprite.Position = new Vector2(2,3);

		sprite.Scale = new Vector2(2,2);
		sprite.Pivot = new Vector2(0.5f,0.5f);

		sprite.Color = new Vector4(0.0f, 0.0f, 1.0f, 0.5f);
		sprite.BlendMode = BlendMode.Normal;

		sprite.Schedule( ( dt ) =>

			{ 
				float period_in_seconds = 3.0f;
				float wave = ( ( 1.0f + FMath.Sin( (float)Director.Instance.CurrentScene.SceneTime * Math.Pi / period_in_seconds ) ) * 0.5f );

				sprite.Rotation = Vector2.Rotation( Math.Pi * 2.0f * wave );
				sprite.Scale = new Vector2( 1.0f + 2.0f * wave );
			} 
		);

//		System.Console.WriteLine( Director.Instance.GL.GetViewportf().Aspect );
		Vector2 bpos = new Vector2(0.0f,0.0f);

		AddButton( scene, "Pivot at 0,0", ref bpos, () => { sprite.Pivot = new Vector2(0.0f,0.0f);} );
		AddButton( scene, "Pivot at 0.5,0.5", ref bpos, () => { sprite.Pivot = new Vector2(0.5f,0.5f);} ) ;

		scene.AdHocDraw += () => 

		{
			Director.Instance.GL.ModelMatrix.Push();
			Director.Instance.GL.ModelMatrix.SetIdentity(); // go in world space

			Director.Instance.GL.SetBlendMode( BlendMode.None );
			Director.Instance.DrawHelpers.SetColor( Colors.Yellow );
			Director.Instance.DrawHelpers.DrawDisk( sprite.LocalToWorld( sprite.Pivot ), 0.05f, 16 );

			Director.Instance.GL.SetBlendMode( BlendMode.Normal );
			Director.Instance.SpriteRenderer.DefaultFontShader.SetColor( ref Colors.Yellow );
			Director.Instance.SpriteRenderer.DrawTextDebug( ".Pivot=" +  sprite.Pivot.ToString() , sprite.LocalToWorld( sprite.Pivot ) + new Vector2(0.05f,0.05f)
												  , scene.Camera2D.GetPixelSize() * EmbeddedDebugFontData.CharSizei.Y );

			Director.Instance.GL.ModelMatrix.Pop();
		};

		scene.AddChild( sprite, -1 );

		return scene;
	}

	public static Scene MakeTestActionsScene()
	{
		TextureInfo Characters = new TextureInfo( new Texture2D( "/Application/data/PlanetCute/Objects.png", false ), new Vector2i(7,3) );

		float world_scale = 0.036f;

		var sprite = new SpriteTile();
		sprite.TextureInfo = Characters;
		sprite.TileIndex1D = 2;
		sprite.Quad.S = sprite.CalcSizeInPixels() * world_scale; 
		sprite.CenterSprite();
		sprite.Color = new Vector4(1.0f,1.0f,1.0f,0.75f);
		sprite.BlendMode = BlendMode.Normal;

		var scene = new Scene() { Name = "Action tests" };

		scene.AddChild( sprite );

		var pos = new Vector2( 0.0f, 0.0f );

		int writing_color_tag = 333;
		int writing_position_tag = 65406;

		AddButton( scene, "MoveTo (0,0)", ref pos, () =>{ sprite.RunAction( new MoveTo( new Vector2(0.0f,0.0f), 0.1f ));});
		AddButton( scene, "MoveBy (3,0)", ref pos, () =>{ sprite.RunAction( new MoveBy( new Vector2(3.0f,0.0f), 0.1f ));});
		AddButton( scene, "ScaleBy (2,2)", ref pos, () =>{ sprite.RunAction( new ScaleBy( new Vector2(2.0f,2.0f), 0.1f ));});
		AddButton( scene, "ScaleBy (0.5,0.5)", ref pos, () =>{ sprite.RunAction( new ScaleBy( new Vector2(0.5f,0.5f), 0.1f ));});

		AddButton( scene, "TintTo White", ref pos, () =>
			{  
				// prevent from running an other action that writes the color
				if ( sprite.GetActionByTag( writing_color_tag ) != null )
					sprite.StopActionByTag( writing_color_tag );

				sprite.RunAction( new TintTo( Math.SetAlpha( Colors.White, 0.75f ), 2.0f ) { Tag = writing_color_tag }  );
			}
		);

		AddButton( scene, "TintTo Blue", ref pos, () =>
			{ 
				// prevent from running an other action that writes the color
				if ( sprite.GetActionByTag( writing_color_tag ) != null )
					sprite.StopActionByTag( writing_color_tag );

				sprite.RunAction( new TintTo( Math.SetAlpha( Colors.Blue, 0.75f ), 2.0f ) { Tag = writing_color_tag } );
			}
		);

		AddButton( scene, "Pingpong colors", ref pos, () =>

			{
				// prevent from running an other action that writes the color
				if ( sprite.GetActionByTag( writing_color_tag ) != null )
					sprite.StopActionByTag( writing_color_tag );

				var action12 = new TintTo( Colors.Green, 0.5f )
				{ 
					Tween = (t) => FMath.Sin(t*Math.Pi*0.5f),
				};

				var action13 = new TintTo( Colors.Magenta, 0.5f )
				{
					Tween = (t) => FMath.Sin(t*Math.Pi*0.5f),
				};

				var seq = new Sequence();
				seq.Add( action12 );
				seq.Add( action13 );
				var repeat0 = new RepeatForever(){ InnerAction = seq, Tag = writing_color_tag };
				sprite.RunAction( repeat0 );
			} 
		);

		AddButton( scene, "Pingpong position", ref pos, () =>

			{
				// prevent from running the same action twice
			    // (we could also just hold an Action object somewhere and re-run, so that this check wouldn't be needed) 
				if ( sprite.GetActionByTag( writing_position_tag ) != null )
					sprite.StopActionByTag( writing_position_tag );

				var action12 = new MoveTo( new Vector2(-5,0), 0.5f )
				{ 
					Tween = (t) => FMath.Sin(t*Math.Pi*0.5f),
				};

				var action13 = new MoveTo( new Vector2(5,0), 0.5f )
				{
					Tween = (t) => FMath.Sin(t*Math.Pi*0.5f),
				};

				var seq = new Sequence();
				seq.Add( action12 );
				seq.Add( action13 );
				var repeat0 = new RepeatForever(){ InnerAction = seq, Tag = writing_position_tag };

				sprite.RunAction( repeat0 );
			} 
		);

		AddButton( scene, "StopAllActions", ref pos, () => sprite.StopAllActions() );

		scene.RegisterDisposeOnExit( (System.IDisposable)Characters );

		return scene;
	}

	public static Scene MakeForestScene()
	{
//		System.Console.WriteLine( "MakeForestScene" );

		var scene = new Scene() { Name = "Forest" };

		TextureInfo ObjectsMap = new TextureInfo( new Texture2D( "/Application/data/PlanetCute/Objects.png", false ), new Vector2i(7,3) );
		Vector2i[] tree_indexes = new Vector2i[3];
		tree_indexes[0] = PlanetCute.TreeShort;
		tree_indexes[1] = PlanetCute.TreeTall;
		tree_indexes[2] = PlanetCute.TreeUgly;

		Math.RandGenerator rgen = new Math.RandGenerator();

		Bounds2 bounds = scene.Camera2D.CalcBounds();
		Vector2i numcells = new Vector2i(7,4);
		Vector2 cellsize = bounds.Size / numcells.Vector2();
		System.Random rnd1 = new System.Random();

		SpriteList sprite_list = new SpriteList( ObjectsMap );

		// we are going to put a tree at a random location inside each cell of a regular grid
		for ( int x=0; x<numcells.X; ++x )
		{
			// generate rows of tree top to bottom, for draw order
			for ( int y=numcells.Y-1; y>=0; --y ) 
			{
				Vector2 cellindexf = new Vector2((float)x,(float)y);
				var sprite = new SpriteTile();
//				sprite.TextureInfo = tree_tex[rnd1.Next(3)];
				sprite.TextureInfo = ObjectsMap;
				sprite.TileIndex2D = tree_indexes[rnd1.Next(3)];

				// bounds for one cell
				Bounds2 gen_bounds = new Bounds2( bounds.Min + cellindexf * cellsize, 
												  bounds.Min + cellindexf * cellsize + cellsize );

				// scale gen_bounds to countrols irregularity
				gen_bounds = gen_bounds.Scale( new Vector2(0.6f), gen_bounds.Center );

				// pick up a random point in that cell
				sprite.Position = gen_bounds.Min + gen_bounds.Size * rgen.NextVector2( Math._00, Math._11 );

				// make the size of the tree match the size of cells, preserve texture ratio
				Vector2 aspect = sprite.CalcSizeInPixels() / sprite.CalcSizeInPixels().X;
				sprite.Quad.S = cellsize.X * aspect; 

				// make the sprite bottom center point be the new pivot (for Skew)
				sprite.Quad.Centering( TRS.Local.BottomCenter );

				// make the trees move in the wind
				sprite.Schedule( (dt) => { 

					int hc = sprite.GetHashCode();
					System.Random rnd2 = new System.Random(hc);

					sprite.Skew = new Vector2( Math.Deg2Rad( 1.0f ) * FMath.Sin( (float)Director.Instance.DirectorTime * 1.0f * rnd2.Next(4096)/4096.0f ), 
											   Math.Deg2Rad( 2.0f ) * FMath.Sin( (float)Director.Instance.DirectorTime * 3.0f * rnd2.Next(4096)/4096.0f ) );

					} );

				sprite_list.AddChild( sprite );
			}
		}

		scene.Camera2D.Center += new Vector2(0.0f,2.0f);

		scene.AddChild( sprite_list );

		scene.RegisterDisposeOnExit( (System.IDisposable)ObjectsMap );

		return scene;
	}

	public static Scene MakeParticlesScene()
	{
		var scene = new Scene() { Name = "Particles" };

		float scale = 1.5f;

		var body  = new SpriteUV( new TextureInfo( new Texture2D( "/Application/data/arrow.png", false ), new Vector2i(4,4) ) );
		body.CenterSprite( TRS.Local.BottomCenter );
		body.Scale = body.TextureInfo.TextureSizef * scene.Camera.GetPixelSize() * scale;
		body.BlendMode = BlendMode.Additive;

		scene.AddChild( body );

		body.Schedule( ( dt ) => { 

			if ( Input2.Touch00.Down )
			{
				Vector2 touch_pos = Director.Instance.CurrentScene.Camera.GetTouchPos();
				if ( ( body.Position - touch_pos ).Length() > 0.01f )
					body.Rotation = Math.LerpUnitVectors( body.Rotation, Math.Perp( ( body.Position - touch_pos ).Normalize( )), 0.3f );
				body.Position = Math.Lerp( body.Position, touch_pos, 0.3f );
			}

			body.Position += Input2.GamePad0.Dpad * 0.1f;

		}  );

		Particles fire_node= new Particles( 300 );
		ParticleSystem fire = fire_node.ParticleSystem;
		fire.TextureInfo = new TextureInfo( new Texture2D( "/Application/data/fire.png", false ) );
//		fire.BlendMode = BlendMode.PremultipliedAlpha;

		fire_node.Schedule( ( dt ) => 

			{
				fire.Emit.Transform = fire_node.GetWorldTransform();        
				fire.Emit.TransformForVelocityEstimate = fire_node.GetWorldTransform();
				fire.RenderTransform = Director.Instance.CurrentScene.GetWorldTransform(); // most probably identity
			}

			, -1 );

		fire_node.AdHocDraw += () => 

			{
				// debug draw the emission rectangle in pink

				Director.Instance.GL.SetBlendMode( BlendMode.Additive );
				Director.Instance.DrawHelpers.SetColor( Colors.Pink * 0.3f );
				Director.Instance.DrawHelpers.DrawBounds2( Bounds2.QuadMinus1_1.Scale( fire.Emit.PositionVar, Math._00 ) );
			};

		fire.Emit.Velocity = new Vector2( 0.0f, -2.0f );
		fire.Emit.VelocityVar = new Vector2( 0.2f, 0.3f );
		fire.Emit.ForwardMomentum = 0.2f;
		fire.Emit.AngularMomentun = 0.0f;
		fire.Emit.LifeSpan = 0.4f;
		fire.Emit.WaitTime = 0.005f;
		float s = 0.25f;
		fire_node.Position = new Vector2(0.0f,-s);
		fire.Emit.PositionVar = new Vector2(s,s);
		fire.Emit.ColorStart = Colors.Red;
		fire.Emit.ColorStartVar = new Vector4(0.0f,0.0f,0.0f,0.0f);
		fire.Emit.ColorEnd = Colors.White;
		fire.Emit.ColorEndVar = new Vector4(0.2f,0.0f,0.0f,0.0f);
		fire.Emit.ScaleStart = 0.3f * scale;
		fire.Emit.ScaleStartRelVar = 0.2f;
		fire.Emit.ScaleEnd = 0.6f * scale;
		fire.Emit.ScaleEndRelVar = 0.2f;
		fire.Simulation.Fade = 0.3f;

		body.AddChild( fire_node );

		Particles smoke_node = new Particles( 300 );
		ParticleSystem smoke = smoke_node.ParticleSystem;
		smoke.TextureInfo = new TextureInfo( new Texture2D( "/Application/data/fire.png", false ) );
//		smoke.BlendMode = BlendMode.PremultipliedAlpha;

		smoke_node.Schedule( ( dt ) => 

			{
				smoke.Emit.Transform = smoke_node.GetWorldTransform();        
				smoke.Emit.TransformForVelocityEstimate = smoke_node.GetWorldTransform();
				smoke.RenderTransform = Director.Instance.CurrentScene.GetWorldTransform(); // most probably identity
			}

			, -1 );

		smoke_node.AdHocDraw += () => 

			{
				// debug draw the emission rectangle in pink

				Director.Instance.GL.SetBlendMode( BlendMode.Additive );
				Director.Instance.DrawHelpers.SetColor( Colors.Pink * 0.3f );
				Director.Instance.DrawHelpers.DrawBounds2( Bounds2.QuadMinus1_1.Scale( smoke.Emit.PositionVar, Math._00 ) );
			};

		smoke.Emit.Velocity = new Vector2( 0.0f, -2.0f );
		smoke.Emit.VelocityVar = new Vector2( 0.2f, 0.3f );
		smoke.Emit.ForwardMomentum = 0.2f;
		smoke.Emit.AngularMomentun = 0.0f;
		smoke.Emit.LifeSpan = 2.4f;
		smoke.Emit.WaitTime = 0.02f;
		
		smoke_node.Position = new Vector2(0.0f,-s);
		smoke.Emit.PositionVar = new Vector2(s,s);
		smoke.Emit.ColorStart = Colors.Black;
		smoke.Emit.ColorStartVar = new Vector4(0.0f,0.0f,0.0f,0.0f);
		smoke.Emit.ColorEnd = Colors.Grey30;
		smoke.Emit.ColorEndVar = new Vector4(0.2f,0.0f,0.0f,0.0f);
		smoke.Emit.ScaleStart = 0.3f * scale * 3.0f;
		smoke.Emit.ScaleStartRelVar = 0.2f;
		smoke.Emit.ScaleEnd = 0.6f * scale *4.0f;
		smoke.Emit.ScaleEndRelVar = 0.2f;
		smoke.Simulation.Fade = 0.3f;

		body.AddChild( smoke_node );

		scene.RegisterDisposeOnExit( (System.IDisposable)fire );
		scene.RegisterDisposeOnExit( (System.IDisposable)smoke );
		scene.RegisterDisposeOnExit( (System.IDisposable)body.TextureInfo );

		return scene;
	}

	public static Scene MakeManySpritesScene()
	{
		var tex1 = new TextureInfo( new Texture2D( "/Application/data/slime_green_frames.png", false )
													, new Vector2i(4,4) );

		var scene = new Scene() { Name = "many SpriteUV scene" };

		Bounds2 bounds = scene.Camera.CalcBounds();

		Vector2i num_cells = new Vector2i( 8,8 );
		Vector2 cell_size = bounds.Size / num_cells.Vector2();

		Math.RandGenerator rgen = new Math.RandGenerator();
		System.Random random = new System.Random();

		for ( int y=0; y < num_cells.Y; ++y )
		{
			for ( int x=0; x < num_cells.X; ++x )
			{
				Vector2 uv = new Vector2( (float)x, (float)y ) / num_cells.Vector2();

				var sprite = new SpriteTile() 
				{ 
					TextureInfo = tex1 
					, Color = Math.SetAlpha( new Vector4(0.5f) + rgen.NextVector4( Math._0000, Math._1111 ) * 0.5f, 0.75f )
					, BlendMode = BlendMode.None
					, TileIndex2D = new Vector2i( random.Next( 0, 4 ) ,
												random.Next( 0, 4 ) )
				};

				Vector2 p = bounds.Min + bounds.Size * uv;

				// init position/size
				sprite.Position = p;
				sprite.Quad.S = cell_size; // note: we don't want to touch the Node.Scale here
				sprite.Pivot = sprite.Quad.S * 0.5f;

				sprite.Schedule( ( dt ) =>
					{
						sprite.Rotation = sprite.Rotation.Rotate( Math.Deg2Rad( 1.0f ) );
						sprite.Rotation = sprite.Rotation.Normalize();

						if ( Common.FrameCount % 8 == 0 )
							sprite.TileIndex1D = ( sprite.TileIndex1D + 1 ) % 16;
					} );

				scene.AddChild( sprite );
			}
		}

		scene.RegisterDisposeOnExit( (System.IDisposable)tex1 );

		return scene;
	}

	public static Scene MakeManySpritesInSpriteListScene()
	{
		var tex1 = new TextureInfo( new Texture2D( "/Application/data/water_puddle.png", false )
													, new Vector2i(6,1) );

		var scene = new Scene() { Name = "many SpriteTile in SpriteList scene" };

		Bounds2 bounds = scene.Camera.CalcBounds();

		Vector2i num_cells = new Vector2i( 16,8 );
		Vector2 cell_size = bounds.Size / num_cells.Vector2();

		Math.RandGenerator rgen = new Math.RandGenerator();
		System.Random random = new System.Random();

		SpriteList sprite_list = new SpriteList( tex1 ){ 
			BlendMode = BlendMode.None
		};

		sprite_list.EnableLocalTransform = true;

		scene.AddChild( sprite_list );

		for ( int y=0; y < num_cells.Y; ++y )
		{
			for ( int x=0; x < num_cells.X; ++x )
			{
				Vector2 uv = new Vector2( (float)x, (float)y ) / num_cells.Vector2();

				var sprite = new SpriteTile()
				{
					// all those properties (TextureInfo, Color, BlendMode) will be ignored 
					// if sprite is used in SpriteList
					TextureInfo = tex1
					, Color = Math.SetAlpha( new Vector4(0.8f) + rgen.NextVector4( Math._0000, Math._1111 ) * 0.2f, 0.75f)
					, BlendMode = BlendMode.Normal
					// TileIndex2D can still be set per sprite
					, TileIndex2D = new Vector2i( random.Next( 0, 6 ), 0 )
				};

				Vector2 p = bounds.Min + bounds.Size * uv;

				// init position/size
				if ( sprite_list.EnableLocalTransform )
				{
					// use .Position as we did in MakeManySpritesScene 
					sprite.Position = p;
				}
				else
				{
					// .Position will be ignored, so use the geometry directly
					sprite.Quad.T = p;
				}
				sprite.Quad.S = cell_size; // note: we don't want to touch the Node.Scale here
				sprite.Pivot = sprite.Quad.S * 0.5f;

				sprite.Schedule( ( dt ) =>
					{
						sprite.Rotation = sprite.Rotation.Rotate( Math.Deg2Rad( 1.0f ) );
						sprite.Rotation = sprite.Rotation.Normalize();

						// if you turn EnableLocalTransform off for performances, you can 
						// still move the sprite freely by changing its geometry directly:

//  					sprite.Quad.R = sprite.Quad.R.Rotate( Math.Deg2Rad( 1.0f ) );
//  					sprite.Quad.R = sprite.Quad.R.Normalize();
// 
						if ( Common.FrameCount % 7 == 0 )
							sprite.TileIndex1D = ( sprite.TileIndex1D + 1 ) % 6;
					} );

				sprite_list.AddChild( sprite );
			}
		}

		scene.RegisterDisposeOnExit( (System.IDisposable)tex1 );

		return scene;
	}

	public class SceneWith3DCameraAndPlane 
	: Scene
	{
		Camera2D m_camera2d;
		Camera3D m_camera3d = new Camera3D( Director.Instance.GL, Director.Instance.DrawHelpers );
		public Plane3D Plane = new Plane3D();

		public bool ToggleCameraTypeSignal = false;

		public SceneWith3DCameraAndPlane()
		{
			m_camera2d = (Camera2D)base.Camera;

			m_camera3d.Frustum.Znear = 0.1f;
			m_camera3d.Frustum.Zfar = 1000.0f;

			ScheduleUpdate();

			AddChild( Plane );

			Plane.AdHocDraw += () => 

				{
					// show the camera bounds in blue (initial position matched 2d view)
					Director.Instance.DrawHelpers.SetColor( Colors.Blue );
					Director.Instance.DrawHelpers.DrawBounds2( Camera.CalcBounds() );

					if ( Input2.Touch00.Down )
					{
						// show the touch position / picking point as a green circle
						Director.Instance.DrawHelpers.SetColor( Colors.Green );
						Director.Instance.DrawHelpers.DrawCircle( Camera.GetTouchPos(), 0.3f, 32 );
					}
				};
		}

		bool Is3D { get { return(ICamera)base.Camera == (ICamera)m_camera3d;}}

		public override void Update( float dt )
		{
			if ( ToggleCameraTypeSignal )
			{
				if ( Is3D )	base.Camera = (ICamera)m_camera2d;
				else base.Camera = (ICamera)m_camera3d;

				ToggleCameraTypeSignal = false;
			}

			m_camera3d.SetAspectFromViewport();
			m_camera3d.SetFromCamera2D( m_camera2d ); // to have some navigation

			PitchRoll pitch_roll = new PitchRoll( new Vector2( Math.Deg2Rad( 15.0f ) * FMath.Sin( (float)Director.Instance.DirectorTime * 1.033f ) ,
															   Math.Deg2Rad( 15.0f ) * FMath.Sin( (float)Director.Instance.DirectorTime * 0.87f ) ) );

			Plane.ModelMatrix = Is3D ? pitch_roll.ToMatrix() : Matrix4.Identity;

			m_camera3d.TouchPlaneMatrix = Plane.ModelMatrix;
		}
	}

	public class ChainedLabel
		: Label
	{
		public ChainedLabel Target;
		public Label Shadow;
		Vector2 m_center = Math._00;
		float m_radius = 5.0f;
		float m_radius_target = 5.0f;
		bool m_dragged = false;
		Vector2 m_drag_start;
		Vector2 m_drag_start_center;

		public ChainedLabel( ChainedLabel target, Scene scene, Plane3D parent, float alpha, int order )
		: base()
		{
			if ( alpha != 0.0f )
				alpha = Math.Lerp( 0.2f, 1.0f, alpha );

			FontMap = LargeFontMap;
			HeightScale = scene.Camera.GetPixelSize();
			Color = Math.Lerp( Colors.White, Colors.Grey60, alpha );

			VertexZ = 1.0f;

			Text = "Label.VertexZ=" + VertexZ;

			Shadow = new Label() { FontMap = LargeFontMap, Color = Colors.Grey05 };
			Shadow.HeightScale = HeightScale;
			Shadow.Text = Text;

			Target = target;

			Schedule( ( dt ) =>

				{ 
					if ( Target == null )
					{
						// head node

						this.Position = m_center + m_radius * new Vector2( FMath.Cos( (float)Director.Instance.DirectorTime * 0.5f * 2.033f ),
																		   FMath.Cos( (float)Director.Instance.DirectorTime * 0.5f * 0.98033f + 0.5f ) );

						Vector2 touch_pos = Director.Instance.CurrentScene.Camera.GetTouchPos();

						// you can drag the Label

						if ( Input2.Touch00.Down )
						{
							if ( Input2.Touch00.Press
								 // not that since we set VertexZ and have used a perspective in this particular test scene,  
								 // the picking here won't be very precise
								 && this.IsWorldPointInsideContentLocalBounds( touch_pos ) )
							{
								m_dragged = true;
								m_drag_start = touch_pos;
								m_center = this.Position; // this is our new position, we zero the cosine offset too
								m_radius = 0.0f;
								m_drag_start_center = m_center;
							}

							if ( m_dragged )
							{
								m_center = m_drag_start_center + ( touch_pos - m_drag_start );
								m_radius = 0.0f;
							}
						}
						else
						{
							m_dragged = false;
							m_radius += ( m_radius_target - m_radius ) * 0.001f;
						}
					}
					else
					{
						// follower node

						this.Position += 0.08f * ( Target.Position - this.Position );
					}

					this.Shadow.Position = this.Position;
				} 
			);

			parent.AddChild( Shadow, -1 );
			parent.AddChild( this, order );
		}
	}

	public static Scene MakeTestPlane3D()
	{
		var scene = new SceneWith3DCameraAndPlane();

		var camera_toggle_button = new Label() 
		{ 
			Position = new Vector2( -14.0f, 0.0f ),
			FontMap = UIFontMap
		};

		scene.Plane.AddChild( camera_toggle_button );

		{
			int n=5;
			ChainedLabel prevnode = null;
			for ( int i=0; i < n; ++i )
			{
				var node = new ChainedLabel( prevnode, scene, scene.Plane, n==1?0.0f:(float)i/(float)(n-1), n==1?0:n-1-i );
				prevnode = node;
			}
		}

		camera_toggle_button.Schedule( ( dt ) => 

			{ 
				// update text size based on current view
				camera_toggle_button.HeightScale = Director.Instance.CurrentScene.Camera.GetPixelSize();
				camera_toggle_button.Color = Colors.White;

				if ( Input2.Touch00.Press && camera_toggle_button.IsWorldPointInsideContentLocalBounds( camera_toggle_button.GetTouchPos() )  )
				{
					camera_toggle_button.Color = Colors.Green;
					scene.ToggleCameraTypeSignal = true;
				}

				camera_toggle_button.Text = "Click to toggle camera type\ncurrent: " + Director.Instance.CurrentScene.Camera.GetType().ToString();

				camera_toggle_button.Color = Input2.Touch00.Down && camera_toggle_button.IsWorldPointInsideContentLocalBounds( camera_toggle_button.GetTouchPos() )
				? Colors.Red
				: Colors.White; 
			} 
		);

		return scene;
	}

	public static Scene MakeTestGameScene()
	{
//		System.Console.WriteLine( "MakeTestGameScene" );

		var scene = new Scene() { Name = "GameScene" };

		scene.Camera2D.SetViewFromWidthAndCenter( 10.0f, new Vector2(3.5f,2.6f) );

		PlanetCute voxel_world = new PlanetCute( new Vector3i(7,7,6) );

		SpriteUV background = new SpriteUV();
		background = new SpriteUV(); // background rectangle
		background.BlendMode = BlendMode.None;
		background.TextureInfo = new TextureInfo( new Texture2D( "/Application/data/PlanetCute/bluegrad.png", false ) );
		scene.AddChild( background );

		scene.RegisterDisposeOnExit( (System.IDisposable)background.TextureInfo );
		
		background.Schedule( (dt) => {
			background.Quad = new TRS( scene.Camera2D.CalcBounds() );
    	} );

/* 
// ShadowSideWest test
		voxel_world.SetBlock( new Vector3i(0,1,0), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(0,0,1), PlanetCute.StoneBlock );
*/
/* 
// ShadowWest test
		voxel_world.SetBlock( new Vector3i(0,0,1), PlanetCute.StoneBlock );
    	voxel_world.SetBlock( new Vector3i(1,0,0), PlanetCute.StoneBlock );
*/
/*
		voxel_world.SetBlock( new Vector3i(0,0,1), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(1,0,1), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(2,0,1), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(0,1,1), PlanetCute.StoneBlock );

		voxel_world.SetBlock( new Vector3i(1,1,0), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(2,1,1), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(0,2,1), PlanetCute.StoneBlock );
		voxel_world.SetBlock( new Vector3i(1,2,1), PlanetCute.StoneBlock );
    	voxel_world.SetBlock( new Vector3i(2,2,1), PlanetCute.StoneBlock );
*/
		voxel_world.CreateRPGTestScene();

		scene.AddChild( voxel_world );
		
		return scene;
	}

	public static Scene MakeIsometricScene()
	{
		var scene = new Scene() { Name = "IsometricScene" };

		IsometricScene isometric_world = new IsometricScene( new Vector3i(20,20,6) );

		isometric_world.CreateTestScene();

		scene.Camera2D.SetViewFromWidthAndCenter( 8.0f // center camera on the center of the base grid of the voxel world:
												  , ( isometric_world.ToIso * ( isometric_world.NumCells.Xy.Vector2() * 0.5f ).Xy1 ).Xy );
		scene.AddChild( isometric_world );

		scene.RegisterDisposeOnExit( (System.IDisposable)isometric_world );

		return scene;
	}

	public class Trail<T>
	{
		T[] m_data;
		int m_next;
//		public uint AddCount;

		public Trail( int size )
		{
			m_data = new T[size];
//			AddCount = 0;
		}

		public int Count  { get{return m_data.Length;}}

		// At(0) is the most recently added, At(1) is second most recent etc.
		public T At( int i )
		{
			return m_data[ Common.WrapIndex( m_next-1-i, Count ) ];
		}

		public void Add( T value )
		{
//			++AddCount;
			m_data[m_next] = value;
			++m_next;
			if ( m_next==Count )
				m_next=0;
		}

		public T Front { get { return At(0);}}

		public void Fill( T value )
		{
			for ( int i=0; i < m_data.Length; ++i )
				m_data[i] = value;
//			AddCount += (uint)m_data.Length;
		}
	}

	// indexing for drawing a grid with a single triangle strip
	static int GridIndexing( Vector2i size, ushort[] indices = null )
	{
		int num = 0;
		int w = size.X;
		int h = size.Y;

		Common.Assert( size.Y > 1 );

		int index=0;

		for ( int j = h-1; j > 0; --j )
		{
			for ( int i=0; i < size.X; ++i )
			{
				++num; if ( indices != null ) indices[index++] = (ushort)(i + j * w);
				++num; if ( indices != null ) indices[index++] = (ushort)(i + ( j - 1 ) * w);
			}

			if ( j != 1 )
			{
				++num; if ( indices != null ) indices[index++] = (ushort)(( w - 1 ) + ( j - 1 ) * w);
				++num; if ( indices != null ) indices[index++] = (ushort)(            ( j - 1 ) * w);
			}
		}

		return num;
	}

	class Grid : System.IDisposable
	{
		Vector2i m_size;
		int m_num_vertices;
		int m_num_indices;
		VertexBuffer m_vertex_buffers;
		DrawHelpers.Vertex[] m_tmp_vertices; // tmp vertices array for writing everyframe

		public int NumVertices { get { return m_num_vertices; } }
		public Vector2i Size { get { return m_size; } }
		public VertexBuffer VertexBuffer { get { return m_vertex_buffers; } }
		public DrawHelpers.Vertex[] TmpVertices { get { return m_tmp_vertices; } }

		public Grid( Vector2i size )
		{
			m_size = size;
			m_num_vertices = size.Product();
			m_num_indices = GridIndexing( size, null );
			m_vertex_buffers = new VertexBuffer( m_num_vertices, m_num_indices, VertexFormat.Float4, VertexFormat.Float4 );
			ushort[] indices = new ushort[ m_num_indices ];
			GridIndexing( size, indices );
			m_vertex_buffers.SetIndices( indices, 0, 0, m_num_indices );
			m_tmp_vertices = new DrawHelpers.Vertex[ m_num_vertices ];
		}

		public void Draw()
		{
			Director.Instance.GL.Context.SetVertexBuffer( 0, m_vertex_buffers );
			Director.Instance.GL.Context.DrawArrays( DrawMode.TriangleStrip, 0, m_num_indices );
		}

		public void Dispose()
		{
			Common.DisposeAndNullify< VertexBuffer >( ref m_vertex_buffers ); 
		}
	}

	public static Scene MakeMultiTouchScene()
	{
		Trail<Vector2>[] touch = new Trail<Vector2>[ Input2.Touch.MaxTouch ];
		Grid[] trail_polys =  new Grid[ Input2.Touch.MaxTouch ];

		for ( int i=0; i < touch.Length; ++i )
			touch[i] = new Trail<Vector2>( 16 );

		Vector2i grid_size = new Vector2i(20,24);

		for ( int i=0; i < trail_polys.Length; ++i )
			trail_polys[i] = new Grid( grid_size );

		Vector2[] curve_points = new Vector2[ grid_size.Y ];
		Vector2[] circle_points = new Vector2[ grid_size.X ];
		for ( int i=0; i < circle_points.Length; ++i )
			circle_points[i] = Vector2.Rotation( Math.TwicePi * (float)i/(float)(circle_points.Length-1) );
		
		List<Vector2> control_points = new List<Vector2>();

		Vector4[] colors = {

//			Colors.Black,
			Colors.Red,
			Colors.Green,
			Colors.Yellow,
			Colors.Blue,
			Colors.Magenta,
			Colors.Cyan,
			Colors.White,
			Colors.Lime,
			Colors.LightBlue,
			Colors.Pink,
			Colors.Orange,
			Colors.LightCyan,
			Colors.Purple,
		};

		var scene = new Scene();
		
		{
			var info = new Label();
			info.Camera = UICamera;
			info.Position = UICamera.CalcBounds().Point00;
			info.FontMap = UIFontMap;
			info.HeightScale = UICamera.GetPixelSize();
			info.Text = "try touch the screen at multiple points at the same time";

			scene.AddChild( info );
		}

		scene.OnExitEvents += () => {

			foreach ( Grid grid in trail_polys )
				grid.Dispose();
			trail_polys = null;
		};

		scene.AdHocDraw += () => 

			{
				BlendFunc blend_func = Director.Instance.GL.Context.GetBlendFunc();
				DepthFunc depth_func = Director.Instance.GL.Context.GetDepthFunc();

				for ( int k=0; k < touch.Length; ++k )
				{
					bool down = Input2.Touch.GetData(0)[k].Down;
					bool press = Input2.Touch.GetData(0)[k].Press;
					Vector2 p = Director.Instance.CurrentScene.GetTouchPos(k);

					if ( !down )
						continue;

					// press means a new touch data has just been "created" 
					// -> fill the whole trail to the new position
					if ( press ) 
						touch[k].Fill( p );

					touch[k].Add( p );

					control_points.Clear();
					control_points.Add( touch[k].At( 0 ) );
					for ( int i=0; i < touch[k].Count; ++i )
						control_points.Add( touch[k].At(i) );

					{
						Vector4 start_color = colors[k];
						Vector4 end_color = Math.SetAlpha( start_color, 0.0f );
						float start_radius = 2.0f;
						float end_radius = 1.8f;

						Grid grid  = trail_polys[k];

						for ( int i=0; i < curve_points.Length; ++i )
							curve_points[i] = Curves.CatmullRom( (float)i/(float)(curve_points.Length-1), control_points, false ); 

						for ( int j=0; j < grid.Size.Y; ++j )
						{
							float y = (float)j/(float)(grid.Size.Y-1);

							for ( int i=0; i < grid.Size.X; ++i )
								grid.TmpVertices[i+j*grid.Size.X] = new DrawHelpers.Vertex( ( curve_points[j] + circle_points[i] * Math.Lerp( start_radius, end_radius, y ) ).Xy01
																							, Math.Lerp( start_color, end_color, y ) );
						}

						grid.VertexBuffer.SetVertices(grid.TmpVertices,0,0,grid.NumVertices);
						Director.Instance.GL.SetBlendMode( BlendMode.Normal );
						Director.Instance.DrawHelpers.ShaderPush();
						grid.Draw();

						{
							Director.Instance.DrawHelpers.ImmBegin( DrawMode.TriangleFan, (uint)grid.Size.X );
							for ( int i=0; i < grid.Size.X; ++i )
								Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( ( curve_points[0] + circle_points[i] * start_radius ).Xy01
																								 , start_color ) );
							Director.Instance.DrawHelpers.ImmEnd();
						}

						Director.Instance.DrawHelpers.ShaderPop();
					}
				}

				// restore states

				Director.Instance.GL.Context.Disable( EnableMode.DepthTest );

				Director.Instance.GL.Context.SetDepthFunc( depth_func );
				Director.Instance.GL.Context.SetBlendFunc( blend_func );
			};

		return scene;
	}

	static void Main( string[] args )
	{		
		Director.Initialize();

		Director.Instance.GL.Context.SetClearColor( Colors.Grey20 );

		UICamera = new Camera2D( Director.Instance.GL, Director.Instance.DrawHelpers );
		UICamera.SetViewFromWidthAndCenter( 16.0f, Math._00 );

		UIFontMap = new FontMap( new Font( FontAlias.System, 20, FontStyle.Bold ) );
		LargeFontMap = new FontMap( new Font( FontAlias.System, 48, FontStyle.Bold ) );

		CurrentSharedMenuButtons = new Button[NUM_SHARED_MENU_BUTTONS];

		Director.Instance.DebugFlags |= DebugFlags.Navigate; // press left alt + mouse to navigate in 2d space
		Director.Instance.DebugFlags |= DebugFlags.DrawGrid; // shows the default grid (expensive)
//		Director.Instance.DebugFlags |= DebugFlags.DrawContentWorldBounds;
//		Director.Instance.DebugFlags |= DebugFlags.DrawContentLocalBounds;
//		Director.Instance.DebugFlags |= DebugFlags.DrawTransform;
//		Director.Instance.DebugFlags |= DebugFlags.Log;

		var scene = new Scene();

		#if SINGLE_SCENE

//		scene = MakeManySpritesScene();
//		scene = MakeManySpritesInSpriteListScene();
//		scene = MakeTestGameScene();
//		scene = MakeForestScene();
		scene = MakeParticlesScene();
//		scene = MakeIsometricScene();

		#else
		
		AddSharedMenu( scene );

		#endif

		Director.Instance.RunWithScene( scene );
	}
}

