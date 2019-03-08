/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// The way to make the simple vehicle by using joint setting
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
// Simple vehicle can be composed of two spheres + two joints.
// But position constraint of joint should be relaxed for Y-axis direction because of the suspension effect.
// It may not be necessary to render this suspention part as actual position of the suspension.
//
// Course is composed of box and sine curve.
// But sine curve does not have the width and it is easy for interpenetration to happen,
// and to avoid this interpenetration, similar sine curve is duplicated under the original one.
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
    public class VehicleMakeScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
		
		public Vector2 eyePos = new Vector2(0.0f, 0.0f);
		
		private int vehicle_body = 0;
		
		private int[] vehicle_tire = {0, 0};
		
        public VehicleMakeScene()
        {
            // Simulation Scene Set Up
            InitScene();

            // Setup for Rendering Object
            for (int i = 0; i < numShape; i++)
            {
                if (sceneShapes[i].numVert == 0)
                {
                    vertices[i] = new VertexBuffer(38, VertexFormat.Float3);
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

        ~VehicleMakeScene()
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
			sceneName = "VehicleMakeScene";
			
			// Use one by one method to define scene property for each rigid bodies
			uniqueProperty = false;
			
			printPerf = true;
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(400, 2);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(2.5f, 7.5f);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(1.0f, 1.0f);
            sceneShapes[2] = new PhysicsShape(box_width);
			
			float sphere_width = 0.5f;
			sceneShapes[3] = new PhysicsShape(sphere_width);
			
			Vector2 wheel_edge = new Vector2(0.5f, 0.5f);
			sceneShapes[4] = new PhysicsShape(wheel_edge);
			
			float wheel_radius = 2.0f;
			sceneShapes[5] = new PhysicsShape(wheel_radius - wheel_edge.X);
			
			Vector2 wheel_body = new Vector2(2.5f, 1.0f);
			sceneShapes[6] = new PhysicsShape(wheel_body);
			
			{
				Vector2[] sin_chain = new Vector2[25];
				
				for(int i=0; i<25; i++)
				{
					sin_chain[i] = new Vector2(5.0f*i, 20.0f * (float)Math.Sin(PhysicsUtility.GetRadian((float)i/25.0f*180.0f)));
				}
					                           
				sceneShapes[7] = new PhysicsShape(sin_chain, 25, true);		                           
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
                sceneBodies[numBody].position = new Vector2(0, wall_height.Y + 25.0f);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 0;
                numBody++;
            }
					
			// Create sine curve course
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);
            	sceneBodies[numBody].position = new Vector2(-200, -15);
            	sceneBodies[numBody].rotation = 0;
            	sceneBodies[numBody].shapeIndex = 7;
				numBody++;
	
				// Curve dose not have the width and it is easy for interpenetration to happen
				// ,and to avoid this interpenetration, similar sine curve is dupulicated under the original curve.
				// But it will waste simulation cost too.
				if(true)
				{				
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);
	            	sceneBodies[numBody].position = new Vector2(-200, -16);
	            	sceneBodies[numBody].rotation = 0;
	            	sceneBodies[numBody].shapeIndex = 7;
					numBody++;					
				}
			}
				
			// Create Sine Curve Course
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);
            	sceneBodies[numBody].position = new Vector2(-250, -15);
            	sceneBodies[numBody].rotation = 0;
            	sceneBodies[numBody].shapeIndex = 7;
				numBody++;
				
				// Curve dose not have the width and it is easy for interpenetration to happen
				// ,and to avoid this interpenetration, similar sine curve is dupulicated under the original curve
				// But it will waste simulation cost too
				if(true)
				{
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);
	            	sceneBodies[numBody].position = new Vector2(-250, -16);
	            	sceneBodies[numBody].rotation = 0;
	            	sceneBodies[numBody].shapeIndex = 7;
					numBody++;
				}
			}
		
			
			// Create sine curve course
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], PhysicsUtility.FltMax);
	            sceneBodies[numBody].position = new Vector2(-162, 1.0f);
	            sceneBodies[numBody].rotation = 0;
	            sceneBodies[numBody].shapeIndex = 6;
				sceneBodies[numBody].collisionFilter = 0;
				numBody++;	
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], PhysicsUtility.FltMax);
	            sceneBodies[numBody].position = new Vector2(-95, -6.5f);
	            sceneBodies[numBody].rotation = 0;
	            sceneBodies[numBody].shapeIndex = 6;
				sceneBodies[numBody].collisionFilter = 1;
				numBody++;
	
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], PhysicsUtility.FltMax);
	            sceneBodies[numBody].position = new Vector2(-230, -6.5f);
	            sceneBodies[numBody].rotation = 0;
	            sceneBodies[numBody].shapeIndex = 6;
				sceneBodies[numBody].collisionFilter = 1;
				numBody++;
			}

			
			// Create a simple vehicle
			{
				
				Vector2 startPos = new Vector2(90.0f, 0.0f);
				
				// Create the body (suspension part)
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 10.0f);
	            sceneBodies[numBody].position = new Vector2(-2.5f, 1.5f) + startPos;
	            sceneBodies[numBody].rotation = 0;
	            sceneBodies[numBody].shapeIndex = 6;
				sceneBodies[numBody].collisionFilter = 1;
				vehicle_body = numBody++;
	
				for(int tire = 0; tire < 2; tire++)
				{
					Vector2 center = new Vector2(-5.0f + 5.0f*tire, 0.5f) + startPos;
					
					// Create the wheel
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 2.0f);
					sceneBodies[numBody].position = center;
					sceneBodies[numBody].rotation = 0;
					sceneBodies[numBody].shapeIndex = 5;
					sceneBodies[numBody].collisionFilter = 1;
					// Give a higher tangent friction to the wheel 
					sceneBodies[numBody].tangentFriction = (0.5f*0.5f)/0.3f;	
					vehicle_tire[tire] = numBody++;
				
					// Set the joint and give impulse for rotation
					{
						PhysicsBody b1 = sceneBodies[vehicle_body];
						PhysicsBody b2 = sceneBodies[numBody-1];
						sceneJoints[numJoint] = new PhysicsJoint(b1, b2, b2.position, (uint)vehicle_body, (uint)(numBody-1));
						sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
						sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 0.3f);
						sceneJoints[numJoint].angleLim = 0;
						sceneJoints[numJoint].impulseRotMotor = sceneBodies[numBody-1].Inertia * PhysicsUtility.GetRadian(45.0f);
						numJoint++;
					}		
				}
			}

			// Create dust box
			for(int i = 0; i < 20; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
				sceneBodies[numBody].position = new Vector2(-200.0f + 5.0f*i, 7.0f);
				sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
			}
			
			eyePos = sceneBodies[vehicle_body].position;
		
			return;
			
        }
        
#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "VehicleMakeScene";
			
			// Use one by one method to define scene property for each rigid bodies
			UniqueProperty = false;
			
			PrintPerf = true;
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting AddBoxShape( "width", "height" )
            Vector2 wall_width = new Vector2(400, 2);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(2.5f, 7.5f);
            AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(1.0f, 1.0f);
            AddBoxShape(box_width);
			
			float sphere_width = 0.5f;
			AddSphereShape(sphere_width);
			
			Vector2 wheel_edge = new Vector2(0.5f, 0.5f);
			AddBoxShape(wheel_edge);
			
			float wheel_radius = 2.0f;
			AddSphereShape(wheel_radius - wheel_edge.X);
			
			Vector2 wheel_body = new Vector2(2.5f, 1.0f);
			AddBoxShape(wheel_body);
			
			{
				Vector2[] sin_chain = new Vector2[25];
				
				for(int i=0; i<25; i++)
				{
					sin_chain[i] = new Vector2(5.0f*i, 20.0f * (float)Math.Sin(PhysicsUtility.GetRadian((float)i/25.0f*180.0f)));
				}
					                           
				AddLineChainShape(sin_chain, 25);		                           
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
                body.Position = new Vector2(0, wall_height.Y + 25.0f);
                body.Rotation = 0;
            }
					
			// Create sine curve course
			{
				PhysicsBody body = null;
				
				body = AddBody(SceneShapes[7], PhysicsUtility.FltMax);
            	body.Position = new Vector2(-200, -15);
            	body.Rotation = 0;
	
				// Curve dose not have the width and it is easy for interpenetration to happen
				// ,and to avoid this interpenetration, similar sine curve is dupulicated under the original curve.
				// But it will waste simulation cost too.
				if(true)
				{				
					body = AddBody(SceneShapes[7], PhysicsUtility.FltMax);
	            	body.Position = new Vector2(-200, -16);
	            	body.Rotation = 0;					
				}
			}
				
			// Create Sine Curve Course
			{
				PhysicsBody body = null;
				
				body = AddBody(SceneShapes[7], PhysicsUtility.FltMax);
            	body.Position = new Vector2(-250, -15);
            	body.Rotation = 0;
				
				// Curve dose not have the width and it is easy for interpenetration to happen
				// ,and to avoid this interpenetration, similar sine curve is dupulicated under the original curve
				// But it will waste simulation cost too
				if(true)
				{
					body = AddBody(SceneShapes[7], PhysicsUtility.FltMax);
	            	body.Position = new Vector2(-250, -16);
	            	body.Rotation = 0;
				}
			}
		
			
			// Create sine curve course
			{
				PhysicsBody body = null;
				
				body = AddBody(SceneShapes[6], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-162, 1.0f);
	            body.Rotation = 0;
				body.CollisionFilter = 0;

				body = AddBody(SceneShapes[6], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-95, -6.5f);
	            body.Rotation = 0;
				body.CollisionFilter = 1;
	
				body = AddBody(SceneShapes[6], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-230, -6.5f);
	            body.Rotation = 0;
				body.CollisionFilter = 1;
			}

			
			// Create a simple vehicle
			{
				
				Vector2 startPos = new Vector2(90.0f, 0.0f);
				
				// Create the body (suspension part)
				PhysicsBody body = AddBody(SceneShapes[6], 10.0f);
	            body.Position = new Vector2(-2.5f, 1.5f) + startPos;
	            body.Rotation = 0;
				body.CollisionFilter = 1;
				vehicle_body = NumBody-1;
	
				for(int i = 0; i < 2; i++)
				{
					Vector2 center = new Vector2(-5.0f + 5.0f*i, 0.5f) + startPos;
					
					// Create the wheel
					PhysicsBody tire = AddBody(SceneShapes[5], 2.0f);
					tire.Position = center;
					tire.Rotation = 0;
					tire.CollisionFilter = 1;
					// Give a higher tangent friction to the wheel 
					tire.TangentFriction = (0.5f*0.5f)/0.3f;	
					vehicle_tire[i] = NumBody-1;
				
					// Set the joint and give impulse for rotation
					{
						PhysicsJoint joint = AddJoint(body, tire, tire.Position);
						joint.Axis1Lim = new Vector2(1.0f, 0.0f);
						joint.Axis2Lim = new Vector2(0.0f, 0.3f);
						joint.AngleLim = 0;
						joint.ImpulseRotMotor = tire.Inertia * PhysicsUtility.GetRadian(45.0f);
					}		
				}
			}

			// Create dust box
			for(int i = 0; i < 20; i++)
			{
				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
				body.Position = new Vector2(-200.0f + 5.0f*i, 7.0f);
				body.Rotation = 0;
			}
			
			eyePos = SceneBodies[vehicle_body].Position;
		
			return;
			
        }		
#endif
		
		public override void UpdateFuncBeforeSim ()
		{
			// Eye position is moved related to the position of the vehicle
			eyePos += 0.1f * (sceneBodies[vehicle_body].position - eyePos);
			
			// Clear the velocity of vehicle at the left end
			if(sceneBodies[vehicle_body].position.X < -300.0f)
			{
				sceneJoints[0].impulseRotMotor = 0.0f;
				sceneJoints[1].impulseRotMotor = 0.0f;
			}

        }
		
		// To attach the vehicle to the ground, 
		// higher external force is given to the vehicle in addition to the usual gravity.
		public override void UpdateFuncAfterSim ()
		{
			sceneBodies[vehicle_body].force = sceneBodies[vehicle_body].mass * new Vector2(0, -20.0f);
			sceneBodies[vehicle_tire[0]].force = sceneBodies[vehicle_tire[0]].mass * new Vector2(0, -20.0f);
			sceneBodies[vehicle_tire[1]].force = sceneBodies[vehicle_tire[1]].mass * new Vector2(0, -20.0f);
        }
		
		
		// Line Rendering for Object
        private void MakeLineListConvex(PhysicsShape con, VertexBuffer vertices)
        {

            if (con.numVert == 0)
            {
                float[] vertex = new float[3 * 38];

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
				i++;
				
                vertex[3 * i + 0] = vertex[3 * (i/2) + 0];
                vertex[3 * i + 1] = vertex[3 * (i/2) + 1];
                vertex[3 * i + 2] = vertex[3 * (i/2) + 2];
		
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
                        graphics.DrawArrays(DrawMode.LineStrip, 0, 38);
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


		
		

		

		
