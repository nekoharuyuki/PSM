/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class BgModel
{
    Matrix4 posture;

    // mesh
    VertexBuffer vbGrid;
    MeshData meshGrid;
    ShaderProgram shaderVtxColor;

    private Texture2D texture;

    public BgModel( float width, float depth, int divW, int divH )
    {
        meshGrid = BasicMeshFactory.CreatePlane( width, depth,
                                                 divW, divH, 0.25f, 0.25f );

        vbGrid = new VertexBuffer( meshGrid.VertexCount,
                                   meshGrid.IndexCount,
                                   VertexFormat.Float3,
                                   VertexFormat.Float2 );
        vbGrid.SetVertices( 0, meshGrid.Positions );
        vbGrid.SetVertices( 1, meshGrid.TexCoords );

        vbGrid.SetIndices( meshGrid.Indices ) ;

        // vertex color shader
        shaderVtxColor = new ShaderProgram( "/Application/shaders/Texture.cgx" );
        shaderVtxColor.SetAttributeBinding( 0, "a_Position" );
        shaderVtxColor.SetAttributeBinding( 1, "a_TexCoord" );
        texture = new Texture2D( "/Application/data/renga.png", false );
        texture.SetWrap( TextureWrapMode.Repeat );

        posture = Matrix4.Translation( new Vector3( 0.0f, 0.0f, 0.0f ) );
    }

    public void Dispose()
    {
        vbGrid.Dispose();
        meshGrid.Dispose();
        shaderVtxColor.Dispose();
        texture.Dispose();
    }

    public void Render( GraphicsContext graphics, Camera camera )
    {
        Matrix4 worldViewProj = camera.Projection * camera.View * posture;

        // uniform value
        shaderVtxColor.SetUniformValue( shaderVtxColor.FindUniform( "WorldViewProj" ), ref worldViewProj );
        graphics.SetShaderProgram( shaderVtxColor );
        graphics.SetTexture( 0, texture );
        graphics.SetVertexBuffer( 0, vbGrid );
        graphics.DrawArrays( meshGrid.Prim, 0, meshGrid.IndexCount );
    }
}

} // end ns Sample
