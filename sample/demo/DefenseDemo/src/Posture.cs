/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

/// 姿勢制御クラス
/**
 * @version 0.1, 2011/06/23
 */
public class Posture {

	/// 位置
	private Vector3 pos;
	/// 姿勢情報
	private Matrix4 posture;

	/// コンストラクタ
	/**
	 */
	public Posture()
	{
		pos = new Vector3();
		posture = new Matrix4();

		posture = Matrix4.Identity;
	}

	/// デストラクタ
	/**
	 */
	~Posture()
	{
	}

	/// 位置情報の設定
	/**
	 * @param [in] x : x座標
	 * @param [in] y : y座標
	 * @param [in] z : z座標
	 */
	public void SetPosition( float x, float y, float z )
	{
		pos.X = x;
		pos.Y = y;
		pos.Z = z;

		posture.M41 = x;
		posture.M42 = y;
		posture.M43 = z;
		posture.M44 = 1.0f;
	}

	/// 位置情報の取得
	/**
	 * @param [in] x : x座標
	 * @param [in] y : y座標
	 * @param [in] z : z座標
	 */
	public Vector3 GetPosition()
	{
		return pos;
	}

	/// 位置情報の設定(加算)
	/**
	 * @param [in] x : x座標
	 * @param [in] y : y座標
	 * @param [in] z : z座標
	 */
	public void AddPosition( float x, float y, float z )
	{
			
		if( x != 0.0f ){
			posture.M41 = ( posture.M41 + ( -posture.M11 * x ) );
			posture.M42 = ( posture.M42 + ( posture.M21 * x ) );
			posture.M43 = ( posture.M43 + ( posture.M31 * x ) );
		}
		if( y != 0.0f ){
			posture.M41 = ( posture.M41 + ( -posture.M12 * y ) );
			posture.M42 = ( posture.M42 + ( posture.M22 * y ) );
			posture.M43 = ( posture.M43 + ( posture.M32 * y ) );
		}
		if( z != 0.0f ){
			posture.M41 = ( posture.M41 + ( -posture.M13 * z ) );
			posture.M42 = ( posture.M42 + ( posture.M23 * z ) );
			posture.M43 = ( posture.M43 + ( posture.M33 * z ) );
		}
			
		pos.X = posture.M41;
		pos.Y = posture.M42;
		pos.Z = posture.M43;
	}
		
	/// 位置情報の設定(加算)
	/**
	 * @param [in] x : x座標
	 * @param [in] y : y座標
	 * @param [in] z : z座標
	 */
	public void AddPositionW( float x, float y, float z )
	{
		pos.X = pos.X + x;
		pos.Y = pos.Y + y;
		pos.Z = pos.Z + z;
		
		posture.M41 = pos.X;
		posture.M42 = pos.Y;
		posture.M43 = pos.Z;
		posture.M44 = 1.0f;
	}

	/// 指定した位置の方向を向く
	/**
	 * @param [in] x : x座標
	 * @param [in] y : y座標
	 * @param [in] z : z座標
	 */
	public void LookAt( float x, float y, float z )
	{
		float targetX, targetY, targetZ;
		if( x == posture.M41 &&
			y == posture.M42 &&
			z == posture.M43 ){
			return;
		}
			
		targetX = ( x - posture.M41 );
		targetY = ( y - posture.M42 );
		targetZ = ( z - posture.M43 );
		
		LookAtDir( targetX, targetY, targetZ );
	}
		
	public void LookAt2( float x, float y, float z )
	{
//		Console.WriteLine( "[log][posture.cs]====LookAt2(x:"+x+"/y:"+y+"/z:"+z );
		Vector3 trgPos = new Vector3( x, y, z );
		Vector3 orgPos = GetPosition();
//		Console.WriteLine( "[log][posture.cs]====orgPos/x:"+orgPos.X+"/y:"+orgPos.Y+"/z:"+orgPos.Z );
		float lenZ = trgPos.Z - orgPos.Z;
//		Console.WriteLine( "[log][posture.cs]====lenZ:"+lenZ );
		float lenX = trgPos.X - orgPos.X;
//		Console.WriteLine( "[log][posture.cs]====lenX:"+lenX );
		float lenY = trgPos.Y - orgPos.Y;
//		Console.WriteLine( "[log][posture.cs]====lenY:"+lenY );
		float angleH = FMath.Atan( lenY/lenZ );
//		Console.WriteLine( "[log][posture.cs]====angleH:"+angleH );
		float angleW = FMath.Atan( lenX/lenZ );
//		Console.WriteLine( "[log][posture.cs]====angleW:"+angleW );
		Vector3 distance = ( trgPos - orgPos );
		float dis = FMath.Sqrt( distance.Dot(distance) );
//		Console.WriteLine( "[log][posture.cs]====dis:"+dis );
			
		angleH = ( angleH * 180.0f / 3.141593f );
		angleW = ( angleW * 180.0f / 3.141593f );
//		Console.WriteLine( "[log][posture.cs]====angleH:"+angleH );
//		Console.WriteLine( "[log][posture.cs]====angleW:"+angleW );
			
		Vector3 rot = new Vector3( -angleH, -angleW, 0.0f );
			
		SetLookAt( rot, trgPos, dis );
			
//		Console.WriteLine( "[log][posture.cs]====lookat->orgPos/x:"+GetPosition().X+"/y:"+GetPosition().Y+"/z:"+GetPosition().Z );
	}

	/// 指定した方向を向く
	/**
	 * @param [in] x : x座標
	 * @param [in] y : y座標
	 * @param [in] z : z座標
	 */
	public void LookAtDir( float x, float y, float z )
	{
		Vector3 vecX = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 vecY = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 vecZ = new Vector3( 0.0f, 0.0f, 0.0f );

		// Z方向を設定
		vecZ.X = x;
		vecZ.Y = y;
		vecZ.Z = z;
		vecZ = Vector3.Normalize( vecZ );

		// ワールドのY方向を仮に設定
		vecY.X = 0.0f;
		vecY.Y = 1.0f;
		vecY.Z = 0.0f;

		// Y方向、Z方向の外積からX方向を算出
		vecX = Cross2( vecY, vecZ );
		vecX = Vector3.Normalize( vecX );
			
		vecY = Cross2( vecZ, vecX );
		vecY = Vector3.Normalize( vecY );


		posture.M11 = vecX.X;
		posture.M12 = vecX.Y;
		posture.M13 = vecX.Z;
		posture.M14 = 0.0f;
			
		posture.M21 = vecY.X;
		posture.M22 = vecY.Y;
		posture.M23 = vecY.Z;
		posture.M24 = 0.0f;

		posture.M31 = vecZ.X;
		posture.M32 = vecZ.Y;
		posture.M33 = vecZ.Z;
		posture.M34 = 0.0f;

		posture.M41 = pos.X;
		posture.M42 = pos.Y;
		posture.M43 = pos.Z;
		posture.M44 = 1.0f;

	}

	/// 回転値を加算する
	/**
	 * @param [in] p : x軸回転値
	 * @param [in] y : y軸回転値
	 * @param [in] r : z軸回転値
	 */
	public void AddYPR( float p, float y, float r )
	{
		Matrix4 rot;
			
		if( y != 0.0f ){
			rot = Matrix4.RotationY( y );
			posture = posture * rot;
		}
			
		if( p != 0.0f ){
			rot = Matrix4.RotationX( p );
			posture = posture * rot;
		}		
			
		if( r != 0.0f ){
			rot = Matrix4.RotationZ( r );
			posture = posture * rot;
		}
			
		posture.M41 = pos.X;
		posture.M42 = pos.Y;
		posture.M43 = pos.Z;
		posture.M44 = 1.0f;
			
	}

	/// 姿勢情報を取得する
	/**
	 * @return Matrix4 : 姿勢情報
	 */
	public Matrix4 GetPosture()
	{
		return posture;
	}
		
	/// 姿勢情報を設定する
	/**
	 * @param [in] matrix : 姿勢情報
	 */
	public void SetPosture( Matrix4 matrix )
	{
		posture.M11 = matrix.M11;
		posture.M12 = matrix.M12;
		posture.M13 = matrix.M13;
		posture.M14 = matrix.M14;
		posture.M21 = matrix.M21;
		posture.M22 = matrix.M22;
		posture.M23 = matrix.M23;
		posture.M24 = matrix.M24;
		posture.M31 = matrix.M31;
		posture.M32 = matrix.M32;
		posture.M33 = matrix.M33;
		posture.M34 = matrix.M34;
		posture.M41 = matrix.M41;
		posture.M42 = matrix.M42;
		posture.M43 = matrix.M43;
		posture.M44 = matrix.M44;
			
		pos.X = posture.M41;
		pos.Y = posture.M42;
		pos.Z = posture.M43;
	}
		
	/// デバッグ用Matrix値出力
	/**
	 *
	 */
	public void DbgPosturePring()
	{
//		Console.WriteLine( "[log][Posture.cs]start----------" );
//		Console.WriteLine( "[log][Posture.cs]M11:"+posture.M11 );
//		Console.WriteLine( "[log][Posture.cs]M12:"+posture.M12 );
//		Console.WriteLine( "[log][Posture.cs]M13:"+posture.M13 );
//		Console.WriteLine( "[log][Posture.cs]M14:"+posture.M14 );
//		Console.WriteLine( "[log][Posture.cs]M21:"+posture.M21 );
//		Console.WriteLine( "[log][Posture.cs]M22:"+posture.M22 );
//		Console.WriteLine( "[log][Posture.cs]M23:"+posture.M23 );
//		Console.WriteLine( "[log][Posture.cs]M24:"+posture.M24 );
//		Console.WriteLine( "[log][Posture.cs]M31:"+posture.M31 );
//		Console.WriteLine( "[log][Posture.cs]M32:"+posture.M32 );
//		Console.WriteLine( "[log][Posture.cs]M33:"+posture.M33 );
//		Console.WriteLine( "[log][Posture.cs]M34:"+posture.M34 );
//		Console.WriteLine( "[log][Posture.cs]M41:"+posture.M41 );
//		Console.WriteLine( "[log][Posture.cs]M42:"+posture.M42 );
//		Console.WriteLine( "[log][Posture.cs]M43:"+posture.M43 );
//		Console.WriteLine( "[log][Posture.cs]M44:"+posture.M44 );
//		Console.WriteLine( "[log][Posture.cs]end------------" );
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

	/// スケール値の設定
	/**
	 * @param [in] x : X軸成分
	 * @param [in] x : Y軸成分
	 * @param [in] x : Z軸成分
	 */
	public void SetScale( float x, float y, float z )
	{
		Matrix4.Scale( x, y, z, out posture );
	}

	/// PostureにMatrix4を掛ける
	/**
	 * @param [in] m : Potureに掛けるMatrix4の情報
	 */
	public void Multiply2( Matrix4 m )
	{
		posture = Matrix4.Multiply( posture, m );
		pos.X = posture.M41;
		pos.Y = posture.M42;
		pos.Z = posture.M43;
	}
		
    public void SetLookAt( Vector3 trgRot, Vector3 trgPos, float trgDis )
    {
		float pi = 3.141593f;
		Vector3 camPos = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 camUp = new Vector3( 0.0f, 0.0f, 0.0f );
			
		float a_Cal1, a_Cal2, a_Cal3;
		float a_Cal		= (float)(pi / 180.0);
		float angleX	= trgRot.X * a_Cal;
		float angleY	= trgRot.Y * a_Cal;
		float angleZ	= trgRot.Z * a_Cal;

		float a_sinx	= FMath.Sin( angleX );
		float a_cosx	= FMath.Cos( angleX );
		float a_siny	= FMath.Sin( angleY );
		float a_cosy	= FMath.Cos( angleY );
		float a_sinz	= FMath.Sin( angleZ );
		float a_cosz	= FMath.Cos( angleZ );
			
		a_Cal1 = trgDis * a_cosx;
		a_Cal2 = trgDis * a_sinx;
		a_Cal3 = trgDis * a_cosx;

		camPos.X = trgPos.X + ( a_Cal1 * a_siny );
		camPos.Y = trgPos.Y + a_Cal2;
		camPos.Z = trgPos.Z + ( a_Cal3 * a_cosy );

		camUp.X =  ( a_sinz * a_cosy );
		camUp.Y =  a_cosz;
		camUp.Z = -( a_sinz * a_siny );

		posture = Matrix4.LookAt( camPos, trgPos, camUp ).Inverse();
			
		pos.X = camPos.X;
		pos.Y = camPos.Y;
		pos.Z = camPos.Z;
	}
		
	public void SetLookAt2( Vector3 trgRot, Vector3 trgPos, float trgDis )
	{
		Posture tmpPosture = new Posture();
		tmpPosture.SetPosition( trgPos.X, trgPos.Y, trgPos.Z );
		tmpPosture.AddYPR( trgRot.X*(float)(Math.PI/180.0f), 0.0f, 0.0f );
		tmpPosture.AddYPR( 0.0f, trgRot.Y*(float)(Math.PI/180.0f), trgRot.Z*(float)(Math.PI/180.0f) );
		tmpPosture.AddPosition( 0.0f, 0.0f, trgDis );
			
		Matrix4 testLookAt = Matrix4.LookAt(
					tmpPosture.GetPosition(),
					trgPos,
					new Vector3(0.0f,1.0f,0.0f));
		SetPosture( testLookAt.Inverse() );
		pos.X = tmpPosture.GetPosition().X;
		pos.Y = tmpPosture.GetPosition().Y;
		pos.Z = tmpPosture.GetPosition().Z;
	}
		
}

} // end ns DefenseDemo
