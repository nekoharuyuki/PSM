/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;

namespace Demo_SpaceRocks
{
	public class sObject
	{
		public Vector3	position;
		public Vector3	rotation;
		public Vector3	velocity;
		public Vector3	angularVelocity;
		public Vector3	acceleration;
		public Vector3	angularAcceleration;
		public float	scale;
		public float	age;
		public bool		bIsActive;

		public void Clear()
		{
			position = Vector3.Zero;
			rotation = Vector3.Zero;
			velocity = Vector3.Zero;
			angularVelocity = Vector3.Zero;
			acceleration = Vector3.Zero;
			angularAcceleration = Vector3.Zero;
			scale = 0.0f;
			age = 0.0f;
			bIsActive = false;
		}
	}
	
	public class PlayerClass
	{
		private sObject		playerObject = new sObject();
		private float		blinkTimer;
		private bool		bDrawPlayer;
		public float		invincibleTimer;
		
		public sObject Get()
		{
			return playerObject;
		}

		public void Init()
		{
			playerObject.Clear();
			playerObject.scale = 1.0f;
			invincibleTimer = 5.0f;
			blinkTimer = 1.0f;
		}
		
		public bool IsInvincible()
		{
			return (invincibleTimer > 0.0f);
		}
		
		public void Update(float dT)
		{
			// tick the invincible timer
			if (invincibleTimer >= 0.0f)
				invincibleTimer -= dT;
			else
				invincibleTimer = 0.0f;
	
			// blink the player if invincible
			bDrawPlayer = true;
			if (invincibleTimer > 0.0f)
			{
				if (invincibleTimer > 4.0f)
					blinkTimer -= dT;
				else if (invincibleTimer > 2.5f)
					blinkTimer -= dT * 2.0f;
				else if (invincibleTimer > 1.0f)
					blinkTimer -= dT * 4.0f;
				else if (invincibleTimer > 0.0f)
					blinkTimer -= dT * 8.0f;
	
				if (blinkTimer >= 0.5f)
				{
					bDrawPlayer = false;
				}
				else if (blinkTimer <= 0.0f)
				{
					blinkTimer = 1.0f;
				}
			}

			playerObject.age += dT;
			playerObject.velocity += playerObject.acceleration * dT;
			playerObject.velocity *= Defs.SHIP_DRAG;
			playerObject.position += playerObject.velocity * dT;

			// check for screen wrap
			if (playerObject.position.X < -Defs.SCREEN_EDGE_X-playerObject.scale)
				playerObject.position.X = Defs.SCREEN_EDGE_X+playerObject.scale;
			else if (playerObject.position.X > Defs.SCREEN_EDGE_X+playerObject.scale)
				playerObject.position.X = -Defs.SCREEN_EDGE_X-playerObject.scale;
			if (playerObject.position.Z < -Defs.SCREEN_EDGE_Z-playerObject.scale)
				playerObject.position.Z = Defs.SCREEN_EDGE_Z+playerObject.scale;
			else if (playerObject.position.Z > Defs.SCREEN_EDGE_Z+playerObject.scale)
				playerObject.position.Z = -Defs.SCREEN_EDGE_Z-playerObject.scale;
		}
		
		public void Render()
		{
			// render the player
			if (bDrawPlayer)
			{
				SpaceRocksProgram.DrawModel(playerObject.position, playerObject.rotation, playerObject.scale, Defs.eModel.MODEL_SHIP);
			}
		}
	}

	public class BulletClass
    {
		private sObject[]	bulletObjects = new sObject[Defs.MAX_BULLETS];
		
		public sObject Get(int index)
		{
			return bulletObjects[index];
		}

		public void Init()
		{
			for (int i=0; i<Defs.MAX_BULLETS; i++)
			{
				bulletObjects[i] = new sObject();
				bulletObjects[i].Clear();
			}
		}
		
		public void Update(float dT)
		{
			for (int i=0; i<Defs.MAX_BULLETS; i++)
			{
				if (bulletObjects[i].bIsActive)
				{
					bulletObjects[i].age += dT;
					bulletObjects[i].position += bulletObjects[i].velocity * dT;
					bulletObjects[i].rotation += new Vector3(0, 360.0f, 0) * dT;

					// check for screen wrap
					Defs.WRAP_SCREEN(ref bulletObjects[i].position, bulletObjects[i].scale);

					// check for death
					if (bulletObjects[i].age > Defs.BULLET_LIFE)
						bulletObjects[i].bIsActive = false;
				}
			}
		}
		
		public void Render()
		{
			for (int i=0; i<Defs.MAX_BULLETS; i++)
			{
				if (bulletObjects[i].bIsActive)
				{
					SpaceRocksProgram.DrawModel(bulletObjects[i].position, bulletObjects[i].rotation, bulletObjects[i].scale, Defs.eModel.MODEL_BULLET);
				}
			}
		}
		public void Spawn(Vector3 initialPosition, float angle)
        {
			for (int i=0; i<Defs.MAX_BULLETS; i++)
			{
				if (!bulletObjects[i].bIsActive)
				{
					bulletObjects[i].Clear();
					bulletObjects[i].bIsActive = true;
					bulletObjects[i].scale = Defs.BULLET_SCALE;
					bulletObjects[i].position = initialPosition;
					bulletObjects[i].velocity.X = (float)Math.Sin(Defs.DEGREES_TO_RADIANS(angle)) * Defs.BULLET_SPEED;
					bulletObjects[i].velocity.Z = (float)Math.Cos(Defs.DEGREES_TO_RADIANS(angle)) * Defs.BULLET_SPEED;
					break;
				}
			}
		}
	}
			
	public class RockClass
    {
		private sObject[]	rockObjects = new sObject[Defs.MAX_ROCKS];

		public sObject Get(int index)
		{
			return rockObjects[index];
		}

		public void Init(int level)
        {
			for (int i=0; i<Defs.MAX_ROCKS; i++)
			{
				rockObjects[i] = new sObject();
				rockObjects[i].Clear();
			}
					
			for (int i=0; i<Defs.NUM_ROCKS_START+level; i++)
			{
				Spawn(Defs.MAX_ROCK_SIZE, Vector3.Zero);
			}
		}
		
		public void Update(float dT)
		{
			for (int i=0; i<Defs.MAX_ROCKS; i++)
			{
				if (rockObjects[i].bIsActive)
				{
					rockObjects[i].age += dT;
					rockObjects[i].position += rockObjects[i].velocity * dT;
					rockObjects[i].rotation += rockObjects[i].angularVelocity * dT;
	
					// check for screen wrap
					Defs.WRAP_SCREEN(ref rockObjects[i].position, rockObjects[i].scale);
				}
			}
		}
			
		public void Render()
		{
			for (int i=0; i<Defs.MAX_ROCKS; i++)
			{
				if (rockObjects[i].bIsActive)
				{
					SpaceRocksProgram.DrawModel(rockObjects[i].position, rockObjects[i].rotation, rockObjects[i].scale, Defs.eModel.MODEL_ROCK);
				}
			}
		}

		public void Spawn(float size, Vector3 initialPosition)
        {
			for (int i=0; i<Defs.MAX_ROCKS; i++)
			{
				if (!rockObjects[i].bIsActive)
				{
					rockObjects[i].Clear();
					rockObjects[i].bIsActive = true;
					rockObjects[i].scale = size + Defs.RANDOM_RANGE(size*0.1f);
					if (initialPosition.IsZero())
					{
						// don't let rocks spawn in the middle of the screen to avoid the player
						do 
						{
							rockObjects[i].position.X = Defs.SCREEN_EDGE_X/2.0f - Defs.RANDOM_RANGE(Defs.SCREEN_EDGE_X);
							rockObjects[i].position.Z = Defs.SCREEN_EDGE_Z/2.0f - Defs.RANDOM_RANGE(Defs.SCREEN_EDGE_Z);
						} while ((Math.Abs(rockObjects[i].position.X) < 2.0f) && (Math.Abs(rockObjects[i].position.Z) < 2.0f));
					}
					else
					{
						rockObjects[i].position = initialPosition;
					}
					rockObjects[i].rotation.X = Defs.RANDOM_RANGE(360.0f);
					rockObjects[i].rotation.Z = Defs.RANDOM_RANGE(360.0f);
		
					float scale = Defs.MAX_ROCK_SIZE-size+1.0f;
					rockObjects[i].velocity.X = scale - Defs.RANDOM_RANGE(scale*2.0f);
					rockObjects[i].velocity.Z = scale - Defs.RANDOM_RANGE(scale*2.0f);
					rockObjects[i].angularVelocity.X = scale*40.0f - Defs.RANDOM_RANGE(scale*80.0f);
					rockObjects[i].angularVelocity.Z = scale*40.0f - Defs.RANDOM_RANGE(scale*80.0f);
					break;
				}
			}
		}
	}	
}