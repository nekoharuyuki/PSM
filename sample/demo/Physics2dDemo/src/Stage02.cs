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

/// Stage01クラス
/// シミュレーションシーンを作るにはPhysicsSceneを継承する
public class Stage02 : Stage 
{
	private int     shovelIdx     = 0;          // シャベルのsceneBodiesインデックス
	private int[]   drumIdx       = new int[5]; // ドクロドラム缶のsceneBodiesインデックス
	private int     drumCnt       = 0;	
	private int     roofIdx       = 0;          // 屋根のsceneBodiesインデックス
	private int[]   brickBIdx     = new int[7]; // レンガ(大)のsceneBodiesインデックス
	private int     brickBCnt     = 0;
	private int[]   brickSIdx     = new int[7]; // レンガ(小)のsceneBodiesインデックス
	private int     brickSCnt     = 0;
	private int     foundationIdx = 0;          // 土台のsceneBodiesインデックス
	private int[]   craneIdx      = new int[3]; // クレーンのsceneBodiesインデックス
	private bool[]  colFlag       = new bool[4];//衝突判定フラグ 筋交4本
	private int[]   scaffoldIdx   = new int[4]; // 足場板のsceneBodiesインデックス
	private int     scaffoldCnt   = 0;
	private int[]   frameIdx      = new int[8]; // 建枠のsceneBodiesインデックス
	private int     frameCnt      = 0;
	private int[]   crossIdx      = new int[4]; // 筋交のsceneBodiesインデックス
	private int     crossCnt      = 0;
	private int     bucketIdx     = 0;          // バケツのsceneBodiesインデックス
	private int     bucketheadIdx = 0;          // 蓋のsceneBodiesインデックス
	private int     crane2Idx     = 0;           // 修正版クレーン（フック）のsceneBodiesインデックス	
	private string  name;

	/// コンストラクタ
	public Stage02(){
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
        Vector2 wall_width = new Vector2(854, 16); //地面
        sceneShapes[0] = new PhysicsShape(wall_width*scene_scale);

        Vector2 wall_height = new Vector2(1, 240); //左壁
        sceneShapes[1] = new PhysicsShape(wall_height*scene_scale);		
				        
		Vector2 gravel_width = new Vector2(11.0f, 10.0f); // 砂利
		sceneShapes[2] = new PhysicsShape(gravel_width*scene_scale);
		
		Vector2 shovel_width = new Vector2(20.0f, 80.0f-10.0f); // シャベル //補正
		sceneShapes[3] = new PhysicsShape(shovel_width*scene_scale);

		Vector2 drum_width = new Vector2(48.0f, 72.0f); // ドラム缶
		sceneShapes[4] = new PhysicsShape(drum_width*scene_scale);

		Vector2[] roof_pos = new Vector2[5]; // 屋根
		roof_pos[0] = new Vector2(     0, 32.0f)*scene_scale;
		roof_pos[1] = new Vector2(-48.0f, 16.0f)*scene_scale;
		roof_pos[2] = new Vector2(-48.0f,-32.0f)*scene_scale;
		roof_pos[3] = new Vector2( 48.0f,-32.0f)*scene_scale;
		roof_pos[4] = new Vector2( 48.0f, 16.0f)*scene_scale;
		sceneShapes[5] = new PhysicsShape(roof_pos, 5);
						
		Vector2 brickB_width = new Vector2(28.0f, 20.0f); // レンガ（大）
		sceneShapes[6] = new PhysicsShape(brickB_width*scene_scale);
			
		Vector2 brickS_width = new Vector2(12.0f, 20.0f); // レンガ（小）
		sceneShapes[7] = new PhysicsShape(brickS_width*scene_scale);
			
		Vector2 foundation_width = new Vector2(56.0f, 24.0f); // 土台
		sceneShapes[8] = new PhysicsShape(foundation_width*scene_scale);

		Vector2 scaffold_width = new Vector2(80.0f, 12.0f); // 足場板
		sceneShapes[9] = new PhysicsShape(scaffold_width*scene_scale);
						
		Vector2 frame_width = new Vector2(8.0f, 80.0f); // 建枠
		sceneShapes[10] = new PhysicsShape(frame_width*scene_scale);
			
		Vector2[] crossA_pos = new Vector2[5]; // 筋交 //想定より短く作成	
		crossA_pos[0] = new Vector2( -63.0f, 55.0f)*scene_scale;
		crossA_pos[1] = new Vector2(  55.0f,-55.0f)*scene_scale;	
		crossA_pos[2] = new Vector2(  63.0f,-55.0f)*scene_scale;
		crossA_pos[3] = new Vector2( -55.0f, 55.0f)*scene_scale;
		sceneShapes[11] = new PhysicsShape(crossA_pos, 4);

		float sphere_width = 12.0f;
		sceneShapes[12] = new PhysicsShape(sphere_width*scene_scale);//ワイヤーに使用
		
		Vector2 crane_parts1 = new Vector2(20.0f, 20.0f);// クレーンのパーツ1
		sceneShapes[13] = new PhysicsShape(crane_parts1*scene_scale);
						
		float arm_width = 14.0f;
		sceneShapes[14] = new PhysicsShape(arm_width*scene_scale);// クレーンアーム
			
		Vector2 arm_box = new Vector2(48.0f, 72.0f);// クレーンがつる箱
		sceneShapes[15] = new PhysicsShape(arm_box*scene_scale);

		Vector2 enemy_width = new Vector2(70.0f, 25.0f);// 作業員
		Vector2[] enemy_pos = new Vector2[4];
		enemy_pos[0] = new Vector2(-80.0f, 0.0f)*scene_scale;
		enemy_pos[1] = new Vector2( -35.0f,-25.0f)*scene_scale;
		enemy_pos[2] = new Vector2( 50.0f,-25.0f)*scene_scale;
		enemy_pos[3] = new Vector2( 70.0f, 15.0f)*scene_scale;
		sceneShapes[16] = new PhysicsShape(enemy_pos, 4);
			
		Vector2[] buckethead_pos = new Vector2[5];//バケツ蓋
		buckethead_pos[0] = new Vector2(  0.0f, 13.0f)*scene_scale;
		buckethead_pos[1] = new Vector2( -48.0f,  1.0f)*scene_scale;
		buckethead_pos[2] = new Vector2( -48.0f,-14.0f)*scene_scale;	
		buckethead_pos[3] = new Vector2(  48.0f,-14.0f)*scene_scale;
		buckethead_pos[4] = new Vector2(  48.0f,  1.0f)*scene_scale;
		sceneShapes[17] = new PhysicsShape(buckethead_pos, 5);
			
		Vector2[] bucket_pos = new Vector2[4];//バケツ
		bucket_pos[0] = new Vector2( -46.0f, 58.0f)*scene_scale;
		bucket_pos[1] = new Vector2( -38.0f,-58.0f)*scene_scale;	
		bucket_pos[2] = new Vector2(  38.0f,-58.0f)*scene_scale;
		bucket_pos[3] = new Vector2(  46.0f, 58.0f)*scene_scale;
		sceneShapes[18] = new PhysicsShape(bucket_pos, 4);

		Vector2[] crossB_pos = new Vector2[5]; // 対の筋交 //想定より短く作成	
		crossB_pos[0] = new Vector2( 63.0f, 55.0f)*scene_scale;
		crossB_pos[1] = new Vector2( 55.0f, 55.0f)*scene_scale;	
		crossB_pos[2] = new Vector2(-63.0f,-55.0f)*scene_scale;
		crossB_pos[3] = new Vector2(-55.0f,-55.0f)*scene_scale;
		sceneShapes[19] = new PhysicsShape(crossB_pos, 4);

        Vector2 wall_widthLeft = new Vector2(675, 16); //地面(穴空き,左)		
        Vector2 wall_widthLeft2 = new Vector2(675, 16+50); //地面(穴空き,左)補正	
        sceneShapes[20] = new PhysicsShape(wall_widthLeft2*scene_scale);
        Vector2 wall_widthRight = new Vector2(95, 16); //地面(穴空き,右)		
        Vector2 wall_widthRight2 = new Vector2(95, 16+50); //地面(穴空き,右)補正	
        sceneShapes[21] = new PhysicsShape(wall_widthRight2*scene_scale);

		Vector2 crane_set = new Vector2(20.0f, 105.0f);// クレーンset
		sceneShapes[22] = new PhysicsShape(crane_set*scene_scale);			
			
		numShape = 23;
		
		// 剛体の生成・設定
        // 壁(静的剛体）で動的剛体の挙動範囲を制限
        {
			// 0.底辺
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(0-854+wall_widthLeft.X, -wall_height.Y-50);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 20;
			sceneBodies[numBody].colFriction = 0.05f;
            numBody++;
			// 1.底辺
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(854-wall_widthRight.X, -wall_height.Y-50);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 21;
			sceneBodies[numBody].colFriction = 0.05f;
            numBody++;
								
			// 2.左辺
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-wall_width.X*3, 0);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].rotation = 0;
            sceneBodies[numBody].shapeIndex = 1;
            numBody++;
        }
				
		// 動的な剛体でスタックの生成
		{
			//3.作業員
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[16], 15.0f);
	        sceneBodies[numBody].Position = new Vector2(0, -wall_height.Y) + new Vector2(0, wall_width.Y + enemy_width.Y + 0.005f) + new Vector2(0f, (0.005f) * enemy_width.Y);
	        sceneBodies[numBody].Position += new Vector2(-316-427, 10.0f);
			sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
            sceneBodies[numBody].shapeIndex = 16;
			sceneBodies[numBody].Rotation = PhysicsUtility.GetRadian(30.0f);
			sceneBodies[numBody].SetBodyKinematic();	
			throwObjIdx = numBody++;
			
			// バケツ
			{
        	    // 4.蓋
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[17], 5.0f);
		  		sceneBodies[numBody].Position = new Vector2(551 -854 + 96/2, -(320-wall_height.Y)-28/2);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 17;
				bucketheadIdx = numBody++;
					
				// 5.バケツ
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[18], 15.0f);
		  		sceneBodies[numBody].Position = new Vector2(553.0f -854 + 92/2, -(348-wall_height.Y)- 116/2 );
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 18;
				sceneBodies[numBody].colFriction = 0.02f;
				bucketIdx = numBody++;
					
				// 6.シャベル
				{
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 622, -(318 - wall_height.Y));//被らないよう変更
			        sceneBodies[numBody].Position += new Vector2( 160.0f/2 - 5,  -160.0f/2 + 5 );
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
					sceneBodies[numBody].Rotation = PhysicsUtility.GetRadian(30.0f);//追記
		            sceneBodies[numBody].shapeIndex = 3;
					sceneBodies[numBody].colFriction = 0.02f;
					shovelIdx = numBody++;
				}
			}
			
			// 7-11.ドラム缶
			{
				drumCnt = 0;
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 20.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 893, -(176 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].colFriction = 0.3f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 20.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 997, -(176 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].colFriction = 0.3f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 20.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 825, -(320 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].colFriction = 0.3f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 20.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 929, -(320 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				sceneBodies[numBody].colFriction = 0.3f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 20.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 1041, -(320 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( drum_width.X, -drum_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].SetBodyKinematic();	//一時的に静体にする	
				sceneBodies[numBody].colFriction = 0.3f;
				drumIdx[drumCnt] = numBody++;
				drumCnt++;
			}
				
			// レンガ
			{
				//12.屋根
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 0.5f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 343, -(72 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( 48.0f, -32.0f);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 5;
				//sceneBodies[numBody].friction = 0.01f;
				roofIdx = numBody++;
					
				//13-26レンガ
				brickBCnt=0;//カウント用
				brickSCnt=0;//カウント用
				for (int i = 0; i < 7; i++)
            	{
					if(i%2 == 0)
            		{
	                	sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], 2.4f-i*0.2f);
						sceneBodies[numBody].Position = new Vector2(-854 + 351 + brickS_width.X, -wall_height.Y+wall_width.Y+foundation_width.Y*2+brickS_width.Y*2)// 開始位置
								+ new Vector2( 0, -brickS_width.Y) + new Vector2( 0, (2 * i) * brickS_width.Y);// 高さ
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						sceneBodies[numBody].shapeIndex = 7;
						//sceneBodies[numBody].sleep = true;
						brickSIdx[brickSCnt] = numBody++;
						brickSCnt++;
						
	                	sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 2.4f-i*0.2f);
						sceneBodies[numBody].Position = new Vector2(-854 + 351 + brickS_width.X*2 + brickB_width.X, -wall_height.Y+wall_width.Y+foundation_width.Y*2+brickS_width.Y*2)// 開始位置
								+ new Vector2( 0, -brickB_width.Y) + new Vector2( 0, (2 * i) * brickB_width.Y);// 高さ
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						sceneBodies[numBody].shapeIndex = 6;
						//sceneBodies[numBody].sleep = true;
						brickBIdx[brickBCnt] = numBody++;
						brickBCnt++;
					}
					else
					{
	                	sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], 2.4f-i*0.2f);
						sceneBodies[numBody].Position = new Vector2(-854 + 351 + brickB_width.X*2 + brickS_width.X, -wall_height.Y+wall_width.Y+foundation_width.Y*2+brickS_width.Y*2)// 開始位置
								+ new Vector2( 0, -brickS_width.Y) + new Vector2( 0, (2 * i) * brickS_width.Y);// 高さ
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						sceneBodies[numBody].shapeIndex = 7;
						//sceneBodies[numBody].sleep = true;
						brickSIdx[brickSCnt] = numBody++;
						brickSCnt++;
						
	                	sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 2.4f-i*0.2f);
						sceneBodies[numBody].Position = new Vector2(-854 + 351 + brickB_width.X, -wall_height.Y+wall_width.Y+foundation_width.Y*2+brickS_width.Y*2)// 開始位置
								+ new Vector2( 0, -brickB_width.Y) + new Vector2( 0, (2 * i) * brickB_width.Y);// 高さ
						sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
						sceneBodies[numBody].shapeIndex = 6;
						//sceneBodies[numBody].sleep = true;							
						brickBIdx[brickBCnt] = numBody++;
						brickBCnt++;
					}
				}
				
				//27.土台
	            sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 20.0f);
		        sceneBodies[numBody].Position = new Vector2(-854 + 335, -(416 -wall_height.Y));
		        sceneBodies[numBody].Position += new Vector2( foundation_width.X, -foundation_width.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
	            sceneBodies[numBody].shapeIndex = 8;
				sceneBodies[numBody].colFriction = 0.1f;
				sceneBodies[numBody].SetBodyKinematic();//一時静態
				foundationIdx = numBody++;
			}
						
			// クレーン
			int crane_cnt=0;
			{						
				//28　上辺				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 0;
				sceneBodies[numBody].position = new Vector2(0, wall_height.Y+crane_set.Y);
				sceneBodies[numBody].position += new Vector2(0, (arm_width+1.0f)-(arm_width+1.0f)*2);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].collisionFilter = 1 << 2;
				numBody++;
						
				//29 回転部
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[12], PhysicsUtility.FltMax);
				sceneBodies[numBody].shapeIndex = 12;
				sceneBodies[numBody].position = new Vector2(1422.0f-wall_width.X+40/2, wall_height.Y + crane_set.Y);
				sceneBodies[numBody].position += new Vector2(0, (arm_width+1.0f));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				sceneBodies[numBody].colFriction+=0.1f;
				sceneBodies[numBody].collisionFilter = 1 << 2;
				numBody++;

				//30長方形
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[22], 20.0f);
				sceneBodies[numBody].shapeIndex = 22;
				sceneBodies[numBody].position = new Vector2(1422.0f-wall_width.X+40/2, wall_height.Y);
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;					
				sceneBodies[numBody].collisionFilter = 1 << 2;
				crane2Idx = numBody++;
									
				//　球とジョイント
				{
					PhysicsBody b1 = sceneBodies[numBody-2];
					PhysicsBody b2 = sceneBodies[numBody-1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, sceneBodies[numBody-2].Position, (uint)(numBody-2), (uint)(numBody-1));
					sceneJoints[numJoint].angleLim = 1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-30.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(30.0f);
					numJoint++;
				}
					
				// 31.アームとドラム缶はジョイント（しない）
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[15], 20.0f);
				sceneBodies[numBody].shapeIndex = 15;
				sceneBodies[numBody].position = new Vector2(1422.0f-wall_width.X+40/2, wall_height.Y-112);
				sceneBodies[numBody].position += new Vector2( 0.0f, - ( arm_box.Y + 2.0f));
				sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
				//sceneBodies[numBody].colFriction += 0.04f;
				sceneBodies[numBody].SetBodyKinematic();//一時静態
				numBody++;
				craneIdx[crane_cnt]=numBody-1;
				stageToDrumIdx = craneIdx[crane_cnt];
				crane_cnt++;									
			}

			//32-47.足場
			{
				float friction = 0.01f;
				scaffoldCnt = 0;
				frameCnt    = 0;
				crossCnt    = 0;
				//左上
				{
					//足場板
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1178, -wall_height.Y+wall_width.Y+scaffold_width.Y*4+frame_width.Y*4);
			        sceneBodies[numBody].Position += new Vector2( scaffold_width.X, -scaffold_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 9;
					sceneBodies[numBody].colFriction = friction;
					scaffoldIdx[scaffoldCnt] = numBody++;
					scaffoldCnt++;
					//筋交（二つを接合）
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[11], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1179, -(127 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( 159/2, -135/2 );//高さの位置補正
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 11;
					sceneBodies[numBody].colFriction = friction;
					sceneBodies[numBody].SetBodyKinematic();//一時静態
					crossIdx[crossCnt] = numBody++;
					crossCnt++;

					//建枠
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1178, -(120 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1322, -(120 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
				}
				
				// 左下
				{
					//足場板
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1178, -wall_height.Y+wall_width.Y+scaffold_width.Y*2+frame_width.Y*2);
			        sceneBodies[numBody].Position += new Vector2( scaffold_width.X, -scaffold_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 9;
					sceneBodies[numBody].colFriction = friction;
					scaffoldIdx[scaffoldCnt] = numBody++;
					scaffoldCnt++;

					//筋交
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[19], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1179, -(310-wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( 159/2, -136/2);//高さの位置補正
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 19;
					sceneBodies[numBody].colFriction = friction;
					sceneBodies[numBody].SetBodyKinematic();//一時静態
					crossIdx[crossCnt] = numBody++;
					crossCnt++;

					//建枠
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1178, -wall_height.Y+wall_width.Y+frame_width.Y*2);
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1322, -(304 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
				}

				//右上
				{
					//足場板
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1532, -wall_height.Y+wall_width.Y+scaffold_width.Y*4+frame_width.Y*4);
			        sceneBodies[numBody].Position += new Vector2( scaffold_width.X, -scaffold_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 9;
					sceneBodies[numBody].colFriction = friction;
					scaffoldIdx[scaffoldCnt] = numBody++;
					scaffoldCnt++;

					//筋交（二つを接合）
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[11], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1533, -(127 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( 159/2, -135/2 );//高さの位置補正
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 11;
					sceneBodies[numBody].colFriction = friction;
					sceneBodies[numBody].SetBodyKinematic();//一時静態
					crossIdx[crossCnt] = numBody++;
					crossCnt++;

					//建枠
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1532, -(120 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1676, -(120 -wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
				}
					
				//右下
				{
					//足場板
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1532, -wall_height.Y+wall_width.Y+scaffold_width.Y*2+frame_width.Y*2);
			        sceneBodies[numBody].Position += new Vector2( scaffold_width.X, -scaffold_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 9;
					sceneBodies[numBody].colFriction = friction;
					scaffoldIdx[scaffoldCnt] = numBody++;
					scaffoldCnt++;

					//筋交
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[19], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1533, -(310-wall_height.Y));
			        sceneBodies[numBody].Position += new Vector2( 159/2, -136/2);//高さの位置補正
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 19;
					sceneBodies[numBody].colFriction = friction;
					sceneBodies[numBody].SetBodyKinematic();//一時静態
					crossIdx[crossCnt] = numBody++;
					crossCnt++;

					//建枠
		            sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1532, -wall_height.Y+wall_width.Y+frame_width.Y*2);
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[10], 5.0f);
			        sceneBodies[numBody].Position = new Vector2(-854 + 1676, -wall_height.Y+wall_width.Y+frame_width.Y*2);
			        sceneBodies[numBody].Position += new Vector2( frame_width.X, -frame_width.Y);
					sceneBodies[numBody].position = sceneBodies[numBody].position*scene_scale;
		            sceneBodies[numBody].shapeIndex = 10;
					sceneBodies[numBody].colFriction = friction;
					frameIdx[frameCnt] = numBody++;
					frameCnt++;
				}
			}
		}				

		for(int j=0; j<numBody;j++){
			sceneBodies[j].picking = false; // 最終的に触れなくする				
		}
		
		//sceneBodies[throwObjIdx].inertia *= 0.50f;
		//sceneBodies[throwObjIdx].invInertia *= 2.0f;			
		//sceneBodies[throwObjIdx].oldInertia *= 0.50f;
		//sceneBodies[throwObjIdx].oldInvInertia *= 2.0f;			
			
		colFlag[0] = false;
		colFlag[1] = false;
		colFlag[2] = false;
		colFlag[3] = false;
		isCrane_st3 = false;//ドラム缶落下フラグ
			
		// 2Dレイアウトなど読み込み
		CreateObject();
	}
	
	/// 2D素材の生成
	public override void CreateObject()
	{
		var image = Resource2d.GetInstance().ImageStage02;
		objectManager = new TargetManager();
		int priority_offset=0;
			
		objectManager.Regist(new PhysicsObject("BucketHead", image, 
		160, 176, 96, 28, 551, 320, priority_offset));//バケツ蓋
		objectManager.FindTarget("BucketHead").action.SetCenter("BucketHead", 96/2, 28/2);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Bucket", image, 
		162, 208, 92, 116, 553, 348, priority_offset));//バケツ
		objectManager.FindTarget("Bucket").action.SetCenter("Bucket", 92/2, 116/2);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Shovel", image, 
		2, 174, 40, 160, 622, 318, priority_offset));//シャベル
		objectManager.FindTarget("Shovel").action.SetCenter("Shovel", 40/2, 160/2);//2DのCenter設定						
		priority_offset++;

		for(int i=0; i<drumCnt; i++){
			name = "Drum"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
			2, 2, 96, 144, 893, 176, i+priority_offset));//ドラム缶
			objectManager.FindTarget(name).action.SetCenter(name, 96/2, 144/2);//2DのCenter設定				
		}
		priority_offset += drumCnt;

		objectManager.Regist(new PhysicsObject("Roof", image, 
			44, 174, 96, 64, 343, 72, priority_offset));//屋根
		objectManager.FindTarget("Roof").action.SetCenter("Roof", 96/2, 64/2);//2DのCenter設定				
		priority_offset++;

		for (int i = 0; i < 7; i++){
			name = "BrickS"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
				44, 240, 24, 40, 351, 376, i+priority_offset));//レンガ（小）
			objectManager.FindTarget(name).action.SetCenter(name, 24/2, 40/2);//2DのCenter設定				
			priority_offset++;
			
			name = "BrickB"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
				70, 240, 56, 40, 351, 376, i+priority_offset));//レンガ（大）
			objectManager.FindTarget(name).action.SetCenter(name, 56/2, 40/2);//2DのCenter設定				
			priority_offset++;
		}

		objectManager.Regist(new PhysicsObject("Foundation", image, 
			44, 282, 112, 48, 335, 416, priority_offset));//土台
		objectManager.FindTarget("Foundation").action.SetCenter("Foundation", 112/2, 48/2);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Rope", image, 
				216, 1, 70, 162, 1399, 96, priority_offset));//ロープ
		objectManager.FindTarget("Rope").action.SetCenter("Rope", 70/2, 162/2+20/2);//2DのCenter設定				
		priority_offset++;

		//フック修正版
		objectManager.Regist(new PhysicsObject("fook2", image, 
				288, 36, 40, 73, 1414, 32, priority_offset));//フック修正版
		objectManager.FindTarget("fook2").action.SetCenter("fook2", 40/2, -32);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("fook2_wire", image, 
				288, 2, 24, 32, 1422, 0, priority_offset));//ワイヤー
		objectManager.FindTarget("fook2_wire").action.SetCenter("fook2_wire", 24/2, 0);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Wire", image, 
				288, 2, 24, 32, 1422, 0, priority_offset));//ワイヤー
		objectManager.FindTarget("Wire").action.SetCenter("Wire", 24/2, 24);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Wire2", image, 
				288, 2, 24, 32-10, 1422, 0-32, priority_offset));//ワイヤー
		objectManager.FindTarget("Wire2").action.SetCenter("Wire2", 24/2, 24+24);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Wire3", image, 
				288, 2, 24, 32, 1422, 0-32, priority_offset));//ワイヤー
		objectManager.FindTarget("Wire3").action.SetCenter("Wire3", 24/2, 24+24+24);//2DのCenter設定				
		priority_offset++;

		objectManager.Regist(new PhysicsObject("Wire4", image, 
				288, 2, 24, 32, 1422, 0-32, priority_offset));//ワイヤー
		objectManager.FindTarget("Wire4").action.SetCenter("Wire4", 24/2, 24+24+24+24);//2DのCenter設定				
		priority_offset++;
					
		objectManager.Regist(new PhysicsObject("FookDrum", image, 
				100, 2, 96, 144, 1386,112, priority_offset));//ドラム缶
		objectManager.FindTarget("FookDrum").action.SetCenter("FookDrum", 96/2, 144/2);//2DのCenter設定				
		priority_offset++;

		for(int i=0; i<crossCnt; i++){
			name = "CrossA"+i;// 筋交
			objectManager.Regist(new PhysicsObject(name, image, 
			329, 2, 159, 152, 1178, 128, priority_offset));//筋交
			objectManager.FindTarget(name).action.SetCenter(name, 159/2, 152/2);//2DのCenter設定				
			priority_offset++;
		}
			
		for(int i=0; i<scaffoldCnt; i++){
			name = "Scaffold"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
			2, 148, 160, 24, 1178, 96, i+priority_offset));//足場板
			objectManager.FindTarget(name).action.SetCenter(name, 160/2, 24/2);//2DのCenter設定				
		}
		priority_offset += scaffoldCnt;

		for(int i=0; i<frameCnt; i++){
			name = "Frame"+i;
			objectManager.Regist(new PhysicsObject(name, image, 
			198, 2, 16, 160, 1178, 120, i+priority_offset));//建枠
			objectManager.FindTarget(name).action.SetCenter(name, 16/2, 160/2);//2DのCenter設定				
		}
		priority_offset += frameCnt;
			
		image = Resource2d.GetInstance().ImageStage02_BGP;
		objectManager.Regist(new PhysicsObject("Bgp1", image, 
				0, 7, 256, 25, 256,  455, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp2", image, 
				0, 34, 256, 30, 512, 450, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp3", image, 
				0, 66, 256, 30, 768, 450, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp4", image, 
				0, 99, 256, 37,1024, 443, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp5", image, 
				0,139, 256, 29,1280, 451, priority_offset));//
		priority_offset++;
		objectManager.Regist(new PhysicsObject("Bgp6", image, 
				0,175, 172, 33,1536, 447, priority_offset));//
		priority_offset++;

		// 背景14分割
		image = Resource2d.GetInstance().ImageStage02_BG;
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

		objectManager.FindTarget("BucketHead").InitPos(854+(int)(sceneBodies[bucketheadIdx].Position.X*(1/scene_scale)) - 96/2, 
			-((int)(sceneBodies[bucketheadIdx].Position.Y*(1/scene_scale)) - 240 + 28/2));//バケツ蓋
		objectManager.FindTarget("BucketHead").action.SetDegree(sceneBodies[bucketheadIdx].Rotation);	

		objectManager.FindTarget("Bucket").InitPos(854+(int)(sceneBodies[bucketIdx].Position.X*(1/scene_scale)) - 92/2, 
			-((int)(sceneBodies[bucketIdx].Position.Y*(1/scene_scale)) - 240 + 116/2));//バケツ
		objectManager.FindTarget("Bucket").action.SetDegree(sceneBodies[bucketIdx].Rotation);	

		objectManager.FindTarget("Shovel").InitPos(854+(int)(sceneBodies[shovelIdx].Position.X*(1/scene_scale)) - 40/2, 
				-((int)(sceneBodies[shovelIdx].Position.Y*(1/scene_scale)) - 240 + 160/2));//シャベル
		objectManager.FindTarget("Shovel").action.SetDegree(sceneBodies[shovelIdx].Rotation);	

		for(int i=0; i<drumCnt; i++){
			name = "Drum"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[drumIdx[i]].Position.X*(1/scene_scale)) - 96/2, 
				-((int)(sceneBodies[drumIdx[i]].Position.Y*(1/scene_scale)) - 240 + 144/2));//ドラム缶
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[drumIdx[i]].Rotation);	
		}
			
		objectManager.FindTarget("Roof").InitPos(854+(int)(sceneBodies[roofIdx].Position.X*(1/scene_scale)) - 96/2, 
			-((int)(sceneBodies[roofIdx].Position.Y*(1/scene_scale)) - 240 + 64/2));//屋根
		objectManager.FindTarget("Roof").action.SetDegree(sceneBodies[roofIdx].Rotation);	
			
		for (int i = 0; i < brickBCnt; i++){
			name = "BrickS"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[brickSIdx[i]].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[brickSIdx[i]].Position.Y*(1/scene_scale)) - 240 + 40/2));//レンガ（小）
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[brickSIdx[i]].Rotation);	
				
			name = "BrickB"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[brickBIdx[i]].Position.X*(1/scene_scale)) - 56/2, 
				-((int)(sceneBodies[brickBIdx[i]].Position.Y*(1/scene_scale)) - 240 + 40/2));//レンガ（大）
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[brickBIdx[i]].Rotation);	
		}

		objectManager.FindTarget("Foundation").InitPos(854+(int)(sceneBodies[foundationIdx].Position.X*(1/scene_scale)) - 112/2, 
			-((int)(sceneBodies[foundationIdx].Position.Y*(1/scene_scale)) - 240 + 48/2));//土台
		objectManager.FindTarget("Foundation").action.SetDegree(sceneBodies[foundationIdx].Rotation);	
			
		//クレーン修正版
		objectManager.FindTarget("Wire").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24));//ワイヤー
		objectManager.FindTarget("Wire").action.SetDegree(sceneBodies[crane2Idx].Rotation);	

		objectManager.FindTarget("Wire2").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24 + 24));//ワイヤー
		objectManager.FindTarget("Wire2").action.SetDegree(sceneBodies[crane2Idx].Rotation);	

		objectManager.FindTarget("Wire3").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24 + 24 + 24));//ワイヤー
		objectManager.FindTarget("Wire3").action.SetDegree(sceneBodies[crane2Idx].Rotation);	

		objectManager.FindTarget("Wire4").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 + 24 + 24 + 24 + 24));//ワイヤー
		objectManager.FindTarget("Wire4").action.SetDegree(sceneBodies[crane2Idx].Rotation);	

		objectManager.FindTarget("fook2").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 40/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240 )+32);//フック
		objectManager.FindTarget("fook2").action.SetDegree(sceneBodies[crane2Idx].Rotation);
			
		objectManager.FindTarget("fook2_wire").InitPos(854+(int)(sceneBodies[crane2Idx].Position.X*(1/scene_scale)) - 24/2, 
				-((int)(sceneBodies[crane2Idx].Position.Y*(1/scene_scale))-240));//ワイヤー
		objectManager.FindTarget("fook2_wire").action.SetDegree(sceneBodies[crane2Idx].Rotation);

		objectManager.FindTarget("FookDrum").InitPos(854+(int)(sceneBodies[craneIdx[0]].Position.X*(1/scene_scale)) - 96/2, 
				-((int)(sceneBodies[craneIdx[0]].Position.Y*(1/scene_scale))-240 + 144/2));//ドラム缶
		objectManager.FindTarget("FookDrum").action.SetDegree(sceneBodies[craneIdx[0]].Rotation);	

		objectManager.FindTarget("Rope").InitPos(854+(int)(sceneBodies[craneIdx[0]].Position.X*(1/scene_scale)) - 70/2, 
				-((int)(sceneBodies[craneIdx[0]].Position.Y*(1/scene_scale))-240 + 162/2+20/2));//ロープ (ドラム缶との位置補正)
		objectManager.FindTarget("Rope").action.SetDegree(sceneBodies[craneIdx[0]].Rotation);	

		for(int i=0; i<scaffoldCnt; i++){
			name = "Scaffold"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[scaffoldIdx[i]].Position.X*(1/scene_scale)) - 160/2, 
				-((int)(sceneBodies[scaffoldIdx[i]].Position.Y*(1/scene_scale)) - 240 + 24/2));//足場
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[scaffoldIdx[i]].Rotation);	
		}
			
		for(int i=0; i<frameCnt; i++){
			name = "Frame"+i;
			objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[frameIdx[i]].Position.X*(1/scene_scale)) - 16/2, 
				-((int)(sceneBodies[frameIdx[i]].Position.Y*(1/scene_scale)) - 240 + 160/2));//建枠
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[frameIdx[i]].Rotation);	
		}
		
		//筋交
		for(int i=0; i<crossCnt; i++){
			name = "CrossA"+i;
			if(i==0 || i==2){
				objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[crossIdx[i]].Position.X*(1/scene_scale)) - (160)/2, 
					-((int)(sceneBodies[crossIdx[i]].Position.Y*(1/scene_scale)) - 240 + (132)/2));//筋交（／）
			}else{
				objectManager.FindTarget(name).InitPos(854+(int)(sceneBodies[crossIdx[i]].Position.X*(1/scene_scale)) - (159)/2, 
					-((int)(sceneBodies[crossIdx[i]].Position.Y*(1/scene_scale)) - 240 + (132)/2));//筋交（＼）
			}
			objectManager.FindTarget(name).action.SetDegree(sceneBodies[crossIdx[i]].Rotation);	
		}
	}

    /// シミュレーション後のアクション定義
    /// この時点で衝突点がわかっているので、衝突点にかかわる処理
    /// 速度の修正はここで。位置の修正はUpdateFuncBeforeSim()に戻ってから行うこと
	public override void UpdateFuncAfterSim ()
	{
		if(objectManager != null)
			objectManager.Update();

		FittingPosition();

		//クレーンのドラム缶解除
		if(!isCrane_st3){
			for(int i=3; i<numBody; i++){
	            // 修正版
				if (QueryContact((uint)craneIdx[0], (uint)i)){
	               	sceneBodies[craneIdx[0]].BackToDynamic();
					isCrane_st3 = true;
				}else if (QueryContact((uint)crane2Idx, (uint)i)){
	               	sceneBodies[craneIdx[0]].BackToDynamic();
					isCrane_st3 = true;
	            }
			}
		}
		if(!colFlag[0]){
			for(int i=4; i<numBody; i++){
				if(QueryContact((uint)crossIdx[0], (uint)i)){
	                sceneBodies[crossIdx[0]].BackToDynamic();//筋交 左上
					colFlag[0] = true;
	           	}
			}
		}
		if(!colFlag[1]){
			for(int i=4; i<numBody; i++){
				if(QueryContact((uint)crossIdx[1], (uint)i)){
	                sceneBodies[crossIdx[1]].BackToDynamic();//筋交 左下
					colFlag[1] = true;
	           	}
			}
		}
		if(!colFlag[2]){
			for(int i=4; i<numBody; i++){
				if(QueryContact((uint)crossIdx[2], (uint)i)){
	                sceneBodies[crossIdx[2]].BackToDynamic();//筋交 右上
					colFlag[2] = true;
	            }
			}
		}
		if(!colFlag[3]){
			for(int i=4; i<numBody; i++){
				if(QueryContact((uint)crossIdx[3], (uint)i)){
                	sceneBodies[crossIdx[3]].BackToDynamic();//筋交 右下
					colFlag[3] = true;
				}
			}	
		}
	}

	/// クレーンは静態にしない
	public override void SetsceneBodiesKinematic(int idx)
	{	
		if(idx < 3 || idx == 29 || idx == 30 || idx == 31){
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
