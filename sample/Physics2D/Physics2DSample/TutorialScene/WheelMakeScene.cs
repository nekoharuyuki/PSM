/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// The way to make the wheel by using compound rigid body & ball joint
//


// [How to set up the simulation scene]
//
// (1) Make an simulation scene
// (2) Arrange coefficients for contact solver or gravity etc...
// (3) Prepare collision shapes for rigid sceneBodies
// (4) Make the rigid body instances by specifying collision shapes and masses 
// (5) Set the position, rotation, velocity and angular velocity based on the scene
//
// Recommendation of scales
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
// Wheel can be composed of compound shape + ball joint, or can be composed of fixed joint + ball joint.
// But the latter case can become relatively unstable when heavy object collide with the wheel.
//
// Please notice that there are some cases which Solver phase of simulation cannot solve the joint 
// completely within pre-defined iteration number of calculation.
//

// Use direct access to structure of simulation data to set up initial scene
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
    public class WheelMakeScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
        
		int start_index1 = 0, end_index1 = 0;
		int start_index2 = 0, end_index2 = 0;
		
        public WheelMakeScene()
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

        ~WheelMakeScene()
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
			sceneName = "WheelMakeScene";
			
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
			
			float sphere_width = 1.5f;
			sceneShapes[3] = new PhysicsShape(sphere_width);
			
			Vector2 wheel_edge = new Vector2(1.0f, 1.0f);
			sceneShapes[4] = new PhysicsShape(wheel_edge);
			
			float wheel_radius = 8.0f;
			sceneShapes[5] = new PhysicsShape(wheel_radius - wheel_edge.X);
			
			
            numShape = 6;
			
			
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
			
			// This is a recommendation
			// This is more stable way ( wheel with compound shape + ball joint )
			//
			// Create a compound shape at first (center component + edge components)
			// And then attach it to the world by the ball joint
			//
			{
				const int divide_num = 9;
				Vector2 center = new Vector2(-20, 0);
				
				start_index1 = numBody;
				
				// Create the center part of the wheel
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 5.0f);
				sceneBodies[numBody].position = center;
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 5;
				sceneBodies[numBody].picking = false; // disable picking
				numBody++;
				
				// Create the edge part of the wheel
				for(int i=0; i<divide_num; i++)
				{
					float angle = (float)i*(360.0f/(float)divide_num)/180.0f*PhysicsUtility.Pi;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 0.3f);
					sceneBodies[numBody].position = center + wheel_radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
					sceneBodies[numBody].rotation = angle;
					sceneBodies[numBody].shapeIndex = 4;
					sceneBodies[numBody].picking = false; // disable picking					
					numBody++;
				}
				
				end_index1 = numBody-1;

				
				// Attach all parts to one compound shape
				{
					uint[] combination = new uint[divide_num + 1];
					
					for(int i=start_index1; i<=end_index1; i++) 
						combination[i-start_index1] = (uint)i;
					
					PhysicsBody.MergeBody(sceneBodies, combination, divide_num+1, sceneShapes, compoundMap);
				}
				
				// Attach this compound shape to the world by ball joint
				// Ball joint with rotation's free
				{
					PhysicsBody b1 = sceneBodies[0];
					PhysicsBody b2 = sceneBodies[start_index1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, b2.position, (uint)0, (uint)start_index1);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 0;
					sceneJoints[numJoint].friction = 0.001f;
					numJoint++;
				}
			}
			
			
			// This is relatively unstable way ( wheel with fixed joint(locked ball joint) + ball joint )
			// because fixed joint cannot solve the joint solver for heavy object within pre-defined iteration 
			//
			// Create edge components and attach those to the center component by fixed joint
			// And then attach the center component to the world by the ball joint
			//	
			{
				const int divide_num = 9;
				Vector2 center = new Vector2(20, 0);
				
				start_index2 = numBody;
								
				// Create the center part of the wheel
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
				sceneBodies[numBody].position = center;
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 5;
				sceneBodies[numBody].collisionFilter = (1 << 1);	// to avoid collision between center and edge compontnt
				sceneBodies[numBody].picking = false; // disable picking				
				numBody++;
				
				// Create the edge part of the wheel
				for(int i=0; i<divide_num; i++)
				{
					float angle = (float)i*(360.0f/(float)divide_num)/180.0f*PhysicsUtility.Pi;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 0.3f);
					sceneBodies[numBody].position = center + wheel_radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
					sceneBodies[numBody].rotation = angle;
					sceneBodies[numBody].shapeIndex = 4;
					sceneBodies[numBody].collisionFilter = (1 << 1); // to avoid collision between center and edge compontnt
					sceneBodies[numBody].picking = false; // disable picking	
					numBody++;
					
					// Fixed to center part of the wheel by fixed joint (locked ball joint)
					PhysicsBody b1 = sceneBodies[start_index2];
					PhysicsBody b2 = sceneBodies[numBody-1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, b2.position, (uint)start_index2, (uint)numBody-1);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 1;
					numJoint++;
				}
				
				end_index2 = numBody-1;
				
				// Attach this compound shape to the world by ball joint
				{
					PhysicsBody b1 = sceneBodies[0];
					PhysicsBody b2 = sceneBodies[start_index2];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, b2.position, (uint)0, (uint)start_index2);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim = 0;
					sceneJoints[numJoint].friction = 0.001f;
					numJoint++;
				}
			}
			
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 5.0f);
			sceneBodies[numBody].position = new Vector2(-20, 20);
			sceneBodies[numBody].rotation = 0;
			sceneBodies[numBody].shapeIndex = 3;
			numBody++;
			
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 5.0f);
			sceneBodies[numBody].position = new Vector2(20, 20);
			sceneBodies[numBody].rotation = 0;
			sceneBodies[numBody].shapeIndex = 3;
			numBody++;
			
			
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 5.0f);
         	sceneBodies[numBody].position = new Vector2(0, -10);
			sceneBodies[numBody].rotation = 0;
			sceneBodies[numBody].shapeIndex = 2;
			numBody++;
			
			
        }
#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "WheelMakeScene";
			
			// Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            AddBoxShape(box_width);
			
			float sphere_width = 1.5f;
			AddSphereShape(sphere_width);
			
			Vector2 wheel_edge = new Vector2(1.0f, 1.0f);
			AddBoxShape(wheel_edge);
			
			float wheel_radius = 8.0f;
			AddSphereShape(wheel_radius - wheel_edge.X);

			
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
			
			// This is a recommendation
			// This is more stable way ( wheel with compound shape + ball joint )
			//
			// Create a compound shape at first (center component + edge components)
			// And then attach it to the world by the ball joint
			//
			{
				const int divide_num = 9;
				Vector2 center = new Vector2(-20, 0);

				PhysicsBody[] mergeList =new PhysicsBody[divide_num+1];
				
				start_index1 = NumBody;
				
				// Create the center part of the wheel
				mergeList[0] = AddBody(SceneShapes[5], 5.0f);
				mergeList[0].Position = center;
				mergeList[0].Rotation = 0;
				mergeList[0].Picking = false; // disable picking
				
				// Create the edge part of the wheel
				for(int i=0; i<divide_num; i++)
				{
					float angle = (float)i*(360.0f/(float)divide_num)/180.0f*PhysicsUtility.Pi;
					mergeList[i+1] = AddBody(SceneShapes[4], 0.3f);
					mergeList[i+1].Position = center + wheel_radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
					mergeList[i+1].Rotation = angle;
					mergeList[i+1].Picking = false; // disable picking					
				}
				
				MergeCompound(mergeList, mergeList.Length);
				
				// Attach this compound shape to the world by ball joint
				// Ball joint with rotation's free
				{
					PhysicsBody b1 = SceneBodies[0];
					PhysicsBody b2 = mergeList[0];
					PhysicsJoint joint = AddJoint(b1, b2, b2.Position);
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 0;
					joint.Friction = 0.001f;
				}
			}
			
			
			// This is relatively unstable way ( wheel with fixed joint(locked ball joint) + ball joint )
			// because fixed joint cannot solve the joint solver for heavy object within pre-defined iteration 
			//
			// Create edge components and attach those to the center component by fixed joint
			// And then attach the center component to the world by the ball joint
			//	
			{
				const int divide_num = 9;
				Vector2 center = new Vector2(20, 0);

				PhysicsBody[] mergeList =new PhysicsBody[divide_num+1];
					
				start_index2 = NumBody;
								
				// Create the center part of the wheel
				mergeList[0] = AddBody(SceneShapes[5], 1.0f);
				mergeList[0].Position = center;
				mergeList[0].Rotation = 0;
				mergeList[0].CollisionFilter = (1 << 1);	// to avoid collision between center and edge compontnt
				mergeList[0].Picking = false; // disable picking				
				
				// Create the edge part of the wheel
				for(int i=0; i<divide_num; i++)
				{
					float angle = (float)i*(360.0f/(float)divide_num)/180.0f*PhysicsUtility.Pi;
					mergeList[i+1] = AddBody(SceneShapes[4], 0.3f);
					mergeList[i+1].Position = center + wheel_radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
					mergeList[i+1].Rotation = angle;
					mergeList[i+1].CollisionFilter = (1 << 1); // to avoid collision between center and edge compontnt
					mergeList[i+1].Picking = false; // disable picking	
					
					// Fixed to center part of the wheel by fixed joint (locked ball joint)
					PhysicsBody b1 = mergeList[0];
					PhysicsBody b2 = mergeList[i+1];
					PhysicsJoint joint = AddJoint(b1, b2, b2.Position);
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 1;
				}
				
				end_index2 = NumBody-1;
				
				// Attach this compound shape to the world by ball joint
				{
					PhysicsBody b1 = SceneBodies[0];
					PhysicsBody b2 = SceneBodies[start_index2];
					PhysicsJoint joint = AddJoint(b1, b2, b2.Position);
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim = 0;
					joint.Friction = 0.001f;
				}
			}
			
			{
				PhysicsBody body = AddBody(SceneShapes[3], 5.0f);
				body.Position = new Vector2(-20, 20);
				body.Rotation = 0;
			}
			
			{
				PhysicsBody body = AddBody(SceneShapes[3], 5.0f);
				body.Position = new Vector2(20, 20);
				body.Rotation = 0;
			}
			
			{
				PhysicsBody body = AddBody(SceneShapes[2], 5.0f);
         		body.Position = new Vector2(0, -10);
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
						
						if((i>=start_index1)&&(i<=end_index1))
						{
							color = new Vector3(1.0f, 1.0f, 0.0f);
						}
						else if((i>=start_index2)&&(i<=end_index2))
						{
							color = new Vector3(0.0f, 1.0f, 0.0f);
						}
						else
						{
							color = new Vector3(0.0f, 1.0f, 1.0f);
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
			
			
			// Draw the center of the joint
			//
			// the center of the joint is saved as two patterns
			//
			// One is relative position to bodyA of the joint (bodyA <-> bodyB)
			// The other is relative position to bodyB of the joint (body A <-> bodyB)
			//
			// In the that the solver phase of the joint is perfectly solved,
			// the center of joint calculated by the relative position to bodyA is equal to 
			// the center of joint calculated by the relative position to bodyB.
			//
			// But it often happens such that those two positions have different values
			// because the force (or torque) is applied to the joint and the solver phase cannot solve the joint 
			// perfectly within the limited iteration of the calculation.
			//
			
            for (uint i = 0; i < numJoint; i++)
            {
                PhysicsJoint jnt = sceneJoints[i];
                PhysicsBody b1 = sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[i].totalIndex)];
                PhysicsBody b2 = sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[i].totalIndex)];

                float angle1 = b1.rotation;
                float angle2 = b2.rotation;

                Vector2 r1 = jnt.localAnchor1;
                Vector2 r2 = jnt.localAnchor2;
				
				// g1 is calculated by the relative position to bodyA
                Vector2 g1 = b1.position + new Vector2((float)System.Math.Cos(angle1) * r1.X - (float)System.Math.Sin(angle1) * r1.Y,
                    (float)System.Math.Sin(angle1) * r1.X + (float)System.Math.Cos(angle1) * r1.Y);
				
				// g2 is calculated by the relative position to bodyB
                Vector2 g2 = b2.position + new Vector2((float)System.Math.Cos(angle2) * r2.X - (float)System.Math.Sin(angle2) * r2.Y,
                    (float)System.Math.Sin(angle2) * r2.X + (float)System.Math.Cos(angle2) * r2.Y);

				{  
					Matrix4 transMatrix = Matrix4.Translation(new Vector3(g1.X, g1.Y, 0.0f));
					
					Matrix4 WorldMatrix = renderMatrix * transMatrix;
					program.SetUniformValue(0, ref WorldMatrix);
	
					Vector3 color = new Vector3(0.0f, 1.0f, 0.0f);
					program.SetUniformValue(1, ref color);
					
					graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
				}
				
				{
					Matrix4 transMatrix = Matrix4.Translation(new Vector3(g2.X, g2.Y, 0.0f));
					
					
					Matrix4 WorldMatrix = renderMatrix * transMatrix;
					program.SetUniformValue(0, ref WorldMatrix);
	
					Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
					program.SetUniformValue(1, ref color);
					
					graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
				}
            }
        }
	}
}


		
		

		

		
