/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Tutorial.Utility;


namespace Sample
{
	public class Star : GameActor
	{
		float speed;
		
		public Star(GameFrameworkSample gs, string name, Texture2D textrue, Vector3 position, Vector4 color, float speed) : base(gs, name)
		{
			sprite = new SimpleSprite(gs.Graphics, textrue);
			sprite.Position = position;
			sprite.SetColor(color);
			this.speed = speed;
		}
		
		public override void Update()
		{
			sprite.Position.Y += speed;
			
			//@e Return to the top of the screen if it gets out of the screen. 
			//@j 画面の外に出たら、画面上に戻す。
			if (sprite.Position.Y > gs.rectScreen.Height )
			{
				sprite.Position.Y = 0.0f;
				sprite.Position.X = (int)(gs.rectScreen.Width * gs.rand.NextDouble());
			}
			
			base.Update();
		}
	}
}
