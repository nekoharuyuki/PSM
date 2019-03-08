// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace DemoGame{

public
class DepthOfFealdFilter
    : Filter
{
    private TextureRenderer texRenderer;
    private ShaderProgram shaderDOF;

//  private FrameBuffer dstBuffer;
    private Texture2D texScene;
    private Texture2D texDepth;
    private Texture2D texBlur;

    /// コンストラクタ
    public DepthOfFealdFilter()
    {
        texRenderer = new TextureRenderer();

        // DOF
        shaderDOF = new ShaderProgram( "/Application/shaders/DOF.cgx" );
        shaderDOF.SetAttributeBinding( 0, "a_Position" );
        shaderDOF.SetAttributeBinding( 1, "a_TexCoord" );

        shaderDOF.SetUniformValue( shaderDOF.FindUniform( "FocusDepth" ), 0.0f );
    }

    /// デストラクタ
    ~DepthOfFealdFilter()
    {
    }

    public override void Dispose()
    {
        if( shaderDOF == null ){
            shaderDOF.Dispose();
            shaderDOF = null;
        }

        if( texRenderer == null ){
            texRenderer.Dispose();
            texRenderer = null;
        }
    }

    /// ソース(フィルター対象となるTexture)の設定
    public void SetSource( int idx, Texture2D texSrc )
    {
        if( idx == 0 ){
            texScene = texSrc;
        }
        if( idx == 1 ){
            texDepth = texSrc;
        }
        if( idx == 2 ){
            texBlur = texSrc;
        }
    }


    /// フィルタ処理の実行
    public void Filter( GraphicsContext graphics )
    {
        texRenderer.BindGraphicsContext( graphics );
        FrameBuffer oldBuffer = graphics.GetFrameBuffer();

        bool isSwap = false;
        int width = oldBuffer.Width;
        int height = oldBuffer.Height;

        if( this.Target != null ){
            graphics.SetFrameBuffer( this.Target );
            width = this.Target.Width;
            height = this.Target.Height;
            isSwap = true;
        }
        graphics.SetViewport( 0, 0, width, height );

        texRenderer.Begin( shaderDOF );
        graphics.SetTexture( 2, texDepth );
        texRenderer.Render( texScene,
                            texBlur,
                            0, 0, 0, 0, width, height );
        texRenderer.End();
        if( isSwap ){
            graphics.SetFrameBuffer( oldBuffer );
        }
    }

}



} // end ns SceneScript
//===
// EOF
//===
