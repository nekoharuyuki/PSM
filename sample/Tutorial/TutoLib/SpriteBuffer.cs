
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
	//@e Buffer for sprite.
	//@j スプライトバッファ用スプライト。
	public class SpriteB
	{
		//@e Texture coordinate
		//@j テクスチャ座標。
		float[] texcoords = {
			0.0f, 0.0f,	// left top
			0.0f, 1.0f,	// left bottom
			1.0f, 0.0f,	// right top
			1.0f, 1.0f,	// right bottom
		};
		
		
		public Short2[] _uv = new Short2[4];
		
		
		public float[] Texcoords 
		{
			get{ return texcoords; }	
		}
		
		float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,	// left top
			1.0f,	1.0f,	1.0f,	1.0f,	// left bottom
			1.0f,	1.0f,	1.0f,	1.0f,	// right top
			1.0f,	1.0f,	1.0f,	1.0f,	// right bottom
		};

		public float[] Colors
		{
			get{ return colors; }	
		}
		
		
		//@e Property cannot describe Position.X, public variable is used.
		//@j プロパティではPosition.Xという記述ができないため、publicの変数にしています。
		
		/// <summary>スプライトの表示位置。</summary>
		public Vector3 Position ;
		
		/// <summary>
		/// スプライトの中心座標を指定。0.0f～1.0fの範囲で指定してください。
		/// スプライトの中心を指定する場合 X=0.5f, Y=0.5fとなります。
		/// </summary>
		public Vector2 Center;
		
		public Vector2 Scale=Vector2.One;
		
		public float Rotation;
		
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
		
		
		public SpriteB(UnifiedTextureInfo textureInfo)
		{
			this.SetTextureInfo(textureInfo);	
		}
		

		/*
		// GetUV( UnifiedTextureInfo textureInfo, indexX, indexY, float tileWidth, float tileHeight)
		
		public SpriteB()
		{
			
			
		}
		*/
		
		
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
		
		
		public void SetTextureInfo(UnifiedTextureInfo textureInfo)
		{
			texcoords[0] = textureInfo.u0;	// left top u
			texcoords[1] = textureInfo.v0;	// left top v
			
			texcoords[2] = textureInfo.u0;	// left bottom u
			texcoords[3] = textureInfo.v1;	// left bottom v
			
			texcoords[4] = textureInfo.u1;	// right top u
			texcoords[5] = textureInfo.v0;	// right top v
			
			texcoords[6] = textureInfo.u1;	// right bottom u
			texcoords[7] = textureInfo.v1;	// right bottom v
			
			
#if true
			_uv[0] = new Short2((short)(textureInfo.u0*32767),(short)(textureInfo.v0*32767));//(0,0)
			_uv[1] = new Short2((short)(textureInfo.u0*32767),(short)(textureInfo.v1*32767));//(1,0)
			_uv[2] = new Short2((short)(textureInfo.u1*32767),(short)(textureInfo.v0*32767));//(0,1)
			_uv[3] = new Short2((short)(textureInfo.u1*32767),(short)(textureInfo.v1*32767));//(1,1)
#else
			_uv[0] = new Short2((short)(textureInfo.u0*32767),(short)(textureInfo.v0*32767));
			_uv[1] = new Short2((short)(textureInfo.u1*32767),(short)(textureInfo.v0*32767));//(1,0)
			_uv[2] = new Short2((short)(textureInfo.u0*32767),(short)(textureInfo.v1*32767));//(0,1)
			_uv[3] = new Short2((short)(textureInfo.u1*32767),(short)(textureInfo.v1*32767));//(1,1)
#endif
			
			this.width = textureInfo.w;
			this.height = textureInfo.h;
		}
	
		static public int CompareZDepthBackToFront(SpriteB sprite01, SpriteB sprite02)
		{
			//float ff=sprite02.Position.Z - sprite01.Position.Z; 
			if( sprite02.Position.Z - sprite01.Position.Z > 0.0f)
				return 1;
			else if(sprite02.Position.Z - sprite01.Position.Z<0.0f)
				return -1;
			else
				return 0;
		}

		
	}
	
	
	public class SpriteBuffer
	{
		static ShaderProgram shaderProgram=null;
		
		int m_maxNumOfSprite;
		GraphicsContext m_graphics;
		Int32 cnt=0;
		Matrix4 screenMatrix;

		VertexBuffer vertexBuffer;
		float[] bufferVerties;
		float[] bufferTexcoords;
		float[] bufferColors;
		ushort[] bufferIndices;
		
		List<SpriteB> spriteList=new List<SpriteB>();

		const int VERTEX_NUM_PER_SPRITE = 4;
		const int VERTEX_SIZE_PER_SPRITE = VERTEX_NUM_PER_SPRITE*3;//12;
		const int TEXCOORD_SIZE_PER_SPRITE = VERTEX_NUM_PER_SPRITE*2;//8;
		const int COLOR_SIZE_PER_SPRITE = VERTEX_NUM_PER_SPRITE*4;//16;
		const int INDEX_SIZE_PER_SPRITE = 6;
		
		
		
		public SpriteBuffer (GraphicsContext graphics,int maxNumOfSprite)
		{
			this.m_graphics = graphics;
			
			if(shaderProgram == null)
			{
				shaderProgram=CreateSimpleSpriteShader();
			}
			
			screenMatrix = new Matrix4(
				 2.0f/m_graphics.Screen.Rectangle.Width,	0.0f,	0.0f, 0.0f,
				 0.0f, -2.0f/m_graphics.Screen.Rectangle.Height,	0.0f, 0.0f,
				 0.0f,	0.0f, 1.0f, 0.0f,
				-1.0f, 1.0f, 0.0f, 1.0f
			);			
			
			m_maxNumOfSprite=maxNumOfSprite;
	
			bufferVerties = new float[m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE * 3];
			bufferTexcoords = new float[m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE * 2];
			bufferColors = new float[m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE * 4];
			bufferIndices = new ushort[m_maxNumOfSprite * INDEX_SIZE_PER_SPRITE];
			
			//
			vertexBuffer = new VertexBuffer(
				m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE, 
				m_maxNumOfSprite * INDEX_SIZE_PER_SPRITE, 
				VertexFormat.Float3, 
				VertexFormat.Float2, 
				VertexFormat.Float4);
			
		}
		
		public void Clear()
		{
			spriteList.Clear();
			cnt=0;
		}
		
		public int GetNumOfSprite()
		{
			return cnt;	
		}
		
		public void Copy2Buffer(SpriteB spriteB, int index)
		{
			if(spriteB.Rotation == 0.0f)
			{
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE]=spriteB.Position.X - spriteB.Width*spriteB.Center.X;//0.0f;	// x0
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+1]=spriteB.Position.Y - spriteB.Height*spriteB.Center.Y;//0.0f;	// y0
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+2]=spriteB.Position.Z;	// z0
				
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+3]=spriteB.Position.X - spriteB.Width*spriteB.Center.X;//0.0f;	// x1
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+4]=spriteB.Position.Y + spriteB.Height*(1.0f-spriteB.Center.Y);//1.0f;	// y1
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+5]=spriteB.Position.Z;	// z1
				
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+6]=spriteB.Position.X + spriteB.Width*(1.0f-spriteB.Center.X);//1.0f;	// x2
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+7]=spriteB.Position.Y - spriteB.Height*spriteB.Center.Y;//0.0f;	// y2
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+8]=spriteB.Position.Z;	// z2
				
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+9]=spriteB.Position.X + spriteB.Width*(1.0f-spriteB.Center.X);//1.0f;	// x3
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+10]=spriteB.Position.Y + spriteB.Height*(1.0f-spriteB.Center.Y);//1.0f;	// y3
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+11]=spriteB.Position.Z;	// z3
			}
			else
			{
				float x,y, rc, rs;

				rc=FMath.Cos(spriteB.Rotation);
				rs=FMath.Sin(spriteB.Rotation);
				
				x=-spriteB.Width*spriteB.Center.X;
				y=-spriteB.Height*spriteB.Center.Y;
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE]=spriteB.Position.X + (float)(x*rc - y*rs);// x0
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+1]=spriteB.Position.Y + (float)(x*rs + y*rc);// y0
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+2]=spriteB.Position.Z;	// z0
	
				x=-spriteB.Width*spriteB.Center.X;
				y=spriteB.Height*(1.0f-spriteB.Center.Y);
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+3]=spriteB.Position.X + (float)(x*rc - y*rs);// x1
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+4]=spriteB.Position.Y + (float)(x*rs + y*rc);// y1
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+5]=spriteB.Position.Z;	// z1
	
				x=spriteB.Width*(1.0f-spriteB.Center.X);
				y=-spriteB.Height*spriteB.Center.Y;
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+6]=spriteB.Position.X + (float)(x*rc-y*rs);// x2
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+7]=spriteB.Position.Y + (float)(x*rs+y*rc);// y2
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+8]=spriteB.Position.Z;	// z2
	
				x=spriteB.Width*(1.0f-spriteB.Center.X);
				y=spriteB.Height*(1.0f-spriteB.Center.Y);
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+9]=spriteB.Position.X + (float)(x*rc-y*rs);// x3
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+10]=spriteB.Position.Y + (float)(x*rs+y*rc);// y3
				bufferVerties[index*VERTEX_SIZE_PER_SPRITE+11]=spriteB.Position.Z;	// z3
			}
			
#if false
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE]=texcoords[0];		// left top u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+1]=texcoords[1]; 	// left top v
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+2]=texcoords[2];	// left bottom u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+3]=texcoords[3];	// left bottom v
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+4]=texcoords[4];	// right top u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+5]=texcoords[5];	// right top v
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+6]=texcoords[6];	// right bottom u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+7]=texcoords[7];	// right bottom v
#else
			Buffer.BlockCopy(spriteB.Texcoords, 0, bufferTexcoords, index*TEXCOORD_SIZE_PER_SPRITE*sizeof(float), 8*sizeof(float));
#endif
			
			// color
			
#if false			
			bufferColors[index*COLOR_SIZE_PER_SPRITE]=colors[0];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+1]=colors[1];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+2]=colors[2];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+3]=colors[3];
			
			bufferColors[index*COLOR_SIZE_PER_SPRITE+4]=colors[4];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+5]=colors[5];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+6]=colors[6];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+7]=colors[7];
			
			bufferColors[index*COLOR_SIZE_PER_SPRITE+8]=colors[8];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+9]=colors[9];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+10]=colors[10];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+11]=colors[11];
			
			bufferColors[index*COLOR_SIZE_PER_SPRITE+12]=colors[12];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+13]=colors[13];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+14]=colors[14];
			bufferColors[index*COLOR_SIZE_PER_SPRITE+15]=colors[15];
#else
			Buffer.BlockCopy(spriteB.Colors, 0, bufferColors, index*COLOR_SIZE_PER_SPRITE*sizeof(float), 16*sizeof(float));
#endif

			bufferIndices[index*INDEX_SIZE_PER_SPRITE] = (ushort)(index*VERTEX_NUM_PER_SPRITE);
			bufferIndices[index*INDEX_SIZE_PER_SPRITE + 1] = (ushort)(index*VERTEX_NUM_PER_SPRITE+1);
			bufferIndices[index*INDEX_SIZE_PER_SPRITE + 2] = (ushort)(index*VERTEX_NUM_PER_SPRITE+2);
			bufferIndices[index*INDEX_SIZE_PER_SPRITE + 3] = (ushort)(index*VERTEX_NUM_PER_SPRITE+1);
			bufferIndices[index*INDEX_SIZE_PER_SPRITE + 4] = (ushort)(index*VERTEX_NUM_PER_SPRITE+2);
			bufferIndices[index*INDEX_SIZE_PER_SPRITE + 5] = (ushort)(index*VERTEX_NUM_PER_SPRITE+3);
		}
		
		public void Add(SpriteB spriteB)
		{
			if(cnt >=  m_maxNumOfSprite)
				return;
			
			this.spriteList.Add(spriteB);
			++cnt;
		}
		
		public enum SortMode
		{
			BackToFront,
			FrontToBack,
			NoSort,
		}
		
		public SortMode sortMode;
		
		
		public void Sort()
		{
			if(this.sortMode == SortMode.BackToFront)
			{
				spriteList.Sort(
					delegate(SpriteB sprite01, SpriteB sprite02)
				    {
						float ff=sprite02.Position.Z - sprite01.Position.Z; 
						if( ff > 0.0f)
							return 1;
						else if(ff<0.0f)
							return -1;
						else
							return 0;
					});
			}
			else if(this.sortMode == SortMode.FrontToBack)
			{
				spriteList.Sort(
					delegate(SpriteB sprite01, SpriteB sprite02)
				    {
						float ff=sprite02.Position.Z - sprite01.Position.Z; 
						if( ff > 0.0f)
							return -1;
						else if(ff<0.0f)
							return 1;
						else
							return 0;
					});
			}
		}
		
		public bool IsFull()
		{
			if(cnt>=m_maxNumOfSprite)
				return true;
			else 
				return false;
		}
		
		
		public void RenderCPU()
		{
			//@e Copy sprite list to buffer.
			//@j ここでlistのスプライトをバッファにコピーする。
			for(int i=0; i< spriteList.Count; ++i)
			{
				Copy2Buffer(spriteList[i], i);
			}
		}
		
		
		public void Render()
		{
			Sort();
			RenderCPU();
			
			m_graphics.SetShaderProgram(shaderProgram);
			
			vertexBuffer.SetVertices(0, bufferVerties);
			vertexBuffer.SetVertices(1, bufferTexcoords);
			vertexBuffer.SetVertices(2, bufferColors);
			
			vertexBuffer.SetIndices(bufferIndices);
			m_graphics.SetVertexBuffer(0, vertexBuffer);
						
			shaderProgram.SetUniformValue(0, ref screenMatrix);
			
			//Draw Call
			m_graphics.DrawArrays(DrawMode.Triangles, 0, INDEX_SIZE_PER_SPRITE*cnt);
			
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

