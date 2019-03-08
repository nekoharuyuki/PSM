
using System.Collections.Generic;

using Sce.PlayStation.Core;

namespace SirAwesome
{
	public class Coin
		: PhysicsGameEntity
	{
		public int index;
		public static LinkedList<Coin> Cache { get; set; }
		
		public static void InitializeCache()
		{
			Cache = new LinkedList<Coin>();
			for (int i = 0; i < 32; ++i)
			{
				Coin coin = new Coin();
				coin.index = i;
				coin.Scale = new Vector2(1.5f);
				coin.StopAllActions();
				coin.UnscheduleAll();
				Cache.AddFirst(coin);
			}
		}
		
		public static void Spawn(Vector2 position)
		{
			foreach (var c in Cache)
			{
				if (c.Parent != null)
					return;
			}
			
			if (Cache.Count == 0)
				return;
				
			Coin coin = Cache.First.Value;
			Cache.RemoveFirst();
			
			coin.Position = position;
			coin.Velocity = new Vector2(
				Game.Instance.Random.NextSignedFloat() * 0.5f,
				Game.Instance.Random.NextSignedFloat() * 2.5f + 10.0f
			);
			coin.FrameCount = 0;
			
			coin.Sprite.StopAllActions();
			coin.Sprite.RunAction(coin.Animation);
			
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.UnscheduleAll(coin);
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(coin, coin.Tick, 0.0f, false);
			Game.Instance.World.AddChild(coin);

			Support.SoundSystem.Instance.Play("coin_spawn.wav");
		}
		
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
		public Support.AnimationAction Animation { get; set; }
		
		public Coin()
		{
            Sprite = Support.TiledSpriteFromFile("/Application/assets/coins.png", 8, 3);
			Animation = new Support.AnimationAction(Sprite, 0, 8, 0.7f, looping: true);
			this.AddChild(Sprite);
			
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite) + new Vector2(0.0f, -8.0f),
				radius = () => 22.0f,
			});
		}
		
		public override void Tick(float dt)
		{
            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(SpriteBatchType.Coin, this.Position, Sprite.TileIndex2D, Sprite.FlipU);

			base.Tick(dt);
			
			if (FrameCount > 60 * 4)
				Die(null, 0.0f);
		}
		
		public override void Die(Vector2? source, float damage)
		{
			Sprite.StopAllActions();	
			Sprite.RunAction(new Support.AnimationAction(Sprite, 0, 8, 0.3f, looping: true));
			Cache.AddFirst(this);
			base.Die(source, damage);
		}
	};
}
