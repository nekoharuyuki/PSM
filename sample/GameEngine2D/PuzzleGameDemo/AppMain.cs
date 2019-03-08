/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PuzzleGameDemo
{
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

	public class AppMain
	{
		public static void Main(string[] args)
		{
			Initialize();

			while (true) {
				SystemEvents.CheckEvents();
				Update();
				Render();
			}
		}

		public static void Initialize()
		{
			Director.Initialize();
			Game.Instance = new Game();
			Game.Instance.Initialize();
			
			Vector2 ideal_screen_size = new Vector2(960.0f, 544.0f);
			Camera2D camera = Game.Instance.GameScene.Camera as Camera2D;
			camera.SetViewFromHeightAndCenter(ideal_screen_size.Y, ideal_screen_size / 2.0f);
		}

		public static void Update()
		{
			// Query gamepad for current state
			var gamePadData = GamePad.GetData(0);

			Director.Instance.Update();
		}

		public static void Render()
		{
			Director.Instance.Render();

			// Present the screen
			Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.Context.SwapBuffers();
			Director.Instance.PostSwap();
		}
	}
}
