/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace Demo_BrickSmash
{
	public class Defs
	{
		public const float READY_TIME =		3.0f;
		public const float NUM_RINGS =		5.0f;
		public const int MAX_BRICKS =		100;
		public const int MAX_PARTICLES =	100;
		public const float BALL_SPEED =		5.0f;
		public const float BALL_SPEED_MAX =	20.0f;
		public const float BALL_SCALE =		0.5f;
		public const float PLAYER_RADIUS =	20.0f;
		public const float PLAYER_WIDTH =	1.0f;
		public const float PLAYER_ANGLE =	7.0f;
		public const float BRICK_ANGLE =	10.0f;
		public const float BRICK_WIDTH =	1.25f;
		
		public static int SCREEN_WIDTH =	854;
		public static int SCREEN_HEIGHT =	480;
	
		public static Vector3 vecGlobalLight =	new Vector3(-1.0f, -1.0f, 1.0f);
		public static Vector3 cameraPosition =	new Vector3(0.0f, 50.0f, 0.0f);
		public static Vector3 cameraRotation =	new Vector3(DEGREES_TO_RADIANS(-90.0f), 0.0f, 0.0f);
		public static Random gRandNum =			new Random();

		public static Matrix4 matProjection, matView, matWorld;

		public enum eModel
		{
			MODEL_PADDLE,
			MODEL_BALL,
			MODEL_BRICK,
			MODEL_MAX
		}
		
		public enum eSound
		{
			SND_PADDLEHIT,
			SND_BRICKHIT,
			SND_DIE,
			SND_MAX
		}
		
		public enum eGameState
		{
			STATE_GAMEOVER,
			STATE_READY,
			STATE_PLAYING
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

		public static void CARTESIAN_TO_POLAR(ref Vector3 position, out float distance, out float angle)
		{
			distance = position.Length();
			angle = (float)Math.Atan2(position.Z, position.X);
		}
 	}
}
