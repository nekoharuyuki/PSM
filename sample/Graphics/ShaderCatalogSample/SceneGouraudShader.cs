/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneGouraudShader
    : IScene
{
    private static VertexBuffer vbTeapot;
    private static ShaderProgram shaderGouraud;

    public SceneGouraudShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Gouraud >"; }
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

        shaderGouraud = new ShaderProgram( "/Application/shaders/Gouraud.cgx" );
        shaderGouraud.SetAttributeBinding( 0, "a_Position" ) ;
        shaderGouraud.SetAttributeBinding( 1, "a_Normal" ) ;
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbTeapot.Dispose();
        shaderGouraud.Dispose();
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

        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "WorldViewProj" ), ref worldViewProj );

        // model local Light Direction
        Matrix4 worldInverse = world.Inverse();
        Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f ));
        Vector3 localLightPos = new Vector3( localLightPos4.X, localLightPos4.Y, localLightPos4.Z );

        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "LocalLightPosition" ), ref localLightPos );

        // model local eye
        Vector4 localEye4 = worldInverse * (new Vector4( camera.Position, 1.0f ));
        Vector3 localEye = new Vector3( localEye4.X, localEye4.Y, localEye4.Z);

        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "EyePosition" ), ref localEye );

        // light
        Vector4 i_a = light.Ambient;
        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "IAmbient" ), ref i_a );
        
        Vector4 i_d = light.Color;
        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "IDiffuse" ), ref i_d );

        // material
        Vector4 k_a = model.DiffuseColor;
        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "KAmbient" ), ref k_a );

        Vector4 k_d = model.DiffuseColor;
        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "KDiffuse" ), ref k_d );

        Vector4 k_s = model.SpecularColor;
        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "KSpecular" ), ref k_s );
        shaderGouraud.SetUniformValue( shaderGouraud.FindUniform( "Shininess" ), 5.0f );

        graphics.SetShaderProgram( shaderGouraud );
        graphics.SetVertexBuffer( 0, vbTeapot );
        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }
}

} // end ns Sample
