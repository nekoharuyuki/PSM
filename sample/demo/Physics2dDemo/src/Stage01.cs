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

///
/// Stage01クラス
/// シミュレーションシーンを作るにはPhysicsSceneを継承する
///
public class Stage01 : Stage 
{	
	private int[] cementIdx   = new int[7]; // セメントのsceneBodiesインデックス
	private int   cementCnt   = 0;
	private int[] gravelIdx   = new int[10];// 砂利のsceneBodiesインデックス
	private int   gravelCnt   = 0;
	private int[] concreteIdx = new int[10];// コンクリートのsceneBodiesインデックス
	private int   concreteCnt = 0;
	private int   helmetIdx   = 0;          // ヘルメットのsceneBodiesインデックス
	private int[] drumIdx     = new int[3]; // ドラム缶のsceneBodiesインデックス
	private int   drumCnt     = 0;	
	private int[] shovelIdx   = new int[7]; // ショベルのsceneBodiesインデックス
	private int   shovelCnt   = 0;
	private bool  isShovelCollision = false;

	/// コンストラクタ
	public Stage01(){
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
	///
	public override void InitScene()
    {
		// 空のシミュレーションシーンを作成
        base.InitScene();

		scene_scale = 0.04f;
		
		// 反発係数を設定
		restitutionCoeff = 0.2f;

		// Box Shape設定 PhysicsShape( "width", "height" )
		// 必要な形状を全て設定
		// sceneShapes[]の中の数字はObjectId
        Vector2 wall_width = new Vector2(854, 16); //地面
        Vector2 wall_width2 = new Vector2(854, 16+50); //地面補正
        sceneShapes[0] = new PhysicsShape(wall_width2*scene_scale);

        Vector2 wall_height = new Vector2(1, 240); //左壁
        sceneShapes[1] = new PhysicsShape(wall_height*scene_scale);		
	
		Vector2 cement_width = new Vector2(44.0f, 14.0f);// セメント
		sceneShapes[2] = new PhysicsShape(cement_width*scene_scale);
			        
		Vector2 gravel_width = new Vector2(11.0f, 9.0f); // 砂利
		sceneShapes[3] = new PhysicsShape(gravel_width*scene_scale);
		
		Vector2 concrete_width = new Vector2(48.0f, 20.0f); // コンクリートブロック
		sceneShapes[4] = new PhysicsShape(concrete_width*scene_scale);

		Vector2 drum_width = new Vector2(48.0f, 72.0f); // ドラム缶
		sceneShapes[5] = new PhysicsShape(drum_width*scene_scale);

		Vector2[] helmet_pos = new Vector2[4]; // ヘルメット
		helmet_pos[0] = new Vector2(-23.5f,-14.0f)*scene_scale;
		helmet_pos[1] = new Vector2( 23.5f,-14.0f)*scene_scale;
		helmet_pos[2] = new Vector2( 23.5f, 14.0f)*scene_scale;
		helmet_pos[3] = new Vector2( -8.5f, 14.0f)*scene_scale;
		sceneShapes[6] = new PhysicsShape(helmet_pos, 4);
			

		Vector2[] shovel_pos = new Vector2[4]; // ショベル（本体）
		shovel_pos[0] = new Vector2(-164.5f,-109.0f)*scene_scale;
		shovel_pos[1] = new Vector2( 184.5f,-109.0f)*scene_scale;
		shovel_pos[2] = new Vector2( 184.5f, 109.0f)*scene_scale;
		shovel_pos[3] = new Vector2(-134.5f, 109.0f)*scene_scale;
		sceneShapes[7] = new PhysicsShape(shovel_pos, 4);
			
		float wheel_width = 25.0f-8.0f+4.0f;
		sceneShapes[8] = new PhysicsShape(wheel_width*scene_scale);// ショベル（アーム１回転部）

		Vector2[] arm1_pos = new Vector2[4]; // ショベル（アーム１）
		arm1_pos[0] = new Vector2( 25.5f-0.5f, 24f)*scene_scale;
		arm1_pos[1] = new Vector2(-35.5f+10.5f, 44f)*scene_scale;
		arm1_pos[2] = new Vector2(-35.5f+3.5f,-264f+10.0f)*scene_scale;
		arm1_pos[3] = new Vector2(-35.5f+3.5f+35.0f,-264f+10.0f-20.0f)*scene_scale;
		sceneShapes[9] = new PhysicsShape(arm1_pos, 4);
			
		Vector2[] arm3_pos = new Vector2[4]; // ショベル（アーム３）
		arm3_pos[0] = new Vector2(-62.0f, 68.0f)*scene_scale;
		arm3_pos[1] = new Vector2(-62.0f, 38.0f)*scene_scale;
		arm3_pos[2] = new Vector2( 102.0f,-68.0f)*scene_scale;
		arm3_pos[3] = new Vector2( 102.0f,  0.0f)*scene_scale;
		sceneShapes[10] = new PhysicsShape(arm3_pos, 4);
			
		Vector2[] arm4_pos = new Vector2[4]; // ショベル（アーム４）
		arm4_pos[0] = new Vector2(-40.0f, 54.0f)*scene_scale;
		arm4_pos[1] = new Vector2(-40.0f, 0.0f)*scene_scale;
		arm4_pos[2] = new Vector2( 40.0f,-54f)*scene_scale;
		arm4_pos[3] = new Vector2( 40.0f, 40f)*scene_scale;
		sceneShapes[11] = new PhysicsShape(arm4_pos, 4);
			
		Vector2[] arm5_pos = new Vector2[5]; // ショベル（アーム５）
		arm5_pos[0] = new Vector2( -50.0f, 89.5f)*scene_scale;
		arm5_pos[1] = new Vector2( -50.0f,-19.0f)*scene_scale;
		arm5_pos[2] = new Vector2(   0.0f,-89.5f)*scene_scale;
		arm5_pos[3] = new Vector2(  50.0f,-89.5f)*scene_scale;
		arm5_pos[4] = new Vector2(  10.0f, 39.5f)*scene_scale;
		sceneShapes[12] = new PhysicsShape(arm5_pos, 5);

		Vector2 caterpillar_width = new Vector2(187.0f, 48.5f); // ショベル（キャタピラー）
		sceneShapes[13] = new PhysicsShape(caterpillar_width*scene_scale);

		Vector2 enemy_width = new Vector2(70.0f, 25.0f);// 作業員
		Vector2[] enemy_pos = new Vector2[4];
		enemy_pos[0] = new Vector2(-80.0f, 0.0f)*scene_scale;
		enemy_pos[1] = new Vector2( -35.0f,-25.0f)*scene_scale;
		enemy_pos[2] = new Vector2( 50.0f,-25.0f)*scene_scale;
		enemy_pos[3] = new Vector2( 70.0f, 15.0f)*scene_scale;
		sceneShapes[14] = new PhysicsShape(enemy_pos, 4);
			
		Vector2 sand1_width = new Vector2(75.0f, 1.5f);// 砂利の土台用
		sceneShapes[15] = new PhysicsShape(sand1_width*scene_scale);
		Vector2 sand2_width = new Vector2(55.0f, 5.0f);// 砂利の土台用
		sceneShapes[16] = new PhysicsShape(sand2_width*scene_scale);
		Vector2 sand3_width = new Vector2(35.0f, 5.0f);// 砂利の土台用
		sceneShapes[17] = new PhysicsShape(sand3_width*scene_scale);
		Vector2 sand4_width = new Vector2(11.0f, 2.5f);// 砂利の土台用
		sceneShapes[18] = new PhysicsShape(sand4_width*scene_scale);

		numShape = 19;
		
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
		    sceneBodies[numBody].Position = new Vector2(0, wall_height.Y);
		    sceneBodies[numBody].Position += new Vector2(0, wall_width2.Y + 30.0f );	
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 0;
			sceneBodies[numBody].collisionFilter = 1<<2;
            numBody++;
				
			// 2-5.砂利の土台用
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[15], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-wall_width.X+1281, -wall_height.Y + wall_width.Y + sand1_width.Y);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 15;
            numBody++;
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-wall_width.X+1281, -wall_height.Y + wall_width.Y + sand1_width.Y*2+sand2_width.Y);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 16;
            numBody++;
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-wall_width.X+1281, -wall_height.Y + wall_width.Y + sand1_width.Y*2+sand2_width.Y*2+sand3_width.Y);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 17;
			numBody++;
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[18], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-wall_width.X+1281, -wall_height.Y + wall_width.Y + sand1_width.Y*2+sand2_width.Y*2+sand3_width.Y*2+sand4_width.Y);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 18;
			numBody++;

        }
				
		// 動的な剛体でスタックの生成
		{
			//6.作業員
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[14], 10.0f);
	        sceneBodies[numBody].Position = new Vector2(0, -wall_height.Y) + new Vector2(0, wall_width.Y + enemy_width.Y + 0.005f) + new Vector2(0f, (0.005f) * enemy_width.Y);
	        sceneBodies[numBody].Position += new Vector2(-316-427, 10.0f);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].shapeIndex = 14;
			sceneBodies[numBody].Rotation = PhysicsUtility.GetRadian(30.0f);
			sceneBodies[numBody].SetBodyKinematic();	
			throwObjIdx = numBody++;

			// 7-13.セメント
			{
				cementCnt = 0;
				float friction = 0.05f;
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 5.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 443, -wall_height.Y);
		        sceneBodies[numBody].Position += new Vector2( cement_width.X, wall_width.Y+cement_width.Y*7);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].colFriction = friction;
	            sceneBodies[numBody].shapeIndex = 2;
				cementIdx[cementCnt] = numBody++;
				cementCnt++;

				for (int i = 0; i < 3; i++)
            	{
					for (int j = 0; j < 2; j++)
            		{
	                	sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 5.0f);
	                	sceneBodies[numBody].Position = new Vector2(383 -854 + cement_width.X , -wall_height.Y)// 開始位置
								+ new Vector2( 0, wall_width.Y + cement_width.Y) + new Vector2( 0, 2 * i * cement_width.Y);// 高さ
	                	sceneBodies[numBody].Position += new Vector2((cement_width.X * 2) * j, 0);
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	                	sceneBodies[numBody].shapeIndex = 2;
						sceneBodies[numBody].colFriction = friction;
						cementIdx[cementCnt] = numBody++;
						cementCnt++;
					}	
				}
			}
			
			// 14-22.砂利
			{
				float friction = 0.3f;
				gravelCnt=0;//カウント用

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1206.0f + gravel_width.X-3, -240+wall_width.Y + gravel_width.Y+3);//位置補正
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1224.0f + gravel_width.X, -240+wall_width.Y + gravel_width.Y+13);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1235.0f + gravel_width.X-11, -240+wall_width.Y + gravel_width.Y+gravel_width.Y*2+13);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1255.0f + gravel_width.X-9, -240+wall_width.Y + gravel_width.Y+23);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1269.0f + gravel_width.X, -240+wall_width.Y + gravel_width.Y+28);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1288.0f + gravel_width.X+4, -240+wall_width.Y + gravel_width.Y+23);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1303.0f + gravel_width.X+13 , -240+wall_width.Y + gravel_width.Y+gravel_width.Y*2+13);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1314.0f + gravel_width.X+2, -240+wall_width.Y + gravel_width.Y+13);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
        	    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 3.0f);
		  		sceneBodies[numBody].Position = new Vector2(-854 + 1331.0f + gravel_width.X+6, -240+wall_width.Y + gravel_width.Y+3);//位置補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].colFriction = friction;
				gravelIdx[gravelCnt] = (int)numBody++;
				gravelCnt++;
			}
				
			// 23-32.コンクリートブロック
			{
				concreteCnt=0;//カウント用
				for (int i = 0; i < 5; i++)
            	{
					for (int j = 0; j < 2; j++)
            		{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
						sceneBodies[numBody].Position = new Vector2(-854 + 960 + concrete_width.X, -wall_height.Y)// 開始位置
								+ new Vector2( 0, wall_width.Y + concrete_width.Y) + new Vector2( 0, 2 * i * concrete_width.Y);// 高さ
	                	sceneBodies[numBody].Position += new Vector2((concrete_width.X * 2 ) * j, 0);
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	                	sceneBodies[numBody].shapeIndex = 4;
						sceneBodies[numBody].colFriction = 0.05f;
						concreteIdx[concreteCnt] = numBody++;
						concreteCnt++;
					}	
				}
			}
							
			// 33-35ドラム缶
			{
				float drum_weight = 10.0f;
					
				drumCnt = 0;
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], drum_weight);
		        sceneBodies[numBody].Position = new Vector2(-854 + 589, -(320 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 5;
				sceneBodies[numBody].colFriction = 0.05f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], drum_weight);
		        sceneBodies[numBody].Position = new Vector2(-854 + 692, -(320 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 5;
				sceneBodies[numBody].colFriction = 0.05f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], drum_weight);
				sceneBodies[numBody].Rotation = PhysicsUtility.GetRadian(90.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 931, -(368 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( -drum_width.Y, -drum_width.X);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 5;
				sceneBodies[numBody].colFriction = 0.05f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			}
				
			// 36.ヘルメット
			{
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 4.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 614, -(292 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 23.5f, -14.0f );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 6;
				sceneBodies[numBody].colFriction = 0.04f;
				helmetIdx = numBody++;
			}
				
			//37-43ショベル
			{
				shovelCnt=0;
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[13], PhysicsUtility.FltMax);//キャタピラー
		        sceneBodies[numBody].Position = new Vector2(-854 + 1496, -(367 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 187.0f, -48.5f );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 13;
				sceneBodies[numBody].colFriction = 0.0f; // 0.04f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);//本体
		        sceneBodies[numBody].Position = new Vector2(-854 + 1536, -(150 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 184.5f, -109.0f );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 7;
				sceneBodies[numBody].colFriction = 0.0f; // 0.04f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[12], PhysicsUtility.FltMax);//アーム５
		        sceneBodies[numBody].Position = new Vector2(-854 + 1564, -(86 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 50.0f, -89.0f+0.5f );//切れ目補正
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 12;
				sceneBodies[numBody].colFriction = 0.0f; // 0.04f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[11], PhysicsUtility.FltMax);//アーム４
		        sceneBodies[numBody].Position = new Vector2(-854 + 1484, -(67 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 40.0f, -54.0f );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 11;
				sceneBodies[numBody].colFriction = 0.0f; // 0.04f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], PhysicsUtility.FltMax);//アーム３
		        sceneBodies[numBody].Position = new Vector2(-854 + 1280, -(1 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 102.5f, -68.0f );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 10;
				sceneBodies[numBody].colFriction = 0.0f; // 0.04f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 20.0f);//アーム回転部
		        sceneBodies[numBody].Position = new Vector2(-854 + 1241, -(-wheel_width -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 35.5f, -wheel_width );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 8;
				sceneBodies[numBody].colFriction = 0.0f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
				//　壁にジョイント
				{
					PhysicsBody b1 = sceneBodies[0];
					PhysicsBody b2 = sceneBodies[numBody-1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.Position), (uint)0, (uint)(numBody-1));
					sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
					sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
					sceneJoints[numJoint].angleLim = 0;
					numJoint++;
				}
				
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], 20.0f);//アーム１
		        sceneBodies[numBody].Position = new Vector2(-854 + 1241, -(-17.0f -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 35.5f, -99.0f +20.0f);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 9;
				sceneBodies[numBody].colFriction = 0.00f;
				shovelIdx[shovelCnt] = numBody++;
				shovelCnt++;
					
				//球とジョイント
				{
					PhysicsBody b1 = sceneBodies[numBody-2];
					PhysicsBody b2 = sceneBodies[numBody-1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b1.position + b2.position)*0.5f, (uint)(numBody-2), (uint)(numBody-1));
					numJoint++;	
				}				
			}
		}				

		for(int j=1; j<numBody;j++)
		{
			sceneBodies[j].picking = false; // 最終的に触れなくする			
		}
			
		sceneBodies[throwObjIdx].inertia *= 0.50f;
		sceneBodies[throwObjIdx].invInertia *= 2.0f;			
		sceneBodies[throwObjIdx].oldInertia *= 0.50f;
		sceneBodies[throwObjIdx].oldInvInertia *= 2.0f;	
			
		sceneBodies[shovelIdx[6]].inertia *= 0.25f;
		sceneBodies[shovelIdx[6]].invInertia *= 4.0f;			
		sceneBodies[shovelIdx[6]].oldInertia *= 0.25f;
		sceneBodies[shovelIdx[6]].oldInvInertia *= 4.0f;	
			
		// 2Dレイアウトなど読み込み
		CreateObject();
	}
	
	/// 2D素材の生成
	public override void CreateObject()
	{
		var image = Resource2d.GetInstance().ImageStage01;
		objectManager = new TargetManager();
		int priority_offset=0;
			
		for(int i=0; i<cementCnt; i++)	
		{
			string name = "Cement"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
				126, 470, 96, 32, 443, 336, i));//セメント
			objectManager.FindTarget(name).action.SetCenter(name, 96/2, 32/2);//2DのCenter設定				
		}
		priority_offset = cementCnt;

		for(int i=0; i<concreteCnt; i++)	
		{
			string name = "Concrete"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
			2, 470, 96, 40, 942, 264, i+priority_offset));//コンクリート
			objectManager.FindTarget(name).action.SetCenter(name, 96/2, 40/2);//2DのCenter設定				
		}
		priority_offset += concreteCnt;

		for(int i=0; i<gravelCnt; i++)	
		{
			string name = "Gravel"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
			100, 470, 22, 20, 1247, 351, priority_offset));//砂利
			objectManager.FindTarget(name).action.SetCenter(name, 22/2, 20/2);//2DのCenter設定				
			priority_offset++;
		}
		
		objectManager.Regist(new PhysicsObject("Helmet", image, 
		223, 467, 57, 28, 614, 292, priority_offset));//ヘルメット
		objectManager.FindTarget("Helmet").action.SetCenter("Helmet", 57/2, 28/2);//2DのCenter設定				
		
		priority_offset++;
			
		for(int i=0; i<drumCnt; i++)	
		{
			string name = "Drum"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
			2, 2, 96, 144, 589, 320, i+priority_offset));//ドラム缶
			objectManager.FindTarget(name).action.SetCenter(name, 96/2, 144/2);//2DのCenter設定				
		}
		priority_offset += drumCnt;

		objectManager.Regist(new Shovel("Shovel"));//油圧

		objectManager.Regist(new PhysicsObject("Shovel4", image, 
		  2,148,369,218,1536,150, priority_offset));//本体
		objectManager.FindTarget("Shovel4").action.SetCenter("Shovel4", 369/2, 218/2);//2DのCenter設定				
		priority_offset++;
		
		objectManager.Regist(new PhysicsObject("Shovel1", image, 
		100,  2,204,136,1280,  1, priority_offset));//アーム３
		objectManager.FindTarget("Shovel1").action.SetCenter("Shovel1", 204/2, 136/2);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Shovel2", image, 
		304,  2, 80,107,1484, 67, priority_offset));//アーム４
		objectManager.FindTarget("Shovel2").action.SetCenter("Shovel2", 80/2, 108/2);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Shovel3", image, 
		384,  2,100,179,1564-1, 86, priority_offset));//アーム５
		objectManager.FindTarget("Shovel3").action.SetCenter("Shovel3", 100/2, 179/2);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Shovel5", image, 
		  2,368,374, 97,1496,367, priority_offset));//キャタピラ
		objectManager.FindTarget("Shovel5").action.SetCenter("Shovel5", 374/2, 97/2);//2DのCenter設定				
		priority_offset++;
			
		image = Resource2d.GetInstance().ImageStage01_BGP;
		objectManager.Regist(new PhysicsObject("Bgp1", image, 
				0, 4, 256, 20, 256,  460, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp2", image, 
				0, 31, 256, 25, 512, 455, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp3", image, 
				0, 63, 256, 25, 768, 455, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp4", image, 
				0, 95, 168, 33, 1024,447, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp5", image, 
				0,135, 168, 41, 1368,439, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp6", image, 
				0,183, 172, 33, 1536,447, priority_offset));//
		priority_offset++;
			
		// 背景14分割
		image = Resource2d.GetInstance().ImageStage01_BG;
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

		for(int i=0; i<cementCnt; i++){
			string name = "Cement"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[cementIdx[i]].Position.X*(1/scene_scale)) - 96/2, 
				-((int)(sceneBodies[cementIdx[i]].Position.Y*(1/scene_scale)) - 240 + 32/2));//セメント
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[cementIdx[i]].Rotation);	
		}

		for(int i=0; i<concreteCnt; i++){
			string name = "Concrete"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[concreteIdx[i]].Position.X*(1/scene_scale)) - 96/2, 
				-((int)(sceneBodies[concreteIdx[i]].Position.Y*(1/scene_scale)) - 240 + 40/2));//コンクリート
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[concreteIdx[i]].Rotation);	
		}
		
		for(int i=0; i<gravelCnt; i++){
			string name = "Gravel"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[gravelIdx[i]].Position.X*(1/scene_scale)) - 22/2, 
				-((int)(sceneBodies[gravelIdx[i]].Position.Y*(1/scene_scale)) - 240 + 20/2));//砂利
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[gravelIdx[i]].Rotation);	
		}

		objectManager.FindTarget("Helmet").InitPos(854+(int)(sceneBodies[helmetIdx].Position.X*(1/scene_scale)) - 57/2, 
				-((int)(sceneBodies[helmetIdx].Position.Y*(1/scene_scale)) - 240 + 28/2));//ヘルメット
		objectManager.FindTarget("Helmet").action.SetDegree(sceneBodies[helmetIdx].Rotation);

		for(int i=0; i<drumCnt; i++){
			string name = "Drum"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[drumIdx[i]].Position.X*(1/scene_scale)) - 96/2, 
				-((int)(sceneBodies[drumIdx[i]].Position.Y*(1/scene_scale)) - 240 + 144/2));//ドラム缶
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[drumIdx[i]].Rotation);	
		}

		objectManager.FindTarget("Shovel").InitPos(854+(int)(sceneBodies[shovelIdx[6]].Position.X*(1/scene_scale)) - 35, 
				-((int)(sceneBodies[shovelIdx[6]].Position.Y*(1/scene_scale)) - 240 + 94));//ショベル
		objectManager.FindTarget("Shovel").action.SetDegree(sceneBodies[shovelIdx[6]].Rotation);	

		objectManager.FindTarget("Shovel1").InitPos(854+(int)(sceneBodies[shovelIdx[4]].Position.X*(1/scene_scale)) - 204/2, 
				-((int)(sceneBodies[shovelIdx[4]].Position.Y*(1/scene_scale)) - 240 + 138/2));//アーム3
		objectManager.FindTarget("Shovel2").InitPos(854+(int)(sceneBodies[shovelIdx[3]].Position.X*(1/scene_scale)) - 78/2, 
				-((int)(sceneBodies[shovelIdx[3]].Position.Y*(1/scene_scale)) - 240 + 110/2));//アーム4 //切れ目補正
		objectManager.FindTarget("Shovel3").InitPos(854+(int)(sceneBodies[shovelIdx[2]].Position.X*(1/scene_scale)) - 100/2, 
				-((int)(sceneBodies[shovelIdx[2]].Position.Y*(1/scene_scale)) - 240 + 179/2));//アーム5
		objectManager.FindTarget("Shovel4").InitPos(854+(int)(sceneBodies[shovelIdx[1]].Position.X*(1/scene_scale)) - 369/2, 
				-((int)(sceneBodies[shovelIdx[1]].Position.Y*(1/scene_scale)) - 240 + 218/2));//本体
		objectManager.FindTarget("Shovel5").InitPos(854+(int)(sceneBodies[shovelIdx[0]].Position.X*(1/scene_scale)) - 374/2, 
				-((int)(sceneBodies[shovelIdx[0]].Position.Y*(1/scene_scale)) - 240 + 97/2));//キャタピラ			
	}

    /// シミュレーション後のアクション定義
    /// この時点で衝突点がわかっているので、衝突点にかかわる処理
    /// 速度の修正はここで。位置の修正はUpdateFuncBeforeSim()に戻ってから行うこと
	public override void UpdateFuncAfterSim ()
	{
		if(objectManager != null)
			objectManager.Update();

		if(!isShovelCollision){
			if (QueryContact((uint)throwObjIdx, (uint)shovelIdx[0]) ||
				QueryContact((uint)throwObjIdx, (uint)shovelIdx[1]) ||
				QueryContact((uint)throwObjIdx, (uint)shovelIdx[2]) ||
				QueryContact((uint)throwObjIdx, (uint)shovelIdx[3]) ||
				QueryContact((uint)throwObjIdx, (uint)shovelIdx[4])){
				AudioManager.PlaySound("S25");
				isShovelCollision = true;
			}
		}
			
		FittingPosition();
	}

	//ショベルと静体は除く
	public override void SetsceneBodiesKinematic(int idx)
	{	

		if(idx < 6 || (idx > 36 && idx < 44 )){		
		}else{
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
