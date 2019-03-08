
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

using System.IO;
using System.Reflection;


namespace Tutorial.Utility
{
	/// <summary>簡単なスプライト。</summary>
	public class SimpleSprite
	{
		static ShaderProgram shaderProgram;
		
		protected GraphicsContext graphics;
		
		//@e Vertex coordinate
		//@j 頂点座標。
		float[] vertices=new float[12];
		
		//@e Texture coordinate
		//@j テクスチャ座標。
		float[] texcoords = {
			0.0f, 0.0f,	// left top
			0.0f, 1.0f,	// left bottom
			1.0f, 0.0f,	// right top
			1.0f, 1.0f,	// right bottom
		};
		
		//@e Vertex color
		//@j 頂点色。
		float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,	// left top
			1.0f,	1.0f,	1.0f,	1.0f,	// left bottom
			1.0f,	1.0f,	1.0f,	1.0f,	// right top
			1.0f,	1.0f,	1.0f,	1.0f,	// right bottom
		};
		
		//@e Index
		//@j インデックス。
		const int indexSize = 4;
		ushort[] indices;
		
		//@e Vertex buffer 
		//@j 頂点バッファ。
		VertexBuffer vertexBuffer;
		
		protected Texture2D texture;
		
		//@e Property cannot describe Position.X, public variable is used.
		//@j プロパティではPosition.Xという記述ができないため、publicの変数にしています。
		
		/// <summary>スプライトの表示位置。</summary>
		public Vector3 Position;
		
		/// <summary>
		/// スプライトの中心座標を指定。0.0f～1.0fの範囲で指定してください。
		/// スプライトの中心を指定する場合 X=0.5f, Y=0.5fを指定します。
		/// </summary>
		public Vector2 Center;
		
		public Vector2 Scale=Vector2.One;
		
		protected Vector4 color=Vector4.One;
		

		float width,height;
		/// <summary>スプライトの幅。</summary>
		public float Width 
		{
			get { return width * Scale.X;}
		}
		/// <summary>スプライトの高さ。</summary>
		public float Height 
		{
			get { return height * Scale.Y;}
		}

		/// <summary>コンストラクタ。</summary>
		public SimpleSprite(GraphicsContext graphics, Texture2D texture)
		{
			if(shaderProgram == null)
			{
				shaderProgram=CreateSimpleSpriteShader();
			}
			
			if (texture == null)
			{
				throw new Exception("ERROR: texture is null.");
			}
			
			this.graphics = graphics;
			this.texture = texture;
			this.width = texture.Width;
			this.height = texture.Height;

			indices = new ushort[indexSize];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 3;
			
			
			//@e                                                Vertex coordinate,               Texture coordinate,     Vertex color
			//@j                                                頂点座標,                テクスチャ座標,     頂点色
			vertexBuffer = new VertexBuffer(4, indexSize, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
			
		}

		//@e Vertex color settings
		//@j 頂点色の設定。
		public void SetColor(Vector4 color)
		{
			SetColor(color.R, color.G, color.B, color.A);
		}
		
		//@e Vertex color settings
		//@j 頂点色の設定。
		public void SetColor(float r, float g, float b, float a)
		{
			this.color.R = r;
			this.color.G = g;
			this.color.B = b;
			this.color.A = a;
			
			for (int i = 0; i < colors.Length; i+=4)
			{
				colors[i] = r;
				colors[i+1] = g;
				colors[i+2] = b;
				colors[i+3] = a;
			}
		}
		
		
		/// <summary>テクスチャ座標を指定します。ピクセル単位で指定してください。</summary>
		public void SetTextureCoord(float x0, float y0, float x1, float y1)
		{
			texcoords[0] = x0 / texture.Width;	// left top u
			texcoords[1] = y0 / texture.Height; // left top v
			
			texcoords[2] = x0 / texture.Width;	// left bottom u
			texcoords[3] = y1 / texture.Height;	// left bottom v
			
			texcoords[4] = x1 / texture.Width;	// right top u
			texcoords[5] = y0 / texture.Height;	// right top v
			
			texcoords[6] = x1 / texture.Width;	// right bottom u
			texcoords[7] = y1 / texture.Height;	// right bottom v
		}
		
		/// <summary>テクスチャ座標を指定します。ピクセル単位で指定してください。</summary>
		public void SetTextureCoord(Vector2 topLeft, Vector2 bottomRight)
		{
			SetTextureCoord(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
		}
		
		/// <summary>UV(0.0f～1.0f)でテクスチャ座標を指定します。</summary>
		public void SetTextureUV(float u0, float v0, float u1, float v1)
		{
			texcoords[0] = u0;	// left top u
			texcoords[1] = v0;	// left top v
			
			texcoords[2] = u0;	// left bottom u
			texcoords[3] = v1;	// left bottom v
			
			texcoords[4] = u1;	// right top u
			texcoords[5] = v0;	// right top v
			
			texcoords[6] = u1;	// right bottom u
			texcoords[7] = v1;	// right bottom v
		}
		
		
		
		/// <summary>スプライトの描画。</summary>
		public void Render()
		{
			vertices[0]=Position.X - Width*Center.X;	// x0
			vertices[1]=Position.Y - Height*Center.Y;	// y0
			vertices[2]=Position.Z;						// z0
			
			vertices[3]=Position.X - Width*Center.X;	// x1
			vertices[4]=Position.Y + Height*(1.0f-Center.Y);// y1
			vertices[5]=Position.Z;						// z1
			
			vertices[6]=Position.X + Width*(1.0f-Center.X);	// x2
			vertices[7]=Position.Y - Height*Center.Y;		// y2
			vertices[8]=Position.Z;							// z2
			
			vertices[9]=Position.X + Width*(1.0f-Center.X);	// x3
			vertices[10]=Position.Y + Height*(1.0f-Center.Y);// y3
			vertices[11]=Position.Z;					// z3
			
			graphics.SetShaderProgram(shaderProgram);
			
			vertexBuffer.SetVertices(0, vertices);
			vertexBuffer.SetVertices(1, texcoords);
			vertexBuffer.SetVertices(2, colors);
			
			vertexBuffer.SetIndices(indices);
			graphics.SetVertexBuffer(0, vertexBuffer);
			graphics.SetTexture(0, texture);
			

#if false			
			Matrix4 screenMatrix = new Matrix4(
				 Width*2.0f/screenWidth,	0.0f,		0.0f, 0.0f,
				 0.0f,	 Height*(-2.0f)/screenHeight,	0.0f, 0.0f,
				 0.0f,	 0.0f, 1.0f, 0.0f,
				 -1.0f+(Position.X-Width * Center.X)*2.0f/screenWidth,  1.0f+(Position.Y-Height*Center.Y)*(-2.0f)/screenHeight, 0.0f, 1.0f
			);
#else
			Matrix4 screenMatrix = new Matrix4(
				 2.0f/graphics.Screen.Rectangle.Width,	0.0f,		0.0f, 0.0f,
				 0.0f,	 -2.0f/graphics.Screen.Rectangle.Height,	0.0f, 0.0f,
				 0.0f,	 0.0f, 1.0f, 0.0f,
				-1.0f, 1.0f, 0.0f, 1.0f
			);
#endif
			shaderProgram.SetUniformValue(0, ref screenMatrix);

			graphics.DrawArrays(DrawMode.TriangleStrip, 0, indexSize);
		}
		
		
		//@e Initialization of shader program
		//@j シェーダープログラムの初期化。
		private static ShaderProgram CreateSimpleSpriteShader()
		{
			string resourceName = "TutoLib.shaders.SimpleSprite.cgx";
			
			Byte[] dataBuffer = Utility.ReadEmbeddedFile(resourceName);
				
			ShaderProgram shaderProgram = new ShaderProgram(dataBuffer);
			shaderProgram.SetUniformBinding(0, "u_WorldMatrix");
	
			return shaderProgram;
		}
	}
}
