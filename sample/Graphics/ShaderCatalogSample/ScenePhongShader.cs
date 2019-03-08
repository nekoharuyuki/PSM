/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class ScenePhongShader
    : IScene
{
    private static VertexBuffer vbTeapot;
    private static ShaderProgram shaderPhong;

    public ScenePhongShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Phong >"; }
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

        shaderPhong = new ShaderProgram( "/Application/shaders/Phong.cgx" );
        shaderPhong.SetAttributeBinding( 0, "a_Position" ) ;
        shaderPhong.SetAttributeBinding( 1, "a_Normal" ) ;
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbTeapot.Dispose();
        shaderPhong.Dispose();
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
        graphics.Clear() ;

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

        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "WorldViewProj" ), ref worldViewProj );

        // model local Light Direction
        Matrix4 worldInverse = world.Inverse();
        Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f ));
        Vector3 localLightDir = new Vector3( -localLightPos4.X, -localLightPos4.Y, -localLightPos4.Z );
        localLightDir = localLightDir.Normalize();

        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "LocalLightDirection" ), ref localLightDir );

        // model local eye
        Vector4 localEye4 = worldInverse * (new Vector4( camera.Position, 1.0f ));
        Vector3 localEye = new Vector3( localEye4.X, localEye4.Y, localEye4.Z);

        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "EyePosition" ), ref localEye );

        // light
        Vector4 i_a = light.Ambient;
        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "IAmbient" ), ref i_a );
        
        Vector4 i_d = light.Color;
        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "IDiffuse" ), ref i_d );

        // material
        Vector4 k_a = model.AmbientColor;
        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "KAmbient" ), ref k_a );

        Vector4 k_d = model.DiffuseColor;
        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "KDiffuse" ), ref k_d );
        
        Vector4 k_s = model.SpecularColor;
        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "KSpecular" ), ref k_s );
        shaderPhong.SetUniformValue( shaderPhong.FindUniform( "Shininess" ), 100.0f );

        graphics.SetShaderProgram( shaderPhong );
        graphics.SetVertexBuffer( 0, vbTeapot );
        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }
}

} // end ns Sample
