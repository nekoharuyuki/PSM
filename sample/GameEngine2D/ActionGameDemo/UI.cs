
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace SirAwesome
{
	public class UI
		: Sce.PlayStation.HighLevel.GameEngine2D.Node
	{
		public Node HealthNode { get; set; }
		public Node BeerNode { get; set; }
		public Node CoinsNode { get; set; }
		public SpriteTile HealthPanel { get; set; }
		public SpriteTile BeerPanel { get; set; }
		public SpriteTile CoinsPanel { get; set; }
		public SpriteUV HealthBar { get; set; }
		public SpriteUV BeerBar { get; set; }
		public SpriteTile HealthBarBG { get; set; }
		public SpriteTile BeerBarBG { get; set; }
		public Label CoinsLabel { get; set; }
		public Label CoinsLabelShadow { get; set; }
		public Title Title { get; set; }
		public Font Font { get; set; }
		public FontMap FontMap { get; set; }
		
		public float FallSpeed { get; set; }
		public float HangDown { get; set; }
		public float HangDownTarget { get; set; }
		public float HangDownSpeed { get; set; }
		public float Shake { get; set; }
		public int FrameCount { get; set; }
		private float HealthSway { get; set; }
		private float BeerSway { get; set; }
		private float CoinsSway { get; set; }
		
		public UI()
		{
			this.AdHocDraw += this.Draw;
			
			HealthNode = new Sce.PlayStation.HighLevel.GameEngine2D.Node();
			BeerNode = new Sce.PlayStation.HighLevel.GameEngine2D.Node();
			CoinsNode = new Sce.PlayStation.HighLevel.GameEngine2D.Node();
			
			CoinsLabel = new Label();
			CoinsLabelShadow = new Label();
			
			HealthBarBG = Support.TiledSpriteFromFile("/Application/assets/ui_gauge_fill.png", 1, 3);
			BeerBarBG = Support.TiledSpriteFromFile("/Application/assets/ui_gauge_fill.png", 1, 3);
			HealthBar = Support.SpriteUVFromFile("/Application/assets/ui_gauge_fill.png");
			BeerBar = Support.SpriteUVFromFile("/Application/assets/ui_gauge_fill.png");
			
			float third = 1.0f / 3.0f;
			HealthBar.UV.T = new Vector2(0.0f, third * 1.0f);
			BeerBar.UV.T = new Vector2(0.0f, third * 2.0f);
			HealthBar.UV.S = new Vector2(1.0f, third);
			BeerBar.UV.S = new Vector2(1.0f, third);
			
			HealthPanel = Support.TiledSpriteFromFile("/Application/assets/ui_panels.png", 3, 1);
			BeerPanel = Support.TiledSpriteFromFile("/Application/assets/ui_panels.png", 3, 1);
			CoinsPanel = Support.TiledSpriteFromFile("/Application/assets/ui_panels.png", 3, 1);
			
			HealthPanel.TileIndex2D = new Vector2i(0, 0);
			BeerPanel.TileIndex2D = new Vector2i(1, 0);
			CoinsPanel.TileIndex2D = new Vector2i(2, 0);
			
			float pivot_height = 750.0f;
			HealthNode.Pivot = HealthPanel.TextureInfo.TextureSizef / 6.0f + Vector2.UnitY * pivot_height;
			BeerNode.Pivot = BeerPanel.TextureInfo.TextureSizef / 6.0f + Vector2.UnitY * pivot_height;
			CoinsNode.Pivot = CoinsPanel.TextureInfo.TextureSizef / 6.0f + Vector2.UnitY * pivot_height;
			
			HealthPanel.VertexZ = 0.05f;
			
			Title = new Title();
			
			Font = new Font("/Application/assets/fonts/IndieFlower.ttf", 72, FontStyle.Bold);
			FontMap = new FontMap(Font);
			
			CoinsLabel.FontMap = FontMap;
			CoinsLabelShadow.FontMap = FontMap;
			
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(this, Tick, 0.0f, false);
		}
		
		public void TitleMode()
		{
			this.RemoveChild(CoinsNode, true);
			this.RemoveChild(BeerNode, true);
			this.RemoveChild(HealthNode, true);
			CoinsNode.RemoveChild(CoinsLabel, true);
			CoinsNode.RemoveChild(CoinsLabelShadow, true);
			CoinsNode.RemoveChild(CoinsPanel, true);
			BeerNode.RemoveChild(BeerPanel, true);
			BeerNode.RemoveChild(BeerBar, true);
			BeerNode.RemoveChild(BeerBarBG, true);
			HealthNode.RemoveChild(HealthPanel, true);
			HealthNode.RemoveChild(HealthBar, true);
			HealthNode.RemoveChild(HealthBarBG, true);
			this.AddChild(Title);

			Support.MusicSystem.Instance.Play("game_title_screen.mp3");
		}
		
		public void GameMode()
		{
			this.RemoveChild(Title, true);
			HealthNode.AddChild(HealthBarBG);
			HealthNode.AddChild(HealthBar);
			HealthNode.AddChild(HealthPanel);
			BeerNode.AddChild(BeerBarBG);
			BeerNode.AddChild(BeerBar);
			BeerNode.AddChild(BeerPanel);
			CoinsNode.AddChild(CoinsPanel);
			CoinsNode.AddChild(CoinsLabelShadow);
			CoinsNode.AddChild(CoinsLabel);
			this.AddChild(HealthNode);
			this.AddChild(BeerNode);
			this.AddChild(CoinsNode);
			
			Shake = 0.0f;
			FallSpeed = 0.0f;
			HangDown = 0.0f;			
			HangDownTarget = 1.0f;
			HangDownSpeed = 0.175f;

			Support.MusicSystem.Instance.Stop("game_title_screen.mp3");
		}
		
		public void Tick(float dt)
		{
			Shake *= 0.98f;
			if (HangDown < 1.0f)
			{
				FallSpeed += 0.0025f;
				Shake += 0.05f;
			}
			
			FallSpeed *= 0.9f;	
			HangDown += FallSpeed;
			HangDown = FMath.Lerp(HangDown, HangDownTarget, HangDownSpeed);
			
			if (Shake > 0.01f)
			{
				HealthSway = FMath.Lerp(HealthSway, FMath.Sin(FrameCount * 0.10f) * 0.025f * Shake, 0.25f);
				BeerSway = FMath.Lerp(BeerSway, FMath.Sin(FrameCount * 0.15f) * 0.025f * Shake, 0.25f);
				CoinsSway = FMath.Lerp(CoinsSway, FMath.Cos(FrameCount * 0.11f) * 0.025f * Shake, 0.25f);
			}
			else
			{
				HealthSway = FMath.Lerp(HealthSway, 0.0f, 0.01f);
				BeerSway = FMath.Lerp(BeerSway, 0.0f, 0.01f);
				CoinsSway = FMath.Lerp(CoinsSway, 0.0f, 0.01f);
			}
		
			FrameCount += 1;
		}
		
		public new void Draw()
		{
			Vector2 topleft = Director.Instance.CurrentScene.Camera.CalcBounds().Point01;
			
			float hanging_base = -0.0f;
			float fall_distance = -130.0f;
			float hanging = hanging_base + fall_distance * HangDown;
			
			Vector2 health_pos = topleft + new Vector2(960.0f / 3.0f * 0.0f + HealthSway, hanging);
			Vector2 beer_pos = topleft + new Vector2(960.0f / 3.0f * 1.0f + BeerSway, hanging);
			Vector2 coins_pos = topleft + new Vector2(960.0f / 3.0f * 2.0f + CoinsSway, hanging);
			
			Vector2 bar_offset = new Vector2(70.0f, 32.0f);
			
			HealthBarBG.Position = bar_offset;
			HealthBar.Position = bar_offset;
			
			HealthNode.Position = health_pos;
			HealthNode.Rotation = Vector2.UnitX.Rotate(HealthSway);
			
			BeerBarBG.Position = bar_offset;
			BeerBar.Position = bar_offset;
			
			BeerNode.Position = beer_pos;
			BeerNode.Rotation = Vector2.UnitX.Rotate(BeerSway);
			
			CoinsLabel.Position = new Vector2(120.0f, 20.0f);
			CoinsLabelShadow.Position = CoinsLabel.Position + new Vector2(2.0f, -2.0f);
			
			CoinsNode.Position = coins_pos;
			CoinsNode.Rotation = Vector2.UnitX.Rotate(CoinsSway);
			
			float health = 0.0f;
			float beer = 0.0f;
			int coins = 0;
			
			if (Game.Instance.Player != null)
			{
				health = Game.Instance.Player.Health / 5.0f;
				beer = Game.Instance.Player.Beer / 4.0f;
				coins = Game.Instance.Player.Coins;
			}
			
			float third = 1.0f / 3.0f;
			HealthBar.Scale = new Vector2(health, third);
			BeerBar.Scale = new Vector2(beer, third);
			HealthBar.UV.S = new Vector2(health, third);
			BeerBar.UV.S = new Vector2(beer, third);
			
			CoinsLabel.Color = Support.Color(255, 255, 0, 255);
			CoinsLabelShadow.Color = Support.Color(0, 0, 0, 255);
			
			CoinsLabel.Text = String.Format("{0}", coins);
			CoinsLabelShadow.Text = CoinsLabel.Text;
		}
	}
}
