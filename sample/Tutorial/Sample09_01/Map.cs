/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Tutorial.Utility;


namespace Sample
{
	
	public class Map : GameActor
	{
		float speed;
		int numOfmapTileY=0;
		
		List<SpriteB> listSpriteB = new List<SpriteB>();
		
		public Map(GameFrameworkSample gs, string name) : base(gs, name)
		{
			//string mapTexutre="image/background.png";//森。
			string mapTexutre="image/map_spaceA.png";//宇宙。
			
			float tileMapWidth=gs.dicTextureInfo[mapTexutre].w;
			float tileMapHeight=gs.dicTextureInfo[mapTexutre].h;
			
			numOfmapTileY = (int)(gs.rectScreen.Height/tileMapHeight)+2;
			
			for(float y=-tileMapHeight; y < gs.rectScreen.Height; y=y+=tileMapHeight)
			{
				for(float x=0; x < gs.rectScreen.Width; x=x+tileMapWidth)
				{
					SpriteB spriteTileMap = new SpriteB(gs.dicTextureInfo[mapTexutre]);
					
					spriteTileMap.Position.X=x;
					spriteTileMap.Position.Y=y;
					
					spriteTileMap.Position.Z=0.9f;
					
					listSpriteB.Add(spriteTileMap);
				}
			}

			this.speed = 1.0f;
		}

		public override void Update()
		{
			foreach(var sprite in listSpriteB)
			{
				sprite.Position.Y += speed;
			
				//@j 画面の外に出たら、画面上に戻す。
				//@e Return onto the screen if it gets out of the screen. 
				if (sprite.Position.Y > gs.rectScreen.Height )
				{
					sprite.Position.Y = sprite.Position.Y-sprite.Height*numOfmapTileY;
				}
			}
			
			base.Update();
		}
		
		public override void Render()
		{
			foreach(var sprite in listSpriteB)
			{
				//sprite.Render();	
				//gs.spriteBuffer.Add(sprite);
				gs.piSprite.Add(sprite);
				
			}
			
			base.Render();
		}
	}
}
