
using System;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class PlayerGlassAttack
		: PhysicsGameEntity
	{
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
		public float Damage { get; set; }
		public bool Exploded { get; set; }
		public float CustomRotation { get; set; }
		public float Radius { get; set; }
		
		public PlayerGlassAttack()
		{
			Sprite = Support.SpriteFromFile("/Application/assets/glass_frames.png");
			this.AddChild(Sprite);
			
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite),
				radius = () => Radius,
			});
			
			Sprite.Pivot = Sprite.TextureInfo.TextureSizef / 2.0f;
			
			Damage = 4.0f;
			//InvincibleTime = 60.0f;
			Radius = 16.0f;
		}
		
		public override void CollideTo(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemySlime) ||
			    type == typeof(EnemyRedSlime) ||
				type == typeof(EnemyZombie) ||
				type == typeof(EnemyBat))
			{
				Vector2 ofs = GetCollisionCenter(owner) - GetCollisionCenter(Sprite);
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				Vector2 dir = ofs.Normalize();
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += dir * 6.0f + Vector2.UnitY * 16.0f;
				owner.InvincibleTime = 0.25f;
				owner.TakeDamage(Damage, GetCollisionCenter(Sprite));
				
				if (!Exploded)
					Explode();
			}
		}
		
		public override void Tick(float dt)
		{
			base.Tick(dt);
			Velocity += Vector2.UnitY * -0.025f;
			Sprite.Rotation = Vector2.Rotation(CustomRotation);
			CustomRotation += 0.275f;
			
			if (Exploded)
				Die(null, 1.0f);
				
			if (AirborneTime <= 0.0f)
			{
				Explode();
			}
		}
		
		public void Explode()
		{
			Exploded = true;
			InvincibleTime = 0.0f;
			Radius = 72.0f;
		}
		
		public override void Die(Vector2? source, float damage)
		{
			base.Die(source, damage);
			
			Vector2 p = GetCollisionCenter(Sprite) + Vector2.UnitY * -12.0f;
			Vector4 color_a = new Vector4(250.0f / 255.0f, 250.0f / 255.0f, 160.0f / 255.0f, 1.0f);
			Vector4 color_b = new Vector4(233.0f / 255.0f, 169.0f / 255.0f, 5.0f / 255.0f, 1.0f);
			Vector4 color_c = new Vector4(9.0f / 255.0f, 7.0f / 255.0f, 2.0f / 255.0f, 1.0f);
			Game.Instance.ParticleEffects.AddParticlesBurstRandomy(48, p, Vector2.UnitY * 6.0f, color_a, 3.5f, 2.0f);
			Game.Instance.ParticleEffects.AddParticlesBurstRandomy(16, p, Vector2.UnitY * 6.0f, color_b, 3.5f, 1.7f);
			Game.Instance.ParticleEffects.AddParticlesBurstRandomy(8, p, Vector2.UnitY * 6.0f, color_c, 2.5f, 1.7f);
			Support.SoundSystem.Instance.Play("beer_splash.wav");
		}	
	}
}
