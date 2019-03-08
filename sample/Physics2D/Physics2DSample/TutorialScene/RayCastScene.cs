/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// Raycast Scene --- Basic Raycast Creation
//

// [How to set up the simulation scene]
//
// (1) Make an simulation scene
// (2) Arrange coefficients for contact solver or gravity etc...
// (3) Prepare collision shapes for rigid sceneBodies
// (4) Make the rigid body instances by specifying collision shapes and masses 
// (5) Set the position, rotation, velocity and angular velocity based on the scene
//
// Recommendation of scales for dynamic rigid body
//	
// mass : 0.2[Kg] - 20.0[Kg] for dynamic rigid body
// length : 0.2[m] - 20.0[m] for dynamic rigid body
//
// 1.0[Kg] x 1[m] is a typical mass & length of rigid body for dynamic rigid body 
//
// Please notice that the object has infinite mass while user is picking it 
//
//

// [Tips and notice]
//
// This sample shows the way to use simple raycast.
//
// When raycast starts from the point outside of object, it contacts with the surface of object.
// When raycast starts from the point inside of object, it contacts with the back of object.
//


// use direct access to structure of simulation data to set up initial scene
#define USE_OLD_API

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

using System.Diagnostics;
using System.Runtime.InteropServices;

// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2DSample
{
    // To create a simulation scene, please inherit from PhysicsScene class
    public class RayCastScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];
		
        // Vertex Buffer for Debug Rendering
        private VertexBuffer aabbVert = null;
		
		private VertexBuffer colVert = null;
        
		private VertexBuffer rayVert = null;

        private System.Random rand_gen = new System.Random(10);
		
		private RayHit testRay;
		
        public RayCastScene()
        {
            // Simulation Scene Set Up
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

            // VertexBuffer for AABB debug rendering
            {
                aabbVert = new VertexBuffer(5, VertexFormat.Float3);

                float[] vertex = new float[]
                {
                    0.0f, 0.0f, 0.0f,
                    1.0f, 0.0f, 0.0f,
                    1.0f, 1.0f, 0.0f,
                    0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f,
                };

                aabbVert.SetVertices(0, vertex);
            }

            // VertexBuffer for contact points debug rendering
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
			
			// VertexBuffer used for joints debug rendering
            {
                rayVert = new VertexBuffer(2, VertexFormat.Float3);

                float[] vertex = new float[]
                { 
					0.0f, 0.0f, 0.0f, 
					0.0f, 0.0f, 0.0f 
				};
                
                rayVert.SetVertices(0, vertex);
            }
        }

        ~RayCastScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if(vertices[i] != null)
                    vertices[i].Dispose();

            if(aabbVert != null) aabbVert.Dispose();
            if(colVert != null) colVert.Dispose();						
			if(rayVert != null) rayVert.Dispose();
        }
		
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			
			sceneName = "RayCastScene";
			
			// Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);
			
            Vector2 box_width = new Vector2(2.0f, 2.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
			
			float sphere_width = 2.0f;
			sceneShapes[3] = new PhysicsShape(sphere_width);

            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 2.0f / 1000.0f;
            }

            // Convex Shape Setting (by using random points)
            sceneShapes[4] = PhysicsShape.CreateConvexHull(test_point, 10);
			
			
			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)i, 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian(i*20.0f)));
				}
					                           
				sceneShapes[5] = new PhysicsShape(sin_chain, 20, true);
					                           
			}

			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)(i+19), 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian((i+19)*20.0f)));
				}
					                           
				sceneShapes[6] = new PhysicsShape(sin_chain, 20, true);
					                           
			}
			
			numShape = 7;
			
			
            // Create static walls to limit the range of action of active rigid sceneBodies
            {
                // new PhysicsBody( "shape of the body",  "mass of the body(kg)" ) 
                sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);

                // Set the position & the rotation
                sceneBodies[numBody].position = new Vector2(0, -wall_height.Y);
                sceneBodies[numBody].rotation = 0;

                // Make shapeIndex consistent with what we set as convex shape
                sceneBodies[numBody].shapeIndex = 0;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(wall_width.X, 0);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 1;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(-wall_width.X, 0);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 1;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(0, wall_height.Y);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 0;
                numBody++;
            }
			
			
			
			for(int i=0; i<2; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(0, 10) + new Vector2(10.0f*i, 0);
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(0, 0) + new Vector2(10.0f*i, 0);
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(0, 20) + new Vector2(10.0f*i, 0);
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 4;
				numBody++;
			}
			
					
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
			sceneBodies[numBody].position = new Vector2(0, -20);
			sceneBodies[numBody].shapeIndex = 2;
			numBody++;
				
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
			sceneBodies[numBody].position = new Vector2(10, -20);
			sceneBodies[numBody].shapeIndex = 2;
			numBody++;
			
			uint[] combination = new uint[2];
			combination[0] = (uint)(numBody-2);
			combination[1] = (uint)(numBody-1);
			// MergeBody(...) returns the position of sceneBodies[compound2_index[0,1] which are shared now.
			PhysicsBody.MergeBody(sceneBodies, combination, 2, sceneShapes, compoundMap);
			
			// Create the line shape
			// Only static rigid body is permitted for the line shape
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(-10, -10);
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 5;
				numBody++;
			}	
			
			// Create the line shape
			// Only static rigid body is permitted for the line shape
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(-10, -10);
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 6;
				numBody++;
			}	
			
			
        }
		
#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			
			SceneName = "RayCastScene";
			
			// Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);
			
            Vector2 box_width = new Vector2(2.0f, 2.0f);
            AddBoxShape(box_width);
			
			float sphere_width = 2.0f;
			AddSphereShape(sphere_width);

            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 2.0f / 1000.0f;
            }

            // Convex Shape Setting (by using random points)
            AddConvexHullShape(test_point, 10);
			
			
			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)i, 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian(i*20.0f)));
				}
					                           
				AddLineChainShape(sin_chain, 20);
					                           
			}

			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)(i+19), 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian((i+19)*20.0f)));
				}
					                           
				AddLineChainShape(sin_chain, 20);
					                           
			}
			

			
            // Create static walls to limit the range of action of active rigid sceneBodies
            {
				PhysicsBody body = null;
                // AddBody( "shape of the body",  "mass of the body(kg)" ) 
                body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);

                // Set the position & the rotation
                body.Position = new Vector2(0, -wall_height.Y);
                body.Rotation = 0;

                body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(wall_width.X, 0);
                body.Rotation = 0;

                body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(-wall_width.X, 0);
                body.Rotation = 0;

                body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
                body.Position = new Vector2(0, wall_height.Y);
                body.Rotation = 0;
            }
			
			
			
			for(int i=0; i<2; i++)
			{
				PhysicsBody bodyA = AddBody(SceneShapes[2], PhysicsUtility.FltMax);
				bodyA.Position = new Vector2(0, 10) + new Vector2(10.0f*i, 0);
				bodyA.Rotation = PhysicsUtility.GetRadian(30.0f);
				
				PhysicsBody bodyB = AddBody(SceneShapes[3], PhysicsUtility.FltMax);
				bodyB.Position = new Vector2(0, 0) + new Vector2(10.0f*i, 0);
				bodyB.Rotation = 0;
				
				PhysicsBody bodyC = AddBody(SceneShapes[4], PhysicsUtility.FltMax);
				bodyC.Position = new Vector2(0, 20) + new Vector2(10.0f*i, 0);
				bodyC.Rotation = 0;
			}
			
			{
				PhysicsBody[] mergeList = {null, null};
				
				mergeList[0] = AddBody(SceneShapes[2], 1.0f);
				mergeList[0].Position = new Vector2(0, -20);
				
				mergeList[1] = AddBody(sceneShapes[2], 1.0f);
				mergeList[1].Position = new Vector2(10, -20);
				
				MergeCompound(mergeList, mergeList.Length);
			}
			
			// Create the line shape
			// Only static rigid body is permitted for the line shape
			{
				PhysicsBody body = AddBody(SceneShapes[5], PhysicsUtility.FltMax);
				body.Position = new Vector2(-10, -10);
				body.Rotation = 0;
			}	
			
			// Create the line shape
			// Only static rigid body is permitted for the line shape
			{
				PhysicsBody body = AddBody(SceneShapes[6], PhysicsUtility.FltMax);
				body.Position = new Vector2(-10, -10);
				body.Rotation = 0;
			}	
			
			
        }
			
#endif
		
		// Check the result of raycast before rendering
		public override void UpdateFuncAfterSim ()
		{			
			// Update the position and direction of raycast
			testRay = new RayHit();
			testRay.start = new Vector2(-20.0f, 20.0f*(float)Math.Cos(sceneFrame/160.0f));
			testRay.end = new Vector2(30.0f, 20.0f*(float)Math.Cos(sceneFrame/160.0f) + 20.0f * (float)Math.Sin (sceneFrame/160.0f));
			
			if(RaycastHit(ref testRay))
			{
				Console.WriteLine("Hit: Yes\n");
			}
			else
			{
				Console.WriteLine("Hit: No\n");
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
					
		            if(testRay.hitIndex == i)
                    {
                        Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                        program.SetUniformValue(1, ref color);
                    }
					
                    if (sceneShapes[index].numVert == 0)
                        graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                    else if(sceneShapes[index].hint != 0)
					{
                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);
					}
					else
					{
                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert);	
					}

                }

            }
        }

        // Debug rendering for contact points(RigidBody A <=> RigidBody B) and AABB(Axis Aligned Bounding Box)
        public override void DrawAdditionalInfo(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix)
        {
            // Draw contact points
            graphics.SetVertexBuffer(0, colVert);

            for (uint i = 0; i < numPhysicsSolverPair; i++)
            {
                // Collision point for RigidBody A 
                {
                    Matrix4 transMatrix = Matrix4.Translation(
                        new Vector3(solverPair[i].resA.X, solverPair[i].resA.Y, 0.0f));

                    Matrix4 WorldMatrix = renderMatrix * transMatrix;
                    program.SetUniformValue(0, ref WorldMatrix);

                    Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                    program.SetUniformValue(1, ref color);

                    graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
                }

                // Collision point for RigidBody B 
                {
                    Matrix4 transMatrix = Matrix4.Translation(
                        new Vector3(solverPair[i].resB.X, solverPair[i].resB.Y, 0.0f));

                    Matrix4 WorldMatrix = renderMatrix * transMatrix;
                    program.SetUniformValue(0, ref WorldMatrix);

                    Vector3 color = new Vector3(1.0f, 0.0f, 0.0f);
                    program.SetUniformValue(1, ref color);

                    graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
                }
            }
			
			graphics.SetVertexBuffer(0, rayVert);
			
			float[] vertex = new float[]
			{
				testRay.start.X, testRay.start.Y, 0.0f,
				testRay.end.X, testRay.end.Y, 0.0f	
			};
            
			if(testRay.hitIndex != -1)
			{
				vertex[3] = testRay.hit.X;
				vertex[4] = testRay.hit.Y;
			}
			
			rayVert.SetVertices(0, vertex);
			{
				Matrix4 WorldMatrix = renderMatrix;
				program.SetUniformValue(0, ref WorldMatrix);

				Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
							
				if(testRay.hitIndex != -1)
					color = new Vector3(0.0f, 1.0f, 0.0f);
					
				program.SetUniformValue(1, ref color);
			     graphics.DrawArrays(DrawMode.Points, 0, 1);
				graphics.DrawArrays(DrawMode.Lines, 0, 2);
			}
			
			
        }
	}
}


		
		

		

		
