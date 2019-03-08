/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// TODO
// - fix collision normals on sides of bricks and paddle
// - add skew to normals for paddle english
// - fix font drop shadow
//

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;

namespace Demo_BrickSmash
{
	public class BrickSmashProgram
	{
		static Stopwatch				stopwatch = new Stopwatch();
		static public GraphicsContext	graphics = new GraphicsContext();
		static ModelPaddle				modelPaddle;
		static ModelBall				modelBall;
		static ModelBrick				modelBrick;
		static SoundSystem				soundPlayer;

		static int						level, score, numShips;
		static float					hudBlinkTimer, readyTimer, lastPaddleHitTime;
		static Defs.eGameState			gameState;
		static bool						bQuit;
			
		static public PlayerClass		thePlayer;
		static BallClass				theBall;
		static BrickClass				theBricks;
		static ParticleSystemClass		theParticles;
		static TextPlane				hudText;

		static void Main(string[] args)
		{
			float startSeconds = 0.0f;
			float endSeconds = 0.016f;
			float lastSeconds;

			Init();
			while (!bQuit)
			{
				lastSeconds = endSeconds - startSeconds;
				startSeconds = (float)stopwatch.ElapsedMilliseconds / 1000.0f;
				SystemEvents.CheckEvents();
				Update(lastSeconds);
				Render(lastSeconds);
				endSeconds = (float)stopwatch.ElapsedMilliseconds / 1000.0f;
			}
			Shutdown();
		}
	
		static void Init()
		{
			stopwatch.Start();
			Defs.SCREEN_WIDTH = graphics.Screen.Width;
			Defs.SCREEN_HEIGHT = graphics.Screen.Height;
			modelPaddle = new ModelPaddle();
			modelBall = new ModelBall();
			modelBrick = new ModelBrick();
			soundPlayer = new SoundSystem();
	
			thePlayer = new PlayerClass();
			theBall = new BallClass();
			theBricks = new BrickClass();
			theParticles = new ParticleSystemClass();

			hudText = new TextPlane(30);

			thePlayer.Init();
			gameState = Defs.eGameState.STATE_GAMEOVER;
			hudBlinkTimer = 0.0f;
			lastPaddleHitTime = 0.0f;
			bQuit = false;
		}
	
		static void Shutdown()
		{
			stopwatch.Stop();
			hudText.Dispose();
			modelPaddle.Dispose();
			modelBall.Dispose();
			modelBrick.Dispose();
			theParticles.Dispose();
		}

		static void Update(float dT)
		{
			GamePadData padData = GamePad.GetData(0);
			List<TouchData> touchDataList = Touch.GetData(0);

			if (gameState != Defs.eGameState.STATE_GAMEOVER)
			{
				// move the paddle with the analog pad...
				float x = padData.AnalogLeftX;
				float y = padData.AnalogLeftY;

				// ...or the touch screen
				foreach (TouchData touch in touchDataList)
				{
					if (touch.Status == TouchStatus.Move)
					{
						x = touch.X;
						y = touch.Y;
						break;
					}
				}

				// turn the x and y data into an angle
				// アナログパッド入力に一定範囲の遊びをもたせる
				if ( (Math.Abs(x) >= 0.1f) || (Math.Abs(y) >= 0.1f) )
				{
					float ang = (float)Math.Atan2(-y, x);
					x = (float)Math.Cos(ang) * Defs.PLAYER_RADIUS;
					y = (float)Math.Sin(ang) * Defs.PLAYER_RADIUS;
					float rot = 90.0f - Defs.RADIANS_TO_DEGREES(ang);
					thePlayer.Get().position.X = x;
					thePlayer.Get().position.Z = y;
					thePlayer.Get().rotation.Y = rot;
				}
			}

			if (gameState == Defs.eGameState.STATE_GAMEOVER)
			{
				if ((padData.ButtonsDown & GamePadButtons.Start) != 0)
				{
					level = 1;
					score = 0;
					numShips = 3;
					hudBlinkTimer = 0.0f;
					lastPaddleHitTime = 0.0f;
					readyTimer = Defs.READY_TIME;
					thePlayer.Init();
					theBall.Init();
					theBricks.Init(level);
					gameState = Defs.eGameState.STATE_READY;
				}

				theParticles.Update(dT);
			}
			else if (gameState == Defs.eGameState.STATE_READY)
			{
				readyTimer -= dT;
				if (readyTimer <= 0.0f)
				{
					gameState = Defs.eGameState.STATE_PLAYING;
				}

				theParticles.Update(dT);
			}
			else if (gameState == Defs.eGameState.STATE_PLAYING)
			{
				// check for end of level
				bool bZeroBricksLeft = true;
				for (int i=0; i<Defs.MAX_BRICKS; i++)
				{
					if (theBricks.Get(i).bIsActive)
					{
						bZeroBricksLeft = false;
						break;
					}
				}
				if (bZeroBricksLeft)
				{
					score += level * 10000;
					level++;
					thePlayer.Init();
					theBall.Init();
					theBricks.Init(level);
					readyTimer = Defs.READY_TIME;
					gameState = Defs.eGameState.STATE_READY;
				}

				// check for paddle collision with ball for the *next* frame
				float ballDistance, ballAngle, checkDistance, checkAngle;
				Vector3 ballPositionNextFrame = theBall.Get().position + theBall.Get().velocity * dT;
				Defs.CARTESIAN_TO_POLAR(ref ballPositionNextFrame, out ballDistance, out ballAngle);
				Defs.CARTESIAN_TO_POLAR(ref thePlayer.Get().position, out checkDistance, out checkAngle);

				// check for death
				if (ballDistance > (Defs.PLAYER_RADIUS + Defs.PLAYER_WIDTH))
				{
					// play a sound and spawn some death particles
					soundPlayer.Play((int)Defs.eSound.SND_DIE, 1.0f, false);
					Vector3 pos = thePlayer.Get().position;
					Vector3 offset;
					for (int p=0; p<30; p++)
					{
						offset.X = 2.0f - Defs.RANDOM_RANGE(4.0f);
						offset.Y = 0.0f;
						offset.Z = 2.0f - Defs.RANDOM_RANGE(4.0f);
						theParticles.Spawn(pos, offset, 2.0f + Defs.RANDOM_RANGE(2.0f), 1.0f + Defs.RANDOM_RANGE(2.0f), new ImageColor(255 - (int)Defs.RANDOM_RANGE(50.0f), 180 - (int)Defs.RANDOM_RANGE(50.0f), 0, 255));
					}

					// missed ball - check for game over
					if (--numShips == 0)
					{
						gameState = Defs.eGameState.STATE_GAMEOVER;
					}
					else
					{
						readyTimer = Defs.READY_TIME;
						gameState = Defs.eGameState.STATE_READY;
						thePlayer.Init();
						theBall.Init();
					}
				}

				// was there an actual collision?
				lastPaddleHitTime -= dT;
				if (lastPaddleHitTime < 0.0f)
					lastPaddleHitTime = 0.0f;
				if ((Math.Abs(checkAngle - ballAngle) < Defs.DEGREES_TO_RADIANS(Defs.PLAYER_ANGLE)) && ((checkDistance - ballDistance) < Defs.PLAYER_WIDTH))
				{
					// calculate the normal of the paddle
					Vector3 vecNormal = thePlayer.Get().position.Normalize();

					// did we hit on the top of the paddle?
					if (((checkDistance - ballDistance) < Defs.PLAYER_WIDTH) && (lastPaddleHitTime == 0.0f))
					{
						theBall.Get().velocity = theBall.Get().velocity.Reflect(vecNormal);
						soundPlayer.Play((int)Defs.eSound.SND_PADDLEHIT, 1.0f, false);
						lastPaddleHitTime = 0.25f;
					}
				}

				// check for ball collision with bricks
				for (int i=0; i<Defs.MAX_BRICKS; i++)
				{
					if (theBricks.Get(i).bIsActive)
					{
						Defs.CARTESIAN_TO_POLAR(ref ballPositionNextFrame, out ballDistance, out ballAngle);
						Defs.CARTESIAN_TO_POLAR(ref theBricks.Get(i).position, out checkDistance, out checkAngle);
						if ((Math.Abs(checkDistance - ballDistance) < Defs.BRICK_WIDTH) && (Math.Abs(checkAngle - ballAngle) < Defs.DEGREES_TO_RADIANS(Defs.BRICK_ANGLE)))
						{
							Vector3 vecNormal = theBricks.Get(i).position.Normalize();
							theBall.Get().velocity = theBall.Get().velocity.Reflect(vecNormal);
							soundPlayer.Play((int)Defs.eSound.SND_BRICKHIT, 1.0f, false);
							score += (int)(10.0f * (Defs.NUM_RINGS-theBricks.Get(i).value+1.0f));
							theBricks.Get(i).bIsActive = false;

							// spawn some particles
							Vector3 vel;
							float size = Defs.NUM_RINGS-theBricks.Get(i).value+1.0f;
							float colorScale = theBricks.Get(i).value;
							for (int p=0; p<4*(int)size; p++)
							{
								vel.X = 2.0f*size - Defs.RANDOM_RANGE(4.0f*size);
								vel.Y = 0.0f;
								vel.Z = 2.0f*size - Defs.RANDOM_RANGE(4.0f*size);
								theParticles.Spawn(theBricks.Get(i).position, vel, size*0.25f, 0.5f, new ImageColor((int)(255.0f*colorScale/Defs.NUM_RINGS), 0, (int)(255.0f*(Defs.NUM_RINGS-colorScale)/Defs.NUM_RINGS), 255));
							}

							// speed up as we hit bricks
							if (theBall.Get().velocity.Length() < Defs.BALL_SPEED_MAX)
							{
								theBall.Get().velocity *= 1.05f;
							}
						}
					}
				}

				// update
				thePlayer.Update(dT);
				theBall.Update(dT);
				theBricks.Update(dT);
				theParticles.Update(dT);
			}
		}
		
		public static void DrawModel(Vector3 position, Vector3 rotation, float scale, Defs.eModel which, Vector4 color)
		{
			Matrix4 matTemp1, matTemp2;

			matTemp1 = Matrix4.Transformation(position, new Vector3(scale, scale, scale));
			matTemp2 = Matrix4.RotationYxz(Defs.DEGREES_TO_RADIANS(rotation.X), Defs.DEGREES_TO_RADIANS(rotation.Y), Defs.DEGREES_TO_RADIANS(rotation.Z));
			Defs.matWorld = matTemp1 * matTemp2;
			matTemp1 = Defs.matProjection * Defs.matView;
			matTemp2 = matTemp1 * Defs.matWorld;

			// calculate light vector
			Vector3 vecTemp;
			matTemp1 = Defs.matWorld.InverseOrthonormal();
			vecTemp = matTemp1.TransformVector(Defs.vecGlobalLight);

			if (which == Defs.eModel.MODEL_PADDLE)
			{
				modelPaddle.Render(graphics, ref matTemp2, ref vecTemp, ref color);
			}
			else if (which == Defs.eModel.MODEL_BALL)
			{
				modelBall.Render(graphics, ref matTemp2, ref vecTemp, ref color);
			}
			else if (which == Defs.eModel.MODEL_BRICK)
			{
				modelBrick.Render(graphics, ref matTemp2, ref vecTemp, ref color);
			}
		}

		static void Render(float dT)
		{
			Matrix4 matTemp1, matTemp2;
			string str;

			Defs.matProjection = Matrix4.Perspective(Defs.DEGREES_TO_RADIANS(45.0f), (float)Defs.SCREEN_WIDTH / (float)Defs.SCREEN_HEIGHT, 0.1f, 10000.0f);
			Defs.matWorld = Matrix4.Identity;
			matTemp1 = Matrix4.RotationYxz(Defs.cameraRotation);
			matTemp2 = Matrix4.Translation(Defs.cameraPosition);
			Defs.matView = matTemp1 * matTemp2;

			graphics.SetViewport(0, 0, Defs.SCREEN_WIDTH, Defs.SCREEN_HEIGHT);
			graphics.SetClearColor(0.0f, 0.0f, 0.25f, 0.0f);
			graphics.SetClearDepth(1.0f);
			graphics.Clear();
	
			graphics.Disable(EnableMode.Blend);
			graphics.Enable(EnableMode.CullFace);
			graphics.SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);
			graphics.Enable(EnableMode.DepthTest);
			graphics.SetDepthFunc(DepthFuncMode.LEqual, true);

	
			if (gameState == Defs.eGameState.STATE_GAMEOVER)
			{
				hudText.Print(new ImagePosition(-1, 120), new ImageColor(255, 255, 0, ((int)stopwatch.ElapsedMilliseconds)%255), "Smash the Bricks!", true, 0, true);
				hudText.Print(new ImagePosition(-1, 300), new ImageColor(255, 255, 255, 255), "Press START to play", true, 1, false);
				hudText.Print(new ImagePosition(-1, 340), new ImageColor(0, 255, 128, 255), "Touch the screen or use the left analog to move your paddle", true, 2, false);
				hudText.Print(new ImagePosition(-1, 380), new ImageColor(0, 255, 128, 255), "Avoid missing ball for high score", true, 3, false);
				hudText.Print(new ImagePosition(-1, 200), new ImageColor(255, 0, 0, ((int)stopwatch.ElapsedMilliseconds)%750), "GAME OVER!", true, 4, true);
				hudText.SetRender(0, true);
				hudText.SetRender(1, true);
				hudText.SetRender(2, true);
				hudText.SetRender(3, true);
				hudText.SetRender(4, true);

				theParticles.Render();
			}
			else if (gameState == Defs.eGameState.STATE_READY)
			{
				hudText.SetRender(0, false);
				hudText.SetRender(1, false);
				hudText.SetRender(2, false);
				hudText.SetRender(3, false);
				hudText.SetRender(4, false);

				hudBlinkTimer += dT;
				if (hudBlinkTimer < 0.25f)
				{
					hudText.Print(new ImagePosition(-1, 210), new ImageColor(255, 255, 0, 255), "Get ready!", true, 10, false);
				}
				if (hudBlinkTimer >= 0.5f)
				{
					hudBlinkTimer = 0.0f;
				}

				str = String.Format("{0}", (int)(readyTimer+0.99f));
				hudText.Print(new ImagePosition(-1, 240), new ImageColor(0, 255, 0, 255), str, true, 11, true);

				hudText.SetRender(10, true);
				hudText.SetRender(11, true);

				theBall.Render();
				theBricks.Render();
				thePlayer.Render();
				theParticles.Render();
			}
			else if (gameState == Defs.eGameState.STATE_PLAYING)
			{
				hudText.SetRender(10, false);
				hudText.SetRender(11, false);
				theBall.Render();
				theBricks.Render();
				thePlayer.Render();
				theParticles.Render();
			}
		
			// render the HUD
			str = String.Format("Score: {0:d8}", score);
			hudText.Print(new ImagePosition(20, 20), new ImageColor(255, 204, 0, 255), str, true, 20, true);

			str = String.Format("Balls: {0}", numShips);
			hudText.Print(new ImagePosition(380, 20), new ImageColor(255, 204, 0, 255), str, true, 21, true);

			str = String.Format("Stage: {0:d2}", level);
			hudText.Print(new ImagePosition(710, 20), new ImageColor(255, 204, 0, 255), str, true, 22, true);

//			str = String.Format("{0}", (int)(dT*1000.0f));
//			hudText.Print(new ImagePosition(20, 430), new ImageColor(255, 255, 255, 255), str, true);

			hudText.Render();
			graphics.SwapBuffers();
		}
	}
}