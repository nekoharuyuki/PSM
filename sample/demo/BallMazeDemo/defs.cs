/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace Demo_BallMaze
{
	public class Defs
	{
		public const float	NEAR_CLIP				= 2.0f;
		public const float	FAR_CLIP				= 8500.1f;
		public const float	FOV_Y					= 40.0f;	//field of view in the y direction
		public const float	MUSIC_CROSSFADE_TIME	= 2.0f;
		public const int	MAZE_SIZE				= 31;		// Must be an odd number to allow room for the hallways!!!
		public const int	NUM_PENALTY_BLOCKS		= (int)(Defs.MAZE_SIZE*0.75f);

		public const float  BALL_SCALE			= 0.7f;
		public const float  BALL_ACCELERATION	= 10.0f;
		public const float  BALL_MAX_VELOCITY	= 500.0f;
		public const float  BALL_MIN_VELOCITY	= 0.01f;
		public const float  BALL_FRICTION		= 0.98f;
		public const float  BALL_BOUNCE_DAMP	= 0.3f;
		public const float  BALL_PAD_SENS		= 20.0f;
		public const float	BALL_PAD_Y_OFFS		= 0.11f;
		public const float	BALL_PAD_DEAD_ZONE	= 0.05f;

		public const float  CAMERA_HEIGHT		= 10.0f;
		public const float  CAMERA_FOLLOW		= 6.0f;
		public const float  CAMERA_ANGLE		= -60.0f;
		public const float  CAMERA_SPEED		= 3.0f;

		public const float  DROP_SHADOW_SIZE	= 3.0f;
		public const float  OUT_OF_TIME			= 300.0f;
		public const float  CUBE_FALL_DIST		= 60.0f;
		public const float  PAD_BIAS			= 0.01f;
		public const float  CORNER_NUDGE		= 1.0f;

		public const int	MAX_STARS			= 500;
		public const float	MAX_STAR_SPEED		= 80.0f;
		public const float	MAX_STAR_SPREAD		= 80.0f;
		public const float	STAR_HEIGHT			= 50.0f;

		public static Vector3 vecGlobalLight	= new Vector3(-1.0f, -1.0f, 1.0f);
		public static Vector3 cameraPosition	= new Vector3(0.0f, -Defs.MAZE_SIZE*1.5f, 0.0f);
		public static Vector3 cameraRotation	= new Vector3(90.0f, 0.0f, 0.0f);
		public static Vector3 lightGlobalAmb	= new Vector3(0.01f, 0.01f, 0.01f);
		public static Random gRandNum			= new Random();

		public static int SCREEN_WIDTH			= 854;
		public static int SCREEN_HEIGHT			= 480;

		public static Matrix4 matProjection, matView, matWorld;

		// collision bits - 2 per side
		public enum eCollisionBits
		{
			COLL_UL		= 0x00000003,
			COLL_U		= 0x0000000C,
			COLL_UR		= 0x00000030,
			COLL_L		= 0x000000C0,
			COLL_C		= 0x00000300,
			COLL_R		= 0x00000C00,
			COLL_BL		= 0x00003000,
			COLL_B		= 0x0000C000,
			COLL_BR		= 0x00030000,
		}

		// maze block types
		public enum eBlockType
		{
			MAZE_EMPTY,
			MAZE_STARTPAD,
			MAZE_FINISHPAD,
			MAZE_INNERWALL,
			MAZE_OUTERWALL,
			MAZE_PENALTYWALL,
			MAZE_MAX_WALL
		}

		public enum eModel
		{
			MODEL_ARROW,
			MODEL_BALL,
			MODEL_CUBE,
			MODEL_PLANE,
			MODEL_MAX
		}

		public enum eSound
		{
			SND_WALL_HIT,
			SND_PENALTY_HIT,
			SND_CUBE_BOUNCE,
			SND_READY1_BLIP,
			SND_READY2_BLIP,
			SND_BALL_ROLL_LOOP,
			SND_BALL_SLIDE_LOOP,
			MUS_NO_MUSIC,
			MUS_ATTRACT_LOOP,
			MUS_PLAY_LOOP,
			MUS_WINNER_LOOP,
			MUS_GAMEOVER_LOOP,
			MUS_LOSER_LOOP,
			SND_MAX
		}
		
		public enum eGameState
		{
			STATE_ATTRACT,		// "press X" to start mode
			STATE_READY,		// ready countdown timer
			STATE_PLAY,			// default timed play mode
			STATE_FINISH		// display last score
		}
	
	    public static float RANDOM_RANGE(float range)
	    {
	        return ((float)gRandNum.NextDouble() * range);
	    }
	
		public static float DEGREES_TO_RADIANS(float degrees)
		{
			return (degrees / 180.0f * (float)Math.PI);
		}

		public static float RADIANS_TO_DEGREES(float radians)
		{
			return (radians * 180.0f / (float)Math.PI);
		}
 	}
}
