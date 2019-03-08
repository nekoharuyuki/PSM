/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneToonShader
    : IScene
{
    private static ShaderProgram shaderToon;
    private static VertexBuffer vbTeapot;
    private static Texture2D texToon;

    public SceneToonShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Toon >"; }
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

        shaderToon = new ShaderProgram( "/Application/shaders/Toon.cgx" );
        shaderToon.SetAttributeBinding( 0, "a_Position" );
        shaderToon.SetAttributeBinding( 1, "a_Normal" );

        texToon = new Texture2D( "/Application/data/toon.png", false ) ;
        texToon.SetFilter( Sce.PlayStation.Core.Graphics.TextureFilterMode.Nearest,
                           Sce.PlayStation.Core.Graphics.TextureFilterMode.Nearest,
                           Sce.PlayStation.Core.Graphics.TextureFilterMode.Disabled );

    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbTeapot.Dispose();
        shaderToon.Dispose();
        texToon.Dispose();
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

        shaderToon.SetUniformValue( shaderToon.FindUniform( "WorldViewProj" ), ref worldViewProj );

        // model local Light Direction
        Matrix4 worldInverse = world.Inverse();

        Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f ));
        Vector3 localLightDir = new Vector3( -localLightPos4.X, -localLightPos4.Y, -localLightPos4.Z );
        localLightDir = localLightDir.Normalize();

        shaderToon.SetUniformValue( shaderToon.FindUniform( "LocalLightDirection" ), ref localLightDir );

        // model local eye
        Vector4 localEye4 = worldInverse * (new Vector4( camera.Position, 1.0f ));
        Vector3 localEye = new Vector3( localEye4.X, localEye4.Y, localEye4.Z);
        shaderToon.SetUniformValue( shaderToon.FindUniform( "EyePosition" ), ref localEye );

        graphics.SetShaderProgram( shaderToon ) ;
        graphics.SetVertexBuffer( 0, vbTeapot ) ;
        graphics.SetTexture( 0, texToon );
        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }
}

} // end ns Sample
