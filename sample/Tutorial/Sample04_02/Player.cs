/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Tutorial.Utility;

namespace Sample
{
	public class Player : GameActor
	{
		public Player(GameFrameworkSample gs, string name, Texture2D textrue) : base(gs, name)
		{
			sprite = new SimpleSprite(gs.Graphics, textrue);
			sprite.Position.X=gs.rectScreen.Width/2;
			sprite.Position.Y=gs.rectScreen.Height/2;
			sprite.Center.X = 0.5f;
			sprite.Center.Y = 0.5f;
			sprite.Position.Z=0.5f;
		}

		public override void Update ()
		{
#if DEBUG			
			gs.debugString.WriteLine(string.Format("Position=({0},{1})\n", sprite.Position.X, sprite.Position.Y));
#endif
				
			int speed = 4;
			
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
			
			base.Update();
		}
	}
}


