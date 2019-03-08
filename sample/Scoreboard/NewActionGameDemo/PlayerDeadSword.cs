
namespace SirAwesome
{
	public class PlayerDeadSword
		: PhysicsGameEntity
	{
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
		public float RotationSpeed { get; set; }
		public float CustomRotation { get; set; }
		
		public PlayerDeadSword()
		{
			Sprite = Support.TiledSpriteFromFile("/Application/assets/dead_sword.png", 1, 1);
			//Sprite.Pivot = Sprite.TextureInfo.Sizef * 0.5f;
			Sprite.Pivot = Sprite.TextureInfo.TextureSizef * 0.5f;
			
			this.AddChild(Sprite);
		}
		
		public override void Tick(float dt)
		{
			base.Tick(dt);	
			
			if (AirborneTime > 0.0f)
			{
				Sprite.Rotation = Sprite.Rotation.Rotate(0.22f);
			}
		}
	};
}
