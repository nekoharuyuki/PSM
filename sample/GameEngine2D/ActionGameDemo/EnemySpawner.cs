
namespace SirAwesome
{
	public class EnemySpawner
        : GameEntity
    {
    	public float SpawnCounter;
        public float SpawnRate;
        public int Type;
        public int Total;

        public override void Tick(float dt)
        {
        	base.Tick(dt);

            SpawnCounter += dt;

            if (SpawnCounter > SpawnRate)
            {
                SpawnCounter -= SpawnRate;
                SpawnEnemy();
            }
        }

        public void SpawnEnemy()
        {
			// don't spawn any more if player is dead
			if (Game.Instance.PlayerDead)
				return;

        	// -1 is infinite spawning
        	if (Total == 0)
				return;
			
			// DEBUG
			//Type = 2;
			//return;

        	switch (Type)
			{
			case 0: Game.Instance.AddQueue.Add(new EnemySlime() { Position = this.Position, }); break;
			case 1: Game.Instance.AddQueue.Add(new EnemyRedSlime() { Position = this.Position, }); break;
			case 2: Game.Instance.AddQueue.Add(new EnemyZombie() { Position = this.Position, }); break;
			case 3: Game.Instance.AddQueue.Add(new EnemyBat() { Position = this.Position, }); break;
			}
			
			Total -= 1;
        }
    }
}
