/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneFogShader
    : IScene
{
    private static ShaderProgram shaderFog;
    private static VertexBuffer vbTeapot;

    public SceneFogShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Fog >"; }
    }

    /// Initialization
    public void Setup( GraphicsContext graphics, Model model )
    {
        MeshData meshTeapot = model.Mesh;

        // teapot
        vbTeapot = new VertexBuffer( meshTeapot.VertexCount,
                                     meshTeapot.IndexCount,
                                     VertexFormat.Float3,
                                     VertexFormat.Float3 );
        vbTeapot.SetVertices( 0, meshTeapot.Positions );
        vbTeapot.SetVertices( 1, meshTeapot.Normals );
        vbTeapot.SetIndices( meshTeapot.Indices );
        
        shaderFog = new ShaderProgram( "/Application/shaders/Fog.cgx" );
        shaderFog.SetAttributeBinding( 0, "a_Position" );
        shaderFog.SetAttributeBinding( 1, "a_Normal" );
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        shaderFog.Dispose();
        vbTeapot.Dispose();
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
        // Fog Near - Far
        float a_Near = 20.0f;
        float a_Far = 80.0f;

        shaderFog.SetUniformValue( shaderFog.FindUniform( "FogNear" ), a_Far /(a_Far-a_Near));
        shaderFog.SetUniformValue( shaderFog.FindUniform( "FogFar" ), -1.0f /(a_Far-a_Near));

        // Fog Color
        Vector4 a_FogColor  = new Vector4( 1.0f, 0.0f, 0.0f, 0.0f );
        shaderFog.SetUniformValue( shaderFog.FindUniform( "FogColor" ), ref a_FogColor);

        // light
        Vector4 i_a = light.Ambient;
        shaderFog.SetUniformValue( shaderFog.FindUniform( "IAmbient" ), ref i_a );

        Vector4 i_d = light.Color;
        shaderFog.SetUniformValue( shaderFog.FindUniform( "IDiffuse" ), ref i_d);

        // material
        Vector4 k_a = model.AmbientColor;
        shaderFog.SetUniformValue( shaderFog.FindUniform( "KAmbient" ), ref k_a );

        Vector4 k_d = model.DiffuseColor;
        shaderFog.SetUniformValue( shaderFog.FindUniform( "KDiffuse" ), ref k_d );

        for( int z = 0 ; z < 5; z++ ){
            Matrix4 world = Matrix4.Translation( new Vector3( 5.0f * z, 0.0f, -12.0f * z) ) * model.Posture;
            Matrix4 worldViewProj = camera.Projection * camera.View * world;

            shaderFog.SetUniformValue( shaderFog.FindUniform( "WorldViewProj" ), ref worldViewProj );

            // model local Light Direction
            Matrix4 worldInverse = world.Inverse();

            Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f ));
            Vector3 localLightDir = new Vector3( -localLightPos4.X, -localLightPos4.Y, -localLightPos4.Z );
            localLightDir = localLightDir.Normalize();

            shaderFog.SetUniformValue( shaderFog.FindUniform( "LocalLightDirection" ), ref localLightDir );

            // model local eye
            Vector4 localEye4 = worldInverse * (new Vector4( camera.Position, 1.0f ));
            Vector3 localEye = new Vector3( localEye4.X, localEye4.Y, localEye4.Z);
            shaderFog.SetUniformValue( shaderFog.FindUniform( "EyePosition" ), ref localEye );

            graphics.SetShaderProgram( shaderFog ) ;
            graphics.SetVertexBuffer( 0, vbTeapot );
            graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
        }
    }
}

} // end ns Sample
