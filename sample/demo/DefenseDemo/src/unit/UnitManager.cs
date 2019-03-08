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

/// ユニット管理クラス
/**
 * @version 0.1, 2011/06/23
 */
class UnitManager {

	/// ユニットリスト
//	private List< Unit > unitList;
	private Unit[] unitList;
	/// 当たり判定
	private CollisionCheck colCheck;
	/// 当たり判定処理用三角形クラス
	public CollisionTriangle hitTriangle = new CollisionTriangle();
	/// リストへのユニット登録数
	private int unitListNum = 0;
	private int addPoints = 0;
	private CommonUtil comUtil;
	private Rout rout;
	private Vector3 checkDef2TrgTmp = new Vector3( 0.0f, 0.0f, 0.0f );
	public bool enemyArrive = false;
	public bool changeUnitModelLow = false;
	public bool changeUnitModelHigh = false;
	public bool nowAllEnemyDead = false;
	private bool[] playEneMoveSeCheck;
	private bool[] playEneMoveSe;
		
	private Posture calcAnglePosture = new Posture();
	private Vector3 calcVecPosZ = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 calcVecPosY = new Vector3( 0.0f, 0.0f, 0.0f );

	private Posture upPosture = new Posture();
	private Posture downPosture = new Posture();
	private Posture muzzlePosture = new Posture();
	private Posture basePosture = new Posture();
		
	/// コンストラクタ
	/**
	 */
	public UnitManager()
	{
	}
		
	/// デストラクタ
	/**
	 */
	~UnitManager()
	{
	}

	/// 初期化
	/**
	 * @return 正常終了:true、異常終了:false
	 */
	public bool Init()
	{
		int i;
			
		unitList = new Unit[255];
		unitListNum = 0;
		colCheck = new CollisionCheck();
		addPoints = 0;
		comUtil = CommonUtil.Inst();
		rout = new Rout();
		rout.Init();
			
		enemyArrive = false;
		changeUnitModelLow = false;
		changeUnitModelHigh = false;
		nowAllEnemyDead = false;
			
		playEneMoveSe = new bool[5];
		playEneMoveSeCheck = new bool[5];
		i = 0;
		while( i<playEneMoveSe.Length ){
			playEneMoveSe[i] = false;
			playEneMoveSeCheck[i] = false;
			i++;
		}
			
		return true;
	}

	/// 解放
	/**
	 * @return 正常終了:true、異常終了:false
	 */
	public void Term()
	{
		rout.Term();
		rout = null;
			
		int i = 0;
		while( i<GetListCount() )
		{
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			GetUnit(i).Term();
			unitList[i] = null;
			i++;
		}
		unitList = null;
			
		comUtil = null;
			
		colCheck = null;
	}
		
	public int GetListCount()
	{
		return unitList.Length;
//		return unitListNum;
	}

	/// ユニットの取得
	public Unit GetUnit( int index )
	{
		if( index < 0 ){
			return null;
		}
		return unitList[index];
	}

	/// ユニットの登録
	/**
	 */
	public int AddUnit( Unit unit )
	{
		int i = 0;
		int index = 0;

		while( i < GetListCount() ){
			if( unitList[i] == null ){
				unitList[i] = unit;
				index = i;
				unitListNum++;
				break;
			}
			i++;
		}

		return index;
	}

	/// ユニットの削除
	/**
	 */
	public void DelUnit( int index )
	{
		if( GetUnit(index) == null ){
			return;
		}
		GetUnit(index).Term();
		unitList[index] = null;
		unitListNum--;
	}

	/// ユニットの登録情報解放
	/**
	 */
	public void ClearAllUnit()
	{
	}

	/// 登録ユニットの初期化処理
	/**
	 */
	public void UnitInit()
	{
		int i = 0;
		while( i<GetListCount() )
		{
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			GetUnit(i).Init();
			i++;
		}
	}
	
	/// 登録ユニットの更新処理
	/**
	 */
	public void UnitUpdate()
	{
		int i = 0;
		int n = 0;
		int m = 0;

		if( changeUnitModelHigh || changeUnitModelLow ){
			while( i<GetListCount() ){

				if( GetUnit(i) == null ){
					i++;
					continue;
				}
					
				if( changeUnitModelLow ){
					GetUnit(i).changeModelLevel2 = true;
				}
				else if( changeUnitModelHigh ){
					GetUnit(i).changeModelLevel0 = true;
				}
						
				i++;
			}
			changeUnitModelHigh = false;
			changeUnitModelLow = false;
		}
			
			
		/// 各ユニットの更新処理
		while( i<GetListCount() ){

			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			GetUnit(i).Update();
			i++;
		}

//		comUtil.SetLog( "====[log][UnitManager.cs]search enemy start" );			
		/// 各防衛ユニットの索敵判定
		i = 0;
		int targetIndex = -1;
		while( i<GetListCount() ){

			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			/// 防衛ユニットのみ索敵を行う
			if( GetUnit(i).type == Unit.TYPE_DEF ){
				if( GetUnit(i).kind == Unit.KIND_DEF_MISSILE ){
					if( GetUnit(i).missileCnt > 0 ){
						i++;
						continue;
					}
				}
				if( GetUnit(i).target < 0 ){
					targetIndex = -1;
					/// 索敵に引っ掛かった敵ユニットのインデックスを取得
					targetIndex = SearchEnemy( i );
				
					/// 敵ユニットを見つけたか確認
					if( targetIndex >= 0 ){
							
						if( CheckDefCanAttackTarget( i, targetIndex ) ){
							/// 攻撃対象としてインデックス値を保持する
							GetUnit(i).target = targetIndex;
								
							GetUnit(i).rot2TrgStart = true;
								
							/// ターゲット位置を更新
							if( GetUnit(i).target >= 0 ){
								GetUnit(i).targetPos.X = GetUnit(GetUnit(i).target).GetPosition().X;
								GetUnit(i).targetPos.Y = GetUnit(GetUnit(i).target).GetPosition().Y;
								GetUnit(i).targetPos.Z = GetUnit(GetUnit(i).target).GetPosition().Z;
							}
						}
					}
				}
			}
			i++;
		}
			
//		comUtil.SetLog( "====[log][UnitManager.cs]search enemy end" );			
			
//		comUtil.SetLog( "====[log][UnitManager.cs]set dir defense to enemy start" );			
		/// 攻撃中の防衛ユニットは敵ユニット方向に向ける
		i = 0;
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).rot2TrgStart ){
				i++;
				continue;
			}
				
			/// 防衛ユニットのみ攻撃を行う
			if( GetUnit(i).type == Unit.TYPE_DEF ){
					
					/// ユニットの攻撃中フラグがONである事を確認
//					if( GetUnit(i).unitAtkWait ){
						
						/// ターゲット位置を更新
						if( GetUnit(i).target >= 0 ){
							GetUnit(i).targetPos.X = GetUnit(GetUnit(i).target).GetPosition().X;
							GetUnit(i).targetPos.Y = GetUnit(GetUnit(i).target).GetPosition().Y;
							GetUnit(i).targetPos.Z = GetUnit(GetUnit(i).target).GetPosition().Z;
						}
//					}
			}
			i++;
		}
//		comUtil.SetLog( "====[log][UnitManager.cs]set dir defense to enemy end" );			

//		comUtil.SetLog( "====[log][UnitManager.cs]attack defense to enemy start" );			
		/// 各防衛ユニットの攻撃処理
		i = 0;
		while( i<GetListCount() ){

			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			
			/// 防衛ユニットのみ攻撃を行う
			if( GetUnit(i).type == Unit.TYPE_DEF ){
				/// 攻撃対象のインデックス値を保持しているか確認
				if( GetUnit(i).target >= 0 && GetUnit(GetUnit(i).target).hp > 0 ){
					/// ユニットの攻撃中フラグがOFFである事を確認
					if( GetUnit(i).unitAtkWait == false ){
						/// ターゲットの角度が範囲外の場合、攻撃しない
						if( !CheckDefCanAttackTarget( i, GetUnit(i).target ) ){
							i++;
							continue;
						}
						/// ミサイルか確認
						if( GetUnit(i).kind == Unit.KIND_DEF_MISSILE ){
							/// ミサイルは4発ずつ攻撃する為、4発全部着弾したか確認
							n = 0;
							while( n < GetUnit(i).bulletUnitIndex.Length ){
								if( GetUnit(i).bulletUnitIndex[n] >= 0 ){
									break;
								}
								n++;
							}
							/// 着弾していない弾が有る場合、攻撃しない
							if( n < GetUnit(i).bulletUnitIndex.Length ){
								i++;
								continue;
							}
						}
							
							
						if( GetUnit(i).rot2TrgStart ){
							i++;
							continue;
						}
						
							
						/// 攻撃中フラグON
						GetUnit(i).unitAtkWait = true;

						/// 弾ユニットの初期化
						Unit unit;
						if( GetUnit(i).kind == Unit.KIND_DEF_LASER ){
							/// レーザー
							unit = new UnitBullet1();
							AudioManager.PlaySound( "SE_LASER" );
						}else if( GetUnit(i).kind == Unit.KIND_DEF_WIDE_LASER ){
							/// ワイドレーザー
							unit = new UnitBullet2();
						}else if( GetUnit(i).kind == Unit.KIND_DEF_HIGH_EXPLOSIVE ){
							/// 榴弾砲
							unit = new UnitBullet3();
							AudioManager.PlaySound( "SE_EXPLOSIVE" );
						}else if( GetUnit(i).kind == Unit.KIND_DEF_MISSILE ){
							/// ミサイル
							unit = new UnitBullet4();
							AudioManager.PlaySound( "SE_MISSILE" );
						}else{
							i++;
							continue;
//							unit = new UnitBullet1();
						}

						/// 弾の初期化
						unit.Init();

						/// 弾ユニットのターゲットに敵ユニットIndexを設定
						unit.target = GetUnit(i).target;
						/// ターゲットの位置を設定
						unit.targetPos = new Vector3(
								GetUnit(GetUnit(i).target).GetPosition().X,
								GetUnit(GetUnit(i).target).GetPosition().Y,
								GetUnit(GetUnit(i).target).GetPosition().Z );
							
						/// 弾ユニットの発射位置を設定
						unit.muzzlePos.X = GetUnit(i).muzzlePos.X;
						unit.muzzlePos.Y = GetUnit(i).muzzlePos.Y;
						unit.muzzlePos.Z = GetUnit(i).muzzlePos.Z;
						unit.SetPosition( GetUnit(i).muzzlePos.X,
										  GetUnit(i).muzzlePos.Y,
										  GetUnit(i).muzzlePos.Z );

						/// 弾がターゲットに当たるまでのフレーム数算出
						long hitTime;
						hitTime = GetHitFrameBullet2Enemy(
								unit.GetPosition(),
								GetUnit(unit.target).GetPosition(),
								GetUnit(i).kind );							
							
						/// 弾が当たるまでのフレーム数でターゲットがどこまで移動するか算出
						int hitRoutPoint = GetRoutPoint2Frame( unit.target, hitTime );



						/// 弾とターゲットの当たりを再確認

						/// 最初に出した弾が当たるまでのフレーム数を確保
						long hitTimeOld = hitTime;
						/// 新しいターゲット位置と弾が当たるまでのフレーム数を算出
						hitTime = GetHitFrameBullet2Enemy(
								unit.GetPosition(),
								rout.GetRoutPointPos(
									GetUnit(unit.target).routePattern, hitRoutPoint, 0.0f ),
								GetUnit(i).kind );
						/// 最初のフレーム数より大きい場合
						if( hitTime > hitTimeOld ){
							/// (差分/2)を最初のフレーム数から減算する
							hitTime = hitTimeOld - (hitTime-hitTimeOld)/2;
						/// 最初のフレーム数より小さい場合
						}else if( hitTime < hitTimeOld ){
							/// (差分/2)を新しいフレーム数に加算する
							hitTime = hitTime + (hitTimeOld-hitTime)/2;
						}
						/// ターゲット位置を再算出
						hitRoutPoint = GetRoutPoint2Frame( unit.target, hitTime );
									
							
						/// ライン位置を含めたターゲット位置を算出
						float xOffset = 0.0f;
						/// ライン位置が左側の場合
						if( GetUnit(unit.target).nowRouteLine == Unit.ROUTE_LINE_LEFT ){
							xOffset = -3.0f;
						/// ライン位置が右側の場合
						}else if( GetUnit(unit.target).nowRouteLine == Unit.ROUTE_LINE_RIGHT ){
							xOffset = 3.0f;
						}
						/// ターゲット位置算出
						unit.targetPos = rout.GetRoutPointPos( GetUnit(unit.target).routePattern, hitRoutPoint, xOffset );
							
							
						/// 弾ユニットの姿勢を敵方向へ向ける
						unit.LookAt( unit.targetPos.X,
									 unit.targetPos.Y,
									 unit.targetPos.Z );

						/// 弾ユニットの移動フラグをON
						unit.bulletMove = true;
						/// 弾ユニットをユニット管理クラスに追加
						int bulletIndex = AddUnit( unit );
							
//						GetUnit(i).bulletUnitIndex[0] = bulletIndex;

						/// 弾ユニットのユニットIndexを味方ユニットが所持
						n = 0;
						while( n < GetUnit(i).bulletUnitIndex.Length ){
							if( GetUnit(i).bulletUnitIndex[n] < 0 ){
								GetUnit(i).bulletUnitIndex[n] = bulletIndex;
								break;
							}
							n++;
						}
						
						
						/// ワイドレーザー用処理

						/// ワイドレーザーは範囲内のターゲット全てに攻撃を行う為、
						/// 他の攻撃とは別に処理を行う
						if( GetUnit(i).kind == Unit.KIND_DEF_WIDE_LASER ){
							n = 0;
							while( n < GetListCount() ){
								if( n == GetUnit(i).target ){
									n++;
									continue;
								}
								if( GetUnit(n) == null ){
									n++;
									continue;
								}
								if( GetUnit(n).type != Unit.TYPE_ACT ){
									n++;
									continue;
								}

								/// ワイドレーザーの範囲内にターゲットが存在するか確認
								if( comUtil.GetDistance( GetUnit(i).GetPosition(),
										GetUnit(n).GetPosition() ) < GetUnit(i).length ){
									/// 最初に見つけたターゲットの横＋－30度の範囲にいる敵をターゲットとする
									float rad = comUtil.getRadian(
											new Vector3( GetUnit(i).GetPosition().X, GetUnit(i).GetPosition().Y, GetUnit(i).GetPosition().Z ),
											new Vector3( GetUnit(GetUnit(i).target).GetPosition().X,GetUnit(i).GetPosition().Y, GetUnit(GetUnit(i).target).GetPosition().Z),
											new Vector3( GetUnit(n).GetPosition().X, GetUnit(i).GetPosition().Y, GetUnit(n).GetPosition().Z) );
									rad = (float)(rad*180.0f/Math.PI);
									if( rad <= 30 && rad > -30 ){
//										comUtil.SetLog( "[log][UnitManager.cs]====wide lazer hit:"+n );
											
										Unit unitBullet = new UnitBullet2();
										/// 弾の初期化
										unitBullet.Init();

										/// 弾ユニットのターゲットに敵ユニットIndexを設定
										unitBullet.target = n;
										/// ターゲットの位置を設定
										unitBullet.targetPos = new Vector3(
												GetUnit(unitBullet.target).GetPosition().X,
												GetUnit(unitBullet.target).GetPosition().Y,
												GetUnit(unitBullet.target).GetPosition().Z );
							
										/// 弾ユニットの発射位置を設定
										unitBullet.muzzlePos.X = GetUnit(i).muzzlePos.X;
										unitBullet.muzzlePos.Y = GetUnit(i).muzzlePos.Y;
										unitBullet.muzzlePos.Z = GetUnit(i).muzzlePos.Z;
										unitBullet.SetPosition( GetUnit(i).muzzlePos.X,
														  GetUnit(i).muzzlePos.Y,
														  GetUnit(i).muzzlePos.Z );

										/// 弾が当たるまでのフレーム数を算出
										hitTime = GetHitFrameBullet2Enemy(
												unit.GetPosition(),
												GetUnit(unit.target).GetPosition(),
												GetUnit(i).kind );
										/// 算出したフレーム数の、敵移動先を算出
										hitRoutPoint = GetRoutPoint2Frame( unit.target, hitTime );

										/// ※ワイドレーザーはターゲットに1フレーム後に当たり、
										/// ずれる事が無い為、補正処理は行わない

										/// ライン位置を含めたターゲット位置を算出
										xOffset = 0.0f;
										/// ライン位置が左側の場合
										if( GetUnit(unitBullet.target).nowRouteLine == Unit.ROUTE_LINE_LEFT ){
											xOffset = -3.0f;
										/// ライン位置が右側の場合
										}else if( GetUnit(unitBullet.target).nowRouteLine == Unit.ROUTE_LINE_RIGHT ){
											xOffset = 3.0f;
										}
										/// ターゲット位置を算出
										unitBullet.targetPos = rout.GetRoutPointPos( GetUnit(unitBullet.target).routePattern, hitRoutPoint, xOffset );
							
							
										/// 弾ユニットの姿勢を敵方向へ向ける
										unitBullet.LookAt( unitBullet.targetPos.X,
													 unitBullet.targetPos.Y,
													 unitBullet.targetPos.Z );

										/// 弾ユニットの移動フラグをON
										unitBullet.bulletMove = true;
										/// 弾ユニットをユニット管理クラスに追加
										bulletIndex = AddUnit( unitBullet );

										/// 弾ユニットのユニットIndexを味方ユニットが所持
										m = 0;
										while( m < GetUnit(i).bulletUnitIndex.Length ){
											if( GetUnit(i).bulletUnitIndex[m] < 0 ){
												GetUnit(i).bulletUnitIndex[m] = bulletIndex;
												break;
											}
											m++;
										}
											
									}
									
								}
								n++;
							}
						}/// ワイドレーザー用処理 終了
						

						/// ミサイル用処理
						/// ミサイルは1度の攻撃で4発ずつ弾を発射する為、
						/// 他の攻撃とは別に処理を行う
						if( GetUnit(i).kind == Unit.KIND_DEF_MISSILE ){
							/// ミサイル残弾数を設定
							/// ※最初に1発、発射している為、残弾3となる
							GetUnit(i).missileCnt = 3;
							/// 4発連続で発射する際の間隔(フレーム数)
							GetUnit(i).missileWait = 10;
						}

							
					}
				}
			}
			i++;
		}
//		comUtil.SetLog( "====[log][UnitManager.cs]attack defense to enemy end" );		
		/// ミサイルが1度の攻撃で、4発続けて発射する処理
		i = 0;
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type != Unit.TYPE_DEF ){
				i++;
				continue;
			}
			if( GetUnit(i).kind != Unit.KIND_DEF_MISSILE ){
				i++;
				continue;
			}

			/// ミサイルの残弾があるか確認
			if( GetUnit(i).missileCnt > 0 ){
				/// ミサイルとミサイルの間を空ける為のウェイト値が0か確認
				if( GetUnit(i).missileWait == 0 ){
					
					/// ミサイル発射処理
					SettingUnit2Bullet( i );

					/// 残弾数を減らす
					GetUnit(i).missileCnt--;

					/// 残弾数が0で無ければ、ウェイト値を設定
					if( GetUnit(i).missileCnt > 0 ){
						GetUnit(i).missileWait = 10;
					}
				}
				/// ウェイト値を減らす
				else{
					GetUnit(i).missileWait--;
				}
			}
			i++;
		}
			
		
		/// 攻撃と敵ユニットの当たり判定
//		comUtil.SetLog( "====[log][UnitManager.cs]check attack to enemy start" );			
		i = 0;
		Vector3 colCheckResult = new Vector3( 0.0f, 0.0f, 0.0f );
		int bulletUnitIndex = -1;
		while( i<GetListCount() ){
				
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			if( GetUnit(i).bulletUnitIndex != null ){
				n = 0;
				/// 同時に射出可能な弾数分繰り返す
				while( n < GetUnit(i).bulletUnitIndex.Length ){
					/// 弾のユニットインデックス値を取得
					bulletUnitIndex = GetUnit(i).bulletUnitIndex[n];
						
					/// ユニットリストに登録済みか確認
					if( bulletUnitIndex < 0 ){
						n++;
						continue;
					}
					
					if( GetUnit(bulletUnitIndex) == null ){
						n++;
						continue;
					}
						
					if( GetUnit(bulletUnitIndex).deadEffectStart ){
						n++;
						continue;
					}

						/// 弾を射出し、移動中か確認
						if( GetUnit(bulletUnitIndex).bulletMove ){
							
							if( GetUnit(bulletUnitIndex).target < 0 ||
								GetUnit(GetUnit(bulletUnitIndex).target) == null ){
								
								/// 弾の移動フラグをOFF
								GetUnit(bulletUnitIndex).bulletMove = false;
								/// 弾を消す
								GetUnit(bulletUnitIndex).deadEffectStart = true;
							}
							else{
							
							
								/// 弾と対象ユニットの当たり判定
								if( colCheck.CheckHitCapsuleToCapsule(
										GetUnit(bulletUnitIndex).colCapsule,
										GetUnit(GetUnit(bulletUnitIndex).target).colCapsule,
										ref colCheckResult ) ){

									/// 弾の移動フラグをOFF
									GetUnit(bulletUnitIndex).bulletMove = false;

									/// 弾を消す
									GetUnit(bulletUnitIndex).deadEffectStart = true;

									/// 敵ユニットのダメージ処理
									if( GetUnit( GetUnit(bulletUnitIndex).target ).hp > 0 ){
										GetUnit(bulletUnitIndex).hitEnemy = true;
										GetUnit( GetUnit(bulletUnitIndex).target ).hp -= GetUnit(i).attack;
										if( GetUnit( GetUnit(bulletUnitIndex).target ).hp <= 0 ){
											GetUnit( GetUnit(bulletUnitIndex).target ).hp = 0;
											GetUnit( GetUnit(bulletUnitIndex).target ).deadEffectStart = true;
										}
									}
								}
								else{
									
									float radian = comUtil.getRadian(
										GetUnit(bulletUnitIndex).targetPos,
										GetUnit(bulletUnitIndex).GetPosition(),
										GetUnit(bulletUnitIndex).muzzlePos );
									radian = (radian * 180.0f/(float)Math.PI );
									float length = comUtil.GetDistance(
										GetUnit(bulletUnitIndex).targetPos,
										GetUnit(bulletUnitIndex).GetPosition() );
									if( radian >= 90.0f ||
										length <= 1.0f ||
										(GetUnit(bulletUnitIndex).targetPos.X == GetUnit(bulletUnitIndex).GetPosition().X &&
										 GetUnit(bulletUnitIndex).targetPos.Y == GetUnit(bulletUnitIndex).GetPosition().Y &&
										 GetUnit(bulletUnitIndex).targetPos.Z == GetUnit(bulletUnitIndex).GetPosition().X )
									){
										/// 弾の移動フラグをOFF
										GetUnit(bulletUnitIndex).bulletMove = false;
										/// 弾を消す
										GetUnit(bulletUnitIndex).deadEffectStart = true;
										
										/// 敵ユニットのダメージ処理
										if( GetUnit( GetUnit(bulletUnitIndex).target ).hp > 0 ){
											GetUnit( GetUnit(bulletUnitIndex).target ).hp -= GetUnit(i).attack;
											GetUnit(bulletUnitIndex).hitEnemy = true;
											if( GetUnit( GetUnit(bulletUnitIndex).target ).hp <= 0 ){
												GetUnit( GetUnit(bulletUnitIndex).target ).hp = 0;
												GetUnit( GetUnit(bulletUnitIndex).target ).deadEffectStart = true;
											}
										}
									}
								
								}
								
							}
							
					}
					n++;
				}
			}
			i++;
		}		
//		comUtil.SetLog( "====[log][UnitManager.cs]check attack to enemy end" );			

		/// 敵ユニットの破壊判定
		/// ※敵ユニットを破壊し、破壊エフェクト中でもターゲットしたままなので、
		/// 他の敵ユニットの攻撃を可能とする為、破壊を判定しターゲットを外す
		i = 0;
		while( i<GetListCount() ){
				
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			if( GetUnit(i).type == Unit.TYPE_ACT ){
				if( GetUnit(i).hp <= 0 ){
					addPoints += GetUnit(i).cost;
					GetUnit(i).cost = 0;
					/// ユニットのIndexを全ユニットのターゲットから消す
					DelUnitTargetIndex( i );
				}
			}
				
			i++;
		}
			
			
			
//		comUtil.SetLog( "====[log][UnitManager.cs]check enemy dead start" );			

		/// 敵ユニットの消滅判定
		i = 0;
		while( i<GetListCount() ){
				
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			if( GetUnit(i).type == Unit.TYPE_ACT ){
				if( GetUnit(i).hp <= 0 && GetUnit(i).deadEffectEnd ){
//					addPoints += GetUnit(i).cost;
//					GetUnit(i).cost = 0;
//					if( !CheckDelUnitTarget2Bullet( i ) ){
						/// ユニット管理クラスから削除
						DelUnit(i);
//					}
				}
			}
				
			i++;
		}
//		comUtil.SetLog( "====[log][UnitManager.cs]check enemy dead end" );			
	
//		comUtil.SetLog( "====[log][UnitManager.cs]check bullet dead start" );		

		/// 弾の着弾エフェクト終了判定
		i = 0;
		while( i < GetListCount() ){

			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			
			if( GetUnit(i).type == Unit.TYPE_DEF ){
				n = 0;
				while( n < GetUnit(i).bulletUnitIndex.Length ){
					/// 弾のユニットインデックス値を取得
					bulletUnitIndex = GetUnit(i).bulletUnitIndex[n];
					
					if( bulletUnitIndex < 0 ){
						n++;
						continue;
					}
						
					if( GetUnit(bulletUnitIndex).deadEffectEnd ){
						DelUnit( bulletUnitIndex );
						GetUnit(i).bulletUnitIndex[n] = -1;
							
						if( Unit.DEF_PARAM_DATA[(GetUnit(i).kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_ACT_INTERVAL] == 0 ){
							
							GetUnit(i).unitAtkWait = false;
						}
					}
						
					n++;
				}
					
			}
			
			i++;
		}
//		comUtil.SetLog( "====[log][UnitManager.cs]check bullet dead end" );	

//		comUtil.SetLog( "====[log][UnitManager.cs]check search2 range start" );	

		/// ターゲット中の敵が、味方ユニットの攻撃範囲外か確認し、ターゲットを外す
		i = 0;
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type == Unit.TYPE_DEF ){
				if( GetUnit(i).target < 0 ){
					i++;
					continue;
				}
				
				checkDef2TrgTmp = (GetUnit(i).GetPosition() - GetUnit(GetUnit(i).target).GetPosition());
				float dis = FMath.Sqrt( checkDef2TrgTmp.Dot(checkDef2TrgTmp) );
				if( dis > GetUnit(i).length ){
					GetUnit(i).target = -1;
				}
				else if( !CheckDefCanAttackTarget( i, GetUnit(i).target ) ){
					GetUnit(i).target = -1;
				}
			}
				
			i++;
		}
//		comUtil.SetLog( "====[log][UnitManager.cs]check search2 range end" );	

		/// 敵ユニットの移動用サウンド再生処理
		/// 対象の敵ユニットが1体でも存在していたら、サウンドをループ再生する
		/// 対象の敵ユニットが1体も存在していない場合、サウンドを停止する
		i = 0;
		playEneMoveSeCheck[Unit.KIND_ACT_TRACK] = false;
		playEneMoveSeCheck[Unit.KIND_ACT_ARMORED_CAR] = false;
		playEneMoveSeCheck[Unit.KIND_ACT_BATTLE_TANK] = false;
		playEneMoveSeCheck[Unit.KIND_ACT_FLOATING_POT] = false;
		playEneMoveSeCheck[Unit.KIND_ACT_MULTI_LEGGED_TANK] = false;
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type != Unit.TYPE_ACT ){
				i++;
				continue;
			}
			if( GetUnit(i).hp <= 0 ){
				i++;
				continue;
			}
				
			playEneMoveSeCheck[GetUnit(i).kind] = true;
				
			i++;
		}
		/// トラック用
		if( playEneMoveSeCheck[Unit.KIND_ACT_TRACK] ){
			if( !playEneMoveSe[Unit.KIND_ACT_TRACK] ){
				AudioManager.PlaySound( "SE_TRACK", true );
				playEneMoveSe[Unit.KIND_ACT_TRACK] = true;
			}
		}
		else{
			if( playEneMoveSe[Unit.KIND_ACT_TRACK] ){
				AudioManager.StopSound( "SE_TRACK" );
				playEneMoveSe[Unit.KIND_ACT_TRACK] = false;
			}
		}
		/// 装甲車用
		if( playEneMoveSeCheck[Unit.KIND_ACT_ARMORED_CAR] ){
			if( !playEneMoveSe[Unit.KIND_ACT_ARMORED_CAR] ){
				AudioManager.PlaySound( "SE_CAR", true );
				playEneMoveSe[Unit.KIND_ACT_ARMORED_CAR] = true;
			}
		}
		else{
			if( playEneMoveSe[Unit.KIND_ACT_ARMORED_CAR] ){
				AudioManager.StopSound( "SE_CAR" );
				playEneMoveSe[Unit.KIND_ACT_ARMORED_CAR] = false;
			}
		}
		/// 戦車用
		if( playEneMoveSeCheck[Unit.KIND_ACT_BATTLE_TANK] ){
			if( !playEneMoveSe[Unit.KIND_ACT_BATTLE_TANK] ){
				AudioManager.PlaySound( "SE_TANK", true );
				playEneMoveSe[Unit.KIND_ACT_BATTLE_TANK] = true;
			}
		}
		else{
			if( playEneMoveSe[Unit.KIND_ACT_BATTLE_TANK] ){
				AudioManager.StopSound( "SE_TANK" );
				playEneMoveSe[Unit.KIND_ACT_BATTLE_TANK] = false;
			}
		}
		/// フローティングポット用
		if( playEneMoveSeCheck[Unit.KIND_ACT_FLOATING_POT] ){
			if( !playEneMoveSe[Unit.KIND_ACT_FLOATING_POT] ){
				AudioManager.PlaySound( "SE_FLOATING", true );
				playEneMoveSe[Unit.KIND_ACT_FLOATING_POT] = true;
			}
		}
		else{
			if( playEneMoveSe[Unit.KIND_ACT_FLOATING_POT] ){
				AudioManager.StopSound( "SE_FLOATING" );
				playEneMoveSe[Unit.KIND_ACT_FLOATING_POT] = false;
			}
		}
		/// ボス用
		if( playEneMoveSeCheck[Unit.KIND_ACT_MULTI_LEGGED_TANK] ){
			if( !playEneMoveSe[Unit.KIND_ACT_MULTI_LEGGED_TANK] ){
				AudioManager.PlaySound( "SE_BOSS", true );
				playEneMoveSe[Unit.KIND_ACT_MULTI_LEGGED_TANK] = true;
			}
		}
		else{
			if( playEneMoveSe[Unit.KIND_ACT_MULTI_LEGGED_TANK] ){
				AudioManager.StopSound( "SE_BOSS" );
				playEneMoveSe[Unit.KIND_ACT_MULTI_LEGGED_TANK] = false;
			}
		}
			

		/// 敵ユニットが最終防衛ラインに到達したか確認
		i = 0;
		while( i < GetListCount() ){
			if( enemyArrive ){
				break;
			}
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type == Unit.TYPE_ACT ){
					
				/// 到達していた場合、到達フラグをON
				if( GetUnit(i).routNum >= (rout.GetRoutPointMaxNum(GetUnit(i).routePattern)-1) ){
					enemyArrive = true;
//					GetUnit(i).hp = 0;
//					GetUnit(i).deadEffectStart = true;
				}	
								 
			}
			i++;
		}
			
		/// 出現した敵が全て倒されたか確認
		i = 0;
		nowAllEnemyDead = true;
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type == Unit.TYPE_ACT ){
				nowAllEnemyDead = false;
				break;
			}
			i++;
		}
		
			
	}

	/// 登録ユニットの描画処理
	/**
	 */
	public void UnitRender()
	{
		int i = 0;
		CameraInfo camInfo = CameraInfo.Inst();
			
//		comUtil.SetLog( "[log][UnitManager.cs]====start" );
		while( i<GetListCount() ){

			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			if( !CheckUnitCulling(i) ){
				i++;
				continue;
			}
				

//			Console.WriteLine( "[log][UnitManager.cs]====["+i+"]:actionIndex:"+GetUnit(i).animIndex[0]+"/actionFrame:"+GetUnit(i).animFrame[0] );
			GetUnit(i).proj = camInfo.GetProjection();
			GetUnit(i).view = camInfo.GetView();
			GetUnit(i).Render();

			i++;
		}
//		comUtil.SetLog( "[log][UnitManager.cs]====end" );
	}
		
	public void UnitRenderAlpha()
	{
		int i = 0;
		CameraInfo camInfo = CameraInfo.Inst();
			
		while( i<GetListCount() ){

			if( GetUnit(i) == null ){
				i++;
				continue;
			}
				
			if( !CheckUnitCulling(i) ){
				i++;
				continue;
			}
				

			GetUnit(i).proj = camInfo.GetProjection();
			GetUnit(i).view = camInfo.GetView();
			GetUnit(i).RenderAlpha();

			i++;
		}
	}

	/// 登録ユニットと地面の当たり判定
	/**
	 * 当たっている場合、地面に面した位置まで移動する
	 * @warning 「CheckColUnit2Unit()」の後に実行する。
	 * @param [in] ground : 地面データクラス
	 */
	public void CheckColUnit2Ground( Ground ground )
	{
		int triCnt = 0;
		bool result = false;
		float distance = -1.0f;
		int i = 0;
		float trgDistance = -1.0f;
		int trgTriNum = -1;
		Vector3 trgTriResultPos = new Vector3( 0.0f, 0.0f, 0.0f );
		float nomAngle = 0.0f;

		while( i<GetListCount() ){
				
			result = false;
			trgDistance = -1.0f;
			trgTriNum = -1;


			if( GetUnit(i) == null ){
				i++;
				continue;
			}

			/// 敵ユニットで判定を行う
//			if( GetUnit(i).type != Unit.TYPE_ACT && GetUnit(i).type != Unit.TYPE_BUL ){
			if( GetUnit(i).type != Unit.TYPE_ACT ){
				i++;
				continue;
			}
				
			CollisionCapsule colCap = new CollisionCapsule();
			Vector3 resultPos = new Vector3( 0.0f, 0.0f, 0.0f );

			/// 当たり判定用カプセル情報の作成
			/// 始点に移動前位置、終点に移動後位置を設定
			colCap.CreateCapsule(
					GetUnit(i).GetPosition(),
					new Vector3( GetUnit(i).colMoveSphere.position.X,
								 GetUnit(i).colMoveSphere.position.Y,
								 GetUnit(i).colMoveSphere.position.Z ),
					GetUnit(i).colMoveSphere.r );

			/// 地面との当たり判定
			/// 傾斜が30度より少ない地面と当たりを判定
			triCnt = 0;
			while( triCnt<ground.collisionGround[0].triangle.Count ){
					
				if( ground.collisionGround[0].triangle[triCnt].vertex[0].Y > GetUnit(i).colMoveSphere.position.Y + (GetUnit(i).colMoveSphere.r*2) &&
					ground.collisionGround[0].triangle[triCnt].vertex[1].Y > GetUnit(i).colMoveSphere.position.Y + (GetUnit(i).colMoveSphere.r*2) &&
					ground.collisionGround[0].triangle[triCnt].vertex[2].Y > GetUnit(i).colMoveSphere.position.Y + (GetUnit(i).colMoveSphere.r*2) ){
					triCnt++;
					continue;
				}
					
				if( ground.collisionGround[0].triangle[triCnt].vertex[0].Y < GetUnit(i).colMoveSphere.position.Y - (GetUnit(i).colMoveSphere.r*2) &&
					ground.collisionGround[0].triangle[triCnt].vertex[1].Y < GetUnit(i).colMoveSphere.position.Y - (GetUnit(i).colMoveSphere.r*2) &&
					ground.collisionGround[0].triangle[triCnt].vertex[2].Y < GetUnit(i).colMoveSphere.position.Y - (GetUnit(i).colMoveSphere.r*2) ){
					triCnt++;
					continue;
				}
				/// ポリゴンとの衝突判定
				result = colCheck.CheckHitCapsuleToTriangles( colCap, ground.collisionGround[0].triangle[triCnt], ref resultPos );
				if( result == true ){

					/// 衝突箇所と始点の距離を算出
					distance = colCheck.Distance( colCap.colLine.posStart, resultPos );
					/// 距離が近いポリゴンの情報を保持する
					if( trgDistance < 0.0f || trgDistance > distance ){
						trgDistance = distance;
						trgTriNum = triCnt;
						trgTriResultPos = resultPos;
					}
			
				}
				triCnt++;
			}
			/// 傾斜が30度以上の地面と当たりを判定
			triCnt = 0;
			while( triCnt<ground.collisionGround[0].triangle.Count ){
				/// 平らな地面に対する法線の角度を算出
				nomAngle = GetTriangleNormalAngle( ground.collisionGround[0].triangle[triCnt] );
				/// 角度が30度より少ない場合は、判定しない
				if( nomAngle < 15.0f ){
					triCnt++;
					continue;
				}
					
				/// ポリゴンとの衝突判定
				result = colCheck.CheckHitCapsuleToTriangles( colCap, ground.collisionGround[0].triangle[triCnt], ref resultPos );
				if( result == true ){

					/// 衝突箇所と始点の距離を算出
					distance = colCheck.Distance( colCap.colLine.posStart, resultPos );
					/// 距離が近いポリゴンの情報を保持する
					if( trgDistance < 0.0f || trgDistance > distance ){
						trgDistance = distance;
						trgTriNum = triCnt;
						trgTriResultPos = resultPos;
					}
			
				}
				triCnt++;
			}
			/// 地面のポリゴンと当たりがあるか確認
			if( trgTriNum >= 0 ){
					result = true;
					resultPos = trgTriResultPos;
					triCnt = trgTriNum;

					/// 衝突後の位置を、移動用の当たり情報に設定
					hitTriangle.SetPos(
						ground.collisionGround[0].triangle[triCnt].vertex[0],
						ground.collisionGround[0].triangle[triCnt].vertex[1],
						ground.collisionGround[0].triangle[triCnt].vertex[2] );
											
					GetUnit(i).colMoveSphere.position.X = resultPos.X + (ground.collisionGround[0].triangle[triCnt].plane.nor.X * colCap.r );
					GetUnit(i).colMoveSphere.position.Y = resultPos.Y + (ground.collisionGround[0].triangle[triCnt].plane.nor.Y * colCap.r );
					GetUnit(i).colMoveSphere.position.Z = resultPos.Z + (ground.collisionGround[0].triangle[triCnt].plane.nor.Z * colCap.r );
					/// 地面の傾斜を算出、設定する
					float angle = GetTriangleNormalAngle( ground.collisionGround[0].triangle[triCnt] );
					GetUnit(i).landAngleX = angle;
			}

			/// 壁との当たり判定
			if( result == false ){
				triCnt = 0;
				while( triCnt<ground.collisionGround[1].triangle.Count ){
						
					if( ground.collisionGround[1].triangle[triCnt].vertex[0].Y > GetUnit(i).colMoveSphere.position.Y+(5.0f) &&
						ground.collisionGround[1].triangle[triCnt].vertex[1].Y > GetUnit(i).colMoveSphere.position.Y+(5.0f) &&
						ground.collisionGround[1].triangle[triCnt].vertex[2].Y > GetUnit(i).colMoveSphere.position.Y+(5.0f) ){
						triCnt++;
						continue;
					}
						
					result = colCheck.CheckHitCapsuleToTriangles( colCap, ground.collisionGround[1].triangle[triCnt], ref resultPos );
					if( result == true ){
						
						hitTriangle.SetPos(
							ground.collisionGround[1].triangle[triCnt].vertex[0],
							ground.collisionGround[1].triangle[triCnt].vertex[1],
							ground.collisionGround[1].triangle[triCnt].vertex[2] );
						
						distance = colCheck.Distance( colCap.colLine.posStart, resultPos );
						
						GetUnit(i).colMoveSphere.position.X = resultPos.X + (ground.collisionGround[1].triangle[triCnt].plane.nor.X * colCap.r );
						GetUnit(i).colMoveSphere.position.Y = resultPos.Y + (ground.collisionGround[1].triangle[triCnt].plane.nor.Y * colCap.r );
						GetUnit(i).colMoveSphere.position.Z = resultPos.Z + (ground.collisionGround[1].triangle[triCnt].plane.nor.Z * colCap.r );
						break;
					}
					triCnt++;
				}
			}

			/// 移動用当たり情報を、表示位置に設定
			GetUnit(i).SetPosition( GetUnit(i).colMoveSphere.position.X,
								  GetUnit(i).colMoveSphere.position.Y + GetUnit(i).colMoveSphere.r,
								  GetUnit(i).colMoveSphere.position.Z );
				
			if( GetUnit(i).type == Unit.TYPE_BUL ){
				if( result ){
					GetUnit(i).deadEffectStart = true;
				}
			}
			
			colCap.Term();
			colCap = null;
			i++;
		}
	}

	/// 登録ユニットと地面の当たり判定
	/**
	 * 当たっている場合、地面に面した位置まで移動する
	 * @warning 「CheckColUnit2Unit()」の後に実行する。
	 * @param [in] ground : 地面データクラス
	 */
	public void CheckColBullet2Ground( Ground ground )
	{
		int triCnt = 0;
		bool result = false;
		float distance = -1.0f;
		int i = 0;
		int n = 0;
		float trgDistance = -1.0f;
		int trgTriNum = -1;
		Vector3 trgTriResultPos = new Vector3( 0.0f, 0.0f, 0.0f );
		float nomAngle = 0.0f;
		int bulletIndex = -1;

		while( i<GetListCount() ){
				
			result = false;
			trgDistance = -1.0f;
			trgTriNum = -1;


			if( GetUnit(i) == null ){
				i++;
				continue;
			}

			if( GetUnit(i).type != Unit.TYPE_DEF ){
				i++;
				continue;
			}
				
			CollisionCapsule colCap = new CollisionCapsule();
			Vector3 resultPos = new Vector3( 0.0f, 0.0f, 0.0f );
				
			n = 0;
			while( n < GetUnit(i).bulletUnitIndex.Length ){
					
				bulletIndex = GetUnit(i).bulletUnitIndex[n];
					
				if( bulletIndex < 0 ){
					n++;
					continue;
				}

				/// 当たり判定用カプセル情報の作成
				/// 始点に移動前位置、終点に移動後位置を設定
				colCap = GetUnit(bulletIndex).colCapsule;

				/// 地面との当たり判定
				/// 傾斜が30度より少ない地面と当たりを判定
				triCnt = 0;
				while( triCnt<ground.collisionGround[0].triangle.Count ){
					/// 平らな地面に対する法線の角度を算出
					nomAngle = GetTriangleNormalAngle( ground.collisionGround[0].triangle[triCnt] );
					/// 角度が30度以上の場合は、判定しない
					if( nomAngle >= 30.0f ){
						triCnt++;
						continue;
					}
					/// ポリゴンとの衝突判定
					result = colCheck.CheckHitCapsuleToTriangles( colCap, ground.collisionGround[0].triangle[triCnt], ref resultPos );
					if( result == true ){

						/// 衝突箇所と始点の距離を算出
						distance = colCheck.Distance( colCap.colLine.posStart, resultPos );
						/// 距離が近いポリゴンの情報を保持する
						if( trgDistance < 0.0f || trgDistance > distance ){
							trgDistance = distance;
							trgTriNum = triCnt;
							trgTriResultPos = resultPos;
						}
			
					}
					triCnt++;
				}
				/// 傾斜が30度以上の地面と当たりを判定
				triCnt = 0;
				while( triCnt<ground.collisionGround[0].triangle.Count ){
					/// 平らな地面に対する法線の角度を算出
					nomAngle = GetTriangleNormalAngle( ground.collisionGround[0].triangle[triCnt] );
					/// 角度が30度より少ない場合は、判定しない
					if( nomAngle < 30.0f ){
						triCnt++;
						continue;
					}
					/// ポリゴンとの衝突判定
					result = colCheck.CheckHitCapsuleToTriangles( colCap, ground.collisionGround[0].triangle[triCnt], ref resultPos );
					if( result == true ){

						/// 衝突箇所と始点の距離を算出
						distance = colCheck.Distance( colCap.colLine.posStart, resultPos );
						/// 距離が近いポリゴンの情報を保持する
						if( trgDistance < 0.0f || trgDistance > distance ){
							trgDistance = distance;
							trgTriNum = triCnt;
							trgTriResultPos = resultPos;
						}
			
					}
					triCnt++;
				}
				/// 地面のポリゴンと当たりがあるか確認
				if( trgTriNum >= 0 ){
						result = true;
						resultPos = trgTriResultPos;
						triCnt = trgTriNum;

						/// 衝突後の位置を、移動用の当たり情報に設定
						hitTriangle.SetPos(
							ground.collisionGround[0].triangle[triCnt].vertex[0],
							ground.collisionGround[0].triangle[triCnt].vertex[1],
							ground.collisionGround[0].triangle[triCnt].vertex[2] );
											
						GetUnit(bulletIndex).colMoveSphere.position.X = resultPos.X + (ground.collisionGround[0].triangle[triCnt].plane.nor.X * colCap.r );
						GetUnit(bulletIndex).colMoveSphere.position.Y = resultPos.Y + (ground.collisionGround[0].triangle[triCnt].plane.nor.Y * colCap.r );
						GetUnit(bulletIndex).colMoveSphere.position.Z = resultPos.Z + (ground.collisionGround[0].triangle[triCnt].plane.nor.Z * colCap.r );
						/// 地面の傾斜を算出、設定する
						float angle = GetTriangleNormalAngle( ground.collisionGround[0].triangle[triCnt] );
						GetUnit(bulletIndex).landAngleX = angle;
				}

				/// 壁との当たり判定
				if( result == false ){
					triCnt = 0;
					while( triCnt<ground.collisionGround[1].triangle.Count ){
						result = colCheck.CheckHitCapsuleToTriangles( colCap, ground.collisionGround[1].triangle[triCnt], ref resultPos );
						if( result == true ){
						
							hitTriangle.SetPos(
								ground.collisionGround[1].triangle[triCnt].vertex[0],
								ground.collisionGround[1].triangle[triCnt].vertex[1],
								ground.collisionGround[1].triangle[triCnt].vertex[2] );
						
							distance = colCheck.Distance( colCap.colLine.posStart, resultPos );
						
							GetUnit(bulletIndex).colMoveSphere.position.X = resultPos.X + (ground.collisionGround[1].triangle[triCnt].plane.nor.X * colCap.r );
							GetUnit(bulletIndex).colMoveSphere.position.Y = resultPos.Y + (ground.collisionGround[1].triangle[triCnt].plane.nor.Y * colCap.r );
							GetUnit(bulletIndex).colMoveSphere.position.Z = resultPos.Z + (ground.collisionGround[1].triangle[triCnt].plane.nor.Z * colCap.r );
							break;
						}
						triCnt++;
					}
				}

				if( result ){
					GetUnit(bulletIndex).deadEffectStart = true;
				}
				n++;
			}
			
			colCap.Term();
			colCap = null;
			i++;
		}
	}
		
		
	/// 敵ユニットと移動用ルートポイントの当たり判定
	/**
	 * @param [in] ground : Groundクラス
	 */
	public void CheckColUnit2RoutePoint( Ground ground )
	{
		int i = 0;
		int m = 0;
		Vector3 unitPos = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 routePos = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 disVec;
		float distance = -1;
		Posture point = new Posture();

		while( i<GetListCount() ){
				
			if( GetUnit(i) == null ){
				i++;
				continue;
			}

			/// 敵ユニットのみ判定を行う
			if( GetUnit(i).type != Unit.TYPE_ACT ){
				i++;
				continue;
			}
				
			unitPos.X = GetUnit(i).GetPosition().X;
			unitPos.Y = GetUnit(i).GetPosition().Y;
			unitPos.Z = GetUnit(i).GetPosition().Z;
				
			m = 0;
			while( m < ground.routePoint.Length ){

				point.SetPosture( ground.routePoint[m] );
				routePos.X = ground.routePoint[m].M41;
				routePos.Y = ground.routePoint[m].M42;
				routePos.Z = ground.routePoint[m].M43;
				
				/// ユニットとルートポイントとの距離を算出
				disVec = unitPos - routePos;
				distance = FMath.Sqrt( disVec.Dot(disVec) );
				
				/// 一定距離以内なら当たりと判断する
				/// ※ポイントによって当たり範囲を調整
				if( ground.routePointName[m] == "dummy_branch_point" ||
					ground.routePointName[m] == "dummy_branch_point2" ||
					ground.routePointName[m] == "dummy_branch_point3" ){
					if( distance <= 4.0f ){
						GetUnit(i).hitRoutePoint = m;
						break;
					}
				}
				else if( ground.routePointName[m] == "dummy_null" ||
						 ground.routePointName[m] == "dummy_null24" ||
						 ground.routePointName[m] == "dummy_null42" ||
						 ground.routePointName[m] == "dummy_null85" ||
						 ground.routePointName[m] == "dummy_null86" ||
						 ground.routePointName[m] == "dummy_null22" ||
						 ground.routePointName[m] == "dummy_null45" ){
					if( distance <= 5.0f ){
						GetUnit(i).hitRoutePoint = m;
						break;
					}
				}
				else if( ground.routePointName[m] == "dummy_null33" ||
					ground.routePointName[m] == "dummy_null34" ){
					if( distance <= 2.0f ){
						GetUnit(i).hitRoutePoint = m;
						break;
					}
				}
				else if( distance <= 3.0f ){
					GetUnit(i).hitRoutePoint = m;
					break;
				}

				m++;
			}

			/// ルートポイントとの当たり処理
			if( GetUnit(i).hitRoutePoint >= 0 && GetUnit(i).hitRoutePoint != GetUnit(i).nowRoutePoint ){
					
				GetUnit(i).nowRoutePoint = GetUnit(i).hitRoutePoint;
				/// 分岐ポイントとの当たり処理
				/// ルートA
				if( GetUnit(i).routePattern == Unit.ROUTE_PATTERN_A ){
					if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point" ){
						GetUnit(i).AddYPR( 0.0f, (45.0f*(3.141593f/180.0f)), 0.0f );
						if( GetUnit(i).nowRouteLine == Unit.ROUTE_LINE_RIGHT ){
							GetUnit(i).AddYPR( 0.0f, (25.0f*(3.141593f/180.0f)), 0.0f );
						}
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point2" ){
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point3" ){
					}else{
						/// ルートポイントの向きをユニットに設定
						GetUnit(i).SetPosture( ground.routePoint[GetUnit(i).hitRoutePoint] );
						/// ユニットのワールド位置を再設定
						GetUnit(i).SetPosition( unitPos.X, unitPos.Y, unitPos.Z );
					}
				/// ルートB
				}else if( GetUnit(i).routePattern == Unit.ROUTE_PATTERN_B ){
					if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point" ){
						GetUnit(i).AddYPR( 0.0f, (45.0f*(3.141593f/180.0f)), 0.0f );
						if( GetUnit(i).nowRouteLine == Unit.ROUTE_LINE_RIGHT ){
							GetUnit(i).AddYPR( 0.0f, (25.0f*(3.141593f/180.0f)), 0.0f );
						}
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point2" ){
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point3" ){
						GetUnit(i).AddYPR( 0.0f, (-80.0f*(3.141593f/180.0f)), 0.0f );
					}else{
						/// ルートポイントの向きをユニットに設定
						GetUnit(i).SetPosture( ground.routePoint[GetUnit(i).hitRoutePoint] );
						/// ユニットのワールド位置を再設定
						GetUnit(i).SetPosition( unitPos.X, unitPos.Y, unitPos.Z );
					}
				/// ルートC
				}else if( GetUnit(i).routePattern == Unit.ROUTE_PATTERN_C ){
					if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point" ){
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point2" ){
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point3" ){
					}else{
						/// ルートポイントの向きをユニットに設定
						GetUnit(i).SetPosture( ground.routePoint[GetUnit(i).hitRoutePoint] );
						/// ユニットのワールド位置を再設定
						GetUnit(i).SetPosition( unitPos.X, unitPos.Y, unitPos.Z );
					}
				/// ルートD
				}else if( GetUnit(i).routePattern == Unit.ROUTE_PATTERN_D ){
					if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point" ){
						GetUnit(i).AddYPR( 0.0f, (-45.0f*(3.141593f/180.0f)), 0.0f );
						if( GetUnit(i).nowRouteLine == Unit.ROUTE_LINE_LEFT ){
							GetUnit(i).AddYPR( 0.0f, (-25.0f*(3.141593f/180.0f)), 0.0f );
						}
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point2" ){
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point3" ){
					}else{
						/// ルートポイントの向きをユニットに設定
						GetUnit(i).SetPosture( ground.routePoint[GetUnit(i).hitRoutePoint] );
						/// ユニットのワールド位置を再設定
						GetUnit(i).SetPosition( unitPos.X, unitPos.Y, unitPos.Z );
					}
				/// ルートE
				}else if( GetUnit(i).routePattern == Unit.ROUTE_PATTERN_E ){
					if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point" ){
						GetUnit(i).AddYPR( 0.0f, (-45.0f*(3.141593f/180.0f)), 0.0f );
						if( GetUnit(i).nowRouteLine == Unit.ROUTE_LINE_LEFT ){
							GetUnit(i).AddYPR( 0.0f, (-25.0f*(3.141593f/180.0f)), 0.0f );
						}
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point2" ){
						GetUnit(i).AddYPR( 0.0f, (70.0f*(3.141593f/180.0f)), 0.0f );
					}else if( ground.routePointName[GetUnit(i).hitRoutePoint] == "dummy_branch_point3" ){
					}else{
						/// ルートポイントの向きをユニットに設定
						GetUnit(i).SetPosture( ground.routePoint[GetUnit(i).hitRoutePoint] );
						/// ユニットのワールド位置を再設定
						GetUnit(i).SetPosition( unitPos.X, unitPos.Y, unitPos.Z );
					}
				/// それ以外(※ここの処理は行われない)
				}else{
					/// ルートポイントの向きをユニットに設定
					GetUnit(i).SetPosture( ground.routePoint[GetUnit(i).hitRoutePoint] );
					/// ユニットのワールド位置を再設定
					GetUnit(i).SetPosition( unitPos.X, unitPos.Y, unitPos.Z );
				}
			}

			i++;
		}

	}

	/// 登録ユニット同士の当たり判定
	/**
	 * @warning 「CheckColUnit2Stage()」より先に実行する。
	 */
	public void CheckColUnit2Unit()
	{
	}

	/// 防衛ユニットの索敵判定
	/**
	 * @param [in] index : ユニットのインデックス番号
	 * @return int : 見つけたユニットのインデックス番号
	 */
	public int SearchEnemy( int index )
	{
		Unit defUnit = GetUnit(index);
		Unit actUnit;
			
		if( defUnit == null ){
			return -1;
		}
			
		if( defUnit.type != Unit.TYPE_DEF ){
			return -1;
		}
			
		if( !defUnit.enterActFlg ){
			return -1;
		}
			
		Vector3 pos;
		float dis = 0.0f;
		int i = 0;
		int targetIndex = -1;
		float targetLength = 0.0f;
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			actUnit = GetUnit(i);
			if( actUnit.type == Unit.TYPE_ACT ){
				pos = (GetUnit(index).GetPosition() - actUnit.GetPosition());
				dis = FMath.Sqrt( pos.Dot(pos) );
				if( dis <= defUnit.length ){
					if( targetLength == 0.0f || targetLength > dis ){
						targetIndex = i;
						targetLength = dis;
					}
				}
			}
			i++;
		}
		
		return targetIndex;
	}

	/// 防衛ユニットの向きをターゲットユニット方向に設定
	/**
	 * @param [in] defIndex : 向きを変更する防衛ユニットのインデックス番号
	 * @param [in] targetIndex : 変更する向きの対象となるユニットのインデックス番号
	 */
	public bool CheckDefCanAttackTarget( int defIndex, int targetIndex )
	{
		Unit defUnit = GetUnit(defIndex);
		bool result = true;
		float rotUpAngle = 0.0f;
		float rotDownAngle = 0.0f;


		if( defUnit.kind == Unit.KIND_DEF_LASER ){
			rotUpAngle = 70.0f;
			rotDownAngle = 23.0f;
		}
		else if( defUnit.kind == Unit.KIND_DEF_WIDE_LASER ){
			rotUpAngle = 70.0f;
			rotDownAngle = 20.0f;
		}
		else if( defUnit.kind == Unit.KIND_DEF_HIGH_EXPLOSIVE ){
			rotUpAngle = 40.0f;
			rotDownAngle = 18.0f;
		}
		else if( defUnit.kind == Unit.KIND_DEF_MISSILE ){
			rotUpAngle = 60.0f;
			rotDownAngle = 17.0f;
		}
			
		rotUpAngle = Unit.DEF_PARAM_DATA[(defUnit.kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_UP_ANGLE];
		rotDownAngle = Unit.DEF_PARAM_DATA[(defUnit.kind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_DOWN_ANGLE];
			
		result = CheckAngle2Target( defIndex, targetIndex, rotUpAngle, rotDownAngle );

		return result;
	}
		
	public bool CheckAngle2Target( int defIndex, int trgIndex, float limitUp, float limitDown )
	{
		Unit defUnit = GetUnit(defIndex);
		Unit tarUnit = GetUnit(trgIndex);
		float angleUpLimit = limitUp;
		float angleDownLimit = limitDown;
		Vector3 trgPos = tarUnit.GetPosition();
		Vector3 muzzlePos;
			
		Posture tmpSetEnemyPosture = new Posture();
		tmpSetEnemyPosture.SetPosture( defUnit.GetPosture() );
		tmpSetEnemyPosture.LookAt( trgPos.X, tmpSetEnemyPosture.GetPosition().Y, trgPos.Z );
		defUnit.obj3d[0].model.WorldMatrix = defUnit.GetPosture();
		defUnit.obj3d[0].model.Update();
			
		if( defUnit.kind == Unit.KIND_DEF_LASER ){
			basePosture.SetPosture( defUnit.obj3d[0].model.Bones[comUtil.GetBoneId( defUnit.obj3d[0].model, "A01_y_axis" )].WorldMatrix );
		}
		else if( defUnit.kind == Unit.KIND_DEF_WIDE_LASER ){
			basePosture.SetPosture( defUnit.obj3d[0].model.Bones[comUtil.GetBoneId( defUnit.obj3d[0].model, "A02_y_axis" )].WorldMatrix );
		}
		else if( defUnit.kind == Unit.KIND_DEF_HIGH_EXPLOSIVE ){
			basePosture.SetPosture( defUnit.obj3d[0].model.Bones[comUtil.GetBoneId( defUnit.obj3d[0].model, "A03_y_axis" )].WorldMatrix );
		}
		else if( defUnit.kind == Unit.KIND_DEF_MISSILE ){
			basePosture.SetPosture( defUnit.obj3d[0].model.Bones[comUtil.GetBoneId( defUnit.obj3d[0].model, "A04_base" )].WorldMatrix );
		}
			
		muzzlePosture.SetPosture( defUnit.obj3d[0].model.Bones[comUtil.GetBoneId( defUnit.obj3d[0].model, "y_axis_x_axis" )].WorldMatrix );
		muzzlePos = muzzlePosture.GetPosition();
		muzzlePosture.SetPosture( basePosture.GetPosture() );
		muzzlePosture.SetPosition( muzzlePos.X, muzzlePos.Y, muzzlePos.Z );
			
		muzzlePosture.LookAt( trgPos.X, muzzlePosture.GetPosition().Y, trgPos.Z );
				
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
		targetMat.M41 = trgPos.X;
		targetMat.M42 = trgPos.Y;
		targetMat.M43 = trgPos.Z;
				
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
		
	public float GetTargetAngleX( int orgIndex, int targetIndex )
	{
		Unit orgUnit = GetUnit( orgIndex );
		Unit trgUnit = GetUnit( targetIndex );
			
		float z = ( trgUnit.GetPosition().Z - orgUnit.GetPosition().Z );
		float y = ( trgUnit.GetPosition().Y - orgUnit.GetPosition().Y );
			
		float seeta = FMath.Atan( y/z );
		seeta = (seeta*180.0f/3.141593f);
			
		return seeta;
	}
		
	public float GetTriangleNormalAngle( CollisionTriangle tri )
	{
		float result = 0.0f;

		Vector3 objSphere = new Vector3( 0.0f, 1.0f, 0.0f );
		objSphere = Vector3.Normalize( objSphere );
		Vector3 objNormal = new Vector3(
				tri.plane.nor.X,
				tri.plane.nor.Y,
				tri.plane.nor.Z );
		double a = (objSphere.X*objSphere.X) + (objSphere.Y*objSphere.Y) + (objSphere.Z*objSphere.Z);
		a = Math.Sqrt( a );
		double b = (objNormal.X*objNormal.X) + (objNormal.Y*objNormal.Y) + (objNormal.Z*objNormal.Z);
		b = Math.Sqrt( b );
		float ab = (float)(a*b);
		float vec = Vector3.Dot( objSphere, objNormal );
		float cosValue = (vec/ab);
		double cosAngle = Math.Acos((double)cosValue);
		float angle = (float)(cosAngle*180.0f/3.141593f);
			
		result = angle;
			
		return result;
	}

	/// ３頂点からラジアンを返す
	private float getRadian( Vector3 posBase, Vector3 pos1, Vector3 pos2 )
	{
		Vector3 calA = pos1 - posBase;
		Vector3 calB = pos2 - posBase;

		float lba	= calA.Length();
		float lca	= calB.Length();
		float radian= FMath.Acos( calA.Dot(calB) / (lba*lca) );

		return radian;
	}
		
	private void DelUnitTargetIndex( int trgIndex )
	{
		int i = 0;
		
//		comUtil.SetLog( "====[log][UnitManager.cs]DelUnitTargetIndex() start" );			
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type != Unit.TYPE_DEF ){
				i++;
				continue;
			}
				
			if( GetUnit(i).target == trgIndex ){
				GetUnit(i).target = -1;					
			}
			i++;
		}
//		comUtil.SetLog( "====[log][UnitManager.cs]DelUnitTargetIndex() end" );			
	}
		

	private bool CheckDelUnitTarget2Bullet( int trgIndex )
	{
		int i = 0;
		
		while( i < GetListCount() ){
			if( GetUnit(i) == null ){
				i++;
				continue;
			}
			if( GetUnit(i).type != Unit.TYPE_BUL ){
				i++;
				continue;
			}
				
			if( GetUnit(i).target == trgIndex ){
				return true;
			}
			i++;
		}
			
		return false;
	}
		
	public int GetPoints()
	{
		return addPoints;
	}
		
	public void ClearPoints()
	{
		addPoints = 0;
	}

	/// ユニット位置がカメラ表示領域内か確認
	/**
	 * @param [in] index : ユニット管理Index
	 */
	public bool CheckUnitCulling( int index )
	{
		CommonUtil comUtil = CommonUtil.Inst();
		CameraInfo camInfo = CameraInfo.Inst();
		Unit unit = GetUnit(index);
			
		Vector3 vec1 = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 vec2 = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 crossRes;
		Matrix4 matNear;
		Matrix4 matFar;
		Matrix4 matChar;
		Matrix4 matCamera;
		Matrix4 matCameraInverse = Matrix4.Inverse( camInfo.GetPosture().GetPosture() );
		Vector3 tmpVec = new Vector3( 0.0f, 0.0f, 0.0f );
		
		/// Far平面左上の3D座標算出
		comUtil.GetScreen2WorldPos( 0, 0, 1.0f, ref tmpVec );

		/// カメラのMatrixを取得
		matCamera = camInfo.GetPosture().GetPosture();
			
		/// Far平面左上のMatrix作成
		matFar = matCamera;
		matFar.M41 = tmpVec.X;
		matFar.M42 = tmpVec.Y;
		matFar.M43 = tmpVec.Z;
			
		comUtil.GetScreen2WorldPos( 0, 0, 0.0f, ref tmpVec );
		matNear = matCamera;
		matNear.M41 = tmpVec.X;
		matNear.M42 = tmpVec.Y;
		matNear.M43 = tmpVec.Z;

		/// ユニットのMatrixを取得
		matChar = unit.GetPosture();

		/// カメラMatrixの逆行列を掛け、カメラ位置を原点とした各座標算出
		matFar = matCameraInverse * matFar;
		matChar = matCameraInverse * matChar;
		matCamera = matCamera * matCameraInverse;
		matNear = matCameraInverse * matNear;

		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matChar.M41 - matCamera.M41;
		vec1.Y = matChar.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = (matFar.M41-10.0f) - matCamera.M41;
		vec2.Y = matFar.M42 - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※Y座標はカメラ位置を設定

		/// 外積
		crossRes = comUtil.Cross2( vec1, vec2 );

		/// 左側の画面外に出ているか判定
		if( crossRes.Y < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show left out" );
			return false;
		}

		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matCamera.M41 - matCamera.M41;
		vec1.Y = matChar.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = matCamera.M41 - matCamera.M41;
		vec2.Y = (matFar.M42+10.0f) - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※X座標はカメラ位置を設定

		/// 外積
		crossRes = comUtil.Cross2( vec1, vec2 );

		/// 上側の画面外に出ているか判定
		if( crossRes.X < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show up out" );
			return false;
		}



		/// Far平面右下の3D座標算出
		comUtil.GetScreen2WorldPos( Graphics2D.Width, Graphics2D.Height, 1.0f, ref tmpVec );

		/// Far平面右下のMatrixを作成
		matFar.M41 = tmpVec.X;
		matFar.M42 = tmpVec.Y;
		matFar.M43 = tmpVec.Z;
		/// カメラMatrixの逆行列を掛け、カメラ位置を原点とした座標算出
		matFar = matCameraInverse * matFar;

		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matChar.M41 - matCamera.M41;
		vec1.Y = matCamera.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = (matFar.M41+10.0f) - matCamera.M41;
		vec2.Y = matCamera.M42 - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※Y座標はカメラ位置を設定
		
		/// 外積
		crossRes = comUtil.Cross2( vec2, vec1 );
			
		/// 右側の画面外に出ているか判定
		if( crossRes.Y < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show right out" );
			return false;
		}


		/// カメラ位置からユニット位置へのベクトル算出
		vec1.X = matCamera.M41 - matCamera.M41;
		vec1.Y = matChar.M42 - matCamera.M42;
		vec1.Z = matChar.M43 - matCamera.M43;
		/// カメラ位置からFar平面へのベクトル算出
		vec2.X = matCamera.M41 - matCamera.M41;
		vec2.Y = (matFar.M42-10.0f) - matCamera.M42;
		vec2.Z = matFar.M43 - matCamera.M43;
		/// ※X座標はカメラ位置を設定
		
		/// 外積
		crossRes = comUtil.Cross2( vec2, vec1 );
			
		/// 下側の画面外に出ているか判定
		if( crossRes.X < 0.0f ){
//			comUtil.SetLog( "[log][UnitManager.cs]====show bottom out" );
			return false;
		}

		/// 対象のユニットはカメラ表示領域内に存在する
		return true;
	}
		
	public long getHitTimeBullet2Enemy( Vector3 bullet, Vector3 enemy ){
		float distanse = 0.0f;
		float time = 0.0f;
				
		distanse = comUtil.GetDistance( bullet, enemy );
		time = distanse;
				
		return (long)time;
	}
		
	public long getHitTimeBullet2Enemy( Vector3 bullet, Vector3 enemy, float move ){
		float distanse = 0.0f;
		float time = 0.0f;
				
		distanse = comUtil.GetDistance( bullet, enemy );
		time = distanse;
				
		return (long)time;
	}
		
	public long GetHitFrameBullet2Enemy( Vector3 bullet, Vector3 enemy, int bKind )
	{
		float distanse = 0.0f;
		distanse = comUtil.GetDistance( bullet, enemy );
		float bulletSpeed = Unit.DEF_PARAM_DATA[(bKind*Unit.DEF_PARAM_NUM)+Unit.DEF_PARAM_BUL_SPEED]/100.0f;
			
		if( bKind == Unit.KIND_DEF_LASER ){
			return (long)(distanse/bulletSpeed);
		}
		else if( bKind == Unit.KIND_DEF_WIDE_LASER ){
			return (long)1;
		}
		else if( bKind == Unit.KIND_DEF_HIGH_EXPLOSIVE ){
			return (long)distanse;
		}
		else if( bKind == Unit.KIND_DEF_MISSILE ){
			return (long)distanse;
		}
			
		return 0;
	}
		
		
	public int GetRoutPointMs( int trg, long ms )
	{
		int nowRoutPoint = GetUnit(trg).routNum;
			
		return (nowRoutPoint + (int)ms/2);
	}
		
	public int GetRoutPoint2Frame( int index, long frame )
	{
		int nowRoutPointId = 0;
		int nextRoutPointId = 0;
		int routPatturn = GetUnit(index).routePattern;
		Matrix4 nowRoutPointMat;
		Matrix4 nextRoutPointMat;
		Vector3 nowRoutPointPos = new Vector3( 0.0f, 0.0f, 0.0f );
		Vector3 nextRoutPointPos = new Vector3( 0.0f, 0.0f, 0.0f );
		long nowFrame = 0;
		int nowRoutPoint = GetUnit(index).routNum;
		float disNow2Next = 0.0f;
		float moveOneFrame = 0.0f;
		float addNextValue = 0.0f;
		float angleX = 0.0f;
		
		while( nowFrame < frame ){
				
			if( nowRoutPoint >= rout.GetRoutPointMaxNum( GetUnit(index).routePattern ) -1 ){
				return (rout.GetRoutPointMaxNum( GetUnit(index).routePattern ) -1);
			}
				
			nowRoutPointId = nowRoutPoint;
			nextRoutPointId = nowRoutPoint+1;
			nowRoutPointMat = rout.GetRoutPointMatrix( routPatturn, nowRoutPointId );
			nextRoutPointMat = rout.GetRoutPointMatrix( routPatturn, nextRoutPointId );
			nowRoutPointPos.X = nowRoutPointMat.M41;
			nowRoutPointPos.Y = nowRoutPointMat.M42;
			nowRoutPointPos.Z = nowRoutPointMat.M43;
			nextRoutPointPos.X = nextRoutPointMat.M41;
			nextRoutPointPos.Y = nextRoutPointMat.M42;
			nextRoutPointPos.Z = nextRoutPointMat.M43;
				
			moveOneFrame = (Unit.ACT_PARAM_DATA[(GetUnit(index).kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_MOVE_SPEED])/30.0f;
				
			angleX = GetRoutPointMatrixAngle( nowRoutPointMat );
				
			if( angleX > 100.0f ){
				moveOneFrame = moveOneFrame * (Unit.ACT_PARAM_DATA[(GetUnit(index).kind*Unit.ACT_PARAM_NUM)+Unit.ACT_PARAM_CLIME]/100.0f);
			}
				
			disNow2Next = comUtil.GetDistance( nowRoutPointPos, nextRoutPointPos );
			
			if( addNextValue > 0.0f ){
				disNow2Next -= addNextValue;
				addNextValue = 0.0f;
			}

			while( disNow2Next >= 0.0f ){
				nowFrame++;
				disNow2Next -= moveOneFrame;
				if( nowFrame >= frame ){
					break;
				}
			}
				
			if( nowFrame >= frame ){
				break;
			}
				
			if( disNow2Next < 0.0f ){
				addNextValue = disNow2Next * -1;
			}
			else{
				addNextValue = 0.0f;
			}
			
			nowRoutPoint++;
					
		}
	
		return nowRoutPoint;
	}
		
	public float GetRoutPointMatrixAngle( Matrix4 mat )
	{
		float angle = 0.0f;
				
		calcAnglePosture.SetPosture( mat );
		calcAnglePosture.AddPosition( 0.0f, 0.0f, 1.0f );
		calcVecPosZ = calcAnglePosture.GetPosition();
					
		calcAnglePosture.SetPosture( mat );
		calcAnglePosture.AddPositionW( 0.0f, 1.0f, 0.0f );
		calcVecPosY = calcAnglePosture.GetPosition();
					
		calcAnglePosture.SetPosture( mat );
		angle = comUtil.getRadian(
					calcAnglePosture.GetPosition(),
					calcVecPosZ,
					calcVecPosY );
		angle = angle * 180.0f / (float)Math.PI;
				
//		comUtil.SetLog( "====angle:"+angle );
		return angle;
	}
		
	public void SettingUnit2Bullet( int unitIndex )
	{
		int i = unitIndex;
		int n = 0;
		/// 弾ユニットの初期化
		Unit unit;
		if( GetUnit(i).kind == Unit.KIND_DEF_LASER ){
			unit = new UnitBullet1();
			AudioManager.PlaySound( "SE_LASER" );
		}else if( GetUnit(i).kind == Unit.KIND_DEF_WIDE_LASER ){
			unit = new UnitBullet2();
		}else if( GetUnit(i).kind == Unit.KIND_DEF_HIGH_EXPLOSIVE ){
			unit = new UnitBullet3();
			AudioManager.PlaySound( "SE_EXPLOSIVE" );
		}else if( GetUnit(i).kind == Unit.KIND_DEF_MISSILE ){
			unit = new UnitBullet4();
			AudioManager.PlaySound( "SE_MISSILE" );
		}else{
			unit = new UnitBullet1();
		}
		unit.Init();

		/// 弾ユニットのターゲットに敵ユニットIndexを設定
		unit.target = GetUnit(i).target;
		/// ターゲットの位置を設定
		unit.targetPos = GetUnit(i).targetPos;
							
		/// 弾ユニットの発射位置を設定
		unit.muzzlePos.X = GetUnit(i).muzzlePos.X;
		unit.muzzlePos.Y = GetUnit(i).muzzlePos.Y;
		unit.muzzlePos.Z = GetUnit(i).muzzlePos.Z;
		unit.SetPosition( GetUnit(i).muzzlePos.X,
						  GetUnit(i).muzzlePos.Y,
						  GetUnit(i).muzzlePos.Z );
							
		/// 弾ユニットの姿勢を敵方向へ向ける
		unit.LookAt( unit.targetPos.X,
					 unit.targetPos.Y,
					 unit.targetPos.Z );

		/// 弾ユニットの移動フラグをON
		unit.bulletMove = true;
		/// 弾ユニットをユニット管理クラスに追加
		int bulletIndex = AddUnit( unit );
							
		/// 弾ユニットのユニットIndexを味方ユニットが所持
		n = 0;
		while( n < GetUnit(i).bulletUnitIndex.Length ){
			if( GetUnit(i).bulletUnitIndex[n] < 0 ){
				GetUnit(i).bulletUnitIndex[n] = bulletIndex;
				break;
			}
			n++;
		}
	}
		
}

} // end ns DefenseDemo
