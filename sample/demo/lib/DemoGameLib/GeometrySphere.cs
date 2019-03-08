/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame
{

/// 球
public class GeometrySphere
{
	public Vector3 Pos;
	public float R;


	/// コンストラクタ
	public GeometrySphere()
    {
    }
	public GeometrySphere( Vector3 pos, float radius )
    {
		Set( pos, radius );
    }
	public GeometrySphere( float posX, float posY, float posZ, float radius )
    {
		Set( posX, posY, posZ, radius );
    }

	/// デストラクタ
	~GeometrySphere()
    {
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 作成
    public bool Set( Vector3 pos, float radius )
	{
		Set( pos.X, pos.Y, pos.Z, radius );
		return true;
	}

    public bool Set( float posX, float posY, float posZ, float radius )
	{
		this.Pos.X	= posX;
		this.Pos.Y	= posY;
		this.Pos.Z	= posZ;
		this.R	= radius;
		return true;
	}


	/// 行列変換
	public void SetMult( Vector4 pos, float radius, Matrix4 mtx )
	{
		this.Pos.X = (mtx.M11 * pos.X) + (mtx.M21 * pos.Y) + ( mtx.M31 * pos.Z ) + ( mtx.M41 * pos.W );
		this.Pos.Y = (mtx.M12 * pos.X) + (mtx.M22 * pos.Y) + ( mtx.M32 * pos.Z ) + ( mtx.M42 * pos.W );
		this.Pos.Z = (mtx.M13 * pos.X) + (mtx.M23 * pos.Y) + ( mtx.M33 * pos.Z ) + ( mtx.M43 * pos.W );

		this.R	= radius;
	}


/// プロパティ
///---------------------------------------------------------------------------

    /// 配置座標
    public float X
    {
        set {this.Pos.X = value;}
        get {return this.Pos.X;}
    }
    public float Y
    {
        set {this.Pos.Y = value;}
        get {return this.Pos.Y;}
    }
    public float Z
    {
        set {this.Pos.Z = value;}
        get {return this.Pos.Z;}
    }


} // GeometrySphere

}
