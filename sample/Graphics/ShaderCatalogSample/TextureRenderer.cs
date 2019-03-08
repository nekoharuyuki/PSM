/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

public class TextureRenderer
{
    private static ShaderProgram shaderCurrent; ///< Shader currently in use
    private static ShaderProgram shaderTexture; ///< Default shader
    private static VertexBuffer vertices;
    int idWVP;

    // GraphicsContext Status
    GraphicsContext graphics;
    bool isEnabledCallFace;
    bool isEnabledDepthTest;

    public TextureRenderer()
    {
        shaderTexture = new ShaderProgram( "/Application/shaders/Texture.cgx" );
        shaderTexture.SetAttributeBinding( 0, "a_Position" );
        shaderTexture.SetAttributeBinding( 1, "a_TexCoord" );
        idWVP = shaderTexture.FindUniform( "WorldViewProj" );
        shaderCurrent = shaderTexture;

        vertices = new VertexBuffer( 4, VertexFormat.Float3, VertexFormat.Float2 );

        float[] positions = { 
            0.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f,
            1.0f, 0.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
        };
        float[] texcoords = { 
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
        };

        vertices.SetVertices( 0, positions );
        vertices.SetVertices( 1, texcoords );

    }

    public void BindGraphicsContext( GraphicsContext graphics )
    {
        this.graphics = graphics;
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        shaderTexture.Dispose();
        shaderCurrent.Dispose();
        vertices.Dispose();
    }

    /// Start rendering
    /**
     * Backup the GraphicsContext states we are going to modify
     */
    public void Begin()
    {
        Begin( shaderTexture );
    }

    /// Start rendering, setting a shader
    public void Begin( ShaderProgram shader )
    {
        isEnabledCallFace = graphics.IsEnabled( EnableMode.CullFace );
        isEnabledDepthTest = graphics.IsEnabled( EnableMode.DepthTest );

        // status 
        graphics.Enable( EnableMode.CullFace, false );
        graphics.Enable( EnableMode.DepthTest, false ) ;

        // shader
        shaderCurrent = shader;
        idWVP = shaderCurrent.FindUniform( "WorldViewProj" );
    }

    /// End rendering
    /**
     * Restore the GraphicsContext states we modified
     */
    public void End()
    {
        graphics.Enable( EnableMode.CullFace, isEnabledCallFace );
        graphics.Enable( EnableMode.DepthTest, isEnabledDepthTest ) ;
    }

    /// Display fullscreen
    public void Render( Texture2D tex )
    {
        FrameBuffer fb = graphics.GetFrameBuffer();

        this.Render( tex, 0, 0, 0, 0, fb.Width, fb.Height );
    }

    public void Render( Texture2D tex,
                        int x, int y,
                        float sx, float sy,
                        float width, float height )
    {
        FrameBuffer fb = graphics.GetFrameBuffer();

        if( idWVP >= 0 ){
            //
            float hw = fb.Width / 2.0f;
            float hh = fb.Height / 2.0f;
            Matrix4 P = Matrix4.Ortho( -hw, hw, -hh, hh, 0.1f, 100.0f );
            Matrix4 S = Matrix4.Scale( new Vector3( width, height, 1.0f ) );
            Matrix4 T = Matrix4.Translation( new Vector3( (float)x - hw, (float)-y + hh, -1.0f ) );
            Matrix4 WVP = P * T * S;
            shaderCurrent.SetUniformValue( idWVP, ref WVP );
        }

        graphics.SetShaderProgram( shaderCurrent );
        graphics.SetVertexBuffer( 0, vertices );
        graphics.SetTexture( 0, tex );
        graphics.DrawArrays( DrawMode.TriangleStrip, 0, 4 );

    }

    /// Render 2
    public void Render( Texture2D tex0,
                        Texture2D tex1,
                        int x, int y,
                        float sx, float sy,
                        float width, float height )
    {
        FrameBuffer fb = graphics.GetFrameBuffer();

        if( idWVP >= 0 ){
            //
            float hw = fb.Width / 2.0f;
            float hh = fb.Height / 2.0f;
            Matrix4 P = Matrix4.Ortho( -hw, hw, -hh, hh, 0.1f, 100.0f );
            Matrix4 S = Matrix4.Scale( new Vector3( width, height, 1.0f ) );
            Matrix4 T = Matrix4.Translation( new Vector3( (float)x - hw, (float)-y + hh, -1.0f ) );
            Matrix4 WVP = P * T * S;
            shaderCurrent.SetUniformValue( idWVP, ref WVP );
        }

        graphics.SetShaderProgram( shaderCurrent );
        graphics.SetVertexBuffer( 0, vertices );
        graphics.SetTexture( 0, tex0 );
        graphics.SetTexture( 1, tex1 );
        graphics.DrawArrays( DrawMode.TriangleStrip, 0, 4 );

    }
}

} // end ns Sample