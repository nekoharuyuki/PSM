
using System;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class PlayerTouchAttack
		: GameEntity
	{
		public float Damage { get; set; }
		
		public PlayerTouchAttack()
		{
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = this,
				center = () => Position,
				radius = () => 32.0f,
			});
			
			Damage = 0.5f;
		}
		
		public override void CollideTo(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemySlime))
			{
				Vector2 ofs = GetCollisionCenter(owner) - Position;
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += Vector2.UnitY * 8.0f;
				owner.InvincibleTime = 0.10f;
				owner.TakeDamage(Damage, Position);
			}
			
			if (type == typeof(EnemyRedSlime))
			{
				Vector2 ofs = GetCollisionCenter(owner) - Position;
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += Vector2.UnitY * 8.0f;
				owner.InvincibleTime = 0.10f;
				owner.TakeDamage(Damage, Position);
			}
			
			if (type == typeof(EnemyZombie))
			{
				Vector2 ofs = GetCollisionCenter(owner) - Position;
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += Vector2.UnitY * 8.0f;
				owner.InvincibleTime = 0.10f;
				owner.TakeDamage(Damage, Position);
			}
			
			if (type == typeof(EnemyBat))
			{
				Vector2 ofs = GetCollisionCenter(owner) - Position;
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += Vector2.UnitY * 8.0f;
				owner.InvincibleTime = 0.10f;
				owner.TakeDamage(Damage, Position);
			}
		}
		
		public override void Tick(float dt)
		{
			base.Tick(dt);
			
			if (FrameCount > 4)
				Die(null, 0.0f);
		}
	}
}
