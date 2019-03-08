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
	
	public class BulletB : GameActor
	{
		static int idNum=0;
		float speed=6;
		
		Vector2 trans;
		
		static float depth=0.0f;
		
		
		public BulletB(GameFrameworkSample gs, string name,  Vector3 position, float speed, float direction) : base (gs, name)
		{
			Name = name + idNum.ToString();
			this.spriteB = new SpriteB(gs.dicTextureInfo["image/bullet_red.png"]);
			//this.spriteB = new SpriteB(gs.dicTextureInfo["image/particle.png"]);
			//this.spriteB = new SpriteB(gs.dicTextureInfo["image/BrackSmoke.png"]);
			
			this.spriteB.Center.X = 0.5f;
			this.spriteB.Center.Y = 0.5f;
			//this.spriteB.Scale=new Vector2(1.0f, 2.0f);
			this.spriteB.Scale=new Vector2(1.0f, 1.0f);
			
			this.speed = speed;

			this.spriteB.Rotation = (direction+90)/180*FMath.PI;
			trans.X= FMath.Cos(direction/180*FMath.PI);
			trans.Y= FMath.Sin(direction/180*FMath.PI);
		
			this.spriteB.Position = position;
			this.spriteB.Position.Z += depth;
			
			depth+=0.0001f;
			if(depth>0.1f)
				depth=0.0f;
			
			++idNum;
		}

		public override void Update()
		{
			spriteB.Position.X = spriteB.Position.X+trans.X * speed;
			spriteB.Position.Y = spriteB.Position.Y+trans.Y * speed;

			if (spriteB.Position.X < 0 )
			{
				trans.X *=-1.0f;
				this.spriteB.Rotation = (FMath.PI-this.spriteB.Rotation)*2+this.spriteB.Rotation;
			}
			
			if(   gs.rectScreen.Width < spriteB.Position.X)
			{
				trans.X *=-1.0f;
				this.spriteB.Rotation = (FMath.PI-this.spriteB.Rotation)*2+this.spriteB.Rotation;
			}
			
			if (spriteB.Position.Y < 0 )
			{
				trans.Y *=-1.0f;	
				this.spriteB.Rotation = (FMath.PI/2.0f-this.spriteB.Rotation)*2+this.spriteB.Rotation;
			}
			
			if( gs.rectScreen.Height < spriteB.Position.Y)
			{
				trans.Y *=-1.0f;
				this.spriteB.Rotation = (FMath.PI/2.0f-this.spriteB.Rotation)*2+this.spriteB.Rotation;
			}
			
			base.Update();
		}
		
	}
	
}
