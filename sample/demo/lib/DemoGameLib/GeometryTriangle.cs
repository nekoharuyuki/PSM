/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame
{

/// 三角形
public class GeometryTriangle {

	public Vector3[]		Pos;
	public GeometryPlane	Plane;


	/// コンストラクタ
	public GeometryTriangle()
    {
		this.Plane	= new GeometryPlane();
		this.Pos		= new Vector3[3];
		for( int i=0; i<3; i++ ){
			this.Pos[i] = new Vector3();
		}
    }
	public GeometryTriangle( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
    {
		this.Plane	= new GeometryPlane();
		this.Pos		= new Vector3[3];
		for( int i=0; i<3; i++ ){
			this.Pos[i] = new Vector3();
		}
		Set( pos1, pos2, pos3 );
    }

	/// デストラクタ
	~GeometryTriangle()
    {
		this.Pos		= null;
		this.Plane	= null;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 作成
    public bool Set( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
	{
		this.Pos[0] = pos1;
		this.Pos[1] = pos2;
		this.Pos[2] = pos3;
		this.Plane.Set( pos1, pos2, pos3 );
		return true;
	}

	/// 行列変換
	public void SetMult( Vector4 pos1, Vector4 pos2, Vector4 pos3, Matrix4 mtx )
	{
		this.Pos[0].X = (mtx.M11 * pos1.X) + (mtx.M21 * pos1.Y) + ( mtx.M31 * pos1.Z ) + ( mtx.M41 * pos1.W );
		this.Pos[0].Y = (mtx.M12 * pos1.X) + (mtx.M22 * pos1.Y) + ( mtx.M32 * pos1.Z ) + ( mtx.M42 * pos1.W );
		this.Pos[0].Z = (mtx.M13 * pos1.X) + (mtx.M23 * pos1.Y) + ( mtx.M33 * pos1.Z ) + ( mtx.M43 * pos1.W );

		this.Pos[1].X = (mtx.M11 * pos2.X) + (mtx.M21 * pos2.Y) + ( mtx.M31 * pos2.Z ) + ( mtx.M41 * pos2.W );
		this.Pos[1].Y = (mtx.M12 * pos2.X) + (mtx.M22 * pos2.Y) + ( mtx.M32 * pos2.Z ) + ( mtx.M42 * pos2.W );
		this.Pos[1].Z = (mtx.M13 * pos2.X) + (mtx.M23 * pos2.Y) + ( mtx.M33 * pos2.Z ) + ( mtx.M43 * pos2.W );

		this.Pos[2].X = (mtx.M11 * pos3.X) + (mtx.M21 * pos3.Y) + ( mtx.M31 * pos3.Z ) + ( mtx.M41 * pos3.W );
		this.Pos[2].Y = (mtx.M12 * pos3.X) + (mtx.M22 * pos3.Y) + ( mtx.M32 * pos3.Z ) + ( mtx.M42 * pos3.W );
		this.Pos[2].Z = (mtx.M13 * pos3.X) + (mtx.M23 * pos3.Y) + ( mtx.M33 * pos3.Z ) + ( mtx.M43 * pos3.W );

		this.Plane.Set( this.Pos[0], this.Pos[1], this.Pos[2] );
	}

	/// 座標取得
	public Vector3 GetPos( int idx )
	{
		return this.Pos[idx];
	}

/// プロパティ
///---------------------------------------------------------------------------

    /// 座標取得
    public Vector3 Pos1
    {
        set {this.Pos[0] = value;}
        get {return this.Pos[0];}
    }
    public Vector3 Pos2
    {
        set {this.Pos[1] = value;}
        get {return this.Pos[1];}
    }
    public Vector3 Pos3
    {
        set {this.Pos[2] = value;}
        get {return this.Pos[2];}
    }


} // GeometryTriangle

}
