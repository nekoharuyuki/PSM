/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Imaging;

namespace DemoGame
{

/// デバック用：幾何図形の描画
/**
 * 使用するシェーダは以下の名前のuniform名が必要です。
 * IAmbientで直接色を変えています。
 * 「WorldViewProj」「a_Position」「IAmbient」
 */
public class RenderGeometry {

	private const int				debSphDiv = 20;

    private static ShaderProgram	debShader = null;
    private static int				debUIdWVP = -1;
	private MeshData				debMesh = null;
	private MeshData				debMesh2 = null;
    private VertexBuffer			debVb = null;
    private VertexBuffer			debVb2 = null;


	/// コンストラクタ
	public RenderGeometry()
    {
	}

	/// デストラクタ
	~RenderGeometry()
    {
		Dispose();
    }

	/// シェーダのセット
	public static void Init( string vshName, string fshName )
	{
        // vertex color shader
        debShader = new ShaderProgram( vshName, fshName );
		debShader.SetAttributeBinding( 0, "a_Position" );
		debUIdWVP = debShader.FindUniform( "WorldViewProj" );
	}

	/// シェーダの解放
	public static void Term()
	{
		if( debShader != null ){
			debShader.Dispose();
		}
		debShader	= null;
	}



/// public メソッド
///---------------------------------------------------------------------------

	public void Dispose()
	{
		if( debMesh != null ){
			debMesh.Dispose();
		}
		if( debVb != null ){
			debVb.Dispose();
		}
		if( debMesh2 != null ){
			debMesh2.Dispose();
		}
		if( debVb2 != null ){
			debVb2.Dispose();
		}

		debMesh		= null;
		debMesh2	= null;
		debVb		= null;
		debVb2		= null;
	}

	/// 線生成
	///------------------------------------------------------------
	public void MakeLine()
	{
		if( debMesh != null ){
			debMesh.Dispose();
			debVb.Dispose();
		}

        debMesh = new MeshData( DrawMode.Lines, (2*3), 2 );
        debVb = new VertexBuffer( debMesh.VertexCount,
                                   debMesh.IndexCount,
                                   VertexFormat.Float3,
                                   VertexFormat.Float4 );
		debMesh.Indices[ 0 ] = 0;
		debMesh.Indices[ 1 ] = 1;
	}
	/// Line描画
    public void DrawLine( GraphicsContext graphics, GeometryLine trgLine, Camera cam, Rgba color )
	{
		if( debShader == null ){
			return ;
		}

		/// バーテクスの更新
		///------------------------------------------------------------
		for( int i=0; i<2; i++ ){
			debMesh.Positions[ i*3+0 ] = trgLine.GetPos(i).X;
			debMesh.Positions[ i*3+1 ] = trgLine.GetPos(i).Y;
			debMesh.Positions[ i*3+2 ] = trgLine.GetPos(i).Z;

			debMesh.Normals[ i*3+0 ] = 0.0f;
			debMesh.Normals[ i*3+1 ] = 1.0f;
			debMesh.Normals[ i*3+2 ] = 0.0f;
		}

		debVb.SetVertices( 0, debMesh.Positions );
		debVb.SetIndices( debMesh.Indices );

		drawMesh( graphics, cam, color );
	}		


	

	/// 三角形生成
	///------------------------------------------------------------
	public void MakeTriangle()
	{
		if( debMesh != null ){
			debMesh.Dispose();
			debVb.Dispose();
		}

        debMesh = new MeshData( DrawMode.Triangles, (3*3), 3 );
        debVb = new VertexBuffer( debMesh.VertexCount,
                                   debMesh.IndexCount,
                                   VertexFormat.Float3,
                                   VertexFormat.Float4 );
		debMesh.Indices[ 0 ] = 0;
		debMesh.Indices[ 1 ] = 1;
		debMesh.Indices[ 2 ] = 2;
	}
	/// 三角形描画
    public void DrawTriangle( GraphicsContext graphics, GeometryTriangle trgTri, Camera cam, Rgba color )
	{
		if( debShader == null ){
			return ;
		}

		/// バーテクスの更新
		///------------------------------------------------------------
		for( int i=0; i<3; i++ ){
			debMesh.Positions[ i*3+0 ] = trgTri.GetPos(i).X;
			debMesh.Positions[ i*3+1 ] = trgTri.GetPos(i).Y;
			debMesh.Positions[ i*3+2 ] = trgTri.GetPos(i).Z;

			debMesh.Normals[ i*3+0 ] = trgTri.Plane.Nor.X;
			debMesh.Normals[ i*3+1 ] = trgTri.Plane.Nor.Y;
			debMesh.Normals[ i*3+2 ] = trgTri.Plane.Nor.Z;
		}

		debVb.SetVertices( 0, debMesh.Positions );
		debVb.SetIndices( debMesh.Indices );

		drawMesh( graphics, cam, color );
	}		



	/// 球生成
	///------------------------------------------------------------
	public void MakeSphere()
	{
		debMesh	= BasicMeshFactory.CreateSphere( 1.0f, debSphDiv );
        debVb	= new VertexBuffer( debMesh.VertexCount,
                                   debMesh.IndexCount,
                                   VertexFormat.Float3,
                                   VertexFormat.Float4 );
	}
	/// 球描画
    public void DrawSphere( GraphicsContext graphics, GeometrySphere trgSph, Camera cam, Rgba color )
	{
		if( debShader == null ){
			return ;
		}

		debVb.SetVertices( 0, debMesh.Positions );
		debVb.SetIndices( debMesh.Indices );

 		Matrix4 world = Matrix4.Translation( new Vector3( trgSph.X, trgSph.Y, trgSph.Z ) ) * Matrix4.Scale( new Vector3( trgSph.R, trgSph.R, trgSph.R ) );
        Matrix4 worldViewProj = cam.Projection * cam.View * world;

        // uniform value
        debShader.SetUniformValue( debUIdWVP, ref worldViewProj );

		Vector4 a_Color = new Vector4( (color.R / 255.0f), (color.G / 255.0f), (color.B / 255.0f), (color.A / 255.0f) );
        debShader.SetUniformValue( debShader.FindUniform( "IAmbient" ), ref a_Color );


        graphics.SetShaderProgram( debShader );

        graphics.SetVertexBuffer( 0, debVb );
        graphics.DrawArrays( debMesh.Prim, 0, debMesh.IndexCount );
	}		



	/// カプセル生成
	///------------------------------------------------------------
	public void MakeCapsule()
	{
		/// 球体部分
		debMesh		= BasicMeshFactory.CreateSphere( 1.0f, debSphDiv );
        debVb		= new VertexBuffer( debMesh.VertexCount,
                    	               debMesh.IndexCount,
                        	           VertexFormat.Float3,
                            	       VertexFormat.Float4 );

		/// パイプ部分
		debMesh2	= BasicMeshFactory.CreatePipe( 1.0f, 1.0f, debSphDiv );
        debVb2		= new VertexBuffer( debMesh2.VertexCount,
                                	   debMesh2.IndexCount,
                                	   VertexFormat.Float3,
                                	   VertexFormat.Float4 );
	}
	/// カプセル描画
    public void DrawCapsule( GraphicsContext graphics, GeometryCapsule trgCap, Camera cam, Rgba color )
	{
		if( debShader == null || trgCap.R <= 0.00001f ){
			return ;
		}

		/// 球体部分
		///---------------------------------------------
		DrawSphere( graphics, new GeometrySphere( trgCap.StartPos, trgCap.R ), cam, color );
		DrawSphere( graphics, new GeometrySphere( trgCap.EndPos, trgCap.R ), cam, color );


		/// パイプ部分
		///---------------------------------------------
		setCapsulePipeVertex( trgCap );


 
		debVb2.SetVertices( 0, debMesh2.Positions );
		debVb2.SetIndices( debMesh2.Indices );

		Matrix4 localMtx;
		if( trgCap.StartPos.X == trgCap.EndPos.X && trgCap.StartPos.Z == trgCap.EndPos.Z ){
			localMtx = Matrix4.LookAt( trgCap.EndPos, trgCap.StartPos, new Vector3(0.0f, 0.0f, 1.0f));
		}
		else{
			localMtx = Matrix4.LookAt( trgCap.EndPos, trgCap.StartPos, new Vector3(0.0f, 1.0f, 0.0f));
		}
		Matrix4 world = localMtx.Inverse() * Matrix4.Scale( new Vector3( trgCap.R, trgCap.R, trgCap.R ) );
		world.M41 = trgCap.StartPos.X;
		world.M42 = trgCap.StartPos.Y;
		world.M43 = trgCap.StartPos.Z;

        Matrix4 worldViewProj = cam.Projection * cam.View * world;

        // uniform value
        debShader.SetUniformValue( debUIdWVP, ref worldViewProj );

		Vector4 a_Color = new Vector4( (color.R / 255.0f), (color.G / 255.0f), (color.B / 255.0f), (color.A / 255.0f) );
        debShader.SetUniformValue( debShader.FindUniform( "IAmbient" ), ref a_Color );

        graphics.SetShaderProgram( debShader );

        graphics.SetVertexBuffer( 0, debVb2 );
        graphics.DrawArrays( debMesh2.Prim, 0, debMesh2.IndexCount );
	}		


	
/// private メソッド
///---------------------------------------------------------------------------

	/// 描画
	private void drawMesh( GraphicsContext graphics, Camera cam, Rgba color )
	{
		Matrix4 world = Matrix4.Translation( new Vector3( 0, 0, 0 ) );
        Matrix4 worldViewProj = cam.Projection * cam.View * world;

        // uniform value
        debShader.SetUniformValue( debUIdWVP, ref worldViewProj );

		Vector4 a_Color = new Vector4( (color.R / 255.0f), (color.G / 255.0f), (color.B / 255.0f), (color.A / 255.0f) );
        debShader.SetUniformValue( debShader.FindUniform( "IAmbient" ), ref a_Color );


        graphics.SetShaderProgram( debShader );

        graphics.SetVertexBuffer( 0, debVb );
        graphics.DrawArrays( debMesh.Prim, 0, debMesh.IndexCount );
	}


	/// カプセルのパイプ部分の頂点設定
    public void setCapsulePipeVertex( GeometryCapsule trgCap )
	{
		Vector3	MoveVec		= trgCap.EndPos-trgCap.StartPos;
		float height		= FMath.Sqrt( MoveVec.Dot(MoveVec) ); 
        int   a_pos_cnt		= 0;

		height *= (1.0f / trgCap.R);
   
	    for( int j = 0; j < debSphDiv*2; j++ ){
            debMesh2.Positions[ a_pos_cnt + 2 ] = height;
			a_pos_cnt += 6;
		}
		
	}


} // GeometryLine

}
