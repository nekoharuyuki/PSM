
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace SirAwesome
{
		public enum SpriteBatchType
	{
		Slime,
		RedSlime,
		Zombie,
		Bat,
		Coin,
		Health,
		Gauge,
		Panel,
	};

	public class SpriteBatch
		: Node
	{
		public struct SpriteItem
		{
			public Vector2 position;
			public Vector2i tile;
			public bool flip_u;
		};

		public List<SpriteTile> Sprites = new List<SpriteTile>();
		public List<SpriteItem> Slimes = new List<SpriteItem>();
		public List<SpriteItem> RedSlimes = new List<SpriteItem>();
		public List<SpriteItem> Zombies = new List<SpriteItem>();
		public List<SpriteItem> Bats = new List<SpriteItem>();
		public List<SpriteItem> Coins = new List<SpriteItem>();
		public List<SpriteItem> Healths = new List<SpriteItem>();
		public List<SpriteItem> Gauges = new List<SpriteItem>();
		public List<SpriteItem> Panels = new List<SpriteItem>();

		public SpriteBatch()
		{
			PrecacheSprites();

			Sprites.Clear();
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/slime_green_frames.png", 4, 4));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/slime_red_frames.png", 4, 6));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/zombie_frames.png", 4, 2));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/coins.png", 8, 3));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/item_health.png", 1, 1));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/ui_gauge_fill.png", 1, 3));
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/ui_panels.png", 3, 1));

			AdHocDraw += this.DrawBatch;
		}

		public void PrecacheSprites()
		{
			Support.PrecacheTiledSprite("/Application/assets/slime_green_frames.png", 4, 4);
			Support.PrecacheTiledSprite("/Application/assets/slime_red_frames.png", 4, 6);
			Support.PrecacheTiledSprite("/Application/assets/zombie_frames.png", 4, 2);
			Support.PrecacheTiledSprite("/Application/assets/bat_frames.png", 2, 2);
			Support.PrecacheTiledSprite("/Application/assets/coins.png", 8, 3);
			Support.PrecacheTiledSprite("/Application/assets/item_health.png", 1, 1);
			Support.PrecacheTiledSprite("/Application/assets/dead_sword.png", 1, 1);
			Support.PrecacheTiledSprite("/Application/assets/ui_gauge_fill.png", 1, 3);
			Support.PrecacheTiledSprite("/Application/assets/ui_panels.png", 3, 1);
		}

		public void Register(SpriteBatchType type, Vector2 position, Vector2i tile, bool flip_u)
		{
			List<SpriteItem> list = null;

			switch (type)
			{
				case SpriteBatchType.Slime: list = Slimes; break;
				case SpriteBatchType.RedSlime: list = RedSlimes; break;
				case SpriteBatchType.Zombie: list = Zombies; break;
				case SpriteBatchType.Bat: list = Bats; break;
				case SpriteBatchType.Coin: list = Coins; break;
				case SpriteBatchType.Health: list = Healths; break;
				case SpriteBatchType.Gauge: list = Gauges; break;
				case SpriteBatchType.Panel: list = Panels; break;
			}

			list.Add(new SpriteItem() { position = position, tile = tile, flip_u = flip_u });
		}

		public void DrawBatch()
		{
			DrawList(Slimes, Sprites[(int)SpriteBatchType.Slime]);
			DrawList(RedSlimes, Sprites[(int)SpriteBatchType.RedSlime]);
			DrawList(Zombies, Sprites[(int)SpriteBatchType.Zombie]);
			DrawList(Bats, Sprites[(int)SpriteBatchType.Bat]);
			DrawList(Coins, Sprites[(int)SpriteBatchType.Coin]);
			DrawList(Healths, Sprites[(int)SpriteBatchType.Health]);
			//DrawList(Gauges, Sprites[(int)SpriteBatchType.Gauge]);
			//DrawList(Panels, Sprites[(int)SpriteBatchType.Panel]);
		}

		public void DrawList(List<SpriteItem> items, SpriteTile sprite)
		{
			Director.Instance.GL.SetBlendMode( sprite.BlendMode );
			sprite.Shader.SetColor( ref sprite.Color );
            sprite.Shader.SetUVTransform( ref Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.UV_TransformFlipV );

			Director.Instance.SpriteRenderer.BeginSprites(sprite.TextureInfo, sprite.Shader, items.Count);

			for (int i = 0; i < items.Count; ++i)
			{
				SpriteItem item = items[i];
				sprite.Quad.T = item.position;
				sprite.TileIndex2D = item.tile;
				Director.Instance.SpriteRenderer.FlipU = item.flip_u;
				Director.Instance.SpriteRenderer.FlipV = sprite.FlipV;
				TRS copy = sprite.Quad;
				Director.Instance.SpriteRenderer.AddSprite( ref copy, sprite.TileIndex2D );
			}

			Director.Instance.SpriteRenderer.EndSprites(); 

			items.Clear();
		}
	};
}
