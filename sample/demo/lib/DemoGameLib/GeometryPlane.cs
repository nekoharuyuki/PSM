/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame
{

/// 平面
public class GeometryPlane {

	public Vector3 Nor;
	public float	D;


	/// コンストラクタ
	public GeometryPlane()
    {
	}
    /// 法線と一点から平面を作成

    public GeometryPlane( Vector3 prmNor, Vector3 pos )
    {
		Nor	= prmNor.Normalize();
		D	= prmNor.Dot( pos ) * -1;
    }

	public GeometryPlane( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
    {
		Set( pos1, pos2, pos3 );
    }

	/// デストラクタ
	~GeometryPlane()
    {
    }


/// public メソッド
///---------------------------------------------------------------------------

	public bool Set( Vector3 pos1, Vector3 pos2, Vector3 pos3 )
	{
		Vector3 calVec1, calVec2;

		/// 法線の作成
		calVec1 = pos2 - pos1;
		calVec2 = pos3 - pos1;

		calVec1 = calVec1.Normalize();
		calVec2 = calVec2.Normalize();

		this.Nor.X = (calVec1.Y * calVec2.Z ) - calVec1.Z * calVec2.Y;
		this.Nor.Y = (calVec1.Z * calVec2.X ) - calVec1.X * calVec2.Z;
		this.Nor.Z = (calVec1.X * calVec2.Y ) - calVec1.Y * calVec2.X;

		this.Nor	= this.Nor.Normalize();
		this.D	= this.Nor.Dot( pos1 ) * -1;
		return true;
	}



/// プロパティ
///---------------------------------------------------------------------------

} // GeometryPlane

}
