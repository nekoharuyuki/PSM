/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// TODO
// - arrow lighting still looks wrong
// - drop shadow on font is broken
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

namespace Demo_BallMaze
{
	public class BallMazeProgram
	{
		static public Stopwatch stopwatch = new Stopwatch();
		static public GraphicsContext graphics = new GraphicsContext();
		static public ModelArrow modelArrow;
		static public ModelBall modelBall;
		static public ModelCube modelCube;
		static public ModelPlane modelPlane;
		static public SoundSystem soundPlayer;

		static public Defs.eGameState gameState;
		static bool bQuit;

		static public PlayerClass thePlayer;
		static ArrowClass theArrow;
		static MazeClass theMaze;
		static PlaneClass thePlane;
		static StarfieldClass theStarfield;
		static TextPlane hudText;

		// the cubes used to construct the maze
		static int			cubeCount;				// how many total cubes there are in this maze

		// camera stuff
		static public bool	bZoomMode;

		static public float	currentGameTime, lastGameTime, bestGameTime;
		static public bool	bHitPenalty, bWinner;
		static public float	countdownTimer, penaltyTimer;

		static void UpdateLight(bool bOverridePos, Vector3 newPos)
		{
			// update light
			if (bOverridePos)
			{
				Defs.vecGlobalLight = newPos;
			}
			else
			{
                Defs.vecGlobalLight = thePlayer.Get().position + new Vector3(0.0f, Defs.CAMERA_HEIGHT*0.5f, 0.0f);
			}
		}

		//
		// Code to generate a random square maze with a guaranteed solution via recursive division
		//
		// Begin with a rectangular space with no walls.
		// Build at random points two walls that are perpendicular to each other.
		// These two walls divide the large chamber into four smaller chambers separated by four walls.
		// Choose three of the four walls at random, and open a one cell-wide hole at a random point in each of the three.
		// Continue in this manner recursively, until every chamber has a width of one cell in either of the two directions.
		//
		static Defs.eBlockType[,] Maze = new Defs.eBlockType[Defs.MAZE_SIZE,Defs.MAZE_SIZE];	// a grid of cells that are either occupied or not

		static int GetWallHole(int min, int max)
		{
			int newHole = min;

			// only calculate if there is room
			if (max > min)
			{
				newHole = min + ((int)Defs.RANDOM_RANGE(max - min));
				if (((newHole % 2) == 0) && (newHole < max))
					newHole++;
			}
			return newHole;
		}

		static void MazeSubdivide(int left, int top, int right, int bottom)
		{
			int i, j, wallX, wallY, noHoleWall;

			// check for a narrow hall and return
			if ((right-left < 0) || (bottom-top < 0))
			{
				return;
			}

			// create two perpendicular walls at random points

			// special case for hallways that are only wide enough for one possible wall
			if (right == left)
			{
				wallX = left;
			}
			else
			{
				wallX = left + ((int)Defs.RANDOM_RANGE(right - left));

				// make sure the walls are on even boundaries to leave room for the corridors
				if (((wallX % 2) != 0) && (wallX < right))
					wallX++;
			}

			if (bottom == top)
			{
				wallY = top;
			}
			else
			{
				wallY = top + ((int)Defs.RANDOM_RANGE(bottom - top));

				// make sure the walls are on even boundaries to leave room for the corridors
				if (((wallY % 2) != 0) && (wallY < bottom))
					wallY++;
			}

			// draw the walls into the maze
			for (i=left-1; i<=right+1; i++)
			{
                Maze[i,wallY] = Defs.eBlockType.MAZE_INNERWALL;
			}
			for (j=top-1; j<=bottom+1; j++)
			{
                Maze[wallX,j] = Defs.eBlockType.MAZE_INNERWALL;
			}

			// pick one wall at random to NOT poke a hole in
			noHoleWall = (int)Defs.RANDOM_RANGE(4.0f);

			// put a hole at a random position in the remaining 3 walls
			Maze[GetWallHole(left-1, wallX-1),wallY] = (noHoleWall == 0) ? Defs.eBlockType.MAZE_INNERWALL : Defs.eBlockType.MAZE_EMPTY;
			Maze[GetWallHole(wallX+1, right+1),wallY] = (noHoleWall == 1) ? Defs.eBlockType.MAZE_INNERWALL : Defs.eBlockType.MAZE_EMPTY;
			Maze[wallX,GetWallHole(top-1, wallY-1)] = (noHoleWall == 2) ? Defs.eBlockType.MAZE_INNERWALL : Defs.eBlockType.MAZE_EMPTY;
			Maze[wallX,GetWallHole(wallY+1, bottom+1)] = (noHoleWall == 3) ? Defs.eBlockType.MAZE_INNERWALL : Defs.eBlockType.MAZE_EMPTY;

			// recursively subdivide each new room
			MazeSubdivide(left, top, wallX-2, wallY-2);
			MazeSubdivide(wallX+2, top, right, wallY-2);
			MazeSubdivide(left, wallY+2, wallX-2, bottom);
			MazeSubdivide(wallX+2, wallY+2, right, bottom);
		}

		static void GenerateMaze()
		{
			int i, j, rndX, rndY;

			// reset the ball parameters
			bWinner = false;

			// empty the entire maze
			for (j=0; j<Defs.MAZE_SIZE; j++)
			{
				for (i=0; i<Defs.MAZE_SIZE; i++)
				{
					Maze[i,j] = (int)Defs.eBlockType.MAZE_EMPTY;
				}
			}

			// create the outer boundary walls
			for (i=0; i<Defs.MAZE_SIZE; i++)
			{
				Maze[i,0] = Defs.eBlockType.MAZE_OUTERWALL;
				Maze[i,(Defs.MAZE_SIZE-1)] = Defs.eBlockType.MAZE_OUTERWALL;
				Maze[0,i] = Defs.eBlockType.MAZE_OUTERWALL;
				Maze[(Defs.MAZE_SIZE-1),i] = Defs.eBlockType.MAZE_OUTERWALL;
			}

			// subdivide the maze recursively
			MazeSubdivide(2, 2, Defs.MAZE_SIZE-3, Defs.MAZE_SIZE-3);

			// add the start and finish blocks
			Maze[1,1] = Defs.eBlockType.MAZE_STARTPAD;
			Maze[(Defs.MAZE_SIZE-2),(Defs.MAZE_SIZE-2)] = Defs.eBlockType.MAZE_FINISHPAD;

			// randomly create some penalty blocks
			for (i=0; i<Defs.NUM_PENALTY_BLOCKS; i++)
			{
				rndX = 1 + (int)Defs.RANDOM_RANGE(Defs.MAZE_SIZE-3);
				rndY = 1 + (int)Defs.RANDOM_RANGE(Defs.MAZE_SIZE-3);

				// make sure the walls are on even boundaries to leave room for the corridors
				if ((rndX % 2) != 0)
					rndX++;
				if ((rndY % 2) != 0)
					rndY++;

				// place the block if there is a wall there, otherwise, pick another spot
				if (Maze[rndX,rndY] >= Defs.eBlockType.MAZE_INNERWALL)
					Maze[rndX,rndY] = Defs.eBlockType.MAZE_PENALTYWALL;
				else
					i--;
			}

			// setup the cube array
			cubeCount = 0;
			for (j=0; j<Defs.MAZE_SIZE; j++)
			{
				for (i=0; i<Defs.MAZE_SIZE; i++)
				{
					if (Maze[i,j] > Defs.eBlockType.MAZE_EMPTY)
					{
						theMaze.Get(i,j).bIsActive = true;
						theMaze.Get(i,j).type = Maze[i,j];

						// start all of the cubes up in the air randomly to drop at start
						theMaze.Get(i,j).position = new Vector3((float)(i-(Defs.MAZE_SIZE/2)), Defs.CUBE_FALL_DIST + 10.0f - Defs.RANDOM_RANGE(20.0f), (float)(j-(Defs.MAZE_SIZE/2)));
						cubeCount++;
					}
				}
			}

			UpdateLight(false, Vector3.Zero);
		}

		static ulong CheckCollision(Vector3 checkPos, float size)
		{
			int i, j, checkX, checkZ;
			ulong hit = 0;

			for (j=-1; j<=1; j++)
			{
				for (i=-1; i<=1; i++)
				{
					checkX = (int)(checkPos.X + Defs.MAZE_SIZE*0.5f + i*size);
					checkZ = (int)(checkPos.Z + Defs.MAZE_SIZE*0.5f + j*size);
					if(checkX > Defs.MAZE_SIZE - 1){
						checkX = Defs.MAZE_SIZE - 1;
					}
					if(checkX < 0){
						checkX = 0;
					}

					if(checkZ > Defs.MAZE_SIZE - 1){
						checkZ = Defs.MAZE_SIZE - 1;
					}

					if(checkZ < 0){
						checkZ = 0;
					}
					if (Maze[checkX,checkZ] >= Defs.eBlockType.MAZE_INNERWALL)
					{
						// 2 bits for collision area = type of wall hit
						hit |= ((ulong)(Maze[checkX,checkZ] - Defs.eBlockType.MAZE_FINISHPAD)) << (((i+1)+3*(j+1))*2);
					}
				}
			}

			return hit;
		}

		static Defs.eBlockType GetMazeTile(Vector3 checkPos)
		{
			int checkX = (int)(checkPos.X + Defs.MAZE_SIZE*0.5f);
			int checkZ = (int)(checkPos.Z + Defs.MAZE_SIZE*0.5f);
			return (Maze[checkX,checkZ]);
		}

		static void UpdateViewMatrix()
		{
			if (Defs.cameraRotation.X <= -360.0f)
				Defs.cameraRotation.X += 360.0f;
			if (Defs.cameraRotation.X >= 360.0f)
				Defs.cameraRotation.X -= 360.0f;
			if (Defs.cameraRotation.Y <= -360.0f)
				Defs.cameraRotation.Y += 360.0f;
			if (Defs.cameraRotation.Y >= 360.0f)
				Defs.cameraRotation.Y -= 360.0f;
			if (Defs.cameraRotation.Z <= -360.0f)
				Defs.cameraRotation.Z += 360.0f;
			if (Defs.cameraRotation.Z >= 360.0f)
				Defs.cameraRotation.Z -= 360.0f;

			Defs.matView	= Matrix4.Identity;
			Matrix4 trans   = Matrix4.Translation(Defs.cameraPosition);	// set the translation
			Matrix4 rotX    = Matrix4.RotationX(Defs.DEGREES_TO_RADIANS(Defs.cameraRotation.X));	// X rotation  
			Matrix4 rotY    = Matrix4.RotationY(Defs.DEGREES_TO_RADIANS(Defs.cameraRotation.Y));	// Y rotation 
			Matrix4 rotZ    = Matrix4.RotationZ(Defs.DEGREES_TO_RADIANS(Defs.cameraRotation.Z));	// Z rotation 
			Defs.matView	= rotX * rotY * rotZ * trans;				// multiply them all together, but translate independent of rotation
		}

		static void UpdateInput(float dT)
		{
			Vector3 smoothViewPos = Vector3.Zero;
			Vector3 smoothViewAng = Vector3.Zero;
			Vector3 offsetPos = Vector3.Zero;
			Vector3 offsetAng = Vector3.Zero;

			GamePadData padData = GamePad.GetData(0);
			MotionData motionData = Motion.GetData(0);
			List<TouchData> touchDataList = Touch.GetData(0);

			// compensate for any bias in accelerometer if it's active - should be an auto calibration
			if ((motionData.Acceleration.X != 0.0f) && (motionData.Acceleration.Y != 0.0f))
			{
				motionData.Acceleration.Y += Defs.BALL_PAD_Y_OFFS;
			}

			// main state machine
			if (gameState == Defs.eGameState.STATE_ATTRACT)		// just display the stars with the attract messages
			{
				// use the controller tilt to change the view angle for the starfield
				Vector3 starMove;
				starMove.X = 90.0f - motionData.Acceleration.X * 3.0f;
				starMove.Y = 0.0f;
				starMove.Z = -motionData.Acceleration.Y * 3.0f;
				smoothViewAng = starMove;

				foreach (TouchData touch in touchDataList)
				{
					if (touch.Status == TouchStatus.Down)
					{
						theMaze.Clear();
						GenerateMaze();
						thePlayer.Init();
						countdownTimer = 2.99f;
						gameState = Defs.eGameState.STATE_READY;
						break;
					}
				}
			}
			else if (gameState == Defs.eGameState.STATE_READY)	// ready countdown timer - blocks falling into place
			{
				float lastCountdownTimer = countdownTimer+0.01f;
				countdownTimer -= dT;

				// blip every number
				if ((int)countdownTimer != (int)lastCountdownTimer)
				{
					soundPlayer.Play(Defs.eSound.SND_READY1_BLIP, 1.0f, false);
				}

				if (countdownTimer <= 0.0f)
				{
					Defs.eSound handle;
					currentGameTime = 0.0f;
					countdownTimer = 0.0f;
					handle = soundPlayer.Play(Defs.eSound.SND_READY1_BLIP, 1.0f, false);
					soundPlayer.SetPitch(handle, 2.0f);
					handle = soundPlayer.Play(Defs.eSound.SND_READY2_BLIP, 1.0f, false);
					soundPlayer.SetPitch(handle, 4.0f);
					gameState = Defs.eGameState.STATE_PLAY;
				}
				else if (countdownTimer <= 1.0f)
				{
					bZoomMode = false;
				}
			}
			else if (gameState == Defs.eGameState.STATE_FINISH)	// circle the camera and display the score
			{
				if (SoundSystem.ballLoopSoundHandle != Defs.eSound.SND_MAX)
				{
					soundPlayer.Stop(SoundSystem.ballLoopSoundHandle);
					SoundSystem.ballLoopSoundHandle = Defs.eSound.SND_MAX;
				}

				foreach (TouchData touch in touchDataList)
				{
					if (touch.Status == TouchStatus.Down)
					{
						gameState = Defs.eGameState.STATE_ATTRACT;
						theMaze.Clear();
						break;
					}
				}
			}
			else if (gameState == Defs.eGameState.STATE_PLAY)	// normal play state
			{
				bool bHitSound = false;

				// time goes faster when zoomed
				currentGameTime += dT;
				if (bZoomMode)
					currentGameTime += 1.5f*dT;

				// add a time penalty if necessary
				if (penaltyTimer > 0.0f)
				{
					penaltyTimer -= dT;
					if (penaltyTimer <= 0.0f)
						penaltyTimer = 0.0f;
					if (bHitPenalty)
					{
						currentGameTime += 5.0f;
						bHitPenalty = false;
					}
				}

				// check for maze completion
				if (GetMazeTile(thePlayer.Get().position) == Defs.eBlockType.MAZE_FINISHPAD)
				{
					lastGameTime = currentGameTime;
					if (lastGameTime < bestGameTime)
					{
						bWinner = true;
						bestGameTime = lastGameTime;
					}
					currentGameTime = 0.0f;
					bZoomMode = true;
					gameState = Defs.eGameState.STATE_FINISH;
					return;
				}

				// check for too much time (5 minutes should be enough)
				if (currentGameTime >= Defs.OUT_OF_TIME)
				{
					lastGameTime = Defs.OUT_OF_TIME;
					currentGameTime = 0.0f;
					bZoomMode = true;
					gameState = Defs.eGameState.STATE_FINISH;
					return;
				}

				// zoom out to view entire maze
				bZoomMode = false;
				foreach (TouchData touch in touchDataList)
				{
					if (touch.Status == TouchStatus.Move)
					{
						bZoomMode = true;
						break;
					}
				}

				thePlayer.Get().acceleration = Vector3.Zero;

				// use the tilt sensor to steer the ball
				float tiltX = 0.0f;
				float tiltZ = 0.0f;
				
				// add a dead zone
				if ((float)Math.Abs(motionData.Acceleration.X) > Defs.BALL_PAD_DEAD_ZONE)
					tiltX = motionData.Acceleration.X;
				if ((float)Math.Abs(motionData.Acceleration.Y) > Defs.BALL_PAD_DEAD_ZONE)
					tiltZ = -motionData.Acceleration.Y;

				// debug use keys
				{
					if ((padData.Buttons & GamePadButtons.Left) != 0)
						tiltX = -1.0f;
					if ((padData.Buttons & GamePadButtons.Right) != 0)
						tiltX = 1.0f;
					if ((padData.Buttons & GamePadButtons.Up) != 0)
						tiltZ = -1.0f;
					if ((padData.Buttons & GamePadButtons.Down) != 0)
						tiltZ = 1.0f;
				}

				thePlayer.Get().acceleration.X = tiltX * Defs.BALL_PAD_SENS;
				thePlayer.Get().acceleration.Z = tiltZ * Defs.BALL_PAD_SENS;

				// update the ball velocity
				thePlayer.Get().velocity += thePlayer.Get().acceleration * dT;

				// add fake ball friction
				thePlayer.Get().velocity *= Defs.BALL_FRICTION;

				// no Y movement
				thePlayer.Get().velocity.Y = 0.0f;

				// clamp the velocity
				if (thePlayer.Get().velocity.X > Defs.BALL_MAX_VELOCITY)
					thePlayer.Get().velocity.X = Defs.BALL_MAX_VELOCITY;
				if (thePlayer.Get().velocity.X < -Defs.BALL_MAX_VELOCITY)
					thePlayer.Get().velocity.X = -Defs.BALL_MAX_VELOCITY;
				if (thePlayer.Get().velocity.Z > Defs.BALL_MAX_VELOCITY)
					thePlayer.Get().velocity.Z = Defs.BALL_MAX_VELOCITY;
				if (thePlayer.Get().velocity.Z < -Defs.BALL_MAX_VELOCITY)
					thePlayer.Get().velocity.Z = -Defs.BALL_MAX_VELOCITY;

				// clamp to zero
				if (Math.Abs(thePlayer.Get().velocity.X) < Defs.BALL_MIN_VELOCITY)
					thePlayer.Get().velocity.X = 0.0f;
				if (Math.Abs(thePlayer.Get().velocity.Z) < Defs.BALL_MIN_VELOCITY)
					thePlayer.Get().velocity.Z = 0.0f;

				// check for potential maze collision
				ulong hitBits = 0;
				Vector3 newBallPos = thePlayer.Get().position + 0.5f * thePlayer.Get().velocity * dT;

				// check in every direction
				hitBits = CheckCollision(newBallPos, Defs.BALL_SCALE*0.5f);

				// invert and damp any velocities if we are going to hit directly
				if (((hitBits & (ulong)Defs.eCollisionBits.COLL_L) != 0) && (thePlayer.Get().velocity.X < 0.0f))	// left
				{
					bHitSound = true;
					thePlayer.Get().velocity.X = -thePlayer.Get().velocity.X * Defs.BALL_BOUNCE_DAMP;
					if ((penaltyTimer == 0.0f) && ((hitBits & (ulong)Defs.eCollisionBits.COLL_L) == (ulong)Defs.eCollisionBits.COLL_L))		// penalty wall
					{
						bHitPenalty = true;
						penaltyTimer = 2.0f;
					}
				}
				if (((hitBits & (ulong)Defs.eCollisionBits.COLL_R) != 0) && (thePlayer.Get().velocity.X > 0.0f))	// right
				{
					bHitSound = true;
					thePlayer.Get().velocity.X = -thePlayer.Get().velocity.X * Defs.BALL_BOUNCE_DAMP;
					if ((penaltyTimer == 0.0f) && ((hitBits & (ulong)Defs.eCollisionBits.COLL_R) == (ulong)Defs.eCollisionBits.COLL_R))		// penalty wall
					{
						bHitPenalty = true;
						penaltyTimer = 2.0f;
					}
				}
				if (((hitBits & (ulong)Defs.eCollisionBits.COLL_U) != 0) && (thePlayer.Get().velocity.Z < 0.0f))	// up
				{
					bHitSound = true;
					thePlayer.Get().velocity.Z = -thePlayer.Get().velocity.Z * Defs.BALL_BOUNCE_DAMP;
					if ((penaltyTimer == 0.0f) && ((hitBits & (ulong)Defs.eCollisionBits.COLL_U) == (ulong)Defs.eCollisionBits.COLL_U))		// penalty wall
					{
						bHitPenalty = true;
						penaltyTimer = 2.0f;
					}
				}
				if (((hitBits & (ulong)Defs.eCollisionBits.COLL_B) != 0) && (thePlayer.Get().velocity.Z > 0.0f))	// down
				{
					bHitSound = true;
					thePlayer.Get().velocity.Z = -thePlayer.Get().velocity.Z * Defs.BALL_BOUNCE_DAMP;
					if ((penaltyTimer == 0.0f) && ((hitBits & (ulong)Defs.eCollisionBits.COLL_B) == (ulong)Defs.eCollisionBits.COLL_B))		// penalty wall
					{
						bHitPenalty = true;
						penaltyTimer = 2.0f;
					}
				}

				// re-check in every direction with updated position
				newBallPos = thePlayer.Get().position + 0.5f * thePlayer.Get().velocity * dT;
				hitBits = CheckCollision(newBallPos, Defs.BALL_SCALE*0.5f);

				// check for corner collisions
				if (((hitBits & (ulong)(Defs.eCollisionBits.COLL_UL | Defs.eCollisionBits.COLL_BL)) != 0) && (thePlayer.Get().velocity.X < 0.0f))	// left
				{
					bHitSound = true;
					thePlayer.Get().velocity.X = -thePlayer.Get().velocity.X * Defs.BALL_BOUNCE_DAMP;
				}
				if (((hitBits & (ulong)(Defs.eCollisionBits.COLL_UR | Defs.eCollisionBits.COLL_BR)) != 0) && (thePlayer.Get().velocity.X > 0.0f))	// right
				{
					bHitSound = true;
					thePlayer.Get().velocity.X = -thePlayer.Get().velocity.X * Defs.BALL_BOUNCE_DAMP;
				}
				if (((hitBits & (ulong)(Defs.eCollisionBits.COLL_UL | Defs.eCollisionBits.COLL_UR)) != 0) && (thePlayer.Get().velocity.Z < 0.0f))	// up
				{
					bHitSound = true;
					thePlayer.Get().velocity.Z = -thePlayer.Get().velocity.Z * Defs.BALL_BOUNCE_DAMP;
				}
				if (((hitBits & (ulong)(Defs.eCollisionBits.COLL_BL | Defs.eCollisionBits.COLL_BR)) != 0) && (thePlayer.Get().velocity.Z > 0.0f))	// down
				{
					bHitSound = true;
					thePlayer.Get().velocity.Z = -thePlayer.Get().velocity.Z * Defs.BALL_BOUNCE_DAMP;
				}

				// check to see if we are sticking to a corner, and if so, move us away
				float offsetX = Math.Abs(thePlayer.Get().position.X+0.5f) - (int)Math.Abs(thePlayer.Get().position.X+0.5f);
				float offsetZ = Math.Abs(thePlayer.Get().position.Z+0.5f) - (int)Math.Abs(thePlayer.Get().position.Z+0.5f);
				if ((hitBits & (ulong)Defs.eCollisionBits.COLL_UL) != 0)
				{
					if (1.0f-offsetX < 1.0f-offsetZ)
					{
						thePlayer.Get().velocity.Z += Defs.CORNER_NUDGE;
					}
					else
					{
						thePlayer.Get().velocity.X += Defs.CORNER_NUDGE;
					}
				}
				if ((hitBits & (ulong)Defs.eCollisionBits.COLL_UR) != 0)
				{
					if (offsetX < 1.0f-offsetZ)
					{
						thePlayer.Get().velocity.Z += Defs.CORNER_NUDGE;
					}
					else
					{
						thePlayer.Get().velocity.X -= Defs.CORNER_NUDGE;
					}
				}
				if ((hitBits & (ulong)Defs.eCollisionBits.COLL_BL) != 0)
				{
					if (1.0f-offsetX < offsetZ)
					{
						thePlayer.Get().velocity.Z -= Defs.CORNER_NUDGE;
					}
					else
					{
						thePlayer.Get().velocity.X += Defs.CORNER_NUDGE;
					}
				}
				if ((hitBits & (ulong)Defs.eCollisionBits.COLL_BR) != 0)
				{
					if (offsetX < offsetZ)
					{
						thePlayer.Get().velocity.Z -= Defs.CORNER_NUDGE;
					}
					else
					{
						thePlayer.Get().velocity.X -= Defs.CORNER_NUDGE;
					}
				}

				// update the ball position
				thePlayer.Get().position += 0.5f * thePlayer.Get().velocity * dT;
				
				UpdateLight(false, Vector3.Zero);

				// update the ball and its rotation using quaternions
				Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f).Cross(thePlayer.Get().velocity);
				float angle = (float)Math.Sqrt(Math.Sqrt(thePlayer.Get().velocity.Length()));
				Vector3 angVel = (axis * angle);
				Quaternion dQ = new Quaternion(angVel, 0.0f) * thePlayer.quat * 0.5f;
				thePlayer.quat += dQ * dT;
				thePlayer.quat = thePlayer.quat.Normalize();

				// play a rolling ball sound
				float ballSpeed = thePlayer.Get().velocity.Length();
				if (ballSpeed > 0.25f)
				{
					if (SoundSystem.ballLoopSoundHandle == Defs.eSound.SND_MAX)
					{
						SoundSystem.ballLoopSoundHandle = soundPlayer.Play(Defs.eSound.SND_BALL_ROLL_LOOP, 4.0f * ballSpeed, true);
					}
					else
					{
						soundPlayer.SetVolume(Defs.eSound.SND_BALL_ROLL_LOOP, 4.0f * ballSpeed);
					}
				}
				else
				{
					if (SoundSystem.ballLoopSoundHandle != Defs.eSound.SND_MAX)
					{
						soundPlayer.Stop(SoundSystem.ballLoopSoundHandle);
						SoundSystem.ballLoopSoundHandle = Defs.eSound.SND_MAX;
					}
				}

				// play a collision sound
				if (bHitSound)
				{
					// only make a sound if we are moving, but not sliding against the wall
					if (CheckCollision(thePlayer.Get().position, (Defs.BALL_SCALE*0.5f)+0.02f) == 0)
					{
						soundPlayer.Play(Defs.eSound.SND_WALL_HIT, 1.0f, false);
					}
				}

				if (bHitPenalty)
				{
					soundPlayer.Play(Defs.eSound.SND_PENALTY_HIT, 1.0f, false);
				}
			}

			// move the camera smoothly
			if (gameState != Defs.eGameState.STATE_ATTRACT)
			{
				if (bZoomMode)
				{
					// zoom out to view entire maze
					smoothViewPos = new Vector3(0.0f, -Defs.MAZE_SIZE*1.5f, 0.0f);
					smoothViewAng = new Vector3(90.0f, 0.0f, 0.0f);
				}
				else
				{
					// update the camera to follow the ball and tilt
					smoothViewPos = new Vector3(-thePlayer.Get().position.X, -Defs.CAMERA_HEIGHT, -thePlayer.Get().position.Z - Defs.CAMERA_FOLLOW);
					smoothViewAng = new Vector3(-Defs.CAMERA_ANGLE + (thePlayer.Get().acceleration.Z*0.01f), 0.0f, -thePlayer.Get().acceleration.X*0.01f);
				}
			}

			// smooth the view to the desired location
			offsetPos = (smoothViewPos - Defs.cameraPosition) * dT * Defs.CAMERA_SPEED;
			offsetAng = (smoothViewAng - Defs.cameraRotation) * dT * Defs.CAMERA_SPEED * 2.0f;

			// rotate the camera around if we are done
			if (gameState == Defs.eGameState.STATE_FINISH)
			{
				float lightX, lightZ;
				lightX = (float)Math.Sin(stopwatch.ElapsedMilliseconds*0.001f);
				lightZ = (float)Math.Cos(stopwatch.ElapsedMilliseconds*0.001f);
				UpdateLight(true, new Vector3(lightX*Defs.MAZE_SIZE*0.5f, Defs.CAMERA_HEIGHT*0.5f, lightZ*Defs.MAZE_SIZE*0.5f));
				offsetAng.Y = dT * -20.0f;
			}

			Defs.cameraPosition += offsetPos;
			Defs.cameraRotation += offsetAng;
		}

		static void RenderHUD(float dT)
		{
			string str;
			int lineSpacing = 35;
			int headerY = 40;
			int footerY = Defs.SCREEN_HEIGHT-lineSpacing-40;
			int lineY = headerY+lineSpacing+100;
			int flash = ((int)(stopwatch.ElapsedMilliseconds*0.5f)) % 255;

//			str = String.Format("{0}ms", (int)(dT*1000.0f));
//			hudText.Print(new ImagePosition(10, 20), new ImageColor(255, 255, 255, 255), str, true, false);

			hudText.Print(new ImagePosition(Defs.SCREEN_WIDTH - 150, 20), new ImageColor(0, 255, 50, 255), "Best Time", true, false, 0, false);
			str = String.Format("{0:f1}", bestGameTime);
			hudText.Print(new ImagePosition(Defs.SCREEN_WIDTH - 150, 20+lineSpacing), new ImageColor(0, 255, 50, 255), str, true, false, 1, true);

			if (gameState == Defs.eGameState.STATE_ATTRACT)
			{
				hudText.SetRender(30, false);
				hudText.SetRender(31, false);
				hudText.SetRender(32, false);
				hudText.SetRender(33, false);
				hudText.SetRender(34, false);
				hudText.Print(new ImagePosition(-1, headerY), new ImageColor(255, 0, 255, 255), "Ball-In-A-Box In Space!", true, true, 10, false);
				hudText.Print(new ImagePosition(-1, lineY), new ImageColor(0, 255, 255, 255), "Tilt the device to roll the ball!", true, false, 11, false);
				hudText.Print(new ImagePosition(-1, lineY+lineSpacing), new ImageColor(0, 255, 178, 255), "Tap and hold the screen to zoom out, but you get a time penalty!", true, false, 12, false);
				hudText.Print(new ImagePosition(-1, lineY+lineSpacing*2), new ImageColor(0, 255, 100, 255), "Do not touch the evil cubes!", true, false, 13, false);
				hudText.Print(new ImagePosition(-1, lineY+lineSpacing*3), new ImageColor(0, 255, 50, 255), "Try to reach the finish square quickly!", true, false, 14, false);
				hudText.Print(new ImagePosition(-1, footerY), new ImageColor(255, 255, 0, 255), "Tap the screen to play now!", true, true, 15, false);
				hudText.SetRender(10, true);
				hudText.SetRender(11, true);
				hudText.SetRender(12, true);
				hudText.SetRender(13, true);
				hudText.SetRender(14, true);
				hudText.SetRender(15, true);
			}
			else if (gameState == Defs.eGameState.STATE_READY)
			{
				hudText.SetRender(10, false);
				hudText.SetRender(11, false);
				hudText.SetRender(12, false);
				hudText.SetRender(13, false);
				hudText.SetRender(14, false);
				hudText.SetRender(15, false);

				float countdownFlash = countdownTimer - (int)countdownTimer;
				hudText.Print(new ImagePosition(-1, (Defs.SCREEN_HEIGHT>>1) - 80), new ImageColor(0, 255, 0, 255), "Ready?", true, true, 20, false);
				str = String.Format("{0}", (int)countdownTimer+1.0f);
				hudText.Print(new ImagePosition(-1, Defs.SCREEN_HEIGHT>>1), new ImageColor(0, 255, 0, (int)(255.0f*countdownFlash)), str, true, true, 21, true);
				hudText.SetRender(20, true);
				hudText.SetRender(21, true);
			}
			else if (gameState == Defs.eGameState.STATE_FINISH)
			{
				hudText.SetRender(40, false);
				hudText.SetRender(41, false);
				hudText.SetRender(42, false);
				hudText.SetRender(43, false);
				hudText.SetRender(44, false);
				hudText.Print(new ImagePosition(-1, headerY), new ImageColor(255, 0, 255, 255), "GAME OVER", true, true, 30, false);
				hudText.Print(new ImagePosition(-1, footerY), new ImageColor(255, 255, 0, 255), "Tap the screen to try again", true, true, 31, false);
				if (lastGameTime == Defs.OUT_OF_TIME)
				{
					hudText.Print(new ImagePosition(-1, (Defs.SCREEN_HEIGHT>>1) - 80), new ImageColor(255, 0, 0, 255), "You ran out of time!", true, true, 32, false);
				}
				else
				{
					str = String.Format("Your time was {0:f1} s", lastGameTime);
					hudText.Print(new ImagePosition(-1, (Defs.SCREEN_HEIGHT>>1) - 80), new ImageColor(255, 255, 0, 255), str, true, true, 33, true);
					if (lastGameTime <= bestGameTime)
					{
						hudText.Print(new ImagePosition(-1, Defs.SCREEN_HEIGHT>>1), new ImageColor(0, 255, 0, flash), "A New Record!!", true, true, 34, true);
					}
				}
				hudText.SetRender(30, true);
				hudText.SetRender(31, true);
				hudText.SetRender(32, true);
				hudText.SetRender(33, true);
			}
			else if (gameState == Defs.eGameState.STATE_PLAY)
			{
				hudText.SetRender(20, false);
				hudText.SetRender(21, false);
				hudText.Print(new ImagePosition(-1, 20), new ImageColor(0, 255, 255, 255), "Time", true, false, 40, false);
				hudText.SetRender(40, true);
				str = String.Format("{0:f1}", currentGameTime);
				if (bZoomMode || (penaltyTimer > 0.0f))
				{
					hudText.Print(new ImagePosition(-1, 20+lineSpacing), new ImageColor(255, 64, 0, flash), str, true, false, 41, true);

					if (bZoomMode)
					{
						hudText.Print(new ImagePosition(-1, (Defs.SCREEN_HEIGHT>>1) - lineSpacing), new ImageColor(255, 0, 0, flash), "Zoom Time Penalty!", true, true, 42, false);
						hudText.SetRender(41, true);
					}

					if (penaltyTimer > 0.0f)
					{
						hudText.Print(new ImagePosition(-1, (Defs.SCREEN_HEIGHT>>1) + lineSpacing), new ImageColor(255, 0, 0, (int)(127.0f*penaltyTimer)), "+5s Time Penalty!", true, true, 43, true);
					}
					hudText.SetRender(44, false);
				}
				else
				{
					hudText.SetRender(41, false);
					hudText.SetRender(42, false);
					hudText.SetRender(43, false);
					hudText.Print(new ImagePosition(-1, 20+lineSpacing), new ImageColor(0, 255, 255, 255), str, true, false, 44, true);
				}
			}
		}

		public static void DrawModel(Vector3 position, Vector3 rotation, bool bUseQuaternion, Quaternion quat, Vector3 scale, Defs.eModel which, int texture)
		{
			Matrix4 matTranslation, matRotation, matWVP, matInvWV, matTemp;

			matTranslation = Matrix4.Transformation(position, scale);
			if (bUseQuaternion)
			{
				matRotation = quat.ToMatrix4();
			}
			else
			{
				matRotation = Matrix4.RotationYxz(Defs.DEGREES_TO_RADIANS(rotation.X), Defs.DEGREES_TO_RADIANS(rotation.Y), Defs.DEGREES_TO_RADIANS(rotation.Z));
			}
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

			if (which == Defs.eModel.MODEL_BALL)
			{
				modelBall.Render(graphics, ref matWVP, ref vecLight, ref vecEyePosLocal);
			}
			else if (which == Defs.eModel.MODEL_CUBE)
			{
				modelCube.Render(graphics, ref matWVP, ref vecLight, ref vecEyePosLocal, texture);
			}
			else if (which == Defs.eModel.MODEL_ARROW)
			{
				modelArrow.Render(graphics, ref matWVP, ref vecLight, ref vecEyePosLocal);
			}
			else if (which == Defs.eModel.MODEL_PLANE)
			{
				modelPlane.Render(graphics, ref matWVP, ref vecLight, ref vecEyePosLocal, texture);
			}
		}

		static void RenderBegin()
		{
			Defs.matProjection = Matrix4.Perspective(Defs.DEGREES_TO_RADIANS(Defs.FOV_Y), (float)Defs.SCREEN_WIDTH / (float)Defs.SCREEN_HEIGHT, Defs.NEAR_CLIP, Defs.FAR_CLIP);
			Defs.matWorld = Matrix4.Identity;

			graphics.SetViewport(0, 0, Defs.SCREEN_WIDTH, Defs.SCREEN_HEIGHT);
			graphics.SetClearColor(0.0f, 0.0f, 0.1f, 0.0f);
			graphics.SetClearDepth(1.0f);
			graphics.Clear();

			graphics.Enable(EnableMode.Blend);
			graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			graphics.Enable(EnableMode.CullFace);
			graphics.SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);
			graphics.Enable(EnableMode.DepthTest);
			graphics.SetDepthFunc(DepthFuncMode.LEqual, true);

			//hudText.Clear();
		}

		static void RenderEnd()
		{
			hudText.Render();
			graphics.SwapBuffers();
		}

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

				// update
				UpdateInput(lastSeconds);
				UpdateViewMatrix();
				theStarfield.Update(lastSeconds);
				theMaze.Update(lastSeconds);
				thePlayer.Update(lastSeconds);
				theArrow.Update(lastSeconds);
				soundPlayer.UpdateMusic(lastSeconds);

				// render
				RenderBegin();
				theStarfield.Render();
				theMaze.Render();
				thePlayer.Render();
				thePlane.Render();
				theArrow.Render();
				RenderHUD(lastSeconds);
				RenderEnd();

				endSeconds = (float)stopwatch.ElapsedMilliseconds / 1000.0f;
			}
			Shutdown();
		}
	
		static void Init()
		{
			// setup our vars
			bZoomMode = true;

			Defs.SCREEN_WIDTH = graphics.Screen.Width;
			Defs.SCREEN_HEIGHT = graphics.Screen.Height;

			currentGameTime = 0.0f;
			lastGameTime = Defs.OUT_OF_TIME;
			bestGameTime = Defs.OUT_OF_TIME;
			countdownTimer = 3.0f;
			bHitPenalty = false;
			penaltyTimer = 0.0f;
			SoundSystem.musicPlaying = Defs.eSound.MUS_NO_MUSIC;
			SoundSystem.oldMusicPlaying = Defs.eSound.MUS_NO_MUSIC;
			SoundSystem.musicPlayingHandle = Defs.eSound.MUS_NO_MUSIC;
			SoundSystem.oldMusicPlayingHandle = Defs.eSound.MUS_NO_MUSIC;
			SoundSystem.masterMusicVolume = 0.25f;
			SoundSystem.masterSoundVolume = 0.25f;
			SoundSystem.musicVolume = 1.0f;
			SoundSystem.oldMusicVolume = 0.0f;
			SoundSystem.ballLoopSoundHandle = Defs.eSound.SND_MAX;

			gameState = Defs.eGameState.STATE_ATTRACT;

			stopwatch.Start();
			modelArrow = new ModelArrow();
			modelBall = new ModelBall();
			modelCube = new ModelCube();
			modelPlane = new ModelPlane();
			soundPlayer = new SoundSystem();
	
			thePlayer = new PlayerClass();
			theArrow = new ArrowClass();
			theMaze = new MazeClass();
			thePlane = new PlaneClass();
			theStarfield = new StarfieldClass();

			hudText = new TextPlane(30, 50);

			thePlayer.Init();
			theMaze.Init();
			thePlane.Init();
			theArrow.Init();

			GenerateMaze();
			UpdateViewMatrix();

			bQuit = false;
		}
	
		static void Shutdown()
		{
			stopwatch.Stop();
			hudText.Dispose();
			modelArrow.Dispose();
			modelBall.Dispose();
			modelCube.Dispose();
			modelPlane.Dispose();
		}
	}
}
