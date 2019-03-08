/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


// use direct access to structure of simulation data to set up initial scene
#define USE_OLD_API

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;


// Air Hockey Game Scene
//
// [How to play] 
//
// Grab the sphere located on your side and throw it on the disk located around the center,
// try to make it go in the opponent's goal
//
// To restart the scene, press the triangle button
// To move to the next scene, press the right arrow button
// To move to the previous scene, press the left arrow button




// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2DSample
{
	public class AirHockeyScene : PhysicsScene
	{
        private int my_index;
		private int enemy_index;
		private int ball_disc_index;
		
		private int my_goal_index;
		private int enemy_goal_index;
		
		private int my_point = 0;
		private int enemy_point = 0;

        private bool my_goal_in = false;
        private bool enemy_goal_in = false;

        
        // Vertex Buffer for Body Rendering
        private VertexBuffer[] vertices = new VertexBuffer[100];

        private VertexBuffer centerVert = null;
        private VertexBuffer goalVert = null;
        private VertexBuffer ballVert = null;

		public AirHockeyScene ()
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


            // To render the center line
            {
                centerVert = new VertexBuffer(2, VertexFormat.Float3);

                float[] vertex = new float[]
                {
                    -13.0f, 0.0f, 0.0f,
                    13.0f, 0.0f, 0.0f,
                };

                centerVert.SetVertices(0, vertex);
            }

            // To render the goal line
            {
                goalVert = new VertexBuffer(4, VertexFormat.Float3);

                float[] vertex = new float[]
                {
                    -1.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 0.0f,
                    1.0f, 1.0f, 0.0f,
                    -1.0f, 1.0f, 0.0f
                };

                goalVert.SetVertices(0, vertex);

            }

            // To render the hockey disc
            {
                ballVert = new VertexBuffer(36, VertexFormat.Float3);

                float[] vertex = new float[3 * 36];

                int i = 0;

                for (float ang = 0.0f; ang < 360.0f; ang = ang + 10.0f)
                {
                    float rad = ang / 180.0f * PhysicsUtility.Pi;

                    float x = (float)Math.Cos(rad);
                    float y = (float)Math.Sin(rad);

                    vertex[3 * i + 0] = x;
                    vertex[3 * i + 1] = y;
                    vertex[3 * i + 2] = 0.0f;
                    i++;
                }

                ballVert.SetVertices(0, vertex);
            }
        }

        ~AirHockeyScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {
            for (int i = 0; i < numShape; i++)
                if (vertices[i] != null)
                    vertices[i].Dispose();

            if(centerVert != null) centerVert.Dispose();
            if(goalVert != null) goalVert.Dispose();
            if(ballVert != null) ballVert.Dispose();
        }
		
		
#if USE_OLD_API
		
		public override void InitScene()
        {
            
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "AirHockeyScene";
			
            // Set the gravity to zero
            gravity = new Vector2(0, 0);

            // Make embedded shape's elasticity (bounciness) a bit stronger
            this.penetrationRepulse = 0.5f;
            this.penetLimit = 0.0f;
            this.tangentFriction = 0.0f;
            this.restitutionCoeff = 0.90f;


            Vector2 bar_width1 = new Vector2(8.0f, 4.0f);
            Vector2 bar_width2 = new Vector2(18.0f, 4.0f);
            Vector2 bar_width3 = new Vector2(5.0f, 4.0f);
			float corner_circle = 1.0f;
			
            sceneShapes[0] = new PhysicsShape(bar_width1);
            sceneShapes[1] = new PhysicsShape(bar_width2);
            sceneShapes[2] = new PhysicsShape(2.0f);
            sceneShapes[3] = new PhysicsShape(bar_width3);
            sceneShapes[4] = new PhysicsShape(1.5f);
			sceneShapes[5] = new PhysicsShape(corner_circle);
			
            numShape = 6;

            // Set up the game court
            {
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(13, -22);
                    sceneBodies[numBody].rotation = 0.0f;
                    sceneBodies[numBody].shapeIndex = 0;
                    numBody++;

                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(-13, -22);
                    sceneBodies[numBody].rotation = 0.0f;
                    sceneBodies[numBody].shapeIndex = 0;
                    numBody++;

                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(0, -22);
                    sceneBodies[numBody].rotation = 0.0f;
                    sceneBodies[numBody].shapeIndex = 3;
                    my_goal_index = numBody;
                    numBody++;
                }

                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(13, 22);
                    sceneBodies[numBody].rotation = 0.0f;
                    sceneBodies[numBody].shapeIndex = 0;
                    numBody++;

                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(-13, 22);
                    sceneBodies[numBody].rotation = 0.0f;
                    sceneBodies[numBody].shapeIndex = 0;
                    numBody++;

                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(0, 22);
                    sceneBodies[numBody].rotation = 0.0f;
                    sceneBodies[numBody].shapeIndex = 3;
                    enemy_goal_index = numBody;
                    numBody++;
                }

                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(17, 0);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
                    sceneBodies[numBody].shapeIndex = 1;
                    numBody++;

                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                    sceneBodies[numBody].position = new Vector2(-17, 0);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(90.0f);
                    sceneBodies[numBody].shapeIndex = 1;
                    numBody++;
                }
            }

            // Prepare the disks for the player, the opponent and the hockey disc
            {
                sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                sceneBodies[numBody].position = new Vector2(0, -5);
                sceneBodies[numBody].rotation = 0.0f;
                sceneBodies[numBody].shapeIndex = 2;
                my_index = numBody;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                sceneBodies[numBody].position = new Vector2(10, 0);
                sceneBodies[numBody].velocity = new Vector2(-10, 0);
                sceneBodies[numBody].rotation = 0.0f;
                sceneBodies[numBody].shapeIndex = 2;
				sceneBodies[numBody].picking = false;
                ball_disc_index = numBody;
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                sceneBodies[numBody].position = new Vector2(0, 8);
                sceneBodies[numBody].rotation = 0.0f;
                sceneBodies[numBody].shapeIndex = 2;
                enemy_index = numBody;
                numBody++;
            }
			
			
			// To avoid the corner interpenetration because very simple AI
			// You don't need to render this object for the game
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(13, -18);
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 5;
                numBody++;	
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(13, 18);
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 5;
                numBody++;
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(-13, -18);
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 5;
                numBody++;		
				
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], PhysicsUtility.FltMax);
				sceneBodies[numBody].position = new Vector2(-13, 18);
				sceneBodies[numBody].rotation = 0.0f;
				sceneBodies[numBody].shapeIndex = 5;
                numBody++;		
			}
			
        }

#else
		public override void InitScene()
        {
            
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "AirHockeyScene";
			
            // Set the gravity to zero
            Gravity = new Vector2(0, 0);

            // Make embedded shape's elasticity (bounciness) a bit stronger
            this.PenetrationRepulse = 0.5f;
            this.PenetLimit = 0.0f;
            this.TangentFriction = 0.0f;
            this.RestitutionCoeff = 0.90f;


            Vector2 bar_width1 = new Vector2(8.0f, 4.0f);
            Vector2 bar_width2 = new Vector2(18.0f, 4.0f);
            Vector2 bar_width3 = new Vector2(5.0f, 4.0f);
			float corner_circle = 1.0f;
			
            AddBoxShape(bar_width1);
            AddBoxShape(bar_width2);
            AddSphereShape(2.0f);
            AddBoxShape(bar_width3);
            AddSphereShape(1.5f);
			AddSphereShape(corner_circle);

            // Set up the game court
            {
                {
					PhysicsBody body = null;
                    body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
                    body.Position = new Vector2(13, -22);
                    body.Rotation = 0.0f;

                    body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
                    body.Position = new Vector2(-13, -22);
                    body.Rotation = 0.0f;

                    body = AddBody(SceneShapes[3], PhysicsUtility.FltMax);
                    body.Position = new Vector2(0, -22);
                    body.Rotation = 0.0f;
                    my_goal_index = numBody-1;
                }

                {
					PhysicsBody body = null;					
                    body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
                    body.Position = new Vector2(13, 22);
                    body.Rotation = 0.0f;

                    body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);
                    body.Position = new Vector2(-13, 22);
                    body.Rotation = 0.0f;

                    body = AddBody(SceneShapes[3], PhysicsUtility.FltMax);
                    body.Position = new Vector2(0, 22);
                    body.Rotation = 0.0f;
                    enemy_goal_index = NumBody-1;
                }

                {
					PhysicsBody body = null;	
                    body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                    body.Position = new Vector2(17, 0);
                    body.Rotation = PhysicsUtility.GetRadian(90.0f);

                    body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                    body.Position = new Vector2(-17, 0);
                    body.Rotation = PhysicsUtility.GetRadian(90.0f);
                }
            }

            // Prepare the disks for the player, the opponent and the hockey disc
            {
				PhysicsBody body = null;
				
                body = AddBody(SceneShapes[2], 1.0f);
                body.Position = new Vector2(0, -5);
                body.Rotation = 0.0f;
                my_index = NumBody-1;

				body = AddBody(SceneShapes[2], 1.0f);
                body.Position = new Vector2(10, 0);
                body.Velocity = new Vector2(-10, 0);
                body.Rotation = 0.0f;
				body.Picking = false;
                ball_disc_index = NumBody-1;

                body = AddBody(SceneShapes[2], 1.0f);
                body.Position = new Vector2(0, 8);
                body.Rotation = 0.0f;
                enemy_index = NumBody-1;
            }
			
			
			// To avoid the corner interpenetration because very simple AI
			// You don't need to render this object for the game
			{
				PhysicsBody body = null;
				
				body = AddBody(SceneShapes[5], PhysicsUtility.FltMax);
				body.Position = new Vector2(13, -18);
				body.Rotation = 0.0f;
				
				body = AddBody(SceneShapes[5], PhysicsUtility.FltMax);
				body.Position = new Vector2(13, 18);
				body.Rotation = 0.0f;
				
				body = AddBody(SceneShapes[5], PhysicsUtility.FltMax);
				body.Position = new Vector2(-13, -18);
				body.Rotation = 0.0f;

				body = AddBody(SceneShapes[5], PhysicsUtility.FltMax);
				body.Position = new Vector2(-13, 18);
				body.Rotation = 0.0f;	
			}
			
        }
	
#endif
		
        // Prevent the player and the opponent from going outside the game court
		public override void UpdateFuncBeforeSim ()
		{
            my_goal_in = false;
            enemy_goal_in = false;
        }

		// Prevent player and opponent's disks from going outside the game court
		public override void UpdateFuncAfterSim ()
		{
            // Opponent's behavior/AI
			Vector2 vec = (sceneBodies[ball_disc_index].position + new Vector2(0, 1) + 1.0f/60.0f * sceneBodies[ball_disc_index].velocity - sceneBodies[enemy_index].position);
			sceneBodies[enemy_index].velocity = 0.5f * vec.Normalize() * 60.0f;
			
            if((sceneBodies[ball_disc_index].position - sceneBodies[enemy_index].position).Length() > 10.0f)
			{
				sceneBodies[enemy_index].velocity = new Vector2(
				       (float)Math.Sin(PhysicsUtility.GetRadian(sceneFrame)) * (0.5f * 60.0f),
				       (float)Math.Cos(PhysicsUtility.GetRadian(sceneFrame)) * (0.5f * 60.0f));				                                 
				                                 
			}

			// Check the goal status
			if(QueryContact((uint)my_goal_index, (uint)ball_disc_index))
			{
				Console.WriteLine("Enemy get a point!");
				Console.WriteLine("Me : Enemy = {0:d} : {1:d}", my_point, ++enemy_point);
                my_goal_in = true;
			}
			
			if(QueryContact((uint)enemy_goal_index, (uint)ball_disc_index))
			{						
				Console.WriteLine("Me get a point!");
				Console.WriteLine("Me : Enemy = {0:d} : {1:d}", ++my_point, enemy_point);
                enemy_goal_in = true;
			}
			
            // When the hockey disk goes too fast it sometimes goes outside the court, this this is for preventing this from happening
			sceneBodies[ball_disc_index].position = Vector2.Max(new Vector2(-12, -17), sceneBodies[ball_disc_index].position);
			sceneBodies[ball_disc_index].position = Vector2.Min(new Vector2(12, 17), sceneBodies[ball_disc_index].position);
			
			// Prevent the player and the opponent from going outside the game court
			sceneBodies[my_index].position = Vector2.Max(new Vector2(-11, -17), sceneBodies[my_index].position);
			sceneBodies[my_index].position = Vector2.Min(new Vector2(11, -1), sceneBodies[my_index].position);
			
            sceneBodies[enemy_index].position = Vector2.Max(new Vector2(-10.0f, 1.5f), sceneBodies[enemy_index].position);
            sceneBodies[enemy_index].position = Vector2.Min(new Vector2(10.0f, 16f), sceneBodies[enemy_index].position);

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

                    if (sceneShapes[index].numVert == 0)
                        graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
                    else
                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);
                }

            }
        }

        public override void DrawAdditionalInfo(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix)
        {
            // Draw the center line
            graphics.SetVertexBuffer(0, centerVert);

            {
                Matrix4 WorldMatrix = renderMatrix;
                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color = new Vector3(0.0f, 1.0f, 0.0f);
                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.Lines, 0, 2);
            }

    
            // Draw the goal post [inside the goal] or [outside of the goal]
            graphics.SetVertexBuffer(0, goalVert);

            {

                Matrix4 rotationMatrix = Matrix4.RotationZ(sceneBodies[my_goal_index].rotation);

                Matrix4 transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[my_goal_index].position.X,
                            sceneBodies[my_goal_index].position.Y, 0.0f));

                Matrix4 local_rotationMatrix = Matrix4.RotationZ(sceneBodies[my_goal_index].localRotation);

                Matrix4 local_transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[my_goal_index].localPosition.X,
                            sceneBodies[my_goal_index].localPosition.Y, 0.0f));

                Matrix4 scaleMatrix = new Matrix4(
                      5.0f, 0.0f, 0.0f, 0.0f,
                      0.0f, 4.0f, 0.0f, 0.0f,
                      0.0f, 0.0f, 1.0f, 0.0f,
                      0.0f, 0.0f, 0.0f, 1.0f
                  );

                Matrix4 WorldMatrix = renderMatrix * transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix * scaleMatrix;

                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color;

                if (my_goal_in)
                    color = new Vector3(1.0f, 0.0f, 1.0f);
                else
                    color = new Vector3(0.0f, 1.0f, 1.0f);

                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);

            }

            // Draw goal post [inside the goal] or [outside of the goal]
            graphics.SetVertexBuffer(0, goalVert);

            {

                Matrix4 rotationMatrix = Matrix4.RotationZ(sceneBodies[enemy_goal_index].rotation);

                Matrix4 transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[my_goal_index].position.X,
                            sceneBodies[enemy_goal_index].position.Y, 0.0f));

                Matrix4 local_rotationMatrix = Matrix4.RotationZ(sceneBodies[enemy_goal_index].localRotation);

                Matrix4 local_transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[enemy_goal_index].localPosition.X,
                            sceneBodies[enemy_goal_index].localPosition.Y, 0.0f));


                Matrix4 scaleMatrix = new Matrix4(
                    5.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 4.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );


                Matrix4 WorldMatrix = renderMatrix * transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix * scaleMatrix;

                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color;

                if (enemy_goal_in)
                    color = new Vector3(1.0f, 0.0f, 1.0f);
                else
                    color = new Vector3(0.0f, 1.0f, 1.0f);

                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);

            }

            // Draw hockey disc
            graphics.SetVertexBuffer(0, ballVert);

            {

                Matrix4 rotationMatrix = Matrix4.RotationZ(sceneBodies[ball_disc_index].rotation);

                Matrix4 transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[ball_disc_index].position.X,
                            sceneBodies[ball_disc_index].position.Y, 0.0f));

                Matrix4 local_rotationMatrix = Matrix4.RotationZ(sceneBodies[ball_disc_index].localRotation);

                Matrix4 local_transMatrix = Matrix4.Translation(
                    new Vector3(sceneBodies[ball_disc_index].localPosition.X,
                            sceneBodies[ball_disc_index].localPosition.Y, 0.0f));

                Matrix4 WorldMatrix = renderMatrix * transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix;

                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.TriangleFan, 0, 36);
            }
        }

	}
}
