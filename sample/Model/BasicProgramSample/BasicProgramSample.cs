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

class BasicProgramSample {

	static GraphicsContext graphics ;
	static BasicProgram program ;
	static VertexBuffer vbuffer ;
	static Texture2D texture ;

	static float turn ;
	static float scroll ;

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

		program = new BasicProgram() ;
		vbuffer = new VertexBuffer( 67, 134,
			VertexFormat.Float3,    // POSITION
			VertexFormat.Float3,    // NORMAL
			VertexFormat.None,      // COLOR
			VertexFormat.Float2     // TEXCOORD
		) ;
		texture = new Texture2D( "/Application/basic.png", false ) ;
		texture.SetWrap( TextureWrapMode.Repeat ) ;

		vbuffer.SetVertices( vertices ) ;
		vbuffer.SetIndices( indices ) ;
	}

	static void Term() {
		SampleDraw.Term() ;
		SampleTimer.Term() ;
		program.Dispose() ;
		texture.Dispose() ;
		vbuffer.Dispose() ;
		graphics.Dispose() ;
	}

	static void Update() {
		SampleDraw.Update() ;
	}

	static void Draw() {
		SampleTimer.StartFrame() ;

		turn = FMath.Repeat( turn + SampleTimer.DeltaTime * 10.0f, 0.0f, 360.0f ) ;
		scroll = FMath.Repeat( scroll - SampleTimer.DeltaTime * 0.02f, 0.0f, 1.0f ) ;

		Matrix4 proj = Matrix4.Perspective( FMath.Radians( 45.0f ), graphics.Screen.AspectRatio, 1.0f, 1000000.0f ) ;
		Matrix4 view = Matrix4.LookAt( new Vector3( 0.0f, 0.0f, 20.0f ),
										new Vector3( 0.0f, 0.0f, 0.0f ),
										new Vector3( 0.0f, 1.0f, 0.0f ) ) ;
		Matrix4 world = Matrix4.RotationY( FMath.Radians( turn ) ) ;

		Vector3 litDirection = new Vector3( 1.0f, -1.0f, -1.0f ).Normalize() ;
		Vector3 litColor = new Vector3( 1.0f, 1.0f, 1.0f ) ;
		Vector3 litAmbient = new Vector3( 0.3f, 0.3f, 0.3f ) ;
		Vector3 fogColor = new Vector3( 0.0f, 0.5f, 1.0f ) ;
		Vector3 matDiffuse = new Vector3( 1.0f, 1.0f, 1.0f ) ;
		Vector3 matSpecular = new Vector3( 1.0f, 1.0f, 1.0f ) ;
		Vector3 matAmbient = new Vector3( 0.0f, 0.0f, 0.0f ) ;
		Vector3 matEmission = new Vector3( 0.0f, 0.0f, 1.0f ) ;

		BasicParameters parameters = program.Parameters ;
		parameters.Enable( BasicEnableMode.Lighting, true ) ;
		parameters.Enable( BasicEnableMode.Fog, true ) ;

		parameters.SetProjectionMatrix( ref proj ) ;
		parameters.SetViewMatrix( ref view ) ;
		parameters.SetWorldCount( 1 ) ;
		parameters.SetWorldMatrix( 0, ref world ) ;

		parameters.SetLightCount( 1 ) ;
		parameters.SetLightDirection( 0, ref litDirection ) ;
		parameters.SetLightDiffuse( 0, ref litColor ) ;
		parameters.SetLightSpecular( 0, ref litColor ) ;
		parameters.SetLightAmbient( ref litAmbient ) ;
		parameters.SetFogRange( 10.0f, 30.0f ) ;
		parameters.SetFogColor( ref fogColor ) ;

		parameters.SetMaterialDiffuse( ref matDiffuse ) ;
		parameters.SetMaterialSpecular( ref matSpecular ) ;
		parameters.SetMaterialAmbient( ref matAmbient ) ;
		parameters.SetMaterialEmission( ref matEmission ) ;
		parameters.SetMaterialOpacity( 1.0f ) ;
		parameters.SetMaterialShininess( 5.0f ) ;
		parameters.SetTexCoordOffset( 0, 0.0f, scroll, 1.0f, 1.0f ) ;

		graphics.SetViewport( 0, 0, graphics.Screen.Width, graphics.Screen.Height ) ;
		graphics.SetClearColor( 0.0f, 0.5f, 1.0f, 0.0f ) ;
		graphics.Clear() ;

		graphics.Enable( EnableMode.Blend ) ;
		graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
		graphics.Enable( EnableMode.CullFace ) ;
		graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;
		graphics.Enable( EnableMode.DepthTest ) ;
		graphics.SetDepthFunc( DepthFuncMode.LEqual, true ) ;

		graphics.SetShaderProgram( program ) ;
		graphics.SetVertexBuffer( 0, vbuffer ) ;
		graphics.SetTexture( 0, texture ) ;

		graphics.DrawArrays( DrawMode.TriangleStrip, 0, 68 ) ;
		graphics.DrawArrays( DrawMode.TriangleStrip, 68, 11, 6 ) ;

		graphics.Disable( EnableMode.CullFace ) ;
		graphics.Disable( EnableMode.DepthTest ) ;
		SampleDraw.DrawText( "BasicProgram Sample", 0xffffffff, 0, 0 ) ;
			
		SampleTimer.EndFrame() ;
		graphics.SwapBuffers() ;
	}

	static ushort[] indices = {
		0, 65, 1, 4, 2, 5, 26, 28, 39, 41, 52, 54, 66, 55, 56, 42, 43, 29, 30, 7, 9, 6, 8, 65, 10, 12, 11, 13, 31, 32, 44, 45, 57, 58, 66, 59, 60, 46, 47, 33, 34, 15, 17, 14, 16, 65, 18, 20, 19, 21, 35, 36, 48, 49, 61, 62, 66, 63, 64, 50, 51, 37, 38, 23, 25, 22, 24, 65,
		0, 1, 3, 2, 27, 26, 40, 39, 53, 52, 66, 
		65, 6, 4, 7, 5, 29, 28, 42, 41, 55, 54, 
		8, 10, 9, 11, 30, 31, 43, 44, 56, 57, 66, 
		65, 14, 12, 15, 13, 33, 32, 46, 45, 59, 58, 
		16, 18, 17, 19, 34, 35, 47, 48, 60, 61, 66, 
		65, 22, 20, 23, 21, 37, 36, 50, 49, 63, 62, 
	} ;
	static float[] vertices = {
		2.165063f, -4.330127f, -1.250000f, 0.459773f, -0.847435f, -0.265450f, 0.000000f, 0.833333f,
		1.250000f, -4.330127f, -2.165063f, 0.265450f, -0.847435f, -0.459773f, 0.083333f, 0.833333f,
		2.165064f, -2.500000f, -3.750000f, 0.437992f, -0.482342f, -0.758624f, 0.083333f, 0.666667f,
		3.750000f, -2.500000f, -2.165063f, 0.758624f, -0.482342f, -0.437992f, 0.000000f, 0.666667f,
		0.000000f, -4.330127f, -2.500000f, 0.000000f, -0.847435f, -0.530900f, 0.166667f, 0.833333f,
		0.000000f, -2.500000f, -4.330127f, 0.000000f, -0.482342f, -0.875983f, 0.166667f, 0.666667f,
		-1.250000f, -4.330127f, -2.165063f, -0.265450f, -0.847435f, -0.459773f, 0.250000f, 0.833333f,
		-2.165063f, -2.500000f, -3.750000f, -0.437992f, -0.482342f, -0.758624f, 0.250000f, 0.666667f,
		-2.165063f, -4.330127f, -1.250000f, -0.459773f, -0.847435f, -0.265450f, 0.333333f, 0.833333f,
		-3.750000f, -2.500000f, -2.165063f, -0.758624f, -0.482342f, -0.437992f, 0.333333f, 0.666667f,
		-2.500000f, -4.330127f, -0.000000f, -0.530900f, -0.847435f, 0.000000f, 0.416667f, 0.833333f,
		-4.330127f, -2.500000f, -0.000000f, -0.875983f, -0.482342f, -0.000000f, 0.416667f, 0.666667f,
		-2.165063f, -4.330127f, 1.250000f, -0.459773f, -0.847435f, 0.265450f, 0.500000f, 0.833333f,
		-3.750000f, -2.500000f, 2.165063f, -0.758624f, -0.482342f, 0.437992f, 0.500000f, 0.666667f,
		-1.250000f, -4.330127f, 2.165063f, -0.265450f, -0.847435f, 0.459773f, 0.583333f, 0.833333f,
		-2.165064f, -2.500000f, 3.750000f, -0.437992f, -0.482342f, 0.758624f, 0.583333f, 0.666667f,
		-0.000000f, -4.330127f, 2.500000f, -0.000000f, -0.847435f, 0.530900f, 0.666667f, 0.833333f,
		-0.000000f, -2.500000f, 4.330127f, -0.000000f, -0.482342f, 0.875983f, 0.666667f, 0.666667f,
		1.250000f, -4.330127f, 2.165063f, 0.265450f, -0.847435f, 0.459773f, 0.750000f, 0.833333f,
		2.165063f, -2.500000f, 3.750000f, 0.437991f, -0.482342f, 0.758624f, 0.750000f, 0.666667f,
		2.165063f, -4.330127f, 1.250000f, 0.459773f, -0.847435f, 0.265450f, 0.833333f, 0.833333f,
		3.750000f, -2.500000f, 2.165064f, 0.758624f, -0.482342f, 0.437992f, 0.833333f, 0.666667f,
		2.500000f, -4.330127f, 0.000000f, 0.530900f, -0.847435f, -0.000000f, 0.916667f, 0.833333f,
		4.330127f, -2.500000f, 0.000000f, 0.875983f, -0.482342f, -0.000000f, 0.916667f, 0.666667f,
		2.165063f, -4.330127f, -1.250000f, 0.459773f, -0.847435f, -0.265450f, 1.000000f, 0.833333f,
		3.750000f, -2.500000f, -2.165063f, 0.758624f, -0.482342f, -0.437992f, 1.000000f, 0.666667f,
		2.500000f, 0.000000f, -4.330126f, 0.500000f, 0.000000f, -0.866025f, 0.083333f, 0.500000f,
		4.330127f, 0.000000f, -2.500000f, 0.866025f, -0.000000f, -0.500000f, 0.000000f, 0.500000f,
		0.000000f, 0.000000f, -5.000000f, 0.000000f, -0.000000f, -1.000000f, 0.166667f, 0.500000f,
		-2.500000f, 0.000000f, -4.330127f, -0.500000f, 0.000000f, -0.866025f, 0.250000f, 0.500000f,
		-4.330126f, 0.000000f, -2.500000f, -0.866025f, 0.000000f, -0.500000f, 0.333333f, 0.500000f,
		-5.000000f, 0.000000f, -0.000000f, -1.000000f, 0.000000f, -0.000000f, 0.416667f, 0.500000f,
		-4.330127f, 0.000000f, 2.500000f, -0.866025f, 0.000000f, 0.500000f, 0.500000f, 0.500000f,
		-2.500000f, 0.000000f, 4.330127f, -0.500000f, 0.000000f, 0.866025f, 0.583333f, 0.500000f,
		-0.000000f, 0.000000f, 5.000000f, -0.000000f, 0.000000f, 1.000000f, 0.666667f, 0.500000f,
		2.500000f, 0.000000f, 4.330127f, 0.500000f, 0.000000f, 0.866025f, 0.750000f, 0.500000f,
		4.330127f, 0.000000f, 2.500000f, 0.866025f, 0.000000f, 0.500000f, 0.833333f, 0.500000f,
		5.000000f, 0.000000f, 0.000000f, 1.000000f, -0.000000f, -0.000000f, 0.916667f, 0.500000f,
		4.330127f, 0.000000f, -2.500000f, 0.866025f, -0.000000f, -0.500000f, 1.000000f, 0.500000f,
		2.165064f, 2.500000f, -3.750000f, 0.437992f, 0.482342f, -0.758624f, 0.083333f, 0.333333f,
		3.750000f, 2.500000f, -2.165063f, 0.758624f, 0.482342f, -0.437992f, 0.000000f, 0.333333f,
		0.000000f, 2.500000f, -4.330127f, 0.000000f, 0.482342f, -0.875983f, 0.166667f, 0.333333f,
		-2.165063f, 2.500000f, -3.750000f, -0.437992f, 0.482342f, -0.758624f, 0.250000f, 0.333333f,
		-3.750000f, 2.500000f, -2.165063f, -0.758624f, 0.482342f, -0.437992f, 0.333333f, 0.333333f,
		-4.330127f, 2.500000f, -0.000000f, -0.875983f, 0.482342f, -0.000000f, 0.416667f, 0.333333f,
		-3.750000f, 2.500000f, 2.165063f, -0.758624f, 0.482342f, 0.437992f, 0.500000f, 0.333333f,
		-2.165064f, 2.500000f, 3.750000f, -0.437992f, 0.482342f, 0.758624f, 0.583333f, 0.333333f,
		-0.000000f, 2.500000f, 4.330127f, -0.000000f, 0.482342f, 0.875983f, 0.666667f, 0.333333f,
		2.165063f, 2.500000f, 3.750000f, 0.437991f, 0.482342f, 0.758624f, 0.750000f, 0.333333f,
		3.750000f, 2.500000f, 2.165064f, 0.758624f, 0.482342f, 0.437992f, 0.833333f, 0.333333f,
		4.330127f, 2.500000f, 0.000000f, 0.875983f, 0.482342f, -0.000000f, 0.916667f, 0.333333f,
		3.750000f, 2.500000f, -2.165063f, 0.758624f, 0.482342f, -0.437992f, 1.000000f, 0.333333f,
		1.250000f, 4.330127f, -2.165063f, 0.265450f, 0.847435f, -0.459773f, 0.083333f, 0.166667f,
		2.165063f, 4.330127f, -1.250000f, 0.459773f, 0.847435f, -0.265450f, 0.000000f, 0.166667f,
		0.000000f, 4.330127f, -2.500000f, 0.000000f, 0.847435f, -0.530900f, 0.166667f, 0.166667f,
		-1.250000f, 4.330127f, -2.165063f, -0.265450f, 0.847435f, -0.459773f, 0.250000f, 0.166667f,
		-2.165063f, 4.330127f, -1.250000f, -0.459773f, 0.847435f, -0.265450f, 0.333333f, 0.166667f,
		-2.500000f, 4.330127f, -0.000000f, -0.530900f, 0.847435f, -0.000000f, 0.416667f, 0.166667f,
		-2.165063f, 4.330127f, 1.250000f, -0.459773f, 0.847435f, 0.265450f, 0.500000f, 0.166667f,
		-1.250000f, 4.330127f, 2.165063f, -0.265450f, 0.847435f, 0.459773f, 0.583333f, 0.166667f,
		-0.000000f, 4.330127f, 2.500000f, -0.000000f, 0.847435f, 0.530900f, 0.666667f, 0.166667f,
		1.250000f, 4.330127f, 2.165063f, 0.265450f, 0.847435f, 0.459773f, 0.750000f, 0.166667f,
		2.165063f, 4.330127f, 1.250000f, 0.459773f, 0.847435f, 0.265450f, 0.833333f, 0.166667f,
		2.500000f, 4.330127f, 0.000000f, 0.530900f, 0.847435f, -0.000000f, 0.916667f, 0.166667f,
		2.165063f, 4.330127f, -1.250000f, 0.459773f, 0.847435f, -0.265450f, 1.000000f, 0.166667f,
		0.000000f, -5.000000f, 0.000000f, -0.000000f, -1.000000f, 0.000000f, 0.500000f, 1.000000f,
		0.000000f, 5.000000f, 0.000000f, 0.000000f, 1.000000f, -0.000000f, 0.500000f, 0.000000f,
	} ;
}

} // namespace
