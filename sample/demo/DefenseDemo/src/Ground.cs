/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;
using DemoGame;


namespace DefenseDemo {

/// 地面クラス
/**
 * @version 0.1, 2011/06/23
 */
public
class Ground
	: Posture
{
	/// 3Dデータ
	private Object3D[] obj3d;
			
	public List< CollisionTriangles > collisionGround;
	public Matrix4[] routePoint;
	public String[] routePointName;
	public Matrix4 tmpMat;
	private Stopwatch ActTimer = new Stopwatch();
	private float animFrame = 0.0f;
	private int animIndex = 0;
	private Rout routData = new Rout();
	private int[] routNum = new int[10];
			
	/// コンストラクタ
	/**
	 */
	public Ground()
	{
		collisionGround = new List<CollisionTriangles>();
	}

	/// デストラクタ
	/**
	 */
	~Ground()
	{
		collisionGround.Clear();
		collisionGround = null;
	}
			
	/// 初期化
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public bool Init()
	{
		int i = 0;
				
		obj3d = new Object3D[8];
				
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
				
		obj3d[0] = resContainer.GetObject3D( "GROUND" );
		obj3d[1] = resContainer.GetObject3D( "ROUT" );
		obj3d[3] = resContainer.GetObject3D( "UNITPOS" );
		obj3d[7] = resContainer.GetObject3D( "ENE02L2" );
				
		SetPosition( 0.0f, 0.0f, 0.0f );
				
		/// 当たり判定処理の初期化
		for( i=0;i<2;i++ ){
			collisionGround.Add( new CollisionTriangles() );
		}
		collisionGround[0].Init( CollisionStageData.Positions00 );
		collisionGround[1].Init( CollisionStageData.Positions50 );
			
		routePoint = new Matrix4[obj3d[1].model.Bones.Length];
		routePointName = new string[obj3d[1].model.Bones.Length];
		i = 0;
		while( i < obj3d[1].model.Bones.Length ){
			routePoint[i] = obj3d[1].model.Bones[i].WorldMatrix;
					
			Quaternion quat = new Quaternion(
						obj3d[1].model.Bones[i].Rotation.X,
						obj3d[1].model.Bones[i].Rotation.Y,
						obj3d[1].model.Bones[i].Rotation.Z,
						obj3d[1].model.Bones[i].Rotation.W );
			routePoint[i] = routePoint[i]* quat.ToMatrix4();
			routePoint[i].M41 = obj3d[1].model.Bones[i].Translation.X;
			routePoint[i].M42 = obj3d[1].model.Bones[i].Translation.Y;
			routePoint[i].M43 = obj3d[1].model.Bones[i].Translation.Z;
			routePointName[i] = obj3d[1].model.Bones[i].Name;

			i++;
		}
				
		obj3d[7].actFrame = 0;
		obj3d[7].actIndex = 0;
				
		ActTimer.Start();
		animFrame = 0.0f;
		animIndex = 0;
				
		for(i=0;i<routNum.Length;i++ ){
			routNum[i] = (i*(-30));
		}

				
		return true;
	}

	/// 解放
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public bool Term()
	{
		ActTimer.Stop();

		for( int i=0; i<obj3d.Length; i++ ){
			obj3d[i] = null;
		}
		obj3d = null;
		return true;
	}

	/// 更新
	/**
	 */
	public void Update()
	{
		/// アクションを最大フレームまで再生
		animFrame++;
		if( animFrame >= obj3d[0].model.GetMotionFrameMax( animIndex ) ){
			/// 次のアクションへ移行
			animFrame = 0.0f;
		}
			
		for( int i=0;i<routNum.Length;i++ ){
			routNum[i]++;
			if( routNum[i] >= routData.GetRoutPointMaxNum(Rout.ROUT_PATTURN_0) ){
				routNum[i] = 0;
			}
		}
	}

	/// 描画
	/**
	 */
	public void Render()
	{
		GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
				
		obj3d[0].model.WorldMatrix = GetPosture();
		obj3d[0].model.SetAnimFrame( animIndex , animFrame );
		obj3d[0].model.Update();
		obj3d[0].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse() ), camInfo.GetViewEye() );
	}
			
	public int GetBoneId( int index, String name )
	{
		int resultId = -1;
		int i = 0;
		while( i < obj3d[index].model.Bones.Length ){
			if( obj3d[index].model.Bones[i].Name == name ){
				resultId = i;
				break;
			}
			i++;
		}

		return resultId;
	}
}

} // end ns DefenseDemo
