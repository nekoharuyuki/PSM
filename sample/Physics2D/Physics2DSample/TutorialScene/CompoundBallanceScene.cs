/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// Basic compound rigid body creation
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
// This sample shows the way to make the compound rigid body in basic manner.
//
// Create necessary parts of compound rigid body by using usual creation of dynamic rigid body at first,
// and make those parts to combine to the one compound rigid body.
//
// To combine several rigid sceneBodies to one compound rigid body, it is necessary to call MergeBody(...).
//
// Please notice that the center of the gravity of the compound rigid body
//  is calculated by the center of the bounding boxy of the compound rigid body.
// 
// And this is also a point, and if one of parts is created as a static one, then the final compound
// rigid body also becomes the static rigid body.
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
    public class CompoundBallanceScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
        private VertexBuffer jointVert = null;
		
		private int[] compound1_index = new int[3];
		private int[] compound2_index = new int[3];
		
        public CompoundBallanceScene()
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
			
			// VertexBuffer used for joints debug rendering
            {
                jointVert = new VertexBuffer(2, VertexFormat.Float3);

                float[] vertex = new float[]
                { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                
                jointVert.SetVertices(0, vertex);
            }
        }

        ~CompoundBallanceScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if(vertices[i] != null)
                    vertices[i].Dispose();

            if(colVert != null) colVert.Dispose();
			if(jointVert != null) jointVert.Dispose();
			
        }
		
	
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "CompoundBallanceScene";
        
			// Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);
			
			Vector2 box_width = new Vector2(3.0f, 2.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
	
			float sphere_width = 3.0f;
            sceneShapes[3] = new PhysicsShape(sphere_width);
		
			Vector2 box_width2 = new Vector2(3.0f, 3.0f);
            sceneShapes[4] = new PhysicsShape(box_width2);
			
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
				
					
			// Create a compound rigid body
			//
			// It is allow to put a space among rigid sceneBodies constructing the compound shape.
			//
			// But please notice that the center of the gravity is calculated based on the
			// center of bounding box of the compound shape.
			//
			// If this center of the gravity is not set properly, 
			// it can cause the instability of the simulation.
			//
			for(int i=0; i<1; i++)
			{
				compound2_index[0] = numBody;
				
				// Create a dynamic rigid body whose shape is box (1.0 Kg)
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
				sceneBodies[numBody].position = new Vector2(-10.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
				
				compound2_index[1] = numBody;
				
				// Create a dynamic rigid body whose shape is box (1.0 Kg)
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 10.0f);
				sceneBodies[numBody].position = new Vector2(0.0f, 0.0f);
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(45.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;
				
				
				compound2_index[2] = numBody;

				// Create a dynamic rigid body whose shape is sphere (1.0 Kg)
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 20.0f);
				sceneBodies[numBody].position = new Vector2(10.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;		
				
							
				// Attach all parts to one compound rigid body
				//
				// At first prepare index list to add to the compound rigid body
				// And call MergeBody with several parameters
				//
				// sceneBodies --- this is not necessary to change, and jut give sceneBodies[...] of this PhysicsScene class
				// combination --- rigid body index list of sceneBodies[...]
				// 2 --- the number of rigid sceneBodies to add to  the compound rigid body
				// sceneShapes --- this is not necessary to change, and just give sceneShapessceneBodies[...] of this PhysicsScene class
				// compoundMap --- this is not necessary to change, and jut give compoundMapsceneBodies[...] of this PhysicsScene class
				Vector2 center_of_coumpound = new Vector2(0, 0);
				{
					uint[] combination = new uint[3];
					combination[0] = (uint)compound2_index[0];
					combination[1] = (uint)compound2_index[1];
					combination[2] = (uint)compound2_index[2];
					// MergeBody(...) returns the position of sceneBodies[compound2_index[0,1,2] which are shared now.
					center_of_coumpound = PhysicsBody.MergeBody(sceneBodies, combination, 3, sceneShapes, compoundMap);
				}
				
				// Attach this compound shape to the world by ball joint
				{
					PhysicsBody bodyA = sceneBodies[0];
					// Give first index of compound rigid body
					PhysicsBody bodyB = sceneBodies[compound2_index[0]];
				
					// please notice, original value of sceneBodies[compound2_index[0]].position was already changed by MergeBody(...)
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, new Vector2(0.0f, 0.0f), (uint)0, (uint)compound2_index[0]);
				
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
	
					// To make the "loose restriction rotation ", please set this value (lower than 1.0f)
					sceneJoints[numJoint].angleLim = 0;
					numJoint++;
				}
			}
			
			
			
			// Create test box				
			// Create a dynamic rigid body whose shape is box (1.0 Kg)
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 19.0f);
			sceneBodies[numBody].position = new Vector2(-10.0f, box_width.Y + box_width2.Y + 0.02f);
			sceneBodies[numBody].shapeIndex = 4;
			numBody++;
			
        }
		
#else

		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "CompoundBallanceScene";
        
           // Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);
			
			Vector2 box_width = new Vector2(3.0f, 2.0f);
            AddBoxShape(box_width);
	
			float sphere_width = 3.0f;
            AddSphereShape(sphere_width);
		
			Vector2 box_width2 = new Vector2(3.0f, 3.0f);
            AddBoxShape(box_width2);

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
				
					
			// Create a compound rigid body
			//
			// It is allow to put a space among rigid sceneBodies constructing the compound shape.
			//
			// But please notice that the center of the gravity is calculated based on the
			// center of bounding box of the compound shape.
			//
			// If this center of the gravity is not set properly, 
			// it can cause the instability of the simulation.
			//
			for(int i=0; i<1; i++)
			{
				PhysicsBody[] bodyList = {null, null, null};
				
				// Create a dynamic rigid body whose shape is box (1.0 Kg)
				bodyList[0] = AddBody(SceneShapes[2], 1.0f);
				bodyList[0].Position = new Vector2(-10.0f, 0.0f);
				
				// Create a dynamic rigid body whose shape is box (1.0 Kg)
				bodyList[1] = AddBody(SceneShapes[3], 10.0f);
				bodyList[1].Position = new Vector2(0.0f, 0.0f);
				bodyList[1].Rotation = PhysicsUtility.GetRadian(45.0f);
				
				// Create a dynamic rigid body whose shape is sphere (1.0 Kg)
				bodyList[2] = AddBody(SceneShapes[2], 20.0f);
				bodyList[2].Position = new Vector2(10.0f, 0.0f);

							
				// Attach all parts to one compound rigid body
				//
				// At first prepare index list to add to the compound rigid body
				// And call MergeBody with several parameters
				//
				// sceneBodies --- this is not necessary to change, and jut give sceneBodies[...] of this PhysicsScene class
				// combination --- rigid body index list of sceneBodies[...]
				// 2 --- the number of rigid sceneBodies to add to  the compound rigid body
				// sceneShapes --- this is not necessary to change, and just give sceneShapessceneBodies[...] of this PhysicsScene class
				// compoundMap --- this is not necessary to change, and jut give compoundMapsceneBodies[...] of this PhysicsScene class
				Vector2 center_of_coumpound = new Vector2(0, 0);
				
				// MergeBody(...) returns the position of sceneBodies[compound2_index[0,1,2] which are shared now.
				center_of_coumpound = MergeCompound(bodyList, bodyList.Length);
				
				// Attach this compound shape to the world by ball joint
				{
					PhysicsBody bodyA = SceneBodies[0];
					// Give first index of compound rigid body
					PhysicsBody bodyB = bodyList[0];
				
					// please notice, original value of sceneBodies[compound2_index[0]].position was already changed by MergeBody(...)
					PhysicsJoint joint = AddJoint(bodyA, bodyB, new Vector2(0.0f, 0.0f));
				
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
	
					// To make the "loose restriction rotation ", please set this value (lower than 1.0f)
					joint.AngleLim = 0;
				}
			}
			
			
			{
				// Create test box				
				// Create a dynamic rigid body whose shape is box (1.0 Kg)
				PhysicsBody body = AddBody(SceneShapes[4], 19.0f);
				body.Position = new Vector2(-10.0f, box_width.Y + box_width2.Y + 0.02f);
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
						
						if(i == compound2_index[0])
						{
							color = new Vector3(1.0f, 1.0f, 0.0f);
						}
						else if(i == compound2_index[2])
						{
							color = new Vector3(0.0f, 1.0f, 0.0f);
						}
						else
						{
							color = new Vector3(0.0f, 1.0f, 1.0f);
						}	
						
						
						// Check the sleep status here
						
						if (sceneBodies[i].sleep == true)
                        {
							color = new Vector3(1.0f, 1.0f, 1.0f);
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
			
			graphics.SetVertexBuffer(0, jointVert);
			
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
					float[] vertex = new float[]
					{
						g1.X, g1.Y, 0.0f,
						g2.X, g2.Y, 0.0f
					};

                	jointVert.SetVertices(0, vertex);

                	Matrix4 WorldMatrix = renderMatrix;

                	program.SetUniformValue(0, ref WorldMatrix);

                	Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
                	program.SetUniformValue(1, ref color);

                	graphics.DrawArrays(DrawMode.Points, 0, 2);
                	graphics.DrawArrays(DrawMode.Lines, 0, 2);
				}
            }
        }
	}
}


		
		

		

		
