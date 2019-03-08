/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

// 2D Physicsフレームワークをインクルード
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2dDemo
{

/// Stage00クラス
/// シミュレーションシーンを作るにはPhysicsSceneを継承する
public class Stage00 : Stage 
{
	private int[]  colObjIdx   = new int[50]; // 鉄骨（ビル）衝突判定用
	private int    colObjCnt   = 0;
	private int[]  colBuildIdx = new int[25]; // ビルのindex
	private int    colBuildCnt = 0;
	private bool   colFlag     = false;       // 鉄骨に衝突したかどうか
	private int[]  boneSIdx    = new int[6];  // 横の形鋼のsceneBodiesインデックス
	private int    boneSCnt    = 0;	
	private int[]  boneLIdx    = new int[6];  // 縦の形鋼のsceneBodiesインデックス
	private int    boneLCnt    = 0;	
	private int[]  boneSLIdx   = new int[4];  // 斜めの形鋼のsceneBodiesインデックス
	private int    boneSLCnt   = 0;
	private int[]  boneJIdx    = new int[9];  // 接合部のsceneBodiesインデックス
	private int[]  signIdx     = new int[4];  // 看板のsceneBodiesインデックス
	private int[]  coneIdx     = new int[2];  // 三角コーンのsceneBodiesインデックス
	private int    coneCnt     = 0;
	private int[]  boxIdx      = new int[16]; // 資材のsceneBodiesインデックス
	private int    boxCnt      = 0;
	private int[]  craneIdx    = new int[3];  // クレーンのsceneBodiesインデックス
	private int    truckIdx    = 0;           // トラックのsceneBodiesインデックス	
	private int    crane2Idx    = 0;           // 修正版クレーン（フック）のsceneBodiesインデックス	
	private string name;
	private bool   isTruckCollision = false;

	/// コンストラクタ
	public Stage00(){
		InitScene();
			
		// レンダリングオブジェクトのセットアップ
        for (int i = 0; i < numShape; i++){
            if (sceneShapes[i].numVert == 0){
                vertices[i] = new VertexBuffer(37, VertexFormat.Float3);
            }else{
                vertices[i] = new VertexBuffer(sceneShapes[i].numVert + 1, VertexFormat.Float3);
            }

            makeLineList_Convex(sceneShapes[i], vertices[i]);
        }
		
        // 衝突点用　デバッグレンダリングバッファ
        {
            colVert = new VertexBuffer(4, VertexFormat.Float3);

            const float scale = 0.2f;

            float[] vertex = new float[]
            {
                -1.0f, -1.0f, 0.0f,
                 1.0f, -1.0f, 0.0f,
                 1.0f, 1.0f, 0.0f,
                -1.0f, 1.0f, 0.0f
            };

            for (int i = 0; i < 12; i++)
                vertex[i] = vertex[i] * scale;

            colVert.SetVertices(0, vertex);
        }

        // ジョイント用　デバッグレンダリングバッファ
        {
            jointVert = new VertexBuffer(2, VertexFormat.Float3);

            float[] vertex = new float[]
            { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            
            jointVert.SetVertices(0, vertex);
        }
	}
		
	/// シーンの初期設定
	/// 剛体の生成・設定などを行なう
	public override void InitScene()
    {
		// 空のシミュレーションシーンを作成
        base.InitScene();
		
		scene_scale = 0.04f;
		
		// 反発係数を設定
		restitutionCoeff = 0.2f;

		// Box Shape設定 PhysicsShape( "width", "height" )
		// 必要な形状を全て設定
        Vector2 wall_width = new Vector2(854, 16);
        Vector2 wall_width2 = new Vector2(854, 16+50);
        sceneShapes[0] = new PhysicsShape(wall_width2*scene_scale);

        Vector2 wall_height = new Vector2(1, 240);
        sceneShapes[1] = new PhysicsShape(wall_height*scene_scale);		
	
		Vector2 box_width = new Vector2(36.0f, 32.0f);//木箱
		sceneShapes[2] = new PhysicsShape(box_width*scene_scale);
			
		float sphere_width = 12.0f;
		sceneShapes[3] = new PhysicsShape(sphere_width*scene_scale);//ワイヤーに使用
        
		Vector2[] board_pos1 = new Vector2[5]; // 看板部品1(左三角)
		board_pos1[0] = new Vector2( -15.0f, 50.0f)*scene_scale;
		board_pos1[1] = new Vector2(-50.0f,  0.0f)*scene_scale;
		board_pos1[2] = new Vector2( -15.0f,-50.0f)*scene_scale;
		board_pos1[3] = new Vector2(  0.0f,-50.0f)*scene_scale;
		board_pos1[4] = new Vector2(  0.0f, 50.0f)*scene_scale;
		sceneShapes[4] = new PhysicsShape(board_pos1, 5);
		
		Vector2[] bottom_pos = new Vector2[4]; // 形鋼の土台
		bottom_pos[0] = new Vector2( 0.0f,   0.0f)*scene_scale;
		bottom_pos[1] = new Vector2( 600.0f, 0.0f)*scene_scale;
		bottom_pos[2] = new Vector2( 600.0f, 32.0f)*scene_scale;
		bottom_pos[3] = new Vector2( 32.0f,  32.0f)*scene_scale;
		sceneShapes[5] = new PhysicsShape(bottom_pos, 4);
		
		Vector2[] board_pos3 = new Vector2[5]; // 看板部品3(右三角)
		board_pos3[0] = new Vector2( 15.0f,-50.0f)*scene_scale;
		board_pos3[1] = new Vector2(50.0f,  0.0f)*scene_scale;
		board_pos3[2] = new Vector2( 15.0f, 50.0f)*scene_scale;
		board_pos3[3] = new Vector2( 0.0f, 50.0f)*scene_scale;
		board_pos3[4] = new Vector2( 0.0f,-50.0f)*scene_scale;
		sceneShapes[6] = new PhysicsShape(board_pos3, 5);

		Vector2 bar_width = new Vector2(56.0f, 8.0f); // 看板部品2(横棒)
		sceneShapes[7] = new PhysicsShape(bar_width*scene_scale);

		Vector2 bar_joint1 = new Vector2(4.0f, 5.0f); // 看板部品3(短い縦棒)
		sceneShapes[8] = new PhysicsShape(bar_joint1*scene_scale);
		
		Vector2 bar_joint2 = new Vector2(4.0f, 12.0f); // 看板部品4(長い縦棒)
		sceneShapes[9] = new PhysicsShape(bar_joint2*scene_scale);

		Vector2 bar_joint3 = new Vector2(28.0f, 4.0f); // 看板部品5(台)
		sceneShapes[10] = new PhysicsShape(bar_joint3*scene_scale);

		Vector2 enemy_width = new Vector2(70.0f, 25.0f);// 作業員
		Vector2[] enemy_pos = new Vector2[4];
		enemy_pos[0] = new Vector2(-80.0f, 0.0f)*scene_scale;
		enemy_pos[1] = new Vector2( -35.0f,-25.0f)*scene_scale;
		enemy_pos[2] = new Vector2( 50.0f,-25.0f)*scene_scale;
		enemy_pos[3] = new Vector2( 70.0f, 15.0f)*scene_scale;
		sceneShapes[11] = new PhysicsShape(enemy_pos, 4);

		Vector2[] truck_pos = new Vector2[4]; // トラック0 上
		truck_pos[0] = new Vector2(-132.0f,104.0f)*scene_scale;
		truck_pos[1] = new Vector2(-182.0f,  0.0f)*scene_scale;
		truck_pos[2] = new Vector2(  -15.0f,  0.0f)*scene_scale;
		truck_pos[3] = new Vector2(  -15.0f,104.0f)*scene_scale;
		sceneShapes[12] = new PhysicsShape(truck_pos, 4);
		
		Vector2 wheel_box = new Vector2(252,22); // トラックのタイヤのかわり
		sceneShapes[13] = new PhysicsShape(wheel_box*scene_scale);

		Vector2[] truck2_pos = new Vector2[4]; // トラック2+タイヤ
		truck2_pos[0] = new Vector2( -252.0f,  48.0f+22.0f)*scene_scale;
		truck2_pos[1] = new Vector2( -252.0f, -48.0f-22.0f+52.0f)*scene_scale;
		truck2_pos[2] = new Vector2(  252.0f, -48.0f-22.0f+52.0f)*scene_scale;
		truck2_pos[3] = new Vector2(  252.0f,  48.0f+22.0f)*scene_scale;
		sceneShapes[14] = new PhysicsShape(truck2_pos, 4);
			
		Vector2[] cone_pos = new Vector2[3]; // カラーコーン
		cone_pos[0] = new Vector2( 0.0f,  53.0f)*scene_scale;
		cone_pos[1] = new Vector2(-36.0f, -53.0f)*scene_scale;
		cone_pos[2] = new Vector2( 36.0f, -53.0f)*scene_scale;
		sceneShapes[15] = new PhysicsShape(cone_pos, 3);
	
		Vector2 boneL_width = new Vector2(16.0f, 88.0f); // 縦の形鋼
		sceneShapes[16] = new PhysicsShape(boneL_width*scene_scale);
		
		Vector2 boneS_width = new Vector2(32.0f, 16.0f); // 横の形鋼
		sceneShapes[17] = new PhysicsShape(boneS_width*scene_scale);

		// 接合部は四角形で対応
		Vector2 bone_joint1 = new Vector2(20.0f, 20.0f); // 接合部1
		sceneShapes[18] = new PhysicsShape(bone_joint1*scene_scale);
		
		Vector2 bone_joint2 = new Vector2(20.0f, 24.0f); // 接合部2
		sceneShapes[19] = new PhysicsShape(bone_joint2*scene_scale);
			
		Vector2 bone_joint3 = new Vector2(24.0f, 24.0f); // 接合部3
		sceneShapes[20] = new PhysicsShape(bone_joint3*scene_scale);
			
		Vector2 bone_joint4 = new Vector2(24.0f, 20.0f);  // 接合部4
		sceneShapes[21] = new PhysicsShape(bone_joint4*scene_scale);
			
		Vector2[] boneSL_pos = new Vector2[4]; // 斜めの形鋼
		boneSL_pos[0] = new Vector2( -40.0f,  88.0f)*scene_scale;
		boneSL_pos[1] = new Vector2(  24.0f, -88.0f)*scene_scale;
		boneSL_pos[2] = new Vector2(  40.0f, -88.0f)*scene_scale;
		boneSL_pos[3] = new Vector2( -24.0f,  88.0f)*scene_scale;
		sceneShapes[22] = new PhysicsShape(boneSL_pos, 4);

		Vector2 crane_parts1 = new Vector2(20.0f, 20.0f);// クレーンのパーツ1
		sceneShapes[23] = new PhysicsShape(crane_parts1*scene_scale);
						
		float arm_width = 14.0f;
		sceneShapes[24] = new PhysicsShape(arm_width*scene_scale);// クレーンアーム
			
		Vector2 arm_box = new Vector2(36.0f, 32.0f);// クレーンがつる箱
		sceneShapes[25] = new PhysicsShape(arm_box*scene_scale);			

		Vector2 crane_set = new Vector2(20.0f, 105.0f);// クレーンset
		sceneShapes[26] = new PhysicsShape(crane_set*scene_scale);			

		numShape = 27;

		// 剛体の生成・設定
        // 壁(静的剛体）で動的剛体の挙動範囲を制限
        {
			// 0.底辺
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(0, -wall_height.Y-50);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 0;
            numBody++;

			// 1.上辺
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
			sceneBodies[numBody].position = new Vector2(0, wall_height.Y+crane_set.Y);
			sceneBodies[numBody].position += new Vector2(0, (arm_width+1.0f));
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 0;
			sceneBodies[numBody].collisionFilter = 1 << 2;
            numBody++;

			// 2.形鋼の土台
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-wall_width.X + 1424.0f, -wall_height.Y + wall_width.Y);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 5;
            numBody++;
		}
		
		// 動的な剛体でスタックの生成
		{
			// 3. 作業員
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[11], 15.0f);
	        sceneBodies[numBody].Position = new Vector2(0, -wall_height.Y) + new Vector2(0, wall_width.Y + enemy_width.Y + 0.005f) + new Vector2(0f, (0.005f) * enemy_width.Y);
	        sceneBodies[numBody].Position += new Vector2(-316-427, 10.0f);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].shapeIndex = 11;
			sceneBodies[numBody].Rotation = PhysicsUtility.GetRadian(30.0f);
			sceneBodies[numBody].SetBodyKinematic();
			throwObjIdx = numBody++;
	        
			colObjIdx[colObjCnt] = numBody-1;
			colObjCnt++;//ビルとの衝突用	
				
			// 4-8. 資材
			boxCnt=0;//資材カウント用
			for (int i = 0; i < 2; i++)
            {
				for (int j = 0; j < 3 - i; j++)
            	{
	                sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
	                sceneBodies[numBody].Position = new Vector2(-wall_width.X+1183.0f + box_width.X + (box_width.X + 0.1f) * i, -wall_height.Y)// 開始位置
							+ new Vector2(0, wall_width.Y + box_width.Y + 0.005f) + new Vector2( 0, (2 * i + 0.005f) * box_width.Y);// 高さ
	                sceneBodies[numBody].Position += new Vector2((box_width.X * 2 ) * j, 0);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	                sceneBodies[numBody].shapeIndex = 2;
	                colObjIdx[colObjCnt] = numBody++;
					colObjCnt++;//ビルとの衝突用	
					boxIdx[boxCnt]= numBody-1;
					boxCnt++;	
	            }	
			}			
				
			// 9-15.看板
			{
				float sign_w, sign_h;//看板幅高さ
				sign_h = 100.0f;
				sign_w = 100.0f;
					
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X + 321.0f + sign_w/2, bar_joint3.Y*2+bar_joint2.Y*2+bar_joint1.Y*2+bar_width.Y*4 -wall_height.Y) 
					+ new Vector2(0, wall_width.Y + sign_h/2);				
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 4;
				numBody++;
				signIdx[0] = numBody-1;//座標取得のため記憶

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X + 321.0f + sign_w/2, bar_joint3.Y*2+bar_joint2.Y*2+bar_joint1.Y*2+bar_width.Y*4 -wall_height.Y) 
					+ new Vector2(0, wall_width.Y + sign_h/2);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 6;
				numBody++;

				// J: 剛体の接合を行う
				uint[] compound_combination = new uint[2]{(uint)(numBody-2), (uint)(numBody-1)};
				PhysicsBody.MergeBody(sceneBodies, compound_combination, 2, sceneShapes, compoundMap, false);
					
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], 1.0f);//横棒
				//sceneBodies[numBody].friction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X+315+bar_width.X, bar_joint3.Y*2+bar_joint2.Y*2+bar_joint1.Y*2+bar_width.Y*2 -wall_height.Y) 
					+ new Vector2(0, wall_width.Y + bar_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 7;
				numBody++;
				signIdx[1] = numBody-1;//座標取得のため記憶
					
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 1.0f);
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X+343+28.0f, bar_width.Y*2 + bar_joint3.Y*2+bar_joint2.Y*2-wall_height.Y)
					+ new Vector2(0, wall_width.Y + bar_joint1.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 8;
				numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], 1.0f);//横棒
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X+315+bar_width.X, bar_joint3.Y*2+bar_joint2.Y*2 -wall_height.Y) 
					+ new Vector2(0, wall_width.Y + bar_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 7;
				numBody++;
				signIdx[2] = numBody-1;//座標取得のため記憶

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], 1.0f);//支柱
				//sceneBodies[numBody].friction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X+343+28.0f, bar_joint3.Y*2-wall_height.Y)
					+ new Vector2(0, wall_width.Y + bar_joint2.Y );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 9;
				numBody++;
				signIdx[3] = numBody-1;//座標取得のため記憶

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 1.0f);//支柱
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2( -wall_width.X+343+28.0f, -wall_height.Y)
					+ new Vector2(0, wall_width.Y + bar_joint3.Y );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].shapeIndex = 10;
				numBody++;
				
				// J: 剛体の接合を行う
				uint[] compound_combination2 = new uint[5]{(uint)(numBody-5), (uint)(numBody-4), (uint)(numBody-3), (uint)(numBody-2), (uint)(numBody-1)};
				PhysicsBody.MergeBody(sceneBodies, compound_combination2, 5, sceneShapes, compoundMap, false);
			}
				
			// 16-21.トラック(静的剛体)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[12], PhysicsUtility.FltMax);//本体上
				sceneBodies[numBody].shapeIndex = 12;
				sceneBodies[numBody].Position = new Vector2(-wall_width.X+661.0f+144.0f/2+120.0f, 
						-wall_height.Y + wall_width.Y  + 96.0f + 44.0f);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				numBody++;

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[14], PhysicsUtility.FltMax);//荷台
				sceneBodies[numBody].shapeIndex = 14;
				sceneBodies[numBody].Position = new Vector2(-wall_width.X+661.0f+504.0f/2, 
						-wall_height.Y + wall_width.Y + truck2_pos[0].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				numBody++;
				truckIdx = numBody-1;//座標取得のため記憶
				
				{		
					//箱一段目
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 2;
	                sceneBodies[numBody].Position = new Vector2(-wall_width.X + 858.0f + box_width.X , 0);
		            sceneBodies[numBody].Position += new Vector2(0, -(284.0f - wall_height.Y) - box_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					numBody++;
					boxIdx[boxCnt]= numBody-1;
					boxCnt++;

					sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 2;
	                sceneBodies[numBody].Position = new Vector2(-wall_width.X + 933.0f + box_width.X , 0);
		            sceneBodies[numBody].Position += new Vector2(0, -(284.0f - wall_height.Y) - box_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					numBody++;
					boxIdx[boxCnt]= numBody-1;
					boxCnt++;
						
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 2;
	                sceneBodies[numBody].Position = new Vector2(-wall_width.X + 1009.0f + box_width.X , 0);
		            sceneBodies[numBody].Position += new Vector2(0, -(284.0f - wall_height.Y) - box_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					numBody++;
					boxIdx[boxCnt]= numBody-1;
					boxCnt++;
						
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 2;
	                sceneBodies[numBody].Position = new Vector2(-wall_width.X + 1082.0f + box_width.X , 0);
		            sceneBodies[numBody].Position += new Vector2(0, -(284.0f - wall_height.Y) - box_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					numBody++;
					boxIdx[boxCnt]= numBody-1;
					boxCnt++;
						
					// J: 剛体の接合を行う
					uint[] compound_combination = new uint[6]{ (uint)numBody-6, (uint)numBody-5, (uint)numBody-4, (uint)numBody-3, (uint)numBody-2, (uint)numBody-1};
					PhysicsBody.MergeBody(sceneBodies, compound_combination, 6, sceneShapes, compoundMap, false);
				}					
				// 22-(24+2).箱
				for (int i = 0; i < 2; i++)//数変更の場合SceneMainの判定も修正
	            {
					for (int j = 1; j < 4 - i; j++)
	            	{
		                sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 5.0f);
		                sceneBodies[numBody].Position = new Vector2( -wall_width.X + 861.0f + box_width.X*(i+1), 0) +
								new Vector2(0, -(284.0f - wall_height.Y) + box_width.Y);
		                sceneBodies[numBody].Position += new Vector2((box_width.X* 2 ) * j , (2 * i )* box_width.Y);//箱分移動
		                sceneBodies[numBody].shapeIndex = 2;
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		                numBody++;
						boxIdx[boxCnt]= numBody-1;
						boxCnt++;
						colObjIdx[colObjCnt] = numBody-1;
						colObjCnt++;
		            }	
				}

			}
				
			// (25+2)-(26+2).カラーコーン
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[15], 1.0f);
				sceneBodies[numBody].shapeIndex = 15;
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2(-wall_width.X+479.0f+cone_pos[2].X*(1/scene_scale), -wall_height.Y)
					+ new Vector2(0, wall_width.Y + cone_pos[0].Y*(1/scene_scale) );	
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				coneIdx[coneCnt] = numBody++;
				coneCnt++;

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[15], 1.0f);
				sceneBodies[numBody].shapeIndex = 15;
				sceneBodies[numBody].colFriction = 0.01f;
				sceneBodies[numBody].Position = new Vector2(-wall_width.X+559.0f+cone_pos[2].X*(1/scene_scale), -wall_height.Y)
					+ new Vector2(0, wall_width.Y + cone_pos[0].Y*(1/scene_scale) );	
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				coneIdx[coneCnt] = numBody++;
				coneCnt++;
			}
			
			// (27+2)-(51+2)形鋼
			{
				float weight =5.0f;
				float friction = 0.0f; //0.05f;

				//29横中央鉄骨
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], weight);
				sceneBodies[numBody].shapeIndex = 17;
				sceneBodies[numBody].colFriction = friction;
			    sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y) 
						+ new Vector2(1512 -wall_width.X + boneS_width.X, wall_width.Y + boneS_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint2.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				boneSIdx[0] = numBody++;
				boneSCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;

				//30横中央鉄骨(右)Utility.FLT_MAX
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 17;
				sceneBodies[numBody].colFriction = friction;
			    sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y) 
						+ new Vector2(1624 -wall_width.X + boneS_width.X, wall_width.Y + boneS_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint2.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneSIdx[1] = numBody++;
				boneSCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;

				//31上部横鉄骨
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], weight);
				sceneBodies[numBody].shapeIndex = 17;
				sceneBodies[numBody].colFriction = friction;
			    sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y) 
						+ new Vector2(1512 -wall_width.X + boneS_width.X, wall_width.Y + boneS_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint4.Y*2+boneL_width.Y*4+bone_joint2.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				boneSIdx[2] = numBody++;
				boneSCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;

				//32上部横鉄骨(右)
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], weight);
				sceneBodies[numBody].shapeIndex = 17;
				sceneBodies[numBody].colFriction = friction;
			    sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y) 
						+ new Vector2(1624 -wall_width.X + boneS_width.X, wall_width.Y + boneS_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint4.Y*2+boneL_width.Y*4+bone_joint2.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				boneSIdx[3] = numBody++;
				boneSCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;

				//33下横鉄骨
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 17;
				sceneBodies[numBody].colFriction = friction;
			    sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y) 
						+ new Vector2(1512 -wall_width.X + boneS_width.X, wall_width.Y + boneS_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneSIdx[4] = numBody++;
				boneSCnt++;
					
				//34下横鉄骨(右)Utility.FLT_MAX
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 17;
				sceneBodies[numBody].colFriction = friction;
			    sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y) 
						+ new Vector2(1624 -wall_width.X + boneS_width.X, wall_width.Y + boneS_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneSIdx[5] = numBody++;
				boneSCnt++;

				//35左下縦鉄骨
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], weight);
				sceneBodies[numBody].shapeIndex = 16;
				sceneBodies[numBody].colFriction = friction;
		        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
						+ new Vector2(1472 -wall_width.X + boneL_width.X, wall_width.Y + boneL_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneLIdx[0] = numBody++;
				boneLCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;
				
				//36中下縦鉄骨Utility.FLT_MAX
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 16;
				sceneBodies[numBody].colFriction = friction;
		        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
						+ new Vector2(1584 -wall_width.X + boneL_width.X, wall_width.Y + boneL_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneLIdx[1] = numBody++;
				boneLCnt++;

				//37右下縦鉄骨Utility.FLT_MAX
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 16;
				sceneBodies[numBody].colFriction = friction;
		        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
						+ new Vector2(1696 -wall_width.X + boneL_width.X, wall_width.Y + boneL_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneLIdx[2] = numBody++;
				boneLCnt++;

				//38上部縦鉄骨（左）
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], weight);
				sceneBodies[numBody].shapeIndex = 16;
				sceneBodies[numBody].colFriction = friction;
		        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
						+ new Vector2(1472 -wall_width.X + boneL_width.X, wall_width.Y + boneL_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2 + bone_joint2.Y*2 + boneL_width.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				boneLIdx[3] = numBody++;
				boneLCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;

				//39上部縦鉄骨（中）
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], weight);
				sceneBodies[numBody].shapeIndex = 16;
				sceneBodies[numBody].colFriction = friction;
		        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
						+ new Vector2(1584 -wall_width.X + boneL_width.X, wall_width.Y + boneL_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2 + bone_joint2.Y*2 + boneL_width.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				boneLIdx[4] = numBody++;
				boneLCnt++;
				colBuildIdx[colBuildCnt] = numBody-1;
				colBuildCnt++;
				
				//40上部縦鉄骨（右）Utility.FLT_MAX
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 16;
				sceneBodies[numBody].colFriction = friction;
		        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
						+ new Vector2(1696 -wall_width.X + boneL_width.X, wall_width.Y + boneL_width.Y);
				sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2 + bone_joint2.Y*2 + boneL_width.Y*2);
				sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				boneLIdx[5] = numBody++;
				boneLCnt++;

				{
					//41左下接合部Utility.FLT_MAX
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[18], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 18;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1472 - wall_width.X + bone_joint1.X, wall_width.Y + bone_joint1.Y);
					sceneBodies[numBody].Position += new Vector2( 0 , bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[0] = numBody++;
				}

				{
					//42中央下接合部Utility.FLT_MAX
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[21], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 21;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1576 - wall_width.X + bone_joint4.X, wall_width.Y + bone_joint4.Y);
					sceneBodies[numBody].Position += new Vector2( 0 , bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[1] = numBody++;
				}

				{
					//43右下接合部Utility.FLT_MAX
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[21], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 21;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1688 - wall_width.X + bone_joint4.X, wall_width.Y + bone_joint4.Y);
					sceneBodies[numBody].Position += new Vector2( 0 , bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[2] = numBody++;
				}

				//44中央左接合部
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[19], weight);
					sceneBodies[numBody].shapeIndex = 19;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1472 - wall_width.X + bone_joint2.X, wall_width.Y + bone_joint2.Y);
					sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint1.Y*2);
					sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[3] = numBody++;
					colBuildIdx[colBuildCnt] = numBody-1;
					colBuildCnt++;					
				}
					

				//45中央接合部Utility.FLT_MAX
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[20], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 20;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1576 - wall_width.X + bone_joint3.X, wall_width.Y + bone_joint3.Y);
					sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint1.Y*2);
					sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[4] = numBody++;
				}

				//46中央右接合部Utility.FLT_MAX
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[20], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 20;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1688 - wall_width.X + bone_joint3.X, wall_width.Y + bone_joint3.Y);
					sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint1.Y*2);
					sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[5] = numBody++;
				}

				//47上部左接合部
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[18], weight);
					sceneBodies[numBody].shapeIndex = 18;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1472 - wall_width.X + bone_joint1.X, wall_width.Y + bone_joint1.Y);
					sceneBodies[numBody].Position += new Vector2( 0, bone_joint4.Y*2+boneL_width.Y*4+bone_joint2.Y*2);
					sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[6] = numBody++;
					colBuildIdx[colBuildCnt] = numBody-1;
					colBuildCnt++;					
				}
					
				//48上部中央接合部
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[21], weight);
					sceneBodies[numBody].shapeIndex = 21;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1576 - wall_width.X + bone_joint4.X, wall_width.Y + bone_joint4.Y);
					sceneBodies[numBody].Position += new Vector2( 0, bone_joint4.Y*2+boneL_width.Y*4+bone_joint2.Y*2);
					sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[7] = numBody++;
					colBuildIdx[colBuildCnt] = numBody-1;
					colBuildCnt++;					
				}

				//49上部右接合部Utility.FLT_MAX
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[20], PhysicsUtility.FltMax);
					sceneBodies[numBody].shapeIndex = 20;
					sceneBodies[numBody].colFriction = friction;
			        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
							+ new Vector2(1688 - wall_width.X + bone_joint2.X, wall_width.Y + bone_joint2.Y);
					sceneBodies[numBody].Position += new Vector2( 0, bone_joint4.Y*2+boneL_width.Y*4+bone_joint2.Y*2);
					sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					boneJIdx[8] = numBody++;
					colBuildIdx[colBuildCnt] = numBody-1;
					colBuildCnt++;					
				}
					
				// 斜めの形鋼
				{
					//50左下
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[22], weight);
						sceneBodies[numBody].shapeIndex = 22;
						sceneBodies[numBody].colFriction = friction;
				        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
								+ new Vector2(1496 -wall_width.X + 48, wall_width.Y + boneSL_pos[0].Y*(1/scene_scale));
						sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2);
						sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
						sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						boneSLIdx[boneSLCnt] = numBody++;
						boneSLCnt++;
						colBuildIdx[colBuildCnt] = numBody-1;
						colBuildCnt++;					
					}
					//51右下Utility.FLT_MAX
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[22], PhysicsUtility.FltMax);
						sceneBodies[numBody].shapeIndex = 22;
						sceneBodies[numBody].colFriction = friction;
				        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
								+ new Vector2(1608 -wall_width.X + 48, wall_width.Y + boneSL_pos[0].Y*(1/scene_scale));
						sceneBodies[numBody].Position += new Vector2( 0, bone_joint1.Y*2);
						sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						boneSLIdx[boneSLCnt] = numBody++;
						boneSLCnt++;
					}
						
					//52左上
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[22], weight);
						sceneBodies[numBody].shapeIndex = 22;
						sceneBodies[numBody].colFriction = friction;
				        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
								+ new Vector2(1496 -wall_width.X + 48, wall_width.Y + boneSL_pos[0].Y*(1/scene_scale));
						sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint1.Y*2+bone_joint2.Y*2);
						sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
						sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						boneSLIdx[boneSLCnt] = numBody++;
						boneSLCnt++;
						colBuildIdx[colBuildCnt] = numBody-1;
						colBuildCnt++;					
					}
					//53右上
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[22], weight);
						sceneBodies[numBody].shapeIndex = 22;
						sceneBodies[numBody].colFriction = friction;
				        sceneBodies[numBody].Position = new Vector2( 0.0f , -wall_height.Y)
								+ new Vector2(1608 -wall_width.X + 48, wall_width.Y + boneSL_pos[0].Y*(1/scene_scale));
						sceneBodies[numBody].Position += new Vector2( 0, boneL_width.Y*2+bone_joint1.Y*2+bone_joint2.Y*2);
						sceneBodies[numBody].Position += new Vector2( 0, bottom_pos[2].Y*(1/scene_scale));
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
						boneSLIdx[boneSLCnt] = numBody++;
						boneSLCnt++;
						colBuildIdx[colBuildCnt] = numBody-1;
						colBuildCnt++;					
					}					
				}
			}
			
			// (52+2)-(55+2).クレーン
			int crane_cnt=0;
			{
				//54インデックス合わせるための数合わせ
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[24], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 24;
				sceneBodies[numBody].position = new Vector2(1195-wall_width.X+40/2, wall_height.Y+crane_set.Y*3);
				sceneBodies[numBody].position += new Vector2(0, (arm_width+1.0f)-(arm_width+1.0f)*2);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				//sceneBodies[numBody].friction+=0.1f;
				numBody++;
						
				//55 回転部
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[24], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 24;
				sceneBodies[numBody].position = new Vector2(1195-wall_width.X+40/2, wall_height.Y + crane_set.Y);
				sceneBodies[numBody].position += new Vector2(0, (arm_width+1.0f));
				sceneBodies[numBody].colFriction+=0.2f;
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].collisionFilter = 1 << 2;
					
				numBody++;

				//56長方形
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[26], 20.0f);
				sceneBodies[numBody].shapeIndex = 26;
				sceneBodies[numBody].position = new Vector2(1195-wall_width.X+40/2, wall_height.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;	
				sceneBodies[numBody].collisionFilter = 1 << 2;
					
					
				crane2Idx = numBody++;
									
				//　球とジョイント
				{
					PhysicsBody b1 = sceneBodies[numBody-2];
					PhysicsBody b2 = sceneBodies[numBody-1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b1.position), (uint)(numBody-2), (uint)(numBody-1));	
					sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
					sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-30.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(30.0f);
					numJoint++;
				}

				//57 アームと箱をジョイントしない
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[25], 10.0f);
				sceneBodies[numBody].shapeIndex = 25;
				sceneBodies[numBody].position = new Vector2(1195-wall_width.X+crane_parts1.X, wall_height.Y - 111);
				sceneBodies[numBody].position += new Vector2( 0.0f, - (arm_box.Y + 2.0f));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].SetBodyKinematic();//一時静態
				numBody++;
				craneIdx[crane_cnt]=numBody-1;
				crane_cnt++;
			}		
		}

		for(int j=0; j<numBody;j++){
			sceneBodies[j].picking = false; // 最終的に触れなくする
			sceneBodies[j].colFriction+=0.01f;
			sceneBodies[j].airFriction+=0.01f;
				
			// オブジェクトを回転しやすくする
			if(sceneBodies[j].IsDynamic())
			{
			//	sceneBodies[j].inertia *= 0.25f;
			//	sceneBodies[j].invInertia *= 4.0f;
			}
			else if(sceneBodies[j].IsKinematic())
			{
			//	sceneBodies[j].oldInertia *= 0.25f;
			//	sceneBodies[j].oldInvInertia *= 4.0f;			
			}
					
		}
			
		sceneBodies[throwObjIdx].inertia *= 0.25f;
		sceneBodies[throwObjIdx].invInertia *= 4.0f;			
		sceneBodies[throwObjIdx].oldInertia *= 0.25f;
		sceneBodies[throwObjIdx].oldInvInertia *= 4.0f;				
			
		sceneBodies[crane2Idx].inertia *= 0.25f;
		sceneBodies[crane2Idx].invInertia *= 4.0f;			
		sceneBodies[crane2Idx].oldInertia *= 0.25f;
		sceneBodies[crane2Idx].oldInvInertia *= 4.0f;				
			
		colFlag = false;//形鋼が衝突したかどうか
		isCrane_st1 = false;//クレーン箱が衝突したかどうか
		isTruckCollision = false;
		
		// 2Dレイアウトなど読み込み
		CreateObject();
	}
	
	/// 2D素材の生成
	public override void CreateObject()
	{
		var image = Resource2d.GetInstance().ImageStage00;
		objectManager = new TargetManager();
		int priority_offset=0;

		objectManager.Regist(new PhysicsObject("Sign2", image, 
				0, 0, 100, 100, 321, 287, priority_offset));//看板
		objectManager.FindTarget("Sign2").action.SetCenter("Sign2",50, 50);//2DのCenter設定
		priority_offset++;			

		objectManager.Regist(new PhysicsObject("Sign3", image, 
				102, 0, 112, 16, 315, 387, priority_offset));//その他1
		objectManager.FindTarget("Sign3").action.SetCenter("Sign3",56, 8*2+24);//2DのCenter設定 接合しているのでCenter補正
		priority_offset++;
		
		objectManager.Regist(new PhysicsObject("Sign4", image, 
				102, 0, 112, 16, 315, 411, priority_offset));//その他2
		objectManager.FindTarget("Sign4").action.SetCenter("Sign4", 56, 8);//2DのCenter設定
		priority_offset++;		
		
		objectManager.Regist(new PhysicsObject("Sign1", image, 
				0, 102, 56, 104, 343, 359, priority_offset));//支柱
		objectManager.FindTarget("Sign1").action.SetCenter("Sign1", 28, 52+16);//2DのCenter設定
		priority_offset++;
			
		for(int i=0; i<2; i++)	
		{
			name = "Cone"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				242, 0, 72, 106, 479, 357, priority_offset));//三角コーン
			objectManager.FindTarget(name).action.SetCenter(name, 36, 53);//2DのCenter設定
			priority_offset++;
		}
		
		objectManager.Regist(new Truck("Truck"));//トラック
		
		for(int i=0; i<boxCnt; i++)	
		{
			name = "Sack"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				180, 325, 72, 64,1231, 335, priority_offset));//資材
			objectManager.FindTarget(name).action.SetCenter(name, 72/2, 64/2);//2DのCenter設定
			priority_offset++;
		}
			
		for(int i=0; i<6; i++)	
		{
			name = "boneH"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				316, 0, 32, 176, 1472, -8, priority_offset));//縦形鋼
			objectManager.FindTarget(name).action.SetCenter(name, 32/2, 176/2);//2DのCenter設定
			priority_offset++;
		}

		for(int i=0; i<6; i++)	
		{
			name = "boneW"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				350, 0, 64, 32, 1512, -48, priority_offset));//横形鋼
			objectManager.FindTarget(name).action.SetCenter(name, 64/2, 32/2);//2DのCenter設定
			priority_offset++;
		}

		for(int i=0; i<2; i++)	
		{
			name = "boneJ"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				350,  34, 40, 40, 1472, -48, priority_offset));//接合部
			objectManager.FindTarget(name).action.SetCenter(name, 40/2, 40/2);//2DのCenter設定
			priority_offset++;
		}

		objectManager.Regist(new PhysicsObject("boneJ3", image, 
				350,  76, 40, 48, 1472, 168, priority_offset));//接合部
		objectManager.FindTarget("boneJ3").action.SetCenter("boneJ3", 40/2, 48/2);//2DのCenter設定				
		priority_offset++;

		for(int i=0; i<2; i++){
			name = "boneJ"+(i+4);
			objectManager.Regist(new PhysicsObject(name, image, 
				350, 168, 48, 48, 1576, 168, priority_offset));//接合部
			objectManager.FindTarget(name).action.SetCenter(name, 48/2, 48/2);//2DのCenter設定				
			priority_offset++;
		}

		for(int i=0; i<4; i++){
			name = "boneJ"+(i+6);
			objectManager.Regist(new PhysicsObject(name, image, 
				350, 126, 48, 40, 1576, -48, priority_offset));//接合部
			objectManager.FindTarget(name).action.SetCenter(name, 48/2, 40/2);//2DのCenter設定				
			priority_offset++;
		}

		for(int i=0; i<boneSLCnt; i++){
			name = "boneSL"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				416, 0, 96/2+10, 192/2+10, 1496, -16, priority_offset));//斜め横形鋼
			objectManager.FindTarget(name).action.SetCenter(name, 96/2, 192/2);//2DのCenter設定				
			priority_offset++;
			name = "boneSL2"+(i+1);
			objectManager.Regist(new PhysicsObject(name, image, 
				416+96/2-10, 192/2-10, 96/2+10, 192/2+10, 1496, -16, priority_offset));//斜め横形鋼
			objectManager.FindTarget(name).action.SetCenter(name, 10, 10);//2DのCenter設定				
			priority_offset++;
		}
			
		objectManager.Regist(new PhysicsObject("Rope", image, 
				102, 36, 62, 82, 1184, 95, priority_offset));//ロープ
		objectManager.FindTarget("Rope").action.SetCenter("Rope", 62/2, 82/2+18/2);//2DのCenter設定				
		priority_offset++;
		
		objectManager.Regist(new PhysicsObject("crane_sack", image, 
				166, 36, 72, 64, 1179, 111, priority_offset));//資材
		objectManager.FindTarget("crane_sack").action.SetCenter("crane_sack", 72/2, 64/2);//2DのCenter設定				
		priority_offset++;
			
		objectManager.Regist(new PhysicsObject("fook2", image, 
				58, 102, 40, 73, 427, 31, priority_offset));//フック
		objectManager.FindTarget("fook2").action.SetCenter("fook2", 40/2, -32);//2DのCenter設定				
		priority_offset++;
		objectManager.Regist(new PhysicsObject("fook2_wire", image, 
				216, 0, 24, 32, 1203, -1, priority_offset));//ワイヤー
		objectManager.FindTarget("fook2_wire").action.SetCenter("fook2_wire", 24/2, 0);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("wire", image, 
				216, 0, 24, 32, 1203, -1, priority_offset));//ワイヤー
		objectManager.FindTarget("wire").action.SetCenter("wire", 24/2, 24);//2DのCenter設定				
		priority_offset++;
		
		objectManager.Regist(new PhysicsObject("wire2", image, 
				216, 0, 24, 32, 1203, -1, priority_offset));//ワイヤー
		objectManager.FindTarget("wire2").action.SetCenter("wire2", 24/2, 24+24);//2DのCenter設定				
		priority_offset++;
		
		objectManager.Regist(new PhysicsObject("wire3", image, 
				216, 0, 24, 32, 1203, -1, priority_offset));//ワイヤー
		objectManager.FindTarget("wire3").action.SetCenter("wire3", 24/2, 24+24+24);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("wire4", image, 
				216, 0, 24, 32, 1203, -1, priority_offset));//ワイヤー
		objectManager.FindTarget("wire4").action.SetCenter("wire4", 24/2, 24+24+24+24);//2DのCenter設定				
		priority_offset++;
		
		image = Resource2d.GetInstance().ImageStage00_BGP;
		objectManager.Regist(new PhysicsObject("Bgp1", image, 
				0, 0, 256, 42, 241,  438, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp2", image, 
				0, 43, 256, 25, 497, 455, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp3", image, 
				0, 69, 256, 25, 753, 455, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp4", image, 
				0, 95, 256, 32, 1009,448, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp5", image, 
				0,128, 256, 28, 1265,452, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp6", image, 
				0,157, 187, 29, 1521,451, priority_offset));//
		priority_offset++;
			
		// 背景14分割
		image = Resource2d.GetInstance().ImageStage00_BG;
		objectManager.Regist(new PhysicsObject("Bg1", image, 
				0, 0, 122, 480, 0, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg2", image, 
				122, 0, 122, 480, 122, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg3", image, 
				244, 0, 122, 480, 244, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg4", image, 
				366, 0, 122, 480, 366, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg5", image, 
				488, 0, 122, 480, 488, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg6", image, 
				610, 0, 122, 480, 610, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg7", image, 
				732, 0, 122, 480, 732, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg8", image, 
				854, 0, 122, 480, 854, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg9", image, 
				976, 0, 122, 480, 976, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg10", image, 
				1098, 0, 122, 480, 1098, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg11", image, 
				1220, 0, 122, 480, 1220, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg12", image, 
				1342, 0, 122, 480, 1342, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg13", image, 
				1464, 0, 122, 480, 1464, 0, priority_offset));//背景
		objectManager.Regist(new PhysicsObject("Bg14", image, 
				1586, 0, 122, 480, 1586, 0, priority_offset));//背景	
	}
	
	/// 剛体との位置を合わせる
	public override void FittingPosition()
	{	
		if(objectManager == null )
			return;

		objectManager.FindTarget("Sign2").InitPos(854+(int)(sceneBodies[signIdx[0]].Position.X*(1/scene_scale)) - 50, 
				-((int)(sceneBodies[signIdx[0]].Position.Y*(1/scene_scale))-240 + 50));//看板	
		objectManager.FindTarget("Sign2").action.SetDegree(sceneBodies[signIdx[0]].Rotation);//物体の回転	
			
		objectManager.FindTarget("Sign3").InitPos(854+(int)(sceneBodies[signIdx[1]].Position.X*(1/scene_scale)) - (int)(sceneBodies[signIdx[1]].width.X*(1/scene_scale)), 
				-(30 + (int)(sceneBodies[signIdx[1]].Position.Y*(1/scene_scale))-240 + 8));//その他
		objectManager.FindTarget("Sign3").action.SetDegree(sceneBodies[signIdx[1]].Rotation);
			
		objectManager.FindTarget("Sign4").InitPos(854+(int)(sceneBodies[signIdx[2]].Position.X*(1/scene_scale)) - (int)(sceneBodies[signIdx[1]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[signIdx[2]].Position.Y*(1/scene_scale))-240 + 8));//その他
		objectManager.FindTarget("Sign4").action.SetDegree(sceneBodies[signIdx[2]].Rotation);
			
		objectManager.FindTarget("Sign1").InitPos(854+(int)(sceneBodies[signIdx[3]].Position.X*(1/scene_scale)) - 28, 
				-(-22+(int)(sceneBodies[signIdx[3]].Position.Y*(1/scene_scale))-240 + 88));//支柱
		objectManager.FindTarget("Sign1").action.SetDegree(sceneBodies[signIdx[3]].Rotation);
			
		objectManager.FindTarget("Cone1").InitPos(854+(int)(sceneBodies[coneIdx[0]].Position.X*(1/scene_scale)) - 36, 
				-((int)(sceneBodies[coneIdx[0]].Position.Y*(1/scene_scale))-240 + 53));//三角コーン
		objectManager.FindTarget("Cone1").action.SetDegree(sceneBodies[coneIdx[0]].Rotation);

		objectManager.FindTarget("Cone2").InitPos(854+(int)(sceneBodies[coneIdx[1]].Position.X*(1/scene_scale)) - 36, 
				-((int)(sceneBodies[coneIdx[1]].Position.Y*(1/scene_scale))-240 + 53));//三角コーン
		objectManager.FindTarget("Cone2").action.SetDegree(sceneBodies[coneIdx[1]].Rotation);

		for(int i=0; i<boxCnt; i++)
		{
			name= "Sack"+(i+1);
			if(i>4 && i<9)
			{//トラックに接合しているものは別処理
				if(i==5){
					objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boxIdx[i]].Position.X*(1/scene_scale)) - 55, 
						-((int)(sceneBodies[boxIdx[i]].Position.Y*(1/scene_scale))-240 + 32 + 26 ) + 22);//資材
					objectManager.FindTarget(name).action.SetDegree(sceneBodies[boxIdx[i]].Rotation);
				}else if(i==6){
					objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boxIdx[i]].Position.X*(1/scene_scale)) + 21, 
						-((int)(sceneBodies[boxIdx[i]].Position.Y*(1/scene_scale))-240 + 32 + 26 ) + 22);//資材
					objectManager.FindTarget(name).action.SetDegree(sceneBodies[boxIdx[i]].Rotation);
				}else if(i==7){
					objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boxIdx[i]].Position.X*(1/scene_scale)) + 97, 
						-((int)(sceneBodies[boxIdx[i]].Position.Y*(1/scene_scale))-240 + 32 + 26 ) + 22);//資材
					objectManager.FindTarget(name).action.SetDegree(sceneBodies[boxIdx[i]].Rotation);
				}else if(i==8){
					objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boxIdx[i]].Position.X*(1/scene_scale)) + 170, 
						-((int)(sceneBodies[boxIdx[i]].Position.Y*(1/scene_scale))-240 + 32 + 26) + 22);//資材
					objectManager.FindTarget(name).action.SetDegree(sceneBodies[boxIdx[i]].Rotation);
				}
			}
			else
			{
				objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boxIdx[i]].Position.X*(1/scene_scale)) - 36, 
					-((int)(sceneBodies[boxIdx[i]].Position.Y*(1/scene_scale))-240 + 32));//資材
				objectManager.FindTarget(name).action.SetDegree(sceneBodies[boxIdx[i]].Rotation);
			}
		}
			
		for(int i=0; i<6; i++)
		{
			name= "boneH"+(i+1);
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boneLIdx[i]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneLIdx[i]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneLIdx[i]].Position.Y*(1/scene_scale))-240 + 88));//縦形鋼
			if(i==0 || i==3 || i==4)	
				objectManager.FindTarget(name).action.SetDegree(sceneBodies[boneLIdx[i]].Rotation);
		}	
		for(int i=0; i<6; i++)
		{
			name= "boneW"+(i+1);
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boneSIdx[i]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneSIdx[i]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneSIdx[i]].Position.Y*(1/scene_scale))-240 + 16));//横形鋼
			if(i==0 || i==2 || i==3)	
				objectManager.FindTarget(name).action.SetDegree(sceneBodies[boneSIdx[i]].Rotation);
		}	

		for(int i=0; i<boneSLCnt; i++)
		{
			name= "boneSL"+(i+1);
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boneSLIdx[i]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneSLIdx[i]].width.X*(1/scene_scale)) -6, 
				-((int)(sceneBodies[boneSLIdx[i]].Position.Y*(1/scene_scale))-240 + 96));//斜め形鋼
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[boneSLIdx[i]].Rotation);
			name= "boneSL2"+(i+1);
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[boneSLIdx[i]].Position.X*(1/scene_scale)) -8, 
				-((int)(sceneBodies[boneSLIdx[i]].Position.Y*(1/scene_scale))-240 +9));//斜め形鋼
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[boneSLIdx[i]].Rotation);
		}
			
		objectManager.FindTarget("boneJ2").InitPos(854+(int)(sceneBodies[boneJIdx[0]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[0]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[0]].Position.Y*(1/scene_scale))-240 + 20));//左下接合部

		objectManager.FindTarget("boneJ8").InitPos(854+(int)(sceneBodies[boneJIdx[1]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[1]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[1]].Position.Y*(1/scene_scale))-240 + 20));//中央下接合部

		objectManager.FindTarget("boneJ9").InitPos(854+(int)(sceneBodies[boneJIdx[2]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[2]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[2]].Position.Y*(1/scene_scale))-240 + 20));//右下接合部

		objectManager.FindTarget("boneJ3").InitPos(854+(int)(sceneBodies[boneJIdx[3]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[3]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[3]].Position.Y*(1/scene_scale))-240 + 24));//中央左接合部
		objectManager.FindTarget("boneJ3").action.SetDegree(sceneBodies[boneJIdx[3]].Rotation);

		objectManager.FindTarget("boneJ4").InitPos(854+(int)(sceneBodies[boneJIdx[4]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[4]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[4]].Position.Y*(1/scene_scale))-240 + 24));//中央接合部

		objectManager.FindTarget("boneJ5").InitPos(854+(int)(sceneBodies[boneJIdx[5]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[5]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[5]].Position.Y*(1/scene_scale))-240 + 24));//中央右接合部

		objectManager.FindTarget("boneJ1").InitPos(854+(int)(sceneBodies[boneJIdx[6]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[6]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[6]].Position.Y*(1/scene_scale))-240 + 20));//上部左接合部
		objectManager.FindTarget("boneJ1").action.SetDegree(sceneBodies[boneJIdx[6]].Rotation);

		objectManager.FindTarget("boneJ6").InitPos(854+(int)(sceneBodies[boneJIdx[7]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[7]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[7]].Position.Y*(1/scene_scale))-240 + 20));//上部中央接合部
		objectManager.FindTarget("boneJ6").action.SetDegree(sceneBodies[boneJIdx[7]].Rotation);

		objectManager.FindTarget("boneJ7").InitPos(854+(int)(sceneBodies[boneJIdx[8]].Position.X*(1/scene_scale)) - (int)(sceneBodies[boneJIdx[8]].width.X*(1/scene_scale)), 
				-((int)(sceneBodies[boneJIdx[8]].Position.Y*(1/scene_scale))-240 + 22));//上部右接合部
		
		objectManager.FindTarget("Truck").InitPos(854+(int)(sceneBodies[truckIdx].Position.X*(1/scene_scale)) - 252, 
				-((int)(sceneBodies[truckIdx].Position.Y*(1/scene_scale)) - 240 + 14 -30));//トラック

		//クレーン修正版
		objectManager.FindTarget("wire").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24));//ワイヤー
		objectManager.FindTarget("wire").action.SetDegree(sceneBodies[crane2Idx].Rotation);

		objectManager.FindTarget("wire2").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24 + 24));//ワイヤー
		objectManager.FindTarget("wire2").action.SetDegree(sceneBodies[crane2Idx].Rotation);

		objectManager.FindTarget("wire3").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24 + 24 + 24));//ワイヤー
		objectManager.FindTarget("wire3").action.SetDegree(sceneBodies[crane2Idx].Rotation);

		objectManager.FindTarget("wire4").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24 + 24 + 24 + 24));//ワイヤー
		objectManager.FindTarget("wire4").action.SetDegree(sceneBodies[crane2Idx].Rotation);

		objectManager.FindTarget("fook2").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 40/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 )+32);//フック
		objectManager.FindTarget("fook2").action.SetDegree(sceneBodies[crane2Idx].Rotation);
		objectManager.FindTarget("fook2_wire").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240));//ワイヤー
		objectManager.FindTarget("fook2_wire").action.SetDegree(sceneBodies[crane2Idx].Rotation);
						
		objectManager.FindTarget("crane_sack").InitPos(854+(int)(sceneBodies[craneIdx[0]].Position.X*(1/scene_scale)) - 72/2, 
				-((int)(sceneBodies[craneIdx[0]].Position.Y*(1/scene_scale))-240 + 64/2));//資材
		objectManager.FindTarget("crane_sack").action.SetDegree(sceneBodies[craneIdx[0]].Rotation);

		objectManager.FindTarget("Rope").InitPos(854+(int)(sceneBodies[craneIdx[0]].Position.X*(1/scene_scale)) - 62/2, 
				-((int)(sceneBodies[craneIdx[0]].Position.Y*(1/scene_scale))-240 + 82/2 + 18/2));//ロープ (資材との位置補正)
		objectManager.FindTarget("Rope").action.SetDegree(sceneBodies[craneIdx[0]].Rotation);		
	}
	
    /// シミュレーション後のアクション定義
    /// この時点で衝突点がわかっているので、衝突点にかかわる処理
    /// 速度の修正はここで。位置の修正はUpdateFuncBeforeSim()に戻ってから行うこと
	public override void UpdateFuncAfterSim ()
	{
		if(objectManager != null)
			objectManager.Update();	

		FittingPosition();

		if(!isTruckCollision){
			if (QueryContact((uint)throwObjIdx, (uint)(truckIdx-1))){
				AudioManager.PlaySound("S25");
				isTruckCollision = true;
			}
		}

		//クレーンの箱解除
		if(!isCrane_st1){
			for(int i=2; i<numBody; i++){
				if(!isCrane_st1)
				{
					if (QueryContact((uint)craneIdx[0], (uint)i)){
						if(i != 56){
		               		sceneBodies[craneIdx[0]].BackToDynamic();
							isCrane_st1 = true;
						}
					}else if (QueryContact((uint)crane2Idx, (uint)i)){
						if(i != 57){
		            	   	sceneBodies[craneIdx[0]].BackToDynamic();
							isCrane_st1 = true;
						}
					}else if (QueryContact((uint)(crane2Idx-1), (uint)i)){
		               	sceneBodies[craneIdx[0]].BackToDynamic();
						isCrane_st1 = true;
					}else if (QueryContact((uint)(crane2Idx-2), (uint)i)){
		               	sceneBodies[craneIdx[0]].BackToDynamic();
						isCrane_st1 = true;
		            }
				}
			}
		}

		if(!colFlag){
			for(int i=0; i<colObjCnt; i++){
				for(int j=0; j<colBuildCnt; j++){						
					if (QueryContact((uint)colObjIdx[i], (uint)colBuildIdx[j])){
						float len = sceneBodies[colObjIdx[i]].Velocity.Length();
						if(len > 200.0f*scene_scale){
							//Console.WriteLine("over borderline for buildings ");
						
							//一部静態剛体のままとする
		                	sceneBodies[boneSIdx[0]].BackToDynamic();//中央左
		                	sceneBodies[boneSIdx[2]].BackToDynamic();//上部左
		                	sceneBodies[boneSIdx[3]].BackToDynamic();//上部右
		                	sceneBodies[boneLIdx[0]].BackToDynamic();//左下
		                	sceneBodies[boneLIdx[3]].BackToDynamic();//左上
		                	sceneBodies[boneLIdx[4]].BackToDynamic();//中央上
		                	sceneBodies[boneSLIdx[0]].BackToDynamic();//左下
		                	sceneBodies[boneSLIdx[2]].BackToDynamic();//左上
		                	sceneBodies[boneSLIdx[3]].BackToDynamic();//右上
		                	sceneBodies[boneJIdx[3]].BackToDynamic();//中央左
		                	sceneBodies[boneJIdx[6]].BackToDynamic();//左上
		                	sceneBodies[boneJIdx[7]].BackToDynamic();//中央上						
							colFlag = true;
						}

	            	}
				}
			}	
		}
	}
	
	/// クレーンまたは静体のものは静態にしない
	public override void SetsceneBodiesKinematic(int idx)
	{	
		if(idx < 3 || (idx > 15 && idx < 22 ) || idx == 30 || idx == 33 ||
			idx == 34 || idx == 36 || idx == 36 || idx == 37 ||
			(idx > 39 && idx < 44 ) || idx == 45 || idx == 46 || idx == 49 || idx == 51 ||
			idx == 54 || idx == 55 || idx == 56)
		{
		}
		else
		{
//			sceneBodies[idx].SetBodyKinematic();
			sceneBodies[idx].mass = PhysicsUtility.FltMax;
			sceneBodies[idx].invMass = 0;
			sceneBodies[idx].velocity = new Vector2(0, 0);
            sceneBodies[idx].angularVelocity = 0.0f;
			sceneBodies[idx].sleep= true;
		}
	}

}

} // Physics2dDemo
