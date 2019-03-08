
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
	public class SpriteBuffer2
	{
		static ShaderProgram shaderProgram=null;
		
		int m_maxNumOfSprite;
		GraphicsContext m_graphics;
		Int32 cnt=0;
		Matrix4 screenMatrix;
		Vector2 screenScale;

		VertexBuffer vertexBuffer;
		float[] bufferVerties;	//0
		float[] bufferTexcoords;//1
		float[] bufferColors;	//2
		float[] bufferWH;		//3
		float[] bufferCenter;	//4
		float[] bufferRot;		//5
		float[] bufferSpritePosition;//6
		ushort[] bufferIndices;
		
		List<SpriteB> spriteList=new List<SpriteB>();
		
		const int VERTEX_NUM_PER_SPRITE = 4;

		const int FNUM_VERTEX_PER_SPRITE = VERTEX_NUM_PER_SPRITE*3;	//x12 float;
		const int FNUM_TEXCOORD_PER_SPRITE = VERTEX_NUM_PER_SPRITE*2;//x8 float;
		const int FNUM_COLOR_PER_SPRITE = VERTEX_NUM_PER_SPRITE*4;	//x16 float;
		
		const int FNUM_WH_PER_SPRITE = VERTEX_NUM_PER_SPRITE*2;		//x8 float;
		const int FNUM_CENTER_PER_SPRITE = VERTEX_NUM_PER_SPRITE*2;	//x8 float;
		const int FNUM_ROT_PER_SPRITE = VERTEX_NUM_PER_SPRITE*2;	//x8 float;
		const int FNUM_SPOSITION_PER_SPRITE = VERTEX_NUM_PER_SPRITE*3;	//x12 float;
		
		const int INDEX_SIZE_PER_SPRITE = 6;
		
		ImageRect rectAppScreen;
		
		public SpriteBuffer2 (GraphicsContext graphics,int maxNumOfSprite)
			:this (graphics, maxNumOfSprite, Vector2.One, new ImageRect(0,0, graphics.Screen.Width, graphics.Screen.Height)){}
		
		//TODO:GraphicsContextにAppScreenを保持できればよいが。
		public SpriteBuffer2 (GraphicsContext graphics,int maxNumOfSprite, Vector2 screenScale, ImageRect rectAppScreen)
		{
			this.m_graphics = graphics;
			this.screenScale=screenScale;
			this.rectAppScreen=rectAppScreen;
			
			if(shaderProgram == null)
			{
				shaderProgram=CreateSpriteBShader();
			}
			
			m_maxNumOfSprite=maxNumOfSprite;
			
#if true
			bufferVerties = 	new float[m_maxNumOfSprite * FNUM_VERTEX_PER_SPRITE];	//0
			bufferTexcoords = 	new float[m_maxNumOfSprite * FNUM_TEXCOORD_PER_SPRITE];	//1
			bufferColors = 		new float[m_maxNumOfSprite * FNUM_COLOR_PER_SPRITE];	//2

			bufferWH = 			new float[m_maxNumOfSprite * FNUM_WH_PER_SPRITE];		//3
			bufferCenter = 		new float[m_maxNumOfSprite * FNUM_CENTER_PER_SPRITE];	//4
			bufferRot = 		new float[m_maxNumOfSprite * FNUM_ROT_PER_SPRITE];		//5
			bufferSpritePosition=new float[m_maxNumOfSprite * FNUM_SPOSITION_PER_SPRITE];//6
			
			bufferIndices = new ushort[m_maxNumOfSprite * INDEX_SIZE_PER_SPRITE];
			
			//
			vertexBuffer = new VertexBuffer(
				m_maxNumOfSprite * VERTEX_NUM_PER_SPRITE,
				m_maxNumOfSprite * INDEX_SIZE_PER_SPRITE,
				VertexFormat.Float3,//0 
				VertexFormat.Float2,//1
				VertexFormat.Float4,//2
				VertexFormat.Float2,//3
				VertexFormat.Float2,//4
				VertexFormat.Float2,//5
				VertexFormat.Float3//6
				);
#else
			//正しい。
			bufferVerties = new float[m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE * 3];	//0
			bufferTexcoords = new float[m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE * 2];	//1
			bufferColors = new float[m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE * 4];	//2
			bufferIndices = new ushort[m_maxNumOfSprite * INDEX_SIZE_PER_SPRITE];
			
			//
			vertexBuffer = new VertexBuffer(
				m_maxNumOfSprite * VERTEX_SIZE_PER_SPRITE, 
				m_maxNumOfSprite * INDEX_SIZE_PER_SPRITE, 
				VertexFormat.Float3, 
				VertexFormat.Float2, 
				VertexFormat.Float4);
#endif
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
		
		public void SetScreenScale(Vector2 screenScale)
		{
			this.screenScale=screenScale;
		}
		
		
		public void Copy2Buffer(SpriteB spriteB, int index)
		{
			// 頂点座標の計算。

			bufferVerties[index*FNUM_VERTEX_PER_SPRITE]=0;		//0.0f;	// x0
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+1]=0;	//0.0f;	// y0
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+2]=0;	// z0
			
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+3]=0;	//0.0f;	// x1
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+4]=spriteB.Height;//1.0f;	// y1
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+5]=0;	// z1
			
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+6]=spriteB.Width;//1.0f;	// x2
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+7]=0;	//0.0f;	// y2
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+8]=0;	// z2
			
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+9]=spriteB.Width;//1.0f;	// x3
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+10]=spriteB.Height;//1.0f;	// y3
			bufferVerties[index*FNUM_VERTEX_PER_SPRITE+11]=0;	// z3
			
			bufferWH[index*FNUM_WH_PER_SPRITE]=spriteB.Width;
			bufferWH[index*FNUM_WH_PER_SPRITE+1]=spriteB.Height;
			bufferWH[index*FNUM_WH_PER_SPRITE+2]=spriteB.Width;
			bufferWH[index*FNUM_WH_PER_SPRITE+3]=spriteB.Height;
			bufferWH[index*FNUM_WH_PER_SPRITE+4]=spriteB.Width;
			bufferWH[index*FNUM_WH_PER_SPRITE+5]=spriteB.Height;
			bufferWH[index*FNUM_WH_PER_SPRITE+6]=spriteB.Width;
			bufferWH[index*FNUM_WH_PER_SPRITE+7]=spriteB.Height;
			
			bufferCenter[index*FNUM_CENTER_PER_SPRITE]=spriteB.Center.X;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+1]=spriteB.Center.Y;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+2]=spriteB.Center.X;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+3]=spriteB.Center.Y;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+4]=spriteB.Center.X;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+5]=spriteB.Center.Y;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+6]=spriteB.Center.X;
			bufferCenter[index*FNUM_CENTER_PER_SPRITE+7]=spriteB.Center.Y;
			
			bufferRot[index*FNUM_ROT_PER_SPRITE]=spriteB.Rotation;
			bufferRot[index*FNUM_ROT_PER_SPRITE+1]=0.0f;
			bufferRot[index*FNUM_ROT_PER_SPRITE+2]=spriteB.Rotation;
			bufferRot[index*FNUM_ROT_PER_SPRITE+3]=0.0f;
			bufferRot[index*FNUM_ROT_PER_SPRITE+4]=spriteB.Rotation;
			bufferRot[index*FNUM_ROT_PER_SPRITE+5]=0.0f;
			bufferRot[index*FNUM_ROT_PER_SPRITE+6]=spriteB.Rotation;
			bufferRot[index*FNUM_ROT_PER_SPRITE+7]=0.0f;
			
			
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE]=spriteB.Position.X;	//x
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+1]=spriteB.Position.Y;	//y
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+2]=spriteB.Position.Z;	//z
				
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+3]=spriteB.Position.X;	//x
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+4]=spriteB.Position.Y;	//y
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+5]=spriteB.Position.Z;	//z

			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+6]=spriteB.Position.X;	//x
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+7]=spriteB.Position.Y;	//y
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+8]=spriteB.Position.Z;	//z

			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+9]=spriteB.Position.X;	//x
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+10]=spriteB.Position.Y;//y
			bufferSpritePosition[index*FNUM_SPOSITION_PER_SPRITE+11]=spriteB.Position.Z;//z
			
			// テクスチャ座標のコピー。
#if false
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE]=texcoords[0];	// left top u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+1]=texcoords[1]; // left top v
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+2]=texcoords[2];	// left bottom u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+3]=texcoords[3];	// left bottom v
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+4]=texcoords[4];	// right top u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+5]=texcoords[5];	// right top v
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+6]=texcoords[6];	// right bottom u
			bufferTexcoords[index*TEXCOORD_SIZE_PER_SPRITE+7]=texcoords[7];	// right bottom v
#else
			Buffer.BlockCopy(spriteB.Texcoords, 0, bufferTexcoords, index*FNUM_TEXCOORD_PER_SPRITE*sizeof(float), 8*sizeof(float));
#endif
			
			// colorのコピー。
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
			Buffer.BlockCopy(spriteB.Colors, 0, bufferColors, index*FNUM_COLOR_PER_SPRITE*sizeof(float), 16*sizeof(float));
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

#if true
				spriteList.Sort(
					delegate(SpriteB spriteA, SpriteB spriteB)
				    {
						if( spriteB.Position.Z - spriteA.Position.Z > 0.0f)
							return 1;
						else if(spriteB.Position.Z - spriteA.Position.Z<0.0f)
							return -1;
						else
							return 0;

					});
#else
				spriteList.Sort(SpriteB.CompareZDepthBackToFront);
#endif
			}
			else if(this.sortMode == SortMode.FrontToBack)
			{
				spriteList.Sort(
					delegate(SpriteB spriteA, SpriteB spriteB)
				    {
						if( spriteB.Position.Z - spriteA.Position.Z > 0.0f)
							return -1;
						else if(spriteB.Position.Z - spriteA.Position.Z < 0.0f)
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
			//ここでlistのスプライトの拡大・回転の計算をし、バッファにコピーする。
			for(int i=0; i< spriteList.Count; ++i)
			{
				Copy2Buffer(spriteList[i], i);
			}
		}
		
		
		public void Render()
		{
			//TODO:実験中。位置を移動。
			//RenderCPU();
			
			m_graphics.SetShaderProgram(shaderProgram);
			
			vertexBuffer.SetVertices(0, bufferVerties);
			vertexBuffer.SetVertices(1, bufferTexcoords);
			vertexBuffer.SetVertices(2, bufferColors);
			vertexBuffer.SetVertices(3, bufferWH);
			vertexBuffer.SetVertices(4, bufferCenter);
			vertexBuffer.SetVertices(5, bufferRot);
			vertexBuffer.SetVertices(6, bufferSpritePosition);
			
			vertexBuffer.SetIndices(bufferIndices);
			m_graphics.SetVertexBuffer(0, vertexBuffer);
			
			//TODO:newしないようにする。
			screenMatrix = new Matrix4(
				 2.0f/this.rectAppScreen.Width*screenScale.X,	0.0f,	0.0f, 0.0f,
				 0.0f, -2.0f/this.rectAppScreen.Height*screenScale.Y,	0.0f, 0.0f,
				 0.0f,	0.0f, 1.0f, 0.0f,
				-1.0f+(1.0f-screenScale.X), 1.0f-(1.0f-screenScale.Y), 0.0f, 1.0f
			);
			
			shaderProgram.SetUniformValue(0, ref screenMatrix);
			
			//ここでDraw Callをおこなう。
			m_graphics.DrawArrays(DrawMode.Triangles, 0, INDEX_SIZE_PER_SPRITE*cnt);
			
		}
		
		//@j シェーダープログラムの初期化。
		//@e Initialization of shader program
		private static ShaderProgram CreateSpriteBShader()
		{
			string resourceName = "TutoLib.shaders.SpriteB.cgx";
			
			Byte[] dataBuffer = Utility.ReadEmbeddedFile(resourceName);
			
			ShaderProgram shaderProgram = new ShaderProgram(dataBuffer);
			shaderProgram.SetUniformBinding(0, "u_WorldMatrix");
	
			return shaderProgram;
		}
		
	}
}

