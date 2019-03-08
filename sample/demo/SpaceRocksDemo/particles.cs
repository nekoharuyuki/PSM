/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;

namespace Demo_SpaceRocks
{
	public class sParticle
	{
		public Vector3	    position;
		public Vector3	    velocity;
        public ImageColor   color;
		public float	    scale;
		public float		scaleStart;
		public float		age;
		public float		ageStart;

		public void Clear()
		{
			position = Vector3.Zero;
			velocity = Vector3.Zero;
			scale = 0.0f;
			scaleStart = 0.0f;
			age = 0.0f;
			ageStart = 0.0f;
            color.R = color.G = color.B = color.A = 0;
		}
	}
	
	class ParticleSystemClass
	{
		static GraphicsContext graphics = SpaceRocksProgram.graphics;
		ShaderProgram	program;
		Texture2D		texture;
		VertexBuffer 	vertices;
		sParticle[]		particleObjects = new sParticle[Defs.MAX_PARTICLES];
		Vector3[]		positions = new Vector3[6*Defs.MAX_PARTICLES];
		Vector2[]		texcoords = new Vector2[6*Defs.MAX_PARTICLES];
		Vector4[]		colors = new Vector4[6 * Defs.MAX_PARTICLES];

		public ParticleSystemClass()
		{
			program = new ShaderProgram("/Application/shaders/particle.cgx");
			texture = new Texture2D("/Application/data/particle.png", false);

			program.SetUniformBinding(0, "projViewMatrix");
			program.SetAttributeBinding(0, "a_Position");
			program.SetAttributeBinding(1, "a_TexCoord");

			vertices = new VertexBuffer(6*Defs.MAX_PARTICLES, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);

			for (int i=0; i<Defs.MAX_PARTICLES; i++)
			{
				particleObjects[i] = new sParticle();
				particleObjects[i].Clear();
			}
		}
		
		public void Dispose()
		{
			vertices.Dispose();
			texture.Dispose();
			program.Dispose();
		}

		public void Spawn(Vector3 position, Vector3 velocity, float scale, float age, ImageColor color)
		{
			for (int i=0; i<Defs.MAX_PARTICLES; i++)
			{
				if (particleObjects[i].age == 0.0f)
				{
					particleObjects[i].position = position;
					particleObjects[i].velocity = velocity;
					particleObjects[i].scale = scale;
					particleObjects[i].scaleStart = scale;
					particleObjects[i].age = age;
					particleObjects[i].ageStart = age;
					particleObjects[i].color = color;
					break;
				}
			}
		}

		public void Update(float dT)
		{
			for (int i=0; i<Defs.MAX_PARTICLES; i++)
			{
				if (particleObjects[i].age > 0.0f)
				{
					particleObjects[i].position += particleObjects[i].velocity * dT;
	
					// check for particle death
					particleObjects[i].age -= dT;
					if (particleObjects[i].age <= 0.0f)
					{
						particleObjects[i].Clear();
					}

					// scale alpha and size based on age
					particleObjects[i].color.A = (int)(255.0f * particleObjects[i].age / particleObjects[i].ageStart);
					particleObjects[i].scale -= 0.5f * dT;
				}
			}
		}
		
		public void Render()
		{
			int count = 0;
			Matrix4 matTemp1, matTemp2;
			Defs.matWorld = Matrix4.Identity;
			matTemp1 = Defs.matProjection * Defs.matView;
			matTemp2 = matTemp1 * Defs.matWorld;

			for (int i=0; i<Defs.MAX_PARTICLES; i++)
			{
				if (particleObjects[i].age > 0.0f)
				{
					Vector3 pos = particleObjects[i].position;
					float scale = particleObjects[i].scale * 0.5f;
					Vector4 color;
					color.X = (float)particleObjects[i].color.R / 255.0f;
					color.Y = (float)particleObjects[i].color.G / 255.0f;
					color.Z = (float)particleObjects[i].color.B / 255.0f;
					color.W = (float)particleObjects[i].color.A / 255.0f;

					positions[6 * count + 0] = new Vector3(pos.X - scale, 0.0f, pos.Z - scale);
					positions[6 * count + 1] = new Vector3(pos.X + scale, 0.0f, pos.Z - scale);
					positions[6 * count + 2] = new Vector3(pos.X - scale, 0.0f, pos.Z + scale);
					positions[6 * count + 3] = new Vector3(pos.X - scale, 0.0f, pos.Z + scale);
					positions[6 * count + 4] = new Vector3(pos.X + scale, 0.0f, pos.Z - scale);
					positions[6 * count + 5] = new Vector3(pos.X + scale, 0.0f, pos.Z + scale);
					texcoords[6 * count + 0] = new Vector2(0.0f, 0.0f);
					texcoords[6 * count + 1] = new Vector2(1.0f, 0.0f);
					texcoords[6 * count + 2] = new Vector2(0.0f, 1.0f);
					texcoords[6 * count + 3] = new Vector2(0.0f, 1.0f);
					texcoords[6 * count + 4] = new Vector2(1.0f, 0.0f);
					texcoords[6 * count + 5] = new Vector2(1.0f, 1.0f);
					colors[6 * count + 0] = new Vector4(color.X, color.Y, color.Z, color.W);
					colors[6 * count + 1] = new Vector4(color.X, color.Y, color.Z, color.W);
					colors[6 * count + 2] = new Vector4(color.X, color.Y, color.Z, color.W);
					colors[6 * count + 3] = new Vector4(color.X, color.Y, color.Z, color.W);
					colors[6 * count + 4] = new Vector4(color.X, color.Y, color.Z, color.W);
					colors[6 * count + 5] = new Vector4(color.X, color.Y, color.Z, color.W);
					count++;
				}
			}

			vertices.SetVertices(0, positions);
			vertices.SetVertices(1, texcoords);
			vertices.SetVertices(2, colors);

			program.SetUniformValue(0, ref matTemp2);

			graphics.SetShaderProgram(program);
			graphics.SetVertexBuffer(0, vertices);
			graphics.SetTexture(0, texture);
			
			graphics.Enable(EnableMode.Blend);
			graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			graphics.Disable(EnableMode.DepthTest);

			graphics.DrawArrays(DrawMode.Triangles, 0, 6*count);
		}
	}
}
