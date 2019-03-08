
using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class EnemyRedSlime
        : PhysicsGameEntity
    {
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
        public Support.AnimationAction IdleAnimation { get; set; }
        public Support.AnimationAction RollAnimation { get; set; }
        public Support.AnimationAction BombAnimation { get; set; }
        public Support.AnimationAction BombAnimationOn { get; set; }
        public Support.AnimationAction BombAnimationOff { get; set; }
        public Sequence RollToIdleAction { get; set; }
        public Sequence RollToBombAction { get; set; }
        public Sequence BombAnimationSequence { get; set; }
        public float MoveDelay { get; set; }
        public float MoveTime { get; set; }
        public int MoveDirection { get; set; }
        public bool IsExploding { get; set; }
        
        public EnemyRedSlime()
        {
            Sprite = Support.TiledSpriteFromFile("/Application/assets/slime_red_frames.png", 4, 6);
            this.AddChild(Sprite);
            
			IdleAnimation = new Support.AnimationAction(Sprite, 0, 4, 0.6f, looping: true);
			RollAnimation = new Support.AnimationAction(Sprite, 4, 12, 0.7f, looping: false);
			BombAnimation = new Support.AnimationAction(Sprite, 4, 12, 0.7f, looping: false);
			
			BombAnimationOn = new Support.AnimationAction(Sprite, 0, 1, 0.1f, looping: false);
			BombAnimationOff = new Support.AnimationAction(Sprite, 12, 13, 0.1f, looping: false);
			
			RollToIdleAction = new Sequence();
			RollToIdleAction.Add(RollAnimation);
			RollToIdleAction.Add(new DelayTime() { Duration = 0.05f });
			RollToIdleAction.Add(RollAnimation);
			RollToIdleAction.Add(new DelayTime() { Duration = 0.05f });
			RollToIdleAction.Add(IdleAnimation);
			
			RollToBombAction = new Sequence();
			RollToBombAction.Add(RollAnimation);
			RollToBombAction.Add(new DelayTime() { Duration = 0.05f });
			RollToBombAction.Add(RollAnimation);
			RollToBombAction.Add(new DelayTime() { Duration = 0.05f });
			RollToBombAction.Add(BombAnimation);
			
			BombAnimationSequence = new Sequence();
			float delay = 1.0f / 60.0f * 5.0f;
			for (int i = 0; i < 12; ++i)
			{
				BombAnimationSequence.Add(BombAnimationOn);
				BombAnimationSequence.Add(new DelayTime() { Duration = delay });
				BombAnimationSequence.Add(BombAnimationOff);
				BombAnimationSequence.Add(new DelayTime() { Duration = delay });
				delay -= 1.0f / 60.0f * 4.5f;
			}
			
			BombAnimationSequence.Add(new CallFunc(this.Explode));
			BombAnimationSequence.Add(new CallFunc(() => this.Die(GetCollisionCenter(Sprite) + -Vector2.UnitY, 9.0f)));
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite) + new Vector2(0.0f, -2.0f),
				radius = () => 24.0f,
			});
			
			GroundFriction = new Vector2(0.65f);
			Health = 4.0f;
			
			Sprite.RunAction(IdleAnimation);
        }
        
        public override void Tick(float dt)
        {
        	base.Tick(dt);

            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(SpriteBatchType.RedSlime, this.Position, Sprite.TileIndex2D, Sprite.FlipU);
			
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
			
			if (MoveTime > 0.0f)
			{
				Velocity += new Vector2(MoveDirection * 1.25f, 0.0f);
			}
			
			if (MoveDelay <= 0.0f)
			{
	            Vector2 offset = Game.Instance.Player.LocalToWorld(Vector2.Zero) - LocalToWorld(Vector2.Zero);
				MoveDirection = FMath.Sign(offset.X);
				
				MoveTime = 1.45f;
				MoveDelay = 3.0f;
				
				Sprite.StopAllActions();
				Sprite.RunAction(RollToIdleAction);
				Support.SoundSystem.Instance.Play("red_slime_roll.wav");
			}
        }
        
		public override void TakeDamage(float damage, Vector2? source)
		{
			base.TakeDamage(damage, source);
			SpawnDamageParticles(GetCollisionCenter(Sprite), (Vector2)source, damage, Support.Color(180, 40, 43));
			MoveTime = 0.0f;
			MoveDelay = 30.0f;
			
			if (Health <= 0.0f)
				return;
			
			if (IsExploding)
				return;
			
			Sprite.StopAllActions();
			Sprite.RunAction(BombAnimationSequence);
			
			IsExploding = true;
			Support.SoundSystem.Instance.Play("red_slime_explode.wav");
		}
		
		public override void Die(Vector2? source, float damage)
		{
			base.Die(source, damage);
			Vector2 offset = (GetCollisionCenter(Sprite) - (Vector2)source);
			if (offset.LengthSquared() > 0.0f)
				offset = offset.Normalize() * 4.0f;	
			Game.Instance.ParticleEffects.AddParticlesTile("EnemyRedSlime", Support.GetTileIndex(Sprite), Sprite.FlipU, GetCollisionCenter(Sprite), offset + Vector2.UnitY * 3.0f, damage * 2.0f);
			DropCoinsWithAChanceOfHeart(GetCollisionCenter(Sprite), 3);
		}
		
		public void Explode()
		{
			if (Health <= 0.0f)
				return;
			
			var explosion = new EnemyRedSlimeExplosion();
			explosion.Position = GetCollisionCenter(Sprite) + Vector2.UnitY * -10.0f;
			Game.Instance.World.AddChild(explosion);
		}
    }
}
