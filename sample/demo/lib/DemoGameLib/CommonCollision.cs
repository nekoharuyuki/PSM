/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame{

static public class CommonCollision
{
	private const float pi = 3.141593f;
	private const float epsilon = 0.00001f;
 	static private float length1, length2;

	static private GeometryLine		calLine = new GeometryLine();
	static private GeometrySphere	calSph	= new GeometrySphere();


/// public メンバ （点の移動）
///---------------------------------------------------------------------------

	/// 点の移動とカプセルとの衝突チェック
	/**
	 * moveLineの軌道での点の移動とtrgCapとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
	static public bool CheckLineAndCapsule( GeometryLine moveLine, GeometryCapsule trgCap, ref Vector3 collPos )
	{
		Vector3 c1 = new Vector3(0,0,0);
		Vector3 c2 = new Vector3(0,0,0);
			
		float dist		= getClosestPtLineLine( moveLine, trgCap.Line, ref c1, ref c2 );
		float radius	= trgCap.R;

		/// カプセル同士の衝突は無し
		if( dist > radius*radius ){
			return false;
		}

		/// カプセル同士の衝突点の算出
		///-----------------------------------------------

		/// カプセルの中心の線分同士が交わる
		if( dist <= epsilon ){

			/// 線分同士の交点を用いて、２つの線分の角度を求める
			float rad = getRadian( c1, moveLine.StartPos, trgCap.Line.EndPos );

			/// 衝突点を求める
			float sin = FMath.Sin( rad );
			if( sin > 0.0f ){
				float dis = radius / sin;
				collPos = (moveLine.Vec * dis * -1) + c1;
			}
			else{
				collPos = (moveLine.Vec * radius * -1) + c1;
			}
		}

		/// カプセルの外側の球に接触する
		else{
			/// 対象カプセルの中心線分の最近接点に球体を生成、その球体と移動する球の軌道との衝突を行い衝突点を算出する
			calSph.Set( c2, trgCap.R );
			CheckLineAndSphere( moveLine, calSph, ref collPos );
		}

		return true;
	}



	/// 点の移動と三角形との衝突チェック
	/**
	 * moveLineの軌道による点の移動とtrgTriとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
	static public bool CheckLineAndTriangle( GeometryLine moveLine, GeometryTriangle trgTri, ref Vector3 collPos )
	{
		/// 表裏判定
		if( moveLine.Vec.Dot( trgTri.Plane.Nor ) >= 0.0f ){
			return false;
		}
		/// 平面との交差チェック
		if( checkRayCrossPlane( trgTri.Plane, moveLine ) == false ){
			return false;
		}
		/// 平面との交点を求める
		if( getRayPlaneCrossPoint( trgTri.Plane, moveLine, ref collPos ) < epsilon ){
			return false;
		}
		/// 三角形の内外判定
		if( checkInsideTriangle( trgTri, collPos ) == false ){
			return false;
		}

		return true;
	}


	/// 点の移動と球との衝突チェック
	/**
	 * moveLineの軌道による点の移動とtrgSphとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
	static public bool CheckLineAndSphere( GeometryLine moveLine, GeometrySphere trgSph, ref Vector3 collPos )
	{
		Vector3 calVec  = trgSph.Pos - moveLine.StartPos;
		calVec = calVec.Normalize();

		if( calVec.Dot( moveLine.Vec ) >= 0.0f ){

			Vector3 calVec2 = trgSph.Pos - moveLine.StartPos;
			float isT = FMath.Sqrt( calVec2.Dot(calVec2) );
			if( isT < (moveLine.Length+trgSph.R) ){

				float dis = checkRayCrossSphere( moveLine.StartPos, moveLine.Vec, trgSph );
				if( dis >= 0.0f || dis < moveLine.Length ){
					collPos = moveLine.StartPos + (moveLine.Vec * dis);
					return true;
				}
			}
		}
		return false;
	}



/// public メンバ （球の移動）
///---------------------------------------------------------------------------

	/// 球の移動と三角形との衝突チェック
	/**
	 * moveCapの軌道による球の移動とtrgTriとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
	static public bool CheckSphereAndTriangle( GeometryCapsule moveCap, GeometryTriangle trgTri, ref Vector3 collPos )
	{
		/// 表裏判定
		if( moveCap.Line.Vec.Dot( trgTri.Plane.Nor ) >= 0.0f ){
			return false;
		}

		Vector3	firstHitPos, firstHitMovePos;
		float	trgDis;
		bool	isCrossFlg;


		/// 三角形の平面との球の最近接点（頂点P）を算出
		firstHitPos.X = moveCap.Line.StartPos.X + ( moveCap.R * (trgTri.Plane.Nor.X * -1) );
		firstHitPos.Y = moveCap.Line.StartPos.Y + ( moveCap.R * (trgTri.Plane.Nor.Y * -1) );
		firstHitPos.Z = moveCap.Line.StartPos.Z + ( moveCap.R * (trgTri.Plane.Nor.Z * -1) );

		/// 既にPと球の中心が三角形を跨いでいるかチェック
		isCrossFlg			= false;

		/// 符合が一致すると平面に対して交差した事にならない
		float sign1, sign2;
		sign1 = trgTri.Plane.Nor.Dot( moveCap.Line.StartPos ) + trgTri.Plane.D;
		sign2 = trgTri.Plane.Nor.Dot( firstHitPos ) + trgTri.Plane.D;

		if( (sign1 > epsilon && sign2 > epsilon) || (sign1 < epsilon && sign2 < epsilon) ){
			calLine.Vec		 = moveCap.Line.Vec;
			calLine.Length	 = moveCap.Line.Length;
			calLine.StartPos = firstHitPos;

			/// Pが移動する地点P'を算出
			calLine.EndPos.X = firstHitPos.X + ( moveCap.Line.Vec.X * moveCap.Line.Length );
			calLine.EndPos.Y = firstHitPos.Y + ( moveCap.Line.Vec.Y * moveCap.Line.Length );
			calLine.EndPos.Z = firstHitPos.Z + ( moveCap.Line.Vec.Z * moveCap.Line.Length );
		}

		else{
			isCrossFlg  = true;

			/// Pが移動する地点P'を算出
			firstHitMovePos.X = moveCap.Line.StartPos.X + ( moveCap.Line.Vec.X * (moveCap.Line.Length+moveCap.R) );
			firstHitMovePos.Y = moveCap.Line.StartPos.Y + ( moveCap.Line.Vec.Y * (moveCap.Line.Length+moveCap.R) );
			firstHitMovePos.Z = moveCap.Line.StartPos.Z + ( moveCap.Line.Vec.Z * (moveCap.Line.Length+moveCap.R) );
			firstHitPos		= moveCap.Line.StartPos;

			/// 三角形との最初に衝突する移動ラインを作成
			calLine.Set( firstHitPos, firstHitMovePos );
		}


		/// 移動ラインと平面との交差チェック
		if( isCrossFlg == true || checkRayCrossPlane( trgTri.Plane, calLine ) == true ){

			/// 平面との交点を求める
			trgDis = getRayPlaneCrossPoint( trgTri.Plane, calLine, ref collPos );

			/// 三角形の内外判定
			///-----------------------------------------------------
			if( checkInsideTriangle( trgTri, collPos ) == true ){
				return true;
			}

			/// カプセルが三角形の３辺と接触しているかのチェック
			///-----------------------------------------------------
			else{

				/// 三角形の３辺と点との最近接点を取得
				Vector3	linePos = new Vector3(0,0,0);

				getClosestPtPosTriangle( collPos, trgTri, ref linePos );

				Vector3	calVec = moveCap.Line.Vec * -1.0f;


				/// 球が三角形の交点と接触するかチェック
				calSph.Set( moveCap.Line.StartPos, moveCap.R );

				float aT = checkRayCrossSphere( linePos, calVec, calSph );
				trgDis = FMath.Sqrt( calSph.Pos.Dot( linePos ) );
				if( trgDis < calSph.R ){
					aT = trgDis;
				}

				/// 衝突確定	
				if( aT >= 0.0f && aT <= moveCap.Line.Length ){
					collPos = linePos;
					return true;
				}
			}
		}

		return false;
	}


	/// 球の移動と球との衝突チェック
	/**
	 * moveCapの軌道による球の移動とtrgSphとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
	static public bool CheckSphereAndSphere( GeometryCapsule moveCap, GeometrySphere trgSph, ref Vector3 collPos )
	{
		calSph.Set( trgSph.Pos, (moveCap.R + trgSph.R) );
		return( CheckLineAndSphere( moveCap.Line, calSph, ref collPos ) );
	}


	/// 球の移動とカプセルとの衝突チェック
	/**
	 * moveCapの軌道での球の移動とtrgCapとの衝突チェックを行います。
	 * collPosには衝突した座標を返します（返り値がtrueの時のみ情報が更新）
	 */
	static public bool CheckCapsuleAndCapsule( GeometryCapsule moveCap, GeometryCapsule trgCap, ref Vector3 collPos )
	{
	//		Vector3 c1, c2;
		Vector3 c1 = new Vector3(0,0,0);
		Vector3 c2 = new Vector3(0,0,0);

		float dist		= getClosestPtLineLine( moveCap.Line, trgCap.Line, ref c1, ref c2 );
		float radius	= moveCap.R + trgCap.R;

		/// カプセル同士の衝突は無し
		if( dist > radius*radius ){
			return false;
		}

		/// カプセル同士の衝突点の算出
		///-----------------------------------------------

		/// カプセルの中心の線分同士が交わる
		if( dist <= epsilon ){

			/// 線分同士の交点を用いて、２つの線分の角度を求める
			float rad = getRadian( c1, moveCap.Line.StartPos, trgCap.Line.EndPos );

			/// 衝突点を求める
			float sin = FMath.Sin( rad );
			if( sin > 0.0f ){
				float dis = radius / sin;
				collPos = (moveCap.Vec * dis * -1) + c1;
			}
			else{
				collPos = (moveCap.Vec * radius * -1) + c1;
			}
		}

		/// カプセルの外側の球に接触する
		else{
			/// 対象カプセルの中心線分の最近接点に球体を生成、その球体と移動する球の軌道との衝突を行い衝突点を算出する
			calSph.Set( c2, (moveCap.R+trgCap.R) );
			CheckLineAndSphere( moveCap.Line, calSph, ref collPos );
		}

		return true;
	}


/// public メンバ （最近接点の算出）
///---------------------------------------------------------------------------

	/// 点と線分の最近接点を算出
	static public float GetClosestPtPosLine( Vector3 trgPos, GeometryLine trgLine, ref Vector3 cPos )
	{
		return( getClosestPtPosLine( trgPos, trgLine.StartPos, trgLine.EndPos, ref cPos ) );
	}

	/// 点と線分の最近接点を算出
	static public float GetClosestPtPosRay( Vector3 trgPos, Vector3 trgRay, Vector3 trgRayPos, ref Vector3 cPos )
	{
		Vector3 ab, ca;
		float a_t;
		float a_dotCA;
		float a_dotAA;

		ab = trgRay;
		ca = trgPos - trgRayPos;

		a_dotCA = ca.Dot( ab );
		a_dotAA = ab.Dot( ab );

		if( a_dotAA <= epsilon ){
			a_t = 0.0f;
		}
		else{
			a_t = (a_dotCA / a_dotAA);
		}
//		if( a_t < epsilon ){
//			a_t = 0.0f;
//		}
//		else if( a_t > 1.0f ){
//			a_t = 1.0f;
//		}

		/// 最近接点の算出
		cPos.X = trgRayPos.X + ( a_t * ab.X );
		cPos.Y = trgRayPos.Y + ( a_t * ab.Y );
		cPos.Z = trgRayPos.Z + ( a_t * ab.Z );

		/// 最近接点と点との距離を返す
		Vector3 calVec = trgPos - cPos;
		float dis = FMath.Sqrt( calVec.Dot(calVec) );

		return dis;
	}


	/// 点と面との最近接点を算出
	static public float GetClosestPtPosPlane( Vector3 trgPos, GeometryPlane trgPlane, ref Vector3 cPos )
	{
		float num, denom;
		float t;

		/// 法線とレイの１頂点とのベクトルの内積を求める
		num	= trgPlane.Nor.Dot( trgPos );

		/// 法線とレイの内積を求める
		denom	= trgPlane.Nor.Dot( trgPlane.Nor );

		/// 平面と平行なので交点なし 
		if( denom == 0.0f ){
			return -1.0f;
		}
   
		/// 媒介変数を求める
		t = ( -(num + trgPlane.D) ) / denom;
 
		/// 直線の方程式から交点を求める
		cPos.X = trgPos.X + ( t * trgPlane.Nor.X );
		cPos.Y = trgPos.Y + ( t * trgPlane.Nor.Y );
		cPos.Z = trgPos.Z + ( t * trgPlane.Nor.Z );

		return t;
	}



/// private メンバ
///---------------------------------------------------------------------------

	/// レイと球との交差を求める
	/**
	 * 返り値：交点との距離を返す
	 */
	static private float checkRayCrossSphere( Vector3 trgRayPos, Vector3 trgRayVec, GeometrySphere trgSph )
	{
		float a, b, c, d;
		float t;
		Vector3 q;

		q = trgRayPos - trgSph.Pos;

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
			  ( q.Z * q.Z ) ) - ( trgSph.R * trgSph.R );


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


	/// ３頂点から角度を返す
	static private float getAngles( Vector3 posBase, Vector3 pos1, Vector3 pos2 )
	{
		Vector3 calA = pos1 - posBase;
		Vector3 calB = pos2 - posBase;

		float lba	= calA.Length();
		float lca	= calB.Length();
		float radian= FMath.Acos( calA.Dot(calB) / (lba*lca) );
		float angle	= (float)( 180.0f/pi * radian );

		return angle;
	}


	/// ３頂点からラジアンを返す
	static private float getRadian( Vector3 posBase, Vector3 pos1, Vector3 pos2 )
	{
		Vector3 calA = pos1 - posBase;
		Vector3 calB = pos2 - posBase;

		float lba	= calA.Length();
		float lca	= calB.Length();
		float radian= FMath.Acos( calA.Dot(calB) / (lba*lca) );

		return radian;
	}

	

	/// 線分と平面との交点をもとめる
	static private float getRayPlaneCrossPoint( GeometryPlane plane, GeometryLine line, ref Vector3 crossPos )
	{
		float num, denom;
		float t;

		/// 法線とレイの１頂点とのベクトルの内積を求める
		num	= plane.Nor.Dot( line.StartPos );

		/// 法線とレイの内積を求める
		denom	= plane.Nor.Dot( line.Vec );

		/// 平面と平行なので交点なし 
		if( denom == 0.0f ){
			return -1.0f;
		}
   
		/// 媒介変数を求める
		t = ( -(num + plane.D) ) / denom;
 
		/// 直線の方程式から交点を求める
		crossPos.X = line.StartPos.X + ( t * line.Vec.X );
		crossPos.Y = line.StartPos.Y + ( t * line.Vec.Y );
		crossPos.Z = line.StartPos.Z + ( t * line.Vec.Z );

		return t;
	}


	/// 点と三角形の３辺との最近接点を算出
	static private float getClosestPtPosTriangle( Vector3 trgPos, GeometryTriangle trgTri, ref Vector3 cPos )
	{
		float isDis, bestDis;
		Vector3 crosePos = new Vector3(0,0,0);

		bestDis = 0.0f;
		for( int i=0; i<3; i++ ){
			isDis = getClosestPtPosLine( trgPos, trgTri.GetPos(i), trgTri.GetPos((i+1)%3), ref crosePos );

			/// 最近接点を更新
			if( i == 0 || isDis < bestDis ){
				bestDis = isDis;
				cPos = crosePos;
			}
		}
		return bestDis;
	}


	/// 点と線分の最近接点を算出
	static private float getClosestPtPosLine( Vector3 trgPos, Vector3 linePos1, Vector3 linePos2, ref Vector3 cPos )
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

	/// ２つの線分の最近接点の算出
	static private float getClosestPtLineLine( GeometryLine p1, GeometryLine p2, ref Vector3 c1, ref Vector3 c2 )
	{
		Vector3 d1	= p1.EndPos - p1.StartPos;
		Vector3 d2	= p2.EndPos - p2.StartPos;
		Vector3 r	= p1.StartPos - p2.StartPos;
		
		float a		= d1.Dot(d1);
		float e		= d2.Dot(d2);
		float f		= d2.Dot(r);
		Vector3 cal;

		/// 両方とも線分が点になっている
		if( a <= epsilon && e <= epsilon ){
			length1 = length2 = 0.0f;
			c1 = p1.StartPos;
			c2 = p2.StartPos;
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

		c1 = p1.StartPos + d1 * length1;
		c2 = p2.StartPos + d2 * length2;
		cal = c1 - c2;
		return( cal.Dot( cal ) );
	}


	/// 線分と平面との交差ﾁｪｯｸ
	static private bool checkRayCrossPlane( GeometryPlane plane, GeometryLine line )
	{
		float sign1, sign2;

		/// 法線と頂点との位置関係を内積で求める（正：面の正面、負：面の背後）
		sign1 = plane.Nor.Dot( line.StartPos ) + plane.D;
		sign2 = plane.Nor.Dot( line.EndPos ) + plane.D;

		/// 符合が一致すると平面に対して交差した事にならない
		if( (sign1 > epsilon && sign2 > epsilon) || (sign1 < epsilon && sign2 < epsilon) ){
			return false;
		}
	
		return true;
	}


	/// 三角形と点との内外判定
	static private bool checkInsideTriangle( GeometryTriangle trgTri, Vector3 crossPos )
	{
		int type;
		float d1, d2, d3;

		if( FMath.Abs( trgTri.Plane.Nor.X ) < FMath.Abs( trgTri.Plane.Nor.Y ) ){
			type = (FMath.Abs( trgTri.Plane.Nor.Y ) < FMath.Abs( trgTri.Plane.Nor.Z ) ) ? 3 : 2;
		}
		else{
			type = (FMath.Abs( trgTri.Plane.Nor.X ) < FMath.Abs( trgTri.Plane.Nor.Z )) ? 3 : 1;
		}


		switch( type ){

		/// X方向に面が傾いているためX軸を破棄
		case 1:
			d1 = ( (crossPos.Y - trgTri.Pos1.Y) * (crossPos.Z - trgTri.Pos2.Z) ) -
				 ( (crossPos.Z - trgTri.Pos1.Z) * (crossPos.Y - trgTri.Pos2.Y) );
			d2 = ( (crossPos.Y - trgTri.Pos2.Y) * (crossPos.Z - trgTri.Pos3.Z) ) -
				 ( (crossPos.Z - trgTri.Pos2.Z) * (crossPos.Y - trgTri.Pos3.Y) );
			d3 = ( (crossPos.Y - trgTri.Pos3.Y) * (crossPos.Z - trgTri.Pos1.Z) ) -
				 ( (crossPos.Z - trgTri.Pos3.Z) * (crossPos.Y - trgTri.Pos1.Y) );
			break;

		/// y方向に面が傾いているためY軸を破棄
		case 2:
			d1 = ( (crossPos.X - trgTri.Pos1.X) * (crossPos.Z - trgTri.Pos2.Z) ) -
				 ( (crossPos.Z - trgTri.Pos1.Z) * (crossPos.X - trgTri.Pos2.X) );
			d2 = ( (crossPos.X - trgTri.Pos2.X) * (crossPos.Z - trgTri.Pos3.Z) ) -
				 ( (crossPos.Z - trgTri.Pos2.Z) * (crossPos.X - trgTri.Pos3.X) );
			d3 = ( (crossPos.X - trgTri.Pos3.X) * (crossPos.Z - trgTri.Pos1.Z) ) -
				 ( (crossPos.Z - trgTri.Pos3.Z) * (crossPos.X - trgTri.Pos1.X) );
			break;

		/// Z方向に面が傾いているためZ軸を破棄
		case 3:
			d1 = ( (crossPos.X - trgTri.Pos1.X) * (crossPos.Y - trgTri.Pos2.Y) ) -
				 ( (crossPos.Y - trgTri.Pos1.Y) * (crossPos.X - trgTri.Pos2.X) );
			d2 = ( (crossPos.X - trgTri.Pos2.X) * (crossPos.Y - trgTri.Pos3.Y) ) -
				 ( (crossPos.Y - trgTri.Pos2.Y) * (crossPos.X - trgTri.Pos3.X) );
			d3 = ( (crossPos.X - trgTri.Pos3.X) * (crossPos.Y - trgTri.Pos1.Y) ) -
				 ( (crossPos.Y - trgTri.Pos3.Y) * (crossPos.X - trgTri.Pos1.X) );
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


}
} // end ns DemoGame
