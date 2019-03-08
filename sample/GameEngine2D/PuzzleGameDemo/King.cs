/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace PuzzleGameDemo
{
	public class King : Node
	{
		public int State;
		public SpriteTile CurrentSprite;
		public SpriteTile SpriteIdle;
		public SpriteTile SpriteBlink;
		public SpriteTile SpriteWatering1;
		public SpriteTile SpriteWatering2;
		public SpriteTile SpriteEmpty1;
		public SpriteTile SpriteEmpty2;
		public SpriteTile SpriteGameOver;

		public King()
		{
			SpriteIdle = Support.TiledSpriteFromFile("/Application/assets/king_chillin.png", 1, 1);
			SpriteBlink = Support.TiledSpriteFromFile("/Application/assets/king_chillin_blink.png", 1, 1);
			SpriteWatering1 = Support.TiledSpriteFromFile("/Application/assets/king_watering1.png", 1, 1);
			SpriteWatering2 = Support.TiledSpriteFromFile("/Application/assets/king_watering2.png", 1, 1);
			SpriteEmpty1 = Support.TiledSpriteFromFile("/Application/assets/king_empty1.png", 1, 1);
			SpriteEmpty2 = Support.TiledSpriteFromFile("/Application/assets/king_empty2.png", 1, 1);
			SpriteGameOver = Support.TiledSpriteFromFile("/Application/assets/king_game_over.png", 1, 1);

			Vector2 position = new Vector2(24.0f, 32.0f);
			SpriteIdle.Position = position;
			SpriteBlink.Position = position;
			SpriteWatering1.Position = position;
			SpriteWatering2.Position = position;
			SpriteEmpty1.Position = position;
			SpriteEmpty2.Position = position;
			SpriteGameOver.Position = position;

			AddChild(SpriteIdle);
			AddChild(SpriteBlink);
			AddChild(SpriteWatering1);
			AddChild(SpriteWatering2);
			AddChild(SpriteEmpty1);
			AddChild(SpriteEmpty2);
			AddChild(SpriteGameOver);

			ShowSprite(SpriteBlink);
		}

		public void DoBlinkSequence()
		{
			this.StopAllActions();

			var blink_speed = Game.Instance.Random.NextFloat() * 0.1f + 0.1f;
			var sequence = new Sequence();
			sequence.Add(new CallFunc(() => ShowSprite(SpriteIdle)));
			sequence.Add(new DelayTime() { Duration = 1.0f });

			if (Game.Instance.Random.Next() % 100 < 50)
			{
				sequence.Add(new CallFunc(() => ShowSprite(SpriteBlink)));
				sequence.Add(new DelayTime() { Duration = blink_speed });
				sequence.Add(new CallFunc(() => ShowSprite(SpriteIdle)));
				sequence.Add(new DelayTime() { Duration = blink_speed });
			}

			sequence.Add(new CallFunc(() => ShowSprite(SpriteBlink)));
			sequence.Add(new DelayTime() { Duration = blink_speed });
			sequence.Add(new CallFunc(() => ShowSprite(SpriteIdle)));
			sequence.Add(new DelayTime() { Duration = Game.Instance.Random.NextFloat() * 2.0f + 0.5f });
			sequence.Add(new CallFunc(() => DoBlinkSequence()));

			this.RunAction(sequence);
		}

		public void DoGameOverSequence()
		{
			this.StopAllActions();

			ShowSprite(SpriteGameOver);
		}

		public void DoWaterEmptySequence()
		{
			this.StopAllActions();

			var anim_speed = 0.2f;
			var sequence = new Sequence();

			for (int i = 0; i < 6; ++i)
			{
				sequence.Add(new CallFunc(() => ShowSprite(SpriteEmpty1)));
				sequence.Add(new DelayTime() { Duration = anim_speed });
				sequence.Add(new CallFunc(() => ShowSprite(SpriteEmpty2)));
				sequence.Add(new DelayTime() { Duration = anim_speed });
			}

			sequence.Add(new CallFunc(() => DoBlinkSequence()));
			this.RunAction(sequence);
		}

		public void DoWaterSequence()
		{
			this.StopAllActions();

			var anim_speed = 0.3f;
			var sequence = new Sequence();

			for (int i = 0; i < 1; ++i)
			{
				sequence.Add(new CallFunc(() => ShowSprite(SpriteWatering1)));
				sequence.Add(new DelayTime() { Duration = anim_speed });
				sequence.Add(new CallFunc(() => ShowSprite(SpriteWatering2)));
				sequence.Add(new DelayTime() { Duration = anim_speed });
			}

			sequence.Add(new CallFunc(() => { if (Game.Instance.Board.WaterMode) DoWaterSequence(); else DoWaterEmptySequence(); }));

			this.RunAction(sequence);
		}

		public void ShowSprite(SpriteTile sprite)
		{
			CurrentSprite = sprite;

			SpriteIdle.Visible = false;
			SpriteBlink.Visible = false;
			SpriteWatering1.Visible = false;
			SpriteWatering2.Visible = false;
			SpriteEmpty1.Visible = false;
			SpriteEmpty2.Visible = false;
			SpriteGameOver.Visible = false;

			CurrentSprite.Visible = true;
		}
	};

}

