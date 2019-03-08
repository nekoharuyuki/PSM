
using System.Collections.Generic;

using Sce.PlayStation.Core;

namespace SirAwesome
{
	public class EnemyWave
		: GameEntity
	{
		public List<EnemySpawner> Spawners { get; set; }
			
		public EnemyWave(float difficulty)
		{
			float inverse_difficulty = 1.0f - difficulty;
			Spawners = new List<EnemySpawner>();
			
			int count = 5 + Game.Instance.Random.Next() % 4;
			for (int i = 0; i < 8; ++i)
			{
				int type = Game.Instance.Random.Next() % 4;
				float spawn_base = inverse_difficulty * Game.Instance.Random.NextFloat() * 5.0f;
				
				switch (type)
				{
				case 0: spawn_base *= 0.3f; break;
				case 1: spawn_base *= 0.5f; break;
				case 2: spawn_base *= 0.7f; break;
				case 3: spawn_base *= 0.8f; break;
				}
				
				float spawn_rate = spawn_base + inverse_difficulty * 5.0f;
				
				int total = (int)(inverse_difficulty * 3.0f + Game.Instance.Random.NextFloat() * 4.0f);
				
				var spawner = new EnemySpawner() {
					SpawnCounter = Game.Instance.Random.NextFloat() * -i + 2.5f * -i,
					SpawnRate = spawn_rate,
					Type = type,
					Total = total,
					Position = new Vector2(-300.0f + Game.Instance.Random.NextFloat() * 1500.0f, 600.0f),
				};
				
				this.AddChild(spawner);
			}
		}
		
		public bool IsDone()
		{
			foreach (EnemySpawner spawner in Spawners)
			{
				if (spawner.Total != 0)
					return false;
			}
			
			return true;
		}
	};
}

