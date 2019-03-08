/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using System.IO;
using System.Reflection;

using Tutorial.Utility;


namespace Tutorial.Utility
{
	/// <summary>デバッグ文字列。アスキー文字のみ利用できます。それ以外の文字は'?'が表示されます。</summary>
	public class DebugString  : IDisposable
	{
		static ShaderProgram shaderProgram;
		
		//@e Maximum number of characters
		//@j 最大文字数。
		int maxNumOfCharactor=256;
		

		StringBuilder stringBuilder = new StringBuilder(1024);
		
		
		Texture2D texture = null;
		int charaWidth = 8;
		int charaHeight = 8;
		
		/// <summary>表示位置。</summary>
		public Vector3 PixelScreenPosition;

		
		/// <summary>文字の色。インスタンスごとに設定できます。</summary>
		Vector4 color=new Vector4(1.0f,1.0f,1.0f,1.0f);
		
		
		//@e Line to convert a pixel coordinate system (of original point at upper left) into a screen coordinate system.
		//@j ピクセルの座標系(左上が原点)をスクリーンの座標系に変換する行列。
		Matrix4 unitScreenMatrix;
		
		GraphicsContext graphics;
		VertexBuffer vertices;
		
		const int indexSize = 6;
		float[] charaPositions;
		float[] charaTexcoords;
		float[] charaColors;
		ushort[] charaIndices;
		
		
		/// <summary>アセンブリに埋め込まれた画像ファイルを読み込み、テクスチャを生成します。</summary>
		static public Texture2D CreateTextureFromEmbeddedFile(string resourceName)
		{
			Byte[] dataBuffer = Utility.ReadEmbeddedFile(resourceName);
			
			return new Texture2D(dataBuffer, false);
		}
		
		
		/// <summary>コンストラクタ。文字のテクスチャは、デフォルトのものを使います。</summary>
		public DebugString(GraphicsContext graphics) : this(graphics, null, 10, 20){}
			
		
		public DebugString(GraphicsContext graphics, Texture2D texture, int charaWidth, int charaHeight)
		{
			if(shaderProgram == null)
			{
				shaderProgram=CreateSimpleSpriteShader();
			}
			
			this.graphics = graphics;
			
			if( texture != null)
				this.texture = texture;
			else 
				this.texture = CreateTextureFromEmbeddedFile("TutoLib.resources.DebugFont.png");
			
			this.charaWidth = charaWidth;
			this.charaHeight = charaHeight;
			
			
			charaPositions = new float[maxNumOfCharactor * 4 * 3];
			charaTexcoords = new float[maxNumOfCharactor * 4 * 2];
			charaColors = new float[maxNumOfCharactor * 4 * 4];
			charaIndices = new ushort[maxNumOfCharactor * indexSize];

			vertices = new VertexBuffer(maxNumOfCharactor * 4, maxNumOfCharactor * indexSize, 
				VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
		
			PixelScreenPosition = new Vector3(0.0f, 0.0f, 0.0f);
		
			ImageRect rectPixelScreen = graphics.GetViewport();
			
			//@e Matrix to convert a pixel coordinate system into a screen coordinate system.
			//@j ピクセルの座標系をスクリーンの座標系に変換する行列。
			unitScreenMatrix = new Matrix4(
				 2.0f/rectPixelScreen.Width,	0.0f,	0.0f, 0.0f,
				 0.0f, -2.0f/rectPixelScreen.Height,	0.0f, 0.0f,
				 0.0f,	0.0f, 1.0f, 0.0f,
				-1.0f, 1.0f, 0.0f, 1.0f
			);
		}
		
		public void Dispose()
		{
			shaderProgram.Dispose();
			vertices.Dispose();
		}
		
		
		/// <summary>
		/// 左上を原点とする座標系で位置を指定する。
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z">深度値。0.0f～1.0fで指定してください。</param>
		public void SetPosition(float x, float y, float z)
		{
			PixelScreenPosition.X = x;
			PixelScreenPosition.Y = y;
			PixelScreenPosition.Z = z;
		}
		
		
		public void SetPosition(Vector3 position)
		{
			PixelScreenPosition = position;
		}
		
		
		/// <summary>
		/// デバッグフォントの色の設定。
		/// </summary>
		/// <param name="color">デバッグフォントの色。</param>
 		public void SetColor(Vector4 color)
		{
			this.color = color;
		}
		
		
		public void Clear()
		{
			stringBuilder.Clear();
		}

		public void Write(string text)
		{
			stringBuilder.Append(text);
		}
		
		public void WriteLine(string text)
		{
			stringBuilder.Append(text);
			stringBuilder.Append("\n");
		}
		
		
		/// <summary>デバッグフォントの描画。</summary>
		public void Render()
		{
			graphics.Disable(EnableMode.DepthTest);
			
			graphics.SetTexture(0, texture);	
			
			int width = charaWidth;
			int height = charaHeight;
			
			
			//@e Texture coordinate settings
			//@j テクスチャ座標の設定。
			float charaTexWidth = (float)width / texture.Width;
			float charaTexHeight = (float)height / texture.Height;
			
			
			float offsetX = PixelScreenPosition.X;
			float offsetY = PixelScreenPosition.Y;
			int count = 0;
			
			//@e Reduce stringBuilder.Length to the maximum number of characters.
			//@j stringBuilder.Lengthを最大文字数に切り詰める。
			if(stringBuilder.Length >= maxNumOfCharactor)
				stringBuilder.Length = maxNumOfCharactor-1;
			
			//@e Vertex coordinate settings
			//@j 頂点座標の設定。
			for (int i = 0; i < stringBuilder.Length * 4 * 3; i += 12)
			{
				charaPositions[i] = offsetX;
				charaPositions[i + 1] = offsetY;
				charaPositions[i + 2] = PixelScreenPosition.Z;

				charaPositions[i + 3] = offsetX;
				charaPositions[i + 4] = offsetY + height;
				charaPositions[i + 5] = PixelScreenPosition.Z;

				charaPositions[i + 6] = offsetX + width;
				charaPositions[i + 7] = offsetY;
				charaPositions[i + 8] = PixelScreenPosition.Z;

				charaPositions[i + 9] = offsetX + width;
				charaPositions[i + 10]= offsetY + height;
				charaPositions[i + 11] = PixelScreenPosition.Z;

				offsetX += width;

				int code = stringBuilder[count];
				if (code == '\n')
				{
					offsetX = PixelScreenPosition.X;
					offsetY += height;
				}
				count++;
			}
			
			//@e Processing of strings
			//@j 文字列の処理。
			count = 0;
			for (int i = 0; i < stringBuilder.Length * 4 * 2; i += 8)
			{
				int code = stringBuilder[count];
				int val=0;
				if (' ' <= code && code <= '~')
				{
					val = code - ' ';
				}
				else if(code == '\n')
				{
					val = 0;
				}
				else
				{
					val = '?' - ' ';
				}
				
				int col = val % 16;
				int row = val / 16;
				
				charaTexcoords[i] = charaTexWidth * col;
				charaTexcoords[i + 1] = charaTexHeight * row;
				
				charaTexcoords[i + 2] = charaTexWidth * col;
				charaTexcoords[i + 3] = charaTexHeight * (row + 1);
				
				charaTexcoords[i + 4] = charaTexWidth * (col + 1);
				charaTexcoords[i + 5] = charaTexHeight * row;
				
				charaTexcoords[i + 6] = charaTexWidth * (col + 1);
				charaTexcoords[i + 7] = charaTexHeight * (row + 1);
				
				count++;
			}
			
			//@e Settings of color (Vertex color)
			//@j 色（頂点色）の設定。
			for (int i = 0; i < stringBuilder.Length * 4 * 4; i+=4)
			{
				charaColors[i] = color.R;
				charaColors[i + 1] = color.G;
				charaColors[i + 2] = color.B;
				charaColors[i + 3] = color.A;
			}
			
			//@e Index settings
			//@j インデックスの設定。
			count = 0;
			for (int i = 0; i < stringBuilder.Length * indexSize; i += indexSize)
			{
				charaIndices[i] = (ushort)count;
				charaIndices[i + 1] = (ushort)(count+1);
				charaIndices[i + 2] = (ushort)(count+2);
				charaIndices[i + 3] = (ushort)(count+1);
				charaIndices[i + 4] = (ushort)(count+2);
				charaIndices[i + 5] = (ushort)(count+3);
				count += 4;
			}
			
			vertices.SetVertices(0, charaPositions);
			vertices.SetVertices(1, charaTexcoords);
			vertices.SetVertices(2, charaColors);
			
			vertices.SetIndices(charaIndices);
			graphics.SetVertexBuffer(0, vertices);
			
			graphics.SetShaderProgram(shaderProgram);
			shaderProgram.SetUniformValue(0, ref unitScreenMatrix);
			
			graphics.DrawArrays(DrawMode.Triangles, 0, stringBuilder.Length * indexSize);
			
			graphics.Enable(EnableMode.DepthTest);
		}
		
		private static ShaderProgram CreateSimpleSpriteShader()
		{
			Byte[] dataBuffer = Utility.ReadEmbeddedFile("TutoLib.shaders.SimpleSprite.cgx");
				
			ShaderProgram shaderProgram = new ShaderProgram(dataBuffer);
			shaderProgram.SetUniformBinding(0, "u_WorldMatrix");
			
			return shaderProgram;
		}
	}
}
