/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// Line Chain Scene --- Chain of Line Segment Creation
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

//
// Tips and notice
//
// Line chain is very thin rigid body which does not have the width at all.
// Therefore, generally speaking, it is difficult to maintain the simulation 
// stability especially for collision detection.
//
// Therefore only static rigid body is allowed to create as line chain segments.
// It is not permitted to craete dynamic rigid body as line chain segments.
//
// It is easy for interpenetration happen when rigid body collides 
// with line chain segments if the other rigid body has high velocity.
// In this case, to avoid this interpenetration, there may be several ways of workaround
// ,and one of those is to duplicate the same line shape slightly under the original one.
//
// Caution: 
// Compared with usual primitives, line chain segments wastes a lot of calculation cost easily.
// It is not good to use line chain segments to create simple box or other primitive shapes.
// It is necessary to use box, sphere or usual convex shape as much as possible because of the performance.
//


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
    // To create a simulation scene, please inherit from PhysicsScene class
    public class LineChainScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer aabbVert = null;
        private VertexBuffer colVert = null;
		
		// VertexBuffer used for normal of collision
		private VertexBuffer normalVert = null;
		

        private System.Random rand_gen = new System.Random(10);
		
		private bool renderFlag = false;
        
        public LineChainScene()
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
			
			// VertexBuffer used for normal debug rendering
			{
				normalVert = new VertexBuffer(2, VertexFormat.Float3);
			
				float[] vertex = new float[]
				{ 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
				normalVert.SetVertices(0, vertex);
			}
        }

        ~LineChainScene()
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
			if(normalVert != null) normalVert.Dispose();

        }
		
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "LineChainScene";
			
			// print performance of the simulation
			printPerf = true;
			
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            sceneShapes[2] = new PhysicsShape(box_width);

            // Sphere Shape Setting PhysicsShape( "radius" )
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
				
		 	//	this is not allowed because there are multiple points on the same line
		 	//	
			//	Vector2[] line_chain = 
			//	{
			//		new Vector2(-20.0f, 0.0f),
			//		new Vector2(-15.0f, 0.0f),
			//		new Vector2(-10.0f, 0.0f),
			//		new Vector2(-5.0f, 0.0f),
			//		new Vector2(0.0f, -10.0f)
			//	};
			//	
	        //  sceneShapes[5] = new PhysicsShape(line_chain, 5, true);
	           
				
				Vector2[] line_chain =
				{
					new Vector2(-20.0f, 0.0f),
					new Vector2(-5.0f, 0.0f),
					new Vector2(0.0f, 10.0f)	
					
				};
				
				sceneShapes[5] = new PhysicsShape(line_chain, 3, true);
			
			}
		
			// Create the left part of sine curve
			//
			// Because discontinuity may happen for the normal of collision between left part and right part,
			//  and cautions may be necessary for that kind of cases.
			//
			// Therefore connection point should be carefully set in the case 
			// which shape is divided into several line parts.
			//
			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)i, 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian(i*20.0f)));
				}
					                           
				sceneShapes[6] = new PhysicsShape(sin_chain, 20, true);
					                           
			}
			
			// Create the righ part of sine curve
			//
			// Because discontinuity may happen for the normal of collision between left part and right part,
			//  and cautions may be necessary for that kind of cases.
			//
			// Therefore connection point should be carefully set in the case 
			// which shape is divided into several line parts.
			//
			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)(i+19), 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian((i+19)*20.0f)));
				}
					                           
				sceneShapes[7] = new PhysicsShape(sin_chain, 20, true);
					                           
			}
			
            numShape = 8;
     

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

            // Create dynamic rigid sceneBodies
            {
                // Create a box-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(-10.0f, 15.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    numBody++;
                }

                // Create a sphere-shaped dynamic rigid body
               {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
                    sceneBodies[numBody].position = new Vector2(0.0f, 15.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 3;
                    numBody++;
                }

                // Create a convex-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
                    sceneBodies[numBody].position = new Vector2(10.0f, 15.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 4;
                    numBody++;
                }
            }
			
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
			
			// Create the line shape
			// Only static rigid body is permitted for the line shape
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(-10, -10);
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 7;
				numBody++;
			}	

        }

#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "LineChainScene";
			
			// print performance of the simulation
			PrintPerf = true;
			
			// Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            AddBoxShape(box_width);

            // Sphere Shape Setting PhysicsShape( "radius" )
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
				
		 	//	this is not allowed because there are multiple points on the same line
		 	//	
			//	Vector2[] line_chain = 
			//	{
			//		new Vector2(-20.0f, 0.0f),
			//		new Vector2(-15.0f, 0.0f),
			//		new Vector2(-10.0f, 0.0f),
			//		new Vector2(-5.0f, 0.0f),
			//		new Vector2(0.0f, -10.0f)
			//	};
			//	
	        //  sceneShapes[5] = new PhysicsShape(line_chain, 5, true);
	           
				
				Vector2[] line_chain =
				{
					new Vector2(-20.0f, 0.0f),
					new Vector2(-5.0f, 0.0f),
					new Vector2(0.0f, 10.0f)	
					
				};
				
				AddLineChainShape(line_chain, 3);
			
			}
		
			// Create the left part of sine curve
			//
			// Because discontinuity may happen for the normal of collision between left part and right part,
			//  and cautions may be necessary for that kind of cases.
			//
			// Therefore connection point should be carefully set in the case 
			// which shape is divided into several line parts.
			//
			{
				Vector2[] sin_chain = new Vector2[20];
				
				for(int i=0; i<20; i++)
				{
					sin_chain[i] = new Vector2((float)i, 5.0f * (float)Math.Sin(PhysicsUtility.GetRadian(i*20.0f)));
				}
					                           
				AddLineChainShape(sin_chain, 20);
					                           
			}
			
			// Create the righ part of sine curve
			//
			// Because discontinuity may happen for the normal of collision between left part and right part,
			//  and cautions may be necessary for that kind of cases.
			//
			// Therefore connection point should be carefully set in the case 
			// which shape is divided into several line parts.
			//
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

            // Create dynamic rigid sceneBodies
            {
                // Create a box-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(-10.0f, 15.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

                // Create a sphere-shaped dynamic rigid body
               {
                    PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
                    body.Position = new Vector2(0.0f, 15.0f);
                    body.Rotation = 0;
                }

                // Create a convex-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[4], 1.0f);
                    body.Position = new Vector2(10.0f, 15.0f);
                    body.Rotation = 0;
                }
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
			
			// Create the line shape
			// Only static rigid body is permitted for the line shape
			{
				PhysicsBody body = AddBody(SceneShapes[7], PhysicsUtility.FltMax);
				body.Position = new Vector2(-10, -10);
				body.Rotation = 0;
			}	

        }	
#endif
		
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

				if(con.hint != 0)
				{
					vertex[3 * i + 0] = vertex[3 * 0 + 0];
					vertex[3 * i + 1] = vertex[3 * 0 + 1];
					vertex[3 * i + 2] = vertex[3 * 0 + 2];
				}

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
			
				
			graphics.SetVertexBuffer(0, normalVert);
			
			for (uint i = 0; i < numPhysicsSolverPair; i++)
            {
                Vector2 g1 = new Vector2(solverPair[i].resA.X, solverPair[i].resA.Y);

                Vector2 g2 = new Vector2(solverPair[i].resA.X, solverPair[i].resA.Y)
					+ 1.0f * new Vector2(solverPair[i].normal.X, solverPair[i].normal.Y);
				
				{           
					float[] vertex = new float[]
					{
						g1.X, g1.Y, 0.0f,
						g2.X, g2.Y, 0.0f
					};

                	normalVert.SetVertices(0, vertex);

                	Matrix4 WorldMatrix = renderMatrix;

                	program.SetUniformValue(0, ref WorldMatrix);

                	Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
                	program.SetUniformValue(1, ref color);

                	//graphics.DrawArrays(DrawMode.Points, 0, 2);
                	graphics.DrawArrays(DrawMode.Lines, 0, 2);
				}
            }
		
			// if AABB rendering is necessary
			if(renderFlag){
	            // Draw AABB Bounding Box
	            graphics.SetVertexBuffer(0, aabbVert);
	
	            for (uint i = 0; i < numBody; i++)
	            {
	
	                Matrix4 scaleMatrix = new Matrix4(
	                     sceneBodies[i].aabbMax.X - sceneBodies[i].aabbMin.X, 0.0f, 0.0f, 0.0f,
	                     0.0f, sceneBodies[i].aabbMax.Y - sceneBodies[i].aabbMin.Y, 0.0f, 0.0f,
	                     0.0f, 0.0f, 1.0f, 0.0f,
	                     0.0f, 0.0f, 0.0f, 1.0f
	                 );
	
	                Matrix4 transMatrix = Matrix4.Translation(
	                    new Vector3(sceneBodies[i].aabbMin.X, sceneBodies[i].aabbMin.Y, 0.0f));
	
	                Matrix4 WorldMatrix = renderMatrix * transMatrix * scaleMatrix;
	                program.SetUniformValue(0, ref WorldMatrix);
	
	                Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
	                program.SetUniformValue(1, ref color);
	
	                graphics.DrawArrays(DrawMode.LineStrip, 0, 5);
	            }
			}
        }
	}
}


		
		

		

		
