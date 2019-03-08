
using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
    public class EnemyZombie
        : PhysicsGameEntity
    {
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
        public Support.AnimationAction IdleAnimation { get; set; }
        public Support.AnimationAction WalkAnimation { get; set; }
        public Sequence WalkToIdleSequence { get; set; }
        
        public float MoveDelay { get; set; }
        public float MoveTime { get; set; }
		public int MoveDirection { get; set; }
        
        public EnemyZombie()
        {
            Sprite = Support.TiledSpriteFromFile("/Application/assets/zombie_frames.png", 4, 2);
            this.AddChild(Sprite);
            
			IdleAnimation = new Support.AnimationAction(Sprite, 6, 8, 3.0f, looping: true);
			WalkAnimation = new Support.AnimationAction(Sprite, 0, 7, 1.0f, looping: false);
			
			WalkToIdleSequence = new Sequence();
			WalkToIdleSequence.Add(WalkAnimation);
			WalkToIdleSequence.Add(new DelayTime() { Duration = 0.05f });
			WalkToIdleSequence.Add(IdleAnimation);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite),
				radius = () => 24.0f,
			});
			
			GroundFriction = new Vector2(0.5f);
			Health = 5.0f;
			
			Sprite.RunAction(IdleAnimation);
        }

        public override void Tick(float dt)
        {
        	base.Tick(dt);

            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(SpriteBatchType.Zombie, this.Position, Sprite.TileIndex2D, Sprite.FlipU);
			
			if (InvincibleTime > 0.0f)
				return;
				
			// 0.0f will just leave the flip state as-is
			if (Velocity.X < 0.0f)
				Sprite.FlipU = true;
			if (Velocity.X > 0.0f)
				Sprite.FlipU = false;
				
			if (AirborneTime > 0.0f)
				return;
				
			MoveTime -= dt;
			MoveDelay -= dt;
			
			if (MoveTime > 0.0f && MoveTime < 0.5f)
			{
				//Velocity += new Vector2(MoveDirection * 0.25f, 0.0f);
				Velocity = new Vector2(MoveDirection * 1.5f, 0.0f);
			}
			
			if (MoveDelay <= 0.0f)
			{
				Vector2 player_position = Position;
				if (Game.Instance.Player != null)
		            player_position = Game.Instance.Player.LocalToWorld(Vector2.Zero);
	            Vector2 offset = player_position - LocalToWorld(Vector2.Zero);
				MoveDirection = FMath.Sign(offset.X);
				
				MoveTime = 0.65f;
				MoveDelay = 2.0f + Game.Instance.Random.NextFloat() * 1.0f;;
				
				Sprite.StopAllActions();
				Sprite.RunAction(WalkToIdleSequence);

				Support.SoundSystem.Instance.Play("zombie_shuffle.wav");
			}
        }
		
		public override void TakeDamage(float damage, Vector2? source)
		{
			base.TakeDamage(damage, source);
			SpawnDamageParticles(GetCollisionCenter(Sprite), (Vector2)source, damage, Support.Color(117, 168, 130));
			MoveTime = 0.0f;
			MoveDelay = 2.0f;
			Sprite.StopAllActions();
			Sprite.RunAction(IdleAnimation);
			Support.SoundSystem.Instance.Play("zombie_take_damage.wav");
		}
		
		public override void Die(Vector2? source, float damage)
		{
			base.Die(source, damage);
			Vector2 offset = (GetCollisionCenter(Sprite) - (Vector2)source);
			if (offset.LengthSquared() > 0.0f)
				offset = offset.Normalize() * 4.0f;	
			Game.Instance.ParticleEffects.AddParticlesTile("EnemyZombie", Support.GetTileIndex(Sprite), Sprite.FlipU, GetCollisionCenter(Sprite), offset + Vector2.UnitY * 4.0f, damage * 2.0f);
			Support.SoundSystem.Instance.Play("zombie_die.wav");
			DropCoinsWithAChanceOfHeart(GetCollisionCenter(Sprite), 5);
		}
    }
}
