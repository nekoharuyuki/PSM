/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace FlightDemo{

/// あたり判定のデバッグ用球体表示
public class DbgSphere
{
    private static VertexBuffer vb;
#if false
    private static DrawMode prim;
    private static int indexCount;
    private static int idWorldViewProj;
    private static int idIAmbient;
#endif
    private static ShaderProgram program;

    /// 初期化処理
    public static void Init()
    {
		MeshData mesh = BasicMeshFactory.CreateSphere( 1.0f, 20 );
        vb = new VertexBuffer( mesh.VertexCount,
                               mesh.IndexCount,
                               VertexFormat.Float3 );

		vb.SetVertices( 0, mesh.Positions );
		vb.SetIndices( mesh.Indices );

#if false
        prim = mesh.Prim;
        indexCount = mesh.IndexCount;
        idWorldViewProj = program.FindUniform( "WorldViewProj" );
        idIAmbient = program.FindUniform( "IAmbient" );
#endif
        program = new ShaderProgram( "/Application/shaders/AmbientColor.cgx" );
		program.SetAttributeBinding( 0, "a_Position" );


    }

    /// 解放処理
    public static void Term()
    {
        if( vb == null ){
            vb.Dispose();
            vb = null;
        }
    }

    /// 表示
    public static void Draw( GraphicsContext graphics, Matrix4 viewProj, Matrix4 posture, GeometrySphere sphere )
    {
#if false
        Vector4 localPos = new Vector4( sphere.Pos.X, sphere.Pos.Y, sphere.Pos.Z, 1.0f );
        Vector4 pos = posture * localPos;
        Matrix4 world = Matrix4.Translation(new Vector3(pos.X, pos.Y, pos.Z)) * Matrix4.Scale(new Vector3(sphere.R, sphere.R, sphere.R));
        Matrix4 wvp = viewProj * world;
		Vector4 color = new Vector4( 1.0f, 0.0f, 0.0f, 0.5f );

        graphics.SetBlendFunc( BlendFuncMode.Add,
                               BlendFuncFactor.SrcAlpha,
                               BlendFuncFactor.OneMinusSrcAlpha );

        program.SetUniformValue( idWorldViewProj, ref wvp );
        program.SetUniformValue( idIAmbient, ref color );

        graphics.SetShaderProgram( program );

        graphics.SetVertexBuffer( 0, vb );
        graphics.DrawArrays( prim, 0, indexCount );
#endif
    }

}

} // end ns FlightDemo
//===
// EOF
//===
