/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;

namespace FlightDemo{

/// (プレイヤー)飛行機の表示オブジェクト
public class PlaneModel
    : FlightUnitModel
{
    private float properaTime = 0.0f;
    private float efxTime = 0.0f;
    private BasicModel modelEfxGoodA;
    private BasicModel modelEfxGoodB;
    private BasicModel modelEfxBad;
    private BasicModel modelEfx = null;
    private BasicModel modelPlane;
    private BasicModel modelTurn;
    private BasicModel modelUpDownR;
    private BasicModel modelUpDownL;
    private BasicModel modelPropera;



    /// コンストラクタ
    public PlaneModel()
    {
    }

    /// デストラクタ
    ~PlaneModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        modelPlane = gameData.ModelContainer.Find( "PLANE" );
        modelTurn = gameData.ModelContainer.Find( "TURN" );
        modelUpDownR = gameData.ModelContainer.Find( "UPDOWN-R" );
        modelUpDownL = gameData.ModelContainer.Find( "UPDOWN-L" );
        modelPropera = gameData.ModelContainer.Find( "PROPERA" );

        modelEfxGoodA = gameData.ModelContainer.Find( "EFX-GOOD-A" );
        modelEfxGoodB = gameData.ModelContainer.Find( "EFX-GOOD-B" );
        modelEfxBad = gameData.ModelContainer.Find( "EFX-BAD" );
        modelEfx = null; // アクティブになる時だけ設定される
        
        properaTime = 0.0f;

        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        // 所有権なし
        modelPlane = null;
        modelTurn = null;
        modelUpDownR = null;
        modelUpDownL = null;
        modelPropera = null;

        modelEfxGoodA = null;
        modelEfxGoodB = null;
        modelEfxBad = null;

        return true;
    }

    /// Animation 更新
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        PlaneUnit plane = unit as PlaneUnit;


        int idxAnim = plane.Speed() > 0.5f ? 1 : 0;

        properaTime += delta;
        float len = modelPropera.GetMotionLength( idxAnim );
        properaTime %= len;

        // rudder / aileron は、[-1...1] の範囲を動き、アニメーションは [0..1] の範囲に
        // 正規化されているので、 val / 2.0f + 0.5f
        modelTurn.SetAnimTime( 0, (plane.Rudder * -1.0f) / 2.0f + 0.5f );
        modelUpDownL.SetAnimTime( 0, plane.AileronL / 2.0f + 0.5f );
        modelUpDownR.SetAnimTime( 0, plane.AileronR / 2.0f + 0.5f );
        modelPropera.SetAnimTime( idxAnim, properaTime );
        modelPlane.SetAnimTime( 0, 0.0f );

        if( modelEfx != null ){
            efxTime += delta;
            //efxTime %= modelEfx.GetMotionLength( 0 );
            if( efxTime > modelEfx.GetMotionLength( 0 ) ){
                modelEfx = null;
            }
        }

        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        GraphicsContext graphics = Renderer.GetGraphicsContext();
        if( modelEfx != null ){
            modelEfx.SetAnimTime( 0, efxTime );
            modelEfx.WorldMatrix = unit.GetPosture();
            modelEfx.Update();
            modelEfx.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );
        }

        modelPlane.WorldMatrix = unit.GetPosture();
        modelPlane.Update();
        modelPlane.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );

        modelTurn.WorldMatrix = unit.GetPosture();
        modelTurn.Update();
        modelTurn.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );

        modelUpDownR.WorldMatrix = unit.GetPosture();
        modelUpDownR.Update();
        modelUpDownR.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );

        modelUpDownL.WorldMatrix = unit.GetPosture();
        modelUpDownL.Update();
        modelUpDownL.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );

        PlaneUnit planeUnit = unit as PlaneUnit;
        DbgSphere.Draw( graphics, gameData.GetViewProj(), planeUnit.GetPosture(), planeUnit.GetCollision() );

        modelPropera.WorldMatrix = unit.GetPosture();
        modelPropera.Update();
        modelPropera.Draw( graphics, gameData.ShaderContainer, gameData.GetViewProj(), gameData.GetEyePos() );


        return true;
    }

    /// 良いほうのエフェクトA(ゲート)再生を設定
    public void EfxGoodA()
    {
        efxTime = 0.0f;
        modelEfx = modelEfxGoodA;
    }

    /// 良いほうのエフェクトB(アイテム)再生を設定
    public void EfxGoodB()
    {
        efxTime = 0.0f;
        modelEfx = modelEfxGoodB;
    }

    /// 悪いほうのエフェクト再生を設定
    public void EfxBad()
    {
        efxTime = 0.0f;
        modelEfx = modelEfxBad;
    }

}

} // end ns FlightDemo
//===
// EOF
//===
