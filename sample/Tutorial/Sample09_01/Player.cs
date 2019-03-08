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
		
		
		public Player(GameFrameworkSample gs, string name) : base(gs, name)
		{
			spriteB = new SpriteB(gs.dicTextureInfo["image/myship.png"]);

			spriteB.Center.X = 0.5f;
			spriteB.Center.Y = 0.5f;
			
			this.Initilize();
		}
		
		public void Initilize()
		{
			this.playerStatus = PlayerStatus.Normal;
			spriteB.Position.X=gs.rectScreen.Width/2;
			spriteB.Position.Y=gs.rectScreen.Height*3/4;
			spriteB.Position.Z=0.1f;
			cnt=0;
		}
		

		public override void Update ()
		{
		
			if(this.playerStatus == PlayerStatus.Normal || this.playerStatus == PlayerStatus.Invincible)
			{
				if(this.playerStatus == PlayerStatus.Invincible && ++cnt > 120)
				{
					this.playerStatus= PlayerStatus.Normal;
					this.spriteB.SetColor(Vector4.One);
					cnt=0;
				}
				
				if((gs.PadData.Buttons & GamePadButtons.Left) != 0)
				{
					spriteB.Position.X -= speed;
					if(spriteB.Position.X < spriteB.Width/2.0f)
						spriteB.Position.X=spriteB.Width/2.0f;
				}
				if((gs.PadData.Buttons & GamePadButtons.Right) != 0)
				{
					spriteB.Position.X += speed;
					if(spriteB.Position.X> gs.rectScreen.Width - spriteB.Width/2.0f)
						spriteB.Position.X=gs.rectScreen.Width - spriteB.Width/2.0f;
				}
				if((gs.PadData.Buttons & GamePadButtons.Up) != 0)
				{
					spriteB.Position.Y -= speed;
					if(spriteB.Position.Y < spriteB.Height/2.0f)
						spriteB.Position.Y =spriteB.Height/2.0f;
				}
				if((gs.PadData.Buttons & GamePadButtons.Down) != 0)
				{
					spriteB.Position.Y += speed;
					if(spriteB.Position.Y > gs.rectScreen.Height - spriteB.Height/2.0f)
						spriteB.Position.Y=gs.rectScreen.Height - spriteB.Height/2.0f;
				}
				

				const int numOfBullet=64;
				
				//@j 弾をだす。
				//@e Shoot bullets.
				if((gs.PadData.ButtonsDown & (GamePadButtons.Circle)) != 0)
				{
					Actor bulletManager=gs.Root.Search("bulletManager");
					
					for(int i=0; i<numOfBullet; ++i)
					{
						//if(bulletManager.Children.Count<gs.spriteBuffer.IsFull()==true)
						//if(gs.spriteBuffer.IsFull()==false)
						if(gs.piSprite.IsFull()==false)
						{
							bulletManager.AddChild(
								new BulletB(gs, "BulletB",new Vector3(this.spriteB.Position.X,this.spriteB.Position.Y, 0.4f),
						                                                    2.0f, i*360.0f/numOfBullet));
						}
						else
							break;
					}
				}
			
				// スプライトをクリア。
				if((gs.PadData.ButtonsDown & (GamePadButtons.Triangle)) != 0)
				{
					gs.Root.Search("bulletManager").Children.Clear();
				}
				
			}
			
			base.Update();
		}
	}
}

