/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoModel;
using DemoGame;


namespace DefenseDemo {

/// ユニットクラス
/**
 * @version 0.1, 2011/06/23
 */
public
class UnitOffense4
	: Unit
{
	private int OBJ3D_ENE04L2 = 0;
	private int OBJ3D_E41 = 1;
	private int OBJ3D_MAX = 2;
			
	private Rout routData;
	private Stopwatch stopwatch;
	private CommonUtil comUtil;
	private bool showFlg = true;
			
	private Posture tmpPosture = new Posture();
	private bool playDeadSound = false;
			
	private Posture calcAnglePosture = new Posture();
	private Vector3 calcVecPosZ = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 calcVecPosY = new Vector3( 0.0f, 0.0f, 0.0f );
	private Posture moveNextPoint = new Posture();
	private Vector3 moveNextPointDir = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 calcDis = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 calcDis2 = new Vector3( 0.0f, 0.0f, 0.0f );
	private Posture drawTmpPosture = new Posture();
	private float moveRotPercent = 0.0f;
			
	/// 初期化
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Init()
	{
		obj3d = new Object3D[OBJ3D_MAX];
		colMoveSphere = new CollisionSphere();
		colCapsule = new CollisionCapsule();
		routData = new Rout();
		stopwatch = new Stopwatch();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
				
		obj3d[OBJ3D_ENE04L2] = resContainer.GetObject3D( "ENE04L2" );
		obj3d[OBJ3D_E41] = resContainer.GetObject3D( "E41" );
				
		animFrame = new float[obj3d.Length];
		animIndex = new int[obj3d.Length];
				
		/// ユニットタイプの設定
		type = Unit.TYPE_ACT;
		/// ユニット種別の設定
		kind = Unit.KIND_ACT_FLOATING_POT;
		/// ユニットHPを取得
		hp = Unit.ACT_PARAM_DATA[(kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_HP];
		cost = Unit.ACT_PARAM_DATA[(kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_GET_POINT];
				
		routData.Init();
				
		stopwatch.Start();
				
		routNum = 0;
				
		comUtil = CommonUtil.Inst();
				
		animIndex[OBJ3D_ENE04L2] = 0;
		animFrame[OBJ3D_ENE04L2] = 0.0f;
				
		animIndex[OBJ3D_E41] = 0;
		animFrame[OBJ3D_E41] = 0.0f;
				
		playDeadSound = false;
				
		return true;
	}

	/// 解放
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Term()
	{
		routData.Term();
		routData = null;
				
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
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();				

		float camDis = comUtil.GetDistance( GetPosition(), camInfo.GetPosture().GetPosition() );
		if( camDis < Unit.lodLength ){
			changeModelLevel0 = true;
		}
		else{
			changeModelLevel2 = true;
		}
		/// モデルレベル変更フラグ確認
		if( changeModelLevel0 || changeModelLevel2 ){
			/// レベル0
			if( changeModelLevel0 && modelLevel != 0 ){
				obj3d[0] = resContainer.GetObject3D( "ENE04L0" );
				modelLevel = Unit.MODEL_LEVEL_HIGH;
			}
			/// 既に変更済みの場合はフラグOFF
			else{
				changeModelLevel0 = false;
			}

			/// レベル2
			if( changeModelLevel2 && modelLevel != 2 ){
				obj3d[0] = resContainer.GetObject3D( "ENE04L2" );
				modelLevel = Unit.MODEL_LEVEL_LOW;
			}
			/// 既に変更済みの場合はフラグOFF
			else{
				changeModelLevel2 = false;
			}
		}

		if( deadEffectStart ){
			if( !playDeadSound ){
				AudioManager.PlaySound( "SE_DEAD", false, 1.0f );
				playDeadSound = true;
			}
			animFrame[OBJ3D_E41]++;
			if( animFrame[OBJ3D_E41] >= obj3d[OBJ3D_E41].model.GetMotionFrameMax( animIndex[OBJ3D_E41] ) ){
				animFrame[OBJ3D_E41] = 0.0f;
				deadEffectEnd = true;
			}
		}
		
		if( !deadEffectStart  ){
					
			SetColCapsule();
				
			landAngleX = GetUnitAngle();
				
			if( showFlg ){

				/// 移動値を取得
				float moveFrameValue = Unit.ACT_PARAM_DATA[(kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_MOVE_SPEED];
				/// 1フレームの移動値を算出
				moveFrameValue = (moveFrameValue/30.0f);

				/// 坂道か判定
				if( landAngleX > 100.0f ){
					/// 坂道なら、パラメータによって移動値に制限を掛ける
					moveFrameValue = moveFrameValue * (Unit.ACT_PARAM_DATA[(kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_CLIME]/100.0f);
				}

				/// ルートポイントが最大数を超えているか確認
				if( routNum < routData.GetRoutPointMaxNum( routePattern )-1 ){
					/// 次のルートポイントのMatrixを取得
					Matrix4 matNextPoint = routData.GetRoutPointMatrix( routePattern, routNum+1 );

					/// 次ルートポイントの位置を一時確保
					calcDis.X = matNextPoint.M41;
					calcDis.Y = matNextPoint.M42;
					calcDis.Z = matNextPoint.M43;
					
					/// 現在位置と次ルートポイントの距離を算出
					float disNext = comUtil.GetDistance( GetPosition(), calcDis );

					/// 算出した距離が、1フレームの移動値より少ないか確認
					if( disNext <= moveFrameValue )
					{
						/// 1フレームの移動値より少ない場合
						/// ルートポイントを進める
						routNum++;
						/// ルートポイントが最大値以上の場合、最大値を設定
						if( routNum >= routData.GetRoutPointMaxNum( routePattern ) ){
							routNum = (routData.GetRoutPointMaxNum( routePattern )-1);
						}
						else{
							/// 次ルートポイントのMatrixを取得
							matNextPoint = routData.GetRoutPointMatrix( routePattern, routNum+1 );
							/// 現ルートポイントのMatrixを取得し、位置を更新
							SetPosture( routData.GetRoutPointMatrix( routePattern, routNum ) );
							/// 1フレームの移動値残り分を移動する
							moveNextPoint.SetPosture( GetPosture() );
							moveNextPointDir.X = matNextPoint.M41 - GetPosition().X ;
							moveNextPointDir.Y = matNextPoint.M42 - GetPosition().Y ;
							moveNextPointDir.Z = matNextPoint.M43 - GetPosition().Z ;
							moveNextPointDir = moveNextPointDir.Normalize();
							moveNextPoint.AddPositionW(
										moveNextPointDir.X * (moveFrameValue-disNext),
										moveNextPointDir.Y * (moveFrameValue-disNext),
										moveNextPointDir.Z * (moveFrameValue-disNext) );
							SetPosition( moveNextPoint.GetPosition().X, moveNextPoint.GetPosition().Y, moveNextPoint.GetPosition().Z );
						}
					}
					else{
						/// 1フレームの移動値分移動する
						moveNextPoint.SetPosture( GetPosture() );
						moveNextPointDir.X = matNextPoint.M41 - GetPosition().X ;
						moveNextPointDir.Y = matNextPoint.M42 - GetPosition().Y ;
						moveNextPointDir.Z = matNextPoint.M43 - GetPosition().Z ;
						moveNextPointDir = moveNextPointDir.Normalize();
						moveNextPoint.AddPositionW(
									moveNextPointDir.X * moveFrameValue,
									moveNextPointDir.Y * moveFrameValue,
									moveNextPointDir.Z * moveFrameValue );
						SetPosition( moveNextPoint.GetPosition().X, moveNextPoint.GetPosition().Y, moveNextPoint.GetPosition().Z );
					}
							
					/// 次のルートポイントのMatrixを取得
					matNextPoint = routData.GetRoutPointMatrix( routePattern, routNum+1 );

					/// 次ルートポイントの位置を一時確保
					calcDis.X = matNextPoint.M41;
					calcDis.Y = matNextPoint.M42;
					calcDis.Z = matNextPoint.M43;
					
					/// 現在位置と次ルートポイントの距離を算出
					disNext = comUtil.GetDistance( GetPosition(), calcDis );
							
					Matrix4 matNowPoint = routData.GetRoutPointMatrix( routePattern, routNum );
					calcDis2.X = matNowPoint.M41;
					calcDis2.Y = matNowPoint.M42;
					calcDis2.Z = matNowPoint.M43;
					float disNextMax = comUtil.GetDistance( calcDis2, calcDis );
					moveRotPercent = (disNext/disNextMax);		

				}

			}
					
			if( animIndex[OBJ3D_ENE04L2] == 0 ){
				if( (float)((float)hp/(float)Unit.ACT_PARAM_DATA[(kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_HP]) <= 0.5f ){
					animIndex[OBJ3D_ENE04L2] = 1;
					animFrame[OBJ3D_ENE04L2] = 0.0f;
				}
			}
			else if( animIndex[OBJ3D_ENE04L2] == 1 ){
				if( (float)((float)hp/(float)Unit.ACT_PARAM_DATA[(kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_HP]) <= 0.25f ){
					animIndex[OBJ3D_ENE04L2] = 2;
					animFrame[OBJ3D_ENE04L2] = 0.0f;
				}
			}

			animFrame[OBJ3D_ENE04L2]++;
			if( animFrame[OBJ3D_ENE04L2] >= obj3d[OBJ3D_ENE04L2].model.GetMotionFrameMax( animIndex[OBJ3D_ENE04L2] ) ){
				animFrame[OBJ3D_ENE04L2] = 0.0f;
			}
		}
				
	}

	/// 描画
	/**
	 */
	public override void Render()
	{
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
				
		if( !showFlg ){
			return;
		}
				
		if( deadEffectEnd || deadEffectStart ){
			return;
		}
		routData.GetRoutPointRot( routePattern, routNum, routNum+1, moveRotPercent );
				
		if( nowRouteLine == Unit.ROUTE_LINE_LEFT ){
			drawTmpPosture.SetPosture( routData.GetRoutPointMatrixAddOffset( routePattern, routNum, -3.0f, GetPosition() ) );
		}
		else if( nowRouteLine == Unit.ROUTE_LINE_RIGHT ){
			drawTmpPosture.SetPosture( routData.GetRoutPointMatrixAddOffset( routePattern, routNum, 3.0f, GetPosition() ) );
		}
		else{
			drawTmpPosture.SetPosture( routData.GetRoutPointMatrixAddOffset( routePattern, routNum, 0.0f, GetPosition() ) );
		}
				
		obj3d[OBJ3D_ENE04L2].model.WorldMatrix = drawTmpPosture.GetPosture();
		obj3d[OBJ3D_ENE04L2].model.SetAnimFrame( animIndex[OBJ3D_ENE04L2], animFrame[OBJ3D_ENE04L2] );
        obj3d[OBJ3D_ENE04L2].model.Update();
        obj3d[OBJ3D_ENE04L2].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
				
	}

	/// 描画
	/**
	 */
	public override void RenderAlpha()
	{
        GraphicsContext graphics = Renderer.GetGraphicsContext();
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
				
		if( deadEffectEnd ){
			return;
		}
		if( deadEffectStart ){
			tmpPosture.SetPosture( GetPosture() );
			tmpPosture.LookAt(
					camInfo.GetPosture().GetPosition().X,
					camInfo.GetPosture().GetPosition().Y,
					camInfo.GetPosture().GetPosition().Z );
			obj3d[OBJ3D_E41].model.WorldMatrix = tmpPosture.GetPosture();
			obj3d[OBJ3D_E41].model.SetAnimFrame( animIndex[OBJ3D_E41], animFrame[OBJ3D_E41] );
	        obj3d[OBJ3D_E41].model.Update();
	        obj3d[OBJ3D_E41].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
	}
			
	public void SetColCapsule()
	{
		Posture tmp = new Posture();
		Vector3 posS;
		Vector3 posE;
				
		tmp.SetPosture( GetPosture() );
		tmp.AddPosition( 0.0f, 0.0f, 1.0f );
		posS = tmp.GetPosition();
		tmp.SetPosture( GetPosture() );
		tmp.AddPosition( 0.0f, 0.0f, -1.0f );
		posE = tmp.GetPosition();
		colCapsule.CreateCapsule( posS, posE, 3.0f );
	}

	public float GetUnitAngle()
	{
		float angle = 0.0f;
				
		calcAnglePosture.SetPosture( GetPosture() );
		calcAnglePosture.AddPosition( 0.0f, 0.0f, 1.0f );
		calcVecPosZ = calcAnglePosture.GetPosition();
					
		calcAnglePosture.SetPosture( GetPosture() );
		calcAnglePosture.AddPositionW( 0.0f, 1.0f, 0.0f );
		calcVecPosY = calcAnglePosture.GetPosition();
					
		angle = comUtil.getRadian(
					GetPosition(),
					calcVecPosZ,
					calcVecPosY );
		angle = angle * 180.0f / (float)Math.PI;
				
		return angle;
	}

}

} // end ns DefenseDemo
