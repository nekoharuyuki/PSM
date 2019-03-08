/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Demo_BallMaze
{
	public class sObject
	{
		public Vector3			position;
		public Vector3			rotation;
		public Vector3			velocity;
		public Vector3			angularVelocity;
		public Vector3			acceleration;
		public float			scale;
		public Defs.eBlockType	type;
		public bool				bIsActive;

		public void Clear()
		{
			position = Vector3.Zero;
			rotation = Vector3.Zero;
			velocity = Vector3.Zero;
			angularVelocity = Vector3.Zero;
			acceleration = Vector3.Zero;
			scale = 0.0f;
			type = (int)Defs.eBlockType.MAZE_EMPTY;
			bIsActive = false;
		}
	}
	
	public class PlayerClass
	{
		private sObject		playerObject = new sObject();
		public Quaternion	quat;
		
		public sObject Get()
		{
			return playerObject;
		}

		public void Init()
		{
			playerObject.Clear();
			playerObject.scale = Defs.BALL_SCALE;
			quat = Quaternion.Identity;

			playerObject.position = new Vector3(1.0f-(Defs.MAZE_SIZE*0.5f), Defs.BALL_SCALE*0.5f, 1.0f-(Defs.MAZE_SIZE*0.5f)) + new Vector3(0.5f, 0.0f, 0.5f);
			playerObject.position.Y = Defs.CUBE_FALL_DIST;
		}
		
		public void Update(float dT)
		{
			if (BallMazeProgram.gameState == Defs.eGameState.STATE_READY)
			{
				float newY;
				if (BallMazeProgram.countdownTimer <= 2.0f)
				{
					newY = playerObject.position.Y;
					if (newY != Defs.BALL_SCALE*0.5f)
					{
						playerObject.velocity.Y -= dT * Defs.CUBE_FALL_DIST * 0.1f;
						if (playerObject.velocity.Y < -2.0f)
						{
							playerObject.velocity.Y = -2.0f;
						}
						newY += playerObject.velocity.Y;

						// bounce when we hit the ground unless we are almost out of time
						if (newY < Defs.BALL_SCALE*0.5f)
						{
							if (BallMazeProgram.countdownTimer > 0.2f)
							{
								if (Math.Abs(playerObject.velocity.Y) > 0.5f)
								{
									BallMazeProgram.soundPlayer.Play(Defs.eSound.SND_WALL_HIT, 1.0f, false);
								}
								playerObject.velocity.Y *= -0.5f;
								newY = Defs.BALL_SCALE*0.5f+playerObject.velocity.Y;
							}
							else
							{
								playerObject.velocity.Y = 0.0f;
								newY = Defs.BALL_SCALE*0.5f;
							}
						}

						playerObject.position.Y = newY;
					}
				}
			}
			else
			{
				playerObject.position.Y = Defs.BALL_SCALE*0.5f;
			}
		}

		public void Render()
		{
			if (BallMazeProgram.gameState != Defs.eGameState.STATE_ATTRACT)
			{
				BallMazeProgram.DrawModel(playerObject.position, playerObject.rotation, true, quat, new Vector3(playerObject.scale, playerObject.scale, playerObject.scale), Defs.eModel.MODEL_BALL, 0);
			}
		}
	}

	public class MazeClass
	{
		static GraphicsContext graphics = BallMazeProgram.graphics;
		ShaderProgram program;
		Texture2D[] texture = new Texture2D[2];
		VertexBuffer vertices;

		const int bufferStride = 8;
		static float[] interleavedBufferPenalties = new float[bufferStride*3*ModelCube.numTriangles_ModelCube*Defs.NUM_PENALTY_BLOCKS];
		static float[] interleavedBufferWalls = new float[bufferStride*3*ModelCube.numTriangles_ModelCube*(Defs.MAZE_SIZE*Defs.MAZE_SIZE-Defs.NUM_PENALTY_BLOCKS)];
		private sObject[,] mazeObjects = new sObject[Defs.MAZE_SIZE,Defs.MAZE_SIZE];
		bool bBlocksAreSet = false;
		bool bBlocksAreDrawn = false;
		int numWalls = 0;
		int numPenalties = 0;

		public MazeClass()
		{
			program = new ShaderProgram("/Application/shaders/main.cgx");
			texture[0] = new Texture2D("/Application/data/wood.png", false);
			texture[1] = new Texture2D("/Application/data/skull.png", false);

			program.SetUniformBinding(0, "projViewMatrix");
			program.SetAttributeBinding(0, "a_Position");
			program.SetAttributeBinding(1, "a_Normal");
			program.SetAttributeBinding(2, "a_TexCoord");

			// fragment shader bindings
			program.SetUniformBinding(1, "lightPosition");
			program.SetUniformBinding(2, "lightAmbient");
			program.SetUniformBinding(3, "eyePosLocal");
			program.SetUniformBinding(4, "shininess");

			vertices = new VertexBuffer(3*ModelCube.numTriangles_ModelCube*Defs.MAZE_SIZE*Defs.MAZE_SIZE, VertexFormat.Float3, VertexFormat.Float3, VertexFormat.Float2);
		}

		public sObject Get(int x, int y)
		{
			return mazeObjects[x,y];
		}

		public void Init()
		{
			for (int j=0; j<Defs.MAZE_SIZE; j++)
			{
				for (int i=0; i<Defs.MAZE_SIZE; i++)
				{
					mazeObjects[i,j] = new sObject();
					mazeObjects[i,j].Clear();
					mazeObjects[i,j].scale = 1.0f;
				}
			}
			bBlocksAreSet = false;
			bBlocksAreDrawn = false;
			numWalls = 0;
			numPenalties = 0;
		}
		
		public void Clear()
		{
			for (int j=0; j<Defs.MAZE_SIZE; j++)
			{
				for (int i=0; i<Defs.MAZE_SIZE; i++)
				{
					mazeObjects[i,j].Clear();
					mazeObjects[i,j].scale = 1.0f;
				}
			}
			bBlocksAreSet = false;
			bBlocksAreDrawn = false;
			numWalls = 0;
			numPenalties = 0;
		}

		public void Update(float dT)
		{
			if (BallMazeProgram.gameState == Defs.eGameState.STATE_READY)
			{
				// draw the cubes falling from the sky
				bool bDropSound = false;
				float newY;
				for (int j=0; j<Defs.MAZE_SIZE; j++)
				{
					for (int i=0; i<Defs.MAZE_SIZE; i++)
					{
						if (mazeObjects[i,j].bIsActive)
						{
							newY = mazeObjects[i,j].position.Y;

							if (newY != 0.0f)
							{
								mazeObjects[i,j].velocity.Y -= dT * Defs.CUBE_FALL_DIST * 0.1f;
								if (mazeObjects[i,j].velocity.Y < -2.0f)
								{
									mazeObjects[i,j].velocity.Y = -2.0f;
								}
								newY += mazeObjects[i,j].velocity.Y;

								// bounce when we hit the ground unless we are almost out of time
								if (newY < 0.0f)
								{
									if (BallMazeProgram.countdownTimer > 1.0f)
									{
										if ((Math.Abs(mazeObjects[i,j].velocity.Y) > 0.5f) && (Defs.RANDOM_RANGE(1.0f) < 0.05f))
										{
											bDropSound = true;
										}
										mazeObjects[i,j].velocity.Y *= -0.5f;
										newY = mazeObjects[i,j].velocity.Y;
									}
									else
									{
										mazeObjects[i,j].velocity.Y = 0.0f;
										newY = 0.0f;
									}
								}

								mazeObjects[i,j].position.Y = newY;
							}
						}
					}
				}
				if (bDropSound)
				{
					BallMazeProgram.soundPlayer.Play(Defs.eSound.SND_CUBE_BOUNCE, 1.0f, false);
				}
			}
			else
			{
				if (!bBlocksAreSet)
				{
					for (int j=0; j<Defs.MAZE_SIZE; j++)
					{
						for (int i=0; i<Defs.MAZE_SIZE; i++)
						{
							if (mazeObjects[i, j].bIsActive)
							{
								mazeObjects[i, j].position.Y = 0.0f;
							}
						}
					}
					bBlocksAreSet = true;
				}
			}
		}

		public void Render()
		{
			int i, j, k;
			int offsetWalls = 0;
			int offsetPenalties = 0;

			if (BallMazeProgram.gameState != Defs.eGameState.STATE_ATTRACT)
			{
				if (!bBlocksAreDrawn)
				{
					numWalls = 0;
					numPenalties = 0;

					// draw walls first...
					for (j=0; j<Defs.MAZE_SIZE; j++)
					{
						for (i=0; i<Defs.MAZE_SIZE; i++)
						{
							if (mazeObjects[i, j].bIsActive)
							{
								if (mazeObjects[i, j].type == Defs.eBlockType.MAZE_PENALTYWALL)
								{
									for (k=0; k<3*ModelCube.numTriangles_ModelCube; k++)
									{
										interleavedBufferPenalties[0+offsetPenalties] = mazeObjects[i, j].position.X + ModelCube.fVertices_ModelCube[3*k+0];
										interleavedBufferPenalties[1+offsetPenalties] = mazeObjects[i, j].position.Y + ModelCube.fVertices_ModelCube[3*k+1];
										interleavedBufferPenalties[2+offsetPenalties] = mazeObjects[i, j].position.Z + ModelCube.fVertices_ModelCube[3*k+2];
										interleavedBufferPenalties[3+offsetPenalties] = ModelCube.fNormals_ModelCube[3*k+0];
										interleavedBufferPenalties[4+offsetPenalties] = ModelCube.fNormals_ModelCube[3*k+1];
										interleavedBufferPenalties[5+offsetPenalties] = ModelCube.fNormals_ModelCube[3*k+2];
										interleavedBufferPenalties[6+offsetPenalties] = ModelCube.fTexcoords_ModelCube[2*k+0];
										interleavedBufferPenalties[7+offsetPenalties] = ModelCube.fTexcoords_ModelCube[2*k+1];
										offsetPenalties += bufferStride;
									}
									numPenalties++;
								}
								else if ((mazeObjects[i, j].type == Defs.eBlockType.MAZE_INNERWALL) || (mazeObjects[i, j].type == Defs.eBlockType.MAZE_OUTERWALL))
								{
									for (k=0; k<3*ModelCube.numTriangles_ModelCube; k++)
									{
										interleavedBufferWalls[0+offsetWalls] = mazeObjects[i, j].position.X + ModelCube.fVertices_ModelCube[3*k+0];
										interleavedBufferWalls[1+offsetWalls] = mazeObjects[i, j].position.Y + ModelCube.fVertices_ModelCube[3*k+1];
										interleavedBufferWalls[2+offsetWalls] = mazeObjects[i, j].position.Z + ModelCube.fVertices_ModelCube[3*k+2];
										interleavedBufferWalls[3+offsetWalls] = ModelCube.fNormals_ModelCube[3*k+0];
										interleavedBufferWalls[4+offsetWalls] = ModelCube.fNormals_ModelCube[3*k+1];
										interleavedBufferWalls[5+offsetWalls] = ModelCube.fNormals_ModelCube[3*k+2];
										interleavedBufferWalls[6+offsetWalls] = ModelCube.fTexcoords_ModelCube[2*k+0];
										interleavedBufferWalls[7+offsetWalls] = ModelCube.fTexcoords_ModelCube[2*k+1];
										offsetWalls += bufferStride;
									}
									numWalls++;
								}
							}
						}
					}
					if (bBlocksAreSet)
					{
						bBlocksAreDrawn = true;
					}
				}

				// draw the batched up cubes
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

				vertices.SetVertices(interleavedBufferPenalties, 0, 0, 36*numPenalties);
				graphics.SetVertexBuffer(0, vertices);
				graphics.SetTexture(0, texture[1]);
				graphics.DrawArrays(DrawMode.Triangles, 0, 36*numPenalties);

				vertices.SetVertices(interleavedBufferWalls, 0, 0, interleavedBufferWalls.Length / 8 );
				graphics.SetVertexBuffer(0, vertices);
				graphics.SetTexture(0, texture[0]);
				graphics.DrawArrays(DrawMode.Triangles, 0, 36*numWalls);


				// then draw the start and finish pads for correct alpha
				i = j = 1;
				BallMazeProgram.DrawModel(mazeObjects[i, j].position - new Vector3(0.0f, Defs.PAD_BIAS, 0.0f), mazeObjects[i, j].rotation, false, Quaternion.Identity, new Vector3(mazeObjects[i, j].scale, mazeObjects[i, j].scale, mazeObjects[i, j].scale), Defs.eModel.MODEL_PLANE, 1);
				i = j = Defs.MAZE_SIZE-2;
				BallMazeProgram.DrawModel(mazeObjects[i, j].position - new Vector3(0.0f, Defs.PAD_BIAS, 0.0f), mazeObjects[i, j].rotation, false, Quaternion.Identity, new Vector3(mazeObjects[i, j].scale, mazeObjects[i, j].scale, mazeObjects[i, j].scale), Defs.eModel.MODEL_PLANE, 2);
			}
		}
	}

	public class ArrowClass
	{
		private sObject arrowObject = new sObject();

		public sObject Get()
		{
			return arrowObject;
		}

		public void Init()
		{
			arrowObject.Clear();
			arrowObject.scale = 0.15f;
		}

		public void Update(float dT)
		{
			if (BallMazeProgram.gameState == Defs.eGameState.STATE_PLAY)
			{
				Vector3 smoothArrowPos = Vector3.Zero;
				Vector3 smoothArrowAng = Vector3.Zero;
				Vector3 offsetPos = Vector3.Zero;
				Vector3 offsetAng = Vector3.Zero;

				smoothArrowPos = new Vector3((Defs.MAZE_SIZE/2)-1.0f, 0.0f, (Defs.MAZE_SIZE/2)-1.0f);

				if (BallMazeProgram.bZoomMode || ((BallMazeProgram.thePlayer.Get().position.X > (Defs.MAZE_SIZE/2)-6.0f) && (BallMazeProgram.thePlayer.Get().position.Z > (Defs.MAZE_SIZE/2)-4.0f)))
				{
					// if zoomed or very close, draw the arrow above the finish and pointing down
					smoothArrowAng = new Vector3(0.0f, 0.0f, -90.0f);
					smoothArrowPos.Y = 2.0f + (float)Math.Sin(4.0f*(float)BallMazeProgram.stopwatch.ElapsedMilliseconds/1000.0f);
				}
				else
				{
					// otherwise, draw it near the ball and pointing in the direction of the finish
					// normalize the vector
					smoothArrowPos -= BallMazeProgram.thePlayer.Get().position;

					float size = (float)Math.Sqrt(smoothArrowPos.X*smoothArrowPos.X + smoothArrowPos.Z*smoothArrowPos.Z);
					smoothArrowPos.X = (smoothArrowPos.X / size) * 3.0f;
					smoothArrowPos.Z = (smoothArrowPos.Z / size) * 3.0f;

					// keep it from getting to zero
					if (smoothArrowPos.X < 0.001f)
						smoothArrowPos.X = 0.001f;
					if (smoothArrowPos.Z < 0.001f)
						smoothArrowPos.Z = 0.001f;

					// calculate the angle to point
					smoothArrowAng = new Vector3(0.0f, Defs.RADIANS_TO_DEGREES((float)-Math.Atan(smoothArrowPos.Z / smoothArrowPos.X)), 0.0f);

					// put the arrow above the maze and move it
					smoothArrowPos += BallMazeProgram.thePlayer.Get().position;
					smoothArrowPos.Y = 1.5f;
				}

				// smooth the view to the desired location
				offsetPos = (smoothArrowPos - arrowObject.position) * dT * Defs.CAMERA_SPEED * 2.0f;
				offsetAng = (smoothArrowAng - arrowObject.rotation) * dT * Defs.CAMERA_SPEED;

				arrowObject.position += offsetPos;
				arrowObject.rotation += offsetAng;
			}
		}

		public void Render()
		{
			if (BallMazeProgram.gameState == Defs.eGameState.STATE_PLAY)
			{
				BallMazeProgram.DrawModel(arrowObject.position, arrowObject.rotation, false, Quaternion.Identity, new Vector3(arrowObject.scale, arrowObject.scale, arrowObject.scale), Defs.eModel.MODEL_ARROW, 0);
			}
		}
	}

	public class PlaneClass
	{
		private sObject planeObject = new sObject();

		public sObject Get()
		{
			return planeObject;
		}

		public void Init()
		{
			planeObject.Clear();
			planeObject.scale = (float)Defs.MAZE_SIZE;
		}

		public void Update(float dT)
		{
		}

		public void Render()
		{
			if (BallMazeProgram.gameState != Defs.eGameState.STATE_ATTRACT)
			{
				BallMazeProgram.DrawModel(planeObject.position, planeObject.rotation, false, Quaternion.Identity, new Vector3(planeObject.scale, planeObject.scale, planeObject.scale), Defs.eModel.MODEL_PLANE, 0);
			}
		}
	}
}