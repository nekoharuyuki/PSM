/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

public class FlightPipeModel
    : FlightUnitModel
{
    private BasicModel model;
    private BasicProgram program;
    private int idLocalTargetPosition;
    private int idRadius;

    /// コンストラクタ
    public FlightPipeModel()
    {
    }

    /// デストラクタ
    ~FlightPipeModel()
    {
    }
    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        model = gameData.ModelContainer.Find( "PIPE" );

        program = new BasicProgram( "/Application/shaders/Pipe.cgx", null );
        //        program.SetAttributeBinding( 0, "a_Position" );
        //        program.SetAttributeBinding( 3, "a_TexCoord0" );
        idRadius = program.FindUniform( "Radius" );
        idLocalTargetPosition = program.FindUniform( "LocalTargetPosition" );
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        if( program != null ){
            program.Dispose();
            program = null;
        }
        // 所有権なし
        model = null;
        return true;
    }

    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        FlightPipeUnit myUnit = unit as FlightPipeUnit;
        GraphicsContext graphics = Renderer.GetGraphicsContext();

        Matrix4 posture = myUnit.GetPosture();
        
        if( idLocalTargetPosition >= 0 ){
            Matrix4 invPosture = posture.Inverse();
            Vector3 pos = myUnit.GetTargetPos();
            Vector4 localPos = invPosture * (new Vector4( pos.X, pos.Y, pos.Z, 1.0f ));
            program.SetUniformValue( idLocalTargetPosition, ref localPos );
        }
        if( idRadius >= 0 ){
            program.SetUniformValue( idRadius, 4.0f );
        }
        //        graphics.SetShaderProgram( program );

        model.WorldMatrix = posture;
        model.Update();
        model.Draw( graphics, program, gameData.GetViewProj(), gameData.GetEyePos() );


        return true;
    }

}


} // end ns FlightDemo
//===
// EOF
//===
