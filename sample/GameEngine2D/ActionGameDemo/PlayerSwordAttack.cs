
using System;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class PlayerSwordAttack
		: GameEntity
	{
		public float Damage { get; set; }
		
		public PlayerSwordAttack()
		{
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = this,
				center = () => this.GetAttackCenter(),
				radius = () => 58.0f,
			});
			
			Damage = 1.0f;
		}

		public Vector2 GetAttackCenter()
		{
			if (Parent == null) // collision delegate gets called
			{
//				System.Console.WriteLine( "Common.FrameCount + " PlayerSwordAttack.GetAttackCenter called but Parent is null" );
				return Sce.PlayStation.HighLevel.GameEngine2D.Base.Math._00;
			}
			Vector2 center = GetCollisionCenter((Parent as Player).BodySprite);
			Vector2 offset = new Vector2(34.0f, -4.0f);
			Player owner = Parent as Player;
			if (owner.BodySprite.FlipU)
				offset *= new Vector2(-1.0f, 1.0f);
			return center + offset;
		}
		
		public override void CollideTo(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);

			Player player = Parent as Player;	
			SpriteTile sprite = player.BodySprite;
			Vector2 center = GetCollisionCenter(sprite);
			
			Type type = owner.GetType();
			if (type == typeof(EnemySlime))
			{
				Vector2 ofs = GetCollisionCenter(owner) - GetCollisionCenter(Parent as Player);
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				Vector2 dir = ofs.Normalize();
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += dir * new Vector2(4.0f, 0.25f) + Vector2.UnitY * 8.0f;
				owner.InvincibleTime = 0.25f;
				owner.TakeDamage(Damage, GetCollisionCenter((Parent as Player).BodySprite));
				Support.SoundSystem.Instance.Play("player_sword_hit_enemy.wav");
			}
			
			if (type == typeof(EnemyRedSlime))
			{
				Vector2 ofs = GetCollisionCenter(owner) - GetCollisionCenter(Parent as Player);
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				Vector2 dir = ofs.Normalize();
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += dir * 8.0f + Vector2.UnitY * 8.0f;
				owner.InvincibleTime = 0.25f;
				owner.TakeDamage(Damage, GetCollisionCenter((Parent as Player).BodySprite));
				Support.SoundSystem.Instance.Play("player_sword_hit_enemy.wav");
			}
			
			if (type == typeof(EnemyZombie))
			{
				Vector2 ofs = GetCollisionCenter(owner) - GetCollisionCenter(Parent as Player);
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				Vector2 dir = ofs.Normalize();
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += dir * 6.0f + Vector2.UnitY * 5.0f;
				owner.InvincibleTime = 0.25f;
				owner.TakeDamage(Damage, GetCollisionCenter((Parent as Player).BodySprite));
				Support.SoundSystem.Instance.Play("player_sword_hit_enemy.wav");
			}
			
			if (type == typeof(EnemyBat))
			{
				Vector2 ofs = GetCollisionCenter(owner) - GetCollisionCenter(Parent as Player);
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				Vector2 dir = ofs.Normalize();
				PhysicsGameEntity p = owner as PhysicsGameEntity;
				p.Velocity += dir * 6.0f + Vector2.UnitY * 5.0f;
				owner.InvincibleTime = 0.25f;
				owner.TakeDamage(Damage, GetCollisionCenter((Parent as Player).BodySprite));
				Support.SoundSystem.Instance.Play("player_sword_hit_enemy.wav");
			}
		}
	}

}
