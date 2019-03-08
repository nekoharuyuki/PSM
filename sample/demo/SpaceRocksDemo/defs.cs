/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace Demo_SpaceRocks
{
	public class Defs
	{
		public const int MAX_ROCKS_LARGE =	10;
		public const int ROCKS_PER_LARGE =	4;
		public const int ROCKS_PER_MEDIUM =	3;
		public const int MAX_ROCKS =		(MAX_ROCKS_LARGE*ROCKS_PER_LARGE*ROCKS_PER_MEDIUM);
		public const int NUM_ROCKS_START =	1;
		public const float MAX_ROCK_SIZE =	2.0f;

		public const int MAX_PARTICLES =	100;
		public const int MAX_BULLETS =		10;
		public const float BULLET_LIFE =	0.75f;
		public const float BULLET_SPEED =	15.0f;
		public const float BULLET_SCALE =	0.15f;
		public const float SHIP_THRUST =	10.0f;
		public const float SHIP_DRAG =		0.99f;
		public const float SCREEN_EDGE_X =	15.0f;
		public const float SCREEN_EDGE_Z =	8.0f;
		
		public static int SCREEN_WIDTH =	854;
		public static int SCREEN_HEIGHT =	480;
	
		public static Vector3 vecGlobalLight =	new Vector3(-1.0f, -1.0f, 1.0f);
		public static Vector3 cameraPosition =	new Vector3(0.0f, 20.0f, 0.0f);
		public static Vector3 cameraRotation =	new Vector3(DEGREES_TO_RADIANS(-90.0f), 0.0f, 0.0f);
		public static Random gRandNum =			new Random();

		public static Matrix4 matProjection, matView, matWorld;

		public enum eModel
		{
			MODEL_ROCK,
			MODEL_SHIP,
			MODEL_BULLET,
			MODEL_MAX
		}
		
		public enum eSound
		{
			SND_SHOOT,
			SND_ROCKHIT,
			SND_DIE,
			SND_ENGINELOOP,
			SND_MAX
		}
		
		public enum eGameState
		{
			STATE_GAMEOVER,
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

		public static void WRAP_SCREEN(ref Vector3 position, float scale)
		{
			if (position.X < -SCREEN_EDGE_X-scale)
				position.X = SCREEN_EDGE_X+scale;
			else if (position.X > SCREEN_EDGE_X+scale)
				position.X = -SCREEN_EDGE_X-scale;
			if (position.Z < -SCREEN_EDGE_Z-scale)
				position.Z = SCREEN_EDGE_Z+scale;
			else if (position.Z > SCREEN_EDGE_Z+scale)
				position.Z = -SCREEN_EDGE_Z-scale;
		}
	}
}
