
using System;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using System.IO;
using System.Reflection;
using Tutorial.Utility;


namespace Sce.PlayStation.Framework
{

	public class Line
	{
		static ShaderProgram shaderProgram=null;
		static Matrix4 unitScreenMatrix;
		
		protected GraphicsContext graphics;
		
		VertexBuffer vertexBuffer;
		
		public Vector3 Position0,Position1;
		public Vector4 Color0, Color1;
		
		
		float[] positions = {
			0.0f,	0.0f,	0.0f,
			1.0f,	1.0f,	1.0f,
		};

		float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,
			1.0f,	1.0f,	1.0f,	1.0f,
		};


		public Line(GraphicsContext graphics)
		{
			if(shaderProgram == null)
			{
				shaderProgram=CreateLineShader();
			}

			//if( unitScreenMatrix == null)
			{
				 unitScreenMatrix= new Matrix4(
					 2.0f/graphics.Screen.Rectangle.Width,	0.0f,	0.0f, 0.0f,
					 0.0f, -2.0f/graphics.Screen.Rectangle.Height,	0.0f, 0.0f,
					 0.0f,	0.0f, 1.0f, 0.0f,
					-1.0f, 1.0f, 0.0f, 1.0f
				);
			}
			
			this.graphics = graphics;
			vertexBuffer = new VertexBuffer(2, VertexFormat.Float3, VertexFormat.Float4);
		}

		public void SetPosition(Vector3 pos0, Vector3 pos1)
		{
			this.Position0=pos0;
			this.Position1=pos1;
		}

		public void SetPosition0(Vector3 pos0)
		{
			this.Position0=pos0;
		}

		public void SetPosition1(Vector3 pos1)
		{
			this.Position1=pos1;
		}


		public void SetColor(Vector4 color0, Vector4 color1)
		{
			this.Color0=color0;
			this.Color1=color1;
		}

		public void SetColor(Vector4 color0)
		{
			this.Color0=color0;
			this.Color1=color0;
		}

		/// <summary>描画処理。</summary>
		public void Render()
		{
			positions[0] = Position0.X;
			positions[1] = Position0.Y;
			positions[2] = Position0.Z;

			positions[3] = Position1.X;
			positions[4] = Position1.Y;
			positions[5] = Position1.Z;
			

			colors[0] = Color0.R;
			colors[1] = Color0.G;
			colors[2] = Color0.B;
			colors[3] = Color0.A;

			colors[4] = Color1.R;
			colors[5] = Color1.G;
			colors[6] = Color1.B;
			colors[7] = Color1.A;
			
			graphics.SetShaderProgram(shaderProgram);
			shaderProgram.SetUniformValue(0, ref unitScreenMatrix);
			
			vertexBuffer.SetVertices(0, positions);
			vertexBuffer.SetVertices(1, colors);

			graphics.SetVertexBuffer(0, vertexBuffer);

			graphics.DrawArrays(DrawMode.Lines, 0, 2);
			
		}
		
		//@j シェーダープログラムの初期化。
		//@e Initialization of shader program
		public static ShaderProgram CreateLineShader()
		{
			Byte[] dataBuffer=Utility.ReadEmbeddedFile("TutoLib.shaders.Line.cgx");
			ShaderProgram shaderProgram = new ShaderProgram(dataBuffer);
			shaderProgram.SetUniformBinding(0, "u_WorldMatrix");
	
			return shaderProgram;
		}
	}
	
	
	/// <summary>
	/// DrawArraysの呼び出しを抑える実験版。
	/// </summary>
	public class Line2
	{
		const int NumOfVertexPerLine = 2;	// ライン一本分の頂点数。
		const int PositionSize = 3;	// VertexFormat.Float3
		const int ColorSize = 4;	// VertexFormat.Float4

		static GraphicsContext m_Graphics;
		static VertexBuffer m_Vertices;
		
		static int NumOfLines;

		static bool[] isAlive;
		static bool[] disp;
	
		static float[] positions;
		static float[] colors;
		static ushort[] indices;

		int id;

		public static void Initialize(int numOfLines)
		{
			NumOfLines = numOfLines;
			m_Vertices = new VertexBuffer(NumOfLines * NumOfVertexPerLine, NumOfLines * NumOfVertexPerLine, VertexFormat.Float3, VertexFormat.Float4);
			positions = new float[NumOfLines * NumOfVertexPerLine * PositionSize];
			colors = new float[NumOfLines * NumOfVertexPerLine * ColorSize];
			indices = new ushort[NumOfLines * NumOfVertexPerLine];
			disp = new bool[NumOfLines];
			isAlive = new bool[NumOfLines];
		}
		

		public Line2(GraphicsContext graphics)
		{
			if (m_Vertices == null)
			{
				throw new Exception("Line is not initialized.");
			}

			m_Graphics = graphics;

			bool bFound = false;

			for (int i = 0; i < NumOfLines; ++i)
			{
				if (isAlive[i] == false)
				{
					id = i;
					isAlive[id] = true;
					disp[id] = false;
					bFound = true;
					break;
				}
			}

			if (bFound == false)
			{
				throw new Exception("Line buffer is full.");
			}

			SetColor(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

		}
		
		/*
		~Line2()
		{
			Console.WriteLine("~Line() id="+id);
			isAlive[id] = false;
		}
		*/

		public void SetPosition(Vector3 pos0, Vector3 pos1)
		{
			int offset = id * PositionSize * NumOfVertexPerLine;
			positions[offset]     = pos0.X;
			positions[offset + 1] = pos0.Y;
			positions[offset + 2] = pos0.Z;

			positions[offset + 3] = pos1.X;
			positions[offset + 4] = pos1.Y;
			positions[offset + 5] = pos1.Z;
		}


		public void SetColor(Vector4 color0)
		{
			int offset = id * ColorSize * NumOfVertexPerLine;
			colors[offset]	   = color0.R;
			colors[offset + 1] = color0.G;
			colors[offset + 2] = color0.B;
			colors[offset + 3] = color0.A;

			colors[offset + 4] = color0.R;
			colors[offset + 5] = color0.G;
			colors[offset + 6] = color0.B;
			colors[offset + 7] = color0.A;
		}

		/// <summary>
		/// 描画処理。
		/// </summary>
		public void Draw()
		{
			disp[id] = true;
		}

		static public void DrawAll()
		{
			if (m_Vertices == null)
			{
				throw new Exception("ERROR!!: Initialize before you use this method.");
			}


			int cnt = 0; int numOfAlive=0;

			// 表示フラグがオンになっているもののみインデックスにいれる。
			for (int i = 0; i < NumOfLines; ++i)
			{
				if (isAlive[i] == true)
				{
					numOfAlive++;
					if (disp[i] == true)
					{
						indices[cnt] = (ushort)(i * NumOfVertexPerLine);
						cnt++;
						indices[cnt] = (ushort)(i * NumOfVertexPerLine + 1);
						cnt++;
					}
				}
			}

			//GameSystem.debugFont.AddString("drawIndex=" + cnt + ",numOfAlive="+numOfAlive+"\n");

			m_Vertices.SetVertices(0, positions);
			m_Vertices.SetVertices(1, colors);
			m_Vertices.SetIndices(indices);

			m_Graphics.SetVertexBuffer(0, m_Vertices);
			m_Graphics.DrawArrays(DrawMode.Lines, 0, cnt);

			// 表示フラグをオフにする。
			for (int i = 0; i < NumOfLines; ++i)
			{
				disp[i] = false;
			}
		}
	}
	
	
	public class LineBuffer
	{
		static ShaderProgram shaderProgram=null;
		Matrix4 unitScreenMatrix;

		GraphicsContext graphics;
		
		const int NUM_OF_VERTEX_PER_LINE = 2;
		const int VERTEX_SIZE_PER_LINE = NUM_OF_VERTEX_PER_LINE*3;
		const int COLOR_SIZE_PER_LINE = NUM_OF_VERTEX_PER_LINE*4;
		
		Int32 maxNumOfLines;
		
		float[] bufferVerties;
		float[] bufferColors;
		
		VertexBuffer vertexBuffer;
		
		Int32 cnt=0;
		
		public LineBuffer(GraphicsContext graphics, Int32 maxNumOfLines)
		{
			if(shaderProgram == null)
			{
				shaderProgram=Line.CreateLineShader();
			}
			
			this.graphics=graphics;
			
			this.maxNumOfLines=	maxNumOfLines;
			
			bufferVerties = new float[maxNumOfLines * VERTEX_SIZE_PER_LINE];
			bufferColors = new float[maxNumOfLines * COLOR_SIZE_PER_LINE];
			
			//
			vertexBuffer = new VertexBuffer(
				maxNumOfLines*NUM_OF_VERTEX_PER_LINE, 
				VertexFormat.Float3, 
				VertexFormat.Float4);


			unitScreenMatrix= new Matrix4(
				 2.0f/graphics.Screen.Rectangle.Width,	0.0f,	0.0f, 0.0f,
				 0.0f, -2.0f/graphics.Screen.Rectangle.Height,	0.0f, 0.0f,
				 0.0f,	0.0f, 1.0f, 0.0f,
				-1.0f, 1.0f, 0.0f, 1.0f
			);
		}
		
		
		public void Clear()
		{
			cnt=0;	
		}
		
		public void Set(Line line)
		{
			if( cnt >= maxNumOfLines)
				return;

			bufferVerties[cnt*VERTEX_SIZE_PER_LINE]=line.Position0.X;
			bufferVerties[cnt*VERTEX_SIZE_PER_LINE+1]=line.Position0.Y;
			bufferVerties[cnt*VERTEX_SIZE_PER_LINE+2]=line.Position0.Z;
			
			bufferVerties[cnt*VERTEX_SIZE_PER_LINE+3]=line.Position1.X;
			bufferVerties[cnt*VERTEX_SIZE_PER_LINE+4]=line.Position1.Y;
			bufferVerties[cnt*VERTEX_SIZE_PER_LINE+5]=line.Position1.Z;
			
			bufferColors[cnt*COLOR_SIZE_PER_LINE]=line.Color0.R;
			bufferColors[cnt*COLOR_SIZE_PER_LINE+1]=line.Color0.G;
			bufferColors[cnt*COLOR_SIZE_PER_LINE+2]=line.Color0.B;
			bufferColors[cnt*COLOR_SIZE_PER_LINE+3]=line.Color0.A;
			
			bufferColors[cnt*COLOR_SIZE_PER_LINE+4]=line.Color1.R;
			bufferColors[cnt*COLOR_SIZE_PER_LINE+5]=line.Color1.G;
			bufferColors[cnt*COLOR_SIZE_PER_LINE+6]=line.Color1.B;
			bufferColors[cnt*COLOR_SIZE_PER_LINE+7]=line.Color1.A;
			
			++cnt;
		}
		
		public void Render()
		{
			graphics.SetShaderProgram(shaderProgram);
			shaderProgram.SetUniformValue(0, ref unitScreenMatrix);
			
			vertexBuffer.SetVertices(0, bufferVerties);
			vertexBuffer.SetVertices(1, bufferColors);

			graphics.SetVertexBuffer(0, vertexBuffer);
			graphics.DrawArrays(DrawMode.Lines, 0, NUM_OF_VERTEX_PER_LINE*cnt);
		}
		
		public int GetNumOfLine()
		{
			return cnt;	
		}
		
		
	}
	
}
