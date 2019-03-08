/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// Joint Scene --- (Fixed, Rotate, Slider) Joint Creation
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
	public class JointScene : PhysicsScene
	{
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        // VertexBuffer used for rendering joints
        private VertexBuffer jointVert = null;

		public JointScene ()
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


            // VertexBuffer used for joints debug rendering
            {
                jointVert = new VertexBuffer(2, VertexFormat.Float3);

                float[] vertex = new float[]
                { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                
                jointVert.SetVertices(0, vertex);
            }
		}

        ~JointScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if (vertices[i] != null)
                    vertices[i].Dispose();

           if(jointVert != null) jointVert.Dispose();
        }

#if USE_OLD_API
		
		public override void InitScene ()
		{
            // Create an empty simulation scene 
			base.InitScene();
			sceneName = "JointScene";
			
            // Set the restitution coefficient a bit stronger
			this.restitutionCoeff = 0.5f;

            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);
			
			Vector2 box_width = new Vector2(3.0f, 2.0f);
			sceneShapes[2] = new PhysicsShape(box_width);
			
			float sphere_width = 2.0f;
			sceneShapes[3] = new PhysicsShape(sphere_width);
			
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


            // Link a box rigid body to the scene with a fixed joint.
			// If you just want to fix a rigid body to the scene perfectly, it is best simply making a static rigidbody.
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 10.0f);
				sceneBodies[numBody].position = new Vector2(-30.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
				
				PhysicsBody b1 = sceneBodies[0];
				PhysicsBody b2 = sceneBodies[numBody-1];
				sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), 0, (uint)numBody-1);
				sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
				sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
				sceneJoints[numJoint].angleLim = 1;
				numJoint++;	
			}


            // Link a box rigid body to the scene with a rotation joint（with angle constraints)
            {
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 10.0f);
				sceneBodies[numBody].position = new Vector2(-20.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
				
				PhysicsBody b1 = sceneBodies[0];
				PhysicsBody b2 = sceneBodies[numBody-1];
				sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), 0, (uint)numBody-1);
				sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
				sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
				sceneJoints[numJoint].angleLim = 1;
				sceneJoints[numJoint].angleLower = PhysicsUtility.GetRadian(-45.0f);
				sceneJoints[numJoint].angleUpper = PhysicsUtility.GetRadian(45.0f);
				numJoint++;	
			}


            // Link a box rigid body to the scene with a vertical slider joint (with movement constraints) 
            {
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 10.0f);
				sceneBodies[numBody].position = new Vector2(-10.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
				
				PhysicsBody b1 = sceneBodies[0];
				PhysicsBody b2 = sceneBodies[numBody-1];
				sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), 0, (uint)numBody-1);
				sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
				sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
				sceneJoints[numJoint].axis2Lower = -10.0f;
				sceneJoints[numJoint].axis2Upper = 10.0f;
				sceneJoints[numJoint].angleLim = 1;
				numJoint++;	
			}

            // Link a box rigid body to the scene with a horizontal slider joint (with movement constraints) 
            {
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 10.0f);
				sceneBodies[numBody].position = new Vector2(0.0f, 0.0f);
				sceneBodies[numBody].shapeIndex = 2;
				numBody++;
				
				PhysicsBody b1 = sceneBodies[0];
				PhysicsBody b2 = sceneBodies[numBody-1];
				sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), 0, (uint)numBody-1);
				sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
				sceneJoints[numJoint].axis1Lower = -10.0f;
				sceneJoints[numJoint].axis1Upper = 10.0f;
				sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
				sceneJoints[numJoint].angleLim = 1;
				numJoint++;	
				
                // The horizontal slider joint is quite slippy,
                // so to make the rigid body stop we add some air friction
				b2.airFriction = 0.01f;
			}

            // Create a chain by connecting sphere rigid sceneBodies with rotation jonts
            {
				for(int i=0; i<5; i++)
				{
					int index;
					sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
					sceneBodies[numBody].position = new Vector2(20.0f, 10.0f - (sceneShapes[3].vertList[0].X * 2.0f + 0.5f)*i);
					sceneBodies[numBody].shapeIndex = 3;
					index = numBody++;
					
					if(i==0)
					{
						PhysicsBody b1 = sceneBodies[0];
						PhysicsBody b2 = sceneBodies[index];
						sceneJoints[numJoint] = new PhysicsJoint(b1, b2, b2.position, (uint)0, (uint)index);
						sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
						sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
						sceneJoints[numJoint].angleLim = 1;
						numJoint++;
					}
					else
					{
						PhysicsBody b1 = sceneBodies[index-1];
						PhysicsBody b2 = sceneBodies[index];
						sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b1.position + b2.position)*0.5f, (uint)index-1, (uint)index);
						sceneJoints[numJoint].axis1Lim = new Vector2(1, 0);
						sceneJoints[numJoint].axis2Lim = new Vector2(0, 1);
						sceneJoints[numJoint].angleLim = 0;
						numJoint++;	
					}
				}		
			}

            // Create a sphere-shaped dynamic rigid body
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
				sceneBodies[numBody].position = new Vector2(-18.0f, 20.0f);
				sceneBodies[numBody].shapeIndex = 3;
				numBody++;
			}
	    }

#else		
		public override void InitScene ()
		{
            // Create an empty simulation scene 
			base.InitScene();
			SceneName = "JointScene";
			
            // Set the restitution coefficient a bit stronger
			this.RestitutionCoeff = 0.5f;

            Vector2 wall_width = new Vector2(50, 8);
			AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            AddBoxShape(wall_height);
			
			Vector2 box_width = new Vector2(3.0f, 2.0f);
			AddBoxShape(box_width);
			
			float sphere_width = 2.0f;
			AddSphereShape(sphere_width);
			
            // Create static walls to limit the range of action of active rigid sceneBodies
            {
				PhysicsBody body;
                
                body = AddBody(0, PhysicsUtility.FltMax);
                body.Position = new Vector2(0, -wall_height.Y);
                body.Rotation = 0;

                body = AddBody(1, PhysicsUtility.FltMax);
                body.Position = new Vector2(wall_width.X, 0);
                body.Rotation = 0;

                body = AddBody(1, PhysicsUtility.FltMax);
                body.Position = new Vector2(-wall_width.X, 0);
                body.Rotation = 0;

                body = AddBody(0, PhysicsUtility.FltMax);
                body.Position = new Vector2(0, wall_height.Y);
                body.Rotation = 0;
            }


            // Link a box rigid body to the scene with a fixed joint.
			// If you just want to fix a rigid body to the scene perfectly, it is best simply making a static rigidbody.
			{
				PhysicsBody body;
				body = AddBody(SceneShapes[2], 10.0f);
				body.Position = new Vector2(-30.0f, 0.0f);
				
				PhysicsBody b1 = SceneBodies[0];
				PhysicsBody b2 = SceneBodies[NumBody-1];
				
				PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
				joint.Axis1Lim = new Vector2(1, 0);
				joint.Axis2Lim = new Vector2(0, 1);
				joint.AngleLim = 1;
			}


            // Link a box rigid body to the scene with a rotation joint（with angle constraints)
            {
				PhysicsBody body;
				body = AddBody(SceneShapes[2], 10.0f);
				body.Position = new Vector2(-20.0f, 0.0f);
				
				PhysicsBody b1 = SceneBodies[0];
				PhysicsBody b2 = SceneBodies[NumBody-1];
				PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
				joint.Axis1Lim = new Vector2(1, 0);
				joint.Axis2Lim = new Vector2(0, 1);
				joint.AngleLim = 1;
				joint.AngleLower = PhysicsUtility.GetRadian(-45.0f);
				joint.AngleUpper = PhysicsUtility.GetRadian(45.0f);
			}


            // Link a box rigid body to the scene with a vertical slider joint (with movement constraints) 
            {
				PhysicsBody body;
				body = AddBody(SceneShapes[2], 10.0f);
				body.Position = new Vector2(-10.0f, 0.0f);
				
				PhysicsBody b1 = SceneBodies[0];
				PhysicsBody b2 = SceneBodies[NumBody-1];
				PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
				joint.Axis1Lim = new Vector2(1, 0);
				joint.Axis2Lim = new Vector2(0, 1);
				joint.Axis2Lower = -10.0f;
				joint.Axis2Upper = 10.0f;
				joint.AngleLim = 1;
			}

            // Link a box rigid body to the scene with a horizontal slider joint (with movement constraints) 
            {
				PhysicsBody body;
				body = AddBody(SceneShapes[2], 10.0f);
				body.Position = new Vector2(0.0f, 0.0f);
				
				PhysicsBody b1 = SceneBodies[0];
				PhysicsBody b2 = SceneBodies[NumBody-1];
				PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
				joint.Axis1Lim = new Vector2(1, 0);
				joint.Axis1Lower = -10.0f;
				joint.Axis1Upper = 10.0f;
				joint.Axis2Lim = new Vector2(0, 1);
				joint.AngleLim = 1;
				
                // The horizontal slider joint is quite slippy,
                // so to make the rigid body stop we add some air friction
				b2.AirFriction = 0.01f;
			}

            // Create a chain by connecting sphere rigid sceneBodies with rotation jonts
            {
				for(int i=0; i<5; i++)
				{
					int index;
					PhysicsBody body;
					body = AddBody(SceneShapes[3], 1.0f);
					body.Position = new Vector2(20.0f, 10.0f - (sceneShapes[3].VertList[0].X * 2.0f + 0.5f)*i);
					index = NumBody-1;
					
					if(i==0)
					{
						PhysicsBody b1 = SceneBodies[0];
						PhysicsBody b2 = SceneBodies[index];
						PhysicsJoint joint = AddJoint(b1, b2, b2.Position);
						joint.Axis1Lim = new Vector2(1, 0);
						joint.Axis2Lim = new Vector2(0, 1);
						joint.AngleLim = 1;
					}
					else
					{
						PhysicsBody b1 = SceneBodies[index-1];
						PhysicsBody b2 = SceneBodies[index];
						PhysicsJoint joint = AddJoint(b1, b2, (b1.Position + b2.Position)*0.5f);
						joint.Axis1Lim = new Vector2(1, 0);
						joint.Axis2Lim = new Vector2(0, 1);
						joint.AngleLim = 0;
					}
				}		
			}

            // Create a sphere-shaped dynamic rigid body
			{
				PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
				body.Position = new Vector2(-18.0f, 20.0f);
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


        // Debug rendering for joints
        public override void DrawAdditionalInfo(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix)
        {
            graphics.SetVertexBuffer(0, jointVert);

            for (uint i = 0; i < numJoint; i++)
            {
                PhysicsJoint c = sceneJoints[i];
                PhysicsBody b1 = sceneBodies[PhysicsUtility.UnpackIdx1(sceneJoints[i].totalIndex)];
                PhysicsBody b2 = sceneBodies[PhysicsUtility.UnpackIdx2(sceneJoints[i].totalIndex)];

                float angle1 = b1.rotation;
                float angle2 = b2.rotation;

                Vector2 r1 = c.localAnchor1;
                Vector2 r2 = c.localAnchor2;

                Vector2 g1 = b1.position + new Vector2((float)System.Math.Cos(angle1) * r1.X - (float)System.Math.Sin(angle1) * r1.Y,
                    (float)System.Math.Sin(angle1) * r1.X + (float)System.Math.Cos(angle1) * r1.Y);

                Vector2 g2 = b2.position + new Vector2((float)System.Math.Cos(angle2) * r2.X - (float)System.Math.Sin(angle2) * r2.Y,
                    (float)System.Math.Sin(angle2) * r2.X + (float)System.Math.Cos(angle2) * r2.Y);

                float[] vertex = new float[]
                {
                    g1.X, g1.Y, 0.0f,
                    g2.X, g2.Y, 0.0f
                };

                jointVert.SetVertices(0, vertex);

                Matrix4 WorldMatrix = renderMatrix;

                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.Points, 0, 2);
                graphics.DrawArrays(DrawMode.Lines, 0, 2);
            }
        }
	}
}


		
		

		

		
