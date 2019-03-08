/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;

namespace DefenseDemo {

/// 三角形状の当たり情報クラス
/**
 * @version 0.1, 2011/06/23
 */
public class CollisionTriangles {

	/// 頂点データ
	public List< CollisionTriangle > triangle;

	/// コンストラクタ
	/**
	 */
	public CollisionTriangles()
	{
	}

	/// デストラクタ
	/**
	 */
	~CollisionTriangles()
	{
	}

	/// 初期化
	/**
	 * @param [in] num 頂点数
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init( float[] data )
	{
		int i = 0;
			
		triangle = new List<CollisionTriangle>();
			
//		Console.WriteLine( "[log][CollisionTriangles.cs]Init()data.Length:"+data.Length );
			
		while( i<data.Length ){
			CollisionTriangle tri = new CollisionTriangle();
			tri.SetPos( new Vector3(data[i],data[i+1],data[i+2]),
						new Vector3(data[i+3],data[i+4],data[i+5]),
						new Vector3(data[i+6],data[i+7],data[i+8]));
			triangle.Add( tri );
			i+=9;
		}
			
//		Console.WriteLine( "[log][CollisionTriangles.cs]Init()i:"+i );

		return true;
	}

	/// 解放
	/**
	 */
	public void Term()
	{
		if( triangle != null ){
//			Console.WriteLine( "[log][CollisionTriangles.cs]Term()triangle.Count:"+triangle.Count);
			for( int i=0; i<triangle.Count; i++ ){
				triangle[i] = null;
			}
			triangle.Clear();
		}
		
		triangle = null;
	}

} // end ns DefenseDemo

}