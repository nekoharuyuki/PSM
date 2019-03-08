
using System;

using Sce.PlayStation.Core;

namespace SirAwesome
{
	public class EnemyRedSlimeExplosion
		: GameEntity
	{           
		public EnemyRedSlimeExplosion()
		{
			CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = this,
				center = () => this.Position,
				radius = () => 140.0f,
			});
			
			CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = this,
				center = () => this.Position,
				radius = () => 160.0f,
			});
		}
		
		public override void Tick(float dt)
		{
			base.Tick(dt);
			Die(null, 0.0f);
		}
		
		public override void CollideTo(GameEntity owner, Sce.PlayStation.HighLevel.GameEngine2D.Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemySlime) ||
				type == typeof(EnemyRedSlime) || 
				type == typeof(EnemyZombie) ||
				type == typeof(EnemyBat))
			{
				PhysicsGameEntity pge = owner as PhysicsGameEntity;
				Vector2 offset = pge.Position - Position;
				if (offset.LengthSquared() > 0.0f)
					offset = offset.Normalize();
				
				pge.Velocity += Vector2.UnitY * 15.0f;
				pge.Velocity += offset * 6.0f;
				
				owner.TakeDamage(1.0f, Position);
			}
		}
	}
}
