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
class UnitBullet1
	: Unit
{
	private int OBJ3D_E01 = 0;
	private int OBJ3D_E02 = 1;
	private int OBJ3D_E03 = 2;
	private int OBJ3D_E04 = 3;
	private int OBJ3D_MAX = 4;
			
	private float modelScale = 0.0f;
	private Stopwatch stopwatch;
	private CommonUtil comUtil;
			
	private bool effectBulletStart = false;
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
				
		obj3d[OBJ3D_E01] = resContainer.GetObject3D( "E01" );
		obj3d[OBJ3D_E02] = resContainer.GetObject3D( "E02" );
		obj3d[OBJ3D_E03] = resContainer.GetObject3D( "E03" );
		obj3d[OBJ3D_E04] = resContainer.GetObject3D( "E04" );
				
		animFrame = new float[obj3d.Length];
		animIndex = new int[obj3d.Length];
				
		colCapsule = new CollisionCapsule();
				
		type = Unit.TYPE_BUL;
				
		kind = Unit.KIND_DEF_LASER;
				
		modelScale = 0.0f;
				
		stopwatch = new Stopwatch();
		stopwatch.Start();

		animIndex[0] = 0;
		animFrame[0] = 0.0f;
		animIndex[1] = 0;
		animFrame[1] = 0.0f;
		effectBulletStart = true;
		effectBulletHitEnemy = false;
		effectBulletHitNone = false;
				
				
		return true;
	}

	/// 解放
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Term()
	{
		stopwatch.Stop();
		stopwatch = null;

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
		trgDir = trgDir.Normalize();
		addPosX = trgDir.X*(Unit.DEF_PARAM_DATA[(kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_BUL_SPEED]/100.0f);
		addPosY = trgDir.Y*(Unit.DEF_PARAM_DATA[(kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_BUL_SPEED]/100.0f);
		addPosZ = trgDir.Z*(Unit.DEF_PARAM_DATA[(kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_BUL_SPEED]/100.0f);
				
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
			
		/// 当たり用位置情報を更新する
		calcBulletModelScale();
		
		if( deadEffectStart && !deadEffectEnd ){
			if( hitEnemy ){
				if( !effectBulletHitEnemy ){
					animIndex[OBJ3D_E03] = 0;
					animFrame[OBJ3D_E03] = 0.0f;
					effectBulletHitEnemy = true;
				}
			}else{
				if( !effectBulletHitNone ){
					animIndex[OBJ3D_E04] = 0;
					animFrame[OBJ3D_E04] = 0.0f;
					effectBulletHitNone = true;
				}
			}
		}
				
		/// アクションを最大フレームまで再生
		if( effectBulletStart ){
			animFrame[OBJ3D_E02]++;
			if( animFrame[OBJ3D_E02] >= obj3d[OBJ3D_E02].model.GetMotionFrameMax( animIndex[OBJ3D_E02] ) ){
				animFrame[OBJ3D_E02] = 0.0f;
				effectBulletStart = false;
			}
		}
		if( effectBulletHitEnemy ){
			animFrame[OBJ3D_E03]++;
			if( animFrame[OBJ3D_E03] >= obj3d[OBJ3D_E03].model.GetMotionFrameMax( animIndex[OBJ3D_E03] ) ){
				animFrame[OBJ3D_E03] = 0.0f;
				effectBulletHitEnemy = false;
				hitEnemy = false;
				deadEffectEnd = true;
			}
		}
		if( effectBulletHitNone ){
			animFrame[OBJ3D_E04]++;
			if( animFrame[OBJ3D_E04] >= obj3d[OBJ3D_E04].model.GetMotionFrameMax( animIndex[OBJ3D_E04] ) ){
				animFrame[OBJ3D_E04] = 0.0f;
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
				
		if( !deadEffectStart ){
			tmpMat = GetPosture();
			tmpMat = tmpMat * Matrix4.Scale( 1.0f, 1.0f, modelScale );
			obj3d[OBJ3D_E01].model.WorldMatrix = tmpMat;
	        obj3d[OBJ3D_E01].model.Update();
	        obj3d[OBJ3D_E01].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		if( effectBulletStart ){
			tmpMat = GetPosture();
			tmpMat.M41 = muzzlePos.X;
			tmpMat.M42 = muzzlePos.Y;
			tmpMat.M43 = muzzlePos.Z;
			obj3d[OBJ3D_E02].model.WorldMatrix = tmpMat;
			obj3d[OBJ3D_E02].model.SetAnimFrame( animIndex[OBJ3D_E02], animFrame[OBJ3D_E02] );
	        obj3d[OBJ3D_E02].model.Update();
	        obj3d[OBJ3D_E02].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
			
		if( effectBulletHitEnemy ){
			tmpTrgDir = ( targetPos - GetPosition() );
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
			obj3d[OBJ3D_E03].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E03].model.SetAnimFrame( animIndex[OBJ3D_E03], animFrame[OBJ3D_E03] );
	        obj3d[OBJ3D_E03].model.Update();
	        obj3d[OBJ3D_E03].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		if( effectBulletHitNone ){
			tmpTrgDir = ( targetPos - GetPosition() );
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
			obj3d[OBJ3D_E04].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E04].model.SetAnimFrame( animIndex[OBJ3D_E04], animFrame[OBJ3D_E04] );
	        obj3d[OBJ3D_E04].model.Update();
	        obj3d[OBJ3D_E04].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
	}
			
	public void calcBulletModelScale()
	{
		comUtil = CommonUtil.Inst();
				
		float dis = comUtil.GetDistance( muzzlePos, GetPosition() );
		if( dis < 15.0f ){
			modelScale = (float)(dis/15.0f);
		}else{
			modelScale = 1.0f;
					
					
			dis = comUtil.GetDistance( GetPosition(), targetPos );
			if( dis < 15.0f ){
				modelScale = (float)(dis/15.0f);
			}else{
				modelScale = 1.0f;
			}
		}
				
				
	}

}

} // end ns DefenseDemo
