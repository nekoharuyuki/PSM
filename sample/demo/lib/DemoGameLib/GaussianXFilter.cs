// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace DemoGame{

public class GaussianXFilter
    : Filter
{
    // filter
    private ShaderProgram shaderGaussianX;
    private TextureRenderer texRenderer;
    private float[] gaussianWeightTable = new float[ 8 ]; ///< 8 * 2 = 16サンプル のテーブル

    private Texture2D texScene;


    /// コンストラクタ
    public GaussianXFilter()
    {
        texRenderer = new TextureRenderer();

        // gaussian x
        shaderGaussianX = new ShaderProgram( "/Application/shaders/GaussianX.cgx" );
		shaderGaussianX.SetAttributeBinding( 0, "a_Position" );
		shaderGaussianX.SetAttributeBinding( 1, "a_TexCoord" );
    }

    /// デストラクタ
    ~GaussianXFilter()
    {
        this.Dispose();
    }

    public override void Dispose()
    {
        if( shaderGaussianX != null ){
            shaderGaussianX.Dispose();
            shaderGaussianX = null;
        }

        gaussianWeightTable = null;
    }

    /// ソース(フィルター対象となるTexture)の設定
    public void SetSource( int idx, Texture2D texSrc )
    {
        if( idx == 0 ){
            texScene = texSrc;
        }
    }

    public void Filter( GraphicsContext graphics )
    {
        // Y軸方向へのガウス
        // uniform
        shaderGaussianX.SetUniformValue( shaderGaussianX.FindUniform( "ReciprocalTexWidth" ),
                                         1.0f / texScene.Width );
        shaderGaussianX.SetUniformValue( shaderGaussianX.FindUniform( "Weight" ), 0, gaussianWeightTable );
        Vector2 offset = new Vector2( 16.0f / texScene.Width, 0.0f );
        shaderGaussianX.SetUniformValue( shaderGaussianX.FindUniform( "Offset" ), ref offset );

        // render
        FrameBuffer oldBuffer = graphics.GetFrameBuffer();

        if( this.Target != null ){
            graphics.SetFrameBuffer( this.Target );
        }

        texRenderer.BindGraphicsContext( graphics );
        texRenderer.Begin( shaderGaussianX );
        texRenderer.Render( texScene );
        texRenderer.End();

        if( this.Target != null ){
            graphics.SetFrameBuffer( oldBuffer );
        }

    }

    /// ガウス関数の重みの計算
    public void UpdateGaussianWeight( 
                                     float i_dispersion 
                                      )
    {
        float a_total = 0.0f;

        for( int i = 0; i < gaussianWeightTable.Length; i++ ){
            float a_pos = 1.0f + 2.0f * (float)i;
            gaussianWeightTable[ i ] = (float)Math.Exp( -0.5f * (float)(a_pos * a_pos) / i_dispersion );
            a_total += 2.0f * gaussianWeightTable[ i ];
        }

        // 正規化
        for( int i = 0; i < gaussianWeightTable.Length; i++ ){
            gaussianWeightTable[ i ] /= a_total;
        }
    }
}
} // end ns SceneScript
//===
// EOF
//===
