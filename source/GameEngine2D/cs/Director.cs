/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//#define ENABLE_PROFILE
//#define DEBUG_SCENE_TRANSITIONS

using System;
using System.Collections.Generic;

using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// @if LANG_EN
	/// <summary>
	/// The flag bits used by Director.Instance.DebugFlags. 
	/// By default they are all turned off.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>フラグのビットは Director.Instance.DebugFlags によって使用されます。
	/// デフォルトでは、全てオフです。
	/// </summary>
	/// @endif
	public static class DebugFlags
	{
		/// @if LANG_EN
		/// <summary>
		/// Draw each node's transform matrix as a red arrow + green arrow.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>それぞれのノードの変換行列を赤い矢印+緑の矢印として描画します。
		/// </summary>
		/// @endif
		public static uint DrawTransform = 1;
		/// @if LANG_EN
		/// <summary>
		/// Show the pivot for all nodes.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>すべてのノードの中心軸を表示します。
		/// </summary>
		/// @endif
		public static uint DrawPivot = 2;
		/// @if LANG_EN
		/// <summary>
		/// Show the content local bounds of nodes that defined GetlContentLocalBounds.
		/// Note that the content local bounds is transformed by the parent.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>GetlContentLocalBounds で定義されるノードの、コンテントローカル境界を表示します。
		/// コンテントローカル境界は親によって変換されることを注意してください。
		/// </summary>
		/// @endif
		public static uint DrawContentLocalBounds = 4;
		/// @if LANG_EN
		/// <summary>
		/// Show the content world bounds of nodes that defined GetlContentLocalBounds.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>GetlContentLocalBounds で定義されるノードの、コンテントワールド境界を表示します。
		/// </summary>
		/// @endif
		public static uint DrawContentWorldBounds = 8;
		/// @if LANG_EN
		/// <summary>
		/// Draw a debug world grid, with axis in black and rulers in grey.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>軸を黒、ルーラーを灰色で、デバッグワールドグリッドを描画します。
		/// </summary>
		/// @endif
		public static uint DrawGrid = 16;
		/// @if LANG_EN
		/// <summary>
		/// Enable debug camera navigation (workd with Camera2D only.)
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デバッグカメラナビゲーションを使用可にします(カメラ2Dのみ動作します)。
		/// </summary>
		/// @endif
		public static uint Navigate = 32;
		// enable runtime log (not very refined at the moment)
		internal static uint Log = 64;
	}

	/// @if LANG_EN
	/// <summary>
	/// The Director is a singleton (accessed via Director.Instance) that manages the scene stack 
	/// and calls the update loop of the Scheduler and ActionManager. Its Update/Render/PostSwap 
	/// functions must be called manually once in the user main loop, if you are managing the main
	/// loop yourself.
	/// It also holds a graphics context (GL), a SpriteRenderer object (that is mostly for internal
	/// used but that you can also use directly), and a DrawHelpers that is mosty used internally 
	/// when drawing debug information.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Director はシーンスタックを管理し、Scheduler と ActionManager の update ループを呼び出すシングルトンです。
	/// 開発者自身が独自にメインループを実装する場合、開発者はUpdate / Render / PostSwap 関数をメインループの中で呼び出さなければなりません。
	/// また Director は、グラフィックスコンテキスト (GL) 、SpriteRenderer オブジェクト ( 主に内部的に利用されますが、開発者が直接使うこともできます )、 デバッグ情報を描画する DrawHelpers を保持します。
	/// </summary>
	/// @endif
	public class Director : System.IDisposable
	{
		List< Scene > m_scenes_stack = new List< Scene >();
		Timer m_frame_timer;
		bool m_paused;
		bool m_run_with_scene_called;
		double m_elapsed;

		/// @if LANG_EN
		/// <summary>A timer that gets incremented everytime Update gets called.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update が呼ばれるたびにインクリメントされるタイマー。
		/// </summary>
		/// @endif
		public double DirectorTime { get{ return m_elapsed; } }

		/// @if LANG_EN
		/// <summary>The graphics context + matrix stack.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>グラフィックスコンテキスト + 行列スタック。
		/// </summary>
		/// @endif
		public GraphicsContextAlpha GL;

		/// @if LANG_EN
		/// <summary>The main SpriteRenderer object.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>メインの SpriteRenderer オブジェクト。
		/// </summary>
		/// @endif
		public SpriteRenderer SpriteRenderer;

		/// @if LANG_EN
		/// <summary>Some draw helpers for debug draw.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デバッグ描画のための描画ヘルパー。
		/// </summary>
		/// @endif
		public DrawHelpers DrawHelpers;

		/// @if LANG_EN
		/// <summary>Some debug flags to enable logging, debug draw, camera navigation etc.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ログ、デバッグ描画、カメラナビゲーションなどを有効にするデバッグフラグ。
		/// </summary>
		/// @endif
		public uint DebugFlags = 0;

//		private static readonly Director m_instance;
		static Director m_instance;

		/// @if LANG_EN
		/// <summary>The director singleton.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Director のシングルトン。
		/// </summary>
		/// @endif
		public static Director Instance
		{
			get { return m_instance;}
		}

		/// @if LANG_EN
		/// <summary>
		/// Initialize GameEngine2D
		/// </summary>
		/// <param name="sprites_capacity">The maximum number of sprites, passed to SpriteRenderer's constructor.</param>
		/// <param name="draw_helpers_capacity">The maximum number of vertices that we can use in DrawHelpers.</param>
		/// <param name="context">The core graphics context.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>GameEngine2D を初期化します。
		/// </summary>
		/// <param name="sprites_capacity">SpriteRenderer のコンストラクタに渡されるスプライトの最大数。</param>
		/// <param name="draw_helpers_capacity">DrawHelpers で使用する頂点の最大数。</param>
		/// <param name="context">コアグラフィックスコンテキスト。</param>
		/// @endif
		static public void Initialize( uint sprites_capacity = 500, uint draw_helpers_capacity = 400
								 , Sce.PlayStation.Core.Graphics.GraphicsContext context = null )
		{
			m_instance = new Director( sprites_capacity, draw_helpers_capacity, context );
			Scheduler.m_instance = new Scheduler();
			ActionManager.m_instance = new ActionManager();
		}

		/// @if LANG_EN
		/// <summary>Terminate GameEngine2D</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>GameEngine2D の終了処理。
		/// </summary>
		/// @endif
		static public void Terminate()
		{
			while ( m_instance.m_scenes_stack.Count != 0 )
				m_instance.pop_scene();

			Common.DisposeAndNullify< Director >( ref m_instance );
			
			if ( Scheduler.m_instance != null ) {
				Scheduler.m_instance.UnscheduleAll();
				Scheduler.m_instance = null;
			}
			
			if ( ActionManager.m_instance != null ) {
				ActionManager.m_instance.RemoveAllActions();
				ActionManager.m_instance = null;
			}
			
			TransitionFadeBase.Terminate();
			TransitionDirectionalFade.Terminate();
			ParticleSystem.Terminate();
		}

		/// @if LANG_EN
		/// <summary>Director constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Director コンストラクタ。
		/// </summary>
		/// @endif
		Director( uint sprites_capacity, uint draw_helpers_capacity
						 , Sce.PlayStation.Core.Graphics.GraphicsContext context )
		{
			m_paused = false;
			m_frame_timer = new Timer();
			m_run_with_scene_called = false;
			m_elapsed = 0.0;

			GL = new GraphicsContextAlpha( context );
			DrawHelpers = new DrawHelpers( GL, draw_helpers_capacity );
			SpriteRenderer = new SpriteRenderer( GL, 6 * sprites_capacity );
//			DebugFlags |= GameEngine2D.DebugFlags.Log;

			m_canceled_replace_scene = new HashSet< Scene >();
		}

		/// @if LANG_EN
		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Dispose の実装。
		/// </summary>
		/// @endif
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				Common.DisposeAndNullify< GraphicsContextAlpha >( ref GL );
				Common.DisposeAndNullify< DrawHelpers >( ref DrawHelpers );
				Common.DisposeAndNullify< SpriteRenderer >( ref SpriteRenderer );
			}
		}

//		[Conditional("DEBUG")]
		public void DebugLog( string text )
		{
//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & GameEngine2D.DebugFlags.Log ) != 0 )
				System.Console.WriteLine( System.String.Format( "{0:00000}", Common.FrameCount ) + " " + text );
//			#endif
		}

		void on_scene_exit( ref Scene scene )  
		{
			if ( scene == null ) return;
			#if DEBUG_SCENE_TRANSITIONS
			System.Console.WriteLine( Common.FrameCount + " on_scene_exit " + scene.DebugInfo() );
			#endif // #if DEBUG_SCENE_TRANSITIONS
			scene.OnExit();
		}

		void on_scene_enter( ref Scene scene )  
		{
			if ( scene == null ) return;
			#if DEBUG_SCENE_TRANSITIONS
			System.Console.WriteLine( Common.FrameCount + " on_scene_enter " + scene.DebugInfo() );
			#endif // #if DEBUG_SCENE_TRANSITIONS
			scene.OnEnter();
		}

		// this function is called everytime scene changes
		void on_scene_change( Scene previous_scene, Scene next_scene )
		{
//			if ( previous_scene!=null ) System.Console.WriteLine( "previous_scene " + previous_scene.DebugInfo()  );
//			if ( next_scene!=null ) System.Console.WriteLine( "next_scene " + next_scene.DebugInfo()  );

			Common.Assert( previous_scene != next_scene );

			bool prev_is_transition = previous_scene != null && previous_scene.IsTransitionScene();
			bool next_is_transition = next_scene != null && next_scene.IsTransitionScene();

			if ( !prev_is_transition && !next_is_transition )
			{
				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " 1 [ scene -> scene ]" );
				#endif // #if DEBUG_SCENE_TRANSITIONS

				on_scene_exit( ref previous_scene );
				on_scene_enter( ref next_scene );
			}
			else if ( !prev_is_transition && next_is_transition )
			{
				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " 2 [ scene -> transition ]" );
				System.Console.WriteLine( Common.FrameCount + " >>>>>>>>>>>>>> start transition" );
				#endif // #if DEBUG_SCENE_TRANSITIONS

				// initialize PreviousScene (TransitionScene sets it to null)
				((TransitionScene)next_scene).PreviousScene = previous_scene;

//				on_scene_exit( ref previous_scene ); // no! this will be called at the end of the transition, in case 3

				on_scene_enter( ref next_scene );// entering a transition scene
				on_scene_enter( ref ((TransitionScene)next_scene).NextScene );
			}
			else if ( prev_is_transition && !next_is_transition )
			{
				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " 3 [ transition -> scene ]" );
				if ( next_scene == null )
					System.Console.WriteLine( Common.FrameCount + " next_scene is null" );
				#endif // #if DEBUG_SCENE_TRANSITIONS

				// assume we are replacing the current transition by its own NextScene, ie:

				#if DEBUG_SCENE_TRANSITIONS
				if ( next_scene != ((TransitionScene)previous_scene).NextScene )
				{
					System.Console.WriteLine( Common.FrameCount + "  ((TransitionScene)previous_scene).NextScene = " +  ((TransitionScene)previous_scene).NextScene.DebugInfo() );
					System.Console.WriteLine( Common.FrameCount + "  -> next_scene = " + next_scene.DebugInfo() );
				}
				#endif // #if DEBUG_SCENE_TRANSITIONS

				// This; may be
//				Common.Assert( next_scene == ((TransitionScene)previous_scene).NextScene ); // https://npdev-ext.scei.co.jp/pssuite/bugzilla/process_bug.cgi

				on_scene_exit( ref ((TransitionScene)previous_scene).PreviousScene );
				on_scene_exit( ref previous_scene ); // leaving a transition scene

				// scene transition is on the way, when transition is necessary for the next scene
				if ( next_scene != ( ( TransitionScene ) previous_scene ).NextScene )
				{
					on_scene_enter( ref next_scene );
				}

				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " <<<<<<<<<<<<<< end transition" );
				#endif // #if DEBUG_SCENE_TRANSITIONS
			}
			else if ( prev_is_transition && next_is_transition )
			{
				// interrupting a transition by an other transition:

				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " 4 [ transition -> transition ]" );
				#endif // #if DEBUG_SCENE_TRANSITIONS

				((TransitionScene)next_scene).PreviousScene = ((TransitionScene)previous_scene).NextScene;

				// this transition scene is "cancelled", remove its ReplaceScene action from action manager
				((TransitionScene)previous_scene).cancel_replace_scene(); 
				// also remember that this scene has been cancelled in case the ReplaceScene was added to the event list we are executing *in this frame*
				m_canceled_replace_scene.Add( ((TransitionScene)previous_scene).NextScene ); 
				
				on_scene_exit( ref ((TransitionScene)previous_scene).PreviousScene );
				on_scene_exit( ref previous_scene ); // leaving a transition scene

				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " >>>>>>>>>>>>>> start *new* transition (" + ((TransitionScene)previous_scene).PreviousScene.DebugInfo() + " interrupted)" );
				#endif // #if DEBUG_SCENE_TRANSITIONS

				on_scene_enter( ref next_scene );// entering a transition scene
				on_scene_enter( ref ((TransitionScene)next_scene).NextScene );
			}
		}

		Scene get_top_scene()
		{
			if ( m_scenes_stack.Count == 0 ) return null;
			return m_scenes_stack[ m_scenes_stack.Count - 1 ];
		}

		// make this public?
		bool is_transitionning()
		{
			Scene cur = get_top_scene();
			if ( cur == null ) return false;
			return cur.IsTransitionScene();
		}

		void replace_scene( Scene new_scene )
		{
			#if DEBUG_SCENE_TRANSITIONS
			System.Console.WriteLine( "" );
			System.Console.WriteLine( Common.FrameCount + " replace_scene " + get_top_scene().DebugInfo() + " -> " + new_scene.DebugInfo() );
			#endif // #if DEBUG_SCENE_TRANSITIONS

			if ( m_canceled_replace_scene.Contains( new_scene ) )
			{
				#if DEBUG_SCENE_TRANSITIONS
				System.Console.WriteLine( Common.FrameCount + " => replace_scene cancelled because this scene has been interrupted by an other transition during this frame" );
				#endif // #if DEBUG_SCENE_TRANSITIONS
				return;
			}

			Common.Assert( m_scenes_stack.Count > 0, "Can't ReplaceScene: scene stack is empty" );
			Scene previous_scene = get_top_scene();
			m_scenes_stack[ m_scenes_stack.Count - 1 ] = new_scene;
			Common.Assert( previous_scene != new_scene );
			on_scene_change( previous_scene, new_scene );
		}

		void push_scene( Scene new_scene )
		{
			#if DEBUG_SCENE_TRANSITIONS
			System.Console.WriteLine( Common.FrameCount + " push_scene" );
			#endif // #if DEBUG_SCENE_TRANSITIONS

			Scene previous_scene = get_top_scene();
			m_scenes_stack.Add( new_scene );
			on_scene_change( previous_scene, new_scene );
		}

		void pop_scene()
		{
			#if DEBUG_SCENE_TRANSITIONS
			System.Console.WriteLine( Common.FrameCount + " pop_scene" );
			#endif // #if DEBUG_SCENE_TRANSITIONS

			Common.Assert( m_scenes_stack.Count > 0, "Can't PopScene: scene stack is empty" );
			Scene previous_scene = get_top_scene();

			m_scenes_stack.RemoveAt( m_scenes_stack.Count - 1 );
			on_scene_change( previous_scene, get_top_scene() );
		}

		/// @if LANG_EN
		/// <summary>Get the currently running Scene object.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在実行している Scene オブジェクトを取得します。
		/// </summary>
		/// @endif
		public Scene CurrentScene { get { return get_top_scene();}}

		delegate void DSceneEvent();
		event DSceneEvent m_scene_events; // we can't execute scene events as we traverse, so store them as callbacks here
		HashSet< Scene > m_canceled_replace_scene;

		/// @if LANG_EN
		/// <summary>Replace current scene by an other one.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在のシーンを他のシーンに置き換えます。
		/// </summary>
		/// @endif
		public void ReplaceScene( Scene new_scene )
		{
			m_scene_events += () => replace_scene( new_scene );
		}

		/// @if LANG_EN
		/// <summary>Push a new scene on the scene stack.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>新しいシーンをシーンスタックにプッシュします。
		/// </summary>
		/// @endif
		public void PushScene( Scene new_scene )
		{
			m_scene_events += () => push_scene( new_scene );
		}

		/// @if LANG_EN
		/// <summary>Pop the top scene on the scene stack (sets previous scene).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シーンスタック (前のシーン) のトップのシーンをポップします。
		/// </summary>
		/// @endif
		public void PopScene()
		{
			m_scene_events += () => pop_scene();
		}

		/// @if LANG_EN
		/// <summary>Pause everything.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>一時停止します。
		/// </summary>
		/// @endif
		public void Pause()
		{
			m_paused = true;
		}

		/// @if LANG_EN
		/// <summary>Resume everything.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>再開します。
		/// @endif
		public void Resume()
		{
			m_paused = false;
			m_frame_timer.Reset();
		}

		/// @if LANG_EN
		/// <summary>Print some debug information, content might vary in the future.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デバッグ情報を出力します(内容は将来変更される場合があります)。
		/// </summary>
		/// @endif
		public void Dump()
		{
			DebugLog( "CurrentScene.m_elapsed time = " + CurrentScene.m_elapsed );
			// print all nodes
			CurrentScene.Traverse( ( node, depth ) =>{ 
								   DebugLog( " " + new System.String(' ',depth*2) + node.DebugInfo() );
								 return true;
								 }, 0 );
		}

		// Update with delta time dt, can be called several times a frame.
		void internal_step( float dt )
		{
//			#if DEBUG
			if ( ( ( this.DebugFlags & Sce.PlayStation.HighLevel.GameEngine2D.DebugFlags.Navigate ) != 0 )
				 && ( CurrentScene != null ) )
			{
				bool zoom = Input2.GamePad0.Triangle.Down; // zoom = A key on PC
				bool pan = Input2.GamePad0.Square.Down; // pan = W key on PC

				CurrentScene.Camera.Navigate( pan ? 1 : zoom ? 2 : 0 );
			}
//			#endif

			//Common.Profiler.Push("Scheduler.Instance.Update");
			Scheduler.Instance.Update( dt );
			//Common.Profiler.Pop();

			//Common.Profiler.Push("ActionManager.Instance");
			ActionManager.Instance.Update( dt );
			//Common.Profiler.Pop();

			CurrentScene.m_elapsed += (double)dt;
			m_elapsed += (double)dt;
		}

		/// @if LANG_EN
		/// <summary>
		/// The main stepping function, that you must call once a frame.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>メインのステップ関数です。1フレームに1度呼び出す必要があります。</summary>
		/// @endif
		public void Update()
		{
			Common.Assert( m_run_with_scene_called, "RunWithScene hasn't been called" );
//			Common.Assert( CurrentScene != null );

			//Common.Profiler.Push("Director.Instance.Update");

			{
				// process pending scene events first

				m_canceled_replace_scene.Clear();

				if ( m_scene_events != null )
					m_scene_events();
				m_scene_events = null;
			}

			if ( CurrentScene == null )
			{
				//Common.Profiler.Pop();
				return;
			}

			if ( !m_paused )
			{
				float dt = (float)m_frame_timer.Seconds();
				m_frame_timer.Reset();

//				System.Console.WriteLine( "dt = " + dt );

				// the actuall stepping is isolated in internal_step(), so
				// that we can easily change the implementation to use an 
				// accumulator + constant stepping (calling internal_step() 
				// in a while loop), if needed.

				internal_step( dt );
			}

			//Common.Profiler.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// The main render function, that you must call once a frame.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>メインの描画関数です。1フレームに1度呼び出す必要があります。
		/// </summary>
		/// @endif
		public void Render()
		{
			//Common.Profiler.Push("Director.Instance.Render");
			if ( CurrentScene != null )	CurrentScene.render();
			else DebugLog( "no scene has been set, please call RunWithScene" );
			//Common.Profiler.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// A post swap callback that you must call after SwapBuffers().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スワップコールバック関数。SwapBuffers() の後に呼び出す必要があります。
		/// </summary>
		/// @endif
		public void PostSwap()
		{
			Common.OnSwap();

			#if ENABLE_PROFILE

			if ( Common.FrameCount % 20 == 0 )
			{
				Dump(); // DebugFlags.Log must be set
				//Common.Profiler.Dump();
				System.Console.WriteLine( "Director.Instance.DebugStats.DrawArraysCount " + Director.Instance.GL.DebugStats.DrawArraysCount );
//				Scheduler.Instance.Dump();
//				ActionManager.Instance.Dump();
			}

			//Common.Profiler.HeartBeat();

			#endif // #if ENABLE_PROFILE
		}

		/// @if LANG_EN
		/// <summary>
		/// This function must be called once by user, to tell the system which Scene object it should start with.
		/// </summary>
		/// <param name="scene">The scene to run.</param>
		/// <param name="manual_loop">Is set to true, the main loop won't be started (the user will have to implement it).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>システムにどの Scene オブジェクトをスタートさせるのか伝えるために、開発者はこの関数を1度呼び出さなければなりません。
		/// </summary>
		/// <param name="scene">実行するシーン。</param>
		/// <param name="manual_loop">trueをセットすると、メインループを開始しません ( 開発者はそれを実装する必要があります)。</param>
		/// @endif
		public void RunWithScene( Scene scene, bool manual_loop = false )
		{
			Common.Assert( CurrentScene == null );
			Common.Assert( !m_run_with_scene_called, "You can't call RunWithScene more than once." );

			PushScene( scene );

			m_run_with_scene_called = true;

			if ( manual_loop ) 
				return;

			for ( ; ; )
			{
				Sce.PlayStation.Core.Environment.SystemEvents.CheckEvents();

				Update();
				Render();

				// note: might want to Render first on Vita

				Director.Instance.GL.Context.SwapBuffers();
				PostSwap();
			}
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

