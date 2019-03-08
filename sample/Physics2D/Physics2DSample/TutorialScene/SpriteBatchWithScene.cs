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


// [How to play] 
//
// By touching the area of the sky at first, eye point is moved to the center of the touch point.
//
// By pulling and releasing the yellow little monster, it is thrown up to the air.  
//
// By pressing GamePadButtons.Square, rendering mode is changed.
//
// You can also grab or pull any of rigid bodies inside the scene.
//

// use direct access to structure of simulation data to set up initial scene
#define USE_OLD_API

using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

using System.Diagnostics;
using System.Runtime.InteropServices;

// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

namespace Physics2DSample
{
    // To create a simulation scene, please inherit from PhysicsScene class
    public class SpriteBatchWithScene : PhysicsScene
	{
		
		// AnimationSpriteBatch Class		
		// This is faster thatn AnimationSprite Class
		public class AnimationSpriteBatch
		{
			private long currentFrame = 0;
			
			private int frameSync = 1;
			private int frameWidth = 1;
			private int frameHeight = 1;
			
			public bool startAnimation = true;
			
			private VertexBuffer vertices = null;
			private Texture2D texture = null;
			
			private int numVert = 0;
			private int numBatch = 0;

			private float[] orgVertex = null;
			private float[] orgTexture = null;
			
			private float[] transVertex = null;
			private float[] transTexture = null;
		
			public Matrix4[] mMatrix = null;

	        public AnimationSpriteBatch(PhysicsShape con, string fileName, bool mipmap, int num, int width, int height, int sync)
	        {
				
				frameWidth = width;
				frameHeight = height;
				frameSync = sync;
				
				numBatch = num;
				
				mMatrix = new Matrix4[numBatch];
	
	            if (con.numVert == 0)
	            {
					numVert = 36 * 3;
					
					vertices = new VertexBuffer(numVert * numBatch, VertexFormat.Float3, VertexFormat.Float2);
					
					// original vertex before transformation
	                orgVertex = new float[3 * numVert];
					orgTexture = new float[2 * numVert];
					
					// transformed vertex
		            transVertex = new float[3 * numVert * numBatch];
					transTexture = new float[2 * numVert * numBatch];
					
	                int i = 0;
	                float rad = con.vertList[0].X;
	
	                for (float th = 0.0f; th < 360.0f; th += 10.0f)
	                {
						orgVertex[9 * i + 0] = 0.0f;
	                    orgVertex[9 * i + 1] = 0.0f;
	                    orgVertex[9 * i + 2] = 0.0f;
						
	                    orgVertex[9 * i + 3] = rad * (float)Math.Cos(th / 180.0f * PhysicsUtility.Pi);
	                    orgVertex[9 * i + 4] = rad * (float)Math.Sin(th / 180.0f * PhysicsUtility.Pi);
	                    orgVertex[9 * i + 5] = 0.0f;

	                    orgVertex[9 * i + 6] = rad * (float)Math.Cos((th+10.0f) / 180.0f * PhysicsUtility.Pi);
	                    orgVertex[9 * i + 7] = rad * (float)Math.Sin((th+10.0f) / 180.0f * PhysicsUtility.Pi);
	                    orgVertex[9 * i + 8] = 0.0f;

						orgTexture[6 * i + 0] = 0.0f;
	                    orgTexture[6 * i + 1] = 0.0f;
						
	                    orgTexture[6 * i + 2] = 0.5f * (float)Math.Cos(th / 180.0f * PhysicsUtility.Pi) + 0.5f;
	                    orgTexture[6 * i + 3] = 0.5f * (float)Math.Sin(th / 180.0f * PhysicsUtility.Pi) + 0.5f;

	                    orgTexture[6 * i + 4] = 0.5f * (float)Math.Cos((th+10.0f) / 180.0f * PhysicsUtility.Pi) + 0.5f;
	                    orgTexture[6 * i + 5] = 0.5f * (float)Math.Sin((th+10.0f) / 180.0f * PhysicsUtility.Pi) + 0.5f;
						
	                    i++;
	                }
	            }
	            else
	            {
					numVert = (con.numVert-2) * 3;
					
					vertices = new VertexBuffer(numVert * numBatch, VertexFormat.Float3, VertexFormat.Float2);
					
	                orgVertex = new float[3 * numVert];
					orgTexture = new float[2 * numVert];
				
	                transVertex = new float[3 * numVert * numBatch];
					transTexture = new float[2 * numVert * numBatch];
					
	                int i;
	
	                for (i = 1; i < con.numVert-1; i++)
	                {
	                    orgVertex[9 * (i-1) + 0] = con.vertList[0].X;
	                    orgVertex[9 * (i-1) + 1] = con.vertList[0].Y;
	                    orgVertex[9 * (i-1) + 2] = 0.0f;
						
						orgVertex[9 * (i-1) + 3] = con.vertList[i].X;
	                    orgVertex[9 * (i-1) + 4] = con.vertList[i].Y;
	                    orgVertex[9 * (i-1) + 5] = 0.0f;
						
						orgVertex[9 * (i-1) + 6] = con.vertList[i+1].X;
	                    orgVertex[9 * (i-1) + 7] = con.vertList[i+1].Y;
	                    orgVertex[9 * (i-1) + 8] = 0.0f;
						
						orgTexture[6 * (i-1) + 0] = con.vertList[0].X;
						orgTexture[6 * (i-1) + 1] = con.vertList[0].Y;
						
						orgTexture[6 * (i-1) + 2] = con.vertList[i].X;
						orgTexture[6 * (i-1) + 3] = con.vertList[i].Y;			
						
						orgTexture[6 * (i-1) + 4] = con.vertList[i+1].X;
						orgTexture[6 * (i-1) + 5] = con.vertList[i+1].Y;
	                }

					
					float x_min, y_min, x_max, y_max;
					
					x_min = x_max = orgTexture[0];
					y_min = y_max = orgTexture[1];
					
					for (i = 0; i < (con.numVert-2) * 3; i++)
	                {
						x_min = Math.Min(orgTexture[2 * i + 0], x_min);
						x_max = Math.Max(orgTexture[2 * i + 0], x_max);
						y_min = Math.Min(orgTexture[2 * i + 1], y_min);
						y_max = Math.Max(orgTexture[2 * i + 1], y_max);
					}
					
					for (i = 0; i < (con.numVert-2) * 3; i++)
	                {
						orgTexture[2 * i + 0] = (orgTexture[2 * i + 0] - x_min)/(x_max - x_min);
						orgTexture[2 * i + 1] = (orgTexture[2 * i + 1] - y_min)/(y_max - y_min);
					}
	            }

				// Create Texture
				texture = new Texture2D(fileName, mipmap, PixelFormat.Rgba);
	        }
			
			// Should release vertex buffer and texture before going out of this scene
	        public void Release()
			{
				if(vertices != null) vertices.Dispose();
				if(texture != null) texture.Dispose();
	        }
			
	        ~AnimationSpriteBatch()
			{
				Release();
	        }
			
			public void Draw(ref GraphicsContext graphics, ref ShaderProgram program, ref Matrix4 renderMatrix)
			{	
				
				int xPos = ((int)(currentFrame/frameSync)%(frameWidth*frameHeight))%frameWidth;
				int yPos = ((int)(currentFrame/frameSync)%(frameWidth*frameHeight))/frameWidth;
		
				
				// Transformation
				for(int i=0; i<numBatch; i++)
				{
					for(int j=0; j<numVert; j++)
					{
						Vector4 vec = new Vector4(
							orgVertex[3*j+0],
							orgVertex[3*j+1],
							orgVertex[3*j+2],
							1.0f);
	
						vec = mMatrix[i] * vec;
						
						transVertex[3*i*numVert + 3*j+0] = vec.X;
						transVertex[3*i*numVert + 3*j+1] = vec.Y;					
						transVertex[3*i*numVert + 3*j+2] = vec.Z;
						
						transTexture[2*i*numVert + 2*j + 0] = (1.0f/(float)frameWidth)*xPos + (1.0f/(float)frameWidth)*orgTexture[2*j + 0];
						transTexture[2*i*numVert + 2*j + 1] = (1.0f/(float)frameHeight)*yPos + (1.0f/(float)frameHeight)*orgTexture[2*j + 1];						
						transTexture[2*i*numVert + 2*j + 1] = 1.0f - transTexture[2*i*numVert + 2*j + 1];
					}
				}
						
				vertices.SetVertices(0, transVertex);
				vertices.SetVertices(1, transTexture);
			
				program.SetUniformValue(0, ref renderMatrix);
				
				Vector3 color = new Vector3(1.0f, 1.0f, 1.0f);
				program.SetUniformValue(1, ref color);
				
				graphics.SetVertexBuffer(0, vertices);	
				graphics.SetTexture(0, texture);
				
				graphics.DrawArrays(DrawMode.Triangles, 0, numVert*numBatch);
				
			}
						
			public void Update()
			{
				if(startAnimation)
					currentFrame++;
			}
		}
		
				
		// AnimationSprite Class
		public class AnimationSprite
		{
			long currentFrame = 0;
			int frameSync = 1;
			
			int frameWidth = 1;
			int frameHeight = 1;
			
			public bool startAnimation = true;
			
			VertexBuffer vertices = null;
			Texture2D texture = null;
			
			private int numVert = 0;
			
			private float[] orgVertex = null;
			private float[] orgTexture = null;
			private float[] transTexture = null;

	        public AnimationSprite(PhysicsShape con, string fileName, bool mipmap, int width, int height, int sync)
	        {
				frameWidth = width;
				frameHeight = height;
				frameSync = sync;
				
	            if (con.numVert == 0)
	            {
					vertices = new VertexBuffer(37, VertexFormat.Float3, VertexFormat.Float2);
					numVert = 36;
					
	                orgVertex = new float[3 * 37];
					orgTexture = new float[2 * 37];
					transTexture = new float[2 * 37];
						
	                int i = 0;
	                float rad = con.vertList[0].X;
	
	                for (float th1 = 0.0f; th1 < 360.0f; th1 = th1 + 10.0f)
	                {
	                    float th1_rad = th1 / 180.0f * PhysicsUtility.Pi;
	
	                    float x1 = rad * (float)Math.Cos(th1_rad);
	                    float y1 = rad * (float)Math.Sin(th1_rad);
	
	                    orgVertex[3 * i + 0] = x1;
	                    orgVertex[3 * i + 1] = y1;
	                    orgVertex[3 * i + 2] = 0.0f;
						
						orgTexture[2 * i + 0] = x1/rad * 0.5f + 0.5f;
						orgTexture[2 * i + 1] = y1/rad * 0.5f + 0.5f;
		
	                    i++;
	                }
	
	                orgVertex[3 * i + 0] = orgVertex[3 * 0 + 0];
	                orgVertex[3 * i + 1] = orgVertex[3 * 0 + 1];
	                orgVertex[3 * i + 2] = orgVertex[3 * 0 + 2];
					
					orgTexture[2 * i + 0] = orgTexture[2 * 0 + 0]/rad * 0.5f + 0.5f;
					orgTexture[2 * i + 1] = orgTexture[2 * 0 + 1]/rad * 0.5f + 0.5f;
	
	                vertices.SetVertices(0, orgVertex);
					vertices.SetVertices(1, orgTexture);
	            }
	            else
	            {
					vertices = new VertexBuffer(con.numVert + 1, VertexFormat.Float3, VertexFormat.Float2);
					numVert = con.numVert;
					
	                orgVertex = new float[3 * (con.numVert + 1)];
					orgTexture = new float[2 * (con.numVert + 1)];
					transTexture = new float[2 * (con.numVert + 1)];
					
	                int i;
	
	                for (i = 0; i < con.numVert; i++)
	                {
	                    Vector2 v1;
	                    v1 = con.vertList[i];
	
	                    orgVertex[3 * i + 0] = v1.X;
	                    orgVertex[3 * i + 1] = v1.Y;
	                    orgVertex[3 * i + 2] = 0.0f;
						
						orgTexture[2 * i + 0] = v1.X;
						orgTexture[2 * i + 1] = v1.Y;
	                }
	
	                orgVertex[3 * i + 0] = orgVertex[3 * 0 + 0];
	                orgVertex[3 * i + 1] = orgVertex[3 * 0 + 1];
	                orgVertex[3 * i + 2] = orgVertex[3 * 0 + 2];
					
					orgTexture[2 * i + 0] = orgTexture[2 * 0 + 0];
					orgTexture[2 * i + 1] = orgTexture[2 * 0 + 1];
					
	                vertices.SetVertices(0, orgVertex);
					
					float x_min, y_min, x_max, y_max;
					
					x_min = x_max = orgTexture[2 * i + 0];
					y_min = y_max = orgTexture[2 * i + 1];
					
					for (i = 0; i <= con.numVert; i++)
	                {
						x_min = Math.Min(orgTexture[2 * i + 0], x_min);
						x_max = Math.Max(orgTexture[2 * i + 0], x_max);
						y_min = Math.Min(orgTexture[2 * i + 1], y_min);
						y_max = Math.Max(orgTexture[2 * i + 1], y_max);
					}
					
					for (i = 0; i <= con.numVert; i++)
	                {
						orgTexture[2 * i + 0] = (orgTexture[2 * i + 0] - x_min)/(x_max - x_min);
						orgTexture[2 * i + 1] = (orgTexture[2 * i + 1] - y_min)/(y_max - y_min);
					}
					
					vertices.SetVertices(1, orgTexture);
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
			}
			
			public void Update()
			{
				if(startAnimation)
					currentFrame++;
			}
		}
				
		
		// Rendering Mode
		int renderMode = 0;
		
        // Vertex Buffer for Debug Rendering
        private VertexBuffer colVert = null;
		
		// VertexBuffer used for rendering joints
        private VertexBuffer jointVert = null;
		
       	private System.Random rand_gen = new System.Random(10);
		
		// Animation Shader
		public ShaderProgram animprogram;
		
		// Texture Shader
		public ShaderProgram texprogram;
		
		private VertexBuffer[] vertices = new VertexBuffer[100];
		private AnimationSprite[] sprite = new AnimationSprite[100];
		private AnimationSpriteBatch[] spritebatch = new AnimationSpriteBatch[100];
		
		// Rendering Order (back to front)
		private int[] renderingOrder = new int[100];
		
		private int clickIndex = -1;
		private int oldclickIndex = -1;
		private int littleYellowIndex = -1;

		public bool eyeTrack = false;
		
		public Vector2 eyePos = new Vector2(0.0f, 0.0f);
		public Vector2 clickPos = new Vector2(0, 0);
			
		private Stopwatch totalPerf = new Stopwatch();
		private Stopwatch simPerf = new Stopwatch();
		
        public SpriteBatchWithScene()
        {
            // Simulation Scene Set Up
            InitScene();
			
			// Setup for Shader Program
			texprogram = new ShaderProgram("/Application/shaders/texture.cgx");
            texprogram.SetUniformBinding(0, "u_WorldMatrix");
            texprogram.SetUniformBinding(1, "u_Color");
			texprogram.SetAttributeBinding( 0, "a_Position" );
			texprogram.SetAttributeBinding( 1, "a_TexCoord" );
			
			// Setup for Shader Program
			animprogram = new ShaderProgram("/Application/shaders/animation.cgx");
            animprogram.SetUniformBinding(0, "u_WorldMatrix");
            animprogram.SetUniformBinding(1, "u_Color");
			animprogram.SetUniformBinding(2, "u_TexSlip");
			animprogram.SetAttributeBinding( 0, "a_Position" );
			animprogram.SetAttributeBinding( 1, "a_TexCoord" );
			
			
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
			for (int i = 0; i < numShape; i++)
			{
				switch(i)
				{
					case 0:
					case 1:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/renga.png", true, 1, 1, 1 );
						break;
	
					case 2:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/monster.png", true, 4, 4, 5 );
						break;
					
					case 3:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/red.png", true, 1, 1, 1 );
						break;
					
					case 4:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/blue.png", true, 1, 1, 1 );
						break;
					
					case 7:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/background.png", true, 1, 1, 1 );
						break;
	
					case 8:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/robot.png", true, 4, 4, 5 );
						break;
					
					default:
						sprite[i] = new AnimationSprite(sceneShapes[i], "/Application/data/blue.png", true, 1, 1, 1 );
						break;
				}
			}
			
			for (int i = 0; i < numShape; i++)
			{
				int counter = 0;
				
				for(int j=0; j< numBody; j++)
				{
					if(sceneBodies[j].shapeIndex == i)
						counter++;
				}
				
				switch(i)
				{
					case 0:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/renga.png", true, counter, 1, 1, 1 );
						break;
					
					case 1:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/renga.png", true, counter, 1, 1, 1 );
						break;
	
					case 2:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/monster.png", true, counter, 4, 4, 5 );
						break;
					
					case 3:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/red.png", true, counter, 1, 1, 1 );
						break;
					
					case 4:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/blue.png", true, counter, 1, 1, 1 );
						break;
					
					case 5:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/blue.png", true, counter, 1, 1, 1 );
						break;
					
					case 6:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/blue.png", true, counter, 1, 1, 1 );
						break;
					
					case 7:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/background.png", true, counter, 1, 1, 1 );
						break;
	
					case 8:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/robot.png", true, counter, 4, 4, 5 );
						break;
					
					default:
						spritebatch[i] = new AnimationSpriteBatch(sceneShapes[i], "/Application/data/blue.png", true, counter, 1, 1, 1 );
						break;
				}
				
			}
			
			
			for(int i=0; i<100; i++)
				renderingOrder[i] = i;
			
			renderingOrder[0] = 7;
			renderingOrder[1] = 0;
			renderingOrder[2] = 1;
			renderingOrder[3] = 3;
			renderingOrder[4] = 4;
			renderingOrder[5] = 5;
			renderingOrder[6] = 6;
			renderingOrder[7] = 2;
			renderingOrder[8] = 8;
			
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

        ~SpriteBatchWithScene()
        {
            ReleaseScene();
        }

        public override void ReleaseScene()
        {

            if(colVert != null) colVert.Dispose();
			
			if(jointVert != null) jointVert.Dispose();
			
			if(texprogram != null) texprogram.Dispose();
			
			if(animprogram != null) animprogram.Dispose();
			
			for (int i=0; i < numShape; i++)
				if(vertices[i] != null)
					vertices[i].Dispose();
			
			for (int i = 0; i < numShape; i++)
				if(sprite[i]!= null) sprite[i].Release();
			
			for (int i = 0; i < numShape; i++)
				if(spritebatch[i]!= null) spritebatch[i].Release ();
			
        }
		
#if USE_OLD_API
		
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			sceneName = "SpriteBatchWithScene";
			
			printPerf = true;
			
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting PhysicsShape( "width", "height" )
            Vector2 wall_width = new Vector2(200, 8);
            sceneShapes[0] = new PhysicsShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            sceneShapes[1] = new PhysicsShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            sceneShapes[2] = new PhysicsShape(box_width);

            // Sphere Shape Setting PhysicsShape( "radius" )
            float sphere_width = 2.0f;
            sceneShapes[3] = new PhysicsShape(sphere_width);

            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 2.0f / 1000.0f;
            }

            // Convex Shape Setting (by using random points)
            sceneShapes[4] = PhysicsShape.CreateConvexHull(test_point, 10);
			
            Vector2 box_width2 = new Vector2(0.5f, 4.0f);
            sceneShapes[5] = new PhysicsShape(box_width2);
            
			Vector2 box_width3 = new Vector2(5.0f, 0.5f);
            sceneShapes[6] = new PhysicsShape(box_width3);
			
			Vector2 box_width4 = new Vector2(200, 40);
			sceneShapes[7] = new PhysicsShape(box_width4);
			       
			Vector2 box_width5 = new Vector2(2.0f, 2.0f);
            sceneShapes[8] = new PhysicsShape(box_width5);
			
            numShape = 9;

 			// Create background wall without collision detection
            {
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[7], PhysicsUtility.FltMax);
           		sceneBodies[numBody].position = new Vector2(100, 15);
            	sceneBodies[numBody].rotation = 0;
				sceneBodies[numBody].shapeIndex = 7;
				sceneBodies[numBody].groupFilter = 0;
				numBody++;
			}
			
			// Create static walls to limit the range of action of active rigid sceneBodies
			{
                // new PhysicsBody( "shape of the body",  "mass of the body(kg)" ) 
                sceneBodies[numBody] = new PhysicsBody(sceneShapes[0], PhysicsUtility.FltMax);

                // Set the position & the rotation
                sceneBodies[numBody].position = new Vector2(0, -wall_height.Y);
                sceneBodies[numBody].rotation = 0;

                // Make shapeIndex consistent with what we set as convex shape
                sceneBodies[numBody].shapeIndex = 0;
				sceneBodies[numBody].position += new Vector2(100, 0);
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(wall_width.X, 0);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 1;
				sceneBodies[numBody].position += new Vector2(100, 0);
                numBody++;

                sceneBodies[numBody] = new PhysicsBody(sceneShapes[1], PhysicsUtility.FltMax);
                sceneBodies[numBody].position = new Vector2(-wall_width.X, 0);
                sceneBodies[numBody].rotation = 0;
                sceneBodies[numBody].shapeIndex = 1;
				sceneBodies[numBody].position += new Vector2(100, 0);
                numBody++;
            }
             
			
			// Create Little Yellow Monster
			{
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[2], 1.0f);
				sceneBodies[numBody].position = new Vector2(-30.0f, -5.0f);
				sceneBodies[numBody].rotation = PhysicsUtility.GetRadian(30.0f); 
				sceneBodies[numBody].shapeIndex = 2;
				littleYellowIndex = numBody++;

				PhysicsBody bodyA = sceneBodies[0];
				PhysicsBody bodyB = sceneBodies[numBody-1];
				sceneJoints[numJoint] = new PhysicsJoint(bodyA, bodyB, bodyB.position, (uint)0, (uint)numBody-1);
				sceneJoints[numJoint].axis1Lim = (new Vector2(1.0f, 0.0f)).Normalize() * 0.1f;
				sceneJoints[numJoint].axis2Lim = (new Vector2(0.0f, 1.0f)).Normalize() * 0.1f;
				sceneJoints[numJoint].angleLim =1;
				numJoint++;
			}
			
			// Create a sphere-shaped dynamic rigid body
			{   
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[3], 1.0f);  
				sceneBodies[numBody].position = new Vector2(-20.0f, -5.0f);
				sceneBodies[numBody].rotation = 0;     
				sceneBodies[numBody].shapeIndex = 3;  
				sceneBodies[numBody].colFriction = 0.01f;     
				numBody++;    
			}

               
			// Create a convex-shaped dynamic rigid body     
			{   
				sceneBodies[numBody] = new PhysicsBody(sceneShapes[4], 1.0f);   
				sceneBodies[numBody].position = new Vector2(-10.0f, -5.0f);  
				sceneBodies[numBody].rotation = 0;      
				sceneBodies[numBody].shapeIndex = 4;  
				numBody++;
			}
      
			// Create a target tower
			for(int block = 0; block < 5; block++)
			{
					Vector2 center = new Vector2(30.0f * block, 0.0f);
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
                    sceneBodies[numBody].position = new Vector2(50.0f, -wall_height.Y + wall_width.Y + 0.005f + box_width2.Y);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 5;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++; 
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
                    sceneBodies[numBody].position = new Vector2(60.0f, -wall_height.Y + wall_width.Y + 0.005f + box_width2.Y);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 5;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++;
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
                    sceneBodies[numBody].position = new Vector2(70.0f, -wall_height.Y + wall_width.Y + 0.005f + box_width2.Y);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 5;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++; 
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
                    sceneBodies[numBody].position = new Vector2(55.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + box_width3.Y + 0.005f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 6;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++; 
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[6], 1.0f);
                    sceneBodies[numBody].position = new Vector2(65.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + box_width3.Y + 0.005f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 6;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++; 
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
                    sceneBodies[numBody].position = new Vector2(55.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + 2.0f*box_width3.Y + 0.005f + box_width2.Y);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 5;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++;
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[5], 1.0f);
                    sceneBodies[numBody].position = new Vector2(65.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + 2.0f*box_width3.Y + 0.005f + box_width2.Y);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 5;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++; 
				
                    sceneBodies[numBody] = new PhysicsBody(sceneShapes[8], 1.0f);
                    sceneBodies[numBody].position = new Vector2(60.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + 2.0f*box_width3.Y + 0.005f + 2.0f);
                    sceneBodies[numBody].rotation = 0;
                    sceneBodies[numBody].shapeIndex = 8;
					sceneBodies[numBody].sleep = true;
					sceneBodies[numBody].position += center;
					numBody++; 
			}	

        }

#else
		public override void InitScene ()
        {
            // Create an empty simulation scene 
            base.InitScene();
			SceneName = "SpriteBatchWithScene";
			
			PrintPerf = true;
			
            // Before making the rigid sceneBodies, setup collision shapes data
          
            // Box Shape Setting AddBoxShape( "width", "height" )
            Vector2 wall_width = new Vector2(200, 8);
            AddBoxShape(wall_width);

            Vector2 wall_height = new Vector2(8, 30);
            AddBoxShape(wall_height);

            Vector2 box_width = new Vector2(2.0f, 2.0f);
            AddBoxShape(box_width);

            // Sphere Shape Setting AddSphereShape( "radius" )
            float sphere_width = 2.0f;
            AddSphereShape(sphere_width);

            Vector2[] test_point = new Vector2[10];

            for (int i = 0; i < 10; i++)
            {
                test_point[i] = new Vector2(rand_gen.Next(-1000, 1000), rand_gen.Next(-1000, 1000)) * 2.0f / 1000.0f;
            }

            // Convex Shape Setting (by using random points)
			AddConvexHullShape(test_point, 10);
			
            Vector2 box_width2 = new Vector2(0.5f, 4.0f);
            AddBoxShape(box_width2);
            
			Vector2 box_width3 = new Vector2(5.0f, 0.5f);
            AddBoxShape(box_width3);
			
			Vector2 box_width4 = new Vector2(200, 40);
			AddBoxShape(box_width4);
			       
			Vector2 box_width5 = new Vector2(2.0f, 2.0f);
            AddBoxShape(box_width5);

 			// Create background wall without collision detection
            {
				PhysicsBody body = AddBody(SceneShapes[7], PhysicsUtility.FltMax);
           		body.Position = new Vector2(100, 15);
            	body.Rotation = 0;
				body.GroupFilter = 0;
			}
			
			// Create static walls to limit the range of action of active rigid sceneBodies
			{
				PhysicsBody body = null;
                
				// AddBody( "shape of the body",  "mass of the body(kg)" ) 
                body = AddBody(SceneShapes[0], PhysicsUtility.FltMax);

                // Set the position & the rotation
                body.Position = new Vector2(0, -wall_height.Y);
                body.Rotation = 0;
				body.Position += new Vector2(100, 0);

                body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(wall_width.X, 0);
                body.Rotation = 0;
				body.Position += new Vector2(100, 0);

                body = AddBody(SceneShapes[1], PhysicsUtility.FltMax);
                body.Position = new Vector2(-wall_width.X, 0);
                body.Rotation = 0;
				body.Position += new Vector2(100, 0);
            }
             
			
			// Create Little Yellow Monster
			{
				PhysicsBody body = AddBody(SceneShapes[2], 1.0f);
				body.Position = new Vector2(-30.0f, -5.0f);
				body.Rotation = PhysicsUtility.GetRadian(30.0f); 
				littleYellowIndex = NumBody-1;

				PhysicsBody bodyA = SceneBodies[0];
				PhysicsBody bodyB = SceneBodies[NumBody-1];
				PhysicsJoint joint = AddJoint(bodyA, bodyB, bodyB.Position);
				joint.Axis1Lim = (new Vector2(1.0f, 0.0f)).Normalize() * 0.1f;
				joint.Axis2Lim = (new Vector2(0.0f, 1.0f)).Normalize() * 0.1f;
				joint.AngleLim = 1;
			}
			
			// Create a sphere-shaped dynamic rigid body
			{   
				PhysicsBody body = AddBody(SceneShapes[3], 1.0f);  
				body.Position = new Vector2(-20.0f, -5.0f);
				body.Rotation = 0;     
				body.ColFriction = 0.01f;         
			}

               
			// Create a convex-shaped dynamic rigid body     
			{   
				PhysicsBody body = AddBody(SceneShapes[4], 1.0f);   
				body.Position = new Vector2(-10.0f, -5.0f);  
				body.Rotation = 0;      
			}
      
			// Create a target tower
			for(int block = 0; block < 5; block++)
			{
					PhysicsBody body = null;
					
					Vector2 center = new Vector2(30.0f * block, 0.0f);
				
                    body = AddBody(SceneShapes[5], 1.0f);
                    body.Position = new Vector2(50.0f, -wall_height.Y + wall_width.Y + 0.005f + box_width2.Y);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;

                    body = AddBody(SceneShapes[5], 1.0f);
                    body.Position = new Vector2(60.0f, -wall_height.Y + wall_width.Y + 0.005f + box_width2.Y);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;

                    body = AddBody(SceneShapes[5], 1.0f);
                    body.Position = new Vector2(70.0f, -wall_height.Y + wall_width.Y + 0.005f + box_width2.Y);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;

                    body = AddBody(SceneShapes[6], 1.0f);
                    body.Position = new Vector2(55.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + box_width3.Y + 0.005f);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;

                    body = AddBody(SceneShapes[6], 1.0f);
                    body.Position = new Vector2(65.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + box_width3.Y + 0.005f);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;

                    body = AddBody(SceneShapes[5], 1.0f);
                    body.Position = new Vector2(55.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + 2.0f*box_width3.Y + 0.005f + box_width2.Y);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;;

                    body = AddBody(SceneShapes[5], 1.0f);
                    body.Position = new Vector2(65.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + 2.0f*box_width3.Y + 0.005f + box_width2.Y);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center;

                    body = AddBody(SceneShapes[8], 1.0f);
                    body.Position = new Vector2(60.0f, -wall_height.Y + wall_width.Y + 0.005f + 2.0f*box_width2.Y + 2.0f*box_width3.Y + 0.005f + 2.0f);
                    body.Rotation = 0;
					body.Sleep = true;
					body.Position += center; 
			}	

        }
#endif
		// Update animation before rendering
		public override void UpdateFuncBeforeSim ()
		{
			simPerf.Reset();
			simPerf.Start();
			
			// Eye position is moved based on the position of the character
			if(eyeTrack)
			{
				eyePos += 0.05f * (sceneBodies[littleYellowIndex].position + new Vector2(0, 15) - eyePos);	
			}
			else if(clickIndex == -1)
			{
				eyePos.X += 0.02f * (clickPos.X - eyePos.X);
			
				if(eyePos.X < -50.0f)
					eyePos.X = -50.0f;
				
				if(eyePos.X > 250.0f)
					eyePos.X = 250.0f;
			}
			
			// Check whether the little yellow monster is thrown up to the air
			if((eyeTrack != true)&&Vector2.Length(sceneBodies[littleYellowIndex].position - new Vector2(-30.0f, -5.0f)) > 10.0f)
			{
				// Check whether drag is released
				if((oldclickIndex == littleYellowIndex)&&(clickIndex != littleYellowIndex))
				{
					numJoint = 0;
					sceneBodies[littleYellowIndex].velocity = 2.5f*(new Vector2(-30.0f, -5.0f) - sceneBodies[littleYellowIndex].position);
					eyeTrack = true;
				}
			}
			
			oldclickIndex = clickIndex;
			
        }
		
		// Update animation before rendering
		public override void UpdateFuncAfterSim ()
		{
			for (int i = 0; i < numShape; i++)
			{
				sprite[i].Update();
				spritebatch[i].Update();
			}
				
			simPerf.Stop();
			totalPerf.Stop();
			
			if(sceneFrame % 20 == 0)
			{
				sceneName = "SpriteBatchWithScene" + " " + "Total=" + totalPerf.ElapsedMilliseconds + "[ms]" + " Sim=" + simPerf.ElapsedMilliseconds + "[ms]";
			
				if(renderMode == 0)
					sceneName += "[SpriteBatch]";
				else if(renderMode == 1)
					sceneName += "[Sprite]";
			}
			
			totalPerf.Reset();
			totalPerf.Start();
		
		}

        // Game controller handling
        public override void KeyboardFunc(GamePadButtons button)
        {
            switch (button)
            {
				// Change rendering with texture or without texture
                case GamePadButtons.Square:    
					renderMode = (renderMode+1)%4;
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
			clickIndex = click_index;
			
			graphics.Enable(EnableMode.Blend);
			graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			
			// AnimationSpriteBatch
			if((renderMode == 0)||(renderMode==2))
			{
				graphics.SetShaderProgram(texprogram);	
				
				for (int j = 0; j < numShape; j++)
	            {	
					int counter = 0;
					
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
	
						// Transform positions of sprite batch
	                    Matrix4 WorldMatrix = transMatrix * rotationMatrix * local_transMatrix * local_rotationMatrix;
						
						spritebatch[renderingOrder[j]].mMatrix[counter++] = WorldMatrix;
					}
					
					spritebatch[renderingOrder[j]].Draw(ref graphics, ref texprogram, ref renderMatrix);
	            }
			
			}
			
			// AnimationSprite
			if(renderMode == 1)
			{

				graphics.SetShaderProgram(animprogram);
		
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
						
						sprite[index].Draw(ref graphics, ref animprogram, ref WorldMatrix);
	
					}
	            }
			}
	            

			
				
			graphics.Disable(EnableMode.Blend);
			graphics.SetShaderProgram(program);
	
			if((renderMode == 2)||(renderMode == 3))
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
	                        Vector3 color = new Vector3(0.0f, 0.5f, 1.0f);
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
			bool renderCollPoint = false;
			
			// Draw contact points
			graphics.SetVertexBuffer(0, colVert);
			
			if((renderMode == 2)||(renderMode == 3))
			{
				if(renderCollPoint)
				{
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

                	Vector3 color = new Vector3(0.0f, 0.0f, 1.0f);
                	program.SetUniformValue(1, ref color);

                	graphics.DrawArrays(DrawMode.Points, 0, 2);
                	graphics.DrawArrays(DrawMode.Lines, 0, 2);
				}
            }
			
        }
	}
}


		
		

		

		
