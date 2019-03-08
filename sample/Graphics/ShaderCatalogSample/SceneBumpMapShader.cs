/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneBumpMapShader
    : IScene
{
    private static MeshData meshGrid;

    private static VertexBuffer vbTeapot;
    private static VertexBuffer vbGrid;

    private static ShaderProgram shaderBumpMap;

    private static Texture2D texColorMap;
    private static Texture2D texNormalMap;

    public SceneBumpMapShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< BumpMap >"; }
    }

    /// Initialization
    public void Setup( GraphicsContext graphics, Model model )
    {
        createVertexBuffer( model );

        // bumpmap shader
        shaderBumpMap = new ShaderProgram( "/Application/shaders/BumpMap.cgx" );
        shaderBumpMap.SetAttributeBinding( 0, "a_Position" );
        shaderBumpMap.SetAttributeBinding( 1, "a_Normal" );
        shaderBumpMap.SetAttributeBinding( 2, "a_Tangent" );
        shaderBumpMap.SetAttributeBinding( 3, "a_TexCoord0" );
        texColorMap = new Texture2D( "/Application/data/renga.png", false );
        texNormalMap = new Texture2D( "/Application/data/normal01.png", false );
        texColorMap.SetWrap( TextureWrapMode.Repeat );
        texNormalMap.SetWrap( TextureWrapMode.Repeat );
    }


    private void createVertexBuffer( Model model)
    {
        MeshData meshTeapot = model.Mesh;

        // teapot
        vbTeapot = new VertexBuffer( meshTeapot.VertexCount,
                                     meshTeapot.IndexCount,
                                     VertexFormat.Float3,
                                     VertexFormat.Float3,
                                     VertexFormat.Float3,
                                     VertexFormat.Float2 );
        vbTeapot.SetVertices( 0, meshTeapot.Positions );
        vbTeapot.SetVertices( 1, meshTeapot.Normals );
        vbTeapot.SetVertices( 2, meshTeapot.Tangents );
        vbTeapot.SetVertices( 3, meshTeapot.TexCoords );
        vbTeapot.SetIndices( meshTeapot.Indices );


        // grid
        meshGrid = BasicMeshFactory.CreatePlane( 40.0f, 40.0f,
                                                 10, 10, 0.25f, 0.25f );


        vbGrid = new VertexBuffer( meshGrid.VertexCount,
                                   meshGrid.IndexCount,
                                   VertexFormat.Float3,
                                   VertexFormat.Float3,
                                   VertexFormat.Float3,
                                   VertexFormat.Float2 );
        vbGrid.SetVertices( 0, meshGrid.Positions );
        vbGrid.SetVertices( 1, meshGrid.Normals );
        vbGrid.SetVertices( 2, meshGrid.Tangents );
        vbGrid.SetVertices( 3, meshGrid.TexCoords );
        vbGrid.SetIndices( meshGrid.Indices ) ;
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbGrid.Dispose();
        meshGrid.Dispose();

        vbTeapot.Dispose();
        shaderBumpMap.Dispose();

        texColorMap.Dispose();
        texNormalMap.Dispose();
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
        drawGround( graphics, camera, light );

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

        shaderBumpMap.SetUniformValue( shaderBumpMap.FindUniform( "WorldViewProj" ), ref worldViewProj );

        Matrix4 worldInverse = world.Inverse();
        Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f ));
        Vector3 localLightPos = new Vector3( localLightPos4.X, localLightPos4.Y, localLightPos4.Z );

        shaderBumpMap.SetUniformValue( shaderBumpMap.FindUniform( "LocalLightPosition" ), ref localLightPos );

        graphics.SetTexture( 0, texNormalMap );
        graphics.SetTexture( 1, texColorMap );
        graphics.SetShaderProgram( shaderBumpMap );

        graphics.SetVertexBuffer( 0, vbTeapot );

        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }


    void drawGround(
                    GraphicsContext graphics,
                    Camera camera,
                    LightModel light
                    )
    {
        Matrix4 world = Matrix4.Translation( new Vector3( 0.0f, 0.0f, 0.0f ) );
        Matrix4 worldViewProj = camera.Projection * camera.View * world;

        // uniform value
        shaderBumpMap.SetUniformValue( shaderBumpMap.FindUniform( "WorldViewProj" ), ref worldViewProj );

        Matrix4 worldInverse = world.Inverse();
        Vector4 localLightPos4 = worldInverse * (new Vector4( light.Position, 1.0f ));
        Vector3 localLightPos = new Vector3( localLightPos4.X, localLightPos4.Y, localLightPos4.Z );

        shaderBumpMap.SetUniformValue( shaderBumpMap.FindUniform( "LocalLightPosition" ), ref localLightPos );

        graphics.SetShaderProgram( shaderBumpMap );
        graphics.SetTexture( 0, texNormalMap );
        graphics.SetTexture( 1, texColorMap );

        graphics.SetVertexBuffer( 0, vbGrid );
        graphics.DrawArrays( meshGrid.Prim, 0, meshGrid.IndexCount );
    }
}

} // end ns Sample
