/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoModel;
using DemoGame;

namespace DefenseDemo {

/// グリッド欠片クラス
/**
 * @version 0.1, 2011/06/23
 */
public class GridPiece : Posture {

	/// グリッドの配置ユニット
	public int unitType;
	/// ユニットの配置座標
	public Vector3 settingPos;
	/// グリッドの当たり情報
	public CollisionSphere colSphere;
	public Object3D[] obj3d;
	public bool showFutaFlg = false;
	public bool showSelectFlg = false;
	private float[] animFrame;
	private int[] animIndex;

	/// コンストラクタ
	/**
	 * @param [in] mat : グリッドの位置、向き、姿勢
	 */
	public GridPiece( Matrix4 mat )
	{
		SetPosture( mat );
		colSphere = new CollisionSphere();
		colSphere.SetPos( GetPosition(), 5.0f );
		unitType = -1;
			
		obj3d = new Object3D[2];
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		obj3d[0] = resContainer.GetObject3D( "FUTA" );
		obj3d[1] = resContainer.GetObject3D( "U00" );
			
		animFrame = new float[obj3d.Length];
		animIndex = new int[obj3d.Length];
			
		showFutaFlg = true;
		showSelectFlg = true;
			
		animFrame[1] = 0.0f;
		animIndex[1] = 0;
	}

	/// デストラクタ
	/**
	 */
	~GridPiece()
	{
		unitType = -1;
		if( colSphere != null ){
			colSphere.Term();
		}
		colSphere = null;
	}

	/// 初期化
	/**
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init()
	{
		return true;
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}
		
	public void Update()
	{
		animFrame[1]++;
		if( animFrame[1] >= obj3d[1].model.GetMotionFrameMax( animIndex[1] ) ){
			animFrame[1] = 0.0f;
		}
	}

	/// 選択中グリッドの描画を行う
	/**
	 */
	public void Render()
	{
		if( !showSelectFlg ){
			return;
		}
	}
		
	public void RenderAlpha()
	{
		if( !showSelectFlg ){
			return;
		}
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
			
		obj3d[1].model.WorldMatrix = GetPosture();
		obj3d[1].model.SetAnimFrame( animIndex[1], animFrame[1] );
        obj3d[1].model.Update();
        obj3d[1].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
	}
		
	public void RenderFuta()
	{
		if( !showFutaFlg ){
			return;
		}
		GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
		
		obj3d[0].model.WorldMatrix = GetPosture();
		obj3d[0].model.Update();
		obj3d[0].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse() ), camInfo.GetViewEye() );
	}

}

} // end ns DefenseDemo
