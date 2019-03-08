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
class UnitBullet3
	: Unit
{
	private int OBJ3D_E21 = 0;
	private int OBJ3D_E22 = 1;
	private int OBJ3D_E23 = 2;
	private int OBJ3D_E24 = 3;
	private int OBJ3D_E25 = 4;
	private int OBJ3D_MAX = 5;
			
//	private float modelScale = 0.0f;
	private Stopwatch stopwatch;
	private CommonUtil comUtil;
			
	private bool effectBulletStart = false;
	private bool effectBulletHitEnemy = false;
	private bool effectBulletHitNone = false;
	private Posture tmpPosture = new Posture();
	private Vector3 tmpTrgDir;
			
	private int[] effectSmokeList = new int[32];
	private Posture[] effectSmokePosture = new Posture[32];
	private int[] effectSmokeAnimIndex = new int[32];
	private float[] effectSmokeAnimFrame = new float[32];
			
	private Vector3 trgEndDir = new Vector3( 0.0f, 0.0f, 0.0f );
			
	private Vector3[] movePoints = new Vector3[5];
	private float[] movePointsLength = new float[5];
	private int movePointsIndex = 0;
	private int STATE_MOVE_INIT = 0;
	private int STATE_MOVE_UPDATE = 1;
	private int STATE_MOVE_END = 2;
//	private int state = 0;
			
	private float myMoveSpeed = 0.0f;
	private float moveG = 0;
	private int frameHalfPoint = 0;
	private Vector3 moveZDir = new Vector3( 0.0f, 0.0f, 0.0f );
	private Posture moveModelPosture = new Posture();
			
	public Vector3 attackDir = new Vector3( 0.0f, 0.0f, 0.0f );
			
			
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
				
		obj3d[OBJ3D_E21] = resContainer.GetObject3D( "E21" );
		obj3d[OBJ3D_E22] = resContainer.GetObject3D( "E22" );
		obj3d[OBJ3D_E23] = resContainer.GetObject3D( "E23" );
		obj3d[OBJ3D_E24] = resContainer.GetObject3D( "E24" );
		obj3d[OBJ3D_E25] = resContainer.GetObject3D( "E25" );
				
		animFrame = new float[obj3d.Length];
		animIndex = new int[obj3d.Length];
				
		colCapsule = new CollisionCapsule();
				
		type = Unit.TYPE_BUL;
				
//		modelScale = 0.0f;
				
		stopwatch = new Stopwatch();
		stopwatch.Start();

		animIndex[OBJ3D_E21] = 0;
		animFrame[OBJ3D_E21] = 0.0f;
		animIndex[OBJ3D_E22] = 0;
		animFrame[OBJ3D_E22] = 0.0f;
		animIndex[OBJ3D_E23] = 0;
		animFrame[OBJ3D_E23] = 0.0f;
		effectBulletStart = true;
		effectBulletHitEnemy = false;
		effectBulletHitNone = false;
				
		InitSmokeEffect();	
				
		int i = 0;
		while( i < movePoints.Length ){
			movePoints[i] = new Vector3( 0.0f, 0.0f, 0.0f );
			i++;
		}
				
		state = STATE_MOVE_INIT;
				
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
				
		TermSmokeEffect();

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
		float dis = 0.0f;
				
		if( state == STATE_MOVE_INIT ){
			myMoveSpeed = 3.0f;
			moveG = -0.1f;
			frameHalfPoint = (int)(myMoveSpeed/(-1*moveG));
			comUtil.GetDistance( muzzlePos, targetPos );
			moveZDir = ( targetPos - muzzlePos );
			state = STATE_MOVE_UPDATE;
		}
		if( state == STATE_MOVE_UPDATE ){
			GetPosition();
			moveModelPosture.SetPosture( GetPosture() );
					
			AddPositionW( (moveZDir.X/2)/frameHalfPoint, (moveZDir.Y/2)/frameHalfPoint, (moveZDir.Z/2)/frameHalfPoint );
			AddPositionW( 0.0f, myMoveSpeed, 0.0f );
			myMoveSpeed += moveG;
					
			moveModelPosture.LookAt( GetPosition().X, GetPosition().Y, GetPosition().Z );			
					
			dis = comUtil.GetDistance( GetPosition(), targetPos );
						
			if( dis <= 1.0f ){
				SetPosition( targetPos.X, targetPos.Y, targetPos.Z );
				state = STATE_MOVE_END;
			}
		}
		if( state == STATE_MOVE_END ){
		}
				
		if( !deadEffectStart ){
			AddSmokeEffect();
		}
				
		if( deadEffectStart && !deadEffectEnd ){
			if( hitEnemy ){
				if( !effectBulletHitEnemy ){
					animIndex[OBJ3D_E24] = 0;
					animFrame[OBJ3D_E24] = 0.0f;
					effectBulletHitEnemy = true;
				}
			}else{
				if( !effectBulletHitNone ){
					animIndex[OBJ3D_E25] = 0;
					animFrame[OBJ3D_E25] = 0.0f;
					effectBulletHitNone = true;
				}
			}
		}
				
		/// アクションを最大フレームまで再生
		 
		if( effectBulletStart ){
			animFrame[OBJ3D_E23]++;
			if( animFrame[OBJ3D_E23] >= obj3d[OBJ3D_E23].model.GetMotionFrameMax( animIndex[OBJ3D_E23] ) ){
				animFrame[OBJ3D_E23] = 0.0f;
				effectBulletStart = false;
			}
		}		

		if( effectBulletHitEnemy ){
			animFrame[OBJ3D_E24]++;
			if( animFrame[OBJ3D_E24] >= obj3d[OBJ3D_E24].model.GetMotionFrameMax( animIndex[OBJ3D_E24] ) ){
				animFrame[OBJ3D_E24] = 0.0f;
				effectBulletHitEnemy = false;
				hitEnemy = false;
				deadEffectEnd = true;
			}
		}
		if( effectBulletHitNone ){
			animFrame[OBJ3D_E25]++;
			if( animFrame[OBJ3D_E25] >= obj3d[OBJ3D_E25].model.GetMotionFrameMax( animIndex[OBJ3D_E25] ) ){
				animFrame[OBJ3D_E25] = 0.0f;
				effectBulletHitNone = false;
				hitEnemy = false;
				deadEffectEnd = true;
			}
		}
				
		UpdateSmokeEffect();
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
		Vector4 x = new Vector4(0,0,0,0);
		Vector4 y = new Vector4(0,0,0,0);
		Vector4 z = new Vector4(0,0,0,0);
		Vector4 w = new Vector4(0,0,0,0);
		Matrix4 tmpMat = new Matrix4(x, y, z, w);

		if( deadEffectEnd ){
			return;
		}
				
		if( !deadEffectStart ){
			tmpMat = moveModelPosture.GetPosture();
			tmpMat.M41 = GetPosition().X;
			tmpMat.M42 = GetPosition().Y;
			tmpMat.M43 = GetPosition().Z;
			
			obj3d[OBJ3D_E21].model.WorldMatrix = tmpMat;
	        obj3d[OBJ3D_E21].model.Update();
	        obj3d[OBJ3D_E21].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		if( effectBulletStart ){
			tmpMat = GetPosture();
			tmpMat.M41 = muzzlePos.X;
			tmpMat.M42 = muzzlePos.Y;
			tmpMat.M43 = muzzlePos.Z;
			tmpPosture.SetPosture( tmpMat );
			tmpPosture.LookAt(
					camInfo.GetPosture().GetPosition().X,
					camInfo.GetPosture().GetPosition().Y,
					camInfo.GetPosture().GetPosition().Z );
			obj3d[OBJ3D_E23].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E23].model.SetAnimFrame( animIndex[OBJ3D_E23], animFrame[OBJ3D_E23] );
	        obj3d[OBJ3D_E23].model.Update();
	        obj3d[OBJ3D_E23].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}

		if( effectBulletHitEnemy ){
			tmpTrgDir = ( targetPos - GetPosition() );
			if( tmpTrgDir.X != 0.0f || tmpTrgDir.Y != 0.0f && tmpTrgDir.Z != 0.0f )
			{
				tmpTrgDir = tmpTrgDir.Normalize();
				tmpMat = GetPosture();
				tmpMat.M41 = targetPos.X - tmpTrgDir.X;
				tmpMat.M42 = targetPos.Y - tmpTrgDir.Y;
				tmpMat.M43 = targetPos.Z - tmpTrgDir.Z;		
				tmpPosture.SetPosture( tmpMat );
			}
			else{
				tmpPosture.SetPosition( targetPos.X, targetPos.Y, targetPos.Z );
			}
			tmpPosture.SetPosition( targetPos.X, targetPos.Y, targetPos.Z );
			tmpPosture.LookAt(
					camInfo.GetPosture().GetPosition().X,
					camInfo.GetPosture().GetPosition().Y,
					camInfo.GetPosture().GetPosition().Z );
			obj3d[OBJ3D_E24].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E24].model.SetAnimFrame( animIndex[OBJ3D_E24], animFrame[OBJ3D_E24] );
	        obj3d[OBJ3D_E24].model.Update();
	        obj3d[OBJ3D_E24].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		if( effectBulletHitNone ){
			tmpTrgDir = ( targetPos - GetPosition() );
			if( tmpTrgDir.X != 0.0f || tmpTrgDir.Y != 0.0f && tmpTrgDir.Z != 0.0f )
			{
				tmpTrgDir = tmpTrgDir.Normalize();
				tmpMat = GetPosture();
				tmpMat.M41 = targetPos.X - tmpTrgDir.X;
				tmpMat.M42 = targetPos.Y - tmpTrgDir.Y;
				tmpMat.M43 = targetPos.Z - tmpTrgDir.Z;		
				tmpPosture.SetPosture( tmpMat );
			}
			else{
				tmpPosture.SetPosition( targetPos.X, targetPos.Y, targetPos.Z );
			}
			tmpPosture.SetPosture( tmpMat );
			tmpPosture.LookAt(
					camInfo.GetPosture().GetPosition().X,
					camInfo.GetPosture().GetPosition().Y,
					camInfo.GetPosture().GetPosition().Z );
			obj3d[OBJ3D_E25].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E25].model.SetAnimFrame( animIndex[OBJ3D_E25], animFrame[OBJ3D_E25] );
	        obj3d[OBJ3D_E25].model.Update();
	        obj3d[OBJ3D_E25].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		RenderSmokeEffect();
	}
			
	public void calcBulletModelScale()
	{
/*		comUtil = CommonUtil.Inst();
				
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
*/
	}
			
	private void InitSmokeEffect()
	{
		int i = 0;
		while( i < effectSmokeList.Length ){
			effectSmokeList[i] = -1;
			effectSmokeAnimIndex[i] = 0;
			effectSmokeAnimFrame[i] = 0.0f;
			effectSmokePosture[i] = new Posture();
			i++;
		}
	}
			
	private void TermSmokeEffect()
	{
		int i = 0;
		while( i < effectSmokeList.Length ){
			effectSmokeList[i] = -1;
			effectSmokeAnimIndex[i] = 0;
			effectSmokeAnimFrame[i] = 0.0f;
			effectSmokePosture[i] = null;
			i++;
		}
		effectSmokeList = null;
		effectSmokeAnimIndex = null;
		effectSmokeAnimFrame = null;
		effectSmokePosture = null;
	}
			
	private void AddSmokeEffect(){
		int i = 0;
		CameraInfo camInfo = CameraInfo.Inst();
		while( i < effectSmokeList.Length ){
			if( effectSmokeList[i] < 0 ){
				effectSmokeList[i] = 0;
				effectSmokeAnimIndex[i] = 0;
				effectSmokeAnimFrame[i] = 0.0f;
				effectSmokePosture[i].SetPosture( GetPosture() );
				effectSmokePosture[i].LookAt(
							camInfo.GetPosture().GetPosition().X,
							camInfo.GetPosture().GetPosition().Y,
							camInfo.GetPosture().GetPosition().Z );
				break;
			}
			i++;
		}
	}
			
	private void UpdateSmokeEffect(){
		int i = 0;
		while( i < effectSmokeList.Length ){
			if( effectSmokeList[i] >= 0 ){
				effectSmokeAnimFrame[i]++;
				if( effectSmokeAnimFrame[i] >= obj3d[OBJ3D_E22].model.GetMotionFrameMax( effectSmokeAnimIndex[i] ) ){
					effectSmokeList[i] = -1;
				}
			}
			i++;
		}		
	}
			
	private void RenderSmokeEffect()
	{
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
				
		int i = 0;
		while( i < effectSmokeList.Length ){
			if( effectSmokeList[i] >= 0 ){
				obj3d[OBJ3D_E22].model.WorldMatrix = effectSmokePosture[i].GetPosture();
				obj3d[OBJ3D_E22].model.SetAnimFrame( effectSmokeAnimIndex[i], effectSmokeAnimFrame[i] );
		        obj3d[OBJ3D_E22].model.Update();
		        obj3d[OBJ3D_E22].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
			}
			i++;
		}
	}
			
	private void CreateMovePoints( Vector3 orgPos, Vector3 trgPos )
	{		
		CommonUtil comUtil = CommonUtil.Inst();
		float dis = 0.0f;
				
		trgEndDir = ( trgPos - orgPos );
		trgEndDir = trgEndDir.Normalize();
		dis = comUtil.GetDistance( orgPos, trgPos );

		movePoints[0].X = orgPos.X;
		movePoints[0].Y = orgPos.Y;
		movePoints[0].Z = orgPos.Z;
				
		movePoints[1].X = orgPos.X + trgEndDir.X * (dis/4);
		movePoints[1].Y = orgPos.Y + trgEndDir.Y * (dis/4) + 25.0f;
		movePoints[1].Z = orgPos.Z + trgEndDir.Z * (dis/4);
				
		movePoints[2].X = orgPos.X + trgEndDir.X * (dis/2);
		movePoints[2].Y = orgPos.Y + trgEndDir.Y * (dis/2) + 50.0f;
		movePoints[2].Z = orgPos.Z + trgEndDir.Z * (dis/2);
				
		movePoints[3].X = orgPos.X + trgEndDir.X * (dis-dis/4);
		movePoints[3].Y = orgPos.Y + trgEndDir.Y * (dis-dis/4) + 25.0f;
		movePoints[3].Z = orgPos.Z + trgEndDir.Z * (dis-dis/4);
				
		movePoints[4].X = orgPos.X + trgEndDir.X * (dis);
		movePoints[4].Y = orgPos.Y + trgEndDir.Y * (dis);
		movePoints[4].Z = orgPos.Z + trgEndDir.Z * (dis);
				
				
		movePointsLength[0] = comUtil.GetDistance( movePoints[0], movePoints[1] );
		movePointsLength[1] = comUtil.GetDistance( movePoints[1], movePoints[2] );
		movePointsLength[2] = comUtil.GetDistance( movePoints[2], movePoints[3] );
		movePointsLength[3] = comUtil.GetDistance( movePoints[3], movePoints[4] );
		movePointsLength[4] = 0.0f;
				
		attackDir.X = movePoints[1].X;
		attackDir.Y = movePoints[1].Y;
		attackDir.Z = movePoints[1].Z;
	}
			
	public int CalcBulletTime()
	{
		float addPosX = 0.0f;
		float addPosY = 0.0f;
		float addPosZ = 0.0f;
		float dis = 0.0f;
		Vector3 movePointsDir = new Vector3( 0.0f, 0.0f, 0.0f );
		int loopCnt = 0;
		int calcState = STATE_MOVE_INIT;
		Posture testPosture = new Posture();
		testPosture.SetPosture( GetPosture() );
				
		while( calcState != STATE_MOVE_END ){
			if( calcState == STATE_MOVE_INIT ){
				CreateMovePoints( muzzlePos, targetPos );
				movePointsIndex = 0;
				testPosture.SetPosition( muzzlePos.X, muzzlePos.Y, muzzlePos.Z );
				calcState = STATE_MOVE_UPDATE;
			}
			if( calcState == STATE_MOVE_UPDATE ){
				if( movePointsIndex == 0 ){
					movePointsDir = ( movePoints[1] - muzzlePos );
					movePointsDir = movePointsDir.Normalize();
					addPosX = movePointsDir.X;
					addPosY = movePointsDir.Y;
					addPosZ = movePointsDir.Z;

					/// 移動先へ位置変更
					testPosture.AddPositionW( addPosX, addPosY, addPosZ );
						
					testPosture.LookAt( movePoints[1].X, movePoints[1].Y, movePoints[1].Z );
						
					dis = comUtil.GetDistance( testPosture.GetPosition(), movePoints[1] );
						
					if( dis <= 1.0f ){
						testPosture.SetPosition( movePoints[1].X, movePoints[1].Y, movePoints[1].Z );
						movePointsIndex = 2;
					}
				}
				else if( movePointsIndex == movePoints.Length ){
					if( targetPos.X == movePoints[movePointsIndex-1].X &&
						targetPos.Y == movePoints[movePointsIndex-1].Y &&
						targetPos.Z == movePoints[movePointsIndex-1].Z ){
						testPosture.SetPosition( targetPos.X, targetPos.Y, targetPos.Z );
						calcState = STATE_MOVE_END;
					}
					else{
							
					movePointsDir = ( targetPos - movePoints[movePointsIndex-1] );
					movePointsDir = movePointsDir.Normalize();
					addPosX = movePointsDir.X;
					addPosY = movePointsDir.Y;
					addPosZ = movePointsDir.Z;

					/// 移動先へ位置変更
					testPosture.AddPositionW( addPosX, addPosY, addPosZ );
						
					testPosture.LookAt( targetPos.X, targetPos.Y, targetPos.Z );
						
					dis = comUtil.GetDistance( testPosture.GetPosition(), targetPos );
						
					if( dis <= 1.0f ){
						testPosture.SetPosition( targetPos.X, targetPos.Y, targetPos.Z );
						calcState = STATE_MOVE_END;
					}
					}
				}
				else{
					movePointsDir = ( movePoints[movePointsIndex] - movePoints[movePointsIndex-1] );
					movePointsDir = movePointsDir.Normalize();
					addPosX = movePointsDir.X * 1.0f;
					addPosY = movePointsDir.Y * 1.0f;
					addPosZ = movePointsDir.Z * 1.0f;

					/// 移動先へ位置変更
					testPosture.AddPositionW( addPosX, addPosY, addPosZ );

					testPosture.LookAt( movePoints[movePointsIndex].X, movePoints[movePointsIndex].Y, movePoints[movePointsIndex].Z );
						
					dis = comUtil.GetDistance( testPosture.GetPosition(), movePoints[movePointsIndex] );
						
					if( dis <= 1.0f ){
						testPosture.SetPosition( movePoints[movePointsIndex].X, movePoints[movePointsIndex].Y, movePoints[movePointsIndex].Z );
						movePointsIndex++;
					}
				}
							
				loopCnt++;
			}
					
		}
//		return loopCnt;
				
		loopCnt = 0;
		float tmpPosBufY = 0.0f;
		calcState = STATE_MOVE_INIT;
		while( calcState != STATE_MOVE_END ){
			if( calcState == STATE_MOVE_INIT ){
				myMoveSpeed = 3.0f;
				moveG = -0.1f;
				frameHalfPoint = (int)(myMoveSpeed/(-1*moveG));
				comUtil.GetDistance( muzzlePos, targetPos );
				moveZDir = ( targetPos - muzzlePos );
				tmpPosBufY = muzzlePos.Y;
				calcState = STATE_MOVE_UPDATE;
			}
			if( calcState == STATE_MOVE_UPDATE ){
					
				tmpPosBufY += myMoveSpeed;
				myMoveSpeed += moveG;
						
				if( myMoveSpeed < 0 && tmpPosBufY < (targetPos.Y) ){
					calcState = STATE_MOVE_END;
				}
			}
			loopCnt++;
		}

		return loopCnt;
	}

}

} // end ns DefenseDemo
