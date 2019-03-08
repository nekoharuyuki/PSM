/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Graphics;


namespace DemoGame
{

/// 各キーやボタンの取得用列挙子
public enum InputGamePadState {
	Left		= (1 << 0),		/// 方向キーの左
	Up			= (1 << 1),		/// 方向キーの上
	Right		= (1 << 2),		/// 方向キーの右
	Down		= (1 << 3),		/// 方向キーの下
	Square		= (1 << 4),		/// □ ボタン
	Triangle	= (1 << 5),		/// △ ボタン
	Circle		= (1 << 6),		/// ○ ボタン
	Cross		= (1 << 7),		/// × ボタン
	Start		= (1 << 8),		/// SELECTボタン
	Select		= (1 << 9),		/// STARTボタン
	L			= (1 << 10),	/// L ボタン
	R			= (1 << 11),	/// R ボタン
	AnalogLeft	= (1 << 12),	/// アナログ左
	AnalogRight	= (1 << 13)		/// アナログ右
} 

public class InputGamePad {

	private int		trgDevIdx;
	private float	inputAnalogLeftX;
	private float	inputAnalogLeftY;
	private float	inputAnalogRightX;
	private float	inputAnalogRightY;

	private InputGamePadState	inputTrig;
	private InputGamePadState	inputScan;
	private InputGamePadState	inputFree;
	private InputGamePadState	inputRepeat;
	private InputGamePadState	inputRepeatTmp;

	private int					repeatStartWaitFrameCnt;
	private int					repeatStartWaitFrameMax;
	private int					repeatEveryWaitFrameCnt;
	private int					repeatEveryWaitFrameMax;


	/// コンストラクタ
	public InputGamePad()
	{
		trgDevIdx	= 0;
	}


/// public メソッド
///---------------------------------------------------------------------------

	/// 初期化
	public bool Init( int trgDeviceIndex )
	{
		Clear();

		trgDevIdx = trgDeviceIndex;
		SetRepeatParam( 60, 5 );

		return( true );
	}

	/// 破棄
	public void Term()
	{
	}


	/// アップデート
	public bool Update()
	{
		GamePadData gamePadData = GamePad.GetData(trgDevIdx);

		/// ボタン入力の保管
		setInputState( ref inputScan, gamePadData.Buttons );		/// 押されているボタン
		setInputState( ref inputTrig, gamePadData.ButtonsDown );	/// 今回押したボタン
		setInputState( ref inputFree, gamePadData.ButtonsUp );		/// 今回離したボタン

		/// アナログ入力の保管
		inputAnalogLeftX	= gamePadData.AnalogLeftX;
		inputAnalogLeftY	= gamePadData.AnalogLeftY;
		inputAnalogRightX	= gamePadData.AnalogRightX;
		inputAnalogRightY	= gamePadData.AnalogRightY;

		/// 連続入力の情報更新
		updateInputRepeat();

/*
		/// アナログ入力
		if( gamePadButtons.AnalogLeftX != 0.0f || gamePadButtons.AnalogLeftY != 0.0f ){
			inputState |= InputGamePadState.AnalogLeft;
		}
		if( gamePadButtons.AnalogRightX != 0.0f || gamePadButtons.AnalogRightY != 0.0f ){
			inputState |= InputGamePadState.AnalogRight;
		}
*/
		//Console.WriteLine( " rep : " + inputRepeat + "<"+repeatStartWaitFrameCnt+", "+repeatEveryWaitFrameCnt+">" + "( "+inputScan+", "+inputRepeatTmp+" )" );



		return true;
	}


	/// 入力情報のクリア
	public void Clear()
	{
		inputTrig		= 0;
		inputScan		= 0;
		inputFree		= 0;
		inputRepeat		= 0;
		inputRepeatTmp	= 0;

		repeatStartWaitFrameCnt	= 0;
		repeatEveryWaitFrameCnt	= 0;
	}


	/// 連続入力の受け付けパラメータ設定
	/**
	 * トリガの発生から同じ入力が『startWaitFrame』の間、継続すると連続入力が有効になります
	 * 連続入力が有効になった後、『everyWaitFrame』おきに入力が有効になります
	 */
	public void SetRepeatParam( int startWaitFrame, int everyWaitFrame )
	{
		repeatStartWaitFrameMax	= startWaitFrame;
		repeatEveryWaitFrameMax	= everyWaitFrame;
	}


/// プロパティ
///---------------------------------------------------------------------------

	/// トリガ
    public InputGamePadState Trig
    {
        get {return inputTrig;}
    }

	/// スキャン
    public InputGamePadState Scan
    {
        get {return inputScan;}
    }

	/// 離したボタン
    public InputGamePadState Free
    {
        get {return inputFree;}
    }

	/// 連続入力
    public InputGamePadState Repeat
    {
        get {return inputRepeat;}
    }

	/// アナログキーの入力情報
    public float AnalogLeftX
    {
        get {return inputAnalogLeftX;}
    }
    public float AnalogLeftY
    {
        get {return inputAnalogLeftY;}
    }
    public float AnalogRightX
    {
        get {return inputAnalogRightX;}
    }
    public float AnalogRightY
    {
        get {return inputAnalogRightY;}
    }



/// private メソッド
///---------------------------------------------------------------------------

	/// パッドの入力情報を格納
	private void setInputState( ref InputGamePadState inputState, GamePadButtons gamePadButtons )
	{
		inputState = 0;

		/// 方向キー入力
		if( (gamePadButtons & GamePadButtons.Up) != 0 ){
			inputState |= InputGamePadState.Up;
		}
		if( (gamePadButtons & GamePadButtons.Down) != 0 ){
			inputState |= InputGamePadState.Down;
		}
		if( (gamePadButtons & GamePadButtons.Left) != 0 ){
			inputState |= InputGamePadState.Left;
		}
		if( (gamePadButtons & GamePadButtons.Right) != 0 ){
			inputState |= InputGamePadState.Right;
		}


		/// ボタン入力
		if( (gamePadButtons & GamePadButtons.Square) != 0 ){
			inputState |= InputGamePadState.Square;
		}
		if( (gamePadButtons & GamePadButtons.Triangle) != 0 ){
			inputState |= InputGamePadState.Triangle;
		}
		if( (gamePadButtons & GamePadButtons.Circle) != 0 ){
			inputState |= InputGamePadState.Circle;
		}
		if( (gamePadButtons & GamePadButtons.Cross) != 0 ){
			inputState |= InputGamePadState.Cross;
		}
		if( (gamePadButtons & GamePadButtons.Start) != 0 ){
			inputState |= InputGamePadState.Start;
		}
		if( (gamePadButtons & GamePadButtons.Select) != 0 ){
			inputState |= InputGamePadState.Select;
		}
		if( (gamePadButtons & GamePadButtons.L) != 0 ){
			inputState |= InputGamePadState.L;
		}
		if( (gamePadButtons & GamePadButtons.R) != 0 ){
			inputState |= InputGamePadState.R;
		}
	}


	/// 繰り返し入力情報を更新
	private void updateInputRepeat()
	{
		inputRepeat		= 0;

		/// 連続入力中
		///---------------------------------------------------
		if( inputRepeatTmp != 0 ){

			/// 開始時の入力が一定時間維持された連続入力とみなす
			if( inputRepeatTmp == inputScan ){

				if( repeatStartWaitFrameCnt >= repeatStartWaitFrameMax ){

					/// 連続入力が有効になった際の有効待機
					repeatEveryWaitFrameCnt ++;
					if( repeatEveryWaitFrameCnt >= repeatEveryWaitFrameMax ){
						inputRepeat				= inputRepeatTmp;
						repeatEveryWaitFrameCnt = 0;
					}
				}
				else{
					repeatStartWaitFrameCnt ++;
				}
			}

			/// 開始時の入力と異なったら連続入力を解除
			else{
				inputRepeatTmp	= 0;
			}
		}

		/// 未連続入力中
		///---------------------------------------------------
		if( inputRepeatTmp == 0 ){

			/// トリガの発生時から連続入力の判定開始
			/// 最初の一回は入力として扱う
			if( inputTrig != 0 || inputFree != 0 ){
				inputRepeat				= inputScan;
				inputRepeatTmp			= inputScan;
				repeatStartWaitFrameCnt	= 0;
				repeatEveryWaitFrameCnt	= repeatEveryWaitFrameMax;
			}
		}
	}


} // InputGamePad

}





