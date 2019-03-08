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
	
	public class Enemy : GameActor
	{
		static int idNum=0;
		
		float speed=4;
		Int32 cnt=0;
		
		public Enemy(GameFrameworkSample gs, string name, Texture2D textrue, Vector3 position) : base (gs, name)
		{
			Name = name + idNum.ToString();
			this.sprite = new SimpleSprite( gs.Graphics, textrue );
			this.sprite.Center.X = 0.5f;
			this.sprite.Center.Y = 0.5f;
			
			idNum++;
			
			this.sprite.Position = position;
		}

		public override void Update()
		{
			sprite.Position.Y += speed;
			
			if (sprite.Position.Y > gs.rectScreen.Height + sprite.Height )
			{
				//@e Dead if it gets out of screen. 
				//@j 画面外にでたら死亡。
				this.Status = Actor.ActorStatus.Dead;
			}
			
			//@e Shoot bullets.
			//@j 弾発射。
			if(cnt == 60)
			{
				GameActor player =(GameActor)gs.Root.Search("Player");
				
				Vector2 direction;
				direction.X= player.Sprite.Position.X - this.sprite.Position.X;
				direction.Y= player.Sprite.Position.Y - this.sprite.Position.Y;
				direction=direction.Normalize();
				
				gs.Root.Search("enemyBulletManager").AddChild(new EnemyBullet(gs, "enemyBullet", gs.textureEnemyBullet, 
					new Vector3(this.sprite.Position.X,this.sprite.Position.Y, 0.5f), direction));
			}
			
			++cnt;
			
			base.Update();
		}
	}
}
