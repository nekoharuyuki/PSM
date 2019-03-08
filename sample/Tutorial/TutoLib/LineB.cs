
using System;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using System.IO;
using System.Reflection;
using Tutorial.Utility;


namespace Sce.PlayStation.Framework
{

	public class LineB
	{
		static ShaderProgram shaderProgram=null;
		static Matrix4 screenMatrix;
		
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


		public LineB(GraphicsContext graphics)
		{
			if(shaderProgram == null)
			{
				shaderProgram=CreateLineShader();
			}

			//if( screenMatrix == null)
			{
				 screenMatrix= new Matrix4(
					 2.0f/graphics.Screen.Rectangle.Width,	0.0f,	0.0f, 0.0f,
					 0.0f, -2.0f/graphics.Screen.Rectangle.Height,	0.0f, 0.0f,
					 0.0f,	0.0f, 1.0f, 0.0f,
					-1.0f, 1.0f, 0.0f, 1.0f
				);
			}
			
			Color0=Color1=Vector4.One;
			
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
			shaderProgram.SetUniformValue(0, ref screenMatrix);
			
			vertexBuffer.SetVertices(0, positions);
			vertexBuffer.SetVertices(1, colors);

			graphics.SetVertexBuffer(0, vertexBuffer);

			graphics.DrawArrays(DrawMode.Lines, 0, 2);
			
		}
		
		//@e Initialization of shader program
		//@j シェーダープログラムの初期化。
		public static ShaderProgram CreateLineShader()
		{
			Byte[] dataBuffer=Utility.ReadEmbeddedFile("TutoLib.shaders.Line.cgx");
			ShaderProgram shaderProgram = new ShaderProgram(dataBuffer);
			shaderProgram.SetUniformBinding(0, "u_WorldMatrix");
	
			return shaderProgram;
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
				shaderProgram=LineB.CreateLineShader();
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

			//TODO:rectAppScreenを指定する。
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
		
		public void Add(LineB line)
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

