/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

// Pin Ball Game Scene
//
// [How to play] 
//
// Press the square button to push the ball up
// This is just a demo and it is not possible to controll the velocity of the ball
//
// To restart the scene, press the triangle button
// To move to the next scene, press the right arrow button
// To move to the previous scene, press the left arrow button


// use direct access to structure of simulation data to set up initial scene
#define USE_OLD_API

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;


// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2DSample
{
	public class PinBallScene : PhysicsScene
	{
        
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];


		private System.Random r = new System.Random(10);
			
		private int target;

        private Vector2 global_center = new Vector2(0, -75);

		public PinBallScene ()
		{
			InitScene();

            
            // Setup for Rendering Object
            for (int i = 0; i < numShape; i++)
            {
                if (sceneShapes[i].numVert == 0)
                {
                    vertices[i] = new VertexBuffer(37, VertexFormat.Float3);
                }
                else
                {
                    vertices[i] = new VertexBuffer(sceneShapes[i].numVert + 1, VertexFormat.Float3);
                }

                MakeLineListConvex(sceneShapes[i], vertices[i]);
            }
		}


        ~PinBallScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if (vertices[i] != null)
                    vertices[i].Dispose();
        }
		
#if USE_OLD_API
		
		public override void InitScene()
		{
            
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "PinBallScene";
			
			const float radius = 8.0f;
			
			Vector2 general_width = new Vector2(1.0f, 1.0f);
			Vector2 ground_width = new Vector2(50, 2);
			Vector2 shoot_width = new Vector2(5, 1);
			
			sceneShapes[numShape++] = new PhysicsShape(general_width);
			sceneShapes[numShape++] = new PhysicsShape(ground_width);
			sceneShapes[numShape++] = new PhysicsShape(shoot_width);

			float[,] vertex_list = new float[,]
			{
				{-0.29870838f, -0.40948039f},
				{0.046578027f, -0.39159656f},
				{0.22971961f, -0.37768012f},
				{0.54784453f, -0.34969464f},
				{0.30659574f, 0.55520970f},
				{-0.036768019f, 0.56702036f},
				{-0.15493569f, 0.43951401f},
				{-0.30792499f, 0.14919144f},
				{-0.33240083f, -0.18248373f}
			};
			
			sceneShapes[numShape] = new PhysicsShape();
			sceneShapes[numShape].numVert = 9;
			
			for(int i=0; i<9; i++)
				sceneShapes[numShape].vertList[i] = 3.0f*new Vector2(vertex_list[i,0], vertex_list[i,1]);
				         
			numShape++;
			
			sceneShapes[numShape++] = new PhysicsShape(2.0f);
			sceneShapes[numShape++] = new PhysicsShape(1.0f);	
			sceneShapes[numShape++] = new PhysicsShape(radius - general_width.X);
			sceneShapes[numShape++] = new PhysicsShape(radius - general_width.X);
			sceneShapes[numShape++] = new PhysicsShape(radius - general_width.X);
            sceneShapes[numShape++] = new PhysicsShape(new Vector2(50, 1));
                
            // Create the ground
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(0.0f, -2.5f) + global_center;
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 1;
				numBody++;
			}		
			
            // Create the wall
            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(50, 50) + global_center;
            sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
            sceneBodies[numBody].shapeIndex = 9;
            numBody++;


            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-50, 50) + global_center;
            sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
            sceneBodies[numBody].shapeIndex = 9;
            numBody++;

            sceneBodies[numBody] = new PhysicsBody(sceneShapes[9], PhysicsUtility.FltMax);
            sceneBodies[numBody].position = new Vector2(-40, 50) + global_center;
            sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
            sceneBodies[numBody].shapeIndex = 9;
            numBody++;
		
			// Create the circular corner
			for(int i=0; i<=20; i++)
			{
				float angle = i*(180.0f/(20));  
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(50.0f * (float)Math.Cos(angle / 180.0f * PhysicsUtility.Pi), 50.0f * (float)Math.Sin(angle / 180.0f * PhysicsUtility.Pi) + 100) + global_center;
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(angle+90.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
			}
		
			// Create embedded convex shape
			for(int i=0; i<=20; i++)
			{
				float angle = i*(180.0f/(20));  
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(50.0f * (float)Math.Cos(angle / 180.0f * PhysicsUtility.Pi), 50.0f * (float)Math.Sin(angle / 180.0f * PhysicsUtility.Pi) + 100) + global_center;
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(angle+90.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;
			}
		
			// Create the rotate bar
			for(int j=0; j<5; j++)
			{
				for(int i=0; i<3; i++)
				{
					int index1;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(i * 22 - 20 + (j % 2) * 5, 100 - j * 10) + global_center;
					sceneBodies[numBody].rotation = 0.0f;
					sceneBodies[numBody].shapeIndex = 2;
					sceneBodies[numBody].collisionFilter = 1<<4;
					index1 = numBody;
					numBody++;
		
					PhysicsBody b1 = sceneBodies[0];
					PhysicsBody b2 = sceneBodies[index1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), (uint)0, (uint)index1);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 0.001f;
					numJoint++;
			
				}
			}		
		
			float[,] pin_pos = new float[,]
			{
				{40.50f, 130.55f},
				{28.03f, 128.78f},
				{25.44f, 116.93f},
				{2.06f, 121.77f},
				{-9.99f, 136.83f},
				{21.13f, 111.22f},
				{10.49f, 128.16f},
				{43.71f, 116.27f},
				{-22.42f, 118.85f},
				{-10.33f, 128.60f}
			};

            // Create random pins
            for (int i = 0; i < 10; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(pin_pos[i, 0], pin_pos[i, 1]) + global_center;                                             
				sceneBodies[numBody].shapeIndex = 5;
				sceneBodies[numBody].collisionFilter = 1<<4;
				numBody++;
			}

			// Create goal pockets
			for(int i=0; i<3; i++)
			{	
		
				Vector2 global_position = new Vector2(-25 + 25*i, 10);
			
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(-1.0f - 0.1f, 11.0f) + global_position + global_center;
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
				sceneBodies[numBody].shapeIndex = 2;
				sceneBodies[numBody].collisionFilter = 1<<4;
				numBody++;
		
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(11.0f + 0.1f, 11.0f) + global_position + global_center;
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
				sceneBodies[numBody].shapeIndex = 2;
				sceneBodies[numBody].collisionFilter = 1<<4;
				numBody++;
		
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 3.0f);
                sceneBodies[numBody].position = new Vector2(5.0f, 9.0f) + global_position + global_center;
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 2;
				sceneBodies[numBody].collisionFilter = 1<<4;
				int index1 = numBody;
				numBody++;
		
				PhysicsBody b1 = sceneBodies[0];
				PhysicsBody b2 = sceneBodies[index1];
				sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), (uint)0, (uint)index1);
				sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
				sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
				sceneJoints[numJoint].angleLim = 0.001f;
				numJoint++;
			}
			
			// Create balls
			for(int i=0; i<15; i++)
			{
				float pachinko_radius = sceneShapes[4].vertList[0].X;
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 5.0f);
                sceneBodies[numBody].position = new Vector2(-45.0f, 3.0f + 2 * pachinko_radius * i) + global_center;
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].SetBodyKinematic();
				numBody++;
			}
			
			target = numBody - 1;
		}
#else
		
		public override void InitScene()
		{
            
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "PinBallScene";
			
			const float radius = 8.0f;
			
			Vector2 general_width = new Vector2(1.0f, 1.0f);
			Vector2 ground_width = new Vector2(50, 2);
			Vector2 shoot_width = new Vector2(5, 1);
			
			AddBoxShape(general_width);
			AddBoxShape(ground_width);
			AddBoxShape(shoot_width);

			Vector2[] vertex_list = new Vector2[9]
			{
				3.0f * new Vector2(-0.29870838f, -0.40948039f),
				3.0f * new Vector2(0.046578027f, -0.39159656f),
				3.0f * new Vector2(0.22971961f, -0.37768012f),
				3.0f * new Vector2(0.54784453f, -0.34969464f),
				3.0f * new Vector2(0.30659574f, 0.55520970f),
				3.0f * new Vector2(-0.036768019f, 0.56702036f),
				3.0f * new Vector2(-0.15493569f, 0.43951401f),
				3.0f * new Vector2(-0.30792499f, 0.14919144f),
				3.0f * new Vector2(-0.33240083f, -0.18248373f)
			};
			
			AddConvexShape(vertex_list, vertex_list.Length);
			
			AddSphereShape(2.0f);
			AddSphereShape(1.0f);	
			AddSphereShape(radius - general_width.X);
			AddSphereShape(radius - general_width.X);
			AddSphereShape(radius - general_width.X);
            AddBoxShape(new Vector2(50, 1));
                
          
			{
				PhysicsBody body = null;
				
				// Create the ground
				
				body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(0.0f, -2.5f) + global_center;
				body.Rotation = 0.0f;	
			
	            // Create the wall
	            body = AddBody(SceneShapes[9], PhysicsUtility.FltMax);
	            body.Position = new Vector2(50, 50) + global_center;
	            body.Rotation = PhysicsUtility.GetRadian(90.0f);
	
	            body = AddBody(SceneShapes[9], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-50, 50) + global_center;
	            body.Rotation = PhysicsUtility.GetRadian(90.0f);
	
	            body = AddBody(SceneShapes[9], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-40, 50) + global_center;
	            body.Rotation = PhysicsUtility.GetRadian(90.0f);
			}
		
			// Create the circular corner
			for(int i=0; i<=20; i++)
			{
				float angle = i*(180.0f/(20));  
				PhysicsBody body = AddBody(SceneShapes[2], PhysicsUtility.FltMax);
                body.Position = new Vector2(50.0f * (float)Math.Cos(angle / 180.0f * PhysicsUtility.Pi), 50.0f * (float)Math.Sin(angle / 180.0f * PhysicsUtility.Pi) + 100) + global_center;
				body.Rotation = PhysicsUtility.GetRadian(angle+90.0f);
			}
		
			// Create embedded convex shape
			for(int i=0; i<=20; i++)
			{
				float angle = i*(180.0f/(20));  
				PhysicsBody body = AddBody(SceneShapes[3], PhysicsUtility.FltMax);
                body.Position = new Vector2(50.0f * (float)Math.Cos(angle / 180.0f * PhysicsUtility.Pi), 50.0f * (float)Math.Sin(angle / 180.0f * PhysicsUtility.Pi) + 100) + global_center;
				body.Rotation = PhysicsUtility.GetRadian(angle+90.0f);
			}
		
			// Create the rotate bar
			for(int j=0; j<5; j++)
			{
				for(int i=0; i<3; i++)
				{
					int index1;
					PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(i * 22 - 20 + (j % 2) * 5, 100 - j * 10) + global_center;
					body.Rotation = 0.0f;
					body.CollisionFilter = 1<<4;
					index1 = NumBody-1;

					PhysicsBody b1 = SceneBodies[0];
					PhysicsBody b2 = SceneBodies[index1];
					PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 0.001f;			
				}
			}		
		
			float[,] pin_pos = new float[,]
			{
				{40.50f, 130.55f},
				{28.03f, 128.78f},
				{25.44f, 116.93f},
				{2.06f, 121.77f},
				{-9.99f, 136.83f},
				{21.13f, 111.22f},
				{10.49f, 128.16f},
				{43.71f, 116.27f},
				{-22.42f, 118.85f},
				{-10.33f, 128.60f}
			};

            // Create random pins
            for (int i = 0; i < 10; i++)
			{
				PhysicsBody body = AddBody(SceneShapes[5], PhysicsUtility.FltMax);
                body.Position = new Vector2(pin_pos[i, 0], pin_pos[i, 1]) + global_center;                                             
				body.CollisionFilter = 1<<4;
			}

			// Create goal pockets
			for(int i=0; i<3; i++)
			{	
		
				Vector2 global_position = new Vector2(-25 + 25*i, 10);
			
				PhysicsBody body = null;
				
				body = AddBody(SceneShapes[2], PhysicsUtility.FltMax);
                body.Position = new Vector2(-1.0f - 0.1f, 11.0f) + global_position + global_center;
				body.Rotation = PhysicsUtility.GetRadian(90.0f);
				body.CollisionFilter = 1<<4;
		
				body = AddBody(SceneShapes[2], PhysicsUtility.FltMax);
                body.Position = new Vector2(11.0f + 0.1f, 11.0f) + global_position + global_center;
				body.Rotation = PhysicsUtility.GetRadian(90.0f);
				body.CollisionFilter = 1<<4;

				body = AddBody(SceneShapes[2], 3.0f);
                body.Position = new Vector2(5.0f, 9.0f) + global_position + global_center;
				body.Rotation = 0.0f;
				body.CollisionFilter = 1<<4;
				int index1 = NumBody-1;
		
				PhysicsBody b1 = SceneBodies[0];
				PhysicsBody b2 = SceneBodies[index1];
				PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
				joint.Axis1Lim = new Vector2(1.0f, 0.0f);
				joint.Axis2Lim = new Vector2(0.0f, 1.0f);
				joint.AngleLim = 0.001f;
			}
			
			// Create balls
			for(int i=0; i<15; i++)
			{
				float pachinko_radius = SceneShapes[4].VertList[0].X;
				PhysicsBody body = AddBody(SceneShapes[4], 5.0f);
                body.Position = new Vector2(-45.0f, 3.0f + 2 * pachinko_radius * i) + global_center;
				body.Rotation = 0.0f;
				body.SetBodyKinematic();
			}
			
			target = NumBody - 1;
		}
		
#endif
		
        // Shoot the ball by pressing the button
        public override void KeyboardFunc(GamePadButtons ePad)
        {
            switch (ePad)
            {
                case GamePadButtons.Square:
                    if (target == numBody - 16)
                    {
                        float pachinko_radius = sceneShapes[4].vertList[0].X;
                        for (int i = 0; i < 15; i++)
                        {
                            sceneBodies[numBody - 15 + i].position = new Vector2(-45.0f, 3.0f + 2 * pachinko_radius * i) + global_center;
                            sceneBodies[numBody - 15 + i].rotation = 0.0f;
                            sceneBodies[numBody - 15 + i].SetBodyKinematic();
                        }
                        target = numBody - 1;
                    }
                    else
                    {
                        sceneBodies[target].BackToDynamic();
                        sceneBodies[target].velocity = new Vector2(0, 150 + 30 * (float)r.Next(-10, 10) / 10.0f) + global_center;
                        sceneBodies[target].angularVelocity = 0.0f;
                        target--;
                    }
                    break;
                default:
                    break;
            }
        }

        // Line Rendering for Object
        private void MakeLineListConvex(PhysicsShape con, VertexBuffer vertices)
        {

            if (con.numVert == 0)
            {
                float[] vertex = new float[3 * 37];

                int i = 0;
                float rad = con.vertList[0].X;

                for (float th1 = 0.0f; th1 < 360.0f; th1 = th1 + 10.0f)
                {
                    float th1_rad = th1 / 180.0f * PhysicsUtility.Pi;

                    float x1 = rad * (float)Math.Cos(th1_rad);
                    float y1 = rad * (float)Math.Sin(th1_rad);

                    vertex[3 * i + 0] = x1;
                    vertex[3 * i + 1] = y1;
                    vertex[3 * i + 2] = 0.0f;
                    i++;
                }

                vertex[3 * i + 0] = vertex[3 * 0 + 0];
                vertex[3 * i + 1] = vertex[3 * 0 + 1];
                vertex[3 * i + 2] = vertex[3 * 0 + 2];

                vertices.SetVertices(0, vertex);

            }
            else
            {
                float[] vertex = new float[3 * (con.numVert + 1)];

                int i;

                for (i = 0; i < con.numVert; i++)
                {
                    Vector2 v1;
                    v1 = con.vertList[i];

                    vertex[3 * i + 0] = v1.X;
                    vertex[3 * i + 1] = v1.Y;
                    vertex[3 * i + 2] = 0.0f;
                }

                vertex[3 * i + 0] = vertex[3 * 0 + 0];
                vertex[3 * i + 1] = vertex[3 * 0 + 1];
                vertex[3 * i + 2] = vertex[3 * 0 + 2];

                vertices.SetVertices(0, vertex);
            }
        }


        
        // Draw objects
        public override void DrawAllBody(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix, int click_index)
        {
            for (int j = 0; j < numShape; j++)
            {
                graphics.SetVertexBuffer(0, vertices[j]);

                for (int i = 0; i < numBody; i++)
                {
                    uint index = sceneBodies[i].shapeIndex;

                    if (j != index) continue;

                    Matrix4 rotationMatrix = Matrix4.RotationZ(sceneBodies[i].rotation);

                    Matrix4 transMatrix = Matrix4.Translation(
                        new Vector3(sceneBodies[i].position.X, sceneBodies[i].position.Y, 0.0f));

                    Matrix4 local_rotationMatrix = Matrix4.RotationZ(sceneBodies[i].localRotation);

                    Matrix4 local_transMatrix = Matrix4.Translation(
                        new Vector3(sceneBodies[i].localPosition.X, sceneBodies[i].localPosition.Y, 0.0f));

                    Matrix4 WorldMatrix = renderMatrix * transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix;

                    program.SetUniformValue(0, ref WorldMatrix);

                    if (i == click_index)
                    {
                        Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                        program.SetUniformValue(1, ref color);
                    }
                    else
                    {
                        Vector3 color = new Vector3(0.0f, 1.0f, 1.0f);
                        program.SetUniformValue(1, ref color);
                    }

                    if (sceneShapes[index].numVert == 0)
                        graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                    else
                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);
                }

            }
        }


	}
}
