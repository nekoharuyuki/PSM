/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Framework;


namespace Sample
{
	public class GameActor : Actor
	{
		protected GameSample gs;
		
		public GameActor(GameSample gs, string name) : base(name) 
		{	
			this.gs = gs;	
		}
	}
}


