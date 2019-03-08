
using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class EnemyBat
        : PhysicsGameEntity
    {
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
        public Support.AnimationAction FlyAnimation { get; set; }
        public Support.AnimationAction DiveAnimation { get; set; }
        public Support.AnimationAction PreDiveAnimation { get; set; }
        public Sce.PlayStation.HighLevel.GameEngine2D.Sequence PreDiveSequence { get; set; }
        
        public float MoveDelay { get; set; }
        public float MoveTime { get; set; }
		public int MoveDirection { get; set; }
		
		public float DiveTime { get; set; }
		
		public float HoverHeight = 300.0f;
		public float RiseHeight = 240.0f;

        public EnemyBat()
        {
            Sprite = Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2);
			AddChild(Sprite);
            
			FlyAnimation = new Support.AnimationAction(Sprite, 0, 4, 0.3f, looping: true);
			DiveAnimation = new Support.AnimationAction(Sprite, 0, 1, 1.0f, looping: false);
			PreDiveAnimation = new Support.AnimationAction(Sprite, 0, 4, 0.22f, looping: true);
			PreDiveSequence = new Sequence();
			PreDiveSequence.Add(new DelayTime() { Duration = 1.0f });
			PreDiveSequence.Add(new CallFunc(() => { Sprite.StopAllActions(); Sprite.RunAction(DiveAnimation); }));
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite),
				radius = () => 16.0f,
			});
			
			GroundFriction = new Vector2(0.5f);
			AirFriction = new Vector2(0.98f);
			Health = 0.6f;
			
			Sprite.RunAction(FlyAnimation);
        }

        public override void Tick(float dt)
        {
            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(SpriteBatchType.Bat, this.Position, Sprite.TileIndex2D, Sprite.FlipU);

	        // reduce gravity
			Velocity += Vector2.UnitY * 0.45f;
			
        	base.Tick(dt);
			
			if (InvincibleTime > 0.0f)
				return;
				
			// 0.0f will just leave the flip state as-is
			if (Velocity.X < 0.0f)
				Sprite.FlipU = true;
			if (Velocity.X > 0.0f)
				Sprite.FlipU = false;
				
			MoveTime -= dt;
			MoveDelay -= dt;
			
			if (FrameCount % 60 == 0)
			{
				if (DiveTime <= 0.0f)
				{
					Support.SoundSystem.Instance.PlayNoClobber("bat_fly.wav");
				}

				if (Game.Instance.Random.Next() % 4 == 0)
				{
					HoverHeight = 300.0f + Game.Instance.Random.NextSignedFloat() * 10.0f * 10.0f;
				}
			}
				
			float velocity_x = FMath.Lerp(Velocity.X, MoveDirection * 4.0f, 0.015f);
			float velocity_y = Velocity.Y;
			
			if (DiveTime > 0.0f)
			{
				if (DiveTime > 2.0f)
				{
					velocity_x *= 0.9f;
					velocity_y *= 0.9f;
					velocity_y += 0.1f;		
				}
				else if (DiveTime > 1.0f)
				{
					velocity_y += -0.30f;		
				}
				else
				{
					velocity_y += 0.06f;		
				}
				
				DiveTime -= dt;
				if (DiveTime <= 0.0f)
				{
					Sprite.StopAllActions();
					Sprite.RunAction(FlyAnimation);
				}
			}
			else
			{
				if (Position.Y > HoverHeight)
					velocity_y += -0.075f;
					
				if (Position.Y < RiseHeight)
					velocity_y += 0.2f;
				
				velocity_y += 0.075f * FMath.Sin(FrameCount * 0.015f) * FMath.Sin(FrameCount * 0.04f);
			}
			
			Velocity = new Vector2(velocity_x, velocity_y);
			
			if (MoveDelay <= 0.0f)
			{
	            Vector2 offset = Game.Instance.Player.LocalToWorld(Vector2.Zero) - LocalToWorld(Vector2.Zero);
				MoveDirection = FMath.Sign(offset.X);
				
				if (Game.Instance.Random.Next() % 3 == 0)
				{
					DiveTime = 3.0f;
					Sprite.StopAllActions();
					Sprite.RunAction(PreDiveAnimation);
					Sprite.RunAction(PreDiveSequence);
				}
				
				MoveTime = 1.75f + Game.Instance.Random.NextFloat() * 1.25f;
				MoveDelay = MoveTime;
			}
        }
		
		public override void TakeDamage(float damage, Vector2? source)
		{
			base.TakeDamage(damage, source);
			SpawnDamageParticles(GetCollisionCenter(Sprite), (Vector2)source, damage, Support.Color(108, 71, 22));
			MoveTime = 0.0f;
			MoveDelay = 2.0f;
			Sprite.StopAllActions();
			Sprite.RunAction(FlyAnimation);
			Support.SoundSystem.Instance.Play("bat_take_damage.wav");
		}
		
		public override void Die(Vector2? source, float damage)
		{
			base.Die(source, damage);
			Vector2 offset = (GetCollisionCenter(Sprite) - (Vector2)source);
			if (offset.LengthSquared() > 0.0f)
				offset = offset.Normalize() * 4.0f;	
			Game.Instance.ParticleEffects.AddParticlesTile("EnemyBat", Support.GetTileIndex(Sprite), Sprite.FlipU, GetCollisionCenter(Sprite), offset + Vector2.UnitY * 4.0f, damage * 2.0f);
			Support.SoundSystem.Instance.PlayNoClobber("bat_die.wav");
			DropCoinsWithAChanceOfHeart(GetCollisionCenter(Sprite), 5);
		}
	}
}
