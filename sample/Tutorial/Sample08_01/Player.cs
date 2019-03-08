/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.Framework;
using Tutorial.Utility;

namespace Sample
{
	public class Player : GameActor
	{
		Int32 cnt=0;
		int speed = 4;
	
		public enum PlayerStatus
		{
			Normal,
			Explosion,
			Invincible,
			GameOver,
		};
		
		public PlayerStatus playerStatus;
		
		
		public Player(GameFrameworkSample gs, string name, Texture2D textrue) : base(gs, name)
		{
			sprite = new SimpleSprite(gs.Graphics, textrue);
			
			sprite.Center.X = 0.5f;
			sprite.Center.Y = 0.5f;
			
			this.Initilize();
		}
		
		public void Initilize()
		{
			this.playerStatus = PlayerStatus.Normal;
			sprite.Position.X=gs.rectScreen.Width/2;
			sprite.Position.Y=gs.rectScreen.Height*3/4;
			sprite.Position.Z=0.1f;
			cnt=0;
		}
		

		public override void Update ()
		{
#if DEBUG			
			gs.debugString.WriteLine(string.Format("Position=({0},{1})\n", sprite.Position.X, sprite.Position.Y));
#endif
			
			if(this.playerStatus == PlayerStatus.Normal || this.playerStatus == PlayerStatus.Invincible)
			{
				if(this.playerStatus == PlayerStatus.Invincible && ++cnt > 120)
				{
					this.playerStatus= PlayerStatus.Normal;
					this.Sprite.SetColor(Vector4.One);
					cnt=0;
				}
				
				
				if((gs.PadData.Buttons & GamePadButtons.Left) != 0)
				{
					sprite.Position.X -= speed;
					if(sprite.Position.X < sprite.Width/2.0f)
						sprite.Position.X=sprite.Width/2.0f;
				}
				if((gs.PadData.Buttons & GamePadButtons.Right) != 0)
				{
					sprite.Position.X += speed;
					if(sprite.Position.X> gs.rectScreen.Width - sprite.Width/2.0f)
						sprite.Position.X=gs.rectScreen.Width - sprite.Width/2.0f;
				}
				if((gs.PadData.Buttons & GamePadButtons.Up) != 0)
				{
					sprite.Position.Y -= speed;
					if(sprite.Position.Y < sprite.Height/2.0f)
						sprite.Position.Y =sprite.Height/2.0f;
				}
				if((gs.PadData.Buttons & GamePadButtons.Down) != 0)
				{
					sprite.Position.Y += speed;
					if(sprite.Position.Y > gs.rectScreen.Height - sprite.Height/2.0f)
						sprite.Position.Y=gs.rectScreen.Height - sprite.Height/2.0f;
				}
			
				//@e Shoot bullets.
				//@j 弾をだす。
				if((gs.PadData.ButtonsDown & (GamePadButtons.Circle | GamePadButtons.Cross)) != 0)
				{
					gs.soundPlayerBullet.Play();
					gs.Root.Search("bulletManager").AddChild(new Bullet(gs, "bullet", gs.textureBullet, 
						new Vector3(this.sprite.Position.X,this.sprite.Position.Y, 0.4f)));
				}
			
			}
			else if(this.playerStatus == PlayerStatus.Explosion)
			{
				this.Status = Actor.ActorStatus.UpdateOnly;
				
				if( ++cnt > 60)
				{
					if( gs.NumShips > 0)
					{
						this.Status = Actor.ActorStatus.Action;
						this.playerStatus = PlayerStatus.Invincible;
						cnt=0;
					}
					else
					{
						this.playerStatus = PlayerStatus.GameOver;
						this.Status = Actor.ActorStatus.UpdateOnly;
					}
				}
			}
			
			if(this.playerStatus == PlayerStatus.Invincible)
			{
				//@e Flash in translucence.
				//@j 半透明で点滅させる。
				if( gs.appCounter % 2 == 0)
					this.Sprite.SetColor(1.0f, 1.0f, 1.0f, 0.75f);
				else
					this.Sprite.SetColor(1.0f, 1.0f, 1.0f, 0.2f);
			}
			
			base.Update();
		}
	}
}

