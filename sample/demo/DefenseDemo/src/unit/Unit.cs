/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace DefenseDemo {

/// ユニットの抽象クラス
/**
 * @version 0.1, 2011/06/23
 */
public abstract class Unit
	: Posture
{
	/// ユニットタイプ(防衛)
	public static int TYPE_DEF = 0;
	/// ユニットタイプ(攻撃)
	public static int TYPE_ACT = 1;
	/// ユニットタイプ(弾)
	public static int TYPE_BUL = 2;
		
	/// 防衛ユニット種別(レーザー)
	public static int KIND_DEF_LASER = 0;
	/// 防衛ユニット種別(ワイドレーザー)
	public static int KIND_DEF_WIDE_LASER = 1;
	/// 防衛ユニット種別(榴弾)
	public static int KIND_DEF_HIGH_EXPLOSIVE = 2;
	/// 防衛ユニット種別(ミサイル)
	public static int KIND_DEF_MISSILE = 3;

	/// 攻撃ユニット種別(トラック)
	public static int KIND_ACT_TRACK = 0;
	/// 攻撃ユニット種別(装甲車)
	public static int KIND_ACT_ARMORED_CAR = 1;
	/// 攻撃ユニット種別(戦車)
	public static int KIND_ACT_BATTLE_TANK = 2;
	/// 攻撃ユニット種別(浮遊型ポッド)
	public static int KIND_ACT_FLOATING_POT = 3;
	/// 攻撃ユニット種別(多脚戦車(※ボス))
	public static int KIND_ACT_MULTI_LEGGED_TANK = 4;

	/// 防衛ユニットパラメータ
	/// 攻撃力
	public static int DEF_PARAM_ACT_POWER = 0;
	/// 攻撃間隔
	/// ※1秒間に撃つ弾数
	/// ※0を指定すると着弾後、次を発射する
	public static int DEF_PARAM_ACT_INTERVAL = 1;
	/// 射程
	public static int DEF_PARAM_ACT_RANGE = 2;
	/// コスト
	public static int DEF_PARAM_COST_POINT = 3;
	/// 弾の速度
	/// 弾の移動速度のパーセンテージ(通常が100%)
	/// ※現状レーザーのみ対応
	public static int DEF_PARAM_BUL_SPEED = 4;
	/// 砲身X軸回転角度上側限界値
	/// ※0～89度の間で設定
	public static int DEF_PARAM_UP_ANGLE = 5;
	/// 砲身X軸回転角度下側限界値
	/// ※0～89度の間で設定
	public static int DEF_PARAM_DOWN_ANGLE = 6;
	/// 防衛ユニットパラメータ数
	public static int DEF_PARAM_NUM = 7;
	/// ユニット種別毎の各パラメータ値
	public static int[] DEF_PARAM_DATA = {
			
			 11,  0,  40,  50, 100, 89, 23,
			  8,  0,  30, 100,   0, 89, 20,
			 39,  0, 100, 200,   0, 60, 70,
			  7,  0, 120, 450,   0, 60, 40
			 
	};

	/// 攻撃ユニットパラメータ

	/// 移動スピード(1秒間(30フレーム)で進むグリッド数)
	public static int ACT_PARAM_MOVE_SPEED = 0;
	/// 耐久力
	public static int ACT_PARAM_HP = 1;
	/// 坂道の速度低下率(パーセントで指定(100%だと通常移動))
	public static int ACT_PARAM_CLIME = 2;
	/// 破壊時の獲得ポイント
	public static int ACT_PARAM_GET_POINT = 3;
	/// グリッドサイズ
	public static int ACT_PARAM_SIZE = 4;
	/// 攻撃ユニットパラメータ数
	public static int ACT_PARAM_NUM = 5;
	/// ユニット種別毎の各パラメータ値
	public static int[] ACT_PARAM_DATA = {
			
			 10,  40, 85,  9,  80,
			  8, 110, 75, 12,  90,
			  6, 180, 65, 20, 100,
			 18,  30,100, 12, 100,
			  3,3000, 50,  0, 600
			
	};
		
	public static int INIT_MAIN_POINT = 150;
		
	public static float DEF_UNIT_TURN_VALUE = 10.0f;
		
	/// 敵ユニット出現情報
	public static int[][] ENEMY_START_DATA = {
		/// 出現時間(秒),ユニット種別,ユニット数
		/// ※ユニット数は1を指定すると1体(左右真中はランダム)登場する
		/// ボスは真中位置に強制的に設定する
		/// 2を設定すると左右1体ずつ登場する
		/// test

			
		/// wave1
		new[]{ 0, 0, 1,
			   2, 0, 1,

			  10, 0, 2,

		  	  20, 1, 1,
			  24, 1, 1,

			  34, 1, 1,
			  38, 1, 1,

			  48, 0, 1,
			  52, 0, 1,
			  56, 0, 2,
				
			  66, 1, 2,
			  68, 0, 2},

		/// wave2
		new[]{ 0, 0, 1,
			   2, 0, 1,
			   4, 0, 1,

			  16, 0, 2,
			  18, 0, 2,

			  34, 1, 1,
			  36, 1, 1,
			  38, 1, 1,

			  50, 0, 1,
			  54, 0, 2,
			  58, 0, 1,

			  68, 2, 1,
			  70, 2, 1,

			  80, 0, 2,
			  82, 0, 2},

		/// wave3
		new[]{ 0, 1, 1,
			   2, 1, 1,
			   4, 0, 2,
				
			  12, 3, 1,
			  14, 0, 2,
				
			  22, 2, 1,
			  24, 2, 1,
				
			  34, 0, 1,
			  38, 2, 1,
			  42, 3, 2,
				
			  64, 2, 1,
			  66, 0, 1,
			  68, 0, 1,
				
			  84, 1, 1,
			  86, 1, 1,
			  88, 2, 1,
				
			  96, 3, 2},

		/// wave4
		new[]{ 0, 2, 2,
			   4, 3, 1,
			   5, 3, 1,
			   6, 3, 1,
			   8, 0, 1,
			  10, 0, 1,
			  12, 0, 2,
			  16, 0, 2,
				
			  26, 2, 2,
			  30, 2, 2,
				
			  44, 1, 2,
			  47, 1, 2,
			  50, 1, 2,
				
			  60, 3, 1,
			  61, 3, 1,
			  62, 3, 1,
			  64, 2, 1,
			  66, 2, 1,
			  68, 2, 1,
				
			  78, 3, 1,
			  80, 3, 1,
			  82, 3, 1,
			  83, 3, 1,
			  84, 3, 1},

		/// wave5
		new[]{ 0, 1, 1,
			   2, 1, 1,
			   4, 1, 1,
			   6, 1, 1,
			   8, 1, 1,
			  10, 0, 1,
			  12, 0, 1,
			  14, 0, 1,
			  16, 0, 1,
			  18, 0, 1,
				
			  30, 2, 1,
			  32, 2, 1,
			  34, 2, 1,
				
			  48, 2, 1,
			  50, 1, 1,
			  52, 0, 1,
			  54, 2, 1,
			  56, 1, 1,
				
			  70, 3, 1,
			  71, 3, 2,
			  72, 3, 1,
			  73, 3, 2,
			  74, 3, 1,
				
			  80, 0, 2,
			  82, 0, 1,
			  84, 0, 2,
			  86, 0, 1,
			  88, 0, 2,
			  90, 1, 1,
			  92, 1, 2,
			  94, 1, 1,
			  96, 1, 2,
				
			 100, 3, 1,
			 101, 3, 2,
			 102, 3, 1,
			 103, 3, 2,
			 104, 3, 1,
			 105, 3, 2,
				
			 120, 1, 2,
			 122, 1, 1,
			 124, 1, 2,
			 126, 1, 1,
			 128, 2, 2,
			 130, 2, 1,
			 132, 2, 2,
			 134, 2, 1,
				
			 140, 3, 2,
			 141, 3, 2,
			 142, 3, 2,
			 143, 3, 2},
					
		/// wave6
		new[]{ 10, 4, 1 }

	};		

	/// 3Dデータ
	public Object3D[] obj3d;
	/// プロジェクションマトリクス
	public Matrix4 proj;
	/// ビューマトリクス
	public Matrix4 view;
	/// 視点マトリクス
	public Matrix4 eye;
	/// 初期位置
	public Vector3 startPos;
	/// 当たり判定情報
	public CollisionCapsule colCapsule;
	/// 移動用当たり判定情報
	public CollisionSphere colMoveSphere;
	/// コスト
	public int cost = 0;
	/// 耐久力
	public int hp = 0;
	/// 攻撃力
	public int attack = 0;
	/// 射程
	public int length = 0;
	/// ユニットタイプ
	public int type = 0;
	/// ユニット種別
	public int kind = 0;
	/// 状態
	public int state = 0;
	/// 捕捉ターゲットのユニットリストインデックス値
	public int target = -1;
	public int targetOld = -1;
	/// 補足ターゲットの位置
	public Vector3 targetPos;
	public Vector3 targetPosOld;
	/// 攻撃中フラグ
	public bool unitAtkWait = false;
	/// 移動速度
	public int moveSpeed = 0;
	/// 当たり半径
	public float colRadius = 0.0f;
	/// 複数当たり可能フラグ
	public bool colMany = false;
	/// 弾移動中フラグ
	public bool bulletMove = false;
	/// 各ユニットで射出した弾の、ユニットリストインデックス値
	public int[] bulletUnitIndex;
	/// 自分のユニットリストインデックス値
	public int listIndex = -1;
	/// ユニットが居る床の傾き(角度0～360)
	public float landAngleX = 0.0f;
	/// 設定中のルートポイント		
	public int nowRoutePoint = -1;
	
	/// 道の初期位置(真中、左端、右端)
	public static int ROUTE_LINE_CENTER = 0;
	public static int ROUTE_LINE_LEFT = 1;
	public static int ROUTE_LINE_RIGHT = 2;

	/// 道の初期位置
	public int nowRouteLine = 0;

	/// 現状触れているルートポイントIndex
	public int hitRoutePoint = -1;
		
	public float[] animFrame;
	public int[] animIndex;
	public long[] animStartTime;

	/// ルートパターン
	public static int ROUTE_PATTERN_A = 0;
	public static int ROUTE_PATTERN_B = 1;
	public static int ROUTE_PATTERN_C = 2;
	public static int ROUTE_PATTERN_D = 3;
	public static int ROUTE_PATTERN_E = 4;
	/// 現状設定中のルートパターン値
	public int routePattern = 0;

	/// ユニットの登場アクション済フラグ
	public bool enterActFlg = true;

	/// 弾発射位置
	public Vector3 muzzlePos = new Vector3( 0.0f, 0.0f, 0.0f );
		
	public bool hitBulletToGround = false;
		
	public bool deadEffectStart = false;
	public bool deadEffectEnd = false;
	public bool hitEnemy = false;
		
	public static int MODEL_LEVEL_HIGH = 0;
	public static int MODEL_LEVEL_NORMAL = 1;
	public static int MODEL_LEVEL_LOW = 2;
	public int modelLevel = 0;
		
	public int routNum = 0;
		
	public bool changeModelLevel0 = false;
	public bool changeModelLevel2 = false;
		
	public int wave = 0;
	public bool waveLast = false;
		
	public int missileCnt = 0;
	public int missileWait = 0;
		
//	public static float lodLength = 150.0f;
	public static float lodLength = 200.0f;
		
	public bool rot2TrgStart = false;

	/// 初期化
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public abstract bool Init();

	/// 解放
	/**
	 * @return 正常終了:true 異常終了:false
	 */
	public abstract bool Term();

	/// 更新
	/**
	 */
	public abstract void Update();

	/// 描画
	/**
	 */
	public abstract void Render();

	/// 描画(半透明)
	/**
	 */
	public abstract void RenderAlpha();
}

} // end ns DefenseDemo
