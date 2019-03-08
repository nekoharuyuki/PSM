
using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
    public class EnemySlime
        : PhysicsGameEntity
    {
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
        public Support.AnimationAction IdleAnimation { get; set; }
        public Support.AnimationAction JumpInAnimation { get; set; }
        public Support.AnimationAction JumpMidAnimation { get; set; }
        public Support.AnimationAction JumpOutAnimation { get; set; }
        public Sequence JumpAnimationSequence { get; set; }
        
		public float MoveDelay { get; set; }

        public EnemySlime()
        {
            Sprite = Support.TiledSpriteFromFile("/Application/assets/slime_green_frames.png", 4, 4);
			
			IdleAnimation = new Support.AnimationAction(Sprite, 0, 8, 0.5f, looping: true);
			JumpInAnimation = new Support.AnimationAction(Sprite, 8, 12, 0.3f, looping: false);
			JumpMidAnimation = new Support.AnimationAction(Sprite, 12, 13, 1.0f, looping: false);
			JumpOutAnimation = new Support.AnimationAction(Sprite, 13, 16, 0.2f, looping: false);
			
			JumpAnimationSequence = new Sequence();
			JumpAnimationSequence.Add(JumpInAnimation);
			JumpAnimationSequence.Add(new CallFunc(this.Jump));
			JumpAnimationSequence.Add(JumpMidAnimation);
			JumpAnimationSequence.Add(new DelayTime() { Duration = 0.40f });
			JumpAnimationSequence.Add(JumpOutAnimation);
			JumpAnimationSequence.Add(new DelayTime() { Duration = 0.05f });
			JumpAnimationSequence.Add(IdleAnimation);
            
            this.AddChild(Sprite);
            Sprite.RunAction(IdleAnimation);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite) + new Vector2(0.0f, -8.0f),
				radius = () => 24.0f,
			});
			
			GroundFriction = new Vector2(0.85f);
			Health = 2.0f;
			MoveDelay = 3.0f;
        }

        public override void Tick(float dt)
        {
        	base.Tick(dt);

            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(SpriteBatchType.Slime, this.Position, Sprite.TileIndex2D, Sprite.FlipU);

			if (InvincibleTime > 0.0f)
				return;
				
			// 0.0f will just leave the flip state as-is
			if (Velocity.X < 0.0f)
				Sprite.FlipU = true;
			if (Velocity.X > 0.0f)
				Sprite.FlipU = false;
				
			if (AirborneTime > 0.0f)
				return;
				
			MoveDelay -= dt;
			if (MoveDelay <= 0.0f)
			{
				Sprite.StopAllActions();
				Sprite.RunAction(JumpAnimationSequence);
				Support.SoundSystem.Instance.Play("green_slime_jump.wav");
				MoveDelay = 2.75f;
			}
        }
        
		public void Jump()
		{
            Vector2 offset = Game.Instance.Player.LocalToWorld(Vector2.Zero) - LocalToWorld(Vector2.Zero);
			Velocity += new Vector2(FMath.Sign(offset.X) * 3.0f, 10.0f + Game.Instance.Random.NextFloat() * 5.0f);
		}
        
		public override void TakeDamage(float damage, Vector2? source)
		{
			base.TakeDamage(damage, source);
			SpawnDamageParticles(GetCollisionCenter(Sprite), (Vector2)source, damage, Support.Color(32, 162, 99));
			MoveDelay = 3.0f;
			Sprite.StopAllActions();
			Sprite.RunAction(IdleAnimation);
			Support.SoundSystem.Instance.Play("green_slime_take_damage.wav");
		}
        
		public override void Die(Vector2? source, float damage)
		{
			base.Die(source, damage);
			Vector2 offset = (GetCollisionCenter(Sprite) - (Vector2)source);
			if (offset.LengthSquared() > 0.0f)
				offset = offset.Normalize() * 4.0f;	
			Game.Instance.ParticleEffects.AddParticlesTile("EnemySlime", Support.GetTileIndex(Sprite), Sprite.FlipU, GetCollisionCenter(Sprite), offset + Vector2.UnitY * 2.0f, damage * 2.0f);
			Support.SoundSystem.Instance.Play("green_slime_die.wav");
			DropCoinsWithAChanceOfHeart(GetCollisionCenter(Sprite), 2);
		}
    }
}
