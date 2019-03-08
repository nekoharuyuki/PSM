/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoModel;
using DemoGame;

namespace DefenseDemo {

/// グリッドクラス
/**
 * @version 0.1, 2011/06/23
 */
public class Grid {

	/// グリッド情報
	public List< GridPiece > gridList;
	/// 選択中グリッド番号
	public int gridIndex;
	/// グリッド表示フラグ
	public bool showFlg;
	/// モデル
	public Object3D[] obj3d;
		
	/// コンストラクタ
	/**
	 */
	public Grid()
	{
		obj3d = new Object3D[2];
		gridList = new List<GridPiece>();
		gridIndex = 0;
		showFlg = true;
	}

	/// デストラクタ
	/**
	 */
	~Grid()
	{
		int i = 0;

		if( gridList != null ){
			gridList.Clear();
		}
		gridList = null;

		i = 0;
		while( i < obj3d.Length ){
			obj3d[i] = null;
			i++;
		}
		obj3d = null;
		
	}

	/// 初期化
	/**
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init()
	{
		int i;

		ResourceDataContainer resContainer = ResourceDataContainer.Inst();

		/// ユニット配置情報用モデルを取得
		obj3d[0] = resContainer.GetObject3D( "UNITPOS" );

		/// ユニット配置用グリッド情報の作成
		i = 1;
		while( i < obj3d[0].model.Bones.Length ){
			gridList.Add( new GridPiece(
					Matrix4.Transformation(
						obj3d[0].model.Bones[i].Translation,
						obj3d[0].model.Bones[i].Rotation,
						obj3d[0].model.Bones[i].Scaling )
					) );
			i++;
		}

		return true;
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	/// 更新
	/**
	 * @param [in] touchFlg : タッチ中フラグ
	 */
	public void Update( bool touchFlg )
	{
		int i = 0;
		/// 蓋描画の必要有無を確認
		while( i < gridList.Count ){
			gridList[i].showFutaFlg = false;
			gridList[i].showSelectFlg = false;
				
			if( gridList[i].unitType < 0 ){
				if( CheckUnitCulling(i) ){
					gridList[i].showFutaFlg = true;
				}
			}
			i++;
		}
		/// 選択中グリッドが無しか確認
		if( gridIndex >= 0 ){
			if( touchFlg ){
				/// 選択中グリッドの描画
				gridList[gridIndex].showSelectFlg = true;
					
				gridList[gridIndex].Update();
			}			
		}

	}

	/// グリッドの描画を行う
	/**
	 */
	public void Render()
	{
			
		/// グリッド表示フラグがOFFか確認
		if( !showFlg ){
			return;
		}

		/// 各グリッドの描画処理
		int i = 0;
		while( i < gridList.Count ){
			gridList[i].Render();
			gridList[i].RenderFuta();
			i++;
		}
			
	}

	/// グリッドの半透明描画を行う
	/**
	 */
	public void RenderAlpha()
	{
		/// グリッド表示フラグがOFFか確認
		if( !showFlg ){
			return;
		}
			
		int i = 0;
		while( i < gridList.Count ){
			gridList[i].RenderAlpha();
			i++;
		}
	}

	/// X、Y座標からグリッドのIndex番号取得
	/**
	 * @param [int] x : X座標
	 * @param [int] y : Y座標
	 * @return int : グリッドのIndex番号
	 */
	public int GetTouchGridIndex( int x, int y )
	{
		Vector3 worldPosStart = new Vector3(0,0,0);
		Vector3 worldPosEnd = new Vector3(0,0,0);

		/// Near平面上の点を取得
		GetScreen2WorldPos( x, y, 0.0f, ref worldPosStart );
		/// Far平面上の点を取得
		GetScreen2WorldPos( x, y, 1.0f, ref worldPosEnd );
		
		CollisionCheck colCheck = new CollisionCheck();
		CollisionLine colLine = new CollisionLine();
					
		colLine.SetPos( worldPosStart, worldPosEnd );
			
		gridIndex = -1;
		int boneCnt = 0;
		float dis = 0.0f;
		while( boneCnt < gridList.Count ){
			dis = colCheck.checkRayCrossSphere(
												colLine.posStart,
												colLine.dir,
												gridList[boneCnt].colSphere );
			if( dis >= 0.0f ){
				gridIndex = boneCnt;
				break;
			}
			boneCnt++;
		}
			
//		Console.WriteLine( "[log][SceneTitle.cs]====touch/grid.gridIndex:"+gridIndex );
		return gridIndex;
	}
		
		
	/// グリッドに設定してあるユニット情報を取得
	/**
	 * @param [in] gridId グリッドID
	 * @return ユニット情報有り:0以上、ユニット情報無し:-1
	 */
	public int GetGridUnitType( int gridId )
	{
		return -1;
	}

	/// スクリーン座標からカレントのカメラのワールド座標を返す
	/**
	 * @param [in] scrPosX : スクリーン上のX座標
	 * @param [in] scrPosY : スクリーン上のY座標
	 * @param [in] fz : 0.0f(Near平面)か1.0f(Far平面)を設定
	 * @param [out] worldPos : 算出したワールド座標
	 */
    public void GetScreen2WorldPos( int scrPosX, int scrPosY, float fZ, ref Vector3 worldPos )
    {
		Matrix4 calView, calPrj, calViewport;
		Matrix4 invView, invPrj, invViewport;
		CameraInfo camInfo = CameraInfo.Inst();

		calView = camInfo.GetPosture().GetPosture();
//		invView = calView.Inverse();
		invView = calView;

		calPrj = camInfo.GetProjection();
		invPrj  = calPrj.Inverse();

		calViewport     = Matrix4.Identity;
		calViewport.M11 = Graphics2D.Width/2.0f;
		calViewport.M22 = -Graphics2D.Height/2.0f;
		calViewport.M41 = Graphics2D.Width/2.0f;
		calViewport.M42 = Graphics2D.Height/2.0f;
		invViewport     = calViewport.Inverse();

		Matrix4 tmp  = invView * invPrj;
		Matrix4 tmp2 = tmp * invViewport;

		Vector4 calPos;
		Vector4 scrPos = new Vector4( (float)scrPosX, (float)scrPosY, fZ, 1.0f );
		calPos = tmp2 * scrPos;

		worldPos.X = calPos.X / calPos.W;
		worldPos.Y = calPos.Y / calPos.W;
		worldPos.Z = calPos.Z / calPos.W;
	}

	/**
	 * @param [in] boneIndex : BoneのIndex番号
	 * @return Matrix4 : 指定Index番号のBoneの位置、向き、姿勢
	 */
	public Matrix4 GetBoneMatrix( int boneIndex )
	{
		
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		obj3d[0] = resContainer.GetObject3D( "UNITPOS" );
		Matrix4 tmpMat;
		tmpMat = obj3d[0].model.Bones[boneIndex].WorldMatrix;
		tmpMat = tmpMat * Matrix4.Transformation(
				obj3d[0].model.Bones[boneIndex].Translation,
				obj3d[0].model.Bones[boneIndex].Rotation,
				obj3d[0].model.Bones[boneIndex].Scaling );
		return tmpMat;
	}
		
	/// ユニット位置がカメラ表示領域内か確認
	/**
	 * @param [in] index : ユニット管理Index
	 */
	public bool CheckUnitCulling( int index )
	{
		CommonUtil comUtil = CommonUtil.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
		GridPiece piece = gridList[index];
			
		Vector3 vec1 = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 vec2 = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 crossRes;
		Matrix4 matNear;
		Matrix4 matFar;
		Matrix4 matChar;
		Matrix4 matCamera;
		Matrix4 matCameraInverse = Matrix4.Inverse( camInfo.GetPosture().GetPosture() );
		Vector3 tmpVec = new Vector3( 0.0f, 0.0f, 0.0f );

		/// Far平面左上の3D座標算出
		comUtil.GetScreen2WorldPos( 0, 0, 1.0f, ref tmpVec );

		/// カメラのMatrixを取得
		matCamera = camInfo.GetPosture().GetPosture();
			
		/// Far平面左上のMatrix作成
		matFar = matCamera;
		matFar.M41 = tmpVec.X;
		matFar.M42 = tmpVec.Y;
		matFar.M43 = tmpVec.Z;

		comUtil.GetScreen2WorldPos( 0, 0, 0.0f, ref tmpVec );
		matNear = matCamera;
		matNear.M41 = tmpVec.X;
		matNear.M42 = tmpVec.Y;
		matNear.M43 = tmpVec.Z;
			
		/// ユニットのMatrixを取得
		matChar = piece.GetPosture();


		/// カメラMatrixの逆行列を掛け、カメラ位置を原点とした各座標算出
		matFar = matCameraInverse * matFar;
		matChar = matCameraInverse * matChar;
		matCamera = matCamera * matCameraInverse;
		matNear = matCameraInverse * matNear;

		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matChar.M41 - matCamera.M41;
		vec1.Y = matCamera.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = (matFar.M41-10.0f) - matCamera.M41;
		vec2.Y = matCamera.M42 - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※Y座標はカメラ位置を設定

		/// 外積
		crossRes = comUtil.Cross2( vec1, vec2 );

		/// 左側の画面外に出ているか判定
		if( crossRes.Y < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show left out" );
			return false;
		}

		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matCamera.M41 - matCamera.M41;
		vec1.Y = matChar.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = matCamera.M41 - matCamera.M41;
		vec2.Y = (matFar.M42+10.0f) - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※X座標はカメラ位置を設定

		/// 外積
		crossRes = comUtil.Cross2( vec1, vec2 );

		/// 上側の画面外に出ているか判定
		if( crossRes.X < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show up out" );
			return false;
		}



		/// Far平面右下の3D座標算出
		comUtil.GetScreen2WorldPos( Graphics2D.Width, Graphics2D.Height, 1.0f, ref tmpVec );

		/// Far平面右下のMatrixを作成
		matFar.M41 = tmpVec.X;
		matFar.M42 = tmpVec.Y;
		matFar.M43 = tmpVec.Z;
		/// カメラMatrixの逆行列を掛け、カメラ位置を原点とした座標算出
		matFar = matCameraInverse * matFar;

		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matChar.M41 - matCamera.M41;
		vec1.Y = matCamera.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = (matFar.M41+10.0f) - matCamera.M41;
		vec2.Y = matCamera.M42 - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※Y座標はカメラ位置を設定
		
		/// 外積
		crossRes = comUtil.Cross2( vec2, vec1 );
			
		/// 右側の画面外に出ているか判定
		if( crossRes.Y < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show right out" );
			return false;
		}


		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matCamera.M41 - matCamera.M41;
		vec1.Y = matChar.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = matCamera.M41 - matCamera.M41;
		vec2.Y = (matFar.M42-10.0f) - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※X座標はカメラ位置を設定
		
		/// 外積
		crossRes = comUtil.Cross2( vec2, vec1 );
			
		/// 下側の画面外に出ているか判定
		if( crossRes.X < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show bottom out" );
			return false;
		}

		/// 対象のユニットはカメラ表示領域内に存在する
		return true;
	}		
}

} // end ns DefenseDemo
