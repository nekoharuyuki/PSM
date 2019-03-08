/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoModel;
using DemoGame;

namespace DefenseDemo {

/// ルートクラス
/**
 * @version 0.1, 2011/06/23
 */
public class Rout {

	public static int ROUT_PATTURN_0 = 0;
	public static int ROUT_PATTURN_1 = 1;
	public static int ROUT_PATTURN_2 = 2;
	public static int ROUT_PATTURN_3 = 3;
	public static int ROUT_PATTURN_4 = 4;
	public static int ROUT_CAMERA_0 = 5;

	private Vector3 pos = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 rot = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 scale = new Vector3( 1.0f, 1.0f, 1.0f );
	private Matrix4 calcMat;
	private Posture routTmpPosture = new Posture();
	private Posture rotTmpPosture = new Posture();


	/// コンストラクタ
	/**
	 */
	public Rout()
	{
	}

	/// デストラクタ
	/**
	 */
	~Rout()
	{
	}

	/// 初期化
	/**
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init()
	{
		return true;
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	/// ルートポイントの描画を行う(※デバッグ用)
	/**
	 */
	public void Render()
	{
	}

	/// 指定パターン、指定番目のルートポイントMatrixを取得
	/**
	 * @param [int] patturn : パターンID
	 * @param [int] num : ルート内のポイント番号
	 * @return Matrix : ルートポイントMatrix
	 */
	public Matrix4 GetRoutPointMatrix( int patturn, int num )
	{
		if( patturn == ROUT_PATTURN_0 ){
			if( (num*3) >= RoutDataPattern.PatternPos00.Length ){
				num = (RoutDataPattern.PatternPos00.Length/3)-1;
			}
			pos.X = (float)RoutDataPattern.PatternPos00[ (num*3)+0 ];
			pos.Y = (float)RoutDataPattern.PatternPos00[ (num*3)+1 ];
			pos.Z = (float)RoutDataPattern.PatternPos00[ (num*3)+2 ];

			rot.X = (float)RoutDataPattern.PatternRot00[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot00[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot00[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_1 ){
			if( (num*3) >= RoutDataPattern.PatternPos01.Length ){
				num = (RoutDataPattern.PatternPos01.Length/3)-1;
			}
			pos.X = (float)RoutDataPattern.PatternPos01[ (num*3)+0 ];
			pos.Y = (float)RoutDataPattern.PatternPos01[ (num*3)+1 ];
			pos.Z = (float)RoutDataPattern.PatternPos01[ (num*3)+2 ];

			rot.X = (float)RoutDataPattern.PatternRot01[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot01[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot01[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_2 ){
			if( (num*3) >= RoutDataPattern.PatternPos02.Length ){
				num = (RoutDataPattern.PatternPos02.Length/3)-1;
			}
			pos.X = (float)RoutDataPattern.PatternPos02[ (num*3)+0 ];
			pos.Y = (float)RoutDataPattern.PatternPos02[ (num*3)+1 ];
			pos.Z = (float)RoutDataPattern.PatternPos02[ (num*3)+2 ];

			rot.X = (float)RoutDataPattern.PatternRot02[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot02[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot02[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_3 ){
			if( (num*3) >= RoutDataPattern.PatternPos03.Length ){
				num = (RoutDataPattern.PatternPos03.Length/3)-1;
			}
			pos.X = (float)RoutDataPattern.PatternPos03[ (num*3)+0 ];
			pos.Y = (float)RoutDataPattern.PatternPos03[ (num*3)+1 ];
			pos.Z = (float)RoutDataPattern.PatternPos03[ (num*3)+2 ];

			rot.X = (float)RoutDataPattern.PatternRot03[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot03[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot03[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_4 ){
			if( (num*3) >= RoutDataPattern.PatternPos04.Length ){
				num = (RoutDataPattern.PatternPos04.Length/3)-1;
			}
			pos.X = (float)RoutDataPattern.PatternPos04[ (num*3)+0 ];
			pos.Y = (float)RoutDataPattern.PatternPos04[ (num*3)+1 ];
			pos.Z = (float)RoutDataPattern.PatternPos04[ (num*3)+2 ];

			rot.X = (float)RoutDataPattern.PatternRot04[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot04[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot04[ (num*3)+2 ];
		}
		else if( patturn == ROUT_CAMERA_0 ){
			if( (num*3) >= RoutDataPattern.CameraPos00.Length ){
				num = (RoutDataPattern.CameraPos00.Length/3)-1;
			}
			pos.X = (float)RoutDataPattern.CameraPos00[ (num*3)+0 ];
			pos.Y = (float)RoutDataPattern.CameraPos00[ (num*3)+1 ];
			pos.Z = (float)RoutDataPattern.CameraPos00[ (num*3)+2 ];

			rot.X = (float)RoutDataPattern.CameraRot00[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.CameraRot00[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.CameraRot00[ (num*3)+2 ];
		}
			
		if( patturn != ROUT_CAMERA_0 ){
			pos.Y += 0.1f;
		}

		calcMat = Matrix4.Transformation( pos, scale );
		Matrix4 tmpRotMat;
		Matrix4.RotationYxz( 0.0f, rot.Y, rot.Z, out tmpRotMat );

		calcMat = calcMat * tmpRotMat;			

		return calcMat;
	}
		
	public Matrix4 GetRoutPointMatrixAddOffset( int patturn, int num, float offset, Vector3 position )
	{
		Matrix4 routData = GetRoutPointMatrix( patturn, num );
		Matrix4 tmpRotMat;

		if( patturn == ROUT_PATTURN_0 ){
			if( (num*3) >= RoutDataPattern.PatternPos00.Length ){
				num = (RoutDataPattern.PatternPos00.Length/3)-1;
			}
			rot.X = (float)RoutDataPattern.PatternRot00[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot00[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot00[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_1 ){
			if( (num*3) >= RoutDataPattern.PatternPos01.Length ){
				num = (RoutDataPattern.PatternPos01.Length/3)-1;
			}
			rot.X = (float)RoutDataPattern.PatternRot01[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot01[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot01[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_2 ){
			if( (num*3) >= RoutDataPattern.PatternPos02.Length ){
				num = (RoutDataPattern.PatternPos02.Length/3)-1;
			}
			rot.X = (float)RoutDataPattern.PatternRot02[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot02[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot02[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_3 ){
			if( (num*3) >= RoutDataPattern.PatternPos03.Length ){
				num = (RoutDataPattern.PatternPos03.Length/3)-1;
			}
			rot.X = (float)RoutDataPattern.PatternRot03[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot03[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot03[ (num*3)+2 ];
		}
		else if( patturn == ROUT_PATTURN_4 ){
			if( (num*3) >= RoutDataPattern.PatternPos04.Length ){
				num = (RoutDataPattern.PatternPos04.Length/3)-1;
			}
			rot.X = (float)RoutDataPattern.PatternRot04[ (num*3)+0 ];
			rot.Y = (float)RoutDataPattern.PatternRot04[ (num*3)+1 ];
			rot.Z = (float)RoutDataPattern.PatternRot04[ (num*3)+2 ];
		}
			
		rotTmpPosture.SetPosture( routData );
		rotTmpPosture.SetPosition( position.X, position.Y, position.Z );
		rotTmpPosture.AddPosition( offset, 0.0f, 0.0f );
			
		tmpRotMat = rotTmpPosture.GetPosture();
			
		tmpRotMat = tmpRotMat * Matrix4.RotationX( rot.X );
			
		return tmpRotMat;
	}

		
		
	/// 指定パターンのルートポイント最大数を取得
	/**
	 * @param [in] patturn : パターンID
	 * @return int : ルートポイント最大数
	 */
	public int GetRoutPointMaxNum( int patturn )
	{
		int len = 0;
		if( patturn == ROUT_PATTURN_0 ){
			len = RoutDataPattern.PatternPos00.Length / 3;
		}
		else if( patturn == ROUT_PATTURN_1 ){
			len = RoutDataPattern.PatternPos01.Length / 3;
		}
		else if( patturn == ROUT_PATTURN_2 ){
			len = RoutDataPattern.PatternPos02.Length / 3;
		}
		else if( patturn == ROUT_PATTURN_3 ){
			len = RoutDataPattern.PatternPos03.Length / 3;
		}
		else if( patturn == ROUT_PATTURN_4 ){
			len = RoutDataPattern.PatternPos04.Length / 3;
		}
		else if( patturn == ROUT_CAMERA_0 ){
			len = RoutDataPattern.CameraPos00.Length / 3;
		}
			
		return len;
	}
		
	public Vector3 GetRoutPointPos( int patturn, int point, float xOffset )
	{
		routTmpPosture.SetPosture( GetRoutPointMatrix( patturn, point ) );
		routTmpPosture.AddPosition( xOffset, 0.0f, 0.0f );
			
		return routTmpPosture.GetPosition();
	}
		
	public Vector3 GetRoutPointRot( int patturn, int nowId, int nextId, float percent )
	{
		int num = 0;
		Vector3 result = new Vector3(0,0,0);;
		Vector3 nowRot = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 nextRot = new Vector3( 0.0f, 0.0f, 0.0f );

		if( patturn == ROUT_PATTURN_0 ){
			if( (num*3) >= RoutDataPattern.PatternRot00.Length ){
				num = (RoutDataPattern.PatternRot00.Length/3)-1;
			}
			num = nowId;
			nowRot.X = (float)RoutDataPattern.PatternRot00[ (num*3)+0 ];
			nowRot.Y = (float)RoutDataPattern.PatternRot00[ (num*3)+1 ];
			nowRot.Z = (float)RoutDataPattern.PatternRot00[ (num*3)+2 ];
				
			num = nextId;
			if( (num*3) >= RoutDataPattern.PatternRot00.Length ){
				num = (RoutDataPattern.PatternRot00.Length/3)-1;
			}
			nextRot.X = (float)RoutDataPattern.PatternRot00[ (num*3)+0 ];
			nextRot.Y = (float)RoutDataPattern.PatternRot00[ (num*3)+1 ];
			nextRot.Z = (float)RoutDataPattern.PatternRot00[ (num*3)+2 ];
				
			result.X = (nextRot.X - nowRot.X)*percent;
			result.Y = (nextRot.Y - nowRot.Y)*percent;
			result.Z = (nextRot.Z - nowRot.Z)*percent;
		}
		else if( patturn == ROUT_PATTURN_1 ){
			if( (num*3) >= RoutDataPattern.PatternRot01.Length ){
				num = (RoutDataPattern.PatternRot01.Length/3)-1;
			}
			num = nowId;
			nowRot.X = (float)RoutDataPattern.PatternRot01[ (num*3)+0 ];
			nowRot.Y = (float)RoutDataPattern.PatternRot01[ (num*3)+1 ];
			nowRot.Z = (float)RoutDataPattern.PatternRot01[ (num*3)+2 ];
				
			num = nextId;
			if( (num*3) >= RoutDataPattern.PatternRot01.Length ){
				num = (RoutDataPattern.PatternRot01.Length/3)-1;
			}
			nextRot.X = (float)RoutDataPattern.PatternRot01[ (num*3)+0 ];
			nextRot.Y = (float)RoutDataPattern.PatternRot01[ (num*3)+1 ];
			nextRot.Z = (float)RoutDataPattern.PatternRot01[ (num*3)+2 ];
				
			result.X = (nextRot.X - nowRot.X)*percent;
			result.Y = (nextRot.Y - nowRot.Y)*percent;
			result.Z = (nextRot.Z - nowRot.Z)*percent;
		}
		else if( patturn == ROUT_PATTURN_2 ){
			if( (num*3) >= RoutDataPattern.PatternRot02.Length ){
				num = (RoutDataPattern.PatternRot02.Length/3)-1;
			}
			num = nowId;
			nowRot.X = (float)RoutDataPattern.PatternRot02[ (num*3)+0 ];
			nowRot.Y = (float)RoutDataPattern.PatternRot02[ (num*3)+1 ];
			nowRot.Z = (float)RoutDataPattern.PatternRot02[ (num*3)+2 ];
				
			num = nextId;
			if( (num*3) >= RoutDataPattern.PatternRot02.Length ){
				num = (RoutDataPattern.PatternRot02.Length/3)-1;
			}
			nextRot.X = (float)RoutDataPattern.PatternRot02[ (num*3)+0 ];
			nextRot.Y = (float)RoutDataPattern.PatternRot02[ (num*3)+1 ];
			nextRot.Z = (float)RoutDataPattern.PatternRot02[ (num*3)+2 ];
				
			result.X = (nextRot.X - nowRot.X)*percent;
			result.Y = (nextRot.Y - nowRot.Y)*percent;
			result.Z = (nextRot.Z - nowRot.Z)*percent;
		}
		else if( patturn == ROUT_PATTURN_3 ){
			if( (num*3) >= RoutDataPattern.PatternRot03.Length ){
				num = (RoutDataPattern.PatternRot02.Length/3)-1;
			}
			num = nowId;
			nowRot.X = (float)RoutDataPattern.PatternRot03[ (num*3)+0 ];
			nowRot.Y = (float)RoutDataPattern.PatternRot03[ (num*3)+1 ];
			nowRot.Z = (float)RoutDataPattern.PatternRot03[ (num*3)+2 ];
				
			num = nextId;
			if( (num*3) >= RoutDataPattern.PatternRot03.Length ){
				num = (RoutDataPattern.PatternRot03.Length/3)-1;
			}
			nextRot.X = (float)RoutDataPattern.PatternRot03[ (num*3)+0 ];
			nextRot.Y = (float)RoutDataPattern.PatternRot03[ (num*3)+1 ];
			nextRot.Z = (float)RoutDataPattern.PatternRot03[ (num*3)+2 ];
				
			result.X = (nextRot.X - nowRot.X)*percent;
			result.Y = (nextRot.Y - nowRot.Y)*percent;
			result.Z = (nextRot.Z - nowRot.Z)*percent;
		}
		else if( patturn == ROUT_PATTURN_4 ){
			if( (num*3) >= RoutDataPattern.PatternRot04.Length ){
				num = (RoutDataPattern.PatternRot04.Length/3)-1;
			}
			num = nowId;
			nowRot.X = (float)RoutDataPattern.PatternRot04[ (num*3)+0 ];
			nowRot.Y = (float)RoutDataPattern.PatternRot04[ (num*3)+1 ];
			nowRot.Z = (float)RoutDataPattern.PatternRot04[ (num*3)+2 ];
				
			num = nextId;
			if( (num*3) >= RoutDataPattern.PatternRot04.Length ){
				num = (RoutDataPattern.PatternRot04.Length/3)-1;
			}
			nextRot.X = (float)RoutDataPattern.PatternRot04[ (num*3)+0 ];
			nextRot.Y = (float)RoutDataPattern.PatternRot04[ (num*3)+1 ];
			nextRot.Z = (float)RoutDataPattern.PatternRot04[ (num*3)+2 ];
				
			result.X = (nextRot.X - nowRot.X)*percent;
			result.Y = (nextRot.Y - nowRot.Y)*percent;
			result.Z = (nextRot.Z - nowRot.Z)*percent;
		}
			
		return result;
	}

}

} // end ns DefenseDemo
