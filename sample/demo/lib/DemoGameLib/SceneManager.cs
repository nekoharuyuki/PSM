/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;


namespace DemoGame
{

public class SceneManager {
		
	private List< IScene > sceneList;
	private int		nextSceneId;
	private bool	prevFlg;
	private bool	stackFlg;
		

    /// コンストラクタ
    public SceneManager()
    {
	}


/// public メソッド
///---------------------------------------------------------------------------

	/// 初期化
	public bool Init()
	{
		sceneList = new List< IScene >();
		if( sceneList == null ){
			return false;
		}

		nextSceneId		= -1;
		prevFlg			= false;
		stackFlg		= false;
		return true;
	}


	/// 破棄
	public void Term()
	{
		if( sceneList != null ){
			for( int i=0; i<sceneList.Count; i++ ){
				sceneList[i].Term();
			}
			sceneList.Clear();
		}

		sceneList		= null;
		nextSceneId	= -1;
	}


	/// シーン切り替え
	/**
	 * stackFlg : シーンの切り替えにスタックを残して切り替えるかのフラグ
	 *			trueの場合には現在のシーンが一時停止します
	 */
	public bool Next( IScene scene, bool stackFlg )
	{
		sceneList.Add( scene );

		if( stackFlg == true ){
			nextSceneId = sceneList.Count-1;
		}
		else{
			nextSceneId = 0;
		}

		prevFlg			= false;
		this.stackFlg	= stackFlg;
		return true;
	}


	/// シーンを戻す
	/**
	 * 現在のカレントのシーンのみを破棄して１つ前のシーンへ戻す
	 */
	public bool Prev()
	{
		if( this.currentId <= 0 ){
			return false;
		}

		nextSceneId	= sceneList.Count - 2;
		prevFlg			= true;
		stackFlg		= false;
		return true;
	}


	/// サスペンド＆レジューム
	public void Suspend()
	{
		if( sceneList != null && sceneList.Count > 0 ){
			sceneList[this.currentId].Suspend();
		}
	}
	public void Resume()
	{
		if( sceneList != null && sceneList.Count > 0 ){
			sceneList[this.currentId].Resume();
		}
	}


	/// フレーム更新
	public bool Update()
	{
		bool res = false;

		if( sceneList == null ){
			return false;
		}
		
		if( nextSceneId >= 0 ){

			///-----------------------------------------
			/// 現在のシーンの切り替え処理
			///-----------------------------------------
			if( stackFlg == false && prevFlg == false ){
				/// シーンを破棄する
				int endCurrentId = (sceneList.Count-2);
				for( int i=endCurrentId; i>=0; i-- ){
					sceneList[i].Term();
					sceneList.RemoveAt( i );
				}
			}
			else{
				if( prevFlg == false ){
					/// シーンを停止する
					int endCurrentId = (sceneList.Count-2);
					sceneList[endCurrentId].Pause();
				}
				else{
					int endCurrentId = this.currentId;
					sceneList[endCurrentId].Term();
					sceneList.RemoveAt( endCurrentId );
				}
			}


			int nxtCurrentId = this.currentId;
			///-----------------------------------------
			/// 切り替えるシーンのセットアップ
			///-----------------------------------------
			if( prevFlg == false ){
				/// 新規シーンへの切り替え
				sceneList[nxtCurrentId].Init( this );
			}
			else{
				/// シーンの再開
				sceneList[nxtCurrentId].Restart();
			}

			nextSceneId	= -1;
		}

		if( sceneList.Count > 0 ){
			res = sceneList[this.currentId].Update();
		}
		return res;
	}


	/// 描画
	public bool Render()
	{
		bool res = false;
		if( nextSceneId >= 0 ){
			return res;
		}
			
		if( sceneList == null ){
			return false;
		}

		if( sceneList.Count > 0 ){
			res = sceneList[this.currentId].Render();
		}
		return res;
	}



/// private メソッド
///---------------------------------------------------------------------------

	/// 現在のカレントシーンのIDを取得
	private int currentId
	{
		get{ return(sceneList.Count-1); }
	}

} // SceneManager

}
