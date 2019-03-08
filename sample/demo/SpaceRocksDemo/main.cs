/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;

namespace Demo_SpaceRocks
{
	public class SpaceRocksProgram
	{
		static Stopwatch				stopwatch = new Stopwatch();
		static public GraphicsContext	graphics = new GraphicsContext();
		static ModelBullet				modelBullet;
		static ModelRock				modelRock;
		static ModelShip				modelShip;
		static SoundSystem				soundPlayer;

		static int						level, score, numShips;
		//static float					hudBlinkTimer;
		static Defs.eGameState			gameState;
		static bool						bQuit;
		static bool						bEngineSoundIsPlaying;
			
		static public PlayerClass		thePlayer;
		static RockClass				theRocks;
		static BulletClass				theBullets;
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
			modelBullet = new ModelBullet();
			modelRock = new ModelRock();
			modelShip = new ModelShip();
			soundPlayer = new SoundSystem();
	
			thePlayer = new PlayerClass();
			theRocks = new RockClass();
			theBullets = new BulletClass();
			theParticles = new ParticleSystemClass();

			hudText = new TextPlane(30);

			thePlayer.Init();
			gameState = Defs.eGameState.STATE_GAMEOVER;
			bQuit = false;
			bEngineSoundIsPlaying = false;
		}
	
		static void Shutdown()
		{
			stopwatch.Stop();
			hudText.Dispose();
			modelBullet.Dispose();
			modelShip.Dispose();
			modelRock.Dispose();
			theParticles.Dispose();
		}

		static void Update(float dT)
		{
			GamePadData padData = GamePad.GetData(0);

			if (gameState == Defs.eGameState.STATE_GAMEOVER)
			{
				if ((padData.ButtonsDown & GamePadButtons.Start) != 0)
				{
					level = 1;
					score = 0;
					numShips = 3;
					thePlayer.Init();
					theBullets.Init();
					theRocks.Init(level);
					gameState = Defs.eGameState.STATE_PLAYING;
				}

				theParticles.Update(dT);
			}
			else if (gameState == Defs.eGameState.STATE_PLAYING)
			{
				// check for end of level
				bool bZeroRocksLeft = true;
				for (int i=0; i<Defs.MAX_ROCKS; i++)
				{
					if (theRocks.Get(i).bIsActive)
					{
						bZeroRocksLeft = false;
						break;
					}
				}
				if (bZeroRocksLeft)
				{
					score += level * 10000;
					level++;
					thePlayer.Init();
					theBullets.Init();
					theRocks.Init(level);
				}
		
				// fire
				if ((padData.ButtonsDown & GamePadButtons.Cross) != 0)
				{
					soundPlayer.Play((int)Defs.eSound.SND_SHOOT, 0.25f, false);	// pitch 4X
					theBullets.Spawn(thePlayer.Get().position, thePlayer.Get().rotation.Y);
	
				}
		
				// move the player with D-pad or analog
				float dir = 0.0f;
				float x = padData.AnalogLeftX;
				float y = padData.AnalogLeftY;
				if ( (Math.Abs(x) != 0.0f) || (Math.Abs(y) != 0.0f) )
				{
					thePlayer.Get().rotation.Y = Defs.RADIANS_TO_DEGREES((float)Math.Atan2(y, x)) + 90.0f;
				}
				else
				{
					if ((padData.Buttons & GamePadButtons.Left) != 0)
					{
						dir = -1.0f;
					}
					if ((padData.Buttons & GamePadButtons.Right) != 0)
					{
						dir = 1.0f;
					}
					thePlayer.Get().rotation.Y = thePlayer.Get().rotation.Y + dir * dT * 250.0f;
				}

				// wrap rotation
				if (thePlayer.Get().rotation.Y < 0.0f)
					thePlayer.Get().rotation.Y += 360.0f;
				if (thePlayer.Get().rotation.Y >= 360.0f)
					thePlayer.Get().rotation.Y -= 360.0f;
				
				float thrust = 0.0f;
				if ((padData.Buttons & GamePadButtons.R) != 0)
				{
					thrust = Defs.SHIP_THRUST;
					if (!bEngineSoundIsPlaying)
					{
						bEngineSoundIsPlaying = true;
						soundPlayer.Play((int)Defs.eSound.SND_ENGINELOOP, 0.5f, true);
					}
				}
				else
				{
					if (bEngineSoundIsPlaying)
					{
						soundPlayer.Stop((int)Defs.eSound.SND_ENGINELOOP);
						bEngineSoundIsPlaying = false;
					}
				}
				thePlayer.Get().acceleration.X = (float)Math.Sin(Defs.DEGREES_TO_RADIANS(thePlayer.Get().rotation.Y)) * thrust;
				thePlayer.Get().acceleration.Z = (float)Math.Cos(Defs.DEGREES_TO_RADIANS(thePlayer.Get().rotation.Y)) * thrust;

				// engine exhaust particles
				if (thrust > 0.0f)
				{
					Vector3 pos;
					Vector3 offset = new Vector3(0.0f, 0.0f, -thePlayer.Get().scale);

					for (int p = 0; p < 4; p++)
					{
						pos = thePlayer.Get().position;
						offset.X = 0.1f - Defs.RANDOM_RANGE(0.2f);
						pos.X += offset.X * (float)Math.Cos(Defs.DEGREES_TO_RADIANS(thePlayer.Get().rotation.Y));
						pos.Z += offset.X * (float)Math.Sin(Defs.DEGREES_TO_RADIANS(thePlayer.Get().rotation.Y));
						pos.X += offset.Z * (float)Math.Sin(Defs.DEGREES_TO_RADIANS(thePlayer.Get().rotation.Y));
						pos.Z += offset.Z * (float)Math.Cos(Defs.DEGREES_TO_RADIANS(thePlayer.Get().rotation.Y));
						theParticles.Spawn(pos, -0.25f * thePlayer.Get().acceleration, 0.6f, 0.2f, new ImageColor(255 - (int)Defs.RANDOM_RANGE(50.0f), 255 - (int)Defs.RANDOM_RANGE(50.0f), 0, 255));
					}
				}
		
				thePlayer.Update(dT);
				theBullets.Update(dT);
				theRocks.Update(dT);
				theParticles.Update(dT);

				// check for player collision with rocks
				if (!thePlayer.IsInvincible())
				{
					for (int i=0; i<Defs.MAX_ROCKS; i++)
					{
						if (theRocks.Get(i).bIsActive)
						{
							float rockDist = theRocks.Get(i).position.DistanceSquared(thePlayer.Get().position);
							if (rockDist < theRocks.Get(i).scale*theRocks.Get(i).scale + thePlayer.Get().scale*thePlayer.Get().scale)
							{
								// kill the rock and play a sound
								theRocks.Get(i).bIsActive = false;
								soundPlayer.Play((int)Defs.eSound.SND_DIE, 0.5f, false);

								// spawn some death particles
								Vector3 pos = thePlayer.Get().position;
								Vector3 offset;
								for (int p=0; p<30; p++)
								{
									offset.X = 2.0f - Defs.RANDOM_RANGE(4.0f);
									offset.Y = 0.0f;
									offset.Z = 2.0f - Defs.RANDOM_RANGE(4.0f);
									theParticles.Spawn(pos, offset, 0.5f + Defs.RANDOM_RANGE(1.0f), 1.0f + Defs.RANDOM_RANGE(2.0f), new ImageColor(255 - (int)Defs.RANDOM_RANGE(50.0f), 180 - (int)Defs.RANDOM_RANGE(50.0f), 0, 255));
								}

								// check for game over
								if (--numShips == 0)
								{
									//hudBlinkTimer = 0.0f;
									if (bEngineSoundIsPlaying)
									{
										soundPlayer.Stop((int)Defs.eSound.SND_ENGINELOOP);
										bEngineSoundIsPlaying = false;
									}
									gameState = Defs.eGameState.STATE_GAMEOVER;
								}
								else
								{
									thePlayer.Init();
								}

								break;
							}
						}
					}
				}
		
				// check for bullet collision
				float bulletDist;
				for (int j=0; j<Defs.MAX_BULLETS; j++)
				{
					// check for player collision first
					if (theBullets.Get(j).bIsActive)
					{
						// check for rock collisions with this bullet
						for (int i=0; i<Defs.MAX_ROCKS; i++)
						{
							if (theRocks.Get(i).bIsActive)
							{
								bulletDist = theRocks.Get(i).position.DistanceSquared(theBullets.Get(j).position);
								if (bulletDist < theBullets.Get(j).scale*theBullets.Get(j).scale + theRocks.Get(i).scale*theRocks.Get(i).scale)
								{
			                        soundPlayer.Play((int)Defs.eSound.SND_ROCKHIT, 0.25f, false);	// pitch (1.0f + MAX_ROCK_SIZE - rocks[i].size)
									score += (int)((3.0f - theRocks.Get(i).scale) * 100.0f);

									// spawn the debris
									float size = theRocks.Get(i).scale * 0.5f;
									int num = (int)((Defs.MAX_ROCK_SIZE - size) * 3.0f);
									Vector3 vel;
									for (int k=0; k<num; k++)
									{
										if (size > 0.2f)
										{
											theRocks.Spawn(size, theRocks.Get(i).position);
										}

										// spawn some particles
										for (int p=0; p<3*(num+1); p++)
										{
											vel.X = 4.0f*size - Defs.RANDOM_RANGE(8.0f*size);
											vel.Y = 0.0f;
											vel.Z = 4.0f*size - Defs.RANDOM_RANGE(8.0f*size);
											theParticles.Spawn(theRocks.Get(i).position, vel, size, 0.5f, new ImageColor(255 - (int)Defs.RANDOM_RANGE(50.0f), 180 - (int)Defs.RANDOM_RANGE(80.0f), 0, 255));
										}
									}
									theRocks.Get(i).bIsActive = false;
									theBullets.Get(j).bIsActive = false;
									break;
								}
							}
						}
					}
				}
			}
		}
		
		public static void DrawModel(Vector3 position, Vector3 rotation, float scale, Defs.eModel which)
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

			if (which == Defs.eModel.MODEL_BULLET)
			{
				modelBullet.Render(graphics, ref matTemp2, ref vecTemp);
			}
			else if (which == Defs.eModel.MODEL_ROCK)
			{
				modelRock.Render(graphics, ref matTemp2, ref vecTemp);
			}
			else if (which == Defs.eModel.MODEL_SHIP)
			{
				modelShip.Render(graphics, ref matTemp2, ref vecTemp);
			}
		}

		static void Render(float dT)
		{
			Matrix4 matTemp1, matTemp2;

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

			//hudText.Clear();
	
			if (gameState == Defs.eGameState.STATE_GAMEOVER)
			{
				hudText.SetRender(0, true);
				hudText.SetRender(1, true);
				hudText.SetRender(2, true);
				hudText.SetRender(3, true);
				hudText.SetRender(4, true);
				hudText.SetRender(5, true);
				hudText.SetRender(10, true);

				hudText.Print(new ImagePosition(-1, 120), new ImageColor(255, 255, 0, ((int)stopwatch.ElapsedMilliseconds)%255), "Rocks in Space!", true, 0, true);
				hudText.Print(new ImagePosition(-1, 240), new ImageColor(255, 255, 255, 255), "Press START to play", true, 1, false);
				hudText.Print(new ImagePosition(-1, 280), new ImageColor(0, 255, 128, 255), "Rotate ship = D-pad or left analog stick", true, 2, false);
				hudText.Print(new ImagePosition(-1, 320), new ImageColor(0, 255, 192, 255), "Fire = X Button", true, 3, false);
				hudText.Print(new ImagePosition(-1, 360), new ImageColor(0, 255, 255, 255), "Thrust = R Button", true, 4, false);
				hudText.Print(new ImagePosition(-1, 400), new ImageColor(255, 255, 0, 255), "Hint: You are invincible while flashing", true, 5, false);
				hudText.Print(new ImagePosition(-1, 200), new ImageColor(255, 0, 0, ((int)stopwatch.ElapsedMilliseconds)%750), "GAME OVER!", true, 10, true);

				theParticles.Render();
			}
			else if (gameState == Defs.eGameState.STATE_PLAYING)
			{
				hudText.SetRender(0, false);
				hudText.SetRender(1, false);
				hudText.SetRender(2, false);
				hudText.SetRender(3, false);
				hudText.SetRender(4, false);
				hudText.SetRender(5, false);
				hudText.SetRender(10, false);

				theBullets.Render();
				theRocks.Render();
				thePlayer.Render();
				theParticles.Render();
			}

			// render the HUD
			string str;
			str = String.Format("Score: {0:d8}", score);
			hudText.Print(new ImagePosition(20, 20), new ImageColor(255, 204, 0, 255), str, true, 20, true);

			str = String.Format("Ships: {0}", numShips);
			hudText.Print(new ImagePosition(380, 20), new ImageColor(255, 204, 0, 255), str, true, 21, true);

			str = String.Format("Stage: {0:d2}", level);
			hudText.Print(new ImagePosition(710, 20), new ImageColor(255, 204, 0, 255), str, true, 22, true);

//			str = String.Format("{0}ms", (int)(dT*1000.0f));
//			hudText.Print(new ImagePosition(20, 430), new ImageColor(255, 255, 255, 255), str, true);

			hudText.Render();

			graphics.SwapBuffers();
		}
	}
}