/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


//
// This is a simple sample to render rigid body with animation
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
// Inside this sample, texture coordinates are generated based on vertices of collision shape.
// All texture coordinates are clamped to [0, 0]<=>[1, 1].
//
// Please notice that the same collision shape can be shared among several rigid bodies.
//
// To render animation, AnimationSprite class is included.
// Please notice that AnimationSprite is not optimized for fast rendering.
// For more optmized rendering, please check SpriteBatchWithScene too.
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
    public class AnimationWithScene : PhysicsScene
	{	
		
		public class AnimationSprite
		{
			long currentFrame = 0;
			int frameSync = 1;
			
			int frameWidth = 1;
			int frameHeight = 1;
			
			int numVert = 0;

			VertexBuffer vertices = null;
			Texture2D texture = null;
		
	        public AnimationSprite(PhysicsShape con, string fileName, bool mipmap, int width, int height, int sync)
	        {
				frameWidth = width;
				frameHeight = height;
				frameSync = sync;
				
	            if (con.numVert == 0)
	            {
					vertices = new VertexBuffer(37, VertexFormat.Float3, VertexFormat.Float2);
					numVert = 36;
					
	                float[] vertex = new float[3 * 37];
					float[] tex = new float[2 * 37];
					
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
						
						tex[2 * i + 0] = x1/rad * 0.5f + 0.5f;
						tex[2 * i + 1] = y1/rad * 0.5f + 0.5f;
		
	                    i++;
	                }
	
	                vertex[3 * i + 0] = vertex[3 * 0 + 0];
	                vertex[3 * i + 1] = vertex[3 * 0 + 1];
	                vertex[3 * i + 2] = vertex[3 * 0 + 2];
					
					tex[2 * i + 0] = tex[2 * 0 + 0]/rad * 0.5f + 0.5f;
					tex[2 * i + 1] = tex[2 * 0 + 1]/rad * 0.5f + 0.5f;
	
	                vertices.SetVertices(0, vertex);
					vertices.SetVertices(1, tex);
	            }
	            else
	            {
					vertices = new VertexBuffer(con.numVert + 1, VertexFormat.Float3, VertexFormat.Float2);
					numVert = con.numVert;
					
	                float[] vertex = new float[3 * (con.numVert + 1)];
					float[] tex = new float[2 * (con.numVert + 1)];
					
	                int i;
	
	                for (i = 0; i < con.numVert; i++)
	                {
	                    Vector2 v1;
	                    v1 = con.vertList[i];
	
	                    vertex[3 * i + 0] = v1.X;
	                    vertex[3 * i + 1] = v1.Y;
	                    vertex[3 * i + 2] = 0.0f;
						
						tex[2 * i + 0] = v1.X;
						tex[2 * i + 1] = v1.Y;
	                }
	
	                vertex[3 * i + 0] = vertex[3 * 0 + 0];
	                vertex[3 * i + 1] = vertex[3 * 0 + 1];
	                vertex[3 * i + 2] = vertex[3 * 0 + 2];
					
					tex[2 * i + 0] = tex[2 * 0 + 0];
					tex[2 * i + 1] = tex[2 * 0 + 1];
					
	                vertices.SetVertices(0, vertex);
					
					float x_min, y_min, x_max, y_max;
					
					x_min = x_max = tex[2 * i + 0];
					y_min = y_max = tex[2 * i + 1];
					
					for (i = 0; i <= con.numVert; i++)
	                {
						x_min = Math.Min(tex[2 * i + 0], x_min);
						x_max = Math.Max(tex[2 * i + 0], x_max);
						y_min = Math.Min(tex[2 * i + 1], y_min);
						y_max = Math.Max(tex[2 * i + 1], y_max);
					}
					
					for (i = 0; i <= con.numVert; i++)
	                {
						tex[2 * i + 0] = (tex[2 * i + 0] - x_min)/(x_max - x_min);
						tex[2 * i + 1] = (tex[2 * i + 1] - y_min)/(y_max - y_min);
					}
					
					vertices.SetVertices(1, tex);
	            }

				// Create Texture
				texture = new Texture2D(fileName, mipmap, PixelFormat.Rgba);
	        }

	        public void Release()
			{
				if(vertices != null) vertices.Dispose();
				if(texture != null) texture.Dispose();
	        }
			
	        ~AnimationSprite()
			{
				Release();
	        }

			public void Draw(ref GraphicsContext graphics, ref ShaderProgram program, ref Matrix4 renderMatrix)
			{	
				program.SetUniformValue(0, ref renderMatrix);
				
				Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
				program.SetUniformValue(1, ref color);
			
				int xPos = ((int)(currentFrame/frameSync)%(frameWidth*frameHeight))%frameWidth;
				int yPos = ((int)(currentFrame/frameSync)%(frameWidth*frameHeight))/frameWidth;
				
				Vector4 texslip = new Vector4((float)frameWidth, (float)frameHeight, (float)xPos, (float)yPos);	
				program.SetUniformValue(2, ref texslip);
					
				graphics.SetVertexBuffer(0, vertices);	
				graphics.SetTexture(0, texture);
				
				graphics.DrawArrays(DrawMode.TriangleFan, 0, numVert);
				
				currentFrame++;
			}
		}
		
		
		
		
        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;

        private System.Random rand_gen = new System.Random(10);
        
		// Rendering Mode
		int renderMode = 0;
		
		public ShaderProgram texprogram;
		
		private VertexBuffer[] vertices = new VertexBuffer[100];
		
		private AnimationSprite[] sprite = new AnimationSprite[100];
				
		// Rendering Order (back to front)
		private int[] renderingOrder = new int[100];
		
        public AnimationWithScene()
        {
            // Simulation Scene Set Up
            InitScene();
			
			// Setup for Shader Program
			texprogram = new ShaderProgram("/Application/shaders/animation.cgx");
            texprogram.SetUniformBinding(0, "u_WorldMatrix");
            texprogram.SetUniformBinding(1, "u_Color");
			texprogram.SetUniformBinding(2, "u_TexSlip");
			texprogram.SetAttributeBinding( 0, "a_Position" );
			texprogram.SetAttributeBinding( 1, "a_TexCoord" );
			
            // Setup for Rendering Object
            for (int i = 0; i < numShape; i++)
            {
                if (sceneShapes[i].numVert == 0)
                {
                    vertices[i] = new VertexBuffer(37, VertexFormat.Float3, VertexFormat.Float2);
                }
                else
                {
                    vertices[i] = new VertexBuffer(sceneShapes[i].numVert + 1, VertexFormat.Float3, VertexFormat.Float2);
                }

				// Setup up rendering vertices and texture vertices
                MakeLineListConvex(sceneShapes[i], vertices[i]);
            }
			
			
			// Load texture images here
			// And bind it with AnimationSprite class
			{
				for (int i = 0; i < numShape; i++)
				{
					switch(i)
					{
						case 0:
						case 1:
							sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/renga.png", false, 1, 1, 1 );
							break;
		
						case 2:
							sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/monster.png", false, 4, 4, 20 );
							break;
						
						case 3:
							// The following is usual one without animation
							//sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/red.png", false, 1, 1, 1 );
							sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/monster.png", false, 4, 4, 20 );
							break;
						
						case 4:
							// The following is usual one without animation
							//sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/blue.png", false, 1, 1, 1 );
							sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/monster.png", false, 4, 4, 20 );
							break;
						
						default:
							sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/renga.png", false, 1, 1, 1 );
							break;
					}
				}
				
				for(int i=0; i<100; i++)
					renderingOrder[i] = i;
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

        ~AnimationWithScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {

            if(colVert != null) colVert.Dispose();
			
			if(texprogram != null) texprogram.Dispose();
			
			for (int i=0; i < numShape; i++)
				if(vertices[i] != null)
					vertices[i].Dispose();
			
			for (int i = 0; i < numShape; i++)
				sprite[i].Release();
			
        }

#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "AnimationWithScene";
			
            // Set the restitution coefficient a bit stronger
            this.restitutionCoeff = 0.8f;

            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(50, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(4.0f, 4.0f);
            sceneShapes[2] = new PhysicsShape(box_width);

            // Sphere Shape Setting PhysicsShape( "radius" )
            float sphere_width = 4.0f;
            sceneShapes[3] = new PhysicsShape(sphere_width);


            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 4.0f / 1000.0f;
            }

            // Convex Shape Setting (by using random points)
            sceneShapes[4] = PhysicsShape.CreateConvexHull(test_point, 10);

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

            // Create dynamic rigid sceneBodies
            {
                // Create a box-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
                    sceneBodies[numBody].position = new Vector2(-10.0f, -5.0f);
                    sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f);
                    sceneBodies[numBody].shapeIndex = 2;
                    numBody++;
                }

                // Create a sphere-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);
                    sceneBodies[numBody].position = new Vector2(0.0f, -5.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 3;
                    sceneBodies[numBody].colFriction = 0.01f;
                    numBody++;
                }

                // Create a convex-shaped dynamic rigid body
                {
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);
                    sceneBodies[numBody].position = new Vector2(10.0f, -5.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 4;
                    numBody++;
                }
            }

        }

#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "AnimationWithScene";
			
            // Set the restitution coefficient a bit stronger
            this.RestitutionCoeff = 0.8f;

            // Before making the rigid sceneBodies, setup collision shapes data

            // Box Shape Setting AddBoxShape(new Vector2("width", "height"))
            Vector2 wall_width = new Vector2(50, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
			AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(4.0f, 4.0f);
            AddBoxShape(box_width);

            // Sphere Shape Setting PhysicsShape( "radius" )
            float sphere_width = 4.0f;
            AddSphereShape(sphere_width);

            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 4.0f / 1000.0f;
            }

            // Convex Shape Setting (by using random points)
            AddConvexHullShape(test_point, 10);
     

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

            // Create dynamic rigid sceneBodies
            {
                // Create a box-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
                    body.Position = new Vector2(-10.0f, -5.0f);
                    body.Rotation = PhysicsUtility.GetRadian(30.0f);
                }

                // Create a sphere-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[3], 1.0f);
                    body.Position = new Vector2(0.0f, -5.0f);
                    body.Rotation = 0;
                    body.ColFriction = 0.01f;
                }

                // Create a convex-shaped dynamic rigid body
                {
                    PhysicsBody body = AddBody(SceneShapes[4], 1.0f);
                    body.Position = new Vector2(10.0f, -5.0f);
                    body.Rotation = 0;
                }
            }

        }
	
#endif

        // Game controller handling
        public override void KeyboardFunc(GamePadButtons button)
        {
            switch (button)
            {
				// Change rendering with texture or without texture
                case GamePadButtons.Square:    
					renderMode = (renderMode+1)%3;
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
				float[] tex = new float[2 * 37];
				
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
					
					tex[2 * i + 0] = x1/rad * 0.5f + 0.5f;
					tex[2 * i + 1] = y1/rad * 0.5f + 0.5f;
	
                    i++;
                }

                vertex[3 * i + 0] = vertex[3 * 0 + 0];
                vertex[3 * i + 1] = vertex[3 * 0 + 1];
                vertex[3 * i + 2] = vertex[3 * 0 + 2];
				
				tex[2 * i + 0] = tex[2 * 0 + 0]/rad * 0.5f + 0.5f;
				tex[2 * i + 1] = tex[2 * 0 + 1]/rad * 0.5f + 0.5f;

                vertices.SetVertices(0, vertex);
				vertices.SetVertices(1, tex);
            }
            else
            {
                float[] vertex = new float[3 * (con.numVert + 1)];
				float[] tex = new float[2 * (con.numVert + 1)];
				
                int i;

                for (i = 0; i < con.numVert; i++)
                {
                    Vector2 v1;
                    v1 = con.vertList[i];

                    vertex[3 * i + 0] = v1.X;
                    vertex[3 * i + 1] = v1.Y;
                    vertex[3 * i + 2] = 0.0f;
					
					tex[2 * i + 0] = v1.X;
					tex[2 * i + 1] = v1.Y;
                }

                vertex[3 * i + 0] = vertex[3 * 0 + 0];
                vertex[3 * i + 1] = vertex[3 * 0 + 1];
                vertex[3 * i + 2] = vertex[3 * 0 + 2];
				
				tex[2 * i + 0] = tex[2 * 0 + 0];
				tex[2 * i + 1] = tex[2 * 0 + 1];
				
                vertices.SetVertices(0, vertex);
				
				float x_min, y_min, x_max, y_max;
				
				x_min = x_max = tex[2 * i + 0];
				y_min = y_max = tex[2 * i + 1];
				
				for (i = 0; i <= con.numVert; i++)
                {
					x_min = Math.Min(tex[2 * i + 0], x_min);
					x_max = Math.Max(tex[2 * i + 0], x_max);
					y_min = Math.Min(tex[2 * i + 1], y_min);
					y_max = Math.Max(tex[2 * i + 1], y_max);
				}
				
				for (i = 0; i <= con.numVert; i++)
                {
					tex[2 * i + 0] = (tex[2 * i + 0] - x_min)/(x_max - x_min);
					tex[2 * i + 1] = (tex[2 * i + 1] - y_min)/(y_max - y_min);
				}
				
				vertices.SetVertices(1, tex);
            }
        }
		
        // Draw objects
        public override void DrawAllBody(ref GraphicsContext graphics, ref ShaderProgram program, Matrix4 renderMatrix, int click_index)
        {
			
			graphics.Enable(EnableMode.Blend);
			graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);	
			
			if((renderMode == 0)||(renderMode==2))
			{
				graphics.SetShaderProgram(texprogram);
				
	            for (int j = 0; j < numShape; j++)
	            {
	                for (int i = 0; i < numBody; i++)
	                {
	                    uint index = sceneBodies[i].shapeIndex;
	
	                    if (renderingOrder[j] != index) continue;
	
	                    Matrix4 rotationMatrix = Matrix4.RotationZ(sceneBodies[i].rotation);
	
	                    Matrix4 transMatrix = Matrix4.Translation(
	                        new Vector3(sceneBodies[i].position.X, sceneBodies[i].position.Y, 0.0f));
	
	                    Matrix4 local_rotationMatrix = Matrix4.RotationZ(sceneBodies[i].localRotation);
	
	                    Matrix4 local_transMatrix = Matrix4.Translation(
	                        new Vector3(sceneBodies[i].localPosition.X, sceneBodies[i].localPosition.Y, 0.0f));
	
	                    Matrix4 WorldMatrix = renderMatrix * transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix;
						
						sprite[index].Draw(ref graphics, ref texprogram, ref WorldMatrix);
	
					}
	            }
										
				graphics.Disable(EnableMode.Blend);
		
				graphics.SetShaderProgram(program);
			}

			
			if((renderMode == 0)||(renderMode == 1))
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
	                        Vector3 color = new Vector3(1.0f, 1.0f, 0.0f);
	                        program.SetUniformValue(1, ref color);
	
	                    }
	
	                    if (sceneShapes[index].numVert == 0)
	                        graphics.DrawArrays(DrawMode.LineStrip, 0, 37);
	                    else
	                        graphics.DrawArrays(DrawMode.LineStrip, 0, sceneShapes[index].numVert + 1);
	
	                }
	            }
			}
        }
		
        // Debug rendering for contact points(RigidBody A <=> RigidBody B)
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


		
		

		

		
