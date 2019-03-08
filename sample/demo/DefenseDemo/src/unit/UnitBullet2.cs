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


namespace DefenseDemo {

/// ユニット(弾)クラス
/**
 * @version 0.1, 2011/06/23
 */
public
class UnitBullet2
	: Unit
{
	private int OBJ3D_E11 = 0;
	private int OBJ3D_E12 = 1;
	private int OBJ3D_E13 = 2;
	private int OBJ3D_E14 = 3;
	private int OBJ3D_MAX = 4;
			
	private int movePointFroSec = 60;
	private CommonUtil comUtil;
	private Stopwatch timerAnim;
	private bool effectBulletHitEnemy = false;
	private bool effectBulletHitNone = false;
	private Posture tmpPosture = new Posture();
	private Vector3 tmpTrgDir;
			
			
	/// 初期化
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Init()
	{
		obj3d = new Object3D[OBJ3D_MAX];
		colMoveSphere = new CollisionSphere();
				
		comUtil = CommonUtil.Inst();
				
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
				
//		obj3d[0] = resContainer.GetObject3D( "CHARA" );
//		obj3d[0] = resContainer.GetObject3D( "E00" );
		obj3d[OBJ3D_E11] = resContainer.GetObject3D( "E11" );
		obj3d[OBJ3D_E12] = resContainer.GetObject3D( "E12" );
		obj3d[OBJ3D_E13] = resContainer.GetObject3D( "E13" );
//		obj3d[OBJ3D_E13] = resContainer.GetObject3D( "E14" );
		obj3d[OBJ3D_E14] = resContainer.GetObject3D( "E14" );
				
		animFrame = new float[obj3d.Length];
		animIndex = new int[obj3d.Length];
//		this.eye = Matrix4.Translation(new Vector3(0.0f, 0.0f, 0.0f));
				
//		AddYPR( 0.0f, 3.2f, 0.0f );
//		AddYPR( 0.0f, 180.0f*(3.141593f/180.0f), 0.0f );
		SetPosition( 0.0f, 10.0f, 10.0f );
//		LookAt( -10.0f, -10.0f, -10.0f );
//		AddYPR( 90.0f*(3.141593f/180.0f), 180.0f*(3.141593f/180.0f), 0.0f );
//		AddYPR( 45.0f*(3.141593f/180.0f), 180.0f*(3.141593f/180.0f), 0.0f*(3.141593f/180.0f) );
		
//		animIndex = 0;
//		animFrame = 0.0f;
				
//		bgModel = new BgModel( 6.0f, 0.5f, 4, 4, true );

		colCapsule = new CollisionCapsule();
				
		type = Unit.TYPE_BUL;
				
		timerAnim = new Stopwatch();
		timerAnim.Start();
		animIndex[OBJ3D_E11] = 0;
		animFrame[OBJ3D_E11] = 0.0f;
		animIndex[OBJ3D_E12] = 0;
		animFrame[OBJ3D_E12] = 0.0f;
				
		return true;
	}

	/// 解放
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Term()
	{
				
		timerAnim.Stop();
		timerAnim = null;
				
		for( int i=0; i<obj3d.Length; i++ ){
			obj3d[i] = null;
		}
		obj3d = null;
		return true;
	}

	/// 更新
	/**
	 */
	public override void Update()
	{
		float addPosX = 0.0f;
		float addPosY = 0.0f;
		float addPosZ = 0.0f;
		Vector3 oldPos = new Vector3(
					GetPosition().X,
					GetPosition().Y,
					GetPosition().Z );
				
		Vector3 trgDir = ( targetPos - GetPosition() );
		addPosX = trgDir.X;
		addPosY = trgDir.Y;
		addPosZ = trgDir.Z;
					
		if( GetPosition().Y <= targetPos.Y ){
			deadEffectStart = true;
		}
				
		if( deadEffectStart ){
			addPosX = 0.0f;
			addPosY = 0.0f;
			addPosZ = 0.0f;
		}

		/// 移動先へ位置変更
		AddPositionW( addPosX, addPosY, addPosZ );
				
		colCapsule.CreateCapsule(
					oldPos,
					GetPosition(),
					1.0f );
			
		/// 当たり判定後に表示用位置情報を更新する


		if( deadEffectStart && !deadEffectEnd ){
			if( hitEnemy ){
				if( !effectBulletHitEnemy ){
					animIndex[OBJ3D_E13] = 0;
					animFrame[OBJ3D_E13] = 0.0f;
					effectBulletHitEnemy = true;
				}
			}else{
				if( !effectBulletHitNone ){
					animIndex[OBJ3D_E14] = 0;
					animFrame[OBJ3D_E14] = 0.0f;
					effectBulletHitNone = true;
				}
			}
		}
		if( effectBulletHitEnemy ){
			animFrame[OBJ3D_E13]++;
			if( animFrame[OBJ3D_E13] >= obj3d[OBJ3D_E13].model.GetMotionFrameMax( animIndex[OBJ3D_E13] ) ){
				animFrame[OBJ3D_E13] = 0.0f;
				effectBulletHitEnemy = false;
				hitEnemy = false;
				deadEffectEnd = true;
			}
		}
		if( effectBulletHitNone ){
			animFrame[OBJ3D_E14]++;
			if( animFrame[OBJ3D_E14] >= obj3d[OBJ3D_E14].model.GetMotionFrameMax( animIndex[OBJ3D_E14] ) ){
				animFrame[OBJ3D_E14] = 0.0f;
				effectBulletHitNone = false;
				hitEnemy = false;
				deadEffectEnd = true;
			}
		}
				
	}

	/// 描画
	/**
	 */
	public override void Render()
	{
	}
			
	/// 描画
	/**
	 */
	public override void RenderAlpha()
	{
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
		Matrix4 tmpMat;

		if( deadEffectEnd ){
			return;
		}
				
		if( effectBulletHitEnemy ){
			tmpTrgDir = ( targetPos - camInfo.GetPosture().GetPosition() );
			tmpTrgDir = tmpTrgDir.Normalize();
			tmpMat = GetPosture();
			tmpMat.M41 = targetPos.X - (tmpTrgDir.X * 3.0f);
			tmpMat.M42 = targetPos.Y - (tmpTrgDir.Y * 3.0f);
			tmpMat.M43 = targetPos.Z - (tmpTrgDir.Z * 3.0f);		
			tmpPosture.SetPosture( tmpMat );
			tmpPosture.LookAt(
					camInfo.GetPosture().GetPosition().X,
					camInfo.GetPosture().GetPosition().Y,
					camInfo.GetPosture().GetPosition().Z );
			obj3d[OBJ3D_E13].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E13].model.SetAnimFrame( animIndex[OBJ3D_E13], animFrame[OBJ3D_E13] );
	        obj3d[OBJ3D_E13].model.Update();
	        obj3d[OBJ3D_E13].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		if( effectBulletHitNone ){
			tmpTrgDir = ( targetPos - camInfo.GetPosture().GetPosition() );
			tmpTrgDir = tmpTrgDir.Normalize();
			tmpMat = GetPosture();
			tmpMat.M41 = targetPos.X - tmpTrgDir.X;
			tmpMat.M42 = targetPos.Y - tmpTrgDir.Y;
			tmpMat.M43 = targetPos.Z - tmpTrgDir.Z;		
			tmpPosture.SetPosture( tmpMat );
			tmpPosture.LookAt(
					camInfo.GetPosture().GetPosition().X,
					camInfo.GetPosture().GetPosition().Y,
					camInfo.GetPosture().GetPosition().Z );
			obj3d[OBJ3D_E14].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E14].model.SetAnimFrame( animIndex[OBJ3D_E14], animFrame[OBJ3D_E14] );
	        obj3d[OBJ3D_E14].model.Update();
	        obj3d[OBJ3D_E14].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
	}
			
	public long getTime2EnemyHit( Vector3 org, Vector3 trg ){
		float distanse = 0.0f;
		float time = 0.0f;
				
		distanse = comUtil.GetDistance( org, trg );
		time = (distanse/1.0f);
		time = time * (1000.0f/(float)movePointFroSec);
				
		return (long)time;
		
	}
			

}

} // end ns DefenseDemo
