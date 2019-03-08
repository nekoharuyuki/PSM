/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// The way to treat kinematic rigid body properly
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
// Please notice that kinematic rigid body also has sleep status and if it is considered as sleep status,
// there is no collision detection between kinematic rigid body and dynamic rigid body.
//
// When you want to make kinematic rigid body move, then please set postion, velocity, rotation and angularVelocity
// properly and do not forget to set it as non-sleep status.
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
    public class KinematicObjectScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
		
        private int index0, index1;
	
		private bool move = false;
		
		private long objectFrame = 0;
		
        public KinematicObjectScene()
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

        ~KinematicObjectScene()
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
			sceneName = "KinematicObjectScene";
			
            // Set the restitution coefficient a bit stronger
            restitutionCoeff = 0.8f;
			
			printPerf = true;
			
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
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

            //
            // Create kinematic rigid sceneBodies
            //
            // Kinematic rigid sceneBodies should be defined as dynamic objects at first,
            // and then those should be switched to kinematic objects by calling setKinematic().
            // Kinematic rigid sceneBodies can be changed to dynamic rigid sceneBodies by calling backToDynamic() again.
            //

			// Create a kinematic rigid body whose shape is box       
			{     
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);   
				sceneBodies[numBody].position = new Vector2(-10.0f, 10.0f);    
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);            
				sceneBodies[numBody].shapeIndex = 2;         
				sceneBodies[numBody].SetBodyKinematic();                   
				index0 = numBody++;  
			}
			
			
			for(int i=0; i<5; i++)
			{

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
				sceneBodies[numBody].position = new Vector2(-30.0f , 10.0f - (4.0f + 0.3f)*(float)i);
				sceneBodies[numBody].shapeIndex = 2;
				sceneBodies[numBody].collisionFilter = (uint)(1 << 1);
				sceneBodies[numBody].airFriction = 0.005f;				
				if(i==0)
				{
					index1 = numBody;
					sceneBodies[numBody].SetBodyKinematic();
				}
				numBody++;
			
				if(i!=0)
				{
					PhysicsBody bodyA = sceneBodies[numBody-2];
					PhysicsBody bodyB = sceneBodies[numBody-1];
					sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, 0.5f * (bodyA.position + bodyB.position), (uint)numBody-2, (uint)numBody-1);
					sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].angleLim =1;
					sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-30.0f);
					sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(30.0f);						
					numJoint++;
				}
			}

            // Create a stack with dynamic rigid sceneBodies	
            for (int i = 0; i < 8; i++)
			{
                for (int j = 0; j < 8 - i; j++)
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2((box_width.X + 0.1f) * i, -wall_height.Y) + new Vector2(0, wall_width.Y + box_width.Y + 0.005f) + new Vector2(0f, (2 * i + 0.005f) * box_width.Y);
                    sceneBodies[numBody].position += new Vector2((box_width.X * 2 + 0.1f * 2) * j, 0);
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
			SceneName = "KinematicObject";
			
            // Set the restitution coefficient a bit stronger
            RestitutionCoeff = 0.8f;
			
			PrintPerf = true;
			
			// Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
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

            //
            // Create kinematic rigid sceneBodies
            //
            // Kinematic rigid sceneBodies should be defined as dynamic objects at first,
            // and then those should be switched to kinematic objects by calling setKinematic().
            // Kinematic rigid sceneBodies can be changed to dynamic rigid sceneBodies by calling backToDynamic() again.
            //

			// Create a kinematic rigid body whose shape is box       
			{     
				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);   
				body.Position = new Vector2(-10.0f, 10.0f);    
				body.Rotation = PhysicsUtility.GetRadian(30.0f);               
				body.SetBodyKinematic();                   
				index0 = NumBody-1;  
			}
			
			
			for(int i=0; i<5; i++)
			{

				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
				body.Position = new Vector2(-30.0f , 10.0f - (4.0f + 0.3f)*(float)i);
				body.CollisionFilter = (uint)(1 << 1);
				body.AirFriction = 0.005f;
				
				if(i==0)
				{
					index1 = NumBody-1;
					body.SetBodyKinematic();
				}
			
				if(i!=0)
				{
					PhysicsBody bodyA = SceneBodies[NumBody-2];
					PhysicsBody bodyB = SceneBodies[NumBody-1];
					PhysicsJoint joint = AddJoint(bodyA, bodyB, 0.5f * (bodyA.Position + bodyB.Position));
					joint.Axis1Lim = new Vector2(1.0f, 0.0f);
					joint.Axis2Lim = new Vector2(0.0f, 1.0f);
					joint.AngleLim =1;
					joint.AngleLower = PhysicsUtility.GetRadian(-30.0f);
					joint.AngleUpper = PhysicsUtility.GetRadian(30.0f);
				}
			}

            // Create a stack with dynamic rigid sceneBodies	
            for (int i = 0; i < 8; i++)
			{
                for (int j = 0; j < 8 - i; j++)
                {
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2((box_width.X + 0.1f) * i, -wall_height.Y) + new Vector2(0, wall_width.Y + box_width.Y + 0.005f) + new Vector2(0f, (2 * i + 0.005f) * box_width.Y);
                    body.Position += new Vector2((box_width.X * 2 + 0.1f * 2) * j, 0);
                }
            }
			 
        }
		
#endif
		
		// Move the kinematic rigid body forcely, and update position,
		// velocity, rotation and angularVelocity at the same time.
		// Kinematic rigid body also has sleep status, but it is set as false before moving.
		public override void UpdateFuncBeforeSim ()
		{
			if(move)	
			{
				objectFrame++;
				{
					sceneBodies[index0].MoveTo(new Vector2(-10.0f, 10.0f) + new Vector2(25.0f*(float)Math.Sin(objectFrame/180.0f*PhysicsUtility.Pi), 0.0f),
					                   sceneBodies[index0].rotation + PhysicsUtility.GetRadian (2.0f),
					                   simDt);
					// The above is same thing as followings
					//
					//Vector2 oldPos = sceneBodies[index0].position;
					//float oldAngle = sceneBodies[index0].rotation;
					//sceneBodies[index0].position = new Vector2(-10.0f, 10.0f) + new Vector2(25.0f*(float)Math.Sin(objectFrame/180.0f*PhysicsUtility.Pi), 0.0f);
					//sceneBodies[index0].velocity = (sceneBodies[index0].position - oldPos)/simDt;
					//sceneBodies[index0].rotation += PhysicsUtility.GetRadian (2.0f);
					//sceneBodies[index0].angularVelocity = (sceneBodies[index0].rotation - oldAngle)/simDt;
					//sceneBodies[index0].sleep = false;
				}
				
				{
					sceneBodies[index1].MoveTo(new Vector2(-30.0f, 10.0f) + new Vector2(10.0f*(float)Math.Sin(objectFrame/180.0f*PhysicsUtility.Pi), 0.0f),
					                   sceneBodies[index1].rotation,
					                   simDt);
					// The above is same thing as followings
					//
					//Vector2 oldPos = sceneBodies[index1].position;
					//sceneBodies[index1].position = new Vector2(-30.0f, 10.0f) + new Vector2(10.0f*(float)Math.Sin(objectFrame/180.0f*PhysicsUtility.Pi), 0.0f);
					//sceneBodies[index1].velocity = (sceneBodies[index1].position - oldPos)/simDt;
					//sceneBodies[index1].sleep = false;	
				}
			}
		}
		
        // Game controller handling
        public override void KeyboardFunc(GamePadButtons button)
        {
            switch (button)
            {
                case GamePadButtons.Square:
					move = !move;
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
					
					if(sceneBodies[i].sleep == true)
					{
                        Vector3 color = new Vector3(0.0f, 1.0f, 0.0f);
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


		
		

		

		
