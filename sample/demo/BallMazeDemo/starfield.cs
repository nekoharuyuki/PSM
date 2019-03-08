/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Demo_BallMaze
{
	public class StarfieldClass
	{
		static GraphicsContext	graphics = BallMazeProgram.graphics;
		ShaderProgram			program;
		Texture2D				texture;
		VertexBuffer 			vertices;

		const int				bufferStride = 8;
		static float[]			interleavedBuffer = new float[bufferStride*3*ModelCube.numTriangles_ModelCube*Defs.MAX_STARS];
		static Vector3[]		starPos = new Vector3[Defs.MAX_STARS];
		static float[]			starVel = new float[Defs.MAX_STARS];

		public StarfieldClass()
		{
			program = new ShaderProgram("/Application/shaders/main.cgx");
			texture = new Texture2D("/Application/data/star.png", false);

			program.SetUniformBinding(0, "projViewMatrix");
			program.SetAttributeBinding(0, "a_Position");
			program.SetAttributeBinding(1, "a_Normal");
			program.SetAttributeBinding(2, "a_TexCoord");

			// fragment shader bindings
			program.SetUniformBinding(1, "lightPosition");
			program.SetUniformBinding(2, "lightAmbient");
			program.SetUniformBinding(3, "eyePosLocal");
			program.SetUniformBinding(4, "shininess");

			vertices = new VertexBuffer(3*ModelCube.numTriangles_ModelCube*Defs.MAX_STARS, VertexFormat.Float3, VertexFormat.Float3, VertexFormat.Float2);

			for (int i=0; i<Defs.MAX_STARS; i++)
			{
				starPos[i] = new Vector3(Defs.MAX_STAR_SPREAD/2.0f - Defs.RANDOM_RANGE(Defs.MAX_STAR_SPREAD), -Defs.STAR_HEIGHT, Defs.MAX_STAR_SPREAD/2.0f - Defs.RANDOM_RANGE(Defs.MAX_STAR_SPREAD));
				starVel[i] = 5.0f + Defs.RANDOM_RANGE(Defs.MAX_STAR_SPEED);
			}
		}
		
		public void Dispose()
		{
			vertices.Dispose();
			texture.Dispose();
			program.Dispose();
		}

		public void Update(float dT)
		{
			for (int i=0; i<Defs.MAX_STARS; i++)
			{
				// check for stars out of range and reset them
				if (starPos[i].Y > Defs.STAR_HEIGHT)
				{
					starPos[i] = new Vector3(Defs.MAX_STAR_SPREAD/2.0f - Defs.RANDOM_RANGE(Defs.MAX_STAR_SPREAD), -Defs.STAR_HEIGHT, Defs.MAX_STAR_SPREAD/2.0f - Defs.RANDOM_RANGE(Defs.MAX_STAR_SPREAD));
					starVel[i] = 5.0f + Defs.RANDOM_RANGE(Defs.MAX_STAR_SPEED);
				}

				// move each star
				starPos[i].Y += starVel[i] * dT;
			}
		}

		public void Render()
		{
			int offset = 0;
			float scale = 0.1f;
			int numStars = 0;

			// use stretched cubes for our stars
			for (int i=0; i<Defs.MAX_STARS; i++)
			{
				// if we are in gameplay mode, skip any stars directly above the playfield
				if ((BallMazeProgram.gameState == Defs.eGameState.STATE_READY) || (BallMazeProgram.gameState == Defs.eGameState.STATE_PLAY) || (BallMazeProgram.gameState == Defs.eGameState.STATE_FINISH))
				{
					if (((starPos[i].X > -Defs.MAZE_SIZE*0.5f) && (starPos[i].X < Defs.MAZE_SIZE*0.5f)) && ((starPos[i].Z > -Defs.MAZE_SIZE*0.5f) && (starPos[i].Z < Defs.MAZE_SIZE*0.5f)))
					{
						continue;
					}
				}

				// scale the Y based on the speed
				float scaleY = scale * (Math.Abs(starVel[i])*0.25f);

				for (int j=0; j<3*ModelCube.numTriangles_ModelCube; j++)
				{
					interleavedBuffer[0+offset] = starPos[i].X + ModelCube.fVertices_ModelCube[3*j+0]*scale;
					interleavedBuffer[1+offset] = starPos[i].Y + ModelCube.fVertices_ModelCube[3*j+1]*scaleY;
					interleavedBuffer[2+offset] = starPos[i].Z + ModelCube.fVertices_ModelCube[3*j+2]*scale;
					interleavedBuffer[3+offset] = ModelCube.fNormals_ModelCube[3*j+0];
					interleavedBuffer[4+offset] = ModelCube.fNormals_ModelCube[3*j+1];
					interleavedBuffer[5+offset] = ModelCube.fNormals_ModelCube[3*j+2];
					interleavedBuffer[6+offset] = ModelCube.fTexcoords_ModelCube[2*j+0];
					interleavedBuffer[7+offset] = ModelCube.fTexcoords_ModelCube[2*j+1];
					offset += bufferStride;
				}

				numStars++;
			}

			// render the new buffer
			Matrix4 matTranslation, matRotation, matWVP, matInvWV, matTemp;

			matTranslation = Matrix4.Transformation(Vector3.Zero, Vector3.One);
			matRotation = Matrix4.RotationYxz(0.0f, 0.0f, 0.0f);
			Defs.matWorld = matTranslation * matRotation;
			matTemp = Defs.matProjection * Defs.matView;
			matWVP = matTemp * Defs.matWorld;

			// calculate light vector
			Vector3 vecLight;
			matTemp = Defs.matWorld.InverseOrthonormal();
			vecLight = matTemp.TransformVector(Defs.vecGlobalLight);

			// calculate the eye position vector for the specular lighting
			matTemp = Defs.matView * Defs.matWorld;
			matInvWV = matTemp.InverseOrthonormal();
			Vector3 vecEyePosLocal = matInvWV.AxisW;

			program.SetUniformValue(0, ref matWVP);
			program.SetUniformValue(1, ref vecLight);
			program.SetUniformValue(2, ref Defs.lightGlobalAmb);
			program.SetUniformValue(3, ref vecEyePosLocal);
			program.SetUniformValue(4, 500.0f);

			graphics.SetShaderProgram(program);
			vertices.SetVertices(interleavedBuffer);
			graphics.SetVertexBuffer(0, vertices);
			graphics.SetTexture(0, texture);

			graphics.DrawArrays(DrawMode.Triangles, 0, 36*numStars);
		}
	}
}
