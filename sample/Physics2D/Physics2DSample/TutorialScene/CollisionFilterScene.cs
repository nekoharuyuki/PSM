/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// The way to use collision filter property of the rigid body
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
// The operation "&" for collisionFilter of two rigid sceneBodies are not equals to zero,
// two rigid body's pair is ignored by the broad phase.
//
// [Physics2DEngine/Broadphase.cs]
// if ((ob1.collisionFilter & ob2.collisionFilter) != 0) return false;  (cancel collision detection if not equals to zero)
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
    public class CollisionFilterScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
        
        public CollisionFilterScene()
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
        }

        ~CollisionFilterScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if(vertices[i] != null)
                    vertices[i].Dispose();

            if(colVert != null) colVert.Dispose();
        }
		
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "CollisionFilterScene";
            
			// Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);
			
			Vector2 box_width = new Vector2(4.0f, 4.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
	
			float sphere_width = 4.0f;
            sceneShapes[3] = new PhysicsShape(sphere_width);
			
			// Convex Shape Setting (by user given points)
			// The center of vertex points should be origin(0, 0) of local coordinate
			{
				Vector2[] user_point = new Vector2[5];
	
	            for (int i = 0; i < 5; i++)
	            {
	                user_point[i] = 4.0f * new Vector2((float)Math.Cos(72.0f*(float)i/180.0f*PhysicsUtility.Pi), (float)Math.Sin(72.0f*(float)i/180.0f*PhysicsUtility.Pi));
	            }
				
				Vector2 center_point = new Vector2(0, 0);
				
				for(int i=0; i<5; i++)
					center_point += user_point[i];
				
				center_point *= (1.0f/5.0f);
				
				for(int i=0; i<5; i++)
					user_point[i] -= center_point;

            	sceneShapes[4] = new PhysicsShape(user_point, 5);
			}
			
            numShape = 5;
			

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
		
			// Box vs Box - no collision
			// Box vs Sphere - collision
			// Box vs Convex - no colllision 
			
			// Create a box with collision filter ( 1 << 1 )
			for(int i=0; i<2; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
				sceneBodies[numBody].position = new Vector2(-30.0f, -10 + 10.0f * (float)i);
				sceneBodies[numBody].shapeIndex = 2;
				sceneBodies[numBody].collisionFilter = (1 << 1);
				numBody++;
			}
			
			
			// Sphere vs Box - collision
			// Sphere vs Sphere - no collision
			// Sphere vs Convex - colllision 
			
			// Create a sphere with collision filter ( )
			for(int i=0; i<2; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(30.0f, -10 + 10.0f * (float)i);
				sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].collisionFilter = (1 << 2);
				numBody++;
			}
			
			// Convex vs Box - no collision
			// Convex vs Sphere - collision
			// Convex vs Convex - no colllision 
			
			// Create a sphere with collision filter ( )
			for(int i=0; i<2; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
				sceneBodies[numBody].position = new Vector2(0.0f, -10 + 10.0f * (float)i);
				sceneBodies[numBody].shapeIndex = 4;
				sceneBodies[numBody].collisionFilter = (1 << 3);
				numBody++;
			}
			
			
        }

#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "CollisionFilterScene";
            
            // Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);

			Vector2 box_width = new Vector2(4.0f, 4.0f);
			AddBoxShape(box_width);
	
			float sphere_width = 4.0f;
            AddSphereShape(sphere_width);
			
			// Convex Shape Setting (by user given points)
			// The center of vertex points should be origin(0, 0) of local coordinate
			{
				Vector2[] user_point = new Vector2[5];
	
	            for (int i = 0; i < 5; i++)
	            {
	                user_point[i] = 4.0f * new Vector2((float)Math.Cos(72.0f*(float)i/180.0f*PhysicsUtility.Pi), (float)Math.Sin(72.0f*(float)i/180.0f*PhysicsUtility.Pi));
	            }
				
				Vector2 center_point = new Vector2(0, 0);
				
				for(int i=0; i<5; i++)
					center_point += user_point[i];
				
				center_point *= (1.0f/5.0f);
				
				for(int i=0; i<5; i++)
					user_point[i] -= center_point;

            	AddConvexShape(user_point, 5);
			}

            // Create static walls to limit the range of action of active rigid sceneBodies
            {
				PhysicsBody body = null;
                // new PhysicsBody( "shape of the body",  "mass of the body(kg)" ) 
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

		
			// Box vs Box - no collision
			// Box vs Sphere - collision
			// Box vs Convex - no colllision 
			
			// Create a box with collision filter ( 1 << 1 )
			for(int i=0; i<2; i++)
			{
				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
				body.Position = new Vector2(-30.0f, -10 + 10.0f * (float)i);
				body.CollisionFilter = (1 << 1);
			}
			
			
			// Sphere vs Box - collision
			// Sphere vs Sphere - no collision
			// Sphere vs Convex - colllision 
			
			// Create a sphere with collision filter ( )
			for(int i=0; i<2; i++)
			{
				PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
				body.Position = new Vector2(30.0f, -10 + 10.0f * (float)i);
				body.CollisionFilter = (1 << 2);
			}
			
			// Convex vs Box - no collision
			// Convex vs Sphere - collision
			// Convex vs Convex - no colllision 
			
			// Create a sphere with collision filter ( )
			for(int i=0; i<2; i++)
			{
				PhysicsBody body = AddBody(SceneShapes[4], 1.0f);
				body.Position = new Vector2(0.0f, -10 + 10.0f * (float)i);
				body.CollisionFilter = (1 << 3);
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
						Vector3 color;
						
						switch(sceneBodies[i].collisionFilter)
						{
							case (1<<1):
								color = new Vector3(0.0f, 1.0f, 0.0f);
								break;
							case (1<<2):
								color = new Vector3(1.0f, 1.0f, 0.0f);
								break;
							case (1<<3):
								color = new Vector3(1.0f, 1.0f, 1.0f);
								break;
							default:
								color = new Vector3(0.0f, 1.0f, 1.0f);
								break;
						}
							
                        program.SetUniformValue(1, ref color);
                    }

                    if (sceneShapes[index].numVert == 0)
                        graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                    else
                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);

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
        }
	}
}


		
		

		

		
