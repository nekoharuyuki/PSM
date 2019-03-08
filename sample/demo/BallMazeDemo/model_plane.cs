/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Demo_BallMaze
{
	public class ModelPlane
	{
		ShaderProgram program;
		VertexBuffer vertices;
		Texture2D[] texture = new Texture2D[3];

		public ModelPlane()
		{
			program = new ShaderProgram("/Application/shaders/main.cgx");
			vertices = new VertexBuffer(3*numTriangles_ModelPlane, VertexFormat.Float3, VertexFormat.Float3, VertexFormat.Float2);
			texture[0] = new Texture2D("/Application/data/glass.png", false);
			texture[1] = new Texture2D("/Application/data/glass_start.png", false);
			texture[2] = new Texture2D("/Application/data/glass_end.png", false);

			// vertex shader bindings
			program.SetUniformBinding(0, "projViewMatrix");
			program.SetAttributeBinding(0, "a_Position");
			program.SetAttributeBinding(1, "a_Normal");
			program.SetAttributeBinding(2, "a_TexCoord");

			// fragment shader bindings
			program.SetUniformBinding(1, "lightPosition");
			program.SetUniformBinding(2, "lightAmbient");
			program.SetUniformBinding(3, "eyePosLocal");
			program.SetUniformBinding(4, "shininess");

			vertices.SetVertices(0, fVertices_ModelPlane);
			vertices.SetVertices(1, fNormals_ModelPlane);
			vertices.SetVertices(2, fTexcoords_ModelPlane);
		}
	
		public void Dispose()
		{
			program.Dispose();
			vertices.Dispose();
			texture[0].Dispose();
			texture[1].Dispose();
			texture[2].Dispose();
		}

		public void Render(GraphicsContext graphics, ref Matrix4 matrix, ref Vector3 lightVec, ref Vector3 vecEyePosLocal, int index)
		{
			program.SetUniformValue(0, ref matrix);
			program.SetUniformValue(1, ref lightVec);
			program.SetUniformValue(2, ref Defs.lightGlobalAmb);
			program.SetUniformValue(3, ref vecEyePosLocal);
			program.SetUniformValue(4, 10000.0f);
	
			graphics.SetShaderProgram(program);
			graphics.SetVertexBuffer(0, vertices);
	
			graphics.SetTexture(0, texture[index]);
			graphics.DrawArrays(DrawMode.Triangles, 0, 3*numTriangles_ModelPlane);
		}

		static int numTriangles_ModelPlane = 2;

		static float[] fVertices_ModelPlane = {
			-0.50000000f, 0.00000000f, 0.50000000f,
			0.50000000f, 0.00000000f, 0.50000000f,
			-0.50000000f, 0.00000000f, -0.50000000f,
			-0.50000000f, 0.00000000f, -0.50000000f,
			0.50000000f, 0.00000000f, 0.50000000f,
			0.50000000f, 0.00000000f, -0.50000000f,
		};

		static float[] fNormals_ModelPlane = {
			0.00000000f, 1.00000000f, 0.00000000f,
			0.00000000f, 1.00000000f, 0.00000000f,
			0.00000000f, 1.00000000f, 0.00000000f,
			0.00000000f, 1.00000000f, 0.00000000f,
			0.00000000f, 1.00000000f, 0.00000000f,
			0.00000000f, 1.00000000f, 0.00000000f,
		};

		static float[] fTexcoords_ModelPlane = {
			0.00000000f, 0.00000000f,
			1.00000000f, 0.00000000f,
			0.00000000f, 1.00000000f,
			0.00000000f, 1.00000000f,
			1.00000000f, 0.00000000f,
			1.00000000f, 1.00000000f,
		};
    }
}