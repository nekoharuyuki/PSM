
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace SirAwesome
{
	public class EntityCollider
	{
		public enum CollisionEntityType
		{
			Player,
			Enemy,
			//Weapon,
			//Spell,
			//Item,
		}
		
		public delegate Vector2 GetCenterDelegate();
		public delegate float GetRadiusDelegate();
		// GetForceVector()?
		
		public struct CollisionEntry
		{
			public CollisionEntityType type;
			public GameEntity owner;
			public Node collider;
			public GetCenterDelegate center;
			public GetRadiusDelegate radius;
		}
		
		List<List<CollisionEntry>> typed_entries;
		
		public EntityCollider()
		{
			typed_entries = new List<List<CollisionEntry>>();
			typed_entries.Add(new List<CollisionEntry>()); // Player
			typed_entries.Add(new List<CollisionEntry>()); // Enemy
		}
		
		public void Add(CollisionEntityType type, GameEntity owner, Node collider, GetCenterDelegate center, GetRadiusDelegate radius)
		{	
			CollisionEntry entry = new CollisionEntry() { type = type, owner = owner, collider = collider, center = center, radius = radius };
			List<CollisionEntry> entries = typed_entries[(int)type];
			entries.Add(entry);
		}
		
		public void Add(CollisionEntry entry)
		{
			List<CollisionEntry> entries = typed_entries[(int)entry.type];
			entries.Add(entry);
		}
		
		public void Collide()
		{
			// for each list
			//   check for each other list
			foreach (List<CollisionEntry> entries in typed_entries)
			{
				foreach (List<CollisionEntry> other_entries in typed_entries)
				{
					if (other_entries == entries)
						continue;
					
					for (int i = 0; i < entries.Count; ++i)
					{
						GameEntity collider_owner = entries[i].owner;
						Node collider_collider = entries[i].collider;
						Vector2 collider_center = entries[i].center();
						float collider_radius = entries[i].radius();
						
						for (int j = 0; j < other_entries.Count; ++j)
						{
							GameEntity collidee_owner = other_entries[j].owner;
							Node collidee_collider = other_entries[j].collider;
							if (collider_owner == collidee_owner)
								continue;
							
							Vector2 collidee_center = other_entries[j].center();
							float collidee_radius = other_entries[j].radius();
							
							float r = collider_radius + collidee_radius;
							
							Vector2 offset = collidee_center - collider_center;
							float lensqr = offset.LengthSquared();	
							
							if (lensqr < r * r)
							{
								collider_owner.CollideTo(collidee_owner, collidee_collider);
								collidee_owner.CollideFrom(collider_owner, collider_collider);
							}
						}
					}
				}
			}
			
			Clear();
		}
		
		public void Clear()
		{
			foreach (List<CollisionEntry> entries in typed_entries)
				entries.Clear();
		}
	}
}

