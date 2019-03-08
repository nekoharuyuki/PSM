/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;


namespace DemoGame
{

public class GraphicsDevice {

	private GraphicsContext			useGraphics;
	private Camera			currentCamera;
	private int displayWidth;
	private int displayHeight;


	/// コンストラクタ
	public GraphicsDevice()
	{
	}


/// public メソッド
///---------------------------------------------------------------------------

	/// 初期化
	public bool Init()
	{
		useGraphics		= new GraphicsContext();
		displayWidth	= useGraphics.GetFrameBuffer().Width;
		displayHeight	= useGraphics.GetFrameBuffer().Height;

		ClearCurrentCamera();
		return true;
	}

	/// 破棄
	public void Term()
	{
		useGraphics 	= null;
		currentCamera	= null;
	}

	/// カレントのカメラを登録
	public void SetCurrentCamera( Camera cam )
	{
		/// 参照
		currentCamera = cam;
	}

	/// カレントのカメラを取得
	public Camera GetCurrentCamera()
	{
		return currentCamera;
	}

	/// カレントのカメラの登録解除
	public void ClearCurrentCamera()
	{
		currentCamera = null;
	}


	/// スクリーン座標からカレントのカメラのワールド座標を返す
    public void GetScreenToWorldPos( int scrPosX, int scrPosY, float fZ, ref Vector3 worldPos )
    {
		Matrix4 calView, calPrj, calViewport;
		Matrix4 invView, invPrj, invViewport;

		calView = currentCamera.View;
		invView = calView.Inverse();

		calPrj  = currentCamera.Projection;
		invPrj  = calPrj.Inverse();

		calViewport     = Matrix4.Identity;
		calViewport.M11 = displayWidth/2.0f;
		calViewport.M22 = -displayHeight/2.0f;
		calViewport.M41 = displayWidth/2.0f;
		calViewport.M42 = displayHeight/2.0f;
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



/// プロパティ
///---------------------------------------------------------------------------

    /// 使用するグラフィクス
    public GraphicsContext Graphics
    {
        get {return useGraphics;}
    }

    /// ディスプレイの横サイズ
    public int DisplayWidth
    {
        get {return displayWidth;}
    }

    /// ディスプレイの縦サイズ
    public int DisplayHeight
    {
        get {return displayHeight;}
    }



} // GraphicsDevice

}
