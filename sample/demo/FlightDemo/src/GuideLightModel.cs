/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

/// 誘導灯の表示オブジェクト
public class GuideLightModel
    : FlightUnitModel
{
    public BasicModel modelPoint = null;
    public ShaderProgram program = null;
    public VertexBuffer vb = null;
    public Primitive[] prims = null;
    public float animTime = 0.0f;
    private int idWorldViewProj;
    private int idMaterialColor;

    /// コンストラクタ
    public GuideLightModel()
    {
    }

    /// デストラクタ
    ~GuideLightModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        animTime = 0.0f;
        modelPoint = gameData.ModelContainer.Find( "POINT" );
        vb = modelPoint.Parts[ 0 ].Meshes[0].VertexBuffer;
        prims = modelPoint.Parts[ 0 ].Meshes[0].Primitives;
        program = new ShaderProgram( "/Application/shaders/Simple.cgx" );

        program.SetAttributeBinding( 0, "a_Position" );
        idWorldViewProj = program.FindUniform( "WorldViewProj" );
        idMaterialColor = program.FindUniform( "MaterialColor" );

        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        vb = null;
        if( program != null ){
            program.Dispose();
            program = null;
        }
        return true;
    }

    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        animTime += delta;
        animTime %= 1.0f;
        //		float len = modelPoint.GetMotionLength( 0 );
        //        animTime %= len;
        //		modelPoint.SetAnimTime( 0, animTime );
        //        modelPoint.Update();
        

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        Matrix4 viewProj = gameData.GetViewProj();

        GuideLightUnit lightUnit = unit as GuideLightUnit;
        
        graphics.SetBlendFunc( BlendFuncMode.Add,
                               BlendFuncFactor.SrcAlpha,
                               BlendFuncFactor.OneMinusSrcAlpha );

        for( int i = 1; i < lightUnit.GetLightCount(); i++ ){

            Vector4 nearColor = new Vector4( 1.0f, 0.0f, 0.0f, 1.0f );
            Vector4 farColor = new Vector4( 1.0f, 0.0f, 0.0f, 0.2f );

            Matrix4 mtx = viewProj * lightUnit.GetLight( i ) * lightUnit.GetPosture();
            program.SetUniformValue( idWorldViewProj, ref mtx );
            float t = (float)(i - 1) / (lightUnit.GetLightCount() - 1);
            Vector4 color = nearColor.Lerp( farColor, t );
            program.SetUniformValue( idMaterialColor, ref color );

            graphics.SetShaderProgram( program );
            graphics.SetVertexBuffer( 0, vb );
            graphics.DrawArrays( prims ) ;
        }

        return true;
    }
}

} // end ns FlightDemo
//===
// EOF
//===
