/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace Demo_BrickSmash
{
	public class sObject
	{
		public Vector3	position;
		public Vector3	rotation;
		public Vector3	velocity;
		public Vector3	angularVelocity;
		public Vector3	acceleration;
		public Vector4	color;
		public float	scale;
		public float	value;
		public float	age;
		public bool		bIsActive;

		public void Clear()
		{
			position = Vector3.Zero;
			rotation = Vector3.Zero;
			velocity = Vector3.Zero;
			angularVelocity = Vector3.Zero;
			acceleration = Vector3.Zero;
			color = Vector4.Zero;
			scale = 0.0f;
			value = 0.0f;
			age = 0.0f;
			bIsActive = false;
		}
	}
	
	public class PlayerClass
	{
		private sObject		playerObject = new sObject();
		
		public sObject Get()
		{
			return playerObject;
		}

		public void Init()
		{
			playerObject.Clear();
			playerObject.scale = 1.0f;

			float ang = (float)Math.Atan2(-1.0f, 0.0f);
			float x = (float)Math.Cos(ang) * Defs.PLAYER_RADIUS;
			float y = (float)Math.Sin(ang) * Defs.PLAYER_RADIUS;
			float rot = 90.0f - Defs.RADIANS_TO_DEGREES(ang);
			playerObject.position.X = x;
			playerObject.position.Z = y;
			playerObject.rotation.Y = rot;
		}
		
		public void Update(float dT)
		{
			playerObject.age += dT;
		}

		public void Render()
		{
			BrickSmashProgram.DrawModel(playerObject.position, playerObject.rotation, playerObject.scale, Defs.eModel.MODEL_PADDLE, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
		}
	}

	public class BallClass
    {
		private sObject	ballObject = new sObject();
		
		public sObject Get()
		{
			return ballObject;
		}

		public void Init()
		{
			ballObject.Clear();

			float ang = (float)Math.Atan2(-1.0f, 0.0f);
			float x = (float)Math.Cos(ang) * Defs.PLAYER_RADIUS * 0.6f;
			float y = (float)Math.Sin(ang) * Defs.PLAYER_RADIUS * 0.6f;
			Spawn(new Vector3(x, 0, y), new Vector3(0.0f, 0.0f, -Defs.BALL_SPEED));
		}
		
		public void Update(float dT)
		{
			ballObject.age += dT;
			ballObject.angularVelocity.Y = ballObject.velocity.X * 20.0f;
			if (Math.Abs(ballObject.velocity.Z) > Math.Abs(ballObject.velocity.X))
				ballObject.angularVelocity.Y = ballObject.velocity.Z * 20.0f;
			ballObject.position += ballObject.velocity * dT;
			ballObject.rotation += ballObject.angularVelocity * dT;
		}
		
		public void Render()
		{
			BrickSmashProgram.DrawModel(ballObject.position, ballObject.rotation, ballObject.scale, Defs.eModel.MODEL_BALL, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
		}
		public void Spawn(Vector3 initialPosition, Vector3 initialVelocity)
        {
			ballObject.Clear();
			ballObject.scale = Defs.BALL_SCALE;
			ballObject.position = initialPosition;
			ballObject.velocity = initialVelocity;
		}
	}
			
	public class BrickClass
    {
		private sObject[]	brickObjects = new sObject[Defs.MAX_BRICKS];

		public sObject Get(int index)
		{
			return brickObjects[index];
		}

		public void Init(int level)
        {
			for (int i=0; i<Defs.MAX_BRICKS; i++)
			{
				brickObjects[i] = new sObject();
				brickObjects[i].Clear();
			}

			// spawn the bricks in concentric rings around the center of the screen
			float radius = 4.5f;
			float ringSpacing = 1.2f;
			float x, y, rot, step;
			for (float ring=0.0f; ring<Defs.NUM_RINGS; ring+=1.0f)
			{
				step = (float)Math.PI / (6.0f+ring);
				for (float ang=0.0f; ang<2.0f*Math.PI; ang+=step)
				{
					x = (float)Math.Cos(ang) * (radius + ring * ringSpacing);
					y = (float)Math.Sin(ang) * (radius + ring * ringSpacing);
					rot = 90.0f - Defs.RADIANS_TO_DEGREES(ang);
					Spawn(1.0f, new Vector3(x, 0.0f, y), new Vector3(0.0f, rot, 0.0f), new Vector4(ring/Defs.NUM_RINGS, 0.0f, (Defs.NUM_RINGS-ring)/Defs.NUM_RINGS, 1.0f), ring);
				}
			}
		}
		
		public void Update(float dT)
		{
			for (int i=0; i<Defs.MAX_BRICKS; i++)
			{
				if (brickObjects[i].bIsActive)
				{
					brickObjects[i].age += dT;
				}
			}
		}
			
		public void Render()
		{
			for (int i=0; i<Defs.MAX_BRICKS; i++)
			{
				if (brickObjects[i].bIsActive)
				{
					BrickSmashProgram.DrawModel(brickObjects[i].position, brickObjects[i].rotation, brickObjects[i].scale, Defs.eModel.MODEL_BRICK, brickObjects[i].color);
				}
			}
		}

		public void Spawn(float size, Vector3 initialPosition, Vector3 initialRotation, Vector4 color, float value)
        {
			for (int i=0; i<Defs.MAX_BRICKS; i++)
			{
				if (!brickObjects[i].bIsActive)
				{
					brickObjects[i].Clear();
					brickObjects[i].bIsActive = true;
					brickObjects[i].scale = size;
					brickObjects[i].position = initialPosition;
					brickObjects[i].rotation = initialRotation;
					brickObjects[i].color = color;
					brickObjects[i].value = value;
					break;
				}
			}
		}
	}	
}