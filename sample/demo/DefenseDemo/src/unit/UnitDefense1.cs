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

/// ユニットクラス
/**
 * @version 0.1, 2011/06/23
 */
public
class UnitDefense1
	: Unit
{
	private Stopwatch stopwatch;
	private bool timer;
	private long prevTime;
	private long currentTime;
	private CommonUtil comUtil;
			
	private Posture upPosture = new Posture();
	private Posture downPosture = new Posture();
	private Posture muzzlePosture = new Posture();
	private bool rot2TrgInit = false;
	private bool rot2TrgUpdate = false;
	private Vector3 rot2TrgPos = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 rot2TrgDir = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 rot2TrgStartPos = new Vector3( 0.0f, 0.0f, 0.0f );
	private float rot2TrgAngleDis = 0.0f;
			
			
	/// 初期化
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Init()
	{
		int i = 0;
				
//		ActTimer.Start();
				
		obj3d = new Object3D[6];
		colMoveSphere = new CollisionSphere();
		animFrame = new float[6];
		animIndex = new int[6];
				
		comUtil = CommonUtil.Inst();
				
		ResourceDataContainer resContainer = ResourceDataContainer.Inst();
				
		obj3d[0] = resContainer.GetObject3D( "A01L2" );
		obj3d[1] = resContainer.GetObject3D( "E01" );
		obj3d[5] = resContainer.GetObject3D( "FUTA_E" );
				
		SetPosition( 0.0f, 10.0f, 10.0f );
		
		type = Unit.TYPE_DEF;

		kind = Unit.KIND_DEF_LASER;

		length = Unit.DEF_PARAM_DATA[(kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_ACT_RANGE];
				
		attack = Unit.DEF_PARAM_DATA[(kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_ACT_POWER];
				
		bulletUnitIndex = new int[16];
		for( i = 0; i < bulletUnitIndex.Length; i++ ){
			bulletUnitIndex[i] = -1;
		}
				
		stopwatch = new Stopwatch();
		stopwatch.Start();
		timer = false;
		prevTime = 0;
		currentTime = 0;
				
		enterActFlg = false;
				
		for( i=0; i<animIndex.Length; i++ ){
			animIndex[i] = -1;
			animFrame[i] = 0.0f;
		}
				
		animIndex[0] = 0;
				
		modelLevel = Unit.MODEL_LEVEL_LOW;
				
				
		rot2TrgStart = false;
				
		return true;
	}

	/// 解放
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public override bool Term()
	{
		stopwatch.Stop();
		
		comUtil = null;
				
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
				obj3d[0] = resContainer.GetObject3D( "A01L0" );
				modelLevel = Unit.MODEL_LEVEL_HIGH;
			}
			/// 既に変更済みの場合はフラグOFF
			else{
				changeModelLevel0 = false;
			}

			/// レベル2
			if( changeModelLevel2 && modelLevel != 2 ){
				obj3d[0] = resContainer.GetObject3D( "A01L2" );
				modelLevel = Unit.MODEL_LEVEL_LOW;
			}
			/// 既に変更済みの場合はフラグOFF
			else{
				changeModelLevel2 = false;
			}
		}

		/// 攻撃中フラグがONの場合
		if( unitAtkWait ){
			animIndex[1] = 0;
			animIndex[3] = 0;
			animIndex[4] = 0;
		}
				
		/// 登場アクション済フラグがOFFの場合
		if( !enterActFlg ){
			/// アクションIndexが0以上
			if( animIndex[0] == 0 ){
				/// アクションを最大フレームまで再生
				animFrame[0]++;
				if( animFrame[0] >= obj3d[0].model.GetMotionFrameMax( animIndex[0] ) ){
					/// 次のアクションへ移行
					animFrame[0] = 0.0f;
					animIndex[0] = 1;
					enterActFlg = true;
							
					setMuzzlePos();

				}
			}
		}else{
			if( animIndex[0] == 1 ){
				animFrame[0]++;
				if( animFrame[0] >= obj3d[0].model.GetMotionFrameMax( animIndex[0] ) ){
					animFrame[0] = 0.0f;
				}
			}
		}			
				
				
		if( Unit.DEF_PARAM_DATA[(kind*DEF_PARAM_NUM)+Unit.DEF_PARAM_ACT_INTERVAL] > 0 ){
			if( unitAtkWait ){
				if( !timer ){
					timer = true;
					prevTime = stopwatch.ElapsedMilliseconds;
				}
				else{
					currentTime = stopwatch.ElapsedMilliseconds;
					if( currentTime - prevTime >= (1000/Unit.DEF_PARAM_DATA[(kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_ACT_INTERVAL]) ){
						unitAtkWait = false;
						timer = false;
					}
				}
			}
			else{
				if( timer ){
					timer = false;
				}
			}
		}

		/// ターゲットへ向け砲身回転
		if( rot2TrgStart && !rot2TrgInit ){
			/// 回転初期化フラグON
			rot2TrgInit = true;

			/// 現在位置とターゲットとの距離算出
			float dis = comUtil.GetDistance( GetPosition(), targetPos );
					
			rot2TrgAngleDis = (dis*1.5f)/90.0f;

			/// 回転開始位置算出
			Posture rot2TrgPosture = new Posture();
			rot2TrgPosture.SetPosture( GetPosture() );
			if( targetOld >= 0 ){
				rot2TrgPosture.LookAt( targetPosOld.X, targetPosOld.Y, targetPosOld.Z );
			}
			rot2TrgPosture.AddPosition( 0.0f, 0.0f, dis );
			rot2TrgStartPos = rot2TrgPosture.GetPosition();

			/// 回転終了位置設定
			rot2TrgPos = targetPos;

			/// 回転開始点⇒終了点へのベクトル算出
			rot2TrgDir.X = targetPos.X - rot2TrgStartPos.X;
			rot2TrgDir.Y = targetPos.Y - rot2TrgStartPos.Y;
			rot2TrgDir.Z = targetPos.Z - rot2TrgStartPos.Z;
				
			rot2TrgDir = Vector3.Normalize( rot2TrgDir );
			targetPos = rot2TrgStartPos;
					
			rot2TrgUpdate = true;
		}
		/// 回転処理
		if( rot2TrgUpdate ){
			/// 現状の回転位置、回転終了位置の距離
			float dis = comUtil.GetDistance( rot2TrgStartPos, rot2TrgPos );		
			/// 距離が1回の移動量より大きい場合、現状位置から終了位置に進める
			if( dis >= (rot2TrgAngleDis*Unit.DEF_UNIT_TURN_VALUE) ){
				rot2TrgStartPos.X = rot2TrgStartPos.X + rot2TrgDir.X*(rot2TrgAngleDis*Unit.DEF_UNIT_TURN_VALUE);
				rot2TrgStartPos.Y = rot2TrgStartPos.Y + rot2TrgDir.Y*(rot2TrgAngleDis*Unit.DEF_UNIT_TURN_VALUE);
				rot2TrgStartPos.Z = rot2TrgStartPos.Z + rot2TrgDir.Z*(rot2TrgAngleDis*Unit.DEF_UNIT_TURN_VALUE);
						
				targetPos = rot2TrgStartPos;
			}
			/// 距離が1回の移動良より小さい場合
			else{
				/// 回転終了処理
				targetPos = rot2TrgPos;
				rot2TrgInit = false;
				rot2TrgUpdate = false;
				rot2TrgStart = false;
			}
					
//			setCannonDir2Target( new Vector3( targetPos.X, targetPos.Y, targetPos.Z), 0.0f, 0.0f );
		}
		/// ターゲットがあり、回転処理を行っていない場合に、ターゲットIDと位置を確保しておく
		if( target >= 0 && !rot2TrgStart ){
			targetOld = target;
			targetPosOld = targetPos;
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
				
		if( !enterActFlg ){
			obj3d[5].model.WorldMatrix = GetPosture();
			obj3d[5].model.SetAnimFrame( animIndex[0] , animFrame[0] );
	        obj3d[5].model.Update();
	        obj3d[5].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
		}
				
		obj3d[0].model.WorldMatrix = GetPosture();
		obj3d[0].model.SetAnimFrame( animIndex[0] , animFrame[0] );
        obj3d[0].model.Update();
				
		/// ターゲットがあり
		if( target >= 0 ){
			/// 回転処理開始フラグONで、回転処理がまだ行われていない場合
			if( rot2TrgStart && !rot2TrgUpdate && targetOld >= 0 ){
				/// 過去のターゲット位置に向ける
				setCannonDir2Target( new Vector3( targetPosOld.X, targetPosOld.Y, targetPosOld.Z), 0.0f, 0.0f );
			}
			else{
				setCannonDir2Target( new Vector3( targetPos.X, targetPos.Y, targetPos.Z), 0.0f, 0.0f );
			}
			setMuzzlePos();
		}
				
		if( target < 0 && targetOld >= 0 ){
			setCannonDir2Target( new Vector3( targetPosOld.X, targetPosOld.Y, targetPosOld.Z), 0.0f, 0.0f );
		}
				
        obj3d[0].model.Draw( graphics, resContainer.shaderContainer, (camInfo.GetProjection()*camInfo.GetView().Inverse()), camInfo.GetViewEye() );
	}

	/// 描画
	/**
	 */
	public override void RenderAlpha()
	{
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
			
	/// 距離を算出
	/**
	 * @param [in] pos1 : 開始点
	 * @param [in] pos2 : 終了点
	 * @return float : 開始点から終了点までの距離
	 */
	public float Distance( Vector3 pos1, Vector3 pos2 )
	{
		Vector3 vec3 = pos1 - pos2;
		float dis = FMath.Sqrt( vec3.Dot(vec3) );
		return dis;
	}
			

	/// ３頂点からラジアンを返す
	/**
	 * @param [in] posBase : 頂点A
	 * @param [in] pos1 : 頂点Aと隣り合う頂点1
	 * @param [in] pos2 : 頂点Aと隣り合う頂点2
	 * @return float : ラジアン角
	 */
	private float getRadian( Vector3 posBase, Vector3 pos1, Vector3 pos2 )
	{
		Vector3 calA = pos1 - posBase;
		Vector3 calB = pos2 - posBase;

		float lba	= calA.Length();
		float lca	= calB.Length();
		float radian= FMath.Acos( calA.Dot(calB) / (lba*lca) );

		return radian;
	}

	/// 砲台の向き設定
	/**
	 * @param [in] dir : 対象の向き
	 * @param [in] rotX : X軸回転
	 * @param [in] rotY : Y軸回転
	 */
	public void setCannonDir2Target( Vector3 trgPos, float rotX, float rotY )
	{
		Matrix4 testMat;	
		Matrix4 baseMat;
		Matrix4 baseRotMat;
		Posture trgPosture = new Posture();

		/// 向きを制御する根元のボーン
		baseMat = obj3d[0].model.Bones[GetBoneId( 0, "A01_y_axis" )].WorldMatrix;

		/// Lgun_b_rot_Lgun_b
		/// 根元に紐付いているボーン
		testMat = obj3d[0].model.Bones[GetBoneId( 0, "y_axis_x_axis" )].WorldMatrix;
				
		/// ボーンを制御する方向情報
		trgPosture.SetPosture( baseMat );
		trgPosture.LookAt( trgPos.X, trgPos.Y, trgPos.Z );
		trgPosture.SetPosition( 0.0f, 0.0f, 0.0f );

		/// 根元のボーンの逆行列を掛け、根元のボーンを原点とした位置姿勢を取得
		testMat = baseMat.Inverse() * testMat;

		/// ボーンを制御する方向情報
		baseRotMat = trgPosture.GetPosture() * Matrix4.RotationX( (float)(rotX * (Math.PI/180.0f)) );

		/// ボーンに制御方向情報を掛ける
		testMat = baseRotMat * testMat;

		/// ワールド座標にボーン位置を戻す
		testMat.M41 = testMat.M41 + baseMat.M41;
		testMat.M42 = testMat.M42 + baseMat.M42;
		testMat.M43 = testMat.M43 + baseMat.M43;

		/// 方向を変更したボーンをモデルのMatrixに設定
		obj3d[0].model.Bones[GetBoneId( 0, "y_axis_x_axis" )].WorldMatrix = testMat;


		/// x_axis_Lgun
		testMat = obj3d[0].model.Bones[GetBoneId( 0, "x_axis_Lgun" )].WorldMatrix;
		testMat = baseMat.Inverse() * testMat;
		testMat = baseRotMat * testMat;
		testMat.M41 = testMat.M41 + baseMat.M41;
		testMat.M42 = testMat.M42 + baseMat.M42;
		testMat.M43 = testMat.M43 + baseMat.M43;
		obj3d[0].model.Bones[GetBoneId( 0, "x_axis_Lgun" )].WorldMatrix = testMat;


		/// Lgun_muzzle
		testMat = obj3d[0].model.Bones[GetBoneId( 0, "Lgun_muzzle" )].WorldMatrix;
		testMat = baseMat.Inverse() * testMat;
		testMat = baseRotMat * testMat;
		testMat.M41 = testMat.M41 + baseMat.M41;
		testMat.M42 = testMat.M42 + baseMat.M42;
		testMat.M43 = testMat.M43 + baseMat.M43;
		obj3d[0].model.Bones[GetBoneId( 0, "Lgun_muzzle" )].WorldMatrix = testMat;

		/// Lgun_Lgun_MA
		if( modelLevel == Unit.MODEL_LEVEL_HIGH || modelLevel == Unit.MODEL_LEVEL_NORMAL ){
			testMat = obj3d[0].model.Bones[GetBoneId( 0, "Lgun_Lgun_MA" )].WorldMatrix;
			testMat = baseMat.Inverse() * testMat;
			testMat = baseRotMat * testMat;
			testMat.M41 = testMat.M41 + baseMat.M41;
			testMat.M42 = testMat.M42 + baseMat.M42;
			testMat.M43 = testMat.M43 + baseMat.M43;
			obj3d[0].model.Bones[GetBoneId( 0, "Lgun_Lgun_MA" )].WorldMatrix = testMat;
		}

		/// A01_y_axis
		trgPosture.SetPosture( baseMat );
		trgPosture.LookAt( trgPos.X, trgPosture.GetPosition().Y, trgPos.Z );
		baseMat = trgPosture.GetPosture() * Matrix4.RotationY( (float)(rotY * (Math.PI/180.0f)) );
		obj3d[0].model.Bones[GetBoneId( 0, "A01_y_axis" )].WorldMatrix = baseMat;
	}

	bool checkAngleX( Vector3 vec, out Vector3 reVec )
	{
		Matrix4 baseMat;
		Posture tmpPosture = new Posture();
		Vector3 pos2UpVec;
		Vector3 pos2DownVec;
		Vector3 result;
		Vector3 normal;

		/// 向きを制御する根元のボーン
		baseMat = obj3d[0].model.Bones[GetBoneId( 0, "A01_y_axis" )].WorldMatrix;
		tmpPosture.SetPosture( baseMat );
		if( vec.Z > 0.0f ){
			tmpPosture.AddPosition( 0.0f, 1.0f, 1.0f );
				
			pos2UpVec = new Vector3( 0.0f,
						tmpPosture.GetPosition().Y-baseMat.M42,
						tmpPosture.GetPosition().Z-baseMat.M43 );
				
			tmpPosture.AddPosition( 0.0f, -2.0f, 1.0f );
			pos2DownVec = new Vector3( 0.0f,
						tmpPosture.GetPosition().Y-baseMat.M42,
						tmpPosture.GetPosition().Z-baseMat.M43 );
		}else{
			tmpPosture.AddPosition( 0.0f, 1.0f, -1.0f );
				
			pos2UpVec = new Vector3( 0.0f,
						tmpPosture.GetPosition().Y-baseMat.M42,
						tmpPosture.GetPosition().Z-baseMat.M43 );
				
			tmpPosture.AddPosition( 0.0f, -2.0f, -1.0f );
			pos2DownVec = new Vector3( 0.0f,
						tmpPosture.GetPosition().Y-baseMat.M42,
						tmpPosture.GetPosition().Z-baseMat.M43 );
		}
				
		normal = Vector3.Cross( pos2UpVec, pos2DownVec );
		normal = Vector3.Normalize( normal );
				
		result = Vector3.Cross( pos2UpVec, vec );
		result = Vector3.Normalize( result );
				
				
		if( result.X != normal.X ){
			reVec = pos2UpVec;
			return false;
		}
				
		result = Vector3.Cross( vec, pos2DownVec );
		result = Vector3.Normalize( result );

		if( result.X != normal.X ){
			reVec = pos2DownVec;
			return false;
		}
				
		reVec = vec;
		return true;
	}
			
	public void setMuzzlePos()
	{
		Matrix4 testMat;	

		/// Lgun_muzzle
		testMat = obj3d[0].model.Bones[GetBoneId( 0, "Lgun_muzzle" )].WorldMatrix;

		muzzlePos.X = testMat.M41;
		muzzlePos.Y = testMat.M42;
		muzzlePos.Z = testMat.M43;
	}
			
	public bool CheckAngle2Target()
	{
		float angleUpLimit =40.0f;
		float angleDownLimit = 23.0f;
		
		Posture trgPosture = new Posture();
		trgPosture.SetPosture( GetPosture() );
		Posture basePosture = new Posture();
		basePosture.SetPosture( obj3d[0].model.Bones[GetBoneId( 0, "A01_base" )].WorldMatrix );
		
				
		trgPosture.AddPosition( 0.0f, -4.0f, 10.0f );			
		targetPos.X = trgPosture.GetPosition().X;
		targetPos.Y = trgPosture.GetPosition().Y;
		targetPos.Z = trgPosture.GetPosition().Z;
				
		muzzlePosture.SetPosture( obj3d[0].model.Bones[GetBoneId( 0, "y_axis_x_axis" )].WorldMatrix );
		Vector3 tmpMuzzlePos = muzzlePosture.GetPosition();
		muzzlePosture.SetPosture( basePosture.GetPosture() );
		muzzlePosture.SetPosition( tmpMuzzlePos.X, tmpMuzzlePos.Y, tmpMuzzlePos.Z );
		
				
		float b = 6.0f;
		float angle = 0.0f;
		float radian = 0.0f;
		float tanR = 0.0f;
		float c = tanR * b;
				
		angle = angleUpLimit;
		radian =  angle * ((float)Math.PI/180.0f);
		tanR = (float)Math.Tan( (double)radian );
		c = tanR * b;
				
		upPosture.SetPosture( muzzlePosture.GetPosture() );
		upPosture.AddPosition( 0.0f, c, b );

				
		angle = angleDownLimit;
		radian =  angle * ((float)Math.PI/180.0f);
		tanR = (float)Math.Tan( (double)radian );
		c = tanR * b;
		
		downPosture.SetPosture( muzzlePosture.GetPosture() );
		downPosture.AddPosition( 0.0f, -c, b );
		
		Matrix4 postureMat = muzzlePosture.GetPosture();
		Matrix4 upMat = upPosture.GetPosture();
		Matrix4 downMat = downPosture.GetPosture();
		Matrix4 postureInverse = Matrix4.Inverse( muzzlePosture.GetPosture() );
				
		Matrix4 targetMat = muzzlePosture.GetPosture();
		targetMat.M41 = targetPos.X;
		targetMat.M42 = targetPos.Y;
		targetMat.M43 = targetPos.Z;
				
		upMat = postureInverse * upMat;
		downMat = postureInverse * downMat;
		targetMat = postureInverse * targetMat;
		postureMat = postureMat * postureInverse;
				
		Vector3 vec1 = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 vec2 = new Vector3( 0.0f, 0.0f, 0.0f );
				
		vec1.X = 0.0f;
		vec1.Y = upMat.M42 - postureMat.M42;
		vec1.Z = upMat.M43 - postureMat.M43;

		vec2.X = 0.0f;
		vec2.Y = targetMat.M42 - postureMat.M42;
		vec2.Z = targetMat.M43 - postureMat.M43;
				
		/// 外積
		Vector3 crossRes = comUtil.Cross2( vec2, vec1 );
			
		/// 右側の画面外に出ているか判定
		if( crossRes.X > 0.0f ){
			return false;
		}
				
		vec1.X = 0.0f;
		vec1.Y = downMat.M42 - postureMat.M42;
		vec1.Z = downMat.M43 - postureMat.M43;

		vec2.X = 0.0f;
		vec2.Y = targetMat.M42 - postureMat.M42;
		vec2.Z = targetMat.M43 - postureMat.M43;
				
		/// 外積
		crossRes = comUtil.Cross2( vec1, vec2 );
			
		/// 右側の画面外に出ているか判定
		if( crossRes.X > 0.0f ){
			return false;
		}
				
				
		return true;
	}
			
}

} // end ns DefenseDemo
