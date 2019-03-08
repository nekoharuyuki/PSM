
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace SirAwesome
{
	public class Layer
    : Node
    {
    }

	public static class RandomExtensions
    {
        public static bool NextBool(this Random self)
        {
            return (self.Next() & 1) == 0;
        }

        public static float NextFloat(this Random self)
        {
            return (float)self.NextDouble();
        }

        public static float NextSignedFloat(this Random self)
        {
            return (float)self.NextDouble() * (float)self.NextSign();
        }

        public static float NextAngle(this Random self)
        {
            return self.NextFloat() * FMath.PI * 2.0f;
        }

        public static float NextSign(this Random self)
        {
            return self.NextDouble() < 0.5 ? -1.0f : 1.0f;
        }

        public static Vector2 NextVector2(this Random self)
        {
            return Vector2.UnitX.Rotate(self.NextFloat() * FMath.PI * 2.0f);
        }

        public static Vector2 NextVector2(this Random self, float magnitude)
        {
            return Vector2.UnitX.Rotate(self.NextFloat() * FMath.PI * 2.0f) * magnitude;
        }

        public static Vector2 NextVector2Variable(this Random self)
        {
            return new Vector2(self.NextFloat(), self.NextFloat());
        }
    }

	static public class EntryPoint
    {
        public static void Run(string[] args)
        {
            Sce.PlayStation.HighLevel.GameEngine2D.Director.Initialize( 1024*4 );

			Game.Instance = new Game();
            var game = Game.Instance;
            
            Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.RunWithScene(game.Scene,true);
            
			Coin.InitializeCache();

			System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            while (true)
            {
            	timer.Start();
                SystemEvents.CheckEvents();

                //Sce.PlayStation.HighLevel.GameEngine2D.Camera.DrawDefaultGrid(32.0f);

                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.SetBlendMode(BlendMode.Normal);
                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Update();
                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Render();
				
                game.FrameUpdate();
                
            	timer.Stop();
                long ms = timer.ElapsedMilliseconds;
                //Console.WriteLine("ms: {0}", (int)ms);
            	timer.Reset();

                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.Context.SwapBuffers();
                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.PostSwap();
            }
        }
    }

	public class GameScene
        : Sce.PlayStation.HighLevel.GameEngine2D.Scene
    {
        //public override void OnCreate()
        //{
            // create player
            // create enemies
        //}
    }
}

static class Program
{
	static void Main( string[] args )
	{
		SirAwesome.EntryPoint.Run(args);
	}
}
