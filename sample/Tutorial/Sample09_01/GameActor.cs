/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Framework;
using Tutorial.Utility;


namespace Sample
{
	public class GameActor : Actor
	{
		protected GameFrameworkSample gs;
		
		public SpriteB spriteB;
		
		//アクション。
		public List<ActionBase> actionList=new List<ActionBase>();
		
		
		public GameActor(GameFrameworkSample gs, string name) : base(name) 
		{	
			this.gs = gs;
		}
		
		public GameActor(GameFrameworkSample gs, string name, UnifiedTextureInfo textureInfo) : this(gs,name) 
		{	
			spriteB = new SpriteB(textureInfo);
		}
		
		
		public void AddAction( ActionBase action)
		{
			actionList.Add(action);	
		}
		
		public override void Update ()
		{
			// アクション。
			foreach( ActionBase action in actionList)
			{
				action.Update();
			}
			
			//アクションが完了したら、Removeする。
			actionList.RemoveAll(action=>action.Done==true);
			
			base.Update ();
		}
		
		
		public override void Render ()
		{
			if(spriteB!=null && this.Status == ActorStatus.Action)
			{
				//gs.spriteBuffer.Add(this.spriteB);
				
				gs.piSprite.Add(this.spriteB);
				
			}
			
			base.Render ();
		}
	}
}
