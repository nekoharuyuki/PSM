/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.Threading ;
using System.Diagnostics ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.HighLevel.Model ;

namespace Sample {

class BasicModelSample {

	static GraphicsContext graphics ;
	static BasicModel model ;
	static BasicProgram program ;

	static float turn ;

	static bool loop = true ;

	static void Main( string[] args ) {
		Init() ;
		while ( loop ) {
			SystemEvents.CheckEvents() ;
			Update() ;
			Draw() ;
		}
		Term() ;
	}

	static void Init() {
		graphics = new GraphicsContext() ;

		SampleDraw.Init( graphics ) ;
		SampleTimer.Init() ;

		//  load model

		model = new BasicModel( "/Application/walker.mdx", 0 ) ;
		program = new BasicProgram() ;
	}

	static void Term() {
		SampleDraw.Term() ;
		SampleTimer.Term() ;
		model.Dispose() ;
		program.Dispose() ;
		graphics.Dispose() ;
	}

	static void Update() {
		SampleDraw.Update() ;
	}

	static void Draw() {
		SampleTimer.StartFrame() ;

		Matrix4 proj = Matrix4.Perspective( FMath.Radians( 45.0f ), graphics.Screen.AspectRatio, 1.0f, 1000000.0f ) ;
		Matrix4 view = Matrix4.LookAt( new Vector3( 0.0f, 0.0f, 30.0f ),
										new Vector3( 0.0f, 0.0f, 0.0f ),
										new Vector3( 0.0f, 1.0f, 0.0f ) ) ;
		Vector3 litDirection = new Vector3( 1.0f, -1.0f, -1.0f ).Normalize() ;
		Vector3 litDirection2 = new Vector3( 0.0f, 1.0f, 0.0f ).Normalize() ;
		Vector3 litColor = new Vector3( 1.0f, 1.0f, 1.0f ) ;
		Vector3 litColor2 = new Vector3( 1.0f, 0.0f, 0.0f ) ;
		Vector3 litAmbient = new Vector3( 0.3f, 0.3f, 0.3f ) ;
		Vector3 fogColor = new Vector3( 0.0f, 0.5f, 1.0f ) ;

		BasicParameters parameters = program.Parameters ;
		parameters.Enable( BasicEnableMode.Lighting, true ) ;
		parameters.Enable( BasicEnableMode.Fog, true ) ;

		parameters.SetProjectionMatrix( ref proj ) ;
		parameters.SetViewMatrix( ref view ) ;
		parameters.SetLightCount( 2 ) ;
		parameters.SetLightDirection( 0, ref litDirection ) ;
		parameters.SetLightDiffuse( 0, ref litColor ) ;
		parameters.SetLightSpecular( 0, ref litColor ) ;
		parameters.SetLightDirection( 1, ref litDirection2 ) ;
		parameters.SetLightDiffuse( 1, ref litColor2 ) ;
		parameters.SetLightSpecular( 1, ref litColor2 ) ;
		parameters.SetLightAmbient( ref litAmbient ) ;
		parameters.SetFogRange( 10.0f, 50.0f ) ;
		parameters.SetFogColor( ref fogColor ) ;

		graphics.SetViewport( 0, 0, graphics.Screen.Width, graphics.Screen.Height ) ;
		graphics.SetClearColor( 0.0f, 0.5f, 1.0f, 0.0f ) ;
		graphics.Clear() ;

		graphics.Enable( EnableMode.Blend ) ;
		graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
		graphics.Enable( EnableMode.CullFace ) ;
		graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;
		graphics.Enable( EnableMode.DepthTest ) ;
		graphics.SetDepthFunc( DepthFuncMode.LEqual, true ) ;

		turn = FMath.Repeat( turn + SampleTimer.DeltaTime * 5.0f, 0.0f, 360.0f ) ;
		Matrix4 world = Matrix4.RotationY( FMath.Radians( turn ) ) ;

		//  adjust position

		if ( model.BoundingSphere.W != 0.0f ) {
			float scale = 10.0f / model.BoundingSphere.W ;
			world *= Matrix4.Scale( scale, scale, scale ) ;
			world *= Matrix4.Translation( -model.BoundingSphere.Xyz ) ;
		}

		//  select motion

		if ( model.Motions.Length > 1 ) {
			if ( SampleTimer.FrameCount % 120 == 0 ) {
				int next = ( model.CurrentMotion + 1 ) % model.Motions.Length ;
				model.SetCurrentMotion( next, 0.1f ) ;
			}
		}

		//  draw model

		model.SetWorldMatrix( ref world ) ;
		model.Animate( SampleTimer.DeltaTime ) ;
		model.Update() ;
		model.Draw( graphics, program ) ;

		graphics.Disable( EnableMode.CullFace ) ;
		graphics.Disable( EnableMode.DepthTest ) ;
		SampleDraw.DrawText( "BasicModel Sample", 0xffffffff, 0, 0 ) ;

		SampleTimer.EndFrame() ;
		graphics.SwapBuffers() ;
	}
}

} // namespace
