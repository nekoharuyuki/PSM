// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace DemoGame{

public class GaussianYFilter
    : Filter
{
    // filter
    private ShaderProgram shaderGaussianY;
    private TextureRenderer texRenderer;
    private float[] gaussianWeightTable = new float[ 8 ]; ///< 8 * 2 = 16サンプル のテーブル

    private Texture2D texScene;


    /// コンストラクタ
    public GaussianYFilter()
    {
        texRenderer = new TextureRenderer();

        // gaussian y
        shaderGaussianY = new ShaderProgram( "/Application/shaders/GaussianY.cgx" );
		shaderGaussianY.SetAttributeBinding( 0, "a_Position" );
		shaderGaussianY.SetAttributeBinding( 1, "a_TexCoord" );

    }

    /// デストラクタ
    ~GaussianYFilter()
    {
        this.Dispose();
    }

    public override void Dispose()
    {
        if( shaderGaussianY != null ){
            shaderGaussianY.Dispose();
            shaderGaussianY = null;
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
        int idReciprocalTexHeight = shaderGaussianY.FindUniform( "ReciprocalTexHeight" );
        if( idReciprocalTexHeight >= 0 ){
            shaderGaussianY.SetUniformValue( idReciprocalTexHeight, 1.0f / texScene.Height );
        }
        int idWeight = shaderGaussianY.FindUniform( "Weight" );
        if( idWeight >= 0 ){
            shaderGaussianY.SetUniformValue( idWeight, 0, gaussianWeightTable );
        }
        int idOffset = shaderGaussianY.FindUniform( "Offset" );
        if( idOffset >= 0 ){
            Vector2 offset = new Vector2( 0.0f, 16.0f / texScene.Height );
            shaderGaussianY.SetUniformValue( idOffset, ref offset );
        }

        // render
        FrameBuffer oldBuffer = graphics.GetFrameBuffer();

        if( this.Target != null ){
            graphics.SetFrameBuffer( this.Target );
        }

        texRenderer.BindGraphicsContext( graphics );
        texRenderer.Begin( shaderGaussianY );
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
