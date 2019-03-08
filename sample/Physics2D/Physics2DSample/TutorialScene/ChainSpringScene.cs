/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// Basic chain spring creation
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
// One important thing is that spring may become unstable when parameters are not set properly.
// And it is difficult for rigid bodies to go into sleep status
// when it is linked to the spring because there is small movement every time by force of spring.
// Pleare take care about elastic and dumping parameter setting.
//
// Spring has two anchor points and this is different from joint.
// And another differnt point is that spring does not treat angles of rigid bodies.
// When there is no friction of rigid body, rigid body will continue to rotate once it starts to rotate.
//
// Calculation cost of the spring is smaller thant calculation of the joint.
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
    public class ChainSpringScene : PhysicsScene
	{	
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
        
        // VertexBuffer used for rendering springs
        private VertexBuffer springVert = null;
		
        public ChainSpringScene()
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
                springVert = new VertexBuffer(2, VertexFormat.Float3);

                float[] vertex = new float[]
                { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                
                springVert.SetVertices(0, vertex);
            }
        }

        ~ChainSpringScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if(vertices[i] != null)
                    vertices[i].Dispose();

            if(colVert != null) colVert.Dispose();
			
			if(springVert != null) springVert.Dispose();
        }
		
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "ChainSpringScene";
			
			// Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);
			
			float sphere_width = 1.0f;
			sceneShapes[2] = new PhysicsShape(sphere_width);
			
            Vector2 box_width0 = new Vector2(2.0f, 1.0f);
            sceneShapes[3] = new PhysicsShape(box_width0);		
            
			numShape = 4;
			
			
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
						
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(-20.0f, 10.0f);
				sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].SetBodyKinematic();
				numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(-20.0f, 5.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;		
				
				PhysicsBody bodyA = sceneBodies[numBody-2];
				PhysicsBody bodyB = sceneBodies[numBody-1];
				sceneSprings[numSpring] = new PhysicsSpring(bodyA, bodyB, bodyA.position, bodyB.position + new Vector2(2.0f, 0.0f), (uint)numBody-2, (uint)numBody-1);
				numSpring++;
			}
			
			{			
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(0.0f, 10.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(0.0f, 5.0f);
				sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].airFriction = 0.01f;
				numBody++;		
				
				PhysicsBody bodyA = sceneBodies[numBody-2];
				PhysicsBody bodyB = sceneBodies[numBody-1];
				sceneSprings[numSpring] = new PhysicsSpring(bodyA, bodyB, bodyA.position, bodyB.position, (uint)numBody-2, (uint)numBody-1);
				numSpring++;
					
					
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(0.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 3;
				sceneBodies[numBody].airFriction = 0.01f;
				numBody++;		
				
				bodyA = sceneBodies[numBody-2];
				bodyB = sceneBodies[numBody-1];
				sceneSprings[numSpring] = new PhysicsSpring(bodyA, bodyB, bodyA.position, bodyB.position, (uint)numBody-2, (uint)numBody-1);
				numSpring++;
			}
			
			
			
			{			
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(20.0f, 10.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(20.0f, 5.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;		
				
				PhysicsBody bodyA = sceneBodies[numBody-2];
				PhysicsBody bodyB = sceneBodies[numBody-1];
				sceneSprings[numSpring] = new PhysicsSpring(bodyA, bodyB, bodyA.position, bodyB.position, (uint)numBody-2, (uint)numBody-1);
				numSpring++;

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(20.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;		
				
				bodyA = sceneBodies[numBody-2];
				bodyB = sceneBodies[numBody-1];
				sceneSprings[numSpring] = new PhysicsSpring(bodyA, bodyB, bodyA.position, bodyB.position, (uint)numBody-2, (uint)numBody-1);
				numSpring++;
			}
	
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
			sceneBodies[numBody].position = new Vector2(0.0f, -10.0f);
			sceneBodies[numBody].shapeIndex = 3;
			numBody++;
			
			sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
			sceneBodies[numBody].position = new Vector2(-20.0f, -10.0f);
			sceneBodies[numBody].shapeIndex = 2;
			numBody++;		

        }

#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "ChainSpringScene";
			
            // Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);
			
			float sphere_width = 1.0f;
			AddSphereShape(sphere_width);
			
            Vector2 box_width0 = new Vector2(2.0f, 1.0f);
            AddBoxShape(box_width0);		
			
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

						
			{
				PhysicsBody bodyA = AddBody(SceneShapes[3], 1.0f);
				bodyA.Position = new Vector2(-20.0f, 10.0f);
				bodyA.SetBodyKinematic();
				
				PhysicsBody bodyB = AddBody(SceneShapes[3], 1.0f);
				bodyB.Position = new Vector2(-20.0f, 5.0f);

				AddSpring(bodyA, bodyB, bodyA.Position, bodyB.Position + new Vector2(2.0f, 0.0f));
			}
			
			{			
				PhysicsBody bodyA = AddBody(SceneShapes[3], PhysicsUtility.FltMax);
				bodyA.Position = new Vector2(0.0f, 10.0f);
				
				PhysicsBody bodyB = AddBody(SceneShapes[3], 1.0f);
				bodyB.Position = new Vector2(0.0f, 5.0f);
				bodyB.AirFriction = 0.01f;

				AddSpring(bodyA, bodyB, bodyA.Position, bodyB.Position);
					
				PhysicsBody bodyC = AddBody(SceneShapes[3], 1.0f);
				bodyC.Position = new Vector2(0.0f, 0.0f);
				bodyC.AirFriction = 0.01f;
				
				AddSpring(bodyB, bodyC, bodyB.Position, bodyC.Position);
			}
			
			
			{			
				PhysicsBody bodyA = AddBody(SceneShapes[3], 1.0f);
				bodyA.Position = new Vector2(20.0f, 10.0f);
				
				PhysicsBody bodyB = AddBody(SceneShapes[3], 1.0f);
				bodyB.Position = new Vector2(20.0f, 5.0f);	
				
				AddSpring(bodyA, bodyB, bodyA.Position, bodyB.Position);

				PhysicsBody bodyC = AddBody(SceneShapes[3], 1.0f);
				bodyC.Position = new Vector2(20.0f, 0.0f);	
				
				AddSpring(bodyB, bodyC, bodyB.Position, bodyC.Position);
			}
	
			{
				PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
				body.Position = new Vector2(0.0f, -10.0f);
			}
			
			{
				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
				body.Position = new Vector2(-20.0f, -10.0f);
			}
        }

#endif
		// Game controller handling
        public override void KeyboardFunc(GamePadButtons button)
        {
            switch (button)
            {
				// Delete one spring
                case GamePadButtons.Square: 
						if(numSpring > 0)
						{
							int currentSpring = DeleteSpring(0);
							Console.WriteLine ("currentSpring = " + currentSpring);
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
                    	if (sceneBodies[i].sleep == true)
                        {
                            Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
                            program.SetUniformValue(1, ref color);
                        }
                        else
                        {
                            Vector3 color = new Vector3(0.0f, 1.0f, 1.0f);
                            program.SetUniformValue(1, ref color);
                        }
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
			
			
			// Draw the start and the end point of the spring
			//
			// One is relative position to bodyA of the spring (bodyA <-> bodyB)
			// The other is relative position to bodyB of the spring (body A <-> bodyB)
			//
            for (uint i = 0; i < numSpring; i++)
            {
                PhysicsSpring jnt = sceneSprings[i];
                PhysicsBody b1 = sceneBodies[PhysicsUtility.UnpackIdx1(sceneSprings[i].totalIndex)];
                PhysicsBody b2 = sceneBodies[PhysicsUtility.UnpackIdx2(sceneSprings[i].totalIndex)];

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
			
			graphics.SetVertexBuffer(0, springVert);
			
			for (uint i = 0; i < numSpring; i++)
            {
                PhysicsSpring jnt = sceneSprings[i];
                PhysicsBody b1 = sceneBodies[PhysicsUtility.UnpackIdx1(sceneSprings[i].totalIndex)];
                PhysicsBody b2 = sceneBodies[PhysicsUtility.UnpackIdx2(sceneSprings[i].totalIndex)];

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

                	springVert.SetVertices(0, vertex);

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


		
		

		

		
