/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using DemoGame;

namespace FlightDemo{

/// モーションセンサーを利用した入力ユニット
/**
 * 本来、画面表示は必要ないがデバッグように用意しておく
 */
public class MotionInputModel
    : FlightUnitModel
{
    /// コンストラクタ
    public MotionInputModel()
    {
    }

    /// デストラクタ
    ~MotionInputModel()
    {
    }

    /// (親となる)Unitが UnitManager に登録されたときに呼び出されるハンドラ
    protected override bool onStart( GameCommonData gameData )
    {
        return true;
    }

    /// (親となる)Unitが UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( GameCommonData gameData )
    {
        return true;
    }

    /// アニメーションの更新処理
    protected override bool onUpdate( GameCommonData gameData, FlightUnit unit, float delta )
    {
        return true;
    }

    /// 描画処理
    protected override bool onRender( GameCommonData gameData, FlightUnit unit )
    {
        MotionInputUnit myUnit = unit as MotionInputUnit;

        int fontHeight = Graphics2D.CurrentFont.Metrics.Height;
        int positionX = 300;
        int positionY = fontHeight * 4;

        Graphics2D.DrawText( "Accel   : " + myUnit.BaseAccel.ToString(), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( "Tangent : " + myUnit.BaseTangent.ToString(), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( "Normal  : " + myUnit.BaseNormal.ToString(), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( "Binormal: " + myUnit.BaseBinormal.ToString(), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( "LN      : " + myUnit.LocalN.ToString(), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( String.Format( "Left    : {0:0.0000}", myUnit.inputLeft), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( String.Format( "Right    : {0:0.0000}", myUnit.inputRight), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( String.Format( "Up    : {0:0.0000}", myUnit.inputUp), 0xffffffff, positionX, positionY );
        positionY += fontHeight;
        Graphics2D.DrawText( String.Format( "Down    : {0:0.0000}", myUnit.inputDown), 0xffffffff, positionX, positionY );
        positionY += fontHeight;

        return true;
    }

}


} // end ns FlightDemo

//===
// EOF
//===
