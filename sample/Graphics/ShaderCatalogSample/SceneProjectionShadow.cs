/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment; // FIXME : for FMath

namespace Sample
{

/// shadow
/**
 * We render in 2 passes.
 * In the 1st pass we render the shadow buffer as seen from the light source.
 * In the 2nd pass we render the scene, projecting the shadow buffer on the ground model.
 */
class SceneProjectionShadow
    : IScene
{
    private int offWidth = ShaderCatalog.DspWidth / 2;
    private int offHeight = ShaderCatalog.DspHeight / 2;

    private static MeshData meshGrid;
    private static VertexBuffer vbTeapot;
    private static VertexBuffer vbGrid;

    private static FrameBuffer frameBuffer;
    private static Texture2D texShadow;
    private static Texture2D texColorMap;

    private static ShaderProgram shaderShadowMap;
    private static ShaderProgram shaderProjectionShadow;
    private static ShaderProgram shaderTexture;

    // test
    private static TextureRenderer texRenderer;

    public SceneProjectionShadow()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Projection Shadow >"; }
    }

    /// Initialization
    public void Setup( GraphicsContext graphics, Model model )
    {
        createVertexBuffer( model.Mesh );
        createShadowBuffer();

        texRenderer = new TextureRenderer();
        texRenderer.BindGraphicsContext( graphics );
    }

    private void createVertexBuffer( MeshData meshTeapot )
    {
        // teapot
        vbTeapot = new VertexBuffer( meshTeapot.VertexCount,
                                     meshTeapot.IndexCount,
                                     VertexFormat.Float3,
                                     VertexFormat.Float2 );
        vbTeapot.SetVertices( 0, meshTeapot.Positions );
        vbTeapot.SetVertices( 1, meshTeapot.TexCoords );
        vbTeapot.SetIndices( meshTeapot.Indices );

        // shadow map(simple)
        shaderShadowMap = new ShaderProgram( "/Application/shaders/Simple.cgx" );
        shaderShadowMap.SetAttributeBinding( 0, "a_Position" );

        // texture
        shaderTexture = new ShaderProgram( "/Application/shaders/Texture.cgx" );
        shaderTexture.SetAttributeBinding( 0, "a_Position" );
        shaderTexture.SetAttributeBinding( 1, "a_TexCoord" );

        texColorMap = new Texture2D( "/Application/data/renga.png", false );
        texColorMap.SetWrap( TextureWrapMode.Repeat );

        // grid
        meshGrid = BasicMeshFactory.CreatePlane( 40.0f, 40.0f,
                                                 10, 10, 0.25f, 0.25f );

        vbGrid = new VertexBuffer( meshGrid.VertexCount,
                                   meshGrid.IndexCount,
                                   VertexFormat.Float3,
                                   VertexFormat.Float2 );

        vbGrid.SetVertices( 0, meshGrid.Positions );
        vbGrid.SetVertices( 1, meshGrid.TexCoords );
        vbGrid.SetIndices( meshGrid.Indices ) ;

        // projection shadow
        shaderProjectionShadow = new ShaderProgram( "/Application/shaders/ProjectionShadow.cgx" );
        shaderProjectionShadow.SetAttributeBinding( 0, "a_Position" );
        shaderProjectionShadow.SetAttributeBinding( 1, "a_TexCoord0" );
    }

    /// Create the buffer to render shadows in
    private void createShadowBuffer()
    {
        frameBuffer = new FrameBuffer();
        texShadow = new Texture2D( offWidth, offHeight, false, PixelFormat.Rgba, PixelBufferOption.Renderable );//Alpha );
        texShadow.SetWrap( TextureWrapMode.ClampToEdge );

        frameBuffer.SetColorTarget( texShadow, 0 );
        frameBuffer.SetDepthTarget( new DepthBuffer( offWidth, offHeight, PixelFormat.Depth16 ) );
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        texShadow.Dispose();
        texColorMap.Dispose();
        texRenderer.Dispose();
        frameBuffer.Dispose();
        shaderTexture.Dispose();
        shaderShadowMap.Dispose();
        shaderProjectionShadow.Dispose();
        vbTeapot.Dispose();
        vbGrid.Dispose();
        meshGrid.Dispose();
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
        // setup a camera aligned with the  light source
        float aspect = offWidth / (float)offHeight;
        float fov = 45.0f * (float)(FMath.PI / 180.0f);
        Matrix4 proj = Matrix4.Perspective( fov,
                                            aspect,
                                            1.0f,
                                            10000.0f );
        Matrix4 view = Matrix4.LookAt( light.Position, model.Position, new Vector3( 0.0f, 1.0f, 0.0f ) );
        Matrix4 world = model.Posture;
        Matrix4 lightVP = proj * view;

        // pass 1
        renderShadowMap( graphics, lightVP * world, model );

        // pass 2
        graphics.Clear();

        drawModel( graphics, camera, lightVP, model );
        light.Render( graphics, camera );

        // test
        texRenderer.Begin();
        int width = texShadow.Width / 2;
        int height = texShadow.Height / 2;
        texRenderer.Render( texShadow, ShaderCatalog.DspWidth - width, 0, 0, 0, width, height );
        texRenderer.End();
    }

    private void renderShadowMap( 
                                 GraphicsContext graphics,
                                 Matrix4 lightWVP,
                                 Model model
                                  )
    {
        // setup to render the ShadowMap
        FrameBuffer oldBuffer = graphics.GetFrameBuffer();
        graphics.SetFrameBuffer( frameBuffer );
        graphics.SetViewport(0, 0, offWidth, offHeight);
        graphics.SetClearColor( 1.0f, 1.0f, 1.0f, 1.0f ) ;
        graphics.Enable( EnableMode.Blend, false );
        graphics.Clear() ;

        // render the model
        shaderShadowMap.SetUniformValue( shaderShadowMap.FindUniform( "WorldViewProj" ), ref lightWVP );

        graphics.SetShaderProgram( shaderShadowMap );

        Vector4 color = new Vector4( 0.0f, 0.0f, 0.0f, 0.5f );
        shaderShadowMap.SetUniformValue( shaderShadowMap.FindUniform( "MaterialColor" ), ref color );

        graphics.SetVertexBuffer( 0, vbTeapot );
        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );

        // restore the frame buffer
        graphics.SetFrameBuffer( oldBuffer );
        graphics.Enable( EnableMode.Blend, true );
        graphics.SetViewport(0, 0, ShaderCatalog.DspWidth, ShaderCatalog.DspHeight );
        graphics.SetClearColor( 0.0f, 0.5f, 1.0f, 1.0f ) ;
    }

    /// Draw the actual model
    private void drawModel(
                           GraphicsContext graphics,
                           Camera camera,
                           Matrix4 lightVP,
                           Model model
                           )
    {
        // teapot
        {
            Matrix4 world = model.Posture;
            Matrix4 worldViewProj = camera.Projection * camera.View * world;

            shaderTexture.SetUniformValue( shaderTexture.FindUniform( "WorldViewProj" ), ref worldViewProj );

            graphics.SetShaderProgram( shaderTexture );
            graphics.SetTexture( 0, texColorMap );

            graphics.SetVertexBuffer( 0, vbTeapot );
            graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
        }

        // ground
        {
            Matrix4 texSpace = new Matrix4(
                                           0.5f,  0.0f, 0.0f, 0.0f,
                                           0.0f,  0.5f, 0.0f, 0.0f,
                                           0.0f,  0.0f, 1.0f, 0.0f,
                                           0.5f,  0.5f, 0.0f, 1.0f );
            Matrix4 worldViewProj = camera.Projection * camera.View; // * Unit;
            Matrix4 worldViewProjTex = texSpace * lightVP;

            // model
            shaderProjectionShadow.SetUniformValue( shaderProjectionShadow.FindUniform( "WorldViewProj" ), ref worldViewProj );
            // light
            shaderProjectionShadow.SetUniformValue( shaderProjectionShadow.FindUniform( "WorldViewProjTex" ), ref worldViewProjTex );

            graphics.SetShaderProgram( shaderProjectionShadow );
            graphics.SetTexture( 0, texColorMap );
            graphics.SetTexture( 1, texShadow );

            graphics.SetVertexBuffer( 0, vbGrid );
            graphics.DrawArrays( meshGrid.Prim, 0, meshGrid.IndexCount );

        }
    }
};

} // end ns Sample