/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneVertexLightingShader
    : IScene
{
    private static VertexBuffer vbTeapot;
    private static ShaderProgram shaderVertexLighting;

    public SceneVertexLightingShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Vertex Lighting >"; }
    }

    /// Initialization
    public void Setup( GraphicsContext graphics, Model model )
    {
        MeshData meshTeapot = model.Mesh;

        vbTeapot = new VertexBuffer( meshTeapot.VertexCount,
                                     meshTeapot.IndexCount,
                                     VertexFormat.Float3,
                                     VertexFormat.Float3 );
        vbTeapot.SetVertices( 0, meshTeapot.Positions );
        vbTeapot.SetVertices( 1, meshTeapot.Normals );
        vbTeapot.SetIndices( meshTeapot.Indices );

        // vertex lighting shader
        shaderVertexLighting = new ShaderProgram( "/Application/shaders/VertexLighting.cgx" );
        shaderVertexLighting.SetAttributeBinding( 0, "a_Position" ) ;
        shaderVertexLighting.SetAttributeBinding( 1, "a_Normal" ) ;
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbTeapot.Dispose();
        shaderVertexLighting.Dispose();
    }

    public void Update( float delta )
    {
    }

    public void Render( 
                       GraphicsContext graphics,
                       Camera camera,
                       LightModel light,
                       Model model,
                       BgModel bg
                        )
    {
        graphics.Clear();

        drawModel( graphics, camera, light, model );
        light.Render( graphics, camera );
        bg.Render( graphics, camera );
    }

    void drawModel( 
                   GraphicsContext graphics,
                   Camera camera,
                   LightModel light,
                   Model model
                    )
    {
        Matrix4 world = model.Posture;
        Matrix4 worldViewProj = camera.Projection * camera.View * world;

        shaderVertexLighting.SetUniformValue( shaderVertexLighting.FindUniform( "WorldViewProj" ), ref worldViewProj );

        Matrix4 worldInverse = world.Inverse();
        Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f) );
        Vector3 localLightDir = new Vector3( -localLightPos4.X, -localLightPos4.Y, -localLightPos4.Z );
        localLightDir = localLightDir.Normalize();

        shaderVertexLighting.SetUniformValue( shaderVertexLighting.FindUniform( "LocalLightDirection" ), ref localLightDir );

        // light
        Vector4 i_a = light.Ambient;
        shaderVertexLighting.SetUniformValue( shaderVertexLighting.FindUniform( "IAmbient" ), ref i_a );

        Vector4 i_d = light.Color;
        shaderVertexLighting.SetUniformValue( shaderVertexLighting.FindUniform( "IDiffuse" ), ref i_d );

        // material
        Vector4 k_a = model.DiffuseColor;
        shaderVertexLighting.SetUniformValue( shaderVertexLighting.FindUniform( "KAmbient" ), ref k_a );

        Vector4 k_d = model.DiffuseColor;
        shaderVertexLighting.SetUniformValue( shaderVertexLighting.FindUniform( "KDiffuse" ), ref k_d );

        graphics.SetShaderProgram( shaderVertexLighting );
        graphics.SetVertexBuffer( 0, vbTeapot );
        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }
}

} // end ns Sample
