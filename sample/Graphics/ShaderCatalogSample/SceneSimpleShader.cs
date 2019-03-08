/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneSimpleShader
    : IScene
{
    private static VertexBuffer vbTeapot;
    private static ShaderProgram shaderSimple;

    public SceneSimpleShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Simple >"; }
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

        // simple shader
        shaderSimple = new ShaderProgram( "/Application/shaders/Simple.cgx" );
        shaderSimple.SetAttributeBinding( 0, "a_Position" );
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbTeapot.Dispose();
        shaderSimple.Dispose();
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

        drawModel( graphics, camera, model );
        bg.Render( graphics, camera );
        light.Render( graphics, camera );
    }

    void drawModel( 
                   GraphicsContext graphics,
                   Camera camera,
                   Model model
                    )
    {
        Matrix4 world = model.Posture;
        Matrix4 worldViewProj = camera.Projection * camera.View * world;

        shaderSimple.SetUniformValue( shaderSimple.FindUniform( "WorldViewProj" ), ref worldViewProj );
        Vector4 color = new Vector4( 1.0f, 1.0f, 0.0f, 1.0f );
        shaderSimple.SetUniformValue( shaderSimple.FindUniform( "MaterialColor" ), ref color );

        graphics.SetShaderProgram( shaderSimple );
        graphics.SetVertexBuffer( 0, vbTeapot );

        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }
}

} // end ns Sample

