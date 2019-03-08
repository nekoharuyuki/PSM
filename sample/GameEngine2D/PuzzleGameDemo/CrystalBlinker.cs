/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PuzzleGameDemo
{
	public class CrystalBlinker : Node
	{
		int BlinkState;
		int BlinkStateTransitionCountdown;

		Crystal BlinkCrystal;

		public void Tick(float dt)
		{
			if (Game.Instance.Board.IsBoardLocked())
				return;

			// 0: blink-close
			// 1: blink-open
			// 2: delay
			switch (BlinkState)
			{
				case 0:
					{
						BlinkStateTransitionCountdown -= 1;
						if (BlinkStateTransitionCountdown > 0)
							return;

						if (BlinkCrystal == null)
						{
							BlinkCrystal = Game.Instance.Board.GetCrystalAtTile(
								Game.Instance.Random.Next() % Game.Instance.Board.BoardSize.X,
								Game.Instance.Random.Next() % Game.Instance.Board.BoardSize.Y
							);
						}

						if (BlinkCrystal.SubType != 2)
						{
							BlinkCrystal = null;
							return;
						}

						BlinkCrystal.Sprite.TileIndex2D = new Vector2i(3, BlinkCrystal.Sprite.TileIndex2D.Y);
						BlinkStateTransitionCountdown = Game.Instance.Random.Next() % 20 + 20;
						BlinkState += 1;
						break;
					}

				case 1:
					{
						BlinkStateTransitionCountdown -= 1;
						if (BlinkStateTransitionCountdown > 0)
						{
							return;
						}

						BlinkCrystal.Sprite.TileIndex2D = new Vector2i(BlinkCrystal.SubType, BlinkCrystal.Sprite.TileIndex2D.Y);
						BlinkStateTransitionCountdown = Game.Instance.Random.Next() % 20 + 20;
						BlinkState -= 1;
						BlinkCrystal = null;
						break;
					}
			}
		}
	}

}

