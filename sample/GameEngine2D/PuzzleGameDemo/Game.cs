/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PuzzleGameDemo
{
	public class Game
	{
		public static Game Instance;
		
		public Random Random = new Random();
		public NoCleanupScene TitleScene;
		public NoCleanupScene GameScene;

		public Board Board;
		public SpriteTile Title;

		public Game()
		{
		}

		public void Initialize()
		{
			TitleScene = new NoCleanupScene();
			Title = Support.TiledSpriteFromFile("/Application/assets/veggie_royale_title.png", 1, 1);
			TitleScene.AddChild(Title);

			Vector2 ideal_screen_size = new Vector2(960.0f, 544.0f);
			Camera2D title_camera = TitleScene.Camera as Camera2D;
			title_camera.SetViewFromHeightAndCenter(ideal_screen_size.Y, ideal_screen_size / 2.0f);

			GameScene = new NoCleanupScene();
			Board = new Board(8, 8);
			GameScene.AddChild(Board);

			Director.Instance.RunWithScene(new Scene(),true);

			// force tick so the scene is set
			Director.Instance.Update();
			
			StartTitle();
		}

		public void StartTitle()
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Unschedule(GameScene, TickGame);
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(TitleScene, TickTitle, 0.0f, false);

			var transition = new TransitionSolidFade(TitleScene) { PreviousScene = Director.Instance.CurrentScene, Duration = 1.5f, Tween = Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Linear	 };
			Director.Instance.ReplaceScene(transition);
		}

		public void TickTitle(float dt)
		{
			// wait for transition
			if (Director.Instance.CurrentScene != TitleScene)
			{
				return;
			}

			Support.MusicSystem.Instance.PlayNoClobber("title.mp3");

			Input2.TouchData touch = Input2.Touch00;
			if (touch.Down)
			{
				StartGame();
			}
		}

		public void StartGame()
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Unschedule(TitleScene, TickTitle);
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(GameScene, TickGame, 0.0f, false);

			var transition = new TransitionSolidFade(GameScene) { PreviousScene = Director.Instance.CurrentScene, Duration = 1.5f, Tween = Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Linear };
			Director.Instance.ReplaceScene(transition);

			Board.Refresh(0);

			Support.SoundSystem.Instance.Play("startgame.wav");
		}

		public void TickGame(float dt)
		{
			Support.MusicSystem.Instance.PlayNoClobber("music_ingame.mp3");

			Board.Tick(dt);

		}
	}

}

