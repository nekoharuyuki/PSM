
using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class Title
		: Sce.PlayStation.HighLevel.GameEngine2D.Node
	{
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile TitleSprite { get; set; }
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile SparkleSprite { get; set; }
		public Sequence SparkleSequence { get; set; }
		public Sce.PlayStation.HighLevel.GameEngine2D.Label PressAnyKeyLabel { get; set; }
		
		public Title()
		{
			TitleSprite = Support.TiledSpriteFromFile("/Application/assets/title.png", 1, 1);
			SparkleSprite = Support.TiledSpriteFromFile("/Application/assets/sparkle_frames.png", 8, 1);
			
			TitleSprite.Scale = new Vector2(1.0f);
			TitleSprite.Position = new Vector2(0.0f, 270.0f);
			SparkleSprite.Scale = new Vector2(1.0f);
			
			SparkleSequence = new Sequence();
			
			SparkleSequence.Add(new CallFunc(() => SparkleSprite.Position = new Vector2(208.0f, 40.0f)));
			SparkleSequence.Add(new Support.AnimationAction(SparkleSprite, 0, 8, 0.4f, looping: false));
			SparkleSequence.Add(new DelayTime() { Duration = 0.6f });
			SparkleSequence.Add(new CallFunc(() => SparkleSprite.Position = new Vector2(336.0f, 151.0f)));
			SparkleSequence.Add(new Support.AnimationAction(SparkleSprite, 0, 8, 0.4f, looping: false));
			SparkleSequence.Add(new DelayTime() { Duration = 0.6f });
			SparkleSequence.Add(new CallFunc(() => SparkleSprite.Position = new Vector2(746.0f, 68.0f)));
			SparkleSequence.Add(new Support.AnimationAction(SparkleSprite, 0, 8, 0.4f, looping: false));
			SparkleSequence.Add(new DelayTime() { Duration = 0.6f });
			SparkleSequence.Add(new CallFunc(() => SparkleSprite.Position = new Vector2(405.0f, 123.0f)));
			SparkleSequence.Add(new Support.AnimationAction(SparkleSprite, 0, 8, 0.4f, looping: false));
			SparkleSequence.Add(new DelayTime() { Duration = 3.0f });
			
			this.AddChild(TitleSprite);
			TitleSprite.AddChild(SparkleSprite);
			
			var repeat = new RepeatForever() { InnerAction = SparkleSequence };
			SparkleSprite.RunAction(repeat);
		}
	}
}
