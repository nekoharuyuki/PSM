/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Framework;
using Tutorial.Utility;


namespace Sample
{
	public class Bullet : GameActor
	{
		static int idNum=0;
		float speed=8;
		
		public Bullet(GameFrameworkSample gs, string name, Texture2D textrue, Vector3 position) : base (gs, name)
		{
			Name = name + idNum.ToString();
			this.sprite = new SimpleSprite( gs.Graphics, textrue);
			this.sprite.Center.X = 0.5f;
			this.sprite.Center.Y = 0.5f;
			
			idNum++;
			
			this.sprite.Position = position;
		}

		public override void Update()
		{
			sprite.Position.Y -= speed;
			
			if (sprite.Position.Y < 0 )
			{
				this.Status = Actor.ActorStatus.Dead;
			}
			
			base.Update();
		}
	}
}

