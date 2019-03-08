/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace PuzzleGameDemo
{
	public class NoCleanupScene : Scene
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void OnExit()
		{
			StopAllActions();
		}
	}

}

