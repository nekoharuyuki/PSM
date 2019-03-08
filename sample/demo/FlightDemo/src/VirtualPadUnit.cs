/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using UnitSys;
using DemoGame;

namespace FlightDemo
{
public class VirtualPadUnit
    : FlightUnit
{
	// 公開される定数(enumだと名前が長い上にintキャストしなくてはいけないのでintで宣言)
	public const int kButton_SpeedUp = 0;	// 加速バーチャルボタン
	public const int kButton_SpeedDown = 1;	// 減速バーチャルボタン
	public const int kButton_Max = 2;

    /// コンストラクタ
	public VirtualPadUnit ()
    : base( null, new VirtualPadModel() )
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
        return true;
    }
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

	public bool IsTouch( int button )
	{
		if( button >= kButton_Max ) return false;

        var touch = InputManager.InputTouch;
		if( touch.GetInputNum() <= 0 ) return false;

		var touchX = touch.GetScrPosX( 0 );
		var touchY = touch.GetScrPosY( 0 );
		Sprite padSprite = ((VirtualPadModel)model).GetPadSprite( button );
		int buttonL = (int)padSprite.PositionX;
		int buttonR = buttonL + (int)padSprite.DrawRectWidth;
		int buttonT = (int)padSprite.PositionY;
		int buttonB = buttonT + (int)padSprite.DrawRectHeight;

		if( buttonL <= touchX && touchX < buttonR &&
			buttonT <= touchY && touchY < buttonB ) return true;

		return false;
	}
}
}

