/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
//  The way to check the collision by the query for contact
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


// [Tips and notice]
//
// This sample shows how to detect the collision in which you are interested. 
// 
// Query for contact will answer whether there is a colllision between one and the other
// in which you are interested in.
//
// Please notice that it may take a lot of computational cost when you use 
// this query for contact many times for each frame.
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
    public class QueryContactScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
        
		int coll_target1=0, coll_target2=1;
		
        public QueryContactScene()
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

        ~QueryContactScene()
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
			sceneName = "QueryContactScene";
			
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(3.0f, 3.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
			
            numShape = 3;
			

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

            // Create three dynamic rigid sceneBodies
			//
			// Will catch the collision between coll_target1 & coll_target2 by QueryContact(...)
			//
			// Please check DrawAllBody function inside this source code too.
			//
            {
                // Create a dynamic rigid body whose shape is box (1.0 Kg)
				// 1.0 Kg is typical mass of rigid body for stable simulation
                {
					coll_target1 = numBody;
					
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(-20.0f, 15.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    numBody++;
                }

   				{
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(0.0f, 15.0f);
                 	sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex =2;
                    numBody++;
                }

                {
					coll_target2 = numBody;
					
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(20.0f, 15.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    numBody++;
                }
			}
        }

#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "QueryContactScene";
			
			// Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(3.0f, 3.0f);
            AddBoxShape(box_width);

			
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

            // Create three dynamic rigid sceneBodies
			//
			// Will catch the collision between coll_target1 & coll_target2 by QueryContact(...)
			//
			// Please check DrawAllBody function inside this source code too.
			//
            {
                // Create a dynamic rigid body whose shape is box (1.0 Kg)
				// 1.0 Kg is typical mass of rigid body for stable simulation
                {
					coll_target1 = NumBody;
					
                   	PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(-20.0f, 15.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

   				{
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(0.0f, 15.0f);
                 	body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

                {
					coll_target2 = NumBody;
					
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(20.0f, 15.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }
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
			
			
			
			// Check the collision between coll_target1 & coll_target2.
			// When there is collision, color of all objects will be changed.
			bool isCollision = QueryContact((uint)coll_target1, (uint)coll_target2);
				
			if(isCollision)
			{
				Console.WriteLine("Contact (5<=>6) found");
			}
			
			
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
					
					Vector3 color;
					
                    if (i == click_index)
                    {
                        color = new Vector3(1.0f, 0.0f, 0.0f);   
					}
					else if(isCollision)
					{
						color = new Vector3(1.0f, 1.0f, 0.0f);
					}
					else if((i == coll_target1) || (i == coll_target2))
					{
						color = new Vector3(0.0f, 1.0f, 0.0f);
					}
					else
					{
						color = new Vector3(0.0f, 1.0f, 1.0f);
							
					}
					
					program.SetUniformValue(1, ref color);
					
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


		
		

		

		
