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
 * CommonUtilクラス
 */
public class CommonUtil
{
	/// 共用クラス
	private static CommonUtil commonUtil;

	/// コンストラクタ
	/**
	 */
	public CommonUtil()
	{
	}

	/// デストラクタ
	/**
	 */
	~CommonUtil()
	{
	}

	/// インスタンスを取得する。
	/**
	 * インスタンスを生成していなければ生成する。
	 */
	public static CommonUtil Inst()
	{
		if( commonUtil == null ) {
			commonUtil = new CommonUtil();
			commonUtil.Init();
			return commonUtil;
		} else {
			return commonUtil;
		}
	}

	/// 初期化
	/**
	 */
	public void Init()
	{
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	/// 距離を算出
	/**
	 * @param [in] pos1 : 開始点
	 * @param [in] pos2 : 終了点
	 * @return float : 開始点から終了点までの距離
	 */
	public float GetDistance( Vector3 pos1, Vector3 pos2 )
	{
		Vector3 vec3 = pos1 - pos2;
		float dis = FMath.Sqrt( vec3.Dot(vec3) );
		return dis;
	}
			

	/// ３頂点からラジアンを返す
	/**
	 * @param [in] posBase : 頂点A
	 * @param [in] pos1 : 頂点Aと隣り合う頂点1
	 * @param [in] pos2 : 頂点Aと隣り合う頂点2
	 * @return float : ラジアン角
	 */
	public float getRadian( Vector3 posBase, Vector3 pos1, Vector3 pos2 )
	{
		Vector3 calA = pos1 - posBase;
		Vector3 calB = pos2 - posBase;

		float lba	= calA.Length();
		float lca	= calB.Length();
		float radian= FMath.Acos( calA.Dot(calB) / (lba*lca) );

		return radian;
	}
	
	/// ボーンIDを取得
	/**
	 * @param [in] model : ボーンIDを抽出するモデルデータ
	 * @param [in] name : ボーン名
	 * @return int : ボーンID
	 */
	public int GetBoneId( BasicModel model, String name )
	{
		int resultId = -1;
		int i = 0;
		while( i < model.Bones.Length ){
			if( model.Bones[i].Name == name ){
				resultId = i;
				break;
			}
			i++;
		}

		return resultId;
	}

	/// コンソール上にログを表示
	/**
	 * @param [in] text : ログとして表示するテキスト文
	 */
	public void SetLog( String text )
	{
//		Console.WriteLine( ""+text );
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
		
	/// X軸ベクトルとY軸ベクトルからZ軸ベクトル算出
	/**
	 * @param [in] u : Y軸ベクトル
	 * @param [in] v : X軸ベクトル
	 * @return Vector3 : 算出したZ軸ベクトルを返す
	 */
	public Vector3 Cross2( Vector3 u, Vector3 v )
	{
		Vector3 res = new Vector3();
		res.X = (u.Y * v.Z) - (u.Z * v.Y);
		res.Y = (u.Z * v.X) - (u.X * v.Z);
		res.Z = (u.X * v.Y) - (u.Y * v.X);
		return res;
	}

		
}

} // DefenseDemo
