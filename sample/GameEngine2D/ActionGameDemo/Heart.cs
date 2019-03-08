
using Sce.PlayStation.Core;

namespace SirAwesome
{
	public class Heart
		: PhysicsGameEntity
	{
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
		
		public Heart()
		{
            Sprite = Support.TiledSpriteFromFile("/Application/assets/item_health.png", 1, 1);
			this.AddChild(Sprite);
			
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite) + new Vector2(0.0f, 0.0f),
				radius = () => 32.0f,
			});
		}
		
		public override void Tick(float dt)
		{
            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(SpriteBatchType.Health, this.Position, Sprite.TileIndex2D, Sprite.FlipU);

			base.Tick(dt);
			
			if (FrameCount > 60 * 4)
				Die(null, 0.0f);
		}
	};
}

