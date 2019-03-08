/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

// Hammer Game Scene
//
// [How to play] 
//
// Grip the box-shaped part of the hammer and turn it round inside the yellow area
// and throw it for aiming to the goal. If the hammer goes out of the yellow area,
// it is not possible to grip the hammer again.
//
// To restart the scene, press the triangle button
// To move to the next scene, press the right arrow button
// To move to the previous scene, press the left arrow button



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
	public class HammerThrowerScene : PhysicsScene
	{

		private int my_controll = 0;
		private int goal_bar = 0;
		private int hammer_index = 0;

        // Start area for throwing the hammer
        private Vector2 start_min = new Vector2(20, -25);
		private Vector2 start_max = new Vector2(40, 20);

        // Check whether the hammer is in start area
		private bool inside_area = true;

        
        // Vertex Buffer for body rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        
        // Vertex Buffer for debug rendering
        private VertexBuffer areaVert = null;
        private VertexBuffer jointVert = null;
			
		public HammerThrowerScene ()
		{
            
            // Simulation scene set up
            InitScene();

            
            // Setup for rendering objects
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

            // Debug rendering buffer for the hammer throw start area
            {
                areaVert = new VertexBuffer(5, VertexFormat.Float3);

                float[] vertex = new float[]
                {
                    0.0f, 0.0f, 0.0f,
                    1.0f, 0.0f, 0.0f,
                    1.0f, 1.0f, 0.0f,
                    0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f,
                };

                areaVert.SetVertices(0, vertex);
            }


            // VertexBuffer used for joints debug rendering
            {
                jointVert = new VertexBuffer(2, VertexFormat.Float3);

                float[] vertex = new float[]
                {
                    0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f
                };

                jointVert.SetVertices(0, vertex);
            }
             
		}

        ~HammerThrowerScene()
        {
            ReleaseScene();
        }


        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if (vertices[i] != null)
                    vertices[i].Dispose();

            if(areaVert != null) areaVert.Dispose();
            if(jointVert != null) jointVert.Dispose();
        }

#if USE_OLD_API
		
		public override void InitScene ()
		{
            
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "HammerThrowerScene";
			
			this.restitutionCoeff = 0.5f;

            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

			float ball_width_big = 2.0f;
			sceneShapes[2] = new PhysicsShape(ball_width_big);
			
			float ball_width_small = 1.0f;
			sceneShapes[3] = new PhysicsShape(ball_width_small);
		
			Vector2 general_width = new Vector2(1.5f, 1.5f);
			sceneShapes[4] = new PhysicsShape(general_width);	
			
			sceneShapes[5] = new PhysicsShape(new Vector2(3.0f, 2.0f));
			
			Vector2 goal_width = new Vector2(6.0f, 2.0f);
			sceneShapes[6] = new PhysicsShape(goal_width);

            numShape = 7;


            
            // Create static walls to limit the range of action of active rigid sceneBodies
            {
                sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(0, -wall_height.Y);
                sceneBodies[numBody].rotation = 0;
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

            // Create the hammer
            const int chain_num = 5;
			{
				for(int i=0; i<chain_num; i++)
				{
					if(i==0)
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 2.0f);
						sceneBodies[numBody].position = new Vector2(30, -10) + new Vector2(0, 3.0f*i);
						sceneBodies[numBody].shapeIndex = 4;
						sceneBodies[numBody].collisionFilter = (1<<2);
						my_controll = numBody;
						numBody++;		
					}
					else if(i==chain_num-1)
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 3.0f);
						sceneBodies[numBody].position = new Vector2(30, -10) + new Vector2(0, 3.0f*i);
						sceneBodies[numBody].shapeIndex = 2;
						sceneBodies[numBody].collisionFilter = (1<<2);
						hammer_index = numBody;
						numBody++;	
					}
					else
					{
						sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
						sceneBodies[numBody].position = new Vector2(30, -10) + new Vector2(0, 3.0f*i);
						sceneBodies[numBody].shapeIndex = 3;
						sceneBodies[numBody].collisionFilter = (1<<2);					
						numBody++;
					}

                    // We can only grab the tip of the hammer (Box)
                    if(i==0)
						sceneBodies[numBody-1].picking = true;
					else
						sceneBodies[numBody-1].picking = false;
				
				}

                // Make the hammer by connecting joints
                for(uint i=4; i<chain_num+4-1; i++)
				{
					uint index0 = i;
					uint index1 = i+1;
					PhysicsBody b1 = sceneBodies[index0];
					PhysicsBody b2 = sceneBodies[index1];
					sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b1.position + b2.position)*0.5f, index0, index1);
					sceneJoints[numJoint].axis1Lim = new Vector2(0.0f, 1.0f);
					sceneJoints[numJoint].axis2Lim = new Vector2(1.0f, 0.0f);
					sceneJoints[numJoint].angleLim = 0.1f;
					numJoint++;
				}
				
			}

            // Set up obstacles	
			Vector2[] pin_pos = new Vector2[]
			{
				new Vector2(-35.0f, 15.0f),
				new Vector2(-10.0f, 15.0f),
				new Vector2(-20.0f, 6.0f)
			};
			
			for(int i=0; i<3; i++)
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
	    	    sceneBodies[numBody].position = pin_pos[i];
	       		sceneBodies[numBody].rotation = 0.0f;
	       		sceneBodies[numBody].shapeIndex = 5;
	        	numBody++;
			
				PhysicsBody b1 = sceneBodies[0];
				PhysicsBody b2 = sceneBodies[numBody-1];
				sceneJoints[numJoint] = new PhysicsJoint(b1, b2, (b2.position), (uint)0, (uint)numBody-1);
				sceneJoints[numJoint].axis1Lim = new Vector2(1.0f, 0.0f);
				sceneJoints[numJoint].axis2Lim = new Vector2(0.0f, 1.0f);
				sceneJoints[numJoint].angleLim = 0.001f;
				numJoint++;
			}

            // Set the goal post
            {
                sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
	            sceneBodies[numBody].position = new Vector2(-30, -15);
	            sceneBodies[numBody].rotation = 0.0f;
	            sceneBodies[numBody].shapeIndex = 6;
				goal_bar = numBody;
	            numBody++;
                
                sceneBodies[goal_bar].SetBodyKinematic();

				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], PhysicsUtility.FltMax);
	            sceneBodies[numBody].position = new Vector2(-30, -15) + new Vector2(goal_width.X+goal_width.Y, goal_width.X-goal_width.Y);
	            sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
	            sceneBodies[numBody].shapeIndex = 6;
	            numBody++;	
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], PhysicsUtility.FltMax);
	            sceneBodies[numBody].position = new Vector2(-30, -15) + new Vector2(-goal_width.X-goal_width.Y, goal_width.X-goal_width.Y);
	            sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
	            sceneBodies[numBody].shapeIndex = 6;
	            numBody++;	
			}
	    }
#else
		public override void InitScene ()
		{
            
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "HammerThrowerScene";
			
			this.RestitutionCoeff = 0.5f;

            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            AddBoxShape(wall_height);

			float ball_width_big = 2.0f;
			AddSphereShape(ball_width_big);
			
			float ball_width_small = 1.0f;
			AddSphereShape(ball_width_small);
		
			Vector2 general_width = new Vector2(1.5f, 1.5f);
			AddBoxShape(general_width);	
			
			AddBoxShape(new Vector2(3.0f, 2.0f));
			
			Vector2 goal_width = new Vector2(6.0f, 2.0f);
			AddBoxShape(goal_width);

			
            // Create static walls to limit the range of action of active rigid sceneBodies
            {
				PhysicsBody body = null;
				
                body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
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

            // Create the hammer
            const int chain_num = 5;
			{
				for(int i=0; i<chain_num; i++)
				{
					if(i==0)
					{
						PhysicsBody body = AddBody(SceneShapes[4], 2.0f);
						body.Position = new Vector2(30, -10) + new Vector2(0, 3.0f*i);
						body.CollisionFilter = (1<<2);
						my_controll = NumBody-1;
					}
					else if(i==chain_num-1)
					{
						PhysicsBody body = AddBody(SceneShapes[2], 3.0f);
						body.Position = new Vector2(30, -10) + new Vector2(0, 3.0f*i);
						body.CollisionFilter = (1<<2);
						hammer_index = NumBody-1;
					}
					else
					{
						PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
						body.Position = new Vector2(30, -10) + new Vector2(0, 3.0f*i);
						body.CollisionFilter = (1<<2);					
					}

                    // We can only grab the tip of the hammer (Box)
                    if(i==0)
						SceneBodies[numBody-1].Picking = true;
					else
						SceneBodies[numBody-1].Picking = false;
				
				}

                // Make the hammer by connecting joints
                for(uint i=4; i<chain_num+4-1; i++)
				{
					uint index0 = i;
					uint index1 = i+1;
					PhysicsBody b1 = SceneBodies[index0];
					PhysicsBody b2 = SceneBodies[index1];
					PhysicsJoint joint = AddJoint(b1, b2, (b1.Position + b2.Position)*0.5f);
					joint.Axis1Lim = new Vector2(0.0f, 1.0f);
					joint.Axis2Lim = new Vector2(1.0f, 0.0f);
					joint.AngleLim = 0.1f;
				}
				
			}

            // Set up obstacles	
			Vector2[] pin_pos = new Vector2[]
			{
				new Vector2(-35.0f, 15.0f),
				new Vector2(-10.0f, 15.0f),
				new Vector2(-20.0f, 6.0f)
			};
			
			for(int i=0; i<3; i++)
			{
				PhysicsBody body = AddBody(SceneShapes[5], 1.0f);
	    	    body.Position = pin_pos[i];
	       		body.Rotation = 0.0f;
				
				PhysicsBody b1 = SceneBodies[0];
				PhysicsBody b2 = SceneBodies[NumBody-1];
				PhysicsJoint joint = AddJoint(b1, b2, (b2.Position));
				joint.Axis1Lim = new Vector2(1.0f, 0.0f);
				joint.Axis2Lim = new Vector2(0.0f, 1.0f);
				joint.AngleLim = 0.001f;
			}

            // Set the goal post
            {
                PhysicsBody body = null;
				
				body = AddBody(SceneShapes[6], 1.0f);
	            body.Position = new Vector2(-30, -15);
	            body.Rotation = 0.0f;
				body.SetBodyKinematic();
				goal_bar = NumBody-1;
	  
				body = AddBody(SceneShapes[6], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-30, -15) + new Vector2(goal_width.X+goal_width.Y, goal_width.X-goal_width.Y);
	            body.Rotation = PhysicsUtility.GetRadian(90.0f);
				
				body = AddBody(SceneShapes[6], PhysicsUtility.FltMax);
	            body.Position = new Vector2(-30, -15) + new Vector2(-goal_width.X-goal_width.Y, goal_width.X-goal_width.Y);
	            body.Rotation = PhysicsUtility.GetRadian(90.0f);
			}
	    }	
#endif
		
        // Define the action before the simulation starts (Mainly based on the position).
        public override void UpdateFuncBeforeSim()
		{
			Vector2 pos = sceneBodies[my_controll].position;
			
			if((pos.X < start_min.X)
			 ||(pos.X > start_max.X)
			 ||(pos.Y < start_min.Y)
			 ||(pos.Y > start_max.Y))
			{
				inside_area = false;
				sceneBodies[my_controll].picking = false;
			}
		}


        // Define the actions executed after the simulation. At this point collision points are known and related processing can be done.
        // This is also the place to modify velocities. Positions modifications should be done in UpdateFuncBeforeSim().
		public override void UpdateFuncAfterSim ()
		{
            // Check the collision list to see if there is a collision between the goal post and the hammer
            if (QueryContact((uint)goal_bar, (uint)hammer_index))
            {
                Console.WriteLine("Goal Congratulation!");
                sceneBodies[goal_bar].BackToDynamic();
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

                    if (i == my_controll)
                    {
                        graphics.DrawArrays(DrawMode.TriangleStrip, 0, sceneShapes[index].numVert + 1);
                    }
                    else
                    {
                        if (sceneShapes[index].numVert == 0)
                            graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                        else
                            graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);
                    }
                }

            }
        }


        // Debug rendering for start area and joints
        public override void DrawAdditionalInfo(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix)
        {
            // Start area debug rendering
            graphics.SetVertexBuffer(0, areaVert);

            {
                Matrix4 scaleMatrix = new Matrix4(
                     start_max.X - start_min.X, 0.0f, 0.0f, 0.0f,
                     0.0f, start_max.Y - start_min.Y, 0.0f, 0.0f,
                     0.0f, 0.0f, 1.0f, 0.0f,
                     0.0f, 0.0f, 0.0f, 1.0f
                 );

                Matrix4 transMatrix = Matrix4.Translation(
                    new Vector3(start_min.X, start_min.Y, 0.0f));

                Matrix4 WorldMatrix = renderMatrix * transMatrix * scaleMatrix;

                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color;

                if (inside_area)
                    color = new Vector3(1.0f, 1.0f, 0.0f);
                else
                    color = new Vector3(1.0f, 0.0f, 1.0f);

                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.LineStrip, 0, 5);

            }


            // Joints debug rendering
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

                Vector3 color = new Vector3(0.0f, 1.0f, 0.0f);
                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.Points, 0, 2);
                graphics.DrawArrays(DrawMode.Lines, 0, 2);
            }
        }
	}
}
