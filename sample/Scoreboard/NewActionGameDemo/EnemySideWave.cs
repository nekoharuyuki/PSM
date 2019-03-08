
using Sce.PlayStation.Core;

namespace SirAwesome
{
	public class EnemySideWave
		: GameEntity
	{
		float Difficulty;
		int Type;
		EnemySpawner Spawner;
		
		public EnemySideWave(int type, float difficulty)
		{
			float id = 1.0f - difficulty;
			Type = type;
			Difficulty = difficulty;
			Spawner = new EnemySpawner() {
				Position = new Vector2(Game.Instance.TitleCameraCenter.X + (Game.Instance.Random.NextBool() ? -700.0f : 700.0f), Game.Instance.FloorHeight),
				Type = type,
				SpawnRate = id * Game.Instance.Random.NextFloat() * 10.0f + id * (float)(Type + 1),
				SpawnCounter = Game.Instance.Random.NextFloat() * 5.0f,
				Total = (int)(Game.Instance.Random.NextFloat() * difficulty * 4.0f + 2.0f),
			};
			
			this.AddChild(Spawner);
		}
		
		public override void Tick(float dt)
		{
			base.Tick(dt);
			
			if (Spawner.Total == 0)
				Die(null, 0.0f);
		}
	}
}

