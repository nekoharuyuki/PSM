
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace SirAwesome
{
	public class GameEntity
		: Sce.PlayStation.HighLevel.GameEngine2D.Node
	{
		public float Health { get; set; }
		public float InvincibleTime { get; set; }
		public int FrameCount { get; set; }
		
		public static Vector2 GetCollisionCenter(Node node)
		{
			Bounds2 bounds = new Bounds2();
			node.GetlContentLocalBounds(ref bounds);
			Vector2 center = node.LocalToWorld(bounds.Center);
			return center;
		}
		
		public List<EntityCollider.CollisionEntry> CollisionDatas;
		
		public GameEntity()
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(this, Tick, 0.0f, false);
			//AdHocDraw += this.DebugDraw;
			CollisionDatas = new List<EntityCollider.CollisionEntry>();
			Health = 1.0f;
		}
		
		public void DebugDraw()
		{
			foreach (EntityCollider.CollisionEntry c in CollisionDatas)
			{
				if (c.owner != null)
				{
					Director.Instance.GL.ModelMatrix.Push();
					Director.Instance.GL.ModelMatrix.SetIdentity();
					Director.Instance.DrawHelpers.DrawCircle(c.center(), c.radius(), 32);
					Director.Instance.GL.ModelMatrix.Pop();
				}
			}
		}
		
		public virtual void Tick(float dt)
		{
			FrameCount += 1;
			
			InvincibleTime -= dt;
			InvincibleTime = System.Math.Max(0.0f, InvincibleTime);
			
			if (InvincibleTime <= 0.0f)
			{
				foreach (EntityCollider.CollisionEntry c in CollisionDatas)
				{
					if (c.owner != null)
						Game.Instance.Collider.Add(c);
				}
			}
		} 
		
		public virtual void CollideTo(GameEntity owner, Node collider) { }
		public virtual void CollideFrom(GameEntity owner, Node collider) { }
		
		public void SpawnDamageParticles(Vector2 position, Vector2 source, float damage, Vector4 color)
		{
			//if (Health <= 0.0f)
				//return;
				
			Vector2 dir = position - source;
			if (dir.LengthSquared() > 0.0f)
				dir = dir.Normalize();
			dir *= 0.25f;
			int particles = (int)(damage * 4.0f);
			float jitter = 1.5f * damage;
			Game.Instance.ParticleEffects.AddParticlesBurst(particles, position, dir * damage * 4.0f + Vector2.UnitY * 2.0f, color, jitter, 1.0f);
		}
		
		public void DropCoinsWithAChanceOfHeart(Vector2 position, int count)
		{
			for (int i = 0; i < count; ++i)
				Coin.Spawn(position);
				
			// chance of heart
			if (Game.Instance.Random.NextFloat() < 0.06f)
			{
				var heart = new Heart() { Position = position };
				Game.Instance.World.AddChild(heart);
			}
		}
		
		public virtual void TakeDamage(float damage, Vector2? source)
		{
			Health -= damage;
			if (Health <= 0.0f)
				Die(source, damage);
		}
		
		public virtual void Die(Vector2? source, float damage)
		{
			Game.Instance.World.RemoveChild(this, true);
		}
	};
}
