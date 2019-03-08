/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;
using DemoGame;

namespace DefenseDemo
{

/**
 * CameraInfoクラス
 */
public class CameraInfo
{
	/// カメラ情報
	private static CameraInfo cameraInfo;
	/// カメラの画角/near/far設定
	private Matrix4 proj;
	/// カメラの姿勢
	private Posture camPosture;
	private float near;
	private float far;
	private float angle;
	private Matrix4 eye;
	private Vector4 viewEye;

	/// コンストラクタ
	/**
	 */
	public CameraInfo()
	{
	}

	/// デストラクタ
	/**
	 */
	~CameraInfo()
	{
	}

	/// インスタンスを取得する。
	/**
	 * インスタンスを生成していなければ生成する。
	 */
	public static CameraInfo Inst()
	{
		if( cameraInfo == null ) {
			cameraInfo = new CameraInfo();
			cameraInfo.Init();
			return cameraInfo;
		} else {
			return cameraInfo;
		}
	}

	/// 初期化
	/**
	 */
	public void Init()
	{
		proj = Matrix4.Perspective( (16.0f * ((float)Math.PI / 180.0f)),
									Graphics2D.Width / (float)Graphics2D.Height,
									30.0f,
									700.0f);
		camPosture = new Posture();
		this.angle = 16.0f;
		this.near = 10.0f;
		this.far = 1000.0f;
		eye = Matrix4.Translation(new Vector3(0.0f, 0.0f, 0.0f));
		viewEye = eye * new Vector4( 0.0f, 0.0f, 0.0f, 1.0f);
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	/// カメラの画角/near/far設定情報の取得
	/**
	 */
	public Matrix4 GetProjection()
	{
		return proj;
	}

	/// カメラの姿勢情報の取得
	/**
	 */
	public Posture GetPosture()
	{
		return camPosture;
	}

	/// モデル描画時に使用するカメラのView情報の取得
	/**
	 */
	public Matrix4 GetView()
	{
		return camPosture.GetPosture();
	}

	/// モデル描画時に使用する視線情報の取得
	/**
	 */
	public Vector4 GetViewEye()
	{
		return viewEye;
	}
		
	/// カメラの画角の取得
	/**
	 */
	public float GetProjAngle()
	{
		return angle;
	}

	/// カメラのニア平面までの距離を取得
	/**
	 */
	public float GetProjNear()
	{
		return near;
	}
		
	/// カメラのファー平面までの距離を取得
	/**
	 */
	public float GetProjFar()
	{
		return far;
	}

}

} // ShootingDemo
