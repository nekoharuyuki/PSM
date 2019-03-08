/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;

namespace Demo_BrickSmash
{
	// テキストの表示を管理するクラス
	class TextPlane : IDisposable
	{
		private static Matrix4 projectionMatrix;
		private static Matrix4 viewMatrix;

		private static GraphicsContext graphics = BrickSmashProgram.graphics;
		private static ShaderProgram	program;
		private static Font				font;
		private static List<TextData>	textDict= new List<TextData>();

		// コンストラクタ
		public TextPlane(int height)
		{
			program = new ShaderProgram("/Application/shaders/font.cgx");

			program.SetUniformBinding(0, "u_WorldMatrix");
			program.SetAttributeBinding(0, "a_Position");
			program.SetAttributeBinding(1, "a_TexCoord");

			font = new Font(FontAlias.System, height, FontStyle.Regular);

			projectionMatrix = Matrix4.Ortho(0, Defs.SCREEN_WIDTH,
											 0, Defs.SCREEN_HEIGHT,
											 0.0f, 32768.0f);

			viewMatrix = Matrix4.LookAt(new Vector3(0, Defs.SCREEN_HEIGHT, 0),
										new Vector3(0, Defs.SCREEN_HEIGHT, 1),
										new Vector3(0, -1, 0));
		}

		public void Dispose()
		{
			font.Dispose();
			program.Dispose();
			textDict.Clear();
		}

		public void Print(ImagePosition pos, ImageColor color, string text, bool bDropShadow, int id, bool isUpdate)
		{
			int	 listID = -1;
			bool listIsUpdate = false;

			// is already added list?
			for(int i=0; i < textDict.Count; i++){
				listID		 = textDict[i].GetTextDataID();
				listIsUpdate = textDict[i].GetTextDataIsUpdate();
				if(listID == id){
					if(true == listIsUpdate){
						textDict.RemoveAt(i);
						textDict.Add(new TextData(font, pos, color, text, id, isUpdate));
					}
					return;
				}
			}

			// add new list
			textDict.Add(new TextData(font, pos, color, text, id, isUpdate));
		}

		public void SetRender(int id, bool isRender)
		{
			for(int i=0; i < textDict.Count; i++){
				int listID = textDict[i].GetTextDataID();
				if(id == listID){
					textDict[i].SetTextRender(isRender);
					return;
				}
			}
		}


		public void Render()
		{
			for(int i=0; i < textDict.Count; i++){
				if(true == textDict[i].GetTextRender()){
					var modelMatrix = textDict[i].CreateModelMatrix();
					var worldViewProj = projectionMatrix * viewMatrix * modelMatrix;

					program.SetUniformValue(0, ref worldViewProj);
					program.SetAttributeBinding(0, "a_Position");
					program.SetAttributeBinding(1, "a_TexCoord");

					graphics.SetShaderProgram(program);
					graphics.SetVertexBuffer(0, textDict[i].vertices);
					graphics.SetTexture(0, textDict[i].texture);
		
					graphics.Enable(EnableMode.Blend);
					graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);

					graphics.DrawArrays(DrawMode.TriangleStrip, 0, 4);
				}
			}
		}
	}

	// 表示するテキストを個別に管理するクラス
	class TextData
	{
		public VertexBuffer 	vertices;
		public Texture2D		texture;
		private string			text = "";
		private int 			id = -1;
		private bool			isUpdate = false;
		private bool			isRender = true;
		private float			positionX;
		private float			positionY;
		private float			centerX;
		private float			centerY;

		public TextData(Font font, ImagePosition pos, ImageColor color, string text, int id, bool isUpdate)
		{
			this.text		= text;
			this.id			= id;
			this.isUpdate	= isUpdate;

			// check for auto-centering
			if (pos.X == -1)
			{
				pos.X = (Defs.SCREEN_WIDTH - font.GetTextWidth(text)) / 2;
			}
			int width = font.GetTextWidth(text, 0, text.Length);
			int height = font.Metrics.Height;

			positionX = pos.X;
			positionY = pos.Y;

			// clamp to max size
			width = Math.Min( width, 2048 );

			var image = new Image(ImageMode.Rgba, new ImageSize(width, height), new ImageColor(0, 0, 0, 0) );

			image.DrawText(text, color, font, new ImagePosition(0, 0));

			texture = new Texture2D(width, height, false, PixelFormat.Rgba);
			texture.SetPixels(0, image.ToBuffer(), 0, 0, width, height);
			image.Dispose();

			centerX = texture.Width / 2;
			centerY = texture.Height / 2;

			float l = 0;
			float t = 0;
			float r = texture.Width;
			float b = texture.Height;

			vertices = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float2);
			vertices.SetVertices(0, new float[]{r, t, 0,
												l, t, 0,
												r, b, 0,
												l, b, 0});

			vertices.SetVertices(1, new float[]{1.0f, 0.0f,
												0.0f, 0.0f,
												1.0f, 1.0f,
												0.0f, 1.0f});

		}

		public Matrix4 CreateModelMatrix()
		{
			var centerMatrix = Matrix4.Translation(new Vector3(-centerX, -centerY, 0.0f));
			var transMatrix = Matrix4.Translation(new Vector3(positionX + centerX, positionY + centerY, 0.0f));
			var rotMatrix = Matrix4.RotationZ((float)(0.0f / 180.0f * FMath.PI));
			var scaleMatrix = Matrix4.Scale(new Vector3(1.0f, 1.0f, 1.0f));

			return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
		}

		public string GetTextDataText()
		{
			return this.text;
		}

		public int GetTextDataID()
		{
			return this.id;
		}

		public bool GetTextDataIsUpdate()
		{
			return this.isUpdate;
		}

		public void SetTextRender(bool isRender)
		{
			this.isRender = isRender;
		}

		public bool GetTextRender()
		{
			return this.isRender;
		}
	}
}
