/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using DemoGame;

namespace DefenseDemo {

/// 当たり判定処理クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionCheck {

	private const float pi = 3.141593f;
	private const float epsilon = 0.00001f;
 	private float length1, length2;
	private CollisionLine calLine = new CollisionLine();
	private CollisionSphere calSph	= new CollisionSphere();
	private Vector3 colDistanceVec3 = new Vector3();

	/// 球体同士の当たり判定
	/**
	 * @param in orgCapsule : 判定を行う元の当たり情報
	 * @param in tarCapsule : 判定を行う対象の当たり情報
	 * @return bool 衝突している:true、衝突していない:false
	 */
	public bool CheckHitCapsuleToCapsule( CollisionCapsule orgCapsule, CollisionCapsule tarCapsule, ref Vector3 collPos )
	{
			
		Vector3 c1 = new Vector3(0,0,0);
		Vector3	c2 = new Vector3(0,0,0);

		float dist		= getClosestPtLineLine( orgCapsule.colLine, tarCapsule.colLine, ref c1, ref c2 );
		float radius = orgCapsule.r + tarCapsule.r;

		/// カプセル同士の衝突は無し
		if( dist > radius*radius ){
			return false;
		}

		/// カプセル同士の衝突点の算出
		///-----------------------------------------------

		/// カプセルの中心の線分同士が交わる
		if( dist <= epsilon ){

			/// 線分同士の交点を用いて、２つの線分の角度を求める
			float rad = getRadian( c1, orgCapsule.colLine.posStart, tarCapsule.colLine.posEnd );

			/// 衝突点を求める
			float sin = FMath.Sin( rad );
			if( sin > 0.0f ){
				float dis = radius / sin;
				collPos = (orgCapsule.colLine.dir * dis * -1) + c1;
			}
			else{
				collPos = (orgCapsule.colLine.dir * radius * -1) + c1;
			}
		}

		/// カプセルの外側の球に接触する
		else{
			/// 対象カプセルの中心線分の最近接点に球体を生成、その球体と移動する球の軌道との衝突を行い衝突点を算出する
			calSph.SetPos( c2, (orgCapsule.r+tarCapsule.r) );
			CheckLineAndSphere( orgCapsule.colLine, calSph, ref collPos );
		}

		return true;
	}

	/// 球体と三角形の当たり判定
	/**
	 * @param in orgCapsule : 判定を行う元の当たり情報
	 * @param in tarTriangles : 判定を行う対象の当たり情報
	 * @return bool 衝突している:true、衝突していない:false
	 */
	public bool CheckHitCapsuleToTriangles( CollisionCapsule orgCapsule, CollisionTriangle tarTriangle, ref Vector3 collPos )
	{
		/// 表裏判定
		if( orgCapsule.colLine.dir.Dot( tarTriangle.plane.nor ) >= 0.0f ){
			return false;
		}

		Vector3	firstHitPos, firstHitMovePos;
		bool	isCrossFlg;

		/// 三角形の平面との球の最近接点（頂点P）を算出
		firstHitPos.X = orgCapsule.colLine.posStart.X + ( orgCapsule.r * ( tarTriangle.plane.nor.X * -1 ) );
		firstHitPos.Y = orgCapsule.colLine.posStart.Y + ( orgCapsule.r * ( tarTriangle.plane.nor.Y * -1 ) );
		firstHitPos.Z = orgCapsule.colLine.posStart.Z + ( orgCapsule.r * ( tarTriangle.plane.nor.Z * -1 ) );
			
		/// 既にPと球の中心が三角形を跨いでいるかチェック
		isCrossFlg			= false;

		/// 符合が一致すると平面に対して交差した事にならない
		float sign1, sign2;
		sign1 = tarTriangle.plane.nor.Dot( orgCapsule.colLine.posStart ) + tarTriangle.plane.d;
		sign2 = tarTriangle.plane.nor.Dot( firstHitPos ) + tarTriangle.plane.d;

		if( (sign1 > epsilon && sign2 > epsilon) || (sign1 < epsilon && sign2 < epsilon) ){
			
			calLine.dir = orgCapsule.colLine.dir;
			calLine.length = orgCapsule.colLine.length;
			calLine.posStart = firstHitPos;

			/// Pが移動する地点P'を算出
			calLine.posEnd.X = firstHitPos.X + ( orgCapsule.colLine.dir.X * orgCapsule.colLine.length );
			calLine.posEnd.Y = firstHitPos.Y + ( orgCapsule.colLine.dir.Y * orgCapsule.colLine.length );
			calLine.posEnd.Z = firstHitPos.Z + ( orgCapsule.colLine.dir.Z * orgCapsule.colLine.length );
		}
		else{
			isCrossFlg  = true;

			/// Pが移動する地点P'を算出
			firstHitMovePos.X = orgCapsule.colLine.posStart.X + ( orgCapsule.colLine.dir.X * (orgCapsule.colLine.length + orgCapsule.r) );
			firstHitMovePos.Y = orgCapsule.colLine.posStart.Y + ( orgCapsule.colLine.dir.Y * (orgCapsule.colLine.length + orgCapsule.r) );
			firstHitMovePos.Z = orgCapsule.colLine.posStart.Z + ( orgCapsule.colLine.dir.Z * (orgCapsule.colLine.length + orgCapsule.r) );
			firstHitPos = orgCapsule.colLine.posStart;

			/// 三角形との最初に衝突する移動ラインを作成
			calLine.SetPos( firstHitPos, firstHitMovePos );
		}
//		Console.WriteLine( "[log][CollisionCheck.cs]isCrossFlg:"+isCrossFlg );


		/// 移動ラインと平面との交差チェック
		if( isCrossFlg == true || checkRayCrossPlane( tarTriangle.plane, calLine ) == true ){
//		Console.WriteLine( "[log][CollisionCheck.cs]line to plane cross check" );

			/// 平面との交点を求める
			getRayPlaneCrossPoint( tarTriangle.plane, calLine, ref collPos );

			/// 三角形の内外判定
			///-----------------------------------------------------
			if( checkInsideTriangle( tarTriangle, collPos ) == true ){
				return true;
			}
		}
		
		return false;
	}

		
	/// ２つの線分の最近接点の算出
	private float getClosestPtLineLine( CollisionLine p1, CollisionLine p2, ref Vector3 c1, ref Vector3 c2 )
	{
		Vector3 d1	= p1.posEnd - p1.posStart;
		Vector3 d2	= p2.posEnd - p2.posStart;
		Vector3 r	= p1.posStart - p2.posStart;
		
		float a		= d1.Dot(d1);
		float e		= d2.Dot(d2);
		float f		= d2.Dot(r);
		Vector3 cal;

		/// 両方とも線分が点になっている
		if( a <= epsilon && e <= epsilon ){
			length1 = length2 = 0.0f;
			c1 = p1.posStart;
			c2 = p2.posStart;
			cal = c1 - c2;
			return( cal.Dot(cal) );
		}

		/// 最初の線分が点に縮退
		else if( a <= epsilon ){
			length1 = 0.0f;
			length2 = f / e;
			length2 = FMath.Clamp( length2, 0.0f, 1.0f );
		}

		else{
			float c = d1.Dot( r );

			/// ２番目の線分が点に縮退
			if( e <= epsilon ){
				length2 = 0.0f;
				length1 = FMath.Clamp( -c/a, 0.0f, 1.0f );
			}

			else{
				float b = d1.Dot( d2 );
				float denom = a*e - b*b;

				/// ２つの線分が平行
				if( denom == 0.0f ){
					length1 = 0.0f;
				}
				else{
					length1 = FMath.Clamp( (b*f - c*e) / denom, 0.0f, 1.0f );
				}

				length2 = (b*length1 + f) / e;

				if( length2 < 0.0f ){
					length2 = 0.0f;
					length1 = FMath.Clamp( -c/a, 0.0f, 1.0f );
				}
				else if( length2 > 1.0f ){
					length2 = 1.0f;
					length1 = FMath.Clamp( (b-c)/a, 0.0f, 1.0f );
				}
			}
		}

		c1 = p1.posStart + d1 * length1;
		c2 = p2.posStart + d2 * length2;
		cal = c1 - c2;
		return( cal.Dot( cal ) );
	}

		
	/// 線分と平面との交差ﾁｪｯｸ
	/**
	 * @param [in] plane : 平面情報
	 * @param [in] line : 線分情報
	 * @return bool : 線分と平面が交差時true、交差しない時false
	 */
	public bool checkRayCrossPlane( CollisionPlane plane, CollisionLine line )
	{
		float sign1, sign2;

		/// 法線と頂点との位置関係を内積で求める（正：面の正面、負：面の背後）
		sign1 = plane.nor.Dot( line.posStart ) + plane.d;
		sign2 = plane.nor.Dot( line.posEnd ) + plane.d;
//		Console.WriteLine( "[log][CollisionCheck.cs][checkRayCrossPlane]sign1:"+sign1 );
//		Console.WriteLine( "[log][CollisionCheck.cs][checkRayCrossPlane]sign1:"+sign2 );

		/// 符合が一致すると平面に対して交差した事にならない
		if( (sign1 > epsilon && sign2 > epsilon) || (sign1 < epsilon && sign2 < epsilon) ){
			return false;
		}
	
		return true;
	}
		
	/// 線分と平面との交点をもとめる
	/**
	 * @param [in] plane : 平面情報
	 * @param [in] line : 線分情報
	 * @param [out] crossPos : 交差位置
	 */
	public float getRayPlaneCrossPoint( CollisionPlane plane, CollisionLine line, ref Vector3 crossPos )
	{
		float num, denom;
		float t;

		/// 法線とレイの１頂点とのベクトルの内積を求める
		num = plane.nor.Dot( line.posStart );

		/// 法線とレイの内積を求める
		denom = plane.nor.Dot( line.dir );

		/// 平面と平行なので交点なし 
		if( denom == 0.0f ){
			return -1.0f;
		}
   
		/// 媒介変数を求める
		t = ( -(num + plane.d) ) / denom;
 
		/// 直線の方程式から交点を求める
		crossPos.X = line.posStart.X + ( t * line.dir.X );
		crossPos.Y = line.posStart.Y + ( t * line.dir.Y );
		crossPos.Z = line.posStart.Z + ( t * line.dir.Z );

		return t;
	}

	/// 三角形と点との内外判定
	/**
	 * @param [in] trgTri : 対象の三角形情報
	 * @param [in] crossPos : 内外判定を行う点
	 * @return bool : 三角形の内側ならtrue、外側ならfalse
	 */
	public bool checkInsideTriangle( CollisionTriangle trgTri, Vector3 crossPos )
	{
		int type;
		float d1, d2, d3;

		if( FMath.Abs( trgTri.plane.nor.X ) < FMath.Abs( trgTri.plane.nor.Y ) ){
			type = (FMath.Abs( trgTri.plane.nor.Y ) < FMath.Abs( trgTri.plane.nor.Z ) ) ? 3 : 2;
		}
		else{
			type = (FMath.Abs( trgTri.plane.nor.X ) < FMath.Abs( trgTri.plane.nor.Z )) ? 3 : 1;
		}


		switch( type ){

		/// X方向に面が傾いているためX軸を破棄
		case 1:
			d1 = ( (crossPos.Y - trgTri.vertex[0].Y) * (crossPos.Z - trgTri.vertex[1].Z) ) -
				 ( (crossPos.Z - trgTri.vertex[0].Z) * (crossPos.Y - trgTri.vertex[1].Y) );
			d2 = ( (crossPos.Y - trgTri.vertex[1].Y) * (crossPos.Z - trgTri.vertex[2].Z) ) -
				 ( (crossPos.Z - trgTri.vertex[1].Z) * (crossPos.Y - trgTri.vertex[2].Y) );
			d3 = ( (crossPos.Y - trgTri.vertex[2].Y) * (crossPos.Z - trgTri.vertex[0].Z) ) -
				 ( (crossPos.Z - trgTri.vertex[2].Z) * (crossPos.Y - trgTri.vertex[0].Y) );
			break;

		/// y方向に面が傾いているためY軸を破棄
		case 2:
			d1 = ( (crossPos.X - trgTri.vertex[0].X) * (crossPos.Z - trgTri.vertex[1].Z) ) -
				 ( (crossPos.Z - trgTri.vertex[0].Z) * (crossPos.X - trgTri.vertex[1].X) );
			d2 = ( (crossPos.X - trgTri.vertex[1].X) * (crossPos.Z - trgTri.vertex[2].Z) ) -
				 ( (crossPos.Z - trgTri.vertex[1].Z) * (crossPos.X - trgTri.vertex[2].X) );
			d3 = ( (crossPos.X - trgTri.vertex[2].X) * (crossPos.Z - trgTri.vertex[0].Z) ) -
				 ( (crossPos.Z - trgTri.vertex[2].Z) * (crossPos.X - trgTri.vertex[0].X) );
			break;

		/// Z方向に面が傾いているためZ軸を破棄
		case 3:
			d1 = ( (crossPos.X - trgTri.vertex[0].X) * (crossPos.Y - trgTri.vertex[1].Y) ) -
				 ( (crossPos.Y - trgTri.vertex[0].Y) * (crossPos.X - trgTri.vertex[1].X) );
			d2 = ( (crossPos.X - trgTri.vertex[1].X) * (crossPos.Y - trgTri.vertex[2].Y) ) -
				 ( (crossPos.Y - trgTri.vertex[1].Y) * (crossPos.X - trgTri.vertex[2].X) );
			d3 = ( (crossPos.X - trgTri.vertex[2].X) * (crossPos.Y - trgTri.vertex[0].Y) ) -
				 ( (crossPos.Y - trgTri.vertex[2].Y) * (crossPos.X - trgTri.vertex[0].X) );
			break;

		  default:
			return true;
		}

		/// 全て符号が同じなら当り。
		if( (d1 >= -epsilon && d2 >= -epsilon && d3 >= -epsilon ) ||
			(d1 <= epsilon && d2 <= epsilon && d3 <= epsilon ) ){
			return true;
		}

		return false;
	}

	/// 点と三角形の３辺との最近接点を算出
	/**
	 * @param [in] trgPos : 点情報
	 * @param [in] trgTri : 三角形情報
	 * @param [out] cPos : 最近接点情報
	 */
	private float getClosestPtPosTriangle( Vector3 trgPos, CollisionTriangle trgTri, ref Vector3 cPos )
	{
		float isDis, bestDis;
		Vector3 crosePos = new Vector3(0,0,0);;

		bestDis = 0.0f;
		for( int i=0; i<3; i++ ){
			isDis = getClosestPtPosLine( trgPos, trgTri.vertex[i], trgTri.vertex[(i+1)%3], ref crosePos );

			/// 最近接点を更新
			if( i == 0 || isDis < bestDis ){
				bestDis = isDis;
				cPos = crosePos;
			}
		}
		return bestDis;
	}

	/// 点と線分の最近接点を算出
	/**
	 * @param [in] trgPos : 点情報
	 * @param [in] linePos1 : 線分の開始点情報
	 * @param [in] linePos2 : 線分の終了点情報
	 * @param [out] cPos : 最近接点
	 */
	private float getClosestPtPosLine( Vector3 trgPos, Vector3 linePos1, Vector3 linePos2, ref Vector3 cPos )
	{
		Vector3 ab, ca;
		float a_t;
		float a_dotCA;
		float a_dotAA;

		ab = linePos2 - linePos1;
		ca = trgPos - linePos1;

		a_dotCA = ca.Dot( ab );
		a_dotAA = ab.Dot( ab );

		if( a_dotAA <= epsilon ){
			a_t = 0.0f;
		}
		else{
			a_t = (a_dotCA / a_dotAA);
		}
		if( a_t < epsilon ){
			a_t = 0.0f;
		}
		else if( a_t > 1.0f ){
			a_t = 1.0f;
		}

		/// 最近接点の算出
		cPos.X = linePos1.X + ( a_t * ab.X );
		cPos.Y = linePos1.Y + ( a_t * ab.Y );
		cPos.Z = linePos1.Z + ( a_t * ab.Z );

		/// 最近接点と点との距離を返す
		Vector3 calVec = trgPos - cPos;
		float dis = FMath.Sqrt( calVec.Dot(calVec) );

		return dis;
	}
		
	/// レイと球との交差を求める
	/**
	 * @param [in] trgRayPos : レイ開始点
	 * @param [in] trgRayVec : レイの向き
	 * @param [in] trgSph : 球情報
	 * @return float : 交点との距離を返す
	 */
	public float checkRayCrossSphere( Vector3 trgRayPos, Vector3 trgRayVec, CollisionSphere trgSph )
	{
		float a, b, c, d;
		float t;
		Vector3 q;

		q = trgRayPos - trgSph.position;

		// 球の式にレイを代入。
		// x^2 + y^2 + z^2 = r^2
		a = ( ( trgRayVec.X * trgRayVec.X ) +
			  ( trgRayVec.Y * trgRayVec.Y ) +
			  ( trgRayVec.Z * trgRayVec.Z ) );

		b = 2 * ( ( q.X * trgRayVec.X ) +
			 	  ( q.Y * trgRayVec.Y ) +
			 	  ( q.Z * trgRayVec.Z ) );

		c = ( ( q.X * q.X ) +
			  ( q.Y * q.Y ) +
			  ( q.Z * q.Z ) ) - ( trgSph.r * trgSph.r );


		// 判別式。
		// D = b^2 -4ac;
		d = ( b * b ) + ( -4 * ( a * c ) );

		// d < 0 ならば、交差はない。
		// d = 0 ならば、球に接する。
		// d > 0 ならば、2つの異なる点が存在する。
		if( d < 0.0f || a <= 0.0f ){
			return -1.0f;
		}

		//      -b -sqrt (d)
		// t = --------------
		//           2a
		t = ( -b -FMath.Sqrt( d ) ) / (a*2);

		return( FMath.Abs(t) );
	}

	/// 距離を算出
	/**
	 * @param [in] pos1 : 開始点
	 * @param [in] pos2 : 終了点
	 * @return float : 開始点から終了点までの距離
	 */
	public float Distance( Vector3 pos1, Vector3 pos2 )
	{
		colDistanceVec3 = pos1 - pos2;
		float dis = FMath.Sqrt( colDistanceVec3.Dot(colDistanceVec3) );
		return dis;
	}

	/// ３頂点からラジアンを返す
	private float getRadian( Vector3 posBase, Vector3 pos1, Vector3 pos2 )
	{
		Vector3 calA = pos1 - posBase;
		Vector3 calB = pos2 - posBase;

		float lba	= calA.Length();
		float lca	= calB.Length();
		float radian= FMath.Acos( calA.Dot(calB) / (lba*lca) );

		return radian;
	}

	/// 点の移動と球との衝突チェック
	/**
	 * moveLineの軌道による点の移動とtrgSphとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
		
	public bool CheckLineAndSphere( CollisionLine moveLine, CollisionSphere trgSph, ref Vector3 collPos )
	{
		Vector3 calVec  = trgSph.position - moveLine.posStart;
		calVec = calVec.Normalize();

		if( calVec.Dot( moveLine.dir ) >= 0.0f ){

			Vector3 calVec2 = trgSph.position - moveLine.posStart;
			float isT = FMath.Sqrt( calVec2.Dot(calVec2) );
			if( isT < (moveLine.length+trgSph.r) ){

				float dis = checkRayCrossSphere( moveLine.posStart, moveLine.dir, trgSph );
				if( dis >= 0.0f || dis < moveLine.length ){
					collPos = moveLine.posStart + (moveLine.dir * dis);
					return true;
				}
			}
		}
		return false;
	}		
		
}

} // end ns DefenseDemo
