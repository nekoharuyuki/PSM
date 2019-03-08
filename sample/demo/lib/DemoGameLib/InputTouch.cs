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

/// タッチ入力の状態ID
public enum InputTouchState{
	None,  		/// 押されていない
	Down, 		/// 押された
	Up,			/// 離された
	Move,		/// 移動した
	Canceled,	/// キャンセルされた
}


public class InputTouchData {
	public int				Id;
	public int				ScrPosX;
	public int				ScrPosY;
	public InputTouchState	State;
};

public class InputTouch {

	private int	trgDevIdx;
	private int	trgScrWidth;
	private int	trgScrHeight;
	private int dataMax;
	private int isDataNum;
	private InputTouchData[]	inputTouchData;


    /// コンストラクタ
    public InputTouch()
    {
		trgDevIdx	= 0;
	}


/// public メソッド
///---------------------------------------------------------------------------

	/// 初期化
	public bool Init( int trgDeviceIndex, int inputMax, int scrWidth, int scrHeight )
	{
		trgDevIdx		= trgDeviceIndex;
		trgScrWidth		= scrWidth;
		trgScrHeight	= scrHeight;
		dataMax			= inputMax;

		inputTouchData	= new InputTouchData[inputMax];
		if( inputTouchData == null ){
			return false;
		}
		for( int i=0; i<dataMax; i++ ){
			inputTouchData[i]	= new InputTouchData();
			clearDataInfo( i );
		}
		isDataNum		= 0;

		return true;
	}

	/// 破棄
	public void Term()
	{
		if( inputTouchData != null ){
			for( int i=0; i<dataMax; i++ ){
				inputTouchData[i]	= null;
			}
			inputTouchData = null;
		}
	}

	/// 入力更新
	public bool Update()
	{
		List< TouchData > touchDataList = new List< TouchData >();
/*
		try{
			touchDataList = Touch.GetData( trgDevIdx );
		} catch ( Exception e ) {
			Console.WriteLine( e ) ;
			return false;
		}
*/
		try{
			foreach( var touchData in Touch.GetData(trgDevIdx) ){
				touchDataList.Add( touchData );
		    }
		}
		catch (Exception e){
			Console.WriteLine(e);
			return false;
		}



		isDataNum		= 0;

		/// 前回キャンセルされた入力を破棄する
		for( int myIdx=0; myIdx<dataMax; myIdx++ ){
			if( inputTouchData[myIdx].Id >= 0 && inputTouchData[myIdx].State == InputTouchState.Canceled ){
				clearDataInfo( myIdx );
			}
		}


		/// 入力情報を保管
		for( int myIdx=0; myIdx<dataMax; myIdx++ ){
			int trgIdx;

			/// 前回入力のあったIDは更新されているかのチェックを行う
			///---------------------------------------------------------------
			if( inputTouchData[myIdx].Id >= 0 ){
				for( trgIdx=0; trgIdx<touchDataList.Count; trgIdx++ ){
					if( inputTouchData[myIdx].Id == touchDataList[trgIdx].ID ){
						break;
					}
				}

				/// 一致するIDの入力があった場合には情報を更新
				if( trgIdx<touchDataList.Count ){
					setDataInfo( myIdx, touchDataList[trgIdx] );
				}

				/// IDが消失した場合にはキャンセル扱いにする
				else{
					inputTouchData[myIdx].State = InputTouchState.Canceled;
				}
			}

			/// 
			///---------------------------------------------------------------
			else{
				int checkIdx;
				for( trgIdx=0; trgIdx<touchDataList.Count; trgIdx++ ){
					for( checkIdx=0; checkIdx<dataMax; checkIdx++ ){
						if( inputTouchData[checkIdx].Id == touchDataList[trgIdx].ID ){
							break;
						}
					}
					if( checkIdx >= dataMax ){
						break;
					}
				}
				if( trgIdx<touchDataList.Count ){
					setDataInfo( myIdx, touchDataList[trgIdx] );
				}
			}
		}

		touchDataList.Clear();
		touchDataList = null;
		return true;
	}


	/// 入力数の取得
	public int GetInputNum()
	{
		return isDataNum;
	}

	/// 入力IDの取得
	public int GetInputId( int index )
	{
		return inputTouchData[index].Id;
	}

	/// 入力状態IDの取得
	public InputTouchState GetInputState( int index )
	{
		return inputTouchData[index].State;
	}

	/// 入力されたスクリーン座標の取得
	public int GetScrPosX( int index )
	{
		return inputTouchData[index].ScrPosX;
	}

	public int GetScrPosY( int index )
	{
		return inputTouchData[index].ScrPosY;
	}




/// private メソッド
///---------------------------------------------------------------------------

	/// スクリーン座標を返す
	private int getScrPosX( TouchData touchData )
	{
		return( (int)((touchData.X + 0.5f) * trgScrWidth) );
	}
	private int getScrPosY( TouchData touchData )
	{
		return( (int)((touchData.Y + 0.5f) * trgScrHeight) );
	}

	/// 入力情報のクリア
	private void clearDataInfo( int index )
	{
		inputTouchData[index].Id		= -1;
	}

	/// 入力情報の登録
	private void setDataInfo( int index, TouchData touchData )
	{
		inputTouchData[index].Id		= touchData.ID;
		inputTouchData[index].ScrPosX	= getScrPosX( touchData );
		inputTouchData[index].ScrPosY	= getScrPosY( touchData );

		switch( touchData.Status ){
		case TouchStatus.None:	 	inputTouchData[index].State	= InputTouchState.None;		break;
		case TouchStatus.Down:	 	inputTouchData[index].State	= InputTouchState.Down;		break;
		case TouchStatus.Up:	 	inputTouchData[index].State	= InputTouchState.Up;		break;
		case TouchStatus.Move:	 	inputTouchData[index].State	= InputTouchState.Move;		break;
		case TouchStatus.Canceled:	inputTouchData[index].State	= InputTouchState.Canceled;	break;
		}

		isDataNum		++;
	}

} // InputTouch

}
